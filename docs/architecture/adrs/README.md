# ADRs — Architecture Decision Records

Decisões arquiteturais registradas pelo **Solutions Architect** e revisadas em PR.

## Formato

| Era | Local | Uso |
|-----|-------|-----|
| ADR-001 … ADR-019 | [10_ARCHITECTURE_DECISIONS.md](../10_ARCHITECTURE_DECISIONS.md) | Legado (documento único) |
| ADR-020+ | `docs/architecture/adrs/ADR-NNN-slug.md` | **Padrão atual** — um arquivo por decisão |

### Índice ADR-020+

| ID | Título | Status | Arquivo |
|----|--------|--------|---------|
| ADR-020 | Arah Core como control plane separado | accepted | [ADR-020-arah-core-como-control-plane-separado.md](./ADR-020-arah-core-como-control-plane-separado.md) |
| ADR-021 | Design system do app como fonte canônica de UI | Accepted | [ADR-021-design-system-app-canonic.md](./ADR-021-design-system-app-canonic.md) |
| ADR-022 | Convenção de sinalização de erros (Result, nullable, exceptions) | accepted | [ADR-022-convencao-sinalizacao-de-erros.md](./ADR-022-convencao-sinalizacao-de-erros.md) |

Índice machine-readable: [ADR-REGISTRY.yaml](../ADR-REGISTRY.yaml)

## Registrar nova ADR

```powershell
./scripts/agents/register-adr.ps1 `
  -Title "Arah Core como control plane" `
  -Status proposed `
  -Context "FASE53 exige hub operacional..." `
  -Decision "Projeto Arah.Core separado; dados de território ficam na instância." `
  -Consequences "Core stateless inicial; persistência Postgres na FASE54." `
  -SpecId FASE53-arah-core `
  -LikeC4View containers
```

Ou via skill / agente:

```powershell
./scripts/agents/arah-agents.ps1 skill -Skill register-adr -Title "..." 
```

## Campos obrigatórios

- **Contexto** — por que decidir agora
- **Decisão** — o que foi escolhido
- **Consequências** — trade-offs (+ / −)
- **Alternativas consideradas** — pelo menos uma rejeitada

Opcional: `Spec-Id`, view LikeC4, diagrama em `docs/architecture/diagrams/`

## Regra (Solutions Architect)

Mudança **estrutural** (novo módulo, fronteira, Core, federação) → **ADR obrigatória** antes ou no mesmo PR da implementação.
