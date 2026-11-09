import 'package:arah_app/core/geo/geo_location_provider.dart';
import 'package:arah_app/core/providers/territory_provider.dart';
import 'package:arah_app/core/theme/app_theme.dart';
import 'package:arah_app/features/feed/presentation/providers/feed_provider.dart';
import 'package:arah_app/features/home/presentation/screens/main_shell_screen.dart';
import 'package:arah_app/features/membership/data/models/membership_info.dart';
import 'package:arah_app/features/membership/presentation/providers/membership_provider.dart';
import 'package:arah_app/features/notifications/data/models/notification_item.dart';
import 'package:arah_app/features/notifications/presentation/providers/notifications_provider.dart';
import 'package:arah_app/features/territories/presentation/providers/territories_list_provider.dart';
import 'package:arah_app/l10n/app_localizations.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';

class _FakeGeoLocationNotifier extends GeoLocationNotifier {
  @override
  Future<void> fetch() async {}
}

class _FakeFeedNotifier extends FeedNotifier {
  _FakeFeedNotifier(Ref ref, List<dynamic> items) : super(ref, null) {
    state = FeedState(items: items);
  }
}

class _FakeMembershipNotifier extends MembershipNotifier {
  _FakeMembershipNotifier(Ref ref, MembershipInfo membership) : super(ref) {
    state = MembershipState(membership: membership);
  }

  @override
  Future<void> refresh() async {}
}

class _FakeNotificationsNotifier extends NotificationsNotifier {
  _FakeNotificationsNotifier(Ref ref, List<NotificationItem> items)
      : super(ref) {
    state = NotificationsState(items: items);
  }

  @override
  Future<void> loadPage(int pageNumber, {required bool append}) async {}
}

Widget _buildApp() {
  final items = <dynamic>[
    {
      'post': {
        'id': 'p1',
        'title': 'Mutirão de limpeza no costão neste sábado',
        'content': 'Encontro às 8h na praça da capela. Traga luvas e água.',
        'type': 'alert',
        'visibility': 'Public',
        'createdAtUtc': DateTime.now()
            .toUtc()
            .subtract(const Duration(hours: 2))
            .toIso8601String(),
        'isHighlighted': true,
      },
      'counts': {'likes': 64, 'comments': 12, 'shares': 0},
      'userInteractions': {'liked': false, 'shared': false},
      'metadata': {'canDelete': false, 'isHighlighted': true},
      'author': {
        'displayName': 'Conselho de Moradores',
        'handle': 'conselho.camburi',
        'membership': 'MORADOR',
      },
      'media': <dynamic>[],
    },
  ];

  return ProviderScope(
    overrides: [
      selectedTerritoryIdValueProvider.overrideWith((ref) => 't1'),
      territoriesListProvider.overrideWith(
        (ref) async => const [
          TerritoryItem(
            id: 't1',
            name: 'Camburi',
            region: 'São Sebastião · SP',
            description: 'Vila caiçara entre a mata e o mar.',
            memberCount: 1284,
          ),
        ],
      ),
      geoLocationStateProvider
          .overrideWith((ref) => _FakeGeoLocationNotifier()),
      feedNotifierProvider('t1')
          .overrideWith((ref) => _FakeFeedNotifier(ref, items)),
      membershipProvider.overrideWith(
        (ref) => _FakeMembershipNotifier(
          ref,
          const MembershipInfo(
            id: 'm1',
            userId: 'u1',
            territoryId: 't1',
            role: 'VISITOR',
          ),
        ),
      ),
      notificationsProvider.overrideWith(
        (ref) => _FakeNotificationsNotifier(
          ref,
          [
            NotificationItem(
              id: 'n1',
              title: 'Novo alerta no território',
              kind: 'alert',
              createdAtUtc: DateTime.now().toUtc(),
            ),
          ],
        ),
      ),
    ],
    child: MaterialApp(
      locale: const Locale('pt'),
      theme: AppTheme.dark,
      localizationsDelegates: AppLocalizations.localizationsDelegates,
      supportedLocales: AppLocalizations.supportedLocales,
      home: const MainShellScreen(),
    ),
  );
}

void main() {
  testWidgets(
      'MainShellScreen mostra top bar territorial e bottom-nav canônica',
      (tester) async {
    tester.view.physicalSize = const Size(780, 1688);
    tester.view.devicePixelRatio = 2;
    addTearDown(() {
      tester.view.resetPhysicalSize();
      tester.view.resetDevicePixelRatio();
    });

    await tester.pumpWidget(_buildApp());
    await tester.pumpAndSettle();

    expect(find.text('Camburi'), findsOneWidget);
    expect(find.text('Feed'), findsOneWidget);
    expect(find.text('Explorar'), findsOneWidget);
    expect(find.text('Publicar'), findsOneWidget);
    expect(find.text('Serviços'), findsOneWidget);
    expect(find.text('Perfil'), findsOneWidget);
    expect(find.text('Você é visitante de Camburi'), findsOneWidget);
    expect(
        find.text('Mutirão de limpeza no costão neste sábado'), findsOneWidget);
  });
}
