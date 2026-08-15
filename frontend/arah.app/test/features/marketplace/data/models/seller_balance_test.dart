import 'package:arah_app/features/marketplace/data/models/seller_balance.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  test('SellerBalance.fromJson mapeia centavos e moeda', () {
    final balance = SellerBalance.fromJson({
      'pendingAmountInCents': 1500,
      'readyForPayoutAmountInCents': 2200,
      'paidAmountInCents': 3000,
      'currency': 'BRL',
    });
    expect(balance.pendingAmountInCents, 1500);
    expect(balance.readyForPayoutAmountInCents, 2200);
    expect(balance.paidAmountInCents, 3000);
    expect(balance.formatCents(1500), 'BRL 15.00');
    expect(balance.isEmpty, isFalse);
  });

  test('SellerBalance.zero representa ausência de ledger (404)', () {
    final balance = SellerBalance.zero();
    expect(balance.isEmpty, isTrue);
    expect(balance.formatCents(0), 'BRL 0.00');
  });
}
