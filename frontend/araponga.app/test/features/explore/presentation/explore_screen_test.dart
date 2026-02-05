import 'package:araponga_app/core/providers/territory_provider.dart';
import 'package:araponga_app/features/explore/presentation/screens/explore_screen.dart';
import 'package:araponga_app/features/territories/presentation/providers/territories_list_provider.dart';
import 'package:araponga_app/l10n/app_localizations.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

void main() {
  testWidgets('ExploreScreen shows title and territories section', (WidgetTester tester) async {
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
          home: const ExploreScreen(),
        ),
      ),
    );
    await tester.pumpAndSettle();

    expect(find.text('Explorar'), findsOneWidget);
    expect(find.text('Territórios'), findsOneWidget);
  });
}
