# Changelog

## Unreleased
- Refactored territory to be purely geographic and moved social logic into membership entities and services.
- Added revised user stories documentation under `docs/user-stories.md`.
- Updated API endpoints for territory search/nearby/suggestions and membership handling.
- Adjusted feed/map/health filtering to use social membership roles.
- Added optional Postgres persistence with EF Core mappings alongside the InMemory provider.
- Added a minimal static API home page plus configuration helper UI.
- Added structured error handling with `ProblemDetails` and testing hooks for exception scenarios.
- Published the self-service portal as a static site in `docs/` for GitHub Pages, linking to documentation and changelog.
- Added notification outbox/inbox flow with in-app notifications and API endpoints to list/mark as read.

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
- **Extraída configuração de DI** para `ServiceCollectionExtensions` (melhor organização)
- **Adicionado `CorrelationIdMiddleware`** para rastreabilidade de requisições
- **Melhorado `RequestLoggingMiddleware`** com correlation ID e logging estruturado

### Documentação
- Adicionado ADR-009 documentando a refatoração do FeedService
- Criado `11_ARCHITECTURE_SERVICES.md` documentando a arquitetura dos services
- Atualizada `21_CODE_REVIEW.md` refletindo as melhorias implementadas
- Documentação reorganizada e normalizada (ver `00_INDEX.md`)