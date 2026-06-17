/// Comentário de um post no feed (resposta BFF `feed/post-comments`).
class FeedComment {
  const FeedComment({
    required this.id,
    required this.content,
    required this.createdAtUtc,
    required this.authorId,
    required this.authorName,
    this.authorAvatarUrl,
  });

  factory FeedComment.fromJson(Map<String, dynamic> json) {
    final author = json['author'] as Map<String, dynamic>? ?? const {};
    return FeedComment(
      id: json['id']?.toString() ?? '',
      content: json['content']?.toString() ?? '',
      createdAtUtc: DateTime.tryParse(json['createdAtUtc']?.toString() ?? '') ?? DateTime.fromMillisecondsSinceEpoch(0),
      authorId: author['id']?.toString() ?? '',
      authorName: author['displayName']?.toString() ?? 'Usuário',
      authorAvatarUrl: author['avatarUrl']?.toString(),
    );
  }

  final String id;
  final String content;
  final DateTime createdAtUtc;
  final String authorId;
  final String authorName;
  final String? authorAvatarUrl;
}

class FeedCommentsPage {
  const FeedCommentsPage({
    required this.items,
    required this.hasMore,
  });

  factory FeedCommentsPage.fromJson(Map<String, dynamic>? json) {
    if (json == null) {
      return const FeedCommentsPage(items: [], hasMore: false);
    }
    final items = (json['items'] as List?)
            ?.whereType<Map<String, dynamic>>()
            .map(FeedComment.fromJson)
            .toList() ??
        const <FeedComment>[];
    final pagination = json['pagination'] as Map<String, dynamic>?;
    final hasMore = pagination?['hasNextPage'] == true;
    return FeedCommentsPage(items: items, hasMore: hasMore);
  }

  final List<FeedComment> items;
  final bool hasMore;
}
