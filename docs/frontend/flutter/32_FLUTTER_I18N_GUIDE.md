# Guia de Internacionalização (i18n) - Arah Flutter App

**Versão**: 1.0  
**Data**: 2025-01-20  
**Status**: 🌐 Guia Completo de Internacionalização  
**Tipo**: Documentação Técnica de i18n

---

## 📋 Índice

1. [Visão Geral](#visão-geral)
2. [Idiomas Suportados](#idiomas-suportados)
3. [Configuração Inicial](#configuração-inicial)
4. [Estrutura de Arquivos](#estrutura-de-arquivos)
5. [Arquivos ARB](#arquivos-arb)
6. [Uso no Código](#uso-no-código)
7. [Formatação de Datas e Números](#formatação-de-datas-e-números)
8. [Pluralização](#pluralização)
9. [Localização de Recursos](#localização-de-recursos)
10. [Testes de i18n](#testes-de-i18n)
11. [Boas Práticas](#boas-práticas)

---

## 🎯 Visão Geral

### Objetivo

Este documento especifica a **estratégia completa de internacionalização (i18n)** para o app Flutter Arah, permitindo suporte a múltiplos idiomas e localizações (formatação de datas, números, moedas).

### Idiomas Iniciais

- **pt-BR** (Português Brasileiro) - Padrão
- **en-US** (Inglês Americano) - Secundário

### Idiomas Futuros

- **es-ES** (Espanhol) - Planejado
- **fr-FR** (Francês) - Planejado

---

## 🌍 Idiomas Suportados

### Locale Atual

**Padrão**: `pt-BR` (Português Brasileiro)

**Suportado**: `en-US` (Inglês Americano)

### Detecção de Idioma

**Ordem de Prioridade**:
1. Preferência do usuário no app (se definida)
2. Idioma do sistema operacional
3. `pt-BR` (fallback padrão)

---

## ⚙️ Configuração Inicial

### Dependências

```yaml
# pubspec.yaml
dependencies:
  flutter:
    sdk: flutter
  flutter_localizations:
    sdk: flutter
  intl: ^0.19.0

# Gerar localizações automaticamente
flutter:
  generate: true

# Configuração de localizações
flutter_localizations:
  supported_locales:
    - pt
    - en
  locale: pt
```

### Configuração em main.dart

```dart
import 'package:flutter/material.dart';
import 'package:flutter_localizations/flutter_localizations.dart';
import 'package:flutter_gen/gen_l10n/app_localizations.dart';

void main() {
  runApp(MyApp());
}

class MyApp extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Arah',
      
      // Suportar localizações
      localizationsDelegates: const [
        AppLocalizations.delegate,
        GlobalMaterialLocalizations.delegate,
        GlobalWidgetsLocalizations.delegate,
        GlobalCupertinoLocalizations.delegate,
      ],
      
      // Locales suportados
      supportedLocales: const [
        Locale('pt', 'BR'),  // Português Brasileiro
        Locale('en', 'US'),  // Inglês Americano
      ],
      
      // Locale padrão
      locale: const Locale('pt', 'BR'),
      
      // Detectar locale do sistema
      // localeResolutionCallback: (locale, supportedLocales) {
      //   return locale;
      // },
      
      home: MyHomePage(),
    );
  }
}
```

---

## 📁 Estrutura de Arquivos

```
lib/
├── l10n/                              # Localizações
│   ├── app_pt.arb                     # Português Brasileiro
│   ├── app_en.arb                     # Inglês Americano
│   └── app_es.arb                     # Espanhol (futuro)
│
└── generated/                         # Gerado automaticamente
    └── l10n/
        ├── app_localizations.dart
        ├── app_localizations_pt.dart
        └── app_localizations_en.dart
```

### Configuração l10n.yaml

```yaml
# l10n.yaml
arb-dir: lib/l10n
template-arb-file: app_pt.arb
output-localization-file: app_localizations.dart
```

---

## 📝 Arquivos ARB

### Formato ARB

**ARB (Application Resource Bundle)**: Formato JSON para localizações

### Exemplo: app_pt.arb

```json
{
  "@@locale": "pt-BR",
  "appName": "Arah",
  "@appName": {
    "description": "Nome do aplicativo"
  },
  
  "welcomeMessage": "Bem-vinda ao Arah",
  "@welcomeMessage": {
    "description": "Mensagem de boas-vindas"
  },
  
  "discoverYourTerritory": "Descubra seu território",
  "@discoverYourTerritory": {
    "description": "Título da tela de descoberta de territórios"
  },
  
  "allowLocation": "Permitir Localização",
  "@allowLocation": {
    "description": "Botão para permitir localização"
  },
  
  "continueWithoutLocation": "Continuar sem Localização",
  "@continueWithoutLocation": {
    "description": "Botão para continuar sem localização"
  },
  
  "loginOrCreateAccount": "Entre ou crie sua conta",
  "@loginOrCreateAccount": {
    "description": "Título da tela de login"
  },
  
  "continueWithGoogle": "Continuar com Google",
  "@continueWithGoogle": {
    "description": "Botão de login com Google"
  },
  
  "continueWithApple": "Continuar com Apple",
  "@continueWithApple": {
    "description": "Botão de login com Apple"
  },
  
  "feed": "Feed",
  "@feed": {
    "description": "Tab de feed"
  },
  
  "map": "Mapa",
  "@map": {
    "description": "Tab de mapa"
  },
  
  "events": "Eventos",
  "@events": {
    "description": "Tab de eventos"
  },
  
  "notifications": "Notificações",
  "@notifications": {
    "description": "Tab de notificações"
  },
  
  "profile": "Perfil",
  "@profile": {
    "description": "Tab de perfil"
  },
  
  "createPost": "Criar Post",
  "@createPost": {
    "description": "Botão para criar novo post"
  },
  
  "postTitle": "Título do Post",
  "@postTitle": {
    "description": "Label do campo título",
    "placeholders": {
      "maxLength": {
        "type": "int",
        "format": "decimalPattern"
      }
    }
  },
  
  "postTitleHint": "Título do post (opcional, máximo {maxLength} caracteres)",
  "@postTitleHint": {
    "description": "Hint do campo título",
    "placeholders": {
      "maxLength": {
        "type": "int",
        "format": "decimalPattern"
      }
    }
  },
  
  "postContent": "Conteúdo",
  "@postContent": {
    "description": "Label do campo conteúdo"
  },
  
  "postContentHint": "O que está acontecendo no território?",
  "@postContentHint": {
    "description": "Hint do campo conteúdo"
  },
  
  "publish": "Publicar",
  "@publish": {
    "description": "Botão para publicar post"
  },
  
  "save": "Salvar",
  "@save": {
    "description": "Botão para salvar"
  },
  
  "cancel": "Cancelar",
  "@cancel": {
    "description": "Botão para cancelar"
  },
  
  "loading": "Carregando...",
  "@loading": {
    "description": "Indicador de carregamento"
  },
  
  "errorOccurred": "Erro ao {action}",
  "@errorOccurred": {
    "description": "Mensagem de erro genérica",
    "placeholders": {
      "action": {
        "type": "String"
      }
    }
  },
  
  "networkError": "Erro de conexão. Verifique sua internet.",
  "@networkError": {
    "description": "Mensagem de erro de rede"
  },
  
  "retry": "Tentar Novamente",
  "@retry": {
    "description": "Botão para tentar novamente"
  },
  
  "postPublished": "Post publicado com sucesso!",
  "@postPublished": {
    "description": "Mensagem de sucesso ao publicar post"
  },
  
  "likePost": "Curtir post",
  "@likePost": {
    "description": "Ação de curtir post"
  },
  
  "unlikePost": "Descurtir post",
  "@unlikePost": {
    "description": "Ação de descurtir post"
  },
  
  "comment": "Comentar",
  "@comment": {
    "description": "Ação de comentar"
  },
  
  "share": "Compartilhar",
  "@share": {
    "description": "Ação de compartilhar"
  },
  
  "timeAgo": "{count, plural, =0{agora} =1{há 1 minuto} other{há {count} minutos}}",
  "@timeAgo": {
    "description": "Tempo relativo (pluralização)",
    "placeholders": {
      "count": {
        "type": "int",
        "format": "decimalPattern"
      }
    }
  },
  
  "visitor": "Visitante",
  "@visitor": {
    "description": "Papel de visitante"
  },
  
  "resident": "Morador",
  "@resident": {
    "description": "Papel de morador"
  },
  
  "becomeResident": "Solicitar Residência",
  "@becomeResident": {
    "description": "Botão para solicitar residência"
  },
  
  "membershipPending": "Aguardando aprovação",
  "@membershipPending": {
    "description": "Status de membrosia pendente"
  }
}
```

### Exemplo: app_en.arb

```json
{
  "@@locale": "en-US",
  "appName": "Arah",
  
  "welcomeMessage": "Welcome to Arah",
  "discoverYourTerritory": "Discover your territory",
  "allowLocation": "Allow Location",
  "continueWithoutLocation": "Continue without Location",
  "loginOrCreateAccount": "Login or create account",
  "continueWithGoogle": "Continue with Google",
  "continueWithApple": "Continue with Apple",
  "feed": "Feed",
  "map": "Map",
  "events": "Events",
  "notifications": "Notifications",
  "profile": "Profile",
  "createPost": "Create Post",
  "postTitle": "Post Title",
  "postTitleHint": "Post title (optional, max {maxLength} characters)",
  "postContent": "Content",
  "postContentHint": "What's happening in the territory?",
  "publish": "Publish",
  "save": "Save",
  "cancel": "Cancel",
  "loading": "Loading...",
  "errorOccurred": "Error occurred while {action}",
  "networkError": "Connection error. Check your internet.",
  "retry": "Retry",
  "postPublished": "Post published successfully!",
  "likePost": "Like post",
  "unlikePost": "Unlike post",
  "comment": "Comment",
  "share": "Share",
  "timeAgo": "{count, plural, =0{now} =1{1 minute ago} other{{count} minutes ago}}",
  "visitor": "Visitor",
  "resident": "Resident",
  "becomeResident": "Request Residency",
  "membershipPending": "Pending approval"
}
```

---

## 💻 Uso no Código

### Import

```dart
import 'package:flutter_gen/gen_l10n/app_localizations.dart';
```

### Uso Básico

```dart
// Em um StatelessWidget
Text(AppLocalizations.of(context)!.welcomeMessage)

// Em um StatefulWidget
Text(AppLocalizations.of(context)!.welcomeMessage)

// Com ConsumerWidget (Riverpod)
Text(AppLocalizations.of(context)!.welcomeMessage)
```

### Exemplo Completo

```dart
import 'package:flutter/material.dart';
import 'package:flutter_gen/gen_l10n/app_localizations.dart';

class WelcomeScreen extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    
    return Scaffold(
      appBar: AppBar(
        title: Text(l10n.appName),
      ),
      body: Column(
        children: [
          Text(
            l10n.welcomeMessage,
            style: Theme.of(context).textTheme.headlineMedium,
          ),
          ElevatedButton(
            onPressed: () {},
            child: Text(l10n.continueWithGoogle),
          ),
        ],
      ),
    );
  }
}
```

### Com Placeholders

```dart
// ARB: "errorOccurred": "Erro ao {action}"
Text(AppLocalizations.of(context)!.errorOccurred('carregar feed'))
// Resultado: "Erro ao carregar feed"

// ARB: "postTitleHint": "Título do post (opcional, máximo {maxLength} caracteres)"
Text(AppLocalizations.of(context)!.postTitleHint(200))
// Resultado: "Título do post (opcional, máximo 200 caracteres)"
```

---

## 📅 Formatação de Datas e Números

### Datas

```dart
import 'package:intl/intl.dart';
import 'package:flutter_gen/gen_l10n/app_localizations.dart';

// Formatar data
final dateFormat = DateFormat.yMMMMd(AppLocalizations.of(context)!.localeName);
final formattedDate = dateFormat.format(DateTime.now());
// pt-BR: "20 de janeiro de 2025"
// en-US: "January 20, 2025"

// Formatar data e hora
final dateTimeFormat = DateFormat.yMMMMd().add_jm(AppLocalizations.of(context)!.localeName);
final formattedDateTime = dateTimeFormat.format(DateTime.now());
// pt-BR: "20 de janeiro de 2025 10:30"
// en-US: "January 20, 2025 10:30 AM"

// Formatar tempo relativo
final timeAgo = DateFormat('relativeTime').format(dateTime);
```

### Números

```dart
import 'package:intl/intl.dart';

// Formatar número (ex: 1234.56)
final numberFormat = NumberFormat.decimalPattern(AppLocalizations.of(context)!.localeName);
final formattedNumber = numberFormat.format(1234.56);
// pt-BR: "1.234,56"
// en-US: "1,234.56"

// Formatar moeda
final currencyFormat = NumberFormat.currency(
  locale: AppLocalizations.of(context)!.localeName,
  symbol: 'R\$', // ou '\$' para USD
);
final formattedCurrency = currencyFormat.format(1234.56);
// pt-BR: "R\$ 1.234,56"
// en-US: "\$ 1,234.56"

// Formatar porcentagem
final percentFormat = NumberFormat.percentPattern(AppLocalizations.of(context)!.localeName);
final formattedPercent = percentFormat.format(0.25);
// pt-BR: "25%"
// en-US: "25%"
```

### Exemplo: Formatador Helper

```dart
// lib/shared/utils/formatters.dart
import 'package:intl/intl.dart';
import 'package:flutter_gen/gen_l10n/app_localizations.dart';

class Formatters {
  static String formatDate(BuildContext context, DateTime date) {
    final locale = AppLocalizations.of(context)!.localeName;
    return DateFormat.yMMMMd(locale).format(date);
  }
  
  static String formatDateTime(BuildContext context, DateTime dateTime) {
    final locale = AppLocalizations.of(context)!.localeName;
    return DateFormat.yMMMMd(locale).add_jm(locale).format(dateTime);
  }
  
  static String formatNumber(BuildContext context, double number) {
    final locale = AppLocalizations.of(context)!.localeName;
    return NumberFormat.decimalPattern(locale).format(number);
  }
  
  static String formatCurrency(BuildContext context, double amount, String symbol) {
    final locale = AppLocalizations.of(context)!.localeName;
    return NumberFormat.currency(locale: locale, symbol: symbol).format(amount);
  }
  
  static String formatTimeAgo(BuildContext context, DateTime dateTime) {
    final now = DateTime.now();
    final difference = now.difference(dateTime);
    
    if (difference.inMinutes < 1) {
      return AppLocalizations.of(context)!.timeAgo(0);
    } else if (difference.inMinutes == 1) {
      return AppLocalizations.of(context)!.timeAgo(1);
    } else {
      return AppLocalizations.of(context)!.timeAgo(difference.inMinutes);
    }
  }
}
```

---

## 🔢 Pluralização

### Sintaxe ARB

```json
{
  "timeAgo": "{count, plural, =0{agora} =1{há 1 minuto} other{há {count} minutos}}",
  "@timeAgo": {
    "placeholders": {
      "count": {
        "type": "int",
        "format": "decimalPattern"
      }
    }
  },
  
  "likesCount": "{count, plural, =0{Sem curtidas} =1{1 curtida} other{{count} curtidas}}",
  "@likesCount": {
    "placeholders": {
      "count": {
        "type": "int"
      }
    }
  }
}
```

### Uso no Código

```dart
// Pluralização automática
Text(AppLocalizations.of(context)!.timeAgo(0))      // "agora"
Text(AppLocalizations.of(context)!.timeAgo(1))      // "há 1 minuto"
Text(AppLocalizations.of(context)!.timeAgo(5))      // "há 5 minutos"

Text(AppLocalizations.of(context)!.likesCount(0))   // "Sem curtidas"
Text(AppLocalizations.of(context)!.likesCount(1))   // "1 curtida"
Text(AppLocalizations.of(context)!.likesCount(10))  // "10 curtidas"
```

---

## 📦 Localização de Recursos

### Imagens Localizadas

```
assets/
├── images/
│   ├── onboarding/
│   │   ├── welcome_pt.png
│   │   ├── welcome_en.png
│   │   ├── location_pt.png
│   │   └── location_en.png
```

### Uso

```dart
// Selecionar imagem baseada no locale
String getLocalizedImage(String baseName) {
  final locale = Localizations.localeOf(context);
  final localeCode = locale.languageCode;
  return 'assets/images/$baseName\_$localeCode.png';
}

Image.asset(getLocalizedImage('welcome'))
```

---

## ✅ Testes de i18n

### Testes de Localização

```dart
testWidgets('should display localized text', (WidgetTester tester) async {
  // pt-BR
  await tester.pumpWidget(
    MaterialApp(
      localizationsDelegates: AppLocalizations.localizationsDelegates,
      supportedLocales: AppLocalizations.supportedLocales,
      locale: Locale('pt', 'BR'),
      home: WelcomeScreen(),
    ),
  );
  
  expect(find.text('Bem-vinda ao Arah'), findsOneWidget);
  
  // en-US
  await tester.pumpWidget(
    MaterialApp(
      localizationsDelegates: AppLocalizations.localizationsDelegates,
      supportedLocales: AppLocalizations.supportedLocales,
      locale: Locale('en', 'US'),
      home: WelcomeScreen(),
    ),
  );
  
  expect(find.text('Welcome to Arah'), findsOneWidget);
});
```

---

## ✅ Boas Práticas

### 1. Organização

- Agrupar strings relacionadas no mesmo arquivo ARB
- Usar prefixos para namespaces (ex: `auth_`, `feed_`, `profile_`)
- Documentar todas as strings com `@description`

### 2. Nomenclatura

- Nomes descritivos e claros
- Usar camelCase para chaves
- Evitar abreviações desnecessárias

### 3. Placeholders

- Sempre definir tipos e formatos de placeholders
- Usar pluralização quando apropriado
- Documentar placeholders no `@description`

### 4. Testes

- Testar todos os idiomas suportados
- Verificar formatação de datas e números
- Validar pluralização

### 5. Manutenção

- Revisar strings regularmente
- Traduzir todas as strings para todos os idiomas
- Usar ferramentas de validação ARB

---

**Versão**: 1.0  
**Última Atualização**: 2025-01-20  
**Referências**: 
- [Flutter Internationalization](https://docs.flutter.dev/accessibility-and-localization/internationalization)
- [intl Package](https://pub.dev/packages/intl)
- [ARB Format](https://github.com/google/app-resource-bundle)
