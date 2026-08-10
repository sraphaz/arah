import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/providers/app_providers.dart';
import '../../data/models/map_pin.dart';
import '../../data/repositories/map_repository.dart';

final mapRepositoryProvider = Provider<MapRepository>((ref) {
  return MapRepository(client: ref.watch(bffClientProvider));
});

/// Filtro de pins do mapa (WA-E2: corpos d'água via assetSubtypes no servidor).
enum MapPinsFilter {
  all,
  waterBodies,
}

final mapPinsFilterProvider = StateProvider<MapPinsFilter>((ref) => MapPinsFilter.all);

class MapPinsQuery {
  const MapPinsQuery({required this.territoryId, required this.filter});

  final String? territoryId;
  final MapPinsFilter filter;

  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      other is MapPinsQuery &&
          runtimeType == other.runtimeType &&
          territoryId == other.territoryId &&
          filter == other.filter;

  @override
  int get hashCode => Object.hash(territoryId, filter);
}

/// Pins do mapa para o território. BFF map/pins (filtro server-side).
final mapPinsProvider =
    FutureProvider.autoDispose.family<List<MapPin>, MapPinsQuery>((ref, query) async {
  final territoryId = query.territoryId;
  if (territoryId == null || territoryId.isEmpty) return [];
  final repo = ref.watch(mapRepositoryProvider);
  if (query.filter == MapPinsFilter.waterBodies) {
    return repo.getPins(
      territoryId: territoryId,
      types: 'asset',
      assetSubtypes: kWaterBodySubtypesCsv,
    );
  }
  return repo.getPins(territoryId: territoryId);
});
