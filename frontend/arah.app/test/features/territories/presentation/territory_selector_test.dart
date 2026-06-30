import 'package:arah_app/core/providers/territory_provider.dart';
import 'package:arah_app/features/territories/presentation/providers/territories_list_provider.dart';
import 'package:arah_app/features/territories/presentation/widgets/territory_selector.dart';
import 'package:arah_app/l10n/app_localizations.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

void main() {
  testWidgets('TerritorySelector shows a search field and the territory list',
      (WidgetTester tester) async {
    await tester.pumpWidget(
      ProviderScope(
        overrides: [
          territoriesListProvider.overrideWith((ref) async => const [
                TerritoryItem(id: 't1', name: 'Sertão do Camburi'),
                TerritoryItem(id: 't2', name: 'Vale do Itamambuca'),
              ]),
          selectedTerritoryIdValueProvider.overrideWith((ref) => 't1'),
        ],
        child: MaterialApp(
          locale: const Locale('pt'),
          localizationsDelegates: AppLocalizations.localizationsDelegates,
          supportedLocales: AppLocalizations.supportedLocales,
          home: const Scaffold(body: TerritorySelector()),
        ),
      ),
    );
    await tester.pumpAndSettle();

    // Campo de busca presente.
    expect(find.text('Buscar território por nome ou cidade'), findsOneWidget);
    expect(find.byType(TextField), findsOneWidget);
    // Lista padrão (sem busca) renderiza os territórios.
    expect(find.text('Sertão do Camburi'), findsOneWidget);
    expect(find.text('Vale do Itamambuca'), findsOneWidget);
  });
}
