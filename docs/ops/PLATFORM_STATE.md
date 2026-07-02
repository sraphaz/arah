# Estado da Plataforma e Operabilidade

**Versão**: 1.0  
**Atualizado**: 2026-07-01  
**Fases totais**: ver [PROJECT_PHASES_CONFIG.md](../PROJECT_PHASES_CONFIG.md) (61 fases: 1–51 comunitárias + 52–61 sustentação)

Este documento é a **fonte de verdade** para o README e para agentes. Níveis de operabilidade do Arah hoje.

---

## Nível 1 — Produto e território

| Aspecto | Estado | Spec / doc |
|---------|--------|------------|
| Território geográfico neutro | ✅ Produção lógica | [product/01_PRODUCT_VISION.md](../product/01_PRODUCT_VISION.md) |
| Onboarding + mapa + membership | ✅ App + BFF | [STABLE_RELEASE_APP_ONBOARDING.md](../STABLE_RELEASE_APP_ONBOARDING.md) |
| Marketplace, eventos, alertas | ✅ Backend modular | [FEATURE_MATRIX_API_BFF_APP.md](../FEATURE_MATRIX_API_BFF_APP.md) |
| Multi-instância / federação | ⏳ FASE58–59 | [FASE59.md](../backlog-api/FASE59.md) |

---

## Nível 2 — Arquitetura e domínio

| Aspecto | Estado | Spec / doc |
|---------|--------|------------|
| Clean Architecture (.NET) | ✅ | [architecture/10_ARCHITECTURE_DECISIONS.md](../architecture/10_ARCHITECTURE_DECISIONS.md) |
| BFF jornadas (Flutter-only API) | ✅ | [backend/60_API_LÓGICA_NEGÓCIO.md](../backend/60_API_LÓGICA_NEGÓCIO.md) |
| Arah Core (control plane) | 🟡 FASE53 | [specs/phases/FASE53-arah-core.spec.yaml](../specs/phases/FASE53-arah-core.spec.yaml) |
| C4 / sustentação | 📋 Handoff | [handoff/arquitetura-c4/](../handoff/arquitetura-c4/) |

---

## Nível 3 — Desenvolvimento e qualidade

| Aspecto | Estado | Harness |
|---------|--------|---------|
| CI build + testes | ✅ | `ci.yml` |
| Cobertura >90% camadas negócio | ✅ | `dotnet test` + Codecov |
| Agents Gates (QA/Security) | ✅ | `agents-gates.yml` |
| Spec-Driven Design | ✅ | `spec-harness.yml` |
| Flutter / Wiki / DevPortal | ✅ | `ci.yml` jobs |

---

## Nível 4 — CI/CD e deploy (FASE52)

| Aspecto | Estado | Spec / doc |
|---------|--------|------------|
| PR → CI bloqueante | ✅ | [specs/phases/FASE52-cicd.spec.yaml](../specs/phases/FASE52-cicd.spec.yaml) |
| Imagens GHCR (api, bff) | ✅ | [CI_CD_PIPELINE.md](./CI_CD_PIPELINE.md) |
| Deploy staging automático | ✅ | `deploy-staging.yml` |
| Gate produção manual | ✅ | `deploy-production.yml` |
| Health `/health` API + BFF | ✅ | smoke + staging |
| Observabilidade baseline prod | 🟡 | FASE52 em progresso |
| GitHub Project (board F52–61) | 🟡 | bootstrap via `github-sync.yml` |

---

## Nível 5 — Operação por agentes

| Aspecto | Estado | Doc |
|---------|--------|-----|
| 29 agentes + 24 skills (contagem via `arah-agents.ps1 validate`) | ✅ | [AGENTS.md](../../AGENTS.md) |
| Contexto Cursor em camadas (PR 14: `.cursorrules` ~4 KB + rules escopadas) | ✅ | [AGENT_OPERATION.md](./AGENT_OPERATION.md#pr-14--contexto-em-camadas--comunicação-passiva-otimização-de-consumo-de-api-2026-07-02) |
| Comunicação passiva entre agentes (hook sem followup; CI autoritativo) | ✅ | `.cursor/hooks/domain-review.ps1` |
| Orquestrador + checklists conduta | ✅ | [AGENT_OPERATION.md](./AGENT_OPERATION.md) |
| PR Steward (bots → merge humano) | ✅ | `agents-pr-steward.yml` |
| next-phase (fila PHASE_QUEUE) | ✅ | [PHASE_QUEUE.yaml](../_meta/PHASE_QUEUE.yaml) |
| GitHub Project v2 (board ondas) | 🟡 | `github-project bootstrap` · [GITHUB_PROJECT_MANAGEMENT.md](./GITHUB_PROJECT_MANAGEMENT.md) |
| Harness agentes | ✅ | [specs/harness/agent-operation.spec.yaml](../specs/harness/agent-operation.spec.yaml) |

---

## Nível 6 — Sustentação operacional (52–61)

| Fase | Nome | Status | Onda |
|------|------|--------|------|
| 52 | CI/CD | 🟡 Em progresso | S0 |
| 53 | Arah Core | 🟡 Spec + início | S0 |
| 54 | IaC 1ª instância | ⏳ | S0 |
| 55–61 | Monetização, cockpit, federação… | ⏳ | S1–S4 |

Detalhe: [STATUS_FASES.md](../STATUS_FASES.md) · [REALINHAMENTO_SUSTENTACAO_OPERACIONAL.md](../backlog-api/REALINHAMENTO_SUSTENTACAO_OPERACIONAL.md)

---

## Nível 7 — Governança documental

| Aspecto | Estado |
|---------|--------|
| Docs taxonomia + `_archive/` | ✅ Wave 1–2 |
| Specs machine-readable | ✅ `docs/specs/` |
| Índice gerado | ✅ `INDEX.generated.md` |
| Wiki / DevPortal | ✅ GitHub Pages |

---

## Quick commands (operabilidade)

```powershell
# Stack local
.\scripts\run-local-stack.ps1

# Agentes
.\scripts\agents\arah-agents.ps1 validate
.\scripts\agents\arah-agents.ps1 harness

# Specs
.\scripts\harness\validate-specs.ps1

# Fases
./scripts/agents/next-phase.ps1 -DryRun
./scripts/agents/arah-agents.ps1 github-project -Skill bootstrap
```

---

## Prioridade atual

**Onda S0**: FASE52 ✅ → FASE53 ✅ → **FASE54** (config ops: [PILOT_STAGING_CONFIG_TODO.md](./PILOT_STAGING_CONFIG_TODO.md)) → **FASE55** (quote/gate v0 em código).

**Provisionamento piloto**: `./scripts/provision/provision-pilot-instance.ps1` · [infrastructure/pilot/README.md](../infrastructure/pilot/README.md)
