# ADR-NNN: Arah Core como control plane separado

**Status**: accepted  
**Data**: 2026-06-30  
**Autor**: Solutions Architect (agente / humano)  
**Spec-Id**: FASE53-arah-core  
**LikeC4 view**: containers

---

## Contexto

FASE53 introduz hub operacional (Arah Core) para registrar instancias, releases e telemetria sem possuir dados de territorio.

## Decisão

Projeto backend/Arah.Core isolado; endpoints api/v1/core/* na API; registries in-memory inicialmente; dados de territorio permanecem na instancia.

## Consequências

Positivo: fronteira clara control plane vs instancia. Negativo: persistencia e mTLS adiados; degradação graciosa ainda pendente (AC-53-6).

## Alternativas consideradas

1. Core embutido em Arah.Api (rejeitado — viola separacao de bounded context). 2. Core como microservico deploy separado ja no MVP (rejeitado — YAGNI ate FASE54).

## Diagrama

_Opcional: export LikeC4 → docs/architecture/diagrams/_

## Referências

- [ADR Registry](../ADR-REGISTRY.yaml)
- [10_ARCHITECTURE_DECISIONS.md](../10_ARCHITECTURE_DECISIONS.md)

