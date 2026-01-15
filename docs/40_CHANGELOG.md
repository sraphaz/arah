# Changelog

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