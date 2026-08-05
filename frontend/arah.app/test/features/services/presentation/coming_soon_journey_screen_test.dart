import 'package:arah_app/features/services/presentation/screens/coming_soon_journey_screen.dart';
import 'package:arah_app/l10n/app_localizations.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  testWidgets('ComingSoonJourneyScreen mostra título e fase',
      (WidgetTester tester) async {
    await tester.pumpWidget(
      MaterialApp(
        locale: const Locale('pt'),
        localizationsDelegates: AppLocalizations.localizationsDelegates,
        supportedLocales: AppLocalizations.supportedLocales,
        home: const ComingSoonJourneyScreen(
          title: 'Compras coletivas',
          phase: 'F17',
        ),
      ),
    );
    await tester.pumpAndSettle();

    expect(find.text('Em breve'), findsWidgets);
    expect(find.text('Compras coletivas'), findsOneWidget);
    expect(find.textContaining('F17'), findsOneWidget);
  });
}
