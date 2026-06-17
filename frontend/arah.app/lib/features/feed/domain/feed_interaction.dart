/// Ações suportadas pelo endpoint BFF `feed/interact`.
enum FeedInteractionAction {
  like('LIKE'),
  comment('COMMENT'),
  share('SHARE');

  const FeedInteractionAction(this.apiValue);
  final String apiValue;
}

/// Contadores de interação de um post no feed.
class FeedPostCounts {
  const FeedPostCounts({
    required this.likes,
    required this.shares,
    required this.comments,
  });

  factory FeedPostCounts.fromJson(Map<String, dynamic>? json) {
    if (json == null) {
      return const FeedPostCounts(likes: 0, shares: 0, comments: 0);
    }
    return FeedPostCounts(
      likes: _readInt(json, 'likes'),
      shares: _readInt(json, 'shares'),
      comments: _readInt(json, 'comments'),
    );
  }

  final int likes;
  final int shares;
  final int comments;

  static int _readInt(Map<String, dynamic> json, String key) {
    final value = json[key];
    if (value is int) return value;
    if (value is num) return value.toInt();
    return 0;
  }
}

/// Estado de interação do usuário atual com um post.
class FeedUserInteractions {
  const FeedUserInteractions({
    required this.liked,
    required this.shared,
    required this.commented,
  });

  factory FeedUserInteractions.fromJson(Map<String, dynamic>? json) {
    if (json == null) {
      return const FeedUserInteractions(liked: false, shared: false, commented: false);
    }
    return FeedUserInteractions(
      liked: json['liked'] == true,
      shared: json['shared'] == true,
      commented: json['commented'] == true,
    );
  }

  final bool liked;
  final bool shared;
  final bool commented;
}
