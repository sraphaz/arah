# Melhorias, erros conhecidos e problemas comuns

Este documento reúne **melhorias observáveis** na estruturação do projeto e na organização dos módulos e camadas, **erros conhecidos** (ou riscos já identificados nos ADRs) e **problemas comuns** de diferentes abordagens (modular monolith, shared kernel, vertical slices, etc.), com foco em decisões práticas para o Araponga.

---

## 1. Estrutura e organização

### 1.1 Inconsistência na estrutura física dos módulos

**Observação:** Alguns módulos têm **pasta aninhada** (ex.: `Araponga.Modules.Feed/Araponga.Modules.Feed/` com o .csproj dentro), outros têm o .csproj na **raiz** do módulo (ex.: `Araponga.Modules.Connections/Araponga.Modules.Connections.csproj`).

**Problema:** Referências e caminhos na solution e nos .csproj ficam diferentes (ex.: `..\Araponga.Modules.Feed\Araponga.Modules.Feed\Araponga.Modules.Feed.csproj` vs `..\Araponga.Modules.Connections\Araponga.Modules.Connections.csproj`). Aumenta chance de erro ao adicionar novos módulos e dificulta onboarding.

**Melhoria proposta:** Padronizar **um único padrão** para todos os módulos:
- **Opção A (recomendada):** Um nível só — `Araponga.Modules.<Nome>/` contém o .csproj e as pastas Domain/, Application/, Infrastructure/ (como Connections hoje). Migrar Feed, Map, Marketplace, etc. movendo o conteúdo da subpasta interna para a raiz do módulo e removendo a pasta duplicada.
- **Opção B:** Manter o aninhamento em todos (menos alinhado ao “um projeto por módulo” na raiz do backend).

**Erro conhecido:** Quem adiciona um novo módulo pode copiar Feed (aninhado) ou Connections (flat) e aumentar a inconsistência. Documentar no README do backend ou em CONTRIBUTING qual padrão usar.

---

### 1.2 Application como “hub” de todos os módulos

**Observação:** `Araponga.Application` referencia **todos** os projetos de módulos (Feed, Events, Map, Marketplace, Moderation, Connections, etc.). A camada de aplicação central conhece todos os contextos.

**Problemas comuns dessa abordagem:**
- **Acoplamento em compile-time:** Qualquer mudança de contrato em um módulo pode exigir recompilar Application e todos os consumidores.
- **Application “gorda”:** Muitos serviços e orquestrações em um único projeto; cresce com cada novo módulo.
- **Difícil extrair módulo para outro processo:** Se no futuro um módulo virar um serviço (ex.: microserviço), Application continuará referenciando os outros; a fronteira não é por “caso de uso” e sim por “módulo técnico”.

**Melhorias propostas (incremental):**
- **Curto prazo:** Manter como está, mas **evitar** que novos *serviços* transversais dependam de N módulos quando um único evento ou contrato bastar (preferir eventos de domínio ou interfaces em Application.Abstractions).
- **Médio prazo:** Identificar “jornadas” ou use cases que orquestram vários módulos e documentá-los (já há controllers Journeys/); considerar que a orquestração pode viver na API ou em um projeto de “Application.UseCases” que referencia só as interfaces necessárias, em vez de todos os módulos.
- **Longo prazo (opcional):** Se a solução crescer muito, avaliar “vertical slices” por jornada (cada slice com sua Application reduzida) ou BFF como único orquestrador, com módulos expondo apenas APIs internas.

**Não é necessariamente um erro:** Para um modular monolith, Application como orquestrador central é um padrão válido. O risco é o crescimento descontrolado e a falta de fronteiras por domínio de uso.

---

### 1.3 Domain “compartilhado” muito grande

**Observação:** `Araponga.Domain` contém Users, Territories, Membership, Governance, Geo, Media, Email, Configuration, **Financial** (transações, plataforma), **Policies**, **Social** (JoinRequests), etc.

**Problemas comuns:**
- **Shared kernel inchado:** Tudo que “mais de um módulo usa” tende a ir para o Domain. Com o tempo, o núcleo vira um “monte de entidades” e perde coerência.
- **Domain anêmico:** Se as entidades forem só DTOs com getters/setters e a lógica estiver em Application, o Domain vira apenas um modelo de dados compartilhado (comum em CRUD); não é um “erro” por si, mas vale documentar a opção (rich domain vs anêmico).
- **Bordas pouco claras:** Financial, Media e partes de Social poderiam ser módulos (ex.: Araponga.Modules.Billing, Araponga.Modules.Media) se crescerem em regras; hoje estão no núcleo.

**Melhorias propostas:**
- **Documentar** no BACKEND_LAYERS_AND_NAMING ou em um ADR o critério: “Está no Domain se é identidade/contexto universal (User, Territory, Membership, políticas globais, geo). O que é contexto de negócio com regras próprias tende a módulo.”
- **Revisão periódica:** A cada novo “bloco” (ex.: billing avançado, mídia com processamento), avaliar se permanece no Domain ou vira módulo.
- **Não mover por mover:** Migrar Financial ou Media para módulo só se houver ganho claro (ownership, deploy independente, testes isolados); senão, manter no Domain e apenas documentar.

---

### 1.4 Duplicação de configuração de entidades em dois DbContexts

**Resolução aplicada:** As entidades foram **removidas do modelo** do `ArapongaDbContext` (DbSets e configuração em `OnModelCreating`). Em runtime os repositórios Feed usam apenas **FeedDbContext**; ver ADR-017. As tabelas `post_geo_anchors` e `post_assets` continuam no banco (usadas pelo Feed). Ao gerar a próxima migração do `ArapongaDbContext`, **remover do método `Up()`** as chamadas `DropTable` para essas duas tabelas.

**Risco:** Manutenção duplicada; alguém pode alterar um contexto e esquecer o outro; confusão sobre “fonte da verdade”.

**Melhoria proposta:** Em uma janela de manutenção (ex.: próxima migração grande), criar uma migração no Core que **remova** essas entidades do snapshot do `ArapongaDbContext` (e do modelo EF, se ainda estiverem configuradas lá), desde que nenhum código no Core as use. Documentar no ADR que “repositórios Feed usam apenas FeedDbContext”.

---

## 2. Camadas e dependências

### 2.1 Infrastructure “principal” vs Infrastructure.Shared

**Observação:** Dois projetos de infraestrutura: um “geral” (vários DbContexts, e-mail, cache, workers) e outro “shared” (SharedDbContext para User, Territory, Membership, etc.).

**Problema comum:** Novos desenvolvedores podem não saber onde colocar um novo repositório (Infrastructure vs Infrastructure.Shared). A regra é: “repositórios das entidades do Domain compartilhado (User, Territory, Membership, Policies, etc.) → Infrastructure.Shared; resto (módulos ou cross-cutting que não é núcleo) → Infrastructure ou no próprio módulo”.

**Melhoria proposta:** No BACKEND_LAYERS_AND_NAMING ou no README do backend, acrescentar uma **regra de decisão** em uma linha: “Novo repositório para entidade do Araponga.Domain (núcleo) → Infrastructure.Shared; para entidade de módulo → no módulo; para outro cross-cutting → Infrastructure.”

---

### 2.2 Registro de serviços (API vs IModule)

**Assimetria conhecida (ADR-017):** A maioria dos repositórios de módulos é registrada via `IModule.RegisterServices`; alguns provedores ou repositórios foram registrados na API (ex.: `IAcceptedConnectionsProvider`). O ADR considera aceitável que “provedores de aplicação” fiquem no Core.

**Risco:** Proliferação de registros na API torna o bootstrap da API pesado e pouco coerente (parte no módulo, parte na API).

**Melhoria proposta:** Convenção explícita: “Repositórios e implementações de persistência → sempre no módulo (ou Infrastructure.Shared). Provedores de aplicação (como IAcceptedConnectionsProvider) que dependem de Application podem ser registrados na API ou em um extension method do Core chamado pela API.” Documentar essa convenção e revisar periodicamente se algo novo foi registrado na API sem necessidade.

---

### 2.3 Dependência entre módulos (Feed → Map)

**Exceção documentada:** Feed referencia Map (tipos de geo para itens do feed). É a única dependência explícita entre módulos.

**Problemas comuns:**
- **Grafos de dependência:** Quando um módulo A referencia B, no futuro B não pode referenciar A sem ciclo. Com muitos módulos, o grafo pode ficar difícil de manter.
- **Deploy:** Se Map e Feed forem extraídos para serviços separados, a dependência Feed → Map vira uma chamada entre serviços; precisa estar bem definida (API estável, versionamento).

**Melhorias propostas:**
- Manter **uma única** dependência entre módulos e documentá-la (já feito). Evitar novas dependências diretas; preferir eventos ou tipos no Domain compartilhado (ex.: um “GeoAnchor” ou “GeoReference” no Domain) se outro módulo precisar do mesmo conceito.
- Se mais módulos precisarem de “geo para conteúdo”, considerar mover o tipo compartilhado para `Araponga.Domain` (Geo ou novo subdomínio) e Feed/Map referenciarem apenas o Domain.

---

## 3. Testes

### 3.1 Assimetria de testes por módulo

**Observação (ADR-019):** Alguns módulos têm projeto de teste dedicado (Connections, Map, Marketplace, Moderation, Subscriptions); Feed, Events, Notifications, Chat, Alerts não têm. Integração de API do Connections está no Core (Araponga.Tests); Subscriptions tem integração no próprio projeto de teste.

**Problemas comuns:**
- Cobertura desigual: mudanças em Feed ou Events são testadas apenas indiretamente via testes do Core.
- Dificuldade de “testar só o módulo X” quando não existe Araponga.Tests.Modules.X.

**Melhorias propostas:**
- **Aplicado:** README dos testes atualizado; testes Moderation Application movidos para Tests.Modules.Moderation; testes Marketplace Application (Cart, Store, Inquiry, Rating, PlatformFee, SellerPayout, MarketplaceSearch, MarketplaceService, TerritoryPayoutConfig) movidos para Tests.Modules.Marketplace. `CacheTestHelper` e `PatternAwareTestCacheService` consolidados em `Araponga.Tests.Shared/TestHelpers` para reuso entre Core e módulos. Ver TEST_SEPARATION_BY_MODULE.md. **Opcional:** Criar Araponga.Tests.Modules.Feed, .Events quando houver demanda (ex.: regras de negócio complexas no módulo, time dedicado ao módulo). Não é obrigatório ter projeto de teste por módulo.
- **Documentar** no README dos testes: “Módulos sem projeto de teste são cobertos por Araponga.Tests (integração e serviços). Para adicionar testes específicos do módulo, criar Araponga.Tests.Modules.<Nome> seguindo o padrão de Connections/Subscriptions.”
- **Convenção de nomenclatura:** Deixar explícito no README: *IntegrationTests = fluxos que cruzam vários endpoints/serviços; *ControllerTests = foco em um controller; *EdgeCasesTests = cenários de borda (Domain/Application).

---

### 3.2 ApiFactory e helpers duplicados

**Observação:** Subscriptions mantém ApiFactory e helper de login próprios para não referenciar Araponga.Tests (evitar dependências pesadas). Core usa AuthTestHelper centralizado.

**Risco:** Duas implementações podem divergir (ex.: mudança em JWT ou session no Core e o factory do Subscriptions não atualizado).

**Melhoria proposta:** Já documentado no ADR — manter sincronia de configuração (JWT, RateLimiting, Persistence). Opcional: extrair um pacote ou projeto “Araponga.Tests.ApiSupport” com apenas ApiFactory base e helpers de auth, referenciado por Araponga.Tests e por Araponga.Tests.Modules.Subscriptions, para reduzir duplicação sem puxar todo o Araponga.Tests.

---

## 4. Problemas comuns de abordagens alternativas

### 4.1 Modular monolith (abordagem atual)

**Vantagens:** Fronteiras claras por módulo; preparação para eventual extração de serviços; um deploy só.

**Problemas comuns:**
- **Boundaries apenas no código:** Se a equipe não respeitar “módulo X não chama diretamente tipo Y do módulo Z”, as fronteiras se corroem. Mitigação: code review, documentação e, se necessário, analyzers (ex.: módulo A não pode referenciar módulo B).
- **Database compartilhada:** Vários módulos usam o mesmo Postgres. Transações que cruzam módulos (ex.: checkout + subscription) exigem coordenação (UoW composto, como hoje). Risco: transações longas e acoplamento por dados.
- **Escalabilidade:** Escala vertical (um processo). Para escalar um módulo específico (ex.: feed muito acessado), seria preciso extrair para outro serviço; até lá, o monolith atende.

**Recomendação:** Manter o modular monolith; reforçar convenções (dependências, registro de serviços, onde colocar novos repositórios) na documentação e em revisões.

---

### 4.2 Shared kernel (Domain + Infrastructure.Shared)

**Uso atual:** Domain e Infrastructure.Shared formam o “núcleo compartilhado”.

**Problemas comuns:**
- **Kernel que cresce:** Tudo que é “usado em dois lugares” tende a ir para o kernel. Resultado: Domain e Infrastructure.Shared inchados e frágeis.
- **Mudanças quebram todos:** Alteração em User ou Territory impacta todos os módulos. Exige disciplina de versionamento e compatibilidade.

**Recomendação:** Critério explícito para “o que entra no Domain” (identidade, território, membership, políticas globais, geo). O que for contexto de negócio com regras próprias tende a módulo. Revisar periodicamente.

---

### 4.3 Vertical slices (não adotado)

**Ideia:** Organizar por “caso de uso” (ex.: CreatePost, JoinTerritory), cada slice com seu handler, DTOs e persistência, em vez de camadas horizontais (todos os controllers, todos os serviços).

**Problemas comuns quando se mistura com modular:**
- Duplicação de conceitos entre slices (ex.: “User” em cada slice vs um Domain compartilhado).
- Dificuldade de reuso (ex.: “preciso do mesmo repositório em duas slices”).

**Recomendação:** Não migrar para vertical slices agora. O atual “módulo por contexto de negócio” já dá uma espécie de slice (Feed, Map, Marketplace). Se no futuro houver muitas “jornadas” cross-module, considerar pastas ou projetos por jornada (ex.: OnboardingJourney, MarketplaceJourney) mantendo os módulos como estão.

---

### 4.4 Um projeto por módulo vs três (Domain/Application/Infrastructure)

**Estado atual:** Um projeto por módulo (pastas Domain/, Application/, Infrastructure/ dentro do projeto).

**Problema comum da opção “um projeto”:** A separação de camadas não é enforced por assembly; em tese, Domain poderia referenciar Infrastructure no mesmo projeto. Mitigação: convenção, code review e, se necessário, analyzer (ex.: arquivos em Domain/ não podem usar tipos de Infrastructure/).

**Vantagem:** Menos projetos na solution; referências mais simples; módulo continua sendo a unidade de evolução.

**Recomendação:** Manter um projeto por módulo. Script de verificação: `backend/scripts/validate-module-boundaries.ps1` (falha se Domain referenciar Infrastructure no mesmo módulo; pode ser executado em CI).

---

## 5. Resumo de ações sugeridas (priorizado)

| Prioridade | Ação | Impacto |
|------------|------|---------|
| Alta | ~~Padronizar estrutura física dos módulos e documentar no README~~ **Feito:** backend/README.md (estrutura flat) | Reduz erros de referência e onboarding |
| Alta | ~~Documentar regra de decisão: “Onde colocar novo repositório?” ~~ **Feito:** BACKEND_LAYERS_AND_NAMING.md, seção Regras de decisão | Clareza e consistência |
| Média | ~~Documentar convenção de registro de serviços~~ **Feito:** BACKEND_LAYERS_AND_NAMING.md | Evita proliferação de registros na API |
| Média | ~~Revisar remoção de PostGeoAnchor/PostAsset do ArapongaDbContext~~ **Feito:** entidades removidas do modelo; ao gerar próxima migração, remover do `Up()` os `DropTable` dessas tabelas | Remove duplicação e confusão |
| Média | ~~Mover testes Application de Marketplace para Tests.Modules.Marketplace; consolidar CacheTestHelper em Tests.Shared~~ **Feito** | Reduz concentração no Core e reuso de helpers |
| Baixa | ~~Critério explícito para “o que entra no Domain” (identidade, território, políticas; o resto tende a módulo) ~~ **Feito:** BACKEND_LAYERS_AND_NAMING.md, seção "O que entra no Araponga.Domain" | Evita kernel inchado |
| Baixa | Opcional: analyzer ou regra de CI para “Domain do módulo não referencia Infrastructure do mesmo projeto” | Reforça fronteiras |
| Baixa | ~~Opcional: projeto Araponga.Tests.ApiSupport~~ **Feito:** BaseApiFactory + AuthTestHelper compartilhados por Araponga.Tests e Subscriptions | Reduz duplicação de testes |

---

## 6. Referências

- [BACKEND_LAYERS_AND_NAMING.md](BACKEND_LAYERS_AND_NAMING.md) — camadas e nomenclatura
- [backend/README.md](../README.md) — estrutura e regras de dependência
- [docs/10_ARCHITECTURE_DECISIONS.md](../../docs/10_ARCHITECTURE_DECISIONS.md) — ADRs (ADR-012 a ADR-019 em especial)
