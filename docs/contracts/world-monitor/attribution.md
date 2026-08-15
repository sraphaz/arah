# Atribuição — World Monitor

**Status**: provisório (validação jurídica **pending** — pergunta 2 em [TI0-PARECER-WORLD-MONITOR.md](../../legal/TI0-PARECER-WORLD-MONITOR.md))  
**Decisão de produto**: [TI0-DECISOES.md](../../backlog-api/TI0-DECISOES.md) (decisão 20)

---

## Texto canônico (pt-BR)

```
dados via World Monitor · fonte: {SourceName}
```

Exemplos:

- `dados via World Monitor · fonte: GDACS`
- `dados via World Monitor · fonte: USGS`
- `dados via World Monitor · fonte: INMET`

Quando houver URL de atribuição do provedor/fonte, expor em campo separado (`attributionUrl`), não substituir o texto.

---

## Regras de UI

| Permitido | Evitar até parecer |
|-----------|-------------------|
| Texto canônico no bloco de fonte | Logo / wordmark do World Monitor |
| Nome da fonte original (`GDACS`, etc.) | White-label ou “powered by” gráfico |
| Timestamps `publishedAt` / `fetchedAt` | Omitir atribuição em push sem revisão jurídica |

---

## Campos sugeridos no contrato Arah

```json
{
  "source": {
    "providerName": "World Monitor",
    "sourceName": "GDACS",
    "publishedAt": "2026-01-14T09:12:00Z",
    "fetchedAt": "2026-01-14T09:15:00Z",
    "attributionText": "dados via World Monitor · fonte: GDACS",
    "attributionUrl": null
  }
}
```

---

## Referências

- [TI0-POLITICA-PUBLICACAO-MODELO.md](../../backlog-api/TI0-POLITICA-PUBLICACAO-MODELO.md)
- Handoff: Agentes e Salvaguardas · Integração World Monitor
