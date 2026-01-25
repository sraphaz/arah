# Fase 17: Sistema de Compra Coletiva e Organização Comunitária de Alimentos

**Duração**: 4 semanas (28 dias úteis)  
**Prioridade**: 🔴 CRÍTICA (Economia local e soberania alimentar)  
**Depende de**: Fase 6 (Marketplace), Fase 14 (Governança/Votação), Fase 22 (Moeda Territorial)  
**Integra com**: Fase 21 (Entregas) - opcional, pode ser feito depois  
**Estimativa Total**: 160 horas  
**Status**: ⏳ Pendente  
**Nota**: Renumerada de Fase 23 para Fase 17, priorizada de P1 para P0 (Onda 3: Economia Local)

---

## 🎯 Objetivo

Implementar sistema de **compra coletiva de alimentos** que:
- Conecta produtores locais (moradores ou visitantes) com consumidores do território
- Organiza a comunidade para indicar interesse de compra
- Implementa agenda de compras comunitárias
- Sistema de opt-in/opt-out para participantes
- Integração com sistema de votação (Fase 14) para decisões coletivas
- Integração com sistema de entregas (Fase 21) para distribuição
- Integração com moeda territorial (Fase 22) para pagamentos
- Gamificação de participação (Fase 42)

**Princípios**:
- ✅ **Economia Local**: Fortalece produtores locais
- ✅ **Soberania Alimentar**: Comunidade decide o que comprar
- ✅ **Organização Comunitária**: Decisões coletivas via votação
- ✅ **Transparência**: Todos veem o que está sendo comprado
- ✅ **Sustentabilidade**: Reduz desperdício e transporte

---

## 📋 Contexto e Requisitos

### Estado Atual
- ✅ Sistema de marketplace (Fase 6)
- ✅ Sistema de votação (Fase 14)
- ✅ Sistema de entregas (Fase 21)
- ✅ Sistema de moeda territorial (Fase 22)
- ✅ Sistema de gamificação (Fase 42)
- ❌ Não existe sistema de compra coletiva
- ❌ Não existe sistema de organização comunitária de alimentos
- ❌ Não existe agenda de compras comunitárias

### Requisitos Funcionais

#### 1. Sistema de Produtores
- ✅ Registrar produtor (morador ou visitante)
- ✅ Cadastrar produtos (tipo, quantidade, preço, sazonalidade)
- ✅ Disponibilidade (quando está disponível)
- ✅ Localização do produtor
- ✅ Métodos de pagamento (moeda territorial, fiat, ambos)

#### 2. Sistema de Compra Coletiva
- ✅ Criar rodada de compra coletiva (organizador)
- ✅ Definir produtos disponíveis (do catálogo de produtores)
- ✅ Definir prazo para indicação de interesse
- ✅ Definir quantidade mínima para viabilizar compra
- ✅ Status: PLANNING, COLLECTING_INTERESTS, CONFIRMED, IN_DELIVERY, COMPLETED, CANCELLED

#### 3. Sistema de Interesse de Compra
- ✅ Usuários indicam interesse (opt-in)
- ✅ Quantidade desejada por produto
- ✅ Confirmação de interesse (antes do prazo)
- ✅ Cancelamento de interesse (antes do prazo)
- ✅ Notificações sobre status da compra

#### 4. Sistema de Agenda de Compras
- ✅ Agenda de rodadas de compra (mensal, quinzenal, semanal)
- ✅ Calendário de compras comunitárias
- ✅ Lembretes automáticos
- ✅ Histórico de compras

#### 5. Integração com Votação
- ✅ Votação para escolher produtos (quais produtos comprar)
- ✅ Votação para escolher produtores (qual produtor escolher)
- ✅ Votação para definir frequência de compras
- ✅ Votação para aprovar organizadores

#### 6. Integração com Entregas
- ✅ Organizar entrega coletiva (Fase 21)
- ✅ Rota otimizada para entregas
- ✅ Pontos de entrega comunitários
- ✅ Entregadores podem ser participantes

#### 7. Integração com Moeda Territorial
- ✅ Pagamento em moeda territorial
- ✅ Desconto para pagamento em moeda territorial
- ✅ Fundos territoriais podem subsidiar compras

#### 8. Gamificação
- ✅ Participação em compra coletiva gera contribuição
- ✅ Organizar compra coletiva gera mais pontos
- ✅ Comprar de produtor local gera mais pontos

---

## 📋 Tarefas Detalhadas

### Semana 1-2: Modelo de Domínio e Produtores

#### 17.1 Modelo de Domínio - Compra Coletiva
**Estimativa**: 32 horas (4 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar enum `ProducerType`:
  - [ ] `RESIDENT` (morador)
  - [ ] `VISITOR` (visitante)
- [ ] Criar enum `ProductCategory`:
  - [ ] `VEGETABLES` (vegetais)
  - [ ] `FRUITS` (frutas)
  - [ ] `GRAINS` (grãos)
  - [ ] `DAIRY` (laticínios)
  - [ ] `MEAT` (carnes)
  - [ ] `HONEY` (mel)
  - [ ] `HERBS` (ervas)
  - [ ] `OTHER` (outros)
- [ ] Criar enum `CollectivePurchaseStatus`:
  - [ ] `PLANNING` (planejando)
  - [ ] `COLLECTING_INTERESTS` (coletando interesses)
  - [ ] `CONFIRMED` (confirmada)
  - [ ] `IN_DELIVERY` (em entrega)
  - [ ] `COMPLETED` (completada)
  - [ ] `CANCELLED` (cancelada)
- [ ] Criar enum `PurchaseInterestStatus`:
  - [ ] `PENDING` (pendente)
  - [ ] `CONFIRMED` (confirmado)
  - [ ] `CANCELLED` (cancelado)
- [ ] Criar modelo `Producer`:
  - [ ] `Id`, `UserId`, `TerritoryId`
  - [ ] `ProducerType` (ProducerType)
  - [ ] `BusinessName?` (nullable, nome do negócio)
  - [ ] `Description?` (nullable)
  - [ ] `LocationLat`, `LocationLng`
  - [ ] `ContactPhone?` (nullable)
  - [ ] `ContactEmail?` (nullable)
  - [ ] `AcceptsTerritoryCurrency` (bool)
  - [ ] `AcceptsFiat` (bool)
  - [ ] `IsActive` (bool)
  - [ ] `CreatedAtUtc`, `UpdatedAtUtc`
- [ ] Criar modelo `ProducerProduct`:
  - [ ] `Id`, `ProducerId`
  - [ ] `Name` (string)
  - [ ] `Category` (ProductCategory)
  - [ ] `Description?` (nullable)
  - [ ] `Unit` (string: kg, unidade, dúzia, etc.)
  - [ ] `PricePerUnit` (decimal, preço por unidade)
  - [ ] `PriceInTerritoryCurrency?` (nullable, preço em moeda territorial)
  - [ ] `MinQuantity` (decimal, quantidade mínima)
  - [ ] `MaxQuantity?` (nullable, quantidade máxima)
  - [ ] `IsAvailable` (bool)
  - [ ] `Seasonality?` (nullable, sazonalidade)
  - [ ] `CreatedAtUtc`, `UpdatedAtUtc`
- [ ] Criar modelo `CollectivePurchase`:
  - [ ] `Id`, `TerritoryId`, `OrganizerUserId`
  - [ ] `Title` (string)
  - [ ] `Description?` (nullable)
  - [ ] `Status` (CollectivePurchaseStatus)
  - [ ] `InterestDeadline` (DateTime, prazo para indicar interesse)
  - [ ] `DeliveryDate` (DateTime?, nullable, data de entrega)
  - [ ] `MinTotalQuantity?` (nullable, quantidade mínima total para viabilizar)
  - [ ] `DeliveryLocationLat?` (nullable)
  - [ ] `DeliveryLocationLng?` (nullable)
  - [ ] `DeliveryPointName?` (nullable, nome do ponto de entrega)
  - [ ] `CreatedAtUtc`, `UpdatedAtUtc`
- [ ] Criar modelo `CollectivePurchaseProduct`:
  - [ ] `Id`, `CollectivePurchaseId`, `ProducerProductId`
  - [ ] `RequestedQuantity` (decimal, quantidade solicitada)
  - [ ] `ConfirmedQuantity?` (nullable, quantidade confirmada)
  - [ ] `PricePerUnit` (decimal, preço na compra)
  - [ ] `IsConfirmed` (bool)
- [ ] Criar modelo `PurchaseInterest`:
  - [ ] `Id`, `CollectivePurchaseId`, `UserId`
  - [ ] `Status` (PurchaseInterestStatus)
  - [ ] `ConfirmedAtUtc?` (nullable)
  - [ ] `CancelledAtUtc?` (nullable)
  - [ ] `CreatedAtUtc`, `UpdatedAtUtc`
- [ ] Criar modelo `PurchaseInterestItem`:
  - [ ] `Id`, `PurchaseInterestId`, `CollectivePurchaseProductId`
  - [ ] `Quantity` (decimal, quantidade desejada)
  - [ ] `ConfirmedQuantity?` (nullable, quantidade confirmada)
- [ ] Criar modelo `CollectivePurchaseSchedule`:
  - [ ] `Id`, `TerritoryId`, `OrganizerUserId`
  - [ ] `Name` (string, ex: "Compra Mensal de Orgânicos")
  - [ ] `Frequency` (string: WEEKLY, BIWEEKLY, MONTHLY, CUSTOM)
  - [ ] `NextPurchaseDate` (DateTime, próxima compra)
  - [ ] `IsActive` (bool)
  - [ ] `CreatedAtUtc`, `UpdatedAtUtc`
- [ ] Criar repositórios
- [ ] Criar migrations

**Arquivos a Criar**:
- `backend/Araponga.Domain/CollectivePurchase/Producer.cs`
- `backend/Araponga.Domain/CollectivePurchase/ProducerType.cs`
- `backend/Araponga.Domain/CollectivePurchase/ProducerProduct.cs`
- `backend/Araponga.Domain/CollectivePurchase/ProductCategory.cs`
- `backend/Araponga.Domain/CollectivePurchase/CollectivePurchase.cs`
- `backend/Araponga.Domain/CollectivePurchase/CollectivePurchaseStatus.cs`
- `backend/Araponga.Domain/CollectivePurchase/CollectivePurchaseProduct.cs`
- `backend/Araponga.Domain/CollectivePurchase/PurchaseInterest.cs`
- `backend/Araponga.Domain/CollectivePurchase/PurchaseInterestStatus.cs`
- `backend/Araponga.Domain/CollectivePurchase/PurchaseInterestItem.cs`
- `backend/Araponga.Domain/CollectivePurchase/CollectivePurchaseSchedule.cs`
- `backend/Araponga.Application/Interfaces/IProducerRepository.cs`
- `backend/Araponga.Application/Interfaces/IProducerProductRepository.cs`
- `backend/Araponga.Application/Interfaces/ICollectivePurchaseRepository.cs`
- `backend/Araponga.Application/Interfaces/IPurchaseInterestRepository.cs`
- `backend/Araponga.Application/Interfaces/ICollectivePurchaseScheduleRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresProducerRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresProducerProductRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresCollectivePurchaseRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresPurchaseInterestRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresCollectivePurchaseScheduleRepository.cs`

**Critérios de Sucesso**:
- ✅ Modelos criados
- ✅ Repositórios implementados
- ✅ Migrations criadas
- ✅ Testes de repositório passando

---

### Semana 2-3: Sistema de Produtores e Compra Coletiva

#### 17.2 Sistema de Produtores
**Estimativa**: 24 horas (3 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `ProducerService`:
  - [ ] `RegisterProducerAsync(Guid userId, Guid territoryId, ...)` → registrar produtor
  - [ ] `UpdateProducerAsync(Guid producerId, ...)` → atualizar produtor
  - [ ] `ListProducersAsync(Guid territoryId, ...)` → listar produtores
  - [ ] `GetProducerAsync(Guid producerId)` → obter produtor
  - [ ] `DeactivateProducerAsync(Guid producerId)` → desativar produtor
- [ ] Criar `ProducerProductService`:
  - [ ] `AddProductAsync(Guid producerId, ...)` → adicionar produto
  - [ ] `UpdateProductAsync(Guid productId, ...)` → atualizar produto
  - [ ] `ListProductsAsync(Guid producerId, ...)` → listar produtos
  - [ ] `GetProductAsync(Guid productId)` → obter produto
  - [ ] `SetProductAvailabilityAsync(Guid productId, bool isAvailable)` → definir disponibilidade
- [ ] Criar `ProducerController`:
  - [ ] `POST /api/v1/producers` → registrar produtor
  - [ ] `GET /api/v1/producers` → listar produtores
  - [ ] `GET /api/v1/producers/{id}` → obter produtor
  - [ ] `PATCH /api/v1/producers/{id}` → atualizar produtor
  - [ ] `POST /api/v1/producers/{id}/products` → adicionar produto
  - [ ] `GET /api/v1/producers/{id}/products` → listar produtos
  - [ ] `PATCH /api/v1/producers/products/{productId}` → atualizar produto
- [ ] Feature flags: `ProducersEnabled`, `ProducerProductsEnabled`
- [ ] Validações
- [ ] Testes

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/ProducerService.cs`
- `backend/Araponga.Application/Services/ProducerProductService.cs`
- `backend/Araponga.Api/Controllers/ProducerController.cs`
- `backend/Araponga.Api/Contracts/CollectivePurchase/RegisterProducerRequest.cs`
- `backend/Araponga.Api/Contracts/CollectivePurchase/ProducerResponse.cs`
- `backend/Araponga.Api/Contracts/CollectivePurchase/AddProductRequest.cs`
- `backend/Araponga.Api/Contracts/CollectivePurchase/ProducerProductResponse.cs`
- `backend/Araponga.Api/Validators/RegisterProducerRequestValidator.cs`
- `backend/Araponga.Api/Validators/AddProductRequestValidator.cs`

**Critérios de Sucesso**:
- ✅ Sistema de produtores funcionando
- ✅ Sistema de produtos funcionando
- ✅ API funcionando
- ✅ Testes passando

---

#### 17.3 Sistema de Compra Coletiva
**Estimativa**: 32 horas (4 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `CollectivePurchaseService`:
  - [ ] `CreatePurchaseAsync(Guid territoryId, Guid organizerUserId, ...)` → criar compra coletiva
  - [ ] `AddProductToPurchaseAsync(Guid purchaseId, Guid producerProductId, ...)` → adicionar produto
  - [ ] `ListPurchasesAsync(Guid territoryId, ...)` → listar compras
  - [ ] `GetPurchaseAsync(Guid purchaseId)` → obter compra
  - [ ] `UpdatePurchaseStatusAsync(Guid purchaseId, CollectivePurchaseStatus status)` → atualizar status
  - [ ] `ConfirmPurchaseAsync(Guid purchaseId)` → confirmar compra (quando atinge quantidade mínima)
  - [ ] `CancelPurchaseAsync(Guid purchaseId, string reason)` → cancelar compra
- [ ] Lógica de confirmação:
  - [ ] Verificar se quantidade mínima foi atingida
  - [ ] Confirmar quantidades com produtor
  - [ ] Atualizar status para CONFIRMED
  - [ ] Notificar participantes
- [ ] Criar `CollectivePurchaseController`:
  - [ ] `POST /api/v1/collective-purchases` → criar compra
  - [ ] `GET /api/v1/collective-purchases` → listar compras
  - [ ] `GET /api/v1/collective-purchases/{id}` → obter compra
  - [ ] `POST /api/v1/collective-purchases/{id}/products` → adicionar produto
  - [ ] `PATCH /api/v1/collective-purchases/{id}/status` → atualizar status
  - [ ] `POST /api/v1/collective-purchases/{id}/confirm` → confirmar compra
  - [ ] `POST /api/v1/collective-purchases/{id}/cancel` → cancelar compra
- [ ] Feature flags: `CollectivePurchasesEnabled`
- [ ] Validações
- [ ] Testes

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/CollectivePurchaseService.cs`
- `backend/Araponga.Api/Controllers/CollectivePurchaseController.cs`
- `backend/Araponga.Api/Contracts/CollectivePurchase/CreateCollectivePurchaseRequest.cs`
- `backend/Araponga.Api/Contracts/CollectivePurchase/CollectivePurchaseResponse.cs`
- `backend/Araponga.Api/Contracts/CollectivePurchase/AddProductToPurchaseRequest.cs`
- `backend/Araponga.Api/Validators/CreateCollectivePurchaseRequestValidator.cs`

**Critérios de Sucesso**:
- ✅ Sistema de compra coletiva funcionando
- ✅ Lógica de confirmação funcionando
- ✅ API funcionando
- ✅ Testes passando

---

### Semana 3-4: Interesse de Compra e Agenda

#### 17.4 Sistema de Interesse de Compra
**Estimativa**: 32 horas (4 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `PurchaseInterestService`:
  - [ ] `ExpressInterestAsync(Guid purchaseId, Guid userId, ...)` → expressar interesse (opt-in)
  - [ ] `UpdateInterestAsync(Guid interestId, ...)` → atualizar interesse
  - [ ] `ConfirmInterestAsync(Guid interestId)` → confirmar interesse
  - [ ] `CancelInterestAsync(Guid interestId)` → cancelar interesse (opt-out)
  - [ ] `ListInterestsAsync(Guid purchaseId, ...)` → listar interesses
  - [ ] `GetInterestAsync(Guid interestId)` → obter interesse
  - [ ] `GetUserInterestAsync(Guid purchaseId, Guid userId)` → obter interesse do usuário
- [ ] Lógica de confirmação automática:
  - [ ] Quando compra é confirmada, confirmar todos os interesses pendentes
  - [ ] Calcular quantidades confirmadas
  - [ ] Notificar participantes
- [ ] Integrar com sistema de notificações:
  - [ ] Notificar quando compra é criada
  - [ ] Notificar quando prazo está próximo
  - [ ] Notificar quando compra é confirmada
  - [ ] Notificar quando compra é cancelada
- [ ] Criar `PurchaseInterestController`:
  - [ ] `POST /api/v1/collective-purchases/{purchaseId}/interests` → expressar interesse
  - [ ] `GET /api/v1/collective-purchases/{purchaseId}/interests` → listar interesses
  - [ ] `GET /api/v1/collective-purchases/{purchaseId}/interests/me` → obter interesse do usuário
  - [ ] `PATCH /api/v1/purchase-interests/{id}` → atualizar interesse
  - [ ] `POST /api/v1/purchase-interests/{id}/confirm` → confirmar interesse
  - [ ] `DELETE /api/v1/purchase-interests/{id}` → cancelar interesse
- [ ] Feature flags: `PurchaseInterestsEnabled`
- [ ] Validações
- [ ] Testes

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/PurchaseInterestService.cs`
- `backend/Araponga.Api/Controllers/PurchaseInterestController.cs`
- `backend/Araponga.Api/Contracts/CollectivePurchase/ExpressInterestRequest.cs`
- `backend/Araponga.Api/Contracts/CollectivePurchase/PurchaseInterestResponse.cs`
- `backend/Araponga.Api/Validators/ExpressInterestRequestValidator.cs`

**Critérios de Sucesso**:
- ✅ Sistema de interesse funcionando
- ✅ Opt-in/opt-out funcionando
- ✅ Confirmação automática funcionando
- ✅ Notificações funcionando
- ✅ Testes passando

---

#### 17.5 Sistema de Agenda de Compras
**Estimativa**: 24 horas (3 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `CollectivePurchaseScheduleService`:
  - [ ] `CreateScheduleAsync(Guid territoryId, Guid organizerUserId, ...)` → criar agenda
  - [ ] `UpdateScheduleAsync(Guid scheduleId, ...)` → atualizar agenda
  - [ ] `ListSchedulesAsync(Guid territoryId, ...)` → listar agendas
  - [ ] `GetScheduleAsync(Guid scheduleId)` → obter agenda
  - [ ] `GenerateNextPurchaseAsync(Guid scheduleId)` → gerar próxima compra
  - [ ] `DeactivateScheduleAsync(Guid scheduleId)` → desativar agenda
- [ ] Background job para gerar compras automaticamente:
  - [ ] Verificar agendas ativas
  - [ ] Gerar compra quando `NextPurchaseDate` chega
  - [ ] Atualizar `NextPurchaseDate` baseado na frequência
- [ ] Sistema de lembretes:
  - [ ] Lembretes antes do prazo de interesse
  - [ ] Lembretes antes da entrega
- [ ] Criar `CollectivePurchaseScheduleController`:
  - [ ] `POST /api/v1/collective-purchase-schedules` → criar agenda
  - [ ] `GET /api/v1/collective-purchase-schedules` → listar agendas
  - [ ] `GET /api/v1/collective-purchase-schedules/{id}` → obter agenda
  - [ ] `PATCH /api/v1/collective-purchase-schedules/{id}` → atualizar agenda
  - [ ] `POST /api/v1/collective-purchase-schedules/{id}/generate-next` → gerar próxima compra
  - [ ] `DELETE /api/v1/collective-purchase-schedules/{id}` → desativar agenda
- [ ] Feature flags: `CollectivePurchaseSchedulesEnabled`
- [ ] Validações
- [ ] Testes

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/CollectivePurchaseScheduleService.cs`
- `backend/Araponga.Api/Controllers/CollectivePurchaseScheduleController.cs`
- `backend/Araponga.Api/Contracts/CollectivePurchase/CreateScheduleRequest.cs`
- `backend/Araponga.Api/Contracts/CollectivePurchase/CollectivePurchaseScheduleResponse.cs`
- `backend/Araponga.Application/BackgroundJobs/CollectivePurchaseScheduleJob.cs`

**Critérios de Sucesso**:
- ✅ Sistema de agenda funcionando
- ✅ Geração automática de compras funcionando
- ✅ Lembretes funcionando
- ✅ Testes passando

---

### Semana 4: Integrações

#### 17.6 Integração com Votação
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Integrar com `VotingService` (Fase 14):
  - [ ] Votação para escolher produtos (quais produtos incluir na compra)
  - [ ] Votação para escolher produtores (qual produtor escolher para cada produto)
  - [ ] Votação para definir frequência de compras
  - [ ] Votação para aprovar organizadores
- [ ] Criar `CollectivePurchaseVotingService`:
  - [ ] `CreateProductVotingAsync(Guid purchaseId, ...)` → criar votação de produtos
  - [ ] `CreateProducerVotingAsync(Guid purchaseId, Guid productId, ...)` → criar votação de produtor
  - [ ] `ProcessVotingResultsAsync(Guid votingId)` → processar resultados
- [ ] Integrar resultados de votação na compra:
  - [ ] Adicionar produtos escolhidos
  - [ ] Associar produtores escolhidos
- [ ] Feature flags: `CollectivePurchaseVotingEnabled`
- [ ] Testes

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/CollectivePurchaseVotingService.cs`
- `backend/Araponga.Api/Controllers/CollectivePurchaseVotingController.cs`

**Critérios de Sucesso**:
- ✅ Integração com votação funcionando
- ✅ Votações sendo criadas automaticamente
- ✅ Resultados sendo processados
- ✅ Testes passando

---

#### 17.7 Integração com Entregas
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Integrar com `DeliveryService` (Fase 21):
  - [ ] Criar entrega coletiva quando compra é confirmada
  - [ ] Organizar rota otimizada para entregas
  - [ ] Definir pontos de entrega comunitários
  - [ ] Entregadores podem ser participantes
- [ ] Criar `CollectivePurchaseDeliveryService`:
  - [ ] `CreateDeliveryForPurchaseAsync(Guid purchaseId)` → criar entrega
  - [ ] `OrganizeDeliveryRouteAsync(Guid deliveryId)` → organizar rota
  - [ ] `AssignDeliveryPersonAsync(Guid deliveryId, Guid deliveryPersonId)` → atribuir entregador
- [ ] Integrar com sistema de pagamento:
  - [ ] Pagamento por entrega em moeda territorial
- [ ] Feature flags: `CollectivePurchaseDeliveryEnabled`
- [ ] Testes

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/CollectivePurchaseDeliveryService.cs`

**Critérios de Sucesso**:
- ✅ Integração com entregas funcionando
- ✅ Entregas sendo criadas automaticamente
- ✅ Rotas sendo organizadas
- ✅ Testes passando

---

#### 17.8 Integração com Moeda Territorial e Gamificação
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Integrar com `WalletService` (Fase 22):
  - [ ] Pagamento em moeda territorial
  - [ ] Desconto para pagamento em moeda territorial
  - [ ] Fundos territoriais podem subsidiar compras
- [ ] Integrar com `ContributionService` (Fase 42):
  - [ ] Participação em compra coletiva gera contribuição
  - [ ] Organizar compra coletiva gera mais pontos
  - [ ] Comprar de produtor local gera mais pontos
- [ ] Criar `CollectivePurchasePaymentService`:
  - [ ] `ProcessPaymentAsync(Guid purchaseId, Guid userId, ...)` → processar pagamento
  - [ ] `CalculateTotalAsync(Guid purchaseId, Guid userId)` → calcular total
  - [ ] `ApplyTerritoryCurrencyDiscountAsync(...)` → aplicar desconto
- [ ] Feature flags: `CollectivePurchaseTerritoryCurrencyEnabled`
- [ ] Testes

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/CollectivePurchasePaymentService.cs`

**Critérios de Sucesso**:
- ✅ Integração com moeda territorial funcionando
- ✅ Integração com gamificação funcionando
- ✅ Pagamentos funcionando
- ✅ Testes passando

---

## 📊 Resumo da Fase 17

| Tarefa | Estimativa | Status | Prioridade |
|--------|------------|--------|------------|
| Modelo de Domínio | 32h | ❌ Pendente | 🔴 Alta |
| Sistema de Produtores | 24h | ❌ Pendente | 🔴 Alta |
| Sistema de Compra Coletiva | 32h | ❌ Pendente | 🔴 Alta |
| Sistema de Interesse | 32h | ❌ Pendente | 🔴 Alta |
| Sistema de Agenda | 24h | ❌ Pendente | 🟡 Média |
| Integração com Votação | 16h | ❌ Pendente | 🔴 Alta |
| Integração com Entregas | 16h | ❌ Pendente | 🔴 Alta |
| Integração Moeda/Gamificação | 16h | ❌ Pendente | 🟡 Média |
| **Total** | **160h (28 dias)** | | |

---

## ✅ Critérios de Sucesso da Fase 17

### Funcionalidades
- ✅ Sistema completo de produtores funcionando
- ✅ Sistema de compra coletiva funcionando
- ✅ Sistema de interesse (opt-in/opt-out) funcionando
- ✅ Sistema de agenda funcionando
- ✅ Integração com votação funcionando
- ✅ Integração com entregas funcionando
- ✅ Integração com moeda territorial funcionando
- ✅ Integração com gamificação funcionando

### Qualidade
- ✅ Testes com cobertura adequada
- ✅ Documentação completa
- ✅ Feature flags implementados
- ✅ Validações e segurança implementadas
- Considerar **Testcontainers + PostgreSQL** para testes de integração (compra coletiva, marketplace, votações) com banco real (estratégia na Fase 43; [TESTCONTAINERS_POSTGRES_IMPACTO](../../TESTCONTAINERS_POSTGRES_IMPACTO.md)).

### Integração
- ✅ Integração com Fase 6 (Marketplace) funcionando
- ✅ Integração com Fase 14 (Votação) funcionando
- ✅ Integração com Fase 21 (Entregas) funcionando
- ✅ Integração com Fase 22 (Moeda Territorial) funcionando
- ✅ Integração com Fase 42 (Gamificação) funcionando

---

## 🔗 Dependências

- **Fase 6**: Marketplace (base para produtos)
- **Fase 14**: Governança/Votação (decisões coletivas)
- **Fase 21**: Entregas (distribuição)
- **Fase 22**: Moeda Territorial (pagamentos)
- **Fase 42**: Gamificação (contribuições)

---

## 📝 Notas de Implementação

### Fluxo de Compra Coletiva

1. **Organizador cria compra coletiva**
   - Define produtos disponíveis
   - Define prazo para interesse
   - Define quantidade mínima

2. **Comunidade indica interesse (opt-in)**
   - Usuários indicam interesse
   - Quantidade desejada por produto
   - Confirmação antes do prazo

3. **Votação (opcional)**
   - Votação para escolher produtos
   - Votação para escolher produtores

4. **Confirmação**
   - Quando quantidade mínima é atingida
   - Confirmar quantidades com produtor
   - Confirmar interesses dos participantes

5. **Entrega**
   - Criar entrega coletiva
   - Organizar rota otimizada
   - Definir pontos de entrega

6. **Pagamento**
   - Pagamento em moeda territorial ou fiat
   - Desconto para moeda territorial
   - Fundos territoriais podem subsidiar

7. **Gamificação**
   - Participação gera contribuição
   - Organizar gera mais pontos
   - Comprar local gera mais pontos

### Agenda de Compras

**Frequências**:
- Semanal: Toda semana no mesmo dia
- Quinzenal: A cada 15 dias
- Mensal: Todo mês no mesmo dia
- Custom: Definido pelo organizador

**Lembretes**:
- 7 dias antes do prazo de interesse
- 3 dias antes do prazo de interesse
- 1 dia antes da entrega

### Privacidade

- Interesses são privados (apenas organizador vê)
- Agregados são públicos (quantidade total por produto)
- Histórico pessoal é privado

---

**Status**: ⏳ **FASE 17 PENDENTE**  
**Depende de**: Fases 6, 14, 21, 22, 42  
**Crítico para**: Economia Local e Soberania Alimentar
