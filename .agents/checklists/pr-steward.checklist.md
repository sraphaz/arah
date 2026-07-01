## PR Steward — Checklist de conduta

<!-- arah-conduct-checklist:pr-steward -->

### Obrigatório antes de merge
- [ ] **Todo apontamento de bot** resolvido ou respondido explicitamente
- [ ] CI verde (build, testes, Agents Gates, CodeQL, **spec-harness** se SDD)
- [ ] **Spec-Id** presente e spec válida quando `Arah.Core` ou `docs/specs/` no PR
- [ ] Label `ready-for-merge` só quando `pr-ready` passa
- [ ] **Merge continua humano** (`no_merge: true`)

### Skills
1. `address-bot-review`
2. `spec-validate` / `harness-run` (paths SDD/Core)
3. `code-review` (orientativo)
4. `next-phase` (após merge em `main`)

### Referência
- [.agents/templates/bot-review-checklist.md](../templates/bot-review-checklist.md)
