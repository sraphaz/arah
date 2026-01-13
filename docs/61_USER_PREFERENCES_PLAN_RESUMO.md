# Resumo Executivo: Preferências de Usuário e Privacidade

**Documento completo**: [61_USER_PREFERENCES_PLAN.md](./61_USER_PREFERENCES_PLAN.md)

---

## 🎯 Objetivo

Implementar funcionalidade completa para que usuários possam configurar:
- **Preferências de Privacidade**: visibilidade do perfil, informações de contato, localização
- **Preferências de Notificações**: habilitar/desabilitar por tipo (posts, comentários, eventos, etc.)
- **Configurações de Perfil**: atualizar nome, email, telefone, endereço

---

## 📊 O que está faltando hoje

### Estado Atual
- ✅ Modelo `User` básico existe (nome, email, telefone, etc.)
- ✅ Sistema de notificações existe, mas sem preferências configuráveis
- ❌ Não há endpoints para gerenciar perfil ou preferências
- ❌ Não há controle de privacidade
- ❌ Notificações são sempre enviadas (sem opção de desabilitar)

### O que será implementado

1. **Entidade `UserPreferences`**:
   - Visibilidade do perfil (Público, Apenas Moradores, Privado)
   - Visibilidade de contato (Email, telefone, endereço)
   - Compartilhamento de localização
   - Visibilidade de membroships (territórios)

2. **Preferências de Notificações**:
   - Posts, comentários, eventos, alertas, marketplace, moderação, solicitações de entrada
   - Cada tipo pode ser habilitado/desabilitado individualmente

3. **Endpoints da API**:
   - `GET /api/v1/users/me/preferences` - Obter preferências
   - `PUT /api/v1/users/me/preferences/privacy` - Atualizar privacidade
   - `PUT /api/v1/users/me/preferences/notifications` - Atualizar notificações
   - `GET /api/v1/users/me/profile` - Obter perfil
   - `PUT /api/v1/users/me/profile/display-name` - Atualizar nome
   - `PUT /api/v1/users/me/profile/contact` - Atualizar contato

---

## 🏗️ Arquitetura

### Componentes Principais

1. **Domínio** (`Araponga.Domain.Users`):
   - `UserPreferences` (entidade)
   - `ProfileVisibility` (enum)
   - `ContactVisibility` (enum)
   - `NotificationPreferences` (value object)

2. **Aplicação** (`Araponga.Application`):
   - `UserPreferencesService` - Gerencia preferências
   - `UserProfileService` - Gerencia perfil do usuário
   - `IUserPreferencesRepository` - Interface de repositório

3. **Infraestrutura**:
   - `InMemoryUserPreferencesRepository` - Para testes/dev
   - `PostgresUserPreferencesRepository` - Para produção
   - Migration para tabela `user_preferences`

4. **API** (`Araponga.Api`):
   - `UserPreferencesController` - Endpoints de preferências
   - `UserProfileController` - Endpoints de perfil

---

## 📋 Estrutura de Dados

### Tabela `user_preferences`

```sql
CREATE TABLE user_preferences (
    user_id UUID PRIMARY KEY,
    profile_visibility VARCHAR(20) DEFAULT 'Public',
    contact_visibility VARCHAR(20) DEFAULT 'ResidentsOnly',
    share_location BOOLEAN DEFAULT false,
    show_memberships BOOLEAN DEFAULT true,
    notifications_posts_enabled BOOLEAN DEFAULT true,
    notifications_comments_enabled BOOLEAN DEFAULT true,
    notifications_events_enabled BOOLEAN DEFAULT true,
    notifications_alerts_enabled BOOLEAN DEFAULT true,
    notifications_marketplace_enabled BOOLEAN DEFAULT true,
    notifications_moderation_enabled BOOLEAN DEFAULT true,
    notifications_membership_requests_enabled BOOLEAN DEFAULT true,
    created_at_utc TIMESTAMP WITH TIME ZONE,
    updated_at_utc TIMESTAMP WITH TIME ZONE
);
```

---

## 🔄 Fluxo de Implementação

### Fase 1: Modelo de Domínio e Repositório
- Criar enums e value objects
- Criar entidade `UserPreferences`
- Implementar repositórios (InMemory e Postgres)
- Criar migration

### Fase 2: Serviços de Aplicação
- Criar `UserPreferencesService`
- Criar `UserProfileService`
- Registrar no DI container

### Fase 3: API e Controllers
- Criar DTOs (requests e responses)
- Criar controllers
- Adicionar validações

### Fase 4: Integração
- Integrar preferências no sistema de notificações
- Aplicar regras de visibilidade no perfil
- Atualizar documentação

### Fase 5: Testes
- Testes unitários
- Testes de integração
- Testes E2E

---

## 📁 Arquivos a Criar

```
backend/Araponga.Domain/Users/
  ├── UserPreferences.cs
  ├── ProfileVisibility.cs
  ├── ContactVisibility.cs
  └── NotificationPreferences.cs

backend/Araponga.Application/
  ├── Interfaces/IUserPreferencesRepository.cs
  └── Services/
      ├── UserPreferencesService.cs
      └── UserProfileService.cs

backend/Araponga.Infrastructure/
  ├── InMemory/InMemoryUserPreferencesRepository.cs
  └── Postgres/
      ├── Entities/UserPreferencesRecord.cs
      ├── PostgresUserPreferencesRepository.cs
      └── Migrations/YYYYMMDDHHMMSS_AddUserPreferences.cs

backend/Araponga.Api/
  ├── Controllers/
  │   ├── UserPreferencesController.cs
  │   └── UserProfileController.cs
  └── Contracts/Users/
      ├── UpdatePrivacyPreferencesRequest.cs
      ├── UpdateNotificationPreferencesRequest.cs
      ├── UpdateDisplayNameRequest.cs
      ├── UpdateContactInfoRequest.cs
      ├── UserPreferencesResponse.cs
      └── UserProfileResponse.cs
```

---

## ✅ Checklist de Implementação

- [ ] Modelo de domínio
- [ ] Repositórios (interface, InMemory, Postgres)
- [ ] Migration de banco de dados
- [ ] Serviços de aplicação
- [ ] Controllers e DTOs
- [ ] Validações
- [ ] Integração com notificações
- [ ] Aplicação de regras de visibilidade
- [ ] Testes (unitários, integração, E2E)
- [ ] Documentação atualizada

---

## 🚀 Próximos Passos

1. Revisar o documento completo: [61_USER_PREFERENCES_PLAN.md](./61_USER_PREFERENCES_PLAN.md)
2. Validar arquitetura proposta
3. Iniciar implementação pela Fase 1 (Modelo de Domínio)

---

**Status**: 📋 Planejamento completo - Pronto para implementação
