# Mídias em Conteúdo

**Versão**: 1.0  
**Data**: 2025-01-17  
**Status**: ✅ Implementado

---

## 📋 Visão Geral

Este documento descreve a integração de mídias em todos os tipos de conteúdo do Araponga:
- **Posts**: Múltiplas imagens por post (até 10)
- **Eventos**: Imagem de capa + imagens adicionais (até 10)
- **Marketplace**: Múltiplas imagens por item (até 10)
- **Chat**: Uma imagem por mensagem (máx. 5MB)

**Status Atual**: Apenas imagens estão implementadas. Suporte a vídeos, áudios e documentos está planejado para a Fase 10.

---

## 🏗️ Arquitetura

### Modelo de Dados

A integração de mídias utiliza duas entidades principais:

#### MediaAsset
Representa um arquivo de mídia armazenado no sistema:
- `Id`: Identificador único
- `UploadedByUserId`: Usuário que fez upload
- `MediaType`: Tipo (Image, Video, Audio, Document)
- `MimeType`: Tipo MIME do arquivo
- `StorageKey`: Chave no storage (S3, Azure Blob, Local)
- `SizeBytes`: Tamanho em bytes
- `WidthPx`, `HeightPx`: Dimensões (para imagens)
- `Checksum`: Hash para verificação de integridade
- `IsDeleted`: Soft delete

#### MediaAttachment
Representa a associação de uma mídia a um conteúdo:
- `Id`: Identificador único
- `MediaAssetId`: Referência ao MediaAsset
- `OwnerType`: Tipo do dono (Post, Event, StoreItem, ChatMessage)
- `OwnerId`: ID da entidade dona
- `DisplayOrder`: Ordem de exibição (0 = primeira/imagem principal)

### Fluxo de Dados

```
1. Upload de Mídia
   POST /api/v1/media/upload
   → Retorna MediaAssetId

2. Criação de Conteúdo com Mídias
   POST /api/v1/feed/posts { mediaIds: [Guid] }
   → Cria MediaAttachments automaticamente

3. Visualização de Conteúdo
   GET /api/v1/feed
   → Retorna URLs de mídias nas respostas
```

---

## 📝 Integração por Tipo de Conteúdo

### 1. Posts (Feed)

#### Request
```json
POST /api/v1/feed/posts
{
  "title": "Post com imagens",
  "content": "Descrição do post",
  "type": "General",
  "visibility": "Public",
  "mediaIds": [
    "guid-1",
    "guid-2",
    "guid-3"
  ]
}
```

#### Response
```json
{
  "id": "post-id",
  "title": "Post com imagens",
  "content": "Descrição do post",
  "mediaUrls": [
    "https://storage.../image1.jpg",
    "https://storage.../image2.jpg",
    "https://storage.../image3.jpg"
  ],
  "mediaCount": 3
}
```

#### Limites
- **Máximo**: 10 imagens por post
- **Validação**: Todas as mídias devem pertencer ao usuário
- **Ordem**: `DisplayOrder` = índice do array

#### Implementação
- **Service**: `PostCreationService.CreatePostAsync`
- **Controller**: `FeedController.CreatePost`
- **Repository**: `IMediaAttachmentRepository.AddAsync`

---

### 2. Eventos

#### Request
```json
POST /api/v1/events
{
  "title": "Evento com imagens",
  "description": "Descrição do evento",
  "startsAtUtc": "2025-02-01T10:00:00Z",
  "coverMediaId": "cover-guid",
  "additionalMediaIds": [
    "additional-1",
    "additional-2"
  ]
}
```

#### Response
```json
{
  "id": "event-id",
  "title": "Evento com imagens",
  "coverImageUrl": "https://storage.../cover.jpg",
  "additionalImageUrls": [
    "https://storage.../img1.jpg",
    "https://storage.../img2.jpg"
  ]
}
```

#### Limites
- **Capa**: 1 imagem (opcional)
- **Adicionais**: Até 10 imagens
- **Validação**: Todas as mídias devem pertencer ao usuário
- **Ordem**: Capa = `DisplayOrder` 0, adicionais = 1+

#### Implementação
- **Service**: `EventsService.CreateEventAsync`
- **Controller**: `EventsController.CreateEvent`
- **Repository**: `IMediaAttachmentRepository.AddAsync`

---

### 3. Marketplace (Items)

#### Request
```json
POST /api/v1/items
{
  "title": "Produto com imagens",
  "description": "Descrição do produto",
  "type": "Product",
  "pricingType": "Fixed",
  "priceAmount": 100.00,
  "mediaIds": [
    "primary-guid",
    "secondary-1",
    "secondary-2"
  ]
}
```

#### Response
```json
{
  "id": "item-id",
  "title": "Produto com imagens",
  "primaryImageUrl": "https://storage.../primary.jpg",
  "imageUrls": [
    "https://storage.../img1.jpg",
    "https://storage.../img2.jpg"
  ]
}
```

#### Limites
- **Máximo**: 10 imagens por item
- **Principal**: Primeira imagem (`DisplayOrder` = 0)
- **Validação**: Todas as mídias devem pertencer ao usuário

#### Implementação
- **Service**: `StoreItemService.CreateItemAsync`
- **Controller**: `ItemsController.CreateItem`
- **Repository**: `IMediaAttachmentRepository.AddAsync`

---

### 4. Chat

#### Request
```json
POST /api/v1/chat/conversations/{id}/messages
{
  "text": "Mensagem com imagem",
  "mediaId": "image-guid"
}
```

#### Response
```json
{
  "id": "message-id",
  "text": "Mensagem com imagem",
  "mediaUrl": "https://storage.../image.jpg",
  "hasMedia": true
}
```

#### Limites
- **Máximo**: 1 imagem por mensagem
- **Tipo**: Apenas imagens (não vídeos)
- **Tamanho**: Máximo 5MB
- **Validação**: Mídia deve pertencer ao usuário

#### Implementação
- **Service**: `ChatService.SendTextMessageAsync`
- **Controller**: `ChatController.SendMessage`
- **Repository**: `IMediaAttachmentRepository.AddAsync`

---

## 🔒 Validações

### Validação de Propriedade
Todos os serviços validam que:
1. O `MediaAsset` existe
2. O `MediaAsset.UploadedByUserId` corresponde ao usuário atual
3. O `MediaAsset.IsDeleted` é `false`

### Validação de Limites
- **Posts**: Máx. 10 mídias
- **Eventos**: 1 capa + máx. 10 adicionais
- **Items**: Máx. 10 mídias
- **Chat**: 1 mídia, máx. 5MB

### Validação de Tipo
- **Chat**: Apenas `MediaType.Image`
- **Outros**: Qualquer tipo (mas recomendado apenas imagens)

---

## 🔍 Busca de URLs de Mídia

### Padrão de Implementação

Todos os controllers usam helpers para buscar URLs em batch:

```csharp
private async Task<List<string>> LoadMediaUrlsForPostsAsync(
    IEnumerable<Guid> postIds,
    CancellationToken cancellationToken)
{
    // 1. Buscar MediaAttachments em batch
    var attachments = await _mediaService
        .ListMediaByOwnerAsync(MediaOwnerType.Post, postIds, cancellationToken);

    // 2. Buscar URLs em batch
    var urls = new List<string>();
    foreach (var attachment in attachments)
    {
        var urlResult = await _mediaService
            .GetMediaUrlAsync(attachment.MediaAssetId, null, cancellationToken);
        if (urlResult.IsSuccess)
        {
            urls.Add(urlResult.Value);
        }
    }

    return urls;
}
```

### Otimizações

- **Batch Loading**: Busca múltiplas mídias em uma query
- **Cache**: URLs são cacheadas via `CachedMediaStorageService`
- **Lazy Loading**: URLs são buscadas apenas quando necessário

---

## 🗑️ Exclusão de Mídias

### Estado Atual

A exclusão automática de mídias quando conteúdo é deletado **não está implementada** nesta fase.

### Comportamento Atual

- **Posts**: Soft delete (`UpdateStatusAsync(PostStatus.Deleted)`)
- **Eventos**: Cancelamento (`CancelEvent`) - não deleta
- **Items**: Arquivamento (`ArchiveItem`) - não deleta
- **Chat Messages**: Soft delete (campos `DeletedAtUtc`, `DeletedByUserId`)

### Recomendação Futura

Implementar exclusão de mídias via:
1. **Event Handlers**: Escutar eventos de exclusão de conteúdo
2. **Database Triggers**: Trigger para deletar `MediaAttachment`
3. **Background Job**: Job periódico para limpar mídias órfãs

---

## 📊 Performance

### Métricas Esperadas

- **Feed com mídias**: < 500ms
- **Batch loading**: < 100ms para 10 posts
- **Cache hit rate**: > 80%

### Otimizações Implementadas

1. **Batch Queries**: Busca múltiplas mídias em uma query
2. **Cache de URLs**: URLs cacheadas por 1 hora
3. **Lazy Loading**: URLs buscadas apenas quando necessário

---

## 🧪 Testes

### Testes Recomendados

#### Unit Tests
- Validação de propriedade de mídias
- Validação de limites (máx. 10 mídias)
- Criação de MediaAttachments

#### Integration Tests
- Criação de post com mídias
- Criação de evento com capa e adicionais
- Criação de item com múltiplas imagens
- Envio de mensagem com imagem

#### Performance Tests
- Feed com 100 posts e mídias
- Batch loading de URLs
- Cache hit rate

---

## 📚 Referências

- **Sistema de Mídia**: `docs/MEDIA_SYSTEM.md`
- **Especificação**: `docs/backlog-api/FASE10.md`
- **Implementação**: `docs/backlog-api/implementacoes/FASE10_IMPLEMENTACAO_COMPLETA.md`
- **Changelog**: `docs/40_CHANGELOG.md`

---

## 🔄 Changelog

### 2025-01-17 - Implementação Inicial
- ✅ Mídias em Posts (até 10 imagens)
- ✅ Mídias em Eventos (capa + adicionais)
- ✅ Mídias em Marketplace (até 10 imagens)
- ✅ Mídias em Chat (1 imagem, máx. 5MB)
- ✅ Validações de propriedade e limites
- ✅ Busca de URLs em batch
- ⏳ Exclusão automática (pendente)

---

**Status**: ✅ **IMPLEMENTAÇÃO PRINCIPAL COMPLETA**  
**Próximos Passos**: Testes de integração, otimizações, exclusão automática
