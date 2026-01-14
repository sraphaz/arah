# Plano de Refatoração: Asset → TerritoryAsset, TerritoryStore → Store, StoreListing → StoreItem

**Data**: 2025-01-13  
**Status**: 📋 Em Planejamento

---

## 📋 Objetivo

Refatorar o código para usar nomenclatura consistente:
- **Asset** → **TerritoryAsset** (não existe Asset genérico)
- **TerritoryStore** → **Store** (a loja pertence ao resident, não ao territory como conceito)
- **StoreListing** → **StoreItem** (produtos e serviços da loja são "items", alinhado com CartItem/CheckoutItem)

---

## 🎯 Regras de Negócio Confirmadas

1. **TerritoryAsset**:
   - Não existe "Asset" genérico, apenas "TerritoryAsset"
   - Um TerritoryAsset pode ter uma lista de localizações (GeoAnchors) OU um perímetro
   - Um morador **cadastra** os assets do território, mas **não é o dono**
   - Cada ativo do território pode ter sua classificação (Type)

2. **Store**:
   - Não existe "TerritoryStore", apenas "Store"
   - A Store pertence a um **resident**, não ao territory como conceito
   - Cada resident pode ter sua loja (Store) com lista de produtos e serviços (Items)

3. **StoreItem**:
   - Não existe "StoreListing", apenas "StoreItem"
   - Alinha com nomenclatura existente: CartItem, CheckoutItem
   - StoreItems podem ser produtos (Product) ou serviços (Service)
   - StoreItems não são TerritoryAssets e não podem vender TerritoryAssets

---

## 📊 Análise de Impacto

### StoreListing → StoreItem

**Classes de Domínio**:
- `StoreListing` → `StoreItem`

**Enums**:
- `ListingType` → `ItemType` (Product vs Service)
- `ListingStatus` → `ItemStatus`
- `ListingPricingType` → `ItemPricingType`

**Relacionamentos**:
- `ListingInquiry` → `ItemInquiry`

**Interfaces**:
- `IListingRepository` → `IStoreItemRepository`

**Serviços**:
- `ListingService` → `StoreItemService`

**Controllers**:
- `ListingsController` → `StoreItemsController`

**Contracts**:
- `ListingResponse` → `StoreItemResponse`
- `CreateListingRequest` → `CreateStoreItemRequest`
- `UpdateListingRequest` → `UpdateStoreItemRequest`

**Repositórios**:
- `PostgresListingRepository` → `PostgresStoreItemRepository`
- `InMemoryListingRepository` → `InMemoryStoreItemRepository`

**Entities (Records)**:
- `StoreListingRecord` → `StoreItemRecord`
- `ListingInquiryRecord` → `ItemInquiryRecord`

**Campos em outras entidades**:
- `CartItem.ListingId` → `CartItem.ItemId`
- `CheckoutItem.ListingId` → `CheckoutItem.ItemId`
- `CheckoutItem.ListingType` → `CheckoutItem.ItemType`
- `ItemInquiry.ListingId` → `ItemInquiry.ItemId`

**Tabelas de Banco**:
- `store_listings` → `store_items` (requer migration)
- `listing_inquiries` → `item_inquiries` (requer migration)

**Variáveis/Parâmetros**:
- `listing` → `item`
- `listings` → `items`
- `listingId` → `itemId`

### TerritoryStore → Store

**Classes de Domínio**:
- `TerritoryStore` → `Store`

**Interfaces**:
- `IStoreRepository` → **MANTER** (já usa `Store`)
- Mas métodos retornam `TerritoryStore` → mudar para `Store`

**Repositórios**:
- `PostgresStoreRepository` → **MANTER** (já está correto)
- `InMemoryStoreRepository` → **MANTER** (já está correto)
- Mas implementam métodos que retornam `TerritoryStore` → mudar para `Store`

**Entities (Records)**:
- `TerritoryStoreRecord` → `StoreRecord`

**Serviços**:
- `StoreService` → **MANTER** (já está correto)
- Mas usa `TerritoryStore` → mudar para `Store`

**Controllers**:
- `StoresController` → **MANTER** (já está correto)
- Mas usa `TerritoryStore` → mudar para `Store`

**Contracts**:
- Já usam "Store" → **MANTER**

**Variáveis/Parâmetros**:
- `territoryStore` → `store`
- `territoryStores` → `stores`

**Tabelas de Banco**:
- `territory_stores` → `stores` (requer migration)

### Asset → TerritoryAsset

**Interfaces a Renomear**:
- `IAssetRepository` → `ITerritoryAssetRepository`
- `IAssetGeoAnchorRepository` → `ITerritoryAssetGeoAnchorRepository` (ou manter, depende)
- `IAssetValidationRepository` → `ITerritoryAssetValidationRepository` (ou manter, depende)

**Serviços a Renomear**:
- `AssetService` → `TerritoryAssetService`

**Modelos a Renomear**:
- `AssetDetails` → `TerritoryAssetDetails`
- `AssetGeoAnchorInput` → `TerritoryAssetGeoAnchorInput`
- `AssetValidationResult` → `TerritoryAssetValidationResult`

**Record/Entity Classes a Manter ou Renomear**:
- `AssetStatus` → **MANTER** (enum, não precisa mudar)
- `AssetGeoAnchor` → **MANTER** (é um value object/record)
- `AssetValidation` → **MANTER** (é uma relação)
- `PostAsset` → **MANTER** (é uma relação, não um asset)

**Controllers**:
- `AssetsController` → `TerritoryAssetsController`

**Contracts**:
- `AssetResponse` → `TerritoryAssetResponse`
- `CreateAssetRequest` → `CreateTerritoryAssetRequest`
- `UpdateAssetRequest` → `UpdateTerritoryAssetRequest`
- `ArchiveAssetRequest` → `ArchiveTerritoryAssetRequest`
- `AssetGeoAnchorRequest` → `TerritoryAssetGeoAnchorRequest`
- `AssetGeoAnchorResponse` → `TerritoryAssetGeoAnchorResponse`
- `AssetValidationResponse` → `TerritoryAssetValidationResponse`

**Repositórios**:
- `PostgresAssetRepository` → `PostgresTerritoryAssetRepository`
- `InMemoryAssetRepository` → `InMemoryTerritoryAssetRepository`

**Entities (Records)**:
- `TerritoryAssetRecord` → **MANTER** (já está correto)
- `AssetGeoAnchorRecord` → **MANTER** ou renomear para `TerritoryAssetGeoAnchorRecord`
- `AssetValidationRecord` → **MANTER** ou renomear para `TerritoryAssetValidationRecord`
- `PostAssetRecord` → **MANTER** (é uma relação)

**Variáveis/Parâmetros**:
- `asset` → `territoryAsset`
- `assets` → `territoryAssets`

---

## 📝 Plano de Execução

### Fase 1: Preparação e Documentação
- [x] Criar plano de refatoração
- [ ] Criar branch de refatoração
- [ ] Documentar mudanças esperadas

### Fase 2: Renomear StoreListing → StoreItem (Domínio)
1. Renomear classe de domínio `StoreListing` → `StoreItem`
2. Renomear enums: `ListingType` → `ItemType`, `ListingStatus` → `ItemStatus`, `ListingPricingType` → `ItemPricingType`
3. Renomear `ListingInquiry` → `ItemInquiry`
4. Atualizar propriedades e métodos
5. Atualizar comentários XML

### Fase 3: Renomear StoreListing → StoreItem (Infraestrutura)
1. Renomear entity `StoreListingRecord` → `StoreItemRecord`
2. Renomear `ListingInquiryRecord` → `ItemInquiryRecord`
3. Atualizar repositórios: `IListingRepository` → `IStoreItemRepository`
4. Atualizar `PostgresListingRepository` → `PostgresStoreItemRepository`
5. Atualizar `InMemoryListingRepository` → `InMemoryStoreItemRepository`
6. Atualizar mappers
7. Criar migrations para renomear tabelas

### Fase 4: Renomear StoreListing → StoreItem (Application e API)
1. Renomear `ListingService` → `StoreItemService`
2. Renomear controllers: `ListingsController` → `StoreItemsController`
3. Renomear contracts (ListingResponse, CreateListingRequest, etc.)
4. Atualizar rotas da API
5. Atualizar CartItem e CheckoutItem (ListingId → ItemId)

### Fase 5: Renomear TerritoryStore → Store
1. Renomear classe de domínio `TerritoryStore` → `Store`
2. Renomear entity `TerritoryStoreRecord` → `StoreRecord`
3. Atualizar todos os usos de `TerritoryStore` para `Store`
4. Atualizar mappers
5. Atualizar testes
6. Criar migration para renomear tabela (se necessário)

### Fase 6: Renomear Asset → TerritoryAsset (Interfaces e Serviços)
1. Renomear `IAssetRepository` → `ITerritoryAssetRepository`
2. Renomear `AssetService` → `TerritoryAssetService`
3. Renomear `AssetDetails` → `TerritoryAssetDetails`
4. Atualizar todos os usos
5. Atualizar DI container

### Fase 7: Renomear Asset → TerritoryAsset (Controllers e Contracts)
1. Renomear `AssetsController` → `TerritoryAssetsController`
2. Renomear contracts (AssetResponse, CreateAssetRequest, etc.)
3. Atualizar rotas da API

### Fase 8: Renomear Asset → TerritoryAsset (Repositórios)
1. Renomear `PostgresAssetRepository` → `PostgresTerritoryAssetRepository`
2. Renomear `InMemoryAssetRepository` → `InMemoryTerritoryAssetRepository`
3. Atualizar todos os usos

### Fase 9: Atualizar Variáveis e Parâmetros
1. Atualizar nomes de variáveis de `asset` para `territoryAsset`
2. Atualizar nomes de variáveis de `territoryStore` para `store`
3. Atualizar nomes de variáveis de `listing` para `item`

### Fase 10: Testes e Validação
1. Executar testes
2. Corrigir testes quebrados
3. Validar que a compilação funciona
4. Validar que os testes passam

### Fase 11: Documentação
1. Atualizar documentação da API
2. Atualizar comentários
3. Atualizar README se necessário

---

## ⚠️ Considerações Importantes

1. **Breaking Changes**: Esta refatoração causa breaking changes na API
   - Endpoints `/api/v1/assets` → `/api/v1/territory-assets` (ou manter se preferir)
   - Endpoints `/api/v1/listings` → `/api/v1/store-items` (ou `/api/v1/items`)
   - Contracts mudam de nome

2. **Migrations**: Será necessário criar migrations para:
   - Renomear tabela `territory_stores` → `stores`
   - Renomear tabela `store_listings` → `store_items`
   - Renomear tabela `listing_inquiries` → `item_inquiries`
   - Renomear colunas `listing_id` → `item_id` em várias tabelas

3. **Testes**: Muitos testes precisarão ser atualizados

4. **Ordem de Execução**: 
   - Primeiro StoreListing → StoreItem (menor impacto, mais isolado)
   - Depois TerritoryStore → Store (menor impacto que Asset)
   - Por último Asset → TerritoryAsset (maior impacto)

---

## 📊 Estatísticas Estimadas

- **Arquivos a modificar**: ~120-150 arquivos
- **Classes/Interfaces a renomear**: ~35-40
- **Métodos a atualizar**: ~300+
- **Testes a atualizar**: ~50-70
- **Migrations a criar**: 3-4

---

**Status**: Aguardando aprovação para iniciar execução
