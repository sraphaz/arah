import '../../../../core/network/api_exception.dart';
import '../../../../core/network/bff_client.dart';

/// Repositório da jornada de feed (territory-feed, create-post).
class FeedRepository {
  FeedRepository({required this.client});

  final BffClient client;

  /// POST feed/create-post?territoryId=... com body título, conteúdo, tipo, visibilidade.
  /// [type] ex: "General", "Alert". [visibility] ex: "Public", "ResidentsOnly".
  Future<Map<String, dynamic>> createPost({
    required String territoryId,
    required String title,
    required String content,
    String type = 'General',
    String visibility = 'Public',
    List<String>? tags,
  }) async {
    if (territoryId.isEmpty) throw ArgumentError('territoryId is required');
    if (title.trim().isEmpty) throw ArgumentError('title is required');
    if (content.trim().isEmpty) throw ArgumentError('content is required');

    final path = 'create-post?territoryId=$territoryId';
    final body = <String, dynamic>{
      'title': title.trim(),
      'content': content.trim(),
      'type': type,
      'visibility': visibility,
      'territoryId': territoryId,
      'tags': tags,
      'mapEntityId': null,
      'mediaIds': null,
    };

    final response = await client.post('feed', path, body: body);
    if (response.statusCode < 200 || response.statusCode >= 300) {
      throw ApiException(
        'HTTP ${response.statusCode}',
        statusCode: response.statusCode,
        body: response.data?.toString(),
      );
    }
    final data = response.data as Map<String, dynamic>?;
    if (data == null) throw ApiException('Resposta inválida');
    return data;
  }
}
