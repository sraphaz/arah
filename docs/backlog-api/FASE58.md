# Fase 58: Operação Multi-Instância — Deploy, Health e Backups

**Duração**: 4 semanas (28 dias úteis)  
**Prioridade**: 🟡 P1  
**Onda**: S1–S2  
**Depende de**: FASE52, FASE53, FASE54  
**Estimativa Total**: 112 horas  
**Status**: ⏳ Pendente  
**Handoff**: [Anexo O3, O8, O9](../handoff/arquitetura-c4/Anexo%20Handoff%20-%20Operacao%20Instancias%20e%20Federacao.dc.html)

---

## Objetivo

Ciclo de vida operacional das instâncias: deploy com migração, rollback automático, health contínuo, backups verificados e alertas no cockpit.

---

## Pipeline: atualizar instância

1. Core publica release stable
2. Instância detecta nova versão
3. **Gate**: backup verificado obrigatório
4. Aplicar migração de schema
5. Smoke test
6. Rollback automático se falha

```
POST /instances/{id}/deploy
POST /instances/{id}/backups
GET  /instances/{id}/health
```

---

## SLA (instância gerenciada)

| Métrica | Alvo |
|---------|------|
| Uptime | 99,9% |
| RPO backup | ≤ 15 min |
| Rollback deploy | Automático |
| Heartbeat Core | 30s |

---

## Casos de borda

- **Core indisponível** — instância autônoma; fila ledger
- **Deploy falha** — rollback; território sem downtime
- **Backup atrasado** — alerta P1; bloqueia novo deploy
- **Instância soberana offline** — local OK; federação pausa

---

## Entidades

- `Deployment` — releaseId, status, migratedAt, rolledBack
- `BackupSnapshot` — takenAt, verified, retentionDays
- `IncidentEvent` — severity, type, resolvedAt

---

## Critérios de aceite

- [ ] Deploy com migração testado; rollback comprovado
- [ ] Backup diário verificado antes de deploy prod
- [ ] Alertas visíveis no cockpit
- [ ] Território nunca fora por update falho
- [ ] Observabilidade: logs, métricas, traces agregados

---

## Referências

- [FASE59](./FASE59.md) · [Plano Operacional P4](../handoff/arquitetura-c4/Plano%20Operacional%20e%20Pipelines%20-%20Arah.dc.html)
