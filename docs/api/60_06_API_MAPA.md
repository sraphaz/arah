# Mapa Territorial - API Araponga

**Parte de**: [API Araponga - Lógica de Negócio e Usabilidade](./60_API_LÓGICA_NEGÓCIO_INDEX.md)  
**Versão**: 2.0  
**Data**: 2025-01-20

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

### Obter Pins do Mapa Paginados (`GET /api/v1/map/pins/paged`)

**Descrição**: Obtém pins do mapa com paginação.

**Como usar**:
- Exige autenticação
- Query params: `territoryId` (opcional), `type` (filtro opcional), `pageNumber` (padrão: 1), `pageSize` (padrão: 20)
- Header `X-Session-Id` para identificar território ativo

**Regras de negócio**:
- **Paginação**: Padrão 20 itens por página
- **Visibilidade**: Respeita regras de visibilidade de cada tipo de conteúdo
- **Filtros**: `type` filtra por tipo de pin
- **Retorno**: `PagedResponse<MapPinResponse>` com metadados de paginação

---

## 📚 Documentação Relacionada

- **[Feed Comunitário](./60_04_API_FEED.md)** - Posts podem referenciar entidades do mapa
- **[Eventos](./60_05_API_EVENTOS.md)** - Eventos aparecem como pins no mapa
- **[Assets](./60_08_API_ASSETS.md)** - Assets aparecem como pins no mapa
- **[Alertas](./60_07_API_ALERTAS.md)** - Alertas aparecem como pins no mapa
- **[Regras de Visibilidade](./60_17_API_VISIBILIDADE.md)** - Visibilidade de entidades

---

**Voltar para**: [Índice da Documentação da API](./60_API_LÓGICA_NEGÓCIO_INDEX.md)
