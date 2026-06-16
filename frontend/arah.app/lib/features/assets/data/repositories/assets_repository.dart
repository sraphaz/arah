import '../../../../core/network/api_exception.dart';
import '../../../../core/network/bff_client.dart';
import '../models/asset_item.dart';

class AssetsRepository {
  AssetsRepository({required BffClient client}) : _client = client;

  final BffClient _client;

  Future<List<AssetItem>> listAssets({required String territoryId}) async {
    final response = await _client.get(
      'assets',
      '',
      queryParameters: {'territoryId': territoryId},
    );
    if (response.statusCode < 200 || response.statusCode >= 300) {
      throw ApiException('HTTP ${response.statusCode}', statusCode: response.statusCode);
    }
    final data = response.data;
    if (data is! List) return const [];
    return data.whereType<Map<String, dynamic>>().map(AssetItem.fromJson).toList();
  }

  Future<AssetItem> createAsset({
    required String territoryId,
    required String type,
    required String name,
    String? description,
    required double latitude,
    required double longitude,
  }) async {
    final response = await _client.post(
      'assets',
      '',
      body: {
        'territoryId': territoryId,
        'type': type,
        'name': name,
        'description': description,
        'geoAnchors': [
          {'latitude': latitude, 'longitude': longitude},
        ],
      },
    );
    if (response.statusCode < 200 || response.statusCode >= 300) {
      throw ApiException('HTTP ${response.statusCode}', statusCode: response.statusCode);
    }
    return AssetItem.fromJson(response.data as Map<String, dynamic>);
  }
}
