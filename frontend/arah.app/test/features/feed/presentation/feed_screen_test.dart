import 'package:arah_app/core/config/app_config.dart';
import 'package:arah_app/core/network/bff_client.dart';
import 'package:arah_app/core/providers/app_providers.dart';
import 'package:arah_app/core/providers/territory_provider.dart';
import 'package:arah_app/core/widgets/arah_error_state.dart';
import 'package:arah_app/features/feed/presentation/providers/feed_provider.dart';
import 'package:arah_app/features/feed/presentation/screens/feed_screen.dart';
import 'package:arah_app/l10n/app_localizations.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';

class _FakeFeedNotifier extends FeedNotifier {
  _FakeFeedNotifier(Ref ref, FeedState initialState) : super(ref, null) {
    state = initialState;
  }

  @override
  Future<void> refresh() async {}
}

class _FakeBffClient extends BffClient {
  _FakeBffClient()
      : super(config: const AppConfig(bffBaseUrl: 'http://localhost'));

  @override
  Future<BffResponse> get(
    String journey,
    String pathAndQuery, {
    String? sessionIdOverride,
    Map<String, dynamic>? queryParameters,
  }) async {
    return BffResponse(
      statusCode: 200,
      data: const {
        'id': 'm1',
        'userId': 'u1',
        'territoryId': 't1',
        'role': 'VISITOR',
      },
    );
  }
}

void main() {
  testWidgets('FeedScreen keeps the empty-state error centered',
      (WidgetTester tester) async {
    await tester.pumpWidget(
      ProviderScope(
        overrides: [
          selectedTerritoryIdValueProvider.overrideWith((ref) => 't1'),
          bffClientProvider.overrideWithValue(_FakeBffClient()),
          feedNotifierProvider('t1').overrideWith(
            (ref) =>
                _FakeFeedNotifier(ref, FeedState(error: StateError('falha'))),
          ),
        ],
        child: MaterialApp(
          locale: const Locale('pt'),
          localizationsDelegates: AppLocalizations.localizationsDelegates,
          supportedLocales: AppLocalizations.supportedLocales,
          home: const Scaffold(body: FeedScreen()),
        ),
      ),
    );
    await tester.pumpAndSettle();

    expect(find.byType(ArahErrorState), findsOneWidget);
    expect(
      find.ancestor(
        of: find.byType(ArahErrorState),
        matching: find.byType(Center),
      ),
      findsOneWidget,
    );
  });
}
