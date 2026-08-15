import 'package:arah_app/features/marketplace/presentation/providers/marketplace_provider.dart';
import 'package:arah_app/features/marketplace/presentation/screens/checkout_journey_screen.dart';
import 'package:arah_app/l10n/app_localizations.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';

class _EmptyCartNotifier extends MarketplaceNotifier {
  _EmptyCartNotifier(Ref ref) : super(ref) {
    state = const MarketplaceState(
      cart: {'items': <dynamic>[]},
    );
  }
}

void main() {
  testWidgets('CheckoutJourneyScreen mostra sacola vazia no passo 1',
      (WidgetTester tester) async {
    await tester.pumpWidget(
      ProviderScope(
        overrides: [
          marketplaceProvider.overrideWith((ref) => _EmptyCartNotifier(ref)),
        ],
        child: MaterialApp(
          locale: const Locale('pt'),
          localizationsDelegates: AppLocalizations.localizationsDelegates,
          supportedLocales: AppLocalizations.supportedLocales,
          home: const CheckoutJourneyScreen(),
        ),
      ),
    );
    await tester.pumpAndSettle();

    expect(find.text('Checkout'), findsOneWidget);
    expect(find.textContaining('sacola está vazia'), findsOneWidget);
  });
}
