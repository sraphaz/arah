import 'package:arah_app/core/providers/territory_provider.dart';
import 'package:arah_app/features/explore/presentation/screens/explore_screen.dart';
import 'package:arah_app/features/territories/presentation/providers/territories_list_provider.dart';
import 'package:arah_app/l10n/app_localizations.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

void main() {
  testWidgets('ExploreScreen shows territories section and services CTA', (WidgetTester tester) async {
    await tester.pumpWidget(
      ProviderScope(
        overrides: [
          territoriesListProvider.overrideWith((ref) async => []),
          selectedTerritoryIdValueProvider.overrideWith((ref) => null),
        ],
        child: MaterialApp(
          locale: const Locale('pt'),
          localizationsDelegates: AppLocalizations.localizationsDelegates,
          supportedLocales: AppLocalizations.supportedLocales,
          // Shell já fornece Material/TopBar; Explore é conteúdo embutido.
          home: const Scaffold(body: ExploreScreen()),
        ),
      ),
    );
    await tester.pumpAndSettle();

    expect(find.text('Explorar'), findsOneWidget);
    expect(find.text('Territórios'), findsOneWidget);
    expect(find.text('Serviços'), findsOneWidget);
  });
}
