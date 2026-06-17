class AssetItem {
  const AssetItem({
    required this.id,
    required this.name,
    required this.type,
    required this.status,
    this.description,
    this.validationsCount = 0,
    this.validationPct = 0,
  });

  factory AssetItem.fromJson(Map<String, dynamic> json) {
    return AssetItem(
      id: json['id']?.toString() ?? '',
      name: json['name']?.toString() ?? 'Asset',
      type: json['type']?.toString() ?? '',
      status: json['status']?.toString() ?? '',
      description: json['description']?.toString(),
      validationsCount: (json['validationsCount'] as num?)?.toInt() ?? 0,
      validationPct: (json['validationPct'] as num?)?.toDouble() ?? 0,
    );
  }

  final String id;
  final String name;
  final String type;
  final String status;
  final String? description;
  final int validationsCount;
  final double validationPct;

  bool get canValidate => status != 'ARCHIVED' && status != 'REJECTED';
  bool get canArchive => status != 'ARCHIVED';
  bool get canCurate => status == 'SUGGESTED';
}
