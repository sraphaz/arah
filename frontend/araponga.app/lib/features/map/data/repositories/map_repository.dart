import '../models/map_pin.dart';
import '../../../../core/network/bff_client.dart';

/// Repositório da jornada BFF map (pins, entidades). Consome GET map/pins.
class MapRepository {
  MapRepository({required BffClient client}) : _client = client;

  final BffClient _client;

  /// GET map/pins?territoryId=...&types=... (types opcional: entity,post,event,asset,alert,media).
  Future<List<MapPin>> getPins({
    required String territoryId,
    String? types,
  }) async {
    var path = 'pins?territoryId=$territoryId';
    if (types != null && types.isNotEmpty) {
      path += '&types=$types';
    }
    final response = await _client.get('map', path);
    final list = response.data is List ? response.data as List : null;
    if (list == null) return [];
    return list
        .map((e) => MapPin.fromJson(e as Map<String, dynamic>))
        .toList();
  }
}
