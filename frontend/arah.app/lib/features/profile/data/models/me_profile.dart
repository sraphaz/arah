/// Resposta do BFF GET me/profile (UserProfileResponse).
///
/// Contagens (`postsCount`, `connectionsCount`, `interestsCount`) são opcionais —
/// o BFF pode enviá-las no root, em `stats`, ou omiti-las.
class MeProfile {
  const MeProfile({
    required this.id,
    required this.displayName,
    this.email,
    this.phoneNumber,
    this.address,
    required this.createdAtUtc,
    this.interests = const [],
    this.avatarUrl,
    this.bio,
    this.postsCount,
    this.connectionsCount,
    this.interestsCount,
  });

  final String id;
  final String displayName;
  final String? email;
  final String? phoneNumber;
  final String? address;
  final DateTime createdAtUtc;
  final List<String> interests;
  final String? avatarUrl;
  final String? bio;
  final int? postsCount;
  final int? connectionsCount;
  final int? interestsCount;

  /// True quando o JSON trouxe ao menos uma contagem explícita de stats.
  bool get hasStatCounts =>
      postsCount != null || connectionsCount != null || interestsCount != null;

  static MeProfile fromJson(Map<String, dynamic> json) {
    final id = json['id'];
    final createdAt = json['createdAtUtc'];
    final stats = json['stats'];
    final statsMap = stats is Map<String, dynamic> ? stats : null;

    return MeProfile(
      id: id?.toString() ?? '',
      displayName: json['displayName'] as String? ?? '',
      email: json['email'] as String?,
      phoneNumber: json['phoneNumber'] as String?,
      address: json['address'] as String?,
      createdAtUtc: createdAt != null
          ? (createdAt is String ? DateTime.tryParse(createdAt) : null) ?? DateTime.now()
          : DateTime.now(),
      interests: (json['interests'] as List<dynamic>?)?.map((e) => e.toString()).toList() ?? [],
      avatarUrl: json['avatarUrl'] as String?,
      bio: json['bio'] as String?,
      postsCount: _readInt(json, statsMap, const [
        'postsCount',
        'postsCreated',
        'posts',
      ]),
      connectionsCount: _readInt(json, statsMap, const [
        'connectionsCount',
        'connections',
      ]),
      interestsCount: _readInt(json, statsMap, const [
        'interestsCount',
        'interestsTotal',
      ]),
    );
  }

  static int? _readInt(
    Map<String, dynamic> json,
    Map<String, dynamic>? stats,
    List<String> keys,
  ) {
    for (final key in keys) {
      final fromRoot = _asInt(json[key]);
      if (fromRoot != null) return fromRoot;
      if (stats != null) {
        final fromStats = _asInt(stats[key]);
        if (fromStats != null) return fromStats;
      }
    }
    return null;
  }

  static int? _asInt(Object? value) {
    if (value is int) return value;
    if (value is num) return value.toInt();
    if (value is String) return int.tryParse(value);
    return null;
  }
}
