import 'package:arah_app/core/theme/app_theme.dart';
import 'package:arah_app/features/feed/presentation/widgets/feed_post_card.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  testWidgets('FeedPostCard mantém o menu de ações visível em alertas largos',
      (tester) async {
    var morePressed = false;

    await tester.pumpWidget(
      MaterialApp(
        theme: AppTheme.dark,
        home: Scaffold(
          body: Center(
            child: SizedBox(
              width: 360,
              child: FeedPostCard(
                title: 'Mutirão de limpeza',
                authorName: 'Conselho de Moradores',
                type: FeedPostType.alert,
                onMorePressed: () => morePressed = true,
              ),
            ),
          ),
        ),
      ),
    );

    expect(find.text('Alerta'), findsOneWidget);
    expect(find.byIcon(Icons.more_horiz), findsOneWidget);

    await tester.tap(find.byIcon(Icons.more_horiz));
    await tester.pump();

    expect(morePressed, isTrue);
  });
}
