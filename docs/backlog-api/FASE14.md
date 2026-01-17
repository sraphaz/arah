# Fase 14: Governança Comunitária e Sistema de Votação

**Duração**: 3 semanas (21 dias úteis)  
**Prioridade**: 🔴 CRÍTICA (Essencial para soberania territorial)  
**Depende de**: Nenhuma (pode ser feito em paralelo)  
**Estimativa Total**: 120 horas  
**Status**: ⏳ Pendente

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

### Estado Atual
- ✅ Feed cronológico territorial implementado
- ✅ Sistema de moderação básico (reports, bloqueios)
- ✅ Feature flags por território
- ❌ Usuários não podem personalizar o que veem
- ❌ Não existe sistema de votação
- ❌ Moderação não é dinâmica/comunitária

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
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar modelo `UserInterest`:
  - [ ] `Id`, `UserId`, `InterestTag` (string)
  - [ ] `CreatedAtUtc`
  - [ ] Validação: tag não vazia, máx. 50 caracteres
- [ ] Criar enum ou lista de interesses predefinidos (opcional):
  - [ ] "Meio Ambiente", "Eventos", "Marketplace", "Saúde", "Educação", "Cultura", "Esportes", "Arte", "Música", "Tecnologia"
- [ ] Criar `IUserInterestRepository`
- [ ] Implementar repositórios (Postgres, InMemory)
- [ ] Criar migration

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
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `UserInterestService`:
  - [ ] `AddInterestAsync(Guid userId, string interestTag)`
  - [ ] `RemoveInterestAsync(Guid userId, string interestTag)`
  - [ ] `ListInterestsAsync(Guid userId)`
  - [ ] `ListUsersByInterestAsync(string interestTag, Guid territoryId)`
- [ ] Validações:
  - [ ] Tag não vazia, máx. 50 caracteres
  - [ ] Máx. 10 interesses por usuário
  - [ ] Normalização de tags (trim, lowercase)
- [ ] Testes unitários

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
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `UserInterestsController`:
  - [ ] `GET /api/v1/users/me/interests` (listar interesses)
  - [ ] `POST /api/v1/users/me/interests` (adicionar interesse)
  - [ ] `DELETE /api/v1/users/me/interests/{tag}` (remover interesse)
- [ ] Atualizar `UserProfileResponse`:
  - [ ] Adicionar campo `Interests` (IReadOnlyList<string>)
- [ ] Atualizar `UserProfileService`:
  - [ ] Incluir interesses ao buscar perfil
- [ ] Validação (FluentValidation)
- [ ] Testes de integração

**Arquivos a Criar**:
- `backend/Araponga.Api/Controllers/UserInterestsController.cs`
- `backend/Araponga.Api/Contracts/Users/AddInterestRequest.cs`
- `backend/Araponga.Api/Validators/AddInterestRequestValidator.cs`
- `backend/Araponga.Tests/Integration/UserInterestsIntegrationTests.cs`

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
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar enum `VotingType`:
  - [ ] `ThemePrioritization` (priorização de temas)
  - [ ] `ModerationRule` (regra de moderação)
  - [ ] `TerritoryCharacterization` (caracterização do território)
  - [ ] `FeatureFlag` (feature flag territorial)
  - [ ] `CommunityPolicy` (política comunitária)
- [ ] Criar enum `VotingStatus`:
  - [ ] `Draft`, `Open`, `Closed`, `Approved`, `Rejected`
- [ ] Criar enum `VotingVisibility`:
  - [ ] `AllMembers`, `ResidentsOnly`, `CuratorsOnly`
- [ ] Criar modelo `Voting`:
  - [ ] `Id`, `TerritoryId`, `CreatedByUserId`
  - [ ] `Type` (VotingType)
  - [ ] `Title`, `Description`
  - [ ] `Options` (lista de opções de voto)
  - [ ] `Visibility` (VotingVisibility)
  - [ ] `Status` (VotingStatus)
  - [ ] `StartsAtUtc`, `EndsAtUtc`
  - [ ] `CreatedAtUtc`, `UpdatedAtUtc`
- [ ] Criar modelo `Vote`:
  - [ ] `Id`, `VotingId`, `UserId`
  - [ ] `SelectedOption` (string)
  - [ ] `CreatedAtUtc`
- [ ] Criar repositórios
- [ ] Criar migrations

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
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `VotingService`:
  - [ ] `CreateVotingAsync(Guid territoryId, Guid userId, VotingType type, string title, string description, IReadOnlyList<string> options, VotingVisibility visibility, DateTime? startsAt, DateTime? endsAt)`
  - [ ] `ListVotingsAsync(Guid territoryId, VotingStatus? status, Guid? userId)`
  - [ ] `GetVotingAsync(Guid votingId, Guid? userId)`
  - [ ] `VoteAsync(Guid votingId, Guid userId, string selectedOption)`
  - [ ] `CloseVotingAsync(Guid votingId, Guid userId)` (apenas criador ou curador)
  - [ ] `GetResultsAsync(Guid votingId)` (contagem de votos)
- [ ] Validações:
  - [ ] Apenas residents/curadores podem criar votações (depende do tipo)
  - [ ] Usuário só pode votar uma vez
  - [ ] Votação deve estar aberta
  - [ ] Usuário deve ter permissão (visibility)
  - [ ] Opção selecionada deve existir
- [ ] Aplicação de resultados:
  - [ ] Se `ThemePrioritization`: atualizar ordem de temas no feed (opcional)
  - [ ] Se `ModerationRule`: criar/atualizar regra de moderação
  - [ ] Se `TerritoryCharacterization`: adicionar tags ao território
  - [ ] Se `FeatureFlag`: habilitar/desabilitar feature flag
  - [ ] Se `CommunityPolicy`: criar política comunitária
- [ ] Testes unitários

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
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `VotingsController`:
  - [ ] `POST /api/v1/territories/{territoryId}/votings` (criar votação)
  - [ ] `GET /api/v1/territories/{territoryId}/votings` (listar votações)
  - [ ] `GET /api/v1/votings/{id}` (obter votação)
  - [ ] `POST /api/v1/votings/{id}/vote` (votar)
  - [ ] `POST /api/v1/votings/{id}/close` (fechar votação)
  - [ ] `GET /api/v1/votings/{id}/results` (resultados)
- [ ] Criar requests/responses
- [ ] Validação (FluentValidation)
- [ ] Testes de integração

**Arquivos a Criar**:
- `backend/Araponga.Api/Controllers/VotingsController.cs`
- `backend/Araponga.Api/Contracts/Governance/CreateVotingRequest.cs`
- `backend/Araponga.Api/Contracts/Governance/VotingResponse.cs`
- `backend/Araponga.Api/Contracts/Governance/VoteRequest.cs`
- `backend/Araponga.Api/Contracts/Governance/VotingResultsResponse.cs`
- `backend/Araponga.Api/Validators/CreateVotingRequestValidator.cs`
- `backend/Araponga.Tests/Integration/VotingsIntegrationTests.cs`

**Critérios de Sucesso**:
- ✅ Endpoints funcionando
- ✅ Validações funcionando
- ✅ Testes passando

---

### Semana 17: Moderação Dinâmica e Feed Filtrado

#### 17.1 Moderação Dinâmica Comunitária
**Estimativa**: 20 horas (2.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar modelo `TerritoryModerationRule`:
  - [ ] `Id`, `TerritoryId`, `CreatedByVotingId?` (nullable, se criado por votação)
  - [ ] `RuleType` (ContentType, ProhibitedWords, Behavior, MarketplacePolicy, EventPolicy)
  - [ ] `Rule` (JSON com configuração da regra)
  - [ ] `IsActive` (bool)
  - [ ] `CreatedAtUtc`, `UpdatedAtUtc`
- [ ] Criar `ITerritoryModerationRuleRepository`
- [ ] Implementar repositórios
- [ ] Criar `TerritoryModerationService`:
  - [ ] `CreateRuleAsync(Guid territoryId, Guid userId, RuleType type, object rule)`
  - [ ] `ListRulesAsync(Guid territoryId, bool? isActive)`
  - [ ] `ApplyRulesAsync(Post post)` (verificar se post viola regras)
  - [ ] `ApplyRulesAsync(StoreItem item)` (verificar se item viola regras)
- [ ] Integração com `PostCreationService`:
  - [ ] Verificar regras antes de criar post
  - [ ] Retornar erro se violar regra
- [ ] Integração com `StoreItemService`:
  - [ ] Verificar regras antes de criar item
- [ ] Criar migration
- [ ] Testes unitários

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
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `InterestFilterService`:
  - [ ] `FilterFeedByInterestsAsync(IReadOnlyList<CommunityPost> posts, IReadOnlyList<string> userInterests)`
  - [ ] Lógica: posts que têm tags/categorias que correspondem aos interesses
  - [ ] Opcional: manter feed completo disponível
- [ ] Atualizar `FeedController`:
  - [ ] Adicionar query parameter `filterByInterests` (bool, default: false)
  - [ ] Se `true`: aplicar filtro de interesses
  - [ ] Se `false`: retornar feed completo (cronológico)
- [ ] Atualizar `FeedService`:
  - [ ] Aceitar parâmetro `filterByInterests`
  - [ ] Chamar `InterestFilterService` se necessário
- [ ] **Importante**: Feed cronológico permanece como padrão, filtro é opcional
- [ ] Testes de integração

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/InterestFilterService.cs`
- `backend/Araponga.Tests/Application/InterestFilterServiceTests.cs`

**Arquivos a Modificar**:
- `backend/Araponga.Api/Controllers/FeedController.cs`
- `backend/Araponga.Application/Services/FeedService.cs`
- `backend/Araponga.Tests/Integration/FeedFilterIntegrationTests.cs`

**Critérios de Sucesso**:
- ✅ Filtro de interesses funcionando
- ✅ Feed completo continua disponível
- ✅ Feed cronológico mantido como padrão
- ✅ Testes passando

---

#### 17.3 Caracterização do Território
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar modelo `TerritoryCharacterization`:
  - [ ] `TerritoryId`, `Tags` (IReadOnlyList<string>)
  - [ ] `UpdatedAtUtc`
  - [ ] Tags podem ser definidas por votações
- [ ] Criar `ITerritoryCharacterizationRepository`
- [ ] Implementar repositórios
- [ ] Criar `TerritoryCharacterizationService`:
  - [ ] `UpdateCharacterizationAsync(Guid territoryId, IReadOnlyList<string> tags)`
  - [ ] `GetCharacterizationAsync(Guid territoryId)`
- [ ] Integração com `VotingService`:
  - [ ] Se votação `TerritoryCharacterization` aprovada: atualizar caracterização
- [ ] Atualizar `TerritoryResponse`:
  - [ ] Adicionar campo `Tags` (caracterização)
- [ ] Criar migration
- [ ] Testes

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
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Atualizar `UserProfileResponse`:
  - [ ] Adicionar campo `VotingHistory` (opcional, lista de votações participadas)
  - [ ] Adicionar campo `ModerationContributions` (opcional, contagem de contribuições)
- [ ] Atualizar `UserProfileService`:
  - [ ] Buscar histórico de votações do usuário
  - [ ] Buscar contribuições para moderação (propostas de regras, reports, etc.)
- [ ] Endpoint opcional: `GET /api/v1/users/{id}/profile/governance` (histórico completo)
- [ ] Testes

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
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Testes de integração completos:
  - [ ] Sistema de interesses
  - [ ] Sistema de votação
  - [ ] Moderação dinâmica
  - [ ] Feed filtrado
  - [ ] Caracterização do território
- [ ] Testes de performance (votações com muitos votos)
- [ ] Testes de segurança (permissões)
- [ ] Documentação técnica:
  - [ ] `docs/GOVERNANCE_SYSTEM.md`
  - [ ] `docs/VOTING_SYSTEM.md`
  - [ ] `docs/COMMUNITY_MODERATION.md`
- [ ] Atualizar `docs/CHANGELOG.md`
- [ ] Atualizar Swagger

**Arquivos a Criar**:
- `backend/Araponga.Tests/Integration/GovernanceCompleteIntegrationTests.cs`
- `docs/GOVERNANCE_SYSTEM.md`
- `docs/VOTING_SYSTEM.md`
- `docs/COMMUNITY_MODERATION.md`

**Critérios de Sucesso**:
- ✅ Testes passando
- ✅ Cobertura >85%
- ✅ Documentação completa

---

## 📊 Resumo da Fase 14

| Tarefa | Estimativa | Status | Prioridade |
|--------|------------|--------|------------|
| Modelo de Domínio - Interesses | 8h | ❌ Pendente | 🔴 Crítica |
| Serviço de Interesses | 12h | ❌ Pendente | 🔴 Crítica |
| Controller e Integração com Perfil | 12h | ❌ Pendente | 🔴 Crítica |
| Modelo de Domínio - Votação | 12h | ❌ Pendente | 🔴 Crítica |
| Serviço de Votação | 20h | ❌ Pendente | 🔴 Crítica |
| Controller de Votação | 12h | ❌ Pendente | 🔴 Crítica |
| Moderação Dinâmica Comunitária | 20h | ❌ Pendente | 🔴 Crítica |
| Feed Filtrado por Interesses | 16h | ❌ Pendente | 🔴 Crítica |
| Caracterização do Território | 12h | ❌ Pendente | 🟡 Importante |
| Histórico de Participação no Perfil | 8h | ❌ Pendente | 🟡 Importante |
| Testes e Documentação | 12h | ❌ Pendente | 🟡 Importante |
| **Total** | **120h (21 dias)** | | |

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

**Status**: ⏳ **FASE 14 PENDENTE**  
**Depende de**: Nenhuma  
**Crítico para**: Soberania Territorial e Governança Comunitária
