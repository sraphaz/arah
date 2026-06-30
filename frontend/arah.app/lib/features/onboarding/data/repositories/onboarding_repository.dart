import 'dart:convert';

import '../../../../core/network/api_exception.dart';
import '../../../../core/network/bff_client.dart';
import '../models/onboarding_models.dart';

/// Repositório da jornada de onboarding (territórios sugeridos e complete).
class OnboardingRepository {
  OnboardingRepository({required this.client});

  final BffClient client;

  /// GET onboarding/suggested-territories?latitude=&longitude=&radiusKm=
  /// Não exige X-Session-Id. Usa geo do client se disponível.
  Future<List<TerritorySuggestion>> getSuggestedTerritories({
    required double latitude,
    required double longitude,
    double radiusKm = 10,
  }) async {
    final path = 'suggested-territories'
        '?latitude=$latitude&longitude=$longitude&radiusKm=$radiusKm';
    final response = await client.get('onboarding', path);
    final list = _extractTerritoriesList(response.data);
    final result = <TerritorySuggestion>[];
    for (final e in list) {
      if (e is! Map<String, dynamic>) continue;
      try {
        result.add(TerritorySuggestion.fromJson(e));
      } catch (_) {}
    }
    return result;
  }

  static List<dynamic> _extractTerritoriesList(dynamic data) {
    if (data == null) return [];
    Map<String, dynamic>? map;
    if (data is Map<String, dynamic>) {
      map = data;
    } else if (data is String) {
      try {
        final decoded = jsonDecode(data);
        if (decoded is Map<String, dynamic>) map = decoded;
      } catch (_) {}
    }
    if (map == null) return [];
    return map['territories'] as List? ?? map['Territories'] as List? ?? [];
  }

  /// POST onboarding/propose-territory — desenho manual, status Pending.
  Future<TerritorySuggestion> proposeTerritory({
    required String city,
    required String state,
    required double latitude,
    required double longitude,
    String? name,
    double? radiusKm,
    List<Map<String, double>>? boundaryPolygon,
  }) async {
    final body = <String, dynamic>{
      'city': city,
      'state': state,
      'latitude': latitude,
      'longitude': longitude,
      if (name != null && name.isNotEmpty) 'name': name,
      if (radiusKm != null) 'radiusKm': radiusKm,
      if (boundaryPolygon != null && boundaryPolygon.isNotEmpty)
        'boundaryPolygon': boundaryPolygon,
    };
    final response = await client.post('onboarding', 'propose-territory', body: body);
    return _parseTerritorySuggestion(response.data, fallbackLat: latitude, fallbackLng: longitude);
  }

  TerritorySuggestion _parseTerritorySuggestion(
    dynamic data, {
    required double fallbackLat,
    required double fallbackLng,
  }) {
    Map<String, dynamic>? map;
    if (data is Map<String, dynamic>) {
      map = data;
    } else if (data is String) {
      try {
        final decoded = jsonDecode(data);
        if (decoded is Map<String, dynamic>) map = decoded;
      } catch (_) {}
    }
    if (map == null) throw ApiException('Resposta inválida');
    final id = map['territoryId'] ?? map['TerritoryId'] ?? map['id'] ?? map['Id'];
    final name = map['name'] ?? map['Name'];
    final description = map['description'] ?? map['Description'];
    final distanceKm = map['distanceKm'] ?? map['DistanceKm'];
    final lat = map['latitude'] ?? map['Latitude'];
    final lng = map['longitude'] ?? map['Longitude'];
    final isPending = map['isPending'] ?? map['IsPending'];
    return TerritorySuggestion(
      id: id == null ? '' : id.toString(),
      name: name is String ? name : (name?.toString() ?? ''),
      description: description is String ? description : (description?.toString()),
      distanceKm: (distanceKm is num) ? distanceKm.toDouble() : 0,
      latitude: (lat is num) ? lat.toDouble() : fallbackLat,
      longitude: (lng is num) ? lng.toDouble() : fallbackLng,
      isPending: isPending == true || (isPending is String && isPending.toLowerCase() == 'true'),
    );
  }

  /// POST onboarding/suggest-municipality — cadastra município IBGE se necessário.
  Future<TerritorySuggestion> suggestMunicipality({
    required double latitude,
    required double longitude,
    String? city,
    String? state,
  }) async {
    final body = <String, dynamic>{
      'latitude': latitude,
      'longitude': longitude,
      if (city != null && city.isNotEmpty) 'city': city,
      if (state != null && state.isNotEmpty) 'state': state,
    };
    final response = await client.post('onboarding', 'suggest-municipality', body: body);
    return _parseTerritorySuggestion(response.data, fallbackLat: latitude, fallbackLng: longitude);
  }

  /// POST onboarding/complete com body { selectedTerritoryId }.
  /// O backend exige header X-Session-Id; enviamos [selectedTerritoryId] como session para esta requisição.
  Future<CompleteOnboardingResult> completeOnboarding(String selectedTerritoryId) async {
    final response = await client.post(
      'onboarding',
      'complete',
      body: <String, dynamic>{
        'selectedTerritoryId': selectedTerritoryId,
      },
      sessionIdOverride: selectedTerritoryId,
    );
    final data = response.data as Map<String, dynamic>?;
    if (data == null) throw ApiException('Resposta inválida');
    return CompleteOnboardingResult.fromJson(data);
  }
}
