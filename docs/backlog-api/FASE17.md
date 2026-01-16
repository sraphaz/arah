# Fase 17: Sistema de Gamificação Harmoniosa

**Duração**: 4 semanas (28 dias úteis)  
**Prioridade**: 🔴 ALTA (Engajamento comunitário sustentável)  
**Depende de**: Fase 9 (Perfil de Usuário), Fase 14 (Governança)  
**Integra com**: Fase 18 (Saúde Territorial) - pode ser desenvolvido em paralelo  
**Estimativa Total**: 160 horas  
**Status**: ⏳ Pendente

---

## 🎯 Objetivo

Implementar sistema de **gamificação harmoniosa** que:
- Reforça **contribuição comunitária real** (não engajamento vazio)
- Respeita **interesses do território** (personalização por território)
- É **suave e não invasivo** (não manipula comportamento)
- Foca no **mais importante**: união comunitária e soberania territorial
- Segue **melhores padrões** de gamificação ética

**Princípios**:
- ✅ **Contribuição Real**: Pontos baseados em ações que agregam valor
- ✅ **Contexto Territorial**: Gamificação adaptada aos interesses do território
- ✅ **Harmonia**: Não compete com o propósito principal (união comunitária)
- ✅ **Transparência**: Usuário entende como ganha pontos
- ✅ **Sem Manipulação**: Não usa técnicas de "engajamento" extrativistas

---

## 📋 Contexto e Requisitos

### Estado Atual
- ✅ Sistema de posts, eventos, marketplace, mapa
- ✅ Sistema de perfil de usuário (Fase 9)
- ✅ Sistema de governança e votações (Fase 14)
- ✅ Sistema de interesses do território (Fase 14)
- ❌ Não existe sistema de gamificação
- ❌ Não existe rastreamento de contribuições

### Requisitos Funcionais

#### 1. Sistema de Contribuições
- ✅ Rastrear ações que agregam valor:
  - Criar post relevante (com GeoAnchor)
  - Criar evento comunitário
  - Participar de evento
  - Criar MapEntity (localização útil)
  - Confirmar MapEntity (confirmação colaborativa)
  - Criar item no marketplace
  - Vender no marketplace
  - Comprar no marketplace (economia local)
  - Ser entregador (entregas territoriais)
  - Participar de votação
  - Criar votação relevante
  - Moderar conteúdo (curadores)
  - Reportar conteúdo inadequado
- ✅ Pontos baseados em **valor agregado**, não quantidade
- ✅ Qualidade > Quantidade (IA pode ajudar a avaliar)

#### 2. Interesses do Território
- ✅ Gamificação adaptada aos interesses do território
- ✅ Ações alinhadas aos interesses ganham mais pontos
- ✅ Exemplo: Se território tem interesse "Sustentabilidade":
  - Post sobre reciclagem → +10 pontos
  - Post genérico → +5 pontos
- ✅ Interesses definidos pela comunidade (Fase 14)

#### 3. Níveis e Reconhecimento
- ✅ Níveis baseados em contribuição (não competitivos)
- ✅ Badges/Conquistas por tipos de contribuição
- ✅ Reconhecimento comunitário (não ranking público)
- ✅ Sem comparação direta entre usuários (evita competição tóxica)

#### 4. Visualização Suave
- ✅ Estatísticas no perfil (já existe em Fase 9)
- ✅ Badges discretos (não invasivos)
- ✅ Notificações ocasionais (não spam)
- ✅ Feed não manipulado (cronológico mantido)

#### 5. Ética e Transparência
- ✅ Usuário vê como ganha pontos
- ✅ Histórico de contribuições
- ✅ Sem "surpresas" ou manipulação
- ✅ Foco em contribuição, não em "engajamento"

---

## 📋 Tarefas Detalhadas

### Semana 26: Modelo de Domínio e Contribuições

#### 26.1 Modelo de Domínio - Contribuições
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar enum `ContributionType`:
  - [ ] `PostCreated` (post com GeoAnchor)
  - [ ] `EventCreated` (evento comunitário)
  - [ ] `EventParticipated` (participação em evento)
  - [ ] `MapEntityCreated` (criação de entidade do mapa)
  - [ ] `MapEntityConfirmed` (confirmação de entidade)
  - [ ] `MarketplaceItemCreated` (item no marketplace)
  - [ ] `MarketplaceSale` (venda no marketplace)
  - [ ] `MarketplacePurchase` (compra no marketplace)
  - [ ] `DeliveryCompleted` (entrega completada)
  - [ ] `VotingParticipated` (participação em votação)
  - [ ] `VotingCreated` (criação de votação relevante)
  - [ ] `ModerationAction` (moderação por curador)
  - [ ] `ReportSubmitted` (report útil)
  - [ ] **`WasteCollection`** (coleta de resíduos) 🔴 NOVO
  - [ ] **`TreePlanting`** (plantio de árvores) 🔴 NOVO
  - [ ] **`TerritoryAction`** (mutirão, manutenção, restauração) 🔴 NOVO
  - [ ] **`HealthObservation`** (observação de saúde territorial) 🔴 NOVO
  - [ ] **`SensorReading`** (leitura de sensor confirmada) 🔴 NOVO
  - [ ] **`NaturalAssetMaintenance`** (manutenção de recurso natural) 🔴 NOVO
- [ ] Criar modelo `Contribution`:
  - [ ] `Id`, `UserId`, `TerritoryId`
  - [ ] `Type` (ContributionType)
  - [ ] `Points` (int, pontos ganhos)
  - [ ] `RelatedEntityId` (Guid?, nullable, ID da entidade relacionada)
  - [ ] `RelatedEntityType` (string?, nullable, tipo da entidade)
  - [ ] `InterestAlignment` (decimal?, nullable, 0-1, alinhamento com interesses)
  - [ ] `QualityScore` (decimal?, nullable, 0-1, qualidade avaliada por IA)
  - [ ] `Description` (string?, nullable, descrição da contribuição)
  - [ ] `CreatedAtUtc`
- [ ] Criar modelo `UserContributionStats`:
  - [ ] `UserId`, `TerritoryId`
  - [ ] `TotalPoints` (int, pontos totais)
  - [ ] `Level` (int, nível baseado em pontos)
  - [ ] `ContributionsCount` (int, número de contribuições)
  - [ ] `LastContributionAtUtc` (DateTime?, nullable)
  - [ ] `UpdatedAtUtc`
- [ ] Criar modelo `Badge`:
  - [ ] `Id`, `Name`, `Description`
  - [ ] `IconUrl` (string?, nullable)
  - [ ] `Category` (string, categoria: Community, Marketplace, Events, Map, etc.)
  - [ ] `Criteria` (JSON, critérios para ganhar)
  - [ ] `IsActive` (bool)
- [ ] Criar modelo `UserBadge`:
  - [ ] `Id`, `UserId`, `BadgeId`
  - [ ] `EarnedAtUtc` (DateTime)
  - [ ] `TerritoryId?` (Guid?, nullable, badge territorial)
- [ ] Criar repositórios
- [ ] Criar migrations

**Arquivos a Criar**:
- `backend/Araponga.Domain/Gamification/Contribution.cs`
- `backend/Araponga.Domain/Gamification/ContributionType.cs`
- `backend/Araponga.Domain/Gamification/UserContributionStats.cs`
- `backend/Araponga.Domain/Gamification/Badge.cs`
- `backend/Araponga.Domain/Gamification/UserBadge.cs`
- `backend/Araponga.Application/Interfaces/IContributionRepository.cs`
- `backend/Araponga.Application/Interfaces/IUserContributionStatsRepository.cs`
- `backend/Araponga.Application/Interfaces/IBadgeRepository.cs`
- `backend/Araponga.Application/Interfaces/IUserBadgeRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresContributionRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresUserContributionStatsRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresBadgeRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresUserBadgeRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/Migrations/YYYYMMDDHHMMSS_AddGamificationSystem.cs`

**Critérios de Sucesso**:
- ✅ Modelos criados
- ✅ Repositórios implementados
- ✅ Migrations aplicadas

---

#### 26.2 Serviço de Contribuições
**Estimativa**: 20 horas (2.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `ContributionService`:
  - [ ] `RecordContributionAsync(Guid userId, Guid territoryId, ContributionType type, Guid? relatedEntityId, string? relatedEntityType, CancellationToken)` → registrar contribuição
  - [ ] `CalculatePointsAsync(ContributionType type, Guid territoryId, decimal? interestAlignment, decimal? qualityScore)` → calcular pontos
  - [ ] `GetUserStatsAsync(Guid userId, Guid territoryId, CancellationToken)` → obter estatísticas
  - [ ] `GetUserContributionsAsync(Guid userId, Guid territoryId, int? limit, CancellationToken)` → listar contribuições
  - [ ] `GetUserLevelAsync(int totalPoints)` → calcular nível
- [ ] Lógica de cálculo de pontos:
  - [ ] Base: pontos por tipo de contribuição
  - [ ] Multiplicador de interesse: se alinhado aos interesses do território → +50%
  - [ ] Multiplicador de qualidade: se qualidade alta (IA) → +25%
  - [ ] Exemplo:
    - Post genérico: 5 pontos
    - Post alinhado a interesse: 7.5 pontos (5 * 1.5)
    - Post alinhado + alta qualidade: 9.4 pontos (5 * 1.5 * 1.25)
- [ ] Integração com `InterestFilterService` (Fase 14):
  - [ ] Calcular alinhamento com interesses do território
- [ ] Integração com `AIService` (Fase 15, opcional):
  - [ ] Avaliar qualidade do conteúdo (opcional, não bloqueante)
- [ ] Atualizar `UserContributionStats` automaticamente:
  - [ ] Ao registrar contribuição: atualizar stats
  - [ ] Calcular nível baseado em pontos
- [ ] Validações:
  - [ ] Usuário deve ser resident do território
  - [ ] Não registrar contribuições duplicadas (mesmo tipo + mesma entidade)
- [ ] Testes unitários

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/ContributionService.cs`
- `backend/Araponga.Application/Models/ContributionPointsConfig.cs` (configuração de pontos)
- `backend/Araponga.Tests/Application/ContributionServiceTests.cs`

**Arquivos a Modificar**:
- `backend/Araponga.Application/Services/PostCreationService.cs` (registrar contribuição ao criar post)
- `backend/Araponga.Application/Services/EventsService.cs` (registrar ao criar evento)
- `backend/Araponga.Application/Services/MapEntityService.cs` (registrar ao criar/confirmar entidade)
- `backend/Araponga.Application/Services/StoreItemService.cs` (registrar ao criar item)
- `backend/Araponga.Application/Services/DeliveryService.cs` (registrar ao completar entrega)
- `backend/Araponga.Application/Services/VotingService.cs` (registrar ao participar/criar votação)

**Critérios de Sucesso**:
- ✅ Serviço implementado
- ✅ Cálculo de pontos funcionando
- ✅ Integração com interesses funcionando
- ✅ Stats atualizados automaticamente
- ✅ Testes passando

---

#### 26.3 Sistema de Badges
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `BadgeService`:
  - [ ] `CheckAndAwardBadgesAsync(Guid userId, Guid territoryId, CancellationToken)` → verificar e conceder badges
  - [ ] `GetUserBadgesAsync(Guid userId, Guid? territoryId, CancellationToken)` → listar badges do usuário
  - [ ] `GetBadgeCriteriaAsync(Guid badgeId, CancellationToken)` → obter critérios
- [ ] Badges padrão:
  - [ ] **Community Builder**: 10 posts criados
  - [ ] **Event Organizer**: 5 eventos criados
  - [ ] **Map Explorer**: 10 MapEntities criados
  - [ ] **Map Validator**: 20 MapEntities confirmados
  - [ ] **Marketplace Seller**: 10 vendas
  - [ ] **Marketplace Buyer**: 10 compras
  - [ ] **Delivery Hero**: 20 entregas completadas
  - [ ] **Active Voter**: 10 participações em votações
  - [ ] **Community Leader**: 5 votações criadas
  - [ ] **Moderator**: 50 ações de moderação
  - [ ] **Territory Guardian**: 10 reports úteis
  - [ ] **Level 5 Contributor**: Alcançar nível 5
  - [ ] **Level 10 Contributor**: Alcançar nível 10
  - [ ] **Level 20 Contributor**: Alcançar nível 20
- [ ] Critérios configuráveis (JSON):
  - [ ] Tipo de contribuição
  - [ ] Quantidade mínima
  - [ ] Período (opcional, ex: "nos últimos 30 dias")
  - [ ] Território específico (opcional)
- [ ] Verificação automática:
  - [ ] Ao registrar contribuição: verificar badges
  - [ ] Background job (opcional): verificar badges periodicamente
- [ ] Notificações discretas:
  - [ ] Notificar quando badge é conquistado (não spam)
- [ ] Testes

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/BadgeService.cs`
- `backend/Araponga.Application/Models/BadgeCriteria.cs`
- `backend/Araponga.Infrastructure/Postgres/SeedData/BadgesSeedData.cs` (badges padrão)
- `backend/Araponga.Tests/Application/BadgeServiceTests.cs`

**Arquivos a Modificar**:
- `backend/Araponga.Application/Services/ContributionService.cs` (chamar verificação de badges)

**Critérios de Sucesso**:
- ✅ Badges padrão criados
- ✅ Verificação automática funcionando
- ✅ Notificações funcionando
- ✅ Testes passando

---

### Semana 27: Integração e Personalização Territorial

#### 27.1 Integração com Interesses do Território
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Integração com `InterestFilterService` (Fase 14):
  - [ ] Ao registrar contribuição: calcular alinhamento com interesses
  - [ ] Usar interesses do território para multiplicar pontos
- [ ] Lógica de alinhamento:
  - [ ] Se contribuição está relacionada a interesse do território → alinhamento = 1.0
  - [ ] Se parcialmente relacionada → alinhamento = 0.5
  - [ ] Se não relacionada → alinhamento = 0.0
  - [ ] Multiplicador: 1.0 + (alinhamento * 0.5) → máximo 1.5x
- [ ] Exemplos:
  - [ ] Território tem interesse "Sustentabilidade"
  - [ ] Post sobre reciclagem → alinhamento 1.0 → +50% pontos
  - [ ] Post sobre tecnologia → alinhamento 0.0 → pontos normais
- [ ] Badges territoriais (opcional):
  - [ ] Badges específicos por interesse do território
  - [ ] Exemplo: "Sustentabilidade Champion" (10 posts sobre sustentabilidade)
- [ ] Testes

**Arquivos a Modificar**:
- `backend/Araponga.Application/Services/ContributionService.cs`
- `backend/Araponga.Application/Services/InterestFilterService.cs` (se necessário)

**Critérios de Sucesso**:
- ✅ Alinhamento com interesses funcionando
- ✅ Multiplicador de pontos aplicado
- ✅ Testes passando

---

#### 27.2 Integração com IA (Opcional)
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ❌ Não implementado (opcional)

**Tarefas**:
- [ ] Integração com `AIService` (Fase 15, opcional):
  - [ ] Avaliar qualidade do conteúdo (posts, eventos)
  - [ ] Score de qualidade: 0.0 - 1.0
  - [ ] Multiplicador: 1.0 + (qualityScore * 0.25) → máximo 1.25x
- [ ] Critérios de qualidade (IA):
  - [ ] Relevância territorial
  - [ ] Originalidade
  - [ ] Valor informativo
  - [ ] Clareza
- [ ] Cache de avaliações:
  - [ ] Avaliar apenas uma vez por conteúdo
  - [ ] Cache por 7 dias
- [ ] Não bloqueante:
  - [ ] Se IA não disponível: usar pontos base (sem multiplicador)
  - [ ] Não atrasar registro de contribuição
- [ ] Testes

**Arquivos a Modificar**:
- `backend/Araponga.Application/Services/ContributionService.cs`
- `backend/Araponga.Application/Services/AIService.cs` (adicionar método de avaliação de qualidade)

**Critérios de Sucesso**:
- ✅ Integração com IA funcionando (se disponível)
- ✅ Cache funcionando
- ✅ Não bloqueante
- ✅ Testes passando

---

#### 27.3 Níveis e Progressão
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Sistema de níveis:
  - [ ] Fórmula: `Level = floor(sqrt(TotalPoints / 100)) + 1`
  - [ ] Exemplo:
    - 0-99 pontos → Nível 1
    - 100-399 pontos → Nível 2
    - 400-899 pontos → Nível 3
    - 900-1599 pontos → Nível 4
    - 1600-2499 pontos → Nível 5
    - ... (progressão exponencial suave)
- [ ] Visualização:
  - [ ] Barra de progresso para próximo nível
  - [ ] Pontos necessários para próximo nível
  - [ ] Percentual de progresso
- [ ] Reconhecimento discreto:
  - [ ] Nível visível no perfil (não invasivo)
  - [ ] Badge de nível (opcional)
- [ ] Sem ranking público:
  - [ ] Usuário vê seu próprio nível
  - [ ] Não há ranking global ou territorial
  - [ ] Foco em progresso pessoal, não competição
- [ ] Testes

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/LevelService.cs`
- `backend/Araponga.Application/Models/LevelProgress.cs`
- `backend/Araponga.Tests/Application/LevelServiceTests.cs`

**Arquivos a Modificar**:
- `backend/Araponga.Application/Services/ContributionService.cs` (calcular nível ao atualizar stats)

**Critérios de Sucesso**:
- ✅ Sistema de níveis funcionando
- ✅ Progressão suave
- ✅ Visualização discreta
- ✅ Testes passando

---

#### 27.4 Controller e API
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `ContributionsController`:
  - [ ] `GET /api/v1/contributions/stats` (estatísticas do usuário)
  - [ ] `GET /api/v1/contributions` (histórico de contribuições)
  - [ ] `GET /api/v1/contributions/badges` (badges do usuário)
  - [ ] `GET /api/v1/contributions/level` (nível e progresso)
- [ ] Criar requests/responses
- [ ] Validação (FluentValidation)
- [ ] Integração com perfil:
  - [ ] Atualizar `UserProfileResponse` para incluir stats
- [ ] Testes de integração

**Arquivos a Criar**:
- `backend/Araponga.Api/Controllers/ContributionsController.cs`
- `backend/Araponga.Api/Contracts/Gamification/ContributionStatsResponse.cs`
- `backend/Araponga.Api/Contracts/Gamification/ContributionResponse.cs`
- `backend/Araponga.Api/Contracts/Gamification/BadgeResponse.cs`
- `backend/Araponga.Api/Contracts/Gamification/LevelProgressResponse.cs`
- `backend/Araponga.Tests/Integration/ContributionsIntegrationTests.cs`

**Arquivos a Modificar**:
- `backend/Araponga.Api/Contracts/Users/UserProfileResponse.cs` (adicionar stats)

**Critérios de Sucesso**:
- ✅ Endpoints funcionando
- ✅ Integração com perfil funcionando
- ✅ Testes passando

---

### Semana 28: Visualização e Finalização

#### 28.1 Visualização Suave no Frontend
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Componente de estatísticas no perfil:
  - [ ] Nível atual
  - [ ] Pontos totais
  - [ ] Barra de progresso para próximo nível
  - [ ] Badges conquistados (grid discreto)
- [ ] Componente de histórico de contribuições:
  - [ ] Lista de contribuições recentes
  - [ ] Tipo de contribuição
  - [ ] Pontos ganhos
  - [ ] Data
- [ ] Notificações discretas:
  - [ ] Badge conquistado (toast suave)
  - [ ] Nível alcançado (toast suave)
  - [ ] Não spam (máximo 1 notificação por dia)
- [ ] Design harmonioso:
  - [ ] Não compete com conteúdo principal
  - [ ] Cores suaves
  - [ ] Ícones discretos
  - [ ] Não invasivo
- [ ] Testes E2E

**Arquivos a Criar**:
- `frontend/portal/components/gamification/ContributionStats.tsx`
- `frontend/portal/components/gamification/BadgeGrid.tsx`
- `frontend/portal/components/gamification/ContributionHistory.tsx`
- `frontend/portal/components/gamification/LevelProgress.tsx`

**Arquivos a Modificar**:
- `frontend/portal/pages/Profile.tsx` (adicionar seção de gamificação)

**Critérios de Sucesso**:
- ✅ Componentes criados
- ✅ Visualização suave e harmoniosa
- ✅ Testes E2E passando

---

#### 28.2 Configuração de Pontos por Território
**Estimativa**: 8 horas (1 dia)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `TerritoryGamificationConfig`:
  - [ ] `TerritoryId`
  - [ ] `PointsConfig` (JSON, configuração de pontos por tipo)
  - [ ] `MultiplierConfig` (JSON, multiplicadores de interesse/qualidade)
  - [ ] `BadgesEnabled` (bool)
  - [ ] `LevelsEnabled` (bool)
- [ ] Configuração padrão:
  - [ ] Todos os territórios começam com configuração padrão
  - [ ] Curadores podem personalizar (opcional)
- [ ] Validação:
  - [ ] Pontos devem ser >= 0
  - [ ] Multiplicadores devem ser >= 1.0
- [ ] Testes

**Arquivos a Criar**:
- `backend/Araponga.Domain/Gamification/TerritoryGamificationConfig.cs`
- `backend/Araponga.Application/Interfaces/ITerritoryGamificationConfigRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresTerritoryGamificationConfigRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/Migrations/YYYYMMDDHHMMSS_AddTerritoryGamificationConfig.cs`

**Arquivos a Modificar**:
- `backend/Araponga.Application/Services/ContributionService.cs` (usar configuração do território)

**Critérios de Sucesso**:
- ✅ Configuração por território funcionando
- ✅ Personalização por curadores funcionando
- ✅ Testes passando

---

#### 28.3 Testes e Documentação
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Testes de integração completos:
  - [ ] Registro de contribuições
  - [ ] Cálculo de pontos com interesses
  - [ ] Sistema de badges
  - [ ] Sistema de níveis
  - [ ] Integração com serviços existentes
- [ ] Testes de performance:
  - [ ] Registro de contribuições em lote
  - [ ] Cálculo de alinhamento com interesses
- [ ] Testes de segurança:
  - [ ] Apenas residents podem ganhar pontos
  - [ ] Validação de ownership
- [ ] Documentação técnica:
  - [ ] `docs/GAMIFICATION_SYSTEM.md`
  - [ ] Como funciona o sistema
  - [ ] Como calcular pontos
  - [ ] Como personalizar por território
- [ ] Atualizar `docs/CHANGELOG.md`
- [ ] Atualizar Swagger

**Arquivos a Criar**:
- `backend/Araponga.Tests/Integration/GamificationCompleteIntegrationTests.cs`
- `docs/GAMIFICATION_SYSTEM.md`

**Critérios de Sucesso**:
- ✅ Testes passando
- ✅ Cobertura >85%
- ✅ Documentação completa

---

## 📊 Resumo da Fase 17

| Tarefa | Estimativa | Status | Prioridade |
|--------|------------|--------|------------|
| Modelo de Domínio - Contribuições | 16h | ❌ Pendente | 🔴 Crítica |
| Serviço de Contribuições | 20h | ❌ Pendente | 🔴 Crítica |
| Sistema de Badges | 12h | ❌ Pendente | 🔴 Crítica |
| Integração com Interesses | 16h | ❌ Pendente | 🔴 Crítica |
| Integração com IA (Opcional) | 12h | ❌ Pendente | 🟡 Importante |
| Níveis e Progressão | 12h | ❌ Pendente | 🔴 Crítica |
| Controller e API | 12h | ❌ Pendente | 🔴 Crítica |
| Visualização Suave | 16h | ❌ Pendente | 🟡 Importante |
| Configuração por Território | 8h | ❌ Pendente | 🟡 Importante |
| Testes e Documentação | 16h | ❌ Pendente | 🟡 Importante |
| **Total** | **120h (21 dias)** | | |

---

## ✅ Critérios de Sucesso da Fase 17

### Funcionalidades
- ✅ Sistema de contribuições funcionando
- ✅ Cálculo de pontos com interesses funcionando
- ✅ Sistema de badges funcionando
- ✅ Sistema de níveis funcionando
- ✅ Visualização suave e harmoniosa
- ✅ Integração com serviços existentes funcionando

### Qualidade
- ✅ Cobertura de testes >85%
- ✅ Testes de integração passando
- ✅ Performance adequada
- ✅ Segurança validada

### Ética e Harmonia
- ✅ Não manipula comportamento
- ✅ Foca em contribuição real
- ✅ Transparente para o usuário
- ✅ Não compete com propósito principal

### Documentação
- ✅ Documentação técnica completa
- ✅ Changelog atualizado
- ✅ Swagger atualizado

---

## 🔗 Dependências

- **Fase 9**: Perfil de Usuário (estatísticas)
- **Fase 14**: Governança e Interesses (alinhamento com interesses)
- **Opcional**: Fase 15 (IA) - Avaliação de qualidade

---

## 📝 Notas de Implementação

### Princípios de Gamificação Harmoniosa

**Contribuição Real**:
- ✅ Pontos baseados em ações que agregam valor
- ✅ Qualidade > Quantidade
- ✅ Não recompensa "engajamento vazio"

**Contexto Territorial**:
- ✅ Gamificação adaptada aos interesses do território
- ✅ Ações alinhadas ganham mais pontos
- ✅ Badges territoriais (opcional)

**Harmonia**:
- ✅ Não compete com propósito principal (união comunitária)
- ✅ Visualização suave e discreta
- ✅ Não manipula feed (cronológico mantido)

**Transparência**:
- ✅ Usuário entende como ganha pontos
- ✅ Histórico de contribuições visível
- ✅ Sem "surpresas" ou manipulação

### Sistema de Pontos

**Pontos Base por Tipo**:
- Post criado: 5 pontos
- Evento criado: 10 pontos
- Participação em evento: 3 pontos
- MapEntity criado: 5 pontos
- MapEntity confirmado: 2 pontos
- Item no marketplace: 5 pontos
- Venda no marketplace: 10 pontos
- Compra no marketplace: 5 pontos
- Entrega completada: 15 pontos
- Participação em votação: 3 pontos
- Votação criada: 10 pontos
- Ação de moderação: 5 pontos
- Report útil: 3 pontos
- **Coleta de resíduos: 10-20 pontos** 🔴 NOVO (depende do volume/tipo)
- **Plantio de árvore: 15-25 pontos** 🔴 NOVO (depende do tipo/espécie)
- **Mutirão: 20-30 pontos** 🔴 NOVO (depende da duração/impacto)
- **Observação de saúde: 5-15 pontos** 🔴 NOVO (depende da severidade)
- **Monitoramento (sensor): 3-10 pontos** 🔴 NOVO (depende da frequência)
- **Manutenção de recurso: 10-20 pontos** 🔴 NOVO (depende do tipo)

**Multiplicadores**:
- Alinhamento com interesses: +50% (máx. 1.5x)
- Qualidade alta (IA): +25% (máx. 1.25x)
- Combinado: até 1.875x (1.5 * 1.25)

### Sistema de Níveis

**Fórmula**:
```
Level = floor(sqrt(TotalPoints / 100)) + 1
```

**Progressão**:
- Nível 1: 0-99 pontos
- Nível 2: 100-399 pontos
- Nível 3: 400-899 pontos
- Nível 4: 900-1599 pontos
- Nível 5: 1600-2499 pontos
- Nível 10: 8100-9999 pontos
- Nível 20: 36100-39999 pontos

**Características**:
- Progressão exponencial suave
- Sem "grind" excessivo
- Reconhecimento por contribuição real

### Badges Padrão

**Comunidade**:
- Community Builder (10 posts)
- Event Organizer (5 eventos)
- Active Voter (10 votações)
- Community Leader (5 votações criadas)

**Mapa**:
- Map Explorer (10 MapEntities criados)
- Map Validator (20 MapEntities confirmados)

**Marketplace**:
- Marketplace Seller (10 vendas)
- Marketplace Buyer (10 compras)

**Entregas**:
- Delivery Hero (20 entregas)

**Moderação**:
- Moderator (50 ações)
- Territory Guardian (10 reports úteis)

**Níveis**:
- Level 5/10/20 Contributor

**Atividades Territoriais** 🔴 NOVO:
- Waste Collector (10 coletas)
- Tree Planter (10 plantios)
- Territory Guardian (5 mutirões)
- Health Monitor (20 observações)
- Sensor Keeper (50 leituras confirmadas)
- Natural Asset Keeper (10 manutenções)

### Integração com Serviços Existentes

**Hooks Automáticos**:
- `PostCreationService`: Registrar contribuição ao criar post
- `EventsService`: Registrar ao criar evento e participar
- `MapEntityService`: Registrar ao criar/confirmar entidade
- `StoreItemService`: Registrar ao criar item
- `CartService`: Registrar ao vender/comprar
- `DeliveryService`: Registrar ao completar entrega
- `VotingService`: Registrar ao participar/criar votação
- `ReportService`: Registrar ao submeter report útil
- `ModerationService`: Registrar ao moderar (curadores)
- **`TerritoryHealthService`**: Registrar ao criar observação de saúde 🔴 NOVO
- **`TerritoryActionService`**: Registrar ao participar/criar mutirão 🔴 NOVO
- **`WasteCollectionService`**: Registrar ao reportar coleta de resíduos 🔴 NOVO
- **`TreePlantingService`**: Registrar ao reportar plantio 🔴 NOVO
- **`NaturalAssetService`**: Registrar ao fazer manutenção 🔴 NOVO

**Não Invasivo**:
- Registro assíncrono (não bloqueia operação principal)
- Falhas não afetam funcionalidade principal
- Logging para debugging

---

**Status**: ⏳ **FASE 17 PENDENTE**  
**Depende de**: Fases 9, 14 (Perfil, Governança)  
**Crítico para**: Engajamento Comunitário Sustentável
