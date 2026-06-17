import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:intl/intl.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../core/providers/territory_provider.dart';
import '../../../../core/widgets/app_snackbar.dart';
import '../../../../core/widgets/arah_scaffold.dart';
import '../../../../l10n/app_localizations.dart';
import '../../../territories/presentation/widgets/territory_indicator_bar.dart';
import '../providers/alerts_provider.dart';

/// Alertas de saúde/comunidade do território (requer morador ou curador na API).
class AlertsScreen extends ConsumerStatefulWidget {
  const AlertsScreen({super.key});

  @override
  ConsumerState<AlertsScreen> createState() => _AlertsScreenState();
}

class _AlertsScreenState extends ConsumerState<AlertsScreen> {
  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final territoryId = ref.watch(selectedTerritoryIdValueProvider);
    final state = ref.watch(alertsProvider);
    final notifier = ref.read(alertsProvider.notifier);

    if (territoryId == null || territoryId.isEmpty) {
      return ArahScaffold(
        appBar: AppBar(title: Text(l10n.alertsTitle)),
        body: Center(child: Text(l10n.chooseTerritoryForAlerts)),
      );
    }

    return ArahScaffold(
      appBar: AppBar(title: Text(l10n.alertsTitle)),
      floatingActionButton: FloatingActionButton(
        onPressed: () => _showCreateDialog(context),
        child: const Icon(Icons.add),
      ),
      body: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          const TerritoryIndicatorBar(),
          Expanded(
            child: RefreshIndicator(
              onRefresh: () => notifier.refresh(),
              child: _buildBody(context, state, notifier),
            ),
          ),
        ],
      ),
    );
  }

  Future<void> _showCreateDialog(BuildContext context) async {
    final l10n = AppLocalizations.of(context)!;
    final titleController = TextEditingController();
    final descController = TextEditingController();
    await showDialog<void>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: Text(l10n.reportAlert),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            TextField(
              controller: titleController,
              decoration: InputDecoration(labelText: l10n.title),
            ),
            TextField(
              controller: descController,
              decoration: InputDecoration(labelText: l10n.descriptionLabel),
              maxLines: 3,
            ),
          ],
        ),
        actions: [
          TextButton(onPressed: () => Navigator.pop(ctx), child: Text(l10n.cancel)),
          FilledButton(
            onPressed: () async {
              try {
                await ref.read(alertsProvider.notifier).createAlert(
                      title: titleController.text.trim(),
                      description: descController.text.trim(),
                    );
                if (ctx.mounted) {
                  Navigator.pop(ctx);
                  showSuccessSnackBar(ctx, l10n.alertCreated);
                }
              } catch (e) {
                if (ctx.mounted) {
                  showErrorSnackBar(
                    ctx,
                    e is ApiException ? e.userMessage : l10n.errorCreateAlert,
                  );
                }
              }
            },
            child: Text(l10n.send),
          ),
        ],
      ),
    );
  }

  Widget _buildBody(BuildContext context, AlertsState state, AlertsNotifier notifier) {
    final l10n = AppLocalizations.of(context)!;
    if (state.isLoading && state.items.isEmpty) {
      return ListView(
        physics: AlwaysScrollableScrollPhysics(),
        children: [SizedBox(height: 240, child: Center(child: CircularProgressIndicator()))],
      );
    }

    if (state.error != null && state.items.isEmpty) {
      final message = state.error is ApiException
          ? (state.error as ApiException).userMessage
          : l10n.errorLoadAlerts;
      return ListView(
        physics: const AlwaysScrollableScrollPhysics(),
        children: [
          Padding(
            padding: const EdgeInsets.all(AppConstants.spacingLg),
            child: Column(
              children: [
                Icon(Icons.lock_outline, size: AppConstants.iconSizeLg, color: Theme.of(context).colorScheme.error),
                const SizedBox(height: AppConstants.spacingMd),
                Text(message, textAlign: TextAlign.center),
                const SizedBox(height: AppConstants.spacingSm),
                Text(
                  l10n.alertsRequireResidency,
                  textAlign: TextAlign.center,
                  style: Theme.of(context).textTheme.bodySmall?.copyWith(
                        color: Theme.of(context).colorScheme.onSurfaceVariant,
                      ),
                ),
                const SizedBox(height: AppConstants.spacingMd),
                FilledButton.tonal(onPressed: () => notifier.refresh(), child: Text(l10n.tryAgain)),
              ],
            ),
          ),
        ],
      );
    }

    if (state.items.isEmpty) {
      return ListView(
        physics: const AlwaysScrollableScrollPhysics(),
        children: [
          SizedBox(
            height: MediaQuery.of(context).size.height * 0.5,
            child: Center(
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Icon(
                    Icons.notifications_active_outlined,
                    size: AppConstants.avatarSizeLg,
                    color: Theme.of(context).colorScheme.primary.withValues(alpha: 0.5),
                  ),
                  const SizedBox(height: AppConstants.spacingMd),
                  Text(l10n.noAlertsActive, style: Theme.of(context).textTheme.titleMedium),
                ],
              ),
            ),
          ),
        ],
      );
    }

    final dateFormat = DateFormat('dd/MM/yyyy HH:mm');

    return ListView.separated(
      physics: const AlwaysScrollableScrollPhysics(),
      padding: const EdgeInsets.all(AppConstants.spacingMd),
      itemCount: state.items.length,
      separatorBuilder: (_, __) => const SizedBox(height: AppConstants.spacingSm),
      itemBuilder: (context, index) {
        final alert = state.items[index];
        return Card(
          child: ListTile(
            leading: Icon(
              Icons.warning_amber_rounded,
              color: Theme.of(context).colorScheme.error,
            ),
            title: Text(alert.title),
            subtitle: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                if (alert.description.isNotEmpty) ...[
                  const SizedBox(height: AppConstants.spacingXs),
                  Text(alert.description),
                ],
                const SizedBox(height: AppConstants.spacingXs),
                Text(
                  '${alert.status} · ${dateFormat.format(alert.createdAtUtc.toLocal())}',
                  style: Theme.of(context).textTheme.labelSmall?.copyWith(
                        color: Theme.of(context).colorScheme.onSurfaceVariant,
                      ),
                ),
              ],
            ),
            isThreeLine: alert.description.isNotEmpty,
          ),
        );
      },
    );
  }
}
