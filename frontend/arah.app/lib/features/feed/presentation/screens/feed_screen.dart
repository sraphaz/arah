import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:intl/intl.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../core/providers/current_territory_name_provider.dart';
import '../../../../core/theme/arah_motion.dart';
import '../../../../core/theme/app_design_tokens.dart';
import '../../../../core/widgets/arah_empty_state.dart';
import '../../../../core/widgets/arah_error_state.dart';
import '../../../../core/widgets/shimmer_skeleton.dart';
import '../../../../core/providers/territory_provider.dart';
import '../../../../l10n/app_localizations.dart';
import '../../../territories/presentation/widgets/territory_selector.dart';
import '../../domain/feed_interaction.dart';
import '../providers/feed_provider.dart';
import '../widgets/feed_post_card.dart';
import '../widgets/feed_comments_sheet.dart';
import '../widgets/visitor_banner.dart';
import 'post_detail_screen.dart';

/// Feed da região. Sem território: mostra seletor. Com território: feed BFF com paginação, pull-to-refresh e scroll infinito.
class FeedScreen extends ConsumerStatefulWidget {
  const FeedScreen({super.key, this.onGoToCreatePost});

  /// Navega para a aba Publicar (shell principal).
  final VoidCallback? onGoToCreatePost;

  @override
  ConsumerState<FeedScreen> createState() => _FeedScreenState();
}

class _FeedScreenState extends ConsumerState<FeedScreen> {
  final ScrollController _scrollController = ScrollController();
  static const double _loadMoreThreshold = 300;
  static const String _neighborhoodFilter = '__public_only__';

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
    final filterType = ref.watch(filterFeedTypeProvider);

    if (territoryId == null || territoryId.isEmpty) {
      return Column(
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
      );
    }

    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Padding(
          padding: const EdgeInsets.fromLTRB(18, 2, 18, 12),
          child: _FeedTypeFilterBar(
            selectedType: filterType,
            onTypeSelected: (type) {
              ref.read(filterFeedTypeProvider.notifier).state = type;
            },
          ),
        ),
        const Padding(
          padding: EdgeInsets.fromLTRB(18, 0, 18, 12),
          child: VisitorBanner(),
        ),
        Expanded(
          child: RefreshIndicator(
            onRefresh: () => notifier.refresh(),
            child: _FeedBody(
              state: feedState,
              territoryId: territoryId,
              filterType: filterType,
              neighborhoodFilterValue: _neighborhoodFilter,
              onRetry: () => notifier.refresh(),
              onLoadMore: () => notifier.loadMore(),
              onGoToCreatePost: widget.onGoToCreatePost,
              scrollController: _scrollController,
              onScrollNearBottom: () => notifier.loadMore(),
              loadMoreThreshold: _loadMoreThreshold,
            ),
          ),
        ),
      ],
    );
  }
}

class _FeedBody extends StatelessWidget {
  const _FeedBody({
    required this.state,
    required this.territoryId,
    required this.filterType,
    required this.neighborhoodFilterValue,
    required this.onRetry,
    required this.onLoadMore,
    this.scrollController,
    this.onScrollNearBottom,
    this.loadMoreThreshold = 300,
    this.onGoToCreatePost,
  });

  final FeedState state;
  final String territoryId;
  final String? filterType;
  final String neighborhoodFilterValue;
  final VoidCallback onRetry;
  final VoidCallback onLoadMore;
  final ScrollController? scrollController;
  final VoidCallback? onScrollNearBottom;
  final double loadMoreThreshold;
  final VoidCallback? onGoToCreatePost;

  @override
  Widget build(BuildContext context) {
    if (state.error != null && state.items.isEmpty) {
      final l10n = AppLocalizations.of(context)!;
      final msg = state.error is ApiException
          ? (state.error as ApiException).userMessage
          : l10n.errorLoad;
      return ArahErrorState(
        message: msg,
        retryLabel: l10n.tryAgain,
        onRetry: onRetry,
      );
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
      neighborhoodFilterValue: neighborhoodFilterValue,
      hasMore: state.hasMore,
      isLoadingMore: state.isLoading,
      onLoadMore: onLoadMore,
      onRetry: onRetry,
      scrollController: scrollController,
      onScrollNearBottom: onScrollNearBottom,
      loadMoreThreshold: loadMoreThreshold,
      onGoToCreatePost: onGoToCreatePost,
    );
  }
}

class _FeedList extends ConsumerStatefulWidget {
  const _FeedList({
    required this.items,
    required this.territoryId,
    required this.filterType,
    required this.neighborhoodFilterValue,
    required this.onRetry,
    this.hasMore = false,
    this.isLoadingMore = false,
    this.onLoadMore,
    this.scrollController,
    this.onScrollNearBottom,
    this.loadMoreThreshold = 300,
    this.onGoToCreatePost,
  });

  final List<dynamic> items;
  final String territoryId;
  final String? filterType;
  final String neighborhoodFilterValue;
  final VoidCallback onRetry;
  final bool hasMore;
  final bool isLoadingMore;
  final VoidCallback? onLoadMore;
  final ScrollController? scrollController;
  final VoidCallback? onScrollNearBottom;
  final double loadMoreThreshold;
  final VoidCallback? onGoToCreatePost;

  @override
  ConsumerState<_FeedList> createState() => _FeedListState();
}

class _FeedListState extends ConsumerState<_FeedList> {
  void _onScroll() {
    final controller = widget.scrollController;
    final onNearBottom = widget.onScrollNearBottom;
    if (controller == null || onNearBottom == null || !controller.hasClients)
      return;
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

  void _openDetail(String postId) {
    Navigator.of(context).push(
      MaterialPageRoute(
        builder: (_) =>
            PostDetailScreen(territoryId: widget.territoryId, postId: postId),
      ),
    );
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
          TextButton(
              onPressed: () => Navigator.pop(ctx, false),
              child: Text(l10n.cancel)),
          FilledButton(
              onPressed: () => Navigator.pop(ctx, true),
              child: Text(l10n.delete)),
        ],
      ),
    );
    if (confirmed != true) return;
    await ref
        .read(feedNotifierProvider(widget.territoryId).notifier)
        .deletePost(postId);
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
                leading: Icon(Icons.delete_outline,
                    color: Theme.of(ctx).colorScheme.error),
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
      if (filterType == widget.neighborhoodFilterValue) {
        final visibility =
            _normalizedVisibility(post?['visibility']?.toString());
        return visibility != 'residents_only';
      }
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

  bool _isPinned(Map<String, dynamic>? item) {
    if (item == null) return false;
    final metadata = item['metadata'] as Map<String, dynamic>?;
    final post = item['post'] as Map<String, dynamic>?;
    return metadata?['isHighlighted'] == true ||
        metadata?['isPinned'] == true ||
        item['isHighlighted'] == true ||
        post?['isHighlighted'] == true;
  }

  String? _authorName(Map<String, dynamic>? item) {
    final author = item?['author'] as Map<String, dynamic>?;
    final raw = author?['displayName'] ?? author?['name'];
    final value = raw?.toString().trim();
    return value == null || value.isEmpty ? null : value;
  }

  String? _authorHandle(Map<String, dynamic>? item) {
    final author = item?['author'] as Map<String, dynamic>?;
    final raw = author?['handle'] ?? author?['username'];
    final value = raw?.toString().trim();
    return value == null || value.isEmpty ? null : value;
  }

  String? _authorRole(Map<String, dynamic>? item) {
    final author = item?['author'] as Map<String, dynamic>?;
    final raw = author?['membership'] ?? author?['role'];
    final value = raw?.toString().trim();
    return value == null || value.isEmpty ? null : value;
  }

  String? _authorAvatarUrl(Map<String, dynamic>? item) {
    final author = item?['author'] as Map<String, dynamic>?;
    final raw = author?['avatarUrl'] ?? author?['avatar'];
    final value = raw?.toString().trim();
    return value == null || value.isEmpty ? null : value;
  }

  String? _visibilityLabel(Map<String, dynamic>? item) {
    final post = item?['post'] as Map<String, dynamic>?;
    final value = _normalizedVisibility(post?['visibility']?.toString());
    if (value == 'residents_only') {
      return AppLocalizations.of(context)!.residentsOnly.toLowerCase();
    }
    return null;
  }

  String _normalizedVisibility(String? value) {
    if (value == null) return '';
    final normalized = value
        .replaceAllMapped(
            RegExp(r'([a-z0-9])([A-Z])'), (match) => '${match[1]}_${match[2]}')
        .replaceAll('-', '_')
        .toLowerCase();
    if (normalized == 'residentsonly') return 'residents_only';
    return normalized;
  }

  String _timeLabel(Map<String, dynamic>? item) {
    final post = item?['post'] as Map<String, dynamic>?;
    final createdAt =
        DateTime.tryParse(post?['createdAtUtc']?.toString() ?? '');
    if (createdAt == null) return '';
    final diff = DateTime.now().toUtc().difference(createdAt.toUtc());
    if (diff.inMinutes < 60) return '${diff.inMinutes.clamp(1, 59)}min';
    if (diff.inHours < 24) return '${diff.inHours}h';
    if (diff.inDays < 7) return '${diff.inDays}d';
    return DateFormat('d/M').format(createdAt.toLocal());
  }

  @override
  Widget build(BuildContext context) {
    final items = _filteredItems();
    final hasMore = widget.hasMore;
    final isLoadingMore = widget.isLoadingMore;
    final onLoadMore = widget.onLoadMore;
    final territory = ref.watch(currentTerritoryItemProvider);
    if (items.isEmpty) {
      final l10n = AppLocalizations.of(context)!;
      return ListView(
        controller: widget.scrollController,
        physics: const AlwaysScrollableScrollPhysics(),
        children: [
          SizedBox(
            height: MediaQuery.of(context).size.height * 0.6,
            child: ArahEmptyState(
              icon: Icons.article_outlined,
              title: l10n.noPostsHere,
              description: l10n.beFirstToPost,
              actionLabel:
                  widget.onGoToCreatePost != null ? l10n.createFirstPost : null,
              onAction: widget.onGoToCreatePost,
            ),
          ),
        ],
      );
    }
    return ListView(
      controller: widget.scrollController,
      physics: const AlwaysScrollableScrollPhysics(),
      padding: const EdgeInsets.fromLTRB(18, 0, 18, 4),
      children: [
        for (var index = 0; index < items.length; index += 1)
          _buildAnimatedCard(
            context,
            item: items[index] as Map<String, dynamic>?,
            index: index,
            isLast: index == items.length - 1,
          ),
        const SizedBox(height: 14),
        if (!hasMore && !isLoadingMore)
          Center(
            child: Text(
              _footerLabel(context, territory),
              style: Theme.of(context).textTheme.bodySmall?.copyWith(
                    fontSize: 12.5,
                    color: Theme.of(context).colorScheme.onSurfaceVariant,
                    letterSpacing: 0.2,
                  ),
              textAlign: TextAlign.center,
            ),
          ),
        if (hasMore || isLoadingMore)
          Padding(
            padding:
                const EdgeInsets.symmetric(vertical: AppConstants.spacingLg),
            child: Center(
              child: isLoadingMore
                  ? const CircularProgressIndicator()
                  : TextButton(
                      onPressed: onLoadMore,
                      child: Text(AppLocalizations.of(context)!.loadMore),
                    ),
            ),
          ),
        const SizedBox(height: 12),
      ],
    );
  }

  Widget _buildAnimatedCard(
    BuildContext context, {
    required Map<String, dynamic>? item,
    required int index,
    required bool isLast,
  }) {
    final post = item?['post'] as Map<String, dynamic>?;
    final postId = post?['id']?.toString() ?? '';
    final title = post?['title']?.toString() ??
        AppLocalizations.of(context)!.postDefaultTitle;
    final content = post?['content']?.toString() ?? '';
    final postType = post?['type']?.toString();
    final counts =
        FeedPostCounts.fromJson(item?['counts'] as Map<String, dynamic>?);
    final interactions = FeedUserInteractions.fromJson(
      item?['userInteractions'] as Map<String, dynamic>?,
    );
    final mediaUrls = _mediaUrls(item);
    final canDelete = _canDelete(item);
    final notifier =
        ref.read(feedNotifierProvider(widget.territoryId).notifier);

    return Padding(
      padding: EdgeInsets.only(bottom: isLast ? 0 : 12),
      child: TweenAnimationBuilder<double>(
        key: ValueKey(postId.isNotEmpty ? postId : index),
        tween: Tween(begin: 0, end: 1),
        duration: ArahMotion.resolve(context, ArahMotion.normal),
        curve: ArahMotion.emphasized,
        builder: (context, value, child) => Opacity(
          opacity: value,
          child: Transform.translate(
            offset: Offset(0, 8 * (1 - value)),
            child: child,
          ),
        ),
        child: FeedPostCard(
          title: title,
          authorName: _authorName(item) ?? title,
          authorInitial: _authorName(item),
          authorHandle: _authorHandle(item),
          authorRole: _authorRole(item),
          authorAvatarUrl: _authorAvatarUrl(item),
          timeLabel: _timeLabel(item),
          content: content,
          type: FeedPostCard.typeFromString(postType),
          visibilityLabel: _visibilityLabel(item),
          likeCount: counts.likes,
          commentCount: counts.comments,
          shareCount: counts.shares,
          isLiked: interactions.liked,
          isShared: interactions.shared,
          mediaUrls: mediaUrls,
          isPinned: _isPinned(item),
          onTap: postId.isEmpty ? null : () => _openDetail(postId),
          onMorePressed: canDelete
              ? () => _showPostMenu(postId: postId, canDelete: canDelete)
              : null,
          onLikePressed: postId.isEmpty
              ? null
              : () => notifier.interact(
                  postId: postId, action: FeedInteractionAction.like),
          onCommentPressed:
              postId.isEmpty ? null : () => _openComments(postId, title),
          onSharePressed: postId.isEmpty
              ? null
              : () => notifier.interact(
                  postId: postId, action: FeedInteractionAction.share),
        ),
      ),
    );
  }

  String _footerLabel(BuildContext context, dynamic territory) {
    final name = territory?.name?.toString();
    final members = territory?.memberCount as int?;
    if (name == null || name.isEmpty) return '';
    if (members == null) {
      return 'Você viu tudo de $name';
    }
    return 'Você viu tudo de $name · ${NumberFormat.decimalPattern('pt_BR').format(members)} moradores';
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
        'alert': 'Avisos',
        '__public_only__': 'Vizinhança',
      };

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final options = _options(l10n);
    return SingleChildScrollView(
      scrollDirection: Axis.horizontal,
      padding: EdgeInsets.zero,
      child: Row(
        children: options.entries.map((entry) {
          final selected =
              selectedType?.toLowerCase() == entry.key?.toLowerCase() ||
                  (selectedType == null && entry.key == null);
          return Padding(
            padding: const EdgeInsets.only(right: AppConstants.spacingSm),
            child: _FeedFilterChip(
              label: entry.value,
              selected: selected,
              onTap: () => onTypeSelected(entry.key == null ? null : entry.key),
            ),
          );
        }).toList(),
      ),
    );
  }
}

class _FeedFilterChip extends StatelessWidget {
  const _FeedFilterChip({
    required this.label,
    required this.selected,
    required this.onTap,
  });

  final String label;
  final bool selected;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final colors = context.appColors;
    return Material(
      color: Colors.transparent,
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(AppConstants.radiusXl),
        child: Ink(
          padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 9),
          decoration: BoxDecoration(
            borderRadius: BorderRadius.circular(AppConstants.radiusXl),
            color: selected ? colors.accentSubtle : Colors.transparent,
            border: Border.all(
                color: selected ? colors.accentBorder : colors.outline),
          ),
          child: Text(
            label,
            style: Theme.of(context).textTheme.labelLarge?.copyWith(
                  color: selected ? colors.primary : colors.onSurfaceVariant,
                  letterSpacing: 0.1,
                ),
          ),
        ),
      ),
    );
  }
}
