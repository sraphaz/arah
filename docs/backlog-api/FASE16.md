# Fase 16: Sistema de Entregas Territoriais

**Duração**: 4 semanas (28 dias úteis)  
**Prioridade**: 🟡 ALTA (Autonomia comunitária e otimização de recursos)  
**Depende de**: Fase 6 (Marketplace), Fase 7 (Payout)  
**Estimativa Total**: 160 horas  
**Status**: ⏳ Pendente

---

## 🎯 Objetivo

Implementar sistema de **entregas territoriais** que permite:
- Usuários se cadastrarem como **entregadores** do território
- Otimização de rotas para economizar recursos naturais e tempo
- Integração com marketplace (entregas de pedidos)
- Rastreamento de entregas em tempo real
- Pagamento justo para entregadores
- Governança comunitária (entregadores verificados pela comunidade)

**Princípios**:
- ✅ **Autonomia Comunitária**: Entregadores são membros da comunidade
- ✅ **Otimização de Recursos**: Rotas otimizadas reduzem consumo de combustível/tempo
- ✅ **Economia Local**: Dinheiro circula dentro do território
- ✅ **Sustentabilidade**: Menos deslocamentos, mais eficiência

---

## 📋 Contexto e Requisitos

### Estado Atual
- ✅ Marketplace funcional (pedidos, checkout)
- ✅ Sistema de payout implementado
- ❌ Não existe sistema de entregas
- ❌ Pedidos precisam ser retirados presencialmente

### Requisitos Funcionais

#### 1. Papel de Entregador
- ✅ Usuário pode se cadastrar como entregador do território
- ✅ Verificação comunitária (entregadores verificados)
- ✅ Capacidades do entregador:
  - Modalidade de transporte (bicicleta, moto, carro, a pé)
  - Raio de atuação (km)
  - Disponibilidade (horários)
  - Taxa de entrega (configurável)
- ✅ Histórico de entregas e avaliações

#### 2. Sistema de Entregas
- ✅ Criar entrega para pedido do marketplace
- ✅ Atribuir entregador (automático ou manual)
- ✅ Otimização de rotas (múltiplas entregas)
- ✅ Rastreamento em tempo real
- ✅ Confirmação de entrega (assinatura digital, foto)
- ✅ Status da entrega (Pendente, Em Rota, Entregue, Cancelada)

#### 3. Otimização de Rotas
- ✅ Agrupar entregas próximas
- ✅ Calcular rota otimizada (menor distância/tempo)
- ✅ Considerar modalidade de transporte
- ✅ Reduzir deslocamentos (economia de recursos)
- ✅ Integração com mapas (Google Maps, OpenStreetMap)

#### 4. Pagamento para Entregadores
- ✅ Taxa de entrega configurável
- ✅ Pagamento proporcional à distância/complexidade
- ✅ Integração com sistema de payout (Fase 7)
- ✅ Histórico de pagamentos

#### 5. Governança Comunitária
- ✅ Entregadores verificados pela comunidade (votação)
- ✅ Avaliações de entregadores
- ✅ Sistema de reputação
- ✅ Suspensão/remoção de entregadores (se necessário)

---

## 📋 Tarefas Detalhadas

### Semana 22: Modelo de Domínio e Entregadores

#### 22.1 Modelo de Domínio - Entregador
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar enum `DeliveryTransportMode`:
  - [ ] `Bicycle` (bicicleta)
  - [ ] `Motorcycle` (moto)
  - [ ] `Car` (carro)
  - [ ] `Walking` (a pé)
  - [ ] `Other` (outro)
- [ ] Criar enum `DeliveryStatus`:
  - [ ] `Pending` (pendente)
  - [ ] `Assigned` (atribuída)
  - [ ] `InTransit` (em rota)
  - [ ] `Delivered` (entregue)
  - [ ] `Cancelled` (cancelada)
- [ ] Criar modelo `DeliveryPerson`:
  - [ ] `Id`, `UserId`, `TerritoryId`
  - [ ] `TransportMode` (DeliveryTransportMode)
  - [ ] `ServiceRadiusKm` (raio de atuação)
  - [ ] `AvailabilitySchedule` (JSON com horários)
  - [ ] `DeliveryFeePerKm` (taxa por km)
  - [ ] `IsVerified` (bool, verificado pela comunidade)
  - [ ] `IsActive` (bool, ativo/inativo)
  - [ ] `Rating` (decimal, média de avaliações)
  - [ ] `TotalDeliveries` (int, contagem)
  - [ ] `CreatedAtUtc`, `UpdatedAtUtc`
- [ ] Criar modelo `Delivery`:
  - [ ] `Id`, `OrderId` (FK para pedido do marketplace)
  - [ ] `TerritoryId`
  - [ ] `DeliveryPersonId?` (nullable, atribuído quando aceito)
  - [ ] `Status` (DeliveryStatus)
  - [ ] `PickupAddress` (endereço de retirada)
  - [ ] `DeliveryAddress` (endereço de entrega)
  - [ ] `PickupCoordinates` (lat/lng)
  - [ ] `DeliveryCoordinates` (lat/lng)
  - [ ] `EstimatedDistanceKm` (distância estimada)
  - [ ] `ActualDistanceKm?` (distância real, nullable)
  - [ ] `EstimatedDurationMinutes` (tempo estimado)
  - [ ] `ActualDurationMinutes?` (tempo real, nullable)
  - [ ] `DeliveryFee` (taxa de entrega)
  - [ ] `RouteOptimizationData` (JSON, dados de otimização)
  - [ ] `TrackingHistory` (JSON, histórico de localização)
  - [ ] `DeliveredAtUtc?` (data de entrega, nullable)
  - [ ] `DeliveryConfirmation` (JSON, assinatura/foto)
  - [ ] `CreatedAtUtc`, `UpdatedAtUtc`
- [ ] Criar repositórios
- [ ] Criar migrations

**Arquivos a Criar**:
- `backend/Araponga.Domain/Delivery/DeliveryPerson.cs`
- `backend/Araponga.Domain/Delivery/Delivery.cs`
- `backend/Araponga.Domain/Delivery/DeliveryTransportMode.cs`
- `backend/Araponga.Domain/Delivery/DeliveryStatus.cs`
- `backend/Araponga.Application/Interfaces/IDeliveryPersonRepository.cs`
- `backend/Araponga.Application/Interfaces/IDeliveryRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresDeliveryPersonRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresDeliveryRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/Migrations/YYYYMMDDHHMMSS_AddDeliverySystem.cs`

**Critérios de Sucesso**:
- ✅ Modelos criados
- ✅ Repositórios implementados
- ✅ Migrations aplicadas

---

#### 22.2 Serviço de Entregadores
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `DeliveryPersonService`:
  - [ ] `RegisterAsDeliveryPersonAsync(Guid userId, Guid territoryId, DeliveryTransportMode transportMode, double serviceRadiusKm, object availabilitySchedule, decimal deliveryFeePerKm)`
  - [ ] `UpdateDeliveryPersonAsync(Guid deliveryPersonId, Guid userId, ...)`
  - [ ] `ListDeliveryPersonsAsync(Guid territoryId, bool? isActive, bool? isVerified)`
  - [ ] `GetDeliveryPersonAsync(Guid deliveryPersonId)`
  - [ ] `DeactivateDeliveryPersonAsync(Guid deliveryPersonId, Guid userId)`
- [ ] Validações:
  - [ ] Apenas residents podem ser entregadores
  - [ ] Raio de atuação > 0 e < 50km (configurável)
  - [ ] Taxa de entrega >= 0
  - [ ] Horários de disponibilidade válidos
- [ ] Testes unitários

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/DeliveryPersonService.cs`
- `backend/Araponga.Tests/Application/DeliveryPersonServiceTests.cs`

**Critérios de Sucesso**:
- ✅ Serviço implementado
- ✅ Validações funcionando
- ✅ Testes passando

---

#### 22.3 Controller de Entregadores
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `DeliveryPersonsController`:
  - [ ] `POST /api/v1/territories/{territoryId}/delivery-persons` (cadastrar como entregador)
  - [ ] `GET /api/v1/territories/{territoryId}/delivery-persons` (listar entregadores)
  - [ ] `GET /api/v1/delivery-persons/{id}` (obter entregador)
  - [ ] `PUT /api/v1/delivery-persons/{id}` (atualizar entregador)
  - [ ] `DELETE /api/v1/delivery-persons/{id}` (desativar)
- [ ] Criar requests/responses
- [ ] Validação (FluentValidation)
- [ ] Testes de integração

**Arquivos a Criar**:
- `backend/Araponga.Api/Controllers/DeliveryPersonsController.cs`
- `backend/Araponga.Api/Contracts/Delivery/RegisterDeliveryPersonRequest.cs`
- `backend/Araponga.Api/Contracts/Delivery/DeliveryPersonResponse.cs`
- `backend/Araponga.Api/Validators/RegisterDeliveryPersonRequestValidator.cs`
- `backend/Araponga.Tests/Integration/DeliveryPersonsIntegrationTests.cs`

**Critérios de Sucesso**:
- ✅ Endpoints funcionando
- ✅ Validações funcionando
- ✅ Testes passando

---

### Semana 23: Sistema de Entregas e Otimização

#### 23.1 Serviço de Otimização de Rotas
**Estimativa**: 20 horas (2.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar interface `IRouteOptimizationService`:
  - [ ] `OptimizeRouteAsync(IReadOnlyList<DeliveryAddress> addresses, DeliveryTransportMode transportMode)` → `OptimizedRoute`
- [ ] Criar `OptimizedRoute`:
  - [ ] `Waypoints` (ordem otimizada)
  - [ ] `TotalDistanceKm` (distância total)
  - [ ] `TotalDurationMinutes` (tempo total)
  - [ ] `RoutePolyline` (polilinha para mapa)
- [ ] Implementar `RouteOptimizationService`:
  - [ ] Integração com Google Maps Directions API (ou OpenRouteService)
  - [ ] Algoritmo de otimização (TSP simplificado ou usar API)
  - [ ] Agrupar entregas próximas (< 2km)
  - [ ] Calcular rota otimizada
- [ ] Cache de rotas (TTL: 1 hora)
- [ ] Testes unitários

**Arquivos a Criar**:
- `backend/Araponga.Application/Interfaces/IRouteOptimizationService.cs`
- `backend/Araponga.Application/Services/RouteOptimizationService.cs`
- `backend/Araponga.Application/Models/OptimizedRoute.cs`
- `backend/Araponga.Application/Models/DeliveryAddress.cs`
- `backend/Araponga.Tests/Application/RouteOptimizationServiceTests.cs`

**Critérios de Sucesso**:
- ✅ Otimização de rotas funcionando
- ✅ Integração com API de mapas funcionando
- ✅ Cache funcionando
- ✅ Testes passando

---

#### 23.2 Serviço de Entregas
**Estimativa**: 20 horas (2.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `DeliveryService`:
  - [ ] `CreateDeliveryAsync(Guid orderId, string pickupAddress, GeoCoordinate pickupCoordinates, string deliveryAddress, GeoCoordinate deliveryCoordinates)` → criar entrega
  - [ ] `AssignDeliveryPersonAsync(Guid deliveryId, Guid deliveryPersonId)` → atribuir entregador
  - [ ] `AutoAssignDeliveryPersonAsync(Guid deliveryId)` → atribuir automaticamente (mais próximo, disponível)
  - [ ] `StartDeliveryAsync(Guid deliveryId, Guid deliveryPersonId)` → iniciar entrega
  - [ ] `UpdateDeliveryLocationAsync(Guid deliveryId, GeoCoordinate location)` → atualizar localização (rastreamento)
  - [ ] `CompleteDeliveryAsync(Guid deliveryId, Guid deliveryPersonId, string? signatureBase64, string? photoBase64)` → completar entrega
  - [ ] `CancelDeliveryAsync(Guid deliveryId, Guid userId, string reason)` → cancelar entrega
  - [ ] `ListDeliveriesAsync(Guid territoryId, DeliveryStatus? status, Guid? deliveryPersonId)` → listar entregas
  - [ ] `GetDeliveryAsync(Guid deliveryId)` → obter entrega
- [ ] Integração com `RouteOptimizationService`:
  - [ ] Ao criar entrega: calcular distância/tempo estimado
  - [ ] Ao atribuir múltiplas entregas: otimizar rota
- [ ] Cálculo de taxa de entrega:
  - [ ] Baseado em distância e taxa por km do entregador
  - [ ] Mínimo e máximo configuráveis
- [ ] Validações:
  - [ ] Apenas entregador atribuído pode iniciar/completar
  - [ ] Endereços devem estar no território (ou próximo)
  - [ ] Entregador deve estar ativo e disponível
- [ ] Testes unitários

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/DeliveryService.cs`
- `backend/Araponga.Tests/Application/DeliveryServiceTests.cs`

**Arquivos a Modificar**:
- `backend/Araponga.Application/Services/CartService.cs` (integrar criação de entrega no checkout)

**Critérios de Sucesso**:
- ✅ Serviço implementado
- ✅ Otimização de rotas integrada
- ✅ Cálculo de taxa funcionando
- ✅ Testes passando

---

#### 23.3 Controller de Entregas
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `DeliveriesController`:
  - [ ] `POST /api/v1/deliveries` (criar entrega - integrado com checkout)
  - [ ] `GET /api/v1/deliveries` (listar entregas)
  - [ ] `GET /api/v1/deliveries/{id}` (obter entrega)
  - [ ] `GET /api/v1/deliveries/{id}/tracking` (rastreamento em tempo real)
  - [ ] `POST /api/v1/deliveries/{id}/assign` (atribuir entregador - apenas curadores)
  - [ ] `POST /api/v1/deliveries/{id}/accept` (aceitar entrega - entregador)
  - [ ] `POST /api/v1/deliveries/{id}/start` (iniciar entrega - entregador)
  - [ ] `POST /api/v1/deliveries/{id}/location` (atualizar localização - entregador)
  - [ ] `POST /api/v1/deliveries/{id}/complete` (completar entrega - entregador)
  - [ ] `POST /api/v1/deliveries/{id}/cancel` (cancelar entrega)
- [ ] Criar requests/responses
- [ ] Validação (FluentValidation)
- [ ] Testes de integração

**Arquivos a Criar**:
- `backend/Araponga.Api/Controllers/DeliveriesController.cs`
- `backend/Araponga.Api/Contracts/Delivery/CreateDeliveryRequest.cs`
- `backend/Araponga.Api/Contracts/Delivery/DeliveryResponse.cs`
- `backend/Araponga.Api/Contracts/Delivery/TrackingResponse.cs`
- `backend/Araponga.Api/Contracts/Delivery/UpdateLocationRequest.cs`
- `backend/Araponga.Api/Contracts/Delivery/CompleteDeliveryRequest.cs`
- `backend/Araponga.Api/Validators/CreateDeliveryRequestValidator.cs`
- `backend/Araponga.Tests/Integration/DeliveriesIntegrationTests.cs`

**Critérios de Sucesso**:
- ✅ Endpoints funcionando
- ✅ Rastreamento funcionando
- ✅ Testes passando

---

### Semana 24: Integração com Marketplace e Pagamentos

#### 24.1 Integração com Checkout
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Atualizar `CartService.CheckoutAsync`:
  - [ ] Adicionar parâmetro `DeliveryAddress?` (opcional)
  - [ ] Se `DeliveryAddress` fornecido:
    - [ ] Criar entrega via `DeliveryService`
    - [ ] Calcular taxa de entrega
    - [ ] Adicionar taxa ao total do pedido
    - [ ] Associar entrega ao pedido
- [ ] Atualizar `CheckoutRequest`:
  - [ ] Adicionar campo `DeliveryAddress` (opcional)
  - [ ] Adicionar campo `DeliveryCoordinates` (opcional)
- [ ] Atualizar `OrderResponse`:
  - [ ] Adicionar campo `Delivery` (DeliveryResponse, nullable)
- [ ] Validação:
  - [ ] Endereço de entrega deve estar no território (ou próximo)
  - [ ] Deve haver entregadores disponíveis no território
- [ ] Testes de integração

**Arquivos a Modificar**:
- `backend/Araponga.Application/Services/CartService.cs`
- `backend/Araponga.Api/Contracts/Marketplace/CheckoutRequest.cs`
- `backend/Araponga.Api/Contracts/Marketplace/OrderResponse.cs`
- `backend/Araponga.Api/Validators/CheckoutRequestValidator.cs`

**Critérios de Sucesso**:
- ✅ Integração funcionando
- ✅ Taxa de entrega calculada corretamente
- ✅ Entrega criada no checkout
- ✅ Testes passando

---

#### 24.2 Integração com Sistema de Payout
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Atualizar `SellerPayoutService`:
  - [ ] Adicionar método `ProcessDeliveryPayoutAsync(Guid deliveryId)`
  - [ ] Quando entrega é completada: criar transação de payout para entregador
  - [ ] Valor: taxa de entrega calculada
- [ ] Criar `DeliveryPayoutTransaction`:
  - [ ] Tipo: `DeliveryFee`
  - [ ] Associado à entrega
  - [ ] Valor: taxa de entrega
- [ ] Atualizar `FinancialTransaction` (se necessário):
  - [ ] Adicionar tipo `DeliveryFee` (se não existir)
- [ ] Integração automática:
  - [ ] Quando `DeliveryService.CompleteDeliveryAsync` é chamado
  - [ ] Criar payout automaticamente
- [ ] Testes de integração

**Arquivos a Modificar**:
- `backend/Araponga.Application/Services/SellerPayoutService.cs`
- `backend/Araponga.Application/Services/DeliveryService.cs`
- `backend/Araponga.Domain/Financial/TransactionType.cs` (adicionar DeliveryFee)

**Critérios de Sucesso**:
- ✅ Payout automático funcionando
- ✅ Transações criadas corretamente
- ✅ Testes passando

---

#### 24.3 Otimização de Múltiplas Entregas
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `DeliveryBatchService`:
  - [ ] `CreateBatchAsync(Guid territoryId, IReadOnlyList<Guid> deliveryIds)` → agrupar entregas
  - [ ] `OptimizeBatchRouteAsync(Guid batchId, Guid deliveryPersonId)` → otimizar rota do lote
  - [ ] `AssignBatchToDeliveryPersonAsync(Guid batchId, Guid deliveryPersonId)` → atribuir lote
- [ ] Lógica de agrupamento:
  - [ ] Entregas próximas (< 2km) podem ser agrupadas
  - [ ] Entregas no mesmo dia podem ser agrupadas
  - [ ] Entregador pode aceitar múltiplas entregas
- [ ] Otimização de rota para lote:
  - [ ] Calcular rota otimizada para todas as entregas
  - [ ] Reduzir distância total (economia de recursos)
  - [ ] Reduzir tempo total (otimização de tempo)
- [ ] Testes

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/DeliveryBatchService.cs`
- `backend/Araponga.Domain/Delivery/DeliveryBatch.cs`
- `backend/Araponga.Application/Interfaces/IDeliveryBatchRepository.cs`
- `backend/Araponga.Tests/Application/DeliveryBatchServiceTests.cs`

**Critérios de Sucesso**:
- ✅ Agrupamento funcionando
- ✅ Otimização de lote funcionando
- ✅ Economia de recursos validada
- ✅ Testes passando

---

### Semana 25: Governança e Finalização

#### 25.1 Verificação Comunitária de Entregadores
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Integração com sistema de votação (Fase 14):
  - [ ] Criar votação `DeliveryPersonVerification` quando entregador se cadastra
  - [ ] Opções: "Aprovar", "Rejeitar"
  - [ ] Se aprovado: `IsVerified = true`
- [ ] Alternativa: aprovação manual por curadores
  - [ ] Endpoint `POST /api/v1/delivery-persons/{id}/verify` (apenas curadores)
- [ ] Notificações:
  - [ ] Notificar entregador quando verificado
  - [ ] Notificar comunidade quando novo entregador se cadastra
- [ ] Testes

**Arquivos a Modificar**:
- `backend/Araponga.Application/Services/DeliveryPersonService.cs`
- `backend/Araponga.Api/Controllers/DeliveryPersonsController.cs`
- `backend/Araponga.Application/Services/VotingService.cs` (se Fase 14 implementada)

**Critérios de Sucesso**:
- ✅ Verificação funcionando
- ✅ Integração com votações funcionando (se disponível)
- ✅ Notificações funcionando
- ✅ Testes passando

---

#### 25.2 Sistema de Avaliações de Entregadores
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar modelo `DeliveryPersonReview`:
  - [ ] `Id`, `DeliveryId`, `DeliveryPersonId`, `ReviewerId` (comprador)
  - [ ] `Rating` (1-5 estrelas)
  - [ ] `Comment` (string?, nullable)
  - [ ] `CreatedAtUtc`
- [ ] Criar `IDeliveryPersonReviewRepository`
- [ ] Implementar repositórios
- [ ] Criar `DeliveryPersonReviewService`:
  - [ ] `CreateReviewAsync(Guid deliveryId, Guid reviewerId, int rating, string? comment)`
  - [ ] `ListReviewsAsync(Guid deliveryPersonId)`
  - [ ] `CalculateAverageRatingAsync(Guid deliveryPersonId)` → atualizar rating do entregador
- [ ] Integração com `DeliveryService.CompleteDeliveryAsync`:
  - [ ] Permitir avaliação após entrega completada
- [ ] Atualizar `DeliveryPersonResponse`:
  - [ ] Incluir `AverageRating` e `TotalReviews`
- [ ] Criar migration
- [ ] Testes

**Arquivos a Criar**:
- `backend/Araponga.Domain/Delivery/DeliveryPersonReview.cs`
- `backend/Araponga.Application/Interfaces/IDeliveryPersonReviewRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresDeliveryPersonReviewRepository.cs`
- `backend/Araponga.Application/Services/DeliveryPersonReviewService.cs`
- `backend/Araponga.Infrastructure/Postgres/Migrations/YYYYMMDDHHMMSS_AddDeliveryReviews.cs`

**Arquivos a Modificar**:
- `backend/Araponga.Application/Services/DeliveryService.cs`
- `backend/Araponga.Api/Contracts/Delivery/DeliveryPersonResponse.cs`

**Critérios de Sucesso**:
- ✅ Avaliações funcionando
- ✅ Rating calculado automaticamente
- ✅ Testes passando

---

#### 25.3 Rastreamento em Tempo Real
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Atualizar `DeliveryService.UpdateDeliveryLocationAsync`:
  - [ ] Armazenar histórico de localização em `TrackingHistory` (JSON)
  - [ ] Timestamp de cada atualização
  - [ ] Limitar histórico (últimas 100 posições)
- [ ] Endpoint de rastreamento:
  - [ ] `GET /api/v1/deliveries/{id}/tracking` → retornar histórico
  - [ ] Formato: array de `{timestamp, latitude, longitude}`
- [ ] Background job (opcional):
  - [ ] Atualizar localização automaticamente via GPS do entregador (se app mobile)
- [ ] Integração com mapas:
  - [ ] Retornar polilinha da rota
  - [ ] Retornar posição atual
- [ ] Testes

**Arquivos a Modificar**:
- `backend/Araponga.Application/Services/DeliveryService.cs`
- `backend/Araponga.Api/Controllers/DeliveriesController.cs`
- `backend/Araponga.Api/Contracts/Delivery/TrackingResponse.cs`

**Critérios de Sucesso**:
- ✅ Rastreamento funcionando
- ✅ Histórico armazenado
- ✅ Endpoint funcionando
- ✅ Testes passando

---

#### 25.4 Testes e Documentação
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Testes de integração completos:
  - [ ] Cadastro de entregador
  - [ ] Criação de entrega
  - [ ] Atribuição de entregador
  - [ ] Rastreamento
  - [ ] Completação de entrega
  - [ ] Payout automático
  - [ ] Otimização de rotas
- [ ] Testes de performance:
  - [ ] Otimização de rotas com muitas entregas
  - [ ] Cálculo de taxa com muitos entregadores
- [ ] Testes de segurança:
  - [ ] Permissões (apenas entregador pode atualizar sua entrega)
  - [ ] Validação de endereços
- [ ] Documentação técnica:
  - [ ] `docs/DELIVERY_SYSTEM.md`
  - [ ] Como cadastrar entregador
  - [ ] Como criar entrega
  - [ ] Como otimizar rotas
  - [ ] Integração com marketplace
- [ ] Atualizar `docs/CHANGELOG.md`
- [ ] Atualizar Swagger

**Arquivos a Criar**:
- `backend/Araponga.Tests/Integration/DeliveryCompleteIntegrationTests.cs`
- `docs/DELIVERY_SYSTEM.md`

**Critérios de Sucesso**:
- ✅ Testes passando
- ✅ Cobertura >85%
- ✅ Documentação completa

---

## 📊 Resumo da Fase 16

| Tarefa | Estimativa | Status | Prioridade |
|--------|------------|--------|------------|
| Modelo de Domínio - Entregador | 12h | ❌ Pendente | 🔴 Crítica |
| Serviço de Entregadores | 16h | ❌ Pendente | 🔴 Crítica |
| Controller de Entregadores | 12h | ❌ Pendente | 🔴 Crítica |
| Serviço de Otimização de Rotas | 20h | ❌ Pendente | 🔴 Crítica |
| Serviço de Entregas | 20h | ❌ Pendente | 🔴 Crítica |
| Controller de Entregas | 12h | ❌ Pendente | 🔴 Crítica |
| Integração com Checkout | 16h | ❌ Pendente | 🔴 Crítica |
| Integração com Sistema de Payout | 12h | ❌ Pendente | 🔴 Crítica |
| Otimização de Múltiplas Entregas | 16h | ❌ Pendente | 🟡 Importante |
| Verificação Comunitária | 12h | ❌ Pendente | 🟡 Importante |
| Sistema de Avaliações | 12h | ❌ Pendente | 🟡 Importante |
| Rastreamento em Tempo Real | 12h | ❌ Pendente | 🟡 Importante |
| Testes e Documentação | 16h | ❌ Pendente | 🟡 Importante |
| **Total** | **160h (28 dias)** | | |

---

## ✅ Critérios de Sucesso da Fase 16

### Funcionalidades
- ✅ Cadastro de entregadores funcionando
- ✅ Criação de entregas funcionando
- ✅ Otimização de rotas funcionando
- ✅ Rastreamento em tempo real funcionando
- ✅ Integração com marketplace funcionando
- ✅ Payout automático funcionando
- ✅ Verificação comunitária funcionando
- ✅ Avaliações funcionando

### Qualidade
- ✅ Cobertura de testes >85%
- ✅ Testes de integração passando
- ✅ Performance adequada (otimização < 5s)
- ✅ Segurança validada (permissões)

### Documentação
- ✅ Documentação técnica completa
- ✅ Changelog atualizado
- ✅ Swagger atualizado

---

## 🔗 Dependências

- **Fase 6**: Sistema de Pagamentos (Marketplace)
- **Fase 7**: Sistema de Payout (Pagamento para entregadores)
- **Opcional**: Fase 14 (Governança) - Verificação comunitária via votações

---

## 📝 Notas de Implementação

### Princípios de Autonomia Comunitária

**Entregadores como Membros da Comunidade**:
- ✅ Entregadores são residents do território
- ✅ Verificação comunitária (não apenas administrativa)
- ✅ Avaliações da comunidade
- ✅ Reputação baseada em entregas

**Otimização de Recursos**:
- ✅ Rotas otimizadas reduzem consumo de combustível
- ✅ Agrupamento de entregas reduz deslocamentos
- ✅ Economia de tempo para entregadores
- ✅ Menor impacto ambiental

**Economia Local**:
- ✅ Dinheiro circula dentro do território
- ✅ Entregadores recebem pagamento justo
- ✅ Fortalecimento da economia comunitária

### Otimização de Rotas

**Algoritmo**:
- Agrupar entregas próximas (< 2km)
- Calcular rota otimizada (menor distância/tempo)
- Considerar modalidade de transporte
- Usar API de mapas (Google Maps, OpenRouteService)

**Economia**:
- **Sem otimização**: 10 entregas = 10 rotas separadas
- **Com otimização**: 10 entregas = 1 rota otimizada
- **Economia estimada**: 40-60% de distância/tempo

### Integração com Marketplace

**Fluxo**:
1. Cliente faz pedido no marketplace
2. No checkout, escolhe opção de entrega
3. Sistema cria entrega automaticamente
4. Sistema atribui entregador (automático ou manual)
5. Entregador aceita e inicia entrega
6. Rastreamento em tempo real
7. Entrega completada
8. Payout automático para entregador

---

**Status**: ⏳ **FASE 16 PENDENTE**  
**Depende de**: Fases 6, 7 (Marketplace, Payout)  
**Crítico para**: Autonomia Comunitária e Otimização de Recursos
