import 'package:dio/dio.dart';
import 'package:http_parser/http_parser.dart';

import '../../../../core/network/api_exception.dart';
import '../../../../core/network/bff_client.dart';

/// Upload de mídia via BFF `media/upload`.
class MediaRepository {
  MediaRepository({required BffClient client}) : _client = client;

  final BffClient _client;

  /// Envia imagem por caminho local e/ou bytes (bytes preferidos no Web).
  Future<String> uploadImage({
    required String fileName,
    String mimeType = 'image/jpeg',
    String? filePath,
    List<int>? bytes,
  }) async {
    if ((filePath == null || filePath.isEmpty) && bytes == null) {
      throw ArgumentError('filePath or bytes is required');
    }

    final contentType = _parseMediaType(mimeType);
    final MultipartFile file = bytes != null
        ? MultipartFile.fromBytes(
            bytes,
            filename: fileName,
            contentType: contentType,
          )
        : await MultipartFile.fromFile(
            filePath!,
            filename: fileName,
            contentType: contentType,
          );

    final formData = FormData.fromMap({'file': file});

    final response =
        await _client.postMultipart('media', 'upload', formData: formData);
    if (response.statusCode < 200 || response.statusCode >= 300) {
      throw ApiException(
        'HTTP ${response.statusCode}',
        statusCode: response.statusCode,
        body: response.data?.toString(),
      );
    }

    final data = response.data as Map<String, dynamic>?;
    final id = data?['id']?.toString();
    if (id == null || id.isEmpty) {
      throw ApiException('Resposta de upload inválida');
    }
    return id;
  }

  MediaType _parseMediaType(String mimeType) {
    final trimmed = mimeType.trim();
    if (trimmed.isEmpty) {
      return MediaType('image', 'jpeg');
    }
    try {
      return MediaType.parse(trimmed);
    } catch (_) {
      return MediaType('image', 'jpeg');
    }
  }
}
