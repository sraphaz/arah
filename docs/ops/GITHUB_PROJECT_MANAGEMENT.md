# Gestão de projeto via GitHub (Issues, Labels, Milestones, Projects)

**Versão**: 1.0  
**Data**: 2026-07-01

O Arah adota um modelo **híbrido**: contratos técnicos no repo (specs SDD, ADRs, `FASE*.md`), **execução e status** no GitHub.

---

## O que ficou no GitHub (fonte operacional)

| Recurso | Onde | Uso |
|---------|------|-----|
| **Issues** | `[Epic] FASE52…` | Épicos de fase, AC resumidos, link para docs |
| **Labels** | `.github/labels.yml` | `area/*`, `wave/S0`, `priority/P0`, `epic/phase` |
| **Milestones** | `.github/milestones.yml` | Ondas S0–S4 |
| **Project v2** | `.github/project/arah-sustentacao.yml` | Kanban (após bootstrap) |
| **Templates** | `.github/ISSUE_TEMPLATE/` | Agent Task, Phase Epic, config chooser |
| **Status JSON** | `.github/phase-status.generated.json` | Snapshot gerado por CI de Issues |

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

### Setup (5 min, uma vez)

1. **Criar PAT** — GitHub → Settings → Developer settings → Personal access tokens → **Tokens (classic)**  
   Scopes: `project`, `repo`

2. **Gravar secret** — Repo `arah` → Settings → Secrets and variables → Actions → **New repository secret**  
   Nome: `GH_PROJECT_TOKEN` · Valor: o PAT

3. **Bootstrap** — Actions → **Bootstrap GitHub Project** → Run workflow  
   Cria o board, views, liga ao repo, popula FASE52–61 e grava `project_url` no YAML.

4. **Local (opcional)**  
   ```powershell
   gh auth refresh -h github.com -s project,read:project
   ./scripts/agents/arah-agents.ps1 github-project -Skill bootstrap
   ```

Após o bootstrap, abra: `https://github.com/users/SEU_USER/projects/N` (URL em `.github/project/arah-sustentacao.yml`).

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

## Workflow CI

| Workflow | Gatilho | Ação |
|----------|---------|------|
| **`project-bootstrap.yml`** | manual (`workflow_dispatch`) | **Cria** Project + views (requer `GH_PROJECT_TOKEN`) |
| `github-sync.yml` | push `main`, semanal | labels/milestones + bootstrap se secret existir |
| `project-board-sync.yml` | issue/PR | reconcile + sync coluna Status + add-to-project |

---

## Referências

- [AGENTS.md](../../AGENTS.md)
- [AGENT_OPERATION.md](./AGENT_OPERATION.md)
- [PHASE_QUEUE.yaml](../_meta/PHASE_QUEUE.yaml)
