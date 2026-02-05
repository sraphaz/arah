import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/providers/app_providers.dart';
import '../../data/models/me_profile.dart';
import '../../data/repositories/me_profile_repository.dart';

final meProfileRepositoryProvider = Provider<MeProfileRepository>((ref) {
  final client = ref.watch(bffClientProvider);
  return MeProfileRepository(client: client);
});

/// Perfil do usuário logado (GET me/profile). Requer token.
final meProfileProvider = FutureProvider.autoDispose<MeProfile>((ref) async {
  final repo = ref.watch(meProfileRepositoryProvider);
  return repo.getProfile();
});

/// Interesses do usuário (GET me/interests).
final meInterestsProvider = FutureProvider.autoDispose<List<String>>((ref) async {
  final repo = ref.watch(meProfileRepositoryProvider);
  return repo.getInterests();
});

/// Preferências do usuário (GET me/preferences).
final mePreferencesProvider = FutureProvider.autoDispose<UserPreferences>((ref) async {
  final repo = ref.watch(meProfileRepositoryProvider);
  return repo.getPreferences();
});
