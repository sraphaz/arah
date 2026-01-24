# Plano de Implementação: Tags Explícitas em Posts

**Última Atualização**: 2025-01-23  
**Status**: 📋 Planejado (Opcional)

---

## 📋 Resumo

Este documento descreve o plano para adicionar tags/categorias explícitas aos posts, permitindo filtragem mais precisa do feed por interesses.

---

## 🎯 Objetivo

Permitir que posts tenham tags explícitas (ex: "meio ambiente", "eventos", "saúde") que podem ser usadas para filtrar o feed de forma mais precisa do que a busca por palavras-chave no título/conteúdo.

---

## 📐 Design

### Modelo de Dados

#### Domain Model

```csharp
// Adicionar ao CommunityPost
public IReadOnlyList<string> Tags { get; private set; }

// Construtor atualizado
public CommunityPost(
    ...,
    IReadOnlyList<string>? tags = null)
{
    // ...
    Tags = tags?.ToList() ?? new List<string>();
}
```

#### Database Schema

```sql
-- Adicionar coluna tags (array de strings)
ALTER TABLE community_posts 
ADD COLUMN tags TEXT[];

-- Criar índice GIN para busca eficiente
CREATE INDEX idx_community_posts_tags_gin 
ON community_posts USING GIN(tags);
```

---

## 🔄 Implementação

### 1. Atualizar Domain Model

**Arquivo**: `backend/Araponga.Domain/Feed/CommunityPost.cs`

- Adicionar propriedade `Tags`
- Atualizar construtor
- Adicionar método `UpdateTags()`

### 2. Atualizar Database Record

**Arquivo**: `backend/Araponga.Infrastructure/Postgres/Entities/CommunityPostRecord.cs`

- Adicionar propriedade `Tags` (string[])

### 3. Criar Migration

**Arquivo**: `backend/Araponga.Infrastructure/Postgres/Migrations/YYYYMMDDHHMMSS_AddPostTags.cs`

- Adicionar coluna `tags TEXT[]`
- Criar índice GIN
- Migrar dados existentes (opcional)

### 4. Atualizar InterestFilterService

**Arquivo**: `backend/Araponga.Application/Services/InterestFilterService.cs`

```csharp
public async Task<IReadOnlyList<CommunityPost>> FilterFeedByInterestsAsync(
    IReadOnlyList<CommunityPost> posts,
    Guid userId,
    Guid territoryId,
    CancellationToken cancellationToken)
{
    // ...
    
    // Priorizar tags explícitas se disponíveis
    var filtered = posts
        .Where(post =>
        {
            // Se post tem tags, verificar match com interesses
            if (post.Tags.Count > 0)
            {
                return post.Tags.Any(tag => 
                    interestTags.Contains(tag.ToLowerInvariant()));
            }
            
            // Fallback para busca em título/conteúdo (comportamento atual)
            var titleLower = post.Title?.ToLowerInvariant() ?? "";
            var contentLower = post.Content?.ToLowerInvariant() ?? "";
            return interestTags.Any(tag =>
                titleLower.Contains(tag) || contentLower.Contains(tag));
        })
        .ToList();
    
    return filtered;
}
```

### 5. Atualizar API Contracts

**Arquivo**: `backend/Araponga.Api/Contracts/Feed/CreatePostRequest.cs`

- Adicionar campo opcional `Tags` (string[])

**Arquivo**: `backend/Araponga.Api/Contracts/Feed/PostResponse.cs`

- Adicionar campo `Tags` (string[])

### 6. Atualizar Validators

**Arquivo**: `backend/Araponga.Api/Validators/CreatePostRequestValidator.cs`

- Validar tags (máx. 10 tags, máx. 50 caracteres por tag)

---

## 📊 Benefícios

1. **Filtragem Mais Precisa**: Tags explícitas são mais confiáveis que busca por palavras-chave
2. **Melhor Performance**: Índice GIN permite busca rápida mesmo com muitos posts
3. **Compatibilidade**: Mantém comportamento atual (busca em título/conteúdo) como fallback
4. **Extensibilidade**: Permite categorização futura e taxonomias

---

## ⚠️ Considerações

### Compatibilidade

- Posts antigos sem tags continuam funcionando (fallback para busca textual)
- Filtro por interesses funciona com ou sem tags

### Performance

- Índice GIN é eficiente para arrays
- Queries com tags são mais rápidas que busca textual

### UX

- Tags podem ser sugeridas baseadas em interesses do usuário
- Tags podem ser autocompletadas ao criar post

---

## 📚 Referências

- [PostgreSQL Array Types](https://www.postgresql.org/docs/current/arrays.html)
- [PostgreSQL GIN Indexes](https://www.postgresql.org/docs/current/gin.html)
- [InterestFilterService Implementation](../backend/Araponga.Application/Services/InterestFilterService.cs)

---

**Nota**: Esta é uma evolução futura. O sistema atual funciona bem com busca textual. Tags explícitas podem ser adicionadas quando houver necessidade de categorização mais estruturada.
