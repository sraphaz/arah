# Feed Comunitário - API Araponga

**Parte de**: [API Araponga - Lógica de Negócio e Usabilidade](./60_API_LÓGICA_NEGÓCIO_INDEX.md)  
**Versão**: 2.0  
**Data**: 2025-01-20

---

## 📝 Feed Comunitário

### Criar Post (`POST /api/v1/feed`)

**Descrição**: Publica um post no feed do território.

**Como usar**:
- Exige autenticação
- Body: título, conteúdo, tipo (GENERAL, ALERT), visibilidade (PUBLIC, RESIDENTS_ONLY)
- Opcional: `mapEntityId`, `assetIds`, `geoAnchors` (derivados de mídias, não enviados manualmente)

**Regras de negócio**:
- **Autenticação**: Obrigatória
- **Território**: Usa território ativo da sessão ou `territoryId` no body
- **Visibilidade**:
  - `PUBLIC`: Visível para todos (visitantes e moradores)
  - `RESIDENTS_ONLY`: Visível apenas para moradores validados
- **Sanções**: Usuários com sanção de posting não podem criar posts
- **Feature Flags**: Posts do tipo `ALERT` só são permitidos se a flag `ALERTPOSTS` estiver habilitada no território.
  - Observação: quando um alerta de saúde é **validado**, a publicação do post `ALERT` no feed também depende dessa mesma flag (para evitar bypass).
- **GeoAnchors**: Deriva automaticamente de mídias (não são enviados manualmente)
- **Limites**: Título máximo 200 caracteres, conteúdo máximo 4000 caracteres
- **Status**: Posts são criados como `PUBLISHED` por padrão

**Validação**:
- Título e conteúdo são obrigatórios
- Título máximo 200 caracteres
- Conteúdo máximo 4000 caracteres
- Tipo e visibilidade devem ser valores válidos
- GeoAnchors máximo 50 (se fornecidos)

**Rate Limiting**:
- **Limite**: 30 requisições por minuto por usuário autenticado
- **Resposta quando excedido**: `429 Too Many Requests`

### Listar Feed (`GET /api/v1/feed`)

**Descrição**: Obtém posts do feed do território ativo.

**Como usar**:
- Exige autenticação
- Query params: `skip`, `take` (paginação), `mapEntityId`, `assetId` (filtros), **`filterByInterests`** (bool, opcional, default: false)
- Header `X-Session-Id` para identificar território ativo

**Regras de negócio**:
- **Filtragem por visibilidade**:
  - Visitantes (VISITOR): Veem apenas posts `PUBLIC`
  - Moradores verificados (RESIDENT + `ResidencyVerification != NONE`): Veem posts `PUBLIC` e `RESIDENTS_ONLY`
  - Moradores não verificados (RESIDENT + `ResidencyVerification = NONE`): Veem apenas posts `PUBLIC`
- **Filtro por interesses** (`filterByInterests=true`): Mantém apenas posts cujo título ou conteúdo contenha palavras que coincidem com os interesses do usuário ([Governança](./60_19_API_GOVERNANCA.md)). Feed cronológico completo permanece como padrão; o filtro é opcional.
- **Bloqueios**: Posts de usuários bloqueados não aparecem
- **Paginação**: Padrão 20 itens por página
- **Ordenação**: Mais recentes primeiro

**Rate Limiting**:
- **Limite**: 100 requisições por minuto por usuário autenticado
- **Resposta quando excedido**: `429 Too Many Requests`

### Curtir Post (`POST /api/v1/feed/{postId}/likes`)

**Descrição**: Adiciona ou remove like em um post.

**Como usar**:
- Exige autenticação
- Path param: `postId`

**Regras de negócio**:
- **Idempotente**: Múltiplas chamadas alternam entre like/deslike
- **Permissão**: Todos usuários autenticados podem curtir
- Não pode curtir posts bloqueados ou não visíveis

### Comentar Post (`POST /api/v1/feed/{postId}/comments`)

**Descrição**: Adiciona comentário em um post.

**Como usar**:
- Exige autenticação
- Path param: `postId`
- Body: `content` (texto do comentário)

**Regras de negócio**:
- **Permissão**: Apenas moradores verificados (geo/doc) podem comentar
- **Visitantes**: Não podem comentar
- **Limites**: Conteúdo máximo 2000 caracteres
- **Bloqueios**: Não pode comentar em posts de usuários bloqueados

### Compartilhar Post (`POST /api/v1/feed/{postId}/shares`)

**Descrição**: Compartilha um post no feed do território.

**Como usar**:
- Exige autenticação
- Path param: `postId`

**Regras de negócio**:
- **Permissão**: Apenas moradores verificados (geo/doc) podem compartilhar
- **Visitantes**: Não podem compartilhar
- **Compartilhamento**: Cria novo post referenciando o original
- **Visibilidade**: Post compartilhado herda visibilidade do original

### Listar Feed Pessoal (`GET /api/v1/feed/me`)

**Descrição**: Obtém posts do próprio usuário.

**Como usar**:
- Exige autenticação
- Query params: `skip`, `take` (paginação)

**Regras de negócio**:
- Retorna apenas posts do usuário autenticado
- Inclui todos os status (PUBLISHED, ARCHIVED, etc.)
- Paginação padrão: 20 itens

---

## 📚 Documentação Relacionada

- **[Governança](./60_19_API_GOVERNANCA.md)** - Interesses do usuário e filtro opcional do feed (`filterByInterests`)
- **[Mídias em Conteúdo](./60_15_API_MIDIAS.md)** - Adicionar imagens, vídeos e áudios aos posts
- **[Regras de Visibilidade](./60_17_API_VISIBILIDADE.md)** - Entender visibilidade PUBLIC vs RESIDENTS_ONLY
- **[Moderação](./60_12_API_MODERACAO.md)** - Reportar posts inadequados
- **[Paginação](./60_00_API_PAGINACAO.md)** - Versão paginada: `GET /api/v1/feed/paged`

---

**Voltar para**: [Índice da Documentação da API](./60_API_LÓGICA_NEGÓCIO_INDEX.md)
