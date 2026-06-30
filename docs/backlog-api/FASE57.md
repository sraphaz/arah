# Fase 57: Cockpit do Implementador (Web)

**Duração**: 5 semanas (35 dias úteis)  
**Prioridade**: 🔴 P0 (MVP na S1; completo na S2)  
**Onda**: S1–S2  
**Depende de**: FASE55, FASE53, FASE54  
**Estimativa Total**: 140 horas  
**Status**: ⏳ Pendente  
**Handoff**: [Cockpit do Implementador](../handoff/arquitetura-c4/Cockpit%20do%20Implementador%20Arah.dc.html)

---

## Objetivo

Painel web para implementadores gerenciarem territórios, comércios, receita/repasses, onboarding e operações — superfície principal de gestão da rede.

---

## Telas (8 módulos)

| Módulo | Função |
|--------|--------|
| Visão geral | KPIs, gráfico receita, próximo repasse, atividade |
| Territórios | Lista, métricas, abrir novo território |
| Comércios | Assinantes, plano, consumo, cobrança |
| Receita & repasses | Split por camada, histórico exportável |
| Transparência | Visão pública read-only |
| Onboarding | Convites, ativação de planos |
| Operações | Health, backups, incidentes, deploys |
| Formação | Trilha implementador, selo (S2+) |

---

## Endpoints `/implementers/me/*`

```
GET  /implementers/me/overview
GET  /implementers/me/territories
POST /implementers/me/merchants/invite
GET  /implementers/me/payouts
GET  /implementers/me/revenue?period=
GET  /implementers/me/instances/{id}/health
```

---

## Entregas por onda

### S1 (MVP)

- Visão geral, territórios, comércios, receita básica
- Solicitar nova instância/território

### S2 (completo)

- Onboarding, operações, transparência integrada
- Formação e mentoria

---

## Critérios de aceite

- [ ] Implementador autenticado vê KPIs da rede
- [ ] Convite de comércio dispara fluxo de assinatura no app
- [ ] Próximo repasse e histórico visíveis
- [ ] Health da instância refletido em tempo real
- [ ] Responsivo mobile-first (design system Arah)

---

## Referências

- [Programa de Implementadores](../handoff/arquitetura-c4/Programa%20de%20Implementadores%20Arah.dc.html)
- [FASE60](./FASE60.md)
