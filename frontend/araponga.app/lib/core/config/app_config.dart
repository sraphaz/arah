import 'package:flutter_riverpod/flutter_riverpod.dart';

/// Configuração da aplicação (BFF base URL, feature flags, etc.).
class AppConfig {
  const AppConfig({
    required this.bffBaseUrl,
    this.apiTimeoutSeconds = 30,
    this.googleSignInClientId,
  });

  final String bffBaseUrl;
  final int apiTimeoutSeconds;
  /// Client ID OAuth 2.0 para Google Sign-In (obrigatório em Web; Android/iOS usam GoogleService se null).
  final String? googleSignInClientId;

  /// URL base das jornadas BFF: [bffBaseUrl]/api/v2/journeys
  String get bffJourneysBaseUrl =>
      bffBaseUrl.replaceAll(RegExp(r'/$'), '') + '/api/v2/journeys';
}

final appConfigProvider = Provider<AppConfig>((ref) {
  const baseUrl = String.fromEnvironment(
    'BFF_BASE_URL',
    defaultValue: 'http://localhost:5001',
  );
  const googleClientId = String.fromEnvironment(
    'GOOGLE_SIGN_IN_CLIENT_ID',
    defaultValue: '',
  );
  return AppConfig(
    bffBaseUrl: baseUrl,
    googleSignInClientId: googleClientId.isEmpty ? null : googleClientId,
  );
});
