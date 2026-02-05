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
}
