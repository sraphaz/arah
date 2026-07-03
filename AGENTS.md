# AGENTS.md — Manual de operação por agentes

**Versão**: 2.0
**Data**: 2026-07-02
**Repo**: `sraphaz/arah`

Fonte de verdade para agentes (Cursor, CI, `arah-agents`). Este arquivo contém o
essencial; procedimentos executáveis vivem em `.skills/` e `.cursor/skills/`,
regras detalhadas em `.cursor/rules/` (escopadas por path), e a operação
completa em `docs/ops/AGENT_OPERATION.md`.

---

## Princípios

1. **Humano comanda, agente executa** — intenção e merge são humanos; agentes propõem PRs.
2. **Tudo via Pull Request** — sem commit direto em `main`.
3. **Escopo mínimo** — cada agente só toca paths permitidos (`.agents/*.agent.yaml`).
4. **Doc como código** — `sync-docs` no mesmo PR; doc desatualizada = bug.
5. **Spec-before-code** — fases S0+ exigem spec em `docs/specs/` e `Spec-Id:` no PR.
6. **Contexto sob demanda** — regras/skills carregadas apenas quando relevantes; comunicação entre agentes é passiva (arquivo + CI), sem turnos extras de modelo.

## Fluxo

```
Intenção (humano) → Orquestrador → Agente + skills → PR → CI + PR Steward → ready-for-merge → Merge (humano) → next-phase
```

**Regra obrigatória — bots**: todo apontamento de bot (CodeRabbit, Dependabot, CodeQL, etc.) deve ser resolvido ou respondido antes do merge. O `pr-steward` só aplica `ready-for-merge` com CI verde e sem pendências.

## Catálogo de agentes

Operacionais (abrem PR): `orchestrator`, `planner`, `docs-steward`, `backend`, `flutter`, `web`, `qa`, `pr-steward`, `spec-steward`, `solutions-architect`, `release`, `security`. Manifests em [`.agents/`](.agents/), checklists em [`.agents/checklists/`](.agents/checklists/).

Consultivos (parecer, não codam): domain agents em [`.agents/domain/`](.agents/domain/) — `control-plane`, `territorio-membership`, `mercado-economia`, `monetizacao-split`, `carteira-arata`, `governanca-transparencia`, `design-ux`, `feed-conteudo`, `mapa-lugares`, `comunidade-conexoes`, `identidade-privacidade` — e specialists em [`.agents/specialists/`](.agents/specialists/).

Roteamento e co-ativação: [`.agents/orchestrator.agent.yaml`](.agents/orchestrator.agent.yaml) + [`.agents/choreography.yaml`](.agents/choreography.yaml).

**Agent Graph**: o grafo operacional (agentes ↔ skills ↔ rules ↔ paths ↔ domínios ↔ specs ↔ harnesses ↔ guardrails ↔ workflows ↔ gates) é formalizado em [`docs/ops/AGENT_GRAPH.md`](docs/ops/AGENT_GRAPH.md), com schema em [`.agents/agent-graph.schema.yaml`](.agents/agent-graph.schema.yaml) e artefato gerado em [`docs/_meta/agent-graph.generated.json`](docs/_meta/agent-graph.generated.json) (`./scripts/agents/arah-agents.ps1 export-graph` / `validate-graph`). Serve para auditar e explicar "por que este agente/skill/harness foi acionado"; não substitui o orquestrador.

## Skills

Catálogo executável em [`.skills/`](.skills/) (25 skills: run-tests, open-pr, sync-docs, spec-validate, harness-run, register-adr, architecture-review, craft-review, next-phase, address-bot-review, etc.), invocáveis via:

```powershell
./scripts/agents/invoke-skill.ps1 -Skill <nome> [args]
./scripts/agents/arah-agents.ps1 <comando>
```

Skills do Cursor (descoberta sob demanda) em [`.cursor/skills/`](.cursor/skills/): `arah-run-tests`, `arah-sync-docs`, `arah-open-pr`, `arah-domain-consult`, `arah-craft`.

## Comunicação entre agentes (modelo passivo)

- **Local (Cursor)**: hook `stop` ([.cursor/hooks.json](.cursor/hooks.json)) grava pareceres de domínio em `.cursor/domain-review.md` — sem `followup_message`, sem turnos extras de modelo. A rule escopada [.cursor/rules/domain-agents-autonomy.mdc](.cursor/rules/domain-agents-autonomy.mdc) instrui o agente a ler o parecer quando tocar código de domínio.
- **CI (PR)**: [agents.yml](.github/workflows/agents.yml) publica os pareceres como comentários (`post-domain-consult.ps1 -PostComment`) em todo `opened`/`synchronize` — esta é a instância autoritativa.
- **Definition of Done**: [docs/governance/DEFINITION_OF_DONE.md](docs/governance/DEFINITION_OF_DONE.md).

## Cursor Cloud — instruções específicas

### Backend
- Use **`Arah.sln`** (não `Araponga.sln`); namespaces `Arah.*`; `backend/Arah.Core/` é o control plane.
- JWT obrigatório: `JWT__SIGNINGKEY=dev-only-change-me` (dev). Default InMemory; Postgres via `Persistence__Provider=Postgres`.
- API: `dotnet run --project backend/Arah.Api` → `:5178`. BFF: `backend/Arah.Api.Bff` → `:5005` (`Bff__ApiBaseUrl=http://localhost:5178`).
- Testes: `dotnet test backend/Tests/Arah.Tests/Arah.Tests.csproj`.
- PRs de fase: corpo com `Spec-Id: <id-da-spec>`.

### Web
- `next lint` quebrado no wiki/portal — use `npm test` e `type-check`. Wiki: porta **3001**, base **`/wiki`**.

### Flutter
- BFF only: `--dart-define=BFF_BASE_URL=...`; l10n: `flutter gen-l10n` + cópia para `lib/l10n/`; `flutter_map` 8.x: taps via `MapOptions.onTap`; jornadas via `BffJourneyRegistry`.

### Documentação (obrigatório em todo PR)
- `docs/CHANGELOG.md`, `docs/STATUS_FASES.md`, fases em `docs/backlog-api/`, specs em `docs/specs/` quando aplicável.

### Guardrails
- **Nunca** merge automático em `main`/prod. **Nunca** secrets no diff.
- CI verde **e** apontamentos de bots resolvidos antes de merge (`pr-steward`).

## Referências

- [docs/ops/AGENT_QUICKSTART.md](docs/ops/AGENT_QUICKSTART.md) — guia de operação para novos devs
- [docs/ops/AGENT_OPERATION.md](docs/ops/AGENT_OPERATION.md) — operação completa (workflows, CLI, visibilidade)
- [docs/_meta/SDD_AND_HARNESS.md](docs/_meta/SDD_AND_HARNESS.md) · [docs/specs/README.md](docs/specs/README.md)
- [docs/ops/PLATFORM_STATE.md](docs/ops/PLATFORM_STATE.md) · [docs/_meta/PHASE_QUEUE.yaml](docs/_meta/PHASE_QUEUE.yaml)
- [docs/ops/GITHUB_PROJECT_MANAGEMENT.md](docs/ops/GITHUB_PROJECT_MANAGEMENT.md)

---

### Changelog

- **2.0** (2026-07-02): Enxugado para reduzir consumo de contexto (de ~15 KB para ~5 KB). Tabelas de SDD, coreografia, skills e visibilidade movidas para `docs/ops/AGENT_OPERATION.md`. Comunicação entre agentes passa a ser passiva (arquivo + CI), sem followups interativos.
- **1.5** (2026-07-01): versão anterior completa.
