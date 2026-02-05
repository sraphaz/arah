import 'package:araponga_app/features/auth/data/models/auth_models.dart';
import 'package:araponga_app/features/auth/data/repositories/auth_repository.dart';
import 'package:araponga_app/features/auth/presentation/providers/auth_state_provider.dart';
import 'package:araponga_app/features/profile/data/models/me_profile.dart';
import 'package:araponga_app/features/profile/presentation/providers/me_profile_provider.dart';
import 'package:araponga_app/features/profile/presentation/screens/profile_screen.dart';
import 'package:araponga_app/l10n/app_localizations.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

class _FakeAuthStateNotifierWithSession extends AuthStateNotifier {
  _FakeAuthStateNotifierWithSession();
  @override
  Future<AuthSession?> build() async => AuthSession(
        accessToken: 'test-token',
        user: const AuthUser(id: '1', displayName: 'Test User', email: 'test@test.com'),
      );
}

void main() {
  testWidgets('ProfileScreen when logged in shows profile and app bar', (WidgetTester tester) async {
    final profile = MeProfile(
      id: '1',
      displayName: 'Test User',
      email: 'test@test.com',
      createdAtUtc: DateTime.utc(2024, 1, 1),
    );

    await tester.pumpWidget(
      ProviderScope(
        overrides: [
          authStateProvider.overrideWith(() => _FakeAuthStateNotifierWithSession()),
          meProfileProvider.overrideWith((ref) async => profile),
        ],
        child: MaterialApp(
          localizationsDelegates: AppLocalizations.localizationsDelegates,
          supportedLocales: AppLocalizations.supportedLocales,
          home: const ProfileScreen(),
        ),
      ),
    );
    await tester.pumpAndSettle();

    expect(find.text('Perfil'), findsOneWidget);
    expect(find.text('Test User'), findsOneWidget);
  });
}
