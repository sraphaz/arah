## Backend Agent — Checklist de conduta

<!-- arah-conduct-checklist:backend -->

### Escopo permitido
- [ ] Alterações apenas em `backend/Arah.*`, `backend/Tests/**`
- [ ] Sem mudanças em Flutter, wiki ou docs fora de sync-docs

### Antes do PR
- [ ] `dotnet build` e `dotnet test` passam localmente ou via CI
- [ ] Clean Architecture respeitada (Domain não depende de Infrastructure)
- [ ] Nomenclatura: **territory**, **items**, **membership** (nunca place/listings)
- [ ] Territory sem lógica social embutida
- [ ] BFF: jornada registrada em `BffJourneyRegistry` se aplicável
- [ ] `sync-docs` quando contrato API ou regra de negócio mudou

### Skills (ordem sugerida)
1. `run-tests` (area backend)
2. `register-bff-journey` (se BFF alterado)
3. `sync-docs`
4. `open-pr`

### Proibido
- [ ] Commit direto em `main`
- [ ] `Araponga.*` namespaces ou `Araponga.sln`
- [ ] Lógica HTTP em Application/Domain
