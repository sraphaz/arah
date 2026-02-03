# FASE 49: Conexões e Círculo de Amigos

**Versão**: 1.1  
**Data**: 2026-01-28  
**Status**: 🚧 MVP implementado (2026-02-02) — solicitações, aceitar/rejeitar/remover, listagem, integração Feed, feature flag por território, busca, sugestões, privacidade. Notificações in-app para solicitação recebida e conexão aceita. Testes de integração: fluxo de notificação (Application) e API (ConnectionsController + Outbox).  
**Prioridade**: Alta  
**Duração Estimada**: 21 dias  
**Dependências**: Fase 3 (Feed), Fase 11 (Notificações)

---

## 📋 Índice

1. [Visão Geral](#visão-geral)
2. [Objetivos](#objetivos)
3. [Arquitetura Técnica](#arquitetura-técnica)
4. [Modelo de Domínio](#modelo-de-domínio)
5. [APIs e Endpoints](#apis-e-endpoints)
6. [Integração com Feed](#integração-com-feed)
7. [Tarefas Detalhadas](#tarefas-detalhadas)
8. [Testes](#testes)
9. [Documentação](#documentação)

---

## 🎯 Visão Geral

Implementar módulo completo de conexões e círculo de amigos, permitindo que moradores e visitantes se conectem mutuamente e priorizem conteúdo de conexões no feed.

### Funcionalidades Principais

1. **Gerenciamento de Conexões**
   - Enviar solicitação de conexão
   - Aceitar/rejeitar solicitação
   - Remover conexão
   - Listar conexões

2. **Privacidade e Configurações**
   - Configurar quem pode me adicionar
   - Configurar visibilidade de conexões
   - Integração com sistema de bloqueio

3. **Integração com Feed**
   - Priorizar posts de conexões no feed
   - Parâmetro opcional para habilitar/desabilitar priorização

4. **Busca e Descoberta**
   - Buscar usuários
   - Sugestões de conexão

---

## 🎯 Objetivos

### Objetivos Funcionais

- ✅ Usuários podem enviar solicitações de conexão
- ✅ Usuários podem aceitar/rejeitar solicitações
- ✅ Usuários podem remover conexões
- ✅ Feed prioriza conteúdo de conexões estabelecidas
- ✅ Configurações de privacidade para conexões
- ✅ Busca e descoberta de usuários

### Objetivos Técnicos

- ✅ Módulo seguindo padrão IModule
- ✅ Clean Architecture (Domain, Application, Infrastructure, Api)
- ✅ Cobertura de testes >90%
- ✅ Performance otimizada (cache de conexões)
- ✅ Integração com feed existente sem breaking changes

---

## 🏗️ Arquitetura Técnica

### Estrutura de Módulo

```
Araponga.Modules.Connections/
├── Domain/
│   ├── UserConnection.cs
│   ├── ConnectionStatus.cs
│   ├── ConnectionPrivacySettings.cs
│   ├── ConnectionRequestPolicy.cs
│   ├── ConnectionVisibility.cs
│   └── Interfaces/
│       └── IUserConnectionRepository.cs
│
├── Application/
│   ├── Services/
│   │   ├── ConnectionService.cs
│   │   ├── ConnectionPrivacyService.cs
│   │   └── ConnectionSuggestionService.cs
│   ├── DTOs/
│   │   ├── ConnectionRequestDto.cs
│   │   ├── ConnectionResponseDto.cs
│   │   ├── ConnectionListResponseDto.cs
│   │   └── ConnectionPrivacySettingsDto.cs
│   └── Interfaces/
│       ├── IConnectionService.cs
│       └── IConnectionPrivacyService.cs
│
├── Infrastructure/
│   ├── Postgres/
│   │   ├── PostgresUserConnectionRepository.cs
│   │   ├── UserConnectionRecord.cs
│   │   └── ConnectionPrivacySettingsRecord.cs
│   └── ConnectionsModule.cs (implementa IModule)
│
└── Api/
    └── Controllers/
        └── ConnectionsController.cs
```

### Dependências

- **Domain**: Nenhuma (entidades puras)
- **Application**: Domain, Application.Interfaces
- **Infrastructure**: Domain, Application.Interfaces, Infrastructure.Postgres
- **Api**: Application, Infrastructure

### Integração com Módulos Existentes

- **Feed**: Modifica `PostFilterService` para priorizar conexões
- **Notificações**: Usa sistema de notificações para solicitações
- **Users**: Usa entidade `User` existente
- **Moderation**: Integra com sistema de bloqueio

---

## 📐 Modelo de Domínio

### 1. UserConnection

```csharp
namespace Araponga.Domain.Connections;

public sealed class UserConnection
{
    public Guid Id { get; }
    public Guid RequesterUserId { get; }
    public Guid TargetUserId { get; }
    public ConnectionStatus Status { get; private set; }
    public Guid? TerritoryId { get; }
    public DateTime RequestedAtUtc { get; }
    public DateTime? RespondedAtUtc { get; private set; }
    public DateTime? RemovedAtUtc { get; private set; }
    public DateTime CreatedAtUtc { get; }
    public DateTime UpdatedAtUtc { get; private set; }

    private UserConnection(
        Guid id,
        Guid requesterUserId,
        Guid targetUserId,
        ConnectionStatus status,
        Guid? territoryId,
        DateTime requestedAtUtc,
        DateTime? respondedAtUtc,
        DateTime? removedAtUtc,
        DateTime createdAtUtc,
        DateTime updatedAtUtc)
    {
        if (requesterUserId == targetUserId)
            throw new ArgumentException("Requester and target cannot be the same user.", nameof(targetUserId));

        Id = id;
        RequesterUserId = requesterUserId;
        TargetUserId = targetUserId;
        Status = status;
        TerritoryId = territoryId;
        RequestedAtUtc = requestedAtUtc;
        RespondedAtUtc = respondedAtUtc;
        RemovedAtUtc = removedAtUtc;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
    }

    public static UserConnection CreatePending(
        Guid id,
        Guid requesterUserId,
        Guid targetUserId,
        Guid? territoryId,
        DateTime requestedAtUtc)
    {
        return new UserConnection(
            id,
            requesterUserId,
            targetUserId,
            ConnectionStatus.Pending,
            territoryId,
            requestedAtUtc,
            respondedAtUtc: null,
            removedAtUtc: null,
            createdAtUtc: requestedAtUtc,
            updatedAtUtc: requestedAtUtc);
    }

    public void Accept(DateTime respondedAtUtc)
    {
        if (Status != ConnectionStatus.Pending)
            throw new InvalidOperationException("Only pending connections can be accepted.");

        Status = ConnectionStatus.Accepted;
        RespondedAtUtc = respondedAtUtc;
        UpdatedAtUtc = respondedAtUtc;
    }

    public void Reject(DateTime respondedAtUtc)
    {
        if (Status != ConnectionStatus.Pending)
            throw new InvalidOperationException("Only pending connections can be rejected.");

        Status = ConnectionStatus.Rejected;
        RespondedAtUtc = respondedAtUtc;
        UpdatedAtUtc = respondedAtUtc;
    }

    public void Remove(DateTime removedAtUtc)
    {
        if (Status != ConnectionStatus.Accepted)
            throw new InvalidOperationException("Only accepted connections can be removed.");

        Status = ConnectionStatus.Removed;
        RemovedAtUtc = removedAtUtc;
        UpdatedAtUtc = removedAtUtc;
    }
}
```

### 2. ConnectionStatus

```csharp
namespace Araponga.Domain.Connections;

public enum ConnectionStatus
{
    Pending,    // Solicitação enviada, aguardando resposta
    Accepted,   // Conexão aceita, relação mútua estabelecida
    Rejected,   // Solicitação rejeitada
    Removed     // Conexão removida por uma das partes
}
```

### 3. ConnectionPrivacySettings

```csharp
namespace Araponga.Domain.Connections;

public sealed class ConnectionPrivacySettings
{
    public Guid UserId { get; }
    public ConnectionRequestPolicy WhoCanAddMe { get; private set; }
    public ConnectionVisibility WhoCanSeeMyConnections { get; private set; }
    public bool ShowConnectionsInProfile { get; private set; }
    public DateTime CreatedAtUtc { get; }
    public DateTime UpdatedAtUtc { get; private set; }

    private ConnectionPrivacySettings(
        Guid userId,
        ConnectionRequestPolicy whoCanAddMe,
        ConnectionVisibility whoCanSeeMyConnections,
        bool showConnectionsInProfile,
        DateTime createdAtUtc,
        DateTime updatedAtUtc)
    {
        UserId = userId;
        WhoCanAddMe = whoCanAddMe;
        WhoCanSeeMyConnections = whoCanSeeMyConnections;
        ShowConnectionsInProfile = showConnectionsInProfile;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
    }

    public static ConnectionPrivacySettings CreateDefault(Guid userId, DateTime createdAtUtc)
    {
        return new ConnectionPrivacySettings(
            userId,
            ConnectionRequestPolicy.Anyone,
            ConnectionVisibility.MyConnections,
            showConnectionsInProfile: true,
            createdAtUtc,
            updatedAtUtc: createdAtUtc);
    }

    public void Update(
        ConnectionRequestPolicy? whoCanAddMe = null,
        ConnectionVisibility? whoCanSeeMyConnections = null,
        bool? showConnectionsInProfile = null,
        DateTime? updatedAtUtc = null)
    {
        if (whoCanAddMe.HasValue)
            WhoCanAddMe = whoCanAddMe.Value;

        if (whoCanSeeMyConnections.HasValue)
            WhoCanSeeMyConnections = whoCanSeeMyConnections.Value;

        if (showConnectionsInProfile.HasValue)
            ShowConnectionsInProfile = showConnectionsInProfile.Value;

        UpdatedAtUtc = updatedAtUtc ?? DateTime.UtcNow;
    }
}
```

### 4. Enums

```csharp
namespace Araponga.Domain.Connections;

public enum ConnectionRequestPolicy
{
    Anyone,              // Qualquer pessoa pode me adicionar
    ResidentsOnly,       // Apenas moradores podem me adicionar
    ConnectionsOnly,     // Apenas pessoas que eu já adicionei
    Disabled            // Ninguém pode me adicionar
}

public enum ConnectionVisibility
{
    OnlyMe,             // Apenas eu
    MyConnections,      // Minhas conexões
    TerritoryMembers,   // Todos no território
    Everyone           // Todos
}
```

### 5. Repository Interface

```csharp
namespace Araponga.Domain.Connections;

public interface IUserConnectionRepository
{
    Task<UserConnection?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<UserConnection?> GetByUsersAsync(Guid userId1, Guid userId2, CancellationToken cancellationToken);
    Task<IReadOnlyList<UserConnection>> GetPendingRequestsAsync(Guid userId, CancellationToken cancellationToken);
    Task<IReadOnlyList<UserConnection>> GetAcceptedConnectionsAsync(Guid userId, CancellationToken cancellationToken);
    Task<IReadOnlyList<UserConnection>> GetConnectionsAsync(Guid userId, ConnectionStatus? status, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(Guid userId1, Guid userId2, CancellationToken cancellationToken);
    Task<UserConnection> AddAsync(UserConnection connection, CancellationToken cancellationToken);
    Task UpdateAsync(UserConnection connection, CancellationToken cancellationToken);
    Task<int> GetConnectionCountAsync(Guid userId, ConnectionStatus status, CancellationToken cancellationToken);
}
```

---

## 🌐 APIs e Endpoints

### ConnectionsController

#### 1. Enviar Solicitação de Conexão

```http
POST /api/v1/connections/request
Authorization: Bearer {token}
Content-Type: application/json

{
  "targetUserId": "guid",
  "territoryId": "guid" // opcional
}
```

**Respostas**:
- `201 Created`: Solicitação criada
- `400 Bad Request`: Validação falhou
- `403 Forbidden`: Usuário não pode adicionar (política de privacidade)
- `409 Conflict`: Conexão já existe
- `429 Too Many Requests`: Limite de solicitações excedido

#### 2. Aceitar Solicitação

```http
POST /api/v1/connections/{connectionId}/accept
Authorization: Bearer {token}
```

**Respostas**:
- `200 OK`: Conexão aceita
- `404 Not Found`: Conexão não encontrada
- `403 Forbidden`: Não é o destinatário da solicitação
- `400 Bad Request`: Conexão não está pendente

#### 3. Rejeitar Solicitação

```http
POST /api/v1/connections/{connectionId}/reject
Authorization: Bearer {token}
```

**Respostas**:
- `200 OK`: Solicitação rejeitada
- `404 Not Found`: Conexão não encontrada
- `403 Forbidden`: Não é o destinatário da solicitação
- `400 Bad Request`: Conexão não está pendente

#### 4. Remover Conexão

```http
DELETE /api/v1/connections/{connectionId}
Authorization: Bearer {token}
```

**Respostas**:
- `200 OK`: Conexão removida
- `404 Not Found`: Conexão não encontrada
- `403 Forbidden`: Não é parte da conexão
- `400 Bad Request`: Conexão não está aceita

#### 5. Listar Conexões

```http
GET /api/v1/connections?status={status}&territoryId={territoryId}&skip={skip}&take={take}
Authorization: Bearer {token}
```

**Query Parameters**:
- `status`: `Pending`, `Accepted`, `Rejected`, `Removed` (opcional)
- `territoryId`: Filtrar por território (opcional)
- `skip`, `take`: Paginação

**Respostas**:
- `200 OK`: Lista de conexões

#### 6. Listar Solicitações Pendentes

```http
GET /api/v1/connections/pending
Authorization: Bearer {token}
```

**Respostas**:
- `200 OK`: Lista de solicitações pendentes recebidas

#### 7. Buscar Usuários

```http
GET /api/v1/connections/users/search?query={query}&territoryId={territoryId}&role={role}
Authorization: Bearer {token}
```

**Query Parameters**:
- `query`: Nome de exibição (opcional)
- `territoryId`: Filtrar por território (opcional)
- `role`: `Resident`, `Visitor` (opcional)

**Respostas**:
- `200 OK`: Lista de usuários

#### 8. Sugestões de Conexão

```http
GET /api/v1/connections/suggestions?territoryId={territoryId}&limit={limit}
Authorization: Bearer {token}
```

**Respostas**:
- `200 OK`: Lista de sugestões

#### 9. Obter Configurações de Privacidade

```http
GET /api/v1/connections/privacy
Authorization: Bearer {token}
```

**Respostas**:
- `200 OK`: Configurações de privacidade

#### 10. Atualizar Configurações de Privacidade

```http
PUT /api/v1/connections/privacy
Authorization: Bearer {token}
Content-Type: application/json

{
  "whoCanAddMe": "Anyone" | "ResidentsOnly" | "ConnectionsOnly" | "Disabled",
  "whoCanSeeMyConnections": "OnlyMe" | "MyConnections" | "TerritoryMembers" | "Everyone",
  "showConnectionsInProfile": true
}
```

**Respostas**:
- `200 OK`: Configurações atualizadas
- `400 Bad Request`: Validação falhou

---

## 🔗 Integração com Feed

### Modificação no PostFilterService

Adicionar método para priorizar por conexões:

```csharp
public async Task<IReadOnlyList<CommunityPost>> FilterAndPrioritizeByConnectionsAsync(
    IReadOnlyList<CommunityPost> posts,
    Guid territoryId,
    Guid? userId,
    bool prioritizeConnections,
    CancellationToken cancellationToken)
{
    // 1. Aplicar filtros existentes
    var filtered = await FilterPostsAsync(posts, territoryId, userId, null, null, cancellationToken);

    // 2. Se não priorizar ou usuário não autenticado, retornar filtrado
    if (!prioritizeConnections || userId is null)
    {
        return filtered.OrderByDescending(p => p.CreatedAtUtc).ToList();
    }

    // 3. Buscar conexões aceitas (com cache)
    var connections = await _connectionCacheService.GetAcceptedConnectionsAsync(
        userId.Value, 
        cancellationToken);
    
    var connectionUserIds = connections
        .Select(c => c.RequesterUserId == userId.Value ? c.TargetUserId : c.RequesterUserId)
        .ToHashSet();

    // 4. Separar posts
    var postsFromConnections = filtered
        .Where(p => connectionUserIds.Contains(p.AuthorUserId))
        .OrderByDescending(p => p.CreatedAtUtc)
        .ToList();

    var postsFromOthers = filtered
        .Where(p => !connectionUserIds.Contains(p.AuthorUserId))
        .OrderByDescending(p => p.CreatedAtUtc)
        .ToList();

    // 5. Combinar: conexões primeiro
    return postsFromConnections.Concat(postsFromOthers).ToList();
}
```

### Modificação no FeedController

Adicionar parâmetro `prioritizeConnections`:

```csharp
[HttpGet]
public async Task<ActionResult<IEnumerable<FeedItemResponse>>> GetFeed(
    [FromQuery] Guid? territoryId,
    [FromQuery] Guid? mapEntityId,
    [FromQuery] Guid? assetId,
    [FromQuery] bool filterByInterests = false,
    [FromQuery] bool prioritizeConnections = true,  // NOVO
    CancellationToken cancellationToken)
{
    // ...
    var posts = await _feedService.ListForTerritoryPagedAsync(
        resolvedTerritoryId.Value,
        userContext.UserId,
        mapEntityId,
        assetId,
        filterByInterests,
        prioritizeConnections,  // NOVO
        pagination,
        cancellationToken);
    // ...
}
```

### Cache de Conexões

Criar serviço de cache para conexões:

```csharp
public class ConnectionCacheService
{
    private readonly IUserConnectionRepository _repository;
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5);

    public async Task<IReadOnlyList<UserConnection>> GetAcceptedConnectionsAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"connections:accepted:{userId}";
        
        if (_cache.TryGetValue(cacheKey, out IReadOnlyList<UserConnection>? cached))
        {
            return cached ?? Array.Empty<UserConnection>();
        }

        var connections = await _repository.GetAcceptedConnectionsAsync(userId, cancellationToken);
        
        _cache.Set(cacheKey, connections, _cacheExpiration);
        
        return connections;
    }

    public void InvalidateCache(Guid userId)
    {
        var cacheKey = $"connections:accepted:{userId}";
        _cache.Remove(cacheKey);
    }
}
```

---

## 📋 Tarefas Detalhadas

### Semana 1: Modelo de Domínio e Repositórios (5 dias)

#### Dia 1-2: Domain Layer
- [ ] Criar `UserConnection` domain model
- [ ] Criar `ConnectionStatus` enum
- [ ] Criar `ConnectionPrivacySettings` domain model
- [ ] Criar `ConnectionRequestPolicy` enum
- [ ] Criar `ConnectionVisibility` enum
- [ ] Criar `IUserConnectionRepository` interface
- [ ] Criar `IConnectionPrivacySettingsRepository` interface
- [ ] Testes unitários de domain models

#### Dia 3-4: Infrastructure Layer
- [ ] Criar `UserConnectionRecord` (EF Core entity)
- [ ] Criar `ConnectionPrivacySettingsRecord` (EF Core entity)
- [ ] Criar `PostgresUserConnectionRepository`
- [ ] Criar `PostgresConnectionPrivacySettingsRepository`
- [ ] Adicionar DbSets ao `ArapongaDbContext`
- [ ] Criar migration
- [ ] Testes de repositório

#### Dia 5: Módulo e Registro
- [ ] Criar `ConnectionsModule` (implementa `IModule`)
- [ ] Registrar serviços no módulo
- [ ] Registrar módulo no `ServiceCollectionExtensions`
- [ ] Testes de integração do módulo

### Semana 2: Application Layer e Services (5 dias)

#### Dia 6-7: ConnectionService
- [ ] Criar `IConnectionService` interface
- [ ] Implementar `ConnectionService`
  - [ ] `RequestConnectionAsync`
  - [ ] `AcceptConnectionAsync`
  - [ ] `RejectConnectionAsync`
  - [ ] `RemoveConnectionAsync`
  - [ ] `GetConnectionsAsync`
  - [ ] `GetPendingRequestsAsync`
- [ ] Validações de negócio
- [ ] Testes unitários

#### Dia 8: ConnectionPrivacyService
- [ ] Criar `IConnectionPrivacyService` interface
- [ ] Implementar `ConnectionPrivacyService`
  - [ ] `GetPrivacySettingsAsync`
  - [ ] `UpdatePrivacySettingsAsync`
  - [ ] `CanUserAddAsync` (verifica política)
- [ ] Testes unitários

#### Dia 9: ConnectionSuggestionService
- [ ] Criar `IConnectionSuggestionService` interface
- [ ] Implementar `ConnectionSuggestionService`
  - [ ] `GetSuggestionsAsync` (algoritmo de sugestão)
- [ ] Testes unitários

#### Dia 10: DTOs e Mappers
- [ ] Criar DTOs (Request, Response, etc.)
- [ ] Criar mappers (Domain → DTO)
- [ ] Testes de mappers

### Semana 3: API e Integração (5 dias)

#### Dia 11-12: ConnectionsController
- [ ] Criar `ConnectionsController`
- [ ] Implementar endpoints:
  - [ ] POST `/api/v1/connections/request`
  - [ ] POST `/api/v1/connections/{id}/accept`
  - [ ] POST `/api/v1/connections/{id}/reject`
  - [ ] DELETE `/api/v1/connections/{id}`
  - [ ] GET `/api/v1/connections`
  - [ ] GET `/api/v1/connections/pending`
  - [ ] GET `/api/v1/connections/users/search`
  - [ ] GET `/api/v1/connections/suggestions`
  - [ ] GET `/api/v1/connections/privacy`
  - [ ] PUT `/api/v1/connections/privacy`
- [ ] Validação de entrada (FluentValidation)
- [ ] Rate limiting
- [ ] Testes de controller (E2E)

#### Dia 13: Integração com Feed
- [ ] Modificar `PostFilterService` para priorizar conexões
- [ ] Criar `ConnectionCacheService`
- [ ] Adicionar parâmetro `prioritizeConnections` ao `FeedController`
- [ ] Invalidar cache quando conexão é criada/removida
- [ ] Testes de integração

#### Dia 14: Integração com Notificações
- [ ] Notificar quando solicitação é recebida
- [ ] Notificar quando solicitação é aceita
- [ ] Integrar com `NotificationService` existente
- [ ] Testes de notificações

#### Dia 15: Documentação e Testes Finais
- [ ] Atualizar documentação de API
- [ ] Atualizar DevPortal
- [ ] Testes E2E completos
- [ ] Validação de performance
- [ ] Code review

### Semana 4: Testes e Validação (6 dias)

#### Dia 16-18: Testes Completos
- [ ] Testes unitários (cobertura >90%)
- [ ] Testes de integração
- [ ] Testes E2E
- [ ] Testes de performance
- [ ] Testes de segurança

#### Dia 19-20: Validação e Ajustes
- [ ] Validação de requisitos
- [ ] Ajustes baseados em testes
- [ ] Otimizações de performance
- [ ] Correção de bugs

#### Dia 21: Entrega
- [ ] Documentação final
- [ ] Changelog
- [ ] PR review
- [ ] Merge

---

## 🧪 Testes

### Testes Unitários

#### Domain Models
- [ ] `UserConnection.CreatePending` - validações
- [ ] `UserConnection.Accept` - transições de estado
- [ ] `UserConnection.Reject` - transições de estado
- [ ] `UserConnection.Remove` - transições de estado
- [ ] `ConnectionPrivacySettings.CreateDefault` - valores padrão
- [ ] `ConnectionPrivacySettings.Update` - atualizações

#### Services
- [ ] `ConnectionService.RequestConnectionAsync` - casos de sucesso e erro
- [ ] `ConnectionService.AcceptConnectionAsync` - validações
- [ ] `ConnectionService.RejectConnectionAsync` - validações
- [ ] `ConnectionService.RemoveConnectionAsync` - validações
- [ ] `ConnectionPrivacyService.CanUserAddAsync` - todas as políticas
- [ ] `ConnectionSuggestionService.GetSuggestionsAsync` - algoritmo

### Testes de Integração

- [ ] Repositório com banco de dados real
- [ ] Cache de conexões
- [ ] Integração com notificações
- [ ] Integração com feed

### Testes E2E

- [ ] Fluxo completo: solicitar → aceitar → ver no feed
- [ ] Fluxo: solicitar → rejeitar → não pode solicitar novamente (30 dias)
- [ ] Fluxo: remover conexão → não aparece mais no feed priorizado
- [ ] Configurações de privacidade
- [ ] Busca e sugestões

### Testes de Performance

- [ ] Cache de conexões (hit rate)
- [ ] Query de conexões (índices)
- [ ] Feed com priorização (tempo de resposta)

### Testes de Segurança

- [ ] Autorização (não pode aceitar conexão de outro usuário)
- [ ] Rate limiting (limite de solicitações)
- [ ] Validação de entrada
- [ ] Proteção contra SQL injection

---

## 📚 Documentação

### Documentação Técnica

- [ ] Atualizar `docs/12_DOMAIN_MODEL.md` com entidades de conexões
- [ ] Atualizar `docs/60_API_LÓGICA_NEGÓCIO.md` com endpoints
- [ ] Criar `docs/api/60_XX_API_CONEXOES.md` (documentação completa da API)
- [ ] Atualizar `docs/11_ARCHITECTURE_SERVICES.md` com novos services

### Documentação Funcional

- [ ] Documento funcional já criado: `docs/funcional/23_CONEXOES_CIRCULO_AMIGOS.md`
- [ ] Atualizar `docs/funcional/03_FEED_COMUNITARIO.md` com priorização
- [ ] Atualizar `docs/funcional/11_NOTIFICACOES.md` com notificações de conexão

### DevPortal

- [ ] Adicionar endpoints ao DevPortal
- [ ] Exemplos de uso
- [ ] Diagramas de fluxo

### Changelog

- [ ] Atualizar `docs/40_CHANGELOG.md` com nova funcionalidade

---

## 🔒 Segurança e Validação

### Validações

- [ ] Usuário não pode adicionar a si mesmo
- [ ] Verificar política de privacidade antes de criar solicitação
- [ ] Verificar se conexão já existe
- [ ] Verificar se usuário está bloqueado
- [ ] Rate limiting: máximo 50 solicitações por dia
- [ ] Cooldown: 30 dias após rejeição

### Autorização

- [ ] Apenas destinatário pode aceitar/rejeitar
- [ ] Apenas partes da conexão podem remover
- [ ] Apenas próprio usuário pode ver/editar configurações de privacidade

### Auditoria

- [ ] Log de todas as ações (criar, aceitar, rejeitar, remover)
- [ ] Rastreabilidade completa

---

## 📊 Métricas de Sucesso

### Funcionais

- ✅ Usuários podem criar conexões
- ✅ Feed prioriza conteúdo de conexões
- ✅ Configurações de privacidade funcionam
- ✅ Notificações são enviadas corretamente

### Técnicas

- ✅ Cobertura de testes >90%
- ✅ Performance: feed com priorização <500ms
- ✅ Cache hit rate >80%
- ✅ Zero breaking changes no feed existente

---

## 🚀 Próximos Passos (Futuro)

### Modularização física (opcional)

Quando a base de código adotar a migração por módulos físicos (projeto `Araponga.Modules.*.Infrastructure`), o módulo **Connections** pode ser migrado conforme `docs/PLANO_MIGRACAO_MODULOS.md`:

- **Entidades a mover**: `UserConnectionRecord`, `ConnectionPrivacySettingsRecord`
- **Repositórios a mover**: `PostgresUserConnectionRepository`, `PostgresConnectionPrivacySettingsRepository`
- **Manter** em Application/Api: `ConnectionService`, `ConnectionPrivacyService`, `ConnectionsController`, eventos e handlers de notificação (até eventual migração de Application por módulo)
- **Referência**: Ver também `docs/TECNICO_MODULARIZACAO.md` (módulo 16 — Conexões) e tabela de dependências (Connections → Auth, Memberships, Notifications)

### Fase 2: Melhorias

- [ ] Algoritmo de sugestão mais sofisticado (machine learning)
- [ ] Grupos de conexões (círculos)
- [ ] Compartilhar conexões
- [ ] Exportar/importar conexões

### Fase 3: Analytics

- [ ] Dashboard de métricas de conexões
- [ ] Relatórios de engajamento
- [ ] A/B testing de algoritmos de priorização

---

**Última Atualização**: 2026-01-28  
**Versão**: 1.0  
**Status**: 📋 Planejamento
