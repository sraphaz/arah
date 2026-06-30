# Skills — procedimentos executáveis

Cada `.skill.yaml` encapsula **comando + guardrail + critério de sucesso**.

| Skill | Arquivo |
|-------|---------|
| run-tests | [run-tests.skill.yaml](run-tests.skill.yaml) |
| open-pr | [open-pr.skill.yaml](open-pr.skill.yaml) |
| sync-docs | [sync-docs.skill.yaml](sync-docs.skill.yaml) |
| register-bff-journey | [register-bff-journey.skill.yaml](register-bff-journey.skill.yaml) |
| gen-l10n | [gen-l10n.skill.yaml](gen-l10n.skill.yaml) |
| code-review | [code-review.skill.yaml](code-review.skill.yaml) |
| backlog-to-issue | [backlog-to-issue.skill.yaml](backlog-to-issue.skill.yaml) |
| release-cut | [release-cut.skill.yaml](release-cut.skill.yaml) |
| dep-audit | [dep-audit.skill.yaml](dep-audit.skill.yaml) |
| doc-taxonomy | [doc-taxonomy.skill.yaml](doc-taxonomy.skill.yaml) |
| iac-plan | [iac-plan.skill.yaml](iac-plan.skill.yaml) |

Execução local: `scripts/agents/invoke-skill.ps1 -Skill run-tests -Area backend`
