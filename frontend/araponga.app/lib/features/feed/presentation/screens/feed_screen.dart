import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../core/widgets/shimmer_skeleton.dart';
import '../../../../core/providers/territory_provider.dart';
import '../../../../l10n/app_localizations.dart';
import '../../../territories/presentation/widgets/territory_selector.dart';
import '../providers/feed_provider.dart';

/// Feed da região. Sem território: mostra seletor. Com território: feed BFF com paginação e pull-to-refresh.
class FeedScreen extends ConsumerWidget {
  const FeedScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final territoryId = ref.watch(selectedTerritoryIdValueProvider);
    final feedState = ref.watch(feedNotifierProvider(territoryId));
    final notifier = ref.read(feedNotifierProvider(territoryId).notifier);

    if (territoryId == null || territoryId.isEmpty) {
      return Scaffold(
        appBar: AppBar(title: const Text('Início')),
        body: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Padding(
              padding: const EdgeInsets.all(AppConstants.spacingMd),
              child: Text(
                'Escolha um território para ver o feed da região',
                style: Theme.of(context).textTheme.titleMedium?.copyWith(
                      color: Theme.of(context).colorScheme.onSurfaceVariant,
                    ),
              ),
            ),
            Expanded(child: TerritorySelector()),
          ],
        ),
      );
    }

    return Scaffold(
      appBar: AppBar(
        title: Text(AppLocalizations.of(context)!.home),
        actions: [
          IconButton(icon: const Icon(Icons.favorite_border), onPressed: () {}),
          IconButton(icon: const Icon(Icons.chat_bubble_outline), onPressed: () {}),
        ],
      ),
      body: RefreshIndicator(
        onRefresh: () => notifier.refresh(),
        child: _FeedBody(
          state: feedState,
          onRetry: () => notifier.refresh(),
          onLoadMore: () => notifier.loadMore(),
        ),
      ),
    );
  }
}

class _FeedBody extends StatelessWidget {
  const _FeedBody({
    required this.state,
    required this.onRetry,
    required this.onLoadMore,
  });

  final FeedState state;
  final VoidCallback onRetry;
  final VoidCallback onLoadMore;

  @override
  Widget build(BuildContext context) {
    if (state.error != null && state.items.isEmpty) {
      return _ErrorView(error: state.error!, onRetry: onRetry);
    }
    if (state.isLoading && state.items.isEmpty) {
      return const SingleChildScrollView(
        physics: AlwaysScrollableScrollPhysics(),
        child: FeedSkeleton(itemCount: 5),
      );
    }
    return _FeedList(
      items: state.items,
      hasMore: state.hasMore,
      isLoadingMore: state.isLoading,
      onLoadMore: onLoadMore,
      onRetry: onRetry,
    );
  }
}

class _FeedList extends StatelessWidget {
  const _FeedList({
    required this.items,
    required this.onRetry,
    this.hasMore = false,
    this.isLoadingMore = false,
    this.onLoadMore,
  });

  final List<dynamic> items;
  final VoidCallback onRetry;
  final bool hasMore;
  final bool isLoadingMore;
  final VoidCallback? onLoadMore;

  @override
  Widget build(BuildContext context) {
    if (items.isEmpty) {
      return ListView(
        physics: const AlwaysScrollableScrollPhysics(),
        children: [
          SizedBox(
            height: MediaQuery.of(context).size.height * 0.6,
            child: Center(
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Icon(
                    Icons.article_outlined,
                    size: AppConstants.avatarSizeLg,
                    color: Theme.of(context).colorScheme.primary.withOpacity(0.5),
                  ),
                  const SizedBox(height: AppConstants.spacingMd),
                  Text(
                    'Nenhum post nesta região',
                    style: Theme.of(context).textTheme.titleMedium,
                  ),
                  const SizedBox(height: AppConstants.spacingSm),
                  Text(
                    'Seja o primeiro a publicar aqui.',
                    style: Theme.of(context).textTheme.bodySmall?.copyWith(
                          color: Theme.of(context).colorScheme.onSurfaceVariant,
                        ),
                  ),
                ],
              ),
            ),
          ),
        ],
      );
    }
    return ListView.builder(
      itemCount: items.length + (hasMore || isLoadingMore ? 1 : 0),
      itemBuilder: (context, index) {
        if (index >= items.length) {
          if (isLoadingMore) {
            return const Padding(
              padding: EdgeInsets.all(AppConstants.spacingLg),
              child: Center(child: CircularProgressIndicator()),
            );
          }
          return Padding(
            padding: const EdgeInsets.all(AppConstants.spacingLg),
            child: Center(
              child: TextButton(
                onPressed: onLoadMore,
                child: Text(AppLocalizations.of(context)!.loadMore),
              ),
            ),
          );
        }
        final item = items[index] as Map<String, dynamic>?;
        final post = item?['post'] as Map<String, dynamic>?;
        final title = post?['title']?.toString() ?? 'Post';
        final content = post?['content']?.toString() ?? '';
        return TweenAnimationBuilder<double>(
          key: ValueKey(post?['id'] ?? index),
          tween: Tween(begin: 0, end: 1),
          duration: const Duration(milliseconds: AppConstants.animationNormal),
          curve: Curves.easeOut,
          builder: (context, value, child) => Opacity(
            opacity: value,
            child: Transform.translate(
              offset: Offset(0, 8 * (1 - value)),
              child: child,
            ),
          ),
          child: Card(
            margin: const EdgeInsets.symmetric(
              horizontal: AppConstants.spacingMd,
              vertical: AppConstants.spacingSm + 2,
            ),
            child: Padding(
              padding: const EdgeInsets.all(AppConstants.spacingMd),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                Row(
                  children: [
                    CircleAvatar(
                      backgroundColor: Theme.of(context).colorScheme.primaryContainer,
                      child: Text(
                        (title.isNotEmpty ? title[0] : '?').toUpperCase(),
                        style: TextStyle(color: Theme.of(context).colorScheme.onPrimaryContainer),
                      ),
                    ),
                    const SizedBox(width: AppConstants.radiusMd),
                    Expanded(
                      child: Text(title, style: Theme.of(context).textTheme.titleSmall),
                    ),
                    IconButton(icon: const Icon(Icons.more_horiz), onPressed: () {}),
                  ],
                ),
                if (content.isNotEmpty) ...[
                  const SizedBox(height: AppConstants.spacingMd - 4),
                  Text(
                    content,
                    style: Theme.of(context).textTheme.bodyMedium,
                    maxLines: 4,
                    overflow: TextOverflow.ellipsis,
                  ),
                ],
                const SizedBox(height: AppConstants.spacingMd - 4),
                Row(
                  children: [
                    IconButton(icon: const Icon(Icons.favorite_border), onPressed: () {}),
                    IconButton(icon: const Icon(Icons.chat_bubble_outline), onPressed: () {}),
                    IconButton(icon: const Icon(Icons.send_outlined), onPressed: () {}),
                  ],
                ),
              ],
              ),
            ),
          ),
        );
      },
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
    final msg = error is ApiException ? (error as ApiException).userMessage : l10n.errorLoad;
    return Center(
      child: Padding(
        padding: const EdgeInsets.all(AppConstants.spacingLg),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(Icons.wifi_off, size: AppConstants.iconSizeLg, color: Theme.of(context).colorScheme.error),
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
