# Retrospectiva DoD — Gaps de fases/PRs anteriores

**Data**: 2026-07-02
**Origem**: `docs/governance/DEFINITION_OF_DONE.md` (DoD rigor total, estabelecida em 2026-07-02)
**Objetivo**: auditar o que foi entregue **antes** da nova DoD e retroalimentar o backlog com o que falta para cada item atingir "pronto" (AC↔teste rastreável, testes de integração HTTP, evidência, parecer de domínio endereçado, doc-sync).

> A nova DoD é retroativa apenas como **dívida priorizada** — não bloqueia o que já está em `main`. Fases antigas são reconciliadas por risco, não em bloco.

---

## Método

1. Inventário de specs (`docs/specs/phases/*.spec.yaml`) e status de fase (`docs/STATUS_FASES.md`).
2. Verificação de rastreabilidade AC↔teste (`covered_by`) por AC.
3. Verificação de execução do gate `spec-validate` sobre as specs existentes.
4. Cruzamento com dívidas técnicas descobertas na implementação FASE55.

---

## Achados

### A. Cobertura de spec (SDD)

| Faixa | Spec SDD | AC↔teste (`covered_by`) |
|-------|----------|--------------------------|
| FASE1–FASE51 (concluídas) | ❌ inexistente | ❌ — sem AC formal |
| FASE52 (CI/CD) | ✅ | ⚠️ ACs de workflow sem teste automatizado (evidência via run) |
| FASE53 (Arah Core) | ✅ | ✅ **corrigido nesta passada** (unit + integração já existiam) |
| FASE54 (IaC) | ✅ | ⚠️ ACs de script/manual; só AC-54-4 é testável (corrigido) |
| FASE55 (Monetização) | ✅ | ✅ padrão-ouro (todos ACs com `covered_by`) |

### B. Gate `spec-validate` quebrado para fases concluídas

- `scripts/harness/validate-specs.ps1` só aceitava root `status ∈ {draft, active, deprecated}`; FASE52/53 usam `status: completed` → o gate **rejeitava** as fases prontas. **Corrigido nesta passada** (`completed` adicionado).

### C. Dívidas técnicas herdadas (descobertas na FASE55)

- Nenhum código de produção transiciona `Checkout` → `Paid` (não há `PaymentService`/webhook). `quote/receipt/refund/payout` só são exercitáveis com estado setado direto (testes).
- FASE55 monetização paralela ao domínio financeiro existente (`SellerPayoutService`, ledger `FinancialTransaction`, `SellerBalance`) — risco de duplicação/divergência.
- `FeeSplitRule` e planos comerciais só em InMemory (sem persistência Postgres).

---

## Backlog (itens acionáveis)

Prioridade: **P0** desbloqueia rigor mínimo; **P1** reduz risco; **P2** melhoria.

| ID | Área | Gap (critério DoD) | Ação | Prioridade | Status |
|----|------|--------------------|------|------------|--------|
| DOD-01 | spec/harness | `spec-validate` rejeitava `completed` | Aceitar `completed` no validador | P0 | ✅ feito |
| DOD-02 | backend | FASE53 sem `covered_by` (AC↔teste) | Anotar 10 ACs (unit+integração Core) | P0 | ✅ feito |
| DOD-03 | ops | FASE54 ACs de script sem evidência | Marcar `status: manual` + `evidence`; AC-54-4 `covered_by` | P0 | ✅ feito |
| DOD-04 | ops | FASE52 ACs de workflow sem evidência | Marcar `status: manual` + `evidence` (run/pacote) | P0 | ✅ feito |
| DOD-05 | backend | Sem `PaymentService`/webhook (Checkout→Paid) | Implementar transição de pagamento (PIX/Stripe) + testes | P1 | ✅ feito (2026-07-02: `PaymentService`, `IPaymentGateway`, `MockPaymentGateway`, endpoints `/pay` e `/confirm-payment`, webhook mock) |
| DOD-06 | backend | Monetização FASE55 paralela ao ledger existente | Unificar `RefundService`/`PayoutConsolidationService` com `SellerPayoutService` + `FinancialTransaction` (fonte única) | P1 | ✅ feito (2026-07-02: `ReversePaidCheckoutAsync`, estorno append-only no ledger) |
| DOD-07 | backend | `FeeSplitRule` + planos comerciais só InMemory | Persistência Postgres (repo + seed/migration) | P1 | ✅ feito (2026-07-02: `PostgresFeeSplitRuleRepository` + migration `AddFeeSplitRules`; planos comerciais já via `SubscriptionPlan` Postgres) |
| DOD-08 | spec/backend | FASE1–FASE51 sem spec/AC | Backfill de spec por **risco/domínio** (monetização, control-plane, moderação, marketplace primeiro) — não em bloco | P2 | ⏳ backlog |
| DOD-09 | qa | Sem gate que exija `covered_by` em AC `covered` | `spec-validate`: falhar se AC `covered` sem `covered_by` | P1 | ✅ feito (2026-07-02: `validate-specs.ps1` falha AC `covered` sem `covered_by` e `manual` sem `evidence`) |
| DOD-10 | ops/qa | Evidência de ACs `manual` não coletada em CI | CD anexar logs (verify-pilot, health, GHCR) como artifact vinculado ao AC | P2 | ⏳ backlog |

---

## Fixado nesta passada

- **DOD-01**: `validate-specs.ps1` aceita `status: completed`.
- **DOD-02**: `FASE53-arah-core.spec.yaml` — 10 ACs com `covered_by` (mapeados a `CoreControllerIntegration`, `CoreInstanceRegistry`, `CoreReleaseCatalog`, `CoreDirectoryService`, `FederationIdentityService`, `FederationControllerIntegration`, `CoreAvailabilityCache`).
- **DOD-03**: `FASE54-iac.spec.yaml` — ACs de script com `status: manual` + `evidence`; `AC-54-4` com `covered_by`.
- **DOD-04**: `FASE52-cicd.spec.yaml` — ACs de workflow com `status: manual` + `evidence`.
- **DOD-05**: `PaymentService` + `IPaymentGateway`/`MockPaymentGateway` — fluxo `Created → AwaitingPayment → Paid`; endpoints `/pay`, `/confirm-payment`; webhook mock; testes unit + HTTP; AC-55-8 na spec FASE55.
- **DOD-06**: `SellerPayoutService.ReversePaidCheckoutAsync` — estorno persiste `FinancialTransaction` tipo `Refund` (append-only), cancela `SellerTransaction` pendente, reverte saldos; `RefundService` delega ao ledger.
- **DOD-07**: `FeeSplitRuleRecord` + `PostgresFeeSplitRuleRepository` + migration `AddFeeSplitRules`; DI Postgres substitui InMemory; planos comerciais já persistiam via `SubscriptionPlan`.

## Próximos (recomendação de ordem)

1. **DOD-08** (backfill de spec por risco) — contínuo, uma fase/domínio por vez (monetização/payout primeiro).
2. **DOD-10** (evidência CI para ACs `manual`) — anexar logs verify-pilot/health/GHCR como artifact.
