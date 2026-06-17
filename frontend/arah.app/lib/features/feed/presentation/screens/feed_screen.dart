import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../core/widgets/shimmer_skeleton.dart';
import '../../../../core/providers/territory_provider.dart';
import '../../../../core/widgets/arah_scaffold.dart';
import '../../../../l10n/app_localizations.dart';
import '../../../territories/presentation/widgets/territory_indicator_bar.dart';
import '../../../territories/presentation/widgets/territory_selector.dart';
import '../../domain/feed_interaction.dart';
import '../providers/feed_provider.dart';
import '../widgets/feed_post_card.dart';
import '../widgets/feed_comments_sheet.dart';

/// Feed da região. Sem território: mostra seletor. Com território: feed BFF com paginação, pull-to-refresh e scroll infinito.
class FeedScreen extends ConsumerStatefulWidget {
  const FeedScreen({super.key});

  @override
  ConsumerState<FeedScreen> createState() => _FeedScreenState();
}

class _FeedScreenState extends ConsumerState<FeedScreen> {
  final ScrollController _scrollController = ScrollController();
  static const double _loadMoreThreshold = 300;

  @override
  void dispose() {
    _scrollController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final territoryId = ref.watch(selectedTerritoryIdValueProvider);
    final feedState = ref.watch(feedNotifierProvider(territoryId));
    final notifier = ref.read(feedNotifierProvider(territoryId).notifier);
    final filterByInterests = ref.watch(filterFeedByInterestsProvider);
    final filterType = ref.watch(filterFeedTypeProvider);

    if (territoryId == null || territoryId.isEmpty) {
      return ArahScaffold(
        appBar: AppBar(title: Text(l10n.home)),
        body: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Padding(
              padding: const EdgeInsets.all(AppConstants.spacingMd),
              child: Text(
                l10n.chooseTerritory,
                style: Theme.of(context).textTheme.titleMedium?.copyWith(
                      color: Theme.of(context).colorScheme.onSurfaceVariant,
                    ),
              ),
            ),
            const Expanded(child: TerritorySelector()),
          ],
        ),
      );
    }

    return ArahScaffold(
      appBar: AppBar(
        title: Text(l10n.home),
        actions: [
          IconButton(
            icon: Icon(
              filterByInterests ? Icons.filter_list : Icons.filter_list_off,
              color: filterByInterests ? Theme.of(context).colorScheme.primary : null,
            ),
            tooltip: l10n.filterByInterests,
            onPressed: () {
              ref.read(filterFeedByInterestsProvider.notifier).state = !filterByInterests;
              ref.invalidate(feedNotifierProvider(territoryId));
            },
          ),
        ],
      ),
      body: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          const TerritoryIndicatorBar(),
          _FeedTypeFilterBar(
            selectedType: filterType,
            onTypeSelected: (type) {
              ref.read(filterFeedTypeProvider.notifier).state = type;
            },
          ),
          Expanded(
            child: RefreshIndicator(
              onRefresh: () => notifier.refresh(),
              child: _FeedBody(
                state: feedState,
                territoryId: territoryId,
                filterType: filterType,
                onRetry: () => notifier.refresh(),
                onLoadMore: () => notifier.loadMore(),
                scrollController: _scrollController,
                onScrollNearBottom: () => notifier.loadMore(),
                loadMoreThreshold: _loadMoreThreshold,
              ),
            ),
          ),
        ],
      ),
    );
  }
}

class _FeedBody extends StatelessWidget {
  const _FeedBody({
    required this.state,
    required this.territoryId,
    required this.filterType,
    required this.onRetry,
    required this.onLoadMore,
    this.scrollController,
    this.onScrollNearBottom,
    this.loadMoreThreshold = 300,
  });

  final FeedState state;
  final String territoryId;
  final String? filterType;
  final VoidCallback onRetry;
  final VoidCallback onLoadMore;
  final ScrollController? scrollController;
  final VoidCallback? onScrollNearBottom;
  final double loadMoreThreshold;

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
      territoryId: territoryId,
      filterType: filterType,
      hasMore: state.hasMore,
      isLoadingMore: state.isLoading,
      onLoadMore: onLoadMore,
      onRetry: onRetry,
      scrollController: scrollController,
      onScrollNearBottom: onScrollNearBottom,
      loadMoreThreshold: loadMoreThreshold,
    );
  }
}

class _FeedList extends ConsumerStatefulWidget {
  const _FeedList({
    required this.items,
    required this.territoryId,
    required this.filterType,
    required this.onRetry,
    this.hasMore = false,
    this.isLoadingMore = false,
    this.onLoadMore,
    this.scrollController,
    this.onScrollNearBottom,
    this.loadMoreThreshold = 300,
  });

  final List<dynamic> items;
  final String territoryId;
  final String? filterType;
  final VoidCallback onRetry;
  final bool hasMore;
  final bool isLoadingMore;
  final VoidCallback? onLoadMore;
  final ScrollController? scrollController;
  final VoidCallback? onScrollNearBottom;
  final double loadMoreThreshold;

  @override
  ConsumerState<_FeedList> createState() => _FeedListState();
}

class _FeedListState extends ConsumerState<_FeedList> {
  void _onScroll() {
    final controller = widget.scrollController;
    final onNearBottom = widget.onScrollNearBottom;
    if (controller == null || onNearBottom == null || !controller.hasClients) return;
    final pos = controller.position;
    if (pos.pixels >= pos.maxScrollExtent - widget.loadMoreThreshold) {
      onNearBottom();
    }
  }

  @override
  void initState() {
    super.initState();
    widget.scrollController?.addListener(_onScroll);
  }

  @override
  void didUpdateWidget(covariant _FeedList oldWidget) {
    super.didUpdateWidget(oldWidget);
    if (oldWidget.scrollController != widget.scrollController) {
      oldWidget.scrollController?.removeListener(_onScroll);
      widget.scrollController?.addListener(_onScroll);
    }
  }

  @override
  void dispose() {
    widget.scrollController?.removeListener(_onScroll);
    super.dispose();
  }

  Future<void> _openComments(String postId, String title) async {
    await FeedCommentsSheet.show(
      context,
      postId: postId,
      territoryId: widget.territoryId,
      postTitle: title,
    );
  }

  Future<void> _confirmDelete(String postId) async {
    final l10n = AppLocalizations.of(context)!;
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: Text(l10n.deletePost),
        content: Text(l10n.deletePostConfirm),
        actions: [
          TextButton(onPressed: () => Navigator.pop(ctx, false), child: Text(l10n.cancel)),
          FilledButton(onPressed: () => Navigator.pop(ctx, true), child: Text(l10n.delete)),
        ],
      ),
    );
    if (confirmed != true) return;
    await ref.read(feedNotifierProvider(widget.territoryId).notifier).deletePost(postId);
  }

  void _showPostMenu({required String postId, required bool canDelete}) {
    final l10n = AppLocalizations.of(context)!;
    showModalBottomSheet<void>(
      context: context,
      builder: (ctx) => SafeArea(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            if (canDelete)
              ListTile(
                leading: Icon(Icons.delete_outline, color: Theme.of(ctx).colorScheme.error),
                title: Text(l10n.deletePost),
                onTap: () {
                  Navigator.pop(ctx);
                  _confirmDelete(postId);
                },
              ),
          ],
        ),
      ),
    );
  }

  List<dynamic> _filteredItems() {
    final filterType = widget.filterType;
    if (filterType == null || filterType.isEmpty) return widget.items;
    return widget.items.where((rawItem) {
      final item = rawItem as Map<String, dynamic>;
      final post = item['post'] as Map<String, dynamic>?;
      final type = post?['type']?.toString().toLowerCase();
      return type == filterType.toLowerCase();
    }).toList();
  }

  List<String> _mediaUrls(Map<String, dynamic>? item) {
    final media = item?['media'] as List?;
    if (media == null) return const [];
    return media
        .whereType<Map<String, dynamic>>()
        .map((entry) => entry['url']?.toString())
        .whereType<String>()
        .where((url) => url.isNotEmpty)
        .toList();
  }

  bool _canDelete(Map<String, dynamic>? item) {
    final metadata = item?['metadata'] as Map<String, dynamic>?;
    return metadata?['canDelete'] == true;
  }

  @override
  Widget build(BuildContext context) {
    final items = _filteredItems();
    final hasMore = widget.hasMore;
    final isLoadingMore = widget.isLoadingMore;
    final onLoadMore = widget.onLoadMore;
    if (items.isEmpty) {
      final l10n = AppLocalizations.of(context)!;
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
                    color: Theme.of(context).colorScheme.primary.withValues(alpha: 0.5),
                  ),
                  const SizedBox(height: AppConstants.spacingMd),
                  Text(
                    l10n.noPostsHere,
                    style: Theme.of(context).textTheme.titleMedium,
                  ),
                  const SizedBox(height: AppConstants.spacingSm),
                  Text(
                    l10n.beFirstToPost,
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
      controller: widget.scrollController,
      physics: const AlwaysScrollableScrollPhysics(),
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
        final postId = post?['id']?.toString() ?? '';
        final title = post?['title']?.toString() ?? AppLocalizations.of(context)!.postDefaultTitle;
        final content = post?['content']?.toString() ?? '';
        final postType = post?['type']?.toString();
        final counts = FeedPostCounts.fromJson(item?['counts'] as Map<String, dynamic>?);
        final interactions = FeedUserInteractions.fromJson(
          item?['userInteractions'] as Map<String, dynamic>?,
        );
        final mediaUrls = _mediaUrls(item);
        final canDelete = _canDelete(item);
        final notifier = ref.read(feedNotifierProvider(widget.territoryId).notifier);

        return TweenAnimationBuilder<double>(
          key: ValueKey(postId.isNotEmpty ? postId : index),
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
          child: FeedPostCard(
            title: title,
            content: content,
            type: FeedPostCard.typeFromString(postType),
            likeCount: counts.likes,
            commentCount: counts.comments,
            shareCount: counts.shares,
            isLiked: interactions.liked,
            isShared: interactions.shared,
            mediaUrls: mediaUrls,
            onMorePressed: canDelete ? () => _showPostMenu(postId: postId, canDelete: canDelete) : null,
            onLikePressed: postId.isEmpty
                ? null
                : () => notifier.interact(postId: postId, action: FeedInteractionAction.like),
            onCommentPressed: postId.isEmpty ? null : () => _openComments(postId, title),
            onSharePressed: postId.isEmpty
                ? null
                : () => notifier.interact(postId: postId, action: FeedInteractionAction.share),
          ),
        );
      },
    );
  }
}

class _FeedTypeFilterBar extends StatelessWidget {
  const _FeedTypeFilterBar({
    required this.selectedType,
    required this.onTypeSelected,
  });

  final String? selectedType;
  final ValueChanged<String?> onTypeSelected;

  static Map<String?, String> _options(AppLocalizations l10n) => {
    null: l10n.filterAll,
    'general': l10n.general,
    'alert': l10n.alert,
  };

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final options = _options(l10n);
    return SingleChildScrollView(
      scrollDirection: Axis.horizontal,
      padding: const EdgeInsets.symmetric(
        horizontal: AppConstants.spacingMd,
        vertical: AppConstants.spacingSm,
      ),
      child: Row(
        children: options.entries.map((entry) {
          final selected = selectedType?.toLowerCase() == entry.key?.toLowerCase() ||
              (selectedType == null && entry.key == null);
          return Padding(
            padding: const EdgeInsets.only(right: AppConstants.spacingSm),
            child: FilterChip(
              label: Text(entry.value),
              selected: selected,
              onSelected: (_) => onTypeSelected(entry.key == null ? null : entry.key),
            ),
          );
        }).toList(),
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
