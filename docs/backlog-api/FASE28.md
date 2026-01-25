# Fase 28: Negociação Territorial e Assinatura Coletiva de Serviços Digitais

**Duração**: 3 semanas (21 dias úteis)  
**Prioridade**: 🟡 ALTA (Economia de escala e inclusão)  
**Depende de**: Fase 26 (Serviços Digitais Base), Fase 22 (Moeda Territorial), Fase 14 (Votação)  
**Estimativa Total**: 120-144 horas  
**Status**: ⏳ Pendente  
**Nota**: Renumerada de Fase 27 para Fase 28 (Onda 6: Autonomia Digital). Referências atualizadas: Fase 26 (Serviços Digitais), Fase 22 (Moeda Territorial), Fase 14 (Votação).

---

## 🎯 Objetivo

Implementar sistema de **negociação territorial de serviços digitais** que permite:
- Territórios negociarem/comprar quotas de serviços digitais
- Disponibilizar serviços para membros através de assinatura coletiva
- Subsidiar acesso para membros que não podem pagar individualmente
- Governança comunitária (votação para aprovar negociações)
- Dashboard territorial de serviços e consumo

**Princípios**:
- ✅ **Economia de Escala**: Negociação coletiva reduz custos
- ✅ **Inclusão**: Acesso para quem não pode pagar
- ✅ **Governança**: Comunidade decide alocação
- ✅ **Transparência**: Uso e custos visíveis
- ✅ **Autonomia**: Território controla seus recursos

---

## 📋 Contexto e Requisitos

### Estado Atual
- ✅ Fase 26 (Serviços Digitais Base) fornece infraestrutura
- ✅ Fase 22 (Moeda Territorial) fornece fundos territoriais
- ✅ Fase 14 (Votação) fornece governança comunitária
- ✅ Feature flags territoriais funcionando
- ❌ Territórios não podem negociar serviços
- ❌ Não existe pool de quotas compartilhado
- ❌ Não existe sistema de subsídios

### Requisitos Funcionais

#### 1. Negociação Territorial de Serviços
- ✅ Acordos de serviço por território
- ✅ Quotas negociadas (tokens, requests, bytes, etc.)
- ✅ Períodos de validade (mensal, anual, etc.)
- ✅ Integração com TerritoryFund para pagamento
- ✅ Votação para aprovar negociações (Fase 14)

#### 2. Pool de Quotas Territoriais
- ✅ Distribuição de quota negociada entre membros
- ✅ Políticas de distribuição (EQUAL, NEED_BASED, RESIDENT_ONLY, etc.)
- ✅ Reserva de quota para alocações específicas
- ✅ Rastreamento de uso e disponibilidade

#### 3. Alocação de Quotas para Membros
- ✅ Alocação automática (política EQUAL)
- ✅ Alocação baseada em necessidade (política NEED_BASED)
- ✅ Solicitação e aprovação de quotas (política VOTATION_BASED)
- ✅ Histórico de alocações

#### 4. Subsídios para Membros
- ✅ Identificação de membros sem quota pessoal
- ✅ Alocação automática de quota territorial (política NEED_BASED)
- ✅ Priorização de subsídios
- ✅ Rastreamento de subsídios

#### 5. Dashboard Territorial
- ✅ Serviços negociados pelo território
- ✅ Quota disponível por serviço
- ✅ Uso e consumo por membro
- ✅ Custos e subsídios

---

## 📋 Tarefas Detalhadas

### Semana 1: Modelo de Dados e Negociação

#### 28.1 Modelo de Domínio - Negociação Territorial
**Estimativa**: 24 horas (3 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar enum `AgreementStatus`:
  - [ ] `Pending = 1` (aguardando aprovação/pagamento)
  - [ ] `Active = 2` (ativo e disponível)
  - [ ] `Expired = 3` (expirado)
  - [ ] `Cancelled = 4` (cancelado)
  - [ ] `Suspended = 5` (suspenso)
- [ ] Criar enum `AgreementType`:
  - [ ] `Purchase = 1` (compra única)
  - [ ] `Subscription = 2` (assinatura recorrente)
  - [ ] `Grant = 3` (doação/concessão)
- [ ] Criar enum `QuotaDistributionPolicy`:
  - [ ] `Equal = 1` (divide igual entre membros)
  - [ ] `NeedBased = 2` (prioriza quem mais precisa)
  - [ ] `ResidentOnly = 3` (apenas moradores)
  - [ ] `VotationBased = 4` (distribuição por votação)
  - [ ] `FirstComeFirstServed = 5` (primeiro a chegar)
- [ ] Criar modelo `TerritoryServiceAgreement`:
  - [ ] `Id`, `TerritoryId`, `Category`, `Provider`
  - [ ] `AgreementType`, `TotalQuotaUnits`, `UnitsType`
  - [ ] `CostPerUnit`, `TotalCost`, `Currency`
  - [ ] `ValidFromUtc`, `ValidUntilUtc`, `IsRecurring`
  - [ ] `FundId` (nullable, fundo usado para pagar)
  - [ ] `PaidByUserId` (nullable), `PaidAtUtc` (nullable)
  - [ ] `ApprovedByVoteId` (nullable, votação que aprovou)
  - [ ] `CreatedByUserId`, `Status`, `CreatedAtUtc`
- [ ] Criar modelo `TerritoryServiceQuotaPool`:
  - [ ] `Id`, `AgreementId`, `TerritoryId`
  - [ ] `TotalQuotaUnits`, `UsedQuotaUnits`, `ReservedQuotaUnits`
  - [ ] `DistributionPolicy`, `RequiresVoteApproval` (bool)
  - [ ] `MaxUnitsPerUser` (int?, nullable)
  - [ ] `MaxUnitsPerRequest` (int?, nullable)
  - [ ] `PeriodStartUtc`, `PeriodEndUtc`
  - [ ] `CreatedAtUtc`, `UpdatedAtUtc`
- [ ] Criar modelos relacionados:
  - [ ] `TerritoryQuotaAllocation` (alocação para usuário)
  - [ ] `TerritoryQuotaUsageRequest` (solicitação de uso)
- [ ] Criar repositórios
- [ ] Criar migrations

**Arquivos a Criar**:
- `backend/Araponga.Domain/DigitalServices/TerritoryServiceAgreement.cs`
- `backend/Araponga.Domain/DigitalServices/TerritoryServiceQuotaPool.cs`
- `backend/Araponga.Domain/DigitalServices/TerritoryQuotaAllocation.cs`
- `backend/Araponga.Domain/DigitalServices/TerritoryQuotaUsageRequest.cs`
- `backend/Araponga.Domain/DigitalServices/AgreementStatus.cs`
- `backend/Araponga.Domain/DigitalServices/AgreementType.cs`
- `backend/Araponga.Domain/DigitalServices/QuotaDistributionPolicy.cs`
- `backend/Araponga.Domain/DigitalServices/AllocationStatus.cs`
- `backend/Araponga.Domain/DigitalServices/RequestStatus.cs`
- `backend/Araponga.Application/Interfaces/ITerritoryServiceAgreementRepository.cs`
- `backend/Araponga.Application/Interfaces/ITerritoryServiceQuotaPoolRepository.cs`
- `backend/Araponga.Application/Interfaces/ITerritoryQuotaAllocationRepository.cs`
- `backend/Araponga.Application/Interfaces/ITerritoryQuotaUsageRequestRepository.cs`

**Critérios de Sucesso**:
- ✅ Modelos criados
- ✅ Repositórios implementados
- ✅ Migrations criadas
- ✅ Testes de repositório passando

---

#### 28.2 Sistema de Negociação Territorial
**Estimativa**: 20 horas (2.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `TerritoryServiceNegotiationService`:
  - [ ] `ProposeAgreementAsync(Guid territoryId, Guid proposerUserId, ...)` → propor negociação
  - [ ] `CreateAgreementFromVoteAsync(Guid votingId, Guid territoryId)` → criar acordo após votação
  - [ ] `PurchaseServiceWithFundAsync(Guid agreementId, Guid fundId, ...)` → comprar com fundo
  - [ ] `ListAgreementsAsync(Guid territoryId, AgreementStatus? status)` → listar acordos
  - [ ] `GetAgreementAsync(Guid agreementId)` → obter acordo
  - [ ] `CancelAgreementAsync(Guid agreementId, Guid userId)` → cancelar acordo
- [ ] Integrar com Fase 14 (Votação):
  - [ ] Criar votação para aprovar negociação
  - [ ] Se aprovada, criar acordo
  - [ ] Tipo de votação: `ServicePurchase`
- [ ] Integrar com Fase 22 (TerritoryFund):
  - [ ] Verificar saldo do fundo
  - [ ] Debitar fundo ao comprar serviço
  - [ ] Criar transação no fundo
- [ ] Validações:
  - [ ] Apenas residents/curadores podem propor
  - [ ] Fundo deve ter saldo suficiente
  - [ ] Acordo deve ser válido
- [ ] Criar pool de quota automaticamente:
  - [ ] Quando acordo é ativado, criar pool
  - [ ] Aplicar política de distribuição
- [ ] Testes unitários

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/TerritoryServiceNegotiationService.cs`
- `backend/Araponga.Tests/Application/TerritoryServiceNegotiationServiceTests.cs`

**Critérios de Sucesso**:
- ✅ Sistema de negociação funcionando
- ✅ Integração com votação funcionando
- ✅ Integração com TerritoryFund funcionando
- ✅ Criação de pool funcionando
- ✅ Testes passando

---

### Semana 2: Pool de Quotas e Alocação

#### 28.3 Sistema de Pool de Quotas
**Estimativa**: 20 horas (2.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `TerritoryQuotaPoolService`:
  - [ ] `GetOrCreatePoolAsync(Guid agreementId, Guid territoryId)` → obter/criar pool
  - [ ] `GetAvailableQuotaAsync(Guid poolId)` → quota disponível
  - [ ] `ReserveQuotaAsync(Guid poolId, int units)` → reservar quota
  - [ ] `ReleaseQuotaAsync(Guid poolId, int units)` → liberar quota
  - [ ] `UseQuotaAsync(Guid poolId, Guid userId, int units, ...)` → usar quota
  - [ ] `GetPoolStatsAsync(Guid poolId)` → estatísticas do pool
- [ ] Políticas de distribuição:
  - [ ] `EQUAL`: Dividir igual entre membros ativos
  - [ ] `NEED_BASED`: Priorizar membros sem quota pessoal
  - [ ] `RESIDENT_ONLY`: Apenas moradores
  - [ ] `VOTATION_BASED`: Requer aprovação por votação
  - [ ] `FIRST_COME_FIRST_SERVED`: Primeiro a solicitar
- [ ] Integração com uso de serviços:
  - [ ] Verificar quota territorial antes de quota pessoal
  - [ ] Usar quota territorial se disponível
  - [ ] Fallback para quota pessoal
- [ ] Atualizar pool:
  - [ ] Decrementar ao usar
  - [ ] Atualizar estatísticas
- [ ] Testes unitários

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/TerritoryQuotaPoolService.cs`
- `backend/Araponga.Tests/Application/TerritoryQuotaPoolServiceTests.cs`

**Critérios de Sucesso**:
- ✅ Pool de quotas funcionando
- ✅ Políticas de distribuição funcionando
- ✅ Integração com uso de serviços funcionando
- ✅ Testes passando

---

#### 28.4 Sistema de Alocação e Subsídios
**Estimativa**: 20 horas (2.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `TerritoryQuotaAllocationService`:
  - [ ] `AllocateQuotaAsync(Guid poolId, Guid userId, int units, string reason, ...)` → alocar quota
  - [ ] `ListAllocationsAsync(Guid poolId, Guid? userId)` → listar alocações
  - [ ] `GetUserAllocationAsync(Guid poolId, Guid userId)` → alocação do usuário
  - [ ] `RevokeAllocationAsync(Guid allocationId, Guid userId, string reason)` → revogar alocação
- [ ] Alocação automática (política EQUAL):
  - [ ] Dividir quota igualmente entre membros ativos
  - [ ] Atualizar alocações ao adicionar/remover membros
- [ ] Alocação baseada em necessidade (política NEED_BASED):
  - [ ] Identificar membros sem quota pessoal
  - [ ] Priorizar membros com maior necessidade
  - [ ] Alocação automática ao solicitar serviço
- [ ] Alocação por votação (política VOTATION_BASED):
  - [ ] Criar solicitação de alocação
  - [ ] Requer aprovação por votação (Fase 14)
  - [ ] Alocar se aprovada
- [ ] Rastreamento de subsídios:
  - [ ] Marcar alocações como subsídio
  - [ ] Registrar quem recebeu subsídio
  - [ ] Dashboard de subsídios
- [ ] Testes unitários

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/TerritoryQuotaAllocationService.cs`
- `backend/Araponga.Tests/Application/TerritoryQuotaAllocationServiceTests.cs`

**Critérios de Sucesso**:
- ✅ Sistema de alocação funcionando
- ✅ Políticas de alocação funcionando
- ✅ Subsídios rastreados
- ✅ Testes passando

---

### Semana 3: Integração e Dashboard

#### 28.5 Integração com Uso de Serviços Digitais
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Atualizar `DigitalServiceManager` (Fase 26):
  - [ ] Verificar quota territorial antes de quota pessoal
  - [ ] Usar quota territorial se disponível
  - [ ] Rastrear uso em quota territorial
- [ ] Atualizar `ChatAIService` (Fase 27):
  - [ ] Verificar quota territorial antes de executar IA
  - [ ] Usar quota territorial se disponível
  - [ ] Indicar uso de quota territorial na resposta
- [ ] Priorização de quotas:
  - [ ] 1. Quota territorial (se disponível)
  - [ ] 2. Quota pessoal do usuário
  - [ ] 3. Bloquear se nenhuma disponível
- [ ] Rastreamento:
  - [ ] Registrar uso em `DigitalServiceUsageLog` com `TerritoryId`
  - [ ] Associar com `TerritoryQuotaPool`
  - [ ] Atualizar estatísticas do pool
- [ ] Notificações:
  - [ ] Notificar quando pool próximo ao esgotamento
  - [ ] Notificar quando subsídio é alocado
- [ ] Testes de integração

**Arquivos a Modificar**:
- `backend/Araponga.Application/Services/DigitalServiceManager.cs`
- `backend/Araponga.Application/Services/ChatAIService.cs` (se existir)

**Critérios de Sucesso**:
- ✅ Integração funcionando
- ✅ Priorização de quotas funcionando
- ✅ Rastreamento funcionando
- ✅ Testes passando

---

#### 28.6 Dashboard Territorial e Controllers
**Estimativa**: 20 horas (2.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `TerritoryServiceDashboardService`:
  - [ ] `GetTerritoryServicesAsync(Guid territoryId)` → serviços negociados
  - [ ] `GetServiceUsageStatsAsync(Guid territoryId, Guid? serviceId)` → estatísticas de uso
  - [ ] `GetSubsidiesReportAsync(Guid territoryId)` → relatório de subsídios
  - [ ] `GetCostAnalysisAsync(Guid territoryId, DateTime? periodStart, DateTime? periodEnd)` → análise de custos
- [ ] Dashboard inclui:
  - [ ] Serviços negociados (ativos, expirados)
  - [ ] Quota disponível por serviço
  - [ ] Uso por membro
  - [ ] Top consumidores
  - [ ] Subsídios concedidos
  - [ ] Custos e ROI
- [ ] Criar `TerritoryServiceAgreementController`:
  - [ ] `POST /api/v1/territories/{territoryId}/service-agreements` → propor negociação
  - [ ] `GET /api/v1/territories/{territoryId}/service-agreements` → listar acordos
  - [ ] `GET /api/v1/service-agreements/{agreementId}` → obter acordo
  - [ ] `POST /api/v1/service-agreements/{agreementId}/purchase` → comprar com fundo
  - [ ] `POST /api/v1/service-agreements/{agreementId}/cancel` → cancelar acordo
- [ ] Criar `TerritoryQuotaPoolController`:
  - [ ] `GET /api/v1/territories/{territoryId}/quota-pools` → listar pools
  - [ ] `GET /api/v1/quota-pools/{poolId}` → obter pool
  - [ ] `GET /api/v1/quota-pools/{poolId}/allocations` → listar alocações
  - [ ] `POST /api/v1/quota-pools/{poolId}/allocations` → criar alocação (se política permitir)
- [ ] Criar `TerritoryServiceDashboardController`:
  - [ ] `GET /api/v1/territories/{territoryId}/services/dashboard` → dashboard completo
- [ ] Feature flags: `DigitalServicesEnabled`, `TerritoryServiceNegotiationEnabled`
- [ ] Validações e permissões
- [ ] Testes de integração

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/TerritoryServiceDashboardService.cs`
- `backend/Araponga.Api/Controllers/TerritoryServiceAgreementController.cs`
- `backend/Araponga.Api/Controllers/TerritoryQuotaPoolController.cs`
- `backend/Araponga.Api/Controllers/TerritoryServiceDashboardController.cs`
- `backend/Araponga.Api/Contracts/TerritoryServices/ServiceAgreementResponse.cs`
- `backend/Araponga.Api/Contracts/TerritoryServices/QuotaPoolResponse.cs`
- `backend/Araponga.Api/Contracts/TerritoryServices/DashboardResponse.cs`

**Critérios de Sucesso**:
- ✅ Dashboard funcionando
- ✅ Controllers funcionando
- ✅ Validações funcionando
- ✅ Testes passando

---

## 📊 Resumo da Fase 28

| Tarefa | Estimativa | Status | Prioridade |
|--------|------------|--------|------------|
| Modelo de Domínio | 24h | ❌ Pendente | 🔴 Alta |
| Sistema de Negociação | 20h | ❌ Pendente | 🔴 Alta |
| Pool de Quotas | 20h | ❌ Pendente | 🔴 Alta |
| Alocação e Subsídios | 20h | ❌ Pendente | 🔴 Alta |
| Integração com Uso | 16h | ❌ Pendente | 🔴 Alta |
| Dashboard e Controllers | 20h | ❌ Pendente | 🔴 Alta |
| **Total** | **120h (21 dias)** | | |

---

## ✅ Critérios de Sucesso da Fase 28

### Funcionalidades
- ✅ Sistema de negociação territorial funcionando
- ✅ Pool de quotas compartilhado funcionando
- ✅ Políticas de distribuição funcionando
- ✅ Sistema de subsídios funcionando
- ✅ Dashboard territorial funcionando
- ✅ Integração com uso de serviços funcionando

### Qualidade
- ✅ Testes com cobertura adequada
- ✅ Documentação completa
- ✅ Validações e permissões implementadas
- Considerar **Testcontainers + PostgreSQL** para testes de integração (negociação territorial, quotas, subsídios) com banco real (estratégia na Fase 43; [TESTCONTAINERS_POSTGRES_IMPACTO](../../TESTCONTAINERS_POSTGRES_IMPACTO.md)).

### Integração
- ✅ Integração com TerritoryFund (Fase 22) funcionando
- ✅ Integração com Votação (Fase 14) funcionando
- ✅ Integração com Serviços Digitais (Fase 26) funcionando
- ✅ Integração com Chat com IA (Fase 27) funcionando

---

## 🔗 Dependências

- **Fase 26**: Serviços Digitais Base (infraestrutura, rastreamento)
- **Fase 22**: TerritoryFund (pagamento de serviços)
- **Fase 14**: Votação (aprovação de negociações)

---

## 📝 Notas de Implementação

### Fluxo de Negociação Territorial

**Exemplo**:
1. Curador propõe: "Comprar 2M tokens OpenAI/mês"
2. Comunidade vota (Fase 14)
3. Se aprovada, acordo é criado
4. Território paga com TerritoryFund (Fase 22)
5. Pool de quota é criado automaticamente
6. Quota é distribuída conforme política
7. Membros usam quota territorial
8. Dashboard mostra uso e custos

### Políticas de Distribuição

**EQUAL**:
- Divide quota igualmente entre membros ativos
- Atualização automática ao adicionar/remover membros
- Exemplo: 2M tokens ÷ 100 membros = 20K tokens/membro

**NEED_BASED**:
- Identifica membros sem quota pessoal
- Prioriza membros com maior necessidade
- Alocação automática ao solicitar serviço
- Subsídio rastreado

**VOTATION_BASED**:
- Requer aprovação por votação para cada alocação
- Maior controle e transparência
- Processo mais lento, mas mais democrático

---

**Status**: ⏳ **FASE 28 PENDENTE**  
**Depende de**: Fases 26, 22, 14  
**Crítico para**: Economia de Escala e Inclusão Digital
