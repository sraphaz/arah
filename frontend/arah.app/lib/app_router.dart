import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import 'core/config/app_config.dart';
import 'core/providers/territory_provider.dart';
import 'core/widgets/arah_scaffold.dart';
import 'core/widgets/arah_loading_indicator.dart';
import 'features/auth/presentation/screens/login_screen.dart';
import 'features/auth/presentation/providers/auth_state_provider.dart';
import 'features/home/presentation/screens/main_shell_screen.dart';
import 'features/map/presentation/screens/map_screen.dart';
import 'features/events/presentation/screens/events_screen.dart';
import 'features/connections/presentation/screens/connections_screen.dart';
import 'features/alerts/presentation/screens/alerts_screen.dart';
import 'features/membership/presentation/screens/membership_screen.dart';
import 'features/marketplace/presentation/screens/marketplace_screen.dart';
import 'features/chat/presentation/screens/chat_list_screen.dart';
import 'features/chat/presentation/screens/chat_conversation_screen.dart';
import 'features/moderation/presentation/screens/moderation_screen.dart';
import 'features/governance/presentation/screens/governance_screen.dart';
import 'features/feed/presentation/screens/post_detail_screen.dart';
import 'features/assets/presentation/screens/assets_screen.dart';
import 'features/subscriptions/presentation/screens/subscriptions_screen.dart';
import 'features/notifications/presentation/screens/notifications_screen.dart';
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
      GoRoute(
        path: '/connections',
        builder: (_, __) => const ConnectionsScreen(),
      ),
      GoRoute(
        path: '/alerts',
        builder: (_, __) => const AlertsScreen(),
      ),
      GoRoute(
        path: '/membership',
        builder: (_, __) => const MembershipScreen(),
      ),
      GoRoute(
        path: '/marketplace',
        builder: (_, __) => const MarketplaceScreen(),
      ),
      GoRoute(
        path: '/chat',
        builder: (_, __) => const ChatListScreen(),
      ),
      GoRoute(
        path: '/chat/:conversationId',
        builder: (_, state) {
          final id = state.pathParameters['conversationId'] ?? '';
          final title = state.uri.queryParameters['title'];
          return ChatConversationScreen(conversationId: id, title: title);
        },
      ),
      GoRoute(
        path: '/moderation',
        builder: (_, __) => const ModerationScreen(),
      ),
      GoRoute(
        path: '/governance',
        builder: (_, state) {
          final territoryId = state.uri.queryParameters['territoryId'];
          return GovernanceScreen(territoryId: territoryId);
        },
      ),
      GoRoute(
        path: '/post',
        builder: (_, state) {
          final territoryId = state.uri.queryParameters['territoryId'] ?? '';
          final postId = state.uri.queryParameters['postId'] ?? '';
          return PostDetailScreen(territoryId: territoryId, postId: postId);
        },
      ),
      GoRoute(
        path: '/assets',
        builder: (_, __) => const AssetsScreen(),
      ),
      GoRoute(
        path: '/subscriptions',
        builder: (_, __) => const SubscriptionsScreen(),
      ),
      GoRoute(
        path: '/notifications',
        builder: (_, __) => const NotificationsScreen(),
      ),
    ],
  );
});

class _SplashOrRedirect extends StatelessWidget {
  const _SplashOrRedirect();

  @override
  Widget build(BuildContext context) {
    return const ArahScaffold(
      showWatermark: true,
      body: Center(child: ArahLoadingIndicator()),
    );
  }
}
