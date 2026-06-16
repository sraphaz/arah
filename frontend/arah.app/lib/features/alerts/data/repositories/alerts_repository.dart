import '../../../../core/network/api_exception.dart';
import '../../../../core/network/bff_client.dart';
import '../models/alert_item.dart';

class AlertsRepository {
  AlertsRepository({required BffClient client}) : _client = client;

  final BffClient _client;

  Future<List<AlertItem>> listAlerts({required String territoryId}) async {
    if (territoryId.isEmpty) throw ArgumentError('territoryId is required');

    final response = await _client.get(
      'alerts',
      '',
      queryParameters: {'territoryId': territoryId},
    );
    if (response.statusCode < 200 || response.statusCode >= 300) {
      throw ApiException(
        'HTTP ${response.statusCode}',
        statusCode: response.statusCode,
        body: response.data?.toString(),
      );
    }

    final data = response.data;
    if (data is! List) return const [];
    return data
        .whereType<Map<String, dynamic>>()
        .map(AlertItem.fromJson)
        .toList();
  }
}
