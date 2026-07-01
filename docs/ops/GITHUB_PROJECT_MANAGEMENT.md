# Gestão de projeto via GitHub (Issues, Labels, Milestones, Projects)

**Versão**: 1.1  
**Data**: 2026-07-01

O Arah adota um modelo **híbrido**: contratos técnicos no repo (specs SDD, ADRs, `FASE*.md`), **execução e status** no GitHub.

**Project ativo:** [sraphaz/projects/3](https://github.com/users/sraphaz/projects/3) — roadmap completo (49 fases + extras).

---

## O que ficou no GitHub (fonte operacional)

| Recurso | Onde | Uso |
|---------|------|-----|
| **Issues** | `[Epic] FASE…` | Épicos de fase, AC resumidos, link para docs |
| **Labels** | `.github/labels.yml` | `area/*`, `wave/C1`…`C10`, `wave/S0`…`S4`, `epic/phase` |
| **Milestones** | `.github/milestones.yml` | Ondas comunidade (C1–C10) + sustentação (S0–S4) |
| **Project v2** | `.github/project/arah-sustentacao.yml` | Kanban — `project_number: 3` |
| **Roadmap meta** | `docs/_meta/PHASE_ROADMAP_META.yaml` | Fases concluídas + overrides |
| **Fila operacional** | `docs/_meta/PHASE_QUEUE.yaml` | Prioridade next-phase (FASE52+) |
| **Templates** | `.github/ISSUE_TEMPLATE/` | Agent Task, Phase Epic, config chooser |
| **Status JSON** | `.github/phase-status.generated.json` | Snapshot gerado por CI |

---

## O que permanece no repo (contrato técnico)

| Artefato | Motivo |
|----------|--------|
| `docs/specs/**/*.yaml` | SDD + harness CI |
| `docs/backlog-api/FASE*.md` | AC detalhados, estimativas |
| `docs/architecture/adrs/` | Decisões versionadas |
| `docs/_meta/PHASE_QUEUE.yaml` | Fila machine-readable (sync → Issues) |
| `docs/STATUS_FASES.md` | Dashboard humano + wiki (complementar) |
| `.agents/`, `.skills/` | Orquestração de agentes |

---

## Por que o Project não aparece?

O **`GITHUB_TOKEN` do Actions não acessa Projects v2** (limitação do GitHub). É necessário um PAT com scope `project` uma única vez.

### Setup (5 min, uma vez) — **PAT classic obrigatório**

> ⚠️ **Fine-grained NÃO funciona** para Projects do seu usuário (`sraphaz`).  
> Só **Tokens (classic)** com scope **`project`** (leitura + escrita).

#### Passo a passo (classic)

1. Abra: **https://github.com/settings/tokens** (não "Fine-grained tokens")
2. **Generate new token** → **Generate new token (classic)**
3. Note: ex. `arah-project-bootstrap`
4. Expiration: 90 dias ou No expiration (sua escolha)
5. Marque os scopes:
   - ✅ **`repo`** (checkbox inteiro — Full control of private repositories)
   - ✅ **`project`** (checkbox inteiro — **não** é só "read"; o scope chama-se `project`)
6. **Generate token** → copie o `ghp_...` (só aparece uma vez)
7. Repo **arah** → **Settings** → **Secrets and variables** → **Actions**
8. **New repository secret** → Name: `GH_PROJECT_TOKEN` → Value: o `ghp_...`
9. Actions → **Bootstrap GitHub Project** → **Run workflow**

#### Como saber se errou o tipo

| Sintoma | Causa |
|---------|--------|
| `Resource not accessible by personal access token` | Fine-grained ou classic sem `project` |
| Workflow "Diagnose" mostra `fine-grained-or-unknown` | Criou em Fine-grained em vez de classic |
| `x-oauth-scopes` sem `project` | Classic sem marcar **project** |

Teste local após configurar secret:
```powershell
$env:GH_PROJECT_TOKEN = 'ghp_...'
./scripts/agents/test-project-token.ps1
```

#### Alternativa: Project manual (sem criar via API)

1. https://github.com/users/sraphaz/projects/3 (já criado)
2. **Link to repository** → `sraphaz/arah` (se ainda não ligou)
3. Actions → **Bootstrap GitHub Project** → Run workflow (popula 49 fases no board)

Ou local (com PAT `project`):

```powershell
gh auth refresh -h github.com -s project,read:project
./scripts/agents/github-project.ps1 bootstrap
```

---

### Setup resumido (referência)

### Fonte de verdade (gestão no GitHub, não no repo)

| O quê | Onde vive |
|-------|-----------|
| Status operacional (em progresso / feito) | **Issues** + coluna Status do **Project** |
| Ondas S0–S4 | **Milestones** + labels `wave/*` |
| Prioridade | Labels `priority/*` |
| Contrato técnico (AC, specs) | Repo: `docs/specs/`, `FASE*.md` |
| Snapshot machine-readable | Artifact CI `phase-status` (não é mais commitado em todo sync) |

---

## Bootstrap (uma vez ou via CI)

```powershell
# Pipeline completo (labels, project, issues, board, phase-status)
./scripts/agents/arah-agents.ps1 github-project -Skill bootstrap

# Ou passo a passo
./scripts/agents/arah-agents.ps1 ensure-labels
./scripts/agents/arah-agents.ps1 sync-milestones
./scripts/agents/arah-agents.ps1 github-project -Skill ensure
./scripts/agents/arah-agents.ps1 github-project -Skill reconcile
./scripts/agents/arah-agents.ps1 github-project -Skill sync-queue -DryRun
./scripts/agents/arah-agents.ps1 github-project -Skill sync-queue
./scripts/agents/arah-agents.ps1 github-project -Skill sync-project
```

**Scope local:** `gh auth refresh -h github.com -s project,read:project`  
**CI:** workflow `github-sync.yml` roda `bootstrap` semanalmente e em push de `PHASE_QUEUE.yaml`.

---

## Fluxo automático (agentes)

```
PHASE_QUEUE.yaml → next-phase (pós-merge main) → Issue [Epic] + labels + milestone
                → pr-steward / planner → backlog-to-issue (fase específica)
                → github-sync.yml (semanal) → phase-status.generated.json
                → agents.yml (labels area/*) → roteamento de agentes
```

### Comandos CLI

```powershell
./scripts/agents/arah-agents.ps1 next-phase -DryRun
./scripts/agents/arah-agents.ps1 backlog-to-issue -SpecId FASE54
./scripts/agents/arah-agents.ps1 github-project -Skill status -Json
./scripts/agents/arah-agents.ps1 export-phase-status -Json
```

---

## Marcadores em Issues

| Marker | Significado |
|--------|-------------|
| `<!-- arah-next-phase id=FASE53 -->` | Issue criada pela fila |
| `<!-- arah-phase-epic -->` | Épico de fase sustentação |
| `<!-- arah-agent-activity:backend -->` | Checklist de agente |

**Conclusão de fase**: fechar a Issue épico (ou label `epic/phase` + estado closed). `next-phase` consulta Issues fechadas antes de `STATUS_FASES.md`.

---

## Project v2 — colunas sugeridas

1. **Backlog** — fase na fila, sem issue ou bloqueada  
2. **Ready** — issue aberta, deps OK  
3. **In Progress** — PR ativo linkado  
4. **Review** — PR em review / bots pendentes  
5. **Done** — issue fechada + merge em main  

Campos custom (opcional): Wave, Priority, Spec-Id — hoje cobertos por **labels**.

---

## Project v2 — views (manual)

A API GraphQL **não expõe mais** `createProjectV2View`. Views extras do board são criadas **no UI** do project:

1. Abra [Arah — Roadmap (Project #3)](https://github.com/users/sraphaz/projects/3)
2. **+ New view** → escolha layout (Board, Table, Roadmap)
3. Sugestões em `.github/project/arah-sustentacao.yml` → `views:`

| View sugerida | Layout |
|---------------|--------|
| Kanban — Roadmap | Board (agrupar por Status) |
| Tabela — Por onda | Table (campo Milestone ou label wave) |
| Tabela — Sustentação (52–61) | Table (filtro label `wave/S0` … `wave/S4`) |
| Roadmap — Completo | Roadmap (Start/End date se configurado) |

O sync (`sync-project` / `sync-status`) **não tenta** criar views — evita warnings de API deprecada.

---

## Workflow CI

| Workflow | Gatilho | Ação |
|----------|---------|------|
| **`project-bootstrap.yml`** | manual (`workflow_dispatch`) | Bootstrap completo (issues, board, status; views manuais no UI) |
| `github-sync.yml` | push `main`, semanal | labels/milestones + bootstrap se secret existir |
| `project-board-sync.yml` | issue/PR | reconcile + sync coluna Status + add-to-project |

---

## Referências

- [AGENTS.md](../../AGENTS.md)
- [AGENT_OPERATION.md](./AGENT_OPERATION.md)
- [PHASE_QUEUE.yaml](../_meta/PHASE_QUEUE.yaml)
