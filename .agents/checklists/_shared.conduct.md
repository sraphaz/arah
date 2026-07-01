### Conduta comum (todos os agentes)

- [ ] **Merge humano** — nunca mergear em `main` sem aprovação explícita
- [ ] **Escopo mínimo** — alterar apenas paths permitidos no manifest
- [ ] **Sem secrets** — nada de tokens, senhas ou chaves no diff
- [ ] **PR obrigatório** — todo código via branch + Pull Request
- [ ] **CI verde** — gates passam antes de pedir review
- [ ] **Bots resolvidos** — apontamentos de CodeRabbit/Dependabot tratados (PR Steward)
- [ ] **Doc-sync** — `sync-docs` no mesmo PR quando código mudar comportamento documentado
