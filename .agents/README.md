# Agent manifests — catálogo versionado

Manifests YAML definem **quem** pode fazer **o quê** no repo. Consumidos por:

- Agentes Cursor (via `AGENTS.md` + leitura do manifest)
- `scripts/agents/orchestrate.ps1` (roteamento)
- `.github/workflows/agents-validate.yml` (validação estrutural)

## Fluxo (abrem PR)

| Arquivo | Agente |
|---------|--------|
| [orchestrator.agent.yaml](orchestrator.agent.yaml) | Orquestrador |
| [planner.agent.yaml](planner.agent.yaml) | Planner |
| [docs-steward.agent.yaml](docs-steward.agent.yaml) | Docs Steward |
| [backend.agent.yaml](backend.agent.yaml) | Backend |
| [flutter.agent.yaml](flutter.agent.yaml) | Flutter |
| [web.agent.yaml](web.agent.yaml) | Web |
| [qa.agent.yaml](qa.agent.yaml) | QA / Review |
| [release.agent.yaml](release.agent.yaml) | Release / DevOps |
| [security.agent.yaml](security.agent.yaml) | Security |

## Consultivos (não abrem PR)

- [domain/](domain/) — regras de negócio
- [specialists/](specialists/) — profundidade técnica por stack
