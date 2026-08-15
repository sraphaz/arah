/// Saldo do vendedor no território — espelha `SellerBalanceResponse` da API.
class SellerBalance {
  const SellerBalance({
    required this.pendingAmountInCents,
    required this.readyForPayoutAmountInCents,
    required this.paidAmountInCents,
    required this.currency,
  });

  factory SellerBalance.zero({String currency = 'BRL'}) {
    return SellerBalance(
      pendingAmountInCents: 0,
      readyForPayoutAmountInCents: 0,
      paidAmountInCents: 0,
      currency: currency,
    );
  }

  factory SellerBalance.fromJson(Map<String, dynamic> json) {
    return SellerBalance(
      pendingAmountInCents: _asCents(json['pendingAmountInCents']),
      readyForPayoutAmountInCents:
          _asCents(json['readyForPayoutAmountInCents']),
      paidAmountInCents: _asCents(json['paidAmountInCents']),
      currency: json['currency']?.toString() ?? 'BRL',
    );
  }

  final int pendingAmountInCents;
  final int readyForPayoutAmountInCents;
  final int paidAmountInCents;
  final String currency;

  bool get isEmpty =>
      pendingAmountInCents == 0 &&
      readyForPayoutAmountInCents == 0 &&
      paidAmountInCents == 0;

  String formatCents(int cents) {
    final amount = cents / 100.0;
    return '$currency ${amount.toStringAsFixed(2)}';
  }

  static int _asCents(Object? value) {
    if (value is int) return value;
    if (value is num) return value.toInt();
    return int.tryParse(value?.toString() ?? '') ?? 0;
  }
}
