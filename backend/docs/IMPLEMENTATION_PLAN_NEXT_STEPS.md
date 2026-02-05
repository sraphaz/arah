# Plano de implementação – Próximas etapas

Este documento consolida as **próximas ações** após a reorganização do backend, BFF com jornadas iniciais e documentação de melhorias. Referência: [BFF_JOURNEYS_IMPLEMENTATION_PLAN.md](BFF_JOURNEYS_IMPLEMENTATION_PLAN.md), [IMPROVEMENTS_AND_KNOWN_ISSUES.md](IMPROVEMENTS_AND_KNOWN_ISSUES.md), [TEST_SEPARATION_BY_MODULE.md](TEST_SEPARATION_BY_MODULE.md).

---

## 1. Estado atual (resumo)

- **Estrutura de pastas:** Projetos na raiz de `backend/`; módulos com estrutura flat (`.csproj` na raiz do módulo, pastas Domain/Application/Infrastructure dentro). Nenhuma pasta aninhada duplicada pendente.
- **BFF:** Jornadas onboarding, feed, events, marketplace implementadas; proxy em `/api/v2/journeys/*` para a API; cache por path; GET `/bff/journeys` para documentação.
- **API:** Jornadas v2 em controllers Journeys; demais funcionalidades em `api/v1/*` (auth, users/me, connections, territories, etc.).
- **Domain/Application:** Application referencia todos os módulos (hub); Domain compartilhado sem tipos de Connections (módulo isolado). Padrão documentado como aceitável para modular monolith.
- **Testes:** Core (Araponga.Tests), por módulo (Connections, Map, Marketplace, Moderation, Subscriptions), BFF (Araponga.Tests.Bff), ApiSupport compartilhado. Movimentação de testes Application para módulos já feita onde planejado.

---

## 2. Jornadas BFF – Fases 1 a 5

### 2.1 Path rewrite (implementado nesta etapa)

O BFF passa a suportar **mapeamento jornada → path na API**:

- Jornadas atuais (onboarding, feed, events, marketplace): `api/v2/journeys/<jornada>/...`
- Novas jornadas (auth, me, etc.): rewrite para `api/v1/...` conforme tabela abaixo.

| Jornada   | Path na API (base)   | Exemplo BFF → API                          |
|-----------|----------------------|--------------------------------------------|
| onboarding| api/v2/journeys/onboarding | /api/v2/journeys/onboarding/complete → api/v2/journeys/onboarding/complete |
| auth      | api/v1/auth          | /api/v2/journeys/auth/login → api/v1/auth/login |
| me        | api/v1/users/me     | /api/v2/journeys/me/profile → api/v1/users/me/profile |

Implementação: `BffJourneyRegistry` expõe `GetApiPathBase(journeyName)`; `JourneyApiProxy` usa esse mapeamento para montar a URI.

### 2.2 Fase 1 – Auth e Me ✅ (implementado)

- **auth:** Registrar no `BffJourneyRegistry`; sem cache; endpoints: login, refresh, logout, 2FA (conforme API).
- **me:** Registrar; cache GET 90s para perfil/preferências; endpoints: (vazio), profile, preferences, interests, devices (conforme API).

### 2.3 Fase 2 – Connections, Territories, Membership ✅ (implementado)

- **connections:** api/v1/connections; cache GET 45s (lista, pending, privacy, users/search, suggestions); POST request, accept, reject; PUT privacy; DELETE.
- **territories:** api/v1/territories; cache GET 90s (lista, paged, {id}, {id}/features); POST {id}/enter.
- **membership:** api/v1/memberships; cache GET 60s (me, {territoryId}/me); POST become-resident, verify-residency, transfer-residency.

### 2.4 Fase 3 – Map, Assets, Media ✅ (implementado)

- **map:** api/v1/map; cache GET 90s (entities, entities/paged, pins, pins/paged); POST entities, confirmations, relations.
- **assets:** api/v1/assets; cache GET 60s (lista, paged, {assetId}); POST upload, archive, validate, curate.
- **media:** api/v1/media; cache GET 60s ({id}, {id}/info); POST upload; DELETE {id}.

### 2.5 Fase 4 – Subscriptions, Notifications, Marketplace-v1 ✅ (implementado)

- **subscription-plans:** api/v1/subscription-plans; cache GET 120s (lista, {id}).
- **subscriptions:** api/v1/subscriptions; cache GET 120s (me, me/capabilities, me/limits, {id}); POST criar, cancel, reactivate, check-capability.
- **notifications:** api/v1/notifications; cache GET 30s (lista, paged); POST {id}/read.
- **marketplace-v1:** api/v1 (path: cart, stores, items); cache GET 60s (cart, stores, stores/me, items, items/paged, items/{id}); POST cart/items, checkout, stores, etc.

### 2.6 Fase 5 – Moderation, Chat, Alerts, Admin ✅ (implementado)

- **moderation:** api/v1/territories; path {territoryId}/work-items, moderation/cases, evidences; cache GET 45s.
- **chat:** api/v1/chat; conversas, mensagens, participantes; cache GET 30s.
- **alerts:** api/v1/alerts; cache GET 45s (lista, paged); POST criar.
- **admin:** api/v1/admin; seed, subscription-plans, system-configs, cache-metrics, work-items, verifications; cache GET 30s.

Cada fase: mapeamento no registry, TTL onde aplicável, testes, `.http`.

---

## 2.7 Geolocalização e convergência com território

- **Regra:** A geolocalização do usuário deve convergir com o território que ele está observando. A validação é feita no **backend**; o app (e qualquer cliente) deve enviar a posição para que a API possa aplicá-la.
- **API:** Cabeçalhos `X-Geo-Latitude` e `X-Geo-Longitude` (definidos em `ApiHeaders`). Leitura via `GeoHeaderReader.TryGetCoordinates(Request.Headers, ...)`; coordenadas válidas validadas com `GeoCoordinate.IsValid`.
- **BFF:** Repassa todos os headers da requisição (exceto `Host`) para a API; não precisa lógica específica de geo.
- **Uso atual:** (1) Onboarding: `suggested-territories` usa query `latitude`, `longitude`, `radiusKm`. (2) Residência: POST `memberships/{territoryId}/verify-residency/geo` com body `{ latitude, longitude }` para provar presença no território.
- **Raio por território:** Cada território pode ter um `RadiusKm` (nullable). Quando null, usa-se o padrão do sistema (`Constants.Geo.VerificationRadiusKm`, 5 km). Assim o perímetro varia por território (bairro 5 km, região 50 km, etc.).
- **Uso atual:** Feed (v1 e jornada), create-post e verify-residency/geo usam `territory.RadiusKm ?? Constants.Geo.VerificationRadiusKm` para validar convergência.

- **Acesso remoto ao território (bypass geo):** Feature flag por território e permissão por usuário permitem ignorar a exigência de convergência geolocalização.
  - **Por território:** `FeatureFlag.RemoteAccessToTerritoryEnabled` — quando ativa no território, qualquer usuário pode acessar (feed, criar post) sem estar no perímetro.
  - **Por usuário:** `SystemPermissionType.RemoteAccessToTerritory` — usuários com essa permissão podem acessar qualquer território sem geo. **Sys admin:** tem bypass por padrão (não precisa da permissão explícita).
  - Serviço `IGeoConvergenceBypassService.ShouldBypassGeoEnforcementAsync(territoryId, userId)` centraliza a decisão; usado em FeedController e FeedJourneyController. Verify-residency/geo não usa bypass (continua exigindo presença no território).

---

## 3. Desacoplamento e assimetrias (incremental)

### 3.1 Domain e Application

- **Manter como está no curto prazo.** Application como orquestrador central está documentado; evitar novos serviços que dependam de N módulos; preferir eventos ou interfaces em Application.Abstractions.
- **Opcional (médio prazo):** Mover serviços de aplicação específicos de um módulo (ex.: ConnectionService) para o próprio módulo (Application), se a equipe decidir que o módulo deve ser totalmente autocontido. Hoje não é obrigatório.

### 3.2 Unit of Work e DbContext

- **Assimetria conhecida:** UoW composto com participantes (DbContext); alguns repositórios usam DbContext direto. Documentado nos ADRs. Nenhuma alteração obrigatória; revisar apenas se houver bugs de transação.

### 3.3 Logs, exceções e autenticação

- **Logs:** Manter convenção atual (ILogger por tipo); não introduzir novo framework.
- **Exceções:** Application exceptions e middleware de erro já existentes; manter padrão.
- **Autenticação:** BFF repassa headers (Authorization) para a API; API valida token. Quando a jornada `auth` estiver no BFF, tokens e refresh continuam apenas repassados; sem lógica de auth no BFF.

---

## 4. Módulo Connections ✅

- **Situação atual:** Módulo com Domain, Application (interfaces), Infrastructure (ConnectionsDbContext, Postgres). Application central (Araponga.Application) contém ConnectionService, ConnectionPrivacyService, AcceptedConnectionsProvider.
- **Recomendação:** Manter. Connections já tem infraestrutura própria no módulo; o serviço de aplicação no Core é uma escolha arquitetural documentada (provedores de aplicação podem ficar no Core). Se no futuro o módulo for extraído como serviço, mover serviços para o módulo.
- **Status:** Mantido conforme recomendação (nenhuma alteração).

---

## 5. Testes ✅

### 5.1 Simetria e modularização

- Seguir [TEST_SEPARATION_BY_MODULE.md](TEST_SEPARATION_BY_MODULE.md): testes 100% do módulo no `Araponga.Tests.Modules.X` quando o projeto existir; integração que cruza vários módulos no Core.
- Criar novos projetos de teste por módulo apenas quando houver demanda (ex.: Feed, Events, Chat, Alerts).
- **Status:** Convenção documentada no README dos testes e em TEST_SEPARATION_BY_MODULE; projetos por módulo existentes (Connections, Map, Marketplace, Moderation, Subscriptions); Feed/Events/Chat/Alerts sem projeto dedicado por opção (sob demanda).

### 5.2 Cobertura BFF (meta 80%)

- Garantir testes para: registro de jornadas, cache (TTL, hit/miss), proxy (rewrite auth/me e v2), endpoint GET `/bff/journeys`. Cobertura de código do BFF via testes unitários e de integração.
- **Status:** Cobertura em dia. Testes em `Araponga.Tests.Bff`: **BffJourneyRegistryTests** (registro, GetApiPathBase, CacheableGetEndpoints), **JourneyCacheTtlTests** (TTL por jornada), **JourneyResponseCacheTests** (ShouldCache, GetTtlSeconds, TryGet/Set — hit/miss), **JourneyApiProxyPathRewriteTests** (rewrite auth/me e v2), **BffJourneysEndpointTests** (GET `/bff/journeys` — integração).

### 5.3 Arquitetura de testes

- **Araponga.Tests:** API, Application/Domain/Infrastructure compartilhados, integração cross-module.
- **Araponga.Tests.Bff:** BFF (proxy, cache, registry, endpoint journeys).
- **Araponga.Tests.ApiSupport:** Factory e helpers de auth compartilhados (Core e Subscriptions).
- **Araponga.Tests.Modules.X:** Testes específicos do módulo X.
- **Status:** Estrutura atual alinhada ao documento; README dos testes referencia TEST_SEPARATION_BY_MODULE.

---

## 6. Cronograma sugerido (próximas entregas)

| Ordem | Entrega | Conteúdo |
|-------|---------|----------|
| 1 | Path rewrite + auth/me | Mapeamento journey→API no BFF; jornadas auth e me no registry; cache me; testes. |
| 2 | ~~Fase 2 BFF~~ ✅ | connections, territories, membership no BFF (registro, cache, testes) — implementado. |
| 3 | ~~Fase 3 BFF~~ ✅ | map, assets, media no BFF (registro, cache, testes) — implementado. |
| 4 | ~~Fases 4–5 BFF~~ ✅ | subscription-plans, subscriptions, notifications, marketplace-v1, moderation, chat, alerts, admin — implementado. Total: 20 jornadas no BFF. |
| 5 | ~~Item 4 – Módulo Connections~~ ✅ | Mantido conforme recomendação (sem alteração). |
| 6 | ~~Item 5 – Testes~~ ✅ | Simetria/modularização documentada; cobertura BFF (registry, cache TTL/hit-miss, proxy, GET /bff/journeys) implementada; arquitetura alinhada ao plano. |
| — | Opcional | Revisão de assimetrias (UoW, logs); mover ConnectionService para módulo Connections se desejado; criar Tests.Modules.Feed/Events/Chat/Alerts sob demanda. |

---

## 7. Referências

- [BFF_JOURNEYS_IMPLEMENTATION_PLAN.md](BFF_JOURNEYS_IMPLEMENTATION_PLAN.md)
- [IMPROVEMENTS_AND_KNOWN_ISSUES.md](IMPROVEMENTS_AND_KNOWN_ISSUES.md)
- [TEST_SEPARATION_BY_MODULE.md](TEST_SEPARATION_BY_MODULE.md)
- [BACKEND_LAYERS_AND_NAMING.md](BACKEND_LAYERS_AND_NAMING.md)
- [README backend](../README.md)
