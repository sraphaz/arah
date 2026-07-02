## Backend Agent — Checklist de conduta

<!-- arah-conduct-checklist:backend -->

### Escopo permitido
- [ ] Alterações em `backend/Arah.*`, `backend/Tests/**` (inclui **`Arah.Core`**)
- [ ] Sem mudanças em Flutter, wiki ou docs fora de sync-docs / spec

### SDD (fases S0+)
- [ ] PR inclui `Spec-Id: <id>` ou spec atualizada em `docs/specs/`
- [ ] **Clarify antes de implementar**: ambiguidades da spec listadas e resolvidas (não adivinhar intenção)
- [ ] `./scripts/agents/arah-agents.ps1 spec-validate` passa
- [ ] Critérios `acceptance` cobertos: `status: covered` exige `covered_by` anotado (`dotnet test --filter FullyQualifiedName~Core` se Core); `status: manual` exige `evidence`

### Antes do PR
- [ ] `dotnet build` e `dotnet test` passam localmente ou via CI
- [ ] Clean Architecture respeitada (Domain não depende de Infrastructure)
- [ ] Nomenclatura: **territory**, **items**, **membership** (nunca place/listings)
- [ ] Territory sem lógica social embutida
- [ ] BFF: jornada registrada em `BffJourneyRegistry` se aplicável
- [ ] `sync-docs` quando contrato API ou regra de negócio mudou

### Skills (ordem sugerida)
1. `run-tests` (area backend)
2. `spec-validate` / `harness-run` (se fase S0+ ou Arah.Core)
3. `register-bff-journey` (se BFF alterado)
4. `sync-docs`
5. `open-pr`

### Consultar quando Core/federação
- [ ] Domain `control-plane` + specialist `core-control-plane`

### Proibido
- [ ] Commit direto em `main`
- [ ] `Araponga.*` namespaces ou `Araponga.sln`
- [ ] Lógica HTTP em Application/Domain
