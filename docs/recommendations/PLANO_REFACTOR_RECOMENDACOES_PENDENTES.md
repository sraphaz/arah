# Plano de Implementação: Recomendações Pendentes de Refatoração

## Resumo

Este documento detalha o plano de implementação para as recomendações pendentes identificadas na revisão de código e no PR de refatoração anterior.

## Recomendações Pendentes

### 1. Migrar todos os services para usar `Result<T>` ✅ ESTRUTURA CRIADA

**Status Atual:**
- ✅ `Result<T>` e `OperationResult` já criados em `Araponga.Application/Common/Result.cs`
- ❌ Ainda há 29 métodos usando tuplas `(bool success, string? error, T? result)`

**Services que precisam migração:**
- `PostInteractionService` (2 métodos)
- `FeedService` (3 métodos)
- `PostCreationService` (1 método)
- `StoreService` (4 métodos)
- `MapService` (3 métodos)
- `ListingService` (3 métodos)
- `CartService` (3 métodos)
- `EventsService` (4 métodos)
- `InquiryService` (1 método)
- `HealthService` (1 método)
- `AssetService` (4 métodos)

**Plano de Implementação:**
1. Migrar um service por vez (começar pelos mais simples)
2. Atualizar controllers para usar `Result<T>`
3. Atualizar testes para refletir mudanças
4. Documentar padrão de uso

**Prioridade:** Alta

---

### 2. Adicionar paginação em todos os métodos de listagem ✅ ESTRUTURA CRIADA

**Status Atual:**
- ✅ `PagedResult<T>` e `PaginationParameters` já criados
- ✅ `FeedService.ListForTerritoryPagedAsync` já implementado
- ❌ Ainda há 21 métodos retornando `IReadOnlyList<T>` sem paginação

**Services que precisam paginação:**
- `FeedService` (2 métodos: `ListForTerritoryAsync`, `ListForUserAsync`)
- `TerritoryService` (3 métodos: `ListAvailableAsync`, `SearchAsync`, `NearbyAsync`)
- `MapService` (1 método: `ListEntitiesAsync`)
- `EventsService` (2 métodos: `ListEventsAsync`, `GetEventsNearbyAsync`)
- `ListingService` (1 método: `SearchListingsAsync`)
- `AssetService` (1 método: `ListAsync`)
- `ReportService` (1 método: `ListAsync`)
- `JoinRequestService` (1 método: `ListIncomingAsync`)
- `InquiryService` (2 métodos: `ListMyInquiriesAsync`, `ListReceivedInquiriesAsync`)
- `HealthService` (1 método: `ListAlertsAsync`)
- `PlatformFeeService` (1 método: `ListActiveAsync`)

**Plano de Implementação:**
1. Adicionar sobrecarga paginada para cada método (manter método antigo para compatibilidade)
2. Implementar paginação no nível do repositório quando possível
3. Adicionar limites padrão (ex: max 100 itens por página)
4. Atualizar controllers para usar métodos paginados
5. Documentar padrão de paginação

**Prioridade:** Alta

---

### 3. Implementar cache (TerritoryCacheService, FeatureFlagCacheService)

**Status Atual:**
- ❌ Nenhum cache implementado
- ❌ Dados frequentemente acessados são consultados do banco toda vez

**Oportunidades de Cache:**
- **Territories:** Lista de territórios ativos (mudam raramente)
- **Feature Flags:** Flags por território (mudam ocasionalmente)
- **Membership Status:** Status de membership (TTL curto, 5-10 minutos)
- **User Blocks:** Lista de bloqueios (TTL curto, 1-2 minutos)

**Plano de Implementação:**
1. Adicionar `Microsoft.Extensions.Caching.Memory` ao projeto
2. Criar `TerritoryCacheService` com cache de territórios ativos
3. Criar `FeatureFlagCacheService` com cache de flags por território
4. Adicionar cache de membership status no `AccessEvaluator`
5. Implementar invalidação de cache quando dados mudam
6. Configurar TTLs apropriados
7. Considerar Redis para cache distribuído (futuro)

**Prioridade:** Média

---

### 4. Adicionar Serilog para logging estruturado

**Status Atual:**
- ✅ `RequestLoggingMiddleware` implementado
- ✅ `CorrelationIdMiddleware` implementado
- ❌ Usando `ILogger` padrão do ASP.NET Core (não estruturado)
- ❌ Falta contexto rico nos logs

**Plano de Implementação:**
1. Adicionar pacotes Serilog:
   - `Serilog.AspNetCore`
   - `Serilog.Sinks.Console`
   - `Serilog.Sinks.File` (opcional)
   - `Serilog.Enrichers.Environment`
   - `Serilog.Enrichers.Thread`
2. Configurar Serilog no `Program.cs`
3. Adicionar enrichers para:
   - Correlation ID
   - User ID (quando disponível)
   - Territory ID (quando disponível)
   - Request path e method
4. Atualizar `RequestLoggingMiddleware` para usar Serilog
5. Adicionar logging estruturado em operações críticas
6. Configurar níveis de log por ambiente

**Prioridade:** Média

---

### 5. Otimizar queries (evitar N+1, usar projections)

**Status Atual:**
- ❌ Possíveis queries N+1 em listagens
- ❌ Carregamento de entidades completas quando apenas subconjunto necessário
- ❌ Filtragem feita em memória após carregar todos os dados

**Oportunidades de Otimização:**
- **FeedService:** Carregar apenas campos necessários para listagem
- **MapService:** Usar projections para pins do mapa
- **EventsService:** Carregar apenas dados necessários para listagem
- **PostFilterService:** Fazer filtragem no nível do repositório

**Plano de Implementação:**
1. Identificar queries N+1 usando profiling
2. Implementar projections usando `Select` no LINQ
3. Mover filtragem para nível do repositório
4. Adicionar índices no banco de dados (quando usando Postgres)
5. Usar `Include` ou `Join` para carregar relacionamentos necessários
6. Documentar otimizações aplicadas

**Prioridade:** Baixa (mas importante para performance)

---

### 6. Implementar rate limiting

**Status Atual:**
- ❌ Nenhum rate limiting implementado
- ❌ Sistema vulnerável a abuso

**Plano de Implementação:**
1. Adicionar pacote `AspNetCoreRateLimit` ou `Microsoft.AspNetCore.RateLimiting`
2. Configurar rate limiting no `Program.cs`
3. Definir políticas por endpoint:
   - Endpoints públicos: limites mais altos
   - Endpoints de autenticação: limites mais baixos
   - Endpoints de criação: limites moderados
4. Implementar rate limiting por usuário (quando autenticado)
5. Implementar rate limiting por IP (para endpoints públicos)
6. Adicionar headers de rate limit nas respostas
7. Configurar mensagens de erro apropriadas

**Prioridade:** Média

---

## Ordem de Implementação Recomendada

### Fase 1 - Fundações (Alta Prioridade)
1. ✅ Migrar services para `Result<T>` (começar pelos mais simples)
2. ✅ Adicionar paginação em métodos de listagem críticos

### Fase 2 - Performance e Observabilidade (Média Prioridade)
3. Implementar cache para territories e feature flags
4. Adicionar Serilog para logging estruturado

### Fase 3 - Segurança e Otimização (Média/Baixa Prioridade)
5. Implementar rate limiting
6. Otimizar queries (N+1, projections)

---

## Estimativa de Esforço

- **Migração para Result<T>:** ~2-3 dias (29 métodos)
- **Paginação:** ~2-3 dias (21 métodos)
- **Cache:** ~1-2 dias
- **Serilog:** ~1 dia
- **Rate Limiting:** ~1-2 dias
- **Otimização de Queries:** ~2-3 dias

**Total estimado:** ~9-14 dias

---

## Checklist de Implementação

### Fase 1
- [ ] Migrar `PostInteractionService` para `Result<T>`
- [ ] Migrar `PostCreationService` para `Result<T>`
- [ ] Migrar `FeedService` para `Result<T>`
- [ ] Migrar outros services para `Result<T>`
- [ ] Adicionar paginação em `FeedService.ListForTerritoryAsync`
- [ ] Adicionar paginação em `TerritoryService.ListAvailableAsync`
- [ ] Adicionar paginação em outros métodos críticos

### Fase 2
- [ ] Implementar `TerritoryCacheService`
- [ ] Implementar `FeatureFlagCacheService`
- [ ] Adicionar cache de membership status
- [ ] Configurar Serilog
- [ ] Atualizar logging em operações críticas

### Fase 3
- [ ] Configurar rate limiting
- [ ] Implementar políticas por endpoint
- [ ] Identificar e corrigir queries N+1
- [ ] Implementar projections onde necessário
- [ ] Adicionar índices no banco de dados

---

## Notas

- Manter compatibilidade com código existente durante migração
- Atualizar testes conforme necessário
- Documentar mudanças em [docs/40_CHANGELOG.md](./40_CHANGELOG.md)
- Considerar breaking changes apenas se necessário
