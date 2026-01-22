# Fase 28: Banco de Sementes e Mudas Territorial

**Duração**: 4 semanas (28 dias úteis)  
**Prioridade**: 🟡 MÉDIA-ALTA (Soberania alimentar e economia circular)  
**Depende de**: TerritoryAsset (existe), Marketplace (existe), Fase 17 (Gamificação), WorkQueue (existe)  
**Estimativa Total**: 144-180 horas  
**Status**: ⏳ Pendente

---

## 🎯 Objetivo

Implementar sistema de **banco de sementes e mudas territorial** que permite:
- Catalogação e preservação de variedades locais
- Doação e troca de sementes entre membros
- Integração com marketplace para trocas
- Rastreabilidade de origem e multiplicação
- Eventos de troca comunitários
- Integração harmoniosa com gamificação, workqueue, notificações, alertas, postagens e chat

**Princípios**:
- ✅ **Soberania Alimentar**: Preservação de variedades locais
- ✅ **Economia Circular**: Troca sem dinheiro
- ✅ **Rastreabilidade**: Origem e multiplicação registradas
- ✅ **Cuidado Coletivo**: Recurso compartilhado pelo território
- ✅ **Integração Harmoniosa**: Todos os sistemas trabalham juntos

---

## 📋 Contexto e Requisitos

### Estado Atual
- ✅ TerritoryAsset existe (base para SeedBank)
- ✅ Marketplace existe (troca de sementes)
- ✅ WorkQueue existe (revisão de doações)
- ✅ Notificações existe (alertas e notificações)
- ✅ Alertas existe (alertas territoriais)
- ✅ Postagens existe (posts no feed)
- ✅ Chat existe (comunicação)
- ✅ Gamificação planejada (Fase 17)
- ❌ Não existe sistema de banco de sementes
- ❌ Não existe catalogação de sementes
- ❌ Não existe rastreabilidade de sementes

### Requisitos Funcionais

#### 1. Banco de Sementes como TerritoryAsset
- ✅ SeedBank especializa TerritoryAsset
- ✅ Tipos de banco (COLLECTIVE, INDIVIDUAL, PRESERVATION)
- ✅ Localização física do banco
- ✅ Guardião/curador do banco
- ✅ Status do banco (ACTIVE, FULL, LOW_STOCK, CLOSED)

#### 2. Catálogo de Sementes
- ✅ Informações da semente (espécie, variedade, origem)
- ✅ Características (tipo, estação, clima)
- ✅ Qualidade e viabilidade (germinação, validade)
- ✅ Estoque e disponibilidade
- ✅ Rastreabilidade (quem doou, quando, multiplicação)

#### 3. Sistema de Doações
- ✅ Usuários doam sementes para o banco
- ✅ Revisão via WorkQueue
- ✅ Aceitação/rejeição por curadores
- ✅ Geração de contribuições (gamificação)

#### 4. Sistema de Solicitações
- ✅ Usuários solicitam sementes do banco
- ✅ Aprovação automática ou por votação
- ✅ Retirada de sementes
- ✅ Compromisso de devolução (opcional)

#### 5. Integração com Marketplace
- ✅ Sementes como ItemType.SEED
- ✅ Preço 0 para doações
- ✅ Trocas via marketplace
- ✅ Moeda territorial para venda (opcional)

#### 6. Integração com WorkQueue
- ✅ WorkItem para revisão de doações
- ✅ WorkItem para solicitações raras
- ✅ Fluxo de aprovação

#### 7. Integração com Gamificação
- ✅ Doação de sementes: +10-25 pontos
- ✅ Multiplicação de sementes: +15 pontos
- ✅ Evento de troca organizado: +50 pontos

#### 8. Integração com Notificações
- ✅ `seed.donation.received` (doação aceita)
- ✅ `seed.request.approved` (solicitação aprovada)
- ✅ `seed.event.created` (evento de troca criado)

#### 9. Integração com Alertas
- ✅ Alerta de estoque baixo
- ✅ Alerta de nova variedade disponível
- ✅ Alerta de evento de troca próximo

#### 10. Integração com Postagens
- ✅ Post pode referenciar SeedCatalog
- ✅ Plantio gera post automaticamente
- ✅ Sementes aparecem no feed

#### 11. Integração com Chat
- ✅ Compartilhar informações sobre sementes
- ✅ Eventos de troca via chat
- ✅ Contexto territorial no chat

#### 12. Eventos de Trocas
- ✅ SeedSwapEvent (especialização de Event)
- ✅ Integração com sistema de eventos
- ✅ Catálogo de sementes no evento

---

## 📋 Tarefas Detalhadas

### Semana 1-2: Modelo de Dados e SeedBank

#### 28.1 Modelo de Domínio - Banco de Sementes
**Estimativa**: 32 horas (4 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar enum `SeedBankType`:
  - [ ] `Collective = 1` (banco comunitário)
  - [ ] `Individual = 2` (banco pessoal)
  - [ ] `Preservation = 3` (preservação)
- [ ] Criar enum `SeedBankStatus`:
  - [ ] `Active = 1`, `Full = 2`, `LowStock = 3`, `Closed = 4`
- [ ] Criar enum `SeedType`:
  - [ ] `Vegetable = 1`, `Fruit = 2`, `Grain = 3`, `Herb = 4`
  - [ ] `Flower = 5`, `Native = 6`, `Medicinal = 7`, `Tree = 8`, `Other = 99`
- [ ] Criar enum `SeedQuality`:
  - [ ] `Excellent = 1` (>90%), `Good = 2` (70-90%), `Fair = 3` (50-70%), `Poor = 4` (<50%)
- [ ] Criar enum `DonationStatus`:
  - [ ] `Pending = 1`, `Accepted = 2`, `Rejected = 3`
- [ ] Criar enum `RequestStatus`:
  - [ ] `Pending = 1`, `Approved = 2`, `Rejected = 3`, `Completed = 4`, `Cancelled = 5`
- [ ] Criar enum `PlantingStatus`:
  - [ ] `Planted = 1`, `Germinating = 2`, `Growing = 3`, `Harvested = 4`, `Failed = 5`
- [ ] Criar modelo `SeedBank`:
  - [ ] Especializa `TerritoryAsset` (Type = "seed_bank")
  - [ ] `SeedBankType`, `Location` (string?), `ManagedByUserId` (Guid?)
  - [ ] `BankStatus` (SeedBankStatus)
- [ ] Criar modelo `SeedCatalog`:
  - [ ] `Id`, `SeedBankId`, `TerritoryId`
  - [ ] `SpeciesName`, `CommonName`, `Variety`, `Origin`
  - [ ] `Type` (SeedType), `GrowingSeason`, `ClimateZone`, `Description`
  - [ ] `DonatedByUserId`, `DonatedAtUtc`, `DonationNotes`, `Generation`
  - [ ] `TotalQuantity`, `AvailableQuantity`, `ReservedQuantity`, `Unit`
  - [ ] `Quality`, `HarvestDate`, `ExpiryDate`, `GerminationRate`
  - [ ] `CreatedAtUtc`, `UpdatedAtUtc`, `IsActive`
- [ ] Criar modelo `SeedDonation`:
  - [ ] `Id`, `SeedBankId`, `SeedCatalogId?`, `TerritoryId`, `DonorUserId`
  - [ ] `Quantity`, `Unit`, `Type`, `SpeciesName`, `CommonName`, `Variety`, `Origin`
  - [ ] `HarvestDate`, `ExpiryDate`, `GerminationRate`, `Quality`, `Notes`
  - [ ] `Status`, `ReviewedByUserId`, `ReviewedAtUtc`, `RejectionReason`
  - [ ] `ContributionId?`, `ContributionPoints?`
  - [ ] `CreatedAtUtc`, `UpdatedAtUtc`
- [ ] Criar modelo `SeedRequest`:
  - [ ] `Id`, `SeedBankId`, `SeedCatalogId`, `TerritoryId`, `RequesterUserId`
  - [ ] `RequestedQuantity`, `Purpose`, `Notes`
  - [ ] `Status`, `ApprovedByUserId`, `ApprovedByVoteId`, `ApprovedAtUtc`, `RejectionReason`
  - [ ] `UsageLogId?`, `ActualQuantityGiven`, `WithdrawnAtUtc`, `WithdrawnByUserId`
  - [ ] `RequiresReturn`, `ReturnQuantity`, `ReturnDueDate`, `ReturnDonationId`
  - [ ] `CreatedAtUtc`, `UpdatedAtUtc`
- [ ] Criar modelo `SeedPlanting`:
  - [ ] `Id`, `SeedRequestId`, `SeedCatalogId`, `TerritoryId`, `PlanterUserId`
  - [ ] `PlantedAtUtc`, `LocationLat`, `LocationLng`, `LocationDescription`, `QuantityPlanted`
  - [ ] `Status`, `GerminatedCount`, `HarvestedQuantity`, `HarvestDateUtc`
  - [ ] `WillReturnSeeds`, `ReturnDonationId`
  - [ ] `CreatedAtUtc`, `UpdatedAtUtc`
- [ ] Criar repositórios
- [ ] Criar migrations

**Arquivos a Criar**:
- `backend/Araponga.Domain/Seeds/SeedBank.cs`
- `backend/Araponga.Domain/Seeds/SeedCatalog.cs`
- `backend/Araponga.Domain/Seeds/SeedDonation.cs`
- `backend/Araponga.Domain/Seeds/SeedRequest.cs`
- `backend/Araponga.Domain/Seeds/SeedPlanting.cs`
- `backend/Araponga.Domain/Seeds/SeedBankType.cs`
- `backend/Araponga.Domain/Seeds/SeedBankStatus.cs`
- `backend/Araponga.Domain/Seeds/SeedType.cs`
- `backend/Araponga.Domain/Seeds/SeedQuality.cs`
- `backend/Araponga.Domain/Seeds/DonationStatus.cs`
- `backend/Araponga.Domain/Seeds/RequestStatus.cs`
- `backend/Araponga.Domain/Seeds/PlantingStatus.cs`
- `backend/Araponga.Application/Interfaces/ISeedBankRepository.cs`
- `backend/Araponga.Application/Interfaces/ISeedCatalogRepository.cs`
- `backend/Araponga.Application/Interfaces/ISeedDonationRepository.cs`
- `backend/Araponga.Application/Interfaces/ISeedRequestRepository.cs`
- `backend/Araponga.Application/Interfaces/ISeedPlantingRepository.cs`

**Critérios de Sucesso**:
- ✅ Modelos criados
- ✅ Repositórios implementados
- ✅ Migrations criadas
- ✅ Testes de repositório passando

---

#### 28.2 Sistema de Banco de Sementes
**Estimativa**: 24 horas (3 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `SeedBankService`:
  - [ ] `CreateSeedBankAsync(Guid territoryId, Guid userId, ...)` → criar banco
  - [ ] `ListSeedBanksAsync(Guid territoryId)` → listar bancos
  - [ ] `GetSeedBankAsync(Guid bankId)` → obter banco
  - [ ] `UpdateSeedBankStatusAsync(Guid bankId, SeedBankStatus status, Guid userId)` → atualizar status
- [ ] Integrar com `TerritoryAssetService`:
  - [ ] SeedBank cria TerritoryAsset automaticamente
  - [ ] Type = "seed_bank"
  - [ ] Aparece no mapa territorial
- [ ] Validações:
  - [ ] Apenas residents/curadores podem criar bancos
  - [ ] Banco deve ter guardião
- [ ] Testes unitários

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/SeedBankService.cs`
- `backend/Araponga.Tests/Application/SeedBankServiceTests.cs`

**Critérios de Sucesso**:
- ✅ Serviço de banco funcionando
- ✅ Integração com TerritoryAsset funcionando
- ✅ Testes passando

---

### Semana 2-3: Doações, Solicitações e Integrações

#### 28.3 Sistema de Doações de Sementes
**Estimativa**: 24 horas (3 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `SeedDonationService`:
  - [ ] `DonateSeedsAsync(Guid bankId, Guid userId, ...)` → doar sementes
  - [ ] `ListDonationsAsync(Guid bankId, DonationStatus? status)` → listar doações
  - [ ] `ReviewDonationAsync(Guid donationId, Guid reviewerUserId, bool accept, string? reason)` → revisar doação
  - [ ] `AcceptDonationAsync(Guid donationId, Guid reviewerUserId)` → aceitar doação
  - [ ] `RejectDonationAsync(Guid donationId, Guid reviewerUserId, string reason)` → rejeitar doação
- [ ] Integração com WorkQueue:
  - [ ] Criar WorkItem ao receber doação
  - [ ] Type = `SEED_DONATION_REVIEW`
  - [ ] Curadores revisam via WorkQueue
- [ ] Integração com Gamificação (Fase 17):
  - [ ] Doação aceita gera contribuição
  - [ ] `ContributionType.SeedDonation` (+10 pontos)
  - [ ] Variedade rara: +25 pontos
- [ ] Integração com Notificações:
  - [ ] Notificar doador quando doação aceita/rejeitada
  - [ ] Tipo: `seed.donation.received`
- [ ] Atualizar catálogo:
  - [ ] Criar novo SeedCatalog se não existir
  - [ ] Atualizar estoque se existir
- [ ] Testes unitários

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/SeedDonationService.cs`
- `backend/Araponga.Tests/Application/SeedDonationServiceTests.cs`

**Critérios de Sucesso**:
- ✅ Sistema de doações funcionando
- ✅ Integração com WorkQueue funcionando
- ✅ Integração com Gamificação funcionando
- ✅ Notificações funcionando
- ✅ Testes passando

---

#### 28.4 Sistema de Solicitações de Sementes
**Estimativa**: 24 horas (3 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `SeedRequestService`:
  - [ ] `RequestSeedsAsync(Guid bankId, Guid catalogId, Guid userId, ...)` → solicitar sementes
  - [ ] `ListRequestsAsync(Guid bankId, RequestStatus? status)` → listar solicitações
  - [ ] `ApproveRequestAsync(Guid requestId, Guid approverUserId)` → aprovar solicitação
  - [ ] `RejectRequestAsync(Guid requestId, Guid approverUserId, string reason)` → rejeitar
  - [ ] `CompleteRequestAsync(Guid requestId, Guid userId, int actualQuantity)` → completar retirada
- [ ] Políticas de aprovação:
  - [ ] Auto-aprovação se estoque disponível e política permitir
  - [ ] Requer aprovação de curador se política exigir
  - [ ] Requer votação se variedade rara (integração futura Fase 14)
- [ ] Integração com WorkQueue:
  - [ ] Criar WorkItem para solicitações raras
  - [ ] Type = `SEED_REQUEST_REVIEW`
- [ ] Integração com Notificações:
  - [ ] Notificar solicitante quando aprovada/rejeitada
  - [ ] Tipo: `seed.request.approved`
- [ ] Atualizar estoque:
  - [ ] Reservar quantidade ao aprovar
  - [ ] Decrementar ao completar retirada
- [ ] Compromisso de devolução:
  - [ ] Registrar se usuário comprometeu devolver
  - [ ] Criar alerta para data de devolução
- [ ] Testes unitários

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/SeedRequestService.cs`
- `backend/Araponga.Tests/Application/SeedRequestServiceTests.cs`

**Critérios de Sucesso**:
- ✅ Sistema de solicitações funcionando
- ✅ Políticas de aprovação funcionando
- ✅ Integração com WorkQueue funcionando
- ✅ Notificações funcionando
- ✅ Testes passando

---

#### 28.5 Integração com Marketplace
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Adicionar `ItemType.Seed` ao enum `ItemType`:
  - [ ] Sementes como novo tipo de item
- [ ] Criar `SeedMarketplaceService`:
  - [ ] `ListSeedsInMarketplaceAsync(Guid territoryId, ...)` → listar sementes no marketplace
  - [ ] `AddSeedToMarketplaceAsync(Guid catalogId, Guid userId, decimal? price, ...)` → adicionar ao marketplace
  - [ ] Integração com sistema de items existente
- [ ] Preços:
  - [ ] Preço 0 para doações
  - [ ] Preço opcional para venda (moeda territorial - Fase 20)
- [ ] Atualizar `StoreItem`:
  - [ ] Permitir ItemType.Seed
  - [ ] Referenciar SeedCatalog
- [ ] Integração com checkout:
  - [ ] Atualizar estoque ao vender semente
  - [ ] Notificar banco de sementes
- [ ] Testes de integração

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/SeedMarketplaceService.cs`
- `backend/Araponga.Tests/Integration/SeedMarketplaceIntegrationTests.cs`

**Arquivos a Modificar**:
- `backend/Araponga.Domain/Marketplace/ItemType.cs` (adicionar Seed)
- `backend/Araponga.Application/Services/StoreItemService.cs` (suporte a Seed)

**Critérios de Sucesso**:
- ✅ Sementes no marketplace funcionando
- ✅ Trocas via marketplace funcionando
- ✅ Integração com checkout funcionando
- ✅ Testes passando

---

### Semana 3-4: Rastreabilidade e Integrações Finais

#### 28.6 Sistema de Plantio e Rastreabilidade
**Estimativa**: 20 horas (2.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `SeedPlantingService`:
  - [ ] `RegisterPlantingAsync(Guid requestId, Guid userId, ...)` → registrar plantio
  - [ ] `UpdatePlantingStatusAsync(Guid plantingId, PlantingStatus status, ...)` → atualizar status
  - [ ] `RecordHarvestAsync(Guid plantingId, int harvestedQuantity, ...)` → registrar colheita
  - [ ] `ListPlantingsAsync(Guid territoryId, Guid? userId)` → listar plantios
- [ ] Rastreabilidade:
  - [ ] Registrar origem da semente (SeedRequest)
  - [ ] Registrar localização do plantio (geo)
  - [ ] Registrar resultado (germinação, colheita)
  - [ ] Rastrear gerações (quantas vezes foi multiplicada)
- [ ] Integração com Postagens:
  - [ ] Plantio pode gerar post automaticamente
  - [ ] Post referencia SeedPlanting
  - [ ] Aparece no feed territorial
- [ ] Compromisso de devolução:
  - [ ] Verificar se usuário comprometeu devolver
  - [ ] Criar SeedDonation quando devolver
  - [ ] Gerar contribuição ao devolver
- [ ] Integração com Gamificação:
  - [ ] Plantio bem-sucedido: +15 pontos (Fase 17)
  - [ ] Multiplicação de sementes: +15 pontos
- [ ] Testes unitários

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/SeedPlantingService.cs`
- `backend/Araponga.Tests/Application/SeedPlantingServiceTests.cs`

**Critérios de Sucesso**:
- ✅ Sistema de plantio funcionando
- ✅ Rastreabilidade funcionando
- ✅ Integração com postagens funcionando
- ✅ Testes passando

---

#### 28.7 Eventos de Trocas de Sementes
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar modelo `SeedSwapEvent`:
  - [ ] Especializa `Event` (tipo de evento)
  - [ ] `SeedBankId?` (banco organizador)
  - [ ] `ParticipatingSeedBanks` (List<Guid>)
  - [ ] `IsOrganizedByTerritory` (bool)
  - [ ] `ExpectedParticipants` (int?)
  - [ ] `AvailableSeedCatalogs` (List<Guid>)
  - [ ] `RequiresPreRegistration` (bool)
  - [ ] `SwapRules` (string?)
  - [ ] `AllowsSale` (bool)
  - [ ] `UsesTerritoryCurrency` (bool, Fase 20)
- [ ] Criar `SeedSwapEventService`:
  - [ ] `CreateEventAsync(Guid territoryId, Guid organizerUserId, ...)` → criar evento
  - [ ] `ListEventsAsync(Guid territoryId, ...)` → listar eventos
  - [ ] `AddSeedsToEventAsync(Guid eventId, List<Guid> catalogIds)` → adicionar sementes
- [ ] Integração com Events existente:
  - [ ] SeedSwapEvent aparece na lista de eventos
  - [ ] Participações funcionam normalmente
  - [ ] Aparece no feed e mapa
- [ ] Integração com Gamificação:
  - [ ] Organizar evento: +50 pontos (Fase 17)
  - [ ] Participar de evento: +10 pontos
- [ ] Integração com Notificações:
  - [ ] Notificar quando evento criado
  - [ ] Tipo: `seed.event.created`
- [ ] Integração com Alertas:
  - [ ] Alerta quando evento próximo (3 dias antes)
- [ ] Testes de integração

**Arquivos a Criar**:
- `backend/Araponga.Domain/Seeds/SeedSwapEvent.cs`
- `backend/Araponga.Application/Services/SeedSwapEventService.cs`
- `backend/Araponga.Tests/Integration/SeedSwapEventIntegrationTests.cs`

**Arquivos a Modificar**:
- `backend/Araponga.Domain/Events/Event.cs` (extensão opcional)

**Critérios de Sucesso**:
- ✅ Eventos de troca funcionando
- ✅ Integração com Events funcionando
- ✅ Gamificação funcionando
- ✅ Notificações funcionando
- ✅ Testes passando

---

#### 28.8 Integração com Alertas e Notificações
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `SeedBankAlertService`:
  - [ ] `CheckStockLevelsAsync(Guid bankId)` → verificar estoques
  - [ ] `CreateLowStockAlertAsync(Guid bankId, Guid catalogId)` → alerta de estoque baixo
  - [ ] `CreateNewVarietyAlertAsync(Guid bankId, Guid catalogId)` → alerta de nova variedade
- [ ] Integração com Alertas:
  - [ ] Alerta de estoque baixo (<10 unidades)
  - [ ] Alerta de nova variedade disponível
  - [ ] Alerta de evento de troca próximo
- [ ] Integração com Notificações:
  - [ ] Novos tipos de notificação:
    - [ ] `seed.donation.received` (doação aceita)
    - [ ] `seed.request.approved` (solicitação aprovada)
    - [ ] `seed.event.created` (evento criado)
    - [ ] `seed.stock.low` (estoque baixo)
    - [ ] `seed.variety.available` (nova variedade)
- [ ] Notificações automáticas:
  - [ ] Notificar doador quando doação aceita
  - [ ] Notificar solicitante quando solicitação aprovada
  - [ ] Notificar membros quando evento criado
  - [ ] Notificar guardião quando estoque baixo
- [ ] Testes de integração

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/SeedBankAlertService.cs`
- `backend/Araponga.Tests/Integration/SeedBankAlertIntegrationTests.cs`

**Arquivos a Modificar**:
- `backend/Araponga.Domain/Users/NotificationPreferences.cs` (adicionar preferências de sementes - opcional)

**Critérios de Sucesso**:
- ✅ Alertas funcionando
- ✅ Notificações funcionando
- ✅ Integração harmoniosa
- ✅ Testes passando

---

#### 28.9 Controllers e Dashboard
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `SeedBankController`:
  - [ ] `POST /api/v1/seed-banks` → criar banco
  - [ ] `GET /api/v1/territories/{territoryId}/seed-banks` → listar bancos
  - [ ] `GET /api/v1/seed-banks/{bankId}` → obter banco
  - [ ] `PATCH /api/v1/seed-banks/{bankId}/status` → atualizar status
- [ ] Criar `SeedCatalogController`:
  - [ ] `GET /api/v1/seed-banks/{bankId}/catalogs` → listar catálogo
  - [ ] `GET /api/v1/seed-catalogs/{catalogId}` → obter semente
- [ ] Criar `SeedDonationController`:
  - [ ] `POST /api/v1/seed-banks/{bankId}/donations` → doar sementes
  - [ ] `GET /api/v1/seed-banks/{bankId}/donations` → listar doações
  - [ ] `PATCH /api/v1/seed-donations/{donationId}/review` → revisar doação
- [ ] Criar `SeedRequestController`:
  - [ ] `POST /api/v1/seed-banks/{bankId}/requests` → solicitar sementes
  - [ ] `GET /api/v1/seed-banks/{bankId}/requests` → listar solicitações
  - [ ] `PATCH /api/v1/seed-requests/{requestId}/approve` → aprovar
  - [ ] `POST /api/v1/seed-requests/{requestId}/complete` → completar retirada
- [ ] Criar `SeedPlantingController`:
  - [ ] `POST /api/v1/seed-plantings` → registrar plantio
  - [ ] `GET /api/v1/seed-plantings` → listar plantios
  - [ ] `PATCH /api/v1/seed-plantings/{plantingId}/status` → atualizar status
- [ ] Feature flags: `SeedBankEnabled`, `SeedBankCollectiveEnabled`
- [ ] Validações e permissões
- [ ] Testes de integração

**Arquivos a Criar**:
- `backend/Araponga.Api/Controllers/SeedBankController.cs`
- `backend/Araponga.Api/Controllers/SeedCatalogController.cs`
- `backend/Araponga.Api/Controllers/SeedDonationController.cs`
- `backend/Araponga.Api/Controllers/SeedRequestController.cs`
- `backend/Araponga.Api/Controllers/SeedPlantingController.cs`
- `backend/Araponga.Api/Contracts/Seeds/SeedBankResponse.cs`
- `backend/Araponga.Api/Contracts/Seeds/SeedCatalogResponse.cs`
- `backend/Araponga.Api/Contracts/Seeds/SeedDonationRequest.cs`
- `backend/Araponga.Api/Contracts/Seeds/SeedRequestRequest.cs`
- `backend/Araponga.Api/Contracts/Seeds/SeedPlantingRequest.cs`

**Critérios de Sucesso**:
- ✅ Controllers funcionando
- ✅ Validações funcionando
- ✅ Feature flags funcionando
- ✅ Testes passando

---

## 📊 Resumo da Fase 28

| Tarefa | Estimativa | Status | Prioridade |
|--------|------------|--------|------------|
| Modelo de Domínio | 32h | ❌ Pendente | 🔴 Alta |
| Sistema de Banco | 24h | ❌ Pendente | 🔴 Alta |
| Sistema de Doações | 24h | ❌ Pendente | 🔴 Alta |
| Sistema de Solicitações | 24h | ❌ Pendente | 🔴 Alta |
| Integração Marketplace | 16h | ❌ Pendente | 🟡 Média |
| Plantio e Rastreabilidade | 20h | ❌ Pendente | 🟡 Média |
| Eventos de Trocas | 16h | ❌ Pendente | 🟡 Média |
| Alertas e Notificações | 12h | ❌ Pendente | 🟡 Média |
| Controllers e Dashboard | 16h | ❌ Pendente | 🔴 Alta |
| **Total** | **184h (28 dias)** | | |

---

## ✅ Critérios de Sucesso da Fase 28

### Funcionalidades
- ✅ Sistema completo de banco de sementes funcionando
- ✅ Doações e solicitações funcionando
- ✅ Rastreabilidade funcionando
- ✅ Eventos de troca funcionando
- ✅ Todas as integrações funcionando harmoniosamente

### Qualidade
- ✅ Testes com cobertura adequada
- ✅ Documentação completa
- ✅ Validações e permissões implementadas
- Considerar **Testcontainers + PostgreSQL** para testes de integração (banco de sementes, catálogo, WorkQueue) com banco real (estratégia na Fase 19; [TESTCONTAINERS_POSTGRES_IMPACTO](../../TESTCONTAINERS_POSTGRES_IMPACTO.md)).

### Integração
- ✅ Integração com TerritoryAsset funcionando
- ✅ Integração com Marketplace funcionando
- ✅ Integração com WorkQueue funcionando
- ✅ Integração com Gamificação (Fase 17) funcionando
- ✅ Integração com Notificações funcionando
- ✅ Integração com Alertas funcionando
- ✅ Integração com Postagens funcionando
- ✅ Integração com Chat funcionando
- ✅ Integração com Events funcionando

---

## 🔗 Dependências

- **TerritoryAsset**: Base para SeedBank
- **Marketplace**: Trocas de sementes
- **WorkQueue**: Revisão de doações
- **Fase 17**: Gamificação (contribuições por sementes)
- **Events**: Eventos de troca (já existe)
- **Notificações**: Alertas e notificações (já existe)
- **Alertas**: Alertas territoriais (já existe)
- **Postagens**: Posts no feed (já existe)
- **Chat**: Comunicação (já existe)

---

## 📝 Notas de Implementação

### Integração Harmoniosa com Sistemas Existentes

**TerritoryAsset**:
- SeedBank é um tipo especializado de TerritoryAsset
- Type = "seed_bank"
- Aparece no mapa territorial
- Pode ter GeoAnchors

**Marketplace**:
- Sementes aparecem como ItemType.Seed
- Preço 0 para doações
- Preço opcional para venda (moeda territorial)
- Integração completa com checkout

**WorkQueue**:
- Doações criam WorkItem para revisão
- Solicitações raras criam WorkItem
- Curadores revisam via WorkQueue existente

**Gamificação**:
- Doações geram contribuições
- Plantios bem-sucedidos geram contribuições
- Eventos geram contribuições
- Integração com ContributionService (Fase 17)

**Notificações e Alertas**:
- Novos tipos de notificação
- Alertas automáticos
- Integração com sistema existente

**Postagens e Chat**:
- Plantios podem gerar posts
- Eventos aparecem no feed
- Chat pode compartilhar informações sobre sementes

---

**Status**: ⏳ **FASE 28 PENDENTE**  
**Depende de**: TerritoryAsset, Marketplace, WorkQueue, Fase 17  
**Crítico para**: Soberania Alimentar e Economia Circular
