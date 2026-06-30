import 'package:arah_app/core/providers/current_territory_name_provider.dart';
import 'package:arah_app/features/governance/data/models/voting.dart';
import 'package:arah_app/features/governance/data/repositories/governance_repository.dart';
import 'package:arah_app/features/governance/presentation/providers/governance_provider.dart';
import 'package:arah_app/features/governance/presentation/screens/governance_screen.dart';
import 'package:arah_app/l10n/app_localizations.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

/// Fake que devolve uma lista controlada de votações, sem rede.
class _FakeGovernanceRepository implements GovernanceRepository {
  _FakeGovernanceRepository(this.votings);

  final List<Voting> votings;

  @override
  Future<List<Voting>> listVotings(String territoryId, {String? status}) async {
    return votings;
  }

  @override
  Future<VotingResults> getResults({
    required String territoryId,
    required String votingId,
  }) async {
    return const VotingResults(votingId: 'v', counts: {});
  }

  @override
  Future<void> vote({
    required String territoryId,
    required String votingId,
    required String selectedOption,
  }) async {}

  @override
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
    throw UnimplementedError();
  }

  @override
  Future<void> closeVoting({
    required String territoryId,
    required String votingId,
  }) async {}
}

void main() {
  Widget buildApp(List<Voting> votings) {
    return ProviderScope(
      overrides: [
        governanceRepositoryProvider
            .overrideWithValue(_FakeGovernanceRepository(votings)),
        currentTerritoryNameProvider.overrideWithValue('Camburi'),
      ],
      child: MaterialApp(
        locale: const Locale('pt'),
        localizationsDelegates: AppLocalizations.localizationsDelegates,
        supportedLocales: AppLocalizations.supportedLocales,
        home: const GovernanceScreen(territoryId: 't1'),
      ),
    );
  }

  testWidgets('GovernanceScreen shows empty state when there are no votings',
      (WidgetTester tester) async {
    await tester.pumpWidget(buildApp(const []));
    await tester.pumpAndSettle();

    expect(find.text('Governança'), findsWidgets);
    expect(find.text('Nenhuma votação no momento.'), findsOneWidget);
  });

  testWidgets('GovernanceScreen lists a voting with vote action',
      (WidgetTester tester) async {
    final voting = Voting(
      id: 'v1',
      territoryId: 't1',
      createdByUserId: 'u1',
      type: 'CommunityPolicy',
      title: 'Horário do silêncio',
      description: 'Definir horário.',
      options: const ['22h', '23h'],
      visibility: 'AllMembers',
      status: 'Open',
      createdAtUtc: DateTime(2026, 1, 1),
      updatedAtUtc: DateTime(2026, 1, 1),
    );

    await tester.pumpWidget(buildApp([voting]));
    await tester.pumpAndSettle();

    expect(find.text('Horário do silêncio'), findsOneWidget);
    expect(find.text('22h'), findsOneWidget);
    expect(find.text('Votar'), findsOneWidget);
  });
}
