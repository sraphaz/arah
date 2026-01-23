# Sistema de Votação Comunitária

**Versão**: 1.0  
**Data**: 2025-01-21  
**Status**: ✅ Implementado

---

## 📋 Visão Geral

O Sistema de Votação permite que comunidades territoriais tomem decisões coletivas através de votações estruturadas. As votações podem ser de diferentes tipos, com diferentes níveis de visibilidade e permissões.

---

## 🎯 Tipos de Votação

### 1. ThemePrioritization (Priorização de Temas)
**Objetivo**: Definir quais temas devem ter prioridade no feed do território.

**Quem pode criar**: Residents  
**Quem pode votar**: Conforme visibilidade (AllMembers, ResidentsOnly, CuratorsOnly)

**Exemplo**:
- Título: "Priorizar temas do território"
- Opções: ["Meio Ambiente", "Eventos", "Marketplace", "Saúde"]
- Resultado: Ordem de prioridade (não altera feed cronológico, apenas destaca)

**Aplicação de Resultado**: Atualiza ordem de temas no feed (opcional, não altera cronologia).

---

### 2. ModerationRule (Regra de Moderação)
**Objetivo**: Criar ou modificar regras de moderação do território.

**Quem pode criar**: Curadores  
**Quem pode votar**: Conforme visibilidade

**Exemplo**:
- Título: "Proibir posts sobre política partidária"
- Opções: ["Aprovar", "Rejeitar"]
- Resultado: Se aprovado, cria regra de moderação

**Aplicação de Resultado**: Cria `TerritoryModerationRule` se aprovado.

---

### 3. TerritoryCharacterization (Caracterização do Território)
**Objetivo**: Adicionar tags que descrevem o território.

**Quem pode criar**: Residents  
**Quem pode votar**: Conforme visibilidade

**Exemplo**:
- Título: "Caracterizar nosso território"
- Opções: ["Rural", "Urbano", "Praia", "Montanha", "Floresta"]
- Resultado: Tags adicionadas ao território

**Aplicação de Resultado**: Adiciona tags vencedoras a `TerritoryCharacterization`.

---

### 4. FeatureFlag (Feature Flag Territorial)
**Objetivo**: Habilitar ou desabilitar funcionalidades do território.

**Quem pode criar**: Curadores  
**Quem pode votar**: Conforme visibilidade

**Exemplo**:
- Título: "Habilitar marketplace no território"
- Opções: ["Habilitar", "Desabilitar"]
- Resultado: Feature flag atualizado

**Aplicação de Resultado**: Atualiza `FeatureFlag` do território.

---

### 5. CommunityPolicy (Política Comunitária)
**Objetivo**: Criar políticas e regras de convivência.

**Quem pode criar**: Residents  
**Quem pode votar**: Conforme visibilidade

**Exemplo**:
- Título: "Política de eventos comunitários"
- Opções: ["Aprovar", "Rejeitar"]
- Resultado: Política criada se aprovada

**Aplicação de Resultado**: Cria política comunitária.

---

## 🔄 Ciclo de Vida de uma Votação

### 1. Draft (Rascunho)
- Votação criada mas não visível
- Apenas criador pode ver
- Pode ser editada

### 2. Open (Aberta)
- Votação visível para membros conforme visibilidade
- Membros podem votar
- Resultados não são visíveis ainda

### 3. Closed (Fechada)
- Votação fechada para novos votos
- Resultados calculados e visíveis
- Resultados podem ser aplicados

### 4. Approved (Aprovada)
- Resultados aplicados com sucesso
- Status final (não pode ser alterado)

### 5. Rejected (Rejeitada)
- Votação rejeitada (opcional)
- Status final (não pode ser alterado)

---

## 🔐 Permissões e Visibilidade

### Visibilidade

#### AllMembers
- Todos os membros do território podem votar
- Inclui visitors e residents

#### ResidentsOnly
- Apenas residents podem votar
- Visitors não podem votar

#### CuratorsOnly
- Apenas curadores podem votar
- Acesso privilegiado

### Permissões para Criar

| Tipo | Permissão |
|------|-----------|
| `ThemePrioritization` | Resident |
| `ModerationRule` | Curator |
| `TerritoryCharacterization` | Resident |
| `FeatureFlag` | Curator |
| `CommunityPolicy` | Resident |

---

## 📊 Resultados e Aplicação

### Cálculo de Resultados

1. **Contagem de Votos**: Conta votos por opção
2. **Opção Vencedora**: Opção com mais votos
3. **Empate**: Primeira opção em caso de empate (pode ser melhorado)

### Aplicação Automática

Quando uma votação é fechada, o sistema aplica automaticamente os resultados:

#### ThemePrioritization
```csharp
// Atualiza ordem de temas (opcional, não altera cronologia)
// Implementação futura: sistema de destaque de temas
```

#### ModerationRule
```csharp
// Cria TerritoryModerationRule
var rule = new TerritoryModerationRule(
    territoryId: voting.TerritoryId,
    createdByVotingId: voting.Id,
    ruleType: RuleType.ProhibitedWords,
    rule: winningOption, // JSON com configuração
    isActive: true);
```

#### TerritoryCharacterization
```csharp
// Adiciona tag vencedora
var characterization = await _characterizationService.GetCharacterizationAsync(
    voting.TerritoryId);
var tags = characterization?.Tags.ToList() ?? new List<string>();
if (!tags.Contains(winningOption))
{
    tags.Add(winningOption);
    await _characterizationService.UpdateCharacterizationAsync(
        voting.TerritoryId, tags);
}
```

#### FeatureFlag
```csharp
// Atualiza feature flag
// Implementação futura: integração com FeatureFlagService
```

#### CommunityPolicy
```csharp
// Cria política comunitária
// Implementação futura: sistema de políticas
```

---

## 🔌 API Endpoints

### Criar Votação
```http
POST /api/v1/territories/{territoryId}/votings
Content-Type: application/json

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

### Listar Votações
```http
GET /api/v1/territories/{territoryId}/votings?status=Open&userId={userId}
```

### Obter Votação
```http
GET /api/v1/votings/{id}
```

### Votar
```http
POST /api/v1/votings/{id}/vote
Content-Type: application/json

{
  "selectedOption": "Meio Ambiente"
}
```

### Fechar Votação
```http
POST /api/v1/votings/{id}/close
```

### Obter Resultados
```http
GET /api/v1/votings/{id}/results
```

**Resposta**:
```json
{
  "results": {
    "Meio Ambiente": 15,
    "Eventos": 8,
    "Marketplace": 3
  }
}
```

---

## 🧪 Exemplos de Uso

### Exemplo 1: Priorização de Temas

```csharp
// 1. Criar votação
var voting = await votingService.CreateVotingAsync(
    territoryId: territoryId,
    userId: userId,
    type: VotingType.ThemePrioritization,
    title: "Priorizar temas",
    description: "Qual tema deve ter prioridade?",
    options: new[] { "Meio Ambiente", "Eventos", "Marketplace" },
    visibility: VotingVisibility.AllMembers,
    startsAtUtc: null,
    endsAtUtc: null);

// 2. Abrir votação
voting.Open();
await votingRepository.UpdateAsync(voting);

// 3. Membros votam
await votingService.VoteAsync(voting.Id, userId1, "Meio Ambiente");
await votingService.VoteAsync(voting.Id, userId2, "Eventos");
// ...

// 4. Fechar votação
await votingService.CloseVotingAsync(voting.Id, creatorId);

// 5. Resultados aplicados automaticamente
var results = await votingService.GetResultsAsync(voting.Id);
// results: { "Meio Ambiente": 15, "Eventos": 8, "Marketplace": 3 }
```

### Exemplo 2: Regra de Moderação

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

// 3. Fechar e aplicar
await votingService.CloseVotingAsync(voting.Id, curatorId);
// Se aprovado, regra de moderação é criada automaticamente
```

---

## 🔒 Validações e Regras

### Validações ao Criar Votação
- ✅ Título não vazio, máximo 200 caracteres
- ✅ Descrição não vazia, máximo 2000 caracteres
- ✅ Mínimo 2 opções, máximo 10 opções
- ✅ Cada opção não vazia, máximo 100 caracteres
- ✅ Tipo de votação válido
- ✅ Visibilidade válida
- ✅ Criador tem permissão (resident/curador conforme tipo)
- ✅ Criador é membro do território

### Validações ao Votar
- ✅ Votação existe
- ✅ Votação está aberta (`Status == Open`)
- ✅ Usuário tem permissão (conforme visibilidade)
- ✅ Opção selecionada existe
- ✅ Usuário ainda não votou
- ✅ Usuário é membro do território

### Validações ao Fechar Votação
- ✅ Votação existe
- ✅ Votação está aberta
- ✅ Usuário é criador ou curador

---

## 📈 Métricas e Analytics

### Métricas Disponíveis
- Total de votações por território
- Taxa de participação (votos / membros elegíveis)
- Resultados por tipo de votação
- Histórico de votações

### Histórico de Participação
- Usuário pode ver histórico de votações participadas
- Endpoint: `GET /api/v1/users/me/profile/governance`

---

## 🔗 Referências

- [GOVERNANCE_SYSTEM.md](./GOVERNANCE_SYSTEM.md): Visão geral do sistema de governança
- [COMMUNITY_MODERATION.md](./COMMUNITY_MODERATION.md): Moderação comunitária
- [FASE14.md](../backlog-api/FASE14.md): Especificação completa

---

**Última Atualização**: 2025-01-21
