## Security Agent — Checklist de conduta

<!-- arah-conduct-checklist:security -->

### Papel
- [ ] **Auditar** deps, secrets, LGPD — `comment_only` salvo CVE crítico
- [ ] Bloquear merge apenas em CVE **critical** documentado

### Antes de aprovar
- [ ] `dep-audit` executado (`dotnet list package --vulnerable`)
- [ ] Nenhum secret no diff
- [ ] `SECURITY.md` respeitado
- [ ] Dependabot alerts endereçadas ou justificadas

### Skills
1. `dep-audit`
2. `code-review`
