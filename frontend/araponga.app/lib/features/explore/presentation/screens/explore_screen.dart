import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/providers/territory_provider.dart';
import '../../../../l10n/app_localizations.dart';
import '../../../territories/presentation/widgets/territory_indicator_bar.dart';
import '../../../territories/presentation/widgets/territory_selector.dart';

/// Explorar: territórios próximos e lista para selecionar outro território.
class ExploreScreen extends ConsumerWidget {
  const ExploreScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final territoryId = ref.watch(selectedTerritoryIdValueProvider);
    final hasTerritory = territoryId != null && territoryId.isNotEmpty;

    return Scaffold(
      appBar: AppBar(
        title: Text(AppLocalizations.of(context)!.explore),
        actions: [
          if (hasTerritory) ...[
            IconButton(
              icon: const Icon(Icons.event_outlined),
              tooltip: AppLocalizations.of(context)!.events,
              onPressed: () => context.push('/events?territoryId=$territoryId'),
            ),
            IconButton(
              icon: const Icon(Icons.map_outlined),
              tooltip: AppLocalizations.of(context)!.viewOnMap,
              onPressed: () => context.push('/map?territoryId=$territoryId'),
            ),
          ],
        ],
      ),
      body: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          if (hasTerritory) const TerritoryIndicatorBar(),
          Padding(
            padding: const EdgeInsets.fromLTRB(
              AppConstants.spacingMd,
              AppConstants.spacingMd,
              AppConstants.spacingMd,
              AppConstants.spacingSm,
            ),
            child: Text(
              AppLocalizations.of(context)!.territories,
              style: Theme.of(context).textTheme.titleMedium?.copyWith(
                    color: Theme.of(context).colorScheme.onSurfaceVariant,
                  ),
            ),
          ),
          Padding(
            padding: const EdgeInsets.symmetric(horizontal: AppConstants.spacingMd),
            child: Text(
              AppLocalizations.of(context)!.territoriesSubtitle,
              style: Theme.of(context).textTheme.bodySmall?.copyWith(
                    color: Theme.of(context).colorScheme.onSurfaceVariant,
                  ),
            ),
          ),
          const SizedBox(height: AppConstants.spacingSm),
          const Expanded(child: TerritorySelector()),
        ],
      ),
    );
  }
}
