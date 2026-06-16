import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:intl/intl.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../core/providers/territory_provider.dart';
import '../../../territories/presentation/widgets/territory_indicator_bar.dart';
import '../providers/alerts_provider.dart';

/// Alertas de saúde/comunidade do território (requer morador ou curador na API).
class AlertsScreen extends ConsumerWidget {
  const AlertsScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final territoryId = ref.watch(selectedTerritoryIdValueProvider);
    final state = ref.watch(alertsProvider);
    final notifier = ref.read(alertsProvider.notifier);

    if (territoryId == null || territoryId.isEmpty) {
      return Scaffold(
        appBar: AppBar(title: const Text('Alertas')),
        body: const Center(child: Text('Escolha um território para ver alertas.')),
      );
    }

    return Scaffold(
      appBar: AppBar(title: const Text('Alertas')),
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

  Widget _buildBody(BuildContext context, AlertsState state, AlertsNotifier notifier) {
    if (state.isLoading && state.items.isEmpty) {
      return const ListView(
        physics: AlwaysScrollableScrollPhysics(),
        children: [SizedBox(height: 240, child: Center(child: CircularProgressIndicator()))],
      );
    }

    if (state.error != null && state.items.isEmpty) {
      final message = state.error is ApiException
          ? (state.error as ApiException).userMessage
          : 'Não foi possível carregar alertas.';
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
                  'Alertas do território exigem residência ou curadoria.',
                  textAlign: TextAlign.center,
                  style: Theme.of(context).textTheme.bodySmall?.copyWith(
                        color: Theme.of(context).colorScheme.onSurfaceVariant,
                      ),
                ),
                const SizedBox(height: AppConstants.spacingMd),
                FilledButton.tonal(onPressed: () => notifier.refresh(), child: const Text('Tentar novamente')),
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
                  Text('Nenhum alerta ativo', style: Theme.of(context).textTheme.titleMedium),
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
