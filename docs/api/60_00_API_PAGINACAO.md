# Paginação - API Araponga

**Parte de**: [API Araponga - Lógica de Negócio e Usabilidade](./60_API_LÓGICA_NEGÓCIO_INDEX.md)  
**Versão**: 2.0  
**Data**: 2025-01-20

---

## 📄 Paginação

Todos os endpoints de listagem têm versões paginadas disponíveis. O padrão de paginação é:

### Parâmetros de Paginação

- `pageNumber` (int, padrão: 1) - Número da página (1-indexed)
- `pageSize` (int, padrão: 20) - Itens por página (mínimo: 1, máximo: 100)

### Resposta Paginada

```json
{
  "items": [...],
  "pageNumber": 1,
  "pageSize": 20,
  "totalCount": 150,
  "totalPages": 8,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

### Endpoints com Paginação

- ✅ `GET /api/v1/territories/paged`
- ✅ `GET /api/v1/feed/paged`
- ✅ `GET /api/v1/feed/me/paged`
- ✅ `GET /api/v1/assets/paged`
- ✅ `GET /api/v1/alerts/paged`
- ✅ `GET /api/v1/events/paged`
- ✅ `GET /api/v1/events/nearby/paged`
- ✅ `GET /api/v1/map/entities/paged`
- ✅ `GET /api/v1/map/pins/paged`
- ✅ `GET /api/v1/notifications/paged`
- ✅ `GET /api/v1/inquiries/me/paged`
- ✅ `GET /api/v1/inquiries/received/paged`
- ✅ `GET /api/v1/join-requests/incoming/paged`
- ✅ `GET /api/v1/reports/paged`
- ✅ `GET /api/v1/items/paged`

**Nota**: Chat usa cursor-based pagination (`beforeCreatedAtUtc`/`beforeMessageId`) em vez de paginação numérica.

---

## 📚 Documentação Relacionada

- **[Visão Geral](./60_00_API_VISAO_GERAL.md)** - Princípios fundamentais da API
- **[Resumo de Endpoints](./60_99_API_RESUMO_ENDPOINTS.md)** - Lista completa de endpoints

---

**Voltar para**: [Índice da Documentação da API](./60_API_LÓGICA_NEGÓCIO_INDEX.md)
