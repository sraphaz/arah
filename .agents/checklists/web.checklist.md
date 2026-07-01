## Web Agent — Checklist de conduta

<!-- arah-conduct-checklist:web -->

### Escopo permitido
- [ ] Alterações em `frontend/wiki/**`, `frontend/portal/**`, `frontend/devportal/**`

### Antes do PR
- [ ] `npm test` e `type-check` na área afetada
- [ ] Wiki: base `/wiki`, porta 3001 em dev
- [ ] Sem cores hardcoded — variáveis CSS / Tailwind configurado
- [ ] Mobile-first
- [ ] `sync-docs` se navegação ou fases documentadas mudaram

### Skills (ordem sugerida)
1. `run-tests` (area web)
2. `sync-docs`
3. `open-pr`

### Proibido
- [ ] `next lint` como gate bloqueante (quebrado no wiki) — usar test + type-check
- [ ] Commit direto em `main`
