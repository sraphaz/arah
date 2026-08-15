# TI-0 — Política modelo de publicação por território

**Data**: 2026-08-15  
**Trilha**: Inteligência Territorial  
**Status**: ✅ Modelo para copiar/adaptar por território (não é política jurídica vinculante)  
**Âncora de domínio**: `IntelligencePolicy` (handoff) · flags `INTELLIGENCE`, `INTELLIGENCE_AUTOPUBLISH_OBJECTIVE`

---

## Princípios

1. **Revisão humana por padrão** — nada chega a `Published` sem `PublicationDecision` com ator humano, salvo allowlist objetiva **explicitamente** ligada.
2. **Allowlist de autopublicação desligada por padrão** — categorias objetivas só autopublicam se o território configurar e a flag estiver on.
3. **Fonte sempre visível** — bloco de atribuição obrigatório (provedor + fonte original + timestamps). Ver [attribution.md](../contracts/world-monitor/attribution.md).
4. **Territory neutro** — esta política referencia `territoryId`; não adiciona campos sociais à entidade Territory.
5. **Sem ansiedade** — sinal sem relevância + utilidade + ação associada não vira notificação push.

---

## Padrão recomendado (piloto)

| Parâmetro | Valor modelo | Notas |
|-----------|--------------|--------|
| `reviewByDefault` | `true` | Invariante |
| `autopublishAllowlist` | `[]` (vazia) | Off por padrão |
| `minSeverityNotify` | `alerta` | Informativo/atenção: inbox, sem push em massa |
| `reviewSlaMinutes` | `60` | Meta piloto; watchdog notifica curadores |
| Categorias MVP sugeridas | `clima`, `desastre` (+ opcional `infraestrutura`) | Demais off até operação estável |
| Categorias nunca autopublicam | `deslocamento`, `saúde`, `news`/oportunidade sensível | Mesmo se alguém tentar allowlist — rejeitar na validação |

---

## Fluxo de decisão

```
Sinal Verified
    │
    ├─ categoria ∈ autopublishAllowlist
    │     E flag INTELLIGENCE_AUTOPUBLISH_OBJECTIVE on
    │     E severidade/confiança mínimas ok
    │        → Published com selo "publicação automática"
    │
    └─ caso contrário
           → AwaitingReview (WorkItem SignalReview)
                 → humano approve | adjust | reject
                 → PublicationDecision gravada
```

Agentes **nunca** possuem credencial de publicação. Allowlist é política do território, não decisão de agente.

---

## Atribuição (obrigatória em toda superfície)

Texto canônico (pt-BR):

> dados via World Monitor · fonte: {SourceName}

Exemplos: `dados via World Monitor · fonte: GDACS`, `… · fonte: INMET` (quando a origem for repassada pelo WM).

- Sem logo / marca visual do WM sem permissão (parecer jurídico pendente).
- Incluir em: detalhe do alerta, feed, mapa, brief, e preferencialmente em push (validação jurídica pendente — pergunta 2 do parecer).

---

## Esboço LGPD (não substitui parecer)

| Tema | Posição modelo |
|------|----------------|
| Dados ao provedor | Só categoria, região/bbox e janela temporal — **nunca** identidade ou localização de pessoa |
| Alertas de proteção à vida / patrimônio comunitário | Interesse legítimo (a validar no [parecer](../legal/TI0-PARECER-WORLD-MONITOR.md)) |
| Confirmação / evidência de morador | Consentimento + opt-in; EXIF-GPS opcional / strip |
| Retenção sinais não publicados | 90 dias |
| Retenção publicados / memória territorial | Permanente **sem** dados pessoais embutidos |
| Direitos do titular | Exclusão da própria verificação/evidência; exportação via fluxo LGPD da plataforma |

Detalhe e ameaças: [Agentes e Salvaguardas](../handoff/inteligencia-territorial/Inteligencia%20Territorial%20-%20Agentes%20e%20Salvaguardas.dc.html).

---

## Mudança de política

- Alterações **não retroagem** sobre `PublicationDecision` já gravadas.
- Toda mudança é auditável (quem, quando, diff de allowlist/categorias).
- Desligar `INTELLIGENCE` no território interrompe notificação e nova publicação; sinais já publicados permanecem com histórico.

---

## Referências

- [TI0.md](./TI0.md) · [TI0-DECISOES.md](./TI0-DECISOES.md)
- [ADR-023](../architecture/adrs/ADR-023-intelligence-provider-abstraction.md)
- [TI0-PARECER-WORLD-MONITOR.md](../legal/TI0-PARECER-WORLD-MONITOR.md)
