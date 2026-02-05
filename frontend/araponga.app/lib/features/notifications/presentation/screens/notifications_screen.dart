import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:intl/intl.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../l10n/app_localizations.dart';
import '../../data/models/notification_item.dart';
import '../providers/notifications_provider.dart';

/// Lista de notificações in-app (BFF notifications). Pull-to-refresh e marcar como lida.
class NotificationsScreen extends ConsumerWidget {
  const NotificationsScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final state = ref.watch(notificationsProvider);
    final notifier = ref.read(notificationsProvider.notifier);

    return Scaffold(
      appBar: AppBar(
        title: Text(AppLocalizations.of(context)!.notifications),
      ),
      body: RefreshIndicator(
        onRefresh: () => notifier.refresh(),
        child: _buildBody(context, state, notifier),
      ),
    );
  }

  Widget _buildBody(
    BuildContext context,
    NotificationsState state,
    NotificationsNotifier notifier,
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
              _NotificationTileSkeleton(),
              _NotificationTileSkeleton(),
              _NotificationTileSkeleton(),
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
                    Icons.notifications_none,
                    size: AppConstants.iconSizeLg,
                    color: Theme.of(context).colorScheme.primary.withOpacity(0.5),
                  ),
                  const SizedBox(height: AppConstants.spacingMd),
                  Text(
                    AppLocalizations.of(context)!.noNotifications,
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
        final item = state.items[index];
        return _NotificationTile(
          item: item,
          onTap: () => notifier.markAsRead(item),
        );
      },
    );
  }
}

class _NotificationTile extends StatelessWidget {
  const _NotificationTile({required this.item, required this.onTap});

  final NotificationItem item;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final dateStr = DateFormat.yMMMd().add_Hm().format(item.createdAtUtc);

    return ListTile(
      leading: CircleAvatar(
        backgroundColor: item.isRead
            ? Theme.of(context).colorScheme.surfaceContainerHighest
            : Theme.of(context).colorScheme.primaryContainer,
        child: Icon(
          _iconForKind(item.kind),
          color: item.isRead
              ? Theme.of(context).colorScheme.onSurfaceVariant
              : Theme.of(context).colorScheme.onPrimaryContainer,
          size: AppConstants.iconSizeMd,
        ),
      ),
      title: Text(
        item.title,
        style: TextStyle(
          fontWeight: item.isRead ? FontWeight.normal : FontWeight.w600,
        ),
      ),
      subtitle: item.body != null && item.body!.isNotEmpty
          ? Text(
              item.body!,
              maxLines: 2,
              overflow: TextOverflow.ellipsis,
            )
          : Text(
              dateStr,
              style: Theme.of(context).textTheme.bodySmall?.copyWith(
                    color: Theme.of(context).colorScheme.onSurfaceVariant,
                  ),
            ),
      isThreeLine: item.body != null && item.body!.isNotEmpty,
      onTap: onTap,
    );
  }

  IconData _iconForKind(String kind) {
    switch (kind.toLowerCase()) {
      case 'alert':
        return Icons.warning_amber;
      case 'post':
      case 'feed':
        return Icons.article;
      case 'event':
        return Icons.event;
      case 'connection':
        return Icons.people;
      default:
        return Icons.notifications;
    }
  }
}

class _NotificationTileSkeleton extends StatelessWidget {
  const _NotificationTileSkeleton();

  @override
  Widget build(BuildContext context) {
    return const Padding(
      padding: EdgeInsets.symmetric(vertical: AppConstants.spacingSm),
      child: Row(
        children: [
          CircleAvatar(radius: 20),
          SizedBox(width: AppConstants.spacingMd),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                SizedBox(height: 8, width: double.infinity),
                SizedBox(height: 4),
                SizedBox(height: 4, width: 120),
              ],
            ),
          ),
        ],
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
