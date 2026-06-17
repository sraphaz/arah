import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/geo/geo_location_provider.dart';
import '../../../../core/providers/app_providers.dart';
import '../../../../core/providers/territory_provider.dart';
import '../../data/models/asset_item.dart';
import '../../data/models/asset_validation_result.dart';
import '../../data/repositories/assets_repository.dart';

class AssetsState {
  const AssetsState({this.items = const [], this.isLoading = false, this.error});

  final List<AssetItem> items;
  final bool isLoading;
  final Object? error;
}

class AssetsNotifier extends StateNotifier<AssetsState> {
  AssetsNotifier(this._ref) : super(const AssetsState()) {
    refresh();
  }

  final Ref _ref;

  AssetsRepository get _repo => AssetsRepository(client: _ref.read(bffClientProvider));

  Future<void> refresh() async {
    final territoryId = _ref.read(selectedTerritoryIdValueProvider);
    if (territoryId == null || territoryId.isEmpty) {
      state = const AssetsState();
      return;
    }
    state = AssetsState(items: state.items, isLoading: true);
    try {
      final items = await _repo.listAssets(territoryId: territoryId);
      state = AssetsState(items: items);
    } catch (e) {
      state = AssetsState(error: e);
    }
  }

  Future<void> createAsset({required String name, required String type}) async {
    final territoryId = _ref.read(selectedTerritoryIdValueProvider);
    final geo = _ref.read(geoLocationStateProvider);
    if (territoryId == null || territoryId.isEmpty || geo == null) return;
    await _repo.createAsset(
      territoryId: territoryId,
      type: type,
      name: name,
      latitude: geo.latitude,
      longitude: geo.longitude,
    );
    await refresh();
  }

  Future<AssetValidationResult> validateAsset(String assetId) async {
    final territoryId = _requireTerritoryId();
    final result = await _repo.validateAsset(territoryId: territoryId, assetId: assetId);
    await refresh();
    return result;
  }

  Future<void> archiveAsset(String assetId, {String? reason}) async {
    final territoryId = _requireTerritoryId();
    await _repo.archiveAsset(territoryId: territoryId, assetId: assetId, reason: reason);
    await refresh();
  }

  Future<void> curateAsset(String assetId, {required String outcome, String? notes}) async {
    final territoryId = _requireTerritoryId();
    await _repo.curateAsset(
      territoryId: territoryId,
      assetId: assetId,
      outcome: outcome,
      notes: notes,
    );
    await refresh();
  }

  String _requireTerritoryId() {
    final territoryId = _ref.read(selectedTerritoryIdValueProvider);
    if (territoryId == null || territoryId.isEmpty) {
      throw StateError('Território não selecionado');
    }
    return territoryId;
  }
}

final assetsProvider = StateNotifierProvider.autoDispose<AssetsNotifier, AssetsState>((ref) {
  return AssetsNotifier(ref);
});
