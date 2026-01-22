# Impacto de Usar Testcontainers + PostgreSQL nos Testes

## O que é Testcontainers?

[Testcontainers](https://testcontainers.com/) é uma biblioteca que sobe **containers Docker reais** durante a execução dos testes. Para .NET, o pacote `Testcontainers.PostgreSql` sobe um container PostgreSQL efêmero, aplica migrações, expõe a connection string e derruba o container ao fim dos testes.

**Objetivo**: Testes de integração contra um banco **idêntico ao de produção**, em vez de InMemory ou SQLite.

---

## Situação Atual

### Infraestrutura de testes

| Componente | Situação atual |
|------------|----------------|
| **ApiFactory** | `WebApplicationFactory<Program>` + `Persistence__Provider=InMemory` |
| **DataStore** | `InMemoryDataStore` (singleton por factory), repositórios InMemory |
| **CI (GitHub Actions)** | `ubuntu-latest`, **sem** PostgreSQL, **sem** serviço de banco |
| **Testes que usam Postgres** | Apenas `ConcurrencyTests` (2), via `DatabaseFixture` + `TEST_DATABASE_CONNECTION_STRING`; **sempre pulados** no CI |
| **Demais testes** | ~740 testes usam InMemory (ApiFactory, FeedServiceTestHelper, etc.) |

### Problema que motiva a discussão

- `DevicesControllerTests.RegisterDevice_WhenValid_CreatesDevice` falha no ambiente InMemory (autenticação / contexto).
- Em produção (Postgres), o fluxo funciona; o defeito é **do ambiente de teste**, não do código.

---

## Impacto da mudança: visão geral

| Aspecto | Impacto |
|--------|---------|
| **Onde mudar** | `ApiFactory`, eventual novo factory, CI, pacotes NuGet |
| **Escopo** | Pode ser **parcial** (só alguns testes) ou **total** (todos os de integração) |
| **Dependência** | **Docker** obrigatório (local e no runner do CI) |
| **Tempo de execução** | Aumento relevante (~segundos por container, mais migrações) |
| **Confiabilidade** | Aumenta (comportamento igual ao de produção) |
| **Complexidade** | Aumenta (lifecycle do container, connection string, possíveis flakiness) |

---

## 1. Impacto na solução atual

### 1.1 ApiFactory e configuração da API

Hoje o `ApiFactory`:

- Define `Persistence__Provider=InMemory`.
- Remove o `InMemoryDataStore` padrão e registra um novo singleton por factory.
- Todos os testes de API usam InMemory.

**Com Testcontainers + Postgres**:

- Precisamos **trocar** (ou **escolher**) o provider de persistência para **Postgres** na hora do teste.
- A connection string virá do **container** (ex.: `container.GetConnectionString()`).
- O `ApiFactory` (ou um `ApiFactoryPostgres`) precisaria:
  - Subir o container (ex.: em `IAsyncLifetime.InitializeAsync`).
  - Definir `Persistence__Provider=Postgres` e a connection string.
  - Garantir que migrações rodem (`EnsureCreated` ou migrações EF) no banco do container.
  - Desligar o container no `Dispose`/`DisposeAsync`.

**Efeito**: mudança **estrutural** no factory de testes; o “modo InMemory” deixa de ser o único.

### 1.2 Testes que hoje usam InMemory

A maioria dos ~740 testes:

- Usa `ApiFactory` + HttpClient **ou**
- Usa `InMemoryDataStore` / `FeedServiceTestHelper` etc. **diretamente**, sem HTTP.

**Se migrarmos só os testes de API para Testcontainers**:

- Testes que **só** chamam `ApiFactory` → passam a usar Postgres (container).
- Testes que instanciam serviços **sobre InMemory** (Application, Infra) → continuam como estão, a menos que você decida migrá-los também.

**Se migrarmos tudo**:

- Todos os que usam persistência precisariam de Postgres (ou de um “modo” configurável).
- Refatoração grande, risco de quebrar testes e de aumentar tempo de suite.

### 1.3 Repositórios e dupla implementação (InMemory + Postgres)

Você já tem **duas** implementações: InMemory e Postgres. Com Testcontainers:

- Os testes de integração passariam a usar **só** os repositórios **Postgres** (e o EF/DbContext).
- InMemory continuaria útil para **unit tests** que mockam repositórios ou usam `InMemoryDataStore` explícito.
- Nada obriga a remover InMemory; o impacto é **adicionar** o uso de Postgres via container nos testes escolhidos.

### 1.4 ConcurrencyTests e DatabaseFixture

Hoje:

- `ConcurrencyTests` usam `DatabaseFixture` e `TEST_DATABASE_CONNECTION_STRING`.
- Estão sempre **pulados** no CI (não há Postgres no workflow).

**Com Testcontainers**:

- Dá para fazer `DatabaseFixture` (ou equivalente) usar um **container** Postgres em vez de um banco externo.
- Os 2 testes de concorrência poderiam **rodar no CI** sem precisar de Postgres hospedado.
- Impacto: ajuste no fixture e nos testes para usar a connection string do container.

---

## 2. Impacto no CI/CD (GitHub Actions)

### 2.1 Docker

- Testcontainers **exige Docker** no ambiente que roda os testes.
- No `ubuntu-latest` do GitHub Actions, o Docker **já está disponível** (serviço incluído na imagem).
- **Impacto**: nenhuma mudança obrigatória de runner; só garantir que o job de testes **não** desative o Docker.

### 2.2 Tempo de build

- **Hoje**: alguns minutos para build + ~1 minuto para os testes (tudo em processo, InMemory).
- **Com Testcontainers**:
  - Pull da imagem Postgres (primeira vez ou quando mudar) + startup do container: na ordem de **5–15 s** por run.
  - Se usar **um container por classe/collection** (ex.: `ICollectionFixture`): 1 startup para dezenas de testes.
  - Se usar **um container por teste**: tempo multiplicado; em geral **não** é recomendado.
- **Impacto**: aumento de **alguns segundos a ~1 minuto** na duração total do CI, dependendo de quantos containers você sobe e de como agrupa os testes.

### 2.3 Comportamento atual do workflow

- O CI **não** configura `Persistence__Provider` nem connection string; a API usa `appsettings` + ambiente.
- O `ApiFactory` **sobrescreve** com `InMemory` para testes.
- Com Testcontainers, o job de testes precisaria:
  - **Ou** usar um factory que sobe o container e configura Postgres (e a connection string),
  - **Ou** definir env vars que o `ApiFactory` usa para escolher Postgres + connection string do container.
- **Impacto**: mudança **só** no projeto de testes e, se fizer sentido, em como o CI chama os testes (ex.: flags, filtros). Nada no código de produção.

### 2.4 Estabilidade

- Containers às vezes falham por rede, disco ou timeout no CI.
- Boas práticas: **retries** para “container failed to start”, **timeouts** adequados, **collection fixture** para reutilizar o mesmo container.
- **Impacto**: um pouco mais de cuidado para evitar flakiness; mas é padrão em projetos que usam Testcontainers em CI.

---

## 3. Benefícios concretos

| Benefício | Descrição |
|-----------|-----------|
| **Resolve o bug de ambiente do DevicesController** | Com Postgres real, `UnitOfWork`/DbContext comportam-se como em produção; o problema de “usuário não encontrado” após login tende a desaparecer. |
| **ConcurrencyTests no CI** | Os 2 testes de concorrência podem rodar sempre, sem Postgres manual. |
| **Menos diferença dev/prod** | Mesmo engine, tipos, constraints, transações. Menos surpresas em produção. |
| **Detecção de bugs de SQL/EF** | Erros de SQL, locks, transações que só aparecem em Postgres passam a ser encontrados nos testes. |
| **Mesma connection string** | Evita caminhos específicos de “test” (ex.: SQLite) que não refletem produção. |

---

## 4. Riscos e custos

| Risco / custo | Descrição |
|---------------|-----------|
| **Docker obrigatório** | Quem roda testes precisa ter Docker (local e CI). |
| **Suite mais lenta** | Startup de container + migrações; ~5–15 s por container, conforme o uso. |
| **Complexidade** | Mais uma camada (Testcontainers, lifecycle, connection string) para manter. |
| **Migração em etapas** | Migrar tudo de uma vez é pesado; fazer por partes exige critério (quais testes mudam primeiro). |
| **Flakiness** | Se mal configurado (ex.: um container por teste, timeouts curtos), risco de falhas intermitentes. |

---

## 5. Estratégias de adoção

### A) Adoção **parcial** (recomendado para começar)

- Manter **InMemory** para a grande maioria dos testes (unit + integração leve).
- Criar **um** `ApiFactoryPostgres` (ou similar) que usa Testcontainers.
- Migrar **só** para Postgres:
  - `DevicesControllerTests` (em especial o teste que falha),
  - `ConcurrencyTests`,
  - eventualmente outros testes de API “críticos” que você queira garantir contra o banco real.
- **Impacto**: baixo na suite atual; ganho onde mais importa (auth, concorrência, etc.).

### B) Adoção **total**

- Todos os testes de integração passam a usar Postgres via Testcontainers.
- **Impacto**: refatoração grande, mais tempo de CI, mais chance de flakiness se não houver padrão (ex.: collection fixtures).

### C) **Híbrido** com flag

- Variável de ambiente, por exemplo `USE_TESTCONTAINERS_POSTGRES=true`.
- Se **ligada**: ApiFactory usa Testcontainers + Postgres.
- Se **desligada**: mantém InMemory (como hoje).
- **Impacto**: mesmo código de testes, comportamento configurável; CI pode usar Postgres, dev local pode escolher.

---

## 6. Exemplo mínimo de alteração (adoção parcial)

```text
1. Adicionar pacote:
   - Testcontainers.PostgreSql

2. Criar ApiFactoryPostgres : WebApplicationFactory<Program>, IAsyncLifetime:
   - InitializeAsync: criar e iniciar PostgreSqlContainer, guardar connection string
   - ConfigureWebHost: Persistence__Provider=Postgres, ConnectionStrings__Postgres=<container>
   - Garantir migrações (ex.: EnsureCreated ou ApplyMigrations)
   - Dispose: parar container

3. DevicesControllerTests (e opcionalmente ConcurrencyTests):
   - Usar IClassFixture<ApiFactoryPostgres> em vez de ApiFactory
   - Ou usar uma collection que compartilha o mesmo container

4. CI:
   - Manter ubuntu-latest (Docker já existe)
   - Rodar esses testes como hoje; não é obrigatório subir Postgres como serviço.
```

---

## 7. Resumo do impacto na solução atual

| Onde | Impacto |
|------|---------|
| **ApiFactory** | Novo factory ou lógica condicional para Postgres + Testcontainers. |
| **InMemory** | Continua para a maioria dos testes; pode ser mantido indefinidamente. |
| **DevicesControllerTests** | Passariam a rodar contra Postgres; tende a resolver o problema de auth/contexto. |
| **ConcurrencyTests** | Deixariam de depender de Postgres externo; rodariam no CI via container. |
| **CI** | Mesmo runner; aumento de tempo de teste na ordem de dezenas de segundos a ~1 minuto, conforme o uso. |
| **Produção** | Nenhum impacto; mudanças ficam restritas ao projeto de testes. |

---

## 8. Conclusão

- **Adotar Testcontainers + PostgreSQL** traz **testes mais realistas**, tende a **eliminar** o problema do `RegisterDevice_WhenValid_CreatesDevice` e permite **rodar os ConcurrencyTests no CI**.
- O **impacto** é **concentrado nos testes e no CI**: novo (ou estendido) factory, Docker, e um pouco mais de tempo de execução.
- A **solução atual** (InMemory, SkippableFact, etc.) **não precisa ser removida**; dá para conviver e migrar **aos poucos**, começando por Devices + Concurrency e, se fizer sentido, ampliando depois.

Se quiser, o próximo passo pode ser um **plano de implementação enxuto** (ex.: PR 1 = ApiFactoryPostgres + 1 teste; PR 2 = DevicesController + Concurrency) com trechos de código adaptados ao seu `Program` e `ServiceCollectionExtensions`.

---

## Onde a estratégia foi incorporada (Fases 13–29)

A estratégia **Testcontainers + PostgreSQL** foi referenciada nas fases do backlog conforme o contexto de cada uma:

| Fase | Onde | Contexto |
|------|------|----------|
| **13** (Emails) | Qualidade | Queue, outbox |
| **14** (Governança/Votação) | Qualidade | Votações, interesses |
| **15** (IA) | Qualidade | Categorização, moderação, persistência |
| **16** (Entregas) | Qualidade | Entregas, marketplace, rotas |
| **17** (Gamificação) | Qualidade | Contribuições, pontos |
| **18** (Saúde Territorial) | Qualidade | Observações, sensores, ações, indicadores |
| **19** (Arquitetura Modular) | §25.4 + Qualidade | **Estratégia estabelecida**; ApiFactoryPostgres, migração parcial |
| **20** (Moeda Territorial) | Qualidade | Moeda, carteiras, transações, fundos — **crítico** |
| **21** (Cripto) | Qualidade | Tradicional + cripto, transações — requisito "Testável" |
| **22** (Integrações) | Qualidade | Postagem cross-platform, assinaturas |
| **23** (Compra Coletiva) | Qualidade | Compra coletiva, marketplace, votações |
| **24** (Trocas) | Qualidade | Trocas, matching, eventos |
| **25** (Hub Serviços Digitais) | Qualidade | Credenciais, consumo, serviços digitais |
| **26** (Chat IA) | Qualidade | Chat IA, consumo por conversa |
| **27** (Negociação Territorial) | Qualidade | Negociação, quotas, subsídios |
| **28** (Banco Sementes) | Qualidade | Banco de sementes, catálogo, WorkQueue |
| **29** (Mobile Avançado) | Qualidade | Analytics mobile, push, deep linking |
