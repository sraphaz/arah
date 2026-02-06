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

O **BFF** expõe tudo sob `/api/v2/journeys/<jornada>/<path>` e faz proxy para a API (v1 ou v2/journeys). A coluna BFF indica se a jornada está **registrada e roteada** no BFF; a coluna **App** indica se o app Flutter **chama** essa jornada.

| Funcionalidade (jornada) | API | BFF | App | Observações |
|--------------------------|-----|-----|-----|-------------|
| **Auth** (check-email, signup, login, social, refresh, logout) | ✅ | ✅ | ✅ | App: check-email, signup, login, Google social, refresh. |
| **Onboarding** (suggested-territories, complete) | ✅ | ✅ | ✅ | App: lista sugeridos, completa com território selecionado. |
| **Territories** (listar, paged, detalhe, enter) | ✅ | ✅ | ✅ | App: paged (explorar), get by id (mapa), enter (trocar território). |
| **Feed** (territory-feed, create-post, interact) | ✅ | ✅ | 🚧 | App: feed paginado + create-post. **Falta**: interact (like, comentar, compartilhar). |
| **Events** (territory-events, participate) | ✅ | ✅ | ✅ | App: lista eventos, participar (interesse/confirmado). |
| **Map** (pins, entities) | ✅ | ✅ | ✅ | App: GET map/pins para exibir pins no mapa. |
| **Me** (profile, preferences, interests, devices) | ✅ | ✅ | 🚧 | App: profile (GET/PUT displayName, bio), interests (GET/POST/DELETE), preferences (GET/PUT notificações). Devices e outros sub-recursos de me não usados. |
| **Notifications** (listar paginado, marcar lida) | ✅ | ✅ | ✅ | App: notifications/paged, notifications/{id}/read. |
| **Membership** (me, become-resident, verify-residency) | ✅ | ✅ | ❌ | App não chama; onboarding usa journey onboarding/complete que associa território à sessão. |
| **Connections** (listar, pending, request, accept, reject, privacy) | ✅ | ✅ | ❌ | Círculo de amigos: API e BFF prontos; app ainda não tem tela. |
| **Assets** (listar, upload, curate) | ✅ | ✅ | ❌ | App não usa upload/lista de assets. |
| **Media** (upload, info, download) | ✅ | ✅ | ❌ | App não usa; posts sem mídia no app ainda. |
| **Marketplace** (search, add-to-cart, checkout) | ✅ | ✅ | ❌ | Jornada v2; app não tem marketplace. |
| **Marketplace V1** (cart, stores, items) | ✅ | ✅ | ❌ | Carrinho, lojas, itens; app não usa. |
| **Subscription plans / Subscriptions** | ✅ | ✅ | ❌ | App não exibe planos nem assinaturas. |
| **Moderation** (work-items, cases, evidences) | ✅ | ✅ | ❌ | App não tem área de moderação. |
| **Chat** (conversations, messages, participants) | ✅ | ✅ | ❌ | App não tem chat. |
| **Alerts** (listar, criar) | ✅ | ✅ | ❌ | App não lista nem cria alertas. |
| **Admin** (seed, cache-metrics, configs) | ✅ | ✅ | ❌ | Uso administrativo; não no app usuário. |

Resumo rápido:

- **Totalmente alinhados (API + BFF + App em uso)**: Auth, Onboarding, Territories, Events, Map, Notifications.
- **Parcialmente no app**: Feed (falta interact), Me (falta devices e outros sub-recursos se necessário).
- **Só API + BFF**: Membership, Connections, Assets, Media, Marketplace, Subscriptions, Moderation, Chat, Alerts, Admin.

---

## O que desenvolver a seguir

Prioridade sugerida para **alinhar** e depois **evoluir**.

### No App (Flutter)

| Prioridade | Funcionalidade | O que fazer |
|------------|----------------|-------------|
| Alta | **Feed – interações** | Usar BFF `feed/interact` (like, comentar, compartilhar); exibir contadores e comentários no feed. |
| Alta | **Mídia nos posts** | Upload de mídia (BFF media ou assets), associar a create-post; exibir imagens nos posts. |
| Alta | **Excluir próprio post** | Endpoint na API/BFF se ainda não existir; botão “Excluir” no app para posts do usuário. |
| Média | **Conexões (círculo de amigos)** | Telas: listar conexões, pendentes, enviar/aceitar/rejeitar; usar BFF connections/*. |
| Média | **Filtros do feed** | Filtro por tipo/tag no app; BFF já suporta filterByInterests e parâmetros no territory-feed. |
| Média | **Preferências no perfil** | Já existe me/preferences no app (notificações); expandir para interesses visíveis e “tipo de post preferido” se a API suportar. |
| Média | **Tipo de post ao publicar** | UI para escolher tipo (geral, alerta, evento) ao criar post; create-post já aceita tipo. |
| Baixa | **Membership** | Tela “Sou morador” / become-resident onde fizer sentido no fluxo. |
| Baixa | **Alertas** | Listar e exibir alertas do território (BFF alerts). |
| Futuro | **Marketplace** | Lojas, listagens, carrinho, checkout no app (jornadas marketplace e marketplace-v1). |
| Futuro | **Chat** | Conversas e mensagens (BFF chat). |

### No BFF

- Garantir que **todos** os endpoints usados pelo app existam e estejam mapeados no `BffJourneyRegistry` (já está coberto para as jornadas acima).
- Se a API tiver novos endpoints (ex.: delete post, feed/interact com mais opções), expor na mesma jornada no BFF.

### Na API

- **Feed**: Endpoint para **excluir post** (do próprio usuário) se ainda não existir; manter feed/interact (like, comment, share) estável.
- **Mídia/Assets**: Manter upload e associação a posts; documentar contrato para o app.
- Novas fases do backlog: implementar na API primeiro, depois BFF, depois app (conforme estratégia acima).

---

## Documentos a revisar (app já existe)

Vários documentos ainda descrevem o frontend como “em planejamento” ou “não existe”. Agora **já existe uma versão do app Flutter** (auth, onboarding, feed, mapa, eventos, perfil, notificações, publicar). Recomenda-se revisar os seguintes arquivos para refletir que o app está em uso e em evolução:

| Documento | O que ajustar |
|-----------|----------------|
| **docs/README.md** | Linha ~207: “Frontend e experiências móveis em planejamento” → indicar que o app Flutter existe (versão estável) e apontar para [STABLE_RELEASE_APP_ONBOARDING.md](./STABLE_RELEASE_APP_ONBOARDING.md) e esta matriz. |
| **docs/24_FLUTTER_FRONTEND_PLAN.md** | Status “Planejamento”: atualizar para “Em implementação / Parcialmente implementado” e referenciar esta matriz e o doc de release estável; manter o plano como guia do que falta. |
| **docs/35_PRIORIZACAO_ESTRATEGICA_API_FRONTEND.md** | Trechos que indicam “não há frontend” ou “Fase 2 bloqueadores frontend”: atualizar para refletir que o app existe e que a prioridade é alinhar API/BFF/App e depois evoluir. |
| **docs/33_FLUTTER_REVIEW_AND_GAPS.md** | Incluir que o app está em uso e que gaps (ex.: segurança frontend, biometric) são melhorias sobre uma base já existente. |
| **README.md (raiz)** | Seção “Estado do Projeto” / “Funcionalidades”: mencionar explicitamente o app Flutter e link para [STABLE_RELEASE_APP_ONBOARDING.md](./STABLE_RELEASE_APP_ONBOARDING.md) e [FEATURE_MATRIX_API_BFF_APP.md](./FEATURE_MATRIX_API_BFF_APP.md). |
| **docs/backlog-api/** (fases que citam “app Flutter” ou “mobile”) | Onde disser “implementar no app Flutter” sem contexto: indicar que o app já existe e que a fase é para **estender** o app (ex.: FASE29, FASE30). |

Sugestão de frase padrão para colocar nesses documentos:

> **App Flutter**: Existe uma versão estável do app (auth, onboarding, feed, mapa, eventos, perfil, notificações, publicar). Ver [Release estável – App e Onboarding](./STABLE_RELEASE_APP_ONBOARDING.md) e [Matriz API/BFF/App](./FEATURE_MATRIX_API_BFF_APP.md).

---

## Referências

- **Registro de jornadas BFF**: `backend/Araponga.Api.Bff/Journeys/BffJourneyRegistry.cs`
- **App – repositórios que chamam o BFF**: `frontend/araponga.app/lib/features/*/data/repositories/*.dart` e `feed/presentation/providers/feed_provider.dart`
- **Release estável e getting started**: [STABLE_RELEASE_APP_ONBOARDING.md](./STABLE_RELEASE_APP_ONBOARDING.md)
- **Planejamento do frontend Flutter**: [24_FLUTTER_FRONTEND_PLAN.md](./24_FLUTTER_FRONTEND_PLAN.md)
- **Backlog e fases**: [docs/backlog-api/](./backlog-api/)
