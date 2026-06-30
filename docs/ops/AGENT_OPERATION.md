# Operação por agentes

**Versão**: 1.1  
**Data**: 2026-06-30  
**Handoff HTML**: [Operacao por Agentes - Arah.dc.html](../handoff/Operacao%20por%20Agentes%20-%20Arah.dc.html)

---

## Objetivo

Transformar o repo `sraphaz/arah` em **operável por agentes**: humano define intenção e aprova merge; agentes executam, testam e documentam via PR.

Esta infraestrutura **precede** a FASE52 (CI/CD) e os épicos de sustentação (F52–61).

---

## Sequência de PRs (handoff G10) — concluída

| PR | Conteúdo | Status |
|----|----------|--------|
| **1** | Esqueleto agentes & skills | ✅ |
| **2** | Orquestrador & `agents.yml` + CLI | ✅ |
| **3** | Docs Steward & taxonomia | ✅ |
| **4** | Skills núcleo executáveis | ✅ |
| **5** | Agentes de código calibrados | ✅ |
| **6** | QA, Security & Release gates | ✅ |
| **7** | Piloto assistido (billing) | ✅ |

---

## Artefatos por PR

### PR 1 — Esqueleto

| Artefato | Caminho |
|----------|---------|
| Manual central | [AGENTS.md](../../AGENTS.md) |
| Manifests | [.agents/](../../.agents/) (18 agentes) |
| Skills | [.skills/](../../.skills/) (11 skills) |
| CODEOWNERS | [CODEOWNERS](../../CODEOWNERS) |
| Issue template | [agent-task.yml](../../.github/ISSUE_TEMPLATE/agent-task.yml) |
| Validação CI | [agents-validate.yml](../../.github/workflows/agents-validate.yml) |

### PR 2 — Orquestrador

| Artefato | Caminho |
|----------|---------|
| CLI | [arah-agents.ps1](../../scripts/agents/arah-agents.ps1) |
| Workflow | [agents.yml](../../.github/workflows/agents.yml) |

### PR 3 — Docs Steward

| Artefato | Caminho |
|----------|---------|
| Taxonomia | [DOC_TAXONOMY.md](../_meta/DOC_TAXONOMY.md) |
| Arquivo legado | [docs/_archive/](../_archive/) |
| Índice gerado | [INDEX.generated.md](../INDEX.generated.md) |
| Workflow índice | [agents-docs.yml](../../.github/workflows/agents-docs.yml) |
| Check sync | [sync-docs-check.ps1](../../scripts/agents/sync-docs-check.ps1) |

### PR 4 — Skills executáveis

| Skill | Script |
|-------|--------|
| run-tests, gen-l10n, sync-docs, open-pr, dep-audit, doc-taxonomy | [invoke-skill.ps1](../../scripts/agents/invoke-skill.ps1) |
| open-pr | [open-pr.ps1](../../scripts/agents/open-pr.ps1) |

### PR 5 — Agentes de código

| Script | Uso |
|--------|-----|
| [register-bff-journey-check.ps1](../../scripts/agents/register-bff-journey-check.ps1) | Valida BffJourneyRegistry + testes |
| [gen-l10n.ps1](../../scripts/agents/gen-l10n.ps1) | flutter gen-l10n + cópia lib/l10n |
| Domain agents | [.agents/domain/](../../.agents/domain/) (carteira-arata, mercado-economia) |

### PR 6 — Gates

| Artefato | Caminho |
|----------|---------|
| Runner | [run-gates.ps1](../../scripts/agents/run-gates.ps1) |
| Workflow | [agents-gates.yml](../../.github/workflows/agents-gates.yml) |
| Templates | [qa-checklist.md](../../.agents/templates/qa-checklist.md), [security-report.md](../../.agents/templates/security-report.md) |

### PR 7 — Piloto billing

| Artefato | Caminho |
|----------|---------|
| Runbook | [AGENT_PILOT_BILLING.md](AGENT_PILOT_BILLING.md) |
| Issue template | [agent-pilot-billing.yml](../../.github/ISSUE_TEMPLATE/agent-pilot-billing.yml) |

---

## Uso rápido

```powershell
./scripts/agents/arah-agents.ps1 orchestrate -Labels area/backend
./scripts/agents/arah-agents.ps1 route-pr -ChangedFiles backend/Arah.Api/Program.cs
./scripts/agents/arah-agents.ps1 validate
./scripts/agents/arah-agents.ps1 gates
./scripts/agents/arah-agents.ps1 ensure-labels   # gh label create
./scripts/agents/invoke-skill.ps1 -Skill sync-docs
```

Labels GitHub (uma vez): `./scripts/agents/arah-agents.ps1 ensure-labels`

---

## Guardrails

- Merge em `main` sempre humano
- CI verde obrigatório (`agents-gates.yml` + `ci.yml`)
- Escopo por manifest (`max_diff_files`, paths)
- Auditoria: plano + skills no corpo do PR

---

## Próximo passo pós-infra agentes

**Onda S / FASE52+** — CI/CD, Arah Core, IaC ([REALINHAMENTO_SUSTENTACAO_OPERACIONAL.md](../backlog-api/REALINHAMENTO_SUSTENTACAO_OPERACIONAL.md))

---

## Referências

- [REALINHAMENTO_SUSTENTACAO_OPERACIONAL.md](../backlog-api/REALINHAMENTO_SUSTENTACAO_OPERACIONAL.md)
- [.cursorrules](../../.cursorrules)
