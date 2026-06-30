# Fase 55: Monetização Open-Core — Billing, Carteira Aratá, Split e Payouts

**Duração**: 6 semanas (45 dias úteis)  
**Prioridade**: 🔴 P0  
**Onda**: S1 — Fundação de receita  
**Depende de**: FASE6, FASE7, FASE15, FASE54  
**Estimativa Total**: 180 horas  
**Status**: ⏳ Pendente  
**Handoff**: [Adendo Monetização](../handoff/arquitetura-c4/Adendo%20de%20Monetizacao%20-%20Handoff%20Arah.dc.html)

---

## Objetivo

Implementar o modelo de receita **open-core**: morador nunca paga; comerciantes assinam plano comercial; transações geram taxa com split transparente; payouts automáticos via Carteira Aratá.

**Amplia**: FASE15 (reposicionada como billing comercial), FASE6/7 (checkout + payout), FASE22 (moeda/carteira).

---

## Princípios

1. Morador nunca paga pelo app comunitário
2. Núcleo aberto — receita por operação, não paywall
3. Transparência: taxa e split antes e depois do pagamento
4. Split prioriza implementador local e fundo do território

---

## Domínios novos/estendidos

| Entidade | Descrição |
|----------|-----------|
| `CommercialPlan` | Planos Loja/Pro por território |
| `Subscription` | Assinatura ativa do comércio |
| `Wallet (Aratá)` | Saldo, titular, payoutMethod |
| `Transaction` | grossAmount, feeAmount, splitId |
| `FeeSplitRule` | Percentuais versionados por revenueType |
| `Payout` | Repasse liquidado por período |
| `ConsumptionMeter` | IA, mídia, notificações medidos |

---

## Regras de billing

1. Toda transação chama `POST /transactions/{id}/quote` antes do pagamento
2. `feeAmount = grossAmount × feeRate` (banker's rounding)
3. Vendedor recebe `grossAmount − feeAmount`; fee dividido pela `FeeSplitRule`
4. Custos PSP/PIX fora do split, linha separada
5. Alteração de taxa exige nova `FeeSplitRule` com `effectiveFrom` — nunca edita vigente

---

## Endpoints principais

```
GET  /territories/{id}/plans
POST /merchants/{id}/subscription
POST /transactions/{id}/quote
GET  /transactions/{id}/receipt
GET  /wallets/{id}
GET  /implementers/{id}/payouts
GET  /merchants/{id}/consumption
```

---

## Telas (app)

- **Ativar loja** — seleção de plano + PIX
- **Pagamento transparente** — checkout com split visível
- **Minha loja** — bloqueada sem assinatura ativa

---

## Critérios de aceite

- [ ] Comércio não vende sem plano ativo
- [ ] Quote exibe taxa e split antes do PIX
- [ ] Comprovante auditável com split aplicado
- [ ] Payout mensal consolidado e liquidado
- [ ] Estorno reverte fee e splits proporcionalmente
- [ ] Morador usa feed/mapa/eventos sem cobrança

---

## Referências

- [FASE56](./FASE56.md) · [FASE15](./FASE15.md) · [REALINHAMENTO](./REALINHAMENTO_SUSTENTACAO_OPERACIONAL.md)
