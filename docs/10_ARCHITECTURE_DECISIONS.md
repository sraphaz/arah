# Decisões Arquiteturais

Este documento registra decisões arquiteturais importantes tomadas durante o desenvolvimento do projeto Araponga.

## ADR-001: Marketplace Implementado Antes do POST-MVP

**Status**: Aceito  
**Data**: 2024  
**Contexto**: A especificação original marcava Marketplace como funcionalidade POST-MVP, mas durante o desenvolvimento identificamos que:

1. **Necessidade de validação**: A funcionalidade de marketplace é crítica para validar o modelo de negócio territorial
2. **Dependências já implementadas**: As dependências necessárias (stores, listings, inquiries) já estavam sendo desenvolvidas para outros casos de uso
3. **Feedback de usuários**: Stakeholders indicaram que marketplace seria essencial para o MVP

**Decisão**: Implementar Marketplace completo no MVP, incluindo:
- Gestão de lojas (stores)
- Listagens de produtos e serviços (listings)
- Sistema de inquiries
- Carrinho e checkout
- Taxas de plataforma

**Consequências**:
- ✅ Funcionalidade completa disponível no MVP
- ✅ Maior complexidade no MVP inicial
- ✅ Necessidade de testes abrangentes
- ⚠️ Possível necessidade de ajustes baseados em feedback real

**Alternativas Consideradas**:
1. Implementar apenas inquiries (rejeitado - muito limitado)
2. Implementar apenas stores sem listings (rejeitado - não valida modelo completo)
3. Adiar para POST-MVP (rejeitado - perde oportunidade de validação)

---

## ADR-002: Sistema de Notificações com Outbox/Inbox

**Status**: Aceito  
**Data**: 2024  
**Contexto**: Necessidade de garantir entrega confiável de notificações in-app mesmo em caso de falhas.

**Decisão**: Implementar padrão Outbox/Inbox para notificações:
- Eventos de domínio geram mensagens no Outbox
- Worker processa Outbox e cria notificações no Inbox
- Inbox é consultado pela API de notificações

**Consequências**:
- ✅ Garantia de entrega (at-least-once)
- ✅ Resiliência a falhas
- ✅ Possibilidade de reprocessamento
- ⚠️ Complexidade adicional
- ⚠️ Necessidade de worker processando Outbox

**Alternativas Consideradas**:
1. Notificações síncronas diretas (rejeitado - sem garantia de entrega)
2. Fila externa (rejeitado - adiciona dependência externa no MVP)

---

## ADR-003: Separação Território vs Camadas Sociais

**Status**: Aceito  
**Data**: 2024  
**Contexto**: Princípio fundamental do projeto - território é geográfico e neutro.

**Decisão**: Território contém apenas dados geográficos. Toda lógica social (memberships, visibilidade, moderação) fica em camadas separadas que referenciam o território.

**Consequências**:
- ✅ Território pode existir sem usuários
- ✅ Lógica social pode evoluir independentemente
- ✅ Evita centralização indevida
- ✅ Mais claro e justo
- ⚠️ Mais complexidade em queries que precisam juntar dados

**Alternativas Consideradas**:
1. Território contém lógica social (rejeitado - viola princípio fundamental)
2. Território é apenas ID (rejeitado - perde dados geográficos essenciais)

---

## ADR-004: PresencePolicy para Validação de Presença Física

**Status**: Aceito  
**Data**: 2024  
**Contexto**: MVP exige presença física para vínculo, mas diferentes políticas podem ser necessárias.

**Decisão**: Implementar `PresencePolicy` configurável:
- `None`: Nenhum vínculo exige geo
- `ResidentOnly`: Apenas RESIDENT exige geo (padrão)
- `VisitorAndResident`: Ambos exigem geo

**Consequências**:
- ✅ Flexibilidade para diferentes contextos
- ✅ Configurável via appsettings
- ✅ Validação consistente em toda aplicação
- ⚠️ Necessidade de documentar política escolhida

**Alternativas Consideradas**:
1. Sempre exigir geo para todos (rejeitado - muito restritivo)
2. Nunca exigir geo (rejeitado - viola princípio do MVP)

---

## ADR-005: GeoAnchors Derivados de Mídias

**Status**: Aceito  
**Data**: 2024  
**Contexto**: Posts podem ter GeoAnchors, mas client não deve definir manualmente.

**Decisão**: GeoAnchors são derivados automaticamente de metadados de mídias quando disponíveis. Client não envia GeoAnchors manualmente.

**Consequências**:
- ✅ Dados geográficos mais confiáveis (vêm de EXIF)
- ✅ Simplifica API do client
- ✅ Posts podem existir sem GeoAnchors (não aparecem no mapa)
- ⚠️ Dependência de processamento de mídia (POST-MVP)

**Alternativas Consideradas**:
1. Client define GeoAnchors manualmente (rejeitado - menos confiável)
2. GeoAnchors obrigatórios (rejeitado - muito restritivo)

---

## ADR-006: Clean Architecture com InMemory e Postgres

**Status**: Aceito  
**Data**: 2024  
**Contexto**: Necessidade de suportar desenvolvimento rápido e produção robusta.

**Decisão**: Implementar repositórios InMemory para desenvolvimento/testes e Postgres para produção. Switch via configuração `Persistence:Provider`.

**Consequências**:
- ✅ Desenvolvimento rápido sem dependências externas
- ✅ Testes rápidos e isolados
- ✅ Produção robusta com Postgres
- ✅ Mesma interface para ambos
- ⚠️ Necessidade de manter duas implementações sincronizadas

**Alternativas Consideradas**:
1. Apenas Postgres (rejeitado - dificulta desenvolvimento)
2. Apenas InMemory (rejeitado - não adequado para produção)

---

## ADR-007: Moderação Automática por Threshold

**Status**: Aceito  
**Data**: 2024  
**Contexto**: Necessidade de proteger comunidade de conteúdo inadequado rapidamente.

**Decisão**: Implementar moderação automática quando threshold de reports únicos é atingido (padrão: 3 reports). Ações automáticas incluem ocultar conteúdo e aplicar sanções.

**Consequências**:
- ✅ Proteção rápida da comunidade
- ✅ Reduz carga de moderação manual
- ✅ Threshold configurável
- ⚠️ Possibilidade de falsos positivos
- ⚠️ Necessidade de auditoria

**Alternativas Consideradas**:
1. Apenas moderação manual (rejeitado - muito lento)
2. Threshold muito baixo (rejeitado - muitos falsos positivos)
3. Threshold muito alto (rejeitado - pouco efetivo)

---

## ADR-008: Feature Flags por Território

**Status**: Aceito  
**Data**: 2024  
**Contexto**: Diferentes territórios podem precisar de funcionalidades diferentes.

**Decisão**: Implementar feature flags por território, gerenciadas por curadores.

**Consequências**:
- ✅ Flexibilidade por território
- ✅ Rollout gradual de funcionalidades
- ✅ Possibilidade de desabilitar funcionalidades problemáticas
- ⚠️ Necessidade de gerenciar flags

**Alternativas Consideradas**:
1. Feature flags globais (rejeitado - menos flexível)
2. Sem feature flags (rejeitado - muito rígido)

---

## ADR-009: Work Queue Genérica (WorkItem) para Revisões Humanas

**Status**: Aceito  
**Data**: 2026  
**Contexto**: O sistema precisa de filas de revisão para diferentes domínios (verificação, curadoria, moderação) sem criar “N filas” específicas e inconsistentes.

**Decisão**: Introduzir um modelo genérico `WorkItem` (Work Queue) com:
- `Type`, `Status`, `Outcome`
- `TerritoryId` opcional (itens globais vs territoriais)
- `RequiredSystemPermission` e/ou `RequiredCapability`
- `SubjectType/SubjectId` + `PayloadJson` para contexto mínimo

**Consequências**:
- ✅ Filas padronizadas e extensíveis (novos tipos sem refatorar infra)
- ✅ Governança clara (SystemAdmin vs Curator/Moderator)
- ✅ Auditoria consistente de criação/conclusão
- ⚠️ `PayloadJson` exige disciplina para evitar acoplamento excessivo

**Alternativas Consideradas**:
1. Filas específicas por domínio (rejeitado - duplicação e divergência)
2. “Somente notificações” sem fila (rejeitado - falta rastreabilidade e SLA humano)

---

## ADR-010: Download de Arquivos por Proxy (inicial) vs URL Pré-assinada

**Status**: Aceito  
**Data**: 2026  
**Contexto**: Precisamos suportar evidências/mídias em storage externo (S3/MinIO), controlando acesso e auditando downloads.

**Decisão**: Adotar **download por proxy** inicialmente:
- Cliente chama API
- API autentica/autoriza, audita e faz streaming do storage

**Consequências**:
- ✅ Controle total de acesso e auditoria
- ✅ Simplifica (não exige política de expiração e assinatura no client)
- ⚠️ Maior carga na API (bandwidth/streaming)

**Alternativas Consideradas**:
1. URLs pré-assinadas (rejeitado nesta fase - mais variáveis operacionais no client)
2. URLs públicas (rejeitado - risco de vazamento)

---

## ADR-011: BFF como Aplicação Separada (Fase 2)

**Status**: Aceito  
**Data**: 2026  
**Contexto**: A documentação do projeto prevê um Backend for Frontend (BFF) para reduzir chamadas de rede e expor jornadas formatadas para UI. Após a Fase 1 (BFF como módulo interno na API), evoluiu-se para a Fase 2: o BFF **não é um módulo interno** — é **outra aplicação**.

**Decisão**: O BFF é uma **aplicação separada** (`Araponga.Api.Bff`):
- Projeto próprio: `backend/Araponga.Api.Bff`, deployável de forma independente
- Expõe as mesmas rotas de jornada sob `/api/v2/journeys` (onboarding, feed, events, marketplace), encaminhando as requisições para a API principal (`Araponga.Api`)
- Configuração `Bff:ApiBaseUrl` na aplicação BFF aponta para a URL da API
- O frontend consome o BFF; o BFF faz proxy (e futuramente pode orquestrar chamadas à API v1 diretamente)
- A API principal continua expondo `/api/v2/journeys` para atender as requisições encaminhadas pelo BFF (ou consumo direto em cenários legados)
- **Cache quando necessário**: respostas GET 2xx podem ser cacheadas em memória no BFF (`Bff:EnableCache`, `Bff:CacheTtlSeconds`, `Bff:CacheTtlByPath`) para reduzir chamadas à API; chave inclui path, query e autorização (respostas por usuário). Header de resposta `X-Bff-Cache: HIT` ou `MISS` indica origem.

**Consequências**:
- ✅ BFF e API são aplicações distintas (deploy, escala e evolução independentes)
- ✅ Frontend fala apenas com o BFF; a API pode ficar interna à rede
- ✅ Mesma autenticação JWT: o BFF repassa o token à API
- ✅ Cache no BFF reduz carga na API em leituras repetidas (GET)
- ⚠️ Latência adicional de uma hop HTTP entre BFF e API (aceitável na maioria dos cenários)

**Referências**:
- [Avaliação BFF](docs/AVALIACAO_BFF_BACKEND_FOR_FRONTEND.md)
- [Contrato OpenAPI BFF](docs/BFF_API_CONTRACT.yaml)

---

## ADR-012: Domain e Application por módulo (modularização completa)

**Status**: Aceito  
**Data**: 2026  
**Contexto**: O projeto evoluiu para **modularização completa**: cada contexto de negócio (Feed, Events, Map, Marketplace, Moderation, Assets, etc.) possui Domain e Application no módulo quando aplicável. Core ficou com apenas o núcleo compartilhado.

**Decisão**:
- **Core.Domain**: contém apenas o **núcleo compartilhado** (Users, Membership, Territories, Governance — ex.: TerritoryModerationRule, RuleType —, Geo, Media, Email, Configuration). Não contém mais Map, Marketplace, Moderation (Work, Evidence, Reports, Sanctions), nem Assets.
- **Core.Application**: serviços e eventos **transversais** (ConnectionService, ReportService, TerritoryModerationService, AnalyticsService, etc.) e interfaces **compartilhadas** (IAuditLogger, IUnitOfWork, IUserRepository, IFileStorage). Referencia os módulos (Application) para contratos de repositório.
- **Módulos**: cada um com domínio próprio expõe **Domain**, **Application** e **Infrastructure**. Exceção: **Admin** só Infrastructure; **Connections** não possui Infrastructure no módulo (implementações em Core.Infrastructure).

**Consequências**:
- ✅ Módulos autocontidos para Map, Marketplace, Moderation, Assets, Feed, Events, Notifications, Chat, Alerts, Subscriptions, Connections.
- ✅ Core enxuto: apenas núcleo compartilhado e orquestração.
- ✅ Testes por módulo (Araponga.Tests.Modules.Map, .Marketplace, .Moderation, .Connections, .Subscriptions) executáveis isoladamente.

---

## Estado de desacoplamento atual (modularização completa)

**Objetivo**: Registrar o estado final da modularização Domain/Application para desacoplamento e evolução independente dos módulos.

### Módulos com Domain e Application (Infrastructure no Core)

| Módulo          | Domain no módulo | Application no módulo | Infrastructure | Testes módulo |
|-----------------|------------------|------------------------|---------------|----------------|
| **Connections** | ✅               | ✅ (IUserConnectionRepository, IConnectionPrivacySettingsRepository, IAcceptedConnectionsProvider) | Core.Infrastructure (Postgres/InMemory) | Araponga.Tests.Modules.Connections |

### Módulos com Domain, Application e Infrastructure no módulo (modularização completa)

| Módulo          | Domain no módulo | Application no módulo | Infrastructure no módulo | Testes módulo |
|-----------------|------------------|------------------------|--------------------------|----------------|
| **Feed**        | ✅               | ✅ (IFeedRepository, etc.) | ✅                       | —              |
| **Events**      | ✅               | ✅ (ITerritoryEventRepository, IEventParticipationRepository) | ✅                       | —              |
| **Notifications** | ✅             | ✅ (INotificationInboxRepository, INotificationConfigRepository) | ✅                       | —              |
| **Chat**        | ✅               | ✅                     | ✅                       | —              |
| **Alerts**      | ✅               | ✅ (IHealthAlertRepository) | ✅                       | —              |
| **Subscriptions** | ✅             | ✅ (ISubscriptionRepository, etc.) | ✅                       | Araponga.Tests.Modules.Subscriptions |
| **Assets**      | ✅               | ✅ (ITerritoryAssetRepository, etc.) | ✅                       | —              |
| **Map**         | ✅               | ✅ (IMapEntityRepository, etc.) | ✅                       | Araponga.Tests.Modules.Map |
| **Marketplace** | ✅               | ✅ (IStoreRepository, ICheckoutRepository, etc.) | ✅                       | Araponga.Tests.Modules.Marketplace |
| **Moderation**  | ✅               | ✅ (IReportRepository, IWorkItemRepository, etc.) | ✅                       | Araponga.Tests.Modules.Moderation |

### Módulos só Infrastructure

| Módulo   | Motivo |
|----------|--------|
| **Admin** | Apenas registros de infraestrutura; sem domínio de negócio próprio. |

### Direção das dependências

- **Core.Application** → Core.Domain, Feed.Application, Events.Application, Notifications.Application, Chat.Application, Alerts.Application, Connections.Application, Subscriptions.Application, **Assets.Application**, **Map.Application**, **Marketplace.Application**, **Moderation.Application**, Shared.
- **Core.Infrastructure** → Core.Domain, Core.Application, Feed.Application, Connections.Application, Marketplace.Application (ex.: PayoutProcessingWorker), Moderation.Domain (ex.: StorageProvider para IFileStorage).
- **Modules.*.Infrastructure** → Application (e Domain) do **próprio** módulo; Core quando necessário. Não referenciam outros módulos.
- **Modules.*.Domain** → podem referenciar **Core.Domain** (ex.: Geo, Territories, Users) quando o domínio do módulo depende do núcleo.

### O que permanece no Core.Domain

Núcleo compartilhado: **Users**, **Membership**, **Territories**, **Governance** (TerritoryModerationRule, RuleType), **Geo**, **Media** (contratos compartilhados), **Email**, **Configuration**. Módulos referenciam Core.Domain para tipos compartilhados (ex.: TerritoryId, UserId, GeoCoordinate).

### Testes por módulo

- **Araponga.Tests.Modules.Connections**: Domain + Application.
- **Araponga.Tests.Modules.Subscriptions**: Application + Api + Performance.
- **Araponga.Tests.Modules.Map**, **.Marketplace**, **.Moderation**: Domain (entidades do módulo).

### Validação (build e testes)

- Build da solution: **sucesso**.
- Testes modulares executáveis isoladamente por projeto (`dotnet test Araponga.Tests.Modules.Map`, etc.).

---

## ADR-013: Estrutura de testes em níveis (Core, Shared, Módulos)

**Status**: Aceito  
**Data**: 2026  
**Contexto**: Com a modularização do backend (Domain/Application por módulo), faz sentido organizar os testes em níveis que reflitam o desacoplamento e permitam rodar suites por escopo.

**Decisão**:
- **Araponga.Tests (Core)**: Testes do núcleo — Api (integração), Application (serviços centrais), Domain (entidades centrais), Infrastructure, Bff, Performance. Referencia Core + módulos necessários. Continua sendo o projeto principal de testes.
- **Araponga.Tests.Shared**: Artefatos compartilhados entre projetos de teste — por exemplo `TestIds` (IDs pré-populados e disponíveis para testes). Sem dependências de Application/Infrastructure para permitir uso por qualquer projeto de teste. Referenciado por Araponga.Tests e por projetos de teste de módulos.
- **Araponga.Tests.Modules.\* (por módulo)**: Testes específicos de um módulo (Domain, Application, Api e Performance). Ex.: `Araponga.Tests.Modules.Connections` (Domain + Application); `Araponga.Tests.Modules.Subscriptions` (Application + Api + Performance). Referenciam o módulo (Domain/Application), Core quando necessário (ex.: InMemory, Api, Application services), e Tests.Shared. Permite rodar apenas os testes do módulo (`dotnet test Araponga.Tests.Modules.Connections`) e evita que o projeto Core de testes fique poluído com testes de todos os módulos.

**Consequências**:
- ✅ Desacoplamento: testes do módulo podem evoluir e ser executados isoladamente.
- ✅ CI pode definir níveis (ex.: core primeiro, depois módulos) ou rodar só o módulo alterado.
- ✅ Mesma massa de testes; apenas reorganização em projetos.
- ✅ Projetos de teste por módulo existentes: Connections, Subscriptions, Map, Marketplace, Moderation.

---

## ADR-014: Modularização completa — convenções e dependências

**Status**: Aceito  
**Data**: 2026  
**Contexto**: Consolidar convenções e regras de dependência após a modularização completa do backend.

**Decisão**:

1. **Estrutura por módulo**: Cada módulo de negócio possui (quando aplicável) três projetos: `Araponga.Modules.<Nome>.Domain`, `Araponga.Modules.<Nome>.Application`, `Araponga.Modules.<Nome>.Infrastructure`. O **Admin** mantém apenas Infrastructure; o **Connections** não possui Infrastructure no módulo (implementações em Core.Infrastructure).

2. **Core.Domain**: Contém apenas o núcleo compartilhado (Users, Membership, Territories, Governance, Geo, Media, Email, Configuration). Módulos podem referenciar Core.Domain para tipos compartilhados (TerritoryId, UserId, GeoCoordinate, etc.).

3. **Core.Application**: Contém serviços transversais e interfaces compartilhadas (IAuditLogger, IUnitOfWork, IUserRepository, IFileStorage). Referencia todos os módulos (Application) para contratos de repositório. Não contém entidades de contexto de módulo.

4. **Dependências**: Modules.*.Infrastructure referenciam apenas o próprio módulo (Domain/Application) e Core quando necessário. Modules.*.Domain podem referenciar Core.Domain. Módulos não referenciam outros módulos.

5. **Namespaces**: Alguns módulos (Connections, Chat, Events, Alerts, Notifications, Subscriptions) mantêm namespaces `Araponga.Domain.*` nos tipos de domínio por compatibilidade; o código está nos projetos `Araponga.Modules.*.Domain`. Migração futura para `Araponga.Modules.*.Domain` é opcional.

**Referência**: Ver tabelas em *Estado de desacoplamento atual* neste documento.

---

## ADR-015: Um projeto por módulo (alternativa à estrutura Domain/Application/Infrastructure separados)

**Status**: Proposto  
**Data**: 2026  
**Contexto**: Cada módulo hoje tem três projetos (Domain, Application, Infrastructure), o que gera muitos .csproj na solution e mais overhead de referências e build. Surge a dúvida: **faz sentido ter um único projeto por módulo?**

**Decisão (alternativa aceitável)**:

Sim, **um projeto por módulo faz sentido** e é uma opção válida:

- **Estrutura**: Um único projeto `Araponga.Modules.<Nome>` com pastas **Domain/**, **Application/** (Interfaces, serviços do contexto) e **Infrastructure/** (Postgres, módulo DI). A separação lógica (camadas) permanece por convenção de pastas e namespaces.
- **Vantagens**:
  - Menos projetos na solution (ex.: 12 módulos × 3 = 36 projetos → 12 projetos).
  - Build e referências mais simples; Core referencia um único projeto por módulo.
  - Módulo continua sendo a unidade de deploy e evolução; a fronteira é o módulo, não a camada.
- **Desvantagens**:
  - A separação Domain/Application/Infrastructure deixa de ser enforced por assembly (Domain poderia, em tese, referenciar Infrastructure no mesmo projeto). Mitigação: convenção de pastas, code review e talvez analyzers (ex.: Domain não pode depender de Application ou Infrastructure).
- **Quando preferir 1 projeto por módulo**: equipes menores, foco em simplicidade da solution, módulos que não precisam ser referenciados “só pelo Domain” ou “só pela Application” por outros assemblies.
- **Quando manter 3 projetos por módulo**: quando se quer garantia forte por assembly (Domain sem dependências de infra), reuso fino (outro módulo ou API referenciar só o Domain do módulo X) ou times maiores com ownership por camada.

**Migração**: Se for adotada a opção “um projeto por módulo”, para cada módulo: criar `Araponga.Modules.<Nome>.csproj` único, mover o conteúdo de Domain, Application e Infrastructure para pastas Domain/, Application/, Infrastructure/ dentro desse projeto, atualizar referências no Core e na solution, e remover os três projetos antigos. Os testes do módulo passam a referenciar apenas o novo projeto único.

---

## ADR-016: Organização de pastas do backend e regras de dependência

**Status**: Aceito  
**Data**: 2026  
**Contexto**: Após a modularização, havia projetos de teste soltos na raiz do backend e necessidade de documentar a estrutura final, evitar referências circulares e registrar exceções (ex.: Feed → Map).

**Decisão**:

1. **Estrutura de pastas**:
   - **backend/Core/** — API, Application, Application.Abstractions, Domain, Infrastructure, Infrastructure.Shared, Shared.
   - **backend/Modules/** — **um projeto por módulo**: cada módulo (Feed, Events, Map, Moderation, Alerts, Assets, Chat, Notifications, Subscriptions, Marketplace, Admin, **Connections**) é um único projeto com pastas Domain/, Application/ e Infrastructure/. **Connections** segue o mesmo padrão: tem **ConnectionsDbContext** e repositórios Postgres na pasta Infrastructure do módulo; as tabelas são as mesmas (user_connections, connection_privacy_settings), e a migration que as criou permanece no Core para compatibilidade.
   - **backend/Tests/** — todos os projetos de teste (Araponga.Tests, Araponga.Tests.Shared, Araponga.Tests.Modules.*) agrupados numa pasta **Tests** para clareza e boas práticas.
   - **backend/Araponga.Api.Bff/** — BFF na raiz do backend.

2. **Referências circulares**:
   - **Araponga.Application.Abstractions** contém apenas IModule e IUnitOfWorkParticipant (namespace Araponga.Application.Interfaces). Módulos que implementam IModule referenciam **Application.Abstractions** (e não Core.Application), quebrando o ciclo Application ↔ Modules.Infrastructure.
   - Core.Application referencia os módulos (Application); módulos (Infrastructure) não referenciam Core.Application diretamente quando precisam de IModule/IUnitOfWorkParticipant — usam Application.Abstractions ou Infrastructure.Shared.

3. **Exceção: dependência entre módulos**:
   - **Feed** referencia **Map** (tipos de geo para itens do feed). É a única dependência explícita entre módulos e foge à regra "módulos não referenciam outros módulos". Deve permanecer documentada; migração futura (ex.: mover tipo compartilhado para Core.Domain) é opcional.

4. **Verificação**: Não há ciclo no grafo de referências entre projetos. Build e testes passam após a reorganização.

**Referência**: [backend/README.md](../backend/README.md), [backend/docs/BACKEND_LAYERS_AND_NAMING.md](../backend/docs/BACKEND_LAYERS_AND_NAMING.md).

---

## ADR-017: Assimetrias de implementação e resoluções (Clean Code / SOLID)

**Status**: Aceito  
**Data**: 2026  
**Contexto**: Existiam assimetrias entre módulos (ex.: Connections com infra no Core vs no módulo), uso de Unit of Work (composite vs DbContext como UoW) e repositórios (interfaces no módulo, implementações no Core). Objetivo: identificar e corrigir para alinhar às melhores práticas.

**Assimetrias identificadas**

1. **Unit of Work (UoW)**
   - **Problema**: Em Postgres, `IUnitOfWork` é implementado por `CompositeUnitOfWork` (agrega participantes) e também por `ArapongaDbContext` e `SharedDbContext`. Os dois DbContexts atuam como UoW (Commit, Begin, Rollback), mas na prática só o `ArapongaDbContext` é usado como “primary” para transação; o commit é feito via participantes. Isso mistura responsabilidade: DbContext é infra de persistência, não contrato de aplicação (IUnitOfWork).
   - **Princípio**: Interface Segregation e Single Responsibility — apenas o orquestrador (CompositeUnitOfWork) e o UoW in-memory (InMemoryUnitOfWork) devem implementar `IUnitOfWork`; DbContexts devem ser apenas participantes.
   - **Resolução**: Criar `DbContextTransactionScopeAdapter` que implementa `IUnitOfWork` e delega Begin/Rollback/HasActive ao contexto principal; `CommitAsync` no adapter é no-op (o commit real é via participante). Remover a implementação de `IUnitOfWork` de `ArapongaDbContext` e `SharedDbContext`.

2. **Repositórios fora do módulo (Feed)**
   - **Problema**: `IPostGeoAnchorRepository` e `IPostAssetRepository` são do módulo Feed, mas as implementações Postgres estavam no Core.Infrastructure.
   - **Resolução (aplicada)**: Implementações Postgres migradas para o módulo Feed (`PostgresPostGeoAnchorRepository`, `PostgresPostAssetRepository` em `Araponga.Modules.Feed.Infrastructure.Postgres`), usando `FeedDbContext`; registros em `FeedModule`. Implementações InMemory permanecem no Core (uso do `InMemoryDataStore` compartilhado em testes/dev). **Repositórios Feed usam apenas FeedDbContext** para PostGeoAnchor e PostAsset.

3. **Duplicação de entidades em dois DbContexts**
   - **Problema**: `PostGeoAnchorRecord` / `PostAssetRecord` e tabelas correspondentes existiam em `ArapongaDbContext` e em `FeedDbContext` (duplicação de configuração).
   - **Resolução (aplicada)**: Entidades **removidas do modelo** do `ArapongaDbContext` (DbSets e `OnModelCreating`). Apenas `FeedDbContext` configura e usa essas tabelas. Ao gerar a próxima migração do ArapongaDbContext, remover do `Up()` as chamadas `DropTable` para `post_geo_anchors` e `post_assets` para não apagar as tabelas.

4. **Registro de serviços na API**
   - **Problema**: Alguns repositórios e provedores (ex.: `IAcceptedConnectionsProvider`) são registrados na API em `AddPostgresRepositories` ou em `AddApplicationServices`, enquanto a maioria dos repositórios de módulos é registrada via `IModule.RegisterServices`.
   - **Estado**: Connections foi alinhado (repositórios no ConnectionsModule); `IAcceptedConnectionsProvider` permanece na API por depender de Application (AcceptedConnectionsProvider). Aceitável: provedores de aplicação podem ficar no Core; repositórios nos módulos.

5. **Namespaces de Application**
   - **Problema**: Maioria dos serviços em `Araponga.Application.Services`; alguns em subnamespaces (ex.: `Araponga.Application.Services.Connections`, `Araponga.Application.Services.Notifications`).
   - **Resolução**: Opcional — padronizar por feature (Connections, Notifications, etc.) para consistência; não obrigatório para funcionamento.

**Decisão (aplicada)**  
- **UoW**: Implementado `DbContextTransactionScopeAdapter`; `ArapongaDbContext` e `SharedDbContext` deixam de implementar `IUnitOfWork`. Apenas `CompositeUnitOfWork`, `InMemoryUnitOfWork` e o adapter (como “primary” de transação) implementam o contrato.
- **Repositórios Feed**: `IPostGeoAnchorRepository` e `IPostAssetRepository` têm implementações Postgres no módulo Feed (FeedDbContext) e registro em `FeedModule`; InMemory permanece no Core.

**Referência**: [backend/README.md](../backend/README.md), VALIDACAO_INFRAESTRUTURA_INTEGRIDADE.md.

---

## ADR-018: Avaliação de assimetrias (logs, exceções, conectividade, acesso, autenticação)

**Status**: Aceito (implementado)  
**Data**: 2026  
**Contexto**: Garantir padronização e integridade da arquitetura além dos repositórios e UoW: avaliar logs, exceções, tipos de conectividade, pontos de acesso e autenticação para identificar assimetrias e equalizar onde necessário. O backend serve o BFF e precisa expor identidade do usuário para autorização (cada usuário acessa apenas seus dados); sessão autenticada; renovação de tokens (refresh); e autenticação sistêmica para workers.

### 1. Conectividade (DbContext / Postgres)

**Estado**: **Uniforme**.

- Todos os módulos e o Core usam `GetConnectionString("Postgres")` e `UseNpgsql` com os mesmos parâmetros: `EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: 5s)`, `CommandTimeout(30)`.
- SharedDbContext (Infrastructure.Shared) segue o mesmo padrão.
- **Ação**: Nenhuma; opcionalmente extrair constantes no futuro.

### 2. Exceções e tratamento global

**Estado**: **Uniforme** (implementado).

- Handler global em `Program.cs` (`UseExceptionHandler`) mapeia para `ProblemDetails`: `ValidationException` → 400, `NotFoundException` → 404, `UnauthorizedException` → 401, `ForbiddenException` → 403, `ConflictException` → 409, `ArgumentException` → 400, `InvalidOperationException` → 409, **`DomainException`** → 400.
- **Implementado**: `DomainException` foi incluído no switch para retornar 400 em vez de 500 quando lançada diretamente.

### 3. Logging

**Estado**: **Documentado**.

- **ILogger**: logs operacionais/debug (serviços, workers, middlewares, controllers).
- **IObservabilityLogger**: eventos de negócio/métricas para tratamento uniforme (agregação, alertas).
- **Ação**: Documentado em [backend/README.md](../backend/README.md).

### 4. Pontos de acesso (controllers, rotas, versionamento)

**Estado**: **Consistente**.

- Rotas: `api/v1/...` (recursos); `api/v2/journeys/...` (jornadas). Padrão: `[ApiController]`, `[Route("api/v1/...")]`, Swagger/ProblemDetails.
- **Ação**: Nenhuma; distinção v1 vs v2 é intencional.

### 5. Autenticação e autorização

**Decisão**: **Opção B implementada** — JWT integrado ao pipeline, políticas de autorização, refresh tokens e client credentials para workers.

**Implementado**:

- **JWT no pipeline**: `JwtAuthenticationHandler` valida o Bearer token e define `HttpContext.User` (ClaimsPrincipal). `CurrentUserAccessor` prioriza `HttpContext.User` e faz fallback para parsing manual do token quando necessário.
- **Resposta de login**: `SocialLoginResponse` inclui `Token` (access), `RefreshToken` e `ExpiresInSeconds`. O BFF/frontend guarda o access token no estado e usa o refresh token para renovar quando expirar.
- **Refresh tokens**: Interface `IRefreshTokenStore` e implementação `InMemoryRefreshTokenStore`; tokens emitidos no login social e nos fluxos de verificação/recuperação 2FA. Endpoint `POST /api/v1/auth/refresh` recebe `RefreshToken` e devolve novo par access + refresh (rotação). Configuração `RefreshToken` (ex.: `RefreshTokenValidity`).
- **Client credentials (workers)**: Endpoint `POST /api/v1/auth/token` para fluxo client_credentials (`client_id`, `client_secret`). `ITokenService.IssueSystemToken` emite JWT para identidade sistêmica. Configuração `ClientCredentials` (clientes permitidos). Workers autenticam-se com esse token para chamar APIs em nome do sistema.

**Consequências**:

- Frontend autentica no backend; estado da aplicação (BFF) pode manter token e refresh para renovação.
- Cada usuário acessa apenas seus dados via autorização baseada em identidade (claims/User).
- Workers têm autenticação sistêmica via client credentials e token de sistema.
- Padrão de "chave que inspira" (access token de vida curta + refresh para obter nova chave) implementado.

### 6. Webhooks (Stripe, MercadoPago)

**Estado**: **Adequado**.

- Endpoints de webhook públicos; Stripe valida assinatura no corpo; MercadoPago segue o mesmo padrão.

### Resumo e priorização

| Área              | Estado        | Ação realizada |
|-------------------|---------------|----------------|
| Conectividade     | Uniforme      | Nenhuma |
| Exceções          | Uniforme      | DomainException no handler (400) |
| Logging           | Documentado   | README: ILogger vs IObservabilityLogger |
| Rotas/API         | Consistente   | Nenhuma |
| Autenticação/Auth | Implementado  | JWT no pipeline; SystemAdmin policy; refresh tokens; client credentials (workers); respostas com Token + RefreshToken + ExpiresIn |
| Webhooks          | OK            | Nenhuma |

**Referência**: [backend/README.md](../backend/README.md), [backend/Araponga.Api/Program.cs](../backend/Araponga.Api/Program.cs), [backend/Araponga.Api/Security/](../backend/Araponga.Api/Security/).

---

## ADR-019: Avaliação de simetria dos testes

**Status**: Aceito (implementado)  
**Data**: 2026  
**Contexto**: Avaliar a simetria dos testes (estrutura, convenções, reuso e padrões) entre Core e módulos para identificar assimetrias e oportunidades de padronização.

### 1. Estrutura por projeto

| Projeto | Domain | Application | Api | Performance | Observação |
|---------|--------|-------------|-----|-------------|------------|
| **Araponga.Tests** (Core) | ✅ | ✅ | ✅ (BDD, muitos *Controller*/*Integration*) | ✅ | Projeto principal; referência ApiFactory em Api/. |
| **Araponga.Tests.Shared** | — | — | — | — | Apenas `TestIds`; sem helpers de auth ou HTTP. |
| **Araponga.Tests.Modules.Connections** | ✅ | ✅ | ❌ | ❌ | Integração de Connections no Core (ConnectionsIntegrationTests). |
| **Araponga.Tests.Modules.Map** | ✅ | ❌ | ❌ | ❌ | Apenas Domain. |
| **Araponga.Tests.Modules.Marketplace** | ✅ | ❌ | ❌ | ❌ | Apenas Domain. |
| **Araponga.Tests.Modules.Moderation** | ✅ | ❌ | ❌ | ❌ | Apenas Domain. |
| **Araponga.Tests.Modules.Subscriptions** | ❌ | ✅ | ✅ | ✅ | Único módulo com Api + ApiFactory próprio. |

**Assimetrias**:
- Integração de API do módulo Connections está no Core, não no módulo; Subscriptions tem integração no módulo.
- Módulos Feed, Events, Notifications, Chat, Alerts não possuem projeto de testes dedicado (cobertura apenas via Core).
- Subscriptions é o único módulo com testes de Performance no próprio projeto.

### 2. Autenticação em testes de API

**Estado**: **Implementado** (Core).

- **AuthTestHelper** (Araponga.Tests/TestHelpers/AuthTestHelper.cs): helper compartilhado com `LoginForTokenAsync`, `LoginAndGetResponseAsync`, `SetAuthHeader`, `SetSessionId` e `SetupAuthenticatedClient`. Testes de API do Core passaram a usar o helper; SessionId centralizado em `SetupAuthenticatedClient` (obrigatório para chamadas autenticadas quando a API usa session).
- **Subscriptions**: mantém `LoginForTokenAsync` local (o projeto não referencia Araponga.Tests para evitar dependências pesadas). Contrato alinhado ao do helper (SocialLoginRequest/Response).

### 3. ApiFactory

**Estado**: **Documentado**.

- **Core**: `Araponga.Tests.Api.ApiFactory` — configuração completa (appsettings, Cors, RateLimiting, Persistence, InMemoryDataStore/InMemorySharedStore isolados).
- **Subscriptions**: `Araponga.Tests.Modules.Subscriptions.Api.ApiFactory` — versão enxuta. Mantido no módulo para evitar referência de Subscriptions ao projeto Araponga.Tests (evita dependências transitivas). **Ação**: documentado no README dos testes; manter variáveis de ambiente (JWT, RateLimiting, Persistence) em sincronia com o Core quando alterar configuração.

### 4. Padrões de uso do factory

**Estado**: **Consistente e documentado**.

- **Maioria**: `using var factory = new ApiFactory();` por teste (isolamento total).
- **Performance**: StressTests, LoadTests, MediaPerformanceTests, FeedWithMediaPerformanceTests usam `IClassFixture<ApiFactory>` para reutilizar uma instância.
- **Documentado** no README: usar fixture em testes de carga; nos demais, factory por teste.

### 5. Nomenclatura e convenções

**Estado**: **Documentado**.

- **Edge cases**: Sufixo `*EdgeCasesTests` usado de forma consistente para cenários de borda (Domain e Application).
- **Integração/Controller**: Uso misto de `*IntegrationTests` e `*ControllerTests` para testes que chamam HTTP; não há regra explícita (ex.: “Integration = múltiplos serviços; Controller = um endpoint”).
- **Recomendação**: Documentar convenção (ex.: *IntegrationTests para fluxos que cruzam vários endpoints ou serviços; *ControllerTests para foco em um controller). Opcional: padronizar usings para `SocialLoginResponse` (um único namespace nos testes de auth).

### 6. TestIds e dados compartilhados

**Estado**: **Documentado**.

- **TestIds**: preferir quando o teste depender de dados pré-populados no InMemoryDataStore; usar GUIDs locais quando o teste criar todas as entidades. **Documentado** no README dos testes.

### 7. SessionId e headers

**Estado**: **Implementado**.

- Alguns testes setam `ApiHeaders.SessionId` antes de chamadas autenticadas (ex.: ConnectionsIntegrationTests, DevicesControllerTests em um dos fluxos); outros não.
- Se a API passar a exigir session em mais endpoints, testes que não setam o header podem falhar de forma desigual.
- **Recomendação**: Definir se SessionId é obrigatório para testes de API autenticados; em caso positivo, centralizar no helper de login ou em um método de extensão “SetupAuthenticatedClient” que defina Bearer + SessionId.

### Resumo e priorização

| Área | Estado | Ação realizada |
|------|--------|----------------|
| Estrutura por módulo | Documentado | Onde vivem testes de integração descrito na tabela e no README. |
| Helper de login | Implementado | AuthTestHelper em Araponga.Tests/TestHelpers; Core usa; Subscriptions mantém local. |
| ApiFactory | Documentado | Subscriptions mantém ApiFactory próprio; README + ADR orientam sincronia de config. |
| Uso do factory | Documentado | README: fixture em Performance; factory por teste nos demais. |
| Nomenclatura | Documentado | README: *IntegrationTests, *ControllerTests, *EdgeCasesTests. |
| TestIds | Documentado | README: quando usar TestIds vs GUIDs locais. |
| SessionId/headers | Implementado | Centralizado em SetupAuthenticatedClient. |

**Referência**: [backend/Tests/](../backend/Tests/), [ADR-013](#adr-013-estrutura-de-testes-em-níveis-core-shared-módulos) (estrutura em níveis).

---

## Referências

- [Product Vision](./01_PRODUCT_VISION.md)
- [User Stories](./04_USER_STORIES.md)
- [Architecture C4](./design/Archtecture/C4_Components.md)
