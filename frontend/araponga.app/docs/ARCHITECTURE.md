# Arquitetura – Ará App (Flutter)

Visão da arquitetura do app: rede, estado, rotas e internacionalização.

## Rede (BFF)

- **BffClient** (`lib/core/network/bff_client.dart`): cliente **Dio** com base em `/api/v2/journeys`.
  - Headers: `Authorization: Bearer <token>`, `X-Session-Id` (território ativo), `X-Geo-Latitude`, `X-Geo-Longitude` (quando disponível).
  - Interceptor: em **401** chama `onUnauthorized` (logout); em **429** faz uma nova tentativa após delay (Retry-After ou 2s).
  - Timeout 30s; erros convertidos para **ApiException** (statusCode, userMessage para 401/403/429/5xx).
- **bffClientProvider** injeta config, token, sessionId e geo; usado por repositórios e providers.

## Estado (Riverpod)

- **authStateProvider:** token, refresh, logout; restauração de sessão na inicialização.
- **selectedTerritoryIdProvider:** território ativo (AsyncValue), persistido em SharedPreferences; valor simples em **selectedTerritoryIdValueProvider** para o BffClient.
- **geoLocationStateProvider:** última posição (GeoPosition); atualizado no MainShell ou onboarding.
- **Feed:** **feedNotifierProvider(territoryId)** – estado paginado (items, page, hasMore, error); refresh e loadMore.
- **Perfil:** **meProfileProvider** (FutureProvider) + MeProfileRepository.
- **Onboarding:** **suggestedTerritoriesProvider** (family por lat/lng), **onboardingRepositoryProvider**.
- **Territórios:** **territoriesListProvider** (paged), **territoriesRepositoryProvider** (enter).

## Rotas (go_router)

- **/** → splash; redireciona para /login (sem token), /onboarding (token sem território) ou /home (token + território).
- **/login** → LoginScreen; após login → /home (ou /onboarding se não houver território).
- **/onboarding** → OnboardingScreen (localização → suggested-territories → complete → set território → /home).
- **/home** → MainShellScreen (IndexedStack: Feed, Explorar, Publicar, Notificações, Perfil).

Guards: sem token não acessa /home nem /onboarding; com token sem território não acessa /home.

## Internacionalização (i18n)

- **lib/l10n/:** `app_pt.arb`, `app_en.arb`, `app_localizations.dart` (classe manual; pode ser substituída por saída de `flutter gen-l10n`).
- **AppLocalizations.of(context)** em todas as telas para textos; **localizationsDelegates** e **supportedLocales** em MaterialApp.
- Idiomas: pt (padrão), en.

## Design e UX

- **AppConstants** (`core/config/constants.dart`): espaçamento, radius, animação, minTouchTargetSize (44), keys de storage.
- **AppDesignTokens** (`core/theme/app_design_tokens.dart`): **fonte única de cores e tokens visuais**; alterar aqui reflete em todo o app. Inclui paleta dark/light e ThemeExtension **AppColors** (`context.appColors`).
- **AppTheme:** monta ThemeData a partir de AppDesignTokens e AppConstants; Material 3 dark (padrão) e light; SnackBar floating, botões 44pt, inputDecorationTheme.
- **Widgets reutilizáveis:** ShimmerBox/FeedSkeleton/ProfileSkeleton, showSuccessSnackBar/showErrorSnackBar.
- Feed: skeleton no loading; animação de entrada (fade + slide) nos itens.
- **Manutenção:** ver [DESIGN_SYSTEM.md](DESIGN_SYSTEM.md) para onde alterar cores, espaçamento e tema.

## Dependências principais

- **go_router** – rotas.
- **flutter_riverpod** – estado.
- **dio** – HTTP (BFF).
- **flutter_secure_storage** – tokens.
- **shared_preferences** – território ativo.
- **geolocator** – posição para headers e onboarding.
- **flutter_localizations** + **intl** – i18n.
