# Marketplace - API Araponga

**Parte de**: [API Araponga - Lógica de Negócio e Usabilidade](./60_API_LÓGICA_NEGÓCIO_INDEX.md)  
**Versão**: 2.0  
**Data**: 2025-01-20

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

**Feature Flag**: O módulo de marketplace é controlado por flag territorial `MARKETPLACEENABLED`. Quando desabilitado no território, endpoints de consulta/ação retornam `404` para evitar exposição do marketplace.

---

## 📚 Documentação Relacionada

- **[Mídias em Conteúdo](./60_15_API_MIDIAS.md)** - Adicionar imagens, vídeos e áudios aos items
- **[Assets](./60_08_API_ASSETS.md)** - Diferenciação: Assets NÃO são vendáveis
- **[Feature Flags](./60_16_API_FEATURE_FLAGS.md)** - Controle de habilitação do marketplace
- **[Paginação](./60_00_API_PAGINACAO.md)** - Versão paginada: `GET /api/v1/items/paged`
- **DevPortal**: [Marketplace Checkout](../devportal/#operacao-marketplace-checkout) - Diagrama de sequência completo

---

**Voltar para**: [Índice da Documentação da API](./60_API_LÓGICA_NEGÓCIO_INDEX.md)
