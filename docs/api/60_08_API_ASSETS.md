# Assets (Recursos Territoriais) - API Arah

**Parte de**: [API Arah - Lógica de Negócio e Usabilidade](./60_API_LÓGICA_NEGÓCIO_INDEX.md)  
**Versão**: 2.0  
**Data**: 2025-01-20

---

## 📦 Assets (Recursos Territoriais)

**TerritoryAssets** representam recursos valiosos do território que pertencem ao próprio território (naturais, culturais, comunitários, infraestruturais, simbólicos). TerritoryAssets não são vendáveis e não devem ser tratados como produtos ou serviços. Mídia (foto, vídeo, documento, link) deve ser tratada como registro/evidência associada a um TerritoryAsset, Event ou Post, não como TerritoryAsset em si.

**Corpos d'água**: rios, córregos, nascentes e fontes entram como assets/recursos naturais curáveis (ponte TerritoryAsset; alvo `NaturalAsset` na FASE24.0). Ver [CORPOS_DAGUA_TERRITORIO](../backlog-api/CORPOS_DAGUA_TERRITORIO.md). Não embutir no modelo `Territory`.

### Criar Asset (`POST /api/v1/assets`)

**Descrição**: Cria um recurso territorial valioso (ex.: trilha, **rio**, nascente, ponto cultural, infraestrutura comunitária).

**Como usar**:
- Exige autenticação
- Body: `territoryId`, `type`, `name`, `description?`, `geoAnchors` (obrigatório), `subtype?` (WA-E1)

**Exemplo hídrico (ponte)**:
```json
{
  "territoryId": "...",
  "type": "natural",
  "subtype": "river",
  "name": "Rio do Vale",
  "description": "Trecho urbano",
  "geoAnchors": [{ "latitude": -23.37, "longitude": -45.02 }]
}
```

**Regras de negócio**:
- **Permissão**: Apenas moradores verificados (RESIDENT + `ResidencyVerification != NONE`) ou curadores podem criar
- **Geolocalização**: Obrigatória (pelo menos um GeoAnchor)
- **Status**: Asset é criado como `Suggested` (aguarda curadoria → `Active`)
- **Subtype (WA-E1)**: opcional; se informado, `type` deve ser `natural` e subtype ∈ `river|stream|spring|waterfall|well|potable_water`
- **Limites**: Nome máximo 200 caracteres, descrição máxima 1000 caracteres, subtype máximo 40
- **Não vendável**: TerritoryAssets não podem ser vendidos ou transferidos via marketplace

**Resposta**: inclui `subtype` (nullable) além de `type`.

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

## 📚 Documentação Relacionada

- **[Mapa Territorial](./60_06_API_MAPA.md)** - Assets aparecem como pins no mapa
- **[Marketplace](./60_09_API_MARKETPLACE.md)** - Assets NÃO são vendáveis (diferenciação importante)
- **[Regras de Visibilidade](./60_17_API_VISIBILIDADE.md)** - Visibilidade de assets

---

**Voltar para**: [Índice da Documentação da API](./60_API_LÓGICA_NEGÓCIO_INDEX.md)
