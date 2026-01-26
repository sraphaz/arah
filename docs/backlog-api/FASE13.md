# Fase 13: Conector de Envio de Emails

**Duração**: 2 semanas (14 dias úteis)  
**Prioridade**: 🔴 ALTA (Comunicação essencial)  
**Depende de**: Nenhuma (pode ser feito em paralelo)  
**Estimativa Total**: 80 horas  
**Status**: ⏳ Pendente

---

## 🎯 Objetivo

Implementar um **conector de envio de emails** para que a plataforma possa enviar emails aos usuários em situações específicas (boas-vindas, recuperação de conta, notificações importantes, alertas críticos).

**Importante**: 
- ❌ **NÃO** é uma caixa de entrada de emails na plataforma
- ✅ É apenas um **serviço de envio** para que a plataforma possa enviar emails
- ✅ Usuários recebem emails em suas próprias caixas de entrada (Gmail, Outlook, etc.)

---

## 📋 Contexto e Requisitos

### Estado Atual
- ✅ Sistema de notificações in-app implementado
- ❌ Não existe capacidade de enviar emails
- ❌ Usuários não recebem emails da plataforma

### Requisitos Funcionais
- ✅ Enviar email de boas-vindas ao criar conta
- ✅ Enviar email de recuperação de senha
- ✅ Enviar email quando evento importante está próximo
- ✅ Enviar email quando pedido no marketplace é confirmado
- ✅ Enviar email de alertas críticos (saúde pública, emergências)
- ✅ Enviar email de confirmação de ações importantes
- ✅ Preferências do usuário (opt-in/opt-out por tipo de email)
- ✅ Queue de envio assíncrono
- ✅ Retry policy para falhas
- ✅ Rate limiting para evitar spam

---

## 📋 Tarefas Detalhadas

### Semana 13: Infraestrutura de Envio

#### 13.1 Interface e Abstração
**Estimativa**: 8 horas (1 dia)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar interface `IEmailSender`:
  - [ ] `SendEmailAsync(string to, string subject, string body, bool isHtml, CancellationToken)`
  - [ ] `SendEmailAsync(string to, string subject, string templateName, object templateData, CancellationToken)`
- [ ] Criar modelo `EmailMessage`:
  - [ ] `To`, `Subject`, `Body`, `IsHtml`, `From`, `ReplyTo`
- [ ] Criar modelo `EmailTemplateData` para dados de template

**Arquivos a Criar**:
- `backend/Araponga.Application/Interfaces/IEmailSender.cs`
- `backend/Araponga.Application/Models/EmailMessage.cs`
- `backend/Araponga.Application/Models/EmailTemplateData.cs`

**Critérios de Sucesso**:
- ✅ Interface criada
- ✅ Modelos criados
- ✅ Abstração clara e extensível

---

#### 13.2 Implementação SMTP
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `SmtpEmailSender`:
  - [ ] Implementar `IEmailSender`
  - [ ] Usar `System.Net.Mail.SmtpClient` ou `MailKit`
  - [ ] Configuração via `IConfiguration`:
    - [ ] `Email:Smtp:Host`
    - [ ] `Email:Smtp:Port`
    - [ ] `Email:Smtp:Username`
    - [ ] `Email:Smtp:Password`
    - [ ] `Email:Smtp:EnableSsl`
    - [ ] `Email:FromAddress`
    - [ ] `Email:FromName`
- [ ] Validação de configuração
- [ ] Tratamento de erros
- [ ] Logging de envios

**Arquivos a Criar**:
- `backend/Araponga.Infrastructure/Email/SmtpEmailSender.cs`
- `backend/Araponga.Infrastructure/Email/EmailConfiguration.cs`

**Arquivos a Modificar**:
- `backend/Araponga.Api/Program.cs` (registrar serviço)

**Critérios de Sucesso**:
- ✅ Envio SMTP funcionando
- ✅ Configuração flexível
- ✅ Logging implementado

---

#### 13.3 Implementação SendGrid (Opcional)
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ❌ Não implementado  
**Prioridade**: 🟢 Opcional (pode ser feito depois)

**Tarefas**:
- [ ] Instalar pacote `SendGrid`
- [ ] Criar `SendGridEmailSender`:
  - [ ] Implementar `IEmailSender`
  - [ ] Usar SendGrid API
  - [ ] Configuração via `IConfiguration`:
    - [ ] `Email:SendGrid:ApiKey`
- [ ] Validação de configuração
- [ ] Tratamento de erros
- [ ] Logging de envios

**Arquivos a Criar**:
- `backend/Araponga.Infrastructure/Email/SendGridEmailSender.cs`

**Critérios de Sucesso**:
- ✅ Envio SendGrid funcionando
- ✅ Configuração flexível
- ✅ Logging implementado

---

#### 13.4 Sistema de Templates
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `EmailTemplateService`:
  - [ ] `RenderTemplateAsync(string templateName, object data)`
  - [ ] Suportar templates Razor ou Handlebars
  - [ ] Templates em arquivos ou banco de dados
- [ ] Criar templates base:
  - [ ] `welcome.html` (boas-vindas)
  - [ ] `password-reset.html` (recuperação de senha)
  - [ ] `event-reminder.html` (lembrete de evento)
  - [ ] `marketplace-order.html` (pedido confirmado)
  - [ ] `alert-critical.html` (alerta crítico)
- [ ] Layout base para emails (header, footer, estilos)
- [ ] Internacionalização (i18n) de templates

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/EmailTemplateService.cs`
- `backend/Araponga.Application/Interfaces/IEmailTemplateService.cs`
- `backend/Araponga.Api/Templates/Email/welcome.html`
- `backend/Araponga.Api/Templates/Email/password-reset.html`
- `backend/Araponga.Api/Templates/Email/event-reminder.html`
- `backend/Araponga.Api/Templates/Email/marketplace-order.html`
- `backend/Araponga.Api/Templates/Email/alert-critical.html`
- `backend/Araponga.Api/Templates/Email/_layout.html`

**Critérios de Sucesso**:
- ✅ Templates funcionando
- ✅ Renderização correta
- ✅ Layout responsivo

---

### Semana 14: Queue, Integração e Preferências

#### 14.1 Queue de Envio Assíncrono
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar modelo `EmailQueueItem`:
  - [ ] `To`, `Subject`, `Body`, `IsHtml`, `TemplateName`, `TemplateData`
  - [ ] `Priority`, `ScheduledFor`, `Attempts`, `Status`
- [ ] Criar `IEmailQueueRepository`
- [ ] Implementar repositórios (Postgres, InMemory)
- [ ] Criar `EmailQueueService`:
  - [ ] `EnqueueEmailAsync(EmailMessage)`
  - [ ] `ProcessQueueAsync()` (background worker)
  - [ ] Retry policy (exponential backoff)
  - [ ] Dead letter queue para falhas persistentes
- [ ] Criar `EmailQueueWorker` (background service)
- [ ] Rate limiting (máx. X emails por minuto)

**Arquivos a Criar**:
- `backend/Araponga.Domain/Email/EmailQueueItem.cs`
- `backend/Araponga.Application/Interfaces/IEmailQueueRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresEmailQueueRepository.cs`
- `backend/Araponga.Infrastructure/InMemory/InMemoryEmailQueueRepository.cs`
- `backend/Araponga.Application/Services/EmailQueueService.cs`
- `backend/Araponga.Infrastructure/Email/EmailQueueWorker.cs`

**Arquivos a Modificar**:
- `backend/Araponga.Infrastructure/Postgres/ArapongaDbContext.cs` (adicionar DbSet)
- `backend/Araponga.Api/Program.cs` (registrar worker)

**Critérios de Sucesso**:
- ✅ Queue funcionando
- ✅ Retry policy implementada
- ✅ Rate limiting funcionando
- ✅ Dead letter queue funcionando

---

#### 14.2 Integração com Notificações
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Atualizar `OutboxDispatcherWorker`:
  - [ ] Verificar se notificação deve gerar email
  - [ ] Verificar preferências do usuário
  - [ ] Enfileirar email se necessário
- [ ] Criar mapeamento de tipos de notificação para templates:
  - [ ] `post.created` → não gera email (apenas in-app)
  - [ ] `event.created` → `event-reminder.html` (se importante)
  - [ ] `marketplace.order.confirmed` → `marketplace-order.html`
  - [ ] `alert.critical` → `alert-critical.html`
- [ ] Priorização: emails apenas para notificações críticas/importantes

**Arquivos a Modificar**:
- `backend/Araponga.Infrastructure/Outbox/OutboxDispatcherWorker.cs`
- `backend/Araponga.Application/Services/EmailNotificationMapper.cs` (novo)

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/EmailNotificationMapper.cs`

**Critérios de Sucesso**:
- ✅ Integração funcionando
- ✅ Preferências respeitadas
- ✅ Priorização funcionando

---

#### 14.3 Preferências de Email do Usuário
**Estimativa**: 8 horas (1 dia)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Atualizar `UserPreferences` (domínio):
  - [ ] Adicionar `EmailPreferences`:
    - [ ] `ReceiveEmails` (bool)
    - [ ] `EmailFrequency` (Imediato, Diário, Semanal)
    - [ ] `EmailTypes` (Posts, Eventos, Marketplace, Alertas)
- [ ] Atualizar `UserPreferencesService`:
  - [ ] Método `UpdateEmailPreferencesAsync`
- [ ] Atualizar `UserPreferencesController`:
  - [ ] Endpoint `PUT /api/v1/users/me/preferences/email`
- [ ] Criar migration
- [ ] Validação: não enviar email se usuário optou out

**Arquivos a Modificar**:
- `backend/Araponga.Domain/Users/NotificationPreferences.cs` (adicionar EmailPreferences)
- `backend/Araponga.Application/Services/UserPreferencesService.cs`
- `backend/Araponga.Api/Controllers/UserPreferencesController.cs`
- `backend/Araponga.Infrastructure/Postgres/Migrations/YYYYMMDDHHMMSS_AddEmailPreferences.cs`

**Critérios de Sucesso**:
- ✅ Preferências funcionando
- ✅ Opt-in/opt-out funcionando
- ✅ Migração aplicada

---

#### 14.4 Casos de Uso Específicos
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] **Email de Boas-Vindas**:
  - [ ] Integrar em `AuthService.CreateUserAsync`
  - [ ] Template `welcome.html`
  - [ ] Dados: nome do usuário, link para ativação (opcional)
- [ ] **Email de Recuperação de Senha**:
  - [ ] Criar endpoint `POST /api/v1/auth/forgot-password`
  - [ ] Gerar token de reset
  - [ ] Template `password-reset.html`
  - [ ] Dados: link de reset, expiração
- [ ] **Email de Lembrete de Evento**:
  - [ ] Background job para verificar eventos próximos (24h antes)
  - [ ] Template `event-reminder.html`
  - [ ] Dados: nome do evento, data, localização
- [ ] **Email de Pedido Confirmado**:
  - [ ] Integrar em `CartService.CheckoutAsync`
  - [ ] Template `marketplace-order.html`
  - [ ] Dados: itens, total, vendedor
- [ ] **Email de Alerta Crítico**:
  - [ ] Integrar em `AlertService.CreateAlertAsync` (se crítico)
  - [ ] Template `alert-critical.html`
  - [ ] Dados: tipo de alerta, descrição, ações

**Arquivos a Modificar**:
- `backend/Araponga.Application/Services/AuthService.cs`
- `backend/Araponga.Api/Controllers/AuthController.cs`
- `backend/Araponga.Application/Services/EventsService.cs`
- `backend/Araponga.Application/Services/CartService.cs`
- `backend/Araponga.Application/Services/AlertService.cs`

**Arquivos a Criar**:
- `backend/Araponga.Application/BackgroundJobs/EventReminderJob.cs`

**Critérios de Sucesso**:
- ✅ Todos os casos de uso funcionando
- ✅ Templates corretos
- ✅ Dados corretos nos emails

---

#### 14.5 Testes e Documentação
**Estimativa**: 8 horas (1 dia)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Testes unitários:
  - [ ] `SmtpEmailSenderTests`
  - [ ] `EmailTemplateServiceTests`
  - [ ] `EmailQueueServiceTests`
- [ ] Testes de integração:
  - [ ] Envio de email funcionando
  - [ ] Queue funcionando
  - [ ] Preferências respeitadas
- [ ] Testes E2E:
  - [ ] Fluxo completo de boas-vindas
  - [ ] Fluxo completo de recuperação de senha
- [ ] Documentação técnica:
  - [ ] `docs/EMAIL_SYSTEM.md`
  - [ ] Configuração de SMTP/SendGrid
  - [ ] Como criar novos templates
- [ ] Atualizar `docs/CHANGELOG.md`

**Arquivos a Criar**:
- `backend/Araponga.Tests/Infrastructure/Email/SmtpEmailSenderTests.cs`
- `backend/Araponga.Tests/Application/EmailTemplateServiceTests.cs`
- `backend/Araponga.Tests/Application/EmailQueueServiceTests.cs`
- `backend/Araponga.Tests/Integration/EmailIntegrationTests.cs`
- `docs/EMAIL_SYSTEM.md`

**Critérios de Sucesso**:
- ✅ Testes passando
- ✅ Cobertura >80%
- ✅ Documentação completa

---

## 📊 Resumo da Fase 13

| Tarefa | Estimativa | Status | Prioridade |
|--------|------------|--------|------------|
| Interface e Abstração | 8h | ❌ Pendente | 🔴 Crítica |
| Implementação SMTP | 12h | ❌ Pendente | 🔴 Crítica |
| Implementação SendGrid | 12h | ❌ Pendente | 🟢 Opcional |
| Sistema de Templates | 16h | ❌ Pendente | 🔴 Crítica |
| Queue de Envio Assíncrono | 16h | ❌ Pendente | 🔴 Crítica |
| Integração com Notificações | 12h | ❌ Pendente | 🔴 Crítica |
| Preferências de Email | 8h | ❌ Pendente | 🟡 Importante |
| Casos de Uso Específicos | 12h | ❌ Pendente | 🔴 Crítica |
| Testes e Documentação | 8h | ❌ Pendente | 🟡 Importante |
| **Total** | **80h (14 dias)** | | |

---

#### 13.X Configuração de Políticas de Presença
**Estimativa**: 16 horas (2 dias)  
**Status**: ⏳ Pendente  
**Prioridade**: 🟢 Baixa

**Contexto**: Política de presença atualmente fixa em `appsettings.json` (`PresencePolicy: Policy: "ResidentOnly"`). Esta tarefa permite configuração por território para políticas mais flexíveis.

**Tarefas**:
- [ ] Criar modelo de domínio `PresencePolicyConfig`:
  - [ ] `Id`, `TerritoryId` (nullable para config global)
  - [ ] `Policy` (enum: ResidentOnly, VerifiedOnly, Public, Custom)
  - [ ] `CustomRules` (JSON, regras customizadas quando Policy = Custom)
  - [ ] `Enabled` (bool)
  - [ ] `CreatedAtUtc`, `UpdatedAtUtc`
- [ ] Criar `IPresencePolicyConfigRepository` e implementações (Postgres, InMemory)
- [ ] Criar `PresencePolicyConfigService`:
  - [ ] `GetConfigAsync(Guid territoryId, CancellationToken)` → busca config territorial ou global
  - [ ] `CreateOrUpdateConfigAsync(PresencePolicyConfig, CancellationToken)`
  - [ ] `EvaluatePresenceAsync(Guid userId, Guid territoryId, CancellationToken)` → avalia política
- [ ] Atualizar `AccessEvaluator` ou serviço de presença:
  - [ ] Usar `PresencePolicyConfig` ao avaliar presença
  - [ ] Fallback para `appsettings.json` se não configurado
- [ ] Criar `PresencePolicyConfigController`:
  - [ ] `GET /api/v1/territories/{territoryId}/presence-policy-config` (Curator)
  - [ ] `PUT /api/v1/territories/{territoryId}/presence-policy-config` (Curator)
  - [ ] `GET /api/v1/admin/presence-policy-config` (global, SystemAdmin)
  - [ ] `PUT /api/v1/admin/presence-policy-config` (global, SystemAdmin)
- [ ] Interface administrativa (DevPortal):
  - [ ] Seção para configuração de políticas de presença
  - [ ] Explicação de diferentes políticas
- [ ] Testes de integração
- [ ] Documentação

**Arquivos a Criar**:
- `backend/Araponga.Domain/Configuration/PresencePolicyConfig.cs`
- `backend/Araponga.Application/Interfaces/Configuration/IPresencePolicyConfigRepository.cs`
- `backend/Araponga.Application/Services/Configuration/PresencePolicyConfigService.cs`
- `backend/Araponga.Api/Controllers/PresencePolicyConfigController.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresPresencePolicyConfigRepository.cs`
- `backend/Araponga.Infrastructure/InMemory/InMemoryPresencePolicyConfigRepository.cs`
- `backend/Araponga.Tests/Api/PresencePolicyConfigIntegrationTests.cs`

**Arquivos a Modificar**:
- `backend/Araponga.Application/Services/AccessEvaluator.cs` (ou serviço de presença equivalente)
- `backend/Araponga.Infrastructure/InMemory/InMemoryDataStore.cs`
- `backend/Araponga.Api/Extensions/ServiceCollectionExtensions.cs`
- `backend/Araponga.Api/wwwroot/devportal/index.html`

**Critérios de Sucesso**:
- ✅ Políticas configuráveis por território
- ✅ Fallback para configuração global funcionando
- ✅ Avaliação de presença usando configuração
- ✅ Interface administrativa disponível
- ✅ Testes passando
- ✅ Documentação atualizada

**Referência**: Consulte `FASE10_CONFIG_FLEXIBILIZACAO_AVALIACAO.md` para contexto completo.

---

## ✅ Critérios de Sucesso da Fase 13

### Funcionalidades
- ✅ Envio de emails funcionando (SMTP)
- ✅ Templates de email funcionando
- ✅ Queue de envio funcionando
- ✅ Preferências de email funcionando
- ✅ Integração com notificações funcionando
- ✅ Casos de uso específicos funcionando

### Qualidade
- ✅ Cobertura de testes >80%
- ✅ Testes de integração passando
- ✅ Retry policy funcionando
- ✅ Rate limiting funcionando
- Considerar **Testcontainers + PostgreSQL** para testes de integração (queue, outbox) com banco real (estratégia na Fase 19; [TESTCONTAINERS_POSTGRES_IMPACTO](../../TESTCONTAINERS_POSTGRES_IMPACTO.md)).

### Documentação
- ✅ Documentação técnica completa
- ✅ Guia de configuração
- ✅ Changelog atualizado

---

## 🔗 Dependências

- **Nenhuma**: Pode ser feito em paralelo com outras fases

---

## 📝 Notas de Implementação

### Configuração SMTP

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

### Configuração SendGrid (Opcional)

```json
{
  "Email": {
    "SendGrid": {
      "ApiKey": "[secret]"
    },
    "FromAddress": "noreply@araponga.com",
    "FromName": "Araponga"
  }
}
```

### Rate Limiting

- **Desenvolvimento**: Sem limite
- **Produção**: Máx. 100 emails/minuto por instância
- **Burst**: Permitir até 200 emails em 1 minuto, depois throttling

### Retry Policy

- **Tentativa 1**: Imediato
- **Tentativa 2**: Após 5 minutos
- **Tentativa 3**: Após 15 minutos
- **Tentativa 4**: Após 1 hora
- **Máx. tentativas**: 4
- **Dead Letter**: Após 4 tentativas falhadas

---

**Status**: ✅ **FASE 13 COMPLETA**  
**Depende de**: Nenhuma  
**Data de Conclusão**: 2026-01-25
