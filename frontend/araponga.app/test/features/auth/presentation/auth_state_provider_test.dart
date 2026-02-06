import 'package:araponga_app/core/config/app_config.dart';
import 'package:araponga_app/core/storage/secure_storage_service.dart';
import 'package:araponga_app/features/auth/data/models/auth_models.dart';
import 'package:araponga_app/features/auth/data/repositories/auth_repository.dart';
import 'package:araponga_app/features/auth/presentation/providers/auth_state_provider.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';

/// Fake storage que não persiste (para testes).
class _FakeSecureStorage extends SecureStorageService {
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

/// Repositório fake para testar o AuthStateNotifier (checkEmailExists, login, signUp).
class FakeAuthRepository extends AuthRepository {
  FakeAuthRepository({
    required super.config,
    required super.secureStorage,
    this.checkEmailExistsResult = false,
    this.loginSession,
    this.signUpSession,
    this.shouldThrowLogin = false,
    this.shouldThrowSignUp = false,
    this.shouldThrowCheckEmail = false,
  });

  final bool checkEmailExistsResult;
  final AuthSession? loginSession;
  final AuthSession? signUpSession;
  final bool shouldThrowLogin;
  final bool shouldThrowSignUp;
  final bool shouldThrowCheckEmail;

  @override
  Future<bool> checkEmailExists(String email) async {
    if (shouldThrowCheckEmail) throw Exception('check-email failed');
    return checkEmailExistsResult;
  }

  @override
  Future<AuthSession> login({required String email, required String password}) async {
    if (shouldThrowLogin) throw Exception('login failed');
    final s = loginSession;
    if (s == null) throw Exception('no session');
    return s;
  }

  @override
  Future<AuthSession> signUp({
    required String email,
    required String displayName,
    required String password,
  }) async {
    if (shouldThrowSignUp) throw Exception('signup failed');
    final s = signUpSession;
    if (s == null) throw Exception('no session');
    return s;
  }

  @override
  Future<AuthSession?> restoreSession() async => null;

  @override
  Future<void> logout() async {}
}

void main() {
  const config = AppConfig(bffBaseUrl: 'http://test');

  group('AuthStateNotifier', () {
    test('checkEmailExists returns true when repo returns true', () async {
      final fakeRepo = FakeAuthRepository(
        config: config,
        secureStorage: _FakeSecureStorage(),
        checkEmailExistsResult: true,
      );
      final container = ProviderContainer(
        overrides: [
          authRepositoryProvider.overrideWithValue(fakeRepo),
        ],
      );
      addTearDown(container.dispose);

      final notifier = container.read(authStateProvider.notifier);
      final result = await notifier.checkEmailExists('user@example.com');

      expect(result, isTrue);
    });

    test('checkEmailExists returns false when repo returns false', () async {
      final fakeRepo = FakeAuthRepository(
        config: config,
        secureStorage: _FakeSecureStorage(),
        checkEmailExistsResult: false,
      );
      final container = ProviderContainer(
        overrides: [
          authRepositoryProvider.overrideWithValue(fakeRepo),
        ],
      );
      addTearDown(container.dispose);

      final notifier = container.read(authStateProvider.notifier);
      final result = await notifier.checkEmailExists('new@example.com');

      expect(result, isFalse);
    });

    test('login sets state to session when repo returns session', () async {
      const session = AuthSession(
        user: AuthUser(id: '1', displayName: 'User', email: 'u@e.com'),
        accessToken: 'token',
      );
      final fakeRepo = FakeAuthRepository(
        config: config,
        secureStorage: _FakeSecureStorage(),
        loginSession: session,
      );
      final container = ProviderContainer(
        overrides: [
          authRepositoryProvider.overrideWithValue(fakeRepo),
        ],
      );
      addTearDown(container.dispose);

      final notifier = container.read(authStateProvider.notifier);
      await notifier.login(email: 'u@e.com', password: 'pass');

      final state = container.read(authStateProvider);
      expect(state.hasError, isFalse);
      expect(state.valueOrNull, equals(session));
      expect(state.valueOrNull?.user?.email, 'u@e.com');
    });

    test('login sets state to error when repo throws', () async {
      final fakeRepo = FakeAuthRepository(
        config: config,
        secureStorage: _FakeSecureStorage(),
        shouldThrowLogin: true,
      );
      final container = ProviderContainer(
        overrides: [
          authRepositoryProvider.overrideWithValue(fakeRepo),
        ],
      );
      addTearDown(container.dispose);

      final notifier = container.read(authStateProvider.notifier);
      await notifier.login(email: 'u@e.com', password: 'pass');

      final state = container.read(authStateProvider);
      expect(state.hasError, isTrue);
    });

    test('signUp sets state to session when repo returns session', () async {
      const session = AuthSession(
        user: AuthUser(id: '2', displayName: 'New User', email: 'new@e.com'),
        accessToken: 'token2',
      );
      final fakeRepo = FakeAuthRepository(
        config: config,
        secureStorage: _FakeSecureStorage(),
        signUpSession: session,
      );
      final container = ProviderContainer(
        overrides: [
          authRepositoryProvider.overrideWithValue(fakeRepo),
        ],
      );
      addTearDown(container.dispose);

      final notifier = container.read(authStateProvider.notifier);
      await notifier.signUp(
        email: 'new@e.com',
        displayName: 'New User',
        password: 'password123',
      );

      final state = container.read(authStateProvider);
      expect(state.hasError, isFalse);
      expect(state.valueOrNull, equals(session));
      expect(state.valueOrNull?.user?.displayName, 'New User');
    });

    test('signUp sets state to error when repo throws', () async {
      final fakeRepo = FakeAuthRepository(
        config: config,
        secureStorage: _FakeSecureStorage(),
        shouldThrowSignUp: true,
      );
      final container = ProviderContainer(
        overrides: [
          authRepositoryProvider.overrideWithValue(fakeRepo),
        ],
      );
      addTearDown(container.dispose);

      final notifier = container.read(authStateProvider.notifier);
      await notifier.signUp(
        email: 'new@e.com',
        displayName: 'New User',
        password: 'password123',
      );

      final state = container.read(authStateProvider);
      expect(state.hasError, isTrue);
    });
  });
}
