# Moderação Comunitária Dinâmica

**Versão**: 1.0  
**Data**: 2025-01-21  
**Status**: ✅ Implementado

---

## 📋 Visão Geral

O Sistema de Moderação Comunitária Dinâmica permite que comunidades territoriais definam suas próprias regras de moderação através de votações ou diretamente por curadores. As regras são aplicadas automaticamente na criação de conteúdo.

---

## 🎯 Tipos de Regras

### 1. ContentType (Tipo de Conteúdo)
**Objetivo**: Definir quais tipos de conteúdo são permitidos.

**Exemplo**:
```json
{
  "allowedTypes": ["General", "Event", "Alert"],
  "prohibitedTypes": ["Spam", "Advertisement"]
}
```

**Aplicação**: Verifica tipo de post/item antes de criar.

---

### 2. ProhibitedWords (Palavras Proibidas)
**Objetivo**: Proibir palavras ou temas específicos.

**Exemplo**:
```json
{
  "words": ["spam", "scam", "fraud"],
  "caseSensitive": false,
  "checkTitle": true,
  "checkContent": true
}
```

**Aplicação**: Verifica título e conteúdo de posts/items.

---

### 3. Behavior (Comportamento)
**Objetivo**: Regras de comportamento e convivência.

**Exemplo**:
```json
{
  "rules": [
    "No harassment",
    "No hate speech",
    "Respectful communication"
  ]
}
```

**Aplicação**: Validações comportamentais (implementação futura).

---

### 4. MarketplacePolicy (Política de Marketplace)
**Objetivo**: Regras específicas para marketplace.

**Exemplo**:
```json
{
  "allowedCategories": ["Food", "Services"],
  "prohibitedItems": ["Weapons", "Drugs"],
  "priceLimits": {
    "min": 0,
    "max": 10000
  }
}
```

**Aplicação**: Verifica items de marketplace antes de criar.

---

### 5. EventPolicy (Política de Eventos)
**Objetivo**: Regras específicas para eventos.

**Exemplo**:
```json
{
  "maxDurationHours": 24,
  "requireLocation": true,
  "allowedTypes": ["Community", "Public"]
}
```

**Aplicação**: Verifica eventos antes de criar (implementação futura).

---

## 🔄 Fluxo de Aplicação

### 1. Criação de Regra

#### Via Votação
1. Curador cria votação do tipo `ModerationRule`
2. Comunidade vota
3. Se aprovado, regra é criada automaticamente
4. `CreatedByVotingId` aponta para a votação

#### Diretamente (Curador)
1. Curador cria regra diretamente
2. `CreatedByVotingId` é `null`
3. Regra ativa imediatamente

### 2. Aplicação Automática

#### Ao Criar Post
```csharp
// PostCreationService verifica regras antes de criar
var violation = await moderationService.CheckRuleViolationAsync(
    territoryId: territoryId,
    contentType: postType,
    title: title,
    content: content);

if (violation != null)
{
    return Result<CommunityPost>.Failure(violation);
}
```

#### Ao Criar Item
```csharp
// StoreItemService verifica regras antes de criar
var violation = await moderationService.CheckMarketplaceRuleViolationAsync(
    territoryId: territoryId,
    title: title,
    description: description,
    category: category);

if (violation != null)
{
    return Result<StoreItem>.Failure(violation);
}
```

### 3. Validação de Regras

O sistema verifica todas as regras ativas do território:
1. Filtra regras ativas (`IsActive == true`)
2. Verifica cada regra conforme tipo
3. Retorna primeira violação encontrada
4. Se nenhuma violação: permite criação

---

## 📊 Modelo de Dados

### TerritoryModerationRule
```csharp
public sealed class TerritoryModerationRule
{
    public Guid Id { get; }
    public Guid TerritoryId { get; }
    public Guid? CreatedByVotingId { get; } // Nullable
    public RuleType RuleType { get; }
    public string Rule { get; } // JSON com configuração
    public bool IsActive { get; }
    public DateTime CreatedAtUtc { get; }
    public DateTime UpdatedAtUtc { get; }
}
```

### RuleType (Enum)
```csharp
public enum RuleType
{
    ContentType,
    ProhibitedWords,
    Behavior,
    MarketplacePolicy,
    EventPolicy
}
```

---

## 🔌 API Endpoints

### Listar Regras
```http
GET /api/v1/territories/{territoryId}/moderation-rules?isActive=true
```

**Resposta**:
```json
[
  {
    "id": "...",
    "territoryId": "...",
    "createdByVotingId": "...",
    "ruleType": "ProhibitedWords",
    "rule": "{\"words\": [\"spam\", \"scam\"]}",
    "isActive": true,
    "createdAtUtc": "2025-01-21T10:00:00Z"
  }
]
```

### Criar Regra (Curador)
```http
POST /api/v1/territories/{territoryId}/moderation-rules
Content-Type: application/json

{
  "ruleType": "ProhibitedWords",
  "rule": "{\"words\": [\"spam\", \"scam\"]}"
}
```

### Atualizar Regra (Curador)
```http
PUT /api/v1/territories/{territoryId}/moderation-rules/{id}
Content-Type: application/json

{
  "rule": "{\"words\": [\"spam\", \"scam\", \"fraud\"]}",
  "isActive": true
}
```

### Desativar Regra (Curador)
```http
DELETE /api/v1/territories/{territoryId}/moderation-rules/{id}
```

---

## 🧪 Exemplos de Uso

### Exemplo 1: Proibir Palavras

```csharp
// 1. Curador cria regra
var rule = new TerritoryModerationRule(
    id: Guid.NewGuid(),
    territoryId: territoryId,
    createdByVotingId: null, // Criada diretamente
    ruleType: RuleType.ProhibitedWords,
    rule: JsonSerializer.Serialize(new
    {
        words = new[] { "spam", "scam", "fraud" },
        caseSensitive = false,
        checkTitle = true,
        checkContent = true
    }),
    isActive: true,
    createdAtUtc: DateTime.UtcNow,
    updatedAtUtc: DateTime.UtcNow);

await ruleRepository.AddAsync(rule);

// 2. Usuário tenta criar post com palavra proibida
var result = await postCreationService.CreatePostAsync(
    territoryId: territoryId,
    userId: userId,
    title: "Oferta imperdível! Não é spam!",
    content: "...",
    // ...
);

// 3. Sistema detecta violação
Assert.True(result.IsFailure);
Assert.Contains("spam", result.Error);
```

### Exemplo 2: Regra via Votação

```csharp
// 1. Curador cria votação
var voting = await votingService.CreateVotingAsync(
    territoryId: territoryId,
    userId: curatorId,
    type: VotingType.ModerationRule,
    title: "Proibir posts sobre política partidária",
    description: "Devemos proibir posts sobre política partidária?",
    options: new[] { "Aprovar", "Rejeitar" },
    visibility: VotingVisibility.ResidentsOnly,
    startsAtUtc: null,
    endsAtUtc: null);

// 2. Abrir e votar
voting.Open();
// ... residents votam ...

// 3. Fechar votação
await votingService.CloseVotingAsync(voting.Id, curatorId);

// 4. Se aprovado, regra é criada automaticamente
// CreatedByVotingId aponta para a votação
```

---

## 🔒 Permissões

### Criar Regra
- **Via Votação**: Qualquer membro pode propor (votação do tipo `ModerationRule`)
- **Diretamente**: Apenas curadores

### Atualizar/Desativar Regra
- Apenas curadores

### Visualizar Regras
- Todos os membros do território (regras são públicas)

---

## 📈 Métricas e Analytics

### Métricas Disponíveis
- Total de regras por território
- Regras ativas vs inativas
- Regras criadas via votações vs diretamente
- Violações detectadas (implementação futura)

---

## 🔗 Integração com Outros Sistemas

### PostCreationService
- Verifica regras antes de criar post
- Retorna erro detalhado se violar regra

### StoreItemService
- Verifica regras antes de criar item
- Retorna erro detalhado se violar regra

### VotingService
- Cria regras automaticamente quando votação `ModerationRule` é aprovada

---

## 🔗 Referências

- [GOVERNANCE_SYSTEM.md](./GOVERNANCE_SYSTEM.md): Visão geral do sistema de governança
- [VOTING_SYSTEM.md](./VOTING_SYSTEM.md): Sistema de votação
- [FASE14.md](../backlog-api/FASE14.md): Especificação completa

---

**Última Atualização**: 2025-01-21
