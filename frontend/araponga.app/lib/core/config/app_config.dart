import 'package:flutter_riverpod/flutter_riverpod.dart';

/// Configuração da aplicação (BFF base URL, feature flags, etc.).
class AppConfig {
  const AppConfig({
    required this.bffBaseUrl,
    this.apiTimeoutSeconds = 30,
  });

  final String bffBaseUrl;
  final int apiTimeoutSeconds;

  /// URL base das jornadas BFF: [bffBaseUrl]/api/v2/journeys
  String get bffJourneysBaseUrl =>
      bffBaseUrl.replaceAll(RegExp(r'/$'), '') + '/api/v2/journeys';
}

final appConfigProvider = Provider<AppConfig>((ref) {
  // Em desenvolvimento use o IP/porta do BFF. Em produção, substituir por variável de ambiente.
  const baseUrl = String.fromEnvironment(
    'BFF_BASE_URL',
    defaultValue: 'http://localhost:5001',
  );
  return AppConfig(bffBaseUrl: baseUrl);
});
