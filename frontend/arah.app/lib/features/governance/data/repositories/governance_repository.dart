import '../../../../core/network/api_exception.dart';
import '../../../../core/network/bff_client.dart';
import '../models/voting.dart';

/// Repositório da jornada BFF governance (votações por território).
/// Proxy para api/v1/territories/{territoryId}/votings na API.
class GovernanceRepository {
  GovernanceRepository({required BffClient client}) : _client = client;

  final BffClient _client;

  static const String _journey = 'governance';

  /// GET governance/{territoryId}/votings?status=
  Future<List<Voting>> listVotings(
    String territoryId, {
    String? status,
  }) async {
    var path = '$territoryId/votings';
    if (status != null && status.isNotEmpty) {
      path += '?status=$status';
    }
    final response = await _client.get(_journey, path);
    _ensureSuccess(response.statusCode);
    final data = response.data;
    if (data is! List) return const [];
    return data
        .whereType<Map<String, dynamic>>()
        .map(Voting.fromJson)
        .toList();
  }

  /// GET governance/{territoryId}/votings/{votingId}/results
  Future<VotingResults> getResults({
    required String territoryId,
    required String votingId,
  }) async {
    final response =
        await _client.get(_journey, '$territoryId/votings/$votingId/results');
    _ensureSuccess(response.statusCode);
    final data = response.data;
    if (data is! Map<String, dynamic>) {
      return VotingResults(votingId: votingId, counts: const {});
    }
    return VotingResults.fromJson(data);
  }

  /// POST governance/{territoryId}/votings/{votingId}/vote
  Future<void> vote({
    required String territoryId,
    required String votingId,
    required String selectedOption,
  }) async {
    final response = await _client.post(
      _journey,
      '$territoryId/votings/$votingId/vote',
      body: {'selectedOption': selectedOption},
    );
    _ensureSuccess(response.statusCode);
  }

  /// POST governance/{territoryId}/votings
  Future<Voting> createVoting({
    required String territoryId,
    required String type,
    required String title,
    required String description,
    required List<String> options,
    required String visibility,
    DateTime? startsAtUtc,
    DateTime? endsAtUtc,
  }) async {
    final response = await _client.post(
      _journey,
      '$territoryId/votings',
      body: {
        'type': type,
        'title': title,
        'description': description,
        'options': options,
        'visibility': visibility,
        if (startsAtUtc != null) 'startsAtUtc': startsAtUtc.toIso8601String(),
        if (endsAtUtc != null) 'endsAtUtc': endsAtUtc.toIso8601String(),
      },
    );
    _ensureSuccess(response.statusCode);
    final data = response.data;
    if (data is! Map<String, dynamic>) {
      throw ApiException('Resposta inválida ao criar votação.',
          statusCode: response.statusCode);
    }
    return Voting.fromJson(data);
  }

  /// POST governance/{territoryId}/votings/{votingId}/close
  Future<void> closeVoting({
    required String territoryId,
    required String votingId,
  }) async {
    final response = await _client.post(
      _journey,
      '$territoryId/votings/$votingId/close',
    );
    _ensureSuccess(response.statusCode);
  }

  void _ensureSuccess(int statusCode) {
    if (statusCode < 200 || statusCode >= 300) {
      throw ApiException('HTTP $statusCode', statusCode: statusCode);
    }
  }
}
