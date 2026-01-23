# Fase 14: Governança Comunitária e Sistema de Votação

**Duração**: 3 semanas (21 dias úteis)  
**Prioridade**: 🔴 CRÍTICA (Essencial para soberania territorial)  
**Depende de**: Nenhuma (pode ser feito em paralelo)  
**Estimativa Total**: 120 horas  
**Status**: ✅ Implementado

---

## 🎯 Objetivo

Implementar sistema de **governança comunitária** que permite:
- Usuários definirem **interesses** que personalizam o que veem no feed
- **Moderação dinâmica** definida pela comunidade através de votações
- **Sistema de votação** para decisões coletivas (priorização de temas, regras, caracterização do território)
- **Associação ao perfil** (interesses, participação em votações, contribuições)

**Princípios**:
- ✅ Feed cronológico permanece (não manipula, apenas filtra)
- ✅ Decisões coletivas através de votações
- ✅ Moderadores têm acesso privilegiado, mas comunidade participa
- ✅ Territórios têm autonomia real

---

## 📋 Contexto e Requisitos

### Estado Atual (pós-implementação)
- ✅ Feed cronológico territorial implementado
- ✅ Sistema de moderação básico (reports, bloqueios)
- ✅ Feature flags por território
- ✅ Interesses do usuário e feed filtrado (opcional, `filterByInterests`)
- ✅ Sistema de votação comunitária (5 tipos, visibilidade)
- ✅ Moderação dinâmica (regras aplicadas em posts e items)
- ✅ Caracterização do território (tags) e histórico no perfil

### Requisitos Funcionais

#### 1. Sistema de Interesses do Usuário
- ✅ Usuário pode definir interesses (tags/categorias)
- ✅ Interesses aparecem no perfil
- ✅ Feed pode ser filtrado por interesses (opcional)
- ✅ Exemplos: "Meio Ambiente", "Eventos", "Marketplace", "Saúde", "Educação", "Cultura"

#### 2. Moderação Dinâmica Comunitária
- ✅ Ferramenta para definir o que é permitido/não permitido no território
- ✅ Moderadores têm acesso privilegiado
- ✅ Usuários podem propor regras
- ✅ Regras podem ser votadas pela comunidade
- ✅ Categorias:
  - Tipos de conteúdo permitidos
  - Palavras/temas proibidos
  - Regras de comportamento
  - Política de marketplace
  - Política de eventos

#### 3. Sistema de Votação
- ✅ Votações para decisões comunitárias
- ✅ Tipos de votações:
  - Priorização de temas (quais aparecem mais no feed)
  - Regras de moderação (o que é permitido/não permitido)
  - Caracterização do território (tags que descrevem)
  - Feature flags territoriais (quais funcionalidades estão ativas)
  - Políticas comunitárias (regras de convivência)
- ✅ Votações podem ser:
  - Abertas (todos os membros)
  - Apenas residents
  - Apenas curadores/moderadores
- ✅ Resultados influenciam configuração do território

#### 4. Associação ao Perfil
- ✅ Interesses do usuário aparecem no perfil
- ✅ Histórico de participação em votações
- ✅ Contribuições para moderação comunitária
- ✅ Reputação comunitária (opcional, baseada em contribuições)

---

## 📋 Tarefas Detalhadas

### Semana 15: Sistema de Interesses

#### 15.1 Modelo de Domínio - Interesses
**Estimativa**: 8 horas (1 dia)  
**Status**: ✅ Implementado

**Tarefas**:
- [x] Criar modelo `UserInterest`:
  - [x] `Id`, `UserId`, `InterestTag` (string)
  - [x] `CreatedAtUtc`
  - [x] Validação: tag não vazia, máx. 50 caracteres
- [x] Criar enum ou lista de interesses predefinidos (opcional):
  - [x] "Meio Ambiente", "Eventos", "Marketplace", "Saúde", "Educação", "Cultura", "Esportes", "Arte", "Música", "Tecnologia"
- [x] Criar `IUserInterestRepository`
- [x] Implementar repositórios (Postgres, InMemory)
- [x] Criar migration

**Arquivos a Criar**:
- `backend/Araponga.Domain/Users/UserInterest.cs`
- `backend/Araponga.Application/Interfaces/IUserInterestRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresUserInterestRepository.cs`
- `backend/Araponga.Infrastructure/InMemory/InMemoryUserInterestRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/Migrations/YYYYMMDDHHMMSS_AddUserInterests.cs`

**Critérios de Sucesso**:
- ✅ Modelo criado
- ✅ Repositórios implementados
- ✅ Migration aplicada

---

#### 15.2 Serviço de Interesses
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ✅ Implementado

**Tarefas**:
- [x] Criar `UserInterestService`:
  - [x] `AddInterestAsync(Guid userId, string interestTag)`
  - [x] `RemoveInterestAsync(Guid userId, string interestTag)`
  - [x] `ListInterestsAsync(Guid userId)`
  - [x] `ListUsersByInterestAsync(string interestTag, Guid territoryId)`
- [x] Validações:
  - [x] Tag não vazia, máx. 50 caracteres
  - [x] Máx. 10 interesses por usuário
  - [x] Normalização de tags (trim, lowercase)
- [x] Testes unitários

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/UserInterestService.cs`
- `backend/Araponga.Tests/Application/UserInterestServiceTests.cs`

**Critérios de Sucesso**:
- ✅ Serviço implementado
- ✅ Validações funcionando
- ✅ Testes passando

---

#### 15.3 Controller e Integração com Perfil
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ✅ Implementado

**Tarefas**:
- [x] Criar `UserInterestsController`:
  - [x] `GET /api/v1/users/me/interests` (listar interesses)
  - [x] `POST /api/v1/users/me/interests` (adicionar interesse)
  - [x] `DELETE /api/v1/users/me/interests/{tag}` (remover interesse)
- [x] Atualizar `UserProfileResponse`:
  - [x] Adicionar campo `Interests` (IReadOnlyList<string>)
- [x] Atualizar `UserProfileService`:
  - [x] Incluir interesses ao buscar perfil
- [x] Validação (FluentValidation)
- [x] Testes de integração (em `GovernanceIntegrationTests`)

**Arquivos a Criar**:
- `backend/Araponga.Api/Controllers/UserInterestsController.cs`
- `backend/Araponga.Api/Contracts/Users/AddInterestRequest.cs`
- `backend/Araponga.Api/Validators/AddInterestRequestValidator.cs`
- `backend/Araponga.Tests/Api/GovernanceIntegrationTests.cs` (cobre interesses)

**Arquivos a Modificar**:
- `backend/Araponga.Api/Contracts/Users/UserProfileResponse.cs`
- `backend/Araponga.Application/Services/UserProfileService.cs`

**Critérios de Sucesso**:
- ✅ Endpoints funcionando
- ✅ Interesses aparecem no perfil
- ✅ Testes passando

---

### Semana 16: Sistema de Votação

#### 16.1 Modelo de Domínio - Votação
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ✅ Implementado

**Tarefas**:
- [x] Criar enum `VotingType`:
  - [x] `ThemePrioritization` (priorização de temas)
  - [x] `ModerationRule` (regra de moderação)
  - [x] `TerritoryCharacterization` (caracterização do território)
  - [x] `FeatureFlag` (feature flag territorial)
  - [x] `CommunityPolicy` (política comunitária)
- [x] Criar enum `VotingStatus`:
  - [x] `Draft`, `Open`, `Closed`, `Approved`, `Rejected`
- [x] Criar enum `VotingVisibility`:
  - [x] `AllMembers`, `ResidentsOnly`, `CuratorsOnly`
- [x] Criar modelo `Voting`:
  - [x] `Id`, `TerritoryId`, `CreatedByUserId`
  - [x] `Type` (VotingType)
  - [x] `Title`, `Description`
  - [x] `Options` (lista de opções de voto)
  - [x] `Visibility` (VotingVisibility)
  - [x] `Status` (VotingStatus)
  - [x] `StartsAtUtc`, `EndsAtUtc`
  - [x] `CreatedAtUtc`, `UpdatedAtUtc`
- [x] Criar modelo `Vote`:
  - [x] `Id`, `VotingId`, `UserId`
  - [x] `SelectedOption` (string)
  - [x] `CreatedAtUtc`
- [x] Criar repositórios
- [x] Criar migrations

**Arquivos a Criar**:
- `backend/Araponga.Domain/Governance/Voting.cs`
- `backend/Araponga.Domain/Governance/Vote.cs`
- `backend/Araponga.Domain/Governance/VotingType.cs`
- `backend/Araponga.Domain/Governance/VotingStatus.cs`
- `backend/Araponga.Domain/Governance/VotingVisibility.cs`
- `backend/Araponga.Application/Interfaces/IVotingRepository.cs`
- `backend/Araponga.Application/Interfaces/IVoteRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresVotingRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresVoteRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/Migrations/YYYYMMDDHHMMSS_AddVotingSystem.cs`

**Critérios de Sucesso**:
- ✅ Modelos criados
- ✅ Repositórios implementados
- ✅ Migrations aplicadas

---

#### 16.2 Serviço de Votação
**Estimativa**: 20 horas (2.5 dias)  
**Status**: ✅ Implementado

**Tarefas**:
- [x] Criar `VotingService`:
  - [x] `CreateVotingAsync(...)` 
  - [x] `ListVotingsAsync(...)`
  - [x] `GetVotingAsync(...)`
  - [x] `VoteAsync(...)`
  - [x] `CloseVotingAsync(...)` (apenas criador ou curador)
  - [x] `GetResultsAsync(...)` (contagem de votos)
- [x] Validações:
  - [x] Apenas residents/curadores podem criar votações (depende do tipo)
  - [x] Usuário só pode votar uma vez
  - [x] Votação deve estar aberta
  - [x] Usuário deve ter permissão (visibility)
  - [x] Opção selecionada deve existir
- [x] Aplicação de resultados:
  - [x] Se `ThemePrioritization`: atualizar ordem de temas no feed (opcional)
  - [x] Se `ModerationRule`: criar/atualizar regra de moderação
  - [x] Se `TerritoryCharacterization`: adicionar tags ao território
  - [x] Se `FeatureFlag`: habilitar/desabilitar feature flag
  - [x] Se `CommunityPolicy`: criar política comunitária
- [x] Testes unitários

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/VotingService.cs`
- `backend/Araponga.Tests/Application/VotingServiceTests.cs`

**Critérios de Sucesso**:
- ✅ Serviço implementado
- ✅ Validações funcionando
- ✅ Aplicação de resultados funcionando
- ✅ Testes passando

---

#### 16.3 Controller de Votação
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ✅ Implementado

**Tarefas**:
- [x] Criar `VotingsController`:
  - [x] `POST /api/v1/territories/{territoryId}/votings` (criar votação)
  - [x] `GET /api/v1/territories/{territoryId}/votings` (listar votações)
  - [x] `GET /api/v1/territories/{territoryId}/votings/{id}` (obter votação)
  - [x] `POST /api/v1/territories/{territoryId}/votings/{id}/vote` (votar)
  - [x] `POST /api/v1/territories/{territoryId}/votings/{id}/close` (fechar votação)
  - [x] `GET /api/v1/territories/{territoryId}/votings/{id}/results` (resultados)
- [x] Criar requests/responses (Governance contracts)
- [x] Validação (FluentValidation)
- [x] Testes de integração (em `GovernanceIntegrationTests`)

**Arquivos a Criar**:
- `backend/Araponga.Api/Controllers/VotingsController.cs`
- `backend/Araponga.Api/Contracts/Governance/CreateVotingRequest.cs`
- `backend/Araponga.Api/Contracts/Governance/VotingResponse.cs`
- `backend/Araponga.Api/Contracts/Governance/VoteRequest.cs`
- `backend/Araponga.Api/Contracts/Governance/VotingResultsResponse.cs`
- `backend/Araponga.Api/Validators/CreateVotingRequestValidator.cs`
- `backend/Araponga.Tests/Api/GovernanceIntegrationTests.cs` (cobre votação)

**Critérios de Sucesso**:
- ✅ Endpoints funcionando
- ✅ Validações funcionando
- ✅ Testes passando

---

### Semana 17: Moderação Dinâmica e Feed Filtrado

#### 17.1 Moderação Dinâmica Comunitária
**Estimativa**: 20 horas (2.5 dias)  
**Status**: ✅ Implementado

**Tarefas**:
- [x] Criar modelo `TerritoryModerationRule`:
  - [x] `Id`, `TerritoryId`, `CreatedByVotingId?` (nullable, se criado por votação)
  - [x] `RuleType` (ContentType, ProhibitedWords, Behavior, MarketplacePolicy, EventPolicy)
  - [x] `Rule` (JSON com configuração da regra)
  - [x] `IsActive` (bool)
  - [x] `CreatedAtUtc`, `UpdatedAtUtc`
- [x] Criar `ITerritoryModerationRuleRepository`
- [x] Implementar repositórios
- [x] Criar `TerritoryModerationService`:
  - [x] `CreateRuleAsync(...)`
  - [x] `ListRulesAsync(...)`
  - [x] `ApplyRulesAsync(Post post)` (verificar se post viola regras)
  - [x] `ApplyRulesAsync(StoreItem item)` (verificar se item viola regras)
- [x] Integração com `PostCreationService`:
  - [x] Verificar regras antes de criar post
  - [x] Retornar erro se violar regra
- [x] Integração com `StoreItemService`:
  - [x] Verificar regras antes de criar item
- [x] Criar migration
- [x] Testes unitários

**Arquivos a Criar**:
- `backend/Araponga.Domain/Governance/TerritoryModerationRule.cs`
- `backend/Araponga.Domain/Governance/RuleType.cs`
- `backend/Araponga.Application/Interfaces/ITerritoryModerationRuleRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresTerritoryModerationRuleRepository.cs`
- `backend/Araponga.Application/Services/TerritoryModerationService.cs`
- `backend/Araponga.Infrastructure/Postgres/Migrations/YYYYMMDDHHMMSS_AddModerationRules.cs`

**Arquivos a Modificar**:
- `backend/Araponga.Application/Services/PostCreationService.cs`
- `backend/Araponga.Application/Services/StoreItemService.cs`

**Critérios de Sucesso**:
- ✅ Regras de moderação funcionando
- ✅ Aplicação de regras funcionando
- ✅ Integração com criação de conteúdo funcionando
- ✅ Testes passando

---

#### 17.2 Feed Filtrado por Interesses
**Estimativa**: 16 horas (2 dias)  
**Status**: ✅ Implementado

**Tarefas**:
- [x] Criar `InterestFilterService`:
  - [x] `FilterFeedByInterestsAsync(posts, userId, territoryId)` — filtra por correspondência **título/conteúdo** com interesses (tags explícitas em posts: ver Fase 14.5)
  - [x] Opcional: manter feed completo disponível
- [x] Atualizar `FeedController`:
  - [x] Adicionar query parameter `filterByInterests` (bool, default: false)
  - [x] Se `true`: aplicar filtro de interesses
  - [x] Se `false`: retornar feed completo (cronológico)
- [x] Atualizar `FeedService`:
  - [x] Aceitar parâmetro `filterByInterests`
  - [x] Chamar `InterestFilterService` se necessário
- [x] **Importante**: Feed cronológico permanece como padrão, filtro é opcional
- [ ] Teste de integração dedicado para `filterByInterests=true` → **Fase 14.5**

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/InterestFilterService.cs`
- `backend/Araponga.Tests/Application/InterestFilterServiceTests.cs`

**Arquivos a Modificar**:
- `backend/Araponga.Api/Controllers/FeedController.cs`
- `backend/Araponga.Application/Services/FeedService.cs`
- `backend/Araponga.Tests/Api/` — teste dedicado em **Fase 14.5**

**Critérios de Sucesso**:
- ✅ Filtro de interesses funcionando
- ✅ Feed completo continua disponível
- ✅ Feed cronológico mantido como padrão
- ✅ Testes passando

---

#### 17.3 Caracterização do Território
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ✅ Implementado

**Tarefas**:
- [x] Criar modelo `TerritoryCharacterization`:
  - [x] `TerritoryId`, `Tags` (IReadOnlyList<string>)
  - [x] `UpdatedAtUtc`
  - [x] Tags podem ser definidas por votações
- [x] Criar `ITerritoryCharacterizationRepository`
- [x] Implementar repositórios
- [x] Criar `TerritoryCharacterizationService`:
  - [x] `UpdateCharacterizationAsync(...)`
  - [x] `GetCharacterizationAsync(...)`
- [x] Integração com `VotingService`:
  - [x] Se votação `TerritoryCharacterization` aprovada: atualizar caracterização
- [x] Atualizar `TerritoryResponse`:
  - [x] Adicionar campo `Tags` (caracterização)
- [x] Criar migration
- [x] Testes

**Arquivos a Criar**:
- `backend/Araponga.Domain/Territories/TerritoryCharacterization.cs`
- `backend/Araponga.Application/Interfaces/ITerritoryCharacterizationRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresTerritoryCharacterizationRepository.cs`
- `backend/Araponga.Application/Services/TerritoryCharacterizationService.cs`
- `backend/Araponga.Infrastructure/Postgres/Migrations/YYYYMMDDHHMMSS_AddTerritoryCharacterization.cs`

**Arquivos a Modificar**:
- `backend/Araponga.Api/Contracts/Territories/TerritoryResponse.cs`
- `backend/Araponga.Application/Services/VotingService.cs`

**Critérios de Sucesso**:
- ✅ Caracterização funcionando
- ✅ Integração com votações funcionando
- ✅ Tags aparecem no território
- ✅ Testes passando

---

#### 17.4 Histórico de Participação no Perfil
**Estimativa**: 8 horas (1 dia)  
**Status**: ✅ Implementado

**Tarefas**:
- [x] `UserProfileGovernanceResponse` (separado): `VotingHistory`, `ModerationContributions`
- [x] Endpoint `GET /api/v1/users/me/profile/governance` (histórico completo)
- [x] Buscar histórico de votações e contribuições para moderação
- [x] Testes

**Arquivos a Modificar**:
- `backend/Araponga.Api/Contracts/Users/UserProfileResponse.cs`
- `backend/Araponga.Application/Services/UserProfileService.cs`
- `backend/Araponga.Api/Controllers/UserProfileController.cs`

**Critérios de Sucesso**:
- ✅ Histórico aparecendo no perfil
- ✅ Contribuições aparecendo no perfil
- ✅ Testes passando

---

#### 17.5 Testes e Documentação
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ✅ Implementado (itens pendentes → **Fase 14.5**)

**Tarefas**:
- [x] Testes de integração: `GovernanceIntegrationTests` (interesses, votação)
- [ ] Testes de integração dedicados: feed filtrado, moderação, caracterização → **Fase 14.5**
- [ ] Testes de performance (votações com muitos votos) → **Fase 14.5**
- [ ] Testes de segurança (permissões) reforçados → **Fase 14.5**
- [x] Documentação técnica:
  - [x] `docs/GOVERNANCE_SYSTEM.md`
  - [x] `docs/VOTING_SYSTEM.md`
  - [x] `docs/COMMUNITY_MODERATION.md`
- [x] Atualizar `docs/CHANGELOG.md`
- [ ] Verificar/atualizar Swagger → **Fase 14.5**

**Arquivos**:
- [x] `backend/Araponga.Tests/Api/GovernanceIntegrationTests.cs`
- [x] `docs/GOVERNANCE_SYSTEM.md`, `VOTING_SYSTEM.md`, `COMMUNITY_MODERATION.md`

**Critérios de Sucesso**:
- ✅ Testes passando
- ✅ Cobertura >85% (validar)
- ✅ Documentação completa

---

## 📊 Resumo da Fase 14

| Tarefa | Estimativa | Status | Prioridade |
|--------|------------|--------|------------|
| Modelo de Domínio - Interesses | 8h | ✅ Implementado | 🔴 Crítica |
| Serviço de Interesses | 12h | ✅ Implementado | 🔴 Crítica |
| Controller e Integração com Perfil | 12h | ✅ Implementado | 🔴 Crítica |
| Modelo de Domínio - Votação | 12h | ✅ Implementado | 🔴 Crítica |
| Serviço de Votação | 20h | ✅ Implementado | 🔴 Crítica |
| Controller de Votação | 12h | ✅ Implementado | 🔴 Crítica |
| Moderação Dinâmica Comunitária | 20h | ✅ Implementado | 🔴 Crítica |
| Feed Filtrado por Interesses | 16h | ✅ Implementado | 🔴 Crítica |
| Caracterização do Território | 12h | ✅ Implementado | 🟡 Importante |
| Histórico de Participação no Perfil | 8h | ✅ Implementado | 🟡 Importante |
| Testes e Documentação | 12h | ✅ Implementado | 🟡 Importante |
| **Total** | **120h (21 dias)** | | |

**Itens pendentes (testes dedicados, performance, segurança, Swagger)** → ver **[Fase 14.5](FASE14_5.md)**.

---

#### 14.X Configuração Avançada de Notificações
**Estimativa**: 24 horas (3 dias)  
**Status**: ⏳ Pendente  
**Prioridade**: 🟡 Média

**Contexto**: `UserPreferences` já permite configuração de notificações por usuário, mas tipos de notificações e canais disponíveis são fixos no código. Esta tarefa permite configuração de tipos, canais e templates por território ou globalmente.

**Tarefas**:
- [ ] Criar modelo de domínio `NotificationConfig`:
  - [ ] `Id`, `TerritoryId` (nullable para config global)
  - [ ] `NotificationTypes` (JSON, array de tipos disponíveis)
  - [ ] `Channels` (JSON, array de canais: Email, Push, InApp, SMS)
  - [ ] `Templates` (JSON, dicionário de templates por tipo)
  - [ ] `DefaultChannels` (JSON, canais padrão por tipo)
  - [ ] `Enabled` (bool)
  - [ ] `CreatedAtUtc`, `UpdatedAtUtc`
- [ ] Criar `INotificationConfigRepository` e implementações (Postgres, InMemory)
- [ ] Criar `NotificationConfigService`:
  - [ ] `GetConfigAsync(Guid? territoryId, CancellationToken)` → busca config territorial ou global
  - [ ] `CreateOrUpdateConfigAsync(NotificationConfig, CancellationToken)`
  - [ ] `GetAvailableTypesAsync(Guid? territoryId, CancellationToken)`
  - [ ] `GetTemplatesAsync(Guid? territoryId, string notificationType, CancellationToken)`
- [ ] Estender `NotificationService`:
  - [ ] Usar `NotificationConfig` ao enviar notificações
  - [ ] Aplicar templates configurados
  - [ ] Respeitar canais disponíveis
- [ ] Criar `NotificationConfigController`:
  - [ ] `GET /api/v1/territories/{territoryId}/notification-config` (Curator)
  - [ ] `PUT /api/v1/territories/{territoryId}/notification-config` (Curator)
  - [ ] `GET /api/v1/admin/notification-config` (global, SystemAdmin)
  - [ ] `PUT /api/v1/admin/notification-config` (global, SystemAdmin)
- [ ] Interface administrativa (DevPortal):
  - [ ] Seção para configuração de notificações
  - [ ] Editor de templates (opcional)
  - [ ] Visualização de canais disponíveis
- [ ] Testes de integração
- [ ] Documentação

**Arquivos a Criar**:
- `backend/Araponga.Domain/Notifications/NotificationConfig.cs`
- `backend/Araponga.Application/Interfaces/Notifications/INotificationConfigRepository.cs`
- `backend/Araponga.Application/Services/Notifications/NotificationConfigService.cs`
- `backend/Araponga.Api/Controllers/NotificationConfigController.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresNotificationConfigRepository.cs`
- `backend/Araponga.Infrastructure/InMemory/InMemoryNotificationConfigRepository.cs`
- `backend/Araponga.Tests/Api/NotificationConfigIntegrationTests.cs`

**Arquivos a Modificar**:
- `backend/Araponga.Application/Services/NotificationService.cs` (ou equivalente)
- `backend/Araponga.Infrastructure/InMemory/InMemoryDataStore.cs`
- `backend/Araponga.Api/Extensions/ServiceCollectionExtensions.cs`
- `backend/Araponga.Api/wwwroot/devportal/index.html`

**Critérios de Sucesso**:
- ✅ Tipos de notificação configuráveis
- ✅ Canais configuráveis por tipo
- ✅ Templates configuráveis
- ✅ Interface administrativa disponível
- ✅ Testes passando
- ✅ Documentação atualizada

**Referência**: Consulte `FASE10_CONFIG_FLEXIBILIZACAO_AVALIACAO.md` para contexto completo.

---

## ✅ Critérios de Sucesso da Fase 14

### Funcionalidades
- ✅ Sistema de interesses funcionando
- ✅ Sistema de votação funcionando
- ✅ Moderação dinâmica funcionando
- ✅ Feed filtrado por interesses funcionando (opcional)
- ✅ Caracterização do território funcionando
- ✅ Histórico de participação no perfil funcionando

### Qualidade
- ✅ Cobertura de testes >85%
- ✅ Testes de integração passando
- ✅ Performance adequada (votações com muitos votos)
- ✅ Segurança validada (permissões)
- Considerar **Testcontainers + PostgreSQL** para testes de integração (votações, interesses) com banco real (estratégia na Fase 19; [TESTCONTAINERS_POSTGRES_IMPACTO](../../TESTCONTAINERS_POSTGRES_IMPACTO.md)).

### Documentação
- ✅ Documentação técnica completa
- ✅ Changelog atualizado
- ✅ Swagger atualizado

---

## 🔗 Dependências

- **Nenhuma**: Pode ser feito em paralelo com outras fases

---

## 📝 Notas de Implementação

### Princípios de Governança

**Feed Cronológico Preservado**:
- ✅ Feed completo (cronológico) é o padrão
- ✅ Filtro por interesses é **opcional** (usuário escolhe)
- ✅ Não manipula ordem, apenas filtra conteúdo
- ✅ Respeita cronologia territorial

**Votações Comunitárias**:
- ✅ Transparência total (todos veem resultados)
- ✅ Decisões coletivas (não apenas moderadores)
- ✅ Resultados aplicados automaticamente (se aprovados)
- ✅ Histórico de votações preservado

**Moderação Dinâmica**:
- ✅ Regras definidas pela comunidade
- ✅ Moderadores têm acesso privilegiado, mas comunidade participa
- ✅ Regras aplicadas automaticamente
- ✅ Transparência nas regras

### Exemplos de Votações

**Priorização de Temas**:
- Opções: "Meio Ambiente", "Eventos", "Marketplace", "Saúde"
- Resultado: Ordem de prioridade (não altera feed cronológico, apenas destaca)

**Regra de Moderação**:
- Proposta: "Proibir posts sobre política partidária"
- Opções: "Aprovar", "Rejeitar"
- Resultado: Se aprovado, regra é criada e aplicada

**Caracterização do Território**:
- Opções: "Rural", "Urbano", "Praia", "Montanha", "Floresta"
- Resultado: Tags adicionadas ao território

---

**Status**: ✅ **FASE 14 IMPLEMENTADA**  
**Depende de**: Nenhuma  
**Crítico para**: Soberania Territorial e Governança Comunitária  
**Itens faltantes**: Ver [Fase 14.5](FASE14_5.md)
