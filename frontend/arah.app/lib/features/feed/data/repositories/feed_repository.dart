import '../../../../core/network/api_exception.dart';
import '../../../../core/network/bff_client.dart';
import '../../domain/feed_interaction.dart';
import '../../domain/feed_comment.dart';

/// Repositório da jornada de feed (territory-feed, create-post, interact).
class FeedRepository {
  FeedRepository({required this.client});

  final BffClient client;

  /// POST feed/create-post?territoryId=... com body título, conteúdo, tipo, visibilidade.
  Future<Map<String, dynamic>> createPost({
    required String territoryId,
    required String title,
    required String content,
    String type = 'General',
    String visibility = 'Public',
    List<String>? tags,
    List<String>? mediaIds,
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
      'mediaIds': mediaIds,
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

  /// POST feed/interact — like, comment ou share.
  Future<Map<String, dynamic>> interactPost({
    required String postId,
    required String territoryId,
    required FeedInteractionAction action,
    String? commentContent,
  }) async {
    if (postId.isEmpty) throw ArgumentError('postId is required');
    if (territoryId.isEmpty) throw ArgumentError('territoryId is required');

    final body = <String, dynamic>{
      'postId': postId,
      'territoryId': territoryId,
      'action': action.apiValue,
      if (commentContent != null && commentContent.trim().isNotEmpty)
        'commentContent': commentContent.trim(),
    };

    final response = await client.post('feed', 'interact', body: body);
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

  /// GET feed/post-comments — lista comentários paginados de um post.
  Future<FeedCommentsPage> getPostComments({
    required String postId,
    required String territoryId,
    int pageNumber = 1,
    int pageSize = 20,
  }) async {
    if (postId.isEmpty) throw ArgumentError('postId is required');
    if (territoryId.isEmpty) throw ArgumentError('territoryId is required');

    final path =
        'post-comments?postId=$postId&territoryId=$territoryId&pageNumber=$pageNumber&pageSize=$pageSize';
    final response = await client.get('feed', path);
    if (response.statusCode < 200 || response.statusCode >= 300) {
      throw ApiException(
        'HTTP ${response.statusCode}',
        statusCode: response.statusCode,
        body: response.data?.toString(),
      );
    }
    return FeedCommentsPage.fromJson(response.data as Map<String, dynamic>?);
  }

  /// GET feed/post-detail — detalhe de um único post (mesmo formato de item do feed).
  /// Usado por deep-links (ex.: pin de post no mapa). Retorna null se não encontrado.
  Future<Map<String, dynamic>?> getPost({
    required String postId,
    required String territoryId,
  }) async {
    if (postId.isEmpty || territoryId.isEmpty) return null;
    final path = 'post-detail?postId=$postId&territoryId=$territoryId';
    final response = await client.get('feed', path);
    if (response.statusCode == 404) return null;
    if (response.statusCode < 200 || response.statusCode >= 300) {
      throw ApiException(
        'HTTP ${response.statusCode}',
        statusCode: response.statusCode,
        body: response.data?.toString(),
      );
    }
    final data = response.data;
    return data is Map<String, dynamic> ? data : null;
  }

  /// DELETE feed/delete-post — exclui post do autor autenticado.
  Future<void> deletePost({
    required String postId,
    required String territoryId,
  }) async {
    if (postId.isEmpty) throw ArgumentError('postId is required');
    if (territoryId.isEmpty) throw ArgumentError('territoryId is required');

    final path = 'delete-post?postId=$postId&territoryId=$territoryId';
    final response = await client.delete('feed', path);
    if (response.statusCode != 204 && (response.statusCode < 200 || response.statusCode >= 300)) {
      throw ApiException(
        'HTTP ${response.statusCode}',
        statusCode: response.statusCode,
        body: response.data?.toString(),
      );
    }
  }
}
