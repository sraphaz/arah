import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:intl/intl.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../core/widgets/app_snackbar.dart';
import '../../../../l10n/app_localizations.dart';
import '../../../../core/providers/territory_provider.dart';
import '../../data/models/event_item.dart';
import '../providers/territory_events_provider.dart';

/// Lista de eventos do território (BFF events/territory-events). Pull-to-refresh e carregar mais.
class EventsScreen extends ConsumerWidget {
  const EventsScreen({super.key, this.territoryId});

  final String? territoryId;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final effectiveTerritoryId =
        territoryId ?? ref.watch(selectedTerritoryIdValueProvider);
    final state = ref.watch(territoryEventsProvider(effectiveTerritoryId));
    final notifier = ref.read(territoryEventsProvider(effectiveTerritoryId).notifier);

    final hasTerritory = effectiveTerritoryId != null && effectiveTerritoryId.isNotEmpty;

    return Scaffold(
      appBar: AppBar(
        title: Text(AppLocalizations.of(context)!.events),
      ),
      body: !hasTerritory
          ? Center(
              child: Padding(
                padding: const EdgeInsets.all(AppConstants.spacingLg),
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    Icon(
                      Icons.event_busy,
                      size: AppConstants.iconSizeLg,
                      color: Theme.of(context).colorScheme.primary.withOpacity(0.5),
                    ),
                    const SizedBox(height: AppConstants.spacingMd),
                    Text(
                      AppLocalizations.of(context)!.chooseTerritory,
                      textAlign: TextAlign.center,
                      style: Theme.of(context).textTheme.titleMedium,
                    ),
                  ],
                ),
              ),
            )
          : RefreshIndicator(
              onRefresh: () => notifier.refresh(),
              child: _buildBody(context, state, notifier),
            ),
    );
  }

  Widget _buildBody(
    BuildContext context,
    TerritoryEventsState state,
    TerritoryEventsNotifier notifier,
  ) {
    if (state.error != null && state.items.isEmpty) {
      return _ErrorView(
        error: state.error!,
        onRetry: () => notifier.refresh(),
      );
    }
    if (state.isLoading && state.items.isEmpty) {
      return const SingleChildScrollView(
        physics: AlwaysScrollableScrollPhysics(),
        child: Padding(
          padding: EdgeInsets.all(AppConstants.spacingMd),
          child: Column(
            children: [
              _EventCardSkeleton(),
              _EventCardSkeleton(),
            ],
          ),
        ),
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
                    Icons.event_available,
                    size: AppConstants.iconSizeLg,
                    color: Theme.of(context).colorScheme.primary.withOpacity(0.5),
                  ),
                  const SizedBox(height: AppConstants.spacingMd),
                  Text(
                    AppLocalizations.of(context)!.noEvents,
                    style: Theme.of(context).textTheme.titleMedium,
                    textAlign: TextAlign.center,
                  ),
                ],
              ),
            ),
          ),
        ],
      );
    }

    return ListView.builder(
      physics: const AlwaysScrollableScrollPhysics(),
      padding: const EdgeInsets.symmetric(horizontal: AppConstants.spacingMd, vertical: AppConstants.spacingSm),
      itemCount: state.items.length + (state.hasMore ? 1 : 0),
      itemBuilder: (context, index) {
        if (index >= state.items.length) {
          if (state.isLoading) {
            return const Padding(
              padding: EdgeInsets.all(AppConstants.spacingLg),
              child: Center(child: CircularProgressIndicator()),
            );
          }
          return Padding(
            padding: const EdgeInsets.all(AppConstants.spacingLg),
            child: Center(
              child: TextButton(
                onPressed: () => notifier.loadMore(),
                child: Text(AppLocalizations.of(context)!.loadMore),
              ),
            ),
          );
        }
        return _EventCard(
          item: state.items[index],
          onParticipate: (item, status) {
            notifier.participate(item, status).catchError((e) {
              if (context.mounted) {
                final msg = e is ApiException ? e.userMessage : AppLocalizations.of(context)!.errorLoad;
                showErrorSnackBar(context, msg);
              }
            });
          },
        );
      },
    );
  }
}

class _EventCard extends StatelessWidget {
  const _EventCard({required this.item, required this.onParticipate});

  final EventItem item;
  final void Function(EventItem item, String status) onParticipate;

  @override
  Widget build(BuildContext context) {
    final dateFormat = DateFormat.yMMMd();
    final timeFormat = DateFormat.Hm();
    final startStr = '${dateFormat.format(item.startsAtUtc)} ${timeFormat.format(item.startsAtUtc)}';

    return Card(
      margin: const EdgeInsets.only(bottom: AppConstants.spacingMd),
      child: Padding(
        padding: const EdgeInsets.all(AppConstants.spacingMd),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Icon(
                  Icons.event,
                  color: Theme.of(context).colorScheme.primary,
                  size: AppConstants.iconSizeMd,
                ),
                const SizedBox(width: AppConstants.spacingSm),
                Expanded(
                  child: Text(
                    item.title,
                    style: Theme.of(context).textTheme.titleMedium?.copyWith(
                          fontWeight: FontWeight.w600,
                        ),
                  ),
                ),
              ],
            ),
            if (item.description != null && item.description!.isNotEmpty) ...[
              const SizedBox(height: AppConstants.spacingSm),
              Text(
                item.description!,
                style: Theme.of(context).textTheme.bodyMedium,
                maxLines: 2,
                overflow: TextOverflow.ellipsis,
              ),
            ],
            const SizedBox(height: AppConstants.spacingSm),
            Text(
              startStr,
              style: Theme.of(context).textTheme.bodySmall?.copyWith(
                    color: Theme.of(context).colorScheme.onSurfaceVariant,
                  ),
            ),
            if (item.locationLabel != null && item.locationLabel!.isNotEmpty)
              Text(
                item.locationLabel!,
                style: Theme.of(context).textTheme.bodySmall?.copyWith(
                      color: Theme.of(context).colorScheme.onSurfaceVariant,
                    ),
              ),
            if (item.participants != null) ...[
              const SizedBox(height: AppConstants.spacingXs),
              Row(
                children: [
                  Icon(Icons.people_outline, size: 14, color: Theme.of(context).colorScheme.onSurfaceVariant),
                  const SizedBox(width: 4),
                  Text(
                    '${item.participants!.interestedCount} interessados · ${item.participants!.confirmedCount} confirmados',
                    style: Theme.of(context).textTheme.bodySmall?.copyWith(
                          color: Theme.of(context).colorScheme.onSurfaceVariant,
                        ),
                  ),
                ],
              ),
            ],
            const SizedBox(height: AppConstants.spacingSm),
            Wrap(
              spacing: AppConstants.spacingSm,
              runSpacing: AppConstants.spacingXs,
              children: [
                if (item.userParticipationStatus != 'CONFIRMED') ...[
                  if (item.userParticipationStatus != 'INTERESTED')
                    FilledButton.tonal(
                      onPressed: () => onParticipate(item, 'INTERESTED'),
                      child: Text(AppLocalizations.of(context)!.eventInterested),
                    ),
                  FilledButton(
                    onPressed: () => onParticipate(item, 'CONFIRMED'),
                    child: Text(AppLocalizations.of(context)!.eventConfirm),
                  ),
                ],
                if (item.userParticipationStatus == 'INTERESTED')
                  Icon(Icons.star_border, size: 16, color: Theme.of(context).colorScheme.primary),
                if (item.userParticipationStatus == 'CONFIRMED')
                  Icon(Icons.check_circle, size: 16, color: Theme.of(context).colorScheme.primary),
              ],
            ),
          ],
        ),
      ),
    );
  }
}

class _EventCardSkeleton extends StatelessWidget {
  const _EventCardSkeleton();

  @override
  Widget build(BuildContext context) {
    return Card(
      margin: const EdgeInsets.only(bottom: AppConstants.spacingMd),
      child: const Padding(
        padding: EdgeInsets.all(AppConstants.spacingMd),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            SizedBox(height: 20, width: double.infinity),
            SizedBox(height: 8),
            SizedBox(height: 12, width: 180),
            SizedBox(height: 4),
            SizedBox(height: 10, width: 120),
          ],
        ),
      ),
    );
  }
}

class _ErrorView extends StatelessWidget {
  const _ErrorView({required this.error, required this.onRetry});

  final Object error;
  final VoidCallback onRetry;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final msg = error is ApiException
        ? (error as ApiException).userMessage
        : l10n.errorLoad;
    return Center(
      child: Padding(
        padding: const EdgeInsets.all(AppConstants.spacingLg),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(
              Icons.wifi_off,
              size: AppConstants.iconSizeLg,
              color: Theme.of(context).colorScheme.error,
            ),
            const SizedBox(height: AppConstants.spacingMd),
            Text(msg, textAlign: TextAlign.center, style: Theme.of(context).textTheme.bodyLarge),
            const SizedBox(height: AppConstants.spacingMd),
            FilledButton.tonal(onPressed: onRetry, child: Text(l10n.tryAgain)),
          ],
        ),
      ),
    );
  }
}
