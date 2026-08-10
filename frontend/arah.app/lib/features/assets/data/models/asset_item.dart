class AssetItem {
  const AssetItem({
    required this.id,
    required this.name,
    required this.type,
    required this.status,
    this.description,
    this.subtype,
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
      subtype: json['subtype']?.toString(),
      validationsCount: (json['validationsCount'] as num?)?.toInt() ?? 0,
      validationPct: (json['validationPct'] as num?)?.toDouble() ?? 0,
    );
  }

  final String id;
  final String name;
  final String type;
  final String status;
  final String? description;
  final String? subtype;
  final int validationsCount;
  final double validationPct;

  bool get canValidate => status != 'ARCHIVED' && status != 'REJECTED';
  bool get canArchive => status != 'ARCHIVED';
  bool get canCurate => status == 'SUGGESTED';

  /// Ponte WA-E1: type=natural + subtype hídrico, ou legado type=river/spring/…
  bool get isWaterBody {
    final sub = subtype?.trim().toLowerCase();
    if (sub != null && sub.isNotEmpty && kWaterBodySubtypeValues.contains(sub)) {
      return true;
    }
    final t = type.trim().toLowerCase();
    return kWaterBodySubtypeValues.contains(t);
  }

  /// Chave de rótulo: subtype preferencial, senão type legado.
  String? get waterBodyKind {
    if (!isWaterBody) return null;
    final sub = subtype?.trim().toLowerCase();
    if (sub != null && sub.isNotEmpty) return sub;
    return type.trim().toLowerCase();
  }
}

/// Allowlist alinhada a NaturalWaterSubtype / mapa WA-E2.
const Set<String> kWaterBodySubtypeValues = {
  'river',
  'stream',
  'spring',
  'waterfall',
  'well',
  'potable_water',
};

const List<String> kWaterBodySubtypeOptions = [
  'river',
  'stream',
  'spring',
  'waterfall',
  'well',
  'potable_water',
];
