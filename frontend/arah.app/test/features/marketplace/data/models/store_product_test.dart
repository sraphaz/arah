import 'package:arah_app/features/marketplace/data/models/store_product.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  test('StoreProduct.fromJson e priceLabel', () {
    final product = StoreProduct.fromJson({
      'id': 'p1',
      'territoryId': 't1',
      'storeId': 's1',
      'type': 'Product',
      'title': 'Mel local',
      'pricingType': 'Fixed',
      'priceAmount': 12.5,
      'currency': 'BRL',
      'status': 'Active',
    });

    expect(product.id, 'p1');
    expect(product.storeId, 's1');
    expect(product.isArchived, isFalse);
    expect(product.priceLabel, 'BRL 12.50');
  });

  test('StoreProduct reconhece archived', () {
    final product = StoreProduct.fromJson({
      'id': 'p2',
      'territoryId': 't1',
      'storeId': 's1',
      'type': 'Product',
      'title': 'X',
      'pricingType': 'Free',
      'status': 'ARCHIVED',
    });
    expect(product.isArchived, isTrue);
  });
}
