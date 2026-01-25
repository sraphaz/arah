# Fase 9: Perfil de Usuário Completo

**Duração**: 3 semanas (21 dias úteis)  
**Prioridade**: 🔴 CRÍTICA (Bloqueante para transição)  
**Depende de**: Fase 8 (Infraestrutura de Mídia)  
**Estimativa Total**: 120 horas  
**Status**: ⏳ Pendente

---

## 🎯 Objetivo

Completar funcionalidades de perfil de usuário para permitir transição suave de outras plataformas, mantendo valores de **soberania territorial** e **união comunitária**.

**Funcionalidades**:
- Avatar/Foto de perfil
- Bio/Descrição pessoal
- Visualizar perfil de outros usuários
- Estatísticas focadas em contribuição territorial (não popularidade)

---

## 📋 Contexto e Requisitos

### Estado Atual
- ✅ `User` existe (DisplayName, Email, PhoneNumber, Address)
- ✅ `UserPreferences` existe (privacidade, notificações)
- ✅ `UserProfileService` existe (atualizar nome, contato)
- ❌ Avatar/Foto de perfil não existe
- ❌ Bio/Descrição pessoal não existe
- ❌ Visualizar perfil de outros não existe
- ❌ Estatísticas do perfil não existem

### Requisitos Funcionais
- ✅ Upload de avatar (imagem de perfil)
- ✅ Editar bio/descrição pessoal
- ✅ Visualizar perfil de outros usuários (respeitando privacidade)
- ✅ Estatísticas de contribuição territorial (posts, eventos, territórios)
- ✅ Histórico de atividades (opcional, pode ser Fase 14)

### Valores Mantidos
- ✅ Estatísticas focadas em **contribuição comunitária**, não popularidade
- ✅ Privacidade respeitada (visibilidade configurável)
- ✅ Contexto territorial (estatísticas por território)

---

## 📋 Tarefas Detalhadas

### Semana 32: Avatar e Bio

#### 32.1 Modelo de Domínio - Avatar e Bio
**Estimativa**: 6 horas (0.75 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Adicionar campos ao `User` (ou criar `UserProfile` separado)
  - [ ] `AvatarMediaAssetId` (Guid?, nullable)
  - [ ] `Bio` (string?, nullable, máx. 500 caracteres)
- [ ] Criar métodos no `User`:
  - [ ] `UpdateAvatar(Guid? mediaAssetId)`
  - [ ] `UpdateBio(string? bio)`
- [ ] Validações:
  - [ ] Bio: máx. 500 caracteres, trim
  - [ ] Avatar: deve existir como `MediaAsset`
- [ ] Atualizar migrations do banco
- [ ] Testes unitários

**Arquivos a Modificar**:
- `backend/Araponga.Domain/Users/User.cs`
- `backend/Araponga.Infrastructure/Postgres/Entities/UserRecord.cs`
- `backend/Araponga.Infrastructure/Postgres/Migrations/YYYYMMDDHHMMSS_AddUserAvatarAndBio.cs`

**Arquivos a Criar**:
- `backend/Araponga.Tests/Domain/Users/UserAvatarBioTests.cs`

**Critérios de Sucesso**:
- ✅ Campos adicionados ao modelo
- ✅ Validações implementadas
- ✅ Migrations criadas
- ✅ Testes unitários passando

---

#### 32.2 Serviço de Perfil - Avatar e Bio
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Atualizar `UserProfileService`:
  - [ ] `UpdateAvatarAsync(Guid userId, Guid mediaAssetId, CancellationToken)`
    - [ ] Validar que `mediaAssetId` existe e pertence ao usuário
    - [ ] Validar que é imagem (não vídeo)
    - [ ] Atualizar `User.AvatarMediaAssetId`
    - [ ] Se havia avatar anterior, deletar (opcional, ou manter histórico)
  - [ ] `UpdateBioAsync(Guid userId, string? bio, CancellationToken)`
    - [ ] Validar tamanho (máx. 500 caracteres)
    - [ ] Atualizar `User.Bio`
  - [ ] `GetProfileAsync(Guid userId, Guid? viewerUserId, CancellationToken)`
    - [ ] Buscar `User`
    - [ ] Buscar `UserPreferences` (para verificar privacidade)
    - [ ] Verificar permissões de visualização
    - [ ] Retornar perfil completo (com avatar URL, bio, etc.)
- [ ] Tratamento de erros
- [ ] Logging
- [ ] Testes unitários

**Arquivos a Modificar**:
- `backend/Araponga.Application/Services/UserProfileService.cs`

**Arquivos a Criar**:
- `backend/Araponga.Tests/Application/Services/UserProfileServiceAvatarBioTests.cs`

**Critérios de Sucesso**:
- ✅ Serviço atualizado
- ✅ Upload de avatar funcionando
- ✅ Atualização de bio funcionando
- ✅ Validações funcionando
- ✅ Testes unitários passando

---

#### 32.3 Controller de Perfil - Avatar e Bio
**Estimativa**: 10 horas (1.25 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Atualizar `UserProfileController`:
  - [ ] `PUT /api/v1/users/me/profile/avatar`
    - [ ] Aceitar `multipart/form-data` com arquivo de imagem
    - [ ] Upload via `MediaService` (Fase 11)
    - [ ] Atualizar avatar via `UserProfileService`
    - [ ] Retornar perfil atualizado
  - [ ] `PUT /api/v1/users/me/profile/bio`
    - [ ] Aceitar JSON com `bio` (string?)
    - [ ] Atualizar bio via `UserProfileService`
    - [ ] Retornar perfil atualizado
  - [ ] `GET /api/v1/users/me/profile` (já existe, atualizar)
    - [ ] Incluir `AvatarUrl` e `Bio` na resposta
- [ ] Validação de request (FluentValidation)
- [ ] Rate limiting
- [ ] Documentação Swagger

**Arquivos a Modificar**:
- `backend/Araponga.Api/Controllers/UserProfileController.cs`
- `backend/Araponga.Api/Contracts/Users/UserProfileResponse.cs`
- `backend/Araponga.Api/Contracts/Users/UpdateBioRequest.cs`

**Arquivos a Criar**:
- `backend/Araponga.Api/Validators/UpdateBioRequestValidator.cs`

**Critérios de Sucesso**:
- ✅ Endpoints implementados
- ✅ Upload de avatar funcionando via API
- ✅ Atualização de bio funcionando via API
- ✅ Testes de integração passando
- ✅ Documentação Swagger atualizada

---

#### 32.4 Visualizar Perfil de Outros
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Adicionar endpoint `GET /api/v1/users/{id}/profile`
  - [ ] Validar autenticação
  - [ ] Buscar perfil via `UserProfileService.GetProfileAsync`
  - [ ] Respeitar `UserPreferences.ProfileVisibility`:
    - [ ] `Public`: Todos podem ver
    - [ ] `ResidentsOnly`: Apenas moradores do mesmo território
    - [ ] `Private`: Apenas o próprio usuário
  - [ ] Retornar apenas informações permitidas
- [ ] Criar `UserProfilePublicResponse` (versão pública do perfil)
  - [ ] Campos: Id, DisplayName, AvatarUrl, Bio, CreatedAtUtc
  - [ ] Não incluir: Email, PhoneNumber, Address (privados)
- [ ] Testes de privacidade
  - [ ] Perfil público visível por todos
  - [ ] Perfil ResidentsOnly visível apenas por moradores
  - [ ] Perfil privado visível apenas pelo próprio usuário

**Arquivos a Modificar**:
- `backend/Araponga.Api/Controllers/UserProfileController.cs`
- `backend/Araponga.Application/Services/UserProfileService.cs`

**Arquivos a Criar**:
- `backend/Araponga.Api/Contracts/Users/UserProfilePublicResponse.cs`
- `backend/Araponga.Tests/Integration/UserProfilePrivacyTests.cs`

**Critérios de Sucesso**:
- ✅ Endpoint implementado
- ✅ Privacidade respeitada
- ✅ Testes de privacidade passando
- ✅ Documentação Swagger atualizada

---

### Semana 33: Estatísticas de Contribuição Territorial

#### 33.1 Modelo de Estatísticas
**Estimativa**: 8 horas (1 dia)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `UserProfileStats` (value object ou record)
  - [ ] `PostsCreated` (int)
  - [ ] `EventsCreated` (int)
  - [ ] `EventsParticipated` (int)
  - [ ] `TerritoriesMember` (int) - territórios onde é membro
  - [ ] `TerritoriesResident` (int) - territórios onde é resident
  - [ ] `MapEntitiesConfirmed` (int) - entidades confirmadas
  - [ ] `StoreItemsCreated` (int) - itens criados no marketplace
  - [ ] `TotalContributions` (int) - soma de contribuições
- [ ] Criar `UserProfileStatsByTerritory` (estatísticas por território)
  - [ ] `TerritoryId`, `TerritoryName`
  - [ ] `PostsCreated`, `EventsCreated`, etc.
- [ ] Documentar que estatísticas são focadas em **contribuição**, não popularidade

**Arquivos a Criar**:
- `backend/Araponga.Application/Models/UserProfileStats.cs`
- `backend/Araponga.Application/Models/UserProfileStatsByTerritory.cs`

**Critérios de Sucesso**:
- ✅ Modelo de estatísticas criado
- ✅ Documentação clara sobre foco em contribuição

---

#### 33.2 Serviço de Estatísticas
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `UserProfileStatsService`:
  - [ ] `GetStatsAsync(Guid userId, CancellationToken)`
    - [ ] Buscar posts criados pelo usuário
    - [ ] Buscar eventos criados pelo usuário
    - [ ] Buscar eventos onde participou (Interested ou Confirmed)
    - [ ] Buscar territórios onde é membro
    - [ ] Buscar territórios onde é resident
    - [ ] Buscar entidades do mapa confirmadas pelo usuário
    - [ ] Buscar itens do marketplace criados pelo usuário
    - [ ] Calcular totais
    - [ ] Retornar `UserProfileStats`
  - [ ] `GetStatsByTerritoryAsync(Guid userId, CancellationToken)`
    - [ ] Agrupar estatísticas por território
    - [ ] Retornar `IReadOnlyList<UserProfileStatsByTerritory>`
- [ ] Otimizações:
  - [ ] Usar queries agregadas (COUNT, GROUP BY)
  - [ ] Cache de estatísticas (TTL: 15 minutos)
- [ ] Testes unitários

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/UserProfileStatsService.cs`
- `backend/Araponga.Tests/Application/Services/UserProfileStatsServiceTests.cs`

**Dependências**:
- `IFeedRepository` (para posts)
- `IEventRepository` (para eventos)
- `IEventParticipationRepository` (para participações)
- `ITerritoryMembershipRepository` (para memberships)
- `IMapEntityRepository` (para entidades)
- `IStoreItemRepository` (para itens do marketplace)

**Critérios de Sucesso**:
- ✅ Serviço implementado
- ✅ Estatísticas calculadas corretamente
- ✅ Performance adequada (< 500ms)
- ✅ Testes unitários passando

---

#### 33.3 Controller de Estatísticas
**Estimativa**: 8 horas (1 dia)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Adicionar endpoints em `UserProfileController`:
  - [ ] `GET /api/v1/users/me/profile/stats`
    - [ ] Retornar estatísticas do usuário autenticado
  - [ ] `GET /api/v1/users/{id}/profile/stats`
    - [ ] Retornar estatísticas de outro usuário (públicas)
    - [ ] Respeitar privacidade
  - [ ] `GET /api/v1/users/me/profile/stats/territories`
    - [ ] Retornar estatísticas agrupadas por território
- [ ] Criar `UserProfileStatsResponse`
- [ ] Documentação Swagger
- [ ] Testes de integração

**Arquivos a Modificar**:
- `backend/Araponga.Api/Controllers/UserProfileController.cs`

**Arquivos a Criar**:
- `backend/Araponga.Api/Contracts/Users/UserProfileStatsResponse.cs`
- `backend/Araponga.Api/Contracts/Users/UserProfileStatsByTerritoryResponse.cs`

**Critérios de Sucesso**:
- ✅ Endpoints implementados
- ✅ Estatísticas retornadas corretamente
- ✅ Testes de integração passando
- ✅ Documentação Swagger atualizada

---

#### 33.4 Integração com Perfil
**Estimativa**: 8 horas (1 dia)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Atualizar `UserProfileResponse` para incluir estatísticas (opcional)
  - [ ] Adicionar campo `Stats` (opcional, apenas se solicitado)
- [ ] Atualizar `GET /api/v1/users/me/profile`:
  - [ ] Query parameter `?includeStats=true` (opcional)
  - [ ] Se `includeStats=true`, incluir estatísticas na resposta
- [ ] Atualizar `GET /api/v1/users/{id}/profile`:
  - [ ] Mesmo comportamento
- [ ] Testes de integração

**Arquivos a Modificar**:
- `backend/Araponga.Api/Controllers/UserProfileController.cs`
- `backend/Araponga.Api/Contracts/Users/UserProfileResponse.cs`

**Critérios de Sucesso**:
- ✅ Estatísticas integradas ao perfil
- ✅ Query parameter funcionando
- ✅ Testes passando

---

### Semana 34: Testes, Otimizações e Documentação

#### 34.1 Testes de Integração Completos
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Testes de integração de perfil completo
  - [ ] Upload de avatar
  - [ ] Atualização de bio
  - [ ] Visualização de perfil próprio
  - [ ] Visualização de perfil de outros (público, residents-only, privado)
  - [ ] Estatísticas do perfil
- [ ] Testes de privacidade
  - [ ] Perfil público visível por todos
  - [ ] Perfil residents-only visível apenas por moradores
  - [ ] Perfil privado visível apenas pelo próprio usuário
- [ ] Testes de performance
  - [ ] Cálculo de estatísticas (< 500ms)
  - [ ] Upload de avatar (< 2s)
- [ ] Testes de segurança
  - [ ] Validação de tipo de arquivo (avatar)
  - [ ] Validação de tamanho (avatar)
  - [ ] Validação de permissões

**Arquivos a Criar**:
- `backend/Araponga.Tests/Integration/UserProfileIntegrationTests.cs`
- `backend/Araponga.Tests/Integration/UserProfilePrivacyTests.cs`

**Critérios de Sucesso**:
- ✅ Testes de integração passando
- ✅ Cobertura >90%
- ✅ Testes de privacidade passando
- ✅ Testes de performance passando

---

#### 34.2 Otimizações
**Estimativa**: 8 horas (1 dia)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Cache de estatísticas
  - [ ] TTL: 15 minutos
  - [ ] Invalidação quando usuário cria post/evento
- [ ] Otimização de queries
  - [ ] Queries agregadas para estatísticas
  - [ ] Índices apropriados
- [ ] Otimização de avatar
  - [ ] Thumbnails (tamanhos diferentes: 64x64, 128x128, 256x256)
  - [ ] CDN para avatares (futuro)
- [ ] Validação de performance
  - [ ] Estatísticas < 500ms
  - [ ] Upload de avatar < 2s

**Arquivos a Modificar**:
- `backend/Araponga.Application/Services/UserProfileStatsService.cs`
- `backend/Araponga.Application/Services/UserProfileService.cs`

**Critérios de Sucesso**:
- ✅ Cache implementado
- ✅ Queries otimizadas
- ✅ Performance validada

---

#### 34.3 Documentação
**Estimativa**: 8 horas (1 dia)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Documentação técnica
  - [ ] `docs/USER_PROFILE_SYSTEM.md` (arquitetura)
  - [ ] Exemplos de uso da API
- [ ] Atualizar `docs/CHANGELOG.md`
- [ ] Atualizar Swagger com exemplos
- [ ] Guia de migração (como adicionar avatar/bio a usuários existentes)

**Arquivos a Criar**:
- `docs/USER_PROFILE_SYSTEM.md`

**Arquivos a Modificar**:
- `docs/CHANGELOG.md`

**Critérios de Sucesso**:
- ✅ Documentação completa
- ✅ Changelog atualizado
- ✅ Swagger atualizado

---

## 📊 Resumo da Fase 9

| Tarefa | Estimativa | Status | Prioridade |
|--------|------------|--------|------------|
| Modelo de Domínio - Avatar e Bio | 6h | ❌ Pendente | 🔴 Crítica |
| Serviço de Perfil - Avatar e Bio | 12h | ❌ Pendente | 🔴 Crítica |
| Controller de Perfil - Avatar e Bio | 10h | ❌ Pendente | 🔴 Crítica |
| Visualizar Perfil de Outros | 12h | ❌ Pendente | 🔴 Crítica |
| Modelo de Estatísticas | 8h | ❌ Pendente | 🟡 Importante |
| Serviço de Estatísticas | 16h | ❌ Pendente | 🟡 Importante |
| Controller de Estatísticas | 8h | ❌ Pendente | 🟡 Importante |
| Integração com Perfil | 8h | ❌ Pendente | 🟡 Importante |
| Testes de Integração | 12h | ❌ Pendente | 🟡 Importante |
| Otimizações | 8h | ❌ Pendente | 🟢 Melhoria |
| Documentação | 8h | ❌ Pendente | 🟢 Melhoria |
| **Total** | **120h (21 dias)** | | |

---

## ✅ Critérios de Sucesso da Fase 9

### Funcionalidades
- ✅ Upload de avatar funcionando
- ✅ Atualização de bio funcionando
- ✅ Visualização de perfil de outros funcionando
- ✅ Privacidade respeitada
- ✅ Estatísticas de contribuição funcionando

### Qualidade
- ✅ Cobertura de testes >90%
- ✅ Testes de privacidade passando
- ✅ Performance adequada (estatísticas < 500ms, upload < 2s)

### Documentação
- ✅ Documentação técnica completa
- ✅ Changelog atualizado
- ✅ Swagger atualizado

---

## 🔗 Dependências

- **Fase 11**: Infraestrutura de Mídia (obrigatória)
- **Bloqueia**: Nenhuma (mas facilita transição de usuários)

---

## 📝 Notas de Implementação

### Valores Mantidos

**Estatísticas Focadas em Contribuição**:
- ✅ Posts criados (contribuição com conteúdo)
- ✅ Eventos criados (organização comunitária)
- ✅ Eventos participados (engajamento)
- ✅ Territórios membro (presença territorial)
- ✅ Entidades confirmadas (mapeamento comunitário)
- ❌ **NÃO**: Seguidores, likes totais, popularidade

**Privacidade Respeitada**:
- ✅ Perfil público: Todos podem ver
- ✅ Perfil residents-only: Apenas moradores do mesmo território
- ✅ Perfil privado: Apenas o próprio usuário
- ✅ Estatísticas podem ser públicas ou privadas (configurável)

---

## 🔄 Impacto em Funcionalidades Existentes

### Análise de Impacto

**Fase 12 (Perfil de Usuário)** impacta principalmente o sistema de perfil existente, mas também pode afetar outras funcionalidades que exibem informações de usuário.

### Ajustes Necessários

#### 1. Modelo de Domínio `User`

**Arquivo**: `backend/Araponga.Domain/Users/User.cs`

**Mudanças**:
- [ ] Adicionar campo `AvatarMediaAssetId` (Guid?, nullable)
- [ ] Adicionar campo `Bio` (string?, nullable, máx. 500 caracteres)
- [ ] Adicionar métodos:
  - [ ] `UpdateAvatar(Guid? mediaAssetId)`
  - [ ] `UpdateBio(string? bio)`
- [ ] Validações:
  - [ ] Bio: máx. 500 caracteres, trim
  - [ ] Avatar: deve existir como `MediaAsset` (validação no serviço)

**Impacto**: 🟡 Médio - Adiciona campos opcionais, não quebra funcionalidades existentes

---

#### 2. Migração do Banco de Dados

**Arquivo**: `backend/Araponga.Infrastructure/Postgres/Migrations/YYYYMMDDHHMMSS_AddUserAvatarAndBio.cs`

**Mudanças**:
- [ ] Adicionar coluna `avatar_media_asset_id` (uuid, nullable)
- [ ] Adicionar coluna `bio` (varchar(500), nullable)
- [ ] Adicionar foreign key para `media_assets` (se necessário)
- [ ] Adicionar índice em `avatar_media_asset_id` (opcional)

**Impacto**: 🟢 Baixo - Colunas nullable, não afeta dados existentes

---

#### 3. `UserProfileService`

**Arquivo**: `backend/Araponga.Application/Services/UserProfileService.cs`

**Mudanças**:
- [ ] Atualizar `GetProfileAsync`:
  - [ ] Buscar `AvatarMediaAssetId` e `Bio` do `User`
  - [ ] Se `AvatarMediaAssetId` existir, buscar URL via `MediaService`
  - [ ] Incluir `AvatarUrl` e `Bio` na resposta
- [ ] Adicionar `UpdateAvatarAsync`:
  - [ ] Validar que `mediaAssetId` existe e pertence ao usuário
  - [ ] Validar que é imagem (não vídeo)
  - [ ] Atualizar `User.AvatarMediaAssetId`
  - [ ] Se havia avatar anterior, deletar (opcional)
- [ ] Adicionar `UpdateBioAsync`:
  - [ ] Validar tamanho (máx. 500 caracteres)
  - [ ] Atualizar `User.Bio`
- [ ] Adicionar `GetProfileAsync` (sobrecarga para visualizar outros):
  - [ ] Aceitar `Guid userId` e `Guid? viewerUserId`
  - [ ] Respeitar `UserPreferences.ProfileVisibility`
  - [ ] Retornar apenas informações permitidas

**Impacto**: 🟡 Médio - Adiciona novos métodos, métodos existentes continuam funcionando

---

#### 4. `UserProfileController`

**Arquivo**: `backend/Araponga.Api/Controllers/UserProfileController.cs`

**Mudanças**:
- [ ] Atualizar `GET /api/v1/users/me/profile`:
  - [ ] Incluir `AvatarUrl` e `Bio` na resposta
- [ ] Adicionar `PUT /api/v1/users/me/profile/avatar`:
  - [ ] Aceitar `multipart/form-data` com arquivo de imagem
  - [ ] Upload via `MediaService`
  - [ ] Atualizar avatar via `UserProfileService`
- [ ] Adicionar `PUT /api/v1/users/me/profile/bio`:
  - [ ] Aceitar JSON com `bio` (string?)
  - [ ] Atualizar bio via `UserProfileService`
- [ ] Adicionar `GET /api/v1/users/{id}/profile`:
  - [ ] Validar autenticação
  - [ ] Respeitar privacidade
  - [ ] Retornar `UserProfilePublicResponse`

**Impacto**: 🟡 Médio - Adiciona novos endpoints, endpoints existentes continuam funcionando

---

#### 5. `UserProfileResponse`

**Arquivo**: `backend/Araponga.Api/Contracts/Users/UserProfileResponse.cs`

**Mudanças**:
- [ ] Adicionar campo `AvatarUrl` (string?, nullable)
- [ ] Adicionar campo `Bio` (string?, nullable)
- [ ] Manter compatibilidade: campos opcionais, não quebram clientes existentes

**Impacto**: 🟢 Baixo - Campos opcionais, retrocompatível

---

#### 6. Funcionalidades que Exibem Informações de Usuário

**Funcionalidades Afetadas**:
- [ ] **Feed**: Posts podem exibir avatar do autor (opcional, futuro)
- [ ] **Eventos**: Eventos podem exibir avatar do criador (opcional, futuro)
- [ ] **Chat**: Mensagens podem exibir avatar do remetente (opcional, futuro)
- [ ] **Marketplace**: Anúncios podem exibir avatar do vendedor (opcional, futuro)

**Estratégia**: 
- ✅ **Não implementar agora** - Pode ser feito incrementalmente
- ✅ **Manter compatibilidade** - Funcionalidades existentes continuam funcionando sem avatar

**Impacto**: 🟢 Baixo - Melhorias opcionais, não bloqueantes

---

#### 7. Testes Existentes

**Arquivos Afetados**:
- `backend/Araponga.Tests/Application/Services/UserProfileServiceTests.cs`
- `backend/Araponga.Tests/Integration/UserProfileIntegrationTests.cs`

**Mudanças**:
- [ ] Testes existentes devem continuar passando (campos opcionais)
- [ ] Adicionar testes para avatar e bio
- [ ] Adicionar testes de privacidade

**Impacto**: 🟡 Médio - Adiciona novos testes, testes existentes continuam passando

---

### Checklist de Validação

**Antes de Finalizar Fase 12**:
- [ ] Testes existentes de perfil continuam passando
- [ ] Endpoints existentes continuam funcionando
- [ ] Novos endpoints funcionando
- [ ] Migração aplicada e testada
- [ ] Avatar e bio funcionando
- [ ] Privacidade respeitada
- [ ] Estatísticas funcionando

---

**Status**: ⏳ **FASE 9 PENDENTE**  
**Depende de**: Fase 8 (Infraestrutura de Mídia)  
**Impacto**: 🟡 Médio (principalmente no perfil, com melhorias opcionais em outras funcionalidades)
