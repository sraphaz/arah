/// Contratos de auth alinhados à API (SocialLoginResponse, refresh).
class AuthUser {
  const AuthUser({
    required this.id,
    required this.displayName,
    this.email,
    this.avatarUrl,
  });
  final String id;
  final String displayName;
  final String? email;
  final String? avatarUrl;

  factory AuthUser.fromJson(Map<String, dynamic> json) {
    final user = json['user'] as Map<String, dynamic>? ?? json;
    final id = user['id'];
    return AuthUser(
      id: id == null ? '' : id.toString(),
      displayName: user['displayName'] as String? ?? user['name'] as String? ?? '',
      email: user['email'] as String?,
      avatarUrl: user['avatarUrl'] as String?,
    );
  }
}

class AuthTokens {
  const AuthTokens({
    required this.accessToken,
    required this.refreshToken,
    required this.expiresInSeconds,
  });
  final String accessToken;
  final String refreshToken;
  final int expiresInSeconds;
}
