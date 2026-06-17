import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/network/api_exception.dart';
import '../../domain/feed_comment.dart';
import '../../domain/feed_interaction.dart';
import '../providers/feed_provider.dart';

/// Bottom sheet com lista de comentários e campo para enviar novo comentário.
class FeedCommentsSheet extends ConsumerStatefulWidget {
  const FeedCommentsSheet({
    super.key,
    required this.postId,
    required this.territoryId,
    required this.postTitle,
  });

  final String postId;
  final String territoryId;
  final String postTitle;

  static Future<void> show(
    BuildContext context, {
    required String postId,
    required String territoryId,
    required String postTitle,
  }) {
    return showModalBottomSheet<void>(
      context: context,
      isScrollControlled: true,
      showDragHandle: true,
      useSafeArea: true,
      builder: (ctx) => Padding(
        padding: EdgeInsets.only(bottom: MediaQuery.viewInsetsOf(ctx).bottom),
        child: FeedCommentsSheet(
          postId: postId,
          territoryId: territoryId,
          postTitle: postTitle,
        ),
      ),
    );
  }

  @override
  ConsumerState<FeedCommentsSheet> createState() => _FeedCommentsSheetState();
}

class _FeedCommentsSheetState extends ConsumerState<FeedCommentsSheet> {
  final TextEditingController _controller = TextEditingController();
  final List<FeedComment> _comments = [];
  bool _isLoading = true;
  bool _isSubmitting = false;
  bool _hasMore = false;
  int _page = 1;
  String? _error;

  @override
  void initState() {
    super.initState();
    _loadComments(reset: true);
  }

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  Future<void> _loadComments({required bool reset}) async {
    final nextPage = reset ? 1 : _page + 1;
    setState(() {
      _isLoading = true;
      _error = null;
    });

    try {
      final repository = ref.read(feedRepositoryProvider);
      final page = await repository.getPostComments(
        postId: widget.postId,
        territoryId: widget.territoryId,
        pageNumber: nextPage,
      );
      if (!mounted) return;
      setState(() {
        if (reset) {
          _comments
            ..clear()
            ..addAll(page.items);
        } else {
          _comments.addAll(page.items);
        }
        _hasMore = page.hasMore;
        _page = nextPage;
        _isLoading = false;
        _error = null;
      });
    } catch (e) {
      if (!mounted) return;
      setState(() {
        _isLoading = false;
        _error = e is ApiException ? e.userMessage : 'Não foi possível carregar comentários.';
      });
    }
  }

  Future<void> _submitComment() async {
    final content = _controller.text.trim();
    if (content.isEmpty || _isSubmitting) return;

    setState(() => _isSubmitting = true);
    try {
      await ref.read(feedNotifierProvider(widget.territoryId).notifier).interact(
            postId: widget.postId,
            action: FeedInteractionAction.comment,
            commentContent: content,
          );
      _controller.clear();
      await _loadComments(reset: true);
    } catch (e) {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text(e is ApiException ? e.userMessage : 'Erro ao comentar.')),
      );
    } finally {
      if (mounted) setState(() => _isSubmitting = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return DraggableScrollableSheet(
      expand: false,
      initialChildSize: 0.75,
      minChildSize: 0.45,
      maxChildSize: 0.95,
      builder: (context, scrollController) {
        return Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Padding(
              padding: const EdgeInsets.fromLTRB(
                AppConstants.spacingMd,
                0,
                AppConstants.spacingMd,
                AppConstants.spacingSm,
              ),
              child: Text(
                widget.postTitle,
                style: theme.textTheme.titleMedium,
                maxLines: 2,
                overflow: TextOverflow.ellipsis,
              ),
            ),
            Expanded(
              child: _isLoading
                  ? const Center(child: CircularProgressIndicator())
                  : _error != null
                      ? _ErrorState(message: _error!, onRetry: () => _loadComments(reset: true))
                      : _comments.isEmpty
                          ? Center(
                              child: Text(
                                'Nenhum comentário ainda.',
                                style: theme.textTheme.bodyMedium?.copyWith(
                                  color: theme.colorScheme.onSurfaceVariant,
                                ),
                              ),
                            )
                          : ListView.builder(
                              controller: scrollController,
                              padding: const EdgeInsets.symmetric(horizontal: AppConstants.spacingMd),
                              itemCount: _comments.length + (_hasMore ? 1 : 0),
                              itemBuilder: (context, index) {
                                if (index >= _comments.length) {
                                  return TextButton(
                                    onPressed: () => _loadComments(reset: false),
                                    child: const Text('Carregar mais'),
                                  );
                                }
                                final comment = _comments[index];
                                return _CommentTile(comment: comment);
                              },
                            ),
            ),
            const Divider(height: 1),
            Padding(
              padding: const EdgeInsets.all(AppConstants.spacingMd),
              child: Row(
                children: [
                  Expanded(
                    child: TextField(
                      controller: _controller,
                      minLines: 1,
                      maxLines: 3,
                      decoration: const InputDecoration(
                        hintText: 'Escreva um comentário',
                        border: OutlineInputBorder(),
                        isDense: true,
                      ),
                      onSubmitted: (_) => _submitComment(),
                    ),
                  ),
                  const SizedBox(width: AppConstants.spacingSm),
                  IconButton.filled(
                    onPressed: _isSubmitting ? null : _submitComment,
                    icon: _isSubmitting
                        ? const SizedBox(
                            width: 18,
                            height: 18,
                            child: CircularProgressIndicator(strokeWidth: 2),
                          )
                        : const Icon(Icons.send),
                  ),
                ],
              ),
            ),
          ],
        );
      },
    );
  }
}

class _CommentTile extends StatelessWidget {
  const _CommentTile({required this.comment});

  final FeedComment comment;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Padding(
      padding: const EdgeInsets.only(bottom: AppConstants.spacingMd),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          CircleAvatar(
            radius: AppConstants.avatarSizeSm / 2,
            child: Text(
              comment.authorName.isNotEmpty ? comment.authorName[0].toUpperCase() : '?',
              style: theme.textTheme.labelSmall,
            ),
          ),
          const SizedBox(width: AppConstants.spacingSm),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  comment.authorName,
                  style: theme.textTheme.labelLarge?.copyWith(fontWeight: FontWeight.w600),
                ),
                const SizedBox(height: 2),
                Text(comment.content, style: theme.textTheme.bodyMedium),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

class _ErrorState extends StatelessWidget {
  const _ErrorState({required this.message, required this.onRetry});

  final String message;
  final VoidCallback onRetry;

  @override
  Widget build(BuildContext context) {
    return Center(
      child: Padding(
        padding: const EdgeInsets.all(AppConstants.spacingLg),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Text(message, textAlign: TextAlign.center),
            const SizedBox(height: AppConstants.spacingMd),
            FilledButton.tonal(onPressed: onRetry, child: const Text('Tentar novamente')),
          ],
        ),
      ),
    );
  }
}
