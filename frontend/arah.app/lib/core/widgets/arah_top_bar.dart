import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../providers/current_territory_name_provider.dart';
import '../theme/app_design_tokens.dart';
import '../../l10n/app_localizations.dart';
import '../../features/notifications/presentation/providers/notifications_provider.dart';
import '../../features/territories/presentation/widgets/territory_indicator_bar.dart';
import 'arah_material_symbol.dart';

/// TopBar território-primeiro (ADR-021): território ativo + Mensagens + Notificações.
class ArahTopBar extends ConsumerWidget {
  const ArahTopBar({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final l10n = AppLocalizations.of(context)!;
    final territory = ref.watch(currentTerritoryItemProvider);
    final colors = context.appColors;
    final notifications = ref.watch(notificationsProvider);
    final hasUnreadNotifications =
        notifications.items.any((item) => !item.isRead);
    final territoryName = territory != null && territory.name.isNotEmpty
        ? territory.name
        : l10n.chooseTerritory;

    return SafeArea(
      bottom: false,
      child: Padding(
        padding: const EdgeInsets.fromLTRB(16, 6, 16, 14),
        child: Row(
          children: [
            Expanded(
              child: Material(
                color: Colors.transparent,
                child: InkWell(
                  onTap: () =>
                      TerritoryIndicatorBar.showTerritorySelectorSheet(context),
                  borderRadius: BorderRadius.circular(18),
                  child: Row(
                    children: [
                      Container(
                        width: 40,
                        height: 40,
                        decoration: BoxDecoration(
                          borderRadius: BorderRadius.circular(13),
                          border:
                              Border.all(color: AppDesignTokens.outlineStrong),
                          gradient: const LinearGradient(
                            begin: Alignment.topLeft,
                            end: Alignment.bottomRight,
                            colors: [
                              Color(0xFF1F2A22),
                              Color(0xFF141A15),
                            ],
                          ),
                          boxShadow: AppDesignTokens.elevation(1, isDark: true),
                        ),
                        child: Center(
                          child: Image.asset(
                            'assets/images/arah-logo.png',
                            width: 28,
                            height: 28,
                            fit: BoxFit.contain,
                            filterQuality: FilterQuality.high,
                          ),
                        ),
                      ),
                      const SizedBox(width: 10),
                      Expanded(
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          mainAxisSize: MainAxisSize.min,
                          children: [
                            Text(
                              'TERRITÓRIO',
                              style: Theme.of(context)
                                  .textTheme
                                  .labelSmall
                                  ?.copyWith(
                                    fontSize: 10.5,
                                    fontWeight: FontWeight.w600,
                                    letterSpacing: 1.2,
                                    color: colors.onSurfaceSubtle,
                                  ),
                            ),
                            const SizedBox(height: 1),
                            Row(
                              children: [
                                Flexible(
                                  child: Text(
                                    territoryName,
                                    style: Theme.of(context)
                                        .textTheme
                                        .titleMedium
                                        ?.copyWith(
                                          fontSize: 19,
                                          letterSpacing: -0.3,
                                        ),
                                    maxLines: 1,
                                    overflow: TextOverflow.ellipsis,
                                  ),
                                ),
                                const SizedBox(width: 5),
                                ArahMaterialSymbol(
                                  'unfold_more',
                                  size: 17,
                                  color: colors.onSurfaceSubtle,
                                ),
                              ],
                            ),
                          ],
                        ),
                      ),
                    ],
                  ),
                ),
              ),
            ),
            const SizedBox(width: 10),
            _TopIconButton(
              tooltip: l10n.chat,
              symbol: 'forum',
              showBadge: false,
              onPressed: () => context.push('/chat'),
            ),
            const SizedBox(width: 8),
            _TopIconButton(
              tooltip: l10n.notifications,
              symbol: 'notifications',
              showBadge: hasUnreadNotifications,
              onPressed: () => context.push('/notifications'),
            ),
          ],
        ),
      ),
    );
  }
}

class _TopIconButton extends StatelessWidget {
  const _TopIconButton({
    required this.tooltip,
    required this.symbol,
    required this.showBadge,
    required this.onPressed,
  });

  final String tooltip;
  final String symbol;
  final bool showBadge;
  final VoidCallback onPressed;

  @override
  Widget build(BuildContext context) {
    final colors = context.appColors;
    return Tooltip(
      message: tooltip,
      child: Material(
        color: Colors.transparent,
        child: InkWell(
          onTap: onPressed,
          borderRadius: BorderRadius.circular(12),
          child: Container(
            width: 40,
            height: 40,
            decoration: BoxDecoration(
              borderRadius: BorderRadius.circular(12),
              border: Border.all(color: colors.outline),
            ),
            child: Stack(
              children: [
                Center(
                  child: ArahMaterialSymbol(
                    symbol,
                    size: 21,
                    color: colors.onSurfaceVariant,
                  ),
                ),
                if (showBadge)
                  Positioned(
                    top: 7,
                    right: 8,
                    child: Container(
                      width: 8,
                      height: 8,
                      decoration: BoxDecoration(
                        shape: BoxShape.circle,
                        color: AppDesignTokens.warning,
                        border: Border.all(
                            color: AppDesignTokens.surfaceElevated, width: 1.5),
                      ),
                    ),
                  ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
