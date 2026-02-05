import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/providers/app_providers.dart';
import '../../data/models/map_pin.dart';
import '../../data/repositories/map_repository.dart';

final mapRepositoryProvider = Provider<MapRepository>((ref) {
  return MapRepository(client: ref.watch(bffClientProvider));
});

/// Pins do mapa para o território. BFF map/pins.
final mapPinsProvider =
    FutureProvider.autoDispose.family<List<MapPin>, String?>((ref, territoryId) async {
  if (territoryId == null || territoryId.isEmpty) return [];
  final repo = ref.watch(mapRepositoryProvider);
  return repo.getPins(territoryId: territoryId);
});
