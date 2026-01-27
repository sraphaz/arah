# Resumo de Contratos BFF - Referência Rápida

**Data**: 2026-01-27  
**Versão**: 2.0.0  
**Base URL**: `/api/v2/journeys`

---

## 📋 Endpoints Principais

### Onboarding

#### `POST /onboarding/complete`
Completa onboarding em uma chamada.

**Request**:
```json
{
  "selectedTerritoryId": "uuid",
  "location": {
    "latitude": -23.5505,
    "longitude": -46.6333
  }
}
```

**Response**:
```json
{
  "user": { "id": "uuid", "displayName": "...", "membership": "VISITOR" },
  "territory": { "id": "uuid", "name": "...", "active": true },
  "initialFeed": { "items": [], "pagination": {...} },
  "suggestedActions": [
    { "action": "REQUEST_RESIDENCY", "title": "...", "priority": "HIGH" }
  ]
}
```

---

### Feed

#### `GET /feed/territory-feed`
Feed do território formatado para UI.

**Query Params**:
- `territoryId` (required): UUID
- `pageNumber` (default: 1)
- `pageSize` (default: 20, max: 100)
- `filterByInterests` (default: false)
- `mapEntityId` (optional): UUID
- `assetId` (optional): UUID

**Response**:
```json
{
  "items": [
    {
      "post": { "id": "uuid", "title": "...", "content": "...", "type": "POST" },
      "counts": { "likes": 15, "shares": 3, "comments": 5 },
      "media": [{ "url": "...", "type": "IMAGE" }],
      "event": null,
      "author": { "id": "uuid", "displayName": "..." },
      "userInteractions": { "liked": false, "shared": false, "commented": false },
      "metadata": { "canEdit": false, "canDelete": false, "canShare": true }
    }
  ],
  "pagination": { "pageNumber": 1, "pageSize": 20, "totalCount": 42, "hasNextPage": true },
  "filters": {
    "availableTypes": ["POST", "ALERT", "EVENT"],
    "availableTags": ["evento", "comunidade"],
    "availableVisibilities": ["PUBLIC", "RESIDENTS_ONLY"]
  }
}
```

#### `POST /feed/create-post`
Cria post com mídias em uma chamada.

**Query Params**:
- `territoryId` (required): UUID

**Request** (multipart/form-data):
- `title` (string, required)
- `content` (string, required)
- `type` (enum: POST, ALERT, EVENT)
- `visibility` (enum: PUBLIC, RESIDENTS_ONLY)
- `mediaFiles[]` (files, max 10)
- `tags[]` (array of strings, max 10)
- `mapEntityId` (UUID, optional)
- `geoAnchors[]` (array, optional)
- `assetIds[]` (array of UUIDs, optional)

**Response**:
```json
{
  "success": true,
  "post": { "id": "uuid", "title": "...", ... },
  "mediaUrls": ["https://..."],
  "suggestions": {
    "similarPosts": [...],
    "suggestedTags": [...]
  }
}
```

#### `POST /feed/interact`
Interage com post (like, comment, share).

**Request**:
```json
{
  "postId": "uuid",
  "action": "LIKE", // LIKE, UNLIKE, COMMENT, SHARE
  "comment": "..." // obrigatório se action=COMMENT
}
```

**Response**:
```json
{
  "success": true,
  "post": { ... },
  "updatedCounts": { "likes": 16, "shares": 3, "comments": 5 }
}
```

---

### Events

#### `GET /events/territory-events`
Lista eventos do território formatados.

**Query Params**:
- `territoryId` (required): UUID
- `from` (date-time, optional)
- `to` (date-time, optional)
- `status` (enum: SCHEDULED, ONGOING, COMPLETED, CANCELLED)
- `pageNumber` (default: 1)
- `pageSize` (default: 20)

**Response**:
```json
{
  "items": [
    {
      "event": {
        "id": "uuid",
        "title": "...",
        "startsAtUtc": "2026-01-27T10:00:00Z",
        "interestedCount": 10,
        "confirmedCount": 5
      },
      "participants": {
        "interested": [...],
        "confirmed": [...]
      },
      "media": {
        "coverImageUrl": "...",
        "additionalImageUrls": [...]
      },
      "userParticipation": {
        "status": "INTERESTED",
        "confirmed": false
      },
      "suggestions": {
        "similarEvents": [...],
        "nearbyEvents": [...]
      }
    }
  ],
  "pagination": {...}
}
```

#### `POST /events/create-event`
Cria evento com mídias.

**Request** (multipart/form-data):
- `territoryId` (UUID, required)
- `title` (string, required)
- `description` (string, optional)
- `startsAtUtc` (date-time, required)
- `endsAtUtc` (date-time, optional)
- `latitude` (double, optional)
- `longitude` (double, optional)
- `locationLabel` (string, optional)
- `coverMediaFile` (file, optional)
- `additionalMediaFiles[]` (files, max 9)

**Response**:
```json
{
  "success": true,
  "event": { ... },
  "mediaUrls": {
    "coverImageUrl": "...",
    "additionalImageUrls": [...]
  }
}
```

#### `POST /events/participate`
Participa de evento.

**Request**:
```json
{
  "eventId": "uuid",
  "action": "INTEREST" // INTEREST, CONFIRM, CANCEL
}
```

**Response**:
```json
{
  "success": true,
  "event": { ... },
  "userParticipation": {
    "status": "INTERESTED",
    "confirmed": false
  }
}
```

---

### Marketplace

#### `GET /marketplace/search`
Busca itens formatados para UI.

**Query Params**:
- `territoryId` (required): UUID
- `query` (string, optional)
- `category` (string, optional)
- `type` (enum: PRODUCT, SERVICE)
- `minPrice` (decimal, optional)
- `maxPrice` (decimal, optional)
- `pageNumber` (default: 1)
- `pageSize` (default: 20)

**Response**:
```json
{
  "items": [
    {
      "item": {
        "id": "uuid",
        "title": "...",
        "pricingType": "FIXED",
        "priceAmount": 50.00,
        "currency": "BRL"
      },
      "store": { "id": "uuid", "name": "..." },
      "media": {
        "primaryImageUrl": "...",
        "imageUrls": [...]
      },
      "availability": {
        "inStock": true,
        "quantity": 10
      }
    }
  ],
  "stores": [...],
  "pagination": {...},
  "filters": {
    "availableCategories": [...],
    "availableTypes": ["PRODUCT", "SERVICE"],
    "priceRange": { "min": 0, "max": 1000 }
  },
  "suggestions": {
    "trendingItems": [...],
    "recommendedItems": [...]
  }
}
```

#### `POST /marketplace/add-to-cart`
Adiciona item ao carrinho.

**Request**:
```json
{
  "territoryId": "uuid",
  "itemId": "uuid",
  "quantity": 1,
  "notes": "..." // optional
}
```

**Response**:
```json
{
  "success": true,
  "cart": {
    "cartId": "uuid",
    "items": [...],
    "total": {
      "subtotal": 50.00,
      "platformFee": 2.50,
      "total": 52.50,
      "currency": "BRL"
    }
  },
  "item": { ... },
  "suggestions": {
    "frequentlyBoughtTogether": [...],
    "similarItems": [...]
  }
}
```

#### `POST /marketplace/checkout`
Finaliza compra.

**Request**:
```json
{
  "territoryId": "uuid",
  "paymentMethod": "PIX", // CREDIT_CARD, DEBIT_CARD, PIX, BANK_TRANSFER
  "shippingAddress": {
    "street": "...",
    "number": "...",
    "neighborhood": "...",
    "city": "...",
    "state": "...",
    "zipCode": "...",
    "country": "BR"
  },
  "message": "..." // optional
}
```

**Response**:
```json
{
  "success": true,
  "order": {
    "id": "uuid",
    "status": "CONFIRMED",
    "total": { ... },
    "items": [...]
  },
  "payment": {
    "id": "uuid",
    "method": "PIX",
    "status": "COMPLETED",
    "amount": 52.50
  },
  "confirmation": {
    "orderId": "uuid",
    "confirmationNumber": "ORD-123456",
    "estimatedDelivery": "2026-01-30T10:00:00Z"
  }
}
```

---

## 🔐 Autenticação

### Header Obrigatório

```http
Authorization: Bearer <token_jwt>
```

### Obter Token

```http
POST /api/v1/auth/social
Content-Type: application/json

{
  "provider": "GOOGLE",
  "token": "...",
  "cpf": "..." // optional
}
```

---

## ⚠️ Códigos de Status

| Código | Significado |
|--------|-------------|
| `200` | Sucesso |
| `201` | Criado |
| `400` | Requisição Inválida |
| `401` | Não Autenticado |
| `402` | Pagamento Necessário |
| `404` | Não Encontrado |
| `429` | Muitas Requisições |
| `500` | Erro do Servidor |

### Estrutura de Erro

```json
{
  "error": "Invalid request parameters",
  "code": "INVALID_REQUEST",
  "details": {
    "field": "title",
    "message": "Title is required"
  },
  "timestamp": "2026-01-27T10:00:00Z"
}
```

---

## 📚 Documentação Completa

- **Contrato OpenAPI**: [BFF_API_CONTRACT.yaml](./BFF_API_CONTRACT.yaml)
- **Guia de Implementação**: [BFF_FRONTEND_IMPLEMENTATION_GUIDE.md](./BFF_FRONTEND_IMPLEMENTATION_GUIDE.md)
- **Avaliação BFF**: [AVALIACAO_BFF_BACKEND_FOR_FRONTEND.md](./AVALIACAO_BFF_BACKEND_FOR_FRONTEND.md)

---

**Última Atualização**: 2026-01-27  
**Status**: 📋 Resumo de Referência - Pronto para Uso
