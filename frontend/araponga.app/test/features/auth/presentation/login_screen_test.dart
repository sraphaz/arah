import 'package:araponga_app/features/auth/data/repositories/auth_repository.dart';
import 'package:araponga_app/features/auth/presentation/providers/auth_state_provider.dart';
import 'package:araponga_app/features/auth/presentation/screens/login_screen.dart';
import 'package:araponga_app/l10n/app_localizations.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

class _FakeAuthStateNotifier extends AuthStateNotifier {
  _FakeAuthStateNotifier();
  @override
  Future<AuthSession?> build() async => null;
}

void main() {
  testWidgets('LoginScreen shows app title and login button', (WidgetTester tester) async {
    await tester.pumpWidget(
      ProviderScope(
        overrides: [
          authStateProvider.overrideWith(() => _FakeAuthStateNotifier()),
        ],
        child: MaterialApp(
          localizationsDelegates: AppLocalizations.localizationsDelegates,
          supportedLocales: AppLocalizations.supportedLocales,
          home: const LoginScreen(bffBaseUrl: 'http://test'),
        ),
      ),
    );
    await tester.pumpAndSettle();

    expect(find.text('Ará'), findsOneWidget);
    expect(find.text('Entrar'), findsOneWidget);
    expect(find.byType(TextFormField), findsWidgets);
  });
}
