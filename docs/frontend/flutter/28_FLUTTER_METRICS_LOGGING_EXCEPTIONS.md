# Métricas, Logs e Exceções - Arah Flutter App

**Versão**: 1.0  
**Data**: 2025-01-20  
**Status**: 📋 Especificação Completa  
**Tipo**: Documentação Técnica de Observabilidade

---

## 📋 Índice

1. [Visão Geral](#visão-geral)
2. [Arquitetura de Observabilidade](#arquitetura-de-observabilidade)
3. [Métricas de Frontend](#métricas-de-frontend)
4. [Logs Estruturados](#logs-estruturados)
5. [Captura e Rastreamento de Exceções](#captura-e-rastreamento-de-exceções)
6. [Integração com Serviços Externos](#integração-com-serviços-externos)
7. [Privacidade e Conformidade](#privacidade-e-conformidade)
8. [Estratégias de Análise e Alertas](#estratégias-de-análise-e-alertas)
9. [Implementação e Configuração](#implementação-e-configuração)
10. [Boas Práticas e Recomendações](#boas-práticas-e-recomendações)

---

## 🎯 Visão Geral

### Objetivo

Este documento especifica a estratégia completa de **observabilidade** para o app Flutter Arah, cobrindo:

- **Métricas**: Performance, uso, engajamento, negócio
- **Logs**: Estruturados, categorizados, rastreáveis
- **Exceções**: Captura, rastreamento, reportagem

### Princípios Fundamentais

1. **Privacidade First**: Nunca coletar dados pessoais sensíveis sem consentimento
2. **Eficiência**: Logs e métricas devem ter impacto mínimo na performance
3. **Rastreabilidade**: Todas as operações devem ser rastreáveis via correlation IDs
4. **Conformidade**: Respeitar LGPD/GDPR e políticas de privacidade
5. **Observabilidade Completa**: Cobrir todos os aspectos críticos do app

### Stack Tecnológica

- **Métricas**: Firebase Analytics, Custom Analytics (mixpanel/amplitude)
- **Logs**: `logger` package + Firebase Crashlytics
- **Exceções**: Sentry + Firebase Crashlytics
- **Performance**: Firebase Performance Monitoring
- **A/B Testing**: Firebase Remote Config

---

## 🏗️ Arquitetura de Observabilidade

### Camadas de Observabilidade

```
┌─────────────────────────────────────────────────────────┐
│                    App Flutter                          │
│                                                         │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐ │
│  │   Métricas   │  │    Logs      │  │  Exceções    │ │
│  │   Service    │  │   Service    │  │   Service    │ │
│  └──────┬───────┘  └──────┬───────┘  └──────┬───────┘ │
│         │                 │                  │          │
│         └─────────────────┴──────────────────┘          │
│                        │                                │
│              ┌─────────▼─────────┐                      │
│              │  Observability    │                      │
│              │     Manager       │                      │
│              └─────────┬─────────┘                      │
└────────────────────────┼────────────────────────────────┘
                         │
         ┌───────────────┼───────────────┐
         │               │               │
    ┌────▼────┐    ┌────▼────┐    ┌────▼────┐
    │Firebase │    │ Sentry  │    │  Custom │
    │Services │    │         │    │  Backend│
    └─────────┘    └─────────┘    └─────────┘
```

### Componentes Principais

1. **MetricsService**: Coleta e envia métricas
2. **LoggingService**: Registra logs estruturados
3. **ExceptionService**: Captura e reporta exceções
4. **ObservabilityManager**: Coordena todos os serviços

---

## 📊 Métricas de Frontend

### Categorias de Métricas

#### 1. Métricas de Performance

**Objetivo**: Medir performance do app e experiência do usuário

**Métricas Principais**:
- **App Start Time**: Tempo de inicialização do app (cold start, warm start)
- **Screen Load Time**: Tempo de carregamento de telas
- **API Response Time**: Latência de requisições HTTP
- **Image Load Time**: Tempo de carregamento de imagens
- **Frame Rendering**: FPS (Frames Per Second), jank rate
- **Memory Usage**: Uso de memória (heap, native)
- **Battery Usage**: Consumo de bateria (estimado)

**Implementação**:
```dart
// Exemplo de métrica de performance
class PerformanceMetrics {
  static Future<void> trackScreenLoadTime(String screenName) async {
    final stopwatch = Stopwatch()..start();
    
    // Aguarda construção da tela
    await Future.delayed(Duration(milliseconds: 100));
    
    stopwatch.stop();
    
    await FirebaseAnalytics.instance.logEvent(
      name: 'screen_load_time',
      parameters: {
        'screen_name': screenName,
        'load_time_ms': stopwatch.elapsedMilliseconds,
        'platform': Platform.operatingSystem,
      },
    );
  }
  
  static Future<void> trackApiResponseTime(
    String endpoint,
    int durationMs,
  ) async {
    await FirebaseAnalytics.instance.logEvent(
      name: 'api_response_time',
      parameters: {
        'endpoint': endpoint,
        'duration_ms': durationMs,
        'platform': Platform.operatingSystem,
      },
    );
  }
}
```

#### 2. Métricas de Uso

**Objetivo**: Entender como usuários interagem com o app

**Métricas Principais**:
- **Screen Views**: Telas visualizadas
- **User Actions**: Ações do usuário (taps, swipes, scrolls)
- **Feature Usage**: Uso de funcionalidades específicas
- **Session Duration**: Duração de sessões
- **Daily/Monthly Active Users**: DAU/MAU
- **Retention**: Taxa de retenção (D1, D7, D30)

**Implementação**:
```dart
// Exemplo de métrica de uso
class UsageMetrics {
  static Future<void> trackScreenView(String screenName) async {
    await FirebaseAnalytics.instance.logScreenView(
      screenName: screenName,
    );
  }
  
  static Future<void> trackUserAction(
    String action,
    Map<String, dynamic>? parameters,
  ) async {
    await FirebaseAnalytics.instance.logEvent(
      name: 'user_action',
      parameters: {
        'action': action,
        ...?parameters,
      },
    );
  }
  
  static Future<void> trackFeatureUsage(String featureName) async {
    await FirebaseAnalytics.instance.logEvent(
      name: 'feature_usage',
      parameters: {
        'feature_name': featureName,
        'timestamp': DateTime.now().toIso8601String(),
      },
    );
  }
}
```

#### 3. Métricas de Engajamento

**Objetivo**: Medir engajamento e participação na comunidade

**Métricas Principais**:
- **Posts Created**: Posts criados por usuário
- **Events Created**: Eventos criados por usuário
- **Events Participated**: Eventos em que usuário participou
- **Comments Made**: Comentários feitos
- **Likes Given**: Likes dados
- **Shares Made**: Compartilhamentos feitos
- **Territory Engagement**: Interação com territórios
- **Marketplace Usage**: Uso do marketplace (compras/vendas)

**Implementação**:
```dart
// Exemplo de métrica de engajamento
class EngagementMetrics {
  static Future<void> trackPostCreated({
    required String territoryId,
    required String postType,
    required String visibility,
  }) async {
    await FirebaseAnalytics.instance.logEvent(
      name: 'post_created',
      parameters: {
        'territory_id': territoryId,
        'post_type': postType, // NOTICE, ALERT, ANNOUNCEMENT
        'visibility': visibility, // PUBLIC, RESIDENTS_ONLY
        'timestamp': DateTime.now().toIso8601String(),
      },
    );
  }
  
  static Future<void> trackEventParticipated({
    required String eventId,
    required String territoryId,
  }) async {
    await FirebaseAnalytics.instance.logEvent(
      name: 'event_participated',
      parameters: {
        'event_id': eventId,
        'territory_id': territoryId,
        'timestamp': DateTime.now().toIso8601String(),
      },
    );
  }
  
  static Future<void> trackLikeGiven({
    required String targetType, // POST, EVENT, COMMENT
    required String targetId,
  }) async {
    await FirebaseAnalytics.instance.logEvent(
      name: 'like_given',
      parameters: {
        'target_type': targetType,
        'target_id': targetId,
        'timestamp': DateTime.now().toIso8601String(),
      },
    );
  }
}
```

#### 4. Métricas de Negócio

**Objetivo**: Medir objetivos de negócio e KPIs

**Métricas Principais**:
- **User Registrations**: Novos registros
- **Membership Requests**: Solicitações de residência
- **Membership Approvals**: Aprovações de residência
- **Territory Selections**: Seleção de territórios
- **Feature Adoption**: Adoção de novas funcionalidades
- **Conversion Funnels**: Funis de conversão

**Implementação**:
```dart
// Exemplo de métrica de negócio
class BusinessMetrics {
  static Future<void> trackUserRegistration({
    required String authProvider, // GOOGLE, APPLE, EMAIL
  }) async {
    await FirebaseAnalytics.instance.logEvent(
      name: 'user_registration',
      parameters: {
        'auth_provider': authProvider,
        'timestamp': DateTime.now().toIso8601String(),
      },
    );
  }
  
  static Future<void> trackMembershipRequest({
    required String territoryId,
    required bool hasDocument,
  }) async {
    await FirebaseAnalytics.instance.logEvent(
      name: 'membership_request',
      parameters: {
        'territory_id': territoryId,
        'has_document': hasDocument,
        'timestamp': DateTime.now().toIso8601String(),
      },
    );
  }
  
  static Future<void> trackConversion({
    required String funnelName,
    required String step,
  }) async {
    await FirebaseAnalytics.instance.logEvent(
      name: 'conversion',
      parameters: {
        'funnel_name': funnelName,
        'step': step,
        'timestamp': DateTime.now().toIso8601String(),
      },
    );
  }
}
```

#### 5. Métricas de Erro e Qualidade

**Objetivo**: Medir qualidade do app e frequência de erros

**Métricas Principais**:
- **Crash Rate**: Taxa de crashes por sessão
- **Error Rate**: Taxa de erros (não-crash) por ação
- **API Error Rate**: Taxa de erros de API
- **Validation Error Rate**: Taxa de erros de validação
- **Network Error Rate**: Taxa de erros de rede

**Implementação**:
```dart
// Exemplo de métrica de erro
class ErrorMetrics {
  static Future<void> trackError({
    required String errorType,
    required String errorMessage,
    String? stackTrace,
    Map<String, dynamic>? context,
  }) async {
    await FirebaseAnalytics.instance.logEvent(
      name: 'error_occurred',
      parameters: {
        'error_type': errorType,
        'error_message': errorMessage.substring(0, 100), // Limita tamanho
        'platform': Platform.operatingSystem,
        ...?context,
      },
    );
  }
  
  static Future<void> trackApiError({
    required String endpoint,
    required int statusCode,
    String? errorMessage,
  }) async {
    await FirebaseAnalytics.instance.logEvent(
      name: 'api_error',
      parameters: {
        'endpoint': endpoint,
        'status_code': statusCode,
        'error_message': errorMessage?.substring(0, 100),
        'timestamp': DateTime.now().toIso8601String(),
      },
    );
  }
}
```

---

## 📝 Logs Estruturados

### Estrutura de Logs

#### Níveis de Log

1. **TRACE**: Logs muito detalhados para debug (desenvolvimento apenas)
2. **DEBUG**: Informações de debug para desenvolvimento
3. **INFO**: Informações gerais sobre operações normais
4. **WARNING**: Avisos sobre situações inesperadas mas não críticas
5. **ERROR**: Erros que não causam crash mas merecem atenção
6. **FATAL**: Erros críticos que podem causar crash

#### Categorias de Logs

1. **AUTH**: Autenticação e autorização
2. **API**: Requisições HTTP e respostas
3. **NAVIGATION**: Navegação entre telas
4. **STATE**: Gerenciamento de estado (Riverpod)
5. **STORAGE**: Operações de armazenamento local
6. **NETWORK**: Operações de rede
7. **UI**: Interações de interface
8. **BUSINESS**: Eventos de negócio

### Implementação

```dart
// Exemplo de serviço de logging estruturado
class LoggingService {
  static final Logger _logger = Logger(
    printer: PrettyPrinter(
      methodCount: 2,
      errorMethodCount: 8,
      lineLength: 120,
      colors: true,
      printEmojis: true,
      printTime: true,
    ),
  );
  
  // Correlation ID para rastreabilidade
  static String? _correlationId;
  
  static String get correlationId {
    _correlationId ??= _generateCorrelationId();
    return _correlationId!;
  }
  
  static String _generateCorrelationId() {
    return '${DateTime.now().millisecondsSinceEpoch}-${Random().nextInt(10000)}';
  }
  
  static void resetCorrelationId() {
    _correlationId = null;
  }
  
  // Log estruturado com categoria
  static void log({
    required LogLevel level,
    required LogCategory category,
    required String message,
    Map<String, dynamic>? parameters,
    Object? error,
    StackTrace? stackTrace,
  }) {
    final logData = {
      'correlation_id': correlationId,
      'category': category.name,
      'message': message,
      'timestamp': DateTime.now().toIso8601String(),
      'platform': Platform.operatingSystem,
      'app_version': _getAppVersion(),
      ...?parameters,
    };
    
    // Log local (console/file)
    switch (level) {
      case LogLevel.TRACE:
        _logger.t(message, error: error, stackTrace: stackTrace);
        break;
      case LogLevel.DEBUG:
        _logger.d(message, error: error, stackTrace: stackTrace);
        break;
      case LogLevel.INFO:
        _logger.i(message, error: error, stackTrace: stackTrace);
        break;
      case LogLevel.WARNING:
        _logger.w(message, error: error, stackTrace: stackTrace);
        break;
      case LogLevel.ERROR:
        _logger.e(message, error: error, stackTrace: stackTrace);
        break;
      case LogLevel.FATAL:
        _logger.f(message, error: error, stackTrace: stackTrace);
        break;
    }
    
    // Log remoto (Firebase Crashlytics) apenas para WARNING+
    if (level.index >= LogLevel.WARNING.index) {
      _logToCrashlytics(level, category, message, logData, error, stackTrace);
    }
  }
  
  static void _logToCrashlytics(
    LogLevel level,
    LogCategory category,
    String message,
    Map<String, dynamic> data,
    Object? error,
    StackTrace? stackTrace,
  ) async {
    try {
      await FirebaseCrashlytics.instance.setCustomKey(
        'log_category',
        category.name,
      );
      
      for (final entry in data.entries) {
        await FirebaseCrashlytics.instance.setCustomKey(
          entry.key,
          entry.value.toString(),
        );
      }
      
      if (level == LogLevel.FATAL || level == LogLevel.ERROR) {
        await FirebaseCrashlytics.instance.recordError(
          error ?? Exception(message),
          stackTrace,
          reason: message,
          information: data.entries.map((e) => '${e.key}: ${e.value}').toList(),
        );
      } else {
        await FirebaseCrashlytics.instance.log('[$level] [$category] $message');
      }
    } catch (e) {
      // Falha silenciosa para não interferir no app
      debugPrint('Failed to log to Crashlytics: $e');
    }
  }
  
  static String _getAppVersion() {
    try {
      final packageInfo = PackageInfo.fromPlatform();
      return '${packageInfo.version}+${packageInfo.buildNumber}';
    } catch (e) {
      return 'unknown';
    }
  }
  
  // Métodos de conveniência por categoria
  static void logAuth(String message, {Map<String, dynamic>? params}) {
    log(
      level: LogLevel.INFO,
      category: LogCategory.AUTH,
      message: message,
      parameters: params,
    );
  }
  
  static void logApi(String message, {Map<String, dynamic>? params, Object? error, StackTrace? stackTrace}) {
    log(
      level: error != null ? LogLevel.ERROR : LogLevel.INFO,
      category: LogCategory.API,
      message: message,
      parameters: params,
      error: error,
      stackTrace: stackTrace,
    );
  }
  
  static void logNavigation(String screenName, {Map<String, dynamic>? params}) {
    log(
      level: LogLevel.INFO,
      category: LogCategory.NAVIGATION,
      message: 'Navigated to $screenName',
      parameters: {
        'screen_name': screenName,
        ...?params,
      },
    );
  }
  
  static void logBusiness(String event, {Map<String, dynamic>? params}) {
    log(
      level: LogLevel.INFO,
      category: LogCategory.BUSINESS,
      message: event,
      parameters: params,
    );
  }
}

// Enums para níveis e categorias
enum LogLevel {
  TRACE,
  DEBUG,
  INFO,
  WARNING,
  ERROR,
  FATAL,
}

enum LogCategory {
  AUTH,
  API,
  NAVIGATION,
  STATE,
  STORAGE,
  NETWORK,
  UI,
  BUSINESS,
}
```

### Formato de Log

```json
{
  "correlation_id": "1705756800000-1234",
  "level": "INFO",
  "category": "API",
  "message": "GET /api/v1/feed completed",
  "timestamp": "2025-01-20T10:00:00.000Z",
  "platform": "ios",
  "app_version": "1.0.0+1",
  "parameters": {
    "endpoint": "/api/v1/feed",
    "method": "GET",
    "status_code": 200,
    "duration_ms": 245,
    "territory_id": "123e4567-e89b-12d3-a456-426614174000"
  }
}
```

---

## 🚨 Captura e Rastreamento de Exceções

### Tipos de Exceções

#### 1. Exceções de Aplicação

**Captura**: Todas as exceções não tratadas na aplicação

**Informações Coletadas**:
- Tipo de exceção
- Mensagem de erro
- Stack trace completo
- Contexto da aplicação (tela atual, estado, etc.)
- Correlation ID
- Informações do dispositivo (OS, versão, modelo)
- Informações do app (versão, build)

#### 2. Exceções de API

**Captura**: Erros HTTP e respostas de erro da API

**Informações Coletadas**:
- Endpoint e método HTTP
- Status code
- Corpo da resposta de erro
- Headers de requisição (sem dados sensíveis)
- Correlation ID da requisição

#### 3. Exceções de Rede

**Captura**: Erros de conectividade e timeouts

**Informações Coletadas**:
- Tipo de erro (timeout, connection refused, etc.)
- Endpoint tentado
- Duração da tentativa
- Estado da conexão (WiFi, celular, sem conexão)

#### 4. Exceções de Validação

**Captura**: Erros de validação de formulários e dados

**Informações Coletadas**:
- Campo validado
- Regra de validação violada
- Valor fornecido (hasheado se sensível)
- Formulário/tela

### Implementação

```dart
// Exemplo de serviço de exceções
class ExceptionService {
  static Future<void> initialize() async {
    // Configurar Sentry
    await Sentry.init(
      (options) {
        options.dsn = Config.sentryDsn;
        options.environment = Config.environment; // dev, staging, prod
        options.release = await _getAppVersion();
        options.tracesSampleRate = Config.environment == 'prod' ? 0.1 : 1.0;
        options.enableAutoSessionTracking = true;
        options.beforeSend = _beforeSend;
        options.maxBreadcrumbs = 100;
      },
      appRunner: () => runApp(MyApp()),
    );
    
    // Configurar Firebase Crashlytics
    await FirebaseCrashlytics.instance.setCrashlyticsCollectionEnabled(
      Config.environment == 'prod',
    );
    
    // Capturar erros não tratados do Flutter
    FlutterError.onError = (FlutterErrorDetails details) {
      FirebaseCrashlytics.instance.recordFlutterFatalError(details);
      Sentry.captureException(
        details.exception,
        stackTrace: details.stack,
        hint: Hint.withMap({'details': details.toString()}),
      );
    };
    
    // Capturar erros não tratados do Zone
    PlatformDispatcher.instance.onError = (error, stack) {
      FirebaseCrashlytics.instance.recordError(
        error,
        stack,
        fatal: true,
      );
      Sentry.captureException(
        error,
        stackTrace: stack,
      );
      return true;
    };
  }
  
  static BeforeSendCallback _beforeSend = (event, {hint}) {
    // Filtrar dados sensíveis antes de enviar
    if (event.request?.data != null) {
      event.request?.data = _sanitizeData(event.request!.data);
    }
    
    // Filtrar breadcrumbs sensíveis
    event.breadcrumbs?.removeWhere((b) => _isSensitive(b.message ?? ''));
    
    return event;
  };
  
  static dynamic _sanitizeData(dynamic data) {
    if (data is Map) {
      return data.map((key, value) {
        if (_isSensitive(key.toString())) {
          return MapEntry(key, '[REDACTED]');
        }
        return MapEntry(key, _sanitizeData(value));
      });
    }
    return data;
  }
  
  static bool _isSensitive(String text) {
    final sensitivePatterns = [
      'password',
      'token',
      'secret',
      'authorization',
      'cookie',
      'session',
    ];
    return sensitivePatterns.any((pattern) => 
      text.toLowerCase().contains(pattern),
    );
  }
  
  static Future<String> _getAppVersion() async {
    try {
      final packageInfo = await PackageInfo.fromPlatform();
      return '${packageInfo.version}+${packageInfo.buildNumber}';
    } catch (e) {
      return 'unknown';
    }
  }
  
  // Capturar exceção manualmente
  static Future<void> captureException(
    Object exception, {
    StackTrace? stackTrace,
    Map<String, dynamic>? context,
    String? level, // fatal, error, warning, info, debug
    bool fatal = false,
  }) async {
    try {
      // Adicionar contexto
      if (context != null) {
        await FirebaseCrashlytics.instance.setCustomKeys(context);
        await Sentry.configureScope((scope) {
          context.forEach((key, value) {
            scope.setContext(key, {'value': value.toString()});
          });
        });
      }
      
      // Adicionar correlation ID
      await FirebaseCrashlytics.instance.setCustomKey(
        'correlation_id',
        LoggingService.correlationId,
      );
      
      // Capturar exceção
      if (fatal) {
        await FirebaseCrashlytics.instance.recordError(
          exception,
          stackTrace,
          fatal: true,
        );
        await Sentry.captureException(
          exception,
          stackTrace: stackTrace,
          hint: Hint.withMap({'level': 'fatal'}),
        );
      } else {
        await FirebaseCrashlytics.instance.recordError(
          exception,
          stackTrace,
        );
        await Sentry.captureException(
          exception,
          stackTrace: stackTrace,
          hint: Hint.withMap({'level': level ?? 'error'}),
        );
      }
    } catch (e) {
      // Falha silenciosa
      debugPrint('Failed to capture exception: $e');
    }
  }
  
  // Capturar exceção de API
  static Future<void> captureApiException({
    required String endpoint,
    required int statusCode,
    required String method,
    String? responseBody,
    Map<String, String>? requestHeaders,
  }) async {
    await captureException(
      Exception('API Error: $method $endpoint - $statusCode'),
      context: {
        'endpoint': endpoint,
        'method': method,
        'status_code': statusCode,
        'response_body': responseBody?.substring(0, 500) ?? 'null',
        'request_headers': _sanitizeHeaders(requestHeaders ?? {}),
      },
      level: statusCode >= 500 ? 'error' : 'warning',
      fatal: statusCode >= 500,
    );
  }
  
  static Map<String, String> _sanitizeHeaders(Map<String, String> headers) {
    final sensitiveHeaders = ['authorization', 'cookie', 'x-api-key'];
    return headers.map((key, value) {
      if (sensitiveHeaders.contains(key.toLowerCase())) {
        return MapEntry(key, '[REDACTED]');
      }
      return MapEntry(key, value);
    });
  }
  
  // Adicionar breadcrumb (contexto de ações)
  static Future<void> addBreadcrumb({
    required String message,
    String? category,
    Map<String, dynamic>? data,
  }) async {
    try {
      await Sentry.addBreadcrumb(
        Breadcrumb(
          message: message,
          category: category,
          data: data,
          level: SentryLevel.info,
          timestamp: DateTime.now(),
        ),
      );
      
      await FirebaseCrashlytics.instance.log(
        '[$category] $message',
      );
    } catch (e) {
      debugPrint('Failed to add breadcrumb: $e');
    }
  }
  
  // Definir usuário para rastreamento
  static Future<void> setUser({
    String? userId,
    String? email,
    String? username,
    Map<String, dynamic>? additionalData,
  }) async {
    try {
      await Sentry.configureScope((scope) {
        scope.setUser(
          SentryUser(
            id: userId,
            email: email,
            username: username,
            data: additionalData,
          ),
        );
      });
      
      await FirebaseCrashlytics.instance.setUserIdentifier(userId ?? 'anonymous');
      if (email != null) {
        await FirebaseCrashlytics.instance.setCustomKey('user_email', email);
      }
    } catch (e) {
      debugPrint('Failed to set user: $e');
    }
  }
}
```

---

## 🔌 Integração com Serviços Externos

### Firebase Services

#### Firebase Analytics

**Configuração**:
```dart
// main.dart
await Firebase.initializeApp();

// Habilitar/desabilitar analytics baseado em consentimento
await FirebaseAnalytics.instance.setAnalyticsCollectionEnabled(
  await ConsentService.hasAnalyticsConsent(),
);
```

**Uso**:
```dart
// Já documentado em Métricas de Frontend
```

#### Firebase Crashlytics

**Configuração**:
```dart
// main.dart
await FirebaseCrashlytics.instance.setCrashlyticsCollectionEnabled(true);
```

**Uso**:
```dart
// Já documentado em Logs Estruturados e Exceções
```

#### Firebase Performance Monitoring

**Configuração**:
```dart
// main.dart
await FirebasePerformance.instance.setPerformanceCollectionEnabled(true);
```

**Uso**:
```dart
// Rastrear performance de operações
class PerformanceTracking {
  static Future<T> trackOperation<T>(
    String operationName,
    Future<T> Function() operation,
  ) async {
    final trace = FirebasePerformance.instance.newTrace(operationName);
    await trace.start();
    
    try {
      final result = await operation();
      await trace.stop();
      return result;
    } catch (e) {
      trace.putAttribute('error', e.toString());
      await trace.stop();
      rethrow;
    }
  }
  
  // Uso
  static Future<List<Post>> loadFeed() async {
    return trackOperation('load_feed', () async {
      // Lógica de carregamento
      return await apiService.getFeed();
    });
  }
}
```

### Sentry

**Configuração**:
```dart
// Já documentado em ExceptionService.initialize()
```

**Uso**:
```dart
// Já documentado em ExceptionService
```

### Custom Backend (Opcional)

**Endpoint de Métricas**:
```
POST /api/v1/metrics
```

**Payload**:
```json
{
  "metric_name": "screen_view",
  "value": 1,
  "timestamp": "2025-01-20T10:00:00.000Z",
  "user_id": "123e4567-e89b-12d3-a456-426614174000",
  "parameters": {
    "screen_name": "FeedScreen",
    "territory_id": "..."
  }
}
```

---

## 🔒 Privacidade e Conformidade

### LGPD/GDPR Compliance

#### Consentimento do Usuário

**Implementação**:
```dart
class ConsentService {
  static Future<bool> hasAnalyticsConsent() async {
    final prefs = await SharedPreferences.getInstance();
    return prefs.getBool('analytics_consent') ?? false;
  }
  
  static Future<void> setAnalyticsConsent(bool consent) async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.setBool('analytics_consent', consent);
    
    // Atualizar estado do Firebase Analytics
    await FirebaseAnalytics.instance.setAnalyticsCollectionEnabled(consent);
  }
  
  static Future<bool> hasCrashlyticsConsent() async {
    final prefs = await SharedPreferences.getInstance();
    return prefs.getBool('crashlytics_consent') ?? false;
  }
  
  static Future<void> setCrashlyticsConsent(bool consent) async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.setBool('crashlytics_consent', consent);
    
    await FirebaseCrashlytics.instance.setCrashlyticsCollectionEnabled(consent);
  }
}
```

#### Dados Sensíveis

**Políticas**:
1. **Nunca coletar**:
   - Senhas
   - Tokens de autenticação completos
   - Informações de pagamento completas
   - Dados biométricos

2. **Anonimizar/Hashear**:
   - Emails (usar hash)
   - IDs de usuário (usar hash ou pseudonimizar)
   - Endereços completos (usar apenas região/território)

3. **Sanitizar antes de enviar**:
   - Logs
   - Exceções
   - Métricas

**Implementação**:
```dart
class PrivacyService {
  static String hashEmail(String email) {
    return sha256.convert(utf8.encode(email.toLowerCase())).toString();
  }
  
  static String hashUserId(String userId) {
    return sha256.convert(utf8.encode(userId)).toString();
  }
  
  static Map<String, dynamic> sanitizeParameters(Map<String, dynamic> params) {
    final sensitiveKeys = [
      'password',
      'token',
      'secret',
      'authorization',
      'email',
      'phone',
      'cpf',
      'address',
    ];
    
    return params.map((key, value) {
      if (sensitiveKeys.any((sensitive) => 
        key.toLowerCase().contains(sensitive.toLowerCase()),
      )) {
        return MapEntry(key, '[REDACTED]');
      }
      return MapEntry(key, value);
    });
  }
}
```

### Retenção de Dados

**Políticas**:
- **Logs**: Retenção de 30 dias
- **Métricas**: Retenção de 1 ano
- **Exceções**: Retenção de 90 dias
- **Dados de usuário**: Deletar quando usuário excluir conta

---

## 📈 Estratégias de Análise e Alertas

### Dashboards Recomendados

#### Dashboard de Performance
- App Start Time (média, P95, P99)
- Screen Load Time por tela
- API Response Time por endpoint
- Frame Rendering (FPS, jank rate)
- Memory Usage

#### Dashboard de Uso
- Daily/Monthly Active Users
- Screen Views por tela
- Feature Usage
- Session Duration
- Retention (D1, D7, D30)

#### Dashboard de Engajamento
- Posts Created por dia/semana
- Events Created/Participated
- Likes/Comments/Shares
- Territory Engagement
- Marketplace Usage

#### Dashboard de Erros
- Crash Rate por versão
- Error Rate por tipo
- API Error Rate por endpoint
- Top Errors
- Error Trends

### Alertas Recomendados

#### Alertas Críticos (Pager)
1. **Crash Rate > 1%**: Crash rate acima de 1% em 5 minutos
2. **Error Rate > 5%**: Error rate acima de 5% em 5 minutos
3. **API Error Rate > 10%**: API error rate acima de 10% em 5 minutos

#### Alertas de Aviso (Email/Slack)
1. **App Start Time > 3s**: App start time acima de 3s (P95)
2. **Screen Load Time > 2s**: Screen load time acima de 2s (P95)
3. **API Response Time > 1s**: API response time acima de 1s (P95)
4. **Memory Usage > 200MB**: Memory usage acima de 200MB
5. **Frame Rendering < 50 FPS**: Frame rendering abaixo de 50 FPS

---

## 🛠️ Implementação e Configuração

### Dependências

```yaml
# pubspec.yaml
dependencies:
  firebase_core: ^2.24.2
  firebase_analytics: ^10.7.4
  firebase_crashlytics: ^3.4.9
  firebase_performance: ^0.9.3+4
  sentry_flutter: ^7.15.0
  logger: ^2.0.2
  crypto: ^3.0.3
  package_info_plus: ^5.0.1
  
dev_dependencies:
  firebase_core_platform_interface: ^4.8.0
```

### Configuração Inicial

```dart
// main.dart
void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  
  // Inicializar Firebase
  await Firebase.initializeApp();
  
  // Inicializar observabilidade
  await ExceptionService.initialize();
  await MetricsService.initialize();
  await LoggingService.initialize();
  
  // Verificar consentimento
  await ConsentService.checkAndRequestConsent();
  
  runApp(MyApp());
}
```

### Estrutura de Pastas

```
lib/
├── services/
│   ├── observability/
│   │   ├── exception_service.dart
│   │   ├── logging_service.dart
│   │   ├── metrics_service.dart
│   │   ├── performance_tracking.dart
│   │   └── observability_manager.dart
│   └── privacy/
│       ├── consent_service.dart
│       └── privacy_service.dart
├── models/
│   └── observability/
│       ├── log_level.dart
│       ├── log_category.dart
│       └── metric_event.dart
└── utils/
    └── sanitizers/
        └── data_sanitizer.dart
```

---

## ✅ Boas Práticas e Recomendações

### Métricas

1. **Não coletar métricas excessivas**: Focar apenas no que é útil
2. **Usar batch de métricas**: Agrupar múltiplas métricas antes de enviar
3. **Definir limites**: Limitar tamanho de parâmetros e número de eventos
4. **Validar métricas**: Garantir que métricas são válidas antes de enviar

### Logs

1. **Usar níveis apropriados**: Não usar ERROR para INFO
2. **Incluir contexto**: Sempre incluir contexto relevante
3. **Evitar logs sensíveis**: Nunca logar senhas, tokens, etc.
4. **Usar correlation IDs**: Para rastrear operações relacionadas
5. **Limitar verbosidade**: Logs DEBUG apenas em desenvolvimento

### Exceções

1. **Capturar apenas o necessário**: Não capturar exceções que não são úteis
2. **Adicionar contexto**: Sempre adicionar contexto relevante
3. **Sanitizar dados**: Remover dados sensíveis antes de enviar
4. **Agrupar exceções similares**: Para evitar spam de exceções
5. **Priorizar exceções críticas**: Fatal > Error > Warning

### Performance

1. **Operações assíncronas**: Logs e métricas devem ser não-bloqueantes
2. **Batch de envio**: Agrupar múltiplos eventos antes de enviar
3. **Retry com backoff**: Retry exponencial para falhas de rede
4. **Cache local**: Cachear eventos localmente antes de enviar

### Privacidade

1. **Consentimento explícito**: Sempre pedir consentimento antes de coletar
2. **Anonimização**: Anonimizar dados sensíveis
3. **Minimização**: Coletar apenas o mínimo necessário
4. **Transparência**: Informar usuário sobre coleta de dados
5. **Deleção**: Deletar dados quando usuário excluir conta

---

**Versão**: 1.0  
**Última Atualização**: 2025-01-20  
**Autor**: Sistema de Documentação Arah

---

## 📚 Referências Relacionadas

- **[Estratégia de Testes](./30_FLUTTER_TESTING_STRATEGY.md)** - Como testar métricas, logs e exceções
- **[Guia de Acessibilidade](./31_FLUTTER_ACCESSIBILITY_GUIDE.md)** - Acessibilidade de componentes de logging/erro
- **[Planejamento do Frontend Flutter](./24_FLUTTER_FRONTEND_PLAN.md)** - Arquitetura completa do app
