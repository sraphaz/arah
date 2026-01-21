# Fix: Proteção contra Overflow Int32 e Implementação de Delete Post

## 🎯 Objetivo

Corrigir o erro recorrente `serialize binary: invalid int 32: 4294967295 [internal]` e implementar o endpoint DELETE para posts que estava faltando.

## 🔧 Mudanças Implementadas

### 1. Proteção contra Overflow Int32

#### Problema
O valor `4294967295` (uint.MaxValue) estava sendo serializado como int32, causando erro de serialização JSON.

#### Solução
Adicionada proteção em múltiplas camadas:

**A. Domain Models:**
- `CommunityPost`: Clamping de `EditCount` no construtor e no método `Edit`
- `MapEntity`: Clamping de `ConfirmationCount` no construtor

**B. Application Layer:**
- `PagedResult`: Proteção de `TotalCount` e `TotalPages`
- `PostCounts`: Clamping no construtor
- `EventParticipationCounts`: Clamping no construtor

**C. Infrastructure Layer:**
- `PostgresEventParticipationRepository`: Clamping em `GetCountsAsync`
- `InMemoryEventParticipationRepository`: Clamping em `GetCountsAsync`
- `PostgresMapRepository`: Proteção em `IncrementConfirmationAsync`
- `PostgresMappers`: Clamping em `ToDomain` methods
- `InMemoryFeedRepository`: Clamping em métodos de contagem

**D. API Controllers:**
- Todos os controllers que criam `PagedResponse` agora aplicam clamping:
  - `FeedController`
  - `EventsController`
  - `MarketplaceSearchController`
  - `MapController`
  - `ItemsController`
  - `TerritoriesController`
  - `NotificationsController`
  - `AlertsController`
  - `AssetsController`
  - `InquiriesController`
  - `JoinRequestsController`
  - `ModerationController`
  - `PlatformFeesController`
  - `UserActivityController`

**E. Helper Criado:**
- `PagedResponseHelper.cs`: Helper para normalização de valores (para uso futuro)

### 2. Implementação de Delete Post

#### Problema
O teste BDD "Deletar post remove mídias associadas" estava falhando porque o endpoint DELETE não existia.

#### Solução
Implementado endpoint completo de deleção de posts:

**A. Repository Layer:**
- `IFeedRepository`: Adicionado método `DeletePostAsync`
- `PostgresFeedRepository`: Implementação do método
- `InMemoryFeedRepository`: Implementação do método

**B. Service Layer:**
- `FeedService`: Adicionado método `DeletePostAsync` e `GetPostAsync`

**C. API Layer:**
- `FeedController`: Endpoint `DELETE /api/v1/feed/{id}`
  - Valida autenticação
  - Valida que o usuário é o autor do post
  - Deleta mídias associadas (`MediaAttachment`)
  - Deleta geo anchors associados
  - Deleta o post

### 3. Correções de Testes

**A. EventsControllerTests:**
- Corrigido construtor de `UpdateEventRequest` (faltavam parâmetros `CoverMediaId` e `AdditionalMediaIds`)

**B. PostEditServiceTests:**
- Substituído `PostType.Community` por `PostType.General` (16 ocorrências)
- Substituído `PostStatus.Active` por `PostStatus.Published` (16 ocorrências)
- Corrigido namespace `MediaType` (de `Domain.Media.MediaType` para `MediaType`)
- Adicionados parâmetros faltantes nos construtores de `MediaAsset` (`deletedByUserId`, `deletedAtUtc`)

## 📊 Resultados

### Build Status
- ✅ API: Build bem-sucedido (0 warnings, 0 erros)
- ✅ Tests: Build bem-sucedido (0 warnings, 0 erros)

### Test Results
- ✅ **671 testes passando**
- ✅ **0 testes falhando**
- ⏭️ **2 testes skipped**

### Testes Específicos
- ✅ Teste "Deletar post remove mídias associadas" agora passa

## 🔍 Arquivos Modificados

### Domain
- `backend/Araponga.Domain/Feed/CommunityPost.cs`
- `backend/Araponga.Domain/Map/MapEntity.cs`

### Application
- `backend/Araponga.Application/Common/PagedResult.cs`
- `backend/Araponga.Application/Common/PostCounts.cs`
- `backend/Araponga.Application/Models/EventParticipationCounts.cs`
- `backend/Araponga.Application/Services/FeedService.cs`
- `backend/Araponga.Application/Interfaces/IFeedRepository.cs`

### Infrastructure
- `backend/Araponga.Infrastructure/Postgres/PostgresFeedRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresEventParticipationRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresMapRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresMappers.cs`
- `backend/Araponga.Infrastructure/InMemory/InMemoryFeedRepository.cs`
- `backend/Araponga.Infrastructure/InMemory/InMemoryEventParticipationRepository.cs`

### API
- `backend/Araponga.Api/Controllers/FeedController.cs`
- `backend/Araponga.Api/Controllers/EventsController.cs`
- `backend/Araponga.Api/Controllers/MarketplaceSearchController.cs`
- `backend/Araponga.Api/Controllers/MapController.cs`
- `backend/Araponga.Api/Controllers/ItemsController.cs`
- `backend/Araponga.Api/Controllers/TerritoriesController.cs`
- `backend/Araponga.Api/Controllers/NotificationsController.cs`
- `backend/Araponga.Api/Controllers/AlertsController.cs`
- `backend/Araponga.Api/Controllers/AssetsController.cs`
- `backend/Araponga.Api/Controllers/InquiriesController.cs`
- `backend/Araponga.Api/Controllers/JoinRequestsController.cs`
- `backend/Araponga.Api/Controllers/ModerationController.cs`
- `backend/Araponga.Api/Controllers/PlatformFeesController.cs`
- `backend/Araponga.Api/Controllers/UserActivityController.cs`
- `backend/Araponga.Api/Helpers/PagedResponseHelper.cs` (novo)

### Tests
- `backend/Araponga.Tests/Api/EventsControllerTests.cs`
- `backend/Araponga.Tests/Application/PostEditServiceTests.cs`

## ✅ Checklist

- [x] Build da API sem erros
- [x] Build dos testes sem erros
- [x] Todos os testes passando (671/673)
- [x] Proteção contra overflow em todas as camadas
- [x] Endpoint DELETE de posts implementado
- [x] Deleção de mídias associadas implementada
- [x] Deleção de geo anchors implementada
- [x] Validação de autor implementada

## 🚀 Próximos Passos

1. Criar migração de banco de dados (se necessário para DELETE CASCADE)
2. Adicionar testes de integração para o endpoint DELETE
3. Documentar o novo endpoint na API

## 📝 Notas

- O erro `serialize binary: invalid int 32: 4294967295` foi resolvido através de proteção em múltiplas camadas
- O endpoint DELETE segue o mesmo padrão de segurança dos outros endpoints (validação de autor)
- As mídias são deletadas via `MediaAttachmentRepository.DeleteByOwnerAsync`, que remove apenas os vínculos, não os arquivos físicos (para auditoria)
