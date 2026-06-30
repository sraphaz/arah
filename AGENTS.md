# AGENTS.md — Manual de operação por agentes

**Versão**: 1.0  
**Data**: 2026-06-30  
**Repo**: `sraphaz/arah`

Este arquivo é a **fonte de verdade** para agentes (Cursor, CI, futuro `arah-agents`). Regras longas permanecem em `.cursorrules`; **procedimentos executáveis** vivem em `.skills/`.

**Handoff interativo**: [docs/handoff/Operacao por Agentes - Arah.dc.html](docs/handoff/Operacao%20por%20Agentes%20-%20Arah.dc.html)

---

## Princípios

1. **Humano comanda, agente executa** — intenção e merge são humanos; agentes propõem PRs.
2. **Tudo via Pull Request** — sem commit direto em `main`.
3. **Escopo mínimo** — cada agente só toca paths permitidos (ver `.agents/*.agent.yaml`).
4. **Doc como código** — `sync-docs` no mesmo PR que código; doc desatualizada = bug.
5. **Território-primeiro** — mesma ética do produto: agentes pequenos, desacoplados.

---

## Fluxo

```
Intenção (humano) → Orquestrador → Agente + skills → PR → CI + QA → Aprovação (humano) → Merge
```

---

## Catálogo de agentes (fluxo)

| ID | Agente | Gatilho | Escopo | Manifest |
|----|--------|---------|--------|----------|
| `orchestrator` | Orquestrador | issue/PR + label | Roteia, não coda | [.agents/orchestrator.agent.yaml](.agents/orchestrator.agent.yaml) |
| `planner` | Planner / Backlog Steward | descoberta, agenda | backlog, STATUS_FASES | [.agents/planner.agent.yaml](.agents/planner.agent.yaml) |
| `docs-steward` | Docs Steward | merge, agenda | `docs/`, README, CHANGELOG | [.agents/docs-steward.agent.yaml](.agents/docs-steward.agent.yaml) |
| `backend` | Backend Agent | `area/backend` | `backend/Arah.Api*`, tests | [.agents/backend.agent.yaml](.agents/backend.agent.yaml) |
| `flutter` | Flutter Agent | `area/flutter` | `frontend/arah.app/` | [.agents/flutter.agent.yaml](.agents/flutter.agent.yaml) |
| `web` | Web Agent | `area/web` | wiki, portal, devportal | [.agents/web.agent.yaml](.agents/web.agent.yaml) |
| `qa` | Review / QA Agent | PR aberto | todos os PRs | [.agents/qa.agent.yaml](.agents/qa.agent.yaml) |
| `release` | Release / DevOps | merge main, tag | `.github/`, infra | [.agents/release.agent.yaml](.agents/release.agent.yaml) |
| `security` | Security / Compliance | PR, agenda | deps, secrets, SECURITY | [.agents/security.agent.yaml](.agents/security.agent.yaml) |

**Consultivos** (não abrem PR sozinhos): `.agents/domain/`, `.agents/specialists/`

---

## Skills componíveis

| Skill | Uso |
|-------|-----|
| [run-tests](.skills/run-tests.skill.yaml) | `dotnet test` / `flutter test` / `npm test` por área |
| [open-pr](.skills/open-pr.skill.yaml) | Branch + PR estruturado |
| [sync-docs](.skills/sync-docs.skill.yaml) | README, CHANGELOG, STATUS_FASES, FASE* |
| [register-bff-journey](.skills/register-bff-journey.skill.yaml) | BffJourneyRegistry |
| [gen-l10n](.skills/gen-l10n.skill.yaml) | flutter gen-l10n + cópia para lib/l10n |
| [code-review](.skills/code-review.skill.yaml) | Checklist 21_CODE_REVIEW / 22_COHESION |
| [backlog-to-issue](.skills/backlog-to-issue.skill.yaml) | Item backlog → issue com AC |
| [release-cut](.skills/release-cut.skill.yaml) | Versão, tag, notas |
| [dep-audit](.skills/dep-audit.skill.yaml) | CVEs, secrets |
| [doc-taxonomy](.skills/doc-taxonomy.skill.yaml) | Taxonomia docs + índice |
| [iac-plan](.skills/iac-plan.skill.yaml) | terraform/helm dry-run |

---

## Gotchas do repo (obrigatório)

### Backend / BFF

- Nova jornada BFF: atualizar `BffJourneyRegistry` (constante, paths, cache) — skill `register-bff-journey`.
- Clean Architecture: Domain não depende de Infrastructure.
- Testes: `dotnet test backend/Tests/Arah.Tests/Arah.Tests.csproj`.

### Flutter

- Após editar `.arb`: `flutter gen-l10n` e copiar gerados para `lib/l10n/` — skill `gen-l10n`.
- `flutter_map` 8.x: usar `onTap` (não API antiga).
- Nomenclatura: **territory**, **items** (nunca place/listings).

### Web (Next.js)

- Wiki: base `/wiki`, porta dev 3001.
- Preferir `npm test` / type-check; `next lint` pode estar quebrado.

### Documentação

- Nada na raiz exceto arquivos permitidos (ver `.cursorrules`).
- Backlog: `docs/backlog-api/FASE*.md`.
- Taxonomia alvo: [docs/_meta/DOC_TAXONOMY.md](docs/_meta/DOC_TAXONOMY.md).

### Guardrails

- **Nunca** merge automático em `main`/prod.
- **Nunca** commitar secrets, `.env`, credenciais.
- PR atômico; preferir &lt; 40 arquivos por PR de agente.
- CI verde obrigatório antes de pedir review.

---

## Rotear uma tarefa (humano)

1. Abra issue com template **Agent Task** (`.github/ISSUE_TEMPLATE/agent-task.yml`).
2. Labels: `area/backend` | `area/flutter` | `area/web` | `area/docs` | `area/ops`.
3. Workflow [`.github/workflows/agents.yml`](.github/workflows/agents.yml) comenta o roteamento na issue/PR.
4. Agente aplica skills → abre PR → QA agent + CI → humano aprova merge.

### CLI `arah-agents`

```powershell
./scripts/agents/arah-agents.ps1 orchestrate -Labels area/backend
./scripts/agents/arah-agents.ps1 route-pr -ChangedFiles backend/Arah.Api/Program.cs,frontend/arah.app/lib/main.dart
./scripts/agents/arah-agents.ps1 validate
./scripts/agents/arah-agents.ps1 ensure-labels   # gera comandos gh label create
```

Atalho legado: `scripts/agents/orchestrate.ps1 -Labels area/backend`

---

## Adoção (fases)

| Fase | Modo |
|------|------|
| 1 Assistido | Agente sugere; humano edita e commita |
| 2 Copiloto | Agente abre PR; humano revisa (**atual**) |
| 3 Autônomo c/ gate | Agente end-to-end; humano só merge |

---

## Referências

- [.cursorrules](.cursorrules) — políticas herdadas
- [docs/ops/AGENT_OPERATION.md](docs/ops/AGENT_OPERATION.md) — operação detalhada
- [REALINHAMENTO_SUSTENTACAO_OPERACIONAL.md](docs/backlog-api/REALINHAMENTO_SUSTENTACAO_OPERACIONAL.md) — épicos pós-infra agentes
