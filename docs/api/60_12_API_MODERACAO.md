# Moderação - API Araponga

**Parte de**: [API Araponga - Lógica de Negócio e Usabilidade](./60_API_LÓGICA_NEGÓCIO_INDEX.md)  
**Versão**: 2.0  
**Data**: 2025-01-20

---

## 🛡️ Moderação

### Reportar Post (`POST /api/v1/reports/posts/{postId}`)

**Descrição**: Reporta um post por violação.

**Como usar**:
- Exige autenticação
- Path param: `postId`
- Body: `reason`, `details` (opcional)

**Regras de negócio**:
- **Permissão**: Todos usuários autenticados podem reportar
- **Deduplicação**: Múltiplos reports do mesmo usuário/post em janela de tempo são deduplicados
- **Status**: Report é criado como `OPEN`
- **Automação**: Se threshold de reports for atingido, sanção automática pode ser aplicada

### Reportar Usuário (`POST /api/v1/reports/users/{userId}`)

**Descrição**: Reporta um usuário por comportamento inadequado.

**Como usar**:
- Exige autenticação
- Path param: `userId`
- Body: `reason`, `details` (opcional)

**Regras de negócio**:
- **Permissão**: Todos usuários autenticados podem reportar
- **Deduplicação**: Múltiplos reports do mesmo usuário/alvo em janela de tempo são deduplicados
- **Status**: Report é criado como `OPEN`
- **Automação**: Threshold de reports pode gerar sanção automática

### Bloquear Usuário (`POST /api/v1/users/{userId}/block`)

**Descrição**: Bloqueia um usuário (não vê mais conteúdo dele).

**Como usar**:
- Exige autenticação
- Path param: `userId`

**Regras de negócio**:
- **Permissão**: Todos usuários autenticados podem bloquear
- **Idempotente**: Bloqueios múltiplos são deduplicados
- **Efeito**: Posts, entidades e conteúdo do usuário bloqueado não aparecem mais
- **Reversível**: Pode desbloquear via `DELETE /api/v1/users/{userId}/block`

### Listar Reports (`GET /api/v1/reports`)

**Descrição**: Lista reports do território (curadoria).

**Como usar**:
- Exige autenticação
- Query params: `territoryId` (opcional), `targetType` (POST, USER), `status` (OPEN, RESOLVED, etc.), `skip`, `take` (paginação)
- Header `X-Session-Id` para identificar território ativo

**Regras de negócio**:
- **Permissão**: Apenas curadores (CURATOR) podem listar reports
- **Filtros**: `targetType` e `status` são opcionais
- **Paginação**: Padrão 20 itens

---

### Moderação dinâmica comunitária (Fase 14)

**Descrição**: Regras de moderação definidas pela comunidade por território, aplicadas automaticamente na criação de posts e itens do marketplace.

**Como funciona**:
- **Regras** (`TerritoryModerationRule`): Podem ser criadas por curadores ou via votações ([Governança](./60_19_API_GOVERNANCA.md)). Tipos: ContentType, ProhibitedWords, Behavior, MarketplacePolicy, EventPolicy.
- **Aplicação**: `PostCreationService` e `StoreItemService` verificam regras ativas antes de criar conteúdo; retornam erro se houver violação.
- **Gestão**: Curadores criam/ativam regras; a comunidade pode propor e votar regras (votação tipo `ModerationRule`).

**Documentação detalhada**: `docs/COMMUNITY_MODERATION.md`, `docs/GOVERNANCE_SYSTEM.md`.

---

## 📚 Documentação Relacionada

- **[Governança](./60_19_API_GOVERNANCA.md)** - Votações e moderação dinâmica comunitária
- **[Regras de Visibilidade](./60_17_API_VISIBILIDADE.md)** - Sanções e restrições
- **[Admin: System Config](./60_14_API_ADMIN.md)** - Configurações de moderação
- **[Paginação](./60_00_API_PAGINACAO.md)** - Versão paginada: `GET /api/v1/reports/paged`

---

**Voltar para**: [Índice da Documentação da API](./60_API_LÓGICA_NEGÓCIO_INDEX.md)
