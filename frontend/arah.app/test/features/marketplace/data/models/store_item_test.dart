import 'package:arah_app/features/marketplace/data/models/store_item.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  test('MyStore.fromJson lê paymentsEnabled', () {
    final store = MyStore.fromJson({
      'id': 's1',
      'displayName': 'Loja Teste',
      'status': 'Active',
      'description': 'desc',
      'paymentsEnabled': true,
    });

    expect(store.id, 's1');
    expect(store.displayName, 'Loja Teste');
    expect(store.paymentsEnabled, isTrue);
  });

  test('MyStore.fromJson defaults paymentsEnabled para false', () {
    final store = MyStore.fromJson({
      'id': 's2',
      'displayName': 'Sem PIX',
      'status': 'Draft',
    });

    expect(store.paymentsEnabled, isFalse);
  });
}
