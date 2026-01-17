# Fase 15: Inteligência Artificial

**Duração**: 4 semanas (28 dias úteis)  
**Prioridade**: 🔴 ALTA (Moderação e busca inteligente)  
**Depende de**: Nenhuma (pode ser feito em paralelo)  
**Estimativa Total**: 208 horas  
**Status**: ⏳ Pendente

---

## 🎯 Objetivo

Implementar funcionalidades de **Inteligência Artificial** para:
- Moderação automática (detecção de conteúdo inadequado)
- Busca inteligente (semântica, não apenas palavras-chave)
- Categorização automática de conteúdo
- Recomendações contextuais (sem manipular feed)
- Análise de conteúdo

**Princípios de IA no Araponga**:
- ✅ **Transparência**: Usuário sabe quando IA está sendo usada
- ✅ **Privacidade**: Dados não são compartilhados sem consentimento
- ✅ **Não Manipulação**: Feed cronológico permanece, IA apenas auxilia
- ✅ **Controle do Usuário**: Pode desabilitar funcionalidades de IA
- ✅ **Ética**: IA serve à comunidade, não à extração de dados

---

## 📋 Contexto e Requisitos

### Estado Atual
- ✅ Moderação básica (reports, bloqueios, thresholds)
- ✅ Busca básica (texto simples)
- ❌ Não existe moderação automática
- ❌ Não existe busca semântica
- ❌ Não existe categorização automática

### Requisitos Funcionais

#### 1. Moderação Automática
- ✅ Detecção de conteúdo inadequado (texto, imagens)
- ✅ Análise de sentimento (detectar toxicidade)
- ✅ Categorização automática de reports
- ✅ Sugestões para moderadores humanos
- ✅ Integração com sistema de moderação existente

#### 2. Busca Inteligente
- ✅ Busca semântica (não apenas palavras-chave)
- ✅ Sugestões de busca
- ✅ Correção automática de erros de digitação
- ✅ Busca em posts, eventos, marketplace, usuários

#### 3. Categorização Automática
- ✅ Categorizar posts automaticamente
- ✅ Sugerir tags relevantes
- ✅ Identificar tópicos principais
- ✅ Integração com sistema de interesses (Fase 14)

#### 4. Recomendações Contextuais
- ✅ Sugerir territórios próximos relevantes
- ✅ Sugerir eventos baseados em interesses (opcional)
- ✅ Sugerir itens do marketplace relevantes
- ⚠️ **Importante**: Feed cronológico permanece, sugestões são opcionais

#### 5. Tradução Automática (Opcional)
- ✅ Traduzir posts para idioma do usuário
- ✅ Manter original disponível

#### 6. Análise de Conteúdo
- ✅ Extrair informações de posts (localização, eventos)
- ✅ Identificar entidades (pessoas, lugares, organizações)

#### 7. Classificação Inteligente de Conteúdo Gerado por IA 🔴 NOVO
- ✅ Detecção automática de conteúdo gerado por IA (texto, imagens)
- ✅ Classificação de probabilidade (0-100% de ser gerado por IA)
- ✅ Marcação transparente de conteúdo gerado por IA
- ✅ Opções de filtro para usuários (mostrar/ocultar conteúdo gerado por IA)
- ✅ Integração com sistema de moderação (alertas para moderadores)
- ✅ Estatísticas de conteúdo gerado por IA por território
- ✅ Respeito à privacidade (não expor dados sensíveis)

#### 8. Inteligência de Relevância de Publicações 🔴 NOVO
- ✅ Cálculo de score de relevância para posts (0-100)
- ✅ Fatores de relevância:
  - Alinhamento com interesses do território (Fase 14)
  - Proximidade geográfica (GeoAnchors)
  - Qualidade do conteúdo (IA)
  - Interações comunitárias (likes, shares, comentários)
  - Recência (posts recentes têm boost)
  - Autoridade do autor (contribuições anteriores)
- ✅ Score armazenado mas **não usado para ordenar feed cronológico**
- ✅ Opção opcional de visualização por relevância (separada do feed cronológico)
- ✅ Badge discreto para posts altamente relevantes
- ✅ API para obter posts mais relevantes (endpoint separado)
- ✅ Transparência: usuário sabe quando está vendo por relevância
- ⚠️ **Princípio**: Feed cronológico permanece como padrão, relevância é opcional

---

## 📋 Tarefas Detalhadas

### Semana 18: Infraestrutura de IA

#### 18.1 Interface e Abstração
**Estimativa**: 8 horas (1 dia)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar interface `IAIService`:
  - [ ] `ModerateContentAsync(string text, CancellationToken)` → `ModerationResult`
  - [ ] `SearchSemanticAsync(string query, SearchOptions, CancellationToken)` → `SearchResults`
  - [ ] `CategorizeAsync(string text, CancellationToken)` → `CategorizationResult`
  - [ ] `TranslateAsync(string text, string targetLanguage, CancellationToken)` → `string`
  - [ ] `ExtractEntitiesAsync(string text, CancellationToken)` → `EntityExtractionResult`
  - [ ] `DetectAIGeneratedContentAsync(string text, string? imageUrl, CancellationToken)` → `AIGeneratedContentResult`
  - [ ] `CalculateRelevanceScoreAsync(Post post, Guid userId, Guid territoryId, CancellationToken)` → `RelevanceScoreResult`
- [ ] Criar modelos de resultado:
  - [ ] `ModerationResult` (toxicidade, categorias, confiança)
  - [ ] `SearchResults` (resultados semânticos)
  - [ ] `CategorizationResult` (categorias, tags, tópicos)
  - [ ] `EntityExtractionResult` (pessoas, lugares, organizações)
  - [ ] `AIGeneratedContentResult` (probabilidade, confiança, tipo de IA detectado)
  - [ ] `RelevanceScoreResult` (score 0-100, fatores, confiança)

**Arquivos a Criar**:
- `backend/Araponga.Application/Interfaces/IAIService.cs`
- `backend/Araponga.Application/Models/AI/ModerationResult.cs`
- `backend/Araponga.Application/Models/AI/SearchResults.cs`
- `backend/Araponga.Application/Models/AI/CategorizationResult.cs`
- `backend/Araponga.Application/Models/AI/EntityExtractionResult.cs`
- `backend/Araponga.Application/Models/AI/AIGeneratedContentResult.cs`
- `backend/Araponga.Application/Models/AI/RelevanceScoreResult.cs`

**Critérios de Sucesso**:
- ✅ Interface criada
- ✅ Modelos criados
- ✅ Abstração clara e extensível

---

#### 18.2 Implementação OpenAI
**Estimativa**: 20 horas (2.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Instalar pacote `OpenAI` (ou similar)
- [ ] Criar `OpenAIService`:
  - [ ] Implementar `IAIService`
  - [ ] Configuração via `IConfiguration`:
    - [ ] `AI:OpenAI:ApiKey` (secret)
    - [ ] `AI:OpenAI:Model` (gpt-4, gpt-3.5-turbo, etc.)
  - [ ] Moderação: usar `Moderation API`
  - [ ] Busca: usar `Embeddings API` + busca vetorial
  - [ ] Categorização: usar `Chat Completion API`
  - [ ] Tradução: usar `Chat Completion API`
  - [ ] Extração de entidades: usar `Chat Completion API`
- [ ] Tratamento de erros (rate limits, timeouts)
- [ ] Retry policy
- [ ] Logging (sem expor dados sensíveis)

**Arquivos a Criar**:
- `backend/Araponga.Infrastructure/AI/OpenAIService.cs`
- `backend/Araponga.Infrastructure/AI/OpenAIConfiguration.cs`

**Arquivos a Modificar**:
- `backend/Araponga.Api/Program.cs` (registrar serviço)

**Critérios de Sucesso**:
- ✅ Integração OpenAI funcionando
- ✅ Todas as funcionalidades implementadas
- ✅ Tratamento de erros funcionando

---

#### 18.3 Implementação Azure (Opcional)
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado  
**Prioridade**: 🟢 Opcional (alternativa ao OpenAI)

**Tarefas**:
- [ ] Instalar pacote `Azure.AI.TextAnalytics`
- [ ] Criar `AzureAIService`:
  - [ ] Implementar `IAIService`
  - [ ] Configuração via `IConfiguration`:
    - [ ] `AI:Azure:Endpoint`
    - [ ] `AI:Azure:ApiKey`
  - [ ] Moderação: usar `Content Moderator`
  - [ ] Busca: usar `Cognitive Search`
  - [ ] Categorização: usar `Text Analytics`
  - [ ] Tradução: usar `Translator`
  - [ ] Extração de entidades: usar `Text Analytics`

**Arquivos a Criar**:
- `backend/Araponga.Infrastructure/AI/AzureAIService.cs`

**Critérios de Sucesso**:
- ✅ Integração Azure funcionando
- ✅ Todas as funcionalidades implementadas

---

### Semana 19: Moderação Automática

#### 19.1 Serviço de Moderação Automática
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `ModerationService`:
  - [ ] `ModeratePostAsync(Post post)` → `ModerationResult`
  - [ ] `ModerateImageAsync(Stream imageStream)` → `ModerationResult`
  - [ ] `CategorizeReportAsync(Report report)` → `ReportCategory`
- [ ] Integração com `PostCreationService`:
  - [ ] Moderar post antes de criar (se feature flag ativo)
  - [ ] Se conteúdo inadequado: retornar erro ou marcar para revisão
- [ ] Integração com `ReportService`:
  - [ ] Categorizar report automaticamente
  - [ ] Sugerir ação para moderador
- [ ] Feature flag: `AIModerationEnabled` (por território)
- [ ] Testes unitários

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/ModerationService.cs`
- `backend/Araponga.Tests/Application/ModerationServiceTests.cs`

**Arquivos a Modificar**:
- `backend/Araponga.Application/Services/PostCreationService.cs`
- `backend/Araponga.Application/Services/ReportService.cs`

**Critérios de Sucesso**:
- ✅ Moderação automática funcionando
- ✅ Integração com criação de posts funcionando
- ✅ Categorização de reports funcionando
- ✅ Testes passando

---

#### 19.2 Integração com Sistema de Moderação
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Atualizar `ModerationCase`:
  - [ ] Adicionar campo `AICategory` (categoria sugerida por IA)
  - [ ] Adicionar campo `AIConfidence` (confiança da IA)
  - [ ] Adicionar campo `AISuggestedAction` (ação sugerida)
- [ ] Atualizar `ModerationCaseService`:
  - [ ] Incluir sugestões de IA ao criar caso
  - [ ] Moderador pode aceitar/rejeitar sugestão
- [ ] Atualizar `ModerationController`:
  - [ ] Incluir sugestões de IA na resposta
- [ ] Testes de integração

**Arquivos a Modificar**:
- `backend/Araponga.Domain/Moderation/ModerationCase.cs`
- `backend/Araponga.Application/Services/ModerationCaseService.cs`
- `backend/Araponga.Api/Controllers/ModerationController.cs`

**Critérios de Sucesso**:
- ✅ Sugestões de IA aparecendo
- ✅ Moderadores podem usar sugestões
- ✅ Testes passando

---

### Semana 20: Busca Inteligente

#### 20.1 Serviço de Busca Semântica
**Estimativa**: 20 horas (2.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `SearchService`:
  - [ ] `SearchPostsAsync(string query, SearchOptions)` → busca semântica em posts
  - [ ] `SearchEventsAsync(string query, SearchOptions)` → busca semântica em eventos
  - [ ] `SearchMarketplaceAsync(string query, SearchOptions)` → busca semântica em marketplace
  - [ ] `SuggestSearchAsync(string partialQuery)` → sugestões de busca
- [ ] Implementar busca vetorial:
  - [ ] Gerar embeddings de conteúdo (posts, eventos, itens)
  - [ ] Armazenar embeddings (PostgreSQL com pgvector ou banco separado)
  - [ ] Buscar por similaridade de embeddings
- [ ] Correção automática de erros:
  - [ ] Detectar erros de digitação
  - [ ] Sugerir correções
- [ ] Feature flag: `AISearchEnabled` (por território)
- [ ] Testes unitários

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/SearchService.cs`
- `backend/Araponga.Application/Interfaces/IEmbeddingRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresEmbeddingRepository.cs`
- `backend/Araponga.Tests/Application/SearchServiceTests.cs`

**Arquivos a Modificar**:
- `backend/Araponga.Infrastructure/Postgres/ArapongaDbContext.cs` (adicionar tabela de embeddings)
- `backend/Araponga.Infrastructure/Postgres/Migrations/YYYYMMDDHHMMSS_AddEmbeddings.cs`

**Critérios de Sucesso**:
- ✅ Busca semântica funcionando
- ✅ Sugestões funcionando
- ✅ Correção de erros funcionando
- ✅ Testes passando

---

#### 20.2 Integração com Controllers
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Atualizar `FeedController`:
  - [ ] Adicionar endpoint `GET /api/v1/feed/search?q={query}` (busca semântica)
  - [ ] Adicionar endpoint `GET /api/v1/feed/search/suggestions?q={partial}` (sugestões)
- [ ] Atualizar `EventsController`:
  - [ ] Adicionar endpoint `GET /api/v1/events/search?q={query}`
- [ ] Atualizar `ItemsController`:
  - [ ] Atualizar endpoint `GET /api/v1/items/search` para usar busca semântica (se feature flag ativo)
- [ ] Testes de integração

**Arquivos a Modificar**:
- `backend/Araponga.Api/Controllers/FeedController.cs`
- `backend/Araponga.Api/Controllers/EventsController.cs`
- `backend/Araponga.Api/Controllers/ItemsController.cs`

**Critérios de Sucesso**:
- ✅ Endpoints de busca funcionando
- ✅ Sugestões funcionando
- ✅ Testes passando

---

### Semana 21: Categorização e Recomendações

#### 21.1 Categorização Automática
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `CategorizationService`:
  - [ ] `CategorizePostAsync(Post post)` → `CategorizationResult`
  - [ ] `SuggestTagsAsync(Post post)` → `IReadOnlyList<string>`
  - [ ] `IdentifyTopicsAsync(Post post)` → `IReadOnlyList<string>`
- [ ] Integração com `PostCreationService`:
  - [ ] Categorizar post automaticamente ao criar (se feature flag ativo)
  - [ ] Sugerir tags (usuário pode aceitar/rejeitar)
- [ ] Integração com sistema de interesses (Fase 14):
  - [ ] Tags sugeridas podem virar interesses do usuário (opcional)
- [ ] Armazenar categorias/tags em `Post` (novos campos)
- [ ] Criar migration
- [ ] Testes

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/CategorizationService.cs`
- `backend/Araponga.Tests/Application/CategorizationServiceTests.cs`

**Arquivos a Modificar**:
- `backend/Araponga.Domain/Feed/CommunityPost.cs` (adicionar campos de categoria/tags)
- `backend/Araponga.Application/Services/PostCreationService.cs`
- `backend/Araponga.Infrastructure/Postgres/Migrations/YYYYMMDDHHMMSS_AddPostCategories.cs`

**Critérios de Sucesso**:
- ✅ Categorização funcionando
- ✅ Sugestão de tags funcionando
- ✅ Integração com criação de posts funcionando
- ✅ Testes passando

---

#### 21.2 Recomendações Contextuais
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `RecommendationService`:
  - [ ] `RecommendTerritoriesAsync(Guid userId, GeoCoordinate location)` → territórios relevantes
  - [ ] `RecommendEventsAsync(Guid userId, IReadOnlyList<string> interests)` → eventos relevantes
  - [ ] `RecommendMarketplaceItemsAsync(Guid userId, IReadOnlyList<string> interests)` → itens relevantes
- [ ] **Importante**: Recomendações são **opcionais** e **não manipulam feed**
- [ ] Endpoints opcionais:
  - [ ] `GET /api/v1/recommendations/territories`
  - [ ] `GET /api/v1/recommendations/events`
  - [ ] `GET /api/v1/recommendations/marketplace`
- [ ] Feature flag: `AIRecommendationsEnabled` (por território e por usuário)
- [ ] Preferências do usuário: pode desabilitar recomendações
- [ ] Testes

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/RecommendationService.cs`
- `backend/Araponga.Api/Controllers/RecommendationsController.cs`
- `backend/Araponga.Tests/Application/RecommendationServiceTests.cs`

**Critérios de Sucesso**:
- ✅ Recomendações funcionando
- ✅ Feed cronológico não é afetado
- ✅ Preferências respeitadas
- ✅ Testes passando

---

#### 21.3 Tradução Automática (Opcional)
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ❌ Não implementado  
**Prioridade**: 🟢 Opcional

**Tarefas**:
- [ ] Criar `TranslationService`:
  - [ ] `TranslatePostAsync(Post post, string targetLanguage)` → `Post` traduzido
  - [ ] `TranslateTextAsync(string text, string targetLanguage)` → `string`
- [ ] Integração com `FeedController`:
  - [ ] Query parameter `translateTo` (opcional)
  - [ ] Se fornecido: traduzir posts para idioma do usuário
  - [ ] Manter original disponível
- [ ] Detectar idioma do post automaticamente
- [ ] Feature flag: `AITranslationEnabled` (por território)
- [ ] Testes

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/TranslationService.cs`
- `backend/Araponga.Tests/Application/TranslationServiceTests.cs`

**Arquivos a Modificar**:
- `backend/Araponga.Api/Controllers/FeedController.cs`

**Critérios de Sucesso**:
- ✅ Tradução funcionando
- ✅ Original mantido
- ✅ Testes passando

---

#### 21.4 Análise de Conteúdo
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `ContentAnalysisService`:
  - [ ] `ExtractLocationAsync(Post post)` → localização mencionada
  - [ ] `ExtractEventsAsync(Post post)` → eventos mencionados
  - [ ] `ExtractEntitiesAsync(Post post)` → pessoas, lugares, organizações
- [ ] Integração com `PostCreationService`:
  - [ ] Extrair informações automaticamente (opcional)
  - [ ] Sugerir GeoAnchors baseados em localização mencionada
- [ ] Armazenar entidades extraídas (opcional, para busca futura)
- [ ] Testes

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/ContentAnalysisService.cs`
- `backend/Araponga.Tests/Application/ContentAnalysisServiceTests.cs`

**Arquivos a Modificar**:
- `backend/Araponga.Application/Services/PostCreationService.cs`

**Critérios de Sucesso**:
- ✅ Extração de informações funcionando
- ✅ Sugestões de GeoAnchors funcionando
- ✅ Testes passando

---

#### 21.5 Classificação Inteligente de Conteúdo Gerado por IA 🔴 NOVO
**Estimativa**: 24 horas (3 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `AIGeneratedContentDetectionService`:
  - [ ] `DetectAIGeneratedTextAsync(string text, CancellationToken)` → `AIGeneratedContentResult`
  - [ ] `DetectAIGeneratedImageAsync(string imageUrl, CancellationToken)` → `AIGeneratedContentResult`
  - [ ] `ClassifyAIGeneratedContentAsync(Post post, CancellationToken)` → `AIGeneratedContentResult`
- [ ] Integração com `IAIService`:
  - [ ] Usar modelos de detecção de IA (GPTZero, OpenAI Detector, ou modelo próprio)
  - [ ] Análise de padrões de texto (perplexidade, burstiness)
  - [ ] Análise de imagens (detecção de artefatos de IA, metadados)
  - [ ] Probabilidade de 0-100% de ser gerado por IA
  - [ ] Confiança da detecção (0-100%)
  - [ ] Tipo de IA detectado (ChatGPT, DALL-E, Midjourney, etc.)
- [ ] Modelo de domínio `AIGeneratedContentClassification`:
  - [ ] `Id`, `PostId`, `TerritoryId`
  - [ ] `IsAIGenerated` (bool, probabilidade > threshold)
  - [ ] `Probability` (decimal, 0-100%)
  - [ ] `Confidence` (decimal, 0-100%)
  - [ ] `AIType` (string?, tipo de IA detectado)
  - [ ] `DetectionMethod` (string, método usado)
  - [ ] `DetectedAtUtc` (DateTime)
  - [ ] `UpdatedAtUtc` (DateTime)
- [ ] Integração com `PostCreationService`:
  - [ ] Detectar automaticamente ao criar post (opcional, via feature flag)
  - [ ] Armazenar classificação no banco
  - [ ] Não bloquear criação (apenas classificar)
- [ ] Integração com `FeedService`:
  - [ ] Incluir flag `IsAIGenerated` em `FeedItemResponse`
  - [ ] Incluir flag `IsBlockedByAIContent` em `FeedItemResponse` (se bloqueado)
  - [ ] Aplicar bloqueio automaticamente baseado em feature flags
  - [ ] Opção de filtro manual: mostrar/ocultar conteúdo gerado por IA (além do bloqueio automático)
- [ ] Integração com `ModerationService`:
  - [ ] Alertar moderadores se conteúdo gerado por IA + probabilidade alta
  - [ ] Se `BlockingMode = Block` no território → bloquear automaticamente
  - [ ] Se `BlockingMode = Warn` → apenas alertar, não bloquear
  - [ ] Se `BlockingMode = None` → apenas classificar, não bloquear
  - [ ] Dashboard de moderação: mostrar estatísticas de conteúdo bloqueado por IA
- [ ] Feature flags territoriais:
  - [ ] `AIGeneratedContentDetectionEnabled` (habilitar/desabilitar detecção no território)
  - [ ] `AIGeneratedContentBlockingEnabled` (habilitar/desabilitar bloqueio no território)
  - [ ] `AIGeneratedContentBlockingThreshold` (decimal, 0-100, threshold para bloquear - padrão: 80%)
  - [ ] `AIGeneratedContentBlockingMode` (enum: `None`, `Warn`, `Block`) - modo de bloqueio:
    - [ ] `None`: Apenas detecta e marca (não bloqueia)
    - [ ] `Warn`: Marca e alerta, mas permite visualização
    - [ ] `Block`: Bloqueia completamente do feed (oculta)
- [ ] Feature flags por usuário (preferências):
  - [ ] `BlockAIGeneratedContent` (bool, bloquear conteúdo gerado por IA)
  - [ ] `AIGeneratedContentBlockingThreshold` (decimal, 0-100, threshold pessoal - padrão: 80%)
  - [ ] `ShowAIGeneratedContent` (bool, mostrar conteúdo gerado por IA - oposto de bloqueio)
  - [ ] Adicionar em `UserPreferences`
- [ ] Lógica de bloqueio (respeita ambos os níveis):
  - [ ] Se território bloqueia (`BlockingMode = Block`) → conteúdo é oculto do feed
  - [ ] Se usuário bloqueia (`BlockAIGeneratedContent = true`) → conteúdo é oculto do feed do usuário
  - [ ] Se ambos permitem mas probabilidade > threshold → apenas marca (não bloqueia)
  - [ ] Prioridade: Bloqueio territorial > Bloqueio do usuário > Permissão
- [ ] Integração com `FeedService`:
  - [ ] Aplicar filtro de bloqueio ao listar feed
  - [ ] Respeitar feature flags territoriais e preferências do usuário
  - [ ] Logging de bloqueios (para auditoria)
- [ ] Estatísticas:
  - [ ] `GetAIGeneratedContentStatsAsync(Guid territoryId)` → estatísticas por território
  - [ ] Percentual de conteúdo gerado por IA
  - [ ] Tendências ao longo do tempo
- [ ] Transparência:
  - [ ] Badge visual discreto em posts gerados por IA (se probabilidade > 80%)
  - [ ] Tooltip explicativo: "Conteúdo pode ter sido gerado por IA"
  - [ ] Link para política de transparência
- [ ] Testes:
  - [ ] Testes unitários (diferentes tipos de conteúdo)
  - [ ] Testes de integração
  - [ ] Testes de performance (latência)

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/AIGeneratedContentDetectionService.cs`
- `backend/Araponga.Domain/AI/AIGeneratedContentClassification.cs`
- `backend/Araponga.Application/Interfaces/IAIGeneratedContentClassificationRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresAIGeneratedContentClassificationRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/Migrations/YYYYMMDDHHMMSS_AddAIGeneratedContentClassification.cs`
- `backend/Araponga.Tests/Application/AIGeneratedContentDetectionServiceTests.cs`

**Arquivos a Modificar**:
- `backend/Araponga.Application/Interfaces/IAIService.cs` (adicionar método de detecção)
- `backend/Araponga.Application/Services/PostCreationService.cs` (integrar detecção e bloqueio)
- `backend/Araponga.Application/Services/FeedService.cs` (aplicar bloqueio baseado em feature flags)
- `backend/Araponga.Application/Services/ModerationService.cs` (alertas e bloqueio automático)
- `backend/Araponga.Application/Services/FeatureFlagService.cs` (adicionar feature flags de bloqueio)
- `backend/Araponga.Domain/Users/UserPreferences.cs` (adicionar preferências de bloqueio)
- `backend/Araponga.Api/Contracts/Feed/FeedItemResponse.cs` (adicionar flags)
- `backend/Araponga.Api/Controllers/FeedController.cs` (aplicar bloqueio)
- `backend/Araponga.Api/Controllers/UserPreferencesController.cs` (gerenciar preferências de bloqueio)

**Critérios de Sucesso**:
- ✅ Detecção de conteúdo gerado por IA funcionando
- ✅ Classificação armazenada no banco
- ✅ Feature flags territoriais funcionando
- ✅ Feature flags por usuário funcionando
- ✅ Bloqueio automático funcionando (respeitando ambos os níveis)
- ✅ Flag visível em posts (se probabilidade alta)
- ✅ Conteúdo bloqueado oculto do feed
- ✅ Filtro manual funcionando no feed
- ✅ Preferências do usuário funcionando
- ✅ Estatísticas funcionando
- ✅ Dashboard de moderação funcionando
- ✅ Transparência garantida
- ✅ Testes passando

**Princípios Éticos**:
- ✅ **Transparência**: Usuário sabe quando conteúdo é gerado por IA
- ✅ **Controle Comunitário**: Território pode decidir bloquear conteúdo gerado por IA
- ✅ **Escolha do Usuário**: Usuário pode escolher bloquear individualmente
- ✅ **Privacidade**: Não expor dados sensíveis na detecção
- ✅ **Decisão Configurável**: Bloqueio pode ser configurado por território e usuário
- ✅ **Modos Flexíveis**: Diferentes níveis de bloqueio (None, Warn, Block)
- ✅ **Auditoria**: Logging de bloqueios para transparência

---

#### 21.6 Inteligência de Relevância de Publicações 🔴 NOVO
**Estimativa**: 24 horas (3 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `PostRelevanceService`:
  - [ ] `CalculateRelevanceScoreAsync(Post post, Guid userId, Guid territoryId, CancellationToken)` → `RelevanceScoreResult`
  - [ ] `CalculateRelevanceScoresBatchAsync(IReadOnlyList<Post> posts, Guid userId, Guid territoryId, CancellationToken)` → `Dictionary<Guid, RelevanceScoreResult>`
  - [ ] `GetMostRelevantPostsAsync(Guid territoryId, Guid userId, int limit, CancellationToken)` → `IReadOnlyList<Post>`
- [ ] Fatores de relevância (pesos configuráveis):
  - [ ] **Interesses do Território** (30%): Alinhamento com interesses do território (Fase 14)
  - [ ] **Proximidade Geográfica** (20%): Distância do usuário aos GeoAnchors do post
  - [ ] **Qualidade do Conteúdo** (15%): Score de qualidade calculado por IA
  - [ ] **Interações Comunitárias** (15%): Likes, shares, comentários (normalizado por tempo)
  - [ ] **Recência** (10%): Boost para posts recentes (decay exponencial)
  - [ ] **Autoridade do Autor** (10%): Histórico de contribuições do autor (Fase 17)
- [ ] Modelo de domínio `PostRelevanceScore`:
  - [ ] `Id`, `PostId`, `UserId`, `TerritoryId`
  - [ ] `Score` (decimal, 0-100)
  - [ ] `InterestAlignment` (decimal, 0-1)
  - [ ] `GeographicProximity` (decimal, 0-1)
  - [ ] `ContentQuality` (decimal, 0-1)
  - [ ] `CommunityInteractions` (decimal, 0-1)
  - [ ] `RecencyBoost` (decimal, 0-1)
  - [ ] `AuthorAuthority` (decimal, 0-1)
  - [ ] `CalculatedAtUtc` (DateTime)
  - [ ] `ExpiresAtUtc` (DateTime, cache TTL: 24h)
- [ ] Integração com `PostCreationService`:
  - [ ] Calcular score automaticamente ao criar post (opcional, via feature flag)
  - [ ] Armazenar score no banco
  - [ ] Não bloquear criação (apenas calcular)
- [ ] Integração com `FeedService`:
  - [ ] Incluir `RelevanceScore` em `FeedItemResponse` (opcional)
  - [ ] Badge discreto para posts altamente relevantes (score > 80)
  - [ ] **Importante**: Feed cronológico permanece como padrão
- [ ] Novo endpoint opcional:
  - [ ] `GET /api/v1/feed/relevant?territoryId={id}&limit={n}` → posts mais relevantes
  - [ ] Query parameter `sort=relevance` (opcional, não padrão)
  - [ ] Transparência: header `X-Sort-Mode: relevance` na resposta
- [ ] Feature flags:
  - [ ] `PostRelevanceScoringEnabled` (habilitar/desabilitar cálculo de relevância)
  - [ ] `PostRelevanceSortEnabled` (habilitar ordenação por relevância como opção)
- [ ] Preferências do usuário:
  - [ ] `DefaultFeedSort` (enum: `Chronological`, `Relevance`) - padrão: `Chronological`
  - [ ] Adicionar em `UserPreferences`
- [ ] Cache e performance:
  - [ ] Cache de scores (TTL: 24 horas)
  - [ ] Recalcular scores periodicamente (background job, diário)
  - [ ] Recalcular scores quando: post recebe interação, interesses do território mudam
- [ ] Estatísticas:
  - [ ] `GetRelevanceStatsAsync(Guid territoryId)` → estatísticas de relevância
  - [ ] Distribuição de scores
  - [ ] Posts mais relevantes do território
- [ ] Transparência:
  - [ ] Badge visual discreto: "Post altamente relevante" (se score > 80)
  - [ ] Tooltip explicativo: "Relevância baseada em interesses do território, proximidade e qualidade"
  - [ ] Link para política de transparência
- [ ] Testes:
  - [ ] Testes unitários (cálculo de score com diferentes fatores)
  - [ ] Testes de integração
  - [ ] Testes de performance (cálculo em lote)

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/PostRelevanceService.cs`
- `backend/Araponga.Domain/AI/PostRelevanceScore.cs`
- `backend/Araponga.Application/Interfaces/IPostRelevanceScoreRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresPostRelevanceScoreRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/Migrations/YYYYMMDDHHMMSS_AddPostRelevanceScore.cs`
- `backend/Araponga.Application/Models/AI/RelevanceScoreFactors.cs`
- `backend/Araponga.Tests/Application/PostRelevanceServiceTests.cs`

**Arquivos a Modificar**:
- `backend/Araponga.Application/Interfaces/IAIService.cs` (adicionar método de relevância)
- `backend/Araponga.Application/Services/PostCreationService.cs` (integrar cálculo de relevância)
- `backend/Araponga.Application/Services/FeedService.cs` (incluir score e opção de ordenação)
- `backend/Araponga.Domain/Users/UserPreferences.cs` (adicionar preferência de ordenação)
- `backend/Araponga.Api/Contracts/Feed/FeedItemResponse.cs` (adicionar score opcional)
- `backend/Araponga.Api/Controllers/FeedController.cs` (endpoint de relevância e query param)

**Critérios de Sucesso**:
- ✅ Cálculo de relevância funcionando
- ✅ Score armazenado no banco
- ✅ Feed cronológico permanece como padrão
- ✅ Opção de ordenação por relevância funcionando (opcional)
- ✅ Badge para posts altamente relevantes funcionando
- ✅ Preferências do usuário funcionando
- ✅ Cache funcionando
- ✅ Estatísticas funcionando
- ✅ Transparência garantida
- ✅ Testes passando

**Princípios Éticos**:
- ✅ **Feed Cronológico Primeiro**: Feed cronológico permanece como padrão
- ✅ **Transparência**: Usuário sabe quando está vendo por relevância
- ✅ **Escolha do Usuário**: Usuário pode escolher ordenação (cronológica ou relevância)
- ✅ **Não Manipulação**: Relevância não manipula feed sem consentimento
- ✅ **Contexto Territorial**: Relevância baseada em interesses e contexto do território

---

#### 21.7 Testes e Documentação
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Testes de integração completos:
  - [ ] Moderação automática
  - [ ] Busca semântica
  - [ ] Categorização
  - [ ] Recomendações
  - [ ] Tradução
  - [ ] Classificação de conteúdo gerado por IA
  - [ ] Inteligência de relevância de publicações
- [ ] Testes de performance (latência de APIs de IA)
- [ ] Testes de segurança (não expor dados sensíveis)
- [ ] Documentação técnica:
  - [ ] `docs/AI_SYSTEM.md`
  - [ ] Configuração de provedores (OpenAI, Azure)
  - [ ] Feature flags
  - [ ] Princípios éticos de IA
- [ ] Atualizar `docs/CHANGELOG.md`
- [ ] Atualizar Swagger

**Arquivos a Criar**:
- `backend/Araponga.Tests/Integration/AICompleteIntegrationTests.cs`
- `docs/AI_SYSTEM.md`

**Critérios de Sucesso**:
- ✅ Testes passando
- ✅ Cobertura >80%
- ✅ Documentação completa

---

#### 15.X Configuração de Rate Limiting
**Estimativa**: 24 horas (3 dias)  
**Status**: ⏳ Pendente  
**Prioridade**: 🔴 Alta

**Contexto**: Rate limiting atualmente configurado em `appsettings.json` com valores globais (`PermitLimit: 60, WindowSeconds: 60`). Esta tarefa permite configuração por território e por tipo de endpoint para proteção mais granular.

**Tarefas**:
- [ ] Criar modelo de domínio `RateLimitConfig`:
  - [ ] `Id`, `TerritoryId` (nullable para config global)
  - [ ] `EndpointType` (enum: All, Posts, Uploads, ApiGeneral, Chat, etc.)
  - [ ] `PermitLimit` (int, requisições permitidas)
  - [ ] `WindowSeconds` (int, janela de tempo)
  - [ ] `QueueLimit` (int, limite de fila)
  - [ ] `Enabled` (bool)
  - [ ] `CreatedAtUtc`, `UpdatedAtUtc`
- [ ] Criar `IRateLimitConfigRepository` e implementações (Postgres, InMemory)
- [ ] Criar `RateLimitConfigService`:
  - [ ] `GetConfigAsync(Guid? territoryId, string endpointType, CancellationToken)`
  - [ ] `CreateOrUpdateConfigAsync(RateLimitConfig, CancellationToken)`
  - [ ] `GetActiveConfigAsync(Guid? territoryId, string endpointType, CancellationToken)` → retorna territorial ou global
- [ ] Criar middleware `RateLimitMiddleware`:
  - [ ] Usar `RateLimitConfigService` para obter configuração
  - [ ] Aplicar rate limiting dinamicamente
  - [ ] Integrar com `Microsoft.AspNetCore.RateLimiting` ou implementação custom
- [ ] Criar `RateLimitConfigController`:
  - [ ] `GET /api/v1/territories/{territoryId}/rate-limit-config` (Curator)
  - [ ] `PUT /api/v1/territories/{territoryId}/rate-limit-config` (Curator)
  - [ ] `GET /api/v1/admin/rate-limit-config` (global, SystemAdmin)
  - [ ] `PUT /api/v1/admin/rate-limit-config` (global, SystemAdmin)
- [ ] Interface administrativa (DevPortal):
  - [ ] Seção para configuração de rate limiting
  - [ ] Visualização de configurações por endpoint
  - [ ] Alertas para limites muito baixos/altos
- [ ] Testes de integração
- [ ] Documentação

**Arquivos a Criar**:
- `backend/Araponga.Domain/Configuration/RateLimitConfig.cs`
- `backend/Araponga.Application/Interfaces/Configuration/IRateLimitConfigRepository.cs`
- `backend/Araponga.Application/Services/Configuration/RateLimitConfigService.cs`
- `backend/Araponga.Api/Middleware/RateLimitMiddleware.cs`
- `backend/Araponga.Api/Controllers/RateLimitConfigController.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresRateLimitConfigRepository.cs`
- `backend/Araponga.Infrastructure/InMemory/InMemoryRateLimitConfigRepository.cs`
- `backend/Araponga.Tests/Api/RateLimitConfigIntegrationTests.cs`

**Arquivos a Modificar**:
- `backend/Araponga.Api/Program.cs` (registrar middleware e serviços)
- `backend/Araponga.Infrastructure/InMemory/InMemoryDataStore.cs`
- `backend/Araponga.Api/Extensions/ServiceCollectionExtensions.cs`
- `backend/Araponga.Api/wwwroot/devportal/index.html`

**Critérios de Sucesso**:
- ✅ Rate limiting configurável por território
- ✅ Configuração por tipo de endpoint funcionando
- ✅ Fallback para `appsettings.json` funcionando
- ✅ Ajustes em tempo real (sem restart)
- ✅ Interface administrativa disponível
- ✅ Testes passando
- ✅ Documentação atualizada

**Referência**: Consulte `FASE10_CONFIG_FLEXIBILIZACAO_AVALIACAO.md` para contexto completo.

---

#### 15.Y Configuração de Autenticação (JWT)
**Estimativa**: 16 horas (2 dias)  
**Status**: ⏳ Pendente  
**Prioridade**: 🟡 Média

**Contexto**: Configuração JWT atualmente em `appsettings.json` (`Issuer`, `Audience`, `ExpirationMinutes`). Esta tarefa permite configuração via painel administrativo para ajustes de segurança sem deploy.

**Tarefas**:
- [ ] Criar modelo de domínio `JwtConfig`:
  - [ ] `Id` (configuração global única)
  - [ ] `Issuer` (string)
  - [ ] `Audience` (string)
  - [ ] `AccessTokenExpirationMinutes` (int)
  - [ ] `RefreshTokenExpirationDays` (int, opcional)
  - [ ] `IsActive` (bool)
  - [ ] `CreatedAtUtc`, `UpdatedAtUtc`
- [ ] Criar `IJwtConfigRepository` e implementações (Postgres, InMemory)
- [ ] Criar `JwtConfigService`:
  - [ ] `GetActiveConfigAsync(CancellationToken)` → retorna config ativa
  - [ ] `CreateOrUpdateConfigAsync(JwtConfig, CancellationToken)`
  - [ ] `ActivateConfigAsync(Guid configId, CancellationToken)` → desativa outras configs
- [ ] Atualizar `JwtTokenService`:
  - [ ] Usar `JwtConfigService` ao gerar tokens
  - [ ] Fallback para `appsettings.json` se não configurado
  - [ ] Suporte a refresh tokens (se configurado)
- [ ] Criar `JwtConfigController`:
  - [ ] `GET /api/v1/admin/jwt-config/active` (SystemAdmin)
  - [ ] `GET /api/v1/admin/jwt-config` (listar todas, SystemAdmin)
  - [ ] `POST /api/v1/admin/jwt-config` (criar, SystemAdmin)
  - [ ] `PUT /api/v1/admin/jwt-config/{configId}` (atualizar, SystemAdmin)
  - [ ] `POST /api/v1/admin/jwt-config/{configId}/activate` (ativar, SystemAdmin)
- [ ] Interface administrativa (DevPortal):
  - [ ] Seção para configuração de JWT
  - [ ] Alertas para expirações muito curtas/longas
  - [ ] Visualização de configuração ativa
- [ ] Testes de integração
- [ ] Documentação

**Arquivos a Criar**:
- `backend/Araponga.Domain/Configuration/JwtConfig.cs`
- `backend/Araponga.Application/Interfaces/Configuration/IJwtConfigRepository.cs`
- `backend/Araponga.Application/Services/Configuration/JwtConfigService.cs`
- `backend/Araponga.Api/Controllers/JwtConfigController.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresJwtConfigRepository.cs`
- `backend/Araponga.Infrastructure/InMemory/InMemoryJwtConfigRepository.cs`
- `backend/Araponga.Tests/Api/JwtConfigIntegrationTests.cs`

**Arquivos a Modificar**:
- `backend/Araponga.Infrastructure/Security/JwtTokenService.cs`
- `backend/Araponga.Infrastructure/InMemory/InMemoryDataStore.cs`
- `backend/Araponga.Api/Extensions/ServiceCollectionExtensions.cs`
- `backend/Araponga.Api/wwwroot/devportal/index.html`

**Critérios de Sucesso**:
- ✅ Configuração JWT via painel administrativo
- ✅ Ajustes de expiração sem deploy
- ✅ Fallback para `appsettings.json` funcionando
- ✅ Suporte a múltiplas configurações (ativação seletiva)
- ✅ Interface administrativa disponível
- ✅ Testes passando
- ✅ Documentação atualizada

**Referência**: Consulte `FASE10_CONFIG_FLEXIBILIZACAO_AVALIACAO.md` para contexto completo.

---

## 📊 Resumo da Fase 15

| Tarefa | Estimativa | Status | Prioridade |
|--------|------------|--------|------------|
| Interface e Abstração | 8h | ❌ Pendente | 🔴 Crítica |
| Implementação OpenAI | 20h | ❌ Pendente | 🔴 Crítica |
| Implementação Azure | 16h | ❌ Pendente | 🟢 Opcional |
| Serviço de Moderação Automática | 16h | ❌ Pendente | 🔴 Crítica |
| Integração com Sistema de Moderação | 12h | ❌ Pendente | 🔴 Crítica |
| Serviço de Busca Semântica | 20h | ❌ Pendente | 🔴 Crítica |
| Integração com Controllers | 12h | ❌ Pendente | 🔴 Crítica |
| Categorização Automática | 16h | ❌ Pendente | 🟡 Importante |
| Recomendações Contextuais | 16h | ❌ Pendente | 🟡 Importante |
| Tradução Automática | 12h | ❌ Pendente | 🟢 Opcional |
| Análise de Conteúdo | 12h | ❌ Pendente | 🟡 Importante |
| Testes e Documentação | 16h | ❌ Pendente | 🟡 Importante |
| **Total** | **160h (28 dias)** | | |

---

## ✅ Critérios de Sucesso da Fase 15

### Funcionalidades
- ✅ Moderação automática funcionando
- ✅ Busca semântica funcionando
- ✅ Categorização automática funcionando
- ✅ Recomendações contextuais funcionando (opcionais)
- ✅ Classificação de conteúdo gerado por IA funcionando
- ✅ Inteligência de relevância de publicações funcionando
- ✅ Tradução automática funcionando (opcional)
- ✅ Análise de conteúdo funcionando

### Qualidade
- ✅ Cobertura de testes >80%
- ✅ Testes de integração passando
- ✅ Performance adequada (latência < 2s para APIs de IA)
- ✅ Segurança validada (dados não expostos)

### Documentação
- ✅ Documentação técnica completa
- ✅ Princípios éticos documentados
- ✅ Changelog atualizado
- ✅ Swagger atualizado

---

## 🔗 Dependências

- **Nenhuma**: Pode ser feito em paralelo com outras fases
- **Opcional**: Fase 14 (Governança) - categorização pode usar interesses do usuário

---

## 📝 Notas de Implementação

### Configuração OpenAI

```json
{
  "AI": {
    "OpenAI": {
      "ApiKey": "[secret]",
      "Model": "gpt-4",
      "ModerationModel": "text-moderation-latest",
      "EmbeddingModel": "text-embedding-3-small"
    },
    "Enabled": true,
    "RateLimit": {
      "RequestsPerMinute": 60,
      "TokensPerMinute": 90000
    }
  }
}
```

### Configuração Azure (Opcional)

```json
{
  "AI": {
    "Azure": {
      "Endpoint": "https://[resource].cognitiveservices.azure.com/",
      "ApiKey": "[secret]"
    },
    "Enabled": false
  }
}
```

### Feature Flags

#### Feature Flags Territoriais

- `AIModerationEnabled` (por território) - Moderação automática
- `AISearchEnabled` (por território) - Busca semântica
- `AICategorizationEnabled` (por território) - Categorização automática
- `AIRecommendationsEnabled` (por território e por usuário) - Recomendações
- `AITranslationEnabled` (por território) - Tradução automática
- `AIGeneratedContentDetectionEnabled` (por território) - Detecção de conteúdo gerado por IA
- `AIGeneratedContentBlockingEnabled` (por território) - Habilitar bloqueio de conteúdo gerado por IA
- `AIGeneratedContentBlockingThreshold` (por território, decimal 0-100) - Threshold para bloquear (padrão: 80%)
- `AIGeneratedContentBlockingMode` (por território, enum) - Modo de bloqueio:
  - `None`: Apenas detecta e marca (não bloqueia)
  - `Warn`: Marca e alerta, mas permite visualização
  - `Block`: Bloqueia completamente do feed (oculta)

#### Feature Flags por Usuário (Preferências)

- `BlockAIGeneratedContent` (bool) - Usuário bloqueia conteúdo gerado por IA
- `AIGeneratedContentBlockingThreshold` (decimal 0-100) - Threshold pessoal (padrão: 80%)
- `ShowAIGeneratedContent` (bool) - Mostrar conteúdo gerado por IA (oposto de bloqueio)

#### Lógica de Bloqueio

**Prioridade de Bloqueio**:
1. **Bloqueio Territorial** (`BlockingMode = Block`) → Conteúdo oculto para todos no território
2. **Bloqueio do Usuário** (`BlockAIGeneratedContent = true`) → Conteúdo oculto apenas para o usuário
3. **Permissão** → Conteúdo visível (com marcação se probabilidade > threshold)

**Exemplos**:
- Território: `BlockingMode = Block`, Threshold = 80% → Posts com probabilidade > 80% são ocultos
- Usuário: `BlockAIGeneratedContent = true`, Threshold = 70% → Posts com probabilidade > 70% são ocultos para este usuário
- Ambos permitem: Posts são visíveis, mas marcados se probabilidade > threshold

### Princípios Éticos

1. **Transparência**: Usuário sempre sabe quando IA está sendo usada
2. **Privacidade**: Dados não são compartilhados sem consentimento explícito
3. **Não Manipulação**: Feed cronológico nunca é alterado por IA (relevância é opcional)
4. **Controle**: Usuário pode desabilitar todas as funcionalidades de IA
5. **Controle Comunitário**: Território pode decidir bloquear conteúdo gerado por IA
6. **Escolha Individual**: Usuário pode escolher bloquear conteúdo gerado por IA
7. **Ética**: IA serve à comunidade, não à extração de dados
8. **Flexibilidade**: Diferentes níveis de bloqueio (None, Warn, Block) para diferentes necessidades

---

**Status**: ⏳ **FASE 15 PENDENTE**  
**Depende de**: Nenhuma (opcional: Fase 14 para integração com interesses)
