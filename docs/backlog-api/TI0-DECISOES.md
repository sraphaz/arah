# TI-0 — Afirmação das decisões 1, 19 e 20

**Data**: 2026-08-15  
**Trilha**: Inteligência Territorial  
**Fonte**: [TI0.md](./TI0.md) · [Handoff · Decisões (20)](../handoff/inteligencia-territorial/Inteligencia%20Territorial%20-%20Handoff%20de%20Desenvolvimento.dc.html) · [ti-research-notes.md](../handoff/inteligencia-territorial/ti-research-notes.md)  
**Status**: ✅ Confirmadas por escrito (TI-0)

---

## Objetivo

Confirmar por escrito as decisões **1**, **19** e **20** do handoff de Inteligência Territorial, eliminando ambiguidade antes de TI-1 (domínio + adapter).

---

## Decisão 1 — REST no MVP (MCP para agentes)

| Campo | Valor |
|-------|--------|
| Tema | Canal de ingestão no MVP |
| Recomendação do handoff | **REST** |
| Alternativa | MCP como canal principal |
| Status TI-0 | ✅ **Afirmada** |

**Afirmação**: a ingestão determinística do MVP usa a API **REST** do World Monitor (`OpenAPI`, batch `POST /api/batch/v1/execute`, quotas e erros documentados). O **MCP** (`worldmonitor.app/mcp`) fica reservado à exploração assistida por agentes (ex.: `signal-scout`), onde flexibilidade vale mais que tipagem rígida.

**Revisão**: quando agentes operarem ingestão contínua (não no escopo do MVP TI-1…TI-3).

**ADR**: [ADR-024](../architecture/adrs/ADR-024-world-monitor-rest-mvp-mcp-agents.md).

---

## Decisão 19 — Plano API (preço e quota)

| Campo | Valor |
|-------|--------|
| Tema | Custos de provedor / plano comercial |
| Recomendação do handoff | Plano **API** US$ 99,99/mês por operação (instância) |
| Alternativa | API Business US$ 249,99/mês |
| Status TI-0 | ✅ **Afirmada** (orçamento); contratação efetiva na Fase C do rollout |

**Afirmação**: o orçamento do MVP assume o plano **API** do World Monitor (**≈ US$ 99,99/mês**, **1.000 req/dia**, chaves `wm_` self-serve, até 5 webhooks). Com batch e cache por freshness, isso comporta ~15 categorias × poucos territórios em cadência conservadora. Ledger de quota alarma em **80%**; upgrade para Business só com demanda medida (> 80% por 2 semanas).

**Assinatura**: a contratação/pagamento do plano pode ser **adiada até a Fase C** (piloto com provedor real). Fases A/B e CI usam **fixtures** — zero chamada real. Esta decisão documenta o plano alvo; não exige assinatura imediata para desbloquear TI-1/TI-2 em mock.

**Revisão**: quota sustentada acima de 80% por duas semanas, ou mudança de catálogo/preço do WM.

---

## Decisão 20 — Consumo por API + atribuição (sem embed AGPL)

| Campo | Valor |
|-------|--------|
| Tema | Licenciamento / modo de consumo |
| Recomendação do handoff | Consumo por API + atribuição visível |
| Alternativa | Fork / embed de código AGPL do WM |
| Status TI-0 | ✅ **Afirmada** |

**Afirmação**:

1. O Arah consome o World Monitor **somente** via API/MCP públicos — **zero** código do repositório AGPL-3.0 do WM no backend da instância.
2. Cliente HTTP próprio no adapter (SDKs MIT oficiais são opcionais; não são obrigatórios no MVP).
3. Atribuição obrigatória no bloco de fonte: texto do tipo `dados via World Monitor · fonte: <origem>` (sem logo/marca visual sem permissão). Ver [attribution.md](../contracts/world-monitor/attribution.md).
4. ToS da API, marca e licenças das fontes originais (GDACS, USGS, etc.) ficam no parecer jurídico — [TI0-PARECER-WORLD-MONITOR.md](../legal/TI0-PARECER-WORLD-MONITOR.md) (status **pending**; TI-1/2 seguem em mock).

**Revisão**: parecer jurídico (M19 / Fase C); alternativa comercial (licença Enterprise/white-label) se ToS impedir multi-território sob uma chave.

**ADR**: [ADR-023](../architecture/adrs/ADR-023-intelligence-provider-abstraction.md) (fronteira de provedor) reforça que o domínio não conhece o WM.

---

## Rastreio de aceite (TI-0)

| Critério | Resultado |
|----------|-----------|
| Decisões 1, 19 e 20 confirmadas ou revisadas por escrito | ✅ Este documento |
| Fixtures sem chamada real ao WM | ✅ [docs/contracts/world-monitor/](../contracts/world-monitor/) |
| Parecer jurídico arquivado ou referência explícita se pendente | ✅ [TI0-PARECER-WORLD-MONITOR.md](../legal/TI0-PARECER-WORLD-MONITOR.md) (pending) |

---

## Referências

- [REALINHAMENTO_INTELIGENCIA_TERRITORIAL](./REALINHAMENTO_INTELIGENCIA_TERRITORIAL.md)
- [TI0-POLITICA-PUBLICACAO-MODELO.md](./TI0-POLITICA-PUBLICACAO-MODELO.md)
- [ADR Registry](../architecture/ADR-REGISTRY.yaml)
