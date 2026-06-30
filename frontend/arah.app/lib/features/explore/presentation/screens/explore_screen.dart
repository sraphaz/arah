import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/providers/territory_provider.dart';
import '../../../../core/widgets/arah_scaffold.dart';
import '../../../../l10n/app_localizations.dart';
import '../../../territories/presentation/widgets/territory_indicator_bar.dart';
import '../../../territories/presentation/widgets/territory_selector.dart';

/// Explorar: territórios próximos e lista para selecionar outro território.
class ExploreScreen extends ConsumerWidget {
  const ExploreScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final l10n = AppLocalizations.of(context)!;
    final territoryId = ref.watch(selectedTerritoryIdValueProvider);
    final hasTerritory = territoryId != null && territoryId.isNotEmpty;

    return ArahScaffold(
      appBar: AppBar(
        title: Text(l10n.explore),
        actions: [
          if (hasTerritory) ...[
            IconButton(
              icon: const Icon(Icons.event_outlined),
              tooltip: l10n.events,
              onPressed: () => context.push('/events?territoryId=$territoryId'),
            ),
            IconButton(
              icon: const Icon(Icons.map_outlined),
              tooltip: l10n.viewOnMap,
              onPressed: () => context.push('/map?territoryId=$territoryId'),
            ),
            IconButton(
              icon: const Icon(Icons.warning_amber_outlined),
              tooltip: l10n.alertsTitle,
              onPressed: () => context.push('/alerts'),
            ),
            IconButton(
              icon: const Icon(Icons.storefront_outlined),
              tooltip: l10n.marketplace,
              onPressed: () => context.push('/marketplace'),
            ),
            IconButton(
              icon: const Icon(Icons.chat_outlined),
              tooltip: l10n.chat,
              onPressed: () => context.push('/chat'),
            ),
            IconButton(
              icon: const Icon(Icons.how_to_vote_outlined),
              tooltip: l10n.governance,
              onPressed: () => context.push('/governance?territoryId=$territoryId'),
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
              l10n.territories,
              style: Theme.of(context).textTheme.titleMedium?.copyWith(
                    color: Theme.of(context).colorScheme.onSurfaceVariant,
                  ),
            ),
          ),
          Padding(
            padding: const EdgeInsets.symmetric(horizontal: AppConstants.spacingMd),
            child: Text(
              l10n.territoriesSubtitle,
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
