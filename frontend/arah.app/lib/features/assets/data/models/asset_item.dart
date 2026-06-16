class AssetItem {
  const AssetItem({
    required this.id,
    required this.name,
    required this.type,
    required this.status,
    this.description,
  });

  factory AssetItem.fromJson(Map<String, dynamic> json) {
    return AssetItem(
      id: json['id']?.toString() ?? '',
      name: json['name']?.toString() ?? 'Asset',
      type: json['type']?.toString() ?? '',
      status: json['status']?.toString() ?? '',
      description: json['description']?.toString(),
    );
  }

  final String id;
  final String name;
  final String type;
  final String status;
  final String? description;
}
