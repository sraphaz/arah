# Validação da Modularização - Araponga

**Data**: 2026-01-28  
**Status**: ✅ Concluída (estrutura base)  
**Tipo**: Documentação Técnica - Validação

---

## 📋 Resumo Executivo

A modularização do backend do Araponga foi **validada e recuperada**. A estrutura de módulos está em uso, o build compila com sucesso e o trabalho pode ser retomado a partir do estado atual.

---

## ✅ O Que Foi Validado e Corrigido

### 1. Erros de Compilação Corrigidos

| Item | Status |
|------|--------|
| Referência `Microsoft.Extensions.Configuration.Abstractions` no `Araponga.Application` | ✅ Adicionada (necessária para `IModule` e `ModuleRegistry`) |
| Chamada inexistente `AddAdditionalPostgresRepositories` | ✅ Substituída por `AddPostgresRepositories` (método privado existente) |
| `ConnectionPoolMetricsService` recebendo `SharedDbContext` | ✅ Ajustado para usar `ArapongaDbContext` temporariamente (compatível com assinatura atual) |

### 2. Módulos Faltantes Criados

Os projetos e classes de módulo abaixo **não existiam** mas eram referenciados em `ServiceCollectionExtensions.cs` e no `.csproj` da API. Foram criados com estrutura mínima (stub):

| Módulo | Projeto | Classe | Observação |
|--------|---------|--------|------------|
| Events | `Araponga.Modules.Events.Infrastructure` | `EventsModule` | Stub; TODO: EventsDbContext quando necessário |
| Map | `Araponga.Modules.Map.Infrastructure` | `MapModule` | Stub; TODO: MapDbContext quando necessário |
| Chat | `Araponga.Modules.Chat.Infrastructure` | `ChatModule` | Stub; TODO: ChatDbContext quando necessário |

### 3. Solution e Referências

- **Araponga.sln**: Incluídos os projetos Feed, Marketplace, Events, Map e Chat (com configurações Debug/Release e NestedProjects).
- **Araponga.Api.csproj**: Já referenciava todos os módulos; nenhuma alteração necessária nas referências.

---

## 📦 Estado Atual dos Módulos

| Módulo | Projeto | Implementação | Observação |
|--------|---------|----------------|------------|
| Feed | `Araponga.Modules.Feed.Infrastructure` | ✅ Completo | FeedDbContext + PostgresFeedRepository |
| Marketplace | `Araponga.Modules.Marketplace.Infrastructure` | ⚠️ Parcial | MarketplaceDbContext; repositórios ainda em Infrastructure principal |
| Events | `Araponga.Modules.Events.Infrastructure` | ✅ Completo | EventsDbContext + 2 repositórios (TerritoryEvent, EventParticipation) |
| Map | `Araponga.Modules.Map.Infrastructure` | ✅ Completo | MapDbContext + 2 repositórios (Map, MapEntityRelation) |
| Chat | `Araponga.Modules.Chat.Infrastructure` | ✅ Completo | ChatDbContext + 4 repositórios (Conversation, Participant, Message, Stats) |
| Subscriptions | `Araponga.Modules.Subscriptions.Infrastructure` | ✅ Completo | SubscriptionsDbContext + 6 repositórios |
| Moderation | `Araponga.Modules.Moderation.Infrastructure` | ✅ Completo | ModerationDbContext + 5 repositórios |
| Notifications | `Araponga.Modules.Notifications.Infrastructure` | ✅ Completo | NotificationsDbContext + 2 repositórios |
| Alerts | `Araponga.Modules.Alerts.Infrastructure` | ✅ Completo | AlertsDbContext + PostgresHealthAlertRepository |
| Assets | `Araponga.Modules.Assets.Infrastructure` | ✅ Completo | AssetsDbContext + 3 repositórios |
| Admin | `Araponga.Modules.Admin.Infrastructure` | 🔲 Stub | Sem DbContext; usa Shared quando necessário |

---

## 🏗️ Infraestrutura Compartilhada

- **Araponga.Infrastructure.Shared**: `SharedDbContext` e entidades compartilhadas (Users, Territories, Memberships, etc.) já existentes e em uso.
- **Araponga.Infrastructure**: Continua com a maioria dos repositórios Postgres e `ArapongaDbContext`; `AddPostgresRepositories` registra esses repositórios quando `Persistence:Provider = Postgres`.
- **Compatibilidade**: `ArapongaDbContext` permanece registrado “temporariamente” para compatibilidade (comentário no código: “será removido na Fase 6”).

---

## ⚠️ Avisos Conhecidos (Build)

- **NU1603**: `Microsoft.Extensions.Caching.Memory` 8.0.11 não encontrado; resolvido para 9.0.0. Pode ser alinhado fixando a versão ou atualizando o pacote no Application.
- **CS0105**: `using Araponga.Application.Interfaces` duplicado em `ServiceCollectionExtensions.cs` (pode ser removido um dos usings).
- **CS8601**: Possíveis atribuições nulas em controllers de Subscriptions/Admin; não bloqueiam build.

---

## 📚 Documentação Criada

Durante a validação e recuperação, foram criados os seguintes documentos:

1. **`docs/VALIDACAO_MODULARIZACAO.md`** (este documento): Resumo da validação e estado atual
2. **`docs/MAPA_REPOSITORIOS_MODULOS.md`**: Mapeamento completo de quais repositórios pertencem a quais módulos
3. **`docs/PLANO_MIGRACAO_MODULOS.md`**: Plano detalhado de como migrar repositórios para os módulos, seguindo o padrão do FeedModule

## 📐 O que falta para a modularização completa?

### Estado por camada

| Camada | Estado | Observação |
|--------|--------|------------|
| **Infrastructure** | ✅ **Modularizada** | Cada módulo tem projeto próprio (Araponga.Modules.X.Infrastructure), DbContext e repositórios; slices removidos da infra central. |
| **Domain** | ✅ **Rico por pastas** | Um único projeto (`Araponga.Domain`) com pastas por domínio (Feed/, Chat/, Events/, Map/, Marketplace/, Moderation/, Subscriptions/, etc.). Entidades e value objects organizados; não há projetos separados por módulo. |
| **Application** | ⚠️ **Rico, não modularizado** | Um único projeto; serviços e interfaces em lista quase plana (ex.: FeedService, EventsService, ChatService no mesmo nível). Poucas subpastas (Media/, Notifications/, Users/). Comportamento por domínio existe, mas não há pastas por módulo (ex.: Application/Services/Feed/, Application/Interfaces/Feed/). |
| **API** | ⚠️ **Por feature, não por módulo** | Controllers em uma pasta (FeedController, EventsController, etc.); não agrupados em Api/Controllers/Feed/, Api/Controllers/Events/, etc. |

### O que “modularização completa” pode incluir (opcional)

**Checklist fronteiras para split:** Infra ✅ (projeto por módulo). Domain ✅ (pastas por domínio). Application ❌ (services/interfaces em lista plana). API ❌ (controllers em uma pasta). Para split futuro sem "caçar" arquivos, falta organizar Application e API em pastas por módulo.

1. **Application organizada por módulo** (pastas, sem obrigação de novos projetos):
   - `Application/Services/Feed/`, `Application/Services/Events/`, etc.
   - `Application/Interfaces/Feed/`, `Application/Interfaces/Events/`, etc.
   - Facilita navegação e ownership por domínio.

2. **API organizada por módulo** (pastas):
   - `Api/Controllers/Feed/`, `Api/Controllers/Events/`, etc.
   - Opcional; impacto principalmente de organização.

3. **Domain em projetos separados** (ex.: Araponga.Domain.Feed, Araponga.Domain.Events):
   - Não é obrigatório; um Domain único com pastas evita dependências circulares e já deixa os domínios ricos e claros.
   - Só faz sentido se a solução evoluir para deploy ou versionamento independente por módulo.

4. **Pendências técnicas**:
   - **Financial**: 8 repositórios ainda em Infrastructure (a decidir: manter central ou módulo Finance/Marketplace).
   - **Admin**: módulo stub; sem DbContext próprio.
   - **ConnectionPoolMetricsService**: ainda usa `ArapongaDbContext`; refatorar para DbContext genérico ou interface.
   - **Migrations**: ainda no `Araponga.Infrastructure`; `ArapongaDbContext` mantém todos os DbSets (compatibilidade e banco único). Migrations por módulo seriam uma evolução futura.

### Os domínios estão ricos e modularizados?

- **Sim, ricos**: Domain tem entidades e value objects por bounded context (Feed, Events, Map, Chat, Marketplace, Moderation, Subscriptions, etc.). Application tem serviços e interfaces para cada um desses domínios.
- **Modularizados em camada de persistência**: Infrastructure está modularizada (um projeto por módulo, com DbContext e repositórios).
- **Não modularizados em Domain/Application/API**: Domain e Application são projetos únicos organizados por pastas; API é um projeto único com controllers por feature. Ou seja: domínios são ricos e bem delimitados em conteúdo; a “modularização completa” no sentido de **estrutura de pastas/projetos por módulo** está feita só na Infrastructure; Domain e Application continuam monolíticos por assembly, mas organizados por domínio em pastas.

**Decisão de arquitetura:** Isolamento real (rede, deploy, falhas independentes) exige serviços separados. Para o tamanho do projeto, o alvo é **infraestrutura independente por módulo** — isolando pontos de manutenção e de falha dentro do mesmo processo. Ver `docs/TECNICO_MODULARIZACAO.md` (seção "Isolamento: infraestrutura independente").

---

## 🎯 Próximos Passos Sugeridos (Recuperar Trabalho)

1. **Migração gradual**: Seguir `docs/PLANO_MIGRACAO_MODULOS.md` para migrar repositórios módulo por módulo, começando por Chat, Events e Map (Fase 1).
2. **Documentação**: Manter `docs/TECNICO_MODULARIZACAO.md` alinhado com a lista de módulos e com o que já tem DbContext vs stub.
3. **Feature flags e dependências**: Quando houver tempo, implementar validação de dependências entre módulos e feature flags conforme `TECNICO_MODULARIZACAO.md`.
4. **ConnectionPoolMetricsService**: Refatorar para aceitar `DbContext` genérico (ou interface) para poder usar `SharedDbContext` no futuro e remover dependência de `ArapongaDbContext`.
5. **Limpeza**: Remover `using` duplicado em `ServiceCollectionExtensions.cs` e, se desejado, tratar avisos CS8601 nos controllers.

---

## ✅ Conclusão

- **Conclusão da modularização em curso**: A **estrutura** da modularização está concluída (interfaces, registry, 11 módulos de infraestrutura, integração na API e na solution). Todos os módulos (Feed, Chat, Events, Map, Marketplace, Subscriptions, Moderation, Notifications, Alerts, Assets) possuem DbContext e repositórios próprios e são registrados exclusivamente pelos módulos; `AddPostgresRepositories` não sobrescreve mais nenhum deles (ex.: `IFeedRepository` foi removido de `AddPostgresRepositories` em 2026-02-02).
- **Slices da infra**: Em 2026-02-02 foram **removidos** da `Araponga.Infrastructure/Postgres` os 38 arquivos de repositórios duplicados (Feed, Chat, Events, Map, Alerts, Moderation, Notifications, Subscriptions, Marketplace, Assets). A infra central ficou apenas com repositórios Shared (Territory, User, Membership, JoinRequest, PostGeoAnchor, PostAsset, FeatureFlag, Audit, Financial, Policies, Media, etc.). Build ok após remoção.
- **Trabalho recuperado**: Build ok, todos os módulos referenciados existem e são carregados via `ModuleRegistry`. O projeto Araponga está em estado consistente para continuar o desenvolvimento.

**Última atualização**: 2026-02-02
