import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/geo/geo_location_provider.dart';
import '../../../../core/providers/main_shell_tab_provider.dart';
import '../../../../core/theme/app_design_tokens.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../core/widgets/arah_scaffold.dart';
import '../../../../core/widgets/arah_material_symbol.dart';
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

    return Theme(
      data: AppTheme.dark,
      child: ArahScaffold(
        extendBody: true,
        body: Stack(
          fit: StackFit.expand,
          children: [
            const ColoredBox(color: AppDesignTokens.surface),
            DecoratedBox(
              decoration: const BoxDecoration(
                gradient: RadialGradient(
                  center: Alignment(-0.6, -0.4),
                  radius: 1.2,
                  colors: AppDesignTokens.scaffoldGradientColors,
                  stops: [0.0, 0.45, 0.75, 1.0],
                ),
              ),
            ),
            Column(
              children: [
                const ArahTopBar(),
                Expanded(
                  child: Padding(
                    padding: const EdgeInsets.only(bottom: 106),
                    child: IndexedStack(
                      index: currentIndex,
                      children: _buildScreens(),
                    ),
                  ),
                ),
              ],
            ),
          ],
        ),
        bottomNavigationBar: _MainShellBottomNav(
          currentIndex: currentIndex,
          onSelect: _setTab,
          labels: (
            feed: 'Feed',
            explore: l10n.explore,
            post: l10n.post,
            services: l10n.services,
            profile: l10n.profile,
          ),
        ),
      ),
    );
  }
}

class _MainShellBottomNav extends StatelessWidget {
  const _MainShellBottomNav({
    required this.currentIndex,
    required this.onSelect,
    required this.labels,
  });

  final int currentIndex;
  final ValueChanged<int> onSelect;
  final ({
    String feed,
    String explore,
    String post,
    String services,
    String profile,
  }) labels;

  @override
  Widget build(BuildContext context) {
    final colors = context.appColors;
    final items = <_NavItem>[
      _NavItem(index: 0, label: labels.feed, symbol: 'eco'),
      _NavItem(index: 1, label: labels.explore, symbol: 'explore'),
      _NavItem(index: 2, label: labels.post, symbol: 'add', isCenter: true),
      _NavItem(index: 3, label: labels.services, symbol: 'grid_view'),
      _NavItem(index: 4, label: labels.profile, symbol: 'person'),
    ];

    return DecoratedBox(
      decoration: BoxDecoration(
        gradient: LinearGradient(
          begin: Alignment.topCenter,
          end: Alignment.bottomCenter,
          colors: [
            AppDesignTokens.surface.withValues(alpha: 0),
            AppDesignTokens.surface,
          ],
          stops: const [0.0, 0.64],
        ),
        border: Border(top: BorderSide(color: colors.outline)),
      ),
      child: SafeArea(
        top: false,
        child: Padding(
          padding: const EdgeInsets.fromLTRB(12, 10, 12, 10),
          child: Row(
            crossAxisAlignment: CrossAxisAlignment.end,
            children: items.map((item) {
              if (item.isCenter) {
                return SizedBox(
                  width: 72,
                  child: _CenterNavButton(
                    label: item.label,
                    selected: currentIndex == item.index,
                    onTap: () => onSelect(item.index),
                  ),
                );
              }
              return Expanded(
                child: _EdgeNavButton(
                  label: item.label,
                  symbol: item.symbol,
                  selected: currentIndex == item.index,
                  onTap: () => onSelect(item.index),
                ),
              );
            }).toList(),
          ),
        ),
      ),
    );
  }
}

class _NavItem {
  const _NavItem({
    required this.index,
    required this.label,
    required this.symbol,
    this.isCenter = false,
  });

  final int index;
  final String label;
  final String symbol;
  final bool isCenter;
}

class _EdgeNavButton extends StatelessWidget {
  const _EdgeNavButton({
    required this.label,
    required this.symbol,
    required this.selected,
    required this.onTap,
  });

  final String label;
  final String symbol;
  final bool selected;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final colors = context.appColors;
    return Material(
      color: Colors.transparent,
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(16),
        child: Padding(
          padding: const EdgeInsets.symmetric(vertical: 4),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              ArahMaterialSymbol(
                symbol,
                size: 25,
                fill: selected ? 1 : 0,
                weight: selected ? 500 : 400,
                color: selected ? colors.primary : colors.onSurfaceSubtle,
              ),
              const SizedBox(height: 5),
              Text(
                label,
                maxLines: 1,
                overflow: TextOverflow.ellipsis,
                style: Theme.of(context).textTheme.labelSmall?.copyWith(
                      fontSize: 10.5,
                      fontWeight: selected ? FontWeight.w600 : FontWeight.w500,
                      color: selected ? colors.primary : colors.onSurfaceSubtle,
                      letterSpacing: 0.1,
                    ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

class _CenterNavButton extends StatelessWidget {
  const _CenterNavButton({
    required this.label,
    required this.selected,
    required this.onTap,
  });

  final String label;
  final bool selected;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final colors = context.appColors;
    return Material(
      color: Colors.transparent,
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(18),
        child: Padding(
          padding: const EdgeInsets.symmetric(vertical: 2),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              Container(
                width: 54,
                height: 40,
                decoration: BoxDecoration(
                  borderRadius: BorderRadius.circular(16),
                  gradient: const LinearGradient(
                    begin: Alignment.topCenter,
                    end: Alignment.bottomCenter,
                    colors: [
                      AppDesignTokens.loginButtonGradientStart,
                      AppDesignTokens.loginButtonGradientEnd,
                    ],
                  ),
                  boxShadow: const [
                    BoxShadow(
                      color: AppDesignTokens.loginButtonGlow,
                      blurRadius: 18,
                      offset: Offset(0, 6),
                    ),
                  ],
                ),
                child: const Center(
                  child: ArahMaterialSymbol(
                    'add',
                    size: 26,
                    color: AppDesignTokens.textOnAccent,
                    weight: 500,
                  ),
                ),
              ),
              const SizedBox(height: 5),
              Text(
                label,
                maxLines: 1,
                overflow: TextOverflow.ellipsis,
                style: Theme.of(context).textTheme.labelSmall?.copyWith(
                      fontSize: 10.5,
                      fontWeight: FontWeight.w500,
                      color: selected ? colors.primary : colors.onSurfaceSubtle,
                      letterSpacing: 0.1,
                    ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
