# ADR-024: World Monitor — REST no MVP, MCP para agentes

**Status**: accepted  
**Data**: 2026-08-15  
**Autor**: Solutions Architect (agente / humano)  
**Spec-Id**: TI-0  
**LikeC4 view**: containers

---

## Contexto

O World Monitor expõe **REST** (OpenAPI, ~35 serviços, batch até 20 ops) e **MCP** (~40 tools, Streamable HTTP). O handoff (decisão 1) recomenda REST para ingestão de produção e MCP para exploração por agentes. Misturar os dois no caminho crítico do MVP aumentaria superfície de erro (JSON-RPC, OAuth vs `wm_`, cobertura MCP ⊂ REST) sem ganho para jobs determinísticos de ingestão.

Plano API (decisão 19) fornece chaves manuais `wm_` adequadas ao adapter REST; o plano Pro oferece MCP via OAuth **sem** chave manual — insuficiente sozinho para ingestão de produção.

## Decisão

1. **MVP (TI-1…TI-3)**: ingestão, health probe, batch e quota usam **somente REST** (`api.worldmonitor.app`), tipada pelo snapshot OpenAPI em `docs/contracts/world-monitor/`.
2. **MCP**: reservado a agentes consultivos (ex.: `signal-scout`) para exploração assistida; **não** é dependência do caminho `Received → … → AwaitingReview`.
3. **Auth de produção**: header `X-WorldMonitor-Key` (plano API/Business); chave por instância, nunca por morador.
4. **CI**: fixtures gravadas; **zero** chamada real ao WM.

## Consequências

**Positivas**
- Contrato versionável, batch previsível, erros HTTP/quota alinhados ao ledger.
- Produção não depende de subset MCP nem de OAuth do plano Pro.
- Agentes mantêm flexibilidade tipada (`describe_tool` / `outputSchema`) sem contaminar o adapter.

**Negativas / trade-offs**
- Tools só-MCP (ex.: alguns cache-backed) ficam fora da ingestão até existir path REST equivalente ou decisão explícita.
- Dois clientes possíveis no longo prazo (REST + MCP) — só o REST é obrigatório no MVP.

## Alternativas consideradas

1. **MCP como canal único de ingestão** — rejeitado: tipagem e quotas menos estáveis para jobs; cobertura ⊂ REST; plano Pro sem chave manual inadequado à Infrastructure.
2. **REST + MCP em paralelo no mesmo job** — rejeitado: YAGNI; dobra custo de resiliência e testes.
3. **Scraping do dashboard Free** — rejeitado: frágil, fora de ToS/contrato, sem OpenAPI.

## Diagrama

```
Jobs / Adapter (MVP) ----REST/OpenAPI----> World Monitor API
signal-scout (opcional) ------MCP--------> worldmonitor.app/mcp
CI ---------------------------------------> fixtures/ (sem rede)
```

## Referências

- [ADR Registry](../ADR-REGISTRY.yaml)
- [10_ARCHITECTURE_DECISIONS.md](../10_ARCHITECTURE_DECISIONS.md)
- [TI0-DECISOES.md](../../backlog-api/TI0-DECISOES.md) (decisões 1 e 19)
- [ADR-023](./ADR-023-intelligence-provider-abstraction.md)
- [docs/contracts/world-monitor/](../../contracts/world-monitor/)
- [Integração World Monitor](../../handoff/inteligencia-territorial/Inteligencia%20Territorial%20-%20Integracao%20World%20Monitor.dc.html)
