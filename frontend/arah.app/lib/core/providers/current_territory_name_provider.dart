import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'territory_provider.dart';
import '../../features/territories/presentation/providers/territories_list_provider.dart';

/// Território atualmente selecionado (a partir do id + lista de territórios).
/// Retorna null se não houver id, lista ainda não carregada ou id não encontrado na lista.
final currentTerritoryItemProvider = Provider<TerritoryItem?>((ref) {
  final territoryId = ref.watch(selectedTerritoryIdValueProvider);
  final territoriesAsync = ref.watch(territoriesListProvider);

  if (territoryId == null || territoryId.isEmpty) return null;
  final list = territoriesAsync.valueOrNull;
  if (list == null) return null;
  for (final t in list) {
    if (t.id == territoryId) return t;
  }
  return null;
});

/// Nome do território atualmente selecionado.
final currentTerritoryNameProvider = Provider<String?>((ref) {
  return ref.watch(currentTerritoryItemProvider)?.name;
});
