import 'package:dio/dio.dart';

import '../../../../core/network/api_exception.dart';
import '../../../../core/network/bff_client.dart';

/// Upload de mídia via BFF `media/upload`.
class MediaRepository {
  MediaRepository({required BffClient client}) : _client = client;

  final BffClient _client;

  Future<String> uploadImage({
    required String filePath,
    required String fileName,
    String mimeType = 'image/jpeg',
  }) async {
    final formData = FormData.fromMap({
      'file': await MultipartFile.fromFile(filePath, filename: fileName),
    });

    final response = await _client.postMultipart('media', 'upload', formData: formData);
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
}
