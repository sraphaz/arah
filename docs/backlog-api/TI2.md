# TI-2: Intelligence Inbox

**Duração**: ~4 semanas  
**Prioridade**: 🔴 P0  
**Trilha**: Inteligência Territorial  
**Depende de**: [TI1](./TI1.md)  
**Ancora fases**: [FASE23](./FASE23.md) (assistência), moderação/WorkItem existente  
**Status**: ⏳ Pendente  
**Spec**: [TI-201](../specs/features/TI-201-signal-review.spec.yaml)

---

## Objetivo

Curadores enxergam e decidem: correlação geográfica, score de relevância explicável, impacto e revisão via **WorkItem** `SignalReview` (reuso da fila existente).

---

## Entregas

- Geo-correlação PostGIS (`signal_territory_matches`)
- Score determinístico com parcelas visíveis
- `TerritorialImpactAssessment`
- WorkItem tipo `SignalReview` + Inbox web admin
- Spec TI-201 / TI-202
- Job `ReviewSlaWatchdogJob` + `FreshnessSweepJob`

---

## Critérios de aceite

- [ ] Sinal relevante (≥ corte da política) abre WorkItem
- [ ] Decisão auditada (`approve` | `adjust` | `reject`); justificativa em reject/adjust
- [ ] Sinal stale não pode ser aprovado
- [ ] Idempotência: segunda decisão → 409
- [ ] Nada publica sem `PublicationDecision` (exceto allowlist — desligada por padrão)

---

## Referências

- [REALINHAMENTO_INTELIGENCIA_TERRITORIAL](./REALINHAMENTO_INTELIGENCIA_TERRITORIAL.md)
- [Handoff](../handoff/inteligencia-territorial/Inteligencia%20Territorial%20-%20Handoff%20de%20Desenvolvimento.dc.html)
- [TI3](./TI3.md)
