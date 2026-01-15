# API Araponga - Lógica de Negócio e Usabilidade

**Documento de Negócio Completo**  
**Versão**: 1.0  
**Data**: 2025-01-13

---

## 📋 Índice

1. [Visão Geral](#visão-geral)
2. [Autenticação e Cadastro](#autenticação-e-cadastro)
3. [Territórios](#territórios)
4. [Vínculos e Membros (Memberships)](#vínculos-e-membros-memberships)
5. [Feed Comunitário](#feed-comunitário)
6. [Eventos](#eventos)
7. [Mapa Territorial](#mapa-territorial)
8. [Alertas de Saúde](#alertas-de-saúde)
9. [Assets (Recursos Territoriais)](#assets-recursos-territoriais)
10. [Marketplace](#marketplace)
11. [Chat (Canais, Grupos e DM)](#chat-canais-grupos-e-dm)
12. [Notificações](#notificações)
13. [Moderação](#moderação)
14. [Solicitações de Entrada (Join Requests)](#solicitações-de-entrada-join-requests)
15. [Feature Flags](#feature-flags)
16. [Regras de Visibilidade e Permissões](#regras-de-visibilidade-e-permissões)
17. [Admin: System Config e Work Queue](#admin-system-config-e-work-queue)
18. [Verificações e Evidências (upload/download)](#verificações-e-evidências-uploaddownload)

---

## 🌐 Visão Geral

O Araponga é uma plataforma **território-first** e **comunidade-first** para organização comunitária local. Todas as operações são orientadas ao território, com diferenciação clara entre **visitantes (VISITOR)** e **moradores (RESIDENT)**.

### Princípios Fundamentais

- **Território é geográfico e neutro**: Representa apenas um lugar físico real
- **Consulta exige cadastro**: Feed, mapa e operações sociais exigem usuário autenticado
- **Presença física é critério de vínculo**: No MVP, não é possível associar território remotamente
- **Visibilidade diferenciada**: Conteúdo pode ser Público (todos) ou Apenas Moradores (RESIDENTS_ONLY)

### 🔒 Segurança e Rate Limiting

A API implementa várias camadas de segurança:

- **Rate Limiting**: Proteção contra abuso e DDoS
  - Auth endpoints: 5 req/min
  - Feed endpoints: 100 req/min
  - Write endpoints: 30 req/min
  - Global: 60 req/min (configurável)
- **HTTPS Obrigatório**: Em produção, todas as conexões são criptografadas
- **Security Headers**: Headers de segurança em todas as respostas
- **Validação de Input**: Validação automática de todos os requests
- **CORS Configurado**: Políticas de CORS por ambiente

Quando o rate limit é excedido, a API retorna:
- **Status Code**: `429 Too Many Requests`
- **Header**: `Retry-After` com tempo em segundos
- **Body**: ProblemDetails com detalhes do erro

---

## 🧰 Admin: System Config e Work Queue

> Referência detalhada: **[33_ADMIN_SYSTEM_CONFIG_WORKQUEUE.md](./33_ADMIN_SYSTEM_CONFIG_WORKQUEUE.md)**.

### System Config (SystemAdmin)
**Objetivo**: centralizar configurações calibráveis (providers, segurança, moderação, validação).

- `GET /api/v1/admin/system-config`
- `GET /api/v1/admin/system-config/{key}`
- `PUT /api/v1/admin/system-config`

### Work Items (filas)
**Objetivo**: padronizar revisões humanas (verificação, curadoria, moderação).

**Globais (SystemAdmin)**:
- `GET /api/v1/admin/work-items`
- `POST /api/v1/admin/work-items/{workItemId}/complete`

**Territoriais (Curator/Moderator)**:
- `GET /api/v1/territories/{territoryId}/work-items`
- `POST /api/v1/territories/{territoryId}/work-items/{workItemId}/complete`

---

## 📎 Verificações e Evidências (upload/download)

### Upload (multipart/form-data)
- **Identidade (global)**:
  - `POST /api/v1/verification/identity/document/upload`
- **Residência (territorial)**:
  - `POST /api/v1/memberships/{territoryId}/verify-residency/document/upload`

### Decisão de verificação (fila humana)
- **Identidade (SystemAdmin)**:
  - `POST /api/v1/admin/verifications/identity/{workItemId}/decide`
- **Residência (Curator)**:
  - `POST /api/v1/territories/{territoryId}/verification/residency/{workItemId}/decide`

### Download por proxy (stream via API)
- **Admin (SystemAdmin)**:
  - `GET /api/v1/admin/evidences/{evidenceId}/download`
- **Território (Curator/Moderator)**:
  - `GET /api/v1/territories/{territoryId}/evidences/{evidenceId}/download`


## 🔐 Autenticação e Cadastro

### Login Social (`POST /api/v1/auth/social`)

**Descrição**: Autentica ou cadastra um usuário via login social.

**Como usar**:
- Envie Provider (ex: "google", "facebook"), ExternalId, DisplayName
- Forneça CPF (formato: "123.456.789-00" ou "12345678900") OU ForeignDocument
- Campos opcionais: Email, PhoneNumber, Address

**Regras de negócio**:
- Se o usuário já existir (mesmo Provider + ExternalId), retorna token existente
- Se não existir, cria novo usuário e retorna token
- CPF e ForeignDocument são mutuamente exclusivos (não pode enviar ambos)
- CPF aceita formatação (pontos e hífen) ou apenas dígitos
- O token JWT retornado deve ser incluído em todas as requisições subsequentes no header `Authorization: Bearer {token}`

**Rate Limiting**:
- **Limite**: 5 requisições por minuto por IP/usuário
- **Resposta quando excedido**: `429 Too Many Requests` com header `Retry-After`

**Resposta**:
- **200 OK**: Token JWT e dados do usuário
- **400 Bad Request**: Validação falhou (campos obrigatórios ausentes, CPF inválido, etc.)
- **429 Too Many Requests**: Rate limit excedido

---

## 🗺️ Territórios

### Listar Territórios (`GET /api/v1/territories`)

**Descrição**: Lista todos os territórios disponíveis para descoberta.

**Como usar**:
- Requisição pública (não exige autenticação)
- Retorna lista paginada de territórios com dados geográficos

**Regras de negócio**:
- Retorna apenas dados geográficos (nome, cidade, estado, coordenadas)
- Não inclui informações sociais (membership, roles, etc.)

### Buscar Territórios Próximos (`GET /api/v1/territories/nearby`)

**Descrição**: Encontra territórios próximos a uma localização.

**Como usar**:
- Query params: `lat`, `lng`, `radiusKm` (opcional, padrão 25km), `limit` (opcional)
- Retorna territórios ordenados por proximidade

**Regras de negócio**:
- Requisição pública (não exige autenticação)
- Ordenação: mais próximo primeiro
- Limite padrão se não especificado

### Buscar Territórios por Texto (`GET /api/v1/territories/search`)

**Descrição**: Busca territórios por nome, cidade ou estado.

**Como usar**:
- Query params: `q` (nome), `city`, `state`
- Parâmetros são opcionais e combinados com AND

**Regras de negócio**:
- Requisição pública
- Busca case-insensitive
- Retorna correspondências parciais

### Consultar Território por ID (`GET /api/v1/territories/{id}`)

**Descrição**: Obtém detalhes de um território específico.

**Como usar**:
- Exige autenticação
- Path param: ID do território

**Regras de negócio**:
- Retorna apenas dados geográficos
- Retorna 404 se território não existir

### Sugerir Território (`POST /api/v1/territories/suggestions`)

**Descrição**: Sugere um novo território para inclusão no sistema.

**Como usar**:
- Exige autenticação
- Body: nome, descrição, cidade, estado, latitude, longitude

**Regras de negócio**:
- Território é criado com status `PENDING` (aguardando curadoria)
- Exige coordenadas válidas (-90 a 90 lat, -180 a 180 lng)
- Não pode sugerir território duplicado (validação por nome/cidade/estado)

### Selecionar Território Ativo (`POST /api/v1/territories/selection`)

**Descrição**: Define o território ativo para a sessão do usuário.

**Como usar**:
- Exige autenticação
- Header `X-Session-Id` obrigatório
- Body: `territoryId`

**Regras de negócio**:
- Define o território contexto para operações subsequentes
- Session ID identifica a sessão do usuário (pode ser qualquer string única)
- Um usuário pode ter múltiplas sessões com territórios diferentes
- O território selecionado é usado por padrão em operações que requerem território

### Consultar Território Ativo (`GET /api/v1/territories/selection`)

**Descrição**: Obtém o território ativo da sessão atual.

**Como usar**:
- Exige autenticação
- Header `X-Session-Id` obrigatório

**Regras de negócio**:
- Retorna 404 se nenhum território estiver selecionado para a sessão
- Retorna dados do território selecionado

---

## 👥 Vínculos e Membros (Memberships)

### Entrar no território como VISITOR (`POST /api/v1/territories/{territoryId}/enter`)

**Descrição**: Cria (ou retorna) o vínculo do usuário no território como **VISITOR**.

**Como usar**:
- Exige autenticação
- Path param: `territoryId`

**Regras de negócio**:
- Cria `TerritoryMembership` com `Role=VISITOR` e `ResidencyVerification=NONE`
- Não existe "validação" para VISITOR; é um vínculo leve para acesso ao conteúdo público

### Solicitar residência (cria JoinRequest) (`POST /api/v1/memberships/{territoryId}/become-resident`)

**Descrição**: Cria uma solicitação (JoinRequest) para virar **RESIDENT**. O usuário permanece VISITOR até aprovação.

**Como usar**:
- Exige autenticação
- Path param: `territoryId`
 - Body opcional:
   - `recipientUserIds` (array) para convite direcionado (quando conhece alguém)
   - `message` (string) opcional

**Regras de negócio**:
- Se `recipientUserIds` for informado, o pedido é direcionado para esses destinatários (desde que elegíveis).
- Se não informar destinatários, o pedido vai para **Curator** do território.
- Se não houver Curator, faz fallback para **SystemAdmin**.
- Idempotente: se já houver JoinRequest pendente, retorna a mesma solicitação
- Regra: 1 Resident por User (se já for Resident em outro território, deve transferir)
- Anti-abuso:
  - `recipientUserIds` tem limite de **3** destinatários
  - Rate limit: no máximo **3** criações (create+cancel+recreate) por usuário/território em janela de **24h**
  - Quando estourar o rate limit, a API retorna **429 Too Many Requests**

### Consultar meu vínculo no território (`GET /api/v1/memberships/{territoryId}/me`)

**Descrição**: Consulta o vínculo do usuário autenticado com um território.

**Como usar**:
- Exige autenticação
- Path param: `territoryId`

**Regras de negócio**:
- Retorna `role` e `residencyVerification` (`NONE`, `GEOVERIFIED`, `DOCUMENTVERIFIED`)
- Se não houver vínculo, retorna `404`

### Verificar residência por geolocalização (`POST /api/v1/memberships/{territoryId}/verify-residency/geo`)

**Descrição**: Marca `ResidencyVerification=GEOVERIFIED` quando as coordenadas estão dentro do raio permitido do território.

**Regras de negócio**:
- Requer `Role=RESIDENT` no território
- Não substitui aprovação do JoinRequest: é um passo de verificação pós-aprovação

### Verificar residência por documento (`POST /api/v1/memberships/{territoryId}/verify-residency/document`)

**Descrição**: Marca `ResidencyVerification=DOCUMENTVERIFIED`.

**Regras de negócio**:
- Requer `Role=RESIDENT` no território
- Fluxo completo com upload/evidências e revisão humana está detalhado em `33_ADMIN_SYSTEM_CONFIG_WORKQUEUE.md`

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
- Query params: `skip`, `take` (paginação), `mapEntityId`, `assetId` (filtros)
- Header `X-Session-Id` para identificar território ativo

**Regras de negócio**:
- **Filtragem por visibilidade**:
  - Visitantes (VISITOR): Veem apenas posts `PUBLIC`
  - Moradores verificados (RESIDENT + `ResidencyVerification != NONE`): Veem posts `PUBLIC` e `RESIDENTS_ONLY`
  - Moradores não verificados (RESIDENT + `ResidencyVerification = NONE`): Veem apenas posts `PUBLIC`
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

## 📅 Eventos

### Criar Evento (`POST /api/v1/events`)

**Descrição**: Cria um evento comunitário no território.

**Como usar**:
- Exige autenticação
- Body: `territoryId`, título, descrição (opcional), `startsAtUtc`, `endsAtUtc` (opcional), `latitude`, `longitude`, `locationLabel` (opcional)

**Regras de negócio**:
- **Permissão**: Visitantes e moradores podem criar eventos
- **Geolocalização**: Obrigatória (latitude e longitude)
- **Data**: `startsAtUtc` deve ser no futuro (ou até 1 ano no passado para ajustes)
- **Data fim**: Se informada, deve ser após data início
- **Limites**: Título máximo 200 caracteres, descrição máxima 2000 caracteres
- **Criação automática**: Cria automaticamente um post no feed referenciando o evento
- **Registro**: Registra se evento foi criado por VISITOR ou RESIDENT (baseado no membership atual)
- **Status**: Eventos são criados como `SCHEDULED`

### Listar Eventos (`GET /api/v1/events`)

**Descrição**: Lista eventos do território.

**Como usar**:
- Exige autenticação
- Query params: `territoryId`, `skip`, `take` (paginação), `startDate`, `endDate` (filtros opcionais)
- Header `X-Session-Id` para usar território ativo

**Regras de negócio**:
- **Visibilidade**: Todos os eventos são públicos (não há RESIDENTS_ONLY para eventos)
- **Paginação**: Padrão 20 itens
- **Filtros**: `startDate` e `endDate` filtram eventos por período

### Buscar Eventos Próximos (`GET /api/v1/events/nearby`)

**Descrição**: Busca eventos próximos a uma localização.

**Como usar**:
- Exige autenticação
- Query params: `lat`, `lng`, `radiusKm` (opcional, padrão 5km), `limit` (opcional)

**Regras de negócio**:
- Ordenação: mais próximo primeiro
- Raio padrão: 5km
- Limite padrão: 20 eventos

### Participar de Evento (`POST /api/v1/events/{eventId}/interest` ou `/confirm`)

**Descrição**: Marca interesse ou confirmação em um evento.

**Como usar**:
- Exige autenticação
- Path param: `eventId`
- Endpoints: `/interest` (interessado) ou `/confirm` (confirmado)

**Regras de negócio**:
- **Idempotente**: Múltiplas chamadas atualizam a participação (upsert)
- **Permissão**: Todos usuários autenticados podem participar
- **Status**: INTEREST (interessado) ou CONFIRMED (confirmado)
- **Contagem**: Eventos retornam contagem de interessados e confirmados

### Cancelar Evento (`POST /api/v1/events/{eventId}/cancel`)

**Descrição**: Cancela um evento.

**Como usar**:
- Exige autenticação
- Path param: `eventId`

**Regras de negócio**:
- **Permissão**: Apenas o criador do evento pode cancelar
- **Status**: Evento é marcado como `CANCELLED`
- **Notificações**: Não gera notificações automáticas

---

## 🗺️ Mapa Territorial

### Listar Entidades do Mapa (`GET /api/v1/map/entities`)

**Descrição**: Obtém entidades (estabelecimentos, espaços públicos, etc.) do território.

**Como usar**:
- Exige autenticação
- Query params: `territoryId` (opcional, usa território ativo se não informado)
- Header `X-Session-Id` para identificar território ativo

**Regras de negócio**:
- **Visibilidade**:
  - Visitantes: Veem apenas entidades `PUBLIC`
  - Moradores validados: Veem entidades `PUBLIC` e `RESIDENTS_ONLY`
- **Bloqueios**: Entidades de usuários bloqueados não aparecem
- **Status**: Apenas entidades com status `VALIDATED` ou `SUGGESTED` são retornadas

### Sugerir Entidade (`POST /api/v1/map/entities`)

**Descrição**: Sugere uma nova entidade territorial (estabelecimento, espaço público, etc.).

**Como usar**:
- Exige autenticação
- Body: nome, categoria, `latitude`, `longitude`, visibilidade (PUBLIC, RESIDENTS_ONLY)

**Regras de negócio**:
- **Permissão**: Visitantes e moradores podem sugerir
- **Geolocalização**: Obrigatória
- **Status**: Entidade é criada como `SUGGESTED` (aguarda validação)
- **Categoria**: Tipos válidos: "estabelecimento", "espaço público", "espaço natural", etc.

### Validar Entidade (`PATCH /api/v1/map/entities/{entityId}/validation`)

**Descrição**: Valida ou rejeita uma entidade sugerida (curadoria).

**Como usar**:
- Exige autenticação
- Path param: `entityId`
- Body: `validated=true` ou `validated=false`

**Regras de negócio**:
- **Permissão**: Apenas curadores (usuários com role CURATOR) podem validar
- **Status**: Se validada, status muda para `VALIDATED`
- **Idempotente**: Pode validar múltiplas vezes sem efeito adicional

### Confirmar Entidade (`POST /api/v1/map/entities/{entityId}/confirmations`)

**Descrição**: Confirma uma entidade no mapa (marcar como relevante).

**Como usar**:
- Exige autenticação
- Path param: `entityId`
- Query param: `territoryId` (obrigatório)

**Regras de negócio**:
- **Permissão**: Todos usuários autenticados podem confirmar
- **Idempotente**: Múltiplas confirmações são contabilizadas uma vez por usuário
- **Contagem**: Entidades retornam contagem de confirmações

### Relacionar-se com Entidade (`POST /api/v1/map/entities/{entityId}/relations`)

**Descrição**: Relaciona um morador com uma entidade (ex: "sou morador deste estabelecimento").

**Como usar**:
- Exige autenticação
- Path param: `entityId`

**Regras de negócio**:
- **Permissão**: Apenas moradores verificados (RESIDENT + `ResidencyVerification != NONE`) podem se relacionar
- **Idempotente**: Relação é única por usuário/entidade
- **Uso**: Usado para identificar moradores vinculados a entidades específicas

### Obter Pins do Mapa (`GET /api/v1/map/pins`)

**Descrição**: Obtém todos os pontos georreferenciados do território (entidades, posts, eventos, assets, alertas).

**Como usar**:
- Exige autenticação
- Query params: `territoryId` (opcional), `type` (filtro opcional: entity, post, asset, alert, event)
- Header `X-Session-Id` para identificar território ativo

**Regras de negócio**:
- **Visibilidade**: Respeita regras de visibilidade de cada tipo de conteúdo
- **Filtros**: `type` filtra por tipo de pin
- **Retorno**: Dados mínimos para projeção no mapa (coordenadas, ID, tipo, título básico)

---

## 🚨 Alertas de Saúde

### Reportar Alerta (`POST /api/v1/alerts`)

**Descrição**: Reporta um alerta de saúde pública no território.

**Como usar**:
- Exige autenticação
- Body: `territoryId`, título, descrição
- Header `X-Session-Id` para usar território ativo

**Regras de negócio**:
- **Permissão**: Visitantes e moradores podem reportar alertas
- **Limites**: Título máximo 200 caracteres, descrição máximo 2000 caracteres
- **Status**: Alerta é criado como `PENDING` (aguarda validação)
- **Post automático**: Cria automaticamente um post do tipo ALERT no feed
- **Feature Flag**: Só funciona se feature flag de alertas estiver habilitada no território

### Listar Alertas (`GET /api/v1/alerts`)

**Descrição**: Lista alertas do território.

**Como usar**:
- Exige autenticação
- Query params: `territoryId` (opcional), `skip`, `take` (paginação)
- Header `X-Session-Id` para identificar território ativo

**Regras de negócio**:
- **Visibilidade**: Apenas alertas validados (`VALIDATED`) são retornados
- **Paginação**: Padrão 20 itens
- **Ordenação**: Mais recentes primeiro

### Validar Alerta (`PATCH /api/v1/alerts/{alertId}/validation`)

**Descrição**: Valida ou rejeita um alerta (curadoria).

**Como usar**:
- Exige autenticação
- Path param: `alertId`
- Body: `validated=true` ou `validated=false`

**Regras de negócio**:
- **Permissão**: Apenas curadores (CURATOR) podem validar
- **Status**: Se validado, status muda para `VALIDATED` e post correspondente é publicado
- **Idempotente**: Pode validar múltiplas vezes

---

## 📦 Assets (Recursos Territoriais)

**TerritoryAssets** representam recursos valiosos do território que pertencem ao próprio território (naturais, culturais, comunitários, infraestruturais, simbólicos). TerritoryAssets não são vendáveis e não devem ser tratados como produtos ou serviços. Mídia (foto, vídeo, documento, link) deve ser tratada como registro/evidência associada a um TerritoryAsset, Event ou Post, não como TerritoryAsset em si.

### Criar Asset (`POST /api/v1/assets`)

**Descrição**: Cria um recurso territorial valioso (ex.: trilha, nascente, ponto cultural, infraestrutura comunitária).

**Como usar**:
- Exige autenticação
- Body: `territoryId`, nome, descrição, tipo, `geoAnchors` (obrigatório)

**Regras de negócio**:
- **Permissão**: Apenas moradores verificados (RESIDENT + `ResidencyVerification != NONE`) ou curadores podem criar
- **Geolocalização**: Obrigatória (pelo menos um GeoAnchor)
- **Status**: Asset é criado como `PENDING` (aguarda validação)
- **Limites**: Nome máximo 200 caracteres, descrição máxima 1000 caracteres
- **Não vendável**: TerritoryAssets não podem ser vendidos ou transferidos via marketplace

### Listar Assets (`GET /api/v1/assets`)

**Descrição**: Lista assets do território.

**Como usar**:
- Exige autenticação
- Query params: `territoryId` (opcional), `assetId` (filtro), `type` (filtro), `skip`, `take` (paginação)
- Header `X-Session-Id` para identificar território ativo

**Regras de negócio**:
- **Visibilidade**: Apenas assets validados (`VALIDATED`) são retornados
- **Filtros**: `assetId` e `type` são opcionais
- **Paginação**: Padrão 20 itens

### Validar Asset (`POST /api/v1/assets/{assetId}/validate`)

**Descrição**: Valida um asset (curadoria).

**Como usar**:
- Exige autenticação
- Path param: `assetId`

**Regras de negócio**:
- **Permissão**: Apenas curadores (CURATOR) podem validar
- **Status**: Se validado, status muda para `VALIDATED`
- **Idempotente**: Pode validar múltiplas vezes
- **Contagem**: Assets retornam contagem de validações e percentual

---

## 🏪 Marketplace

O Marketplace lida exclusivamente com produtos e serviços oferecidos por moradores. Stores e Items não são TerritoryAssets e não podem vender ou transferir TerritoryAssets. Produtos/serviços podem referenciar um TerritoryAsset apenas de forma contextual (ex.: "Serviço de guia na trilha X"), sem implicar propriedade ou venda do asset.

### Criar Store (`POST /api/v1/stores`)

**Descrição**: Cria uma loja/comércio no território para operação econômica de um morador.

**Como usar**:
- Exige autenticação
- Body: `territoryId`, nome, descrição, contato, `contactVisibility`

**Regras de negócio**:
- **Permissão**: Apenas moradores verificados (geo/doc) podem criar stores (curadores podem gerenciar stores de terceiros)
- **Limites**: Nome máximo 200 caracteres, descrição máxima 2000 caracteres
- **Status**: Store é criada como `ACTIVE`
- **Contato**: `contactVisibility` define se contato é público ou privado
- **Não é Asset**: Store representa operação econômica, não é um TerritoryAsset

### Criar Item (`POST /api/v1/items`)

**Descrição**: Cria um produto ou serviço em uma store (oferecido por um morador).

**Como usar**:
- Exige autenticação
- Body: `territoryId`, `storeId`, título, descrição, tipo (PRODUCT, SERVICE), `pricingType`, preço (opcional)

**Regras de negócio**:
- **Permissão**: Apenas moradores verificados (geo/doc) podem criar items
- **Tipos**: PRODUCT (produto) ou SERVICE (serviço)
- **Preço**: Pode ser FREE, FIXED (preço fixo), NEGOTIABLE (negociável)
- **Status**: Item é criado como `ACTIVE`
- **Não vende Assets**: Items não podem vender ou transferir TerritoryAssets; podem apenas referenciar contextualmente (ex.: serviço de guia relacionado a uma trilha)

### Buscar Items (`GET /api/v1/items`)

**Descrição**: Busca produtos e serviços no marketplace.

**Como usar**:
- Exige autenticação
- Query params: `territoryId` (opcional), `storeId` (filtro), `type` (filtro), `q` (busca de texto), `skip`, `take` (paginação)
- Header `X-Session-Id` para identificar território ativo

**Regras de negócio**:
- **Visibilidade**: Apenas items ativos (`ACTIVE`) são retornados
- **Filtros**: `storeId`, `type`, `q` são opcionais e combinados
- **Paginação**: Padrão 20 itens

### Criar Inquiry (`POST /api/v1/items/{itemId}/inquiries`)

**Descrição**: Cria uma consulta sobre um item (interesse em comprar/contratar).

**Como usar**:
- Exige autenticação
- Path param: `itemId`
- Body: `message` (mensagem)

**Regras de negócio**:
- **Permissão**: Todos usuários autenticados podem criar inquiries
- **Status**: Inquiry é criado como `OPEN`
- **Notificação**: Owner da store recebe notificação

### Carrinho e Checkout

**Descrição**: Sistema de carrinho e checkout para produtos.

**Como usar**:
- `POST /api/v1/cart`: Adiciona item ao carrinho
- `GET /api/v1/cart`: Obtém itens do carrinho
- `PUT /api/v1/cart/{itemId}`: Atualiza quantidade
- `DELETE /api/v1/cart/{itemId}`: Remove item
- `POST /api/v1/cart/checkout`: Finaliza compra

**Regras de negócio**:
- **Carrinho**: Por usuário e território
- **Checkout**: Calcula taxas de plataforma (se configuradas)
- **Permissão**: Todos usuários autenticados podem usar carrinho

---

## 🔔 Notificações

### Listar Notificações (`GET /api/v1/notifications`)

**Descrição**: Obtém notificações do usuário autenticado.

**Como usar**:
- Exige autenticação
- Query params: `skip`, `take` (paginação)

**Regras de negócio**:
- **Paginação**: Padrão 50 itens
- **Ordenação**: Mais recentes primeiro
- **Tipos**: Post criado, report criado, inquiry recebido, etc.
- **Sistema**: Notificações são geradas via outbox/inbox confiável

### Marcar como Lida (`POST /api/v1/notifications/{id}/read`)

**Descrição**: Marca uma notificação como lida.

**Como usar**:
- Exige autenticação
- Path param: `id` (ID da notificação)

**Regras de negócio**:
- **Permissão**: Apenas o dono da notificação pode marcar como lida
- **Idempotente**: Pode marcar múltiplas vezes sem efeito

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

## 🔗 Solicitações de Entrada (Join Requests)

> Nota: o caminho recomendado para "virar morador" é `POST /api/v1/memberships/{territoryId}/become-resident`,
> que cria a JoinRequest com destinatários automáticos. O endpoint abaixo existe para casos avançados (escolha manual).

### Criar Solicitação (`POST /api/v1/territories/{territoryId}/join-requests`)

**Descrição**: Solicita aprovação para virar morador (escolhendo destinatários específicos).

**Como usar**:
- Exige autenticação
- Path param: `territoryId`
- Body: `recipientUserIds` (array de IDs de usuários destinatários)

**Regras de negócio**:
- **Permissão**: Visitantes autenticados podem criar solicitações
- **Destinatários**: Apenas moradores já verificados (geo/doc) ou curadores podem ser destinatários (SystemAdmin também é aceito)
- **Status**: Solicitação é criada como `PENDING`
- **Não gera post**: Solicitação não aparece no feed (não é broadcast)
- **Privacidade**: Apenas destinatários veem a solicitação

### Listar Solicitações Recebidas (`GET /api/v1/join-requests/incoming`)

**Descrição**: Lista solicitações onde o usuário é destinatário.

**Como usar**:
- Exige autenticação
- Query params: `status` (PENDING, APPROVED, REJECTED), `skip`, `take` (paginação)

**Regras de negócio**:
- **Permissão**: Apenas destinatários veem suas solicitações recebidas
- **Filtros**: `status` é opcional
- **Paginação**: Padrão 20 itens

### Aprovar Solicitação (`POST /api/v1/join-requests/{id}/approve`)

**Descrição**: Aprova uma solicitação de entrada.

**Como usar**:
- Exige autenticação
- Path param: `id` (ID da solicitação)

**Regras de negócio**:
- **Permissão**: Apenas destinatários da solicitação ou curadores podem aprovar
- **Promoção**: Ao aprovar, o requester recebe membership `RESIDENT` com `ResidencyVerification=NONE` (não verificado)
- **Status**: Solicitação é marcada como `APPROVED`

### Rejeitar Solicitação (`POST /api/v1/join-requests/{id}/reject`)

**Descrição**: Rejeita uma solicitação de entrada.

**Como usar**:
- Exige autenticação
- Path param: `id` (ID da solicitação)

**Regras de negócio**:
- **Permissão**: Apenas destinatários da solicitação ou curadores podem rejeitar
- **Não promove**: Rejeição não altera membership do requester
- **Status**: Solicitação é marcada como `REJECTED`

### Cancelar Solicitação (`POST /api/v1/join-requests/{id}/cancel`)

**Descrição**: Cancela uma solicitação criada pelo próprio usuário.

**Como usar**:
- Exige autenticação
- Path param: `id` (ID da solicitação)

**Regras de negócio**:
- **Permissão**: Apenas o criador da solicitação pode cancelar
- **Status**: Solicitação é marcada como `CANCELLED`

---

## ⚙️ Feature Flags

### Listar Feature Flags (`GET /api/v1/territories/{territoryId}/features`)

**Descrição**: Obtém feature flags habilitadas no território.

**Como usar**:
- Exige autenticação
- Path param: `territoryId`

**Regras de negócio**:
- **Permissão**: Todos usuários autenticados podem consultar
- **Retorno**: Lista de flags **habilitadas** no território (strings em `UPPERCASE`, sem underscores).
- **Exemplos**: `ALERTPOSTS`, `MARKETPLACEENABLED`, `CHATENABLED`, etc.

### Atualizar Feature Flags (`PUT /api/v1/territories/{territoryId}/features`)

**Descrição**: Atualiza feature flags do território (curadoria).

**Como usar**:
- Exige autenticação
- Path param: `territoryId`
- Body: `enabledFlags: string[]` (lista de flags habilitadas, em qualquer casing)

**Regras de negócio**:
- **Permissão**: Apenas curadores (CURATOR) podem atualizar
- **Validação**: Flags inválidas são rejeitadas
- **Auditoria**: Alterações são registradas em log

---

## 🔒 Regras de Visibilidade e Permissões

### Visibilidade de Conteúdo

**PUBLIC (Público)**:
- Visível para todos usuários autenticados
- Visitantes (VISITOR) podem ver
- Moradores (RESIDENT) podem ver

**RESIDENTS_ONLY (Apenas Moradores)**:
- Visível apenas para moradores verificados (RESIDENT + `ResidencyVerification != NONE`)
- Visitantes não veem
- Moradores não verificados (RESIDENT + `ResidencyVerification = NONE`) não veem

### Permissões por Role

**VISITOR (Visitante)**:
- ✅ Ver posts públicos
- ✅ Ver eventos públicos
- ✅ Ver entidades públicas do mapa
- ✅ Criar eventos
- ✅ Reportar alertas
- ✅ Sugerir entidades
- ✅ Reportar posts/usuários
- ✅ Bloquear usuários
- ✅ Criar solicitações de entrada
- ❌ Ver conteúdo RESIDENTS_ONLY
- ❌ Comentar posts
- ❌ Compartilhar posts
- ❌ Criar stores/items
- ❌ Criar assets
- ❌ Relacionar-se com entidades

**RESIDENT (não verificado)**:
- ✅ Todas permissões de VISITOR
- ❌ Ver conteúdo RESIDENTS_ONLY
- ❌ Criar stores/items
- ❌ Criar assets
- ❌ Relacionar-se com entidades

**RESIDENT (verificado)**:
- ✅ Todas permissões de VISITOR
- ✅ Ver conteúdo RESIDENTS_ONLY
- ✅ Comentar posts
- ✅ Compartilhar posts
- ✅ Criar stores/items
- ✅ Criar assets
- ✅ Relacionar-se com entidades

**CURATOR (Curador)**:
- ✅ Todas permissões de RESIDENT (verificado)
- ✅ Validar entidades
- ✅ Validar alertas
- ✅ Validar assets
- ✅ Listar reports
- ✅ Atualizar feature flags
- ✅ Aprovar/rejeitar join requests

### Sanções

**PostingRestriction (Restrição de Postagem)**:
- Usuário não pode criar posts no território
- Usuário não pode criar eventos
- Usuário não pode criar alertas

**Scope (Escopo de Sanção)**:
- **TERRITORY**: Sanção aplicada apenas ao território específico
- **GLOBAL**: Sanção aplicada a todos os territórios

**Duração**:
- Sanções podem ter data de início e fim
- Sanções ativas são verificadas automaticamente

---

## 📚 Resumo de Endpoints Principais

### Autenticação
- `POST /api/v1/auth/social` - Login/Cadastro social

### Territórios
- `GET /api/v1/territories` - Listar territórios
- `GET /api/v1/territories/nearby` - Territórios próximos
- `GET /api/v1/territories/search` - Buscar territórios
- `GET /api/v1/territories/{id}` - Consultar território
- `POST /api/v1/territories/suggestions` - Sugerir território
- `POST /api/v1/territories/selection` - Selecionar território ativo
- `GET /api/v1/territories/selection` - Consultar território ativo

### Memberships
- `POST /api/v1/territories/{id}/membership` - Declarar vínculo
- `GET /api/v1/territories/{id}/membership/me` - Consultar vínculo
- `POST /api/v1/territories/{id}/membership/upgrade` - Atualizar para RESIDENT

### Feed
- `POST /api/v1/feed` - Criar post
- `GET /api/v1/feed` - Listar feed
- `GET /api/v1/feed/me` - Feed pessoal
- `POST /api/v1/feed/{id}/likes` - Curtir post
- `POST /api/v1/feed/{id}/comments` - Comentar post
- `POST /api/v1/feed/{id}/shares` - Compartilhar post

### Eventos
- `POST /api/v1/events` - Criar evento
- `GET /api/v1/events` - Listar eventos
- `GET /api/v1/events/nearby` - Eventos próximos
- `POST /api/v1/events/{id}/interest` - Marcar interesse
- `POST /api/v1/events/{id}/confirm` - Confirmar participação
- `POST /api/v1/events/{id}/cancel` - Cancelar evento

### Mapa
- `GET /api/v1/map/entities` - Listar entidades
- `POST /api/v1/map/entities` - Sugerir entidade
- `PATCH /api/v1/map/entities/{id}/validation` - Validar entidade
- `POST /api/v1/map/entities/{id}/confirmations` - Confirmar entidade
- `POST /api/v1/map/entities/{id}/relations` - Relacionar-se com entidade
- `GET /api/v1/map/pins` - Obter pins do mapa

### Alertas
- `POST /api/v1/alerts` - Reportar alerta
- `GET /api/v1/alerts` - Listar alertas
- `PATCH /api/v1/alerts/{id}/validation` - Validar alerta

### Assets
- `POST /api/v1/assets` - Criar asset
- `GET /api/v1/assets` - Listar assets
- `POST /api/v1/assets/{id}/validate` - Validar asset

### Marketplace
- `POST /api/v1/stores` - Criar store
- `GET /api/v1/stores` - Listar stores
- `POST /api/v1/items` - Criar item
- `GET /api/v1/items` - Buscar items
- `POST /api/v1/items/{id}/inquiries` - Criar inquiry
- `POST /api/v1/cart` - Adicionar ao carrinho
- `GET /api/v1/cart` - Obter carrinho
- `POST /api/v1/cart/checkout` - Finalizar compra
  - Observação: o módulo de marketplace é controlado por flag territorial `MARKETPLACEENABLED`. Quando desabilitado no território, endpoints de consulta/ação retornam `404` para evitar exposição do marketplace.

### Chat
- `GET /api/v1/territories/{territoryId}/chat/channels` - Listar canais do território (Público/Moradores)
- `GET /api/v1/territories/{territoryId}/chat/groups` - Listar grupos do território (apenas ativos)
- `POST /api/v1/territories/{territoryId}/chat/groups` - Criar grupo (pendente de aprovação)
- `POST /api/v1/territories/{territoryId}/chat/groups/{groupId}/approve` - Aprovar/habilitar grupo (curadoria)
- `POST /api/v1/territories/{territoryId}/chat/groups/{groupId}/disable` - Desabilitar grupo (moderação)
- `POST /api/v1/territories/{territoryId}/chat/groups/{groupId}/lock` - Trancar grupo (moderação)
- `GET /api/v1/chat/conversations/{conversationId}` - Detalhes da conversa
- `GET /api/v1/chat/conversations/{conversationId}/messages` - Listar mensagens (cursor-based)
- `POST /api/v1/chat/conversations/{conversationId}/messages` - Enviar mensagem
- `GET /api/v1/chat/conversations/{conversationId}/participants` - Listar participantes
- `POST /api/v1/chat/conversations/{conversationId}/participants` - Adicionar participante (owner/admin)
- `DELETE /api/v1/chat/conversations/{conversationId}/participants/{userId}` - Remover participante
- `POST /api/v1/chat/conversations/{conversationId}/read` - Marcar conversa como lida

### Notificações
- `GET /api/v1/notifications` - Listar notificações
- `POST /api/v1/notifications/{id}/read` - Marcar como lida

### Moderação
- `POST /api/v1/reports/posts/{id}` - Reportar post
- `POST /api/v1/reports/users/{id}` - Reportar usuário
- `GET /api/v1/reports` - Listar reports (curadoria)
- `POST /api/v1/users/{id}/block` - Bloquear usuário
- `DELETE /api/v1/users/{id}/block` - Desbloquear usuário

### Join Requests
- `POST /api/v1/territories/{id}/join-requests` - Criar solicitação
- `GET /api/v1/join-requests/incoming` - Listar recebidas
- `POST /api/v1/join-requests/{id}/approve` - Aprovar
- `POST /api/v1/join-requests/{id}/reject` - Rejeitar
- `POST /api/v1/join-requests/{id}/cancel` - Cancelar

### Feature Flags
- `GET /api/v1/territories/{id}/features` - Listar flags
- `PUT /api/v1/territories/{id}/features` - Atualizar flags (curadoria)

---

## 💬 Chat (Canais, Grupos e DM)

### Objetivo
Fornecer comunicação em tempo real/assíncrona entre usuários com governança territorial, respeitando:
- **Papéis territoriais**: `VISITOR` e `RESIDENT`
- **Capabilidades territoriais**: `CURATOR` e `MODERATOR`
- **Permissões globais**: `SYSTEM_ADMIN`
- **Privacidade** (bloqueio e preferências) e **anti-spam**
- **Feature flags por território** para rollout seguro

### Tipos de conversa (ConversationKind)
- **`TERRITORY_PUBLIC`**: canal público do território (leitura para membros do território; escrita restrita).
- **`TERRITORY_RESIDENTS`**: canal exclusivo de moradores validados e usuários verificados.
- **`GROUP`**: grupo privado (invite-only), criado por moradores validados/verificados e **habilitado por curadoria**.
- **`DIRECT`**: DM (habilitável por território via flag, e sempre respeitando preferências/bloqueios).

### Feature flags (por território)
Todas as operações de chat devem checar flags antes de qualquer acesso ao banco (cacheável).

> Observação: a API de feature flags hoje serializa `FeatureFlag.ToString().ToUpperInvariant()`.  
> Portanto, os valores trafegados tendem a ser como `CHATENABLED`, `CHATGROUPS`, etc. (sem underscores).

- **`CHATENABLED`** (`FeatureFlag.ChatEnabled`): master switch do chat no território.
- **`CHATTERITORYPUBLICCHANNEL`** (`FeatureFlag.ChatTerritoryPublicChannel`): habilita o canal público.
- **`CHATTERITORYRESIDENTSCHANNEL`** (`FeatureFlag.ChatTerritoryResidentsChannel`): habilita o canal de moradores.
- **`CHATGROUPS`** (`FeatureFlag.ChatGroups`): habilita criação/consulta de grupos.
- **`CHATDMENABLED`** (`FeatureFlag.ChatDmEnabled`): habilita DM no território.
- **`CHATMEDIAENABLED`** (`FeatureFlag.ChatMediaEnabled`): habilita envio/visualização de mídia (fase 2).

### Regras de permissão (resumo)
**Premissas**:
- “Usuário verificado” = `User.IdentityVerificationStatus == Verified`.
- “Morador validado” = `IsResidentAsync(userId, territoryId) == true`.

#### Canais do território
- **`TERRITORY_PUBLIC`**
  - **Ler**: usuário autenticado com membership no território (`VISITOR` ou `RESIDENT`).
  - **Escrever**: usuário verificado **e** morador validado.
- **`TERRITORY_RESIDENTS`**
  - **Ler/Escrever**: usuário verificado **e** morador validado.

#### Grupos
- **Criar grupo**: usuário verificado **e** morador validado (**visitante não cria**).
- **Estado inicial**: `PENDING_APPROVAL` (não aparece na descoberta do território).
- **Aprovar/habilitar**: `CURATOR` do território (ou `SYSTEM_ADMIN`).
- **Trancar/desabilitar**: `MODERATOR` do território (ou `SYSTEM_ADMIN`).
- **Participação**: invite-only (admin/owner adiciona/removem participantes).

#### DM (Direct)
- **Habilitação**: depende de flag territorial `CHATDMENABLED` (`FeatureFlag.ChatDmEnabled`).
- **Iniciar**: usuário verificado e permitido pelas preferências do destinatário (`contactVisibility`/chat settings) e por `UserBlock`.
- **Ler/Escrever**: apenas participantes (ou `SYSTEM_ADMIN`).

### Privacidade e bloqueios
- **Bloqueio (`UserBlock`)**:
  - bloqueia DM/convites e impede interação direta entre `A` e `B`.
  - (opcional fase 2) pode filtrar exibição de mensagens em grupos/canais.
- **Preferências**:
  - defaults já protegem contra spam (`contactVisibility: ResidentsOnly`).
  - (planejado) chat settings específicos: quem pode iniciar DM, convites, recibos de leitura etc.

### Conteúdo das mensagens (MVP e evolução)
- **MVP**: texto simples.
- **Fase 2** (atrás de flag):
  - **Mídia** (imagem/anexo) com storage externo + URL assinada e validações.
  - **Referências** a posts/eventos/assets do território (payload estruturado com checagem de acesso no read).

### Performance (recomendação)
- Paginação de mensagens **cursor-based**: `before=<messageId|timestamp>&limit=<N>`.
- Evitar N+1 e agregações pesadas:
  - manter `conversation_stats` (última mensagem/preview/contagem).
  - manter estado do participante (`last_read_*`, mute).

---

## 👤 Preferências de Usuário

### Obter Preferências (`GET /api/v1/users/me/preferences`)

**Descrição**: Obtém as preferências de privacidade e notificações do usuário autenticado.

**Como usar**:
- Requisição autenticada (token JWT obrigatório)
- Retorna preferências existentes ou cria preferências padrão se não existirem

**Regras de negócio**:
- Se o usuário não tiver preferências configuradas, retorna valores padrão:
  - `profileVisibility`: `Public`
  - `contactVisibility`: `ResidentsOnly`
  - `shareLocation`: `false`
  - `showMemberships`: `true`
  - Todas as notificações habilitadas por padrão

**Resposta**:
- **200 OK**: Preferências do usuário
- **401 Unauthorized**: Token inválido ou ausente

### Atualizar Preferências de Privacidade (`PUT /api/v1/users/me/preferences/privacy`)

**Descrição**: Atualiza as preferências de privacidade do usuário autenticado.

**Como usar**:
- Body: `profileVisibility` (Public, ResidentsOnly, Private), `contactVisibility` (Public, ResidentsOnly, Private), `shareLocation` (boolean), `showMemberships` (boolean)

**Regras de negócio**:
- `profileVisibility`: Controla quem pode ver o perfil do usuário
  - `Public`: Visível para todos
  - `ResidentsOnly`: Apenas moradores dos territórios onde o usuário é membro
  - `Private`: Apenas o próprio usuário
- `contactVisibility`: Controla visibilidade de email, telefone e endereço
  - `Public`: Visível para todos
  - `ResidentsOnly`: Apenas moradores validados
  - `Private`: Nunca visível publicamente
- `shareLocation`: Permite compartilhamento de localização
- `showMemberships`: Permite exibir territórios onde o usuário é membro

**Resposta**:
- **200 OK**: Preferências atualizadas
- **400 Bad Request**: Valores inválidos para enums
- **401 Unauthorized**: Token inválido ou ausente

### Atualizar Preferências de Notificações (`PUT /api/v1/users/me/preferences/notifications`)

**Descrição**: Atualiza as preferências de notificações do usuário autenticado.

**Como usar**:
- Body: Flags booleanas para cada tipo de notificação:
  - `postsEnabled`: Notificações de novos posts
  - `commentsEnabled`: Notificações de comentários
  - `eventsEnabled`: Notificações de eventos
  - `alertsEnabled`: Notificações de alertas
  - `marketplaceEnabled`: Notificações do marketplace
  - `moderationEnabled`: Notificações de moderação
  - `membershipRequestsEnabled`: Notificações de solicitações de entrada

**Regras de negócio**:
- Cada tipo de notificação pode ser habilitado/desabilitado independentemente
- Quando desabilitado, o usuário não receberá notificações daquele tipo
- Notificações do sistema (não categorizadas) sempre são enviadas

**Resposta**:
- **200 OK**: Preferências atualizadas
- **401 Unauthorized**: Token inválido ou ausente

### Obter Perfil (`GET /api/v1/users/me/profile`)

**Descrição**: Obtém o perfil do usuário autenticado.

**Como usar**:
- Requisição autenticada (token JWT obrigatório)
- Retorna informações do perfil do próprio usuário

**Regras de negócio**:
- Usuário sempre vê todas as suas próprias informações
- Regras de visibilidade se aplicam apenas quando outros usuários visualizam o perfil

**Resposta**:
- **200 OK**: Perfil do usuário
- **401 Unauthorized**: Token inválido ou ausente

### Atualizar Nome de Exibição (`PUT /api/v1/users/me/profile/display-name`)

**Descrição**: Atualiza o nome de exibição do usuário autenticado.

**Como usar**:
- Body: `displayName` (string, obrigatório, não vazio)

**Regras de negócio**:
- Nome de exibição é obrigatório
- Nome é normalizado (trim de espaços)
- Nome atualizado é refletido imediatamente em todas as operações

**Resposta**:
- **200 OK**: Perfil atualizado
- **400 Bad Request**: Nome vazio ou inválido
- **401 Unauthorized**: Token inválido ou ausente

### Atualizar Informações de Contato (`PUT /api/v1/users/me/profile/contact`)

**Descrição**: Atualiza as informações de contato do usuário autenticado.

**Como usar**:
- Body: `email` (opcional), `phoneNumber` (opcional), `address` (opcional)
- Todos os campos são opcionais, mas pelo menos um deve ser fornecido

**Regras de negócio**:
- Campos opcionais podem ser atualizados independentemente
- Valores são normalizados (trim de espaços)
- Visibilidade das informações de contato é controlada por `contactVisibility` nas preferências

**Resposta**:
- **200 OK**: Perfil atualizado
- **401 Unauthorized**: Token inválido ou ausente

---

**Documento gerado em**: 2025-01-13  
**Versão da API**: v1  
**Status**: Produção
