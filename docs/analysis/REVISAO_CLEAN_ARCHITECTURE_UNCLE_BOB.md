# Revisão Clean Architecture / Clean Code (Uncle Bob)

**Data**: 2026-07-03  
**Critérios**: skill `uncle-bob-craft` (Clean Code, Clean Architecture, The Clean Coder, Clean Agile)  
**PR de implementação**: `refactor/clean-architecture-uncle-bob`

---

## Status pós-PR (Ondas 0–1 implementadas)

| Item | Status |
|---|---|
| Onda 0 — limpeza (órfãos, DRY wiki, duplicatas) | ✅ Implementado |
| Onda 1 — Dependency Rule (Infrastructure separada) | ✅ Implementado |
| Onda 2 — God classes (PostgresMappers, Program.cs…) | ⏳ Pendente |
| Onda 3 — SOLID controllers/services | ⏳ Pendente |
| Onda 4 — Result&lt;T&gt; padronizado, tokens web | ⏳ Pendente |

---

## O que foi corrigido neste PR

### Dependency Rule (ALTA)
- 11 projetos `Arah.Modules.*.Infrastructure` criados; EF Core/Npgsql isolados na borda (Api).
- `Arah.Application` não referencia mais EF Core transitivamente.

### DRY / limpeza (ALTA/MÉDIA)
- Removidos `backend/Arah.Tests/` (órfão), `Araponga.sln` (morto), testes Marketplace duplicados, entidade/interface órfãs de `StoreRatingResponse`.
- Wiki: páginas passam a usar `lib/document.ts` e `lib/markdown.ts`.

---

## Pendências (próximos PRs)

Ver plano completo nas seções 3–6 abaixo. Prioridade:

1. **Onda 2**: fatiar `PostgresMappers.cs`, `ArahDbContext.cs`, `Program.cs`, `InMemoryDataStore.cs`.
2. **Onda 3**: extrair casos de uso de `MapController`/`FeedController`; fatiar `EventsService`, `ChatService`, `SellerPayoutService`.
3. **Onda 4**: padronizar `Result<T>`; unificar tokens Tailwind com `design-tokens.css`; testes no `portal`.

---

## Achados originais (referência)

### Alta severidade (parcialmente resolvida)
- ~~Application depende transitivamente de EF Core~~ → **resolvido** (Onda 1)
- ~~Entidades/testes duplicados (Marketplace)~~ → **parcialmente resolvido** (Onda 0)
- God classes: `PostgresMappers` (~2.2k linhas), `ArahDbContext` (81 DbSets), `Program.cs` (~800 linhas)
- Controllers Map/Feed com regra de negócio inline
- Services com métodos de 160–257 linhas

### Média severidade
- `Result<T>` inconsistente vs `null` vs exceptions
- Paleta duplicada (portal/wiki tailwind vs design-tokens)
- `wiki/app/globals.css` ~1.4k linhas
- Flutter: `onboarding_screen.dart` ~760 linhas

### Positivos
- `Arah.Domain` limpo (zero frameworks)
- App Flutter exemplar (camadas data/domain/presentation, `BffClient`, `AppDesignTokens`)

---

## Plano de ondas (referência)

| Onda | Escopo | Esforço nominal |
|---|---|---|
| 0 | Limpeza segura | 0,5 dia |
| 1 | Dependency Rule (módulos) | 3–5 dias |
| 2 | God classes | 3–4 dias |
| 3 | SOLID services/controllers | 4–6 dias |
| 4 | Convenções e polimento | 2 dias |
