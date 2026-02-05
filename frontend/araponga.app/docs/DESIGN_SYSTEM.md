# Sistema de design – manutenção fácil

O visual do app é controlado por **dois arquivos** e pelo **tema**. Alterar um deles atualiza o app inteiro.

---

## Onde alterar cada coisa

| O que alterar | Arquivo | O que fazer |
|---------------|---------|-------------|
| **Cores** (primária, superfícies, texto) | `lib/core/theme/app_design_tokens.dart` | Editar `AppDesignTokens`: `primaryDark`, `surfaceDark`, `surfaceVariantDark`, etc. Para tema claro: `primaryLight`, `surfaceLight`, `onSurfaceLight`. |
| **Radius** (cards, snackbars, botões) | `lib/core/config/constants.dart` | Editar `AppConstants`: `radiusSm`, `radiusMd`, `radiusLg`. O tema usa em cards e snackbars. |
| **Espaçamento** (padding, margens) | `lib/core/config/constants.dart` | Editar `AppConstants`: `spacingXs` até `spacingXl`. Usar nas telas: `EdgeInsets.all(AppConstants.spacingMd)`. |
| **Tamanho mínimo de toque** | `lib/core/config/constants.dart` | `minTouchTargetSize` (44). Usado no tema em IconButton e FilledButton. |
| **Duração de animações** | `lib/core/config/constants.dart` | `animationFast`, `animationNormal`. Usar em animações: `Duration(milliseconds: AppConstants.animationNormal)`. |
| **Componentes do Material** (AppBar, Card, SnackBar, Input) | `lib/core/theme/app_theme.dart` | Ajustar `AppBarTheme`, `cardTheme`, `snackBarTheme`, `inputDecorationTheme`, etc. Cores vêm de `AppDesignTokens` / `AppColors`. |

---

## Fluxo de cores

1. **`app_design_tokens.dart`** define as cores brutas (ex.: `primaryDark = Color(0xFF81C784)`).
2. **`AppColors`** (ThemeExtension) agrupa cores por tema (dark/light) e é injetado no `ThemeData`.
3. **`AppTheme`** monta o `ColorScheme` e os componentes usando `AppDesignTokens` e `AppColors`.
4. Nas telas: usar `Theme.of(context).colorScheme.primary` ou `context.appColors.primary` (evitar `Color(0xFF...)`).

Assim, para mudar a cor primária do app: altere **só** `primaryDark` e `primaryLight` em `app_design_tokens.dart`.

---

## Uso nas telas

```dart
// Cores (preferir tema)
Theme.of(context).colorScheme.primary
Theme.of(context).colorScheme.onSurfaceVariant
context.appColors.primary   // extensão opcional

// Espaçamento e radius (sempre tokens)
padding: EdgeInsets.all(AppConstants.spacingMd)
borderRadius: BorderRadius.circular(AppConstants.radiusMd)

// Tipografia (preferir textTheme)
Theme.of(context).textTheme.titleMedium
Theme.of(context).textTheme.bodyLarge
```

---

## Novas cores ou tokens

- **Nova cor semântica:** adicione em `AppDesignTokens` e em `AppColors` (dark/light); use no tema ou em `context.appColors.novaCor`.
- **Novo espaçamento/radius:** adicione em `AppConstants` e use nas telas; se for usado no tema (ex.: novo componente), use em `app_theme.dart`.

---

## Resumo

| Arquivo | Responsabilidade |
|---------|------------------|
| `lib/core/theme/app_design_tokens.dart` | Paleta (cores), radius de componentes, insets de snackbar, fontSize de snackbar. |
| `lib/core/config/constants.dart` | Espaçamento, radius genérico, animação, touch target, keys de storage, pageSize. |
| `lib/core/theme/app_theme.dart` | Montagem do ThemeData (ColorScheme, AppBarTheme, cardTheme, etc.) a partir dos tokens. |

Mantendo cores em `app_design_tokens.dart`, layout em `constants.dart` e tema em `app_theme.dart`, a manutenção do design fica centralizada e previsível.
