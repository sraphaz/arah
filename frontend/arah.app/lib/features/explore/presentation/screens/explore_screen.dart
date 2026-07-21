import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/providers/main_shell_tab_provider.dart';
import '../../../../l10n/app_localizations.dart';
import '../../../territories/presentation/widgets/territory_selector.dart';

/// Explorar: troca de território. Ferramentas do lugar vivem em Serviços (ADR-021).
class ExploreScreen extends ConsumerWidget {
  const ExploreScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final l10n = AppLocalizations.of(context)!;

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
        Padding(
          padding: const EdgeInsets.fromLTRB(
            AppConstants.spacingMd,
            AppConstants.spacingSm,
            AppConstants.spacingMd,
            AppConstants.spacingSm,
          ),
          child: OutlinedButton.icon(
            onPressed: () => ref.read(mainShellTabProvider.notifier).state = 3,
            icon: const Icon(Icons.grid_view_outlined),
            label: Text(l10n.services),
          ),
        ),
        const Expanded(child: TerritorySelector()),
      ],
    );
  }
}
