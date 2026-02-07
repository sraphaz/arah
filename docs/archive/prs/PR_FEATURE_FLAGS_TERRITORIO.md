# PR: Feature Flags por Território (Hardening)

**Branch**: `feat/feature-flags-territory`  
**Status**: ✅ Build e testes passando

---

## 📋 Resumo

Este PR fortalece o modelo de **feature flags por território**, eliminando bypasses e padronizando gates em pontos críticos (Feed/Alertas, Marketplace e Chat/DM), com documentação atualizada e testes de regressão.

---

## 🎯 Principais mudanças

### 1) Guard centralizado por território
- Adicionado `TerritoryFeatureFlagGuard` para concentrar decisões de flags e reduzir vazamentos.

### 2) Feed/Alertas: `ALERTPOSTS` sem bypass
- O fluxo de validação de alertas (Health) não publica mais post `ALERT` no feed quando `ALERTPOSTS` estiver desabilitada.

### 3) Marketplace: `MARKETPLACEENABLED` aplicado em leitura e ações
- Busca/listagem de itens, carrinho, checkout e inquiries agora respeitam `MARKETPLACEENABLED`.
- Em endpoints públicos/consultas, **flag OFF → `404`** (fail-closed, reduz exposição do módulo).

### 4) Chat DM: `CHATDMENABLED` fail-safe
- Acesso a conversas `DIRECT` passa a exigir `ChatEnabled` + `ChatDmEnabled` (e nega se `TerritoryId` não existir).

---

## 🔌 Impacto em API (comportamento relevante)

- Marketplace desabilitado no território:
  - `GET /api/v1/items` → `404`
  - `GET /api/v1/items/paged` → `404`
  - `GET /api/v1/items/{id}` → `404` (via serviço)
  - `GET /api/v1/cart` → `404`
  - `POST /api/v1/cart/items` → `404`
  - `POST /api/v1/cart/checkout` → `404`
  - `POST /api/v1/items/{id}/inquiries` → `404`

---

## 📁 Arquivos principais

### Código
- `backend/Arah.Application/Services/TerritoryFeatureFlagGuard.cs`
- `backend/Arah.Application/Services/HealthService.cs`
- `backend/Arah.Application/Services/StoreItemService.cs`
- `backend/Arah.Application/Services/CartService.cs`
- `backend/Arah.Application/Services/InquiryService.cs`
- `backend/Arah.Application/Services/ChatService.cs`
- `backend/Arah.Api/Controllers/ItemsController.cs`
- `backend/Arah.Api/Controllers/CartController.cs`
- `backend/Arah.Api/Controllers/InquiriesController.cs`
- `backend/Arah.Api/Extensions/ServiceCollectionExtensions.cs`

### Testes
- `backend/Arah.Tests/Application/ApplicationServiceTests.cs`
- `backend/Arah.Tests/Application/MarketplaceServiceTests.cs`

### Documentação
- `docs/60_API_LÓGICA_NEGÓCIO.md`
- `docs/11_ARCHITECTURE_SERVICES.md`

---

## ✅ Testes

Executado:

```bash
dotnet test Arah.sln -c Release
```

Resultado: **250/250 passando**.

