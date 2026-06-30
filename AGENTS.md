# AGENTS.md — Manual de operação por agentes

**Versão**: 1.1  
**Data**: 2026-06-30  
**Repo**: `sraphaz/arah`

Este arquivo é a **fonte de verdade** para agentes (Cursor, CI, `arah-agents`). Regras longas permanecem em `.cursorrules`; **procedimentos executáveis** vivem em `.skills/`.

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

## CLI `arah-agents`

```powershell
./scripts/agents/arah-agents.ps1 orchestrate -Labels area/backend
./scripts/agents/arah-agents.ps1 route-pr -ChangedFiles backend/Arah.Api/Program.cs
./scripts/agents/arah-agents.ps1 validate
./scripts/agents/arah-agents.ps1 gates
./scripts/agents/arah-agents.ps1 ensure-labels
```

Workflows: [agents.yml](.github/workflows/agents.yml), [agents-gates.yml](.github/workflows/agents-gates.yml)

---

## Cursor Cloud — instruções específicas

Monorepo: backend .NET 8 + BFF, web (`frontend/wiki`, `portal`, `devportal`), Flutter (`frontend/arah.app`).

### Backend
- Use **`Arah.sln`**, não `Araponga.sln`. Namespaces `Arah.*`.
- JWT obrigatório: `JWT__SIGNINGKEY=...` (ex.: `dev-only-change-me` em dev).
- Default InMemory — Postgres opcional via `Persistence__Provider=Postgres`.
- API: `dotnet run --project backend/Arah.Api` → `:5178`. BFF: `backend/Arah.Api.Bff` → `:5005`, `Bff__ApiBaseUrl=http://localhost:5178`.
- Testes: `dotnet test backend/Tests/Arah.Tests/Arah.Tests.csproj`.

### Web
- `next lint` quebrado no wiki/portal — use `npm test` e `type-check`.
- Wiki: porta **3001**, base **`/wiki`**.

### Flutter
- BFF only: `--dart-define=BFF_BASE_URL=...`
- l10n: `flutter gen-l10n` + copiar para `lib/l10n/` (skill `gen-l10n`).
- `flutter_map` 8.x: taps via `MapOptions.onTap`.
- Jornadas BFF: `BffJourneyRegistry` (skill `register-bff-journey`).

### Documentação (obrigatório em todo PR)
- `docs/CHANGELOG.md`, `docs/STATUS_FASES.md`, fases em `docs/backlog-api/` quando aplicável.
- Ver [docs/ops/AGENT_OPERATION.md](docs/ops/AGENT_OPERATION.md) e [.cursorrules](.cursorrules).

### Guardrails
- **Nunca** merge automático em `main`/prod.
- **Nunca** secrets no diff.
- CI verde antes de review.

---

## Referências

- [docs/ops/AGENT_OPERATION.md](docs/ops/AGENT_OPERATION.md)
- [REALINHAMENTO_SUSTENTACAO_OPERACIONAL.md](docs/backlog-api/REALINHAMENTO_SUSTENTACAO_OPERACIONAL.md)
