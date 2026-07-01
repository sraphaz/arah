alize os proximos passos em toda a documentação para # Estratégia de Testes - Arah Flutter App

**Versão**: 1.0  
**Data**: 2025-01-20  
**Status**: 📋 Estratégia Completa  
**Tipo**: Documentação Técnica de Testes

---

## 📋 Índice

1. [Visão Geral](#visão-geral)
2. [Tipos de Testes](#tipos-de-testes)
3. [Estrutura de Testes](#estrutura-de-testes)
4. [Widget Tests](#widget-tests)
5. [Integration Tests](#integration-tests)
6. [Golden Tests](#golden-tests)
7. [Testes de Acessibilidade](#testes-de-acessibilidade)
8. [Testes de Performance](#testes-de-performance)
9. [Testes de API (Mock)](#testes-de-api-mock)
10. [CI/CD e Testes Automatizados](#cicd-e-testes-automatizados)
11. [Cobertura de Testes](#cobertura-de-testes)
12. [Boas Práticas](#boas-práticas)

---

## 🎯 Visão Geral

### Objetivo

Este documento especifica a **estratégia completa de testes** para o app Flutter Arah, cobrindo todos os níveis de teste necessários para garantir qualidade, confiabilidade e manutenibilidade do código.

### Princípios Fundamentais

1. **Test-Driven Development (TDD)**: Quando possível, escrever testes antes do código
2. **Testes Automatizados**: Todos os testes devem ser executáveis automaticamente
3. **Testes Isolados**: Cada teste deve ser independente e isolado
4. **Testes Rápidos**: Unit tests devem ser rápidos (< 100ms cada)
5. **Cobertura Adequada**: Focar em código crítico (lógica de negócio, segurança)
6. **Testes Mantíveis**: Testes devem ser fáceis de ler, entender e manter

### Pirâmide de Testes

```
        ┌─────────────┐
        │ E2E Tests   │  (10%) - Integration Tests
        │             │        - Testes de fluxo completo
        ├─────────────┤
        │ Widget Tests│  (30%) - UI Tests
        │             │        - Componentes e telas
        ├─────────────┤
        │ Unit Tests  │  (60%) - Lógica de negócio
        │             │        - Services, Repositories
        └─────────────┘
```

---

## 📊 Tipos de Testes

### 1. Unit Tests

**Objetivo**: Testar lógica de negócio isolada (funções, métodos, classes)

**Onde usar**:
- Services (API, Storage, Navigation)
- Repositories
- Models (validações, serialização)
- Utilitários (helpers, validators)
- Providers (lógica de estado)

**Ferramentas**:
- `flutter_test` (padrão)
- `mockito` (mocks)
- `fake_async` (timers assíncronos)

### 2. Widget Tests

**Objetivo**: Testar widgets isolados e interações de UI

**Onde usar**:
- Componentes reutilizáveis (buttons, cards, inputs)
- Telas completas (screens)
- Navegação (rotas, deep linking)
- Estados visuais (loading, error, success)

**Ferramentas**:
- `flutter_test` (WidgetTester)
- `mocktail` (mocks para Riverpod)
- `golden_toolkit` (golden tests)

### 3. Integration Tests

**Objetivo**: Testar fluxos completos end-to-end

**Onde usar**:
- Jornadas críticas do usuário (onboarding, autenticação, criação de post)
- Fluxos multi-tela
- Integração com serviços externos (Firebase, API)

**Ferramentas**:
- `integration_test` (Flutter SDK)
- `flutter_driver` (legacy, não recomendado)

### 4. Golden Tests

**Objetivo**: Testar renderização visual de componentes

**Onde usar**:
- Design system components
- Telas críticas (verificação visual)
- Dark mode (comparação light/dark)

**Ferramentas**:
- `golden_toolkit`
- `flutter_test` (matchesGoldenFile)

---

## 📁 Estrutura de Testes

```
test/
├── unit/                              # Unit Tests
│   ├── core/
│   │   ├── network/
│   │   │   └── dio_client_test.dart
│   │   ├── storage/
│   │   │   └── secure_storage_test.dart
│   │   └── utils/
│   │       └── validators_test.dart
│   ├── shared/
│   │   ├── services/
│   │   │   ├── observability/
│   │   │   │   ├── metrics_service_test.dart
│   │   │   │   ├── logging_service_test.dart
│   │   │   │   └── exception_service_test.dart
│   │   │   └── privacy/
│   │   │       └── consent_service_test.dart
│   │   └── providers/
│   │       ├── auth_provider_test.dart
│   │       └── session_provider_test.dart
│   └── features/
│       ├── auth/
│       │   ├── data/
│       │   │   └── auth_repository_test.dart
│       │   └── domain/
│       │       └── auth_service_test.dart
│       ├── feed/
│       │   └── data/
│       │       └── feed_repository_test.dart
│       └── territories/
│           └── data/
│               └── territory_repository_test.dart
│
├── widget/                             # Widget Tests
│   ├── shared/
│   │   └── widgets/
│   │       ├── buttons/
│   │       │   └── primary_button_test.dart
│   │       ├── cards/
│   │       │   ├── post_card_test.dart
│   │       │   └── glass_card_test.dart
│   │       └── inputs/
│   │           └── text_field_test.dart
│   └── features/
│       ├── feed/
│       │   ├── screens/
│       │   │   ├── feed_screen_test.dart
│       │   │   └── post_detail_screen_test.dart
│       │   └── widgets/
│       │       └── post_list_test.dart
│       ├── auth/
│       │   └── screens/
│       │       └── login_screen_test.dart
│       └── territories/
│           └── screens/
│               └── territory_list_screen_test.dart
│
├── integration/                        # Integration Tests
│   ├── flows/
│   │   ├── onboarding_flow_test.dart
│   │   ├── authentication_flow_test.dart
│   │   ├── create_post_flow_test.dart
│   │   └── territory_selection_flow_test.dart
│   └── helpers/
│       └── test_helpers.dart
│
├── golden/                             # Golden Tests
│   ├── components/
│   │   ├── primary_button_golden_test.dart
│   │   └── post_card_golden_test.dart
│   └── screens/
│       ├── feed_screen_golden_test.dart
│       └── profile_screen_golden_test.dart
│
└── helpers/                            # Test Helpers
    ├── mock_factories.dart
    ├── test_data.dart
    ├── test_widgets.dart
    └── test_providers.dart
```

---

## 🧩 Widget Tests

### Estrutura Básica

```dart
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:hooks_riverpod/hooks_riverpod.dart';
import 'package:mocktail/mocktail.dart';

void main() {
  group('PrimaryButton Widget Tests', () {
    testWidgets('should display button with text', (WidgetTester tester) async {
      // Arrange
      const buttonText = 'Click Me';
      var tapped = false;
      
      // Act
      await tester.pumpWidget(
        MaterialApp(
          home: Scaffold(
            body: PrimaryButton(
              text: buttonText,
              onPressed: () => tapped = true,
            ),
          ),
        ),
      );
      
      // Assert
      expect(find.text(buttonText), findsOneWidget);
      expect(tapped, false);
      
      // Act - Tap button
      await tester.tap(find.text(buttonText));
      await tester.pump();
      
      // Assert
      expect(tapped, true);
    });
    
    testWidgets('should be disabled when onPressed is null', (WidgetTester tester) async {
      // Arrange
      const buttonText = 'Disabled Button';
      
      // Act
      await tester.pumpWidget(
        MaterialApp(
          home: Scaffold(
            body: PrimaryButton(
              text: buttonText,
              onPressed: null,
            ),
          ),
        ),
      );
      
      // Assert
      final button = tester.widget<PrimaryButton>(find.text(buttonText));
      expect(button.onPressed, isNull);
      
      // Try to tap (should not crash)
      await tester.tap(find.text(buttonText));
      await tester.pump();
    });
  });
}
```

### Testes com Riverpod

```dart
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:hooks_riverpod/hooks_riverpod.dart';
import 'package:mocktail/mocktail.dart';

// Mock Provider
class MockFeedRepository extends Mock implements FeedRepository {}

void main() {
  group('FeedScreen Widget Tests', () {
    late MockFeedRepository mockRepository;
    
    setUp(() {
      mockRepository = MockFeedRepository();
    });
    
    testWidgets('should display loading indicator when loading', (WidgetTester tester) async {
      // Arrange
      when(() => mockRepository.getFeed(territoryId: any(named: 'territoryId')))
          .thenAnswer((_) async => []);
      
      // Act
      await tester.pumpWidget(
        ProviderScope(
          overrides: [
            feedRepositoryProvider.overrideWithValue(mockRepository),
          ],
          child: MaterialApp(
            home: FeedScreen(territoryId: 'test-territory-id'),
          ),
        ),
      );
      
      // Assert
      expect(find.byType(CircularProgressIndicator), findsOneWidget);
    });
    
    testWidgets('should display posts when loaded', (WidgetTester tester) async {
      // Arrange
      final posts = [
        Post(id: '1', title: 'Post 1', content: 'Content 1'),
        Post(id: '2', title: 'Post 2', content: 'Content 2'),
      ];
      
      when(() => mockRepository.getFeed(territoryId: any(named: 'territoryId')))
          .thenAnswer((_) async => posts);
      
      // Act
      await tester.pumpWidget(
        ProviderScope(
          overrides: [
            feedRepositoryProvider.overrideWithValue(mockRepository),
          ],
          child: MaterialApp(
            home: FeedScreen(territoryId: 'test-territory-id'),
          ),
        ),
      );
      
      // Wait for async loading
      await tester.pumpAndSettle();
      
      // Assert
      expect(find.text('Post 1'), findsOneWidget);
      expect(find.text('Post 2'), findsOneWidget);
      expect(find.byType(PostCard), findsNWidgets(2));
    });
    
    testWidgets('should display error message when error occurs', (WidgetTester tester) async {
      // Arrange
      when(() => mockRepository.getFeed(territoryId: any(named: 'territoryId')))
          .thenThrow(Exception('Network error'));
      
      // Act
      await tester.pumpWidget(
        ProviderScope(
          overrides: [
            feedRepositoryProvider.overrideWithValue(mockRepository),
          ],
          child: MaterialApp(
            home: FeedScreen(territoryId: 'test-territory-id'),
          ),
        ),
      );
      
      // Wait for async loading
      await tester.pumpAndSettle();
      
      // Assert
      expect(find.text('Erro ao carregar feed'), findsOneWidget);
      expect(find.text('Network error'), findsOneWidget);
    });
  });
}
```

### Testes de Interação

```dart
testWidgets('should handle pull-to-refresh', (WidgetTester tester) async {
  // Arrange
  final posts = [Post(id: '1', title: 'Post 1')];
  
  when(() => mockRepository.getFeed(territoryId: any(named: 'territoryId')))
      .thenAnswer((_) async => posts);
  
  await tester.pumpWidget(
    ProviderScope(
      overrides: [feedRepositoryProvider.overrideWithValue(mockRepository)],
      child: MaterialApp(home: FeedScreen(territoryId: 'test-id')),
    ),
  );
  
  await tester.pumpAndSettle();
  
  // Act - Pull to refresh
  final gesture = await tester.startGesture(tester.getCenter(find.byType(ListView)));
  await gesture.moveBy(Offset(0, 300));
  await tester.pumpAndSettle();
  
  // Assert
  verify(() => mockRepository.getFeed(territoryId: 'test-id')).called(2); // Initial + refresh
});
```

---

## 🔄 Integration Tests

### Estrutura Básica

```dart
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:integration_test/integration_test.dart';
import 'package:Arah/main.dart' as app;

void main() {
  IntegrationTestWidgetsFlutterBinding.ensureInitialized();
  
  group('Onboarding Flow Integration Test', () {
    testWidgets('complete onboarding flow', (WidgetTester tester) async {
      // Start app
      app.main();
      await tester.pumpAndSettle();
      
      // Step 1: Splash screen should appear
      expect(find.text('Arah'), findsOneWidget);
      await tester.pumpAndSettle(Duration(seconds: 2));
      
      // Step 2: Onboarding screen should appear
      expect(find.text('Bem-vinda ao Arah'), findsOneWidget);
      await tester.tap(find.text('Começar'));
      await tester.pumpAndSettle();
      
      // Step 3: Location permission screen
      expect(find.text('Descubra seu território'), findsOneWidget);
      await tester.tap(find.text('Permitir Localização'));
      await tester.pumpAndSettle();
      
      // Step 4: Login screen should appear
      expect(find.text('Entre ou crie sua conta'), findsOneWidget);
    });
  });
  
  group('Authentication Flow Integration Test', () {
    testWidgets('complete authentication flow', (WidgetTester tester) async {
      app.main();
      await tester.pumpAndSettle();
      
      // Skip onboarding if present
      if (find.text('Começar').evaluate().isNotEmpty) {
        await tester.tap(find.text('Começar'));
        await tester.pumpAndSettle();
      }
      
      // Tap Google Sign-In
      await tester.tap(find.text('Continuar com Google'));
      await tester.pumpAndSettle();
      
      // Wait for authentication (mock or real)
      await tester.pumpAndSettle(Duration(seconds: 3));
      
      // Should navigate to territory selection
      expect(find.text('Encontre seu território'), findsOneWidget);
    });
  });
  
  group('Create Post Flow Integration Test', () {
    testWidgets('create and publish a post', (WidgetTester tester) async {
      app.main();
      await tester.pumpAndSettle();
      
      // Navigate to feed
      await tester.tap(find.byIcon(Icons.home));
      await tester.pumpAndSettle();
      
      // Tap FAB to create post
      await tester.tap(find.byIcon(Icons.add));
      await tester.pumpAndSettle();
      
      // Fill form
      await tester.enterText(find.byType(TextField).first, 'Título do Post');
      await tester.enterText(find.byType(TextField).last, 'Conteúdo do post aqui...');
      await tester.pumpAndSettle();
      
      // Select visibility
      await tester.tap(find.text('Público'));
      await tester.pumpAndSettle();
      
      // Tap publish
      await tester.tap(find.text('Publicar'));
      await tester.pumpAndSettle();
      
      // Should navigate back to feed with success message
      expect(find.text('Post publicado com sucesso'), findsOneWidget);
      expect(find.text('Título do Post'), findsOneWidget);
    });
  });
}
```

### Helpers para Integration Tests

```dart
// test/helpers/test_helpers.dart
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

class TestHelpers {
  static Future<void> waitForAppToLoad(WidgetTester tester) async {
    await tester.pumpAndSettle(Duration(seconds: 2));
  }
  
  static Future<void> tapButtonWithText(WidgetTester tester, String text) async {
    await tester.tap(find.text(text));
    await tester.pumpAndSettle();
  }
  
  static Future<void> enterTextInField(WidgetTester tester, String text, int fieldIndex) async {
    final fields = find.byType(TextField);
    await tester.enterText(fields.at(fieldIndex), text);
    await tester.pumpAndSettle();
  }
  
  static Future<void> scrollToWidget(WidgetTester tester, Finder finder) async {
    await tester.scrollUntilVisible(finder, 500, scrollable: find.byType(Scrollable));
    await tester.pumpAndSettle();
  }
}
```

---

## 🎨 Golden Tests

### Setup

```yaml
# pubspec.yaml
dev_dependencies:
  golden_toolkit: ^0.15.0
```

### Exemplo de Golden Test

```dart
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:golden_toolkit/golden_toolkit.dart';
import 'package:Arah/shared/widgets/buttons/primary_button.dart';

void main() {
  group('PrimaryButton Golden Tests', () {
    testGoldens('should match primary button golden', (WidgetTester tester) async {
      // Arrange
      final builder = DeviceBuilder()
        ..overrideDevicesForAllScenarios(devices: [
          Device.phone,
          Device.tabletPortrait,
          Device.tabletLandscape,
        ])
        ..addScenario(
          name: 'default',
          widget: PrimaryButton(
            text: 'Clique aqui',
            onPressed: () {},
          ),
        )
        ..addScenario(
          name: 'disabled',
          widget: PrimaryButton(
            text: 'Desabilitado',
            onPressed: null,
          ),
        );
      
      // Act & Assert
      await tester.pumpDeviceBuilder(builder);
      await screenMatchesGolden(tester, 'primary_button');
    });
    
    testGoldens('should match primary button in dark mode', (WidgetTester tester) async {
      // Arrange
      final widget = MaterialApp(
        theme: ThemeData.light(),
        darkTheme: ThemeData.dark(),
        themeMode: ThemeMode.dark,
        home: Scaffold(
          body: PrimaryButton(
            text: 'Clique aqui',
            onPressed: () {},
          ),
        ),
      );
      
      // Act & Assert
      await tester.pumpWidget(widget);
      await screenMatchesGolden(tester, 'primary_button_dark');
    });
  });
}
```

### Executar Golden Tests

```bash
flutter test --update-goldens
flutter test integration_test/golden/
```

---

## ♿ Testes de Acessibilidade

### Semantics Tests

```dart
testWidgets('should have correct semantics for accessibility', (WidgetTester tester) async {
  // Arrange
  await tester.pumpWidget(
    MaterialApp(
      home: Scaffold(
        body: PrimaryButton(
          text: 'Clique aqui',
          onPressed: () {},
        ),
      ),
    ),
  );
  
  // Assert
  expect(
    find.bySemanticsLabel('Clique aqui'),
    findsOneWidget,
    reason: 'Button should have semantic label',
  );
  
  final semantics = tester.getSemantics(find.byType(PrimaryButton));
  expect(semantics.hasTapAction, true, reason: 'Button should be tappable');
  expect(semantics.isButton, true, reason: 'Button should be identified as button');
});
```

### Acessibilidade Completa

```dart
testWidgets('should be accessible', (WidgetTester tester) async {
  await tester.pumpWidget(
    MaterialApp(
      home: FeedScreen(territoryId: 'test-id'),
    ),
  );
  
  await tester.pumpAndSettle();
  
  // Verify semantics tree
  final semantics = tester.getSemantics(find.byType(FeedScreen));
  
  // Check for proper labels
  expect(semantics.hasLabel, true);
  
  // Check for proper hint
  expect(semantics.hasHint, true);
  
  // Check for proper actions
  expect(semantics.hasTapAction || semantics.hasScrollAction, true);
});
```

---

## ⚡ Testes de Performance

### Performance Tests

```dart
import 'package:flutter_test/flutter_test.dart';

void main() {
  testWidgets('feed screen should render within performance budget', (WidgetTester tester) async {
    // Arrange
    final posts = List.generate(50, (i) => Post(id: '$i', title: 'Post $i'));
    
    // Act
    final stopwatch = Stopwatch()..start();
    
    await tester.pumpWidget(
      ProviderScope(
        child: MaterialApp(
          home: FeedScreen(posts: posts),
        ),
      ),
    );
    
    await tester.pumpAndSettle();
    
    stopwatch.stop();
    
    // Assert
    expect(stopwatch.elapsedMilliseconds, lessThan(500), 
      reason: 'Feed screen should render in less than 500ms',
    );
  });
  
  testWidgets('post card should render efficiently', (WidgetTester tester) async {
    // Arrange
    final post = Post(id: '1', title: 'Test Post', content: 'Content');
    
    // Act
    final stopwatch = Stopwatch()..start();
    
    await tester.pumpWidget(
      MaterialApp(
        home: Scaffold(
          body: PostCard(post: post),
        ),
      ),
    );
    
    await tester.pumpAndSettle();
    
    stopwatch.stop();
    
    // Assert
    expect(stopwatch.elapsedMilliseconds, lessThan(100),
      reason: 'Post card should render in less than 100ms',
    );
  });
}
```

---

## 🌐 Testes de API (Mock)

### Mock Repository

```dart
import 'package:mocktail/mocktail.dart';

class MockFeedRepository extends Mock implements FeedRepository {}

void main() {
  group('FeedRepository Unit Tests', () {
    late MockFeedRepository repository;
    
    setUp(() {
      repository = MockFeedRepository();
    });
    
    test('should return posts when API call succeeds', () async {
      // Arrange
      final expectedPosts = [
        Post(id: '1', title: 'Post 1'),
        Post(id: '2', title: 'Post 2'),
      ];
      
      when(() => repository.getFeed(territoryId: any(named: 'territoryId')))
          .thenAnswer((_) async => expectedPosts);
      
      // Act
      final result = await repository.getFeed(territoryId: 'test-id');
      
      // Assert
      expect(result, equals(expectedPosts));
      verify(() => repository.getFeed(territoryId: 'test-id')).called(1);
    });
    
    test('should throw exception when API call fails', () async {
      // Arrange
      when(() => repository.getFeed(territoryId: any(named: 'territoryId')))
          .thenThrow(Exception('Network error'));
      
      // Act & Assert
      expect(
        () => repository.getFeed(territoryId: 'test-id'),
        throwsException,
      );
    });
  });
}
```

### Mock Dio Client

```dart
import 'package:dio/dio.dart';
import 'package:mocktail/mocktail.dart';

class MockDio extends Mock implements Dio {}

void main() {
  group('ApiService Unit Tests', () {
    late MockDio mockDio;
    late ApiService apiService;
    
    setUp(() {
      mockDio = MockDio();
      apiService = ApiService(dio: mockDio);
    });
    
    test('should return data when GET request succeeds', () async {
      // Arrange
      final responseData = {'id': '1', 'title': 'Post 1'};
      when(() => mockDio.get(any()))
          .thenAnswer((_) async => Response(
            data: responseData,
            statusCode: 200,
            requestOptions: RequestOptions(path: '/feed'),
          ));
      
      // Act
      final result = await apiService.getFeed(territoryId: 'test-id');
      
      // Assert
      expect(result, isNotNull);
      verify(() => mockDio.get('/feed')).called(1);
    });
  });
}
```

---

## 🔄 CI/CD e Testes Automatizados

### GitHub Actions

```yaml
# .github/workflows/test.yml
name: Tests

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main, develop ]

jobs:
  test:
    runs-on: ubuntu-latest
    
    steps:
      - uses: actions/checkout@v3
      
      - uses: subosito/flutter-action@v2
        with:
          flutter-version: '3.19.0'
          channel: 'stable'
      
      - name: Get dependencies
        run: flutter pub get
      
      - name: Run unit tests
        run: flutter test test/unit/
      
      - name: Run widget tests
        run: flutter test test/widget/
      
      - name: Run integration tests (if configured)
        run: flutter test integration_test/
        continue-on-error: true
      
      - name: Generate coverage
        run: flutter test --coverage
      
      - name: Upload coverage to Codecov
        uses: codecov/codecov-action@v3
        with:
          files: coverage/lcov.info
```

### Scripts de Teste

```bash
# scripts/test.sh
#!/bin/bash

echo "Running unit tests..."
flutter test test/unit/ || exit 1

echo "Running widget tests..."
flutter test test/widget/ || exit 1

echo "Running integration tests..."
flutter test integration_test/ || exit 1

echo "All tests passed!"
```

---

## 📊 Cobertura de Testes

### Meta de Cobertura

- **Unit Tests**: 80%+ (lógica de negócio crítica: 90%+)
- **Widget Tests**: 70%+ (componentes principais: 100%)
- **Integration Tests**: 50%+ (fluxos críticos: 100%)

### Gerar Relatório de Cobertura

```bash
flutter test --coverage
genhtml coverage/lcov.info -o coverage/html
open coverage/html/index.html
```

### Excluir do Relatório

```yaml
# lcov.info (editar manualmente ou usar ferramenta)
# Excluir arquivos gerados, modelos, etc.
```

---

## ✅ Boas Práticas

### 1. Nomenclatura

- **Arquivos**: `{feature}_{type}_test.dart` (ex: `feed_repository_test.dart`)
- **Grupos**: `group('FeatureName Tests', () { ... })`
- **Testes**: `test('should do something when condition', () { ... })`

### 2. Estrutura AAA

- **Arrange**: Preparar dados e mocks
- **Act**: Executar ação a ser testada
- **Assert**: Verificar resultado esperado

### 3. Isolamento

- Cada teste deve ser independente
- Usar `setUp()` e `tearDown()` para preparação/limpeza
- Não depender de ordem de execução

### 4. Mocks

- Usar `mocktail` ou `mockito` para mocks
- Verificar chamadas com `verify()`
- Resetar mocks entre testes

### 5. Testes Mantíveis

- Nomes descritivos e claros
- Comentários quando necessário
- Helpers para código repetitivo
- Test data factories para dados de teste

### 6. Performance

- Unit tests devem ser rápidos (< 100ms)
- Widget tests devem ser moderados (< 1s)
- Integration tests podem ser mais lentos (< 30s)

---

**Versão**: 1.0  
**Última Atualização**: 2025-01-20  
**Referências**: 
- [Flutter Testing Documentation](https://docs.flutter.dev/testing)
- [Riverpod Testing Guide](https://riverpod.dev/docs/essentials/testing)
- [Golden Toolkit](https://pub.dev/packages/golden_toolkit)
