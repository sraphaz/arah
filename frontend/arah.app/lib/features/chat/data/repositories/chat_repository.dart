import '../../../../core/network/api_exception.dart';
import '../../../../core/network/bff_client.dart';
import '../models/chat_models.dart';

class ChatRepository {
  ChatRepository({required BffClient client}) : _client = client;

  final BffClient _client;

  Future<List<ChatConversationSummary>> listChannels(String territoryId) async {
    final response = await _client.get('territories', '$territoryId/chat/channels');
    if (response.statusCode < 200 || response.statusCode >= 300) {
      throw ApiException('HTTP ${response.statusCode}', statusCode: response.statusCode);
    }
    final data = response.data as Map<String, dynamic>?;
    final channels = data?['channels'] as List? ?? [];
    return channels.whereType<Map<String, dynamic>>().map(ChatConversationSummary.fromJson).toList();
  }

  Future<List<ChatMessage>> getMessages(String conversationId, {int limit = 50}) async {
    final response = await _client.get(
      'chat',
      'conversations/$conversationId/messages',
      queryParameters: {'limit': limit},
    );
    if (response.statusCode < 200 || response.statusCode >= 300) {
      throw ApiException('HTTP ${response.statusCode}', statusCode: response.statusCode);
    }
    final data = response.data as Map<String, dynamic>?;
    final messages = data?['messages'] as List? ?? [];
    return messages.whereType<Map<String, dynamic>>().map(ChatMessage.fromJson).toList();
  }

  Future<ChatMessage> sendMessage({
    required String conversationId,
    required String text,
  }) async {
    final response = await _client.post(
      'chat',
      'conversations/$conversationId/messages',
      body: {'text': text},
    );
    if (response.statusCode < 200 || response.statusCode >= 300) {
      throw ApiException('HTTP ${response.statusCode}', statusCode: response.statusCode);
    }
    return ChatMessage.fromJson(response.data as Map<String, dynamic>);
  }
}
