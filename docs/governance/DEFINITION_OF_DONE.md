# Definition of Done (DoD) — Arah

**Versão**: 1.0
**Data**: 2026-07-02
**Rigor**: Total (aplicado a todas as fases S0+ e retroativamente a FASE54/FASE55)

Este documento é a **fonte de verdade** do que significa "pronto" no Arah. Nenhum item,
critério de aceite (AC) ou fase é considerado concluído sem satisfazer **todos** os gates
abaixo. O objetivo é garantir que "está tudo funcionando" com **evidência**, e que os
**agentes de domínio** revisaram e aprofundaram a funcionalidade — de forma **autônoma**.

---

## 1. Gates obrigatórios (todo item)

| # | Gate | Como comprovar (evidência) |
|---|------|----------------------------|
| 1 | Build Release sem erros | Log de `dotnet build backend/Arah.sln -c Release` |
| 2 | Testes verdes | Log de `dotnet test` (inclui `Arah.Tests.Modules.*` das áreas tocadas) |
| 3 | AC ↔ teste | Cada `acceptance.id` da spec tem teste que o cobre (ver §2) |
| 4 | Teste de integração HTTP | Todo endpoint novo/alterado tem teste via `WebApplicationFactory` |
| 5 | Parecer de domínio endereçado | `.cursor/domain-review.md` (local) e/ou comentário `arah-domain-consult` no PR |
| 6 | Doc-sync | CHANGELOG, STATUS_FASES, `docs/backlog-api/FASE*`, spec no **mesmo PR** |
| 7 | Sem secrets / sem cor hardcoded / nomenclatura | Gate `code-review` + revisão QA |
| 8 | Harness SDD verde (fases S0+) | Artifact `harness-report.json` |

> Um item que não comprova os 8 gates **não** recebe `ready-for-merge`.

---

## 2. Rastreabilidade AC ↔ teste (obrigatória)

Cada critério de aceite da spec (`docs/specs/**/*.spec.yaml`) deve ser rastreável a um teste:

- **Convenção**: o teste referencia o AC no nome ou em comentário, ex.:
  `Quote_ReturnsSplitByActiveRule_AC_55_2()` ou `// AC-55-3`.
- **Cobertura por camada**:
  - Regra de negócio → teste unit (Domain/Application).
  - Endpoint (contrato HTTP) → teste de integração (`Arah.Tests*`/`ApiFactory`).
- **Sem teste, sem AC fechado.** O harness roda `dotnet test --filter` da spec; se o filtro
  não pega nenhum teste do AC, o AC está aberto.

---

## 3. Evidência (obrigatória)

"Evidência" = artefato verificável anexado ao PR ou à issue da fase:

- Saída de `dotnet test` / `harness-report.json` (spec-harness).
- Comentário(s) de parecer de domínio (`arah-domain-consult`) publicados no PR.
- Para fluxos com UI: screenshot ou gravação (PR template §📸).
- Para E2E de infraestrutura (piloto): saída de `scripts/provision/verify-pilot-instance.ps1`.

---

## 4. Agentes de domínio — autônomos

Os agentes de domínio agem **a cada interação, sem acionamento manual**:

- **Local (Cursor)**: hook `stop` (`.cursor/hooks.json`) executa
  `scripts/agents/domain-autoreview.ps1`, resolve a coreografia
  (`.agents/choreography.yaml`) e gera pareceres em `.cursor/domain-review.md`.
- **CI (PR)**: `agents.yml` publica os pareceres como comentários
  (`post-domain-consult.ps1 -PostComment`) em todo `opened`/`synchronize`.

Cada item de "Validar no PR" do parecer deve ser resolvido antes do merge (gate #5).

---

## 5. Níveis de teste

| Nível | Onde | Escopo |
|-------|------|--------|
| Unit | `Arah.Tests/{Domain,Application}` | Regra de negócio isolada |
| Integração/E2E in-process | `Arah.Tests*` via `WebApplicationFactory` | Contrato HTTP, store InMemory |
| E2E de infraestrutura | `scripts/provision/verify-pilot-instance.ps1` | Stack real (Docker+Postgres+Core) |
| App (Flutter) | `frontend/arah.app/integration_test/` | Jornada de app (quando houver UI) |

Meta de cobertura: **>90%** (ver `docs/governance/22_COHESION_AND_TESTS.md`). Débito atual
documentado; novos itens não podem reduzir a cobertura da área que tocam.

---

## Checklist de fechamento (colar no PR)

```
- [ ] Build Release + testes verdes (com Arah.Tests.Modules.* afetados)
- [ ] Cada AC da spec tem teste (unit + integração HTTP p/ endpoint)
- [ ] Evidência anexada (test output / harness-report.json / screenshot)
- [ ] Pareceres de domínio publicados e itens de "Validar" resolvidos
- [ ] Doc-sync no mesmo PR (CHANGELOG, STATUS_FASES, FASE*, spec)
- [ ] Sem secrets; sem cor hardcoded; nomenclatura território/items
```

---

**Referências**: `.cursorrules` (Validação Antes de PR), `docs/governance/21_CODE_REVIEW.md`,
`docs/governance/22_COHESION_AND_TESTS.md`, `AGENTS.md`, `docs/_meta/SDD_AND_HARNESS.md`.
