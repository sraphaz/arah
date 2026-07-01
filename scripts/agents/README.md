# Scripts de agentes

| Script | Uso |
|--------|-----|
| [arah-agents.ps1](arah-agents.ps1) | CLI: `orchestrate`, `route-pr`, `skill`, `validate`, `gates`, `doc-index`, `bot-review`, `pr-ready`, `next-phase`, `ensure-labels` |
| [arah-agents](arah-agents) | Wrapper bash (CI Linux) |
| [invoke-skill.ps1](invoke-skill.ps1) | Executa skills (run-tests, sync-docs, open-pr, …) |
| [run-gates.ps1](run-gates.ps1) | Gates QA / Security / Release |
| [sync-docs-check.ps1](sync-docs-check.ps1) | Verifica doc-sync no PR |
| [register-bff-journey-check.ps1](register-bff-journey-check.ps1) | Valida alterações BFF |
| [open-pr.ps1](open-pr.ps1) | Branch + PR estruturado |
| [gen-l10n.ps1](gen-l10n.ps1) | Flutter l10n |
| [validate-manifests.ps1](validate-manifests.ps1) | Valida `.agents/` e `.skills/` |
| [generate-docs-index.ps1](generate-docs-index.ps1) | Gera `docs/INDEX.generated.md` |
| [migrate-docs-deflate.ps1](migrate-docs-deflate.ps1) | Wave 1: arquiva PR/RESUMO/ANALISE → `_archive/` |
| [address-bot-review.ps1](address-bot-review.ps1) | Audita apontamentos de bots em PR |
| [pr-ready.ps1](pr-ready.ps1) | CI + bots → ready-for-merge |
| [next-phase.ps1](next-phase.ps1) | Abre issue da próxima fase (`PHASE_QUEUE.yaml`) |

Ver [docs/ops/AGENT_OPERATION.md](../../docs/ops/AGENT_OPERATION.md).
