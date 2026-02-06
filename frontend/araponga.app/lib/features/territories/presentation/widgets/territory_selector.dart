import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../core/providers/territory_provider.dart';
import '../../../../core/widgets/app_snackbar.dart';
import '../../../../core/widgets/shimmer_skeleton.dart';
import '../../../../l10n/app_localizations.dart';
import '../providers/territories_list_provider.dart';

class TerritorySelector extends ConsumerWidget {
  const TerritorySelector({super.key, this.onSelected, this.subtitle = 'Toque para ver o feed da região'});
  final VoidCallback? onSelected;
  final String subtitle;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final async = ref.watch(territoriesListProvider);
    final selectedId = ref.watch(selectedTerritoryIdValueProvider);

    return async.when(
      data: (list) {
        if (list.isEmpty) {
          return Center(
            child: Padding(
              padding: const EdgeInsets.all(AppConstants.spacingLg),
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Icon(Icons.terrain_outlined, size: AppConstants.iconSizeLg, color: Theme.of(context).colorScheme.onSurfaceVariant),
                  const SizedBox(height: AppConstants.spacingMd),
                  Text(AppLocalizations.of(context)!.noTerritoryAvailable, style: Theme.of(context).textTheme.titleMedium, textAlign: TextAlign.center),
                ],
              ),
            ),
          );
        }
        return ListView.builder(
          padding: const EdgeInsets.symmetric(horizontal: AppConstants.spacingMd, vertical: AppConstants.spacingSm),
          itemCount: list.length,
          itemBuilder: (context, index) {
            final t = list[index];
            final isSelected = selectedId == t.id;
            return Card(
              margin: const EdgeInsets.only(bottom: AppConstants.spacingSm),
              child: ListTile(
                leading: CircleAvatar(
                  backgroundColor: isSelected ? Theme.of(context).colorScheme.primary : Theme.of(context).colorScheme.surfaceContainerHighest,
                  child: Icon(Icons.terrain, color: isSelected ? Theme.of(context).colorScheme.onPrimary : Theme.of(context).colorScheme.onSurfaceVariant),
                ),
                title: Text(t.name),
                subtitle: t.description != null && t.description!.isNotEmpty ? Text(t.description!, maxLines: 2, overflow: TextOverflow.ellipsis) : null,
                trailing: isSelected ? Icon(Icons.check_circle, color: Theme.of(context).colorScheme.primary) : null,
                onTap: () async {
                  try {
                    await ref.read(territoriesRepositoryProvider).enterTerritory(t.id);
                    await ref.read(selectedTerritoryIdProvider.notifier).setTerritoryId(t.id);
                    if (context.mounted) onSelected?.call();
                  } catch (e) {
                    if (!context.mounted) return;
                    final msg = e is ApiException ? e.userMessage : 'Não foi possível entrar no território.';
                    showErrorSnackBar(context, msg);
                  }
                },
              ),
            );
          },
        );
      },
      loading: () => ListView.builder(
        padding: const EdgeInsets.symmetric(horizontal: AppConstants.spacingMd, vertical: AppConstants.spacingSm),
        itemCount: 4,
        itemBuilder: (_, __) => Card(
          margin: const EdgeInsets.only(bottom: AppConstants.spacingSm),
          child: Padding(
            padding: const EdgeInsets.all(AppConstants.spacingMd),
            child: Row(
              children: [
                ShimmerBox(
                  width: AppConstants.avatarSizeSm,
                  height: AppConstants.avatarSizeSm,
                  borderRadius: BorderRadius.circular(AppConstants.avatarSizeSm / 2),
                ),
                const SizedBox(width: AppConstants.spacingMd),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      ShimmerBox(width: double.infinity, height: 16, borderRadius: BorderRadius.circular(4)),
                      const SizedBox(height: 6),
                      ShimmerBox(width: 120, height: 12, borderRadius: BorderRadius.circular(4)),
                    ],
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
      error: (err, _) => Center(
        child: Padding(
          padding: const EdgeInsets.all(AppConstants.spacingLg),
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              Icon(Icons.wifi_off, size: AppConstants.iconSizeLg, color: Theme.of(context).colorScheme.error),
              const SizedBox(height: AppConstants.spacingMd),
              Text(err is ApiException ? (err as ApiException).userMessage : err.toString().replaceFirst('ApiException: ', ''), textAlign: TextAlign.center, style: Theme.of(context).textTheme.bodyMedium),
            ],
          ),
        ),
      ),
    );
  }
}
