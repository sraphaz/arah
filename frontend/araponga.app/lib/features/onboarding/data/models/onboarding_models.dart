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
    final id = json['id'] ?? json['Id'];
    final name = json['name'] ?? json['Name'];
    final description = json['description'] ?? json['Description'];
    final distanceKm = json['distanceKm'] ?? json['DistanceKm'];
    final lat = json['latitude'] ?? json['Latitude'];
    final lng = json['longitude'] ?? json['Longitude'];
    return TerritorySuggestion(
      id: id == null ? '' : id.toString(),
      name: name is String ? name : (name?.toString() ?? ''),
      description: description is String ? description : (description?.toString()),
      distanceKm: (distanceKm is num) ? distanceKm.toDouble() : 0,
      latitude: (lat is num) ? lat.toDouble() : 0,
      longitude: (lng is num) ? lng.toDouble() : 0,
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
