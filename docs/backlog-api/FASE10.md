# Fase 10: Mídias em Conteúdo

**Duração**: 4 semanas (20 dias úteis)  
**Prioridade**: 🔴 CRÍTICA (Bloqueante para edição)  
**Depende de**: Fase 8 (Infraestrutura de Mídia)  
**Bloqueia**: Fase 11 (Edição e Gestão)  
**Estimativa Total**: 160 horas  
**Status**: ✅ Implementação Principal Completa + ⚙️ Configuração Avançada por Território (Em Implementação)

---

## 🎯 Objetivo

Integrar mídias (imagens, vídeos e áudios) em todas as funcionalidades de conteúdo, permitindo:
- Múltiplas mídias por post (imagens, vídeos e áudios, máximo 1 vídeo e 1 áudio)
- Mídia de capa em eventos (imagem, vídeo ou áudio, máximo 1 vídeo e 1 áudio no total)
- Múltiplas mídias por item no marketplace (imagens, vídeos e áudios, máximo 1 vídeo e 1 áudio)
- Envio de imagens no chat (vídeos e áudios não permitidos)
- Exclusão de posts com mídias associadas

**Princípios**:
- ✅ **Documentação Territorial**: Mídias servem para documentar território
- ✅ **Fortalecimento Comunitário**: Mídias fortalecem comunidade
- ✅ **Não Captura de Atenção**: Feed permanece cronológico, não algorítmico

---

## 📋 Contexto e Requisitos

### Estado Atual
- ✅ Sistema de mídia implementado (Fase 8)
- ✅ `MediaAsset` e `MediaAttachment` criados
- ✅ Upload/download de imagens funcionando
- ❌ Mídias não integradas em posts
- ❌ Mídias não integradas em eventos
- ❌ Mídias não integradas em marketplace
- ❌ Mídias não integradas em chat

### Requisitos Funcionais

#### 1. Mídias em Posts
- ✅ Múltiplas mídias por post (até 10 mídias: imagens, vídeos e/ou áudios)
- ✅ Máximo 1 vídeo por post (até 50MB, até 60 segundos)
- ✅ Máximo 1 áudio por post (até 10MB, até 5 minutos)
- ✅ Ordem de exibição configurável
- ✅ Exclusão de post deleta mídias associadas
- ✅ Visualização de mídias em posts

#### 2. Mídias em Eventos
- ✅ Mídia de capa do evento (imagem, vídeo ou áudio)
- ✅ Múltiplas mídias adicionais (até 5: imagens, vídeos ou áudios)
- ✅ Máximo 1 vídeo por evento (até 100MB, até 2 minutos)
- ✅ Máximo 1 áudio por evento (até 20MB, até 10 minutos)
- ✅ Exclusão de evento deleta mídias associadas

#### 3. Mídias em Marketplace
- ✅ Múltiplas mídias por item (até 10: imagens, vídeos e/ou áudios)
- ✅ Máximo 1 vídeo por item (até 30MB, até 30 segundos)
- ✅ Máximo 1 áudio por item (até 5MB, até 2 minutos)
- ✅ Imagem principal (primeira mídia, pode ser vídeo ou áudio)
- ✅ Exclusão de item deleta mídias associadas

#### 4. Mídias em Chat
- ✅ Envio de imagens em mensagens (máximo 5MB)
- ✅ Envio de áudios curtos em mensagens (máximo 2MB, até 60 segundos - mensagens de voz)
- ❌ Vídeos não permitidos (apenas imagens e áudios, por questões de performance e privacidade)
- ✅ Visualização de imagens e reprodução de áudios em chat
- ✅ Validação de tamanho e tipo

---

## 📋 Tarefas Detalhadas

### Semana 35: Mídias em Posts

#### 10.1 Integração de Mídias em Posts
**Estimativa**: 24 horas (3 dias)  
**Status**: ✅ Implementado

**Tarefas**:
- [ ] Atualizar `PostCreationService`:
  - [ ] Aceitar lista de `MediaAssetId` no request
  - [ ] Validar que mídias pertencem ao usuário
  - [ ] Criar `MediaAttachment` para cada mídia
  - [ ] Definir `DisplayOrder` (ordem de exibição)
- [ ] Atualizar `PostController`:
  - [ ] `POST /api/v1/feed/posts` aceita `mediaIds` (array de Guid)
  - [ ] Validação de mídias (máx. 10 por post)
- [ ] Atualizar `PostResponse`:
  - [ ] Incluir `MediaUrls` (array de URLs)
  - [ ] Incluir `MediaCount` (int)
- [ ] Atualizar exclusão de posts:
  - [ ] Deletar `MediaAttachment` quando post é deletado
  - [ ] Soft delete de `MediaAsset` (se não usado em outros lugares)
- [ ] Testes de integração

**Arquivos a Modificar**:
- `backend/Araponga.Application/Services/PostCreationService.cs`
- `backend/Araponga.Api/Controllers/FeedController.cs`
- `backend/Araponga.Api/Contracts/Feed/CreatePostRequest.cs`
- `backend/Araponga.Api/Contracts/Feed/PostResponse.cs`

**Critérios de Sucesso**:
- ✅ Posts podem ter múltiplas imagens
- ✅ Ordem de exibição funcionando
- ✅ Exclusão de posts deleta mídias
- ✅ Testes passando

---

#### 10.2 Visualização de Mídias em Posts
**Estimativa**: 16 horas (2 dias)  
**Status**: ✅ Implementado

**Tarefas**:
- [ ] Atualizar `FeedService`:
  - [ ] Incluir URLs de mídias ao buscar posts
  - [ ] Buscar `MediaAttachment` por `OwnerType = Post`
  - [ ] Ordenar por `DisplayOrder`
- [ ] Otimização:
  - [ ] Buscar mídias em batch (não N+1)
  - [ ] Cache de URLs de mídia
- [ ] Testes de performance

**Arquivos a Modificar**:
- `backend/Araponga.Application/Services/FeedService.cs`
- `backend/Araponga.Application/Services/PostFilterService.cs`

**Critérios de Sucesso**:
- ✅ Mídias exibidas em posts
- ✅ Performance adequada (< 500ms para feed)
- ✅ Testes passando

---

### Semana 36: Mídias em Eventos e Marketplace

#### 10.3 Mídias em Eventos
**Estimativa**: 20 horas (2.5 dias)  
**Status**: ✅ Implementado

**Tarefas**:
- [ ] Atualizar `EventsService`:
  - [ ] Aceitar `CoverMediaAssetId` no request
  - [ ] Aceitar `AdditionalMediaIds` (array, opcional)
  - [ ] Validar que mídias pertencem ao usuário
  - [ ] Criar `MediaAttachment` para cada mídia
- [ ] Atualizar `EventsController`:
  - [ ] `POST /api/v1/events` aceita `coverMediaId` e `additionalMediaIds`
  - [ ] Validação (máx. 5 imagens adicionais)
- [ ] Atualizar `EventResponse`:
  - [ ] Incluir `CoverImageUrl`
  - [ ] Incluir `AdditionalImageUrls` (array)
- [ ] Atualizar exclusão de eventos:
  - [ ] Deletar mídias associadas
- [ ] Testes

**Arquivos a Modificar**:
- `backend/Araponga.Application/Services/EventsService.cs`
- `backend/Araponga.Api/Controllers/EventsController.cs`
- `backend/Araponga.Api/Contracts/Events/CreateEventRequest.cs`
- `backend/Araponga.Api/Contracts/Events/EventResponse.cs`

**Critérios de Sucesso**:
- ✅ Eventos podem ter imagem de capa
- ✅ Eventos podem ter imagens adicionais
- ✅ Exclusão deleta mídias
- ✅ Testes passando

---

#### 10.4 Mídias em Marketplace
**Estimativa**: 24 horas (3 dias)  
**Status**: ✅ Implementado

**Tarefas**:
- [ ] Atualizar `StoreItemService`:
  - [ ] Aceitar `MediaIds` (array) no request
  - [ ] Validar que mídias pertencem ao usuário
  - [ ] Criar `MediaAttachment` para cada mídia
  - [ ] Primeira mídia é imagem principal
- [ ] Atualizar `ItemsController`:
  - [ ] `POST /api/v1/items` aceita `mediaIds` (array)
  - [ ] Validação (máx. 10 imagens por item)
- [ ] Atualizar `StoreItemResponse`:
  - [ ] Incluir `PrimaryImageUrl` (primeira mídia)
  - [ ] Incluir `ImageUrls` (array de todas as mídias)
- [ ] Atualizar exclusão de itens:
  - [ ] Deletar mídias associadas
- [ ] Testes

**Arquivos a Modificar**:
- `backend/Araponga.Application/Services/StoreItemService.cs`
- `backend/Araponga.Api/Controllers/ItemsController.cs`
- `backend/Araponga.Api/Contracts/Marketplace/CreateItemRequest.cs`
- `backend/Araponga.Api/Contracts/Marketplace/StoreItemResponse.cs`

**Critérios de Sucesso**:
- ✅ Itens podem ter múltiplas imagens
- ✅ Imagem principal identificada
- ✅ Exclusão deleta mídias
- ✅ Testes passando

---

### Semana 37: Mídias em Chat

#### 10.5 Mídias em Chat
**Estimativa**: 20 horas (2.5 dias)  
**Status**: ✅ Implementado

**Tarefas**:
- [ ] Atualizar `ChatService`:
  - [ ] Aceitar `MediaAssetId` no request de mensagem
  - [ ] Validar que mídia pertence ao usuário
  - [ ] Criar `MediaAttachment` para mensagem
  - [ ] Limitar tamanho de imagens em chat (máx. 5MB)
- [ ] Atualizar `ChatController`:
  - [ ] `POST /api/v1/chat/conversations/{id}/messages` aceita `mediaId` (Guid?)
  - [ ] Validação de tipo (apenas imagens em chat)
- [ ] Atualizar `ChatMessageResponse`:
  - [ ] Incluir `MediaUrl` (se mensagem tem mídia)
  - [ ] Incluir `HasMedia` (bool)
- [ ] Testes

**Arquivos a Modificar**:
- `backend/Araponga.Application/Services/ChatService.cs`
- `backend/Araponga.Api/Controllers/ChatController.cs`
- `backend/Araponga.Api/Contracts/Chat/SendMessageRequest.cs`
- `backend/Araponga.Api/Contracts/Chat/ChatMessageResponse.cs`

**Critérios de Sucesso**:
- ✅ Mensagens podem ter imagens
- ✅ Validação de tamanho funcionando
- ✅ Visualização de imagens em chat
- ✅ Testes passando

---

### Semana 38: Testes, Otimizações e Documentação

#### 10.6 Testes de Integração Completos
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Testes de integração de mídias em posts
- [ ] Testes de integração de mídias em eventos
- [ ] Testes de integração de mídias em marketplace
- [ ] Testes de integração de mídias em chat
- [ ] Testes de exclusão (mídias deletadas corretamente)
- [ ] Testes de performance (feed com mídias < 500ms)
- [ ] Testes de segurança (validação de ownership)

**Arquivos a Criar**:
- `backend/Araponga.Tests/Integration/MediaInPostsTests.cs`
- `backend/Araponga.Tests/Integration/MediaInEventsTests.cs`
- `backend/Araponga.Tests/Integration/MediaInMarketplaceTests.cs`
- `backend/Araponga.Tests/Integration/MediaInChatTests.cs`

**Critérios de Sucesso**:
- ✅ Testes de integração passando
- ✅ Cobertura >90%
- ✅ Testes de performance passando

---

#### 10.7 Otimizações
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Otimizar queries de mídias:
  - [ ] Buscar mídias em batch (evitar N+1)
  - [ ] Usar `Include` ou queries separadas otimizadas
- [ ] Cache de URLs de mídia:
  - [ ] Cache de URLs públicas (TTL: 1 hora)
  - [ ] Invalidação quando mídia é deletada
- [ ] Otimização de serialização:
  - [ ] Lazy loading de URLs (apenas quando necessário)
  - [ ] Projeções para reduzir dados transferidos
- [ ] Validação de performance

**Arquivos a Modificar**:
- `backend/Araponga.Application/Services/FeedService.cs`
- `backend/Araponga.Application/Services/EventsService.cs`
- `backend/Araponga.Application/Services/StoreItemService.cs`
- `backend/Araponga.Application/Services/ChatService.cs`

**Critérios de Sucesso**:
- ✅ Queries otimizadas
- ✅ Cache funcionando
- ✅ Performance adequada (< 500ms)

---

#### 10.8 Documentação
**Estimativa**: 8 horas (1 dia)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Documentação técnica:
  - [ ] `docs/MEDIA_IN_CONTENT.md` (integração de mídias)
  - [ ] Exemplos de uso da API
- [ ] Atualizar `docs/CHANGELOG.md`
- [ ] Atualizar Swagger com exemplos
- [ ] Revisão final

**Arquivos a Criar**:
- `docs/MEDIA_IN_CONTENT.md`

**Arquivos a Modificar**:
- `docs/CHANGELOG.md`

**Critérios de Sucesso**:
- ✅ Documentação completa
- ✅ Changelog atualizado
- ✅ Swagger atualizado

---

#### 10.9 Configuração Avançada de Limites de Mídia
**Estimativa**: 16 horas (2 dias)  
**Status**: ✅ Completo  
**Prioridade**: 🟡 Média

**Contexto**: Estender `TerritoryMediaConfig` (já existente) para incluir configuração de limites de tamanho e tipos MIME, permitindo override de limites globais por território.

**Tarefas**:
- [x] Estender modelo `TerritoryMediaConfig`:
  - [x] Adicionar campos para limites de tamanho por tipo (imagem, vídeo, áudio) em cada contexto (posts, events, marketplace, chat)
  - [x] Adicionar campos para tipos MIME permitidos (override opcional)
  - [x] Adicionar validação de limites mínimos/máximos
- [x] Estender `TerritoryMediaConfigService`:
  - [x] Validar limites contra valores globais (`MediaStorageOptions` via `IGlobalMediaLimits`)
  - [x] Aplicar limites por território quando disponíveis (fallback para global)
- [x] Estender `MediaConfigController`:
  - [x] Endpoints para atualizar limites de tamanho (já existente via `UpdateTerritoryMediaConfigRequest`)
  - [x] Endpoints para atualizar tipos MIME permitidos (já existente via `UpdateTerritoryMediaConfigRequest`)
- [x] Atualizar serviços de conteúdo (`PostCreationService`, `EventsService`, `StoreItemService`, `ChatService`):
  - [x] Usar limites do `TerritoryMediaConfig` quando disponíveis
  - [x] Fallback para `MediaStorageOptions` via `IGlobalMediaLimits` se não configurado
- [x] Interface administrativa (DevPortal):
  - [x] Seção para configuração de limites de mídia
  - [x] Explicação de limites globais vs territoriais
- [x] Testes de integração (existentes: `MediaConfigIntegrationTests`, `MediaConfigValidationIntegrationTests`)
- [x] Documentação

**Arquivos a Modificar**:
- `backend/Araponga.Domain/Media/TerritoryMediaConfig.cs`
- `backend/Araponga.Application/Services/Media/TerritoryMediaConfigService.cs`
- `backend/Araponga.Api/Controllers/MediaConfigController.cs`
- `backend/Araponga.Application/Services/PostCreationService.cs`
- `backend/Araponga.Application/Services/EventsService.cs`
- `backend/Araponga.Application/Services/StoreItemService.cs`
- `backend/Araponga.Application/Services/ChatService.cs`
- `backend/Araponga.Api/wwwroot/devportal/index.html`

**Arquivos a Criar**:
- `backend/Araponga.Tests/Api/MediaLimitsConfigIntegrationTests.cs`

**Critérios de Sucesso**:
- ✅ Limites configuráveis por território
- ✅ Override de limites globais funcionando
- ✅ Validação de limites funcionando
- ✅ Interface administrativa disponível
- ✅ Testes passando
- ✅ Documentação atualizada

**Referência**: Consulte `FASE10_CONFIG_FLEXIBILIZACAO_AVALIACAO.md` para contexto completo.

---

## 📊 Resumo da Fase 10

| Tarefa | Estimativa | Status | Prioridade |
|--------|------------|--------|------------|
| Mídias em Posts | 40h | ❌ Pendente | 🔴 Crítica |
| Mídias em Eventos | 20h | ❌ Pendente | 🔴 Crítica |
| Mídias em Marketplace | 24h | ❌ Pendente | 🔴 Crítica |
| Mídias em Chat | 20h | ❌ Pendente | 🟡 Importante |
| Testes de Integração | 16h | ❌ Pendente | 🟡 Importante |
| Otimizações | 12h | ❌ Pendente | 🟡 Importante |
| Documentação | 8h | ❌ Pendente | 🟢 Melhoria |
| **Total** | **160h (20 dias)** | | |

---

## ✅ Critérios de Sucesso da Fase 10

### Funcionalidades
- ✅ Posts podem ter múltiplas imagens
- ✅ Eventos podem ter imagem de capa e imagens adicionais
- ✅ Itens do marketplace podem ter múltiplas imagens
- ✅ Chat pode enviar imagens
- ✅ Exclusão de conteúdo deleta mídias associadas

### Qualidade
- ✅ Cobertura de testes >90%
- ✅ Performance adequada (feed com mídias < 500ms)
- ✅ Validações funcionando

### Documentação
- ✅ Documentação técnica completa
- ✅ Changelog atualizado
- ✅ Swagger atualizado

---

## 🔗 Dependências

- **Fase 8**: Infraestrutura de Mídia (obrigatória)
- **Bloqueia**: Fase 11 (Edição e Gestão)

---

## 📝 Notas de Implementação

### Limites de Mídias

- **Posts**: Máx. 10 mídias (imagens, vídeos e/ou áudios), máximo 1 vídeo (50MB, 60s) e 1 áudio (10MB, 5min)
- **Eventos**: 1 mídia de capa (imagem, vídeo ou áudio) + máx. 5 mídias adicionais, máximo 1 vídeo (100MB, 120s) e 1 áudio (20MB, 10min) no total
- **Marketplace**: Máx. 10 mídias (imagens, vídeos e/ou áudios), máximo 1 vídeo (30MB, 30s) e 1 áudio (5MB, 2min)
- **Chat**: 1 mídia por mensagem (imagem até 5MB ou áudio até 2MB), vídeos não permitidos

### Regras de Vídeos (Baseadas em Redes Sociais)

#### Posts
- **Duração**: Até 60 segundos (similar a TikTok/Instagram Reels)
- **Tamanho**: Máximo 50MB
- **Quantidade**: Apenas 1 vídeo por post (pode combinar com imagens e áudios, total máximo 10 mídias)

#### Eventos
- **Duração**: Até 2 minutos (mais flexível para eventos)
- **Tamanho**: Máximo 100MB
- **Quantidade**: Apenas 1 vídeo por evento (em capa ou adicionais, total máximo 6 mídias: 1 capa + 5 adicionais)

#### Marketplace (Items)
- **Duração**: Até 30 segundos (demonstração rápida de produto)
- **Tamanho**: Máximo 30MB
- **Quantidade**: Apenas 1 vídeo por item (pode combinar com imagens e áudios, total máximo 10 mídias)

#### Chat
- **Vídeos não permitidos**: Apenas imagens (por questões de performance e privacidade)
- **Limite**: 1 imagem por mensagem, máximo 5MB

### Regras de Áudios (Baseadas em Redes Sociais)

#### Posts
- **Duração**: Até 5 minutos (podcasts curtos, narrações, depoimentos)
- **Tamanho**: Máximo 10MB
- **Quantidade**: Apenas 1 áudio por post (pode combinar com imagens e vídeos, total máximo 10 mídias)
- **Formatos**: MP3, WAV, OGG
- **Referência**: Twitter/X (140s), Instagram Stories (60s), podcasts curtos

#### Eventos
- **Duração**: Até 10 minutos (mais flexível para eventos)
- **Tamanho**: Máximo 20MB
- **Quantidade**: Apenas 1 áudio por evento (em capa ou adicionais, total máximo 6 mídias)
- **Formatos**: MP3, WAV, OGG
- **Uso**: Promoção de eventos, trilhas sonoras, depoimentos

#### Marketplace (Items)
- **Duração**: Até 2 minutos (demonstração rápida de produto/serviço)
- **Tamanho**: Máximo 5MB
- **Quantidade**: Apenas 1 áudio por item (pode combinar com imagens e vídeos, total máximo 10 mídias)
- **Formatos**: MP3, WAV, OGG
- **Uso**: Descrição de produto, demonstração de serviço, áudio promocional

#### Chat
- **Áudios permitidos**: Áudios curtos (mensagens de voz), máximo 2MB, até 60 segundos
- **Imagens permitidas**: Máximo 5MB por imagem
- **Vídeos não permitidos**: Apenas imagens e áudios (vídeos bloqueados por questões de performance e privacidade)
- **Formato de áudio**: MP3, WAV, OGG
- **Uso**: Mensagens de voz curtas (similar a WhatsApp, Telegram)

### Exclusão de Mídias

- Quando conteúdo é deletado, `MediaAttachment` é deletado
- `MediaAsset` tem soft delete (pode ser restaurado)
- Se mídia não está mais associada a nenhum conteúdo, pode ser deletada permanentemente (opcional)

---

**Status**: ✅ **FASE 10 - IMPLEMENTAÇÃO PRINCIPAL COMPLETA**  
**Depende de**: Fase 8 (Infraestrutura de Mídia) ✅  
**Bloqueia**: Fase 11 (Edição e Gestão) - Desbloqueado

**Nota**: Exclusão automática de mídias e testes de integração foram implementados.

**Atualização (Suporte a Vídeos e Áudios)**: Suporte a vídeos e áudios foi implementado em Posts, Eventos e Marketplace, com regras baseadas em redes sociais existentes (TikTok: 60s, Instagram: 60s-10min, Twitter/X: 140s para áudio). Chat aceita áudios curtos (mensagens de voz). Consulte `FASE10_VIDEOS_MAPA_IMPACTO.md` e `FASE10_AUDIO_MAPA_IMPACTO.md` para detalhes completos.

**Atualização (Configuração Avançada por Território)**: Sistema de configuração avançada de mídias por território usando feature flags e configurações granulares está em implementação. Permite que cada território configure tipos de mídia permitidos, limites de tamanho/quantidade, e que usuários escolham quais tipos de mídia visualizar. Consulte `FASE10_CONFIG_MIDIAS_TERRITORIO.md` e `FASE10_CONFIG_MIDIAS_IMPLEMENTACAO.md` para detalhes.

**Atualização (Configuração de Blob Storage via Painel Administrativo)**: Sistema de configuração explícita e aberta do provedor de blob storage para mídias (Local, S3, AzureBlob) via painel administrativo está implementado. Permite configurar provedores de storage sem editar `appsettings.json`, com suporte para Local, Amazon S3 e Azure Blob Storage. Consulte `FASE10_STORAGE_CONFIG_ADMIN.md` para detalhes da arquitetura.

**Atualização (Configuração Avançada de Limites de Mídia - Fase 10.9)**: Sistema de configuração avançada de limites de mídia por território está implementado. Permite que curadores configurem limites de tamanho e tipos MIME permitidos para cada tipo de conteúdo (Posts, Events, Marketplace, Chat), com validação contra limites globais e fallback automático. Todos os serviços de conteúdo (`PostCreationService`, `EventsService`, `StoreItemService`, `ChatService`) usam esses limites configuráveis. Consulte `FASE10_CONFIG_FLEXIBILIZACAO_AVALIACAO.md` para contexto completo.

## 🛡️ Segurança Avançada Implementada

### Validações de Segurança

#### 1. Validação de Propriedade
- ✅ Todas as mídias devem pertencer ao usuário que está criando o conteúdo
- ✅ Validação realizada em `PostCreationService`, `EventsService`, `StoreItemService`, `ChatService`
- ✅ Retorna erro `400 Bad Request` se mídia não pertence ao usuário

#### 2. Validação de Estado
- ✅ Mídias deletadas (soft delete) não podem ser associadas a conteúdo
- ✅ Validação de `IsDeleted` em todos os serviços

#### 3. Validação de Duplicatas
- ✅ IDs duplicados são rejeitados no mesmo request
- ✅ Validação no FluentValidation (validators)

#### 4. Validação de GUIDs Vazios
- ✅ GUIDs vazios são filtrados e rejeitados
- ✅ Normalização em todos os serviços

#### 5. Limites de Quantidade
- ✅ **Posts**: Máx. 10 mídias (imagens, vídeos e/ou áudios), máximo 1 vídeo e 1 áudio (validação em `PostCreationService`)
- ✅ **Eventos**: 1 capa + máx. 5 adicionais, máximo 1 vídeo e 1 áudio no total (validação em `EventsService`)
- ✅ **Items**: Máx. 10 mídias (imagens, vídeos e/ou áudios), máximo 1 vídeo e 1 áudio (validação em `StoreItemService`)
- ✅ **Chat**: 1 mídia por mensagem (imagem ou áudio, validação em `ChatService`)

#### 6. Validação de Tipo, Vídeos e Áudios
- ✅ Chat aceita imagens e áudios (vídeos não permitidos)
- ✅ Posts, Eventos e Items aceitam imagens, vídeos e áudios
- ✅ Validação de `MediaType.Image` e `MediaType.Audio` em `ChatService` (vídeos bloqueados)
- ✅ Validação de `MediaType.Video` em `PostCreationService`, `EventsService`, `StoreItemService` para limitar quantidade
- ✅ Validação de `MediaType.Audio` em `PostCreationService`, `EventsService`, `StoreItemService`, `ChatService` para limitar quantidade

#### 7. Validação de Tamanho
- ✅ Chat: 5MB por imagem, 2MB por áudio (validação em `ChatService`)
- ✅ Posts: 50MB por vídeo, 10MB por áudio (validação em `PostCreationService`)
- ✅ Eventos: 100MB por vídeo, 20MB por áudio (validação em `EventsService`)
- ✅ Items: 30MB por vídeo, 5MB por áudio (validação em `StoreItemService`)
- ✅ Validação de `SizeBytes` em todos os serviços

#### 10. Validação de Vídeos
- ✅ Máximo 1 vídeo por post (validação em `PostCreationService`)
- ✅ Máximo 1 vídeo por evento (validação em `EventsService`)
- ✅ Máximo 1 vídeo por item (validação em `StoreItemService`)
- ✅ Limites de tamanho específicos para vídeos (50MB posts, 100MB eventos, 30MB items)
- ⚠️ Validação de duração ainda não implementada (requer metadados de vídeo - futuro)

#### 11. Validação de Áudios (Novo)
- ✅ Máximo 1 áudio por post (validação em `PostCreationService`)
- ✅ Máximo 1 áudio por evento (validação em `EventsService`)
- ✅ Máximo 1 áudio por item (validação em `StoreItemService`)
- ✅ Limites de tamanho específicos para áudios (10MB posts, 20MB eventos, 5MB items)
- ✅ Formatos suportados: MP3, WAV, OGG (validação em `MediaValidator`)
- ⚠️ Validação de duração ainda não implementada (requer metadados de áudio - futuro)

#### 8. Validação de Overlap
- ✅ `CoverMediaId` não pode estar em `AdditionalMediaIds`
- ✅ Validação em `CreateEventRequestValidator`

#### 9. Validação de Existência
- ✅ Todas as mídias devem existir no sistema antes de serem associadas
- ✅ Validação via `IMediaAssetRepository.ListByIdsAsync`

### Auditoria e Logging
- ✅ Todas as operações de mídia são auditadas via `IAuditLogger`
- ✅ Logs estruturados para rastreabilidade

### Exclusão Automática
- ✅ Quando conteúdo é deletado, `MediaAttachment` é deletado automaticamente
- ✅ Implementado em `ReportService` (posts moderados) e `ModerationCaseService` (posts ocultos)
- ✅ Implementado em `EventsService` (eventos cancelados)
- ✅ Implementado em `StoreItemService` (items arquivados)

### Configuração de Blob Storage via Painel Administrativo
- ✅ Modelo de domínio `MediaStorageConfig` com suporte a Local, S3 e AzureBlob
- ✅ Repositório `IMediaStorageConfigRepository` com implementação InMemory
- ✅ Serviço `MediaStorageConfigService` com auditoria completa
- ✅ API Controller `MediaStorageConfigController` com endpoints administrativos (SystemAdmin)
- ✅ Contratos de API (Response, Request) para configuração de storage
- ✅ Segurança: Secrets (AccessKeyId, ConnectionString) mascarados nas respostas
- ⏳ Integração com `MediaStorageFactory` (em implementação - usar configuração do painel quando disponível, fallback para `appsettings.json`)
- 📝 Documentação completa em `FASE10_STORAGE_CONFIG_ADMIN.md`

### Configuração Avançada de Limites de Mídia por Território (Fase 10.9)
- ✅ Modelo `TerritoryMediaConfig` estendido com campos de limites de tamanho e tipos MIME por tipo de mídia e contexto
- ✅ Serviço `TerritoryMediaConfigService` com validação contra `IGlobalMediaLimits` (abstração de `MediaStorageOptions`)
- ✅ Validação de limites mínimos/máximos contra valores globais
- ✅ Fallback automático para valores globais quando limites territoriais não estão configurados
- ✅ Integração completa em `PostCreationService`, `EventsService`, `StoreItemService` e `ChatService`
- ✅ Validação de tipos MIME permitidos por território (override opcional)
- ✅ API `MediaConfigController` com endpoints para atualizar limites de tamanho e tipos MIME
- ✅ Contratos de API (`UpdateTerritoryMediaConfigRequest`) com suporte a tipos MIME permitidos
- ✅ Testes de integração: `MediaConfigIntegrationTests` e `MediaConfigValidationIntegrationTests`
- ⏳ Interface administrativa no DevPortal (pendente - seção de configuração de limites de mídia)
