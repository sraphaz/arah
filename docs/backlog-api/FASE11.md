# Fase 11: Edição, Gestão e Estatísticas Completas

**Duração**: 3 semanas (15 dias úteis)  
**Prioridade**: 🟡 IMPORTANTE (Completa funcionalidades essenciais)  
**Depende de**: Fase 8 (Infraestrutura Mídia), Fase 10 (Mídias em Conteúdo)  
**Estimativa Total**: 120 horas  
**Status**: ⏳ Pendente

---

## 🎯 Objetivo

Completar funcionalidades essenciais de edição e gestão que permitem:
- Editar posts e eventos (correção de erros, atualização de informações)
- Sistema de avaliações no marketplace (lojas e itens)
- Busca no marketplace (full-text search)
- Histórico de atividades do usuário
- Lista de participantes de eventos

**Princípios**:
- ✅ **Correção e Atualização**: Usuários podem corrigir erros
- ✅ **Transparência**: Avaliações e histórico são públicos
- ✅ **Busca Eficiente**: Busca rápida e relevante
- ✅ **Contexto Territorial**: Tudo mantém contexto territorial

---

## 📋 Contexto e Requisitos

### Estado Atual
- ✅ Sistema de posts, eventos, marketplace implementado
- ✅ Sistema de mídia (Fase 8)
- ✅ Mídias em conteúdo (Fase 10)
- ❌ Não é possível editar posts
- ❌ Não é possível editar eventos
- ❌ Não existe sistema de avaliações no marketplace
- ❌ Não existe busca no marketplace
- ❌ Não existe histórico de atividades

### Requisitos Funcionais

#### 1. Edição de Posts
- ✅ Editar título e conteúdo
- ✅ Adicionar/remover mídias
- ✅ Editar localização (GeoAnchor)
- ✅ Histórico de edições (opcional)
- ✅ Indicação de post editado

#### 2. Edição de Eventos
- ✅ Editar todos os campos (título, descrição, data, localização)
- ✅ Editar capa do evento
- ✅ Cancelar evento
- ✅ Lista de participantes confirmados
- ✅ Histórico de edições (opcional)

#### 3. Sistema de Avaliações no Marketplace
- ✅ Avaliar loja (rating 1-5, comentário)
- ✅ Avaliar item (rating 1-5, comentário)
- ✅ Visualizar avaliações
- ✅ Responder avaliações (vendedor)
- ✅ Filtros por rating

#### 4. Busca no Marketplace
- ✅ Busca full-text em lojas
- ✅ Busca full-text em itens
- ✅ Filtros (categoria, preço, localização)
- ✅ Ordenação (relevância, preço, data)
- ✅ Paginação

#### 5. Histórico de Atividades
- ✅ Histórico de posts criados
- ✅ Histórico de eventos criados
- ✅ Histórico de compras/vendas
- ✅ Histórico de participações
- ✅ Filtros e paginação

---

## 📋 Tarefas Detalhadas

### Semana 12: Edição de Posts e Eventos

#### 11.1 Edição de Posts
**Estimativa**: 24 horas (3 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Estender `Post` domain model:
  - [ ] `EditedAtUtc?` (nullable)
  - [ ] `EditCount` (int, contador de edições)
- [ ] Criar `PostEditService`:
  - [ ] `EditPostAsync(Guid postId, Guid userId, ...)` → editar post
  - [ ] `GetPostEditHistoryAsync(Guid postId)` → histórico de edições (opcional)
- [ ] Validações:
  - [ ] Apenas autor pode editar
  - [ ] Limite de tempo para edição? (opcional, configurável)
- [ ] Criar `PostController` endpoint:
  - [ ] `PATCH /api/v1/posts/{id}` → editar post
- [ ] Feature flags: `PostEditingEnabled`
- [ ] Testes

**Arquivos a Modificar**:
- `backend/Araponga.Domain/Feed/Post.cs`
- `backend/Araponga.Application/Services/PostCreationService.cs` (renomear para `PostService` ou criar `PostEditService`)

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/PostEditService.cs`
- `backend/Araponga.Api/Contracts/Feed/EditPostRequest.cs`
- `backend/Araponga.Api/Validators/EditPostRequestValidator.cs`

**Critérios de Sucesso**:
- ✅ Edição de posts funcionando
- ✅ Validações funcionando
- ✅ Testes passando

---

#### 11.2 Edição de Eventos
**Estimativa**: 24 horas (3 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Estender `TerritoryEvent` domain model:
  - [ ] `EditedAtUtc?` (nullable)
  - [ ] `EditCount` (int)
- [ ] Criar `EventEditService`:
  - [ ] `EditEventAsync(Guid eventId, Guid userId, ...)` → editar evento
  - [ ] `CancelEventAsync(Guid eventId, Guid userId, string reason)` → cancelar evento
  - [ ] `GetEventParticipantsAsync(Guid eventId, ...)` → lista de participantes
- [ ] Validações:
  - [ ] Apenas organizador pode editar
  - [ ] Não pode editar evento já realizado
- [ ] Criar `EventsController` endpoints:
  - [ ] `PATCH /api/v1/events/{id}` → editar evento
  - [ ] `GET /api/v1/events/{id}/participants` → lista de participantes
- [ ] Feature flags: `EventEditingEnabled`
- [ ] Testes

**Arquivos a Modificar**:
- `backend/Araponga.Domain/Events/TerritoryEvent.cs`
- `backend/Araponga.Application/Services/EventsService.cs`

**Arquivos a Criar**:
- `backend/Araponga.Api/Contracts/Events/EditEventRequest.cs`
- `backend/Araponga.Api/Contracts/Events/EventParticipantResponse.cs`
- `backend/Araponga.Api/Validators/EditEventRequestValidator.cs`

**Critérios de Sucesso**:
- ✅ Edição de eventos funcionando
- ✅ Cancelamento funcionando
- ✅ Lista de participantes funcionando
- ✅ Testes passando

---

### Semana 13: Avaliações e Busca no Marketplace

#### 11.3 Sistema de Avaliações no Marketplace
**Estimativa**: 32 horas (4 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar modelo `StoreRating`:
  - [ ] `Id`, `StoreId`, `UserId`
  - [ ] `Rating` (int, 1-5)
  - [ ] `Comment?` (nullable)
  - [ ] `CreatedAtUtc`, `UpdatedAtUtc`
- [ ] Criar modelo `StoreItemRating`:
  - [ ] `Id`, `StoreItemId`, `UserId`
  - [ ] `Rating` (int, 1-5)
  - [ ] `Comment?` (nullable)
  - [ ] `CreatedAtUtc`, `UpdatedAtUtc`
- [ ] Criar modelo `StoreRatingResponse`:
  - [ ] `Id`, `RatingId`, `StoreId`
  - [ ] `ResponseText` (string)
  - [ ] `CreatedAtUtc`
- [ ] Criar `RatingService`:
  - [ ] `RateStoreAsync(Guid storeId, Guid userId, int rating, string? comment)` → avaliar loja
  - [ ] `RateItemAsync(Guid itemId, Guid userId, int rating, string? comment)` → avaliar item
  - [ ] `RespondToRatingAsync(Guid ratingId, Guid storeId, string response)` → responder avaliação
  - [ ] `ListStoreRatingsAsync(Guid storeId, ...)` → listar avaliações da loja
  - [ ] `ListItemRatingsAsync(Guid itemId, ...)` → listar avaliações do item
  - [ ] `GetStoreAverageRatingAsync(Guid storeId)` → média de avaliações
- [ ] Criar `RatingController`:
  - [ ] `POST /api/v1/stores/{id}/ratings` → avaliar loja
  - [ ] `GET /api/v1/stores/{id}/ratings` → listar avaliações
  - [ ] `POST /api/v1/items/{id}/ratings` → avaliar item
  - [ ] `GET /api/v1/items/{id}/ratings` → listar avaliações
  - [ ] `POST /api/v1/ratings/{id}/response` → responder avaliação
- [ ] Feature flags: `MarketplaceRatingsEnabled`
- [ ] Validações
- [ ] Testes

**Arquivos a Criar**:
- `backend/Araponga.Domain/Marketplace/StoreRating.cs`
- `backend/Araponga.Domain/Marketplace/StoreItemRating.cs`
- `backend/Araponga.Domain/Marketplace/StoreRatingResponse.cs`
- `backend/Araponga.Application/Interfaces/IStoreRatingRepository.cs`
- `backend/Araponga.Application/Interfaces/IStoreItemRatingRepository.cs`
- `backend/Araponga.Application/Services/RatingService.cs`
- `backend/Araponga.Api/Controllers/RatingController.cs`
- `backend/Araponga.Api/Contracts/Marketplace/CreateRatingRequest.cs`
- `backend/Araponga.Api/Contracts/Marketplace/RatingResponse.cs`

**Critérios de Sucesso**:
- ✅ Sistema de avaliações funcionando
- ✅ Respostas funcionando
- ✅ Médias calculadas
- ✅ Testes passando

---

#### 11.4 Busca no Marketplace
**Estimativa**: 24 horas (3 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `MarketplaceSearchService`:
  - [ ] `SearchStoresAsync(string query, SearchFilters, ...)` → buscar lojas
  - [ ] `SearchItemsAsync(string query, SearchFilters, ...)` → buscar itens
  - [ ] `SearchAllAsync(string query, SearchFilters, ...)` → buscar tudo
- [ ] Implementar busca full-text:
  - [ ] Usar PostgreSQL full-text search
  - [ ] Índices GIN para performance
  - [ ] Ranking por relevância
- [ ] Filtros:
  - [ ] Por categoria
  - [ ] Por faixa de preço
  - [ ] Por localização (raio)
  - [ ] Por rating mínimo
- [ ] Ordenação:
  - [ ] Por relevância (padrão)
  - [ ] Por preço (crescente/decrescente)
  - [ ] Por data (mais recente)
  - [ ] Por rating (maior)
- [ ] Criar `MarketplaceSearchController`:
  - [ ] `GET /api/v1/marketplace/search` → busca geral
  - [ ] `GET /api/v1/stores/search` → buscar lojas
  - [ ] `GET /api/v1/items/search` → buscar itens
- [ ] Feature flags: `MarketplaceSearchEnabled`
- [ ] Testes

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/MarketplaceSearchService.cs`
- `backend/Araponga.Application/Models/SearchFilters.cs`
- `backend/Araponga.Api/Controllers/MarketplaceSearchController.cs`
- `backend/Araponga.Api/Contracts/Marketplace/SearchRequest.cs`
- `backend/Araponga.Api/Contracts/Marketplace/SearchResponse.cs`

**Arquivos a Modificar**:
- `backend/Araponga.Infrastructure/Postgres/Migrations/` (adicionar índices full-text)

**Critérios de Sucesso**:
- ✅ Busca funcionando
- ✅ Filtros funcionando
- ✅ Ordenação funcionando
- ✅ Performance adequada (< 500ms)
- ✅ Testes passando

---

### Semana 14: Histórico de Atividades

#### 11.5 Histórico de Atividades do Usuário
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `UserActivityService`:
  - [ ] `GetUserPostsAsync(Guid userId, ...)` → posts criados
  - [ ] `GetUserEventsAsync(Guid userId, ...)` → eventos criados
  - [ ] `GetUserPurchasesAsync(Guid userId, ...)` → compras
  - [ ] `GetUserSalesAsync(Guid userId, ...)` → vendas
  - [ ] `GetUserParticipationsAsync(Guid userId, ...)` → participações
  - [ ] `GetUserActivityHistoryAsync(Guid userId, ...)` → histórico completo
- [ ] Integrar com serviços existentes:
  - [ ] `FeedService` → posts
  - [ ] `EventsService` → eventos
  - [ ] `CartService` → compras/vendas
- [ ] Criar `UserActivityController`:
  - [ ] `GET /api/v1/users/me/activity` → histórico completo
  - [ ] `GET /api/v1/users/me/posts` → meus posts
  - [ ] `GET /api/v1/users/me/events` → meus eventos
  - [ ] `GET /api/v1/users/me/purchases` → minhas compras
  - [ ] `GET /api/v1/users/me/sales` → minhas vendas
- [ ] Feature flags: `UserActivityHistoryEnabled`
- [ ] Testes

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/UserActivityService.cs`
- `backend/Araponga.Api/Controllers/UserActivityController.cs`
- `backend/Araponga.Api/Contracts/Users/UserActivityResponse.cs`

**Critérios de Sucesso**:
- ✅ Histórico funcionando
- ✅ Filtros funcionando
- ✅ Paginação funcionando
- ✅ Testes passando

---

## 📊 Resumo da Fase 11

| Tarefa | Estimativa | Status | Prioridade |
|--------|------------|--------|------------|
| Edição de Posts | 24h | ❌ Pendente | 🔴 Alta |
| Edição de Eventos | 24h | ❌ Pendente | 🔴 Alta |
| Sistema de Avaliações | 32h | ❌ Pendente | 🟡 Média |
| Busca no Marketplace | 24h | ❌ Pendente | 🟡 Média |
| Histórico de Atividades | 16h | ❌ Pendente | 🟡 Média |
| **Total** | **120h (15 dias)** | | |

---

---

#### 11.X Configuração de Thresholds de Moderação
**Estimativa**: 24 horas (3 dias)  
**Status**: ⏳ Pendente  
**Prioridade**: 🔴 Alta

**Contexto**: Thresholds de moderação atualmente fixos no código (`ReportService`): janela de 7 dias, threshold de 3 reports únicos. Esta tarefa permite configuração por território (com fallback global) para políticas de moderação mais flexíveis.

**Tarefas**:
- [ ] Criar modelo de domínio `ModerationThresholdConfig`:
  - [ ] `Id`, `TerritoryId` (nullable para config global)
  - [ ] `ThresholdWindowDays` (janela de tempo, padrão: 7)
  - [ ] `ReportThreshold` (número mínimo de reports, padrão: 3)
  - [ ] `AutoAction` (enum: None, HidePost, MuteUser, etc.)
  - [ ] `Enabled` (bool, se automação está ativa)
  - [ ] `CreatedAtUtc`, `UpdatedAtUtc`
- [ ] Criar `IModerationThresholdConfigRepository` e implementações (Postgres, InMemory)
- [ ] Criar `ModerationThresholdConfigService`:
  - [ ] `GetConfigAsync(Guid territoryId, CancellationToken)` → busca config territorial ou global
  - [ ] `CreateOrUpdateConfigAsync(ModerationThresholdConfig, CancellationToken)`
  - [ ] Validação: janela mínima (1 dia), threshold mínimo (1)
- [ ] Atualizar `ReportService`:
  - [ ] Usar `ModerationThresholdConfig` ao avaliar thresholds
  - [ ] Fallback para valores padrão se não configurado
  - [ ] Aplicar `AutoAction` configurado
- [ ] Criar `ModerationThresholdConfigController`:
  - [ ] `GET /api/v1/territories/{territoryId}/moderation-threshold-config` (Curator)
  - [ ] `PUT /api/v1/territories/{territoryId}/moderation-threshold-config` (Curator)
  - [ ] `GET /api/v1/admin/moderation-threshold-config` (global, SystemAdmin)
  - [ ] `PUT /api/v1/admin/moderation-threshold-config` (global, SystemAdmin)
- [ ] Interface administrativa (DevPortal):
  - [ ] Seção para configuração de thresholds de moderação
  - [ ] Explicação de políticas automáticas
- [ ] Testes de integração
- [ ] Documentação

**Arquivos a Criar**:
- `backend/Araponga.Domain/Moderation/ModerationThresholdConfig.cs`
- `backend/Araponga.Application/Interfaces/Moderation/IModerationThresholdConfigRepository.cs`
- `backend/Araponga.Application/Services/Moderation/ModerationThresholdConfigService.cs`
- `backend/Araponga.Api/Controllers/ModerationThresholdConfigController.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresModerationThresholdConfigRepository.cs`
- `backend/Araponga.Infrastructure/InMemory/InMemoryModerationThresholdConfigRepository.cs`
- `backend/Araponga.Tests/Api/ModerationThresholdConfigIntegrationTests.cs`

**Arquivos a Modificar**:
- `backend/Araponga.Application/Services/ReportService.cs`
- `backend/Araponga.Infrastructure/InMemory/InMemoryDataStore.cs`
- `backend/Araponga.Api/Extensions/ServiceCollectionExtensions.cs`
- `backend/Araponga.Api/wwwroot/devportal/index.html`

**Critérios de Sucesso**:
- ✅ Thresholds configuráveis por território
- ✅ Fallback para valores globais funcionando
- ✅ Ações automáticas aplicadas corretamente
- ✅ Interface administrativa disponível
- ✅ Testes passando
- ✅ Documentação atualizada

**Referência**: Consulte `FASE10_CONFIG_FLEXIBILIZACAO_AVALIACAO.md` para contexto completo.

---

## ✅ Critérios de Sucesso da Fase 11

### Funcionalidades
- ✅ Edição de posts funcionando
- ✅ Edição de eventos funcionando
- ✅ Sistema de avaliações funcionando
- ✅ Busca no marketplace funcionando
- ✅ Histórico de atividades funcionando

### Qualidade
- ✅ Testes com cobertura adequada
- ✅ Documentação completa
- ✅ Feature flags implementados
- ✅ Validações e segurança implementadas

### Integração
- ✅ Integração com Fase 8 (Mídia) funcionando
- ✅ Integração com Fase 10 (Mídias em Conteúdo) funcionando
- ✅ Integração com Fase 6 (Marketplace) funcionando

---

## 🔗 Dependências

- **Fase 8**: Infraestrutura de Mídia (para editar mídias)
- **Fase 10**: Mídias em Conteúdo (para editar mídias em posts/eventos)

---

## 📝 Notas de Implementação

### Edição de Posts

**Limitações** (opcional):
- Limite de tempo para edição (ex: 24 horas após criação)
- Indicação visual de post editado
- Histórico de edições (opcional, para auditoria)

### Edição de Eventos

**Regras**:
- Não pode editar evento já realizado
- Cancelar evento notifica participantes
- Lista de participantes mostra apenas confirmados

### Sistema de Avaliações

**Regras**:
- Apenas compradores podem avaliar
- Uma avaliação por compra
- Vendedor pode responder avaliação
- Média calculada automaticamente

### Busca no Marketplace

**Performance**:
- Índices full-text no PostgreSQL
- Cache de resultados frequentes
- Paginação obrigatória

---

**Status**: ⏳ **FASE 11 PENDENTE**  
**Depende de**: Fases 8, 10  
**Crítico para**: Completa funcionalidades essenciais
