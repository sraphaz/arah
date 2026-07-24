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
| Onda 2 — God classes (PostgresMappers, ArahDbContext, Program.cs, InMemoryDataStore) | ✅ Implementado |
| Onda 3 — SOLID controllers/services (extract method) | ✅ Implementado |
| Onda 4 — tokens web unificados | ✅ Implementado |
| Onda 4 — Result&lt;T&gt; | ✅ Resolvido por convenção (ADR-022) |

---

## O que foi corrigido neste PR

### Dependency Rule (ALTA)
- 11 projetos `Arah.Modules.*.Infrastructure` criados; EF Core/Npgsql isolados na borda (Api).
- `Arah.Application` não referencia mais EF Core transitivamente.

### DRY / limpeza (ALTA/MÉDIA)
- Removidos `backend/Arah.Tests/` (órfão), `Araponga.sln` (morto), testes Marketplace duplicados, entidade/interface órfãs de `StoreRatingResponse`.
- Wiki: páginas passam a usar `lib/document.ts` e `lib/markdown.ts`.

---

## O que foi implementado nas Ondas 2–4 (mesmo PR)

- **Onda 2 (God classes)**: `PostgresMappers.cs` → 19 arquivos `partial` por agregado; `ArahDbContext.cs` → 21 arquivos `partial` de configuração + `OnModelCreating` enxuto; `InMemoryDataStore` → seed extraído para `InMemorySeeder`; `Program.cs` → composition root enxuto + extensões (`AddArahObservability/Security/RateLimiting/Swagger`, `UseArahExceptionHandler`, `UseArahPipeline`); `ServiceCollectionExtensions` fatiado por concern.
- **Onda 3 (SOLID)**: métodos gigantes fatiados via *extract method* (`EventsService.CreateEventAsync` 257→69, `SellerPayoutService.ProcessPaidCheckoutAsync` 179→75, `ChatService.SendTextMessageAsync` 174→45); controllers afinados (`MapController.GetPinsPaged` 226→106; `FeedController` com `EnforceGeoConvergenceAsync`/`BuildFeedItemResponse`).
- **Onda 4 (tokens)**: literais de cor da wiki mapeados para tokens; paletas Tailwind documentadas como *sync-with-tokens*.
- **Onda 4 (`Result<T>`)**: resolvido por **convenção** ([ADR-022](../architecture/adrs/ADR-022-convencao-sinalizacao-de-erros.md)) em vez de reescrever ~35 métodos de consulta (evita churn de alto risco).

## Pendências remanescentes (menores, próximos PRs)

Todas as pendências da revisão original foram endereçadas neste PR (exceto itens opcionais de escopo futuro).

| Item | Status |
|---|---|
| Fatiar `wiki/app/globals.css` | ✅ 16 arquivos em `frontend/wiki/styles/` + hub 16 linhas |
| Testes no `frontend/portal` | ✅ Jest (brand + tailwind config) |
| Flutter `onboarding_screen.dart` | ✅ 799→148 linhas; 7 widgets extraídos |
| `MapController` pin assembly | ✅ `MapPinsService` na Application |

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
