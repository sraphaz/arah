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

/// Detalhe de um post do feed: conteúdo completo, todas as mídias, autor, contadores e ações.
/// Lê o item "vivo" do estado do feed por id, para refletir like/comentar/compartilhar/excluir.
class PostDetailScreen extends ConsumerWidget {
  const PostDetailScreen({super.key, required this.territoryId, required this.postId});

  final String territoryId;
  final String postId;

  Map<String, dynamic>? _findItem(FeedState state) {
    for (final raw in state.items) {
      if (raw is! Map<String, dynamic>) continue;
      final post = raw['post'] as Map<String, dynamic>?;
      if (post?['id']?.toString() == postId) return raw;
    }
    return null;
  }

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final l10n = AppLocalizations.of(context)!;
    final state = ref.watch(feedNotifierProvider(territoryId));
    final notifier = ref.read(feedNotifierProvider(territoryId).notifier);
    final item = _findItem(state);

    if (item == null) {
      // Post removido (ex.: após exclusão) ou ausente do estado: volta para o feed.
      WidgetsBinding.instance.addPostFrameCallback((_) {
        if (context.mounted && Navigator.of(context).canPop()) {
          Navigator.of(context).pop();
        }
      });
      return ArahScaffold(
        appBar: AppBar(title: Text(l10n.postDetailTitle)),
        body: const SizedBox.shrink(),
      );
    }

    final post = item['post'] as Map<String, dynamic>? ?? const {};
    final author = item['author'] as Map<String, dynamic>?;
    final counts = FeedPostCounts.fromJson(item['counts'] as Map<String, dynamic>?);
    final interactions = FeedUserInteractions.fromJson(item['userInteractions'] as Map<String, dynamic>?);
    final metadata = item['metadata'] as Map<String, dynamic>?;
    final canDelete = metadata?['canDelete'] == true;

    final title = post['title']?.toString() ?? l10n.postDefaultTitle;
    final content = post['content']?.toString() ?? '';
    final type = FeedPostCard.typeFromString(post['type']?.toString());
    final authorName = author?['displayName']?.toString();
    final createdAt = DateTime.tryParse(post['createdAtUtc']?.toString() ?? '');
    final mediaUrls = _mediaUrls(item);
    final theme = Theme.of(context);

    return ArahScaffold(
      appBar: AppBar(
        title: Text(l10n.postDetailTitle),
        actions: [
          if (canDelete)
            IconButton(
              icon: Icon(Icons.delete_outline, color: theme.colorScheme.error),
              tooltip: l10n.deletePost,
              onPressed: () => _confirmDelete(context, ref, notifier),
            ),
        ],
      ),
      body: ListView(
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
                onPressed: () => notifier.interact(postId: postId, action: FeedInteractionAction.like),
              ),
              _Action(
                icon: Icons.chat_bubble_outline,
                label: counts.comments.toString(),
                onPressed: () => FeedCommentsSheet.show(
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
                onPressed: () => notifier.interact(postId: postId, action: FeedInteractionAction.share),
              ),
            ],
          ),
                ],
              ),
            ),
          ),
        ],
      ),
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

  Future<void> _confirmDelete(BuildContext context, WidgetRef ref, FeedNotifier notifier) async {
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
      await notifier.deletePost(postId);
      // O build reage à remoção do item no estado e fecha a tela.
    } catch (e) {
      if (context.mounted) {
        showErrorSnackBar(context, e is ApiException ? e.userMessage : l10n.errorLoad);
      }
    }
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
