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
    expect(find.textContaining('Em breve'), findsWidgets);
  });
}
