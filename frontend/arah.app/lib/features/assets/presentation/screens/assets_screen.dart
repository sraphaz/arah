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
    var asWaterBody = false;
    var waterSubtype = kWaterBodySubtypeOptions.first;
    var isSubmitting = false;

    try {
      await showDialog<void>(
        context: context,
        builder: (ctx) => StatefulBuilder(
          builder: (ctx, setLocal) => AlertDialog(
            title: Text(asWaterBody ? l10n.newWaterBody : l10n.newAsset),
            content: SingleChildScrollView(
              child: Column(
                mainAxisSize: MainAxisSize.min,
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  Text(
                    asWaterBody ? l10n.waterBodyCreateHint : l10n.assetCreateHint,
                    style: Theme.of(ctx).textTheme.bodySmall?.copyWith(
                          color: Theme.of(ctx).colorScheme.onSurfaceVariant,
                        ),
                  ),
                  const SizedBox(height: AppConstants.spacingMd),
                  SegmentedButton<bool>(
                    segments: [
                      ButtonSegment(value: false, label: Text(l10n.assetKindGeneric)),
                      ButtonSegment(value: true, label: Text(l10n.assetKindWaterBody)),
                    ],
                    selected: {asWaterBody},
                    onSelectionChanged: isSubmitting
                        ? null
                        : (value) {
                            setLocal(() => asWaterBody = value.first);
                          },
                  ),
                  const SizedBox(height: AppConstants.spacingMd),
                  TextField(
                    controller: nameController,
                    enabled: !isSubmitting,
                    decoration: InputDecoration(
                      labelText: asWaterBody ? l10n.waterBodyName : l10n.assetName,
                    ),
                  ),
                  if (asWaterBody) ...[
                    const SizedBox(height: AppConstants.spacingSm),
                    DropdownButtonFormField<String>(
                      key: ValueKey(waterSubtype),
                      initialValue: waterSubtype,
                      decoration: InputDecoration(labelText: l10n.waterBodySubtype),
                      items: kWaterBodySubtypeOptions
                          .map(
                            (s) => DropdownMenuItem(
                              value: s,
                              child: Text(_waterBodyKindLabel(l10n, s)),
                            ),
                          )
                          .toList(),
                      onChanged: isSubmitting
                          ? null
                          : (v) {
                              if (v != null) setLocal(() => waterSubtype = v);
                            },
                    ),
                  ] else
                    TextField(
                      controller: typeController,
                      enabled: !isSubmitting,
                      decoration: InputDecoration(labelText: l10n.assetType),
                    ),
                ],
              ),
            ),
            actions: [
              TextButton(
                onPressed: isSubmitting ? null : () => Navigator.pop(ctx),
                child: Text(l10n.cancel),
              ),
              FilledButton(
                onPressed: isSubmitting
                    ? null
                    : () async {
                        final name = nameController.text.trim();
                        if (name.isEmpty) return;
                        setLocal(() => isSubmitting = true);
                        try {
                          if (asWaterBody) {
                            await ref.read(assetsProvider.notifier).createAsset(
                                  name: name,
                                  type: 'natural',
                                  subtype: waterSubtype,
                                );
                          } else {
                            await ref.read(assetsProvider.notifier).createAsset(
                                  name: name,
                                  type: typeController.text.trim(),
                                );
                          }
                          if (ctx.mounted) {
                            Navigator.pop(ctx);
                            showSuccessSnackBar(
                              ctx,
                              asWaterBody
                                  ? l10n.waterBodySuggested
                                  : l10n.assetCreated,
                            );
                          }
                        } catch (e) {
                          if (ctx.mounted) {
                            setLocal(() => isSubmitting = false);
                            showErrorSnackBar(
                              ctx,
                              e is ApiException
                                  ? e.userMessage
                                  : l10n.errorCreateAsset,
                            );
                          }
                        }
                      },
                child: isSubmitting
                    ? const SizedBox(
                        width: AppConstants.loadingIndicatorSize,
                        height: AppConstants.loadingIndicatorSize,
                        child: CircularProgressIndicator(strokeWidth: 2),
                      )
                    : Text(l10n.create),
              ),
            ],
          ),
        ),
      );
    } finally {
      nameController.dispose();
      typeController.dispose();
    }
  }

  Widget _buildBody(BuildContext context, WidgetRef ref, AssetsState state) {
    final l10n = AppLocalizations.of(context)!;
    if (state.isLoading && state.items.isEmpty) {
      return ListView(
        physics: const AlwaysScrollableScrollPhysics(),
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
        physics: const AlwaysScrollableScrollPhysics(),
        children: [
          Padding(
            padding: const EdgeInsets.symmetric(
              horizontal: AppConstants.spacingLg,
              vertical: AppConstants.spacing3xl,
            ),
            child: Center(
              child: Text(
                l10n.noAssetsOrWaterBodies,
                textAlign: TextAlign.center,
              ),
            ),
          ),
        ],
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
            leading: Icon(
              asset.isWaterBody ? Icons.water_drop_outlined : Icons.place_outlined,
            ),
            title: Text(asset.name),
            subtitle: Text(_assetSubtitle(l10n, asset)),
            trailing: PopupMenuButton<String>(
              onSelected: (value) => _onAssetAction(context, ref, asset, value),
              itemBuilder: (context) => [
                if (asset.canValidate)
                  PopupMenuItem(
                    value: 'validate',
                    child: Text(
                      asset.isWaterBody ? l10n.validateWaterBody : l10n.validate,
                    ),
                  ),
                if (asset.canArchive)
                  PopupMenuItem(value: 'archive', child: Text(l10n.archive)),
                if (asset.canCurate) ...[
                  PopupMenuItem(
                    value: 'approve',
                    child: Text(
                      asset.isWaterBody
                          ? l10n.approveWaterBodyCurator
                          : l10n.approveCurator,
                    ),
                  ),
                  PopupMenuItem(
                    value: 'reject',
                    child: Text(
                      asset.isWaterBody
                          ? l10n.rejectWaterBodyCurator
                          : l10n.rejectCurator,
                    ),
                  ),
                ],
              ],
            ),
          ),
        );
      },
    );
  }

  String _assetSubtitle(AppLocalizations l10n, AssetItem asset) {
    final kind = asset.isWaterBody
        ? _waterBodyKindLabel(l10n, asset.waterBodyKind ?? '')
        : asset.type;
    final base = '$kind · ${_localizedAssetStatus(l10n, asset.status)}';
    if (asset.validationsCount <= 0) return base;
    return '$base · ${l10n.assetValidationsMeta(asset.validationsCount, asset.validationPct.toStringAsFixed(0))}';
  }

  String _localizedAssetStatus(AppLocalizations l10n, String status) {
    switch (status.toUpperCase()) {
      case 'SUGGESTED':
        return l10n.assetStatusSuggested;
      case 'ACTIVE':
        return l10n.assetStatusActive;
      case 'ARCHIVED':
        return l10n.assetStatusArchived;
      case 'REJECTED':
        return l10n.assetStatusRejected;
      default:
        return status;
    }
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
        await notifier.curateAsset(asset.id, outcome: 'Approved');
        if (context.mounted) {
          showSuccessSnackBar(
            context,
            asset.isWaterBody ? l10n.waterBodyApproved : l10n.assetApproved,
          );
        }
      } else if (action == 'reject') {
        await notifier.curateAsset(asset.id, outcome: 'Rejected');
        if (context.mounted) {
          showSuccessSnackBar(
            context,
            asset.isWaterBody ? l10n.waterBodyRejected : l10n.assetRejected,
          );
        }
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

String _waterBodyKindLabel(AppLocalizations l10n, String kind) {
  switch (kind.toLowerCase()) {
    case 'river':
      return l10n.waterBodyRiver;
    case 'stream':
      return l10n.waterBodyStream;
    case 'spring':
      return l10n.waterBodySpring;
    case 'waterfall':
      return l10n.waterBodyWaterfall;
    case 'well':
      return l10n.waterBodyWell;
    case 'potable_water':
      return l10n.waterBodyPotableWater;
    default:
      return l10n.mapFilterWaterBodies;
  }
}
