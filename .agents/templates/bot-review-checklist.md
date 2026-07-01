## PR Steward — Apontamentos de bots

<!-- arah-pr-steward -->

### Regra obrigatória

**Todo apontamento de bot deve ser resolvido ou respondido** antes de merge. Não deixar threads abertas em arquivos alterados neste PR.

### Checklist

- [ ] CI verde (build-test, Flutter, Agents Gates, CodeQL)
- [ ] CodeRabbit / reviews — todos os threads tratados
- [ ] Dependabot / segurança — CVEs novas endereçadas ou justificadas
- [ ] Comentários inline em arquivos do diff — resolvidos ou respondidos
- [ ] `sync-docs-check` sem erros bloqueantes
- [ ] Corpo do PR preenchido (template agente)

### Como resolver

```powershell
./scripts/agents/arah-agents.ps1 bot-review -PrNumber <N>
./scripts/agents/arah-agents.ps1 pr-ready -PrNumber <N>
```

### Merge

- Steward posta **ready-for-merge** quando `pr-ready` passa.
- **Humano** executa merge (ou `workflow_dispatch` em `agents-pr-steward.yml` com confirmação).

---
_Automático via `agents-pr-steward.yml`_
