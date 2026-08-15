# Análise fiscal & conformidade comercial (Brasil) — Arah

**Versão**: 1.0  
**Data**: 2026-08-15  
**Público**: produto, jurídico, ops, engenharia  
**Escopo**: gaps fiscais/tributários e de administração comercial BR para marketplace territorial + open-core, **à luz do que já está em produção lógica** (API/BFF/app + FASE55 v0).

> Este documento **não** é parecer jurídico vinculante. Orienta priorização de backlog; decisões de enquadramento (marketplace × intermediário × IP) exigem consultoria.

---

## 1. Veredito executivo

| Dimensão | Situação |
|----------|----------|
| Produto comunitário + ledger open-core | ✅ Forte (quote, split, receipt de **taxa**, payout consolidado, PIX **mock**) |
| Fiscalidade brasileira (CNPJ/MEI, NFS-e, ISS/ICMS, KYC comercial) | ❌ Quase ausente |
| Bloqueio de piloto **com dinheiro real** | Não é só FASE54 secrets — é **PSP real + KYC/fiscal mínimo + parecer regulatório** |

**Conclusão**: evoluir o backlog com uma frente explícita **FASE62 — Conformidade fiscal & KYC comercial (BR)**, encaixada na Onda S1 junto de FASE55–57, sem misturar imposto com taxa open-core nem lógica social em `Territory`.

---

## 2. O que já está instalado (base para evoluir)

### Já entrega valor / base financeira
- Cadastro com **CPF** (validação) + LGPD (export/exclusão) + termos versionados
- Marketplace, checkout, **receipt de taxa/split** (não é NF)
- **FeeSplitRule** versionada; refund; SellerBalance / Wallet Aratá v0
- Gate comercial (assinatura) + alias `POST /merchants/{id}/subscription`
- UX PIX no app (QR) sobre gateway **mock**; Stripe para assinaturas
- Sustentação: CI/CD, Core, piloto em código (FASE54 config humana pendente)

### Explicitamente fora do escopo atual
- CNPJ / MEI / CNAE / regime tributário
- NFS-e / NF-e / NFC-e
- ISS municipal, ICMS/ST, retenções
- KYC comercial (conta PIX ↔ titular)
- `SellerAccountId` real no payout (TODO no código)
- Parecer Bacen/arranjos de pagamento

Fontes: `FASE55.md`, `REALINHAMENTO_SUSTENTACAO_OPERACIONAL.md` (risco “PSP, KYC, NF”), `Store.cs`, `MockPaymentGateway`, `SellerPayoutService`, `LGPD_COMPLIANCE.md`.

---

## 3. Mapa de gaps fiscais (por bloco)

| # | Gap | Impacto se ignorar | Depende de |
|---|-----|--------------------|------------|
| G1 | Perfil fiscal do comerciante (CNPJ/CPF-MEI, município ISS, regime) | Venda irregular; impossível emitir NF | Domínio Store/MerchantProfile |
| G2 | KYC comercial + vínculo PixKey/conta payout | Fraude, chargeback, bloqueio PSP | G1 + PSP |
| G3 | PIX/PSP produção (webhook Paid, custo PSP fora do split) | Sem receita real | FASE54 secrets + gateway |
| G4 | Payout real (`SellerAccountId`) | Saldo “pronto” sem liquidação | G2–G3 |
| G5 | NFS-e (MVP serviços locais) | Comércio formal exige documento | G1 + provedor municipal/nacional |
| G6 | Destaque tributário no checkout (quando aplicável) | Confusão taxa plataforma × imposto | G5; YAGNI até NF |
| G7 | Retenção fiscal × LGPD (NF não se anonimiza como PII) | Conflito legal | Políticas FASE16 + identidade |
| G8 | Relatórios para contador (export período) | Operação manual frágil | FASE56/57 + G5 |
| G9 | Enquadramento regulatório da plataforma | Risco Bacen / responsabilidade solidária | Parecer externo |
| G10 | Assinatura comercial BR (PIX/boleto além Stripe) | Fricção de conversão | FASE55/15 |

---

## 4. Priorização (considerando o feito)

Ordem alinhada ao realinhamento S0→S1 e ao estado atual:

```text
FASE54 config (humano) ──► fechar PSP sandbox
        │
        ▼
FASE55 restante (PIX real + SellerAccountId) ──► P0 técnico
        │
        ├──► FASE62.a KYC + perfil fiscal mínimo ──► P0 compliance (gate de venda)
        │
        ├──► Parecer regulatório (paralelo jurídico) ──► gate go-live faturando
        │
        ▼
FASE56 transparência (receita comunitária; sem SEFAZ)
FASE57 cockpit (status KYC/NF/payout)
        │
        ▼
FASE62.b NFS-e MVP (serviços) ──► P1
FASE62.c retenção fiscal vs LGPD ──► P1
ICMS/bens / cupom ──► P2 (só se catálogo físico exigir)
```

### P0 — desbloqueia piloto faturando
1. **Secrets + verify FASE54** (humano)  
2. **PSP/PIX produção + custo PSP fora do split** (fechar FASE55)  
3. **SellerAccountId / conta payout** (FASE55)  
4. **FASE62.a** — cadastro fiscal mínimo + KYC comercial + gate “não vende sem aprovado”  
5. **Parecer regulatório** (doc + bloqueio explícito de go-live)

### P1 — confiança e operação
6. FASE56 painel (marcar o que **não** é dado fiscal SEFAZ)  
7. FASE57 status KYC/NF/payout no cockpit  
8. **FASE62.b** — emissão/armazenamento NFS-e (MVP) + link no receipt da plataforma  
9. **FASE62.c** — política retenção documentos fiscais ≠ anonimização LGPD  
10. Assinatura comercial via PIX (além Stripe)

### P2 — escala
11. ICMS / bens físicos  
12. Doações FASE61 — recibo ≠ NF de venda  
13. Multi-município ISS sob federação (FASE58–59)  
14. Alertas fiscais no app implementador (FASE60)

---

## 5. Princípios de desenho (inegociáveis Arah)

1. **Taxa open-core ≠ tributo** — quote/receipt da plataforma nunca se apresentam como NF.  
2. **Territory neutro** — perfil fiscal vive em comerciante/store, nunca em Territory.  
3. **Morador não paga** o núcleo; obrigações fiscais caem no **comércio** (e na plataforma só no que o parecer definir).  
4. **Append-only** no ledger; NF/XML em store imutável com cancelamento = novo evento.  
5. **Spec-before-code** para FASE62 (`docs/specs/`).

---

## 6. Artefatos de backlog

| Artefato | Papel |
|----------|-------|
| [FASE62.md](./FASE62.md) | Fase proposta — conformidade fiscal & KYC comercial BR |
| Este documento | Análise + priorização |
| `REALINHAMENTO_SUSTENTACAO_OPERACIONAL.md` | Já cita risco PSP/KYC/NF — agora materializado |
| FASE55 / 56 / 57 | Ganchos de implementação |

---

## 7. Próximo passo recomendado (produto + eng)

| Quem | Ação |
|------|------|
| Humano ops | Fechar FASE54 (secrets staging) |
| Jurídico | Responder parecer regulatório (marketplace × NF × Pix) — paralelo |
| Eng | Spec `FASE62-fiscal-kyc-br` + fatia 62.a (perfil + gate) após/alongside PIX real |
| Produto | Validar MVP NFS-e só para **serviços** locais no piloto |

---

### Changelog

- **1.0** (2026-08-15): análise inicial a partir do estado FASE55 v0 + app estável; propõe FASE62 e priorização P0–P2.
