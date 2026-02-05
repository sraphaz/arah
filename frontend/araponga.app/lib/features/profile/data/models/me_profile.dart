/// Resposta do BFF GET me/profile (UserProfileResponse).
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

  static MeProfile fromJson(Map<String, dynamic> json) {
    final id = json['id'];
    final createdAt = json['createdAtUtc'];
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
    );
  }
}
