# Quickstart — operar o Arah usando agentes

**Versão**: 1.0
**Data**: 2026-07-02
**Público**: novos devs e implementadores que vão trabalhar com (e via) agentes
**Fonte de verdade da operação**: [AGENTS.md](../../AGENTS.md)

---

## 1. O modelo em uma frase

**Humano define a intenção e aprova o merge; agentes executam, testam, documentam e abrem PR.**
Nenhum agente mescla em `main`. Tudo passa por spec (fases S0+), gates de CI e pareceres de domínio.

```
Intenção (issue/label) → Orquestrador roteia → Agente + skills → PR
→ Gates (QA/Security/Design/SDD) + PR Steward (bots) → ready-for-merge → Merge humano → next-phase
```

---

## 2. Setup (uma vez)

Pré-requisitos: PowerShell 7+, .NET 8 SDK, `gh` CLI autenticado, Flutter (se app), Node 20 (se web).

```powershell
# Validar que a malha de agentes está íntegra
./scripts/agents/arah-agents.ps1 validate

# Rodar o harness SDD completo (specs + manifests + guardrails)
./scripts/agents/arah-agents.ps1 harness

# Criar labels canônicas no GitHub (se repo novo)
./scripts/agents/arah-agents.ps1 ensure-labels
```

No Cursor, nada a configurar:

- **Contexto em camadas**: `.cursorrules` (núcleo ~4 KB) + regras escopadas em
  `.cursor/rules/` (só entram quando você toca `backend/**`, `frontend/**`, `docs/**`).
- **Hook passivo**: `.cursor/hooks.json` grava pareceres em `.cursor/domain-review.md`
  sem turnos extras de modelo; a rule `domain-agents-autonomy.mdc` instrui a ler o arquivo
  ao tocar código de domínio. No PR, o CI publica os pareceres como comentários.

---

## 3. Os três tipos de agente

| Tipo | Onde | O que fazem | Exemplos |
|------|------|-------------|----------|
| **Operacionais** | `.agents/*.agent.yaml` | Abrem PR, rodam skills, operam gates | `backend`, `flutter`, `web`, `qa`, `pr-steward` |
| **Domínio** (consultivos) | `.agents/domain/` | Publicam **parecer** com checklist de negócio; não alteram código | `territorio-membership`, `mercado-economia`, `feed-conteudo`, `identidade-privacidade` |
| **Especialistas** (consultivos) | `.agents/specialists/` | Profundidade técnica por stack | `dotnet`, `bff`, `postgresql`, `nextjs` |

Catálogo completo: [.agents/README.md](../../.agents/README.md).

**Como os agentes são acionados** — pela **coreografia** (`.agents/choreography.yaml`):
cada regra mapeia paths → agentes. Ao tocar em `backend/**/Feed/**`, por exemplo, o domínio
`feed-conteudo` publica parecer automaticamente (localmente via hook; no PR via CI).

---

## 4. Fluxo do dia a dia (dev trabalhando com Cursor)

1. **Pegue uma issue** (ou crie via `backlog-to-issue`). Fases S0+ têm `Spec-Id`.
2. **Leia a spec** em `docs/specs/` — implemente **apenas** o que ela descreve.
   Sem spec? Crie primeiro (skill `spec-author`, template `docs/specs/_template.spec.yaml`).
3. **Implemente**. O hook gera `.cursor/domain-review.md` com pareceres dos domínios
   acionados pelo seu diff (comunicação passiva — sem turno extra automático). Antes de
   concluir, **leia o parecer e enderece cada item de "Validar no PR"** aplicável
   (rule `domain-agents-autonomy.mdc` quando em `backend/**` ou `frontend/**`).
4. **Valide localmente**:

```powershell
dotnet build backend/Arah.sln -c Release
dotnet test backend/Tests/Arah.Tests/Arah.Tests.csproj
# Se tocou em módulos, rode também as suítes afetadas (projetos separados):
# dotnet test backend/Tests/Arah.Tests.Modules.<Modulo>/Arah.Tests.Modules.<Modulo>.csproj
./scripts/agents/arah-agents.ps1 spec-validate
./scripts/agents/run-gates.ps1          # QA + Security + Design + SDD
```

5. **Doc-sync no mesmo PR**: `docs/CHANGELOG.md`, `docs/STATUS_FASES.md`,
   `docs/backlog-api/FASE*.md` e a spec (skill `sync-docs`).
6. **Abra o PR** com `Spec-Id: <id>` no corpo e o checklist do
   [DoD](../governance/DEFINITION_OF_DONE.md) preenchido.
7. **No PR**: CI roda gates, publica pareceres de domínio como comentários e o `pr-steward`
   audita bots (CodeRabbit, CodeQL, etc). Todo apontamento de bot deve ser **resolvido ou
   respondido**. Só então o PR recebe `ready-for-merge`.
8. **Merge é seu** (humano). Após merge em `main`, `next-phase` abre a issue da próxima fase
   desbloqueada em `docs/_meta/PHASE_QUEUE.yaml`.

---

## 5. Definition of Done (resumo dos 8 gates)

| # | Gate |
|---|------|
| 1 | Build Release sem erros |
| 2 | Testes verdes (inclui `Arah.Tests.Modules.*` afetados) |
| 3 | Cada AC da spec tem teste que o cobre (`covered_by`) |
| 4 | Endpoint novo/alterado tem teste de integração HTTP |
| 5 | Pareceres de domínio endereçados |
| 6 | Doc-sync no mesmo PR |
| 7 | Sem secrets, sem cor hardcoded, nomenclatura territory/items |
| 8 | Harness SDD verde (`harness-report.json`) |

Detalhe: [DEFINITION_OF_DONE.md](../governance/DEFINITION_OF_DONE.md).

---

## 6. CLI de referência

```powershell
# Roteamento e coreografia
./scripts/agents/arah-agents.ps1 orchestrate -Labels area/backend
./scripts/agents/arah-agents.ps1 route-pr -ChangedFiles backend/Arah.Core/x.cs
./scripts/agents/arah-agents.ps1 choreograph -ChangedFiles backend/Arah.Modules.Feed/y.cs -Json

# SDD / harness
./scripts/agents/arah-agents.ps1 spec-validate
./scripts/agents/arah-agents.ps1 harness            # ou -SpecId FASE55-monetization

# Pareceres de domínio (manual; o hook faz sozinho)
powershell -NoProfile -File scripts/agents/domain-autoreview.ps1 -Force -Json

# Skills avulsas
./scripts/agents/invoke-skill.ps1 -Skill sync-docs
./scripts/agents/arah-agents.ps1 skill -Skill likec4-export

# PR e fases
./scripts/agents/arah-agents.ps1 bot-review -PrNumber <N>
./scripts/agents/arah-agents.ps1 pr-ready -PrNumber <N>
./scripts/agents/arah-agents.ps1 next-phase -DryRun

# Medição e exploração
./scripts/agents/agent-metrics.ps1 -Days 30              # efetividade dos agentes
./scripts/agents/parallel-attempt.ps1 -Name tarefa -Count 2   # best-of-N em worktrees
```

---

## 7. Onde ver a ação dos agentes

| Onde | O quê |
|------|-------|
| Issue/PR | Comentário `## Agente acionado: …` (`<!-- arah-agent-activity:{id} -->`) |
| PR | Pareceres `<!-- arah-domain-consult:{id} -->` |
| Actions → Agents Orchestrate | Artifact `agent-activity-{run_id}.json` |
| Local | `.cursor/domain-review.md` (gerado pelo hook a cada interação) |

---

## 8. Como estender a malha

### Novo agente de domínio

1. Copie um manifest de `.agents/domain/` (ex.: `feed-conteudo.agent.yaml`).
2. Preencha `id`, `name`, `enrich` (visão de negócio), `validate` (checklist verificável),
   `scope.paths`, `references`.
3. Adicione uma regra em `.agents/choreography.yaml` mapeando os paths → agente.
4. Atualize `.agents/README.md`, `AGENTS.md` (tabelas) e a rule
   `.cursor/rules/domain-agents-autonomy.mdc` (mapeamento resumo).
5. `./scripts/agents/arah-agents.ps1 validate` deve passar.

### Nova skill

1. Crie `.skills/<nome>.skill.yaml` com `id` e o script em `scripts/agents/`.
2. Referencie a skill nos manifests dos agentes que podem usá-la.
3. Adicione na tabela de skills do `AGENTS.md`.

> **Fonte de verdade**: `.skills/*.skill.yaml`. Skills Cursor (`.cursor/skills/arah-*`:
> `arah-run-tests`, `arah-sync-docs`, `arah-open-pr`, `arah-domain-consult`) apenas
> **apontam** para o YAML ou script correspondente — nunca duplicam a lógica.

### Nova fase/épico

1. Adicione a fase em `docs/_meta/PHASE_QUEUE.yaml` (com `blocked_by`).
2. Crie a spec em `docs/specs/phases/` **antes** de qualquer código (spec-before-code).
3. `next-phase` abrirá a issue automaticamente quando desbloqueada.

---

## 9. Guardrails inegociáveis

- **Nunca** merge automático em `main`.
- **Nunca** secrets no diff.
- **Nunca** cor hardcoded em UI (gate DSG-08 falha o PR).
- Escopo por manifest: agente só toca os paths do seu `scope`.
- Nomenclatura: **territory** (nunca place), **items** (nunca listings).
- Doc desatualizada = bug: doc-sync no mesmo PR.

---

## Referências

- [AGENTS.md](../../AGENTS.md) — manual central
- [AGENT_STRATEGY_VALIDATION.md](./AGENT_STRATEGY_VALIDATION.md) — validação vs mercado
- [AGENT_OPERATION.md](./AGENT_OPERATION.md) — histórico de construção (PRs 1–14)
- [SDD_AND_HARNESS.md](../_meta/SDD_AND_HARNESS.md) — spec-driven development
- [DEFINITION_OF_DONE.md](../governance/DEFINITION_OF_DONE.md) — DoD rigor total
- [GITHUB_PROJECT_MANAGEMENT.md](./GITHUB_PROJECT_MANAGEMENT.md) — board e issues
