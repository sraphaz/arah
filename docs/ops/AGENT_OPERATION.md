# Operação por agentes

**Versão**: 1.3  
**Data**: 2026-07-01  
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
| **8** | PR Steward + next-phase | ✅ |
| **9** | Visibilidade + checklists de conduta + FASE52 smoke | ✅ |

---

## Artefatos por PR

### PR 1 — Esqueleto

| Artefato | Caminho |
|----------|---------|
| Manual central | [AGENTS.md](../../AGENTS.md) |
| Manifests | [.agents/](../../.agents/) (19 agentes) |
| Skills | [.skills/](../../.skills/) (14 skills) |
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

### PR 8 — PR Steward (review, bots, next-phase)

| Artefato | Caminho |
|----------|---------|
| Agente | [pr-steward.agent.yaml](../../.agents/pr-steward.agent.yaml) |
| Workflow | [agents-pr-steward.yml](../../.github/workflows/agents-pr-steward.yml) |
| Fila de fases | [PHASE_QUEUE.yaml](../_meta/PHASE_QUEUE.yaml) |
| Checklist bots | [bot-review-checklist.md](../../.agents/templates/bot-review-checklist.md) |
| Scripts | [address-bot-review.ps1](../../scripts/agents/address-bot-review.ps1), [pr-ready.ps1](../../scripts/agents/pr-ready.ps1), [next-phase.ps1](../../scripts/agents/next-phase.ps1) |
| Skills | [address-bot-review.skill.yaml](../../.skills/address-bot-review.skill.yaml), [next-phase.skill.yaml](../../.skills/next-phase.skill.yaml) |

**Comportamento:**
- Em cada PR: audita bots, posta checklist, label `ready-for-merge` só se CI verde **e** zero apontamentos de bot.
- Após push em `main`: `next-phase.ps1` abre issue `[Agent]` da próxima fase desbloqueada na fila.
- Merge continua **humano** (`guardrails.no_merge: true` no manifest).

### PR 9 — Visibilidade e conduta dos agentes

| Artefato | Caminho |
|----------|---------|
| Checklists | [.agents/checklists/](../../.agents/checklists/) (9 agentes + `_shared`) |
| Ativação CI | [run-agent-activation.ps1](../../scripts/agents/run-agent-activation.ps1) |
| Post comentário | [post-agent-activity.ps1](../../scripts/agents/post-agent-activity.ps1) |
| Verificação auto | [agent-conduct-check.ps1](../../scripts/agents/agent-conduct-check.ps1) |
| Skill | [agent-activate.skill.yaml](../../.skills/agent-activate.skill.yaml) |
| Workflow | [agents.yml](../../.github/workflows/agents.yml) (artifact `agent-activity-*.json`) |
| FASE52 smoke | [cd.yml](../../.github/workflows/cd.yml) job `smoke-test-api` |

**Onde ver a ação do agente:**
1. Comentário na issue/PR (`<!-- arah-agent-activity:{id} -->`)
2. Artifact no run do workflow **Agents Orchestrate**
3. Step summary no CD (Release Agent)
4. CLI: `arah-agents activate -Agent … -Issue N`

---

## Uso rápido

```powershell
./scripts/agents/arah-agents.ps1 orchestrate -Labels area/backend
./scripts/agents/arah-agents.ps1 route-pr -ChangedFiles backend/Arah.Api/Program.cs
./scripts/agents/arah-agents.ps1 validate
./scripts/agents/arah-agents.ps1 gates
./scripts/agents/arah-agents.ps1 ensure-labels   # gh label create
./scripts/agents/arah-agents.ps1 bot-review -PrNumber <N>
./scripts/agents/arah-agents.ps1 pr-ready -PrNumber <N>
./scripts/agents/arah-agents.ps1 next-phase -DryRun
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

