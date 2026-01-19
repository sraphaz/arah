# Alertas de Saúde - API Araponga

**Parte de**: [API Araponga - Lógica de Negócio e Usabilidade](./60_API_LÓGICA_NEGÓCIO_INDEX.md)  
**Versão**: 2.0  
**Data**: 2025-01-20

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

## 📚 Documentação Relacionada

- **[Feed Comunitário](./60_04_API_FEED.md)** - Alertas criam posts automaticamente
- **[Mapa Territorial](./60_06_API_MAPA.md)** - Alertas aparecem como pins no mapa
- **[Feature Flags](./60_16_API_FEATURE_FLAGS.md)** - Controle de habilitação de alertas
- **[Paginação](./60_00_API_PAGINACAO.md)** - Versão paginada: `GET /api/v1/alerts/paged`

---

**Voltar para**: [Índice da Documentação da API](./60_API_LÓGICA_NEGÓCIO_INDEX.md)
