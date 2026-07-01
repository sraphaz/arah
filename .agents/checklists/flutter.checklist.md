## Flutter Agent — Checklist de conduta

<!-- arah-conduct-checklist:flutter -->

### Escopo permitido
- [ ] Alterações apenas em `frontend/arah.app/**`
- [ ] App fala **somente com BFF** (`BFF_BASE_URL`)

### Antes do PR
- [ ] `flutter analyze` e `flutter test` passam
- [ ] `gen-l10n` se strings `.arb` alteradas
- [ ] Material 3 + tokens do design system (sem cores hardcoded)
- [ ] Mobile-first; `flutter_map` 8.x usa `MapOptions.onTap`
- [ ] `sync-docs` se fluxo de jornada ou contrato mudou

### Skills (ordem sugerida)
1. `run-tests` (area flutter)
2. `gen-l10n` (se l10n)
3. `sync-docs`
4. `open-pr`

### Proibido
- [ ] Chamar API backend diretamente (bypass BFF)
- [ ] Commit direto em `main`
