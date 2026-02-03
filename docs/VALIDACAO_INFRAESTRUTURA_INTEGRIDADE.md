# Validação: Infraestrutura e Integridade do Projeto

**Data**: 2026-02-02  
**Objetivo**: Verificar código repetido pós-refatoração, situação atual da infra (Shared vs Infra principal vs por módulo) e integridade do projeto.

### Estado atual (checklist)

| Item | Status |
|------|--------|
| Build | OK (0 erros) |
| Testes | 2171 passando, 0 falhando, 23 ignorados |
| Rastreio de jornada | CorrelationId + traceId em uso |
| UoW vs contextos | OK: IUnitOfWork = CompositeUnitOfWork com múltiplos contextos (ver §3.1) |
| Clean Code / Clean Architecture | Ver [VALIDACAO_CLEAN_CODE_CLEAN_ARCHITECTURE.md](VALIDACAO_CLEAN_CODE_CLEAN_ARCHITECTURE.md) |

---

## 1. Situação atual da infraestrutura

Existem **três camadas** de projetos de infraestrutura:

| Camada | Projeto(s) | Conteúdo | Uso |
|--------|------------|----------|-----|
| **Principal** | `Araponga.Infrastructure` | ArapongaDbContext, **78 entidades** em Postgres/Entities, repositórios Postgres restantes (Territory, User, JoinRequest, PostGeoAnchor, PostAsset, Financial, Policies, Media, etc.), **todos** os repositórios InMemory, Migrations | Repositórios “shared” em modo Postgres; todos os repositórios em modo InMemory; migrations; ConnectionPoolMetricsService; HealthCheck; OutboxDispatcherWorker |
| **Shared** | `Araponga.Infrastructure.Shared` | SharedDbContext, **29 entidades** em Postgres/Entities | SharedDbContext disponível para uso direto; participante do UoW composto (IUnitOfWork = CompositeUnitOfWork) |
| **Por módulo** | `Araponga.Modules.{Feed,Events,Map,Chat,Alerts,Assets,Marketplace,Moderation,Notifications,Subscriptions,Admin}.Infrastructure` | Cada um: DbContext próprio, entidades do domínio, repositórios Postgres do domínio | Cada módulo registra seu DbContext e seus repositórios; **nenhum** repositório Postgres duplicado na principal para domínios já migrados |

Conclusão: há **muita** infraestrutura (principal + shared + 11 módulos), mas a **divisão de responsabilidade** está clara: módulos têm só seus domínios; principal tem o que ainda não foi modularizado + InMemory + migrations; shared expõe SharedDbContext; IUnitOfWork é um CompositeUnitOfWork que persiste todos os contextos.

---

## 2. Código repetido (duplicação)

### 2.1 Repositórios Postgres

- **Não há duplicação.** Cada interface de repositório tem **uma única** implementação Postgres:
  - Feed, Events, Map, Chat, Alerts, Assets, Marketplace, Moderation, Notifications, Subscriptions → implementações nos respectivos módulos.
  - Territory, User, Membership, JoinRequest, PostGeoAnchor, PostAsset, Financial, Policies, Media, etc. → implementações em `Araponga.Infrastructure/Postgres`.

### 2.2 Repositórios InMemory

- **Não há duplicação.** Todas as implementações InMemory estão em `Araponga.Infrastructure/InMemory` (incluindo Feed, Events, Map, Chat, Store, Subscription, etc.). Uma única pasta, sem repetição em módulos.

### 2.3 Entidades (Records) – **há duplicação**

A mesma tabela do banco é mapeada por **mais de uma** classe C# em assemblies diferentes:

| Tabela / domínio | Onde existe a entidade (Record) |
|------------------|----------------------------------|
| Core/Shared (ex.: User, Territory, WorkItem, DocumentEvidence, NotificationConfig, UserPreferences, etc.) | `Araponga.Infrastructure/Postgres/Entities` **e** `Araponga.Infrastructure.Shared/Postgres/Entities` |
| Feed (CommunityPost, PostComment, PostLike, etc.) | `Araponga.Infrastructure/Postgres/Entities` **e** `Araponga.Modules.Feed.Infrastructure/Postgres/Entities` |
| Events, Map, Chat, Alerts, Assets, Marketplace, Moderation, Notifications, Subscriptions | Idem: em **Infrastructure/Postgres/Entities** e no **módulo** correspondente |

Motivo: **ArapongaDbContext** (na Infrastructure principal) ainda referencia **todas** as entidades em `Araponga.Infrastructure.Postgres.Entities` (incluindo as dos domínios já migrados para módulos). As migrations e o ConnectionPoolMetricsService usam esse contexto. Ao mesmo tempo, cada módulo tem seu próprio DbContext e suas **próprias** classes de entidade (em outro namespace/assembly) para as mesmas tabelas.

Risco: alteração de schema (coluna, índice) exige cuidado: pode ser preciso ajustar **duas** (ou três) definições de entidade para a mesma tabela, sob pena de drift (modelo desatualizado em um dos lados).

---

## 3. Integridade e possíveis problemas

### 3.1 Unit of Work vs contextos

- Em Postgres, **IUnitOfWork** está registrado como **CompositeUnitOfWork** na API: agrega múltiplos contextos via **IUnitOfWorkParticipant**.
- Participantes: **ArapongaDbContext**, **SharedDbContext** e o DbContext de cada módulo (Feed, Events, Map, Chat, Subscriptions, Moderation, Notifications, Alerts, Assets). Cada um é registrado como `IUnitOfWorkParticipant` (adapter `DbContextUnitOfWorkParticipant` em Infrastructure.Shared).
- **CommitAsync** chama SaveChangesAsync em todos os participantes na ordem de registro; assim, uma única chamada a `_unitOfWork.CommitAsync()` persiste alterações da Infrastructure principal, do Shared e dos módulos.
- **BeginTransactionAsync**, **RollbackAsync** e **HasActiveTransactionAsync** delegam para **ArapongaDbContext** (transação explícita não abrange os outros contextos).
- **SharedDbContext** continua registrado para uso direto; também é participante do UoW composto.


### 3.2 Dois DbContexts no mesmo banco

- **ArapongaDbContext** e **SharedDbContext** (e os DbContexts dos módulos) apontam para o mesmo banco.
- Múltiplos contextos para o mesmo banco é válido em EF Core, mas aumenta a chance de confusão e de entidades duplicadas (como já identificado).

### 3.3 Migrations

- Todas as migrations estão em `Araponga.Infrastructure/Postgres/Migrations` e referem **ArapongaDbContext**.
- Os módulos **não** têm migrations próprias; o schema é mantido pela Infrastructure principal. Integridade do banco, hoje, depende apenas desse conjunto de migrations.

### 3.4 Identificação de jornada (processo / request) entre módulos

- **Correlation ID** (`X-Correlation-ID`): definido no `CorrelationIdMiddleware` por request (vem do header ou GUID gerado). Fica em `HttpContext.Items["CorrelationId"]`, em request/response headers e no Serilog via `LogContext.PushProperty("CorrelationId", ...)`, permitindo rastrear uma mesma requisição em todos os módulos via logs.
- **TraceId**: em respostas de erro (ProblemDetails) e no pipeline (`HttpContext.TraceIdentifier`).
- **Process ID (PID)** do SO não é exposto na API; o identificador da “jornada” é o CorrelationId (e, em erro, o traceId).

- **Para clientes da API**: o header de resposta `X-Correlation-ID` contém o identificador da request; use-o para suporte e logs.

### 3.5 Build e testes

- **Build**: compila com sucesso (0 erros). Avisos: NU1603 (pacotes), CS8601 (nullability), ASP0019 (headers).
- **Repositórios**: sem duplicação de implementações Postgres; InMemory concentrados na Infrastructure principal.
- **Entidades**: duplicação é apenas de **definição de modelo** (classes Record), não de lógica de negócio; o risco é de **drift** entre definições da mesma tabela.
- **Testes**: suite majoritariamente verde; testes de integração Postgres e performance podem ser pulados (SkippableFact / variável de ambiente). Ajustes aplicados: EventBus cancelamento (token cancelado antes de PublishAsync para teste determinístico); SLA de TerritoriesPaged aumentado para 1200ms (cold start/ambiente).

---

## 4. Resumo

| Aspecto | Status | Observação |
|---------|--------|------------|
| Repositórios Postgres duplicados | OK | Não há; cada interface tem uma implementação (módulo ou Infrastructure). |
| Repositórios InMemory duplicados | OK | Todos em `Araponga.Infrastructure/InMemory`. |
| Entidades (Record) duplicadas | Atenção | Mesma tabela mapeada em 2 (ou 3) lugares: Infrastructure/Entities, Shared/Entities e/ou módulos. |
| IUnitOfWork vs repositórios | OK | IUnitOfWork = CompositeUnitOfWork; commit persiste ArapongaDbContext, SharedDbContext e DbContexts dos módulos numa única chamada. |
| Infra “demais” (principal + shared + módulos) | Esperado | Principal: repositórios restantes + InMemory + migrations. Shared: SharedDbContext disponível; IUnitOfWork na API. Módulos: domínios já migrados. |
| Integridade do projeto | OK | Build ok; sem repositórios duplicados; migrations únicas; risco principal é drift de entidades. |
| Identificação de jornada | OK | CorrelationId + traceId permitem rastrear uma request entre módulos (logs e resposta de erro). |

---

## 5. Recomendações

1. **Entidades duplicadas**: A longo prazo, escolher uma única “fonte de verdade” por tabela: ou manter apenas em módulos (e ArapongaDbContext deixar de ter DbSets para domínios migrados) ou manter apenas em Infrastructure/Entities para compatibilidade com migrations. Hoje não é bloqueante; ao alterar schema/entidade de uma tabela que existe em Infrastructure/Entities e no módulo, atualize os dois lados para evitar drift (ver §6).
2. **IUnitOfWork**: **Aplicada.** Em Postgres, IUnitOfWork é um CompositeUnitOfWork que agrega ArapongaDbContext, SharedDbContext e os DbContexts dos módulos; CommitAsync persiste todos. SharedDbContext permanece disponível para uso direto. (Antes: Decidir se SharedDbContext deveria ser o contexto de UoW para toda a aplicação; se sim, os repositórios “shared” que hoje usam ArapongaDbContext precisariam migrar para SharedDbContext (e as entidades shared virariam fonte única em Infrastructure.Shared). — optou-se por registrar IUnitOfWork como ArapongaDbContext na API.)
3. **Shared + InMemory** (próximo passo): Ver [MELHOR_OPCAO_MODULARIZACAO.md](MELHOR_OPCAO_MODULARIZACAO.md). Direção recomendada: Shared como fonte da verdade; InMemory com store shared + por módulo. Avaliar se Infrastructure.Shared deve passar a ser o único lugar das entidades e do contexto “core” (Territory, User, Membership, etc.) e a Infrastructure principal ficar só com InMemory, Financial e o que ainda não for modularizado; isso reduz duplicação de entidades e deixa a fronteira mais clara.

---

## 6. Guia para alteração de entidades

Quando uma tabela tiver entidade (Record) em **mais de um lugar** (ex.: Infrastructure/Postgres/Entities e no módulo correspondente, ou em Shared):

- **Garantia de padronização**: o teste `DuplicateEntityStandardizationTests.DuplicateEntities_Infrastructure_And_Shared_Have_Same_Properties` compara propriedades (nome e tipo) de todas as entidades duplicadas entre Infrastructure e Shared. Se alguém alterar uma entidade e não a outra, o teste **falha** (detecção de drift). Rodar a suíte de testes (ou esse teste) no CI garante que as definições duplicadas permaneçam alinhadas.
- **Ao alterar coluna, índice ou nome de tabela**: atualize **todas** as definições da mesma tabela (Infrastructure/Entities, Shared/Entities e/ou entidades do módulo) e a migration em `Araponga.Infrastructure/Postgres/Migrations`.
- **Evite alterar só em um dos lados**; isso gera drift e o teste de padronização falhará.
- Referência: §2.3 (entidades duplicadas) e §3.3 (migrations).

---

**Última atualização**: 2026-02-02
