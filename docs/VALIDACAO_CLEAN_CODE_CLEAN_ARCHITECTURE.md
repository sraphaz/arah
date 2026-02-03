# Validação: Clean Code e Clean Architecture

**Data**: 2026-02-02  
**Objetivo**: Validar a aplicação Araponga contra os princípios e padrões descritos em *Clean Code* (Robert C. Martin) e *Clean Architecture* (Uncle Bob).

---

## 1. Clean Architecture – Regra das Dependências

### 1.1 Direção das dependências

A regra central: **dependências apontam para dentro**. O núcleo (Domain) não depende de nada externo; camadas externas dependem das internas.

| Camada | Projeto(s) | Depende de | Avaliação |
|--------|-------------|------------|-----------|
| **Entidades (núcleo)** | `Araponga.Domain` | Nenhum projeto; apenas `Araponga.Domain.*` | **OK** – Domain não referencia Application, Infrastructure ou API. Sem EF, Npgsql ou ASP.NET. |
| **Casos de uso** | `Araponga.Application` | Domain, Araponga.Shared | **OK** – Application depende apenas de Domain (e Shared, projeto mínimo). Persistência/cache via **interfaces** (I*Repository, IUnitOfWork, IDistributedCacheService). |
| **Adaptadores de interface** | `Araponga.Infrastructure`, módulos | Domain, Application | **OK** – Implementam interfaces definidas em Application; não são referenciados por Domain ou Application. |
| **Frameworks e drivers** | `Araponga.Api` | Application, Infrastructure, módulos | **OK** – API orquestra serviços de aplicação e infraestrutura; controllers finos delegam para Application. |

**Conclusão**: A **Dependency Rule** é respeitada. Domain é independente; Application define interfaces e usa entidades de domínio; Infrastructure e API dependem para dentro.

### 1.2 Inversão de dependências

- **Repositórios**: Interfaces (`ITerritoryRepository`, `IFeedRepository`, etc.) em Application; implementações em Infrastructure ou módulos. Serviços de aplicação dependem das interfaces, não das implementações.
- **Unit of Work**: `IUnitOfWork` em Application; implementação (ArapongaDbContext em Postgres, InMemoryUnitOfWork em testes) na camada externa.
- **Cache**: `IDistributedCacheService` em Application; implementação em Infrastructure.
- **E-mail / eventos**: `IEmailSender`, `IEventBus` em Application; implementações em Infrastructure.

**Conclusão**: Persistência, cache e integrações externas são abstraídos por interfaces na camada de aplicação; a inversão de dependências está aplicada.

### 1.3 Independência de frameworks e de UI

- **Domain**: Sem referências a EF Core, ASP.NET ou qualquer framework. Entidades puras (Territory, User, etc.) com validação de invariantes no construtor.
- **Application**: Usa apenas `Microsoft.Extensions.Logging` e `Microsoft.Extensions.Configuration` (abstrações estáveis). Não referencia Infrastructure nem API.
- **API**: Controllers dependem de Application (serviços) e de contratos (DTOs); a lógica de negócio fica em Application.

**Conclusão**: O núcleo (Domain + Application) permanece independente de framework de persistência e de UI; a API e a Infrastructure são detalhes plugáveis.

---

## 2. Clean Architecture – Estrutura das camadas

### 2.1 Domain (entidades e regras de negócio)

- **Entidades ricas**: Territory, User, MapEntity, etc. com construtores que validam invariantes (`ArgumentException` para dados inválidos).
- **Sem referências externas**: Apenas namespaces `Araponga.Domain.*`.
- **Validação no construtor**: Ex.: Territory exige name, city, state não vazios; StoreRating exige rating entre 1 e 5.

**Avaliação**: **OK** – Domain concentra regras de negócio e permanece isolado.

### 2.2 Application (casos de uso)

- **Serviços por agregado/use case**: TerritoryService, AuthService, FeedService, etc. Cada serviço orquestra repositórios e publica eventos quando aplicável.
- **Interfaces em Application**: Repositórios, IUnitOfWork, IEmailSender, IEventBus, etc. definidos na camada de aplicação.
- **Modelos de aplicação**: DTOs e resultados (TerritoryCreationResult, PagedResult, etc.) em Application.Models ou Common.
- **Exceções de aplicação**: ValidationException, NotFoundException, ForbiddenException (Application.Exceptions) usadas pelos casos de uso e mapeadas para HTTP na API.

**Avaliação**: **OK** – Casos de uso bem delimitados; dependências invertidas via interfaces.

### 2.3 Infrastructure (adaptadores)

- **Implementações concretas**: Repositórios Postgres/InMemory, DbContext, EmailSender, EventBus, FileStorage.
- **Referências**: Domain e Application; nenhuma referência da Application para Infrastructure (exceto em tempo de composição, na API).

**Avaliação**: **OK** – Infrastructure atua como adaptador entre Application e tecnologias externas.

### 2.4 API (interface HTTP)

- **Controllers finos**: Injetam serviços de aplicação (TerritoryService, etc.); convertem request em chamada de serviço e resultado em DTO/ActionResult.
- **Tratamento de erros centralizado**: Exception handler global mapeia exceções de aplicação para ProblemDetails e códigos HTTP (400, 401, 403, 404, 409, 500).
- **Contratos (DTOs)**: Request/Response em Api.Contracts; controllers não expõem entidades de domínio diretamente.

**Avaliação**: **OK** – API é camada de apresentação que delega para Application.

---

## 3. Clean Code – Princípios aplicados

### 3.1 Nomes significativos

- **Serviços**: TerritoryService, JoinRequestService, CacheInvalidationService – nomes que indicam responsabilidade.
- **Métodos**: ListAvailableAsync, GetByIdAsync, CreateAsync, InvalidateTerritoryCache – verbos claros e consistentes.
- **Resultados**: TerritoryCreationResult, PagedResult&lt;T&gt;, JoinRequestDecisionResult – refletem o retorno.

**Avaliação**: **OK** – Nomenclatura consistente e legível.

### 3.2 Funções pequenas e responsabilidade única

- **Serviços de aplicação**: Métodos em geral focados (ex.: CreateAsync valida, cria entidade, persiste, invalida cache, retorna resultado). Listagens paginadas usam **PaginationHelper.ToPagedResult** (Application.Common), reduzindo duplicação Skip/Take + PagedResult em TerritoryService, JoinRequestService, TerritoryAssetService, ReportService, PostFilterService e InquiryService.
- **CacheInvalidationService**: Múltiplos métodos pequenos por tipo de cache (InvalidateTerritoryCache, InvalidateFeedCache, etc.) – boa granularidade.
- **Controllers**: Actions delegam para um ou poucos serviços e mapeiam resposta – responsabilidade única.

**Avaliação**: **OK** – Funções pequenas e com responsabilidade clara; helper de paginação aplicado onde havia repetição.

### 3.3 DRY (Don't Repeat Yourself)

- **PagedResult&lt;T&gt;, PaginationParameters**: Reutilizados em vários serviços e controllers.
- **Interfaces de repositório**: Uma interface por agregado; implementações únicas (Postgres ou InMemory por contexto).
- **Exception handler**: Um único pipeline para mapear exceções em respostas HTTP.
- **Duplicação de entidades**: Mitigada pelo teste DuplicateEntityStandardizationTests (ver docs/VALIDACAO_INFRAESTRUTURA_INTEGRIDADE.md §6).

**Avaliação**: **OK** – Abstrações comuns reutilizadas; duplicação de entidades controlada por teste de padronização.

### 3.4 Tratamento de erros e exceções

- **Domain**: ArgumentException / ArgumentOutOfRangeException para invariantes violadas no construtor.
- **Application**: NotFoundException, ValidationException, ForbiddenException, UnauthorizedException, ConflictException (Application.Exceptions) para falhas de regra de negócio ou recurso inexistente.
- **API**: Exception handler converte essas exceções em ProblemDetails com status HTTP apropriado; em Development/Testing inclui detalhes (stack trace, tipo da exceção).

**Avaliação**: **OK** – Erros tratados de forma consistente; camada de apresentação não expõe detalhes internos em produção.

### 3.5 Comentários e documentação

- **XML summary** em interfaces (ex.: IUnitOfWork), serviços e entidades quando necessário.
- **Comentários inline** onde a intenção não é óbvia (ex.: CacheInvalidationService sobre limitações do IMemoryCache com wildcards).
- **OpenAPI/Swagger**: Endpoints documentados com ProducesResponseType e remarks onde aplicável.

**Avaliação**: **OK** – Código autoexplicativo onde possível; documentação onde agrega valor.

### 3.6 Testabilidade

- **Domain**: Entidades puras; testáveis sem infraestrutura.
- **Application**: Serviços dependem de interfaces; testes unitários com mocks (IUnitOfWork, I*Repository) e InMemory implementations.
- **API**: Testes de integração com WebApplicationFactory; controllers testados indiretamente via HTTP.

**Avaliação**: **OK** – Inversão de dependências e uso de interfaces permitem testes unitários e de integração.

---

## 4. Resumo da validação

| Critério | Status | Observação |
|----------|--------|------------|
| Dependency Rule (Clean Architecture) | OK | Domain sem deps externas; Application define interfaces; Infrastructure/API dependem para dentro. |
| Inversão de dependências | OK | Repositórios, UoW, cache, e-mail, eventos via interfaces em Application. |
| Independência de frameworks | OK | Domain e Application livres de EF/ASP.NET; detalhes na borda. |
| Domain rico e isolado | OK | Entidades com validação no construtor; sem referências a camadas externas. |
| Casos de uso em Application | OK | Serviços por agregado; uso consistente de interfaces. |
| API como camada fina | OK | Controllers delegam para Application; DTOs e exception handler centralizado. |
| Nomes significativos (Clean Code) | OK | Serviços, métodos e resultados com nomes claros. |
| Funções pequenas / SRP | OK | Métodos focados; PaginationHelper.ToPagedResult aplicado nos serviços de listagem paginada. |
| DRY | OK | PagedResult, PaginationParameters, PaginationHelper, interfaces e exception handler reutilizados. |
| Tratamento de erros | OK | Exceções de domínio/aplicação mapeadas para HTTP de forma consistente. |
| Testabilidade | OK | Interfaces e mocks permitem testes unitários e de integração. |

---

## 5. Recomendações (melhorias opcionais)

1. **Helpers de paginação** – **Aplicada**: Criado `PaginationHelper.ToPagedResult<T>(list, pagination)` em Application.Common; refatorados TerritoryService, JoinRequestService, TerritoryAssetService, ReportService, PostFilterService e InquiryService para usá-lo.
2. **Application e ILogger**: Application usa `Microsoft.Extensions.Logging.ILogger` diretamente. É uma dependência de abstração estável; alternativa seria definir `IAppLogger` em Application e implementar na Infrastructure, para total independência de pacotes Microsoft na camada de aplicação – opcional e de baixo retorno imediato.
3. **Exceções no Domain**: Alguns colocam exceções de domínio (ex.: TerritoryNotFound) no Domain; hoje NotFoundException está em Application. Manter em Application é aceitável (exceção “de caso de uso”); mover para Domain seria uma escolha de estilo para maior riqueza do núcleo.

---

**Última atualização**: 2026-02-02
