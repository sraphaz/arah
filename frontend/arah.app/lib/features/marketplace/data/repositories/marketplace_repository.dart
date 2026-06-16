import '../../../../core/network/api_exception.dart';
import '../../../../core/network/bff_client.dart';
import '../models/marketplace_item.dart';

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
}
