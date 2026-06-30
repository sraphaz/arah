import 'package:cached_network_image/cached_network_image.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:intl/intl.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../core/widgets/app_snackbar.dart';
import '../../../../core/widgets/arah_scaffold.dart';
import '../../../../l10n/app_localizations.dart';
import '../../domain/feed_interaction.dart';
import '../providers/feed_provider.dart';
import '../widgets/feed_post_card.dart';
import '../widgets/feed_comments_sheet.dart';

/// Busca o detalhe de um post quando ele não está no estado do feed (ex.: deep-link de pin do mapa).
final postDetailFetchProvider = FutureProvider.autoDispose
    .family<Map<String, dynamic>?, ({String territoryId, String postId})>((ref, key) async {
  final repo = ref.watch(feedRepositoryProvider);
  return repo.getPost(postId: key.postId, territoryId: key.territoryId);
});

/// Detalhe de um post: conteúdo completo, mídias, autor, contadores e ações.
/// Se o post estiver no feed carregado, usa o item "vivo" (interações via feed); caso
/// contrário (ex.: deep-link), busca por id e exibe em modo leitura + comentários.
class PostDetailScreen extends ConsumerStatefulWidget {
  const PostDetailScreen({super.key, required this.territoryId, required this.postId});

  final String territoryId;
  final String postId;

  @override
  ConsumerState<PostDetailScreen> createState() => _PostDetailScreenState();
}

class _PostDetailScreenState extends ConsumerState<PostDetailScreen> {
  bool _wasInFeed = false;

  Map<String, dynamic>? _findInFeed() {
    final state = ref.watch(feedNotifierProvider(widget.territoryId));
    for (final raw in state.items) {
      if (raw is! Map<String, dynamic>) continue;
      final post = raw['post'] as Map<String, dynamic>?;
      if (post?['id']?.toString() == widget.postId) return raw;
    }
    return null;
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final feedItem = _findInFeed();

    if (feedItem != null) {
      _wasInFeed = true;
      final notifier = ref.read(feedNotifierProvider(widget.territoryId).notifier);
      final canDelete = (feedItem['metadata'] as Map<String, dynamic>?)?['canDelete'] == true;
      return ArahScaffold(
        appBar: AppBar(
          title: Text(l10n.postDetailTitle),
          actions: [
            if (canDelete)
              IconButton(
                icon: Icon(Icons.delete_outline, color: Theme.of(context).colorScheme.error),
                tooltip: l10n.deletePost,
                onPressed: () => _confirmDelete(notifier),
              ),
          ],
        ),
        body: _PostBody(
          item: feedItem,
          territoryId: widget.territoryId,
          onLike: () => notifier.interact(postId: widget.postId, action: FeedInteractionAction.like),
          onShare: () => notifier.interact(postId: widget.postId, action: FeedInteractionAction.share),
        ),
      );
    }

    // Estava no feed e sumiu (ex.: excluído) → fecha.
    if (_wasInFeed) {
      WidgetsBinding.instance.addPostFrameCallback((_) {
        if (mounted && Navigator.of(context).canPop()) Navigator.of(context).pop();
      });
      return ArahScaffold(appBar: AppBar(title: Text(l10n.postDetailTitle)), body: const SizedBox.shrink());
    }

    // Não está no feed (deep-link): busca por id e exibe em leitura.
    final fetchAsync = ref.watch(
      postDetailFetchProvider((territoryId: widget.territoryId, postId: widget.postId)),
    );
    return ArahScaffold(
      appBar: AppBar(title: Text(l10n.postDetailTitle)),
      body: fetchAsync.when(
        data: (item) => item == null
            ? Center(
                child: Padding(
                  padding: const EdgeInsets.all(AppConstants.spacingLg),
                  child: Text(l10n.postNotFound, textAlign: TextAlign.center),
                ),
              )
            : _PostBody(item: item, territoryId: widget.territoryId),
        loading: () => const Center(child: CircularProgressIndicator()),
        error: (err, _) => Center(
          child: Padding(
            padding: const EdgeInsets.all(AppConstants.spacingLg),
            child: Text(
              err is ApiException ? err.userMessage : l10n.errorLoad,
              textAlign: TextAlign.center,
            ),
          ),
        ),
      ),
    );
  }

  Future<void> _confirmDelete(FeedNotifier notifier) async {
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
    try {
      await notifier.deletePost(widget.postId);
      // O build reage à remoção do item no estado e fecha a tela.
    } catch (e) {
      if (mounted) {
        showErrorSnackBar(context, e is ApiException ? e.userMessage : l10n.errorLoad);
      }
    }
  }
}

/// Corpo do detalhe do post (Card para bom contraste). Usado nos modos feed e leitura.
class _PostBody extends StatelessWidget {
  const _PostBody({
    required this.item,
    required this.territoryId,
    this.onLike,
    this.onShare,
  });

  final Map<String, dynamic> item;
  final String territoryId;
  final VoidCallback? onLike;
  final VoidCallback? onShare;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final theme = Theme.of(context);
    final post = item['post'] as Map<String, dynamic>? ?? const {};
    final author = item['author'] as Map<String, dynamic>?;
    final counts = FeedPostCounts.fromJson(item['counts'] as Map<String, dynamic>?);
    final interactions = FeedUserInteractions.fromJson(item['userInteractions'] as Map<String, dynamic>?);

    final title = post['title']?.toString() ?? l10n.postDefaultTitle;
    final content = post['content']?.toString() ?? '';
    final type = FeedPostCard.typeFromString(post['type']?.toString());
    final authorName = author?['displayName']?.toString();
    final createdAt = DateTime.tryParse(post['createdAtUtc']?.toString() ?? '');
    final mediaUrls = _mediaUrls(item);
    final postId = post['id']?.toString() ?? '';

    return ListView(
      padding: const EdgeInsets.all(AppConstants.spacingMd),
      children: [
        Card(
          child: Padding(
            padding: const EdgeInsets.all(AppConstants.spacingMd),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  children: [
                    CircleAvatar(
                      radius: AppConstants.avatarSizeSm / 2,
                      backgroundColor: theme.colorScheme.primaryContainer,
                      child: Text(
                        ((authorName?.isNotEmpty == true ? authorName! : title)[0]).toUpperCase(),
                        style: TextStyle(
                          color: theme.colorScheme.onPrimaryContainer,
                          fontWeight: FontWeight.w600,
                        ),
                      ),
                    ),
                    const SizedBox(width: AppConstants.spacingSm),
                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          if (authorName != null && authorName.isNotEmpty)
                            Text(authorName, style: theme.textTheme.titleSmall),
                          if (createdAt != null)
                            Text(
                              DateFormat('dd/MM/yyyy HH:mm').format(createdAt.toLocal()),
                              style: theme.textTheme.bodySmall?.copyWith(
                                color: theme.colorScheme.onSurfaceVariant,
                              ),
                            ),
                        ],
                      ),
                    ),
                    _TypeChip(type: type),
                  ],
                ),
                const SizedBox(height: AppConstants.spacingMd),
                Text(title, style: theme.textTheme.titleLarge),
                if (content.isNotEmpty) ...[
                  const SizedBox(height: AppConstants.spacingSm),
                  SelectableText(content, style: theme.textTheme.bodyLarge),
                ],
                ...mediaUrls.map(
                  (url) => Padding(
                    padding: const EdgeInsets.only(top: AppConstants.spacingMd),
                    child: ClipRRect(
                      borderRadius: BorderRadius.circular(AppConstants.radiusMd),
                      child: CachedNetworkImage(
                        imageUrl: url,
                        width: double.infinity,
                        fit: BoxFit.cover,
                        placeholder: (_, __) => Container(
                          height: 180,
                          color: theme.colorScheme.surfaceContainerHighest,
                          child: const Center(child: CircularProgressIndicator(strokeWidth: 2)),
                        ),
                        errorWidget: (_, __, ___) => Container(
                          height: 120,
                          color: theme.colorScheme.surfaceContainerHighest,
                          child: const Center(child: Icon(Icons.broken_image_outlined)),
                        ),
                      ),
                    ),
                  ),
                ),
                const SizedBox(height: AppConstants.spacingMd),
                const Divider(),
                Row(
                  children: [
                    _Action(
                      icon: interactions.liked ? Icons.favorite : Icons.favorite_border,
                      label: counts.likes.toString(),
                      color: interactions.liked ? theme.colorScheme.primary : null,
                      onPressed: onLike,
                    ),
                    _Action(
                      icon: Icons.chat_bubble_outline,
                      label: counts.comments.toString(),
                      onPressed: postId.isEmpty
                          ? null
                          : () => FeedCommentsSheet.show(
                                context,
                                postId: postId,
                                territoryId: territoryId,
                                postTitle: title,
                              ),
                    ),
                    _Action(
                      icon: interactions.shared ? Icons.send : Icons.send_outlined,
                      label: counts.shares.toString(),
                      color: interactions.shared ? theme.colorScheme.primary : null,
                      onPressed: onShare,
                    ),
                  ],
                ),
              ],
            ),
          ),
        ),
      ],
    );
  }

  List<String> _mediaUrls(Map<String, dynamic> item) {
    final media = item['media'] as List?;
    if (media == null) return const [];
    return media
        .whereType<Map<String, dynamic>>()
        .map((e) => e['url']?.toString())
        .whereType<String>()
        .where((url) => url.isNotEmpty)
        .toList();
  }
}

class _TypeChip extends StatelessWidget {
  const _TypeChip({required this.type});
  final FeedPostType type;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final theme = Theme.of(context);
    final label = switch (type) {
      FeedPostType.alert => l10n.alert,
      FeedPostType.event => l10n.mapEvent,
      FeedPostType.tip => l10n.feedTypeTip,
      FeedPostType.general => l10n.general,
    };
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: AppConstants.spacingSm, vertical: 2),
      decoration: BoxDecoration(
        color: theme.colorScheme.secondaryContainer,
        borderRadius: BorderRadius.circular(AppConstants.radiusSm),
      ),
      child: Text(
        label,
        style: theme.textTheme.labelSmall?.copyWith(color: theme.colorScheme.onSecondaryContainer),
      ),
    );
  }
}

class _Action extends StatelessWidget {
  const _Action({required this.icon, required this.label, this.color, this.onPressed});
  final IconData icon;
  final String label;
  final Color? color;
  final VoidCallback? onPressed;

  @override
  Widget build(BuildContext context) {
    return Expanded(
      child: TextButton.icon(
        onPressed: onPressed,
        icon: Icon(icon, size: 20, color: color),
        label: Text(label, style: TextStyle(color: color)),
      ),
    );
  }
}
