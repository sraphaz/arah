import 'package:arah_app/features/governance/data/models/voting.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  group('Voting.fromJson', () {
    test('parses full json', () {
      final voting = Voting.fromJson({
        'id': 'v1',
        'territoryId': 't1',
        'createdByUserId': 'u1',
        'type': 'CommunityPolicy',
        'title': 'Horário do silêncio',
        'description': 'Definir horário de silêncio.',
        'options': ['22h', '23h'],
        'visibility': 'AllMembers',
        'status': 'Open',
        'createdAtUtc': '2026-01-01T10:00:00Z',
        'updatedAtUtc': '2026-01-02T10:00:00Z',
      });

      expect(voting.id, 'v1');
      expect(voting.territoryId, 't1');
      expect(voting.type, 'CommunityPolicy');
      expect(voting.title, 'Horário do silêncio');
      expect(voting.options, ['22h', '23h']);
      expect(voting.visibility, 'AllMembers');
      expect(voting.status, 'Open');
      expect(voting.isOpen, isTrue);
    });

    test('isOpen is case-insensitive and false when closed', () {
      final closed = Voting.fromJson({'id': 'v', 'status': 'Closed'});
      expect(closed.isOpen, isFalse);
    });

    test('applies defaults for missing fields', () {
      final voting = Voting.fromJson({'id': 'v2'});
      expect(voting.id, 'v2');
      expect(voting.title, '');
      expect(voting.options, isEmpty);
      expect(voting.status, 'Open');
      expect(voting.startsAtUtc, isNull);
    });
  });

  group('VotingResults.fromJson', () {
    test('parses counts and computes total', () {
      final results = VotingResults.fromJson({
        'votingId': 'v1',
        'results': {'22h': 3, '23h': 5},
      });

      expect(results.votingId, 'v1');
      expect(results.counts['22h'], 3);
      expect(results.counts['23h'], 5);
      expect(results.totalVotes, 8);
    });

    test('handles empty results', () {
      final results = VotingResults.fromJson({'votingId': 'v1'});
      expect(results.counts, isEmpty);
      expect(results.totalVotes, 0);
    });
  });
}
