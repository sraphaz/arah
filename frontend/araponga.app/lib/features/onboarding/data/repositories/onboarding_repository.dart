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
    final data = response.data as Map<String, dynamic>?;
    final list = data?['territories'] as List? ?? [];
    return list
        .map((e) => TerritorySuggestion.fromJson(e as Map<String, dynamic>))
        .toList();
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
