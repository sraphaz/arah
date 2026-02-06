import 'dart:convert';

import 'package:dio/dio.dart';
import 'package:flutter/foundation.dart';

import '../config/app_config.dart';
import 'api_exception.dart';

/// Nomes dos headers de geolocalização (alinhados ao backend [ApiHeaders]).
const String headerGeoLatitude = 'X-Geo-Latitude';
const String headerGeoLongitude = 'X-Geo-Longitude';

/// Cliente HTTP para jornadas BFF (Dio). Injetado com token, sessionId e geo.
/// Interceptors: retry em 429, callback em 401, timeout e erros centralizados.
class BffClient {
  BffClient({
    required this.config,
    this.accessToken,
    this.sessionId,
    this.latitude,
    this.longitude,
    this.onUnauthorized,
  }) : _dio = _createDio(config.bffBaseUrl.replaceAll(RegExp(r'/$'), '') + '/api/v2/journeys/', onUnauthorized);

  final AppConfig config;
  final String? accessToken;
  final String? sessionId;
  final double? latitude;
  final double? longitude;
  final void Function()? onUnauthorized;

  final Dio _dio;
  static const _timeout = Duration(seconds: 30);

  static Dio _createDio(String baseUrl, void Function()? onUnauthorized) {
    final dio = Dio(BaseOptions(
      baseUrl: baseUrl,
      connectTimeout: _timeout,
      receiveTimeout: _timeout,
      sendTimeout: _timeout,
      headers: {
        'Accept': 'application/json',
        'Content-Type': 'application/json',
      },
    ));
    if (kDebugMode) {
      dio.interceptors.add(LogInterceptor(
        request: true,
        requestHeader: true,
        requestBody: true,
        responseHeader: true,
        responseBody: true,
        error: true,
        logPrint: (obj) => debugPrint(obj.toString()),
      ));
    }
    dio.interceptors.add(InterceptorsWrapper(
      onError: (error, handler) async {
        if (error.response?.statusCode == 401) {
          onUnauthorized?.call();
        }
        if (error.response?.statusCode == 429) {
          // Bound retries to avoid endless loops when backend keeps returning 429.
          const maxRetries = 2;
          const retryCountKey = '_429_retries';
          final retries = (error.requestOptions.extra[retryCountKey] as int?) ?? 0;
          if (retries >= maxRetries) {
            return handler.next(error);
          }
          error.requestOptions.extra[retryCountKey] = retries + 1;
          final retryAfter = error.response?.headers.value('Retry-After');
          final seconds = retryAfter != null ? int.tryParse(retryAfter) ?? 2 : 2;
          await Future<void>.delayed(Duration(seconds: seconds.clamp(1, 10)));
          try {
            final response = await dio.fetch(error.requestOptions);
            return handler.resolve(response);
          } catch (e) {
            return handler.next(error);
          }
        }
        return handler.next(error);
      },
    ));
    return dio;
  }

  Map<String, String> _headersFor({String? sessionIdOverride}) {
    final map = <String, String>{
      'Accept': 'application/json',
      'Content-Type': 'application/json',
    };
    if (accessToken != null && accessToken!.isNotEmpty) {
      map['Authorization'] = 'Bearer $accessToken';
    }
    final sid = sessionIdOverride ?? sessionId;
    if (sid != null && sid.isNotEmpty) {
      map['X-Session-Id'] = sid;
    }
    if (latitude != null && longitude != null) {
      map[headerGeoLatitude] = latitude!.toString();
      map[headerGeoLongitude] = longitude!.toString();
    }
    return map;
  }

  /// GET jornada (ex: feed/territory-feed, me/profile).
  Future<BffResponse> get(
    String journey,
    String pathAndQuery, {
    String? sessionIdOverride,
  }) async {
    final path = '$journey/$pathAndQuery';
    try {
      final response = await _dio.get<dynamic>(
        path,
        options: Options(headers: _headersFor(sessionIdOverride: sessionIdOverride)),
      );
      return _fromResponse(response);
    } on DioException catch (e, stackTrace) {
      throw _toApiException(e, stackTrace);
    }
  }

  /// POST jornada (ex: auth/login, onboarding/complete).
  Future<BffResponse> post(
    String journey,
    String pathAndQuery, {
    Map<String, dynamic>? body,
    String? sessionIdOverride,
  }) async {
    final path = '$journey/$pathAndQuery';
    try {
      final response = await _dio.post<dynamic>(
        path,
        data: body,
        options: Options(headers: _headersFor(sessionIdOverride: sessionIdOverride)),
      );
      return _fromResponse(response);
    } on DioException catch (e, stackTrace) {
      throw _toApiException(e, stackTrace);
    }
  }

  /// PUT jornada (ex: me/profile/display-name).
  Future<BffResponse> put(
    String journey,
    String pathAndQuery, {
    Map<String, dynamic>? body,
    String? sessionIdOverride,
  }) async {
    final path = '$journey/$pathAndQuery';
    try {
      final response = await _dio.put<dynamic>(
        path,
        data: body,
        options: Options(headers: _headersFor(sessionIdOverride: sessionIdOverride)),
      );
      return _fromResponse(response);
    } on DioException catch (e, stackTrace) {
      throw _toApiException(e, stackTrace);
    }
  }

  /// DELETE jornada (ex: me/interests/{tag}).
  Future<BffResponse> delete(
    String journey,
    String pathAndQuery, {
    String? sessionIdOverride,
  }) async {
    final path = '$journey/$pathAndQuery';
    try {
      final response = await _dio.delete<dynamic>(
        path,
        options: Options(headers: _headersFor(sessionIdOverride: sessionIdOverride)),
      );
      return _fromResponse(response);
    } on DioException catch (e, stackTrace) {
      throw _toApiException(e, stackTrace);
    }
  }

  static BffResponse _fromResponse(Response<dynamic> response) {
    return BffResponse(
      statusCode: response.statusCode ?? 0,
      data: response.data,
    );
  }

  static ApiException _toApiException(DioException e, [StackTrace? stackTrace]) {
    final statusCode = e.response?.statusCode;
    final rawData = e.response?.data;
    String? body;
    String? serverError;
    if (rawData is String) {
      body = rawData;
      try {
        final decoded = jsonDecode(rawData) as Map<String, dynamic>?;
        serverError = decoded?['error'] as String?;
      } catch (_) {}
    } else if (rawData is Map<String, dynamic>) {
      body = jsonEncode(rawData);
      serverError = rawData['error'] as String?;
    } else if (rawData != null) {
      body = jsonEncode(rawData);
    }
    final message = e.type == DioExceptionType.connectionTimeout ||
            e.type == DioExceptionType.receiveTimeout ||
            e.type == DioExceptionType.sendTimeout
        ? 'Timeout ao conectar'
        : (e.message ?? 'Erro de rede');
    final exception = ApiException(
      message,
      statusCode: statusCode,
      body: body != null && body.length > 500 ? null : body,
      originalError: e,
      serverError: serverError,
    );
    // Log em debug: detalhes para diagnóstico (tipo Dio, URL tentada, erro subjacente).
    if (kDebugMode) {
      final dioType = e.type.name;
      final url = e.requestOptions.uri.toString();
      final underlying = e.error?.toString() ?? '';
      if (statusCode == null) {
        debugPrint('[ApiException] type=$dioType url=$url message=$message');
        if (underlying.isNotEmpty) debugPrint('[ApiException] error=$underlying');
        debugPrint('[ApiException] (BFF inacessível? Backend rodando? CORS para web?)');
      } else {
        debugPrint(
          '[ApiException] statusCode=$statusCode type=$dioType url=$url message=$message serverError=$serverError body=${exception.body ?? "(truncated or null)"}',
        );
        if (underlying.isNotEmpty) debugPrint('[ApiException] error=$underlying');
        if (stackTrace != null) debugPrint('[ApiException] stackTrace:\n$stackTrace');
      }
    }
    return exception;
  }
}

class BffResponse {
  BffResponse({required this.statusCode, this.data});
  final int statusCode;
  final dynamic data;
}
