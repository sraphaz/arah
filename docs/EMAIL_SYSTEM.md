# Sistema de Envio de Emails

**Fase**: 13  
**Status**: ✅ Implementado  
**Data**: 2025-01-22

---

## 🎯 Visão Geral

Sistema completo de envio de emails para a plataforma Araponga, permitindo comunicação com usuários via email em situações específicas (boas-vindas, recuperação de senha, notificações importantes, alertas críticos).

---

## 📋 Arquitetura

### Camadas

```
┌─────────────────────────────────────┐
│  API Layer (Controllers)            │
│  - AuthController (forgot-password) │
│  - UserPreferencesController        │
└─────────────────────────────────────┘
                  ↓
┌─────────────────────────────────────┐
│  Application Layer                  │
│  - EmailQueueService                │
│  - EmailTemplateService             │
│  - EmailNotificationMapper          │
│  - AuthService (welcome email)      │
│  - CartService (order email)        │
└─────────────────────────────────────┘
                  ↓
┌─────────────────────────────────────┐
│  Domain Layer                       │
│  - EmailQueueItem                   │
│  - EmailPreferences                 │
└─────────────────────────────────────┘
                  ↓
┌─────────────────────────────────────┐
│  Infrastructure Layer               │
│  - SmtpEmailSender                  │
│  - EmailQueueWorker (background)    │
│  - PostgresEmailQueueRepository     │
│  - InMemoryEmailQueueRepository     │
└─────────────────────────────────────┘
```

---

## 🔧 Componentes Principais

### 1. Interface IEmailSender

**Localização**: `backend/Araponga.Application/Interfaces/IEmailSender.cs`

Interface para envio de emails com três sobrecargas:
- `SendEmailAsync(string to, string subject, string body, bool isHtml, CancellationToken)`
- `SendEmailAsync(string to, string subject, string templateName, object templateData, CancellationToken)`
- `SendEmailAsync(EmailMessage message, CancellationToken)`

### 2. Implementação SMTP

**Localização**: `backend/Araponga.Infrastructure/Email/SmtpEmailSender.cs`

Implementação usando MailKit para envio via SMTP.

**Configuração** (`appsettings.json`):
```json
{
  "Email": {
    "Smtp": {
      "Host": "smtp.gmail.com",
      "Port": 587,
      "Username": "noreply@araponga.com",
      "Password": "[secret]",
      "EnableSsl": true
    },
    "FromAddress": "noreply@araponga.com",
    "FromName": "Araponga"
  }
}
```

### 3. Sistema de Templates

**Localização**: `backend/Araponga.Application/Services/EmailTemplateService.cs`

Sistema de templates HTML com suporte a:
- Substituição de propriedades (`{{PropertyName}}`)
- Condicionais (`{{#if PropertyName}}...{{/if}}`)
- Loops (`{{#each Items}}...{{/each}}`)

**Templates disponíveis**:
- `welcome.html` - Email de boas-vindas
- `password-reset.html` - Recuperação de senha
- `event-reminder.html` - Lembrete de evento
- `marketplace-order.html` - Pedido confirmado
- `alert-critical.html` - Alerta crítico

**Layout base**: `_layout.html` com estilos responsivos

### 4. Queue de Envio Assíncrono

**Localização**: `backend/Araponga.Application/Services/EmailQueueService.cs`

Sistema de fila para envio assíncrono de emails com:
- **Prioridades**: Low, Normal, High, Critical
- **Retry Policy**: 
  - Tentativa 1: Imediato
  - Tentativa 2: Após 5 minutos
  - Tentativa 3: Após 15 minutos
  - Tentativa 4: Após 1 hora
  - Máx. 4 tentativas, depois Dead Letter
- **Rate Limiting**: Máx. 100 emails/minuto por instância

**Background Worker**: `EmailQueueWorker` processa a fila a cada 30 segundos

### 5. Integração com Notificações

**Localização**: `backend/Araponga.Infrastructure/Outbox/OutboxDispatcherWorker.cs`

O `OutboxDispatcherWorker` agora também enfileira emails quando apropriado:
- Verifica se notificação deve gerar email (`EmailNotificationMapper.ShouldSendEmail`)
- Verifica preferências de email do usuário
- Enfileira email com template apropriado

**Mapeamento de notificações para emails**:
- `event.created` / `event.reminder` → `event-reminder.html`
- `marketplace.order.confirmed` → `marketplace-order.html`
- `alert.critical` → `alert-critical.html`
- `post.created` → Não gera email (apenas in-app)

### 6. Preferências de Email

**Localização**: `backend/Araponga.Domain/Users/EmailPreferences.cs`

Usuários podem configurar:
- `ReceiveEmails` (bool) - Habilitar/desabilitar emails
- `EmailFrequency` (Immediate, Daily, Weekly) - Frequência de envio
- `EmailTypes` (bit flags) - Tipos de email desejados:
  - Welcome
  - PasswordReset
  - Events
  - Marketplace
  - CriticalAlerts

**Endpoint**: `PUT /api/v1/users/me/preferences/email`

---

## 📧 Casos de Uso Implementados

### 1. Email de Boas-Vindas

**Localização**: `backend/Araponga.Application/Services/AuthService.cs`

Enviado automaticamente quando novo usuário se cadastra via `LoginSocialAsync`.

**Template**: `welcome.html`

### 2. Email de Recuperação de Senha

**Endpoint**: `POST /api/v1/auth/forgot-password`

**Template**: `password-reset.html`

**Status**: Endpoint criado, busca por email ainda não implementada (TODO: adicionar `GetByEmailAsync` ao `IUserRepository`)

### 3. Email de Pedido Confirmado

**Localização**: `backend/Araponga.Application/Services/CartService.cs`

Enviado automaticamente após checkout bem-sucedido.

**Template**: `marketplace-order.html`

### 4. Email de Lembrete de Evento

**Integração**: Via `OutboxDispatcherWorker` quando notificação `event.created` ou `event.reminder` é processada.

**Template**: `event-reminder.html`

### 5. Email de Alerta Crítico

**Integração**: Via `OutboxDispatcherWorker` quando notificação `alert.critical` é processada.

**Template**: `alert-critical.html`

---

## 🔐 Segurança

### Rate Limiting
- **Desenvolvimento**: Sem limite
- **Produção**: Máx. 100 emails/minuto por instância
- **Burst**: Permitir até 200 emails em 1 minuto, depois throttling

### Privacidade
- Emails sempre respeitam preferências do usuário
- Usuários podem opt-out por tipo de email
- Sempre retornar sucesso em `forgot-password` (não revelar se email existe)

---

## 📊 Modelo de Dados

### EmailQueueItem

```csharp
public sealed class EmailQueueItem
{
    public Guid Id { get; }
    public string To { get; }
    public string Subject { get; }
    public string Body { get; }
    public bool IsHtml { get; }
    public string? TemplateName { get; }
    public string? TemplateDataJson { get; }
    public EmailQueuePriority Priority { get; }
    public DateTime? ScheduledFor { get; }
    public int Attempts { get; }
    public EmailQueueStatus Status { get; }
    public DateTime CreatedAtUtc { get; }
    public DateTime? ProcessedAtUtc { get; }
    public string? ErrorMessage { get; }
    public DateTime? NextRetryAtUtc { get; }
}
```

### EmailPreferences

```csharp
public sealed record EmailPreferences
{
    public bool ReceiveEmails { get; init; }
    public EmailFrequency EmailFrequency { get; init; }
    public EmailTypes EmailTypes { get; init; }
}
```

---

## 🚀 Como Usar

### Enviar Email Direto

```csharp
var emailSender = serviceProvider.GetRequiredService<IEmailSender>();

var result = await emailSender.SendEmailAsync(
    "usuario@example.com",
    "Assunto",
    "Corpo do email",
    isHtml: true,
    cancellationToken);
```

### Enviar Email com Template

```csharp
var emailSender = serviceProvider.GetRequiredService<IEmailSender>();

var templateData = new WelcomeEmailTemplateData
{
    UserName = "João",
    BaseUrl = "https://araponga.com",
    ActivationLink = null
};

var result = await emailSender.SendEmailAsync(
    "usuario@example.com",
    "Bem-vindo!",
    "welcome",
    templateData,
    cancellationToken);
```

### Enfileirar Email

```csharp
var emailQueueService = serviceProvider.GetRequiredService<EmailQueueService>();

var emailMessage = new EmailMessage
{
    To = "usuario@example.com",
    Subject = "Assunto",
    Body = "Corpo",
    TemplateName = "welcome",
    TemplateData = templateData,
    IsHtml = true
};

await emailQueueService.EnqueueEmailAsync(
    emailMessage,
    EmailQueuePriority.Normal,
    scheduledFor: null,
    cancellationToken);
```

---

## 📝 Próximos Passos (TODOs)

1. **Implementar `GetByEmailAsync` no `IUserRepository`** para recuperação de senha
2. **Background Job para Lembretes de Evento** - Verificar eventos próximos (24h antes) e enviar emails
3. **Implementação SendGrid** (opcional) - Alternativa ao SMTP
4. **Testes Unitários e Integração** - Cobertura >80%
5. **Migration para EmailPreferences** - Adicionar colunas ao banco de dados

---

## 🔗 Referências

- **Documentação da Fase**: `docs/backlog-api/FASE13.md`
- **Templates**: `backend/Araponga.Api/Templates/Email/`
- **Configuração**: `backend/Araponga.Api/appsettings.json`

---

**Última Atualização**: 2025-01-22
