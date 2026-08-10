import '../models/map_pin.dart';
import '../../../../core/network/bff_client.dart';

/// Valores hídricos (legado em Type ou ponte WA-E1 em Subtype) — CSV para assetTypes.
const String kWaterBodySubtypesCsv =
    'river,stream,spring,waterfall,well,potable_water';

/// Repositório da jornada BFF map (pins, entidades). Consome GET map/pins.
class MapRepository {
  MapRepository({required BffClient client}) : _client = client;

  final BffClient _client;

  /// GET map/pins?territoryId=...&types=...&assetTypes=...&assetSubtypes=...
  Future<List<MapPin>> getPins({
    required String territoryId,
    String? types,
    String? assetTypes,
    String? assetSubtypes,
  }) async {
    var path = 'pins?territoryId=$territoryId';
    if (types != null && types.isNotEmpty) {
      path += '&types=$types';
    }
    if (assetTypes != null && assetTypes.isNotEmpty) {
      path += '&assetTypes=$assetTypes';
    }
    if (assetSubtypes != null && assetSubtypes.isNotEmpty) {
      path += '&assetSubtypes=$assetSubtypes';
    }
    final response = await _client.get('map', path);
    final list = response.data is List ? response.data as List : null;
    if (list == null) return [];
    return list
        .map((e) => MapPin.fromJson(e as Map<String, dynamic>))
        .toList();
  }
}
