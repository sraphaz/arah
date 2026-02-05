import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/providers/app_providers.dart';
import '../../data/models/onboarding_models.dart';
import '../../data/repositories/onboarding_repository.dart';

final onboardingRepositoryProvider = Provider<OnboardingRepository>((ref) {
  final client = ref.watch(bffClientProvider);
  return OnboardingRepository(client: client);
});

/// Territórios sugeridos por localização. Só preenchido após ter lat/long.
/// Refetch com [ref.invalidate(suggestedTerritoriesProvider)].
final suggestedTerritoriesProvider = FutureProvider.autoDispose
    .family<List<TerritorySuggestion>, ({double lat, double lng})>((ref, params) async {
  final repo = ref.watch(onboardingRepositoryProvider);
  return repo.getSuggestedTerritories(
    latitude: params.lat,
    longitude: params.lng,
    radiusKm: 10,
  );
});
