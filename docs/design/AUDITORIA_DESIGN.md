# Auditoria de Design — Estado high-premium das interfaces

**Data**: 2026-07-02
**Autor**: Agente `design-ux` (consultivo autônomo)
**Escopo**: `frontend/wiki`, `frontend/portal`, `frontend/devportal`, `frontend/arah.app` (Flutter) e tokens compartilhados
**Fontes de verdade**: `docs/DESIGN_SYSTEM_IDENTIDADE_VISUAL.md`, `docs/CURSOR_DESIGN_RULES.md`, `frontend/shared/styles/design-tokens.css`, `frontend/arah.app/lib/core/theme/app_design_tokens.dart`

> Este documento é o parecer vivo do especialista de design. A cada revisão, novos gaps são
> registrados aqui e retroalimentados no backlog (`docs/_meta/PHASE_QUEUE.yaml` → `design-quality`).
> Lições recorrentes viram entrada em `docs/LICOES_APRENDIDAS.md`.

---

## Veredito

O sistema tem **fundação forte**: tokens centralizados na web (`design-tokens.css`) e no app
(`AppDesignTokens`/`AppColors`), Material 3 no Flutter, glass morphism e escala 8px documentados.
O que impede o "high-premium pleno" são **derivas pontuais** (cores fora de token), o **devportal
legado** desalinhado dos tokens compartilhados e **lacunas de acessibilidade** na landing.

> **Eixo app × UI kit (2026-07-15):** a nota **A-** do Flutter mede disciplina de tokenização,
> não fidelidade ao protótipo premium. Análise profunda tela a tela, IA e gaps `APP-DS-*`:
> [ANALISE_DESIGN_VS_APP_FLUTTER.md](./ANALISE_DESIGN_VS_APP_FLUTTER.md).

Classificação atual por superfície:

| Superfície | Tokens | Mobile-first | A11y | Nota |
|-----------|--------|--------------|------|------|
| `frontend/wiki` | ✅ forte | ✅ | 🟡 (contraste syntax) | **A-** |
| `frontend/arah.app` (Flutter) | ✅ forte | ✅ | 🟡 (auditar toque) | **A-** |
| `frontend/portal` | ✅ bom | 🟡 (espaçamento) | 🔴 (sem aria/alt na page) | **B** |
| `frontend/devportal` | 🔴 legado | ? | ? | **C** |

---

## Gaps mapeados

| ID | Superfície | Gap | Evidência | Severidade | Status |
|----|-----------|-----|-----------|------------|--------|
| DSG-01 | wiki | Cores arbitrárias `bg-[#4dd4a8]`/`[#7dd3ff]` no Mermaid fullscreen (5x) | `frontend/wiki/app/mermaid/fullscreen/page.tsx` | 🔴 viola LIC-001 | ✅ corrigido |
| DSG-02 | Flutter | `Color(0xFF228B22)` e `Colors.orange` fora do tema no onboarding (5x) | `.../onboarding/.../propose_territory_sheet.dart`, `.../onboarding_screen.dart` | 🔴 viola LIC-001 | ✅ corrigido |
| DSG-03 | devportal | ~47 hex hardcoded; não consome `design-tokens.css` | `frontend/devportal/assets/css/*.css` | 🟠 maior débito de consistência | ✅ corrigido (aliases → `design-tokens.css`; `semantic-colors` + `color-depth-system` importados) |
| DSG-04 | portal | Espaçamentos fora da escala 8px (`36px`, `56px`) e px literais em tokens | `frontend/portal/app/globals.css` | 🟡 | ⏳ backlog |
| DSG-05 | portal | Landing sem `aria-label`/`alt`/`role` na página raiz | `frontend/portal/app/page.tsx` (0 ocorrências) | 🟠 a11y | ✅ corrigido (skip link, `main` landmark, nav/header aria, alt na hero) |
| DSG-06 | cross | Deriva de glass tokens entre plataformas (bg 0.98 vs 0.88; raio 24 vs 32) | wiki/portal `globals.css` + Flutter `app_design_tokens.dart` | 🟡 marca | ⏳ backlog |
| DSG-07 | wiki | `--syntax-*` sem verificação WCAG documentada | `frontend/wiki/app/globals.css` | 🟡 a11y | ⏳ backlog |
| DSG-08 | cross | Ausência de gate automático que detecte cor hardcoded no CI | — | 🟠 prevenção | ✅ corrigido |

---

## Corrigido nesta passada

- **DSG-03**: devportal passa a derivar paleta light/dark de `design-tokens.css` (`--color-forest-*`, `--color-dark-*`); `semantic-colors.css` e `color-depth-system.css` sem hex literais, importados por `devportal.css`.
- **DSG-05**: portal landing com skip link, `main` com `id`/`aria-label`, `header`/`nav` semânticos com `aria-label`, links externos descritos, alt descritivo na hero.

## Corrigido em passada anterior

- **DSG-01**: 5 usos de `bg-[#...]` → classes configuradas `bg-accent`/`bg-accent-hover`/`bg-link`/`bg-link-hover` (Tailwind da wiki). Zero cores arbitrárias no arquivo.
- **DSG-01b** (descoberto pelo gate): 22 hex hardcoded nos `themeVariables` do Mermaid (`MermaidDiagram.tsx` + fullscreen) → adapter `frontend/wiki/lib/mermaid-theme.ts` que deriva o tema dos design tokens em runtime (`getComputedStyle` + fallback SSR espelhando tokens). Fonte única para os 2 consumidores.
- **DSG-02**: onboarding Flutter passa a usar `AppDesignTokens.territoryBoundary` e `AppDesignTokens.locationPin` (tokens já existentes) em vez de `Color(0xFF228B22)`/`Colors.orange`.
- **DSG-08**: gate `scripts/agents/design-gate-check.ps1` integrado ao `run-gates.ps1` (QA, roda em todo PR via `agents-gates.yml`). Falha o PR se arquivo de frontend alterado tiver Tailwind arbitrária `[#...]`, hex inline, cor literal em CSS fora de `--token:` ou `Color(0x...)`/`Colors.*` fora do tema Flutter. Testado: passa nos arquivos corrigidos, detecta 9 violações no CSS legado do devportal.

---

## Recomendação de ordem (backlog `design-quality`)

### Web / tokens (DSG-*)

1. **DSG-06 + DSG-04** (unificar glass/espaçamento) — refino "premium" de marca.
2. **DSG-07** (contraste syntax WCAG) — acessibilidade fina.
3. ~~DSG-03 / DSG-05~~ — já corrigidos.

### App Flutter × UI kit (APP-DS-*) — ver análise completa

1. ~~**APP-DS-01..06**~~ — ADR-021, tokens floresta, Sora/Geist, shell Serviços+TopBar, hub (2026-07-21).
2. **APP-DS-07** — RoleBadge, JourneyShell e componentes premium restantes.
3. **APP-DS-08 → 12** — redesign Explorar/Perfil/Mercado + jornadas + estados.
4. **APP-DS-13** — gate visual de regressão de IA.

---

## Como o agente age (autônomo)

- **Local**: ao tocar em `frontend/**`, `docs/design/**` ou `docs/**/design*`, o hook `stop` aciona `design-ux` via coreografia (`.agents/choreography.yaml`) e grava o parecer em `.cursor/domain-review.md`.
- **CI**: `agents.yml` (Agents Orchestrate) publica o parecer como comentário no PR; o gate anti-cor-hardcoded (`design-gate-check.ps1`) roda em `agents-gates.yml` via `run-gates.ps1`.
- **Checklist** (bloco `validate` do manifesto): cores via token, mobile-first, escala 8px, WCAG AA, semântica/aria, consistência cross-plataforma, estados (loading/vazio/erro/foco).
