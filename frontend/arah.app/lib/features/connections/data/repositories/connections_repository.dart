import '../../../../core/network/bff_client.dart';
import '../models/connection_item.dart';
import '../models/connection_user.dart';

class ConnectionsRepository {
  ConnectionsRepository({required BffClient client}) : _client = client;

  final BffClient _client;

  Future<List<ConnectionItem>> listConnections({
    String? territoryId,
    String? status,
  }) async {
    final query = <String, dynamic>{};
    if (status != null && status.isNotEmpty) query['status'] = status;
    if (territoryId != null && territoryId.isNotEmpty) query['territoryId'] = territoryId;

    final response = await _client.get('connections', '', queryParameters: query.isEmpty ? null : query);
    final data = response.data;
    if (data is! List) return const [];
    return data
        .whereType<Map<String, dynamic>>()
        .map(ConnectionItem.fromJson)
        .toList();
  }

  Future<List<ConnectionItem>> listPending() async {
    final response = await _client.get('connections', 'pending');
    final data = response.data;
    if (data is! List) return const [];
    return data
        .whereType<Map<String, dynamic>>()
        .map(ConnectionItem.fromJson)
        .toList();
  }

  Future<void> accept(String connectionId) async {
    await _client.post('connections', '$connectionId/accept');
  }

  Future<void> reject(String connectionId) async {
    await _client.post('connections', '$connectionId/reject');
  }

  Future<void> remove(String connectionId) async {
    await _client.delete('connections', connectionId);
  }

  Future<List<ConnectionUser>> searchUsers({
    required String query,
    String? territoryId,
    int limit = 20,
  }) async {
    final params = <String, dynamic>{
      'query': query,
      'limit': limit,
    };
    if (territoryId != null && territoryId.isNotEmpty) {
      params['territoryId'] = territoryId;
    }

    final response = await _client.get('connections', 'users/search', queryParameters: params);
    final data = response.data;
    if (data is! List) return const [];
    return data
        .whereType<Map<String, dynamic>>()
        .map(ConnectionUser.fromJson)
        .toList();
  }

  Future<List<ConnectionUser>> getSuggestions({
    String? territoryId,
    int limit = 10,
  }) async {
    final params = <String, dynamic>{'limit': limit};
    if (territoryId != null && territoryId.isNotEmpty) {
      params['territoryId'] = territoryId;
    }

    final response = await _client.get('connections', 'suggestions', queryParameters: params);
    final data = response.data;
    if (data is! List) return const [];
    return data
        .whereType<Map<String, dynamic>>()
        .map(ConnectionUser.fromJson)
        .toList();
  }

  Future<void> sendRequest({
    required String targetUserId,
    String? territoryId,
  }) async {
    final query = territoryId != null && territoryId.isNotEmpty
        ? 'request?territoryId=$territoryId'
        : 'request';
    await _client.post(
      'connections',
      query,
      body: {'targetUserId': targetUserId},
    );
  }
}
