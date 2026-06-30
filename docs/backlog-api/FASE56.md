# Fase 56: Transparência e Governança de Taxas

**Duração**: 3 semanas (21 dias úteis)  
**Prioridade**: 🔴 P0  
**Onda**: S2 — Transparência & gestão  
**Depende de**: FASE14, FASE55, FASE24/25 (métricas)  
**Estimativa Total**: 84 horas  
**Status**: ⏳ Pendente  
**Handoff**: [Adendo Monetização A6, A8](../handoff/arquitetura-c4/Adendo%20de%20Monetizacao%20-%20Handoff%20Arah.dc.html)

---

## Objetivo

Painel público de receita do território e processo de governança para alterar taxas/splits — confiança comunitária e conformidade com princípio de transparência.

**Amplia**: FASE14 (governança), FASE24/25 (métricas → transparência).

---

## Painel de transparência (público)

Acessível a qualquer morador:

- Receita total do território (tempo real)
- Repasses por destinatário (implementador, fundo, plataforma)
- Comércios ativos e volume transacionado
- Histórico de regras de taxa vigentes
- Exportação para conselho (CSV/PDF)

```
GET /territories/{id}/revenue
GET /territories/{id}/revenue/export
```

---

## Governança de taxas

Fluxo (estende FASE14):

1. **Proposta** — curador/conselho cria `fee-proposal` com prévia de impacto
2. **Votação** — moradores habilitados; quórum configurável
3. **Vigência futura** — `FeeSplitRule` com `effectiveFrom`; nunca retroage
4. **Transparência** — mudança no painel com autor, data, resultado

```
POST /territories/{id}/fee-proposals
POST /territories/{id}/fee-proposals/{id}/vote
```

---

## Telas

- **Painel de transparência** (novo) — qualquer morador
- **Proposta de taxa** (altera governança existente)

---

## Critérios de aceite

- [ ] Morador vê receita e repasses sem login especial
- [ ] Exportação funcional para conselho
- [ ] Proposta aprovada gera regra futura; transações antigas intactas
- [ ] Painel mostra regra usada por período
- [ ] Rejeição arquiva proposta com auditoria

---

## Referências

- [FASE14](./FASE14.md) · [FASE55](./FASE55.md)
