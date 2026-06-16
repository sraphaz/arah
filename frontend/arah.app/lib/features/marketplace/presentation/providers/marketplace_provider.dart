import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/providers/app_providers.dart';
import '../../../../core/providers/territory_provider.dart';
import '../../data/models/marketplace_item.dart';
import '../../data/repositories/marketplace_repository.dart';

class MarketplaceState {
  const MarketplaceState({
    this.items = const [],
    this.cart,
    this.isLoading = false,
    this.error,
    this.query = '',
  });

  final List<MarketplaceSearchItem> items;
  final Map<String, dynamic>? cart;
  final bool isLoading;
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

    state = MarketplaceState(items: state.items, isLoading: true, query: query);
    try {
      final items = await _repo.search(territoryId: territoryId, query: query);
      final cart = await _repo.getCart(territoryId);
      state = MarketplaceState(items: items, cart: cart, query: query);
    } catch (e) {
      state = MarketplaceState(error: e, query: query);
    }
  }

  Future<void> addToCart(String itemId) async {
    final territoryId = _ref.read(selectedTerritoryIdValueProvider);
    if (territoryId == null || territoryId.isEmpty) return;
    await _repo.addToCart(territoryId: territoryId, itemId: itemId);
    final cart = await _repo.getCart(territoryId);
    state = MarketplaceState(items: state.items, cart: cart, query: state.query);
  }

  Future<Map<String, dynamic>> checkout({String? message}) async {
    final territoryId = _ref.read(selectedTerritoryIdValueProvider);
    if (territoryId == null || territoryId.isEmpty) return {};
    final result = await _repo.checkout(territoryId: territoryId, message: message);
    final cart = await _repo.getCart(territoryId);
    state = MarketplaceState(items: state.items, cart: cart, query: state.query);
    return result;
  }
}

final marketplaceProvider =
    StateNotifierProvider.autoDispose<MarketplaceNotifier, MarketplaceState>((ref) {
  return MarketplaceNotifier(ref);
});
