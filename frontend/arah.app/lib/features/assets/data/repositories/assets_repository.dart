import '../../../../core/network/api_exception.dart';
import '../../../../core/network/bff_client.dart';
import '../models/asset_item.dart';
import '../models/asset_validation_result.dart';

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

  Future<AssetValidationResult> validateAsset({
    required String territoryId,
    required String assetId,
  }) async {
    final response = await _client.post(
      'assets',
      '$assetId/validate',
      queryParameters: {'territoryId': territoryId},
    );
    if (response.statusCode < 200 || response.statusCode >= 300) {
      throw ApiException('HTTP ${response.statusCode}', statusCode: response.statusCode);
    }
    return AssetValidationResult.fromJson(response.data as Map<String, dynamic>);
  }

  Future<AssetItem> archiveAsset({
    required String territoryId,
    required String assetId,
    String? reason,
  }) async {
    final response = await _client.post(
      'assets',
      '$assetId/archive',
      queryParameters: {'territoryId': territoryId},
      body: reason != null && reason.isNotEmpty ? {'reason': reason} : null,
    );
    if (response.statusCode < 200 || response.statusCode >= 300) {
      throw ApiException('HTTP ${response.statusCode}', statusCode: response.statusCode);
    }
    return AssetItem.fromJson(response.data as Map<String, dynamic>);
  }

  Future<AssetItem> curateAsset({
    required String territoryId,
    required String assetId,
    required String outcome,
    String? notes,
  }) async {
    final response = await _client.post(
      'assets',
      '$assetId/curate',
      queryParameters: {'territoryId': territoryId},
      body: {
        'outcome': outcome,
        if (notes != null && notes.isNotEmpty) 'notes': notes,
      },
    );
    if (response.statusCode < 200 || response.statusCode >= 300) {
      throw ApiException('HTTP ${response.statusCode}', statusCode: response.statusCode);
    }
    return AssetItem.fromJson(response.data as Map<String, dynamic>);
  }
}
