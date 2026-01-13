# Planejamento: Preferências de Usuário e Privacidade

**Versão**: 1.0  
**Data**: 2025-01-13  
**Status**: 📋 Planejamento

---

## 🎯 Objetivo

Implementar uma funcionalidade completa para que usuários possam configurar suas preferências de privacidade e outras configurações pessoais que atualmente estão "soltas" no sistema.

---

## 📋 Análise do Estado Atual

### O que existe hoje

1. **Modelo User básico** (`Araponga.Domain.Users.User`):
   - Campos: `DisplayName`, `Email`, `Cpf`, `ForeignDocument`, `PhoneNumber`, `Address`, `Provider`, `ExternalId`, `Role`, `CreatedAtUtc`
   - Não há campos para preferências ou configurações

2. **MER conceitual** (`design/Archtecture/MER.md`):
   - Define `USER_SECURITY_SETTINGS` (não implementado)
   - Define `USER_DEVICE` (não implementado)
   - Não define preferências de privacidade ou notificações

3. **Sistema de notificações**:
   - Existe `UserNotification` (inbox)
   - Não há preferências configuráveis por tipo de notificação
   - Notificações são sempre enviadas quando eventos ocorrem

4. **Endpoints existentes**:
   - `POST /api/v1/auth/social` - Login/cadastro
   - `GET /api/v1/notifications` - Listar notificações
   - `POST /api/v1/notifications/{id}/read` - Marcar como lida
   - **Não existe** endpoint para gerenciar perfil ou preferências

### O que está faltando

1. **Preferências de Privacidade**:
   - Visibilidade do perfil (público, apenas moradores, privado)
   - Visibilidade de informações de contato (email, telefone, endereço)
   - Compartilhamento de localização
   - Visibilidade de membroships (territórios onde é morador/visitante)

2. **Preferências de Notificações**:
   - Habilitar/desabilitar por tipo (posts, comentários, eventos, alertas, marketplace, moderação)
   - Preferências de canal (in-app, email, push - futuro)
   - Frequência de notificações (imediato, resumo diário, semanal)

3. **Configurações de Perfil**:
   - Atualizar `DisplayName`
   - Atualizar `Email`, `PhoneNumber`, `Address`
   - Foto de perfil (futuro)
   - Bio/descrição pessoal (futuro)

4. **Configurações de Segurança**:
   - Autenticação de dois fatores (futuro)
   - Sessões ativas e revogação (futuro)
   - Histórico de login (futuro)

---

## 🏗️ Arquitetura Proposta

### 1. Modelo de Domínio

#### 1.1. Entidade `UserPreferences`

```csharp
namespace Araponga.Domain.Users;

public sealed class UserPreferences
{
    public UserPreferences(
        Guid userId,
        ProfileVisibility profileVisibility,
        ContactVisibility contactVisibility,
        bool shareLocation,
        bool showMemberships,
        NotificationPreferences notificationPreferences,
        DateTime createdAtUtc,
        DateTime updatedAtUtc)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User ID is required.", nameof(userId));
        }

        UserId = userId;
        ProfileVisibility = profileVisibility;
        ContactVisibility = contactVisibility;
        ShareLocation = shareLocation;
        ShowMemberships = showMemberships;
        NotificationPreferences = notificationPreferences;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
    }

    public Guid UserId { get; }
    public ProfileVisibility ProfileVisibility { get; private set; }
    public ContactVisibility ContactVisibility { get; private set; }
    public bool ShareLocation { get; private set; }
    public bool ShowMemberships { get; private set; }
    public NotificationPreferences NotificationPreferences { get; private set; }
    public DateTime CreatedAtUtc { get; }
    public DateTime UpdatedAtUtc { get; private set; }

    public void UpdatePrivacy(
        ProfileVisibility profileVisibility,
        ContactVisibility contactVisibility,
        bool shareLocation,
        bool showMemberships,
        DateTime updatedAtUtc)
    {
        ProfileVisibility = profileVisibility;
        ContactVisibility = contactVisibility;
        ShareLocation = shareLocation;
        ShowMemberships = showMemberships;
        UpdatedAtUtc = updatedAtUtc;
    }

    public void UpdateNotificationPreferences(
        NotificationPreferences preferences,
        DateTime updatedAtUtc)
    {
        NotificationPreferences = preferences;
        UpdatedAtUtc = updatedAtUtc;
    }
}
```

#### 1.2. Value Objects

```csharp
namespace Araponga.Domain.Users;

public enum ProfileVisibility
{
    Public,           // Visível para todos
    ResidentsOnly,    // Apenas moradores dos territórios onde o usuário é membro
    Private           // Apenas o próprio usuário
}

public enum ContactVisibility
{
    Public,           // Email, telefone, endereço visíveis para todos
    ResidentsOnly,    // Apenas moradores validados
    Private           // Nunca visível publicamente
}

public sealed record NotificationPreferences
{
    public NotificationPreferences(
        bool postsEnabled,
        bool commentsEnabled,
        bool eventsEnabled,
        bool alertsEnabled,
        bool marketplaceEnabled,
        bool moderationEnabled,
        bool membershipRequestsEnabled)
    {
        PostsEnabled = postsEnabled;
        CommentsEnabled = commentsEnabled;
        EventsEnabled = eventsEnabled;
        AlertsEnabled = alertsEnabled;
        MarketplaceEnabled = marketplaceEnabled;
        ModerationEnabled = moderationEnabled;
        MembershipRequestsEnabled = membershipRequestsEnabled;
    }

    public bool PostsEnabled { get; init; }
    public bool CommentsEnabled { get; init; }
    public bool EventsEnabled { get; init; }
    public bool AlertsEnabled { get; init; }
    public bool MarketplaceEnabled { get; init; }
    public bool ModerationEnabled { get; init; }
    public bool MembershipRequestsEnabled { get; init; }

    public static NotificationPreferences Default() => new(
        postsEnabled: true,
        commentsEnabled: true,
        eventsEnabled: true,
        alertsEnabled: true,
        marketplaceEnabled: true,
        moderationEnabled: true,
        membershipRequestsEnabled: true);
}
```

#### 1.3. Métodos no `User` para atualização de perfil

```csharp
// Adicionar ao Araponga.Domain.Users.User

public void UpdateDisplayName(string displayName, DateTime updatedAtUtc)
{
    if (string.IsNullOrWhiteSpace(displayName))
    {
        throw new ArgumentException("Display name is required.", nameof(displayName));
    }
    // Nota: Como User é imutável, pode ser necessário criar um novo objeto
    // ou adicionar um campo UpdatedAtUtc e métodos de atualização
}

public void UpdateContactInfo(
    string? email,
    string? phoneNumber,
    string? address,
    DateTime updatedAtUtc)
{
    // Validações e atualização
}
```

**Nota**: O modelo `User` atual é imutável (apenas getters). Será necessário decidir entre:
- Adicionar campos `UpdatedAtUtc` e métodos de atualização
- Criar um novo objeto `User` a cada atualização (padrão atual)
- Usar um padrão de "snapshot" com histórico

---

### 2. Estrutura de Repositório

#### 2.1. Interface `IUserPreferencesRepository`

```csharp
namespace Araponga.Application.Interfaces;

public interface IUserPreferencesRepository
{
    Task<UserPreferences?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<UserPreferences> GetOrCreateDefaultAsync(Guid userId, CancellationToken cancellationToken);
    Task AddAsync(UserPreferences preferences, CancellationToken cancellationToken);
    Task UpdateAsync(UserPreferences preferences, CancellationToken cancellationToken);
}
```

#### 2.2. Implementações

- **InMemory**: `InMemoryUserPreferencesRepository` (para testes e desenvolvimento)
- **Postgres**: `PostgresUserPreferencesRepository` (produção)

---

### 3. Serviços de Aplicação

#### 3.1. `UserPreferencesService`

```csharp
namespace Araponga.Application.Services;

public sealed class UserPreferencesService
{
    private readonly IUserPreferencesRepository _preferencesRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public async Task<UserPreferences> GetPreferencesAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var preferences = await _preferencesRepository.GetOrCreateDefaultAsync(
            userId,
            cancellationToken);
        return preferences;
    }

    public async Task<UserPreferences> UpdatePrivacyPreferencesAsync(
        Guid userId,
        ProfileVisibility profileVisibility,
        ContactVisibility contactVisibility,
        bool shareLocation,
        bool showMemberships,
        CancellationToken cancellationToken)
    {
        var preferences = await _preferencesRepository.GetOrCreateDefaultAsync(
            userId,
            cancellationToken);

        preferences.UpdatePrivacy(
            profileVisibility,
            contactVisibility,
            shareLocation,
            showMemberships,
            DateTime.UtcNow);

        await _preferencesRepository.UpdateAsync(preferences, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return preferences;
    }

    public async Task<UserPreferences> UpdateNotificationPreferencesAsync(
        Guid userId,
        NotificationPreferences notificationPreferences,
        CancellationToken cancellationToken)
    {
        var preferences = await _preferencesRepository.GetOrCreateDefaultAsync(
            userId,
            cancellationToken);

        preferences.UpdateNotificationPreferences(
            notificationPreferences,
            DateTime.UtcNow);

        await _preferencesRepository.UpdateAsync(preferences, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return preferences;
    }
}
```

#### 3.2. `UserProfileService`

```csharp
namespace Araponga.Application.Services;

public sealed class UserProfileService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public async Task<User> UpdateDisplayNameAsync(
        Guid userId,
        string displayName,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            throw new NotFoundException($"User {userId} not found.");
        }

        // Criar novo User com displayName atualizado
        var updatedUser = new User(
            user.Id,
            displayName,
            user.Email,
            user.Cpf,
            user.ForeignDocument,
            user.PhoneNumber,
            user.Address,
            user.Provider,
            user.ExternalId,
            user.Role,
            user.CreatedAtUtc);

        await _userRepository.UpdateAsync(updatedUser, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return updatedUser;
    }

    public async Task<User> UpdateContactInfoAsync(
        Guid userId,
        string? email,
        string? phoneNumber,
        string? address,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            throw new NotFoundException($"User {userId} not found.");
        }

        var updatedUser = new User(
            user.Id,
            user.DisplayName,
            email,
            user.Cpf,
            user.ForeignDocument,
            phoneNumber,
            address,
            user.Provider,
            user.ExternalId,
            user.Role,
            user.CreatedAtUtc);

        await _userRepository.UpdateAsync(updatedUser, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return updatedUser;
    }

    public async Task<User> GetProfileAsync(
        Guid userId,
        Guid? viewerUserId,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            throw new NotFoundException($"User {userId} not found.");
        }

        // Aplicar regras de visibilidade baseadas em preferências
        // (implementar lógica de filtragem baseada em UserPreferences)

        return user;
    }
}
```

---

### 4. Controllers da API

#### 4.1. `UserPreferencesController`

```csharp
namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/users/me/preferences")]
[Produces("application/json")]
[Tags("User Preferences")]
public sealed class UserPreferencesController : ControllerBase
{
    private readonly UserPreferencesService _preferencesService;
    private readonly CurrentUserAccessor _currentUserAccessor;

    /// <summary>
    /// Obtém as preferências do usuário autenticado.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(UserPreferencesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserPreferencesResponse>> GetMyPreferences(
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var preferences = await _preferencesService.GetPreferencesAsync(
            userContext.User.Id,
            cancellationToken);

        return Ok(MapToResponse(preferences));
    }

    /// <summary>
    /// Atualiza as preferências de privacidade do usuário autenticado.
    /// </summary>
    [HttpPut("privacy")]
    [ProducesResponseType(typeof(UserPreferencesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserPreferencesResponse>> UpdatePrivacyPreferences(
        [FromBody] UpdatePrivacyPreferencesRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        if (!Enum.TryParse<ProfileVisibility>(request.ProfileVisibility, out var profileVisibility))
        {
            return BadRequest(new { error = "Invalid profileVisibility." });
        }

        if (!Enum.TryParse<ContactVisibility>(request.ContactVisibility, out var contactVisibility))
        {
            return BadRequest(new { error = "Invalid contactVisibility." });
        }

        var preferences = await _preferencesService.UpdatePrivacyPreferencesAsync(
            userContext.User.Id,
            profileVisibility,
            contactVisibility,
            request.ShareLocation,
            request.ShowMemberships,
            cancellationToken);

        return Ok(MapToResponse(preferences));
    }

    /// <summary>
    /// Atualiza as preferências de notificações do usuário autenticado.
    /// </summary>
    [HttpPut("notifications")]
    [ProducesResponseType(typeof(UserPreferencesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserPreferencesResponse>> UpdateNotificationPreferences(
        [FromBody] UpdateNotificationPreferencesRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var notificationPreferences = new NotificationPreferences(
            request.PostsEnabled,
            request.CommentsEnabled,
            request.EventsEnabled,
            request.AlertsEnabled,
            request.MarketplaceEnabled,
            request.ModerationEnabled,
            request.MembershipRequestsEnabled);

        var preferences = await _preferencesService.UpdateNotificationPreferencesAsync(
            userContext.User.Id,
            notificationPreferences,
            cancellationToken);

        return Ok(MapToResponse(preferences));
    }
}
```

#### 4.2. `UserProfileController`

```csharp
namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/users/me/profile")]
[Produces("application/json")]
[Tags("User Profile")]
public sealed class UserProfileController : ControllerBase
{
    private readonly UserProfileService _profileService;
    private readonly CurrentUserAccessor _currentUserAccessor;

    /// <summary>
    /// Obtém o perfil do usuário autenticado.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserProfileResponse>> GetMyProfile(
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var user = await _profileService.GetProfileAsync(
            userContext.User.Id,
            userContext.User.Id, // Próprio usuário vê tudo
            cancellationToken);

        return Ok(MapToResponse(user));
    }

    /// <summary>
    /// Atualiza o nome de exibição do usuário autenticado.
    /// </summary>
    [HttpPut("display-name")]
    [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserProfileResponse>> UpdateDisplayName(
        [FromBody] UpdateDisplayNameRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var user = await _profileService.UpdateDisplayNameAsync(
            userContext.User.Id,
            request.DisplayName,
            cancellationToken);

        return Ok(MapToResponse(user));
    }

    /// <summary>
    /// Atualiza as informações de contato do usuário autenticado.
    /// </summary>
    [HttpPut("contact")]
    [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserProfileResponse>> UpdateContactInfo(
        [FromBody] UpdateContactInfoRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var user = await _profileService.UpdateContactInfoAsync(
            userContext.User.Id,
            request.Email,
            request.PhoneNumber,
            request.Address,
            cancellationToken);

        return Ok(MapToResponse(user));
    }
}
```

---

### 5. Contracts (DTOs)

#### 5.1. Requests

```csharp
namespace Araponga.Api.Contracts.Users;

public sealed record UpdatePrivacyPreferencesRequest(
    string ProfileVisibility,      // "Public", "ResidentsOnly", "Private"
    string ContactVisibility,       // "Public", "ResidentsOnly", "Private"
    bool ShareLocation,
    bool ShowMemberships);

public sealed record UpdateNotificationPreferencesRequest(
    bool PostsEnabled,
    bool CommentsEnabled,
    bool EventsEnabled,
    bool AlertsEnabled,
    bool MarketplaceEnabled,
    bool ModerationEnabled,
    bool MembershipRequestsEnabled);

public sealed record UpdateDisplayNameRequest(string DisplayName);

public sealed record UpdateContactInfoRequest(
    string? Email,
    string? PhoneNumber,
    string? Address);
```

#### 5.2. Responses

```csharp
namespace Araponga.Api.Contracts.Users;

public sealed record UserPreferencesResponse(
    Guid UserId,
    string ProfileVisibility,
    string ContactVisibility,
    bool ShareLocation,
    bool ShowMemberships,
    NotificationPreferencesResponse Notifications,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);

public sealed record NotificationPreferencesResponse(
    bool PostsEnabled,
    bool CommentsEnabled,
    bool EventsEnabled,
    bool AlertsEnabled,
    bool MarketplaceEnabled,
    bool ModerationEnabled,
    bool MembershipRequestsEnabled);

public sealed record UserProfileResponse(
    Guid Id,
    string DisplayName,
    string? Email,
    string? PhoneNumber,
    string? Address,
    DateTime CreatedAtUtc);
```

---

### 6. Migração de Banco de Dados

#### 6.1. Tabela `user_preferences`

```sql
CREATE TABLE user_preferences (
    user_id UUID PRIMARY KEY REFERENCES users(id) ON DELETE CASCADE,
    profile_visibility VARCHAR(20) NOT NULL DEFAULT 'Public',
    contact_visibility VARCHAR(20) NOT NULL DEFAULT 'ResidentsOnly',
    share_location BOOLEAN NOT NULL DEFAULT false,
    show_memberships BOOLEAN NOT NULL DEFAULT true,
    notifications_posts_enabled BOOLEAN NOT NULL DEFAULT true,
    notifications_comments_enabled BOOLEAN NOT NULL DEFAULT true,
    notifications_events_enabled BOOLEAN NOT NULL DEFAULT true,
    notifications_alerts_enabled BOOLEAN NOT NULL DEFAULT true,
    notifications_marketplace_enabled BOOLEAN NOT NULL DEFAULT true,
    notifications_moderation_enabled BOOLEAN NOT NULL DEFAULT true,
    notifications_membership_requests_enabled BOOLEAN NOT NULL DEFAULT true,
    created_at_utc TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at_utc TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE INDEX idx_user_preferences_user_id ON user_preferences(user_id);
```

#### 6.2. Entity Framework Configuration

```csharp
// Em ArapongaDbContext.OnModelCreating

modelBuilder.Entity<UserPreferencesRecord>(entity =>
{
    entity.ToTable("user_preferences");
    entity.HasKey(p => p.UserId);
    entity.Property(p => p.ProfileVisibility).HasMaxLength(20).IsRequired();
    entity.Property(p => p.ContactVisibility).HasMaxLength(20).IsRequired();
    entity.Property(p => p.CreatedAtUtc).HasColumnType("timestamp with time zone");
    entity.Property(p => p.UpdatedAtUtc).HasColumnType("timestamp with time zone");
    entity.HasOne<UserRecord>()
        .WithOne()
        .HasForeignKey<UserPreferencesRecord>(p => p.UserId)
        .OnDelete(DeleteBehavior.Cascade);
});
```

---

### 7. Integração com Sistema de Notificações

#### 7.1. Modificar `NotificationDispatcher`

Antes de enviar notificações, verificar preferências do usuário:

```csharp
namespace Araponga.Application.Services;

public sealed class NotificationDispatcher
{
    private readonly IUserPreferencesRepository _preferencesRepository;
    private readonly INotificationInboxRepository _notificationRepository;

    public async Task DispatchAsync(
        Guid userId,
        string kind,
        string title,
        string? body,
        string? dataJson,
        CancellationToken cancellationToken)
    {
        var preferences = await _preferencesRepository.GetByUserIdAsync(
            userId,
            cancellationToken);

        // Verificar se o tipo de notificação está habilitado
        if (preferences is not null)
        {
            var shouldNotify = kind switch
            {
                "PostCreated" => preferences.NotificationPreferences.PostsEnabled,
                "CommentCreated" => preferences.NotificationPreferences.CommentsEnabled,
                "EventCreated" => preferences.NotificationPreferences.EventsEnabled,
                "AlertCreated" => preferences.NotificationPreferences.AlertsEnabled,
                "MarketplaceInquiry" => preferences.NotificationPreferences.MarketplaceEnabled,
                "ReportCreated" => preferences.NotificationPreferences.ModerationEnabled,
                "MembershipRequest" => preferences.NotificationPreferences.MembershipRequestsEnabled,
                _ => true // Notificações do sistema sempre habilitadas
            };

            if (!shouldNotify)
            {
                return; // Não enviar notificação
            }
        }

        // Continuar com o envio normal...
    }
}
```

---

## 📊 Estrutura de Arquivos

```
backend/
├── Araponga.Domain/
│   └── Users/
│       ├── UserPreferences.cs          (nova)
│       ├── ProfileVisibility.cs        (nova)
│       ├── ContactVisibility.cs        (nova)
│       └── NotificationPreferences.cs  (nova)
│
├── Araponga.Application/
│   ├── Interfaces/
│   │   └── IUserPreferencesRepository.cs  (nova)
│   └── Services/
│       ├── UserPreferencesService.cs     (nova)
│       └── UserProfileService.cs         (nova)
│
├── Araponga.Infrastructure/
│   ├── InMemory/
│   │   └── InMemoryUserPreferencesRepository.cs  (nova)
│   └── Postgres/
│       ├── Entities/
│       │   └── UserPreferencesRecord.cs   (nova)
│       ├── PostgresUserPreferencesRepository.cs  (nova)
│       └── Migrations/
│           └── YYYYMMDDHHMMSS_AddUserPreferences.cs  (nova)
│
└── Araponga.Api/
    ├── Controllers/
    │   ├── UserPreferencesController.cs  (nova)
    │   └── UserProfileController.cs      (nova)
    └── Contracts/
        └── Users/
            ├── UpdatePrivacyPreferencesRequest.cs      (nova)
            ├── UpdateNotificationPreferencesRequest.cs (nova)
            ├── UpdateDisplayNameRequest.cs             (nova)
            ├── UpdateContactInfoRequest.cs             (nova)
            ├── UserPreferencesResponse.cs              (nova)
            └── UserProfileResponse.cs                  (nova)
```

---

## 🔄 Fluxo de Implementação

### Fase 1: Modelo de Domínio e Repositório
1. ✅ Criar enums `ProfileVisibility` e `ContactVisibility`
2. ✅ Criar value object `NotificationPreferences`
3. ✅ Criar entidade `UserPreferences`
4. ✅ Criar interface `IUserPreferencesRepository`
5. ✅ Implementar `InMemoryUserPreferencesRepository`
6. ✅ Implementar `PostgresUserPreferencesRepository`
7. ✅ Criar migration para tabela `user_preferences`

### Fase 2: Serviços de Aplicação
1. ✅ Criar `UserPreferencesService`
2. ✅ Criar `UserProfileService`
3. ✅ Registrar serviços no DI container

### Fase 3: API e Controllers
1. ✅ Criar contracts (DTOs)
2. ✅ Criar `UserPreferencesController`
3. ✅ Criar `UserProfileController`
4. ✅ Adicionar validações (FluentValidation)

### Fase 4: Integração
1. ✅ Integrar preferências de notificação no `NotificationDispatcher`
2. ✅ Aplicar regras de visibilidade no `UserProfileService.GetProfileAsync`
3. ✅ Atualizar documentação da API

### Fase 5: Testes
1. ✅ Testes unitários para domínio
2. ✅ Testes unitários para serviços
3. ✅ Testes de integração para repositórios
4. ✅ Testes E2E para endpoints

---

## 🧪 Casos de Teste

### Testes de Domínio

1. **UserPreferences**:
   - Criar com valores válidos
   - Rejeitar `userId` vazio
   - Atualizar preferências de privacidade
   - Atualizar preferências de notificação

2. **NotificationPreferences**:
   - Criar com valores padrão
   - Criar com valores customizados

### Testes de Serviço

1. **UserPreferencesService**:
   - Obter preferências existentes
   - Criar preferências padrão quando não existem
   - Atualizar preferências de privacidade
   - Atualizar preferências de notificação

2. **UserProfileService**:
   - Atualizar display name
   - Atualizar informações de contato
   - Obter perfil com regras de visibilidade

### Testes de API

1. **UserPreferencesController**:
   - `GET /api/v1/users/me/preferences` - Retorna preferências
   - `PUT /api/v1/users/me/preferences/privacy` - Atualiza privacidade
   - `PUT /api/v1/users/me/preferences/notifications` - Atualiza notificações
   - Validação de enums inválidos
   - Autenticação obrigatória

2. **UserProfileController**:
   - `GET /api/v1/users/me/profile` - Retorna perfil
   - `PUT /api/v1/users/me/profile/display-name` - Atualiza nome
   - `PUT /api/v1/users/me/profile/contact` - Atualiza contato
   - Validação de campos obrigatórios
   - Autenticação obrigatória

---

## 📝 Documentação

### Atualizações Necessárias

1. **`docs/12_DOMAIN_MODEL.md`**:
   - Adicionar `UserPreferences` à lista de entidades
   - Documentar relacionamento `User 1..1 UserPreferences`

2. **`docs/60_API_LÓGICA_NEGÓCIO.md`**:
   - Adicionar seção "Preferências de Usuário"
   - Documentar endpoints e regras de negócio

3. **`docs/00_INDEX.md`**:
   - Adicionar link para este documento

4. **Swagger/OpenAPI**:
   - Documentar novos endpoints
   - Adicionar exemplos de request/response

---

## 🔐 Considerações de Segurança

1. **Autenticação obrigatória**: Todos os endpoints exigem usuário autenticado
2. **Autorização**: Usuário só pode atualizar suas próprias preferências
3. **Validação de entrada**: Validar enums e campos obrigatórios
4. **Sanitização**: Limpar strings de entrada (trim, normalização)
5. **Auditoria**: Considerar log de mudanças em preferências sensíveis (futuro)

---

## 🚀 Próximos Passos (Pós-MVP)

1. **Foto de perfil**: Upload e armazenamento de imagem
2. **Bio/descrição**: Campo de texto livre para descrição pessoal
3. **Preferências de idioma**: Suporte a múltiplos idiomas
4. **Preferências de tema**: Dark mode / light mode
5. **Histórico de alterações**: Auditoria de mudanças em preferências
6. **Exportação de dados**: Permitir download de dados pessoais (LGPD)
7. **Exclusão de conta**: Funcionalidade de deletar conta e dados

---

## ✅ Checklist de Implementação

- [ ] Modelo de domínio (`UserPreferences`, enums, value objects)
- [ ] Repositórios (interface, InMemory, Postgres)
- [ ] Migration de banco de dados
- [ ] Serviços de aplicação
- [ ] Controllers e DTOs
- [ ] Validações (FluentValidation)
- [ ] Integração com sistema de notificações
- [ ] Aplicação de regras de visibilidade
- [ ] Testes unitários
- [ ] Testes de integração
- [ ] Testes E2E
- [ ] Documentação atualizada
- [ ] Swagger/OpenAPI atualizado

---

**Status**: 📋 Planejamento completo - Pronto para implementação
