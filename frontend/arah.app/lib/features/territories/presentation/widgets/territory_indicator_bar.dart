import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/providers/current_territory_name_provider.dart';
import '../../../../core/theme/app_design_tokens.dart';
import '../../../../l10n/app_localizations.dart';
import 'territory_selector.dart';

/// Barra fina que indica o território atual ("Em: [nome]"). Toque abre o bottom sheet do seletor.
class TerritoryIndicatorBar extends ConsumerWidget {
  const TerritoryIndicatorBar({super.key});

  static void showTerritorySelectorSheet(BuildContext context) {
    showModalBottomSheet<void>(
      context: context,
      isScrollControlled: true,
      useSafeArea: true,
      builder: (ctx) {
        final height = MediaQuery.of(ctx).size.height * 0.66;
        return SizedBox(
          height: height,
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              const SizedBox(height: AppConstants.spacingSm),
              Container(
                width: 38,
                height: 4,
                decoration: BoxDecoration(
                  color: Theme.of(ctx).colorScheme.outline.withValues(alpha: 0.8),
                  borderRadius: BorderRadius.circular(AppConstants.radiusXl),
                ),
              ),
              Padding(
                padding: const EdgeInsets.fromLTRB(
                  AppConstants.spacingLg,
                  AppConstants.spacingMd,
                  AppConstants.spacingLg,
                  AppConstants.spacingMd,
                ),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      AppLocalizations.of(context)!.territories,
                      style: Theme.of(ctx).textTheme.titleLarge?.copyWith(
                            fontFamily: AppDesignTokens.fontFamilyDisplay,
                            letterSpacing: -0.3,
                          ),
                    ),
                    const SizedBox(height: 4),
                    Text(
                      'Você participa como morador ou visitante.',
                      style: Theme.of(ctx).textTheme.bodySmall?.copyWith(
                            color: Theme.of(ctx).colorScheme.onSurfaceVariant,
                          ),
                    ),
                  ],
                ),
              ),
              Expanded(
                child: TerritorySelector(
                  onSelected: () => Navigator.of(ctx).maybePop(),
                  subtitle: 'Selecione um território ativo',
                ),
              ),
            ],
          ),
        );
      },
    );
  }

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final name = ref.watch(currentTerritoryNameProvider);
    final l10n = AppLocalizations.of(context)!;

    return Material(
      color: Theme.of(context).colorScheme.surfaceContainerLowest,
      child: InkWell(
        onTap: () => showTerritorySelectorSheet(context),
        child: Padding(
          padding: const EdgeInsets.symmetric(
            horizontal: AppConstants.spacingMd,
            vertical: AppConstants.spacingSm + 2,
          ),
          child: Row(
            children: [
              Icon(
                Icons.terrain_outlined,
                size: 18,
                color: Theme.of(context).colorScheme.primary,
              ),
              const SizedBox(width: AppConstants.spacingSm),
              Text(
                name != null && name.isNotEmpty
                    ? '${l10n.inTerritory}: $name'
                    : l10n.chooseTerritory,
                style: Theme.of(context).textTheme.bodySmall?.copyWith(
                      color: Theme.of(context).colorScheme.onSurfaceVariant,
                    ),
                maxLines: 1,
                overflow: TextOverflow.ellipsis,
              ),
              const SizedBox(width: AppConstants.spacingXs),
              Icon(
                Icons.keyboard_arrow_down,
                size: 20,
                color: Theme.of(context).colorScheme.onSurfaceVariant,
              ),
            ],
          ),
        ),
      ),
    );
  }
}
