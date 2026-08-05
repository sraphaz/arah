import '../../../../core/network/api_exception.dart';
import '../../../../core/network/bff_client.dart';
import '../models/marketplace_item.dart';
import '../models/store_item.dart';
import '../models/store_product.dart';

class MarketplaceRepository {
  MarketplaceRepository({required BffClient client}) : _client = client;

  final BffClient _client;

  Future<List<MarketplaceSearchItem>> search({
    required String territoryId,
    String? query,
    int pageNumber = 1,
    int pageSize = 20,
  }) async {
    final params = <String, dynamic>{
      'territoryId': territoryId,
      'pageNumber': pageNumber,
      'pageSize': pageSize,
    };
    if (query != null && query.isNotEmpty) params['query'] = query;

    final response = await _client.get('marketplace', 'search', queryParameters: params);
    if (response.statusCode < 200 || response.statusCode >= 300) {
      throw ApiException('HTTP ${response.statusCode}', statusCode: response.statusCode);
    }
    final data = response.data as Map<String, dynamic>?;
    final items = data?['items'] as List? ?? [];
    return items.whereType<Map<String, dynamic>>().map(MarketplaceSearchItem.fromJson).toList();
  }

  Future<void> addToCart({
    required String territoryId,
    required String itemId,
    int quantity = 1,
  }) async {
    final response = await _client.post(
      'marketplace',
      'add-to-cart',
      body: {'territoryId': territoryId, 'itemId': itemId, 'quantity': quantity},
    );
    if (response.statusCode < 200 || response.statusCode >= 300) {
      throw ApiException('HTTP ${response.statusCode}', statusCode: response.statusCode);
    }
  }

  Future<Map<String, dynamic>> checkout({
    required String territoryId,
    String? message,
  }) async {
    final response = await _client.post(
      'marketplace',
      'checkout',
      body: {'territoryId': territoryId, 'message': message},
    );
    if (response.statusCode < 200 || response.statusCode >= 300) {
      throw ApiException('HTTP ${response.statusCode}', statusCode: response.statusCode);
    }
    return response.data as Map<String, dynamic>? ?? {};
  }

  Future<Map<String, dynamic>> getCart(String territoryId) async {
    final response = await _client.get(
      'marketplace-v1',
      'cart',
      queryParameters: {'territoryId': territoryId},
    );
    if (response.statusCode < 200 || response.statusCode >= 300) {
      throw ApiException('HTTP ${response.statusCode}', statusCode: response.statusCode);
    }
    return response.data as Map<String, dynamic>? ?? {};
  }

  Future<MyStore?> getMyStore(String territoryId) async {
    final response = await _client.get(
      'marketplace-v1',
      'stores/me',
      queryParameters: {'territoryId': territoryId},
    );
    if (response.statusCode == 404) return null;
    if (response.statusCode < 200 || response.statusCode >= 300) {
      throw ApiException('HTTP ${response.statusCode}', statusCode: response.statusCode);
    }
    return MyStore.fromJson(response.data as Map<String, dynamic>);
  }

  Future<MyStore> upsertMyStore({
    required String territoryId,
    required String displayName,
    String? description,
  }) async {
    final response = await _client.post(
      'marketplace-v1',
      'stores',
      body: {
        'territoryId': territoryId,
        'displayName': displayName,
        'description': description,
        'contactVisibility': 'PUBLIC',
      },
    );
    if (response.statusCode < 200 || response.statusCode >= 300) {
      throw ApiException('HTTP ${response.statusCode}', statusCode: response.statusCode);
    }
    return MyStore.fromJson(response.data as Map<String, dynamic>);
  }

  /// POST marketplace-v1/transactions/{id}/pay — inicia PIX (mock gateway em dev).
  Future<Map<String, dynamic>> payWithPix(String transactionId) async {
    final response = await _client.post(
      'marketplace-v1',
      'transactions/$transactionId/pay',
      body: const <String, dynamic>{'method': 'pix'},
    );
    if (response.statusCode < 200 || response.statusCode >= 300) {
      throw ApiException('HTTP ${response.statusCode}', statusCode: response.statusCode);
    }
    return response.data as Map<String, dynamic>? ?? {};
  }

  /// POST marketplace-v1/transactions/{id}/confirm-payment
  Future<Map<String, dynamic>> confirmPayment({
    required String transactionId,
    required String gatewayPaymentId,
  }) async {
    final response = await _client.post(
      'marketplace-v1',
      'transactions/$transactionId/confirm-payment',
      body: <String, dynamic>{'gatewayPaymentId': gatewayPaymentId},
    );
    if (response.statusCode < 200 || response.statusCode >= 300) {
      throw ApiException('HTTP ${response.statusCode}', statusCode: response.statusCode);
    }
    return response.data as Map<String, dynamic>? ?? {};
  }

  /// POST marketplace-v1/stores/{id}/payments/enable
  Future<MyStore> setPaymentsEnabled({
    required String storeId,
    required bool enabled,
  }) async {
    final response = await _client.post(
      'marketplace-v1',
      'stores/$storeId/payments/enable',
      body: <String, dynamic>{'enabled': enabled},
    );
    if (response.statusCode < 200 || response.statusCode >= 300) {
      throw ApiException('HTTP ${response.statusCode}', statusCode: response.statusCode);
    }
    return MyStore.fromJson(response.data as Map<String, dynamic>);
  }

  /// GET marketplace-v1/items?territoryId= — filtra por storeId no cliente.
  Future<List<StoreProduct>> listStoreProducts({
    required String territoryId,
    required String storeId,
  }) async {
    final response = await _client.get(
      'marketplace-v1',
      'items',
      queryParameters: {'territoryId': territoryId},
    );
    if (response.statusCode < 200 || response.statusCode >= 300) {
      throw ApiException('HTTP ${response.statusCode}', statusCode: response.statusCode);
    }
    final raw = response.data;
    final list = raw is List
        ? raw
        : (raw is Map ? (raw['items'] as List? ?? raw['value'] as List? ?? []) : []);
    return list
        .whereType<Map<String, dynamic>>()
        .map(StoreProduct.fromJson)
        .where((p) => p.storeId == storeId && !p.isArchived)
        .toList();
  }

  Future<StoreProduct> getProduct(String itemId) async {
    final response = await _client.get('marketplace-v1', 'items/$itemId');
    if (response.statusCode < 200 || response.statusCode >= 300) {
      throw ApiException('HTTP ${response.statusCode}', statusCode: response.statusCode);
    }
    return StoreProduct.fromJson(response.data as Map<String, dynamic>);
  }

  Future<StoreProduct> createProduct({
    required String territoryId,
    required String storeId,
    required String title,
    String? description,
    String? category,
    required String pricingType,
    double? priceAmount,
    String currency = 'BRL',
    String type = 'Product',
  }) async {
    final response = await _client.post(
      'marketplace-v1',
      'items',
      body: <String, dynamic>{
        'territoryId': territoryId,
        'storeId': storeId,
        'type': type,
        'title': title,
        'description': description,
        'category': category,
        'pricingType': pricingType,
        'priceAmount': priceAmount,
        'currency': currency,
        'status': 'Active',
      },
    );
    if (response.statusCode < 200 || response.statusCode >= 300) {
      throw ApiException('HTTP ${response.statusCode}', statusCode: response.statusCode);
    }
    return StoreProduct.fromJson(response.data as Map<String, dynamic>);
  }

  Future<StoreProduct> updateProduct({
    required String itemId,
    String? title,
    String? description,
    String? category,
    String? pricingType,
    double? priceAmount,
    String? currency,
    String? type,
  }) async {
    final body = <String, dynamic>{
      if (title != null) 'title': title,
      if (description != null) 'description': description,
      if (category != null) 'category': category,
      if (pricingType != null) 'pricingType': pricingType,
      if (priceAmount != null) 'priceAmount': priceAmount,
      if (currency != null) 'currency': currency,
      if (type != null) 'type': type,
    };
    final response = await _client.patch(
      'marketplace-v1',
      'items/$itemId',
      body: body,
    );
    if (response.statusCode < 200 || response.statusCode >= 300) {
      throw ApiException('HTTP ${response.statusCode}', statusCode: response.statusCode);
    }
    return StoreProduct.fromJson(response.data as Map<String, dynamic>);
  }

  Future<void> archiveProduct(String itemId) async {
    final response = await _client.post(
      'marketplace-v1',
      'items/$itemId/archive',
    );
    if (response.statusCode < 200 || response.statusCode >= 300) {
      throw ApiException('HTTP ${response.statusCode}', statusCode: response.statusCode);
    }
  }
}
