import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/providers/app_providers.dart';
import '../../../../core/providers/territory_provider.dart';
import '../../data/models/marketplace_item.dart';
import '../../data/models/store_item.dart';
import '../../data/models/store_product.dart';
import '../../data/repositories/marketplace_repository.dart';

class MarketplaceState {
  const MarketplaceState({
    this.items = const [],
    this.cart,
    this.myStore,
    this.myProducts = const [],
    this.isLoading = false,
    this.isStoreLoading = false,
    this.isProductsLoading = false,
    this.error,
    this.query = '',
  });

  final List<MarketplaceSearchItem> items;
  final Map<String, dynamic>? cart;
  final MyStore? myStore;
  final List<StoreProduct> myProducts;
  final bool isLoading;
  final bool isStoreLoading;
  final bool isProductsLoading;
  final Object? error;
  final String query;

  MarketplaceState copyWith({
    List<MarketplaceSearchItem>? items,
    Map<String, dynamic>? cart,
    MyStore? myStore,
    List<StoreProduct>? myProducts,
    bool? isLoading,
    bool? isStoreLoading,
    bool? isProductsLoading,
    Object? error,
    String? query,
    bool clearError = false,
  }) {
    return MarketplaceState(
      items: items ?? this.items,
      cart: cart ?? this.cart,
      myStore: myStore ?? this.myStore,
      myProducts: myProducts ?? this.myProducts,
      isLoading: isLoading ?? this.isLoading,
      isStoreLoading: isStoreLoading ?? this.isStoreLoading,
      isProductsLoading: isProductsLoading ?? this.isProductsLoading,
      error: clearError ? null : (error ?? this.error),
      query: query ?? this.query,
    );
  }
}

class MarketplaceNotifier extends StateNotifier<MarketplaceState> {
  MarketplaceNotifier(this._ref) : super(const MarketplaceState());

  final Ref _ref;
  int _productsLoadGen = 0;
  int _storeLoadGen = 0;

  MarketplaceRepository get _repo =>
      MarketplaceRepository(client: _ref.read(bffClientProvider));

  Future<void> search(String query) async {
    final territoryId = _ref.read(selectedTerritoryIdValueProvider);
    if (territoryId == null || territoryId.isEmpty) return;

    state = state.copyWith(isLoading: true, query: query, clearError: true);
    try {
      final items = await _repo.search(territoryId: territoryId, query: query);
      final cart = await _repo.getCart(territoryId);
      state = state.copyWith(items: items, cart: cart, isLoading: false);
    } catch (e) {
      state = state.copyWith(error: e, isLoading: false);
    }
  }

  Future<void> loadMyStore() async {
    final territoryId = _ref.read(selectedTerritoryIdValueProvider);
    if (territoryId == null || territoryId.isEmpty) return;

    final gen = ++_storeLoadGen;
    state = state.copyWith(isStoreLoading: true, clearError: true);
    try {
      final store = await _repo.getMyStore(territoryId);
      if (gen != _storeLoadGen ||
          _ref.read(selectedTerritoryIdValueProvider) != territoryId) {
        return;
      }
      state = MarketplaceState(
        items: state.items,
        cart: state.cart,
        myStore: store,
        myProducts: store == null ? const [] : state.myProducts,
        isStoreLoading: false,
        query: state.query,
      );
      if (store != null) {
        await loadMyProducts();
      }
    } catch (e) {
      if (gen != _storeLoadGen ||
          _ref.read(selectedTerritoryIdValueProvider) != territoryId) {
        return;
      }
      state = state.copyWith(error: e, isStoreLoading: false);
    }
  }

  Future<void> loadMyProducts() async {
    final territoryId = _ref.read(selectedTerritoryIdValueProvider);
    final store = state.myStore;
    if (territoryId == null || territoryId.isEmpty || store == null) return;
    final storeId = store.id;
    final gen = ++_productsLoadGen;

    state = state.copyWith(isProductsLoading: true);
    try {
      final products = await _repo.listStoreProducts(
        territoryId: territoryId,
        storeId: storeId,
      );
      if (gen != _productsLoadGen ||
          _ref.read(selectedTerritoryIdValueProvider) != territoryId ||
          state.myStore?.id != storeId) {
        return;
      }
      state = state.copyWith(myProducts: products, isProductsLoading: false);
    } catch (e) {
      if (gen != _productsLoadGen ||
          _ref.read(selectedTerritoryIdValueProvider) != territoryId ||
          state.myStore?.id != storeId) {
        return;
      }
      state = state.copyWith(error: e, isProductsLoading: false);
    }
  }

  Future<void> saveMyStore({
    required String displayName,
    String? description,
  }) async {
    final territoryId = _ref.read(selectedTerritoryIdValueProvider);
    if (territoryId == null || territoryId.isEmpty) return;

    final store = await _repo.upsertMyStore(
      territoryId: territoryId,
      displayName: displayName,
      description: description,
    );
    state = state.copyWith(myStore: store);
    await loadMyProducts();
  }

  Future<void> addToCart(String itemId) async {
    final territoryId = _ref.read(selectedTerritoryIdValueProvider);
    if (territoryId == null || territoryId.isEmpty) return;
    await _repo.addToCart(territoryId: territoryId, itemId: itemId);
    final cart = await _repo.getCart(territoryId);
    state = state.copyWith(cart: cart);
  }

  Future<Map<String, dynamic>> checkout({String? message}) async {
    final territoryId = _ref.read(selectedTerritoryIdValueProvider);
    if (territoryId == null || territoryId.isEmpty) return {};
    final result =
        await _repo.checkout(territoryId: territoryId, message: message);
    final cart = await _repo.getCart(territoryId);
    state = state.copyWith(cart: cart);
    return result;
  }

  Future<Map<String, dynamic>> payWithPix(String transactionId) {
    return _repo.payWithPix(transactionId);
  }

  Future<Map<String, dynamic>> confirmPayment({
    required String transactionId,
    required String gatewayPaymentId,
  }) {
    return _repo.confirmPayment(
      transactionId: transactionId,
      gatewayPaymentId: gatewayPaymentId,
    );
  }

  Future<void> setPaymentsEnabled(bool enabled) async {
    final store = state.myStore;
    if (store == null) return;
    final updated = await _repo.setPaymentsEnabled(
      storeId: store.id,
      enabled: enabled,
    );
    state = state.copyWith(myStore: updated);
  }

  Future<StoreProduct> createProduct({
    required String title,
    String? description,
    String? category,
    required String pricingType,
    double? priceAmount,
    String currency = 'BRL',
  }) async {
    final territoryId = _ref.read(selectedTerritoryIdValueProvider);
    final store = state.myStore;
    if (territoryId == null || territoryId.isEmpty || store == null) {
      throw StateError('Store required');
    }
    final created = await _repo.createProduct(
      territoryId: territoryId,
      storeId: store.id,
      title: title,
      description: description,
      category: category,
      pricingType: pricingType,
      priceAmount: priceAmount,
      currency: currency,
    );
    // Atualização local: GET items pode vir do cache BFF (TTL 60s).
    state = state.copyWith(
      myProducts: [
        created,
        ...state.myProducts.where((p) => p.id != created.id),
      ],
    );
    return created;
  }

  Future<StoreProduct> updateProduct({
    required String itemId,
    required String title,
    String? description,
    String? category,
    required String pricingType,
    double? priceAmount,
    String currency = 'BRL',
  }) async {
    final updated = await _repo.updateProduct(
      itemId: itemId,
      title: title,
      description: description,
      category: category,
      pricingType: pricingType,
      priceAmount: pricingType == 'Fixed' ? priceAmount : null,
      includePriceAmount: true,
      currency: currency,
    );
    state = state.copyWith(
      myProducts: state.myProducts
          .map((p) => p.id == updated.id ? updated : p)
          .toList(),
    );
    return updated;
  }

  Future<void> archiveProduct(String itemId) async {
    await _repo.archiveProduct(itemId);
    state = state.copyWith(
      myProducts: state.myProducts.where((p) => p.id != itemId).toList(),
    );
  }

  Future<StoreProduct> getProduct(String itemId) => _repo.getProduct(itemId);
}

/// Não autoDispose: a jornada de produto e o checkout compartilham o mesmo estado
/// ao navegar entre rotas irmãs (`/marketplace` ↔ `/add-product-journey`).
final marketplaceProvider =
    StateNotifierProvider<MarketplaceNotifier, MarketplaceState>((ref) {
  return MarketplaceNotifier(ref);
});
