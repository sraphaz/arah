import 'package:arah_app/features/feed/presentation/providers/feed_provider.dart';
import 'package:arah_app/features/feed/presentation/screens/post_detail_screen.dart';
import 'package:arah_app/l10n/app_localizations.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

/// Notifier falso: não carrega da rede; expõe um estado de feed controlado.
class _FakeFeedNotifier extends FeedNotifier {
  _FakeFeedNotifier(Ref ref, List<dynamic> items) : super(ref, null) {
    state = FeedState(items: items);
  }
}

void main() {
  testWidgets('PostDetailScreen renders the post content from feed state',
      (WidgetTester tester) async {
    final item = {
      'post': {
        'id': 'p1',
        'title': 'Mutirão de limpeza',
        'content': 'Encontro da comunidade na praça.',
        'type': 'general',
        'createdAtUtc': '2026-07-01T12:00:00Z',
      },
      'counts': {'likes': 2, 'comments': 1, 'shares': 0},
      'userInteractions': {'liked': false, 'shared': false},
      'metadata': {'canDelete': false},
      'author': {'displayName': 'Maria'},
      'media': <dynamic>[],
    };

    await tester.pumpWidget(
      ProviderScope(
        overrides: [
          feedNotifierProvider('t1').overrideWith((ref) => _FakeFeedNotifier(ref, [item])),
        ],
        child: MaterialApp(
          locale: const Locale('pt'),
          localizationsDelegates: AppLocalizations.localizationsDelegates,
          supportedLocales: AppLocalizations.supportedLocales,
          home: const PostDetailScreen(territoryId: 't1', postId: 'p1'),
        ),
      ),
    );
    await tester.pumpAndSettle();

    expect(find.text('Publicação'), findsOneWidget);
    expect(find.text('Mutirão de limpeza'), findsOneWidget);
    expect(find.text('Encontro da comunidade na praça.'), findsOneWidget);
    expect(find.text('Maria'), findsOneWidget);
  });
}
