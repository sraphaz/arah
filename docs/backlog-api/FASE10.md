# Fase 10: Mídias em Conteúdo

**Duração**: 4 semanas (20 dias úteis)  
**Prioridade**: 🔴 CRÍTICA (Bloqueante para edição)  
**Depende de**: Fase 8 (Infraestrutura de Mídia)  
**Bloqueia**: Fase 11 (Edição e Gestão)  
**Estimativa Total**: 160 horas  
**Status**: ⏳ Pendente

---

## 🎯 Objetivo

Integrar mídias (imagens, vídeos) em todas as funcionalidades de conteúdo, permitindo:
- Múltiplas imagens por post
- Imagem de capa em eventos
- Múltiplas imagens por item no marketplace
- Envio de imagens no chat
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
- ✅ Múltiplas imagens por post (até 10 imagens)
- ✅ Ordem de exibição configurável
- ✅ Exclusão de post deleta mídias associadas
- ✅ Visualização de mídias em posts

#### 2. Mídias em Eventos
- ✅ Imagem de capa do evento
- ✅ Múltiplas imagens adicionais (opcional)
- ✅ Exclusão de evento deleta mídias associadas

#### 3. Mídias em Marketplace
- ✅ Múltiplas imagens por item (até 10 imagens)
- ✅ Imagem principal (primeira)
- ✅ Exclusão de item deleta mídias associadas

#### 4. Mídias em Chat
- ✅ Envio de imagens em mensagens
- ✅ Visualização de imagens em chat
- ✅ Validação de tamanho e tipo

---

## 📋 Tarefas Detalhadas

### Semana 35: Mídias em Posts

#### 10.1 Integração de Mídias em Posts
**Estimativa**: 24 horas (3 dias)  
**Status**: ❌ Não implementado

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
**Status**: ❌ Não implementado

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
**Status**: ❌ Não implementado

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
**Status**: ❌ Não implementado

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
**Status**: ❌ Não implementado

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

- **Posts**: Máx. 10 imagens por post
- **Eventos**: 1 imagem de capa + máx. 5 imagens adicionais
- **Marketplace**: Máx. 10 imagens por item
- **Chat**: 1 imagem por mensagem, máx. 5MB

### Exclusão de Mídias

- Quando conteúdo é deletado, `MediaAttachment` é deletado
- `MediaAsset` tem soft delete (pode ser restaurado)
- Se mídia não está mais associada a nenhum conteúdo, pode ser deletada permanentemente (opcional)

---

**Status**: ⏳ **FASE 10 PENDENTE**  
**Depende de**: Fase 8 (Infraestrutura de Mídia)  
**Bloqueia**: Fase 11 (Edição e Gestão)
