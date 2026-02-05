import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../data/models/auth_models.dart';
import '../../data/repositories/auth_repository.dart';

/// Estado global de autenticação. Inicializa com restoreSession.
final authStateProvider = AsyncNotifierProvider<AuthStateNotifier, AuthSession?>(AuthStateNotifier.new);

class AuthStateNotifier extends AsyncNotifier<AuthSession?> {
  @override
  Future<AuthSession?> build() async {
    return ref.read(authRepositoryProvider).restoreSession();
  }

  Future<void> login({required String email, String? displayName}) async {
    state = const AsyncLoading();
    state = await AsyncValue.guard(() async {
      final session = await ref.read(authRepositoryProvider).login(email: email, displayName: displayName);
      return session;
    });
  }

  Future<void> logout() async {
    await ref.read(authRepositoryProvider).logout();
    state = const AsyncData(null);
  }

  void setSession(AuthSession? session) {
    state = AsyncData(session);
  }
}

/// Token atual (para injetar no BffClient). Null se não autenticado.
final accessTokenProvider = Provider<String?>((ref) {
  final auth = ref.watch(authStateProvider);
  return auth.valueOrNull?.accessToken;
});

/// Usuário atual (pode ser null se restaurado só por token).
final currentUserProvider = Provider<AuthUser?>((ref) {
  final auth = ref.watch(authStateProvider);
  return auth.valueOrNull?.user;
});
