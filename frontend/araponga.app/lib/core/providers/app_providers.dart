import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../config/app_config.dart';
import '../network/bff_client.dart';
import '../geo/geo_location_provider.dart';
import '../../features/auth/presentation/providers/auth_state_provider.dart';
import 'territory_provider.dart';

final bffClientProvider = Provider<BffClient>((ref) {
  final config = ref.watch(appConfigProvider);
  final token = ref.watch(accessTokenProvider);
  final sessionId = ref.watch(selectedTerritoryIdValueProvider);
  final geo = ref.watch(geoLocationStateProvider);
  return BffClient(
    config: config,
    accessToken: token,
    sessionId: sessionId,
    latitude: geo?.latitude,
    longitude: geo?.longitude,
    onUnauthorized: () {
      ref.read(authStateProvider.notifier).logout();
    },
  );
});
