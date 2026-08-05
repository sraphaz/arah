/// Item (produto/serviço) da loja — espelha `ItemResponse` da API.
class StoreProduct {
  const StoreProduct({
    required this.id,
    required this.territoryId,
    required this.storeId,
    required this.type,
    required this.title,
    required this.pricingType,
    required this.status,
    this.description,
    this.category,
    this.priceAmount,
    this.currency,
    this.unit,
    this.primaryImageUrl,
  });

  factory StoreProduct.fromJson(Map<String, dynamic> json) {
    return StoreProduct(
      id: json['id']?.toString() ?? '',
      territoryId: json['territoryId']?.toString() ?? '',
      storeId: json['storeId']?.toString() ?? '',
      type: json['type']?.toString() ?? 'Product',
      title: json['title']?.toString() ?? '',
      description: json['description']?.toString(),
      category: json['category']?.toString(),
      pricingType: json['pricingType']?.toString() ?? 'Fixed',
      priceAmount: (json['priceAmount'] as num?)?.toDouble(),
      currency: json['currency']?.toString(),
      unit: json['unit']?.toString(),
      status: json['status']?.toString() ?? 'Active',
      primaryImageUrl: json['primaryImageUrl']?.toString(),
    );
  }

  final String id;
  final String territoryId;
  final String storeId;
  final String type;
  final String title;
  final String? description;
  final String? category;
  final String pricingType;
  final double? priceAmount;
  final String? currency;
  final String? unit;
  final String status;
  final String? primaryImageUrl;

  bool get isArchived =>
      status.toLowerCase() == 'archived' || status.toUpperCase() == 'ARCHIVED';

  String get priceLabel {
    if (priceAmount == null) return '—';
    final cur = currency ?? 'BRL';
    return '$cur ${priceAmount!.toStringAsFixed(2)}';
  }
}
