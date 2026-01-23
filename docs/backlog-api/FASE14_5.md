# Fase 14.5: Itens Faltantes e Complementos das Fases 1-14

**Duração**: ~6-8 semanas (30-40 dias úteis)  
**Prioridade**: 🟡 Importante (complementa fases 1-14)  
**Depende de**: Fases 1-14 (parcialmente implementadas)  
**Estimativa Total**: 200-306 horas  
**Status**: ⏳ Pendente

---

## 🎯 Objetivo

Implementar todos os itens que ficaram pendentes ou não plenamente cobertos nas fases 1 até 14, garantindo que todas as funcionalidades planejadas estejam completamente implementadas e testadas.

---

## 📋 Itens Faltantes por Fase

### Fase 1: Segurança e Fundação Crítica

#### 1.1 Connection Pooling Explícito (Parcial)
**Estimativa**: 4-6 h  
**Prioridade**: 🟡 Importante

- [ ] Adicionar métricas de conexões (monitoramento)
- [ ] Validar configuração de pooling em produção
- [ ] Documentar configuração recomendada

**Status atual**: Connection pooling configurado, mas métricas pendentes.

---

#### 1.2 Índices de Banco de Dados (Parcial)
**Estimativa**: 8-12 h  
**Prioridade**: 🟡 Importante

- [ ] Validar performance em staging/produção
- [ ] Adicionar índices faltantes identificados em produção
- [ ] Documentar índices existentes e justificativas

**Status atual**: Índices básicos implementados, validação em produção pendente.

---

#### 1.3 Exception Handling Completo (Parcial)
**Estimativa**: 12-16 h  
**Prioridade**: 🟡 Importante

- [ ] Migrar todos os services para exception handling consistente
- [ ] Atualizar testes para exception handling
- [ ] Documentar padrão de exception handling

**Status atual**: Padrão definido, migração ampla pendente.

---

#### 1.4 Migração Result<T> Completa (Parcial)
**Estimativa**: 8-12 h  
**Prioridade**: 🟡 Importante

- [ ] Atualizar todos os testes para Result<T>
- [ ] Validar que nenhum service usa tuplas
- [ ] Documentar padrão Result<T>

**Status atual**: Services migrados, testes pendentes.

---

### Fase 9: Perfil de Usuário Completo

#### 9.1 Avatar e Bio no User
**Estimativa**: 16-20 h  
**Prioridade**: 🔴 Crítica

- [ ] Adicionar campos `AvatarMediaAssetId` e `Bio` ao `User` (Domain)
- [ ] Criar migration `AddUserAvatarAndBio`
- [ ] Implementar `UpdateAvatarAsync` e `UpdateBioAsync` em `UserProfileService`
- [ ] Endpoints `PUT /api/v1/users/me/profile/avatar` e `PUT /api/v1/users/me/profile/bio`
- [ ] `UserProfileResponse` com `AvatarUrl` e `Bio`
- [ ] Testes unitários e integração

**Arquivos**:
- `backend/Araponga.Domain/Users/User.cs` (adicionar campos)
- `backend/Araponga.Application/Services/UserProfileService.cs` (novos métodos)
- `backend/Araponga.Api/Controllers/UserProfileController.cs` (novos endpoints)
- Migration

---

#### 9.2 Visualizar Perfil de Outros
**Estimativa**: 8-12 h  
**Prioridade**: 🟡 Importante

- [ ] Endpoint `GET /api/v1/users/{id}/profile` (visualizar perfil de outros)
- [ ] Respeitar `UserPreferences.ProfileVisibility` (público, residents-only, privado)
- [ ] Retornar apenas informações permitidas
- [ ] Testes de privacidade

**Arquivos**:
- `backend/Araponga.Application/Services/UserProfileService.cs` (sobrecarga GetProfileAsync)
- `backend/Araponga.Api/Controllers/UserProfileController.cs` (novo endpoint)
- `backend/Araponga.Api/Contracts/Users/UserProfilePublicResponse.cs` (novo contract)

---

#### 9.3 Estatísticas de Contribuição Territorial
**Estimativa**: 12-16 h  
**Prioridade**: 🟡 Importante

- [ ] Criar `UserProfileStatsService`:
  - [ ] Posts criados (contagem)
  - [ ] Eventos criados (contagem)
  - [ ] Eventos participados (contagem)
  - [ ] Territórios membro (contagem)
  - [ ] Entidades confirmadas (contagem)
- [ ] Endpoint `GET /api/v1/users/{id}/profile/stats`
- [ ] Respeitar privacidade (estatísticas podem ser públicas ou privadas)
- [ ] Testes

**Arquivos**:
- `backend/Araponga.Application/Services/UserProfileStatsService.cs`
- `backend/Araponga.Application/Models/UserProfileStats.cs`
- `backend/Araponga.Api/Contracts/Users/UserProfileStatsResponse.cs`
- `backend/Araponga.Api/Controllers/UserProfileController.cs` (endpoint stats)

---

### Fase 10: Mídias em Conteúdo

**Status**: ✅ ~98% Completo conforme FASE10.md

**Itens pendentes**:
- [ ] Validar cobertura de testes >90% para mídias
- [ ] Documentação final (se faltar)

---

### Fase 11: Edição e Gestão

#### 11.1 Edição de Posts (Melhorias)
**Estimativa**: 4-6 h  
**Prioridade**: 🟡 Importante  
**Status**: ✅ **Implementado** (`PostEditService`, `EditPostAsync`, endpoint `PATCH /api/v1/feed/{id}`)

**Itens pendentes**:
- [ ] Histórico de edições (`GetPostEditHistoryAsync`) — opcional
- [ ] Feature flag `PostEditingEnabled` — opcional
- [ ] Validar cobertura de testes

**Arquivos existentes**:
- ✅ `backend/Araponga.Application/Services/PostEditService.cs`
- ✅ `backend/Araponga.Api/Controllers/FeedController.cs` (EditPost)
- ✅ `backend/Araponga.Api/Contracts/Feed/EditPostRequest.cs`
- ✅ `backend/Araponga.Tests/Application/PostEditServiceTests.cs`

---

#### 11.2 Edição de Eventos (Melhorias)
**Estimativa**: 4-6 h  
**Prioridade**: 🟡 Importante  
**Status**: ✅ **Implementado** (`EventsService.UpdateEventAsync`, `CancelEventAsync`, endpoint `PATCH /api/v1/events/{id}`)

**Itens pendentes**:
- [ ] Histórico de edições — opcional
- [ ] Feature flag `EventEditingEnabled` — opcional
- [ ] Validar cobertura de testes

**Arquivos existentes**:
- ✅ `backend/Araponga.Application/Services/EventsService.cs` (UpdateEventAsync, CancelEventAsync)
- ✅ `backend/Araponga.Api/Controllers/EventsController.cs` (UpdateEvent, CancelEvent)

---

#### 11.3 Lista de Participantes de Eventos (Validação)
**Estimativa**: 2-4 h  
**Prioridade**: 🟡 Importante  
**Status**: ✅ **Implementado** (`GetEventParticipantsAsync`, endpoint `GET /api/v1/events/{id}/participants`)

**Itens pendentes**:
- [ ] Validar cobertura de testes

**Arquivos existentes**:
- ✅ `backend/Araponga.Application/Services/EventsService.cs` (GetEventParticipantsAsync)
- ✅ `backend/Araponga.Api/Controllers/EventsController.cs` (GetEventParticipants)
- ✅ `backend/Araponga.Api/Contracts/Events/EventParticipantResponse.cs`

---

#### 11.4 Sistema de Avaliações no Marketplace (Validação)
**Estimativa**: 4-6 h  
**Prioridade**: 🟡 Importante  
**Status**: ✅ **Implementado** (`StoreRating`, `StoreItemRating`, `RatingService`, `RatingController`)

**Itens pendentes**:
- [ ] Feature flag `MarketplaceRatingsEnabled` — opcional
- [ ] Validar cobertura de testes
- [ ] Integração com `StoreItemResponse` (AverageRating, ReviewCount) — opcional

**Arquivos existentes**:
- ✅ `backend/Araponga.Domain/Marketplace/StoreRating.cs`
- ✅ `backend/Araponga.Domain/Marketplace/StoreItemRating.cs`
- ✅ `backend/Araponga.Domain/Marketplace/StoreRatingResponse.cs`
- ✅ `backend/Araponga.Application/Services/RatingService.cs`
- ✅ `backend/Araponga.Api/Controllers/RatingController.cs`
- ✅ Repositórios (Postgres, InMemory)

---

#### 11.5 Busca no Marketplace (Melhorias)
**Estimativa**: 8-12 h  
**Prioridade**: 🟡 Importante  
**Status**: ✅ **Implementado** (`MarketplaceSearchService`, `StoreItemService.SearchItemsAsync`, `SearchStoresAsync`, `SearchAllAsync`, endpoints)

**Itens pendentes**:
- [ ] Busca full-text PostgreSQL (atualmente busca simples por nome) — melhorar performance
- [ ] Índices GIN para busca full-text — adicionar se necessário
- [ ] Feature flag `MarketplaceSearchEnabled` — opcional
- [ ] Validar performance de busca full-text em produção

**Arquivos existentes**:
- ✅ `backend/Araponga.Application/Services/MarketplaceSearchService.cs` (SearchStoresAsync, SearchItemsAsync, SearchAllAsync)
- ✅ `backend/Araponga.Application/Services/StoreItemService.cs` (SearchItemsAsync, SearchItemsPagedAsync)
- ✅ `backend/Araponga.Api/Controllers/MarketplaceSearchController.cs`
- ✅ `backend/Araponga.Api/Controllers/ItemsController.cs` (SearchItems)

---

#### 11.6 Histórico de Atividades (Validação)
**Estimativa**: 2-4 h  
**Prioridade**: 🟡 Importante  
**Status**: ✅ **Implementado** (`UserActivityService`, `UserActivityController`, endpoint `GET /api/v1/users/me/activity`)

**Itens pendentes**:
- [ ] Validar cobertura de testes
- [ ] Filtros e paginação avançados — opcional

**Arquivos existentes**:
- ✅ `backend/Araponga.Application/Services/UserActivityService.cs`
- ✅ `backend/Araponga.Application/Models/UserActivityHistory.cs`
- ✅ `backend/Araponga.Api/Controllers/UserActivityController.cs`
- ✅ `backend/Araponga.Api/Contracts/Users/UserActivityHistoryResponse.cs`

---

### Fase 13: Conector de Envio de Emails

#### 13.1 Implementação SMTP Real
**Estimativa**: 12-16 h  
**Prioridade**: 🔴 Crítica

- [ ] Criar `SmtpEmailSender` (implementar `IEmailSender` com SMTP real)
- [ ] Configuração via `IConfiguration` (Host, Port, Username, Password, EnableSsl)
- [ ] Usar `MailKit` ou `System.Net.Mail.SmtpClient`
- [ ] Validação de configuração
- [ ] Tratamento de erros e logging
- [ ] Testes de integração (SMTP de teste)

**Status atual**: `IEmailSender` existe, mas apenas `LoggingEmailSender` (simula envio). `PasswordResetService` usa `IEmailSender`, mas emails não são enviados de fato.

**Arquivos a Criar**:
- `backend/Araponga.Infrastructure/Email/SmtpEmailSender.cs`
- `backend/Araponga.Infrastructure/Email/EmailConfiguration.cs`

**Arquivos a Modificar**:
- `backend/Araponga.Api/Program.cs` (registrar `SmtpEmailSender` em produção, `LoggingEmailSender` em desenvolvimento)

---

#### 13.2 Sistema de Templates
**Estimativa**: 12-16 h  
**Prioridade**: 🟡 Importante

- [ ] Criar `EmailTemplateService`:
  - [ ] `RenderTemplateAsync(string templateName, object data)`
  - [ ] Suportar templates Razor ou Handlebars
- [ ] Templates base:
  - [ ] `welcome.html` (boas-vindas)
  - [ ] `password-reset.html` (recuperação de senha)
  - [ ] `event-reminder.html` (lembrete de evento)
  - [ ] `marketplace-order.html` (pedido confirmado)
  - [ ] `alert-critical.html` (alerta crítico)
- [ ] Layout base para emails (header, footer, estilos)
- [ ] Internacionalização (i18n) de templates — opcional

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/EmailTemplateService.cs`
- `backend/Araponga.Application/Interfaces/IEmailTemplateService.cs`
- `backend/Araponga.Api/Templates/Email/welcome.html`
- `backend/Araponga.Api/Templates/Email/password-reset.html`
- `backend/Araponga.Api/Templates/Email/event-reminder.html`
- `backend/Araponga.Api/Templates/Email/marketplace-order.html`
- `backend/Araponga.Api/Templates/Email/alert-critical.html`
- `backend/Araponga.Api/Templates/Email/_layout.html`

---

#### 13.3 Queue de Envio Assíncrono
**Estimativa**: 16-20 h  
**Prioridade**: 🟡 Importante

- [ ] Criar modelo `EmailQueueItem`:
  - [ ] `To`, `Subject`, `Body`, `IsHtml`, `TemplateName`, `TemplateData`
  - [ ] `Priority`, `ScheduledFor`, `Attempts`, `Status`
- [ ] Criar `IEmailQueueRepository` e implementações (Postgres, InMemory)
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

---

#### 13.4 Integração com Notificações
**Estimativa**: 8-12 h  
**Prioridade**: 🟡 Importante

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
- `backend/Araponga.Application/Services/EmailNotificationMapper.cs` (criar)

---

#### 13.5 Preferências de Email do Usuário
**Estimativa**: 6-8 h  
**Prioridade**: 🟡 Importante

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
- Migration

---

#### 13.6 Casos de Uso Específicos
**Estimativa**: 12-16 h  
**Prioridade**: 🟡 Importante

- [ ] **Email de Boas-Vindas**:
  - [ ] Integrar em `AuthService.CreateUserAsync`
  - [ ] Template `welcome.html`
- [ ] **Email de Recuperação de Senha**:
  - [ ] Endpoint `POST /api/v1/auth/forgot-password` já existe
  - [ ] Integrar com `PasswordResetService` (já usa `IEmailSender`)
  - [ ] Template `password-reset.html`
- [ ] **Email de Lembrete de Evento**:
  - [ ] Background job para verificar eventos próximos (24h antes)
  - [ ] Template `event-reminder.html`
- [ ] **Email de Pedido Confirmado**:
  - [ ] Integrar em `CartService.CheckoutAsync`
  - [ ] Template `marketplace-order.html`
- [ ] **Email de Alerta Crítico**:
  - [ ] Integrar em `AlertService.CreateAlertAsync` (se crítico)
  - [ ] Template `alert-critical.html`

**Arquivos a Modificar**:
- `backend/Araponga.Application/Services/AuthService.cs`
- `backend/Araponga.Api/Controllers/AuthController.cs` (forgot-password já existe)
- `backend/Araponga.Application/Services/EventsService.cs`
- `backend/Araponga.Application/Services/CartService.cs`
- `backend/Araponga.Application/Services/AlertService.cs`

**Arquivos a Criar**:
- `backend/Araponga.Application/BackgroundJobs/EventReminderJob.cs`

---

### Fase 14: Governança Comunitária

#### 14.1 Teste de integração: feed com `filterByInterests=true`
**Estimativa**: 4-6 h  
**Prioridade**: 🔴 Crítica  
**Status**: ✅ Teste criado (`Feed_WithFilterByInterests_ReturnsOnlyMatchingPosts` em `GovernanceIntegrationTests`). Passa quando `filterByInterests` estiver implementado no endpoint do feed.

- [x] Criar teste de integração que:
  - [x] Adiciona interesses ao usuário
  - [x] Cria posts com título/conteúdo que coincidam (ou não) com os interesses
  - [x] Chama `GET /api/v1/feed?filterByInterests=true` e valida que apenas posts relevantes retornam
  - [x] Chama `GET /api/v1/feed` (sem filtro) e valida feed completo
- [x] Incluir em `GovernanceIntegrationTests`

**Arquivos**:
- `backend/Araponga.Tests/Api/GovernanceIntegrationTests.cs` — `Feed_WithFilterByInterests_ReturnsOnlyMatchingPosts`

---

#### 14.2 Testes de performance: votações com muitos votos
**Estimativa**: 8-12 h  
**Prioridade**: 🟡 Importante

- [ ] Criar cenário de teste (ex.: Performance ou Stress):
  - [ ] Votação com centenas/milhares de votos
  - [ ] Validar tempo de `GetResultsAsync` e de listagem de votações
  - [ ] Definir SLA (ex.: resultados em < 500 ms para N votos)
- [ ] Documentar em `docs/PERFORMANCE_TEST_RESULTS.md` ou equivalente

**Arquivos**:
- `backend/Araponga.Tests/Performance/` ou adicionar em suite existente
- Atualizar documentação de performance

---

#### 14.3 Testes de segurança: permissões em governança
**Estimativa**: 6-8 h  
**Prioridade**: 🟡 Importante

- [ ] Reforçar testes de permissões:
  - [ ] Visitor não vota em votação `ResidentsOnly` ou `CuratorsOnly`
  - [ ] Resident não cria votação tipo `ModerationRule` ou `FeatureFlag` (apenas curador)
  - [ ] Usuário não vota duas vezes na mesma votação
  - [ ] Usuário não fecha votação alheia (exceto curador)
- [ ] Incluir em `GovernanceIntegrationTests` ou suite de segurança

**Arquivos**:
- `backend/Araponga.Tests/Api/GovernanceIntegrationTests.cs` ou `SecurityTests.cs`

---

#### 14.4 Verificar e atualizar Swagger/OpenAPI
**Estimativa**: 2-4 h  
**Prioridade**: 🟡 Importante

- [ ] Garantir que todos os endpoints de governança estão expostos no Swagger
- [ ] Verificar exemplos, schemas e descrições para:
  - [ ] Interesses (`/api/v1/users/me/interests`)
  - [ ] Votações (`/api/v1/territories/{id}/votings`, vote, close, results)
  - [ ] Perfil governança (`/api/v1/users/me/profile/governance`)
- [ ] Ajustar `ProducesResponseType` / anotações se necessário
- [ ] Atualizar DevPortal se houver referência à API de governança

**Arquivos**:
- Controllers em `Araponga.Api`, `wwwroot/devportal` se aplicável

---

#### 14.5 Validar cobertura de testes > 85% (governança)
**Estimativa**: 2 h  
**Prioridade**: 🟡 Importante

- [ ] Rodar cobertura restrita a serviços/controllers de governança
- [ ] Identificar trechos não cobertos e adicionar testes mínimos para atingir > 85%

---

#### 14.6 (Opcional) Filtro de feed por tags/categorias explícitas
**Estimativa**: 12-16 h  
**Prioridade**: 🟢 Baixa

- [ ] Estender modelo de post com tags/categorias explícitas (se ainda não existir)
- [ ] Atualizar `InterestFilterService` para filtrar também por essas tags
- [ ] Manter compatibilidade com filtro atual (título/conteúdo)
- [ ] Documentar em `60_04_API_FEED` e `60_19_API_GOVERNANCA`

**Nota**: Fase 14 já implementa filtro por título/conteúdo. Esta tarefa é evolução futura.

---

#### 14.7 (Opcional) Configuração avançada de notificações (14.X)
**Estimativa**: 24 h  
**Prioridade**: 🟢 Baixa

- [ ] Conforme descrito em **FASE14.md** (seção 14.X):
  - [ ] Modelo `NotificationConfig`, repositórios, `NotificationConfigService`
  - [ ] Endpoints de config por território e global (admin)
  - [ ] Integração com `NotificationService` (tipos, canais, templates)
  - [ ] Interface DevPortal e documentação

**Nota**: Pode ser tratada como fase ou tarefa separada.

---

## 📊 Resumo Fase 14.5

| Item | Fase | Estimativa | Prioridade | Status |
|------|------|------------|------------|--------|
| Connection Pooling (métricas) | 1 | 4-6 h | 🟡 Importante | ⏳ Pendente |
| Índices DB (validação) | 1 | 8-12 h | 🟡 Importante | ⏳ Pendente |
| Exception Handling (completo) | 1 | 12-16 h | 🟡 Importante | ⏳ Pendente |
| Result<T> (testes) | 1 | 8-12 h | 🟡 Importante | ⏳ Pendente |
| Avatar e Bio | 9 | 16-20 h | 🔴 Crítica | ⏳ Pendente |
| Visualizar perfil outros | 9 | 8-12 h | 🟡 Importante | ⏳ Pendente |
| Estatísticas contribuição | 9 | 12-16 h | 🟡 Importante | ⏳ Pendente |
| Validação testes mídias | 10 | 2-4 h | 🟡 Importante | ⏳ Pendente |
| Melhorias edição posts | 11 | 4-6 h | 🟡 Importante | ⏳ Pendente |
| Melhorias edição eventos | 11 | 4-6 h | 🟡 Importante | ⏳ Pendente |
| Validação participantes eventos | 11 | 2-4 h | 🟡 Importante | ⏳ Pendente |
| Validação avaliações marketplace | 11 | 4-6 h | 🟡 Importante | ⏳ Pendente |
| Melhorias busca marketplace | 11 | 8-12 h | 🟡 Importante | ⏳ Pendente |
| Validação histórico atividades | 11 | 2-4 h | 🟡 Importante | ⏳ Pendente |
| SMTP Email Sender | 13 | 12-16 h | 🔴 Crítica | ⏳ Pendente |
| Templates de Email | 13 | 12-16 h | 🟡 Importante | ⏳ Pendente |
| Queue de Email | 13 | 16-20 h | 🟡 Importante | ⏳ Pendente |
| Integração Notificações→Email | 13 | 8-12 h | 🟡 Importante | ⏳ Pendente |
| Preferências Email | 13 | 6-8 h | 🟡 Importante | ⏳ Pendente |
| Casos de Uso Email | 13 | 12-16 h | 🟡 Importante | ⏳ Pendente |
| Teste integração feed `filterByInterests` | 14 | 4-6 h | 🔴 Crítica | ✅ Teste criado |
| Testes performance (votações) | 14 | 8-12 h | 🟡 Importante | ⏳ Pendente |
| Testes segurança (permissões) | 14 | 6-8 h | 🟡 Importante | ⏳ Pendente |
| Swagger/OpenAPI governança | 14 | 2-4 h | 🟡 Importante | ⏳ Pendente |
| Cobertura > 85% governança | 14 | 2 h | 🟡 Importante | ⏳ Pendente |
| (Opc.) Filtro por tags explícitas | 14 | 12-16 h | 🟢 Baixa | ⏳ Pendente |
| (Opc.) Config. notificações 14.X | 14 | 24 h | 🟢 Baixa | ⏳ Pendente |
| **Total (obrigatórios)** | | **200-306 h** | | |
| **Total (com opcionais)** | | **236-338 h** | | |

---

## ✅ Critérios de Sucesso

- [ ] Avatar e Bio implementados e funcionando
- [ ] Visualização de perfil de outros funcionando (com privacidade)
- [ ] Estatísticas de contribuição funcionando
- [ ] SMTP Email Sender funcionando (emails reais enviados)
- [ ] Templates de email funcionando
- [ ] Queue de email funcionando (assíncrono, retry)
- [ ] Integração notificações→email funcionando
- [ ] Preferências de email funcionando
- [ ] Casos de uso específicos (boas-vindas, reset, eventos, pedidos, alertas) funcionando
- [ ] Connection pooling validado
- [ ] Índices validados
- [ ] Exception handling completo
- [ ] Testes Result<T> atualizados
- [ ] Teste de integração para `filterByInterests=true` passando
- [ ] Testes de performance para votações com muitos votos definidos e passando
- [ ] Testes de segurança para permissões de governança passando
- [ ] Swagger atualizado para todos os endpoints de governança
- [ ] Cobertura > 85% para código de governança validada
- [ ] Cobertura de testes >85% para novos itens

---

## 🔗 Dependências

- **Fase 1** (Segurança e Fundação) — parcialmente implementada
- **Fase 8** (Infraestrutura de Mídia) — para Avatar
- **Fase 9** (Perfil de Usuário) — parcialmente implementada
- **Fase 10** (Mídias em Conteúdo) — para Avatar
- **Fase 11** (Edição e Gestão) — parcialmente implementada
- **Fase 13** (Emails) — parcialmente implementada (IEmailSender existe, mas apenas LoggingEmailSender)
- **Fase 14** (Governança) — implementada

---

## 📚 Referências

- [FASE1.md](FASE1.md) — Segurança e Fundação
- [FASE9.md](FASE9.md) — Perfil de Usuário Completo
- [FASE10.md](FASE10.md) — Mídias em Conteúdo
- [FASE11.md](FASE11.md) — Edição e Gestão
- [FASE13.md](FASE13.md) — Conector de Emails
- [FASE14.md](FASE14.md) — Governança Comunitária
- [FASE14 Implementação](implementacoes/FASE14_IMPLEMENTACAO_RESUMO.md)
- [Governança API](../api/60_19_API_GOVERNANCA.md)

---

**Status**: ⏳ **FASE 14.5 PENDENTE**  
**Última atualização**: 2025-01-23
