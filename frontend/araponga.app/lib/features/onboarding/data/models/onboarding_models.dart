/// Um território sugerido pelo backend (onboarding/suggested-territories).
class TerritorySuggestion {
  const TerritorySuggestion({
    required this.id,
    required this.name,
    this.description,
    required this.distanceKm,
    required this.latitude,
    required this.longitude,
  });

  final String id;
  final String name;
  final String? description;
  final double distanceKm;
  final double latitude;
  final double longitude;

  factory TerritorySuggestion.fromJson(Map<String, dynamic> json) {
    final id = json['id'];
    return TerritorySuggestion(
      id: id == null ? '' : id.toString(),
      name: json['name'] as String? ?? '',
      description: json['description'] as String?,
      distanceKm: (json['distanceKm'] as num?)?.toDouble() ?? 0,
      latitude: (json['latitude'] as num?)?.toDouble() ?? 0,
      longitude: (json['longitude'] as num?)?.toDouble() ?? 0,
    );
  }
}

/// Resposta do POST onboarding/complete (resumo para sucesso).
class CompleteOnboardingResult {
  const CompleteOnboardingResult({
    required this.territoryId,
    required this.territoryName,
  });

  final String territoryId;
  final String territoryName;

  factory CompleteOnboardingResult.fromJson(Map<String, dynamic> json) {
    final territory = json['territory'] as Map<String, dynamic>?;
    final id = territory?['id'];
    return CompleteOnboardingResult(
      territoryId: id == null ? '' : id.toString(),
      territoryName: territory?['name'] as String? ?? '',
    );
  }
}
