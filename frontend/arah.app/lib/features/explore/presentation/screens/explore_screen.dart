import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/providers/main_shell_tab_provider.dart';
import '../../../../core/providers/territory_provider.dart';
import '../../../../core/theme/app_design_tokens.dart';
import '../../../../l10n/app_localizations.dart';
import '../../../territories/presentation/widgets/territory_selector.dart';

/// Explorar: troca de território + atalhos das ferramentas do território (ADR-021).
class ExploreScreen extends ConsumerWidget {
  const ExploreScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final l10n = AppLocalizations.of(context)!;
    final colors = context.appColors;
    final territoryId = ref.watch(selectedTerritoryIdValueProvider);
    final hasTerritory = territoryId != null && territoryId.isNotEmpty;

    final tools = <_ExploreTool>[
      if (hasTerritory) ...[
        _ExploreTool(
          label: l10n.map,
          icon: Icons.map_outlined,
          onTap: () => context.push('/map?territoryId=$territoryId'),
        ),
        _ExploreTool(
          label: l10n.events,
          icon: Icons.event_outlined,
          onTap: () => context.push('/events?territoryId=$territoryId'),
        ),
        _ExploreTool(
          label: l10n.alertsTitle,
          icon: Icons.warning_amber_outlined,
          onTap: () => context.push('/alerts'),
        ),
        _ExploreTool(
          label: l10n.marketplace,
          icon: Icons.storefront_outlined,
          onTap: () => context.push('/marketplace'),
        ),
        _ExploreTool(
          label: l10n.governance,
          icon: Icons.how_to_vote_outlined,
          onTap: () => context.push('/governance?territoryId=$territoryId'),
        ),
      ],
      _ExploreTool(
        label: l10n.services,
        icon: Icons.grid_view_outlined,
        onTap: () => ref.read(mainShellTabProvider.notifier).state = 3,
      ),
    ];

    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Padding(
          padding: const EdgeInsets.fromLTRB(
            AppConstants.spacingMd,
            AppConstants.spacingMd,
            AppConstants.spacingMd,
            AppConstants.spacingSm,
          ),
          child: Text(
            l10n.explore,
            style: Theme.of(context).textTheme.titleLarge?.copyWith(
                  fontFamily: AppDesignTokens.fontFamilyDisplay,
                ),
          ),
        ),
        if (tools.isNotEmpty)
          Padding(
            padding: const EdgeInsets.symmetric(horizontal: AppConstants.spacingMd),
            child: Wrap(
              spacing: AppConstants.spacingSm,
              runSpacing: AppConstants.spacingSm,
              children: [
                for (final tool in tools)
                  ActionChip(
                    avatar: Icon(tool.icon, size: AppConstants.iconSizeSm, color: colors.primary),
                    label: Text(tool.label),
                    onPressed: tool.onTap,
                    side: BorderSide(color: colors.outlineSubtle),
                    backgroundColor: colors.surfaceContainer,
                  ),
              ],
            ),
          ),
        Padding(
          padding: const EdgeInsets.fromLTRB(
            AppConstants.spacingMd,
            AppConstants.spacingLg,
            AppConstants.spacingMd,
            AppConstants.spacingSm,
          ),
          child: Text(
            l10n.territories,
            style: Theme.of(context).textTheme.titleMedium?.copyWith(
                  color: colors.onSurfaceVariant,
                ),
          ),
        ),
        Padding(
          padding: const EdgeInsets.symmetric(horizontal: AppConstants.spacingMd),
          child: Text(
            l10n.territoriesSubtitle,
            style: Theme.of(context).textTheme.bodySmall?.copyWith(
                  color: colors.onSurfaceVariant,
                ),
          ),
        ),
        const SizedBox(height: AppConstants.spacingSm),
        const Expanded(child: TerritorySelector()),
      ],
    );
  }
}

class _ExploreTool {
  const _ExploreTool({
    required this.label,
    required this.icon,
    required this.onTap,
  });

  final String label;
  final IconData icon;
  final VoidCallback onTap;
}
