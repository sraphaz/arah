import '../../../../core/network/api_exception.dart';
import '../../../../core/network/bff_client.dart';
import '../models/me_profile.dart';

/// Repositório do perfil do usuário logado (jornada me).
class MeProfileRepository {
  MeProfileRepository({required this.client});

  final BffClient client;

  /// GET me/profile
  Future<MeProfile> getProfile() async {
    final response = await client.get('me', 'profile');
    final data = response.data as Map<String, dynamic>?;
    if (data == null) throw ApiException('Resposta inválida');
    return MeProfile.fromJson(data);
  }

  /// PUT me/profile/display-name
  Future<MeProfile> updateDisplayName(String displayName) async {
    final response = await client.put(
      'me',
      'profile/display-name',
      body: <String, dynamic>{'displayName': displayName},
    );
    final data = response.data as Map<String, dynamic>?;
    if (data == null) throw ApiException('Resposta inválida');
    return MeProfile.fromJson(data);
  }

  /// PUT me/profile/bio
  Future<MeProfile> updateBio(String? bio) async {
    final response = await client.put(
      'me',
      'profile/bio',
      body: <String, dynamic>{'bio': bio ?? ''},
    );
    final data = response.data as Map<String, dynamic>?;
    if (data == null) throw ApiException('Resposta inválida');
    return MeProfile.fromJson(data);
  }

  // --- Interesses (me/interests) ---

  /// GET me/interests
  Future<List<String>> getInterests() async {
    final response = await client.get('me', 'interests');
    final list = response.data is List ? response.data as List : null;
    if (list == null) return [];
    return list.map((e) => e.toString()).toList();
  }

  /// POST me/interests — body: { interestTag }
  Future<void> addInterest(String interestTag) async {
    await client.post(
      'me',
      'interests',
      body: <String, dynamic>{'interestTag': interestTag.trim()},
    );
  }

  /// DELETE me/interests/{tag}
  Future<void> removeInterest(String tag) async {
    final encoded = Uri.encodeComponent(tag);
    await client.delete('me', 'interests/$encoded');
  }

  // --- Preferências (me/preferences) ---

  /// GET me/preferences
  Future<UserPreferences> getPreferences() async {
    final response = await client.get('me', 'preferences');
    final data = response.data as Map<String, dynamic>?;
    if (data == null) throw ApiException('Resposta inválida');
    return UserPreferences.fromJson(data);
  }

  /// PUT me/preferences/notifications
  Future<UserPreferences> updateNotificationPreferences(NotificationPrefs prefs) async {
    final response = await client.put(
      'me',
      'preferences/notifications',
      body: prefs.toJson(),
    );
    final data = response.data as Map<String, dynamic>?;
    if (data == null) throw ApiException('Resposta inválida');
    return UserPreferences.fromJson(data);
  }
}

/// Preferências do usuário (GET me/preferences).
class UserPreferences {
  UserPreferences({
    required this.profileVisibility,
    required this.contactVisibility,
    required this.shareLocation,
    required this.showMemberships,
    required this.notifications,
    required this.email,
  });

  final String profileVisibility;
  final String contactVisibility;
  final bool shareLocation;
  final bool showMemberships;
  final NotificationPrefs notifications;
  final EmailPrefs email;

  factory UserPreferences.fromJson(Map<String, dynamic> json) {
    final notif = json['notifications'] as Map<String, dynamic>?;
    final em = json['email'] as Map<String, dynamic>?;
    return UserPreferences(
      profileVisibility: json['profileVisibility'] as String? ?? 'Public',
      contactVisibility: json['contactVisibility'] as String? ?? 'Public',
      shareLocation: json['shareLocation'] as bool? ?? false,
      showMemberships: json['showMemberships'] as bool? ?? true,
      notifications: notif != null ? NotificationPrefs.fromJson(notif) : NotificationPrefs.empty(),
      email: em != null ? EmailPrefs.fromJson(em) : EmailPrefs.empty(),
    );
  }
}

class NotificationPrefs {
  NotificationPrefs({
    this.postsEnabled = true,
    this.commentsEnabled = true,
    this.eventsEnabled = true,
    this.alertsEnabled = true,
    this.marketplaceEnabled = false,
    this.moderationEnabled = false,
    this.membershipRequestsEnabled = true,
  });

  final bool postsEnabled;
  final bool commentsEnabled;
  final bool eventsEnabled;
  final bool alertsEnabled;
  final bool marketplaceEnabled;
  final bool moderationEnabled;
  final bool membershipRequestsEnabled;

  static NotificationPrefs empty() => NotificationPrefs();

  factory NotificationPrefs.fromJson(Map<String, dynamic> json) {
    return NotificationPrefs(
      postsEnabled: json['postsEnabled'] as bool? ?? true,
      commentsEnabled: json['commentsEnabled'] as bool? ?? true,
      eventsEnabled: json['eventsEnabled'] as bool? ?? true,
      alertsEnabled: json['alertsEnabled'] as bool? ?? true,
      marketplaceEnabled: json['marketplaceEnabled'] as bool? ?? false,
      moderationEnabled: json['moderationEnabled'] as bool? ?? false,
      membershipRequestsEnabled: json['membershipRequestsEnabled'] as bool? ?? true,
    );
  }

  Map<String, dynamic> toJson() => {
        'postsEnabled': postsEnabled,
        'commentsEnabled': commentsEnabled,
        'eventsEnabled': eventsEnabled,
        'alertsEnabled': alertsEnabled,
        'marketplaceEnabled': marketplaceEnabled,
        'moderationEnabled': moderationEnabled,
        'membershipRequestsEnabled': membershipRequestsEnabled,
      };
}

class EmailPrefs {
  EmailPrefs({this.receiveEmails = true, this.emailFrequency = 'Weekly', this.emailTypes = 0});

  final bool receiveEmails;
  final String emailFrequency;
  final int emailTypes;

  static EmailPrefs empty() => EmailPrefs();

  factory EmailPrefs.fromJson(Map<String, dynamic> json) {
    return EmailPrefs(
      receiveEmails: json['receiveEmails'] as bool? ?? true,
      emailFrequency: json['emailFrequency'] as String? ?? 'Weekly',
      emailTypes: (json['emailTypes'] as num?)?.toInt() ?? 0,
    );
  }
}
