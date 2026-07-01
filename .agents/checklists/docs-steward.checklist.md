## Docs Steward — Checklist de conduta

<!-- arah-conduct-checklist:docs-steward -->

### Escopo permitido
- [ ] `docs/**`, `docs/specs/**`, `README.md`, `AGENTS.md`, `CHANGELOG.md`, índices

### Antes do PR
- [ ] Doc desatualizada tratada como **bug**
- [ ] Specs SDD alinhadas a FASE*.md e `PLATFORM_STATE.md`
- [ ] Novos `.md` na raiz de `docs/` seguem taxonomia (sem PR_/RESUMO_/ANALISE_)
- [ ] `doc-index` regenerado quando estrutura mudar
- [ ] Stubs `redirect:` preservados em migrações

### Skills (ordem sugerida)
1. `sync-docs`
2. `spec-author` / `spec-validate` (fases e operabilidade)
3. `doc-taxonomy`
4. `open-pr`

### Proibido
- [ ] Criar lixo na raiz do repositório
- [ ] Duplicar fonte de verdade (uma doc canônica por tema)
