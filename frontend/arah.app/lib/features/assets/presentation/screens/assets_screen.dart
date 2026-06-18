import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../core/providers/territory_provider.dart';
import '../../../../core/widgets/app_snackbar.dart';
import '../../../../core/widgets/arah_list_skeleton.dart';
import '../../../../core/widgets/arah_scaffold.dart';
import '../../../../l10n/app_localizations.dart';
import '../../../territories/presentation/widgets/territory_indicator_bar.dart';
import '../../data/models/asset_item.dart';
import '../providers/assets_provider.dart';

class AssetsScreen extends ConsumerWidget {
  const AssetsScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final l10n = AppLocalizations.of(context)!;
    final territoryId = ref.watch(selectedTerritoryIdValueProvider);
    final state = ref.watch(assetsProvider);
    final notifier = ref.read(assetsProvider.notifier);

    if (territoryId == null || territoryId.isEmpty) {
      return ArahScaffold(
        appBar: AppBar(title: Text(l10n.assetsTitle)),
        body: Center(child: Text(l10n.chooseTerritoryFirst)),
      );
    }

    return ArahScaffold(
      appBar: AppBar(title: Text(l10n.assetsTitle)),
      floatingActionButton: FloatingActionButton(
        onPressed: () => _showCreateDialog(context, ref),
        child: const Icon(Icons.add),
      ),
      body: Column(
        children: [
          const TerritoryIndicatorBar(),
          Expanded(
            child: RefreshIndicator(
              onRefresh: () => notifier.refresh(),
              child: _buildBody(context, ref, state),
            ),
          ),
        ],
      ),
    );
  }

  Future<void> _showCreateDialog(BuildContext context, WidgetRef ref) async {
    final l10n = AppLocalizations.of(context)!;
    final nameController = TextEditingController();
    final typeController = TextEditingController(text: 'Infrastructure');
    await showDialog<void>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: Text(l10n.newAsset),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            TextField(controller: nameController, decoration: InputDecoration(labelText: l10n.assetName)),
            TextField(controller: typeController, decoration: InputDecoration(labelText: l10n.assetType)),
          ],
        ),
        actions: [
          TextButton(onPressed: () => Navigator.pop(ctx), child: Text(l10n.cancel)),
          FilledButton(
            onPressed: () async {
              try {
                await ref.read(assetsProvider.notifier).createAsset(
                      name: nameController.text.trim(),
                      type: typeController.text.trim(),
                    );
                if (ctx.mounted) {
                  Navigator.pop(ctx);
                  showSuccessSnackBar(ctx, l10n.assetCreated);
                }
              } catch (e) {
                if (ctx.mounted) {
                  showErrorSnackBar(
                    ctx,
                    e is ApiException ? e.userMessage : l10n.errorCreateAsset,
                  );
                }
              }
            },
            child: Text(l10n.create),
          ),
        ],
      ),
    );
  }

  Widget _buildBody(BuildContext context, WidgetRef ref, AssetsState state) {
    final l10n = AppLocalizations.of(context)!;
    if (state.isLoading && state.items.isEmpty) {
      return ListView(
        physics: AlwaysScrollableScrollPhysics(),
        children: const [ArahListSkeleton()],
      );
    }
    if (state.error != null && state.items.isEmpty) {
      return ListView(
        physics: const AlwaysScrollableScrollPhysics(),
        children: [
          Padding(
            padding: const EdgeInsets.all(AppConstants.spacingLg),
            child: Text(
              state.error is ApiException
                  ? (state.error as ApiException).userMessage
                  : l10n.errorLoadAssets,
              textAlign: TextAlign.center,
            ),
          ),
        ],
      );
    }
    if (state.items.isEmpty) {
      return ListView(
        physics: AlwaysScrollableScrollPhysics(),
        children: [SizedBox(height: 200, child: Center(child: Text(l10n.noAssetsRegistered)))],
      );
    }
    return ListView.builder(
      physics: const AlwaysScrollableScrollPhysics(),
      padding: const EdgeInsets.all(AppConstants.spacingMd),
      itemCount: state.items.length,
      itemBuilder: (context, index) {
        final asset = state.items[index];
        return Card(
          child: ListTile(
            title: Text(asset.name),
            subtitle: Text(
              '${asset.type} · ${asset.status}'
              '${asset.validationsCount > 0 ? ' · ${l10n.assetValidationsMeta(asset.validationsCount, asset.validationPct.toStringAsFixed(0))}' : ''}',
            ),
            trailing: PopupMenuButton<String>(
              onSelected: (value) => _onAssetAction(context, ref, asset, value),
              itemBuilder: (context) => [
                if (asset.canValidate)
                  PopupMenuItem(value: 'validate', child: Text(l10n.validate)),
                if (asset.canArchive)
                  PopupMenuItem(value: 'archive', child: Text(l10n.archive)),
                if (asset.canCurate) ...[
                  PopupMenuItem(value: 'approve', child: Text(l10n.approveCurator)),
                  PopupMenuItem(value: 'reject', child: Text(l10n.rejectCurator)),
                ],
              ],
            ),
          ),
        );
      },
    );
  }

  Future<void> _onAssetAction(
    BuildContext context,
    WidgetRef ref,
    AssetItem asset,
    String action,
  ) async {
    final l10n = AppLocalizations.of(context)!;
    final notifier = ref.read(assetsProvider.notifier);
    try {
      if (action == 'validate') {
        final result = await notifier.validateAsset(asset.id);
        if (context.mounted) {
          showSuccessSnackBar(
            context,
            l10n.validationRegistered(result.validationPct.toStringAsFixed(0)),
          );
        }
      } else if (action == 'archive') {
        await notifier.archiveAsset(asset.id);
        if (context.mounted) showSuccessSnackBar(context, l10n.assetArchived);
      } else if (action == 'approve') {
        await notifier.curateAsset(asset.id, outcome: 'APPROVED');
        if (context.mounted) showSuccessSnackBar(context, l10n.assetApproved);
      } else if (action == 'reject') {
        await notifier.curateAsset(asset.id, outcome: 'REJECTED');
        if (context.mounted) showSuccessSnackBar(context, l10n.assetRejected);
      }
    } catch (e) {
      if (context.mounted) {
        showErrorSnackBar(
          context,
          e is ApiException ? e.userMessage : l10n.errorCompleteAction,
        );
      }
    }
  }
}
