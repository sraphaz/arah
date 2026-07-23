# Agent manifests — catálogo versionado

Manifests YAML definem **quem** pode fazer **o quê** no repo. Consumidos por:

- Agentes Cursor (via `AGENTS.md` + leitura do manifest)
- `scripts/agents/arah-agents.ps1` (roteamento e coreografia)
- `.github/workflows/agents-validate.yml` (validação estrutural)

Guia de operação para novos devs: [docs/ops/AGENT_QUICKSTART.md](../docs/ops/AGENT_QUICKSTART.md)
Validação da estratégia vs mercado: [docs/ops/AGENT_STRATEGY_VALIDATION.md](../docs/ops/AGENT_STRATEGY_VALIDATION.md)

## Operacionais — fluxo (abrem PR ou operam gates)

| Arquivo | Agente | Papel |
|---------|--------|-------|
| [orchestrator.agent.yaml](orchestrator.agent.yaml) | Orquestrador | Roteia intenções; não coda |
| [planner.agent.yaml](planner.agent.yaml) | Planner | Backlog → issues + specs |
| [docs-steward.agent.yaml](docs-steward.agent.yaml) | Docs Steward | Taxonomia, doc-sync, índices |
| [backend.agent.yaml](backend.agent.yaml) | Backend | API .NET, BFF, Arah Core |
| [flutter.agent.yaml](flutter.agent.yaml) | Flutter | App mobile |
| [web.agent.yaml](web.agent.yaml) | Web | Wiki, portal, devportal |
| [qa.agent.yaml](qa.agent.yaml) | QA / Review | Qualidade em todo PR |
| [pr-steward.agent.yaml](pr-steward.agent.yaml) | PR Steward | Bots, ready-for-merge, next-phase |
| [spec-steward.agent.yaml](spec-steward.agent.yaml) | Spec Steward | Specs SDD + harness |
| [solutions-architect.agent.yaml](solutions-architect.agent.yaml) | Solutions Architect | ADRs, Clean Architecture, LikeC4 |
| [release.agent.yaml](release.agent.yaml) | Release / DevOps | CI/CD, versões, IaC |
| [security.agent.yaml](security.agent.yaml) | Security | Deps, secrets, LGPD |

## Consultivos — domínio (não abrem PR; parecer via coreografia)

| Arquivo | Domínio |
|---------|---------|
| [domain/territorio-membership.agent.yaml](domain/territorio-membership.agent.yaml) | Território & Membership |
| [domain/mercado-economia.agent.yaml](domain/mercado-economia.agent.yaml) | Mercado & Economia Local |
| [domain/monetizacao-split.agent.yaml](domain/monetizacao-split.agent.yaml) | Monetização & Split |
| [domain/carteira-arata.agent.yaml](domain/carteira-arata.agent.yaml) | Carteira Aratá (ledger/payout) |
| [domain/governanca-transparencia.agent.yaml](domain/governanca-transparencia.agent.yaml) | Governança & Transparência |
| [domain/control-plane.agent.yaml](domain/control-plane.agent.yaml) | Arah Core / Federação |
| [domain/design-ux.agent.yaml](domain/design-ux.agent.yaml) | Design & Experiência |
| [domain/feed-conteudo.agent.yaml](domain/feed-conteudo.agent.yaml) | Feed, Eventos & Conteúdo |
| [domain/mapa-lugares.agent.yaml](domain/mapa-lugares.agent.yaml) | Mapa, Lugares & Assets |
| [domain/comunidade-conexoes.agent.yaml](domain/comunidade-conexoes.agent.yaml) | Comunidade, Conexões & Notificações |
| [domain/identidade-privacidade.agent.yaml](domain/identidade-privacidade.agent.yaml) | Identidade, Privacidade & LGPD |
| [domain/signal-scout.agent.yaml](domain/signal-scout.agent.yaml) | TI — Signal Scout (nunca publica) |
| [domain/territorial-analyst.agent.yaml](domain/territorial-analyst.agent.yaml) | TI — Territorial Analyst |
| [domain/source-steward.agent.yaml](domain/source-steward.agent.yaml) | TI — Source Steward |
| [domain/community-brief-writer.agent.yaml](domain/community-brief-writer.agent.yaml) | TI — Community Brief Writer |
| [domain/response-orchestrator.agent.yaml](domain/response-orchestrator.agent.yaml) | TI — Response Orchestrator (só sugere) |
| [domain/intelligence-governance-steward.agent.yaml](domain/intelligence-governance-steward.agent.yaml) | TI — Governance Steward (auditoria) |

## Consultivos — especialistas (profundidade por stack)

| Arquivo | Stack |
|---------|-------|
| [specialists/dotnet.agent.yaml](specialists/dotnet.agent.yaml) | .NET / EF Core |
| [specialists/bff.agent.yaml](specialists/bff.agent.yaml) | BFF journeys |
| [specialists/core-control-plane.agent.yaml](specialists/core-control-plane.agent.yaml) | Arah.Core |
| [specialists/postgresql.agent.yaml](specialists/postgresql.agent.yaml) | PostgreSQL / migrations |
| [specialists/flutter.agent.yaml](specialists/flutter.agent.yaml) | Flutter |
| [specialists/nextjs.agent.yaml](specialists/nextjs.agent.yaml) | Next.js |

## Coreografia

[choreography.yaml](choreography.yaml) mapeia paths → agentes (co-ativação + pareceres de domínio).
Checklists de conduta em [checklists/](checklists/); templates de PR/gates em [templates/](templates/).
