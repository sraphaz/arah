class MarketplaceSearchItem {
  const MarketplaceSearchItem({
    required this.id,
    required this.title,
    required this.description,
    required this.priceAmount,
    required this.currency,
    required this.storeName,
    this.primaryImageUrl,
  });

  factory MarketplaceSearchItem.fromJson(Map<String, dynamic> json) {
    final item = json['item'] as Map<String, dynamic>? ?? {};
    final store = json['store'] as Map<String, dynamic>? ?? {};
    final media = json['media'] as Map<String, dynamic>? ?? {};
    return MarketplaceSearchItem(
      id: item['id']?.toString() ?? '',
      title: item['title']?.toString() ?? 'Item',
      description: item['description']?.toString() ?? '',
      priceAmount: (item['priceAmount'] as num?)?.toDouble() ?? 0,
      currency: item['currency']?.toString() ?? 'BRL',
      storeName: store['name']?.toString() ?? store['ownerDisplayName']?.toString() ?? 'Loja',
      primaryImageUrl: media['primaryImageUrl']?.toString(),
    );
  }

  final String id;
  final String title;
  final String description;
  final double priceAmount;
  final String currency;
  final String storeName;
  final String? primaryImageUrl;
}
