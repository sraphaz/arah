import 'package:arah_app/features/events/data/models/event_item.dart';
import 'package:arah_app/features/events/data/repositories/events_repository.dart';
import 'package:arah_app/features/events/presentation/providers/territory_events_provider.dart';
import 'package:arah_app/features/events/presentation/screens/create_event_screen.dart';
import 'package:arah_app/l10n/app_localizations.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

/// Fake repository que registra a criação e devolve listas controladas, sem rede.
class _FakeEventsRepository implements EventsRepository {
  EventItem? lastCreated;
  Map<String, dynamic>? lastArgs;

  @override
  Future<EventsPage> getTerritoryEvents({
    required String territoryId,
    int pageNumber = 1,
    int pageSize = 20,
    DateTime? from,
    DateTime? to,
    String? status,
  }) async {
    return EventsPage(
      items: lastCreated != null ? [lastCreated!] : const [],
      hasMore: false,
      pageNumber: pageNumber,
    );
  }

  @override
  Future<void> participate({required String eventId, required String status}) async {}

  @override
  Future<EventItem> createEvent({
    required String territoryId,
    required String title,
    String? description,
    required DateTime startsAtUtc,
    DateTime? endsAtUtc,
    double? latitude,
    double? longitude,
    String? locationLabel,
  }) async {
    lastArgs = {
      'territoryId': territoryId,
      'title': title,
      'startsAtUtc': startsAtUtc,
      'endsAtUtc': endsAtUtc,
      'locationLabel': locationLabel,
    };
    lastCreated = EventItem(
      id: 'e-new',
      territoryId: territoryId,
      title: title,
      description: description,
      startsAtUtc: startsAtUtc,
      endsAtUtc: endsAtUtc ?? startsAtUtc,
      status: 'SCHEDULED',
    );
    return lastCreated!;
  }
}

void main() {
  testWidgets('CreateEventScreen renders the form fields', (tester) async {
    await tester.pumpWidget(
      const ProviderScope(
        child: MaterialApp(
          locale: Locale('pt'),
          localizationsDelegates: AppLocalizations.localizationsDelegates,
          supportedLocales: AppLocalizations.supportedLocales,
          home: CreateEventScreen(territoryId: 't1'),
        ),
      ),
    );
    await tester.pumpAndSettle();

    expect(find.text('Novo evento'), findsWidgets);
    expect(find.text('Título do evento'), findsOneWidget);
    expect(find.text('Início'), findsOneWidget);
    expect(find.text('Criar'), findsOneWidget);
  });

  test('TerritoryEventsNotifier.createEvent calls repo and reloads', () async {
    final fake = _FakeEventsRepository();
    final container = ProviderContainer(
      overrides: [eventsRepositoryProvider.overrideWithValue(fake)],
    );
    addTearDown(container.dispose);

    final notifier = container.read(territoryEventsProvider('t1').notifier);
    final start = DateTime.utc(2026, 7, 1, 19, 0);

    final created = await notifier.createEvent(
      title: 'Feira comunitária',
      description: 'Descrição',
      startsAtUtc: start,
      locationLabel: 'Praça Central',
    );

    expect(created.id, 'e-new');
    expect(fake.lastArgs!['territoryId'], 't1');
    expect(fake.lastArgs!['title'], 'Feira comunitária');
    // Após criar, a lista é recarregada e passa a conter o evento criado.
    expect(container.read(territoryEventsProvider('t1')).items.single.title,
        'Feira comunitária');
  });
}
