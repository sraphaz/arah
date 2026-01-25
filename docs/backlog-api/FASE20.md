# Fase 22: Sistema de Moeda Territorial (Mint e Economia Local)

**Duração**: 5 semanas (35 dias úteis)  
**Prioridade**: 🟡 ALTA (Economia circular e autonomia territorial)  
**Depende de**: Fase 6 (Pagamentos), Fase 7 (Payout), Fase 14 (Governança), Fase 42 (Gamificação), Fase 24 (Saúde Territorial)  
**Estimativa Total**: 200 horas  
**Status**: ⏳ Pendente  
**Nota**: Renumerada de Fase 20 para Fase 22 (Onda 4: Economia Local Completa)

---

## 🎯 Objetivo

Implementar sistema de **moeda territorial** que:
- Permite cada território ter sua própria moeda digital
- Recompensa atividades que agregam valor ao território (mint por contribuições)
- Facilita economia circular local (marketplace, entregas, serviços)
- Integra com sistema de gamificação (Fase 42)
- Integra com atividades territoriais (Fase 24)
- Suporta fundos territoriais para projetos comunitários
- Permite conversão com moedas fiat e criptomoedas (preparação para Fase 21)

**Princípios**:
- ✅ **Autonomia**: Cada território define sua moeda e políticas
- ✅ **Economia Circular**: Moeda circula dentro do território
- ✅ **Contribuição Real**: Mint baseado em atividades reais
- ✅ **Transparência**: Todas as transações são auditáveis
- ✅ **Governança Comunitária**: Políticas definidas pela comunidade

---

## 📋 Contexto e Requisitos

### Estado Atual
- ✅ MER prevê estrutura completa (`TERRITORY_CURRENCY`, `USER_WALLET`, `WALLET_TRANSACTION`, `TERRITORY_FUND`, `FUND_ALLOCATION`)
- ✅ Sistema de pagamentos (Fase 6)
- ✅ Sistema de payout (Fase 7)
- ✅ Sistema de gamificação (Fase 42)
- ✅ Sistema de atividades territoriais (Fase 24)
- ❌ Não existe sistema de moeda territorial
- ❌ Não existe sistema de mint
- ❌ Não existe sistema de carteiras digitais
- ❌ Não existe sistema de fundos territoriais

### Requisitos Funcionais

#### 1. Moeda Territorial
- ✅ Criar moeda para território (símbolo, nome, supply inicial)
- ✅ Políticas de mint configuráveis por território
- ✅ Taxas de mint por tipo de atividade
- ✅ Limites de mint (diário, semanal, mensal)
- ✅ Governança comunitária (votação para criar/alterar moeda)

#### 2. Mint (Criação de Moeda)
- ✅ Mint por atividades territoriais:
  - Coleta de resíduos
  - Plantio de árvores
  - Mutirões
  - Observações de saúde
  - Monitoramento (sensores)
  - Manutenção de recursos naturais
- ✅ Mint por contribuições (gamificação):
  - Posts relevantes
  - Eventos comunitários
  - Participação em votações
  - Moderação
- ✅ Mint por vendas no marketplace (opcional)
- ✅ Taxas configuráveis por território

#### 3. Carteiras Digitais
- ✅ Carteira por usuário e território
- ✅ Saldo em moeda territorial
- ✅ Histórico de transações
- ✅ Transferências entre usuários
- ✅ Pagamentos no marketplace
- ✅ Pagamentos por entregas
- ✅ Conversão com fiat (preparação)

#### 4. Transações
- ✅ Transações entre carteiras
- ✅ Transações no marketplace
- ✅ Transações de payout (moeda → fiat)
- ✅ Transações de mint (atividades → moeda)
- ✅ Taxas de transação (opcional, configurável)
- ✅ Auditoria completa

#### 5. Fundos Territoriais
- ✅ Fundo territorial (pool de moeda)
- ✅ Alocações para projetos comunitários
- ✅ Votação para aprovar alocações
- ✅ Acompanhamento de projetos
- ✅ Relatórios de impacto

#### 6. Conversão
- ✅ Conversão moeda territorial ↔ fiat (preparação)
- ✅ Conversão moeda territorial ↔ criptomoeda (Fase 21)
- ✅ Taxas de conversão
- ✅ Histórico de conversões

---

## 📋 Tarefas Detalhadas

### Semana 1-2: Modelo de Domínio e Moeda Territorial

#### 20.1 Modelo de Domínio - Moeda Territorial
**Estimativa**: 32 horas (4 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar modelo `TerritoryCurrency`:
  - [ ] `Id`, `TerritoryId`
  - [ ] `Symbol` (string, ex: "VALE", "SERRA")
  - [ ] `Name` (string, ex: "Vale do Paraíba")
  - [ ] `Supply` (decimal, supply total)
  - [ ] `MintPolicy` (JSON, políticas de mint)
  - [ ] `IsActive` (bool)
  - [ ] `CreatedAtUtc`, `UpdatedAtUtc`
- [ ] Criar modelo `CurrencyMintPolicy`:
  - [ ] `MintRateByWasteCollection` (decimal, taxa por coleta)
  - [ ] `MintRateByTreePlanting` (decimal, taxa por plantio)
  - [ ] `MintRateByTerritoryAction` (decimal, taxa por ação)
  - [ ] `MintRateByHealthObservation` (decimal, taxa por observação)
  - [ ] `MintRateByMonitoring` (decimal, taxa por monitoramento)
  - [ ] `MintRateByContribution` (decimal, taxa por contribuição)
  - [ ] `DailyMintLimit` (decimal?, nullable)
  - [ ] `WeeklyMintLimit` (decimal?, nullable)
  - [ ] `MonthlyMintLimit` (decimal?, nullable)
- [ ] Criar modelo `UserWallet`:
  - [ ] `Id`, `UserId`, `TerritoryId`, `CurrencyId`
  - [ ] `Balance` (decimal, saldo)
  - [ ] `CreatedAtUtc`, `UpdatedAtUtc`
- [ ] Criar modelo `WalletTransaction`:
  - [ ] `Id`, `WalletId`, `TerritoryId`, `CurrencyId`
  - [ ] `Type` (MINT, TRANSFER, PAYMENT, PAYOUT, CONVERSION)
  - [ ] `Amount` (decimal, quantidade)
  - [ ] `FromWalletId?` (nullable, para transferências)
  - [ ] `ToWalletId?` (nullable, para transferências)
  - [ ] `RelatedEntityId?` (nullable, ID da entidade relacionada)
  - [ ] `RelatedEntityType?` (nullable, tipo da entidade)
  - [ ] `Description?` (nullable)
  - [ ] `CreatedAtUtc`
- [ ] Criar modelo `TerritoryFund`:
  - [ ] `Id`, `TerritoryId`, `CurrencyId`
  - [ ] `Balance` (decimal, saldo do fundo)
  - [ ] `CreatedAtUtc`, `UpdatedAtUtc`
- [ ] Criar modelo `FundAllocation`:
  - [ ] `Id`, `FundId`, `ProjectId?` (nullable)
  - [ ] `Amount` (decimal, quantidade alocada)
  - [ ] `Purpose` (string, propósito)
  - [ ] `Status` (PENDING, APPROVED, REJECTED, COMPLETED)
  - [ ] `ApprovedByVotingId?` (nullable, votação que aprovou)
  - [ ] `CreatedAtUtc`, `UpdatedAtUtc`
- [ ] Criar repositórios
- [ ] Criar migrations

**Arquivos a Criar**:
- `backend/Araponga.Domain/Currency/TerritoryCurrency.cs`
- `backend/Araponga.Domain/Currency/CurrencyMintPolicy.cs`
- `backend/Araponga.Domain/Currency/UserWallet.cs`
- `backend/Araponga.Domain/Currency/WalletTransaction.cs`
- `backend/Araponga.Domain/Currency/WalletTransactionType.cs`
- `backend/Araponga.Domain/Currency/TerritoryFund.cs`
- `backend/Araponga.Domain/Currency/FundAllocation.cs`
- `backend/Araponga.Domain/Currency/FundAllocationStatus.cs`
- `backend/Araponga.Application/Interfaces/ITerritoryCurrencyRepository.cs`
- `backend/Araponga.Application/Interfaces/IUserWalletRepository.cs`
- `backend/Araponga.Application/Interfaces/IWalletTransactionRepository.cs`
- `backend/Araponga.Application/Interfaces/ITerritoryFundRepository.cs`
- `backend/Araponga.Application/Interfaces/IFundAllocationRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresTerritoryCurrencyRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresUserWalletRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresWalletTransactionRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresTerritoryFundRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresFundAllocationRepository.cs`

**Critérios de Sucesso**:
- ✅ Modelos criados
- ✅ Repositórios implementados
- ✅ Migrations criadas
- ✅ Testes de repositório passando

---

### Semana 2-3: Sistema de Mint e Carteiras

#### 20.2 Sistema de Mint
**Estimativa**: 40 horas (5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `CurrencyMintService`:
  - [ ] `MintByActivityAsync(Guid territoryId, Guid userId, ActivityType type, ...)` → mint por atividade
  - [ ] `MintByContributionAsync(Guid territoryId, Guid userId, Guid contributionId)` → mint por contribuição
  - [ ] `CheckMintLimitsAsync(Guid territoryId, ...)` → verificar limites
  - [ ] `CalculateMintAmountAsync(Guid territoryId, ActivityType type, ...)` → calcular quantidade
- [ ] Integrar com Fase 24 (Saúde Territorial):
  - [ ] `WasteCollectionService` → mint ao reportar coleta
  - [ ] `TreePlantingService` → mint ao reportar plantio
  - [ ] `TerritoryActionService` → mint ao participar/organizar ação
  - [ ] `HealthObservationService` → mint ao criar observação confirmada
  - [ ] `SensorDeviceService` → mint ao confirmar leitura
- [ ] Integrar com Fase 42 (Gamificação):
  - [ ] `ContributionService` → mint baseado em contribuições
- [ ] Integrar com Fase 6 (Marketplace):
  - [ ] `CartService` → mint opcional por venda (configurável)
- [ ] Criar `CurrencyMintController`:
  - [ ] `POST /api/v1/currency/{currencyId}/mint` → mint manual (admin)
  - [ ] `GET /api/v1/currency/{currencyId}/mint-history` → histórico de mint
- [ ] Feature flags: `TerritoryCurrencyEnabled`, `CurrencyMintEnabled`
- [ ] Validações e limites
- [ ] Testes

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/CurrencyMintService.cs`
- `backend/Araponga.Api/Controllers/CurrencyMintController.cs`
- `backend/Araponga.Api/Contracts/Currency/MintRequest.cs`
- `backend/Araponga.Api/Contracts/Currency/MintHistoryResponse.cs`

**Critérios de Sucesso**:
- ✅ Sistema de mint funcionando
- ✅ Integração com atividades funcionando
- ✅ Limites sendo respeitados
- ✅ Testes passando

---

#### 20.3 Sistema de Carteiras Digitais
**Estimativa**: 32 horas (4 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `WalletService`:
  - [ ] `GetOrCreateWalletAsync(Guid userId, Guid territoryId, Guid currencyId)` → obter/criar carteira
  - [ ] `GetBalanceAsync(Guid walletId)` → obter saldo
  - [ ] `TransferAsync(Guid fromWalletId, Guid toWalletId, decimal amount, ...)` → transferir
  - [ ] `GetTransactionsAsync(Guid walletId, ...)` → listar transações
  - [ ] `GetTransactionHistoryAsync(Guid walletId, ...)` → histórico
- [ ] Integrar com Fase 6 (Marketplace):
  - [ ] `CartService` → pagamento em moeda territorial
- [ ] Integrar com Fase 16 (Entregas):
  - [ ] `DeliveryService` → pagamento por entrega
- [ ] Criar `WalletController`:
  - [ ] `GET /api/v1/wallets/me` → listar carteiras do usuário
  - [ ] `GET /api/v1/wallets/{walletId}` → obter carteira
  - [ ] `GET /api/v1/wallets/{walletId}/balance` → obter saldo
  - [ ] `GET /api/v1/wallets/{walletId}/transactions` → listar transações
  - [ ] `POST /api/v1/wallets/{fromWalletId}/transfer` → transferir
- [ ] Feature flags: `WalletsEnabled`, `WalletTransfersEnabled`
- [ ] Validações e segurança
- [ ] Testes

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/WalletService.cs`
- `backend/Araponga.Api/Controllers/WalletController.cs`
- `backend/Araponga.Api/Contracts/Wallet/WalletResponse.cs`
- `backend/Araponga.Api/Contracts/Wallet/TransferRequest.cs`
- `backend/Araponga.Api/Contracts/Wallet/WalletTransactionResponse.cs`

**Critérios de Sucesso**:
- ✅ Sistema de carteiras funcionando
- ✅ Transferências funcionando
- ✅ Integração com marketplace funcionando
- ✅ Testes passando

---

### Semana 3-4: Fundos Territoriais e Conversão

#### 20.4 Sistema de Fundos Territoriais
**Estimativa**: 32 horas (4 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `TerritoryFundService`:
  - [ ] `GetOrCreateFundAsync(Guid territoryId, Guid currencyId)` → obter/criar fundo
  - [ ] `AllocateFundAsync(Guid fundId, decimal amount, string purpose, ...)` → alocar fundo
  - [ ] `ApproveAllocationAsync(Guid allocationId, Guid votingId)` → aprovar alocação (via votação)
  - [ ] `ListAllocationsAsync(Guid fundId, ...)` → listar alocações
  - [ ] `GetFundBalanceAsync(Guid fundId)` → obter saldo do fundo
- [ ] Integrar com Fase 14 (Governança):
  - [ ] `VotingService` → votação para aprovar alocações
- [ ] Criar `TerritoryFundController`:
  - [ ] `GET /api/v1/territory-funds/{territoryId}` → obter fundo
  - [ ] `POST /api/v1/territory-funds/{fundId}/allocations` → criar alocação
  - [ ] `GET /api/v1/territory-funds/{fundId}/allocations` → listar alocações
  - [ ] `PATCH /api/v1/territory-funds/allocations/{allocationId}/approve` → aprovar alocação
- [ ] Feature flags: `TerritoryFundsEnabled`, `FundAllocationsEnabled`
- [ ] Validações
- [ ] Testes

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/TerritoryFundService.cs`
- `backend/Araponga.Api/Controllers/TerritoryFundController.cs`
- `backend/Araponga.Api/Contracts/Fund/TerritoryFundResponse.cs`
- `backend/Araponga.Api/Contracts/Fund/FundAllocationRequest.cs`
- `backend/Araponga.Api/Contracts/Fund/FundAllocationResponse.cs`

**Critérios de Sucesso**:
- ✅ Sistema de fundos funcionando
- ✅ Alocações funcionando
- ✅ Integração com votação funcionando
- ✅ Testes passando

---

#### 20.5 Sistema de Conversão
**Estimativa**: 24 horas (3 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `CurrencyConversionService`:
  - [ ] `ConvertToFiatAsync(Guid currencyId, decimal amount, string fiatCurrency)` → converter para fiat
  - [ ] `ConvertFromFiatAsync(Guid currencyId, decimal fiatAmount, string fiatCurrency)` → converter de fiat
  - [ ] `GetConversionRateAsync(Guid currencyId, string targetCurrency)` → obter taxa de conversão
  - [ ] `GetConversionHistoryAsync(Guid walletId, ...)` → histórico de conversões
- [ ] Integrar com Fase 7 (Payout):
  - [ ] `PayoutService` → conversão para fiat no payout
- [ ] Preparar para Fase 21 (Criptomoedas):
  - [ ] Estrutura para conversão com criptomoedas
- [ ] Criar `CurrencyConversionController`:
  - [ ] `POST /api/v1/currency/{currencyId}/convert` → converter moeda
  - [ ] `GET /api/v1/currency/{currencyId}/conversion-rate` → obter taxa
  - [ ] `GET /api/v1/currency/{currencyId}/conversion-history` → histórico
- [ ] Feature flags: `CurrencyConversionEnabled`
- [ ] Validações
- [ ] Testes

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/CurrencyConversionService.cs`
- `backend/Araponga.Api/Controllers/CurrencyConversionController.cs`
- `backend/Araponga.Api/Contracts/Currency/ConversionRequest.cs`
- `backend/Araponga.Api/Contracts/Currency/ConversionResponse.cs`

**Critérios de Sucesso**:
- ✅ Sistema de conversão funcionando
- ✅ Integração com payout funcionando
- ✅ Preparação para criptomoedas
- ✅ Testes passando

---

### Semana 4-5: Integração e Governança

#### 20.6 Integração Completa
**Estimativa**: 24 horas (3 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Integrar com Fase 6 (Marketplace):
  - [ ] Pagamento em moeda territorial
  - [ ] Mint opcional por venda
- [ ] Integrar com Fase 7 (Payout):
  - [ ] Conversão moeda → fiat no payout
- [ ] Integrar com Fase 16 (Entregas):
  - [ ] Pagamento por entrega em moeda territorial
- [ ] Integrar com Fase 42 (Gamificação):
  - [ ] Mint baseado em contribuições
- [ ] Integrar com Fase 24 (Saúde Territorial):
  - [ ] Mint por atividades territoriais
- [ ] Integrar com Fase 14 (Governança):
  - [ ] Votação para criar/alterar moeda
  - [ ] Votação para aprovar alocações de fundo
- [ ] Testes de integração
- [ ] Documentação

**Critérios de Sucesso**:
- ✅ Todas as integrações funcionando
- ✅ Testes de integração passando
- ✅ Documentação completa

---

#### 20.7 Governança e Políticas
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `TerritoryCurrencyService`:
  - [ ] `CreateCurrencyAsync(Guid territoryId, ...)` → criar moeda (requer votação)
  - [ ] `UpdateCurrencyPolicyAsync(Guid currencyId, ...)` → atualizar política (requer votação)
  - [ ] `GetCurrencyAsync(Guid currencyId)` → obter moeda
  - [ ] `ListCurrenciesByTerritoryAsync(Guid territoryId)` → listar moedas
- [ ] Integrar com Fase 14 (Governança):
  - [ ] Votação para criar moeda
  - [ ] Votação para alterar políticas
- [ ] Criar `TerritoryCurrencyController`:
  - [ ] `POST /api/v1/territory-currency` → criar moeda (requer votação)
  - [ ] `GET /api/v1/territory-currency/{territoryId}` → listar moedas
  - [ ] `GET /api/v1/territory-currency/{currencyId}` → obter moeda
  - [ ] `PATCH /api/v1/territory-currency/{currencyId}/policy` → atualizar política (requer votação)
- [ ] Feature flags: `TerritoryCurrencyGovernanceEnabled`
- [ ] Validações
- [ ] Testes

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/TerritoryCurrencyService.cs`
- `backend/Araponga.Api/Controllers/TerritoryCurrencyController.cs`
- `backend/Araponga.Api/Contracts/Currency/CreateTerritoryCurrencyRequest.cs`
- `backend/Araponga.Api/Contracts/Currency/TerritoryCurrencyResponse.cs`
- `backend/Araponga.Api/Contracts/Currency/UpdateCurrencyPolicyRequest.cs`

**Critérios de Sucesso**:
- ✅ Sistema de governança funcionando
- ✅ Integração com votação funcionando
- ✅ Testes passando

---

## 📊 Resumo da Fase 22

| Tarefa | Estimativa | Status | Prioridade |
|--------|------------|--------|------------|
| Modelo de Domínio | 32h | ❌ Pendente | 🔴 Alta |
| Sistema de Mint | 40h | ❌ Pendente | 🔴 Alta |
| Carteiras Digitais | 32h | ❌ Pendente | 🔴 Alta |
| Fundos Territoriais | 32h | ❌ Pendente | 🟡 Média |
| Sistema de Conversão | 24h | ❌ Pendente | 🟡 Média |
| Integração Completa | 24h | ❌ Pendente | 🔴 Alta |
| Governança e Políticas | 16h | ❌ Pendente | 🟡 Média |
| **Total** | **200h (35 dias)** | | |

---

## ✅ Critérios de Sucesso da Fase 22

### Funcionalidades
- ✅ Sistema completo de moeda territorial funcionando
- ✅ Sistema de mint funcionando (integração com atividades)
- ✅ Carteiras digitais funcionando
- ✅ Transferências funcionando
- ✅ Fundos territoriais funcionando
- ✅ Sistema de conversão funcionando (preparação para criptomoedas)
- ✅ Integração com todas as fases dependentes funcionando

### Qualidade
- ✅ Testes com cobertura adequada
- ✅ Documentação completa
- ✅ Feature flags implementados
- ✅ Validações e segurança implementadas
- ✅ Auditoria completa de transações
- Considerar **Testcontainers + PostgreSQL** para testes de integração (moeda, carteiras, transações, fundos) com banco real — **crítico** para consistência (estratégia na Fase 19; [TESTCONTAINERS_POSTGRES_IMPACTO](../../TESTCONTAINERS_POSTGRES_IMPACTO.md)).

### Integração
- ✅ Integração com Fase 6 (Marketplace) funcionando
- ✅ Integração com Fase 7 (Payout) funcionando
- ✅ Integração com Fase 14 (Governança) funcionando
- ✅ Integração com Fase 16 (Entregas) funcionando
- ✅ Integração com Fase 42 (Gamificação) funcionando
- ✅ Integração com Fase 24 (Saúde Territorial) funcionando
- ✅ Preparação para Fase 21 (Criptomoedas)

---

## 🔗 Dependências

- **Fase 6**: Pagamentos (base para transações)
- **Fase 7**: Payout (conversão para fiat)
- **Fase 14**: Governança (votação para criar/alterar moeda)
- **Fase 16**: Entregas (pagamento por entrega)
- **Fase 42**: Gamificação (mint por contribuições)
- **Fase 24**: Saúde Territorial (mint por atividades)

---

## 📝 Notas de Implementação

### Políticas de Mint

**Taxas Configuráveis por Território**:
- Coleta de resíduos: 0.1-1.0 moeda por kg
- Plantio de árvore: 1.0-5.0 moedas por árvore
- Mutirão: 2.0-10.0 moedas por participação
- Observação de saúde: 0.5-2.0 moedas por observação
- Monitoramento: 0.1-0.5 moedas por leitura confirmada
- Contribuição (gamificação): 0.1-1.0 moeda por ponto

**Limites**:
- Diário: 100-1000 moedas (configurável)
- Semanal: 500-5000 moedas (configurável)
- Mensal: 2000-20000 moedas (configurável)

### Economia Circular

**Fluxo**:
1. Usuário realiza atividade → Mint de moeda
2. Moeda é usada no marketplace → Economia local
3. Moeda é usada para pagar entregas → Serviços locais
4. Moeda pode ser convertida para fiat → Payout
5. Fundos territoriais → Projetos comunitários

### Governança

**Votação para Criar Moeda**:
- Requer aprovação da comunidade (Fase 14)
- Define símbolo, nome, supply inicial
- Define políticas de mint

**Votação para Alterar Políticas**:
- Requer aprovação da comunidade
- Pode alterar taxas de mint
- Pode alterar limites

### Segurança

- Todas as transações são auditáveis
- Limites de mint para evitar inflação
- Validações de saldo antes de transferências
- Taxas de transação opcionais (configuráveis)

---

**Status**: ⏳ **FASE 20 PENDENTE**  
**Depende de**: Fases 6, 7, 14, 16, 17, 18  
**Crítico para**: Economia Circular e Autonomia Territorial
