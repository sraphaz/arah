# TI-1: Fundação — domínio + adapter World Monitor

**Duração**: ~4 semanas  
**Prioridade**: 🔴 P0  
**Trilha**: Inteligência Territorial  
**Depende de**: TI-0 (fixtures; jurídico pode seguir em paralelo)  
**Ancora fases**: [FASE44](./FASE44.md) (integrações), baseline [FASE4](./FASE4.md)  
**Status**: ⏳ Pendente  
**Handoff**: [Handoff de Desenvolvimento](../handoff/inteligencia-territorial/Inteligencia%20Territorial%20-%20Handoff%20de%20Desenvolvimento.dc.html) · [Integração WM](../handoff/inteligencia-territorial/Inteligencia%20Territorial%20-%20Integracao%20World%20Monitor.dc.html)

---

## Objetivo

Domínio + adapter + persistência **sem UI**: a plataforma passa a receber, normalizar e deduplicar sinais externos. World Monitor é o 1º `SignalProvider` (só em Infrastructure).

---

## Entregas

- Namespace `Domain` / `Application` Intelligence (ports agnósticos de provedor)
- Adapter REST `Infrastructure/Intelligence/Providers/WorldMonitor/`
- Migrations: `signal_providers`, `external_signals`, `intelligence_policies`, `provider_quota_ledger`
- Flags: `INTELLIGENCE`, `INTELLIGENCE_CATEGORY_*`
- Quota ledger + health probe + `emergencyOff`
- Specs TI-101 / TI-102 / TI-103
- Jobs: `IngestSignalsJob` (dry-run), `ProviderHealthProbeJob`, `QuotaLedgerRollupJob`

---

## Critérios de aceite

- [ ] Sinal do mock persiste normalizado e deduplicado (geometria válida, hash estável)
- [ ] Quota conta; bloqueio preventivo no teto; alarme 80%
- [ ] `emergencyOff` corta ingestão em menos de 1 min
- [ ] Zero chamada real ao WM em CI
- [ ] Territory permanece sem lógica social/de inteligência

---

## Referências

- [REALINHAMENTO_INTELIGENCIA_TERRITORIAL](./REALINHAMENTO_INTELIGENCIA_TERRITORIAL.md)
- [TI0](./TI0.md) · [TI2](./TI2.md) · ADR-TI-01/02 (handoff)
