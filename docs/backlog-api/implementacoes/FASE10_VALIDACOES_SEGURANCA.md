# Fase 10: Validações de Segurança Avançadas

## Resumo

Este documento descreve todas as validações de segurança implementadas para mídias em conteúdo na Fase 10.

---

## 1. Validações Implementadas

### 1.1 Validações de Propriedade ✅

**Localização**: `PostCreationService`, `EventsService`, `StoreItemService`, `ChatService`

**Validações**:
- ✅ Usuário só pode usar mídias que ele mesmo fez upload (`UploadedByUserId == userId`)
- ✅ Mídias deletadas não podem ser usadas (`IsDeleted == false`)
- ✅ Validação ocorre antes de criar associações (`MediaAttachment`)

**Código**:
```csharp
if (mediaAssets.Any(media => media.UploadedByUserId != userId || media.IsDeleted))
{
    return Result<CommunityPost>.Failure("One or more media assets are invalid or do not belong to the user.");
}
```

### 1.2 Validações de Limites ✅

**Localização**: FluentValidation Validators + Application Services

**Validações**:
- ✅ **Posts**: Máximo 10 mídias por post
- ✅ **Eventos**: Máximo 5 mídias adicionais + 1 capa (total 6)
- ✅ **Marketplace Items**: Máximo 10 mídias por item
- ✅ **Chat**: Apenas 1 mídia por mensagem (tipo `Guid?`)

**Validators**:
- `CreatePostRequestValidator`: Valida limite de 10 mídias
- `CreateEventRequestValidator`: Valida limite de 5 mídias adicionais
- `CreateItemRequestValidator`: Valida limite de 10 mídias

### 1.3 Validações de Existência ✅

**Localização**: Application Services

**Validações**:
- ✅ Mídias devem existir no repositório (`ListByIdsAsync`)
- ✅ Contagem de mídias encontradas deve corresponder à contagem de IDs fornecidos
- ✅ GUIDs vazios são filtrados antes da validação (`.Where(id => id != Guid.Empty)`)

**Código**:
```csharp
var mediaAssets = await _mediaAssetRepository.ListByIdsAsync(normalizedMediaIds, cancellationToken);
if (mediaAssets.Count != normalizedMediaIds.Count)
{
    return Result<CommunityPost>.Failure("One or more media assets not found.");
}
```

### 1.4 Validações de Tipo de Mídia ✅

**Localização**: `ChatService`

**Validações**:
- ✅ Chat aceita apenas imagens (`MediaType.Image`)
- ✅ Outros tipos de mídia são rejeitados em mensagens de chat

**Código**:
```csharp
if (mediaAsset.MediaType != MediaType.Image)
{
    return OperationResult<ChatMessage>.Failure("Only images are allowed in chat messages.");
}
```

### 1.5 Validações de Tamanho ✅

**Localização**: `ChatService` + `MediaValidator`

**Validações**:
- ✅ **Chat**: Máximo 5MB por imagem (`SizeBytes > 5 * 1024 * 1024`)
- ✅ **Upload**: Validação no `MediaValidator` com limites por tipo MIME
  - Imagens: Configurável via `MediaStorageOptions.MaxImageSizeBytes`
  - Vídeos: Configurável via `MediaStorageOptions.MaxVideoSizeBytes`
  - Validação de MIME types permitidos

**Código**:
```csharp
// Chat
if (mediaAsset.SizeBytes > 5 * 1024 * 1024)
{
    return OperationResult<ChatMessage>.Failure("Image size exceeds 5MB limit for chat.");
}

// Upload (MediaValidator)
if (sizeBytes > maxSize)
{
    errors.Add($"Arquivo excede o tamanho máximo permitido de {maxSizeMB:F1}MB.");
}
```

### 1.6 Validações de Duplicatas ✅

**Localização**: FluentValidation Validators

**Validações**:
- ✅ IDs duplicados em `MediaIds` são rejeitados
- ✅ `CoverMediaId` não pode estar duplicado em `AdditionalMediaIds` (Eventos)
- ✅ GUIDs vazios são rejeitados

**Validators Atualizados**:
- `CreatePostRequestValidator`: Valida duplicatas em `MediaIds`
- `CreateEventRequestValidator`: Valida duplicatas em `AdditionalMediaIds` e overlap com `CoverMediaId`
- `CreateItemRequestValidator`: Valida duplicatas em `MediaIds`

**Código**:
```csharp
RuleFor(x => x.MediaIds!)
    .Must(mediaIds => mediaIds.Distinct().Count() == mediaIds.Count)
    .WithMessage("MediaIds cannot contain duplicate values.");
```

### 1.7 Validações de MIME Types ✅

**Localização**: `MediaValidator`

**Validações**:
- ✅ Apenas MIME types permitidos são aceitos
- ✅ Lista configurável via `MediaStorageOptions`:
  - `AllowedImageMimeTypes`: Tipos de imagem permitidos
  - `AllowedVideoMimeTypes`: Tipos de vídeo permitidos
- ✅ Validação ocorre no upload, antes de armazenar

**Código**:
```csharp
if (!IsAllowedMimeType(mimeType))
{
    errors.Add($"Tipo MIME '{mimeType}' não é permitido.");
}
```

---

## 2. Validações de Segurança Adicionais

### 2.1 Sanções e Restrições ✅

**Localização**: `PostCreationService`

**Validações**:
- ✅ Usuários com `PostingRestriction` não podem criar posts (incluindo posts com mídias)
- ✅ Feature flags validados antes de criar conteúdo

### 2.2 Autorização de Acesso ✅

**Localização**: Controllers + Services

**Validações**:
- ✅ Apenas usuários autenticados podem associar mídias a conteúdo
- ✅ Validação de membership e permissões de território
- ✅ Validação de propriedade de store (Marketplace)

### 2.3 Auditoria ✅

**Localização**: Services

**Validações**:
- ✅ Todas as operações de mídia são auditadas (`AuditLogger`)
- ✅ Operações registradas: `media.uploaded`, `media.deleted`
- ✅ Rastreabilidade completa de ações

---

## 3. Camadas de Validação

### 3.1 Camada de API (FluentValidation) ✅
- Validação de limites (máx 10, máx 5)
- Validação de duplicatas
- Validação de GUIDs vazios
- Validação de tipos e formatos

### 3.2 Camada de Aplicação (Services) ✅
- Validação de propriedade (`UploadedByUserId`)
- Validação de estado (`IsDeleted`)
- Validação de existência
- Validação de tipo de mídia (Chat)
- Validação de tamanho (Chat)
- Validação de permissões e sanções

### 3.3 Camada de Infraestrutura (MediaValidator) ✅
- Validação de MIME types
- Validação de tamanho de arquivo
- Validação de formato de arquivo

---

## 4. Cenários de Segurança Cobertos

### 4.1 Prevenção de Ataques

✅ **Acesso Não Autorizado**: Usuários não podem usar mídias de outros usuários
✅ **Upload Malicioso**: MIME types e tamanhos validados no upload
✅ **DoS por Tamanho**: Limites de tamanho por tipo de conteúdo
✅ **DoS por Quantidade**: Limites de quantidade de mídias por conteúdo
✅ **Injeção de Dados**: GUIDs validados e duplicatas rejeitadas
✅ **Uso de Mídias Deletadas**: Mídias deletadas não podem ser associadas

### 4.2 Integridade de Dados

✅ **Duplicatas**: IDs duplicados são rejeitados
✅ **Referências Inválidas**: GUIDs vazios e IDs inexistentes são rejeitados
✅ **Estado Consistente**: Mídias deletadas não podem ser usadas
✅ **Propriedade Consistente**: Apenas o proprietário pode usar suas mídias

---

## 5. Validações Específicas por Tipo de Conteúdo

### 5.1 Posts
- ✅ Máximo 10 mídias
- ✅ Propriedade validada
- ✅ Mídias não deletadas
- ✅ Sem duplicatas

### 5.2 Eventos
- ✅ Máximo 5 mídias adicionais
- ✅ 1 mídia de capa opcional
- ✅ CoverMediaId não pode estar em AdditionalMediaIds
- ✅ Propriedade validada
- ✅ Mídias não deletadas
- ✅ Sem duplicatas

### 5.3 Marketplace Items
- ✅ Máximo 10 mídias
- ✅ Propriedade validada
- ✅ Mídias não deletadas
- ✅ Sem duplicatas

### 5.4 Chat Messages
- ✅ Apenas 1 mídia por mensagem
- ✅ Apenas imagens (`MediaType.Image`)
- ✅ Máximo 5MB por imagem
- ✅ Propriedade validada
- ✅ Mídias não deletadas

---

## 6. Validações de Segurança Implementadas Recentemente

### 6.1 Validação de Duplicatas ✅ (NOVO)

**Arquivos Modificados**:
- `CreatePostRequestValidator.cs`: Validação de duplicatas em `MediaIds`
- `CreateEventRequestValidator.cs`: Validação de duplicatas e overlap `CoverMediaId`
- `CreateItemRequestValidator.cs`: Validação de duplicatas em `MediaIds`

**Código Adicionado**:
```csharp
RuleFor(x => x.MediaIds!)
    .Must(mediaIds => mediaIds.Distinct().Count() == mediaIds.Count)
    .WithMessage("MediaIds cannot contain duplicate values.");

RuleFor(x => x.MediaIds!)
    .Must(mediaIds => mediaIds.All(id => id != Guid.Empty))
    .WithMessage("MediaIds cannot contain empty GUIDs.");
```

---

## 7. Resumo de Segurança

### ✅ Implementado
- Validação de propriedade
- Validação de estado (IsDeleted)
- Validação de limites de quantidade
- Validação de limites de tamanho
- Validação de tipo de mídia
- Validação de existência
- Validação de duplicatas
- Validação de GUIDs vazios
- Validação de MIME types
- Auditoria de operações

### 📝 Recomendações Futuras (Não Críticas)

1. **Rate Limiting por Usuário**: Limitar uploads de mídia por usuário por período
2. **Validação de Dimensões**: Validar dimensões máximas de imagens (ex: 4000x4000px)
3. **Validação de Conteúdo**: Scan de mídias por conteúdo malicioso (ex: vírus, conteúdo impróprio)
4. **Validação de Watermark**: Adicionar watermark automático em imagens
5. **Compressão Automática**: Comprimir imagens acima de certo tamanho
6. **CDN Cache**: Cache de URLs de mídia em CDN para performance

---

## 8. Conclusão

✅ **Todas as validações de segurança críticas estão implementadas**:
- Propriedade de mídias
- Estado de mídias
- Limites de quantidade e tamanho
- Tipo de mídia
- Duplicatas e GUIDs inválidos
- MIME types

✅ **Camadas múltiplas de validação**:
- API (FluentValidation)
- Application (Services)
- Infrastructure (MediaValidator)

✅ **Cobertura completa de cenários de segurança**:
- Acesso não autorizado
- Ataques DoS
- Integridade de dados
- Auditoria

**Status**: ✅ **Validações de segurança avançadas implementadas e documentadas**
