# Avaliacao por boas praticas - Ara App

Avaliacao do app Flutter em relacao a boas praticas de estrutura, estado, rede, tema e manutencao.

## O que esta alinhado

- **Estrutura em features:** Auth, feed, profile, onboarding, territories, explore, home com data/presentation; core compartilhado.
- **Estado (Riverpod):** Providers por dominio; AsyncValue; StateNotifier para feed paginado.
- **Rede:** Uma porta (BFF); Dio com interceptors (401, 429); ApiException centralizada.
- **Rotas:** go_router com redirect e guards (auth, territorio).
- **i18n:** l10n pt/en; AppLocalizations nas telas.
- **Design tokens:** AppConstants + AppDesignTokens; tema usa tokens.
- **Acessibilidade:** Touch targets 44pt; SnackBar flutuante.
- **Separacao de responsabilidades:** Repos chamam BFF; telas so leem providers.

## Pontos de atencao

- **Cores:** Centralizadas em AppDesignTokens; alterar em um unico arquivo.
- **Tipografia:** Preferir textTheme.titleMedium/bodyLarge; evitar fontSize solto.
- **Valores magicos:** Preferir AppConstants; evitar numeros soltos (24, 48).
- **Testes:** Considerar widget tests para fluxos criticos.
- **Logs:** Em producao considerar logger ou crashlytics.

## Checklist para PRs

- Novas telas usam AppConstants para espacamento e radius.
- Textos via AppLocalizations.of(context).
- Cores via colorScheme ou context.appColors (nao Color(0xFF...) nas telas).
- Chamadas API por repositorios com BffClient.
- Estados async com AsyncValue.
- Botoes com altura minima 44pt.

## Referencias

- Effective Dart, Flutter style guide.
- DESIGN_SYSTEM.md (manutencao do design).
