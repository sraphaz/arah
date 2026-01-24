# Configuração Avançada de Notificações - Araponga

**Última Atualização**: 2025-01-23  
**Status**: 📋 Planejado (Opcional)

---

## 📋 Resumo

Este documento descreve o plano para implementar configuração avançada de notificações por território e globalmente, permitindo controle granular sobre tipos, canais e templates de notificações.

---

## 🎯 Objetivo

Permitir que administradores configurem:
- Quais tipos de notificações são enviados
- Quais canais são usados (in-app, email, push)
- Templates personalizados por território
- Regras de priorização e agrupamento

---

## 📐 Design

### Modelo de Dados

#### Domain Model

```csharp
public sealed class NotificationConfig
{
    public Guid Id { get; }
    public Guid? TerritoryId { get; } // null = global
    public NotificationType Type { get; }
    public IReadOnlyList<NotificationChannel> EnabledChannels { get; }
    public string? TemplateName { get; }
    public bool IsEnabled { get; }
    public int Priority { get; }
    public DateTime CreatedAtUtc { get; }
    public DateTime UpdatedAtUtc { get; }
}

public enum NotificationType
{
    PostCreated,
    CommentCreated,
    EventCreated,
    EventReminder,
    AlertCreated,
    MarketplaceInquiry,
    ReportCreated,
    MembershipRequest,
    VotingCreated,
    VotingClosed
}

public enum NotificationChannel
{
    InApp,
    Email,
    Push
}
```

---

## 🔄 Implementação

### 1. Criar Domain Model

**Arquivo**: `backend/Araponga.Domain/Notifications/NotificationConfig.cs`

### 2. Criar Repository

**Arquivo**: `backend/Araponga.Application/Interfaces/INotificationConfigRepository.cs`
**Arquivo**: `backend/Araponga.Infrastructure/Postgres/PostgresNotificationConfigRepository.cs`

### 3. Criar Service

**Arquivo**: `backend/Araponga.Application/Services/NotificationConfigService.cs`

```csharp
public sealed class NotificationConfigService
{
    public async Task<Result<NotificationConfig>> CreateConfigAsync(...)
    public async Task<Result<NotificationConfig>> UpdateConfigAsync(...)
    public async Task<IReadOnlyList<NotificationConfig>> ListByTerritoryAsync(...)
    public async Task<IReadOnlyList<NotificationConfig>> ListGlobalAsync(...)
}
```

### 4. Integrar com NotificationService

**Arquivo**: `backend/Araponga.Application/Services/NotificationService.cs`

- Verificar configuração antes de enviar notificação
- Respeitar canais habilitados
- Usar template configurado se disponível

### 5. Criar API Endpoints

**Arquivo**: `backend/Araponga.Api/Controllers/NotificationConfigController.cs`

```csharp
// GET /api/v1/territories/{id}/notification-configs
// POST /api/v1/territories/{id}/notification-configs
// PUT /api/v1/territories/{id}/notification-configs/{configId}
// DELETE /api/v1/territories/{id}/notification-configs/{configId}

// GET /api/v1/admin/notification-configs (global)
// POST /api/v1/admin/notification-configs
```

### 6. Criar Migration

**Arquivo**: `backend/Araponga.Infrastructure/Postgres/Migrations/YYYYMMDDHHMMSS_AddNotificationConfigs.cs`

```sql
CREATE TABLE notification_configs (
    id UUID PRIMARY KEY,
    territory_id UUID REFERENCES territories(id) ON DELETE CASCADE,
    notification_type INTEGER NOT NULL,
    enabled_channels INTEGER[] NOT NULL,
    template_name TEXT,
    is_enabled BOOLEAN NOT NULL DEFAULT true,
    priority INTEGER NOT NULL DEFAULT 0,
    created_at_utc TIMESTAMP WITH TIME ZONE NOT NULL,
    updated_at_utc TIMESTAMP WITH TIME ZONE NOT NULL,
    UNIQUE(territory_id, notification_type)
);

CREATE INDEX idx_notification_configs_territory 
ON notification_configs(territory_id) 
WHERE territory_id IS NOT NULL;

CREATE INDEX idx_notification_configs_global 
ON notification_configs(territory_id) 
WHERE territory_id IS NULL;
```

---

## 📊 Hierarquia de Configuração

1. **Global** (territory_id = null) - Configuração padrão para todos os territórios
2. **Territorial** (territory_id != null) - Sobrescreve configuração global para o território
3. **Usuário** (UserPreferences) - Sobrescreve tudo para o usuário específico

**Ordem de Precedência**:
1. UserPreferences (mais específico)
2. TerritoryConfig
3. GlobalConfig (menos específico)

---

## 🔍 Casos de Uso

### Exemplo 1: Desabilitar Emails de Posts

```json
POST /api/v1/territories/{id}/notification-configs
{
  "notificationType": "PostCreated",
  "enabledChannels": ["InApp"],
  "isEnabled": true
}
```

### Exemplo 2: Template Personalizado para Eventos

```json
POST /api/v1/territories/{id}/notification-configs
{
  "notificationType": "EventCreated",
  "enabledChannels": ["InApp", "Email"],
  "templateName": "event-created-custom.html",
  "isEnabled": true
}
```

### Exemplo 3: Configuração Global

```json
POST /api/v1/admin/notification-configs
{
  "notificationType": "AlertCreated",
  "enabledChannels": ["InApp", "Email", "Push"],
  "isEnabled": true,
  "priority": 10
}
```

---

## ✅ Benefícios

1. **Flexibilidade**: Cada território pode ter suas próprias regras
2. **Controle Granular**: Configurar por tipo e canal
3. **Templates Personalizados**: Adaptar mensagens por território
4. **Priorização**: Definir importância de diferentes tipos

---

## ⚠️ Considerações

### Performance

- Cache de configurações (similar a FeatureFlags)
- Invalidação quando configuração é atualizada

### Segurança

- Apenas curadores podem configurar notificações do território
- Apenas admins podem configurar notificações globais

### Compatibilidade

- Se não houver configuração, usar padrões atuais
- Migração gradual sem breaking changes

---

## 📚 Referências

- [NotificationService Implementation](../backend/Araponga.Application/Services/NotificationService.cs)
- [UserPreferences Implementation](../backend/Araponga.Domain/Users/UserPreferences.cs)
- [FeatureFlags Pattern](../backend/Araponga.Application/Services/FeatureFlagService.cs)

---

**Nota**: Esta é uma evolução futura. O sistema atual de notificações funciona bem. Configuração avançada pode ser adicionada quando houver necessidade de personalização por território.
