# Fase 61: Capital Territorial, Patrocínios e Doações

**Duração**: 3 semanas (21 dias úteis)  
**Prioridade**: 🟢 P2  
**Onda**: S3–S4  
**Depende de**: FASE55, FASE56  
**Estimativa Total**: 84 horas  
**Status**: ⏳ Pendente  
**Handoff**: [Adendo A9](../handoff/arquitetura-c4/Adendo%20de%20Monetizacao%20-%20Handoff%20Arah.dc.html)

---

## Objetivo

Camada de capital comunitário: patrocínios, doações à plataforma/território, fundo territorial com aportes e revenue-share — sustentando expansão além da taxa transacional.

---

## Modelos

### Patrocínio & doação

- Escopo: plataforma ou território específico
- Recorrência opcional
- `displayConsent` — transparência ≠ exposição forçada
- Ledger auditável

### Fundo territorial

- Veículo comunitário para infra, bolsas, novos territórios
- Parte do split pode alimentar o fundo (FeeSplitRule)
- Decisões de uso via governança (FASE14/56)

### Investimento-anjo

- Revenue-share com implementador local
- Prestação de contas no painel público

---

## Domínios

- `Sponsorship` — sponsorId, scope, amount, recurring, displayConsent
- `TerritoryFund` — territoryId, balance, contributors[], model

---

## Endpoints

```
POST /sponsorships
GET  /territories/{id}/fund
POST /territories/{id}/fund/contributions
```

---

## Telas

- **Patrocínio & doação** (app) — escopo, valor, recorrência
- **Fundo territorial** (transparência + cockpit)

---

## Critérios de aceite

- [ ] Doação registrada no ledger com comprovante
- [ ] Patrocínio sem consentimento não exibe marca publicamente
- [ ] Saldo do fundo visível no painel de transparência
- [ ] Uso do fundo registrado com decisão de governança
- [ ] Revenue-share de anjo rastreável por território

---

## Referências

- [FASE55](./FASE55.md) · [FASE56](./FASE56.md)
