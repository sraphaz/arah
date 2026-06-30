/// Votação de governança do território (BFF governance/{territoryId}/votings).
/// Alinhado a VotingResponse da API.
class Voting {
  const Voting({
    required this.id,
    required this.territoryId,
    required this.createdByUserId,
    required this.type,
    required this.title,
    required this.description,
    required this.options,
    required this.visibility,
    required this.status,
    this.startsAtUtc,
    this.endsAtUtc,
    required this.createdAtUtc,
    required this.updatedAtUtc,
  });

  final String id;
  final String territoryId;
  final String createdByUserId;
  final String type;
  final String title;
  final String description;
  final List<String> options;
  final String visibility;
  final String status; // Open | Closed | Cancelled
  final DateTime? startsAtUtc;
  final DateTime? endsAtUtc;
  final DateTime createdAtUtc;
  final DateTime updatedAtUtc;

  bool get isOpen => status.toLowerCase() == 'open';

  factory Voting.fromJson(Map<String, dynamic> json) {
    final optionsRaw = json['options'] as List? ?? const [];
    return Voting(
      id: (json['id'] ?? '').toString(),
      territoryId: (json['territoryId'] ?? '').toString(),
      createdByUserId: (json['createdByUserId'] ?? '').toString(),
      type: json['type'] as String? ?? '',
      title: json['title'] as String? ?? '',
      description: json['description'] as String? ?? '',
      options: optionsRaw.map((e) => e.toString()).toList(),
      visibility: json['visibility'] as String? ?? '',
      status: json['status'] as String? ?? 'Open',
      startsAtUtc: _parseDate(json['startsAtUtc']),
      endsAtUtc: _parseDate(json['endsAtUtc']),
      createdAtUtc: _parseDate(json['createdAtUtc']) ?? DateTime.now(),
      updatedAtUtc: _parseDate(json['updatedAtUtc']) ?? DateTime.now(),
    );
  }

  static DateTime? _parseDate(dynamic value) {
    if (value == null) return null;
    return DateTime.tryParse(value.toString());
  }
}

/// Resultados de uma votação: contagem de votos por opção.
/// Alinhado a VotingResultsResponse da API.
class VotingResults {
  const VotingResults({required this.votingId, required this.counts});

  final String votingId;
  final Map<String, int> counts;

  int get totalVotes => counts.values.fold(0, (sum, c) => sum + c);

  factory VotingResults.fromJson(Map<String, dynamic> json) {
    final resultsRaw = json['results'] as Map<String, dynamic>? ?? const {};
    final counts = <String, int>{};
    resultsRaw.forEach((key, value) {
      counts[key] = (value as num?)?.toInt() ?? 0;
    });
    return VotingResults(
      votingId: (json['votingId'] ?? '').toString(),
      counts: counts,
    );
  }
}
