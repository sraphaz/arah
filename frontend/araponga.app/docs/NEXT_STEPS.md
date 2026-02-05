# Próximos passos – Araponga App (Flutter)

Alinhado à **proposta do projeto** (território-first, comunidade-first, interface tipo Instagram para baixa fricção), aos docs **24/25/26/34_FLUTTER_*** e ao **BFF** como única porta de entrada.

---

## Estado atual (resumo)

| Área | Implementado | Pendente |
|------|--------------|----------|
| **Estrutura** | `core/` (config, theme, BffClient, geo), `features/` (auth, home, feed, explore, profile) | Camada data/domain por feature; interceptors; storage |
| **Navegação** | go_router, MainShell (5 abas), **guard de auth** (/ → /login sem token, /home exige token) | Deep links; rotas por jornada |
| **Tema** | Material 3 light/dark, verde território; **AppConstants** no tema e telas | - |
| **BFF** | BffClient GET/POST/**PUT**, token (Bearer), X-Session-Id, X-Geo-*; feed/territory-feed, me/profile | Tratamento 429; retry |
| **Auth** | Login (auth/social dev), persistência token (secure_storage), refresh, logout; 401 → logout | - |
| **Feed** | Lista via BFF; **paginação**; **pull-to-refresh**; **carregar mais**; **scroll infinito** (loadMore ao rolar); **filtro por interesses** (filterByInterests); território ativo | Skeleton loader (já usado no loading inicial) |
| **Perfil** | **GET me/profile** + edição (nome, bio); **GET/PUT me/preferences** (notificações); **GET/POST/DELETE me/interests**; Sair → /login | - |
| **Explorar** | Lista BFF territories; **POST enter** ao selecionar; TerritorySelector; **Mapa** (Ver no mapa); **Eventos** (lista) | - |
| **Notificações** | **Lista** BFF notifications/paged; pull-to-refresh; **marcar como lida** ao toque | Push (registro token) |
| **Eventos** | **Lista** BFF events/territory-events; tela /events; **Participar** (POST participate); link no Explorar | - |
| **Mapa** | Tela /map com flutter_map, pins BFF map/pins, link no Explorar | - |

---

## Princípios de alinhamento

1. **Território primeiro**  
   Todo conteúdo e ações no contexto de um território; seleção de território ativo (X-Session-Id) desde o início.

2. **Comunidade primeiro**  
   Feed territorial, eventos, conexões e mapa como eixos; UX que favorece descoberta e participação local.

3. **Baixa fricção (tipo Instagram)**  
   Scroll fluido, gestos claros, bottom nav estável, carregamento progressivo (skeleton), feedback imediato (26_FLUTTER_DESIGN_GUIDELINES).

4. **BFF como única API**  
   Todas as chamadas passam por `/api/v2/journeys/<jornada>/...`; nenhuma chamada direta à API principal.

5. **Convergência com backend**  
   Priorizar jornadas e fases já cobertas pelo BFF e pela API (34_FLUTTER_API_STRATEGIC_ALIGNMENT).

6. **Geolocalização e território observado**  
   A regra de que a **geolocalização do usuário deve convergir com o território que ele está observando** fica no backend. O app deve **enviar** a posição do usuário para que o backend possa aplicá-la:
   - **Headers:** `X-Geo-Latitude` e `X-Geo-Longitude` (valores em graus decimais, ex.: `-23.5505`, `-46.6333`).
   - **Fluxo:** App obtém a localização (ex.: `geolocator`) → inclui nos headers de cada requisição ao BFF → BFF repassa os headers para a API → API usa `GeoHeaderReader` e pode validar convergência com o território (sessão/query).
   - **Uso hoje:** Onboarding usa `suggested-territories?latitude=&longitude=&radiusKm=` (query); verificação de residência usa POST `verify-residency/geo` com body. Para feed e outras operações por território, o backend pode (quando implementado) exigir que as coordenadas dos headers estejam próximas do território observado.
   - **App:** Incluir geo nos headers quando a permissão de localização estiver concedida (ex.: interceptor ou `BffClient` com parâmetros opcionais `latitude`/`longitude`).

---

## Próximos passos priorizados

### Fase 1 – Fundação (Fase 0 do roadmap)

#### 1.1 Autenticação real e token

- **Objetivo:** Login funcional e sessão persistida.
- **Ações:**
  - Chamar BFF `auth/login` (POST) a partir da tela de login com email/senha.
  - Persistir access (e refresh) token com `flutter_secure_storage`.
  - Incluir token em todas as requisições do `BffClient` (header `Authorization: Bearer`).
  - Tratar 401 (redirect para /login ou refresh) e exibir erro de credenciais na tela.
- **Jornada BFF:** `auth` (login, refresh, logout).
- **Doc:** 24_FLUTTER_FRONTEND_PLAN (Segurança e Autenticação), BFF_FLUTTER_EXAMPLE.

#### 1.2 Guard de rota e contexto de território

- **Objetivo:** Só permitir acesso ao shell principal se houver token; manter território ativo.
- **Ações:**
  - No `app_router`, redirecionar `/` para `/login` se não houver token; para `/home` se houver.
  - Criar provider (Riverpod) para “território ativo” (id/nome) e, quando houver seleção, enviar `X-Session-Id` no BffClient.
  - Opcional: provider de “current user” a partir da jornada `me` (profile).
- **Jornadas BFF:** `auth`, `me`, `territories`.

#### 1.3 Onboarding (primeiro acesso)

- **Objetivo:** Fluxo para novo usuário: localização (opcional), descoberta de territórios, seleção do território.
- **Ações:**
  - Tela(s) de onboarding: permissão de localização (geolocalização), lista de territórios sugeridos via BFF `onboarding/suggested-territories`, seleção e conclusão via `onboarding/complete` (POST).
  - Após concluir, definir território ativo e ir para `/home`.
  - Se usuário já tem território, pular onboarding e ir para home.
- **Jornadas BFF:** `onboarding`.
- **Doc:** 25_FLUTTER_IMPLEMENTATION_ROADMAP (VISITOR – Descoberta).

#### 1.4 Design tokens e constantes

- **Objetivo:** Consistência visual e manutenção fácil (26_FLUTTER_DESIGN_GUIDELINES).
- **Ações:**
  - Criar `core/config/constants.dart` (ou `core/theme/tokens.dart`) com espaçamento (ex.: grid 8px), border radius, durações de animação.
  - Centralizar cores em `app_theme.dart` (já iniciado) usando semantic names (primary, surface, onSurface, etc.).
  - Reutilizar esses tokens nas telas existentes (login, feed, profile, explore).

---

### Fase 2 – Jornadas principais (fluxo território + feed + perfil)

#### 2.1 Perfil real (jornada me)

- **Objetivo:** Exibir e editar dados do usuário logado.
- **Ações:**
  - Chamar BFF `me/profile` (GET) e preencher avatar, nome, bio (conforme contrato).
  - Tela ou bottom sheet de edição: PUT `me/profile` com campos editáveis.
  - Opcional: `me/preferences`, `me/interests`, `me/devices` (lista de dispositivos).
- **Jornada BFF:** `me`.

#### 2.2 Feed territorial completo

- **Objetivo:** Feed utilizável com território ativo e paginação.
- **Ações:**
  - Usar território ativo (provider) em `feed/territory-feed?territoryId=...`.
  - Paginação (pageNumber, pageSize) e scroll infinito ou “carregar mais”.
  - Pull-to-refresh que invalida cache e recarrega primeira página.
  - Tratar empty state e erro de rede com retry (já parcialmente feito).
- **Jornada BFF:** `feed`.

#### 2.3 Explorar – territórios

- **Objetivo:** Listar territórios e permitir “entrar” ou definir como ativo.
- **Ações:**
  - Tela “Explorar” com lista de territórios via BFF `territories` (lista ou paged).
  - Card de território: nome, descrição/resumo, entrada (ex.: “Entrar” → POST territories/{id}/enter ou definição de território ativo conforme BFF).
  - Ao selecionar território, atualizar provider de território ativo e, se desejado, navegar para feed.
- **Jornadas BFF:** `territories`, `membership` (become-resident quando aplicável).

#### 2.4 Criar post (jornada feed)

- **Objetivo:** Publicar post no território ativo.
- **Ações:**
  - Tela ou modal “Publicar” (aba central do bottom nav): título, conteúdo, tipo, visibilidade (conforme BFF).
  - POST `feed/create-post` com territoryId e body; após sucesso, invalidar feed e voltar.
  - Opcional: anexar mídia (jornada assets/media) em etapa posterior.
- **Jornada BFF:** `feed`.

---

### Fase 3 – Refino de UX e camada de rede

#### 3.1 Camada de rede robusta

- **Objetivo:** Alinhar ao plano (24 – Dio, interceptors).
- **Ações:**
  - Opcional: trocar `http` por **Dio**; implementar interceptors: auth (Bearer), session (X-Session-Id), geo (**X-Geo-Latitude**, **X-Geo-Longitude** quando houver localização), retry, rate limit (429), logging em debug.
  - Tratamento centralizado de erros (BffException, 401, 429) e exibição em UI (snackbar ou tela de erro).
- **Doc:** 24_FLUTTER_FRONTEND_PLAN (HTTP e Networking).

#### 3.2 UX tipo Instagram (diretrizes 26)

- **Objetivo:** Fluidez e feedback imediato.
- **Ações:**
  - Skeleton loader no feed enquanto carrega.
  - Animações leves em like/curtir e em botões principais (scale/tap).
  - Snackbar ou toast em ações (post criado, perfil atualizado, erro).
  - Garantir contraste e toques mínimos (44pt) para acessibilidade (26 e 31_FLUTTER_ACCESSIBILITY_GUIDE).

#### 3.3 Internacionalização (i18n)

- **Objetivo:** pt-BR e en-US desde cedo (25 – Fase 0).
- **Ações:**
  - Configurar `flutter_localizations` e `intl`; criar ARB em `l10n/` (pt-BR, en-US).
  - Substituir strings fixas nas telas por `AppLocalizations.of(context)!....`.
- **Doc:** 32_FLUTTER_I18N_GUIDE.

---

## Ordem sugerida de implementação

1. ~~**1.1** Autenticação real + token + BffClient com Bearer.~~ ✅  
2. ~~**1.2** Guard de rota + provider de território ativo (+ X-Session-Id).~~ ✅  
3. ~~**1.4** Design tokens/constants.~~ ✅  
4. ~~**2.1** Perfil real (me/profile).~~ ✅  
5. ~~**2.2** Feed com território ativo + paginação + pull-to-refresh.~~ ✅  
6. ~~**1.3** Onboarding (suggested-territories + complete).~~ ✅  
7. ~~**2.3** Explorar – lista de territórios e entrada (POST enter).~~ ✅  
8. ~~**2.4** Criar post (feed/create-post, aba Publicar).~~ ✅  
9. ~~**3.x** Rede (Dio/interceptors), UX (skeleton, feedback), i18n.~~ ✅

---

## CI/CD e testes

- Estado atual do CI/CD e prioridades (app Flutter no CI, primeiros testes, CD do app): **[CI_CD_AND_TESTS.md](CI_CD_AND_TESTS.md)**.

---

## Referências

- [24_FLUTTER_FRONTEND_PLAN](../../../docs/24_FLUTTER_FRONTEND_PLAN.md) – Stack, estrutura, funcionalidades.
- [25_FLUTTER_IMPLEMENTATION_ROADMAP](../../../docs/25_FLUTTER_IMPLEMENTATION_ROADMAP.md) – Fases 0–15, jornadas por papel.
- [26_FLUTTER_DESIGN_GUIDELINES](../../../docs/26_FLUTTER_DESIGN_GUIDELINES.md) – Design system, cores, UX tipo Instagram.
- [34_FLUTTER_API_STRATEGIC_ALIGNMENT](../../../docs/34_FLUTTER_API_STRATEGIC_ALIGNMENT.md) – Alinhamento com API/BFF e gaps.
- [BFF_FLUTTER_EXAMPLE](../../../docs/BFF_FLUTTER_EXAMPLE.md) – Exemplo de modelos e serviços BFF em Dart.
