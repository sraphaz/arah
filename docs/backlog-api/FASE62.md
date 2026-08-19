# Fase 62: Conformidade fiscal & KYC comercial (Brasil) — packs por território

**Duração**: 6–8 semanas (fatiável 62.0 → 62.a → 62.b → 62.c)  
**Prioridade**: 🔴 P0 (62.0 + 62.a) / 🟡 P1 (62.b–c)  
**Onda**: S1 — Fundação de receita (paralela a FASE55–57)  
**Depende de**: FASE55 (billing/split), FASE54 (piloto/PSP), parecer regulatório (gate go-live)  
**Status**: 🟡 Em progresso (62.0 API ✅ · 62.a–c pendente)  
**Análise**: [ANALISE_FISCAL_BR.md](../compliance/ANALISE_FISCAL_BR.md)  
**Produto / jornadas**: [PACOTES_FISCAIS_POR_TERRITORIO.md](../compliance/PACOTES_FISCAIS_POR_TERRITORIO.md)  
**Spec SDD**: [FASE62-fiscal-kyc-br.spec.yaml](../specs/phases/FASE62-fiscal-kyc-br.spec.yaml)  

---

## Objetivo

Viabilizar comércio local **formalizável** no Brasil com um modelo **pluggável por território**:

1. O **implementador** escolhe e ativa o **pacote fiscal** (MVP: só `FiscalPack.Brazil.v1`) e os **meios de pagamento** daquele território.  
2. Os **comerciantes** completam KYC/fiscal em **Minha loja**.  
3. **Territory** permanece geográfico e neutro — config fiscal/pagamento em entidades escopadas por `territoryId`.

Sem confundir **taxa open-core** com **tributo**; sem misturar FASE61 (capital/investimento) com NF de venda.

---

## Fatias

| Fatia | Escopo | Prioridade |
|-------|--------|------------|
| **62.0** | Catálogo de packs + `TerritoryFiscalPackBinding` + `TerritoryPaymentMethodsConfig` + **API** (UI cockpit em FASE57) | P0 |
| **62.a** | `MerchantFiscalProfile` (CNPJ/CPF-MEI, regime, município ISS); KYC; PixKey; gate de venda **quando** pack BR ativo | P0 |
| **62.b** | Emissão/armazenamento NFS-e MVP (serviços); link no comprovante do pedido | P1 |
| **62.c** | Retenção documental fiscal vs LGPD; export contábil período | P1 |

---

## Domínio

### Configuração do território (62.0)

| Entidade | Campos principais |
|----------|-------------------|
| `FiscalPackDefinition` | id (`brazil.v1`), country, capabilities[] |
| `TerritoryFiscalPackBinding` | territoryId, packId, status (Off\|Active), activatedBy, activatedAt, params (municipalityIbge…) |
| `TerritoryPaymentMethodsConfig` | territoryId, methods[] (Pix, …), pspProvider, updatedAt |

### Comerciante (62.a)

| Entidade | Campos principais |
|----------|-------------------|
| `MerchantFiscalProfile` | storeId, taxIdType (CPF\|CNPJ), taxId, legalName, cnae?, municipalityIbge, taxRegime, kycStatus, verifiedAt |
| Extensão `Store` | `PixKey?` (sem dados em Territory) |

**Gate** (se binding BR `Active`): publicar/vender exige `kycStatus = Approved` **e** assinatura comercial (`CommercialStoreGate`).

---

## Jornadas (resumo)

| Papel | Superfície | Ação |
|-------|------------|------|
| Implementador | Cockpit → Comércio & fiscal | Selecionar pack BR + ativar PIX/PSP |
| Comerciante | App → Minha loja | Preencher fiscal / PixKey / ver status e NFs |
| Comprador | Checkout / pedidos | Meios ativos + comprovante (+ NF se houver) |
| Perfil `/me` | Só alerta/atalho | Não hospeda o formulário fiscal completo |

Detalhe: [PACOTES_FISCAIS_POR_TERRITORIO.md](../compliance/PACOTES_FISCAIS_POR_TERRITORIO.md).

---

## Fora de escopo (P2 / depois)

- Outros packs nacionais além de BR  
- ICMS / NFC-e para bens em escala  
- DAS MEI automático  
- Produto de “investimento” acoplado ao pack (→ FASE61)  
- Federação multi-município ISS (FASE58–59)  

---

## Critérios de aceite (rascunho)

### 62.0
- [x] Implementador ativa `brazil.v1` por território sem mutar entidade Territory  
- [x] Meios de pagamento do território listáveis pela API de checkout  
- [x] Território sem pack: comportamento documentado no spec (comércio legado vs bloqueio)  

### 62.0 — endpoints (API)

| Método | Rota | Auth |
|--------|------|------|
| `GET` | `/api/v1/fiscal-packs` | público |
| `GET` | `/api/v1/territories/{id}/fiscal-pack` | JWT |
| `PUT` | `/api/v1/territories/{id}/fiscal-pack` | SystemAdmin |
| `GET` | `/api/v1/territories/{id}/payment-methods` | público (checkout) |
| `PUT` | `/api/v1/territories/{id}/payment-methods` | SystemAdmin |

UI cockpit (FASE57) consome estes endpoints — fora do slice 62.0.
### 62.a
- [ ] Com pack ativo: comerciante completa CPF-MEI/CNPJ + município; validação de dígitos  
- [ ] KYC pendente → não vende / não publica items  
- [ ] PixKey só acessível ao dono da store  
- [ ] Spec + testes HTTP; sync-docs  

---

## Referências

- [PACOTES_FISCAIS_POR_TERRITORIO.md](../compliance/PACOTES_FISCAIS_POR_TERRITORIO.md)  
- [ANALISE_FISCAL_BR.md](../compliance/ANALISE_FISCAL_BR.md)  
- [FASE55.md](./FASE55.md) · [FASE56.md](./FASE56.md) · [FASE57.md](./FASE57.md) · [FASE61.md](./FASE61.md)  
- [REALINHAMENTO_SUSTENTACAO_OPERACIONAL.md](./REALINHAMENTO_SUSTENTACAO_OPERACIONAL.md)  
- [LGPD_COMPLIANCE.md](../LGPD_COMPLIANCE.md)  
