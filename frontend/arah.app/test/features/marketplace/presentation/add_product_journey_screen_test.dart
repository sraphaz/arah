import 'dart:async';

import 'package:arah_app/core/theme/app_theme.dart';
import 'package:arah_app/features/marketplace/data/models/store_item.dart';
import 'package:arah_app/features/marketplace/data/models/store_product.dart';
import 'package:arah_app/features/marketplace/presentation/providers/marketplace_provider.dart';
import 'package:arah_app/features/marketplace/presentation/screens/add_product_journey_screen.dart';
import 'package:arah_app/l10n/app_localizations.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';

class _PendingCreateMarketplaceNotifier extends MarketplaceNotifier {
  _PendingCreateMarketplaceNotifier(Ref ref) : super(ref) {
    state = const MarketplaceState(
      myStore: MyStore(
        id: 'store-1',
        territoryId: 'territory-1',
        displayName: 'Feira do bairro',
        status: 'ACTIVE',
      ),
    );
  }

  final Completer<void> createCompleter = Completer<void>();

  @override
  Future<StoreProduct> createProduct({
    required String title,
    String? description,
    String? category,
    required String pricingType,
    double? priceAmount,
    String currency = 'BRL',
    List<String>? mediaIds,
  }) async {
    await createCompleter.future;
    return StoreProduct(
      id: 'item-1',
      territoryId: 'territory-1',
      storeId: 'store-1',
      type: 'Product',
      title: title,
      description: description,
      category: category,
      pricingType: pricingType,
      priceAmount: priceAmount,
      currency: currency,
      status: 'Active',
    );
  }

  void completeCreate() {
    if (!createCompleter.isCompleted) {
      createCompleter.complete();
    }
  }
}

Widget _buildTestApp(List<Override> overrides) {
  return ProviderScope(
    overrides: overrides,
    child: MaterialApp(
      theme: AppTheme.dark,
      locale: const Locale('pt'),
      localizationsDelegates: AppLocalizations.localizationsDelegates,
      supportedLocales: AppLocalizations.supportedLocales,
      home: Builder(
        builder: (context) {
          return Scaffold(
            body: Center(
              child: ElevatedButton(
                onPressed: () {
                  Navigator.of(context).push(
                    MaterialPageRoute<void>(
                      builder: (_) => const AddProductJourneyScreen(),
                    ),
                  );
                },
                child: const Text('Abrir'),
              ),
            ),
          );
        },
      ),
    ),
  );
}

void main() {
  testWidgets(
    'AddProductJourneyScreen desabilita voltar enquanto salva',
    (WidgetTester tester) async {
      late _PendingCreateMarketplaceNotifier notifier;

      await tester.pumpWidget(
        _buildTestApp([
          marketplaceProvider.overrideWith((ref) {
            notifier = _PendingCreateMarketplaceNotifier(ref);
            return notifier;
          }),
        ]),
      );
      addTearDown(() => notifier.completeCreate());
      await tester.pumpAndSettle();

      await tester.tap(find.text('Abrir'));
      await tester.pumpAndSettle();

      await tester.enterText(find.byType(TextField).at(0), 'Banana prata');
      await tester.enterText(find.byType(TextField).at(2), '10');
      await tester.pump();

      // Detalhes → foto/descrição
      tester.widget<FilledButton>(find.byType(FilledButton)).onPressed!.call();
      await tester.pumpAndSettle();

      // Foto/descrição → revisão
      tester.widget<FilledButton>(find.byType(FilledButton)).onPressed!.call();
      await tester.pumpAndSettle();

      expect(find.text('Revisão'), findsOneWidget);
      expect(find.byIcon(Icons.arrow_back), findsOneWidget);

      // Publicar (create fica pendente)
      tester.widget<FilledButton>(find.byType(FilledButton)).onPressed!.call();
      await tester.pump();

      // Com primaryLoading: sem voltar; botão de fechar fica desabilitado.
      expect(find.byIcon(Icons.arrow_back), findsNothing);
      expect(find.byIcon(Icons.close), findsOneWidget);
      final closeButton = tester.widget<IconButton>(
        find.widgetWithIcon(IconButton, Icons.close),
      );
      expect(closeButton.onPressed, isNull);
    },
  );
}
