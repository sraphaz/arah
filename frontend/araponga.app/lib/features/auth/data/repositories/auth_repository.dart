import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/config/app_config.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../core/network/bff_client.dart';
import '../../../../core/storage/secure_storage_service.dart';
import '../models/auth_models.dart';

/// Repositório de autenticação: login (social/dev), refresh, logout.
/// API usa auth/social; para dev usamos provider "dev" com documento placeholder.
class AuthRepository {
  AuthRepository({
    required this.config,
    required this.secureStorage,
  });

  final AppConfig config;
  final SecureStorageService secureStorage;

  BffClient _client() => BffClient(config: config);

  /// Login: BFF auth/social. Em produção usar provedor real (Google/Apple).
  /// Para desenvolvimento: AuthProvider=dev, ExternalId=email, ForeignDocument=dev.
  Future<AuthSession> login({required String email, String? displayName}) async {
    final body = <String, dynamic>{
      'authProvider': 'dev',
      'externalId': email,
      'displayName': displayName ?? email,
      'foreignDocument': 'dev',
      'email': email,
    };
    final client = _client();
    final response = await client.post('auth', 'social', body: body);
    final data = response.data as Map<String, dynamic>?;
    if (data == null) throw ApiException('Resposta inválida');
    final user = AuthUser.fromJson(data);
    final token = data['token'] as String? ?? '';
    final refreshToken = data['refreshToken'] as String? ?? '';
    final expiresIn = data['expiresInSeconds'] as int? ?? 900;
    if (token.isEmpty) throw ApiException('Token não retornado');
    await secureStorage.writeAccessToken(token);
    await secureStorage.writeRefreshToken(refreshToken);
    await secureStorage.writeTokenExpiry(
      DateTime.now().add(Duration(seconds: expiresIn)).toIso8601String(),
    );
    return AuthSession(user: user, accessToken: token);
  }

  /// Refresh: BFF auth/refresh.
  Future<AuthSession?> refresh() async {
    final refreshToken = await secureStorage.readRefreshToken();
    if (refreshToken == null || refreshToken.isEmpty) return null;
    try {
      final client = _client();
      final response = await client.post('auth', 'refresh', body: {'refreshToken': refreshToken});
      final data = response.data as Map<String, dynamic>?;
      if (data == null) return null;
      final user = AuthUser.fromJson(data);
      final token = data['token'] as String? ?? '';
      final newRefresh = data['refreshToken'] as String? ?? '';
      final expiresIn = data['expiresInSeconds'] as int? ?? 900;
      await secureStorage.writeAccessToken(token);
      await secureStorage.writeRefreshToken(newRefresh);
      await secureStorage.writeTokenExpiry(
        DateTime.now().add(Duration(seconds: expiresIn)).toIso8601String(),
      );
      return AuthSession(user: user, accessToken: token);
    } catch (_) {
      return null;
    }
  }

  /// Restaura sessão a partir do storage (token ainda válido ou refresh).
  Future<AuthSession?> restoreSession() async {
    final token = await secureStorage.readAccessToken();
    if (token != null && token.isNotEmpty) {
      final expiry = await secureStorage.readTokenExpiry();
      final expiryDate = expiry != null ? DateTime.tryParse(expiry) : null;
      if (expiryDate != null && DateTime.now().isBefore(expiryDate.add(const Duration(minutes: 2)))) {
        return AuthSession(accessToken: token, user: null);
      }
      final refreshed = await refresh();
      if (refreshed != null) return refreshed;
    }
    await logout();
    return null;
  }

  Future<void> logout() async {
    await secureStorage.clearAuth();
  }
}

class AuthSession {
  const AuthSession({required this.accessToken, this.user});
  final String accessToken;
  final AuthUser? user;
}

final secureStorageProvider = Provider<SecureStorageService>((ref) => SecureStorageService());

final authRepositoryProvider = Provider<AuthRepository>((ref) {
  final config = ref.watch(appConfigProvider);
  final storage = ref.watch(secureStorageProvider);
  return AuthRepository(config: config, secureStorage: storage);
});
