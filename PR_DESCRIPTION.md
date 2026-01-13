# PR: Preferências de Usuário e Privacidade

**Data**: 2025-01-13  
**Status**: ✅ Implementado e Testado  
**Branch**: `feature/user-preferences`

---

## 📋 Resumo

Implementação completa da funcionalidade de preferências de privacidade e configurações do usuário, permitindo que usuários controlem:
- Visibilidade do perfil e informações de contato
- Preferências de notificações por tipo
- Atualização de informações do perfil

---

## 🎯 Funcionalidades Implementadas

### 1. Preferências de Privacidade
- **Visibilidade do Perfil**: Public, ResidentsOnly, Private
- **Visibilidade de Contato**: Public, ResidentsOnly, Private
- **Compartilhamento de Localização**: Habilitar/desabilitar
- **Visibilidade de Membroships**: Mostrar/ocultar territórios

### 2. Preferências de Notificações
- Controle individual por tipo:
  - Posts
  - Comentários
  - Eventos
  - Alertas
  - Marketplace
  - Moderação
  - Solicitações de Entrada

### 3. Gerenciamento de Perfil
- Atualizar nome de exibição
- Atualizar informações de contato (email, telefone, endereço)
- Obter perfil do usuário

---

## 📁 Arquivos Criados

### Domínio
- `backend/Araponga.Domain/Users/ProfileVisibility.cs`
- `backend/Araponga.Domain/Users/ContactVisibility.cs`
- `backend/Araponga.Domain/Users/NotificationPreferences.cs`
- `backend/Araponga.Domain/Users/UserPreferences.cs`

### Aplicação
- `backend/Araponga.Application/Interfaces/IUserPreferencesRepository.cs`
- `backend/Araponga.Application/Services/UserPreferencesService.cs`
- `backend/Araponga.Application/Services/UserProfileService.cs`

### Infraestrutura
- `backend/Araponga.Infrastructure/InMemory/InMemoryUserPreferencesRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/Entities/UserPreferencesRecord.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresUserPreferencesRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/Migrations/20250113120000_AddUserPreferences.cs`

### API
- `backend/Araponga.Api/Controllers/UserPreferencesController.cs`
- `backend/Araponga.Api/Controllers/UserProfileController.cs`
- `backend/Araponga.Api/Contracts/Users/UpdatePrivacyPreferencesRequest.cs`
- `backend/Araponga.Api/Contracts/Users/UpdateNotificationPreferencesRequest.cs`
- `backend/Araponga.Api/Contracts/Users/UpdateDisplayNameRequest.cs`
- `backend/Araponga.Api/Contracts/Users/UpdateContactInfoRequest.cs`
- `backend/Araponga.Api/Contracts/Users/UserPreferencesResponse.cs`
- `backend/Araponga.Api/Contracts/Users/UserProfileResponse.cs`

### Testes
- `backend/Araponga.Tests/Domain/UserPreferencesTests.cs`

---

## 📝 Arquivos Modificados

### Infraestrutura
- `backend/Araponga.Infrastructure/InMemory/InMemoryDataStore.cs` - Adicionado `UserPreferences`
- `backend/Araponga.Infrastructure/Postgres/ArapongaDbContext.cs` - Configuração EF Core
- `backend/Araponga.Infrastructure/Postgres/PostgresMappers.cs` - Mappers para UserPreferences
- `backend/Araponga.Infrastructure/InMemory/InMemoryUserRepository.cs` - Método `UpdateAsync`
- `backend/Araponga.Infrastructure/Postgres/PostgresUserRepository.cs` - Método `UpdateAsync`
- `backend/Araponga.Infrastructure/Outbox/OutboxDispatcherWorker.cs` - Integração com preferências

### Aplicação
- `backend/Araponga.Application/Interfaces/IUserRepository.cs` - Método `UpdateAsync`

### API
- `backend/Araponga.Api/Extensions/ServiceCollectionExtensions.cs` - Registro de serviços e repositórios

### Documentação
- `docs/12_DOMAIN_MODEL.md` - Adicionado UserPreferences
- `docs/60_API_LÓGICA_NEGÓCIO.md` - Seção completa de Preferências de Usuário
- `docs/00_INDEX.md` - Links atualizados

---

## 🔌 Endpoints da API

### Preferências
- `GET /api/v1/users/me/preferences` - Obter preferências
- `PUT /api/v1/users/me/preferences/privacy` - Atualizar privacidade
- `PUT /api/v1/users/me/preferences/notifications` - Atualizar notificações

### Perfil
- `GET /api/v1/users/me/profile` - Obter perfil
- `PUT /api/v1/users/me/profile/display-name` - Atualizar nome
- `PUT /api/v1/users/me/profile/contact` - Atualizar contato

---

## 🗄️ Migração de Banco de Dados

### Tabela `user_preferences`
```sql
CREATE TABLE user_preferences (
    user_id UUID PRIMARY KEY,
    profile_visibility INTEGER NOT NULL,
    contact_visibility INTEGER NOT NULL,
    share_location BOOLEAN NOT NULL,
    show_memberships BOOLEAN NOT NULL,
    notifications_posts_enabled BOOLEAN NOT NULL,
    notifications_comments_enabled BOOLEAN NOT NULL,
    notifications_events_enabled BOOLEAN NOT NULL,
    notifications_alerts_enabled BOOLEAN NOT NULL,
    notifications_marketplace_enabled BOOLEAN NOT NULL,
    notifications_moderation_enabled BOOLEAN NOT NULL,
    notifications_membership_requests_enabled BOOLEAN NOT NULL,
    created_at_utc TIMESTAMP WITH TIME ZONE NOT NULL,
    updated_at_utc TIMESTAMP WITH TIME ZONE NOT NULL
);
```

**Para aplicar a migration:**
```bash
dotnet ef database update \
  --project backend/Araponga.Infrastructure \
  --startup-project backend/Araponga.Api
```

---

## 🔄 Integração com Sistema de Notificações

O `OutboxDispatcherWorker` foi modificado para verificar as preferências do usuário antes de enviar notificações. Se o usuário tiver desabilitado um tipo específico de notificação, ela não será enviada.

**Tipos de notificação suportados:**
- `post.created` → `PostsEnabled`
- `comment.created` → `CommentsEnabled`
- `event.created` → `EventsEnabled`
- `alert.created` → `AlertsEnabled`
- `marketplace.inquiry` → `MarketplaceEnabled`
- `report.created` → `ModerationEnabled`
- `membership.request` → `MembershipRequestsEnabled`

---

## ✅ Testes

### Testes Unitários
- ✅ Validação de UserPreferences (requer userId)
- ✅ Criação de preferências padrão
- ✅ Atualização de preferências de privacidade
- ✅ Atualização de preferências de notificações

### Compilação
- ✅ Projeto compila sem erros
- ✅ Sem erros de linter

---

## 📚 Documentação

### Atualizada
- ✅ `docs/12_DOMAIN_MODEL.md` - UserPreferences adicionado ao modelo
- ✅ `docs/60_API_LÓGICA_NEGÓCIO.md` - Seção completa de Preferências de Usuário
- ✅ `docs/00_INDEX.md` - Links atualizados

### Planejamento
- ✅ `docs/61_USER_PREFERENCES_PLAN.md` - Documento de planejamento completo
- ✅ `docs/61_USER_PREFERENCES_PLAN_RESUMO.md` - Resumo executivo

---

## 🔐 Segurança

- ✅ Todos os endpoints exigem autenticação (JWT)
- ✅ Usuário só pode atualizar suas próprias preferências
- ✅ Validação de entrada (enums, campos obrigatórios)
- ✅ Sanitização de strings (trim, normalização)

---

## 🚀 Próximos Passos (Pós-MVP)

1. Foto de perfil
2. Bio/descrição pessoal
3. Preferências de idioma
4. Preferências de tema (dark mode)
5. Histórico de alterações
6. Exportação de dados (LGPD)
7. Exclusão de conta

---

## 📊 Estatísticas

- **Arquivos criados**: 20+
- **Linhas de código**: ~1500+
- **Endpoints**: 6
- **Testes**: 4 testes unitários
- **Migration**: 1 tabela criada

---

## ✅ Checklist de Implementação

- [x] Modelo de domínio (enums, value objects, entidade)
- [x] Repositórios (interface, InMemory, Postgres)
- [x] Migration de banco de dados
- [x] Serviços de aplicação
- [x] Controllers e DTOs
- [x] Validações
- [x] Integração com notificações
- [x] Aplicação de regras de visibilidade
- [x] Testes unitários
- [x] Documentação atualizada
- [x] Registro no DI container
- [x] Compilação sem erros

---

**Status**: ✅ **PRONTO PARA REVIEW E MERGE**

**Documentação completa**: [docs/prs/PR_USER_PREFERENCES.md](docs/prs/PR_USER_PREFERENCES.md)
