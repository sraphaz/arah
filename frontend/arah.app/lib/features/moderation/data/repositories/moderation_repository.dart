import '../../../../core/network/api_exception.dart';
import '../../../../core/network/bff_client.dart';
import '../models/work_item.dart';

class ModerationRepository {
  ModerationRepository({required BffClient client}) : _client = client;

  final BffClient _client;

  Future<List<WorkItem>> listWorkItems(
    String territoryId, {
    String? type,
    String? status,
  }) async {
    final query = <String, dynamic>{};
    if (type != null && type.isNotEmpty) query['type'] = type;
    if (status != null && status.isNotEmpty) query['status'] = status;

    final response = await _client.get(
      'moderation',
      '$territoryId/work-items',
      queryParameters: query.isEmpty ? null : query,
    );
    if (response.statusCode < 200 || response.statusCode >= 300) {
      throw ApiException('HTTP ${response.statusCode}', statusCode: response.statusCode);
    }
    final data = response.data;
    if (data is! List) return const [];
    return data.whereType<Map<String, dynamic>>().map(WorkItem.fromJson).toList();
  }

  Future<void> decideModerationCase({
    required String territoryId,
    required String workItemId,
    required String outcome,
    String? notes,
  }) async {
    await _decide(
      territoryId: territoryId,
      path: '$territoryId/moderation/cases/$workItemId/decide',
      outcome: outcome,
      notes: notes,
    );
  }

  Future<void> decideResidencyVerification({
    required String territoryId,
    required String workItemId,
    required String outcome,
    String? notes,
  }) async {
    await _decide(
      territoryId: territoryId,
      path: '$territoryId/verifications/residency/$workItemId/decide',
      outcome: outcome,
      notes: notes,
    );
  }

  Future<List<int>> downloadEvidence({
    required String territoryId,
    required String evidenceId,
  }) async {
    final bytes = await _client.getBytes('moderation', '$territoryId/evidences/$evidenceId/download');
    return bytes;
  }

  Future<void> _decide({
    required String territoryId,
    required String path,
    required String outcome,
    String? notes,
  }) async {
    final response = await _client.post(
      'moderation',
      path,
      body: {
        'outcome': outcome,
        if (notes != null && notes.isNotEmpty) 'notes': notes,
      },
    );
    if (response.statusCode < 200 || response.statusCode >= 300) {
      throw ApiException('HTTP ${response.statusCode}', statusCode: response.statusCode);
    }
  }
}
