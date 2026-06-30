## QA Agent — Checklist

<!-- arah-qa-gate -->

Revise este PR conforme [docs/21_CODE_REVIEW.md](docs/21_CODE_REVIEW.md) e [docs/22_COHESION_AND_TESTS.md](docs/22_COHESION_AND_TESTS.md).

### Arquitetura
- [ ] Dependências apontam para dentro (Clean Architecture)
- [ ] Territory sem lógica social embutida
- [ ] Nomenclatura: **territory**, **items**, **membership** (nunca place/listings)

### Testes
- [ ] Comportamento novo tem teste
- [ ] `run-tests` passou na área alterada
- [ ] Sem regressão óbvia em edge cases

### UI (se aplicável)
- [ ] Mobile-first
- [ ] Sem cores hardcoded (variáveis CSS / tokens)
- [ ] Acessibilidade básica (labels, contraste)

### PR
- [ ] Escopo atômico
- [ ] sync-docs aplicado
- [ ] Corpo do PR preenchido (template agente)

---
_Automático via `agents-gates.yml` — comentário orientativo; merge continua humano._
