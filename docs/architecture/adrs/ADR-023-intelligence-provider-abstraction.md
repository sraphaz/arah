# ADR-023: Abstração de provedor de inteligência (SignalProvider)

**Status**: accepted  
**Data**: 2026-08-15  
**Autor**: Solutions Architect (agente / humano)  
**Spec-Id**: TI-0  
**LikeC4 view**: containers

---

## Contexto

A trilha de Inteligência Territorial consome sinais externos (inicialmente World Monitor). Se o domínio ou a Application conhecessem o WM diretamente (URLs, headers `X-WorldMonitor-Key`, schemas RPC), trocar de provedor, mockar no CI ou federar fontes oficiais brasileiras ficaria custoso e violaria Clean Architecture (dependências apontando para fora).

O handoff define o conceito `SignalProvider` e o adapter `Infrastructure/Intelligence/Providers/WorldMonitor/` como primeiro provedor. A decisão 20 exige consumo só por API (sem código AGPL no backend).

## Decisão

1. **Porta de domínio/aplicação**: contratos agnósticos (`SignalProvider`, normalização para `ExternalSignal`, health/quota) sem tipos, URLs ou nomes de vendor no núcleo.
2. **Adapter na Infrastructure**: World Monitor é o **primeiro** adapter REST; vive só em Infrastructure (e testes de contrato com fixtures).
3. **Substituição**: mocks/fixtures no CI, provedores futuros (órgãos oficiais, segundo agregador) e desligamento de emergência (`emergencyOff`) operam atrás da mesma porta.
4. **Territory permanece neutro**: relevância, política e provedor **não** entram na entidade Territory.

## Consequências

**Positivas**
- Domínio testável sem rede; CI com fixtures (TI-0/TI-701).
- Troca ou adição de provedor sem reescrever regras de publicação/revisão.
- Alinha com decisão 20 (consumo API + atribuição no mapeamento, não no domínio).

**Negativas / trade-offs**
- Indireção extra (mapper + ledger de quota por provedor).
- Mapeamento de schemas externos exige manutenção quando o OpenAPI do WM mudar (drift check).

## Alternativas consideradas

1. **Chamar WM direto da Application** — rejeitado: acoplamento a vendor, dificulta mock e viola Clean Architecture.
2. **SDK oficial embutido no Domain** — rejeitado: Domain não deve depender de pacotes de integração; SDK MIT, se usado, fica na Infrastructure.
3. **Um único cliente “WorldMonitorService” global sem porta** — rejeitado: impede segundo provedor e testes de contrato limpos.

## Diagrama

```
[Application] --porta--> ISignalProvider
                              ^
                              | implementa
                    [WorldMonitorAdapter]  (Infrastructure)
                              |
                         API REST / fixtures
```

## Referências

- [ADR Registry](../ADR-REGISTRY.yaml)
- [10_ARCHITECTURE_DECISIONS.md](../10_ARCHITECTURE_DECISIONS.md)
- [TI0-DECISOES.md](../../backlog-api/TI0-DECISOES.md) (decisão 20)
- [ADR-024](./ADR-024-world-monitor-rest-mvp-mcp-agents.md)
- [REALINHAMENTO_INTELIGENCIA_TERRITORIAL](../../backlog-api/REALINHAMENTO_INTELIGENCIA_TERRITORIAL.md)
- [ti-research-notes.md](../../handoff/inteligencia-territorial/ti-research-notes.md)
