# TI-0 — Parecer World Monitor (rascunho de perguntas)

**Data**: 2026-08-15  
**Status geral**: ⏳ **pending** (sem parecer definitivo)  
**Gate**: parecer concluído **antes da Fase C** (piloto com provedor real)  
**Efeito em TI-1/TI-2**: ✅ **permitidos em mock/fixtures** — não bloqueados por este pending

---

## Escopo

Arquivar explicitamente as quatro perguntas jurídicas do handoff ([Agentes e Salvaguardas](../handoff/inteligencia-territorial/Inteligencia%20Territorial%20-%20Agentes%20e%20Salvaguardas.dc.html)), com status **pending**, para satisfazer o critério de aceite de TI-0 (“referência explícita se pendente”).

Este arquivo **não** é aconselhamento jurídico. Respostas devem ser preenchidas por jurista / compliance antes do consumo real da API.

---

## Perguntas

### 1 — ToS da API × multi-território sob uma assinatura

**Pergunta**: Os Termos de Serviço da API do World Monitor permitem redistribuir sinais processados a múltiplos territórios/comunidades sob uma única assinatura (plano API) por instância Arah?

| Campo | Valor |
|-------|--------|
| Status | ⏳ pending |
| Hipótese de produto | Uma chave `wm_` por instância; vários `territoryId` na mesma operação |
| Bloqueia TI-1 mock? | Não |
| Bloqueia Fase C? | Sim, até resposta |

---

### 2 — Atribuição e marca (incl. push)

**Pergunta**: O texto de atribuição proposto (`dados via World Monitor · fonte: GDACS`) satisfaz marca e atribuição sem permissão adicional — inclusive em notificações push?

| Campo | Valor |
|-------|--------|
| Status | ⏳ pending |
| Posição técnica provisória | Texto sem logo; ver [attribution.md](../contracts/world-monitor/attribution.md) |
| Bloqueia TI-1 mock? | Não |
| Bloqueia Fase C? | Sim para superfícies públicas com marca; push pode exigir redação ajustada |

---

### 3 — Licenças das fontes originais agregadas

**Pergunta**: Quais fontes agregadas (GDACS, USGS, ReliefWeb, …) impõem termos próprios de reuso que precisam aparecer no bloco de fonte?

| Campo | Valor |
|-------|--------|
| Status | ⏳ pending |
| Posição técnica provisória | Preservar `sourceName` original no bloco de fonte |
| Bloqueia TI-1 mock? | Não |
| Bloqueia Fase C? | Sim para categorias ligadas a fontes com restrição |

---

### 4 — LGPD / retenção em associação comunitária

**Pergunta**: O enquadramento LGPD proposto (interesse legítimo para alertas; consentimento para participação; retenção 90 dias / permanente sem PII) sustenta-se para uma instância operada por associação comunitária?

| Campo | Valor |
|-------|--------|
| Status | ⏳ pending |
| Esboço de produto | [TI0-POLITICA-PUBLICACAO-MODELO.md](../backlog-api/TI0-POLITICA-PUBLICACAO-MODELO.md) § LGPD |
| Bloqueia TI-1 mock? | Não |
| Bloqueia Fase C? | Sim (RIPD / base legal documentada) |

---

## Decisões de engenharia já afirmadas (contexto)

Não substituem o parecer, mas reduzem risco enquanto pending:

- Decisão 20: consumo por API + atribuição; **sem** embed AGPL — [TI0-DECISOES.md](../backlog-api/TI0-DECISOES.md)
- Decisão 19: plano API orçado; contratação na Fase C
- AGPL rege o **código** do WM, não automaticamente os dados servidos pela API (ToS à parte)

---

## Próximos passos (humano / jurídico)

1. Responder as quatro perguntas neste arquivo (status → `answered`, com data e responsável).
2. Atualizar `attribution.md` se a redação aprovada divergir.
3. Liberar Fase C / chave real apenas com status geral ≠ pending.

---

## Referências

- [TI0.md](../backlog-api/TI0.md)
- [ti-research-notes.md](../handoff/inteligencia-territorial/ti-research-notes.md)
- [ADR-023](../architecture/adrs/ADR-023-intelligence-provider-abstraction.md) · [ADR-024](../architecture/adrs/ADR-024-world-monitor-rest-mvp-mcp-agents.md)
