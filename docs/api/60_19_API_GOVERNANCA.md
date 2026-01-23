# Governança Comunitária - API Araponga

**Parte de**: [API Araponga - Lógica de Negócio e Usabilidade](./60_API_LÓGICA_NEGÓCIO_INDEX.md)  
**Versão**: 1.0  
**Data**: 2025-01-23

---

## 🏛️ Governança Comunitária

A API de governança permite interesses do usuário, votações comunitárias, caracterização do território e histórico de participação. Ver também [Moderação](./60_12_API_MODERACAO.md) (moderação dinâmica comunitária).

---

### Interesses do Usuário

#### Listar Interesses (`GET /api/v1/users/me/interests`)

**Descrição**: Lista os interesses (tags) do usuário autenticado.

**Como usar**:
- Exige autenticação
- Retorna `IReadOnlyList<string>` (tags em lowercase)

**Regras de negócio**:
- Máximo 10 interesses por usuário
- Tags normalizadas (trim, lowercase)
- Interesses aparecem no perfil (`UserProfileResponse.Interests`)

---

#### Adicionar Interesse (`POST /api/v1/users/me/interests`)

**Descrição**: Adiciona um interesse ao usuário.

**Como usar**:
- Exige autenticação
- Body: `AddInterestRequest` com `InterestTag` (string, máx. 50 caracteres)

**Regras de negócio**:
- Tag não vazia, máx. 50 caracteres
- Máx. 10 interesses; retorna erro se exceder
- Validação: letras, números, espaços e hífens

**Validação**:
- FluentValidation (`AddInterestRequestValidator`)

---

#### Remover Interesse (`DELETE /api/v1/users/me/interests/{tag}`)

**Descrição**: Remove um interesse do usuário.

**Como usar**:
- Exige autenticação
- Path param: `tag` (ex.: `meio-ambiente`)

**Regras de negócio**:
- Idempotente: 204 mesmo se tag não existir
- Tag na URL deve corresponder ao valor armazenado (lowercase)

---

### Votações Comunitárias

Todas as votações são **por território**. Base path: `GET/POST /api/v1/territories/{territoryId}/votings`.

#### Criar Votação (`POST /api/v1/territories/{territoryId}/votings`)

**Descrição**: Cria uma votação no território.

**Como usar**:
- Exige autenticação
- Path: `territoryId`
- Body: `CreateVotingRequest`:
  - `Type`: `ThemePrioritization` | `ModerationRule` | `TerritoryCharacterization` | `FeatureFlag` | `CommunityPolicy`
  - `Title`, `Description`
  - `Options`: array de strings (opções de voto)
  - `Visibility`: `AllMembers` | `ResidentsOnly` | `CuratorsOnly`
  - `StartsAtUtc`, `EndsAtUtc` (opcional)

**Regras de negócio**:
- **Quem pode criar**: Residents (ThemePrioritization, TerritoryCharacterization, CommunityPolicy) ou Curadores (ModerationRule, FeatureFlag)
- Votação criada como `Draft`; deve ser aberta para votação
- Opções obrigatórias, pelo menos 2

**Validação**: `CreateVotingRequestValidator`

---

#### Listar Votações (`GET /api/v1/territories/{territoryId}/votings`)

**Descrição**: Lista votações do território.

**Como usar**:
- Exige autenticação
- Query params: `status` (opcional, ex.: Open, Closed), `skip`, `take` (paginação)

**Regras de negócio**:
- Respeita visibilidade e permissões do usuário
- Retorna lista paginada de `VotingResponse`

---

#### Obter Votação (`GET /api/v1/territories/{territoryId}/votings/{votingId}`)

**Descrição**: Obtém detalhes de uma votação.

**Como usar**:
- Exige autenticação
- Path: `territoryId`, `votingId`

**Regras de negócio**:
- 404 se votação ou território não existir
- Respeita visibilidade

---

#### Votar (`POST /api/v1/territories/{territoryId}/votings/{votingId}/vote`)

**Descrição**: Registra voto em uma votação aberta.

**Como usar**:
- Exige autenticação
- Body: `VoteRequest` com `SelectedOption` (string)

**Regras de negócio**:
- Votação deve estar `Open`
- Usuário só pode votar uma vez
- `SelectedOption` deve existir em `Options`
- Elegibilidade conforme `Visibility` (AllMembers, ResidentsOnly, CuratorsOnly)
- Retorna 204 No Content em sucesso

---

#### Fechar Votação (`POST /api/v1/territories/{territoryId}/votings/{votingId}/close`)

**Descrição**: Fecha uma votação (apenas criador ou curador).

**Como usar**:
- Exige autenticação
- Apenas criador da votação ou curador do território

**Regras de negócio**:
- Votação passa a `Closed`
- Resultados podem ser aplicados (ModerationRule, TerritoryCharacterization, etc.)

---

#### Resultados (`GET /api/v1/territories/{territoryId}/votings/{votingId}/results`)

**Descrição**: Obtém contagem de votos por opção.

**Como usar**:
- Exige autenticação
- Retorna `VotingResultsResponse` (opção → contagem)

**Regras de negócio**:
- Disponível quando votação está `Closed` ou `Approved`/`Rejected`

---

### Histórico de Governança no Perfil

#### Histórico de Participação (`GET /api/v1/users/me/profile/governance`)

**Descrição**: Retorna histórico de votações participadas e contribuições para moderação.

**Como usar**:
- Exige autenticação
- Retorna `UserProfileGovernanceResponse`:
  - `VotingHistory`: lista de `VotingHistoryItem` (votação, opção escolhida, etc.)
  - `ModerationContributions`: contagem de contribuições (propostas, reports, etc.)

**Regras de negócio**:
- Apenas o próprio usuário (me)
- Dados agregados para exibição no perfil

---

## 📚 Documentação Relacionada

- **[Moderação](./60_12_API_MODERACAO.md)** - Reports, bloqueios e **moderação dinâmica comunitária** (regras por território)
- **[Feed](./60_04_API_FEED.md)** - Filtro opcional por interesses (`filterByInterests`)
- **[Territórios](./60_02_API_TERRITORIOS.md)** - Caracterização (tags) em `TerritoryResponse`
- **Docs de domínio**: `docs/GOVERNANCE_SYSTEM.md`, `docs/VOTING_SYSTEM.md`, `docs/COMMUNITY_MODERATION.md`

---

**Voltar para**: [Índice da Documentação da API](./60_API_LÓGICA_NEGÓCIO_INDEX.md)
