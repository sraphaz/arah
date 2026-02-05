import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../storage/territory_preferences.dart';

/// Território ativo (X-Session-Id). Persistido em SharedPreferences.
final selectedTerritoryIdProvider = StateNotifierProvider<SelectedTerritoryIdNotifier, AsyncValue<String?>>((ref) {
  return SelectedTerritoryIdNotifier();
});

class SelectedTerritoryIdNotifier extends StateNotifier<AsyncValue<String?>> {
  SelectedTerritoryIdNotifier() : super(const AsyncValue.loading()) {
    _load();
  }

  Future<void> _load() async {
    state = const AsyncValue.loading();
    try {
      final id = await loadSelectedTerritoryId();
      state = AsyncValue.data(id);
    } catch (e, st) {
      state = AsyncValue.error(e, st);
    }
  }

  Future<void> setTerritoryId(String? id) async {
    await saveSelectedTerritoryId(id);
    state = AsyncValue.data(id);
  }
}

/// Valor simples para injetar no BffClient (null enquanto carrega).
final selectedTerritoryIdValueProvider = Provider<String?>((ref) {
  final async = ref.watch(selectedTerritoryIdProvider);
  return async.valueOrNull;
});
