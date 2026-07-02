## Spec Steward (SDD) — Checklist de conduta

<!-- arah-conduct-checklist:spec-steward -->

### Escopo permitido
- [ ] `docs/specs/**`, `docs/_meta/SDD_AND_HARNESS.md`, `scripts/harness/**`
- [ ] `.github/workflows/spec-harness.yml`, manifests de agentes ligados a SDD

### Antes do PR (spec-before-code)
- [ ] Spec `*.spec.yaml` existe com `id`, `acceptance`, `status: active|draft`
- [ ] Acceptance em formato EARS (`when` + `then` verificável) quando aplicável
- [ ] **Clarify**: ambiguidades da spec listadas e resolvidas com o humano antes de ativar (nunca deixar o agente "adivinhar")
- [ ] `./scripts/harness/validate-specs.ps1` passa (inclui gate covered_by/evidence)
- [ ] `harness.commands` / `harness.scripts` apontam para comandos reais
- [ ] PR referencia `Spec-Id: <id>` no corpo quando há código de implementação

### Skills (ordem sugerida)
1. `spec-validate`
2. `harness-run` (opcional `-SpecId`)
3. `spec-author` (criar/atualizar spec a partir do template)
4. `sync-docs` (`PLATFORM_STATE`, `STATUS_FASES`, FASE*.md)
5. `open-pr`

### Aderência
- [ ] Critérios `acceptance` cobertos por testes ou scripts no harness
- [ ] Spec não contradiz handoff C4 nem `docs/backlog-api/FASE*.md`
- [ ] Territory data stays on instance (Core não persiste dados de território)

### Proibido
- [ ] Implementar feature de fase sem spec `active` ou `draft` revisada
- [ ] Merge sem workflow `spec-harness` verde (quando paths SDD alterados)
