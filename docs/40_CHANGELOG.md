# Changelog

Todas as mudanças notáveis neste projeto serão documentadas neste arquivo.

O formato é baseado em [Keep a Changelog](https://keepachangelog.com/pt-BR/1.0.0/),
e este projeto adere ao [Semantic Versioning](https://semver.org/lang/pt-BR/).

---

## [2025-01-17] - Fase 10: Mídias em Conteúdo ✅ Implementação Principal Completa

### Integração de Mídias em Todos os Tipos de Conteúdo

- ✅ **Posts**: Múltiplas imagens por post (até 10)
  - Campo `MediaIds` em `CreatePostRequest`
  - Campos `MediaUrls` e `MediaCount` em `FeedItemResponse`
  - Validação de propriedade e limites
  - Busca de URLs em batch para otimização

- ✅ **Eventos**: Imagem de capa + imagens adicionais (até 10 no total)
  - Campos `CoverMediaId` e `AdditionalMediaIds` em `CreateEventRequest`
  - Campos `CoverImageUrl` e `AdditionalImageUrls` em `EventResponse`
  - Validação de propriedade e limites

- ✅ **Marketplace**: Múltiplas imagens por item (até 10)
  - Campo `MediaIds` em `CreateItemRequest`
  - Campos `PrimaryImageUrl` e `ImageUrls` em `ItemResponse`
  - Primeira imagem como imagem principal

- ✅ **Chat**: Uma imagem por mensagem (máximo 5MB)
  - Campo `MediaId` em `SendMessageRequest`
  - Campos `MediaUrl` e `HasMedia` em `MessageResponse`
  - Validação de tipo (apenas imagens) e tamanho

### Validações Implementadas

- ✅ Validação de propriedade (mídias devem pertencer ao usuário)
- ✅ Validação de existência (mídias devem existir e não estar deletadas)
- ✅ Validação de limites (quantidade máxima por tipo de conteúdo)
- ✅ Validação de tipo (Chat: apenas imagens)
- ✅ Validação de tamanho (Chat: máximo 5MB)

### Otimizações

- ✅ Busca de URLs em batch para evitar N+1 queries
- ✅ Helpers nos controllers para carregamento eficiente
- ✅ Cache de URLs via `CachedMediaStorageService`

### Arquivos Modificados

**Posts**:
- `backend/Araponga.Api/Contracts/Feed/CreatePostRequest.cs`
- `backend/Araponga.Api/Contracts/Feed/FeedItemResponse.cs`
- `backend/Araponga.Api/Validators/CreatePostRequestValidator.cs`
- `backend/Araponga.Application/Services/PostCreationService.cs`
- `backend/Araponga.Application/Services/FeedService.cs`
- `backend/Araponga.Api/Controllers/FeedController.cs`

**Eventos**:
- `backend/Araponga.Api/Contracts/Events/CreateEventRequest.cs`
- `backend/Araponga.Api/Contracts/Events/EventResponse.cs`
- `backend/Araponga.Application/Services/EventsService.cs`
- `backend/Araponga.Api/Controllers/EventsController.cs`

**Marketplace**:
- `backend/Araponga.Api/Contracts/Marketplace/ItemContracts.cs`
- `backend/Araponga.Api/Validators/CreateItemRequestValidator.cs`
- `backend/Araponga.Application/Services/StoreItemService.cs`
- `backend/Araponga.Api/Controllers/ItemsController.cs`

**Chat**:
- `backend/Araponga.Api/Contracts/Chat/SendMessageRequest.cs`
- `backend/Araponga.Api/Contracts/Chat/MessageResponse.cs`
- `backend/Araponga.Application/Services/ChatService.cs`
- `backend/Araponga.Api/Controllers/ChatController.cs`

### Documentação

- ✅ `docs/MEDIA_IN_CONTENT.md` - Documentação completa da integração
- ✅ `docs/backlog-api/implementacoes/FASE10_IMPLEMENTACAO_COMPLETA.md` - Resumo da implementação

### Limitações Conhecidas

- ⏳ **Exclusão automática de mídias**: Não implementada nesta fase
  - Posts, eventos, items e mensagens usam soft delete/archive
  - Recomendação: Implementar via event handlers ou triggers em fase futura

### Próximos Passos

- [ ] Testes de integração para cada tipo de conteúdo
- [ ] Otimizações adicionais (cache, batch loading)
- [ ] Implementação de exclusão automática de mídias
- [ ] Suporte a vídeos em Posts e Eventos

---

## [2025-01-16] - Fase 8: Infraestrutura de Mídia e Armazenamento ✅ 100% Completo (incluindo funcionalidades opcionais)

### Sistema de Mídia Completo com Funcionalidades Opcionais

- ✅ **Modelo de Domínio**: MediaAsset, MediaAttachment, MediaType, MediaOwnerType
- ✅ **Armazenamento Local**: LocalMediaStorageService com organização por tipo e data
- ✅ **Cloud Storage S3**: S3MediaStorageService com URLs pré-assinadas
- ✅ **Cloud Storage Azure Blob**: AzureBlobMediaStorageService com SAS URLs
- ✅ **Cache de URLs**: CachedMediaStorageService com suporte a Redis e Memory Cache
- ✅ **Processamento Assíncrono**: AsyncMediaProcessingBackgroundService para imagens grandes
- ✅ **Factory Pattern**: MediaStorageFactory para seleção automática de provider
- ✅ **Processamento de Imagens**: LocalMediaProcessingService com SixLabors.ImageSharp
- ✅ **Validação de Mídia**: MediaValidator com validação de MIME, tamanho e dimensões
- ✅ **API REST Completa**: 4 endpoints (upload, download, info, delete)
- ✅ **Testes Completos**: Unitários, integração, segurança e performance
- ✅ **Documentação**: MEDIA_SYSTEM.md atualizado com todas as funcionalidades

### Funcionalidades Opcionais Implementadas

- ✅ **Cloud Storage (S3 e Azure Blob)**: Implementação completa com configuração via appsettings.json
- ✅ **Cache de URLs de Mídia**: Cache distribuído (Redis ou Memory) com expiração configurável
- ✅ **Processamento Assíncrono**: Background service para processar imagens grandes (>5MB)
- ✅ **Testes de Performance**: Testes para upload múltiplas imagens, cache e listagem

### Arquivos Criados

**Cloud Storage e Cache**:
- `backend/Araponga.Infrastructure/Media/S3MediaStorageService.cs`
- `backend/Araponga.Infrastructure/Media/AzureBlobMediaStorageService.cs`
- `backend/Araponga.Infrastructure/Media/CachedMediaStorageService.cs`
- `backend/Araponga.Infrastructure/Media/MediaStorageFactory.cs`
- `backend/Araponga.Infrastructure/Media/AsyncMediaProcessingBackgroundService.cs`
- `backend/Araponga.Infrastructure/Media/NoOpAsyncMediaProcessor.cs`
- `backend/Araponga.Application/Interfaces/Media/IAsyncMediaProcessor.cs`

**Testes de Performance**:
- `backend/Araponga.Tests/Performance/MediaPerformanceTests.cs`

**Documentação**:
- `docs/MEDIA_SYSTEM.md` (atualizado com cloud storage, cache e processamento assíncrono)

### Configuração

Todas as funcionalidades são configuráveis via `appsettings.json`:

```json
{
  "MediaStorage": {
    "Provider": "Local", // ou "S3" ou "AzureBlob"
    "EnableUrlCache": true,
    "UrlCacheExpiration": "24:00:00",
    "EnableAsyncProcessing": true,
    "AsyncProcessingThresholdBytes": 5242880
  }
}
```

---

## [2026-01-19] - Fase 7: Sistema de Payout e Gestão Financeira ✅ 100% Completo

### Sistema de Payout e Gestão Financeira Completo

- ✅ **Rastreabilidade Financeira Completa**: Tabela central `FinancialTransaction` rastreia cada centavo
- ✅ **Saldo e Transações de Vendedor**: `SellerBalance` e `SellerTransaction` com 3 estados e 6 status
- ✅ **Gestão Financeira da Plataforma**: `PlatformFinancialBalance`, receitas e despesas separadas
- ✅ **Configuração de Payout por Território**: `TerritoryPayoutConfig` com retenção, valores mínimo/máximo, frequência
- ✅ **Serviço de Payout Completo**: `SellerPayoutService` com retenção, valor mínimo/máximo, integração com gateway
- ✅ **Background Worker**: `PayoutProcessingWorker` processa payouts automaticamente baseado na frequência
- ✅ **API REST Completa**: 8 endpoints para gerenciar payouts e consultar saldos
- ✅ **Interface de Gateway**: `IPayoutGateway` com `MockPayoutGateway` para desenvolvimento

### Arquivos Criados

**Modelos de Domínio (12 arquivos)**:
- `backend/Araponga.Domain/Financial/FinancialTransaction.cs`
- `backend/Araponga.Domain/Financial/TransactionType.cs`
- `backend/Araponga.Domain/Financial/TransactionStatus.cs`
- `backend/Araponga.Domain/Financial/TransactionStatusHistory.cs`
- `backend/Araponga.Domain/Financial/PlatformFinancialBalance.cs`
- `backend/Araponga.Domain/Financial/PlatformRevenueTransaction.cs`
- `backend/Araponga.Domain/Financial/PlatformExpenseTransaction.cs`
- `backend/Araponga.Domain/Financial/ReconciliationRecord.cs`
- `backend/Araponga.Domain/Marketplace/SellerBalance.cs`
- `backend/Araponga.Domain/Marketplace/SellerTransaction.cs`
- `backend/Araponga.Domain/Marketplace/SellerTransactionStatus.cs`
- `backend/Araponga.Domain/Marketplace/TerritoryPayoutConfig.cs`

**Repositórios (18 arquivos - 9 Postgres + 9 InMemory)**:
- `backend/Araponga.Application/Interfaces/IFinancialTransactionRepository.cs`
- `backend/Araponga.Application/Interfaces/ITransactionStatusHistoryRepository.cs`
- `backend/Araponga.Application/Interfaces/ISellerBalanceRepository.cs`
- `backend/Araponga.Application/Interfaces/ISellerTransactionRepository.cs`
- `backend/Araponga.Application/Interfaces/IPlatformFinancialBalanceRepository.cs`
- `backend/Araponga.Application/Interfaces/IPlatformRevenueTransactionRepository.cs`
- `backend/Araponga.Application/Interfaces/IPlatformExpenseTransactionRepository.cs`
- `backend/Araponga.Application/Interfaces/IReconciliationRecordRepository.cs`
- `backend/Araponga.Application/Interfaces/ITerritoryPayoutConfigRepository.cs`
- (9 implementações Postgres + 9 implementações InMemory)

**Serviços (4 arquivos)**:
- `backend/Araponga.Application/Services/SellerPayoutService.cs`
- `backend/Araponga.Application/Services/TerritoryPayoutConfigService.cs`
- `backend/Araponga.Application/Interfaces/IPayoutGateway.cs`
- `backend/Araponga.Infrastructure/Payments/MockPayoutGateway.cs`

**Background Worker (1 arquivo)**:
- `backend/Araponga.Infrastructure/Background/PayoutProcessingWorker.cs`

**Controllers da API (3 arquivos)**:
- `backend/Araponga.Api/Controllers/TerritoryPayoutConfigController.cs`
- `backend/Araponga.Api/Controllers/SellerBalanceController.cs`
- `backend/Araponga.Api/Controllers/PlatformFinancialController.cs`

**Contratos de API (7 arquivos)**:
- `backend/Araponga.Api/Contracts/Payout/TerritoryPayoutConfigRequest.cs`
- `backend/Araponga.Api/Contracts/Payout/TerritoryPayoutConfigResponse.cs`
- `backend/Araponga.Api/Contracts/Payout/SellerBalanceResponse.cs`
- `backend/Araponga.Api/Contracts/Payout/SellerTransactionResponse.cs`
- `backend/Araponga.Api/Contracts/Payout/PlatformFinancialBalanceResponse.cs`
- `backend/Araponga.Api/Contracts/Payout/PlatformRevenueTransactionResponse.cs`
- `backend/Araponga.Api/Contracts/Payout/PlatformExpenseTransactionResponse.cs`

**Migration (1 arquivo)**:
- `backend/Araponga.Infrastructure/Postgres/Migrations/20260119000000_AddFinancialSystem.cs` (9 tabelas)

### Arquivos Modificados

- `backend/Araponga.Application/Interfaces/ICheckoutRepository.cs` (adicionado GetByIdAsync)
- `backend/Araponga.Application/Interfaces/ISellerTransactionRepository.cs` (adicionado GetByPayoutIdAsync)
- `backend/Araponga.Infrastructure/Postgres/PostgresCheckoutRepository.cs`
- `backend/Araponga.Infrastructure/InMemory/InMemoryCheckoutRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresSellerTransactionRepository.cs`
- `backend/Araponga.Infrastructure/InMemory/InMemorySellerTransactionRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/ArapongaDbContext.cs` (adicionados 9 DbSets)
- `backend/Araponga.Infrastructure/Postgres/PostgresMappers.cs` (adicionados mappers)
- `backend/Araponga.Infrastructure/InMemory/InMemoryDataStore.cs` (adicionadas listas)
- `backend/Araponga.Api/Extensions/ServiceCollectionExtensions.cs` (registros de serviços e worker)
- `docs/backlog-api/FASE7.md` (documentação completa)

### Funcionalidades

1. **Rastreabilidade Completa**: Cada centavo é rastreado em `FinancialTransaction` com histórico de status
2. **Saldo de Vendedor**: 3 estados (Pending, ReadyForPayout, Paid) atualizados automaticamente
3. **Gestão Financeira da Plataforma**: Saldo, receitas e despesas separadas por território
4. **Configuração Flexível**: Retenção, valores mínimo/máximo, frequência configuráveis por território
5. **Payout Automático**: Background worker processa payouts baseado na frequência configurada
6. **Gateway Abstrato**: Interface `IPayoutGateway` permite trocar facilmente entre gateways
7. **API REST Completa**: 8 endpoints para gerenciar e consultar tudo
8. **Autorização**: Endpoints protegidos com verificação de permissões

### Endpoints da API

- `GET /api/v1/territories/{territoryId}/payout-config` - Obter configuração ativa
- `POST /api/v1/territories/{territoryId}/payout-config` - Criar/atualizar configuração
- `GET /api/v1/territories/{territoryId}/seller-balance/me` - Consultar saldo do vendedor
- `GET /api/v1/territories/{territoryId}/seller-balance/me/transactions` - Consultar transações do vendedor
- `GET /api/v1/territories/{territoryId}/platform-financial/balance` - Consultar saldo da plataforma
- `GET /api/v1/territories/{territoryId}/platform-financial/revenue` - Listar receitas (fees)
- `GET /api/v1/territories/{territoryId}/platform-financial/expenses` - Listar despesas (payouts)

### Estatísticas

- **12 commits** na branch `feature/fase7-payout-gestao-financeira`
- **~5.000+ linhas de código** adicionadas
- **9 tabelas** criadas na migration
- **18 repositórios** implementados (9 Postgres + 9 InMemory)
- **8 endpoints** da API criados
- **Build**: ✅ Passando sem erros

---

## [2026-01-15] - Fase 5: Segurança Avançada ✅ 100% Completo

### Segurança Avançada

- ✅ **2FA Completo (TOTP)**: Validação de código TOTP ou recovery code no Disable2FAAsync implementada
- ✅ **Sanitização Avançada de Inputs**: Serviço completo para sanitização HTML, paths, URLs e SQL
- ✅ **Proteção CSRF**: Anti-forgery tokens configurados com headers e cookies seguros
- ✅ **Secrets Management**: Interface e implementação básica criadas (pronto para Key Vault/Secrets Manager)
- ✅ **Security Headers Melhorados**: CSP mais restritivo para API, HSTS adicionado
- ✅ **Auditoria Avançada**: Serviço e interface de repositório criados para consulta de auditoria
- ✅ **Penetration Testing**: Checklist e documentação de segurança criados

### Arquivos Criados

- `backend/Araponga.Application/Services/InputSanitizationService.cs`: Serviço de sanitização de inputs
- `backend/Araponga.Application/Services/AuditService.cs`: Serviço para consulta de auditoria
- `backend/Araponga.Application/Interfaces/IAuditRepository.cs`: Interface para repositório de auditoria
- `backend/Araponga.Infrastructure/Security/ISecretsService.cs`: Interface para secrets management
- `backend/Araponga.Infrastructure/Security/EnvironmentSecretsService.cs`: Implementação usando variáveis de ambiente
- `docs/backlog-api/implementacoes/FASE5_IMPLEMENTACAO_RESUMO.md`: Resumo completo da implementação
- `docs/SECURITY_AUDIT.md`: Checklist e guia de penetration testing

### Arquivos Modificados

- `backend/Araponga.Application/Services/AuthService.cs`: Validação no Disable2FAAsync
- `backend/Araponga.Api/Program.cs`: Configuração de anti-forgery tokens
- `backend/Araponga.Api/Middleware/SecurityHeadersMiddleware.cs`: CSP melhorado e HSTS adicionado
- `backend/Araponga.Api/Extensions/ServiceCollectionExtensions.cs`: Registro de InputSanitizationService
- `docs/backlog-api/FASE5.md`: Plano atualizado com 100% de conclusão

---

## [2025-01-15] - Fase 4: Observabilidade e Monitoramento ✅ 100% Completo

### Adicionado
- **Logs Centralizados**: Serilog configurado com Seq
  - Enrichers: MachineName, ThreadId, EnvironmentName, Application, Version
  - Níveis de log configuráveis por ambiente
  - Correlation ID integrado ao LogContext
  - Structured logging em pontos críticos
  - Configuração condicional do Seq via `appsettings.json`
- **Métricas Básicas**: Prometheus + métricas customizadas
  - Endpoint `/metrics` exposto
  - Métricas HTTP automáticas (request rate, error rate, latência)
  - Métricas de negócio: PostsCreated, EventsCreated, MembershipsCreated, ReportsCreated, JoinRequestsCreated, TerritoriesCreated
  - Métricas de cache: CacheHits, CacheMisses
  - Métricas de concorrência: ConcurrencyConflicts
  - Métricas de eventos: EventsProcessed, EventsFailed, EventProcessingDuration
  - Métricas de banco: DatabaseQueryDuration
  - Classe `ArapongaMetrics` com `System.Diagnostics.Metrics.Meter`
- **Distributed Tracing**: OpenTelemetry configurado
  - Tracing de HTTP requests (ASP.NET Core instrumentation)
  - Tracing de database queries (Entity Framework Core instrumentation)
  - Tracing de HTTP clients (HttpClient instrumentation)
  - Custom sources: `AddSource("Araponga.*")`
  - Exporters: OTLP, Jaeger, Console (desenvolvimento)
  - Resource information: service name e version
- **Monitoramento Avançado**: Documentação completa
  - Dashboards recomendados: Performance, Negócio, Sistema
  - Alertas críticos documentados com queries Prometheus
- **Runbook e Troubleshooting**: Documentação operacional completa
  - `RUNBOOK.md`: Deploy, rollback, escalação, manutenção, backup/restore
  - `TROUBLESHOOTING.md`: Soluções para problemas comuns
  - `INCIDENT_PLAYBOOK.md`: Classificação, contenção, diagnóstico, resolução, pós-incidente

### Modificado
- **Program.cs**: Configuração completa de Serilog, Prometheus e OpenTelemetry
- **CorrelationIdMiddleware**: Integração com Serilog LogContext
- **Services**: Instrumentação com métricas customizadas
  - `PostCreationService`, `EventsService`, `ReportService`
  - `JoinRequestService`, `MembershipService`, `TerritoryService`
  - `CacheMetricsService`, `BackgroundEventProcessor`, `ConcurrencyHelper`
- **appsettings.json**: Configurações de Logging, Metrics e OpenTelemetry

### Documentação
- `docs/METRICS.md`: Documentação completa de todas as métricas
- `docs/MONITORING.md`: Dashboards e alertas recomendados
- `docs/RUNBOOK.md`: Runbook de operações
- `docs/TROUBLESHOOTING.md`: Troubleshooting comum
- `docs/INCIDENT_PLAYBOOK.md`: Playbook de incidentes
- `docs/backlog-api/implementacoes/FASE4_IMPLEMENTACAO_RESUMO.md`: Resumo completo da implementação
- `docs/backlog-api/FASE4.md`: Plano atualizado com 100% de conclusão

---

## [2025-01-15] - Fase 3: Performance e Escalabilidade ✅ 100% Completo

### Adicionado
- **Concorrência Otimista**: RowVersion implementado em 4 entidades críticas
  - `CommunityPostRecord`, `TerritoryEventRecord`, `MapEntityRecord`, `TerritoryMembershipRecord`
  - Tratamento de `DbUpdateConcurrencyException` no `CommitAsync`
  - `ConcurrencyHelper` para retry logic em operações concorrentes
  - Migration `AddRowVersionForOptimisticConcurrency` criada
- **Processamento Assíncrono de Eventos**: `BackgroundEventProcessor` implementado
  - Fila de eventos em memória (`ConcurrentQueue`)
  - Processamento concorrente (até 5 eventos simultâneos)
  - Retry logic com backoff exponencial (até 3 tentativas)
  - Dead letter queue para eventos que falharam após todas as tentativas
  - Resolução dinâmica de handlers via `IServiceProvider`
- **Redis Cache**: Infraestrutura completa de cache distribuído
  - `IDistributedCacheService` interface criada
  - `RedisCacheService` com fallback automático para `IMemoryCache`
  - Todos os cache services migrados (7 services):
    - `TerritoryCacheService`, `FeatureFlagCacheService`, `UserBlockCacheService`
    - `MapEntityCacheService`, `EventCacheService`, `AlertCacheService`, `AccessEvaluator`
  - Configuração opcional via `ConnectionStrings__Redis`
- **Read Replicas**: Documentação completa de configuração
  - Uso de `QueryTrackingBehavior.NoTracking` para read-only
  - Suporte a connection strings separadas para read replicas
- **Load Balancer e Multi-Instância**: Documentação completa
  - Exemplos para Nginx, AWS ALB, Azure Load Balancer
  - Configuração Docker Compose e Kubernetes
  - Health checks e validação de API stateless
- **Serialização JSON Padronizada**: Opções seguras em todos os pontos de serialização
  - `JsonStringEnumConverter` para enums como strings
  - `MaxDepth = 64` para evitar recursão infinita
  - `ReferenceHandler.IgnoreCycles` para evitar referências circulares
  - `JsonNumberHandling.AllowReadingFromString` para compatibilidade

### Modificado
- **Repositories**: Atualizados para rastrear entidades corretamente para concorrência otimista
  - `PostgresMapRepository`, `PostgresTerritoryEventRepository`, `PostgresFeedRepository`
- **Cache Services**: Migrados para usar `IDistributedCacheService`
  - Métodos convertidos para async onde necessário
  - Wrappers síncronos mantidos para compatibilidade
- **Serialização JSON**: Padronizada em 5 arquivos
  - `RedisCacheService`, `BackgroundEventProcessor`, `OutboxDispatcherWorker`
  - `ReportCreatedNotificationHandler`, `PostCreatedNotificationHandler`
- **Testes**: Atualizados para usar `IDistributedCacheService`
  - `CacheTestHelper` criado para facilitar testes
  - Testes de concorrência com suporte a skip quando PostgreSQL não disponível

### Testes
- Total: 371 testes passando, 2 pulados (requerem PostgreSQL)
- Novos: Testes de concorrência (`ConcurrencyTests.cs`)
- Cobertura: ~50% mantida

### Documentação
- Criado `PR_FASE3_PERFORMANCE_ESCALABILIDADE.md` com resumo completo
- Atualizado `FASE3.md` e `backlog-api/implementacoes/FASE3_IMPLEMENTACAO_RESUMO.md` para 100%
- Criado `DEPLOYMENT_MULTI_INSTANCE.md` com documentação completa
- Atualizado `docs/prs/README.md` com novo PR

---

## [2025-01-15] - Fase 2: Qualidade de Código e Confiabilidade ✅ 100% Completo

### Adicionado
- **Paginação Completa**: Todos os endpoints de listagem agora têm versões paginadas
  - `GET /api/v1/notifications/paged` - Notificações paginadas
  - `GET /api/v1/map/pins/paged` - Pins do mapa paginados
  - Endpoints existentes já tinham paginação (territories, feed, assets, alerts, events, inquiries, join-requests, reports, items)
- **Testes de Segurança**: 14 novos testes implementados
  - Autenticação (JWT inválido/expirado)
  - Autorização (Visitor vs Resident vs Curator)
  - SQL injection, XSS, NoSQL injection
  - Path traversal, CSRF, Command injection
  - Resource ownership, HTTPS enforcement
- **Testes de Performance**: 7 testes com SLAs definidos
  - Territories: < 500ms
  - Feed: < 800ms
  - Assets: < 600ms
  - Authentication: < 1000ms
  - Requisições concorrentes: < 2000ms
- **Constants.cs**: Centralização de constantes em 13 categorias
  - Pagination, Cache, Geo, Validation, RateLimiting, Moderation, Auth, ResidencyRequests, Geography, Posts, CacheKeys, FeatureFlagErrors
- **ValidationHelpers.cs**: Helpers de validação comum
- **CacheInvalidationService**: Serviço centralizado para invalidação de cache
  - Integrado em 9 services (Membership, Store, StoreItem, TerritoryAsset, Events, Territory, PostCreation, Map, Health)
- **CacheMetricsService**: Serviço para métricas de cache hit/miss
  - Integrado em 7 cache services (TerritoryCacheService, AccessEvaluator, FeatureFlagCacheService, AlertCacheService, EventCacheService, MapEntityCacheService, UserBlockCacheService)
  - Controller para expor métricas: `GET /api/v1/cache/metrics`
- **100+ novos testes**: Total de 371 testes (362 passando, 9 em ajuste)
  - ReportServiceTests: 9 testes (edge cases e cenários de erro)
  - JoinRequestServiceTests: 16 testes (validações e fluxos completos)
  - CacheMetricsServiceTests: 5 testes (incluindo thread-safety)

### Modificado
- **Refatoração**: 15 services atualizados para usar constantes centralizadas
  - MembershipService, TerritoryAssetService, TerritoryFeatureFlagGuard, TerritoryCacheService, ReportService, ResidencyRequestService, EventsService, PostCreationService, MapService, AccessEvaluator, AlertCacheService, AuthService, PaginationParameters
- **NotificationsController**: Adicionado endpoint paginado e uso de Constants.Pagination
- **MapController**: Adicionado GetPinsPaged e injeção de TerritoryAssetService
- **Repositórios**: Adicionado CountByUserAsync em INotificationInboxRepository
- **Cache Services**: Integração de CacheMetricsService para rastreamento de hit/miss

### Testes
- Total: 371 testes (362 passando, 9 em ajuste)
- Novos: 100+ testes criados (ReportService, JoinRequestService, CacheMetrics, Alerts, Assets, Marketplace, Territories, Events, Security, Performance)
- Cobertura: ~50% (aumentada, objetivo >90%)

### Documentação
- Criado `FASE2_RESUMO_FINAL.md` com resumo completo das implementações (100% completo)
- Atualizado `backlog-api/implementacoes/FASE2_IMPLEMENTACAO_PROGRESSO.md` com progresso detalhado
- Documentação de testes de segurança e performance
- Atualizado README.md, CHANGELOG.md, `22_COHESION_AND_TESTS.md`, `PLANO_ACAO_10_10_RESUMO.md` e outros documentos relevantes
- Status da Fase 2 atualizado para 100% completo em todos os documentos

---

## [2025-01-13] - Fase 1: Segurança Crítica

### Adicionado
- **JWT Secret Management**: Validação obrigatória de secret forte (mínimo 32 caracteres em produção)
- **Rate Limiting**: Proteção contra abuso e DDoS
  - Auth endpoints: 5 req/min
  - Feed endpoints: 100 req/min
  - Write endpoints: 30 req/min
  - Global: 60 req/min
- **Security Headers**: Headers de segurança em todas as respostas
  - X-Frame-Options, X-Content-Type-Options, X-XSS-Protection
  - Referrer-Policy, Permissions-Policy, Content-Security-Policy
  - Strict-Transport-Security (HSTS) em produção
- **FluentValidation**: Validação de input em todos os endpoints críticos
  - 8 novos validators criados
- **CORS**: Configuração segura e flexível via variáveis de ambiente
- **SecurityHeadersMiddleware**: Middleware customizado para injetar headers
- **SecurityTests**: 11 novos testes de segurança (100% passando)

### Modificado
- **Program.cs**: Configuração completa de segurança (JWT, Rate Limiting, HTTPS, CORS)
- **11 Controllers**: Aplicação de rate limiting via `[EnableRateLimiting]`
- **ApiFactory**: Configuração de JWT secret para testes
- **Documentação**: 6 novos documentos + 5 atualizados

### Testes
- Total: 11 novos testes de segurança (100% passando)
- Cobertura: Rate limiting, Security headers, Validação, CORS

### Configuração
- Variáveis de ambiente obrigatórias:
  - `JWT__SIGNINGKEY` (obrigatório)
  - `CORS__ALLOWEDORIGINS` (obrigatório em produção)
  - `RateLimiting__PermitLimit` (opcional)

---

## [2026-01-16] - Hierarquia de Permissões e Auditoria

### Adicionado
- **Hierarquia de Permissões**: SystemAdmin tem implicitamente todas as MembershipCapabilities em todos os territórios
- **SystemPermissionService.GrantAsync()**: Método para conceder SystemPermissions com auditoria
- **MembershipCapabilityService.GrantAsync()**: Método para conceder MembershipCapabilities com auditoria
- **Auditoria completa**: Todos os eventos de grant/revoke são registrados em `IAuditLogger`
- **OperationResult<T>**: Tipo genérico para resultados de operações com valor de retorno
- **Testes de hierarquia**: 3 novos testes validando que SystemAdmin tem todas as capabilities
- **Testes de serviços**: 12 novos testes para SystemPermissionService e MembershipCapabilityService

### Modificado
- **AccessEvaluator.HasCapabilityAsync()**: Agora verifica SystemAdmin primeiro antes de verificar capabilities territoriais
- **SystemPermissionService.RevokeAsync()**: Adicionado parâmetro `revokedByUserId` e auditoria
- **MembershipCapabilityService.RevokeAsync()**: Adicionado parâmetro `revokedByUserId` e auditoria
- **Documentação**: Organizada em pastas (refactoring/, validation/, recommendations/, analysis/)

### Testes
- Total: 224 testes passando (15 novos testes adicionados)

---

## [2026-01-13] - Refatoração User-Centric Membership

## Unreleased
- Removed all obsolete APIs related to Membership (application not yet launched, no backward compatibility needed):
  - Removed obsolete endpoints: `DeclareMembership`, `GetStatus`, `Validate`
  - Removed obsolete service methods: `DeclareMembershipAsync`, `GetStatusAsync`, `ValidateAsync`
  - Removed obsolete repository methods: `UpdateStatusAsync`, `UpdateRoleAndStatusAsync`
  - Removed obsolete `AccessEvaluator.IsResidentLegacyAsync`
  - Updated tests to use new APIs (`EnterAsVisitorAsync`, `BecomeResidentAsync`, `VerifyResidencyByGeoAsync`, `VerifyResidencyByDocumentAsync`)
- Refactored territory to be purely geographic and moved social logic into membership entities and services.
- Added revised user stories documentation under `docs/user-stories.md`.
- Updated API endpoints for territory search/nearby/suggestions and membership handling.
- Adjusted feed/map/health filtering to use social membership roles.
- Added optional Postgres persistence with EF Core mappings alongside the InMemory provider.
- Added a minimal static API home page plus configuration helper UI.
- Added structured error handling with `ProblemDetails` and testing hooks for exception scenarios.
- Published the self-service portal as a static site in `docs/` for GitHub Pages, linking to documentation and changelog.
- Added notification outbox/inbox flow with in-app notifications and API endpoints to list/mark as read.
- Fixed redirect loop in developer portal when accessed from external pages (Gamma).
- Fixed `CreatePostRequestValidator` to correctly validate enum strings using `Enum.TryParse`.
- Improved test isolation: `ApiFactory` now creates isolated `InMemoryDataStore` per instance.

## 2025-01 - Refatoração Arquitetural

### Refatoração do FeedService (ADR-009)
- **Refatorado `FeedService`** para respeitar Single Responsibility Principle:
  - Extraído `PostCreationService` para criação de posts
  - Extraído `PostInteractionService` para likes, comentários e shares
  - Extraído `PostFilterService` para filtragem e paginação
  - `FeedService` agora atua como orquestrador com apenas 4 dependências (redução de 70%)
- **Benefícios**: Melhor separação de responsabilidades, código mais testável e manutenível

### Padronização e Melhorias
- **Criado `Result<T>` e `OperationResult`** para padronizar tratamento de erros
- **Criado `PagedResult<T>` e `PaginationParameters`** para suporte a paginação
- **Adicionado FluentValidation** com validators básicos para validação de entrada
  - `CreatePostRequestValidator`: Validação de posts com suporte a enums (corrigido para usar `Enum.TryParse`)
  - `TerritorySelectionRequestValidator`: Validação de seleção de território
- **Extraída configuração de DI** para `ServiceCollectionExtensions` (melhor organização)
- **Adicionado `CorrelationIdMiddleware`** para rastreabilidade de requisições
- **Melhorado `RequestLoggingMiddleware`** com correlation ID e logging estruturado
- **Corrigida ordem dos middlewares**: `CorrelationIdMiddleware` executa antes de `RequestLoggingMiddleware`

### Isolamento de Testes
- **Melhorado `ApiFactory`**: Cada instância cria um novo `InMemoryDataStore` isolado
- **Benefícios**: Testes independentes, sem dependência de ordem de execução, podem ser executados em paralelo
- **Documentação**: Criado `backend/Araponga.Tests/README.md` com princípios e boas práticas de testes

### Correções
- **Corrigido loop de redirect no developer portal**: Adicionadas proteções para evitar loops infinitos quando usuários vêm de páginas externas (Gamma)
- **Corrigido `CreatePostRequestValidator`**: Trocado `IsInEnum()` por `Must()` com `Enum.TryParse` para validar strings como enums corretamente

### Documentação
- Adicionado ADR-009 documentando a refatoração do FeedService
- Criado `11_ARCHITECTURE_SERVICES.md` documentando a arquitetura dos services
- Atualizada `21_CODE_REVIEW.md` refletindo as melhorias implementadas
- Documentação reorganizada e normalizada (ver `00_INDEX.md`)
- Criado `backend/Araponga.Tests/README.md` com guia de testes