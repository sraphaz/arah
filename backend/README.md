# Backend Araponga

Estrutura do backend após modularização e organização de pastas.

## Estrutura de pastas

```
backend/
├── Core/                    # Núcleo compartilhado e orquestração
│   ├── Araponga.Api/        # API HTTP principal
│   ├── Araponga.Application/           # Serviços transversais, orquestração
│   ├── Araponga.Application.Abstractions/  # IModule, IUnitOfWorkParticipant (evita ciclo)
│   ├── Araponga.Domain/     # Domínio compartilhado (Users, Territories, etc.)
│   ├── Araponga.Infrastructure/        # Infraestrutura core (Postgres, Redis, etc.)
│   ├── Araponga.Infrastructure.Shared/ # DbContexts e repositórios compartilhados
│   └── Araponga.Shared/     # Utilitários e tipos compartilhados
├── Modules/                 # Módulos de negócio (um projeto por módulo)
│   ├── Admin/
│   ├── Alerts/
│   ├── Assets/
│   ├── Chat/
│   ├── Connections/         # Domain + Application + Infrastructure (ConnectionsDbContext)
│   ├── Events/
│   ├── Feed/
│   ├── Map/
│   ├── Marketplace/
│   ├── Moderation/
│   ├── Notifications/
│   └── Subscriptions/
├── Tests/                   # Todos os projetos de teste
│   ├── Araponga.Tests/              # Testes de integração e core
│   ├── Araponga.Tests.Shared/       # Helpers compartilhados
│   ├── Araponga.Tests.Modules.Connections/
│   ├── Araponga.Tests.Modules.Map/
│   ├── Araponga.Tests.Modules.Marketplace/
│   ├── Araponga.Tests.Modules.Moderation/
│   └── Araponga.Tests.Modules.Subscriptions/
└── Araponga.Api.Bff/        # BFF (Backend for Frontend) na raiz do backend
```

## Regras de dependência (evitar referências circulares)

1. **Core.Application** referencia todos os módulos (camada Application) para contratos; não referencia *Infrastructure* dos módulos.
2. **Core.Infrastructure** referencia Core + módulos necessários (Application/Domain); não referencia *Infrastructure* dos módulos que referenciam Application (evita ciclo).
3. **Módulos** (um projeto cada, com Domain/Application/Infrastructure) referenciam **Araponga.Application.Abstractions** (IModule, IUnitOfWorkParticipant). **Connections** e **Feed** têm sua própria Infrastructure (DbContext e repositórios Postgres no módulo); Feed registra também IPostGeoAnchorRepository e IPostAssetRepository.
4. **Módulos não referenciam outros módulos** em princípio. Exceção documentada: **Feed** referencia **Map** (tipos de geo para itens do feed); ver ADR-014 e [Modules/README.md](Modules/README.md).

## Referências circulares — verificação

- Não há ciclo: Core.Application → Modules (Application); Modules.Infrastructure → Core.Application.Abstractions ou Core.Infrastructure.Shared (não Core.Application). O projeto **Araponga.Application.Abstractions** quebra o ciclo entre Application e módulos que implementam IModule.
- **Feed → Map**: única dependência entre módulos; é explícita e documentada.

## Logging

- **ILogger** (Microsoft.Extensions.Logging / Serilog): uso geral para logs operacionais, debug e rastreamento. Controllers, serviços e workers usam ILogger.
- **IObservabilityLogger**: interface de aplicação para eventos de negócio e métricas que devem ser tratados de forma uniforme (agregação, alertas). Usado em RequestLoggingMiddleware e ReportService; use para eventos como “report criado”, “falha de moderação”, “erro de geolocalização” e métricas de requisição HTTP.

## Build e testes

```bash
# Na raiz do repositório
dotnet build Araponga.sln
dotnet test Araponga.sln

# Testes de um módulo
dotnet test backend/Tests/Araponga.Tests.Modules.Marketplace/Araponga.Tests.Modules.Marketplace.csproj
dotnet test backend/Tests/Araponga.Tests.Modules.Map/Araponga.Tests.Modules.Map.csproj
```

## Documentação

- [Módulos](Modules/README.md) — estrutura por módulo e lista de módulos.
- [Decisões arquiteturais (ADRs)](../docs/10_ARCHITECTURE_DECISIONS.md) — ADR-012, ADR-014, ADR-015, ADR-016.
