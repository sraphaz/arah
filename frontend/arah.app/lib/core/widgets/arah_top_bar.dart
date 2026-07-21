import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../config/constants.dart';
import '../providers/current_territory_name_provider.dart';
import '../theme/app_design_tokens.dart';
import '../../l10n/app_localizations.dart';
import '../../features/territories/presentation/widgets/territory_indicator_bar.dart';

/// TopBar território-primeiro (ADR-021): território ativo + Mensagens + Notificações.
class ArahTopBar extends ConsumerWidget {
  const ArahTopBar({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final l10n = AppLocalizations.of(context)!;
    final name = ref.watch(currentTerritoryNameProvider);
    final colors = context.appColors;

    return Material(
      color: colors.surfaceElevated.withValues(alpha: 0.95),
      child: SafeArea(
        bottom: false,
        child: SizedBox(
          height: kToolbarHeight,
          child: Padding(
            padding: const EdgeInsets.symmetric(horizontal: AppConstants.spacingSm),
            child: Row(
              children: [
                Expanded(
                  child: InkWell(
                    onTap: () => TerritoryIndicatorBar.showTerritorySelectorSheet(context),
                    borderRadius: BorderRadius.circular(AppConstants.radiusMd),
                    child: Padding(
                      padding: const EdgeInsets.symmetric(
                        horizontal: AppConstants.spacingSm,
                        vertical: AppConstants.spacingXs,
                      ),
                      child: Row(
                        children: [
                          Icon(Icons.forest_outlined, size: 22, color: colors.primary),
                          const SizedBox(width: AppConstants.spacingSm),
                          Expanded(
                            child: Column(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              mainAxisAlignment: MainAxisAlignment.center,
                              children: [
                                Text(
                                  l10n.inTerritory.toUpperCase(),
                                  style: Theme.of(context).textTheme.labelSmall?.copyWith(
                                        letterSpacing: AppDesignTokens.letterSpacingWide,
                                        color: colors.onSurfaceSubtle,
                                      ),
                                ),
                                Text(
                                  name != null && name.isNotEmpty
                                      ? name
                                      : l10n.chooseTerritory,
                                  style: Theme.of(context).textTheme.titleMedium,
                                  maxLines: 1,
                                  overflow: TextOverflow.ellipsis,
                                ),
                              ],
                            ),
                          ),
                          Icon(
                            Icons.keyboard_arrow_down,
                            size: 20,
                            color: colors.onSurfaceSubtle,
                          ),
                        ],
                      ),
                    ),
                  ),
                ),
                IconButton(
                  icon: const Icon(Icons.chat_bubble_outline),
                  tooltip: l10n.chat,
                  onPressed: () => context.push('/chat'),
                ),
                IconButton(
                  icon: const Icon(Icons.notifications_outlined),
                  tooltip: l10n.notifications,
                  onPressed: () => context.push('/notifications'),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
