# TI-3: Publicação territorial

**Duração**: ~5 semanas  
**Prioridade**: 🔴 P0  
**Trilha**: Inteligência Territorial  
**Depende de**: [TI2](./TI2.md)  
**Ancora fases**: [FASE24](./FASE24.md) (saúde/alertas), alertas/feed/mapa existentes  
**Status**: ⏳ Pendente

---

## Objetivo

O ciclo chega à comunidade: brief territorial, alerta enriquecido, mapa, feed e notificações por preferência — com **bloco de fonte obrigatório**.

---

## Entregas

- Editor de brief (`territorial_briefs`)
- `TerritorialAlert` + `alert_updates` (timeline append-only)
- Jornada BFF `intelligence` (alerts, detail, preferences)
- Camada de sinais no mapa Flutter + detalhe do alerta
- Notificações `kind: intelligence` respeitando preferências
- Specs TI-301 / TI-302 / TI-303
- Seed cenário Socorro-SP (passos 1–7)

---

## Critérios de aceite

- [ ] Publicar gera alerta + post ALERT + notificação por preferência
- [ ] Bloco de fonte em 100% dos alertas derivados de provedor
- [ ] Publicar sem validade → bloqueio; stale bloqueia publicação
- [ ] Timeline registra updates (não edita in-place)
- [ ] Cenário Socorro passos 1–7 end-to-end com seed
- [ ] Push FCM pendente: in-app + feed aceitos como entrega

---

## Referências

- [REALINHAMENTO_INTELIGENCIA_TERRITORIAL](./REALINHAMENTO_INTELIGENCIA_TERRITORIAL.md)
- [Protótipos](../handoff/inteligencia-territorial/Inteligencia%20Territorial%20-%20Prototipos.dc.html)
- [TI4](./TI4.md)
