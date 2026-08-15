# TI-0: Pesquisa e contratos (Inteligência Territorial)

**Duração**: ~2 semanas  
**Prioridade**: 🔴 P0 (trilha TI)  
**Trilha**: Inteligência Territorial  
**Depende de**: —  
**Status**: 🟡 Docs/contratos entregues — parecer jurídico pending (TI-1 mock liberado)  
**Handoff**: [Roadmap e Backlog](../handoff/inteligencia-territorial/Inteligencia%20Territorial%20-%20Roadmap%20e%20Backlog.dc.html) · [Agentes e Salvaguardas](../handoff/inteligencia-territorial/Inteligencia%20Territorial%20-%20Agentes%20e%20Salvaguardas.dc.html)

---

## Objetivo

Eliminar incertezas que travam o restante: plano/ToS do World Monitor, categorias do piloto, política de publicação modelo e ADRs TI-01/02.

---

## Entregas

- [x] Parecer jurídico **arquivado como pending** (4 perguntas) — [TI0-PARECER-WORLD-MONITOR.md](../legal/TI0-PARECER-WORLD-MONITOR.md); TI-1/2 seguem em mock
- [x] Decisão de plano API documentada (contratação efetiva na Fase C) — [TI0-DECISOES.md](./TI0-DECISOES.md) §19
- [x] Fixtures OpenAPI/JSON gravadas para CI — [docs/contracts/world-monitor/](../contracts/world-monitor/) (wiring do job CI = TI-701 / TI-1)
- [x] Política modelo de publicação por território — [TI0-POLITICA-PUBLICACAO-MODELO.md](./TI0-POLITICA-PUBLICACAO-MODELO.md)
- [x] ADRs aceitos: provider abstraction; REST no MVP / MCP para agentes — [ADR-023](../architecture/adrs/ADR-023-intelligence-provider-abstraction.md) · [ADR-024](../architecture/adrs/ADR-024-world-monitor-rest-mvp-mcp-agents.md)

---

## Critérios de aceite

- [x] Decisões 1, 19 e 20 do handoff confirmadas ou revisadas por escrito — [TI0-DECISOES.md](./TI0-DECISOES.md)
- [x] Fixtures disponíveis sem chamada real ao WM — [docs/contracts/world-monitor/fixtures/](../contracts/world-monitor/fixtures/) (execução no pipeline CI na TI-701)
- [x] Parecer jurídico arquivado em `docs/` ou referência explícita se pendente (TI-1/2 podem seguir em mock) — [TI0-PARECER-WORLD-MONITOR.md](../legal/TI0-PARECER-WORLD-MONITOR.md)

---

## Artefatos deste incremento

| Artefato | Path |
|----------|------|
| Decisões 1 / 19 / 20 | [TI0-DECISOES.md](./TI0-DECISOES.md) |
| Política modelo | [TI0-POLITICA-PUBLICACAO-MODELO.md](./TI0-POLITICA-PUBLICACAO-MODELO.md) |
| Parecer (pending) | [docs/legal/TI0-PARECER-WORLD-MONITOR.md](../legal/TI0-PARECER-WORLD-MONITOR.md) |
| Contratos / fixtures | [docs/contracts/world-monitor/](../contracts/world-monitor/) |
| ADR-023 / ADR-024 | [adrs/](../architecture/adrs/) |

---

## Referências

- [REALINHAMENTO_INTELIGENCIA_TERRITORIAL](./REALINHAMENTO_INTELIGENCIA_TERRITORIAL.md)
- [TI1](./TI1.md) · [Notas de pesquisa](../handoff/inteligencia-territorial/ti-research-notes.md)
- [Handoff TI README](../handoff/inteligencia-territorial/README.md)
