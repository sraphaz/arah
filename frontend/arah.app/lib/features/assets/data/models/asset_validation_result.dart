class AssetValidationResult {
  const AssetValidationResult({
    required this.assetId,
    required this.validationsCount,
    required this.eligibleResidentsCount,
    required this.validationPct,
  });

  factory AssetValidationResult.fromJson(Map<String, dynamic> json) {
    return AssetValidationResult(
      assetId: json['assetId']?.toString() ?? '',
      validationsCount: (json['validationsCount'] as num?)?.toInt() ?? 0,
      eligibleResidentsCount: (json['eligibleResidentsCount'] as num?)?.toInt() ?? 0,
      validationPct: (json['validationPct'] as num?)?.toDouble() ?? 0,
    );
  }

  final String assetId;
  final int validationsCount;
  final int eligibleResidentsCount;
  final double validationPct;
}
