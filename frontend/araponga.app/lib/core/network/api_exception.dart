/// Exceção de chamada à API/BFF.
class ApiException implements Exception {
  ApiException(this.message, {this.statusCode, this.body, this.originalError, this.serverError});
  final String message;
  final int? statusCode;
  final String? body;
  final Object? originalError;
  /// Mensagem de erro retornada pela API (ex: campo "error" do JSON).
  final String? serverError;

  bool get isUnauthorized => statusCode == 401;
  bool get isRateLimited => statusCode == 429;
  bool get isForbidden => statusCode == 403;

  static final _knownErrors = <String, String>{
    'Email is required.': 'E-mail é obrigatório.',
    'Display name is required.': 'Nome é obrigatório.',
    'Password must be at least 6 characters.': 'A senha deve ter no mínimo 6 caracteres.',
    'Email and password are required.': 'E-mail e senha são obrigatórios.',
    'Invalid email or password.': 'E-mail ou senha inválidos.',
    'Invalid email or password': 'E-mail ou senha inválidos.',
    'User already exists with this email.': 'Já existe uma conta com este e-mail.',
    'Email already registered.': 'Já existe uma conta com este e-mail.',
  };

  /// Mensagem para exibir ao usuário. Em 4xx, prefere [serverError] da API quando existir, para não suprimir o erro real.
  String get userMessage {
    if (isUnauthorized) return 'Sessão expirada. Faça login novamente.';
    if (isRateLimited) return 'Muitas tentativas. Tente em alguns minutos.';
    // 4xx (exceto 401): mostrar mensagem da API (serverError) quando existir, senão fallback genérico
    if (statusCode != null && statusCode! >= 400 && statusCode! < 500) {
      if (serverError != null && serverError!.trim().isNotEmpty) return _knownErrors[serverError!] ?? serverError!;
      if (isForbidden) return 'Sua localização está fora do território observado.';
    }
    if (statusCode != null && statusCode! >= 500) return 'Erro no servidor. Tente mais tarde.';
    // Erro de rede (sem status HTTP): CORS, BFF inacessível ou conexão recusada (comum no Flutter Web).
    if (statusCode == null && (message.contains('rede') || message.toLowerCase().contains('network') || message.toLowerCase().contains('connection'))) {
      return 'Não foi possível conectar ao servidor. Verifique se o BFF está em execução (ex.: http://localhost:5001) e tente novamente.';
    }
    final raw = serverError ?? message;
    return _knownErrors[raw] ?? raw;
  }

  @override
  String toString() => 'ApiException($message, statusCode: $statusCode)';
}
