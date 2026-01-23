# Sistema de Governança Comunitária

**Versão**: 1.0  
**Data**: 2025-01-21  
**Status**: ✅ Implementado

---

## 📋 Visão Geral

O Sistema de Governança Comunitária permite que comunidades territoriais tomem decisões coletivas através de votações, definam regras de moderação dinâmicas e personalizem seus feeds através de interesses.

### Princípios Fundamentais

1. **Soberania Territorial**: Cada território tem autonomia para definir suas próprias regras
2. **Transparência**: Todas as votações e regras são públicas e auditáveis
3. **Participação Democrática**: Membros da comunidade participam das decisões
4. **Feed Cronológico Preservado**: O feed completo permanece disponível, filtros são opcionais

---

## 🏗️ Arquitetura

### Componentes Principais

#### 1. Sistema de Interesses (`UserInterest`)
- Permite que usuários definam tags de interesse
- Personaliza o feed (opcional)
- Aparece no perfil do usuário

**Modelo de Domínio**:
```csharp
public sealed class UserInterest
{
    public Guid Id { get; }
    public Guid UserId { get; }
    public string InterestTag { get; } // Máx. 50 caracteres, lowercase
    public DateTime CreatedAtUtc { get; }
}
```

**Limites**:
- Máximo 10 interesses por usuário
- Tags normalizadas (trim, lowercase)
- Validação: apenas letras, números, espaços e hífens

#### 2. Sistema de Votação (`Voting`, `Vote`)
- Votações para decisões comunitárias
- Múltiplos tipos de votação
- Controle de visibilidade e permissões

**Tipos de Votação**:
- `ThemePrioritization`: Priorização de temas no feed
- `ModerationRule`: Criação de regras de moderação
- `TerritoryCharacterization`: Caracterização do território (tags)
- `FeatureFlag`: Habilitação/desabilitação de features
- `CommunityPolicy`: Políticas comunitárias

**Status de Votação**:
- `Draft`: Rascunho (não visível)
- `Open`: Aberta para votação
- `Closed`: Fechada (resultados disponíveis)
- `Approved`: Aprovada (resultados aplicados)
- `Rejected`: Rejeitada

**Visibilidade**:
- `AllMembers`: Todos os membros podem votar
- `ResidentsOnly`: Apenas residentes
- `CuratorsOnly`: Apenas curadores

#### 3. Moderação Dinâmica (`TerritoryModerationRule`)
- Regras definidas pela comunidade
- Aplicadas automaticamente na criação de conteúdo
- Podem ser criadas via votações

**Tipos de Regra**:
- `ContentType`: Tipos de conteúdo permitidos
- `ProhibitedWords`: Palavras/temas proibidos
- `Behavior`: Regras de comportamento
- `MarketplacePolicy`: Políticas de marketplace
- `EventPolicy`: Políticas de eventos

#### 4. Caracterização do Território (`TerritoryCharacterization`)
- Tags que descrevem o território
- Podem ser definidas via votações
- Aparecem nas respostas de território

---

## 🔌 API Endpoints

### Interesses do Usuário

#### `GET /api/v1/users/me/interests`
Lista interesses do usuário autenticado.

**Resposta**: `IReadOnlyList<string>`

#### `POST /api/v1/users/me/interests`
Adiciona um interesse ao usuário.

**Request**: `AddInterestRequest { InterestTag: string }`

**Validações**:
- Tag não vazia
- Máximo 50 caracteres
- Apenas letras minúsculas, números, espaços e hífens
- Máximo 10 interesses por usuário

#### `DELETE /api/v1/users/me/interests/{tag}`
Remove um interesse do usuário.

---

### Votações

#### `POST /api/v1/territories/{territoryId}/votings`
Cria uma nova votação.

**Request**: `CreateVotingRequest`
```json
{
  "type": "ThemePrioritization",
  "title": "Priorizar temas",
  "description": "Qual tema deve ter prioridade?",
  "options": ["Meio Ambiente", "Eventos"],
  "visibility": "AllMembers",
  "startsAtUtc": null,
  "endsAtUtc": null
}
```

**Permissões**:
- `ThemePrioritization`: Apenas residents
- `ModerationRule`: Apenas curadores
- `TerritoryCharacterization`: Apenas residents
- `FeatureFlag`: Apenas curadores
- `CommunityPolicy`: Apenas residents

#### `GET /api/v1/territories/{territoryId}/votings`
Lista votações de um território.

**Query Parameters**:
- `status`: Filtrar por status (opcional)
- `userId`: Filtrar por criador (opcional)

#### `GET /api/v1/votings/{id}`
Obtém detalhes de uma votação.

#### `POST /api/v1/votings/{id}/vote`
Registra um voto.

**Request**: `VoteRequest { SelectedOption: string }`

**Validações**:
- Votação deve estar aberta
- Usuário deve ter permissão (visibilidade)
- Opção selecionada deve existir
- Usuário só pode votar uma vez

#### `POST /api/v1/votings/{id}/close`
Fecha uma votação (apenas criador ou curador).

#### `GET /api/v1/votings/{id}/results`
Obtém resultados de uma votação.

**Resposta**: `VotingResultsResponse { Results: Dictionary<string, int> }`

---

## 🔄 Fluxos de Trabalho

### Criar e Executar uma Votação

1. **Criar Votação**: `POST /api/v1/territories/{id}/votings`
   - Criador deve ter permissão (resident/curador conforme tipo)
   - Votação criada com status `Draft`

2. **Abrir Votação**: Criador ou curador abre a votação
   - Status muda para `Open`
   - Membros podem votar conforme visibilidade

3. **Votar**: `POST /api/v1/votings/{id}/vote`
   - Cada membro pode votar uma vez
   - Opção selecionada deve existir

4. **Fechar Votação**: `POST /api/v1/votings/{id}/close`
   - Status muda para `Closed`
   - Resultados são calculados

5. **Aplicar Resultados**: Automaticamente pelo sistema
   - Se `ThemePrioritization`: atualiza ordem de temas
   - Se `ModerationRule`: cria regra de moderação
   - Se `TerritoryCharacterization`: adiciona tags ao território
   - Se `FeatureFlag`: habilita/desabilita feature
   - Se `CommunityPolicy`: cria política

### Aplicação de Regras de Moderação

1. **Criar Regra**: Via votação ou diretamente (curador)
2. **Regra Ativa**: `IsActive = true`
3. **Aplicação Automática**:
   - Ao criar post: `PostCreationService` verifica regras
   - Ao criar item: `StoreItemService` verifica regras
   - Se violar regra: retorna erro com detalhes

### Feed Filtrado por Interesses

1. **Usuário Define Interesses**: `POST /api/v1/users/me/interests`
2. **Feed Opcionalmente Filtrado**: `GET /api/v1/feed?filterByInterests=true`
   - Se `true`: retorna apenas posts que correspondem aos interesses
   - Se `false` (padrão): retorna feed completo cronológico

---

## 🔒 Segurança e Permissões

### Permissões por Tipo de Votação

| Tipo | Criar | Votar |
|------|-------|-------|
| `ThemePrioritization` | Resident | Conforme visibilidade |
| `ModerationRule` | Curator | Conforme visibilidade |
| `TerritoryCharacterization` | Resident | Conforme visibilidade |
| `FeatureFlag` | Curator | Conforme visibilidade |
| `CommunityPolicy` | Resident | Conforme visibilidade |

### Validações de Segurança

- **Autenticação**: Todos os endpoints requerem autenticação
- **Autorização**: Verificação de membership e permissões
- **Validação de Input**: FluentValidation em todos os requests
- **Rate Limiting**: Aplicado a todos os endpoints

---

## 📊 Modelos de Dados

### UserInterest
```csharp
public sealed class UserInterest
{
    public Guid Id { get; }
    public Guid UserId { get; }
    public string InterestTag { get; } // Máx. 50 chars, lowercase
    public DateTime CreatedAtUtc { get; }
}
```

### Voting
```csharp
public sealed class Voting
{
    public Guid Id { get; }
    public Guid TerritoryId { get; }
    public Guid CreatedByUserId { get; }
    public VotingType Type { get; }
    public string Title { get; }
    public string Description { get; }
    public IReadOnlyList<string> Options { get; }
    public VotingVisibility Visibility { get; }
    public VotingStatus Status { get; }
    public DateTime? StartsAtUtc { get; }
    public DateTime? EndsAtUtc { get; }
    public DateTime CreatedAtUtc { get; }
    public DateTime UpdatedAtUtc { get; }
}
```

### Vote
```csharp
public sealed class Vote
{
    public Guid Id { get; }
    public Guid VotingId { get; }
    public Guid UserId { get; }
    public string SelectedOption { get; }
    public DateTime CreatedAtUtc { get; }
}
```

### TerritoryModerationRule
```csharp
public sealed class TerritoryModerationRule
{
    public Guid Id { get; }
    public Guid TerritoryId { get; }
    public Guid? CreatedByVotingId { get; } // Nullable
    public RuleType RuleType { get; }
    public string Rule { get; } // JSON
    public bool IsActive { get; }
    public DateTime CreatedAtUtc { get; }
    public DateTime UpdatedAtUtc { get; }
}
```

### TerritoryCharacterization
```csharp
public sealed class TerritoryCharacterization
{
    public Guid TerritoryId { get; }
    public IReadOnlyList<string> Tags { get; }
    public DateTime UpdatedAtUtc { get; }
}
```

---

## 🧪 Testes

### Testes Unitários
- `UserInterestServiceTests`: Testes do serviço de interesses
- `VotingServiceTests`: Testes do serviço de votação

### Testes de Integração
- `GovernanceIntegrationTests`: Testes end-to-end da API

### Cobertura
- Meta: >85% de cobertura
- Status: ✅ Implementado

---

## 📝 Notas de Implementação

### Decisões de Design

1. **Feed Cronológico Preservado**: O feed completo permanece como padrão. Filtros são opcionais e não alteram a ordem cronológica.

2. **Aplicação Automática de Resultados**: Resultados de votações são aplicados automaticamente quando a votação é fechada, garantindo que decisões coletivas sejam implementadas.

3. **Regras de Moderação Dinâmicas**: Regras podem ser criadas via votações ou diretamente por curadores, permitindo flexibilidade na moderação.

4. **Transparência Total**: Todas as votações e regras são públicas e auditáveis, garantindo transparência na governança.

---

## 🔗 Referências

- [FASE14.md](../backlog-api/FASE14.md): Especificação completa da fase
- [VOTING_SYSTEM.md](./VOTING_SYSTEM.md): Documentação detalhada do sistema de votação
- [COMMUNITY_MODERATION.md](./COMMUNITY_MODERATION.md): Documentação de moderação comunitária

---

**Última Atualização**: 2025-01-21
