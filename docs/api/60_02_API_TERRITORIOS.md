# Territórios - API Araponga

**Parte de**: [API Araponga - Lógica de Negócio e Usabilidade](./60_API_LÓGICA_NEGÓCIO_INDEX.md)  
**Versão**: 2.0  
**Data**: 2025-01-20

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
- Retorna dados geográficos e **`Tags`** (caracterização do território), definidas pela comunidade via votações ou curadoria ([Governança](./60_19_API_GOVERNANCA.md))
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

**Nota**: Endpoints que retornam `TerritoryResponse` (listagem, nearby, search, selection) incluem o campo **`Tags`** (caracterização do território) quando disponível.

## 📚 Documentação Relacionada

- **[Governança](./60_19_API_GOVERNANCA.md)** - Caracterização do território (tags) e votações
- **[Vínculos e Membros](./60_03_API_MEMBERSHIPS.md)** - Próximo passo após selecionar território
- **[Visão Geral](./60_00_API_VISAO_GERAL.md)** - Princípios fundamentais
- **DevPortal**: [Territórios](../devportal/#territorios) - Exemplos práticos

---

**Voltar para**: [Índice da Documentação da API](./60_API_LÓGICA_NEGÓCIO_INDEX.md)
