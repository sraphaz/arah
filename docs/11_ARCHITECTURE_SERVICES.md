# Arquitetura de Services

Este documento descreve a arquitetura dos services da camada de aplicação do Araponga, especialmente após a refatoração do `FeedService`.

## Princípios de Design

### Single Responsibility Principle (SRP)
Cada service tem uma única responsabilidade bem definida:
- **PostCreationService**: Apenas criação de posts
- **PostInteractionService**: Apenas interações (likes, comentários, shares)
- **PostFilterService**: Apenas filtragem e paginação
- **FeedService**: Orquestração e coordenação dos services especializados

### Dependency Injection
Todos os services são registrados via Dependency Injection em `ServiceCollectionExtensions`:
- Services são `Scoped` (uma instância por requisição HTTP)
- Dependências são injetadas via construtor
- Interfaces permitem fácil substituição para testes

## Services do Feed

### FeedService
**Responsabilidade**: Orquestrar operações de feed, delegando para services especializados.

**Dependências** (4):
- `IFeedRepository`: Acesso aos dados de posts
- `PostCreationService`: Criação de posts
- `PostInteractionService`: Interações com posts
- `PostFilterService`: Filtragem e paginação

**Métodos Principais**:
- `ListForTerritoryAsync`: Lista posts de um território (delega para `PostFilterService`)
- `ListForTerritoryPagedAsync`: Lista posts paginados (delega para `PostFilterService`)
- `CreatePostAsync`: Cria um novo post (delega para `PostCreationService`)
- `LikeAsync`: Adiciona like a um post (delega para `PostInteractionService`)
- `CommentAsync`: Adiciona comentário a um post (delega para `PostInteractionService`)
- `ShareAsync`: Compartilha um post (delega para `PostInteractionService`)

### PostCreationService
**Responsabilidade**: Criar posts com todas as validações e lógica necessária.

**Dependências** (10):
- `IFeedRepository`: Persistência de posts
- `IMapRepository`: Validação de entidades do mapa
- `IAssetRepository`: Validação de assets
- `IPostGeoAnchorRepository`: Persistência de geo anchors
- `IPostAssetRepository`: Persistência de associações post-asset
- `ISanctionRepository`: Verificação de sanções
- `IFeatureFlagService`: Verificação de feature flags
- `IAuditLogger`: Logging de auditoria
- `IEventBus`: Publicação de eventos
- `IUnitOfWork`: Gerenciamento de transações

**Métodos Principais**:
- `CreatePostAsync`: Cria um post com validações completas

**Validações Realizadas**:
- Título e conteúdo obrigatórios
- Feature flags (ex: alert posts)
- Validação de map entity (se fornecida)
- Verificação de sanções de posting
- Validação de assets (se fornecidos)
- Criação de geo anchors
- Publicação de eventos de domínio

### PostInteractionService
**Responsabilidade**: Gerenciar interações com posts (likes, comentários, shares).

**Dependências** (5):
- `IFeedRepository`: Acesso aos posts
- `AccessEvaluator`: Verificação de permissões
- `ISanctionRepository`: Verificação de sanções de interação
- `IAuditLogger`: Logging de auditoria
- `IUnitOfWork`: Gerenciamento de transações

**Métodos Principais**:
- `LikeAsync`: Adiciona like a um post
- `CommentAsync`: Adiciona comentário a um post
- `ShareAsync`: Compartilha um post

**Validações Realizadas**:
- Post existe e pertence ao território
- Visibilidade do post (residents-only requer resident)
- Verificação de sanções de interação
- Permissões de usuário (apenas residents podem comentar/compartilhar)

### PostFilterService
**Responsabilidade**: Filtrar e paginar posts baseado em regras de visibilidade e acesso.

**Dependências** (3):
- `AccessEvaluator`: Verificação de permissões
- `IUserBlockRepository`: Verificação de bloqueios
- `IPostAssetRepository`: Filtragem por asset

**Métodos Principais**:
- `FilterPostsAsync`: Filtra posts baseado em visibilidade, bloqueios e filtros
- `FilterAndPaginateAsync`: Filtra e pagina posts

**Filtros Aplicados**:
- Bloqueios de usuários
- Visibilidade (Public vs ResidentsOnly)
- Status do post (Published, Hidden, Rejected)
- Filtro por map entity
- Filtro por asset
- Paginação (quando aplicável)

## Padrões de Resultado

### Result<T>
Para operações que podem falhar, o padrão `Result<T>` está disponível em `Araponga.Application/Common/Result.cs`:
```csharp
public sealed class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }
}
```

**Status**: Estrutura criada, migração gradual em andamento.

### PagedResult<T>
Para operações de listagem com paginação:
```csharp
public sealed class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public int TotalPages { get; }
    public bool HasPreviousPage { get; }
    public bool HasNextPage { get; }
}
```

**Status**: Implementado em `FeedService.ListForTerritoryPagedAsync`.

## Configuração de DI

A configuração de Dependency Injection está centralizada em `ServiceCollectionExtensions`:

```csharp
public static IServiceCollection AddApplicationServices(this IServiceCollection services)
{
    // Core services
    services.AddScoped<AccessEvaluator>();
    services.AddScoped<CurrentUserAccessor>();

    // Feed services (refactored)
    services.AddScoped<PostCreationService>();
    services.AddScoped<PostInteractionService>();
    services.AddScoped<PostFilterService>();
    services.AddScoped<FeedService>();

    // Other services...
}
```

## Testes

Para facilitar a criação de `FeedService` nos testes, foi criado `FeedServiceTestHelper`:

```csharp
var dataStore = new InMemoryDataStore();
var service = FeedServiceTestHelper.CreateFeedService(dataStore, eventBus);
```

O helper cria automaticamente todas as dependências necessárias, incluindo os services especializados.

## Evolução Futura

### Migração para Result<T>
Os services ainda retornam tuplas `(bool success, string? error, T? result)`. A migração para `Result<T>` está planejada para padronizar o tratamento de erros.

### Cache
Services que acessam dados frequentemente consultados (ex: feature flags, territórios) podem se beneficiar de cache. Implementação futura planejada.

## Services administrativos (P0)

Além do feed, existem services de suporte a **governança**, **filas** e **configurações calibráveis**:

### SystemConfigService / SystemConfigCacheService
- **Responsabilidade**: gerenciar configurações globais (`SystemConfig`) com cache e auditoria.
- **Uso**: calibrar comportamento do sistema (providers, segurança, moderação, validação).

### WorkQueueService
- **Responsabilidade**: enfileirar/listar/completar `WorkItem` (Work Queue genérica).
- **Uso**: padronizar revisões humanas (verificação, curadoria, moderação).

### VerificationQueueService
- **Responsabilidade**: orquestrar fluxos de verificação de identidade (global) e residência (territorial) usando WorkItems.
- **Observação**: nesta fase, sem OCR/IA; submissão segue direto para revisão humana.

### DocumentEvidenceService
- **Responsabilidade**: criar `DocumentEvidence` e persistir conteúdo em storage via `IFileStorage`.
- **Uso**: uploads de documentos e downloads por proxy com autorização/auditoria.

### ModerationCaseService
- **Responsabilidade**: aplicar decisões humanas em casos de moderação (`WorkItemType.ModerationCase`) e atualizar o estado do report/sanção.

## Services de Chat (P0/P1)

### ChatService
**Responsabilidade**: orquestrar operações de chat com governança territorial e performance:
- listar/criar (lazy) os **canais padrão** do território (público e moradores)
- criar grupos como `PendingApproval` e permitir **aprovação por Curator**
- permitir **ações de moderação** (lock/disable) em grupos por `Moderator`
- enviar/listar mensagens com paginação cursor-based
- manter `ChatConversationStats` para evitar agregações pesadas em hot paths

**Dependências** (8):
- `IChatConversationRepository`: persistência de conversas (canais/grupos/DM)
- `IChatConversationParticipantRepository`: participantes (grupos/DM) e estado de leitura/mute
- `IChatMessageRepository`: persistência e listagem cursor-based de mensagens
- `IChatConversationStatsRepository`: read model (última mensagem/preview/contagem)
- `IUserRepository`: checar `UserIdentityVerificationStatus`
- `FeatureFlagCacheService`: gates por território (ChatEnabled, canais, grupos, DM, mídia)
- `AccessEvaluator`: gates de membership/capability (`Resident` validado, `Curator`, `Moderator`, `SystemAdmin`)
- `IUnitOfWork`: commit transacional

**Regras principais (MVP)**:
- **Canais**: leitura exige membership; escrita exige usuário verificado e, no canal público, morador validado.
- **Grupos**: visitante não cria; criação exige morador validado + usuário verificado; aprovação por curadoria; moderação pode trancar/desabilitar.

**Performance**:
- mensagens paginadas por `(ConversationId, CreatedAtUtc, Id)` com cursor
- stats atualizados em escrita para reduzir custo de listagens

### Otimizações de Query
O `PostFilterService` atualmente carrega todos os posts antes de filtrar. Para grandes volumes, a filtragem deve ser feita no nível do repositório.

## Referências

- [Revisão de Código](./21_CODE_REVIEW.md)
- [Decisões Arquiteturais](./10_ARCHITECTURE_DECISIONS.md) - ADR-009
- [Plano de Implementação](./20_IMPLEMENTATION_PLAN.md)
