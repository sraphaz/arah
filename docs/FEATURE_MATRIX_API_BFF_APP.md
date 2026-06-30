# Matriz de funcionalidades: API ↔ BFF ↔ App

Este documento mapeia **o que existe na API**, **o que o BFF expõe** e **o que o app Flutter usa hoje**, para alinhar o desenvolvimento e chegar a um estado em que tudo que está na API esteja disponível no BFF e no app antes de evoluir com novas fases.

---

## Índice

1. [Objetivo e estratégia](#objetivo-e-estratégia)
2. [Tabela de alinhamento (API | BFF | App)](#tabela-de-alinhamento-api--bff--app)
3. [O que desenvolver a seguir](#o-que-desenvolver-a-seguir)
4. [Documentos a revisar (app já existe)](#documentos-a-revisar-app-já-existe)
5. [Referências](#referências)

---

## Objetivo e estratégia

- **Objetivo**: Ter visibilidade clara do que está implementado em cada camada (API, BFF, App) e do que falta para ficar alinhado.
- **Estratégia**:
  1. **Fase de alinhamento**: Garantir que toda funcionalidade que está na **API** esteja exposta no **BFF** e, quando fizer sentido para o usuário final, no **App**.
  2. **Fase de evolução**: Depois de alinhado, desenvolver novas funcionalidades seguindo as fases do backlog (API → BFF → App).

Assim evitamos “API cheia, BFF e app vazios” e conseguimos um produto significativo em cada camada.

---

## Tabela de alinhamento (API | BFF | App)

Legenda:

- **✅** Implementado e em uso (ou exposto de forma estável).
- **🚧** Parcialmente implementado / só leitura ou só uma parte dos endpoints.
- **❌** Não implementado ou não usado no app.
- **➖** Uso administrativo; fora do escopo do app usuário.

O **BFF** expõe tudo sob `/api/v2/journeys/<jornada>/<path>` e faz proxy para a API (v1 ou v2/journeys). A coluna BFF indica se a jornada está **registrada e roteada** no BFF; a coluna **App** indica se o app Flutter **chama** essa jornada.

| Funcionalidade (jornada) | API | BFF | App | Observações |
|--------------------------|-----|-----|-----|-------------|
| **Auth** (check-email, signup, login, social, refresh, logout) | ✅ | ✅ | ✅ | App: check-email, signup, login, Google social, refresh. |
| **Onboarding** (suggested-territories, complete) | ✅ | ✅ | ✅ | App: lista sugeridos, completa com território selecionado. |
| **Territories** (listar, paged, **search**, detalhe, enter, chat/channels) | ✅ | ✅ | ✅ | App: paged (explorar), **busca por nome/cidade** (`territories/search`), get by id (mapa), enter, canais de chat. |
| **Feed** (territory-feed, create-post, interact, post-comments, delete-post) | ✅ | ✅ | ✅ | App: feed paginado, like/comment/share, thread, mídia, excluir, filtros. |
| **Events** (territory-events, create-event, participate) | ✅ | ✅ | ✅ | App: lista eventos, **criar evento** (título, descrição, início/término, local), participar (interesse/confirmado). |
| **Map** (pins, entities) | ✅ | ✅ | ✅ | App: GET map/pins para exibir pins no mapa; **toque no pin abre detalhes (deep-link)** para eventos/assets/alertas/feed. |
| **Me** (profile, preferences, interests, devices) | ✅ | ✅ | ✅ | App: profile, interests, preferences, registro automático de device. |
| **Notifications** (listar paginado, marcar lida) | ✅ | ✅ | ✅ | App: notifications/paged, notifications/{id}/read. |
| **Membership** (me, become-resident, verify-residency) | ✅ | ✅ | ✅ | App: tela Membership com status, solicitar residência e verificação geo. |
| **Connections** (listar, pending, request, accept, reject, privacy) | ✅ | ✅ | ✅ | App: Conexões com busca, sugestões, solicitar/aceitar/rejeitar. |
| **Assets** (listar, upload, curate) | ✅ | ✅ | ✅ | App: listagem, criação, validar, arquivar e curar (aprovar/rejeitar). |
| **Media** (upload, info, download) | ✅ | ✅ | ✅ | App: upload ao publicar post; exibição no feed. |
| **Marketplace** (search, add-to-cart, checkout) | ✅ | ✅ | ✅ | App: busca, adicionar ao carrinho, checkout (v2 + cart v1). |
| **Marketplace V1** (cart, stores, items) | ✅ | ✅ | ✅ | App: cart v1, busca/checkout v2 e gestão de loja própria (`stores/me`, criar/atualizar). |
| **Subscription plans / Subscriptions** | ✅ | ✅ | ✅ | App: planos, minha assinatura, assinar e cancelar. |
| **Moderation** (work-items, cases, evidences) | ✅ | ✅ | ✅ | App: fila, casos (decidir), evidências (download + decidir residência). |
| **Governance** (votings: listar, criar, votar, fechar, resultados) | ✅ | ✅ | ✅ | App: tela Governança (jornada `governance`) — lista votações com filtro de status, votar inline, ver resultados e criar votação. |
| **Chat** (conversations, messages, participants) | ✅ | ✅ | ✅ | App: canais, grupos (criar), mensagens e envio. |
| **Alerts** (listar, criar) | ✅ | ✅ | ✅ | App: listagem e criação de alertas. |
| **Admin** (seed, cache-metrics, configs) | ✅ | ✅ | ➖ | Uso administrativo; não no app usuário. |

Resumo rápido:

- **Totalmente alinhados (API + BFF + App em uso)**: todas as jornadas do app usuário, exceto Admin (➖).
- **Parcialmente no app**: nenhuma pendência crítica na matriz base.
- **Fora do app usuário**: Admin.

---

## O que desenvolver a seguir

Prioridade sugerida para **evoluir** com novas fases do backlog (API → BFF → App).

### No App (Flutter)

| Prioridade | Funcionalidade | O que fazer |
|------------|----------------|-------------|
| — | _Alinhamento base concluído_ | Próximas entregas seguem fases do backlog em `docs/backlog-api/`. |

### No BFF

- Endpoints de chat por território (`territories/{id}/chat/channels|groups`) registrados no BFF.
- Manter registry sincronizado com novos endpoints da API.

### Na API

- Novas fases do backlog: implementar na API primeiro, depois BFF, depois app.

---

## Documentos a revisar (app já existe)

> **App Flutter**: Existe uma versão estável do app (auth + **login com Google na UI**, onboarding, feed, mapa com **deep-links nos pins**, eventos com **criação**, **governança/votações**, perfil, notificações, publicar, marketplace, chat, membership, alertas). Ver [Release estável – App e Onboarding](./STABLE_RELEASE_APP_ONBOARDING.md) e esta matriz.

---

## Referências

- **Registro de jornadas BFF**: `backend/Arah.Api.Bff/Journeys/BffJourneyRegistry.cs`
- **App – repositórios que chamam o BFF**: `frontend/arah.app/lib/features/*/data/repositories/*.dart`
- **Release estável e getting started**: [STABLE_RELEASE_APP_ONBOARDING.md](./STABLE_RELEASE_APP_ONBOARDING.md)
- **Planejamento do frontend Flutter**: [24_FLUTTER_FRONTEND_PLAN.md](./24_FLUTTER_FRONTEND_PLAN.md)
- **Backlog e fases**: [docs/backlog-api/](./backlog-api/)
