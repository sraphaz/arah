# Operação por agentes

**Versão**: 1.5  
**Data**: 2026-07-02  
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
./scripts/agents/arah-agents.ps1 export-graph     # gera Agent Graph (JSON)
./scripts/agents/arah-agents.ps1 validate-graph   # audita consistência do grafo
./scripts/agents/invoke-skill.ps1 -Skill sync-docs
```

**Agent Graph**: formaliza o grafo operacional (agentes ↔ skills ↔ rules ↔ paths
↔ domínios ↔ specs ↔ harnesses ↔ guardrails ↔ workflows ↔ gates) como artefato
auditável. Fonte inicial: `.agents/choreography.yaml`. Ver
[AGENT_GRAPH.md](AGENT_GRAPH.md).

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

### PR 10 — Agentes SDD + Arah Core (2026-07-01)

| Artefato | Caminho |
|----------|---------|
| Spec Steward checklist | [.agents/checklists/spec-steward.checklist.md](../../.agents/checklists/spec-steward.checklist.md) |
| Domain control-plane | [.agents/domain/control-plane.agent.yaml](../../.agents/domain/control-plane.agent.yaml) |
| Specialist Core | [.agents/specialists/core-control-plane.agent.yaml](../../.agents/specialists/core-control-plane.agent.yaml) |
| Skills | [spec-author](../../.skills/spec-author.skill.yaml), [harness-run](../../.skills/harness-run.skill.yaml) |
| Gate SDD | [spec-gate-check.ps1](../../scripts/agents/spec-gate-check.ps1) |
| Co-roteamento | [orchestrator.agent.yaml](../../.agents/orchestrator.agent.yaml) `co_route` |
| Labels | `area/spec`, `area/sdd` |

**Comportamento:**
- PR com `docs/specs/` ou `Arah.Core` aciona **spec-steward** como co-agente.
- `run-gates` inclui `spec-gate-check` (Spec-Id + validate-specs).
- Backend consulta `control-plane` / `core-control-plane` para FASE53+.

### PR 11 — Coreografia + LikeC4 + domínio consultivo (2026-07-01)

| Artefato | Caminho |
|----------|---------|
| Coreografia | [.agents/choreography.yaml](../../.agents/choreography.yaml) |
| Engine | [choreograph-agents.ps1](../../scripts/agents/choreograph-agents.ps1) |
| Ativação (reescrita) | [run-agent-activation.ps1](../../scripts/agents/run-agent-activation.ps1) |
| Parecer domínio | [post-domain-consult.ps1](../../scripts/agents/post-domain-consult.ps1) |
| LikeC4 model | [docs/architecture/likec4/](../architecture/likec4/) |
| Export diagramas | [export-likec4.ps1](../../scripts/diagrams/export-likec4.ps1), [apply-arah-design-tokens.ps1](../../scripts/diagrams/apply-arah-design-tokens.ps1) |
| Skills | [likec4-export](../../.skills/likec4-export.skill.yaml), [domain-consult](../../.skills/domain-consult.skill.yaml) |
| Domain agents (paths) | [.agents/domain/](../../.agents/domain/) — `scope.paths` + `autonomy: consult_post` |

**Comportamento:**
- Todo PR: regra `pr-always` aciona **qa** + **pr-steward**; com `-ExecuteAutonomy` no CI, skills das regras matched são invocadas.
- PR com Marketplace → parecer `mercado-economia`; Core → `control-plane` + `architecture-review`.
- `orchestrate` enriquece `route.json` com bloco `choreography` (regras + domínios).
- CLI: `arah-agents choreograph`, `skill -Skill likec4-export`.

### PR 13 — Autonomia de domínio (sem acionamento manual) + DoD (2026-07-02)

| Artefato | Caminho |
|----------|---------|
| Hook local (Cursor) | [.cursor/hooks.json](../../.cursor/hooks.json), [.cursor/hooks/domain-review.ps1](../../.cursor/hooks/domain-review.ps1) |
| Autoreview | [domain-autoreview.ps1](../../scripts/agents/domain-autoreview.ps1) |
| Rule (escopada por glob) | [.cursor/rules/domain-agents-autonomy.mdc](../../.cursor/rules/domain-agents-autonomy.mdc) |
| Definition of Done | [docs/governance/DEFINITION_OF_DONE.md](../governance/DEFINITION_OF_DONE.md) |
| CI publica pareceres | [agents.yml](../../.github/workflows/agents.yml) (step "Publish domain agent pareceres") |

**Comportamento:**
- **Local**: hook `stop` roda o autoreview sobre o `git diff` a cada interação; gera pareceres em `.cursor/domain-review.md` (idempotente por conjunto de mudanças). Desde o PR 14 **não injeta `followup_message`** — comunicação passiva por arquivo.
- **CI**: em todo PR, os `domain_consults` da coreografia são publicados como comentários `arah-domain-consult` (não apenas listados).
- Cobertura de paths ampliada (Financial, Application/Services/Marketplace, Items) e matcher de glob corrigido para `**` em qualquer posição.

### PR 14 — Contexto em camadas + comunicação passiva (otimização de consumo de API) (2026-07-02)

**Motivação:** o modelo anterior injetava ~20 mil tokens fixos por requisição
(`.cursorrules` ~60 KB always-apply + `AGENTS.md` ~15 KB) e o hook `stop`
devolvia `followup_message`, gerando um turno extra de modelo (nova rodada de
API com o contexto inteiro) a cada interação que tocasse paths de domínio —
inclusive mudanças apenas em docs.

**Novo modelo em 4 camadas (contexto sob demanda):**

| Camada | Artefato | Quando entra no contexto |
|--------|----------|--------------------------|
| 0 — Núcleo | [.cursorrules](../../.cursorrules) (~4 KB) | Sempre (princípios, guardrails, ponteiros) |
| 1 — Regras escopadas | [.cursor/rules/](../../.cursor/rules/) — `backend-standards` (`backend/**`), `frontend-design` (`frontend/**`), `docs-organization` (`docs/**`, `*.md`), `domain-agents-autonomy` (`backend/**`, `frontend/**`, `docs/design/**`) | Só quando arquivos do glob são tocados |
| 2 — Skills | [.cursor/skills/](../../.cursor/skills/) `arah-run-tests`, `arah-sync-docs`, `arah-open-pr`, `arah-domain-consult` → apontam para [.skills/](../../.skills/) | Índice leve sempre; corpo lido só ao usar |
| 3 — Comunicação passiva | Hook `stop` grava `.cursor/domain-review.md` **sem** followup; CI publica pareceres no PR | Agente lê o arquivo quando a rule escopada instrui |

**Efeitos:**
- Contexto fixo por requisição: de ~75 KB para ~4 KB (redução ~95%).
- Zero turnos extras de modelo por hook; pareceres continuam gerados (local) e publicados (CI).
- Conteúdo integral das regras antigas preservado nas regras escopadas e nos docs já existentes (`docs/CURSOR_DESIGN_RULES.md`, `docs/21_CODE_REVIEW.md`, `docs/CURSOR_DOCUMENTATION_RULES.md`).

**Propriedade SDD (tabela movida de AGENTS.md):**

| Objeto | Agente principal | Co-agentes | Skills |
|--------|------------------|------------|--------|
| `docs/specs/*.yaml` | `spec-steward` | `docs-steward`, `planner` | `spec-author`, `spec-validate`, `harness-run` |
| `scripts/harness/` | `spec-steward` | `release` | `harness-run`, `spec-validate` |
| `backend/Arah.Core/` | `backend` | `spec-steward`, `solutions-architect` | `register-adr`, `run-tests`, `harness-run` |
| `docs/architecture/adrs/` | `solutions-architect` | `docs-steward` | `register-adr`, `likec4-export`, `architecture-review` |
| `.github/workflows/spec-harness.yml` | `release` | `spec-steward` | `harness-run` |
| FASE52–61 backlog | `planner` | `spec-steward` | `spec-author`, `backlog-to-issue` |

### PR 12 — Gestão via GitHub (Issues, Project, labels) (2026-07-01)

| Artefato | Caminho |
|----------|---------|
| Labels canônicas | [.github/labels.yml](../../.github/labels.yml) |
| Milestones ondas | [.github/milestones.yml](../../.github/milestones.yml) |
| Project config | [.github/project/arah-sustentacao.yml](../../.github/project/arah-sustentacao.yml) |
| Template épico | [phase-epic.yml](../../.github/ISSUE_TEMPLATE/phase-epic.yml) |
| Scripts | [sync-github-labels.ps1](../../scripts/agents/sync-github-labels.ps1), [GitHub-ProjectApi.ps1](../../scripts/agents/GitHub-ProjectApi.ps1), [github-project.ps1](../../scripts/agents/github-project.ps1), [backlog-to-issue.ps1](../../scripts/agents/backlog-to-issue.ps1) |
| Workflows | [github-sync.yml](../../.github/workflows/github-sync.yml), [project-board-sync.yml](../../.github/workflows/project-board-sync.yml) |
| Snapshot | [.github/phase-status.generated.json](../../.github/phase-status.generated.json) |
| Runbook | [GITHUB_PROJECT_MANAGEMENT.md](./GITHUB_PROJECT_MANAGEMENT.md) |

**Comportamento:**
- `github-project bootstrap` — labels, milestones, Project v2, views, issues, colunas Status
- `project-board-sync.yml` — atualiza board a cada issue/PR (infra/federação refletidas por onda)
- `PHASE_QUEUE.yaml` → Issues `[Epic]` com labels `wave/*`, `priority/*`, milestone
- Status operacional: Issues fechadas = fase concluída (fallback `STATUS_FASES.md`)
- Specs/ADRs/FASE*.md permanecem no repo como contrato técnico

---

## Referências

- [REALINHAMENTO_SUSTENTACAO_OPERACIONAL.md](../backlog-api/REALINHAMENTO_SUSTENTACAO_OPERACIONAL.md)
- [.cursorrules](../../.cursorrules)

