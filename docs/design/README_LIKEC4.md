# Documentação LikeC4 — Ará

A arquitetura do sistema Ará está descrita em **LikeC4** (architecture-as-code), do **contexto** ao **componente**, com **camadas e módulos reais** e **interconexões** entre eles (não genérico).

## Arquivo

- **`arah.likec4`** — modelo com **formas e ícones distintos** e **relações tipadas** (não tudo com o mesmo ícone/seta):
  - **Elementos:** `actor` (person), `webApp` (browser), `mobileApp` (mobile), `apiService` (rectangle), `database` (cylinder), `objectStorage` (bucket), `messageQueue` (queue); cada um com `notation` para legenda.
  - **Relações:** `uses`, `calls` (HTTP sync, linha sólida), `proxies` (BFF→API, diamante), `persists` (escrita, crow), `publishes` (async, linha pontilhada), `auth` (OIDC).
  - **Conteúdo:** Nível 1 (Contexto): atores, sistema Ará e externos. Nível 2 (Containers): App Mobile, Web App, BFF, API, DB, Storage, Queue — cada um com forma própria (mobile, browser, rectangle, cylinder, bucket, queue). Nível 3 — Camadas expostas:
    - **App Mobile:** Core (Config, BffClient, Providers, Storage, Theme, Widgets) + Features (Auth, Feed, Events, Explore, Map, Notifications, Onboarding, Profile, Territories, Home) com relações feature → core.
    - **BFF:** Middleware (JourneyProxy, CorrelationId, Logging), Journey Registry (mapeamento jornada → API), Services (JourneyApiProxy, JourneyResponseCache), Contracts (DTOs por jornada).
    - **Arah API (composição):** API Host (Controllers por área: Platform, Feed, Map, Events, Chat, Marketplace, Moderation, Subscriptions, Users, Journeys, Alerts, Assets, Media, Notifications + Security + Middleware) → Application (módulos: Platform, Feed, Map, Events, Chat, Marketplace, Moderation, Subscriptions, Users, Alerts, Assets, Media, Notifications, Connections) → Domain e Infrastructure; relações explícitas Controller → Módulo e Módulo → Domain/Infrastructure.

## Como usar no Playground

1. Abra o **LikeC4 Playground**: [https://playground.likec4.dev/w/tutorial/index/](https://playground.likec4.dev/w/tutorial/index/).
2. Abra o arquivo **`arah.likec4`** neste repositório, copie todo o conteúdo.
3. No Playground, apague o código de exemplo e cole o conteúdo de `arah.likec4`.
4. Os diagramas são gerados à esquerda. No canto inferior direito, use o ícone de **ajuda** (?) para ver a **legenda** das formas e relações (notations). Use o seletor de **views** para alternar:
   - **index** — vista de contexto (nível 1).
   - **containers** — vista de containers (nível 2).
   - **Arah API** (view of arah.api) — componentes da API (nível 3).
   - **BFF** (view of arah.bff) — componentes do BFF.

## Referências

- [LikeC4 — Get Started](https://likec4.dev/tutorial)
- [LikeC4 — Playground](https://playground.likec4.dev/w/tutorial/index/)
- [LikeC4 — DSL Model](https://likec4.dev/dsl/model/)
- [LikeC4 — DSL Views](https://likec4.dev/dsl/views)
- C4 existente no repo: `design/Archtecture/C4_Context.md`, `C4_Containers.md`, `C4_Components.md`
