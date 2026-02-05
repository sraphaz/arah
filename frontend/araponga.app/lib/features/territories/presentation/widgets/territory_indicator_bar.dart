import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/providers/current_territory_name_provider.dart';
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
        final height = MediaQuery.of(ctx).size.height * 0.6;
        return SizedBox(
          height: height,
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              Padding(
                padding: const EdgeInsets.all(AppConstants.spacingMd),
                child: Text(
                  AppLocalizations.of(context)!.territories,
                  style: Theme.of(ctx).textTheme.titleMedium,
                ),
              ),
              Expanded(
                child: TerritorySelector(
                  onSelected: () => Navigator.of(ctx).maybePop(),
                  subtitle: '',
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
