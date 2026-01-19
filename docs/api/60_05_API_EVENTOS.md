# Eventos - API Araponga

**Parte de**: [API Araponga - Lógica de Negócio e Usabilidade](./60_API_LÓGICA_NEGÓCIO_INDEX.md)  
**Versão**: 2.0  
**Data**: 2025-01-20

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

## 📚 Documentação Relacionada

- **[Mídias em Conteúdo](./60_15_API_MIDIAS.md)** - Adicionar imagens, vídeos e áudios aos eventos
- **[Feed Comunitário](./60_04_API_FEED.md)** - Posts criados automaticamente para eventos
- **[Mapa Territorial](./60_06_API_MAPA.md)** - Eventos aparecem como pins no mapa
- **[Paginação](./60_00_API_PAGINACAO.md)** - Versão paginada: `GET /api/v1/events/paged`

---

**Voltar para**: [Índice da Documentação da API](./60_API_LÓGICA_NEGÓCIO_INDEX.md)
