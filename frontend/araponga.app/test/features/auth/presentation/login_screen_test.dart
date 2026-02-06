import 'package:araponga_app/core/config/app_config.dart';
import 'package:araponga_app/core/storage/secure_storage_service.dart';
import 'package:araponga_app/features/auth/data/repositories/auth_repository.dart';
import 'package:araponga_app/features/auth/presentation/providers/auth_state_provider.dart';
import 'package:araponga_app/features/auth/presentation/screens/login_screen.dart';
import 'package:araponga_app/l10n/app_localizations.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';

class _FakeAuthStateNotifier extends AuthStateNotifier {
  _FakeAuthStateNotifier();
  @override
  Future<AuthSession?> build() async => null;
}

class _FakeStorage extends SecureStorageService {
  @override
  Future<void> writeAccessToken(String value) async {}
  @override
  Future<void> writeRefreshToken(String value) async {}
  @override
  Future<void> writeTokenExpiry(String value) async {}
  @override
  Future<String?> readAccessToken() async => null;
  @override
  Future<String?> readRefreshToken() async => null;
  @override
  Future<String?> readTokenExpiry() async => null;
  @override
  Future<void> clearAuth() async {}
}

class FakeAuthRepository extends AuthRepository {
  FakeAuthRepository({
    required super.config,
    required super.secureStorage,
    this.checkEmailExistsResult = false,
  });
  final bool checkEmailExistsResult;
  @override
  Future<bool> checkEmailExists(String email) async => checkEmailExistsResult;
  @override
  Future<AuthSession?> restoreSession() async => null;
  @override
  Future<void> logout() async {}
}

void main() {
  testWidgets('LoginScreen shows app title and Continuar on email step', (WidgetTester tester) async {
    await tester.pumpWidget(
      ProviderScope(
        overrides: [
          authStateProvider.overrideWith(() => _FakeAuthStateNotifier()),
        ],
        child: MaterialApp(
          locale: const Locale('pt'),
          localizationsDelegates: AppLocalizations.localizationsDelegates,
          supportedLocales: AppLocalizations.supportedLocales,
          home: const LoginScreen(bffBaseUrl: 'http://test'),
        ),
      ),
    );
    await tester.pumpAndSettle();

    expect(find.text('Ará'), findsOneWidget);
    expect(find.text('Continuar'), findsOneWidget);
    expect(find.byType(TextFormField), findsWidgets);
  });

  testWidgets('LoginScreen shows signup form when email does not exist', (WidgetTester tester) async {
    final fakeRepo = FakeAuthRepository(
      config: const AppConfig(bffBaseUrl: 'http://test'),
      secureStorage: _FakeStorage(),
      checkEmailExistsResult: false,
    );
    await tester.pumpWidget(
      ProviderScope(
        overrides: [
          authRepositoryProvider.overrideWithValue(fakeRepo),
        ],
        child: MaterialApp(
          locale: const Locale('pt'),
          localizationsDelegates: AppLocalizations.localizationsDelegates,
          supportedLocales: AppLocalizations.supportedLocales,
          home: const LoginScreen(bffBaseUrl: 'http://test'),
        ),
      ),
    );
    await tester.pumpAndSettle();

    await tester.enterText(find.byType(TextFormField).first, 'new@example.com');
    await tester.tap(find.text('Continuar'));
    await tester.pumpAndSettle();

    expect(find.text('Criar conta'), findsOneWidget);
    expect(find.text('Confirmar senha'), findsOneWidget);
  });
}
