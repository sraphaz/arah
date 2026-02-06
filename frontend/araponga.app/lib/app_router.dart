import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import 'core/config/app_config.dart';
import 'core/providers/territory_provider.dart';
import 'features/auth/presentation/screens/login_screen.dart';
import 'features/auth/presentation/providers/auth_state_provider.dart';
import 'features/home/presentation/screens/main_shell_screen.dart';
import 'features/map/presentation/screens/map_screen.dart';
import 'features/events/presentation/screens/events_screen.dart';
import 'features/onboarding/presentation/screens/onboarding_screen.dart';

/// Rotas com guard de auth e onboarding: sem token → /login; com token sem território → /onboarding; com território → /home.
final goRouterProvider = Provider<GoRouter>((ref) {
  final config = ref.watch(appConfigProvider);
  final auth = ref.watch(authStateProvider);
  final territory = ref.watch(selectedTerritoryIdProvider);

  final hasToken = auth.valueOrNull?.accessToken != null &&
      (auth.valueOrNull!.accessToken.isNotEmpty);
  final territoryLoading = territory.isLoading;
  final hasTerritory = territory.valueOrNull != null && territory.valueOrNull!.isNotEmpty;

  return GoRouter(
    initialLocation: '/',
    redirect: (BuildContext context, GoRouterState state) {
      final location = state.matchedLocation;

      if (auth.isLoading) return null;
      if (!hasToken) {
        if (location == '/login') return null;
        return '/login';
      }

      // Sem território salvo → onboarding (seleção). Com território → feed. O usuário só vira visitante ao concluir o onboarding.
      if (territoryLoading && location == '/') return null;
      if (hasTerritory) {
        if (location == '/') return '/home';
        if (location == '/onboarding') return '/home';
      } else {
        if (location == '/') return '/onboarding';
        if (location == '/home') return '/onboarding';
      }
      if (location == '/login' && hasToken) return hasTerritory ? '/home' : '/onboarding';
      return null;
    },
    routes: [
      GoRoute(
        path: '/',
        builder: (_, __) => const _SplashOrRedirect(),
      ),
      GoRoute(
        path: '/login',
        builder: (_, __) => LoginScreen(bffBaseUrl: config.bffBaseUrl),
      ),
      GoRoute(
        path: '/onboarding',
        builder: (_, __) => const OnboardingScreen(),
      ),
      GoRoute(
        path: '/home',
        builder: (_, __) => const MainShellScreen(),
      ),
      GoRoute(
        path: '/map',
        builder: (_, state) {
          final territoryId = state.uri.queryParameters['territoryId'];
          return MapScreen(territoryId: territoryId);
        },
      ),
      GoRoute(
        path: '/events',
        builder: (_, state) {
          final territoryId = state.uri.queryParameters['territoryId'];
          return EventsScreen(territoryId: territoryId);
        },
      ),
    ],
  );
});

class _SplashOrRedirect extends StatelessWidget {
  const _SplashOrRedirect();

  @override
  Widget build(BuildContext context) {
    return const Scaffold(
      body: Center(child: CircularProgressIndicator()),
    );
  }
}
