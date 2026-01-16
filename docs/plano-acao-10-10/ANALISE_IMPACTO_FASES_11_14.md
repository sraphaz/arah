# Análise de Impacto: Fases 11-14 nas Funcionalidades Existentes

**Data**: 2025-01-13  
**Objetivo**: Identificar todos os ajustes necessários nas funcionalidades existentes devido às novas funcionalidades das Fases 11-14

---

## 🎯 Resumo Executivo

As Fases 11-14 introduzem **mídias** e **novas funcionalidades** que impactam **todas as funcionalidades existentes**:

- **Feed/Posts**: Adicionar mídias, edição, exclusão
- **Eventos**: Adicionar mídias, edição, lista de participantes
- **Marketplace**: Adicionar mídias, avaliações, busca
- **Chat**: Adicionar mídias
- **Perfil**: Avatar, bio, estatísticas, histórico
- **Controllers**: Todos precisam suportar `multipart/form-data`
- **Responses/DTOs**: Todos precisam incluir URLs de mídia
- **Repositórios**: Podem precisar ajustes para queries com mídias
- **Cache**: Precisa invalidar quando mídias são adicionadas/removidas

---

## 📊 Impacto por Funcionalidade Existente

### 1. Feed/Posts

#### Estado Atual
- ✅ `FeedService` existe
- ✅ `PostCreationService` existe
- ✅ `PostInteractionService` existe
- ✅ `PostFilterService` existe
- ✅ `FeedController` existe
- ✅ `CommunityPost` (domínio) existe
- ✅ `FeedItemResponse` existe

#### Impacto das Fases 11-14

**Fase 11 (Infraestrutura de Mídia)**:
- ⚠️ **Nenhum impacto direto** (apenas preparação)

**Fase 13 (Mídias em Posts)**:
- 🔴 **Alto Impacto**:
  - [ ] `PostCreationService.CreatePostAsync` precisa aceitar `IReadOnlyList<Guid> mediaAssetIds`
  - [ ] `FeedService.CreatePostAsync` precisa passar mídias para `PostCreationService`
  - [ ] `FeedController.POST /api/v1/feed` precisa aceitar `multipart/form-data` com `images[]`
  - [ ] `FeedItemResponse` precisa incluir `ImageUrls[]`
  - [ ] `FeedService.ListForTerritoryAsync` precisa buscar mídias associadas aos posts
  - [ ] `PostFilterService` pode precisar filtrar posts com/sem mídias (opcional)

**Fase 13 (Excluir Post)**:
- 🟡 **Médio Impacto**:
  - [ ] `FeedService` precisa de método `DeletePostAsync`
  - [ ] `FeedController` precisa de endpoint `DELETE /api/v1/feed/{id}`
  - [ ] Deletar mídias associadas ao post

**Fase 14 (Editar Post)**:
- 🟡 **Médio Impacto**:
  - [ ] `FeedService` precisa de método `UpdatePostAsync`
  - [ ] `PostCreationService` pode precisar de método `UpdatePostAsync` ou criar `PostUpdateService`
  - [ ] `FeedController` precisa de endpoint `PUT /api/v1/feed/{id}`
  - [ ] Gerenciar mídias (adicionar novas, remover antigas)

#### Arquivos a Modificar

**Fase 13**:
- `backend/Araponga.Application/Services/PostCreationService.cs`
- `backend/Araponga.Application/Services/FeedService.cs`
- `backend/Araponga.Api/Controllers/FeedController.cs`
- `backend/Araponga.Api/Contracts/Feed/CreatePostRequest.cs`
- `backend/Araponga.Api/Contracts/Feed/FeedItemResponse.cs`
- `backend/Araponga.Application/Services/PostFilterService.cs` (opcional)

**Fase 14**:
- `backend/Araponga.Application/Services/FeedService.cs`
- `backend/Araponga.Application/Services/PostCreationService.cs` (ou criar `PostUpdateService`)
- `backend/Araponga.Api/Controllers/FeedController.cs`

#### Migrações de Dados
- ⚠️ **Nenhuma migração necessária** (posts existentes continuam funcionando, apenas sem mídias)

---

### 2. Eventos

#### Estado Atual
- ✅ `EventsService` existe
- ✅ `EventsController` existe
- ✅ `TerritoryEvent` (domínio) existe
- ✅ `EventSummary` existe

#### Impacto das Fases 11-14

**Fase 11 (Infraestrutura de Mídia)**:
- ⚠️ **Nenhum impacto direto**

**Fase 13 (Mídias em Eventos)**:
- 🟡 **Médio Impacto**:
  - [ ] `EventsService.CreateEventAsync` precisa aceitar `Guid? coverImageMediaAssetId`
  - [ ] `EventsController.POST /api/v1/events` precisa aceitar `multipart/form-data` com `coverImage`
  - [ ] `EventSummary` precisa incluir `CoverImageUrl`
  - [ ] `EventsService.BuildSummariesAsync` precisa buscar mídias associadas aos eventos

**Fase 14 (Editar Evento)**:
- 🟡 **Médio Impacto**:
  - [ ] `EventsService` precisa de método `UpdateEventAsync`
  - [ ] `EventsController` precisa de endpoint `PUT /api/v1/events/{id}`
  - [ ] Gerenciar imagem de capa (substituir se nova)

**Fase 14 (Lista de Participantes)**:
- 🟡 **Médio Impacto**:
  - [ ] `EventsService` precisa de método `GetParticipantsAsync`
  - [ ] `EventsController` precisa de endpoint `GET /api/v1/events/{id}/participants`
  - [ ] Buscar participantes com avatares (usa Fase 12)

#### Arquivos a Modificar

**Fase 13**:
- `backend/Araponga.Application/Services/EventsService.cs`
- `backend/Araponga.Api/Controllers/EventsController.cs`
- `backend/Araponga.Api/Contracts/Events/CreateEventRequest.cs`
- `backend/Araponga.Api/Contracts/Events/EventSummary.cs`

**Fase 14**:
- `backend/Araponga.Application/Services/EventsService.cs`
- `backend/Araponga.Api/Controllers/EventsController.cs`

#### Migrações de Dados
- ⚠️ **Nenhuma migração necessária** (eventos existentes continuam funcionando, apenas sem imagem de capa)

---

### 3. Marketplace

#### Estado Atual
- ✅ `StoreService` existe
- ✅ `StoreItemService` existe
- ✅ `ItemsController` existe
- ✅ `Store` (domínio) existe
- ✅ `StoreItem` (domínio) existe
- ✅ `StoreItemResponse` existe

#### Impacto das Fases 11-14

**Fase 11 (Infraestrutura de Mídia)**:
- ⚠️ **Nenhum impacto direto**

**Fase 13 (Mídias em Anúncios)**:
- 🟡 **Médio Impacto**:
  - [ ] `StoreItemService.CreateItemAsync` precisa aceitar `IReadOnlyList<Guid> imageMediaAssetIds`
  - [ ] `ItemsController.POST /api/v1/items` precisa aceitar `multipart/form-data` com `images[]`
  - [ ] `StoreItemResponse` precisa incluir `ImageUrls[]`
  - [ ] `StoreItemService.ListItemsAsync` precisa buscar mídias associadas aos itens

**Fase 14 (Avaliações)**:
- 🔴 **Alto Impacto**:
  - [ ] Criar novo modelo `StoreReview` (domínio)
  - [ ] Criar `StoreReviewService` (novo serviço)
  - [ ] Criar `StoreReviewsController` (novo controller)
  - [ ] `StoreItemResponse` pode incluir `AverageRating` e `ReviewCount` (opcional)

**Fase 14 (Busca)**:
- 🟡 **Médio Impacto**:
  - [ ] `StoreItemService` precisa de método `SearchItemsAsync`
  - [ ] `ItemsController` precisa de endpoint `GET /api/v1/items/search`
  - [ ] `IStoreItemRepository` pode precisar de método `SearchAsync` com full-text search

#### Arquivos a Modificar

**Fase 13**:
- `backend/Araponga.Application/Services/StoreItemService.cs`
- `backend/Araponga.Api/Controllers/ItemsController.cs`
- `backend/Araponga.Api/Contracts/Marketplace/CreateItemRequest.cs`
- `backend/Araponga.Api/Contracts/Marketplace/StoreItemResponse.cs`

**Fase 14**:
- `backend/Araponga.Application/Services/StoreItemService.cs`
- `backend/Araponga.Api/Controllers/ItemsController.cs`
- `backend/Araponga.Application/Interfaces/IStoreItemRepository.cs` (adicionar `SearchAsync`)

#### Arquivos a Criar (Fase 14)

**Avaliações**:
- `backend/Araponga.Domain/Marketplace/StoreReview.cs`
- `backend/Araponga.Application/Interfaces/IStoreReviewRepository.cs`
- `backend/Araponga.Application/Services/StoreReviewService.cs`
- `backend/Araponga.Api/Controllers/StoreReviewsController.cs`

#### Migrações de Dados
- ⚠️ **Nenhuma migração necessária** (itens existentes continuam funcionando, apenas sem imagens)
- ✅ **Nova tabela**: `store_reviews` (Fase 14)

---

### 4. Chat

#### Estado Atual
- ✅ `ChatService` existe
- ✅ `ChatController` existe
- ✅ `ChatMessage` (domínio) existe
- ✅ `ChatMessageResponse` existe

#### Impacto das Fases 11-14

**Fase 11 (Infraestrutura de Mídia)**:
- ⚠️ **Nenhum impacto direto**

**Fase 13 (Mídias em Mensagens)**:
- 🟡 **Médio Impacto**:
  - [ ] `ChatMessage` (domínio) precisa de campo `ContentType` (se não existe) ou `MediaAssetId`
  - [ ] `ChatService.SendMessageAsync` precisa aceitar `Guid? imageMediaAssetId`
  - [ ] `ChatController.POST /api/v1/chat/conversations/{id}/messages` precisa aceitar `multipart/form-data` com `image`
  - [ ] `ChatMessageResponse` precisa incluir `ImageUrl` (se tipo for Image)
  - [ ] `ChatService.ListMessagesAsync` precisa buscar mídias associadas às mensagens

#### Arquivos a Modificar

**Fase 13**:
- `backend/Araponga.Domain/Chat/ChatMessage.cs` (verificar se já tem `ContentType`)
- `backend/Araponga.Application/Services/ChatService.cs`
- `backend/Araponga.Api/Controllers/ChatController.cs`
- `backend/Araponga.Api/Contracts/Chat/SendMessageRequest.cs`
- `backend/Araponga.Api/Contracts/Chat/ChatMessageResponse.cs`

#### Migrações de Dados
- ⚠️ **Pode precisar migração** se `ChatMessage` não tiver campo para tipo de conteúdo
- ⚠️ **Mensagens existentes** continuam funcionando como texto

---

### 5. Perfil de Usuário

#### Estado Atual
- ✅ `UserProfileService` existe
- ✅ `UserProfileController` existe
- ✅ `User` (domínio) existe
- ✅ `UserProfileResponse` existe

#### Impacto das Fases 11-14

**Fase 11 (Infraestrutura de Mídia)**:
- ⚠️ **Nenhum impacto direto**

**Fase 12 (Perfil Completo)**:
- 🔴 **Alto Impacto**:
  - [ ] `User` (domínio) precisa de campos `AvatarMediaAssetId` e `Bio`
  - [ ] `UserProfileService` precisa de métodos:
    - [ ] `UpdateAvatarAsync`
    - [ ] `UpdateBioAsync`
    - [ ] `GetProfileAsync` (atualizar para incluir avatar e bio)
  - [ ] `UserProfileController` precisa de endpoints:
    - [ ] `PUT /api/v1/users/me/profile/avatar`
    - [ ] `PUT /api/v1/users/me/profile/bio`
    - [ ] `GET /api/v1/users/{id}/profile` (visualizar perfil de outros)
  - [ ] `UserProfileResponse` precisa incluir `AvatarUrl` e `Bio`
  - [ ] Criar `UserProfileStatsService` (novo serviço)
  - [ ] `UserProfileController` precisa de endpoints de estatísticas

#### Arquivos a Modificar

**Fase 12**:
- `backend/Araponga.Domain/Users/User.cs`
- `backend/Araponga.Application/Services/UserProfileService.cs`
- `backend/Araponga.Api/Controllers/UserProfileController.cs`
- `backend/Araponga.Api/Contracts/Users/UserProfileResponse.cs`

#### Arquivos a Criar (Fase 12)

**Estatísticas**:
- `backend/Araponga.Application/Services/UserProfileStatsService.cs`
- `backend/Araponga.Application/Models/UserProfileStats.cs`
- `backend/Araponga.Api/Contracts/Users/UserProfileStatsResponse.cs`

**Fase 14 (Histórico de Atividades)**:
- [ ] Criar `UserActivityService` (novo serviço)
- [ ] `UserProfileController` precisa de endpoint `GET /api/v1/users/{id}/activity`

#### Migrações de Dados
- ✅ **Nova coluna**: `avatar_media_asset_id` (nullable) na tabela `users`
- ✅ **Nova coluna**: `bio` (nullable, varchar(500)) na tabela `users`
- ⚠️ **Usuários existentes** continuam funcionando, apenas sem avatar e bio

---

### 6. Controllers e DTOs

#### Impacto Geral

**Todos os Controllers que criam conteúdo**:
- 🔴 **Alto Impacto**:
  - [ ] Aceitar `multipart/form-data` além de `application/json`
  - [ ] Processar arquivos de imagem
  - [ ] Upload via `MediaService`
  - [ ] Validação de arquivos

**Todos os Responses que retornam conteúdo**:
- 🟡 **Médio Impacto**:
  - [ ] Incluir URLs de mídia
  - [ ] Incluir avatar URL (quando relevante)
  - [ ] Manter compatibilidade com versões antigas (opcional)

#### Controllers Afetados

1. **FeedController**:
   - `POST /api/v1/feed` → `multipart/form-data`
   - `PUT /api/v1/feed/{id}` → `multipart/form-data` (Fase 14)
   - `DELETE /api/v1/feed/{id}` → novo endpoint (Fase 13)
   - `GET /api/v1/feed` → incluir `ImageUrls[]` na resposta

2. **EventsController**:
   - `POST /api/v1/events` → `multipart/form-data`
   - `PUT /api/v1/events/{id}` → `multipart/form-data` (Fase 14)
   - `GET /api/v1/events/{id}/participants` → novo endpoint (Fase 14)
   - `GET /api/v1/events` → incluir `CoverImageUrl` na resposta

3. **ItemsController**:
   - `POST /api/v1/items` → `multipart/form-data`
   - `GET /api/v1/items/search` → novo endpoint (Fase 14)
   - `GET /api/v1/items` → incluir `ImageUrls[]` na resposta

4. **ChatController**:
   - `POST /api/v1/chat/conversations/{id}/messages` → `multipart/form-data`
   - `GET /api/v1/chat/conversations/{id}/messages` → incluir `ImageUrl` na resposta

5. **UserProfileController**:
   - `PUT /api/v1/users/me/profile/avatar` → `multipart/form-data` (Fase 12)
   - `GET /api/v1/users/{id}/profile` → novo endpoint (Fase 12)
   - `GET /api/v1/users/{id}/profile/stats` → novo endpoint (Fase 12)
   - `GET /api/v1/users/{id}/activity` → novo endpoint (Fase 14)

---

### 7. Repositórios

#### Impacto Geral

**Repositórios que buscam conteúdo**:
- 🟡 **Médio Impacto**:
  - [ ] Queries podem precisar incluir JOINs com `media_attachments` e `media_assets`
  - [ ] Performance pode ser afetada (necessário otimizar)
  - [ ] Cache pode precisar ser ajustado

#### Repositórios Afetados

1. **IFeedRepository**:
   - ⚠️ Queries de posts podem precisar incluir mídias
   - ⚠️ Performance: JOINs adicionais

2. **ITerritoryEventRepository**:
   - ⚠️ Queries de eventos podem precisar incluir imagem de capa
   - ⚠️ Performance: JOIN adicional

3. **IStoreItemRepository**:
   - ⚠️ Queries de itens podem precisar incluir mídias
   - ⚠️ Novo método `SearchAsync` (Fase 14)
   - ⚠️ Performance: JOINs adicionais + full-text search

4. **IChatMessageRepository**:
   - ⚠️ Queries de mensagens podem precisar incluir mídias
   - ⚠️ Performance: JOIN adicional

5. **IUserRepository**:
   - ⚠️ Queries de usuários podem precisar incluir avatar
   - ⚠️ Performance: JOIN adicional

#### Otimizações Necessárias

- [ ] Usar `Include()` do EF Core apenas quando necessário
- [ ] Lazy loading de mídias (carregar apenas quando solicitado)
- [ ] Cache de URLs de mídia
- [ ] Índices apropriados em `media_attachments` (owner_type, owner_id)

---

### 8. Cache

#### Impacto Geral

**Sistemas de Cache Existentes**:
- 🔴 **Alto Impacto**:
  - [ ] Cache precisa ser invalidado quando mídias são adicionadas/removidas
  - [ ] Cache de URLs de mídia (TTL: 1 hora)
  - [ ] Cache de estatísticas de perfil (TTL: 15 minutos)

#### Cache Services Afetados

1. **EventCacheService**:
   - ⚠️ Invalidação quando imagem de capa é adicionada/removida

2. **TerritoryCacheService**:
   - ⚠️ Pode não ser afetado diretamente

3. **CacheInvalidationService**:
   - ⚠️ Precisa invalidar cache quando mídias são modificadas

#### Ajustes Necessários

- [ ] `CacheInvalidationService` precisa invalidar cache de conteúdo quando mídias são adicionadas/removidas
- [ ] Criar `MediaCacheService` para cache de URLs de mídia
- [ ] Invalidar cache de perfil quando avatar é atualizado

---

### 9. Testes Existentes

#### Impacto Geral

**Todos os Testes de Integração**:
- 🔴 **Alto Impacto**:
  - [ ] Testes de criação de posts precisam incluir mídias (opcional)
  - [ ] Testes de criação de eventos precisam incluir imagem de capa (opcional)
  - [ ] Testes de criação de itens precisam incluir mídias (opcional)
  - [ ] Testes de criação de mensagens precisam incluir imagens (opcional)
  - [ ] Testes de perfil precisam incluir avatar e bio (opcional)

#### Testes Afetados

1. **FeedServiceTests**:
   - ⚠️ Testes de criação podem precisar incluir mídias
   - ⚠️ Novos testes para edição e exclusão

2. **EventsServiceTests**:
   - ⚠️ Testes de criação podem precisar incluir imagem de capa
   - ⚠️ Novos testes para edição e lista de participantes

3. **StoreItemServiceTests**:
   - ⚠️ Testes de criação podem precisar incluir mídias
   - ⚠️ Novos testes para busca

4. **ChatServiceTests**:
   - ⚠️ Testes de envio de mensagem podem precisar incluir imagens

5. **UserProfileServiceTests**:
   - ⚠️ Novos testes para avatar, bio, estatísticas

#### Estratégia de Testes

- ✅ **Manter compatibilidade**: Testes existentes devem continuar passando
- ✅ **Testes opcionais**: Mídias são opcionais, então testes sem mídias devem continuar funcionando
- ✅ **Novos testes**: Adicionar testes específicos para funcionalidades com mídias

---

## 📋 Plano de Ajustes por Fase

### Fase 11: Ajustes Preparatórios

**Nenhum ajuste necessário** - Fase 11 apenas cria infraestrutura, não impacta funcionalidades existentes.

---

### Fase 12: Ajustes no Perfil

**Arquivos a Modificar**:
- `backend/Araponga.Domain/Users/User.cs` (adicionar campos)
- `backend/Araponga.Application/Services/UserProfileService.cs` (novos métodos)
- `backend/Araponga.Api/Controllers/UserProfileController.cs` (novos endpoints)
- `backend/Araponga.Api/Contracts/Users/UserProfileResponse.cs` (novos campos)

**Migrações**:
- Nova coluna `avatar_media_asset_id` (nullable)
- Nova coluna `bio` (nullable, varchar(500))

**Impacto em Testes**:
- Testes existentes de perfil devem continuar passando
- Adicionar testes para avatar e bio

---

### Fase 13: Ajustes em Conteúdo

**Arquivos a Modificar**:

1. **Feed**:
   - `backend/Araponga.Application/Services/PostCreationService.cs`
   - `backend/Araponga.Application/Services/FeedService.cs`
   - `backend/Araponga.Api/Controllers/FeedController.cs`
   - `backend/Araponga.Api/Contracts/Feed/CreatePostRequest.cs`
   - `backend/Araponga.Api/Contracts/Feed/FeedItemResponse.cs`

2. **Eventos**:
   - `backend/Araponga.Application/Services/EventsService.cs`
   - `backend/Araponga.Api/Controllers/EventsController.cs`
   - `backend/Araponga.Api/Contracts/Events/CreateEventRequest.cs`
   - `backend/Araponga.Api/Contracts/Events/EventSummary.cs`

3. **Marketplace**:
   - `backend/Araponga.Application/Services/StoreItemService.cs`
   - `backend/Araponga.Api/Controllers/ItemsController.cs`
   - `backend/Araponga.Api/Contracts/Marketplace/CreateItemRequest.cs`
   - `backend/Araponga.Api/Contracts/Marketplace/StoreItemResponse.cs`

4. **Chat**:
   - `backend/Araponga.Domain/Chat/ChatMessage.cs` (verificar)
   - `backend/Araponga.Application/Services/ChatService.cs`
   - `backend/Araponga.Api/Controllers/ChatController.cs`
   - `backend/Araponga.Api/Contracts/Chat/SendMessageRequest.cs`
   - `backend/Araponga.Api/Contracts/Chat/ChatMessageResponse.cs`

**Migrações**:
- ⚠️ Pode precisar migração se `ChatMessage` não tiver campo para tipo de conteúdo

**Impacto em Testes**:
- Testes existentes devem continuar passando (mídias são opcionais)
- Adicionar testes específicos para mídias

---

### Fase 14: Ajustes em Edição e Gestão

**Arquivos a Modificar**:

1. **Feed**:
   - `backend/Araponga.Application/Services/FeedService.cs` (método `UpdatePostAsync`)
   - `backend/Araponga.Application/Services/PostCreationService.cs` (ou criar `PostUpdateService`)
   - `backend/Araponga.Api/Controllers/FeedController.cs` (endpoint `PUT`)

2. **Eventos**:
   - `backend/Araponga.Application/Services/EventsService.cs` (método `UpdateEventAsync`, `GetParticipantsAsync`)
   - `backend/Araponga.Api/Controllers/EventsController.cs` (endpoints `PUT` e `GET /participants`)

3. **Marketplace**:
   - `backend/Araponga.Application/Services/StoreItemService.cs` (método `SearchItemsAsync`)
   - `backend/Araponga.Application/Interfaces/IStoreItemRepository.cs` (método `SearchAsync`)
   - `backend/Araponga.Api/Controllers/ItemsController.cs` (endpoint `GET /search`)

4. **Perfil**:
   - `backend/Araponga.Application/Services/UserActivityService.cs` (novo serviço)
   - `backend/Araponga.Api/Controllers/UserProfileController.cs` (endpoint `GET /activity`)

**Arquivos a Criar**:

1. **Avaliações**:
   - `backend/Araponga.Domain/Marketplace/StoreReview.cs`
   - `backend/Araponga.Application/Interfaces/IStoreReviewRepository.cs`
   - `backend/Araponga.Application/Services/StoreReviewService.cs`
   - `backend/Araponga.Api/Controllers/StoreReviewsController.cs`

**Migrações**:
- Nova tabela `store_reviews`
- Índice full-text para busca no marketplace (PostgreSQL)

**Impacto em Testes**:
- Novos testes para edição, exclusão, avaliações, busca, histórico

---

## 🔄 Estratégia de Migração

### Compatibilidade Retroativa

**Princípio**: Todas as mudanças devem ser **retrocompatíveis**:
- ✅ Funcionalidades existentes continuam funcionando
- ✅ Mídias são **opcionais** (não obrigatórias)
- ✅ Campos novos são **nullable** (não quebram dados existentes)
- ✅ Endpoints antigos continuam funcionando

### Ordem de Implementação

1. **Fase 11**: Criar infraestrutura (sem impacto)
2. **Fase 12**: Perfil (isolado, pouco impacto)
3. **Fase 13**: Mídias (impacto em múltiplas funcionalidades)
4. **Fase 14**: Edição e gestão (completa funcionalidades)

### Validação de Impacto

**Checklist por Fase**:
- [ ] Testes existentes continuam passando
- [ ] Funcionalidades existentes continuam funcionando
- [ ] Novas funcionalidades funcionando
- [ ] Performance não degradada
- [ ] Cache funcionando corretamente
- [ ] Migrações testadas

---

## 📊 Resumo de Impacto

| Funcionalidade | Fase 11 | Fase 12 | Fase 13 | Fase 14 | Impacto Total |
|----------------|---------|---------|---------|---------|---------------|
| **Feed/Posts** | ⚪ Nenhum | ⚪ Nenhum | 🔴 Alto | 🟡 Médio | 🔴 Alto |
| **Eventos** | ⚪ Nenhum | ⚪ Nenhum | 🟡 Médio | 🟡 Médio | 🟡 Médio |
| **Marketplace** | ⚪ Nenhum | ⚪ Nenhum | 🟡 Médio | 🔴 Alto | 🔴 Alto |
| **Chat** | ⚪ Nenhum | ⚪ Nenhum | 🟡 Médio | ⚪ Nenhum | 🟡 Médio |
| **Perfil** | ⚪ Nenhum | 🔴 Alto | ⚪ Nenhum | 🟡 Médio | 🔴 Alto |
| **Controllers** | ⚪ Nenhum | 🟡 Médio | 🔴 Alto | 🟡 Médio | 🔴 Alto |
| **Repositórios** | ⚪ Nenhum | ⚪ Nenhum | 🟡 Médio | 🟡 Médio | 🟡 Médio |
| **Cache** | ⚪ Nenhum | 🟡 Médio | 🟡 Médio | ⚪ Nenhum | 🟡 Médio |
| **Testes** | ⚪ Nenhum | 🟡 Médio | 🟡 Médio | 🟡 Médio | 🟡 Médio |

**Legenda**:
- ⚪ Nenhum: Sem impacto
- 🟡 Médio: Alguns ajustes necessários
- 🔴 Alto: Múltiplos ajustes necessários

---

## ✅ Checklist de Validação

### Antes de Iniciar Cada Fase

- [ ] Revisar análise de impacto
- [ ] Identificar todos os arquivos a modificar
- [ ] Identificar todas as migrações necessárias
- [ ] Validar compatibilidade retroativa
- [ ] Preparar estratégia de testes

### Durante Cada Fase

- [ ] Testes existentes continuam passando
- [ ] Novas funcionalidades funcionando
- [ ] Performance validada
- [ ] Cache funcionando

### Após Cada Fase

- [ ] Todos os testes passando
- [ ] Documentação atualizada
- [ ] Changelog atualizado
- [ ] Migrações aplicadas e testadas

---

**Documento criado em**: 2025-01-13  
**Status**: ✅ Análise Completa
