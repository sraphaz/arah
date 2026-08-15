## PR Steward — Apontamentos de bots

<!-- arah-pr-steward -->

### Regra obrigatória

**Threads inline de bots de review** (CodeRabbit, Bugbot, Codex, …) devem ser resolvidas ou respondidas antes do merge.

Sinalizações Arah (`arah-domain-consult`, `arah-agent-activity`, templates QA/Security) **não** bloqueiam sozinhas — mas exigem seção **Pareceres endereçados** no corpo do PR.

### Checklist

- [ ] CI verde (build-test, Flutter, Agents Gates, CodeQL)
- [ ] Threads inline de review bots — resolvidas ou respondidas
- [ ] Dependabot / CVE (se aplicável) — endereçadas ou justificadas
- [ ] Seção **Pareceres endereçados** preenchida no corpo do PR
- [ ] `sync-docs-check` sem erros bloqueantes
- [ ] Corpo do PR preenchido (template agente)

### Como resolver

```powershell
./scripts/agents/arah-agents.ps1 bot-review -PrNumber <N>
./scripts/agents/arah-agents.ps1 pr-ready -PrNumber <N>
```

### Merge

- Steward posta **ready-for-merge** quando audit.ready (CI OK + zero threads de review pendentes).
- **Humano** executa merge (ou `workflow_dispatch` em `agents-pr-steward.yml` com confirmação).

---
_Automático via `agents-pr-steward.yml` — contagem filtra ruído Arah (ver `address-bot-review.ps1`)._
