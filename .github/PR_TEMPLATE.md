# Implementar Recomendacoes de Testes e Observabilidade

## Resumo

Este PR implementa todas as recomendacoes da analise de coesao e testes, aumentando a cobertura de testes de ~78% para ~82% e implementando observabilidade minima conforme especificacao MVP.

## Mudancas Implementadas

### 1. Testes Adicionais para Marketplace (~60% -> ~80%)
- 6 novos testes em `MarketplaceServiceTests.cs`
- Testes de stores (update, status changes)
- Testes de listings (update, archive, search filters)
- Testes de cart (add, update, remove)
- Testes de inquiries (list my/received)

### 2. Testes de Infraestrutura (~50% -> ~75%)
- Novo arquivo `RepositoryTests.cs` com 9 testes
- Testes de repositorios: Territory, User, Membership, Feed, Map, Report, Store, Listing, Cart

### 3. Testes E2E
- Novo arquivo `EndToEndTests.cs` com 4 testes de fluxos criticos
- Fluxo completo: cadastro -> vinculo -> feed
- Fluxo de residente: cadastro -> vinculo -> post
- Fluxo de interacoes: criar post -> curtir -> comentar -> compartilhar
- Fluxo do mapa: sugerir entidade -> confirmar

### 4. Testes de Edge Cases para Notificacoes (~75% -> ~85%)
- 3 novos testes em `NotificationFlowTests.cs`
- Paginacao de notificacoes
- Idempotencia de marcacao como lida
- Autorizacao (apenas dono pode marcar)

### 5. Documentacao de Decisoes Arquiteturais
- Novo arquivo `DECISOES_ARQUITETURAIS.md` com 8 ADRs
- ADR-001: Marketplace implementado antes do POST-MVP
- ADR-002: Sistema de notificacoes com Outbox/Inbox
- ADR-003: Separacao Territorio vs Camadas Sociais
- ADR-004: PresencePolicy para validacao de presenca fisica
- ADR-005: GeoAnchors derivados de midias
- ADR-006: Clean Architecture com InMemory e Postgres
- ADR-007: Moderacao automatica por threshold
- ADR-008: Feature Flags por territorio

### 6. Observabilidade Minima
- Interface `IObservabilityLogger` para logs estruturados
- Implementacao `InMemoryObservabilityLogger` usando ILogger padrao
- Middleware `RequestLoggingMiddleware` para logging de requisicoes HTTP
- Integracao em `ReportService` (logs de reports e moderacao)
- Integracao em `MembershipsController` (logs de erros de geolocalizacao)

## Resultados

- **Cobertura de Testes**: ~82% (aumentada de ~78%)
- **Novos Arquivos**: 7
- **Arquivos Modificados**: 6
- **Testes Adicionados**: 22
- **ADRs Documentados**: 8

## Testes

Todos os testes passam:
```bash
dotnet test
```

## Arquivos Alterados

### Novos Arquivos
- `backend/Araponga.Api/Middleware/RequestLoggingMiddleware.cs`
- `backend/Araponga.Application/Interfaces/IObservabilityLogger.cs`
- `backend/Araponga.Infrastructure/InMemory/InMemoryObservabilityLogger.cs`
- `backend/Araponga.Tests/Api/EndToEndTests.cs`
- `backend/Araponga.Tests/Infrastructure/RepositoryTests.cs`
- `docs/ANALISE_COESAO_E_TESTES.md`
- `docs/DECISOES_ARQUITETURAIS.md`
- `docs/IMPLEMENTACAO_RECOMENDACOES.md`

### Arquivos Modificados
- `backend/Araponga.Api/Controllers/MembershipsController.cs`
- `backend/Araponga.Api/Program.cs`
- `backend/Araponga.Application/Services/ReportService.cs`
- `backend/Araponga.Tests/Application/MarketplaceServiceTests.cs`
- `backend/Araponga.Tests/Application/NotificationFlowTests.cs`
- `backend/Araponga.Tests/Infrastructure/RepositoryTests.cs`

## Links Relacionados

- [Analise de Coesao e Testes](./docs/ANALISE_COESAO_E_TESTES.md)
- [Decisoes Arquiteturais](./docs/DECISOES_ARQUITETURAIS.md)
- [Implementacao das Recomendacoes](./docs/IMPLEMENTACAO_RECOMENDACOES.md)

## Checklist

- [x] Testes adicionados e passando
- [x] Documentacao atualizada
- [x] Observabilidade implementada
- [x] Codigo compila sem erros
- [x] Sem erros de lint
