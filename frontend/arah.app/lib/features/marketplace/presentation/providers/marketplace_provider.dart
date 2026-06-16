import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/providers/app_providers.dart';
import '../../../../core/providers/territory_provider.dart';
import '../../data/models/marketplace_item.dart';
import '../../data/models/store_item.dart';
import '../../data/repositories/marketplace_repository.dart';

class MarketplaceState {
  const MarketplaceState({
    this.items = const [],
    this.cart,
    this.myStore,
    this.isLoading = false,
    this.isStoreLoading = false,
    this.error,
    this.query = '',
  });

  final List<MarketplaceSearchItem> items;
  final Map<String, dynamic>? cart;
  final MyStore? myStore;
  final bool isLoading;
  final bool isStoreLoading;
  final Object? error;
  final String query;
}

class MarketplaceNotifier extends StateNotifier<MarketplaceState> {
  MarketplaceNotifier(this._ref) : super(const MarketplaceState());

  final Ref _ref;

  MarketplaceRepository get _repo => MarketplaceRepository(client: _ref.read(bffClientProvider));

  Future<void> search(String query) async {
    final territoryId = _ref.read(selectedTerritoryIdValueProvider);
    if (territoryId == null || territoryId.isEmpty) return;

    state = MarketplaceState(
      items: state.items,
      myStore: state.myStore,
      isLoading: true,
      query: query,
    );
    try {
      final items = await _repo.search(territoryId: territoryId, query: query);
      final cart = await _repo.getCart(territoryId);
      state = MarketplaceState(items: items, cart: cart, myStore: state.myStore, query: query);
    } catch (e) {
      state = MarketplaceState(error: e, myStore: state.myStore, query: query);
    }
  }

  Future<void> loadMyStore() async {
    final territoryId = _ref.read(selectedTerritoryIdValueProvider);
    if (territoryId == null || territoryId.isEmpty) return;

    state = MarketplaceState(
      items: state.items,
      cart: state.cart,
      myStore: state.myStore,
      isStoreLoading: true,
      query: state.query,
    );
    try {
      final store = await _repo.getMyStore(territoryId);
      state = MarketplaceState(
        items: state.items,
        cart: state.cart,
        myStore: store,
        query: state.query,
      );
    } catch (e) {
      state = MarketplaceState(
        error: e,
        items: state.items,
        cart: state.cart,
        myStore: state.myStore,
        query: state.query,
      );
    }
  }

  Future<void> saveMyStore({required String displayName, String? description}) async {
    final territoryId = _ref.read(selectedTerritoryIdValueProvider);
    if (territoryId == null || territoryId.isEmpty) return;

    final store = await _repo.upsertMyStore(
      territoryId: territoryId,
      displayName: displayName,
      description: description,
    );
    state = MarketplaceState(
      items: state.items,
      cart: state.cart,
      myStore: store,
      query: state.query,
    );
  }

  Future<void> addToCart(String itemId) async {
    final territoryId = _ref.read(selectedTerritoryIdValueProvider);
    if (territoryId == null || territoryId.isEmpty) return;
    await _repo.addToCart(territoryId: territoryId, itemId: itemId);
    final cart = await _repo.getCart(territoryId);
    state = MarketplaceState(
      items: state.items,
      cart: cart,
      myStore: state.myStore,
      query: state.query,
    );
  }

  Future<Map<String, dynamic>> checkout({String? message}) async {
    final territoryId = _ref.read(selectedTerritoryIdValueProvider);
    if (territoryId == null || territoryId.isEmpty) return {};
    final result = await _repo.checkout(territoryId: territoryId, message: message);
    final cart = await _repo.getCart(territoryId);
    state = MarketplaceState(
      items: state.items,
      cart: cart,
      myStore: state.myStore,
      query: state.query,
    );
    return result;
  }
}

final marketplaceProvider =
    StateNotifierProvider.autoDispose<MarketplaceNotifier, MarketplaceState>((ref) {
  return MarketplaceNotifier(ref);
});
