# Fase 62: Conformidade fiscal & KYC comercial (Brasil)

**Duração**: 6–8 semanas (fatiável 62.a → 62.b → 62.c)  
**Prioridade**: 🔴 P0 (62.a) / 🟡 P1 (62.b–c)  
**Onda**: S1 — Fundação de receita (paralela a FASE55–57)  
**Depende de**: FASE55 (billing/split), FASE54 (piloto/PSP), parecer regulatório (gate go-live)  
**Status**: ⏳ Proposta (análise 2026-08-15)  
**Análise**: [ANALISE_FISCAL_BR.md](../compliance/ANALISE_FISCAL_BR.md)  
**Spec SDD**: _pendente — `FASE62-fiscal-kyc-br`_

---

## Objetivo

Viabilizar comércio local **formalizável** no Brasil: cadastro fiscal do comerciante, KYC comercial, vínculo a PIX/payout, e caminho para **NFS-e** — sem confundir **taxa open-core** com **tributo**, e sem colocar lógica fiscal em `Territory`.

---

## Fatias

| Fatia | Escopo | Prioridade |
|-------|--------|------------|
| **62.a** | `MerchantFiscalProfile` (CNPJ/CPF-MEI, regime, município ISS); KYC status; gate de venda; PixKey na store | P0 |
| **62.b** | Emissão/armazenamento NFS-e MVP (serviços); link no receipt da plataforma | P1 |
| **62.c** | Retenção documental fiscal vs LGPD; export contábil período | P1 |

---

## Domínio (62.a)

| Entidade | Campos principais |
|----------|-------------------|
| `MerchantFiscalProfile` | storeId, taxIdType (CPF\|CNPJ), taxId, legalName, cnae?, municipalityIbge, taxRegime (MEI\|Simples\|…), kycStatus, verifiedAt |
| Extensão `Store` | `PixKey?`, `fiscalProfileId?` (sem dados sociais de Territory) |

**Gate**: publicar/vender items exige `kycStatus = Approved` **e** assinatura comercial ativa (compõe `CommercialStoreGate`).

---

## Fora de escopo (P2 / depois)

- ICMS / NFC-e para bens físicos em escala  
- DAS MEI automático  
- Federação multi-município ISS (FASE58–59)  
- Doações (FASE61) como NF de venda  

---

## Critérios de aceite (rascunho 62.a)

- [ ] Comerciante informa CPF-MEI ou CNPJ + município; validação de dígitos  
- [ ] KYC pendente → não habilita `paymentsEnabled` / não publica items  
- [ ] PixKey armazenada com controle de acesso (dono da store)  
- [ ] Territory permanece sem campos fiscais  
- [ ] Spec + testes HTTP; sync-docs  

---

## Referências

- [ANALISE_FISCAL_BR.md](../compliance/ANALISE_FISCAL_BR.md)  
- [FASE55.md](./FASE55.md) · [FASE56.md](./FASE56.md) · [FASE57.md](./FASE57.md)  
- [REALINHAMENTO_SUSTENTACAO_OPERACIONAL.md](./REALINHAMENTO_SUSTENTACAO_OPERACIONAL.md)  
- [LGPD_COMPLIANCE.md](../LGPD_COMPLIANCE.md)  
