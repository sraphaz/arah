# Melhor opção para modularização: Shared como fonte da verdade + InMemory espelhando a dinâmica

**Data**: 2026-02-02  
**Objetivo**: Definir a direção recomendada para reduzir duplicação, alinhar Postgres e InMemory, e consolidar a “fonte da verdade” em Infrastructure.Shared.

---

## 1. Visão geral

Hoje temos:

- **Postgres**: SharedDbContext + ArapongaDbContext + um DbContext por módulo; entidades duplicadas (Infrastructure/Entities, Shared/Entities, módulos).
- **InMemory**: um único “monólito” em `Araponga.Infrastructure/InMemory` — todos os repositórios InMemory no mesmo projeto, sem espelhar a fronteira shared vs módulo.

A melhor opção combina dois eixos:

1. **Shared como fonte da verdade** para entidades e contexto “core”.
2. **InMemory replicando a dinâmica de Shared**: um “shared” InMemory + stores/participantes por módulo, espelhando a fronteira Postgres.

Assim, tanto em Postgres quanto em InMemory fica claro o que é **compartilhado** (território, usuário, membership, etc.) e o que é **por módulo** (feed, events, map, etc.), e a manutenção fica alinhada entre os dois modos.

---

## 2. Shared como fonte da verdade

### 2.1 O que significa

- **Entidades “core” / shared** (User, Territory, TerritoryMembership, JoinRequest, WorkItem, DocumentEvidence, NotificationConfig, UserPreferences, etc.) existem **apenas** em `Araponga.Infrastructure.Shared/Postgres/Entities`.
- **SharedDbContext** é o único contexto EF que define e mapeia essas tabelas para persistência Postgres.
- **ArapongaDbContext** (na Infrastructure principal) deixa de ter DbSets para domínios já migrados para módulos e, no limite, pode ser reduzido a:
  - migrations (até migrarmos migrations para Shared ou para um projeto dedicado), e/ou
  - apenas o que ainda não for “shared” nem “módulo” (ex.: Financial, Outbox, EmailQueue, etc., até serem modularizados ou movidos para Shared).

### 2.2 Benefícios

- Uma única definição por tabela core: sem drift entre Infrastructure/Entities e Shared/Entities.
- Teste de padronização (DuplicateEntityStandardizationTests) pode ser simplificado ou focado só em exceções (tabelas ainda duplicadas durante a migração).
- Repositórios Postgres que hoje usam ArapongaDbContext para entidades core passam a usar **SharedDbContext** (ou um contexto que referencie as entidades de Shared).
- Migrations: podem continuar temporariamente em Infrastructure (referenciando Shared para o modelo) ou, em uma fase posterior, migrar para um projeto de “schema” ou para Shared.

### 2.3 Passos sugeridos (resumido)

1. Consolidar em **Shared** todas as entidades que são “core” (já existem em Shared hoje; remover duplicatas da Infrastructure principal).
2. Mover repositórios Postgres de Territory, User, Membership, JoinRequest, etc. da Infrastructure principal para usar **SharedDbContext** (ou para um projeto que dependa só de Shared).
3. Ajustar **ArapongaDbContext** para não mais declarar DbSets/entidades que passaram a ser exclusivas de Shared (e, se necessário, manter um contexto “legado” só para migrations até unificar o schema).
4. Atualizar **CompositeUnitOfWork** e participantes: Shared já é participante; o contexto principal pode vir a ser só Shared para o core, ou manter ArapongaDbContext apenas para o que restar na principal.

---

## 3. InMemory replicando a dinâmica de Shared

### 3.1 Objetivo

Hoje o InMemory é um único bloco: todos os repositórios InMemory vivem em `Araponga.Infrastructure/InMemory` e usam um (ou poucos) stores em memória, sem separar “dados shared” de “dados por módulo”.

A ideia é **replicar a dinâmica de Shared**:

- **InMemory “shared”**: um store (ou um “contexto” InMemory) que mantém os mesmos agregados que SharedDbContext em Postgres (User, Territory, Membership, JoinRequest, etc.).
- **InMemory por módulo**: cada módulo pode expor seu próprio “store” ou seus repositórios InMemory que leem/escrevem em um store do módulo (Feed, Events, Map, etc.), em vez de tudo estar na pasta InMemory da Infrastructure principal.

Assim, em testes e em modo InMemory:

- Dados core ficam no “shared” InMemory.
- Dados de feed, events, map, etc. ficam em stores/participantes por módulo.
- O **InMemoryUnitOfWork** (ou equivalente) pode espelhar o **CompositeUnitOfWork**: um UoW que “commita” todos os participantes (shared + módulos), na mesma linha do Postgres.

### 3.2 Opções de desenho

**Opção A – Store shared + stores por módulo (recomendada para espelhar Postgres)**

- **InMemorySharedStore** (ou similar): dicionários/coleções para User, Territory, Membership, JoinRequest, etc., em `Araponga.Infrastructure.Shared` ou em uma pasta `InMemory` dentro de Shared.
- Cada **módulo** que hoje tem repositório InMemory na Infrastructure principal passa a ter seu próprio **InMemoryStore** (ou lista de stores) no módulo, por exemplo `Araponga.Modules.Feed.Infrastructure/InMemory/InMemoryFeedStore.cs` e repositórios Feed que usam esse store.
- **InMemoryUnitOfWork** vira um “composite”: ao fazer CommitAsync, aplica as “persistências” do shared store e dos stores dos módulos (na mesma ordem ou contrato que o CompositeUnitOfWork em Postgres).
- Registro: em modo InMemory, a API registra o shared store + os módulos registram seus stores; o UoW composto InMemory agrega todos.

**Opção B – Manter um único InMemoryDataStore, mas com “regiões” lógicas**

- O mesmo **InMemoryDataStore** atual é dividido em “regiões” (shared vs Feed vs Events vs …) e cada repositório InMemory acessa só a sua região.
- Menos mudança estrutural, mas a fronteira shared vs módulo continua pouco visível no código (tudo no mesmo projeto/pasta).
- Útil como passo intermediário antes de mover repositórios InMemory para os módulos.

**Recomendação**: evoluir na direção da **Opção A** (shared store + store por módulo e InMemory UoW composto), para alinhar com Postgres e com a ideia de “Shared como fonte da verdade” também no modo em memória.

### 3.3 Benefícios

- Testes e ambiente InMemory passam a usar as **mesmas fronteiras** que produção (shared vs módulo).
- Facilita testes de módulo: o módulo pode prover seu próprio store InMemory e um “shared” mockado ou mínimo.
- Preparação para eventual extração de módulos para serviços separados: a fronteira de dados já está clara em ambos os modos.

---

## 4. Ordem sugerida de execução

1. **Fase 1 – Shared como fonte da verdade (Postgres)**  
   - Consolidar entidades core em Shared; repositórios Postgres core usam SharedDbContext; reduzir ArapongaDbContext ao que não for core (e migrations, se ainda lá).  
   - Atualizar CompositeUnitOfWork/participantes conforme necessário.  
   - Documentar e, se útil, manter o teste de padronização apenas para tabelas ainda em transição.

2. **Fase 2 – InMemory “shared”**  
   - Introduzir **InMemorySharedStore** (ou equivalente) com os mesmos agregados que SharedDbContext, em Infrastructure.Shared (ou pasta InMemory da principal, mas com contrato “shared”).  
   - Repositórios InMemory de Territory, User, Membership, JoinRequest, etc. passam a usar esse store shared.  
   - InMemoryUnitOfWork pode já passar a “commitar” esse store (e, na próxima fase, os stores dos módulos).

3. **Fase 3 – InMemory por módulo**  
   - Por módulo (Feed, Events, Map, etc.), mover ou criar repositórios InMemory no próprio módulo, com store próprio.  
   - Registrar stores e participantes no UoW InMemory composto.  
   - Remover gradualmente os repositórios InMemory desses domínios da pasta única em `Araponga.Infrastructure/InMemory`.

4. **Fase 4 (opcional) – Migrations e limpeza**  
   - Avaliar mover migrations para um projeto que dependa de Shared (ou projeto “Schema”) e deprecar ArapongaDbContext para tudo que já estiver em Shared.  
   - Deixar a Infrastructure principal apenas com o que for realmente “não modularizado” (workers, health, métricas, etc.).

---

## 5. Resumo

| Eixo | Hoje | Melhor opção |
|------|------|--------------|
| **Entidades core** | Duplicadas (Infrastructure + Shared) | **Shared como única fonte da verdade** |
| **Contexto Postgres core** | ArapongaDbContext + SharedDbContext | **SharedDbContext** como contexto das entidades core; ArapongaDbContext só o restante (e migrations até migrar). |
| **InMemory** | Um único bloco na Infrastructure | **InMemory shared** (espelhando Shared) + **InMemory por módulo**; **UoW InMemory composto** espelhando CompositeUnitOfWork. |

Com isso, a modularização fica coerente em Postgres e InMemory, e Shared passa a ser de fato a fonte da verdade para o núcleo compartilhado.

---

## 6. Manter apenas Shared e módulos (estado-alvo)

Objetivo: **eliminar a Infrastructure “principal” como dona de entidades e repositórios**, deixando só:

- **Araponga.Infrastructure.Shared**: fonte da verdade para entidades core + SharedDbContext + repositórios Postgres core + (futuro) InMemory shared.
- **Araponga.Modules.*.Infrastructure**: cada módulo com seu DbContext, entidades e repositórios Postgres/InMemory do domínio.
- **Araponga.Infrastructure** (principal): apenas o que **não** for core nem módulo — por exemplo: migrations (até migrarmos para Shared), workers (Outbox, Email, Renewal, Payout), ConnectionPoolMetricsService, HealthCheck, e repositórios que ainda não foram movidos para Shared ou para módulos. No limite, pode ser reduzido a um “shell” que registra Shared + módulos e hospeda workers/métricas.

### 6.1 Checklist de aplicação

| Etapa | Descrição | Status |
|-------|-----------|--------|
| **Fase 1a** | Repositórios Postgres core em Shared (SharedDbContext): Territory, User, JoinRequest, Membership, Voting, etc. | Concluído (todos os listados em 6.2 migrados) |
| **Fase 1b** | Remover registro desses repositórios de AddPostgresRepositories (principal); registrar em AddSharedCrossCuttingServices | Concluído |
| **Fase 1c** | Mover todos os repositórios Postgres cujas entidades estão em Shared para Infrastructure.Shared | Concluído (Territory, User, Membership, JoinRequest, UserPreferences, UserInterest, Voting, Vote, TerritoryCharacterization, MembershipSettings, MembershipCapability, SystemPermission, SystemConfig, TermsOfService, TermsAcceptance, PrivacyPolicy, PrivacyPolicyAcceptance, UserDevice) |
| **Fase 1d** | ArapongaDbContext: remover DbSets que já estão só em Shared (ou manter só para migrations até migrar) | Pendente |
| **Fase 2a** | InMemorySharedStore em Shared com agregados core | Concluído |
| **Fase 2b** | Repositórios InMemory core usam InMemorySharedStore; InMemoryUnitOfWork composto (shared + módulos) | Concluído |
| **Fase 3** | InMemory por módulo: cada módulo com seu store e repositórios InMemory | Pendente |
| **Fase 4** | Migrations em Shared (ou projeto Schema); deprecar ArapongaDbContext para tabelas já em Shared | Opcional |

### 6.2 Repositórios que vão para Shared (Postgres)

Todos os que usam **apenas** entidades já definidas em Shared/Entities:

- Territory, User, UserPreferences, UserDevice, UserInterest  
- TerritoryMembership, MembershipSettings, MembershipCapability  
- TerritoryJoinRequest, TerritoryJoinRequestRecipient  
- Voting, Vote, TerritoryCharacterization, TerritoryModerationRule  
- NotificationConfig, WorkItem, DocumentEvidence  
- SystemConfig, SystemPermission  
- TermsOfService, TermsAcceptance, PrivacyPolicy, PrivacyPolicyAcceptance  
- OutboxMessage, UserNotification, ActiveTerritory, FeatureFlag, AuditEntry  
- EmailQueueItem  

(Os que dependem de Financial, Media, ModerationReport, UserBlock, etc. podem ficar na principal até essas áreas irem para Shared ou módulos.)

### 6.3 O que permanece na Infrastructure principal (até migrar)

- Migrations (ArapongaDbContext)  
- ConnectionPoolMetricsService, DatabaseHealthCheck  
- Workers: OutboxDispatcherWorker, PayoutProcessingWorker, SubscriptionRenewalWorker, EmailQueueWorker, EventReminderWorker  
- Repositórios Postgres que ainda não foram movidos (Financial, Media, UserBlock, etc.)  
- **Todos** os repositórios InMemory (até Fase 2/3: shared store + por módulo)  

---

**Última atualização**: 2026-02-02
