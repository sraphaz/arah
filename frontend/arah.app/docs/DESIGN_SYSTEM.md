# Sistema de design – manutenção fácil

O visual do app é controlado por **tokens**, **componentes reutilizáveis** e pelo **tema**. Alterar um deles atualiza o app inteiro.

---

## Onde alterar cada coisa

| O que alterar | Arquivo | O que fazer |
|---------------|---------|-------------|
| **Cores** (primária, superfícies, glass, watermark) | `lib/core/theme/app_design_tokens.dart` | Editar `AppDesignTokens` e `AppColors` (dark/light). Espelho de `frontend/shared/styles/design-tokens.css`. |
| **Radius** (cards, snackbars, botões) | `lib/core/config/constants.dart` | `radiusXs` … `radius2xl` (escala 4–32px). |
| **Espaçamento** (padding, margens) | `lib/core/config/constants.dart` | `spacingXs` … `spacing3xl`. |
| **Tamanho mínimo de toque** | `lib/core/config/constants.dart` | `minTouchTargetSize` (44). |
| **Duração de animações** | `lib/core/config/constants.dart` | `animationFast`, `animationNormal`, `animationSlow`, `animationSmooth`. |
| **Componentes Material** (AppBar, Card, SnackBar, Chip) | `lib/core/theme/app_theme.dart` | Temas derivados de `AppDesignTokens` / `AppColors`. |
| **Componentes Arah** (botão, card, glass, empty) | `lib/core/widgets/arah_*.dart` | Biblioteca de handoff do design system. |

---

## Componentes do handoff

| Componente | Uso |
|------------|-----|
| `ArahScaffold` | Fundo com gradiente + watermark opcional |
| `ArahGlassCard` | Painéis com blur (login, bottom sheets) |
| `ArahCard` | Cards com elevação e padding padrão |
| `ArahButton` | primary / secondary / text + loading |
| `ArahEmptyState` | Estados vazios com CTA |
| `ArahLoadingIndicator` | Spinner com cor primária |
| `ArahListSkeleton` / `FeedSkeleton` | Loading de listas |
| `ArahBrandHeader` | Logo + wordmark Arah |
| `ArahWatermark` | Marca d'água do logo |

---

## Fluxo de cores

1. **`frontend/shared/styles/design-tokens.css`** — fonte canônica web.
2. **`app_design_tokens.dart`** — espelho Dart para o app Flutter.
3. **`AppColors`** (ThemeExtension) — agrupa cores por tema.
4. **`AppTheme`** — monta `ThemeData` e componentes.
5. Nas telas: `Theme.of(context).colorScheme` ou `context.appColors`.

---

## Uso nas telas

```dart
// Cores
context.appColors.primary
context.appColors.glassBackground

// Componentes
ArahButton(label: l10n.save, onPressed: _save, expand: true)
ArahEmptyState(icon: Icons.inbox, title: l10n.noItemsFound)

// Layout
padding: EdgeInsets.all(AppConstants.spacingMd)
borderRadius: BorderRadius.circular(AppConstants.radiusMd)
```

---

## Tema claro/escuro

- Preferência persistida em `themeModeProvider` (`SharedPreferences`).
- Toggle em **Perfil → Preferências → Aparência**.
- Padrão: **dark** (identidade Arah).

---

## Resumo

| Arquivo | Responsabilidade |
|---------|------------------|
| `lib/core/theme/app_design_tokens.dart` | Paleta, glass, watermark, elevação, pins |
| `lib/core/config/constants.dart` | Espaçamento, radius, animação, storage keys |
| `lib/core/theme/app_theme.dart` | ThemeData Material 3 |
| `lib/core/widgets/arah_*.dart` | Componentes visuais reutilizáveis |

Mantendo tokens centralizados e telas usando `Arah*`, o handoff do design system permanece completo e previsível.
