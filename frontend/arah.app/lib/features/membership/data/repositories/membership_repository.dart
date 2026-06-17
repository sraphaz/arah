import '../../../../core/network/api_exception.dart';
import '../../../../core/network/bff_client.dart';
import '../models/membership_info.dart';

class MembershipRepository {
  MembershipRepository({required BffClient client}) : _client = client;

  final BffClient _client;

  Future<MembershipInfo?> getTerritoryMembership(String territoryId) async {
    final response = await _client.get('membership', '$territoryId/me');
    if (response.statusCode == 404) return null;
    if (response.statusCode < 200 || response.statusCode >= 300) {
      throw ApiException('HTTP ${response.statusCode}', statusCode: response.statusCode);
    }
    final data = response.data as Map<String, dynamic>?;
    if (data == null) return null;
    return MembershipInfo.fromJson(data);
  }

  Future<List<MembershipInfo>> listMyMemberships() async {
    final response = await _client.get('membership', 'me');
    if (response.statusCode < 200 || response.statusCode >= 300) {
      throw ApiException('HTTP ${response.statusCode}', statusCode: response.statusCode);
    }
    final data = response.data as Map<String, dynamic>?;
    final list = data?['memberships'] as List? ?? [];
    return list.whereType<Map<String, dynamic>>().map(MembershipInfo.fromJson).toList();
  }

  Future<Map<String, dynamic>> becomeResident({
    required String territoryId,
    String? message,
  }) async {
    final response = await _client.post(
      'membership',
      '$territoryId/become-resident',
      body: {'message': message, 'recipientUserIds': null},
    );
    if (response.statusCode < 200 || response.statusCode >= 300) {
      throw ApiException('HTTP ${response.statusCode}', statusCode: response.statusCode);
    }
    return response.data as Map<String, dynamic>? ?? {};
  }

  Future<void> verifyResidencyByGeo({
    required String territoryId,
    required double lat,
    required double lng,
  }) async {
    final response = await _client.post(
      'membership',
      '$territoryId/verify-residency/geo',
      body: {'lat': lat, 'lng': lng},
    );
    if (response.statusCode < 200 || response.statusCode >= 300) {
      throw ApiException('HTTP ${response.statusCode}', statusCode: response.statusCode);
    }
  }
}
