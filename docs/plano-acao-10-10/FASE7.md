# Fase 7: Sistema de Payout e Gestão Financeira

**Duração**: 4 semanas (28 dias úteis)  
**Prioridade**: 🟡 ALTA  
**Bloqueia**: Completar lógica de negócio de pagamentos  
**Estimativa Total**: 176 horas  
**Status**: ⏳ Em Progresso

---

## 🎯 Objetivo

Implementar sistema completo de payout (transferência para vendedores), rastreabilidade financeira completa e gestão financeira da plataforma, permitindo transparência total e autonomia para territórios.

---

## 📋 Contexto e Requisitos

### Problema Atual
Quando um checkout é marcado como `Paid`, o dinheiro fica no gateway mas **não é transferido automaticamente para o vendedor**. Não há rastreabilidade completa nem gestão financeira da plataforma.

### Requisitos Funcionais
- ✅ Rastreabilidade completa de cada centavo (lastro e transparência)
- ✅ Histórico de mudanças de status
- ✅ Logs de quem aprovou/rejeitou payouts
- ✅ Saldo próprio da plataforma
- ✅ Separação de fees por território
- ✅ Relatórios de receita/despesa (por loja e plataforma)
- ✅ Payout automático com work items para fallback
- ✅ Configurações por território (retenção, limites, etc.)
- ✅ Papel financeiro (FinancialManager, FinancialAuditor, FinancialViewer)
- ✅ Workflow de aprovação para transações suspeitas
- ✅ Limites de aprovação por usuário e território
- ✅ Sistema de sanções

---

## 📋 Tarefas Detalhadas

### Semana 13: Fundação - Rastreabilidade e Modelos de Domínio

#### 13.1 Modelos de Domínio - Rastreabilidade
**Estimativa**: 16 horas (2 dias)  
**Status**: ✅ Completo

**Tarefas**:
- [x] Criar `FinancialTransaction` (tabela central de rastreabilidade)
- [x] Criar `TransactionType` enum (Checkout, Payment, Seller, PlatformFee, Payout)
- [x] Criar `TransactionStatus` enum (Pending, Processing, Completed, Failed, Canceled)
- [x] Criar `TransactionStatusHistory` (histórico de mudanças)
- [x] Criar relacionamentos entre transações (RelatedTransactions)
- [ ] Criar migration para tabelas de rastreabilidade
- [ ] Documentar modelo de rastreabilidade

**Arquivos Criados**:
- `backend/Araponga.Domain/Financial/FinancialTransaction.cs` ✅
- `backend/Araponga.Domain/Financial/TransactionType.cs` ✅
- `backend/Araponga.Domain/Financial/TransactionStatus.cs` ✅
- `backend/Araponga.Domain/Financial/TransactionStatusHistory.cs` ✅

**Critérios de Sucesso**:
- ✅ Modelo de rastreabilidade completo
- ✅ Relacionamentos entre transações funcionando
- ✅ Histórico de status implementado
- ⚠️ Migration criada e testada (pendente)

---

#### 13.2 Modelos de Domínio - Saldo e Transações de Vendedor
**Estimativa**: 16 horas (2 dias)  
**Status**: ✅ Completo

**Tarefas**:
- [x] Criar `SellerBalance` (saldo por vendedor/território)
- [x] Criar `SellerTransaction` (transações de vendedor)
- [x] Criar `SellerTransactionStatus` enum
- [x] Criar relacionamento com `Checkout`
- [ ] Criar migration para tabelas de vendedor
- [ ] Documentar modelo de saldo de vendedor

**Arquivos Criados**:
- `backend/Araponga.Domain/Marketplace/SellerBalance.cs` ✅
- `backend/Araponga.Domain/Marketplace/SellerTransaction.cs` ✅
- `backend/Araponga.Domain/Marketplace/SellerTransactionStatus.cs` ✅

**Critérios de Sucesso**:
- ✅ Modelo de saldo de vendedor completo
- ✅ Relacionamento com checkout funcionando
- ⚠️ Migration criada e testada (pendente)

---

#### 13.3 Modelos de Domínio - Gestão Financeira da Plataforma
**Estimativa**: 16 horas (2 dias)  
**Status**: ✅ Completo

**Tarefas**:
- [x] Criar `PlatformFinancialBalance` (saldo da plataforma por território)
- [x] Criar `PlatformRevenueTransaction` (receitas - fees coletadas)
- [x] Criar `PlatformExpenseTransaction` (despesas - payouts processados)
- [x] Criar `ReconciliationRecord` (conciliação bancária)
- [ ] Criar migration para tabelas de gestão financeira
- [ ] Documentar modelo de gestão financeira

**Arquivos Criados**:
- `backend/Araponga.Domain/Financial/PlatformFinancialBalance.cs` ✅
- `backend/Araponga.Domain/Financial/PlatformRevenueTransaction.cs` ✅
- `backend/Araponga.Domain/Financial/PlatformExpenseTransaction.cs` ✅
- `backend/Araponga.Domain/Financial/ReconciliationRecord.cs` ✅

**Critérios de Sucesso**:
- ✅ Modelo de gestão financeira completo
- ✅ Separação por território implementada
- ⚠️ Migration criada e testada (pendente)

---

**Status**: ✅ **MODELOS DE DOMÍNIO COMPLETOS**  
**Próxima Tarefa**: Criar repositórios e migrations

---

### Semana 14: Configuração e Payout

#### 14.1 Configuração de Payout por Território
**Estimativa**: 16 horas (2 dias)  
**Status**: ⏳ Pendente

#### 14.2 Interface de Payout Gateway
**Estimativa**: 16 horas (2 dias)  
**Status**: ⏳ Pendente

#### 14.3 Serviço de Payout
**Estimativa**: 24 horas (3 dias)  
**Status**: ✅ Em Progresso

**Tarefas**:
- [x] Criar `SellerPayoutService`
- [x] Integrar com checkout: quando checkout = `Paid`, criar `SellerTransaction`
- [x] Implementar cálculo de valores (subtotal - fees = valor líquido)
- [x] Atualizar `SellerBalance` após criação de transação
- [x] Criar rastreabilidade completa (FinancialTransaction)
- [x] Criar PlatformRevenueTransaction para fees
- [x] Atualizar PlatformFinancialBalance
- [ ] Implementar lógica de retenção (período configurável)
- [ ] Implementar lógica de valor mínimo (acumular até atingir)
- [ ] Implementar payout automático
- [ ] Criar work items para fallback (usar sistema Outbox existente)
- [ ] Integrar com `IPayoutGateway`

**Arquivos Criados**:
- `backend/Araponga.Application/Services/SellerPayoutService.cs` ✅

**Arquivos Modificados**:
- `backend/Araponga.Application/Interfaces/ICheckoutRepository.cs` ✅ (adicionado GetByIdAsync)
- `backend/Araponga.Infrastructure/Postgres/PostgresCheckoutRepository.cs` ✅
- `backend/Araponga.Infrastructure/InMemory/InMemoryCheckoutRepository.cs` ✅

**Critérios de Sucesso**:
- ✅ Quando checkout = `Paid`, `SellerTransaction` é criada automaticamente
- ✅ Saldo do vendedor é atualizado corretamente
- ✅ Rastreabilidade completa implementada
- ⚠️ Payout automático funcionando (pendente)
- ⚠️ Work items para fallback funcionando (pendente)

---

**Status**: ✅ **FUNDAÇÃO COMPLETA + SELLERPAYOUTSERVICE CRIADO**  
**Próxima Tarefa**: Configuração de Payout e Interface de Gateway
