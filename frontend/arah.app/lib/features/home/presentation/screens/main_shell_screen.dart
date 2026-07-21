import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/geo/geo_location_provider.dart';
import '../../../../core/providers/main_shell_tab_provider.dart';
import '../../../../core/theme/app_design_tokens.dart';
import '../../../../core/widgets/arah_scaffold.dart';
import '../../../../core/widgets/arah_top_bar.dart';
import '../../../../l10n/app_localizations.dart';
import '../../../explore/presentation/screens/explore_screen.dart';
import '../../../feed/presentation/screens/create_post_screen.dart';
import '../../../feed/presentation/screens/feed_screen.dart';
import '../../../profile/presentation/screens/profile_screen.dart';
import '../../../services/presentation/screens/services_hub_screen.dart';

/// Shell principal (ADR-021): Feed · Explorar · Publicar · Serviços · Perfil.
/// TopBar: território + Mensagens + Notificações.
class MainShellScreen extends ConsumerStatefulWidget {
  const MainShellScreen({super.key});

  @override
  ConsumerState<MainShellScreen> createState() => _MainShellScreenState();
}

class _MainShellScreenState extends ConsumerState<MainShellScreen> {
  void _setTab(int index) =>
      ref.read(mainShellTabProvider.notifier).state = index;

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      ref.read(geoLocationStateProvider.notifier).fetch();
    });
  }

  List<Widget> _buildScreens() => [
        FeedScreen(onGoToCreatePost: () => _setTab(2)),
        const ExploreScreen(),
        CreatePostScreen(onSuccess: () => _setTab(0)),
        const ServicesHubScreen(),
        const ProfileScreen(),
      ];

  @override
  Widget build(BuildContext context) {
    final currentIndex = ref.watch(mainShellTabProvider);
    final l10n = AppLocalizations.of(context)!;
    final colors = context.appColors;

    return ArahScaffold(
      extendBody: true,
      body: Column(
        children: [
          const ArahTopBar(),
          Expanded(
            child: IndexedStack(
              index: currentIndex,
              children: _buildScreens(),
            ),
          ),
        ],
      ),
      bottomNavigationBar: NavigationBar(
        selectedIndex: currentIndex,
        onDestinationSelected: _setTab,
        destinations: [
          NavigationDestination(
            icon: const Icon(Icons.home_outlined),
            selectedIcon: const Icon(Icons.home),
            label: l10n.home,
          ),
          NavigationDestination(
            icon: const Icon(Icons.search_outlined),
            selectedIcon: const Icon(Icons.search),
            label: l10n.explore,
          ),
          NavigationDestination(
            icon: const Icon(Icons.add_circle_outline),
            selectedIcon: Icon(
              Icons.add_circle,
              color: colors.primary,
              shadows: [
                Shadow(
                  color: AppDesignTokens.primaryHover.withValues(alpha: 0.35),
                  blurRadius: 18,
                  offset: const Offset(0, 4),
                ),
              ],
            ),
            label: l10n.post,
          ),
          NavigationDestination(
            icon: const Icon(Icons.grid_view_outlined),
            selectedIcon: const Icon(Icons.grid_view),
            label: l10n.services,
          ),
          NavigationDestination(
            icon: const Icon(Icons.person_outline),
            selectedIcon: const Icon(Icons.person),
            label: l10n.profile,
          ),
        ],
      ),
    );
  }
}
