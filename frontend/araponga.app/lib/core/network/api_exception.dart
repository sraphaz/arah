/// Exceção de chamada à API/BFF.
class ApiException implements Exception {
  ApiException(this.message, {this.statusCode, this.body, this.originalError});
  final String message;
  final int? statusCode;
  final String? body;
  final Object? originalError;

  bool get isUnauthorized => statusCode == 401;
  bool get isRateLimited => statusCode == 429;
  bool get isForbidden => statusCode == 403;

  String get userMessage {
    if (isUnauthorized) return 'Sessão expirada. Faça login novamente.';
    if (isRateLimited) return 'Muitas tentativas. Tente em alguns minutos.';
    if (isForbidden) return 'Sua localização está fora do território observado.';
    if (statusCode != null && statusCode! >= 500) return 'Erro no servidor. Tente mais tarde.';
    return message;
  }

  @override
  String toString() => 'ApiException($message, statusCode: $statusCode)';
}
