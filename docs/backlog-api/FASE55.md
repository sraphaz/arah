# Fase 55: Monetização Open-Core — Billing, Carteira Aratá, Split e Payouts

**Duração**: 6 semanas (45 dias úteis)  
**Prioridade**: 🔴 P0  
**Onda**: S1 — Fundação de receita  
**Depende de**: FASE6, FASE7, FASE15, FASE54  
**Estimativa Total**: 180 horas  
**Status**: 🟡 Em progresso (quote/receipt + gate comercial v0)  
**Spec SDD**: [docs/specs/phases/FASE55-monetization.spec.yaml](../specs/phases/FASE55-monetization.spec.yaml)
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

| Método | Rota | Status FASE55 |
|--------|------|---------------|
| `GET` | `/api/v1/territories/{id}/plans` | ✅ v0 — planos comerciais (`MarketplaceAdvanced`) |
| `POST` | `/api/v1/subscriptions` | ✅ existente — assinatura por usuário (mock Stripe sem secret) |
| `POST` | `/api/v1/transactions/{id}/quote` | ✅ v0 — `checkout.Id` como `transactionId` |
| `GET` | `/api/v1/transactions/{id}/receipt` | ✅ v0 — requer `CheckoutStatus.Paid` |
| `POST` | `/api/v1/transactions/{id}/refund` | ✅ v0 — estorno idempotente; reverte fee/split (AC-55-6) |
| `GET` | `/api/v1/territories/{id}/payouts/consolidated` | ✅ v0 — payout consolidado por período (AC-55-5) |
| `POST` | `/merchants/{id}/subscription` | ⏳ — usar `/subscriptions` até alias merchant |
| `GET` | `/wallets/{id}` | ⏳ FASE22 + FASE55 |
| `GET` | `/implementers/{id}/payouts` | ⏳ FASE57 |
| `GET` | `/merchants/{id}/consumption` | ⏳ |

### Quote (resposta v0)

```json
{
  "transactionId": "uuid",
  "grossAmount": 100.00,
  "feeAmount": 10.00,
  "netToSeller": 90.00,
  "split": [
    { "recipient": "implementer", "amount": 4.00 },
    { "recipient": "territory_fund", "amount": 3.00 },
    { "recipient": "platform", "amount": 3.00 }
  ],
  "feeSplitRuleId": "uuid"
}
```

---

## Implementação v0 (código)

| Componente | Caminho |
|------------|---------|
| `FeeSplitRule` | `backend/Arah.Domain/Financial/FeeSplitRule.cs` |
| Quote / receipt | `TransactionQuoteService`, `TransactionsController` |
| Gate comercial | `CommercialStoreGateService` → `StoreService.SetPaymentsEnabled` |
| Seed split | `FeeSplitRuleBootstrapHostedService` (40/30/30 por território) |
| Plano LOJA (InMemory) | `InMemoryDataStore` — território B |

---

## Telas (app)

- **Ativar loja** — seleção de plano + PIX
- **Pagamento transparente** — checkout com split visível
- **Minha loja** — bloqueada sem assinatura ativa

---

## Critérios de aceite

- [x] Comércio não vende sem plano ativo — gate em `SetPaymentsEnabled` (v0)
- [x] Quote exibe taxa e split antes do PIX — `POST /transactions/{id}/quote` (v0)
- [x] Comprovante auditável com split aplicado — `GET /transactions/{id}/receipt` (v0, requer Paid)
- [x] Payout consolidado por período — `GET /territories/{id}/payouts/consolidated?from=&to=` (v0, read-model sobre checkouts pagos)
- [x] Estorno reverte fee e splits proporcionalmente — `POST /transactions/{id}/refund` (ledger append-only via `ReversePaidCheckoutAsync`; idempotente)
- [x] Morador usa feed/mapa/eventos sem cobrança — plano FREE inalterado

---

## Referências

- [FASE56](./FASE56.md) · [FASE15](./FASE15.md) · [REALINHAMENTO](./REALINHAMENTO_SUSTENTACAO_OPERACIONAL.md)
