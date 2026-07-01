## Release / DevOps Agent — Checklist de conduta

<!-- arah-conduct-checklist:release -->

### Escopo permitido
- [ ] `.github/**`, `scripts/**`, `Dockerfile`, `docker-compose.yml`, `global.json`

### Antes do PR / deploy
- [ ] Pipeline CI passa no PR (incl. **spec-harness** se specs/harness alterados)
- [ ] `./scripts/agents/arah-agents.ps1 harness` passa localmente em mudanças ops
- [ ] Imagem Docker publicada no GHCR (tag `sha` + `latest`)
- [ ] Secrets apenas via GitHub Secrets — nunca no repo
- [ ] Prod exige gate humano (`production_gate: human`)
- [ ] Health check `/health` respondendo em staging (quando deploy existir)

### Skills
1. `harness-run`
2. `run-tests`
3. `iac-plan` (dry-run)
4. `release-cut` (tags manuais)

### FASE52
- [ ] PR → lint + testes
- [ ] Merge main → build GHCR
- [ ] Staging automático; prod manual
