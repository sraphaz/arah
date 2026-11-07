import 'package:arah_app/core/providers/territory_provider.dart';
import 'package:arah_app/core/theme/app_theme.dart';
import 'package:arah_app/features/feed/presentation/providers/feed_provider.dart';
import 'package:arah_app/features/feed/presentation/screens/feed_screen.dart';
import 'package:arah_app/features/membership/data/models/membership_info.dart';
import 'package:arah_app/features/membership/presentation/providers/membership_provider.dart';
import 'package:arah_app/features/territories/presentation/providers/territories_list_provider.dart';
import 'package:arah_app/l10n/app_localizations.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';

class _FakeFeedNotifier extends FeedNotifier {
  _FakeFeedNotifier(Ref ref, List<dynamic> items, {bool hasMore = false})
      : super(ref, null) {
    state = FeedState(items: items, hasMore: hasMore);
  }
}

class _FakeMembershipNotifier extends MembershipNotifier {
  _FakeMembershipNotifier(super.ref, MembershipInfo membership) {
    state = MembershipState(membership: membership);
  }

  @override
  Future<void> refresh() async {}
}

Widget _buildApp({MediaQueryData? mediaQuery, bool hasMore = false}) {
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
    {
      'post': {
        'id': 'p2',
        'title': 'Chegou robalo fresco na peixaria',
        'content': 'Pescado da manhã pelo seu Tião.',
        'type': 'general',
        'visibility': 'Public',
        'createdAtUtc': DateTime.now()
            .toUtc()
            .subtract(const Duration(hours: 5))
            .toIso8601String(),
      },
      'counts': {'likes': 38, 'comments': 7, 'shares': 0},
      'userInteractions': {'liked': false, 'shared': false},
      'metadata': {'canDelete': false},
      'author': {
        'displayName': 'Dona Marta',
        'handle': 'marta.peixaria',
        'membership': 'MORADOR',
      },
      'media': <dynamic>[],
    },
    {
      'post': {
        'id': 'p3',
        'title': 'Roda de conversa: água e saneamento',
        'content': 'Próxima terça, 19h, no centro comunitário.',
        'type': 'general',
        'visibility': 'ResidentsOnly',
        'createdAtUtc': DateTime.now()
            .toUtc()
            .subtract(const Duration(days: 1))
            .toIso8601String(),
      },
      'counts': {'likes': 91, 'comments': 28, 'shares': 0},
      'userInteractions': {'liked': false, 'shared': false},
      'metadata': {'canDelete': false},
      'author': {
        'displayName': 'Coletivo Maré',
        'handle': 'coletivo.mare',
        'membership': 'MORADOR',
      },
      'media': <dynamic>[],
    },
  ];

  final child = ProviderScope(
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
      feedNotifierProvider('t1').overrideWith(
        (ref) => _FakeFeedNotifier(ref, items, hasMore: hasMore),
      ),
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
    ],
    child: MaterialApp(
      locale: const Locale('pt'),
      theme: AppTheme.dark,
      localizationsDelegates: AppLocalizations.localizationsDelegates,
      supportedLocales: AppLocalizations.supportedLocales,
      home: const Scaffold(body: FeedScreen()),
    ),
  );

  if (mediaQuery == null) return child;
  return MediaQuery(data: mediaQuery, child: child);
}

void main() {
  testWidgets('FeedScreen filtra avisos e vizinhança', (tester) async {
    tester.view.physicalSize = const Size(780, 1688);
    tester.view.devicePixelRatio = 2;
    addTearDown(() {
      tester.view.resetPhysicalSize();
      tester.view.resetDevicePixelRatio();
    });

    await tester.pumpWidget(_buildApp());
    await tester.pumpAndSettle();

    expect(
        find.text('Mutirão de limpeza no costão neste sábado'), findsOneWidget);
    expect(find.text('Chegou robalo fresco na peixaria'), findsOneWidget);

    await tester.tap(find.text('Avisos').first);
    await tester.pumpAndSettle();
    expect(
        find.text('Mutirão de limpeza no costão neste sábado'), findsOneWidget);
    expect(find.text('Chegou robalo fresco na peixaria'), findsNothing);

    await tester.tap(find.text('Vizinhança'));
    await tester.pumpAndSettle();
    expect(find.text('Chegou robalo fresco na peixaria'), findsOneWidget);
    expect(find.text('Roda de conversa: água e saneamento'), findsNothing);
  });

  testWidgets('FeedScreen permanece utilizável em viewport mobile estreita',
      (tester) async {
    tester.view.physicalSize = const Size(720, 1560);
    tester.view.devicePixelRatio = 2;
    addTearDown(() {
      tester.view.resetPhysicalSize();
      tester.view.resetDevicePixelRatio();
    });

    await tester.pumpWidget(
      _buildApp(
        mediaQuery: const MediaQueryData(
          size: Size(360, 780),
          devicePixelRatio: 2,
          textScaler: TextScaler.linear(1.1),
        ),
      ),
    );
    await tester.pumpAndSettle();

    expect(tester.takeException(), isNull);
    expect(find.text('Você é visitante de Camburi'), findsOneWidget);
    expect(
        find.text('Mutirão de limpeza no costão neste sábado'), findsOneWidget);
  });

  testWidgets(
      'FeedScreen só mostra o rodapé de conclusão quando não há mais páginas',
      (tester) async {
    tester.view.physicalSize = const Size(780, 1688);
    tester.view.devicePixelRatio = 2;
    addTearDown(() {
      tester.view.resetPhysicalSize();
      tester.view.resetDevicePixelRatio();
    });

    await tester.pumpWidget(_buildApp(hasMore: true));
    await tester.pumpAndSettle();

    expect(find.textContaining('Você viu tudo de Camburi'), findsNothing);
  });
}
