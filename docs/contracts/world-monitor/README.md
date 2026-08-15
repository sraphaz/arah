# Contratos World Monitor (TI-0)

**Data**: 2026-08-15  
**Uso**: fixtures e snapshot OpenAPI para CI / adapter mock — **sem chamada real** ao World Monitor  
**ADRs**: [ADR-023](../architecture/adrs/ADR-023-intelligence-provider-abstraction.md) · [ADR-024](../architecture/adrs/ADR-024-world-monitor-rest-mvp-mcp-agents.md)

---

## Conteúdo

| Artefato | Papel |
|----------|--------|
| [openapi.snapshot.yaml](./openapi.snapshot.yaml) | Stub mínimo + **pin de versão** do contrato WM |
| [attribution.md](./attribution.md) | Texto de atribuição obrigatório |
| [fixtures/](./fixtures/) | Respostas JSON sintéticas para testes de contrato |

## Fixtures

| Arquivo | Endpoint / uso simulado |
|---------|-------------------------|
| `health.json` | `GET /api/health?compact=1` |
| `bootstrap.sample.json` | `GET /api/bootstrap` (amostra reduzida) |
| `natural-disasters.gdacs-socorro.json` | Desastres / GDACS — cenário Socorro-SP |
| `climate.sample.json` | Clima / amostra |
| `batch-execute.sample.json` | `POST /api/batch/v1/execute` |

Todos os payloads são **sintéticos** (plausíveis para o piloto Socorro-SP). Não copiam respostas live; não devem ser usados como verdade meteorológica.

## Regras

1. CI e testes de contrato leem estes arquivos — **nunca** `api.worldmonitor.app` em pipeline.
2. Drift check futuro (TI-701): comparar pin em `openapi.snapshot.yaml` com `worldmonitor.app/openapi.yaml` quando houver rede autorizada fora do CI unitário.
3. Atribuição: ver [attribution.md](./attribution.md).

## Referências

- [ti-research-notes.md](../handoff/inteligencia-territorial/ti-research-notes.md)
- [TI0.md](../backlog-api/TI0.md)
- [TI0-DECISOES.md](../backlog-api/TI0-DECISOES.md)
