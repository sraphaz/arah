import 'package:arah_app/features/services/presentation/screens/services_hub_screen.dart';
import 'package:arah_app/l10n/app_localizations.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  testWidgets('ServicesHubScreen mostra categorias live e Em breve',
      (WidgetTester tester) async {
    await tester.pumpWidget(
      MaterialApp(
        locale: const Locale('pt'),
        localizationsDelegates: AppLocalizations.localizationsDelegates,
        supportedLocales: AppLocalizations.supportedLocales,
        home: const Scaffold(body: ServicesHubScreen()),
      ),
    );
    await tester.pumpAndSettle();

    expect(find.text('Serviços'), findsWidgets);
    expect(find.text('Marketplace'), findsOneWidget);

    // Itens "Em breve" ficam abaixo do fold no ListView lazy — rola até achar.
    await tester.scrollUntilVisible(
      find.text('F17'),
      200,
      scrollable: find.byType(Scrollable).first,
    );
    expect(find.text('Em breve'), findsWidgets);
    expect(find.text('F17'), findsOneWidget);
  });
}
