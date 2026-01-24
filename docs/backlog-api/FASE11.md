# Fase 11: Edição, Gestão e Estatísticas Completas

**Duração**: 3.6 semanas (18 dias úteis) - *Ajustado para TDD/BDD (+20%)*  
**Prioridade**: 🟡 IMPORTANTE (Completa funcionalidades essenciais)  
**Depende de**: Fase 8 (Infraestrutura Mídia), Fase 10 (Mídias em Conteúdo)  
**Estimativa Total**: 144 horas (120h + 24h TDD/BDD)  
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
**Status**: ✅ **Implementado**

**Tarefas**:
- [x] Estender `Post` domain model:
  - [x] `EditedAtUtc?` (nullable) — ✅ Implementado em `CommunityPost`
  - [x] `EditCount` (int, contador de edições) — ✅ Implementado em `CommunityPost`
- [x] Criar `PostEditService`:
  - [x] `EditPostAsync(Guid postId, Guid userId, ...)` → editar post — ✅ Implementado
  - [ ] `GetPostEditHistoryAsync(Guid postId)` → histórico de edições (opcional) — ⏳ Opcional, não implementado
- [x] Validações:
  - [x] Apenas autor pode editar — ✅ Implementado
  - [ ] Limite de tempo para edição? (opcional, configurável) — ⏳ Opcional, não implementado
- [x] Criar `FeedController` endpoint:
  - [x] `PATCH /api/v1/feed/{id}` → editar post — ✅ Implementado
- [ ] Feature flags: `PostEditingEnabled` — ⏳ Opcional, não implementado
- [x] Testes — ✅ `PostEditServiceTests.cs` existe

**Arquivos Criados**:
- ✅ `backend/Araponga.Application/Services/PostEditService.cs`
- ✅ `backend/Araponga.Api/Contracts/Feed/EditPostRequest.cs`
- ✅ `backend/Araponga.Tests/Application/PostEditServiceTests.cs`

**Arquivos Modificados**:
- ✅ `backend/Araponga.Domain/Feed/CommunityPost.cs` (campos `EditedAtUtc`, `EditCount` adicionados)
- ✅ `backend/Araponga.Api/Controllers/FeedController.cs` (endpoint `PATCH /api/v1/feed/{id}`)

**Critérios de Sucesso**:
- ✅ Edição de posts funcionando
- ✅ Validações funcionando
- ✅ Testes passando

---

#### 11.2 Edição de Eventos
**Estimativa**: 24 horas (3 dias)  
**Status**: ✅ **Implementado**

**Tarefas**:
- [x] Estender `TerritoryEvent` domain model:
  - [x] Método `Update(...)` — ✅ Implementado
  - [x] Método `Cancel(...)` — ✅ Implementado
- [x] Criar métodos em `EventsService`:
  - [x] `UpdateEventAsync(Guid eventId, Guid userId, ...)` → editar evento — ✅ Implementado
  - [x] `CancelEventAsync(Guid eventId, Guid userId, string reason)` → cancelar evento — ✅ Implementado
  - [x] `GetEventParticipantsAsync(Guid eventId, ...)` → lista de participantes — ✅ Implementado
- [x] Validações:
  - [x] Apenas organizador pode editar — ✅ Implementado
  - [x] Não pode editar evento já realizado — ✅ Implementado
- [x] Criar `EventsController` endpoints:
  - [x] `PATCH /api/v1/events/{id}` → editar evento — ✅ Implementado
  - [x] `GET /api/v1/events/{id}/participants` → lista de participantes — ✅ Implementado
- [ ] Feature flags: `EventEditingEnabled` — ⏳ Opcional, não implementado
- [x] Testes — ✅ Testes em `ApplicationServiceTests.cs` cobrem UpdateEventAsync, CancelEventAsync, GetEventParticipantsAsync

**Arquivos Criados**:
- ✅ `backend/Araponga.Api/Contracts/Events/EditEventRequest.cs`
- ✅ `backend/Araponga.Api/Contracts/Events/EventParticipantResponse.cs`

**Arquivos Modificados**:
- ✅ `backend/Araponga.Domain/Events/TerritoryEvent.cs` (métodos `Update` e `Cancel` implementados)
- ✅ `backend/Araponga.Application/Services/EventsService.cs` (UpdateEventAsync, CancelEventAsync, GetEventParticipantsAsync)
- ✅ `backend/Araponga.Api/Controllers/EventsController.cs` (endpoints implementados)

**Critérios de Sucesso**:
- ✅ Edição de eventos funcionando
- ✅ Cancelamento funcionando
- ✅ Lista de participantes funcionando
- ✅ Testes passando

---

### Semana 13: Avaliações e Busca no Marketplace

#### 11.3 Sistema de Avaliações no Marketplace
**Estimativa**: 32 horas (4 dias)  
**Status**: ✅ **Implementado**

**Tarefas**:
- [x] Criar modelo `StoreRating`:
  - [x] `Id`, `StoreId`, `UserId` — ✅ Implementado
  - [x] `Rating` (int, 1-5) — ✅ Implementado
  - [x] `Comment?` (nullable) — ✅ Implementado
  - [x] `CreatedAtUtc`, `UpdatedAtUtc` — ✅ Implementado
- [x] Criar modelo `StoreItemRating`:
  - [x] `Id`, `StoreItemId`, `UserId` — ✅ Implementado
  - [x] `Rating` (int, 1-5) — ✅ Implementado
  - [x] `Comment?` (nullable) — ✅ Implementado
  - [x] `CreatedAtUtc`, `UpdatedAtUtc` — ✅ Implementado
- [x] Criar modelo `StoreRatingResponse`:
  - [x] `Id`, `RatingId`, `StoreId` — ✅ Implementado
  - [x] `ResponseText` (string) — ✅ Implementado
  - [x] `CreatedAtUtc` — ✅ Implementado
- [x] Criar `RatingService`:
  - [x] `RateStoreAsync(Guid storeId, Guid userId, int rating, string? comment)` → avaliar loja — ✅ Implementado
  - [x] `RateItemAsync(Guid itemId, Guid userId, int rating, string? comment)` → avaliar item — ✅ Implementado
  - [x] `RespondToRatingAsync(Guid ratingId, Guid storeId, string response)` → responder avaliação — ✅ Implementado
  - [x] `ListStoreRatingsAsync(Guid storeId, ...)` → listar avaliações da loja — ✅ Implementado
  - [x] `ListItemRatingsAsync(Guid itemId, ...)` → listar avaliações do item — ✅ Implementado
  - [x] `GetStoreAverageRatingAsync(Guid storeId)` → média de avaliações — ✅ Implementado
- [x] Criar `RatingController`:
  - [x] `POST /api/v1/stores/{id}/ratings` → avaliar loja — ✅ Implementado
  - [x] `GET /api/v1/stores/{id}/ratings` → listar avaliações — ✅ Implementado
  - [x] `POST /api/v1/items/{id}/ratings` → avaliar item — ✅ Implementado
  - [x] `GET /api/v1/items/{id}/ratings` → listar avaliações — ✅ Implementado
  - [x] `POST /api/v1/ratings/{id}/response` → responder avaliação — ✅ Implementado
- [ ] Feature flags: `MarketplaceRatingsEnabled` — ⏳ Opcional, não implementado
- [x] Validações — ✅ Implementado
- [x] Testes — ✅ `RatingServiceTests.cs` existe

**Arquivos Criados**:
- ✅ `backend/Araponga.Domain/Marketplace/StoreRating.cs`
- ✅ `backend/Araponga.Domain/Marketplace/StoreItemRating.cs`
- ✅ `backend/Araponga.Domain/Marketplace/StoreRatingResponse.cs`
- ✅ `backend/Araponga.Application/Interfaces/IStoreRatingRepository.cs`
- ✅ `backend/Araponga.Application/Interfaces/IStoreItemRatingRepository.cs`
- ✅ `backend/Araponga.Application/Services/RatingService.cs`
- ✅ `backend/Araponga.Api/Controllers/RatingController.cs`
- ✅ `backend/Araponga.Api/Contracts/Marketplace/CreateRatingRequest.cs`
- ✅ `backend/Araponga.Api/Contracts/Marketplace/RatingResponse.cs`
- ✅ `backend/Araponga.Tests/Application/RatingServiceTests.cs`

**Critérios de Sucesso**:
- ✅ Sistema de avaliações funcionando
- ✅ Respostas funcionando
- ✅ Médias calculadas
- ✅ Testes passando

---

#### 11.4 Busca no Marketplace
**Estimativa**: 24 horas (3 dias)  
**Status**: ✅ **Implementado**

**Tarefas**:
- [x] Criar `MarketplaceSearchService`:
  - [x] `SearchStoresAsync(string query, SearchFilters, ...)` → buscar lojas — ✅ Implementado
  - [x] `SearchItemsAsync(string query, SearchFilters, ...)` → buscar itens — ✅ Implementado
  - [x] `SearchAllAsync(string query, SearchFilters, ...)` → buscar tudo — ✅ Implementado
- [x] Implementar busca full-text:
  - [x] Usar PostgreSQL full-text search — ✅ Implementado (migration `20250123130000_AddFullTextSearchIndexes.cs`)
  - [x] Índices GIN para performance — ✅ Implementado
  - [x] Ranking por relevância — ✅ Implementado
- [x] Filtros:
  - [x] Por categoria — ✅ Implementado
  - [x] Por faixa de preço — ✅ Implementado
  - [x] Por localização (raio) — ✅ Implementado
  - [x] Por rating mínimo — ✅ Implementado
- [x] Ordenação:
  - [x] Por relevância (padrão) — ✅ Implementado
  - [x] Por preço (crescente/decrescente) — ✅ Implementado
  - [x] Por data (mais recente) — ✅ Implementado
  - [x] Por rating (maior) — ✅ Implementado
- [x] Criar `MarketplaceSearchController`:
  - [x] `GET /api/v1/marketplace/search` → busca geral — ✅ Implementado
  - [x] `GET /api/v1/stores/search` → buscar lojas — ✅ Implementado
  - [x] `GET /api/v1/items/search` → buscar itens — ✅ Implementado
- [ ] Feature flags: `MarketplaceSearchEnabled` — ⏳ Opcional, não implementado
- [x] Testes — ✅ Testes existem

**Arquivos Criados**:
- ✅ `backend/Araponga.Application/Services/MarketplaceSearchService.cs`
- ✅ `backend/Araponga.Application/Models/SearchFilters.cs`
- ✅ `backend/Araponga.Api/Controllers/MarketplaceSearchController.cs`
- ✅ `backend/Araponga.Api/Contracts/Marketplace/SearchRequest.cs`
- ✅ `backend/Araponga.Api/Contracts/Marketplace/SearchResponse.cs`

**Arquivos Modificados**:
- ✅ `backend/Araponga.Infrastructure/Postgres/Migrations/20250123130000_AddFullTextSearchIndexes.cs` (índices full-text adicionados)
- ✅ `backend/Araponga.Infrastructure/Postgres/PostgresStoreItemRepository.cs` (full-text search implementado)

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
**Status**: ✅ **Implementado**

**Tarefas**:
- [x] Criar `UserActivityService`:
  - [x] `GetUserPostsAsync(Guid userId, ...)` → posts criados — ✅ Implementado
  - [x] `GetUserEventsAsync(Guid userId, ...)` → eventos criados — ✅ Implementado
  - [x] `GetUserPurchasesAsync(Guid userId, ...)` → compras — ✅ Implementado
  - [x] `GetUserSalesAsync(Guid userId, ...)` → vendas — ✅ Implementado
  - [x] `GetUserParticipationsAsync(Guid userId, ...)` → participações — ✅ Implementado
  - [x] `GetUserActivityHistoryAsync(Guid userId, ...)` → histórico completo — ✅ Implementado
- [x] Integrar com serviços existentes:
  - [x] `FeedService` → posts — ✅ Implementado
  - [x] `EventsService` → eventos — ✅ Implementado
  - [x] `CartService` → compras/vendas — ✅ Implementado
- [x] Criar `UserActivityController`:
  - [x] `GET /api/v1/users/me/activity` → histórico completo — ✅ Implementado
  - [x] `GET /api/v1/users/me/posts` → meus posts — ✅ Implementado
  - [x] `GET /api/v1/users/me/events` → meus eventos — ✅ Implementado
  - [x] `GET /api/v1/users/me/purchases` → minhas compras — ✅ Implementado
  - [x] `GET /api/v1/users/me/sales` → minhas vendas — ✅ Implementado
- [ ] Feature flags: `UserActivityHistoryEnabled` — ⏳ Opcional, não implementado
- [x] Testes — ✅ `UserActivityServiceTests.cs` existe

**Arquivos Criados**:
- ✅ `backend/Araponga.Application/Services/UserActivityService.cs`
- ✅ `backend/Araponga.Application/Models/UserActivityHistory.cs`
- ✅ `backend/Araponga.Api/Controllers/UserActivityController.cs`
- ✅ `backend/Araponga.Api/Contracts/Users/UserActivityHistoryResponse.cs`
- ✅ `backend/Araponga.Tests/Application/UserActivityServiceTests.cs`

**Critérios de Sucesso**:
- ✅ Histórico funcionando
- ✅ Filtros funcionando
- ✅ Paginação funcionando
- ✅ Testes passando

---

## 🧪 Estratégia TDD/BDD

### Contexto

Esta fase segue o padrão estabelecido na **Fase 0: Fundação TDD/BDD** e o [Plano de Distribuição TDD/BDD](./PLANO_DISTRIBUICAO_TDD_BDD.md), garantindo:
- ✅ **TDD obrigatório**: Testes escritos ANTES do código (Red-Green-Refactor)
- ✅ **BDD para funcionalidades de negócio**: Features Gherkin documentam comportamento
- ✅ **Cobertura >90%**: Meta obrigatória para todas as funcionalidades

### Tempo Adicional Estimado

- **+20% de tempo** para implementação TDD/BDD
- **Duração ajustada**: 15 dias → 18 dias (120h → 144h)
- **Tempo adicional**: +3 dias para TDD/BDD

---

### TDD: Test-Driven Development

#### Processo Red-Green-Refactor

Para cada funcionalidade implementada nesta fase:

1. **Red**: Escrever teste que falha
2. **Green**: Implementar mínimo para passar
3. **Refactor**: Melhorar código mantendo testes passando

#### Testes Obrigatórios

**Para cada funcionalidade**:
- [ ] Testes unitários (Domain, Application)
- [ ] Testes de integração (API, E2E)
- [ ] Testes de validação (edge cases, erros)
- [ ] Testes de autorização (apenas autor pode editar)

**Cobertura mínima**: ✅ **>90%** para todas as funcionalidades

---

### BDD: Behavior-Driven Development

#### Features Gherkin Obrigatórias

**Estrutura de arquivo**:
```
backend/Araponga.Tests/
├── Api/BDD/
│   ├── PostEditing.feature
│   ├── EventEditing.feature
│   ├── MarketplaceRatings.feature
│   └── MarketplaceSearch.feature
└── Application/BDD/
    ├── EditPost.feature
    └── EditEvent.feature
```

#### Features BDD Obrigatórias para Esta Fase

- [ ] `Feature: Editar Post` - Fluxo de edição com validações e histórico
- [ ] `Feature: Editar Evento` - Fluxo de edição, cancelamento e participantes
- [ ] `Feature: Avaliar Item` - Sistema de avaliações do marketplace
- [ ] `Feature: Buscar no Marketplace` - Busca full-text com filtros e ordenação

**Exemplo de Feature**:
```gherkin
Feature: Editar Post
  Como um autor de post
  Eu quero editar meu post
  Para corrigir erros ou atualizar informações

  Background:
    Dado que existe um território "Vale do Itamambuca"
    E que existe um usuário "João" como residente
    E que existe um post criado por "João"

  Scenario: Editar post com sucesso
    Dado que o usuário "João" está autenticado
    Quando ele edita o post alterando o título para "Novo título"
    Então o post deve ser atualizado com sucesso
    E o post deve ter a flag "EditedAtUtc" preenchida

  Scenario: Tentar editar post de outro usuário
    Dado que existe um usuário "Maria" como residente
    E que "Maria" está autenticada
    Quando ela tenta editar o post de "João"
    Então deve retornar erro "Unauthorized"
```

---

### Checklist TDD/BDD por Funcionalidade

**Edição de Posts**:
- [ ] Teste escrito ANTES do código (Red)
- [ ] Teste passa após implementação (Green)
- [ ] Código refatorado mantendo testes verdes (Refactor)
- [ ] Cobertura >90%
- [ ] Feature BDD criada
- [ ] Testes de autorização (apenas autor pode editar)
- [ ] Testes de histórico de edições

**Edição de Eventos**:
- [ ] Teste escrito ANTES do código (Red)
- [ ] Teste passa após implementação (Green)
- [ ] Código refatorado mantendo testes verdes (Refactor)
- [ ] Cobertura >90%
- [ ] Feature BDD criada
- [ ] Testes de cancelamento de evento
- [ ] Testes de lista de participantes

**Sistema de Avaliações**:
- [ ] Teste escrito ANTES do código (Red)
- [ ] Teste passa após implementação (Green)
- [ ] Código refatorado mantendo testes verdes (Refactor)
- [ ] Cobertura >90%
- [ ] Feature BDD criada
- [ ] Testes de validação (apenas compradores podem avaliar)
- [ ] Testes de resposta de vendedor

**Busca no Marketplace**:
- [ ] Teste escrito ANTES do código (Red)
- [ ] Teste passa após implementação (Green)
- [ ] Código refatorado mantendo testes verdes (Refactor)
- [ ] Cobertura >90%
- [ ] Feature BDD criada
- [ ] Testes de performance (busca < 500ms)
- [ ] Testes de filtros e ordenação

---

### Métricas de Sucesso

**Ao final da fase**:
- ✅ Cobertura de código >90%
- ✅ Todas as funcionalidades de negócio com BDD (4 features)
- ✅ 100% dos testes passando
- ✅ Nenhum teste ignorado ou comentado
- ✅ Documentação BDD atualizada

---

### Referências

- [Plano Completo TDD/BDD](../23_TDD_BDD_PLANO_IMPLEMENTACAO.md)
- [Plano de Distribuição TDD/BDD](./PLANO_DISTRIBUICAO_TDD_BDD.md)
- [Fase 0: Fundação TDD/BDD](./FASE0.md)
- [Template TDD/BDD para Fases](./TEMPLATE_TDD_BDD_FASES.md)

---

## 📊 Resumo da Fase 11

| Tarefa | Estimativa | Status | Prioridade |
|--------|------------|--------|------------|
| Edição de Posts | 24h | ✅ Implementado | 🔴 Alta |
| Edição de Eventos | 24h | ✅ Implementado | 🔴 Alta |
| Sistema de Avaliações | 32h | ✅ Implementado | 🟡 Média |
| Busca no Marketplace | 24h | ✅ Implementado | 🟡 Média |
| Histórico de Atividades | 16h | ✅ Implementado | 🟡 Média |
| **Total** | **120h (15 dias)** | ✅ **Completo** | |

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

**Status**: ✅ **FASE 11 IMPLEMENTADA**  
**Depende de**: Fases 8, 10  
**Crítico para**: Completa funcionalidades essenciais

---

## 📝 Notas de Implementação

### Arquivos Implementados

**Edição de Posts**:
- ✅ `backend/Araponga.Application/Services/PostEditService.cs`
- ✅ `backend/Araponga.Domain/Feed/CommunityPost.cs` (campos `EditedAtUtc`, `EditCount`)
- ✅ `backend/Araponga.Api/Controllers/FeedController.cs` (endpoint `PATCH /api/v1/feed/{id}`)

**Edição de Eventos**:
- ✅ `backend/Araponga.Application/Services/EventsService.cs` (UpdateEventAsync, CancelEventAsync, GetEventParticipantsAsync)
- ✅ `backend/Araponga.Domain/Events/TerritoryEvent.cs` (métodos `Update`, `Cancel`)
- ✅ `backend/Araponga.Api/Controllers/EventsController.cs` (endpoints implementados)

**Sistema de Avaliações**:
- ✅ `backend/Araponga.Application/Services/RatingService.cs`
- ✅ `backend/Araponga.Domain/Marketplace/StoreRating.cs`
- ✅ `backend/Araponga.Domain/Marketplace/StoreItemRating.cs`
- ✅ `backend/Araponga.Api/Controllers/RatingController.cs`

**Busca no Marketplace**:
- ✅ `backend/Araponga.Application/Services/MarketplaceSearchService.cs`
- ✅ `backend/Araponga.Infrastructure/Postgres/Migrations/20250123130000_AddFullTextSearchIndexes.cs`
- ✅ `backend/Araponga.Api/Controllers/MarketplaceSearchController.cs`

**Histórico de Atividades**:
- ✅ `backend/Araponga.Application/Services/UserActivityService.cs`
- ✅ `backend/Araponga.Api/Controllers/UserActivityController.cs`

---

**Última atualização**: 2025-01-23
