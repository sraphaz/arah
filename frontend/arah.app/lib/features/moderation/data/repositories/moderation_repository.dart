import '../../../../core/network/api_exception.dart';
import '../../../../core/network/bff_client.dart';
import '../models/work_item.dart';

class ModerationRepository {
  ModerationRepository({required BffClient client}) : _client = client;

  final BffClient _client;

  Future<List<WorkItem>> listWorkItems(String territoryId) async {
    final response = await _client.get('moderation', '$territoryId/work-items');
    if (response.statusCode < 200 || response.statusCode >= 300) {
      throw ApiException('HTTP ${response.statusCode}', statusCode: response.statusCode);
    }
    final data = response.data;
    if (data is! List) return const [];
    return data.whereType<Map<String, dynamic>>().map(WorkItem.fromJson).toList();
  }
}
