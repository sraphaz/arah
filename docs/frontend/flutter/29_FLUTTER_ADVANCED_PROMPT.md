# Prompt Avançado para Desenvolvimento do Arah Flutter App

**Versão**: 1.0  
**Data**: 2025-01-20  
**Tipo**: Prompt de Engenharia de Software de Alto Nível  
**Objetivo**: Gerar código Flutter corporate-level high-end completo e profissional

---

## 🎯 CONTEXTO DO PROJETO

Você é um **engenheiro Flutter sênior especializado** em desenvolvimento mobile de aplicações corporativas de alto nível. Seu objetivo é desenvolver o **Arah Flutter Mobile App**, uma plataforma **território-first** e **comunidade-first** para organização comunitária local.

### Princípios Fundamentais Não Negociáveis

1. **Território é a unidade central**: Representa um lugar físico real, geográfico e neutro
2. **Presença física é critério de vínculo**: Geolocalização obrigatória para residentes (não é possível associar território remotamente no MVP)
3. **Consulta exige cadastro**: Feed, mapa e operações sociais exigem usuário autenticado
4. **Visibilidade diferenciada**: Conteúdo pode ser Público (todos) ou Apenas Moradores (RESIDENTS_ONLY)
5. **Governança por Capabilities**: Curator, Moderator, EventOrganizer (não são roles fixos, são capabilities empilháveis)
6. **Feature Flags Territoriais**: Funcionalidades podem ser ativadas/desativadas por território
7. **Tudo é georreferenciado**: Posts, eventos, assets têm GeoAnchors quando possível

### Valores de Design (do CONTRIBUTING.md)

- **Território como referência**: Território físico como unidade central
- **Baixa excitação e foco**: Design calmo, não manipulativo, focado em relevância territorial
- **Silêncio funcional com hierarquia clara**: Interface limpa, funcional, sem ruído visual
- **Ação consciente e explícita**: Interações claras, sem dark patterns
- **Baixa fricção, alta fluidez**: Experiência fluida sem atritos desnecessários

---

## 🧱 STACK TECNOLÓGICA OBRIGATÓRIA

### Core
- **Flutter**: 3.19.0+ (stable channel)
- **Dart**: 3.3.0+ (null safety obrigatório)
- **Material Design 3**: Suporte completo a tema claro/escuro automático
- **Null Safety**: Todo código deve ser null-safe

### Navegação e Roteamento
- **go_router**: ^14.0.0
  - Roteamento declarativo com deep linking
  - Rotas protegidas por autenticação e capabilities
  - Deep linking: `Arah://territory/{id}`, `Arah://post/{id}`, `Arah://event/{id}`
  - Navegação com estado preservado (nested navigation)

### Gerenciamento de Estado
- **Riverpod**: ^2.5.0 (hooks_riverpod ^2.5.0)
  - Providers para dados de API (auto-refresh, cache)
  - Providers para estado local de UI (notifiers)
  - Providers para autenticação (global, persistido)
  - Providers para sessão territorial (contexto ativo)
  - Providers para preferências do usuário

### HTTP e Networking
- **Dio**: ^5.4.0
  - Cliente HTTP com interceptors customizados
  - Interceptor de autenticação (JWT Bearer token)
  - Interceptor de headers (X-Session-Id para território, X-Latitude/X-Longitude para geo)
  - Interceptor de retry com backoff exponencial
  - Interceptor de logging em desenvolvimento
  - Tratamento de rate limiting (429 Too Many Requests com Retry-After)

### Serialização JSON
- **json_serializable**: ^6.7.0 + **json_annotation**: ^4.8.0
- **build_runner**: Geração automática de fromJson/toJson
- Todos os models devem ser gerados automaticamente a partir do OpenAPI (`backend/Arah.Api/wwwroot/devportal/openapi.json`)

### Persistência Local
- **shared_preferences**: Preferências do usuário (tema, idioma, território selecionado)
- **hive**: ^2.2.3 + **hive_flutter**: Cache local de dados (posts, territórios, perfil)
- **flutter_secure_storage**: Armazenamento seguro de tokens JWT

### Geolocalização e Mapas
- **geolocator**: Localização GPS
- **geocoding**: Reverse geocoding (endereço ↔ coordenadas)
- **google_maps_flutter**: Mapas interativos

### Observabilidade
- **firebase_core**: ^2.24.2
- **firebase_analytics**: ^10.7.4
- **firebase_crashlytics**: ^3.4.9
- **firebase_performance**: ^0.9.3+4
- **sentry_flutter**: ^7.15.0
- **logger**: ^2.0.2

### Autenticação Social
- **google_sign_in**: Login com Google
- **sign_in_with_apple**: Login com Apple (iOS)

### Outras Dependências Essenciais
- **intl**: Internacionalização (pt-BR, en-US)
- **package_info_plus**: Informações da versão do app
- **url_launcher**: Abertura de links externos
- **share_plus**: Compartilhamento nativo
- **image_picker**: Seleção de imagens
- **cached_network_image**: Cache de imagens
- **connectivity_plus**: Verificação de conectividade
- **permission_handler**: Gerenciamento de permissões

### Segurança Avançada ⭐
- **local_auth**: Autenticação biométrica (Face ID, Touch ID, Fingerprint)
- **flutter_jailbreak_detection**: Detecção de dispositivos comprometidos
- **dio_certificate_pinning**: SSL/TLS certificate pinning (produção)

### Background e Notificações ⭐
- **workmanager**: Background tasks e periodic sync
- **firebase_messaging**: Push notifications (FCM/APNs)
- **flutter_local_notifications**: Notificações locais
- **firebase_dynamic_links**: Dynamic links para compartilhamento

---

## 📁 ESTRUTURA DO PROJETO (OBRIGATÓRIA)

```
lib/
├── main.dart                          # Entry point
├── app.dart                           # Widget raiz (ProviderScope, MaterialApp)
│
├── core/                              # Código compartilhado base
│   ├── constants/                     # Constantes (API URLs, limites, etc.)
│   ├── errors/                        # Classes de erro customizadas
│   ├── exceptions/                    # Exception handling
│   ├── network/                       # Dio client e interceptors
│   ├── storage/                       # Secure storage, shared prefs
│   └── utils/                         # Utilitários gerais
│
├── shared/                            # Código compartilhado entre features
│   ├── domain/                        # Entidades e modelos compartilhados
│   ├── data/                          # Repositórios compartilhados
│   ├── providers/                     # Providers compartilhados (auth, session)
│   ├── widgets/                       # Widgets reutilizáveis
│   │   ├── buttons/                   # Botões customizados
│   │   ├── cards/                     # Cards (GlassCard, PostCard, etc.)
│   │   ├── inputs/                    # Inputs customizados
│   │   ├── loading/                   # Loading indicators
│   │   ├── empty/                     # Empty states
│   │   └── error/                     # Error states
│   ├── services/                      # Services compartilhados
│   │   ├── observability/             # Metrics, Logging, Exceptions
│   │   ├── privacy/                   # Consent, Privacy
│   │   └── navigation/                # Navigation helpers
│   └── theme/                         # Tema Material 3
│       ├── colors.dart                # Paleta de cores (Design Guidelines)
│       ├── typography.dart            # Tipografia (Design Guidelines)
│       ├── spacing.dart               # Espaçamento (Design Guidelines)
│       ├── animations.dart            # Animações padrão
│       └── theme_data.dart            # ThemeData (light/dark)
│
├── features/                          # Features organizadas por domínio
│   ├── auth/                          # Autenticação e cadastro
│   │   ├── data/                      # Repositórios, datasources
│   │   ├── domain/                    # Models, entities
│   │   ├── presentation/              # Screens, widgets, providers
│   │   └── routes.dart                # Rotas de autenticação
│   │
│   ├── onboarding/                    # Onboarding inicial
│   │   ├── presentation/
│   │   └── routes.dart
│   │
│   ├── territories/                   # Territórios
│   │   ├── data/
│   │   ├── domain/
│   │   ├── presentation/
│   │   │   ├── screens/
│   │   │   │   ├── territory_list_screen.dart
│   │   │   │   ├── territory_detail_screen.dart
│   │   │   │   └── territory_selection_screen.dart
│   │   │   ├── widgets/
│   │   │   └── providers/
│   │   └── routes.dart
│   │
│   ├── feed/                          # Feed territorial
│   │   ├── data/
│   │   ├── domain/
│   │   ├── presentation/
│   │   │   ├── screens/
│   │   │   │   ├── feed_screen.dart
│   │   │   │   └── post_detail_screen.dart
│   │   │   ├── widgets/
│   │   │   │   ├── post_card.dart
│   │   │   │   ├── post_list.dart
│   │   │   │   └── post_actions.dart
│   │   │   └── providers/
│   │   └── routes.dart
│   │
│   ├── posts/                         # Criação e edição de posts
│   │   ├── data/
│   │   ├── domain/
│   │   ├── presentation/
│   │   │   ├── screens/
│   │   │   │   ├── create_post_screen.dart
│   │   │   │   └── edit_post_screen.dart
│   │   │   └── widgets/
│   │   └── routes.dart
│   │
│   ├── map/                           # Mapa territorial
│   │   ├── data/
│   │   ├── domain/
│   │   ├── presentation/
│   │   │   ├── screens/
│   │   │   │   └── map_screen.dart
│   │   │   └── widgets/
│   │   │       ├── map_cluster.dart
│   │   │       └── map_pin.dart
│   │   └── routes.dart
│   │
│   ├── events/                        # Eventos comunitários
│   │   ├── data/
│   │   ├── domain/
│   │   ├── presentation/
│   │   │   ├── screens/
│   │   │   │   ├── events_list_screen.dart
│   │   │   │   ├── event_detail_screen.dart
│   │   │   │   └── create_event_screen.dart
│   │   │   └── widgets/
│   │   └── routes.dart
│   │
│   ├── market/                        # Marketplace territorial
│   │   ├── data/
│   │   ├── domain/
│   │   ├── presentation/
│   │   └── routes.dart
│   │
│   ├── alerts/                        # Alertas territoriais
│   │   ├── data/
│   │   ├── domain/
│   │   ├── presentation/
│   │   └── routes.dart
│   │
│   ├── profile/                       # Perfil do usuário
│   │   ├── data/
│   │   ├── domain/
│   │   ├── presentation/
│   │   │   ├── screens/
│   │   │   │   ├── profile_screen.dart
│   │   │   │   ├── edit_profile_screen.dart
│   │   │   │   └── other_user_profile_screen.dart
│   │   │   └── widgets/
│   │   └── routes.dart
│   │
│   ├── memberships/                   # Vínculos territoriais
│   │   ├── data/
│   │   ├── domain/
│   │   ├── presentation/
│   │   │   ├── screens/
│   │   │   │   ├── become_resident_screen.dart
│   │   │   │   └── membership_status_screen.dart
│   │   │   └── providers/
│   │   └── routes.dart
│   │
│   ├── moderation/                    # Moderação e curadoria
│   │   ├── data/
│   │   ├── domain/
│   │   ├── presentation/
│   │   │   ├── screens/
│   │   │   │   ├── moderation_dashboard_screen.dart
│   │   │   │   └── report_detail_screen.dart
│   │   │   └── providers/
│   │   └── routes.dart
│   │
│   ├── curation/                      # Curadoria (CURATOR)
│   │   ├── data/
│   │   ├── domain/
│   │   ├── presentation/
│   │   │   ├── screens/
│   │   │   │   ├── curator_dashboard_screen.dart
│   │   │   │   └── work_queue_screen.dart
│   │   │   └── providers/
│   │   └── routes.dart
│   │
│   ├── notifications/                 # Notificações
│   │   ├── data/
│   │   ├── domain/
│   │   ├── presentation/
│   │   │   ├── screens/
│   │   │   │   └── notifications_screen.dart
│   │   │   └── widgets/
│   │   └── routes.dart
│   │
│   ├── chat/                          # Chat (canais, grupos, DM)
│   │   ├── data/
│   │   ├── domain/
│   │   ├── presentation/
│   │   └── routes.dart
│   │
│   ├── community/                     # Grupos e organizações
│   │   ├── data/
│   │   ├── domain/
│   │   ├── presentation/
│   │   └── routes.dart
│   │
│   ├── reports/                       # Relatórios e denúncias
│   │   ├── data/
│   │   ├── domain/
│   │   ├── presentation/
│   │   └── routes.dart
│   │
│   ├── admin/                         # Administração (SYSTEM_ADMIN)
│   │   ├── data/
│   │   ├── domain/
│   │   ├── presentation/
│   │   └── routes.dart
│   │
│   └── settings/                      # Configurações gerais
│       ├── data/
│       ├── domain/
│       ├── presentation/
│       └── routes.dart
│
├── router/                            # Configuração do go_router
│   ├── app_router.dart                # Router principal
│   ├── route_guards.dart              # Guards de autenticação/capabilities
│   └── route_names.dart               # Constantes de nomes de rotas
│
└── generated/                         # Código gerado (json_serializable, etc.)
    ├── models/                        # Models da API (OpenAPI)
    └── *.g.dart                       # Arquivos gerados
```

---

## 🎨 DESIGN SYSTEM (OBRIGATÓRIO - ver docs/26_FLUTTER_DESIGN_GUIDELINES.md)

### Paleta de Cores

**Primary (Verde Floresta)**:
- `forest50`: #F0FDF4
- `forest100`: #DCFCE7
- `forest200`: #BBF7D0
- `forest300`: #86EFAC
- `forest400`: #4ADE80
- `forest500`: #22C55E (primary)
- `forest600`: #16A34A
- `forest700`: #15803D
- `forest800`: #166534
- `forest900`: #14532D

**Secondary (Azul Céu)**:
- `sky50` até `sky900` (similar estrutura)

**Tertiary (Tons Terrosos)**:
- `earth50` até `earth900` (similar estrutura)

**Semânticos**:
- `success`, `warning`, `error`, `info` (variantes 50-900)

**Neutros (Light/Dark Mode)**:
- `neutral50` até `neutral900`

### Tipografia

**Fonte Primária**: Inter (com fallback system sans-serif)

**Escala Tipográfica**:
- **Display**: 64px/72px line height (bold)
- **Heading1**: 48px/56px (bold)
- **Heading2**: 36px/44px (semi-bold)
- **Heading3**: 24px/32px (semi-bold)
- **Heading4**: 20px/28px (semi-bold)
- **BodyLarge**: 18px/28px (regular)
- **BodyMedium**: 16px/24px (regular)
- **BodySmall**: 14px/20px (regular)
- **Label**: 14px/20px (medium)
- **Caption**: 12px/16px (regular)

### Espaçamento

**Base**: 8px (grid system)

**Padding/Margins**:
- `xs`: 4px
- `sm`: 8px
- `md`: 16px
- `lg`: 24px
- `xl`: 32px
- `xxl`: 48px

### Animações

**Durações**:
- `fast`: 150ms
- `base`: 250ms
- `slower`: 400ms

**Easing**: `Curves.easeInOutCubic` (natural, orgânico)

**Microinterações**:
- Like: Scale bounce (0.9 → 1.1 → 1.0)
- Pull-to-refresh: Circular progress + haptic feedback
- Badge: Fade in + scale (0.8 → 1.0)

### Componentes Principais

**Botões**:
- Primary: Verde floresta, elevation 2, padding 16px vertical
- Secondary: Outline, border verde floresta, transparente
- Text: Sem background, apenas texto

**Cards**:
- Padrão: Elevation 1, border radius 12px, padding 16px
- Glass: Glassmorphism (background blur, transparência)

**Inputs**:
- TextField com label flutuante, helper text, error states
- Border radius 8px, padding 12px vertical

### Dark Mode

**Tema Automático**: Seguir preferência do sistema
**Paleta Dark**: Cores invertidas (neutral900 → neutral50)
**Transição Suave**: Fade de 300ms entre temas

---

## 🔐 SEGURANÇA E AUTENTICAÇÃO

### Autenticação

**Métodos**:
1. Google Sign-In (OAuth2)
2. Apple Sign-In (iOS)
3. Email/Telefone (futuro)

**Fluxo**:
1. Usuário seleciona método de login
2. OAuth/autenticação social
3. Backend valida e retorna JWT
4. Token armazenado em `flutter_secure_storage`
5. Token incluído em todas as requisições (interceptor Dio)

**2FA (se habilitado)**:
1. Após login, verifica se 2FA habilitado
2. Mostra tela de código 2FA
3. Input de 6 dígitos
4. Verificação no backend

### Headers HTTP Obrigatórios

- **Authorization**: `Bearer {JWT_TOKEN}`
- **X-Session-Id**: ID da sessão territorial ativa
- **X-Latitude**: Latitude atual (quando disponível)
- **X-Longitude**: Longitude atual (quando disponível)
- **Content-Type**: `application/json`

### Segurança Avançada ⭐

**SSL/TLS Pinning** (Produção):
- Validar certificados SSL em todas as requisições HTTPS
- Prevenir man-in-the-middle attacks
- Configurar SHA256 hashes dos certificados

**Proteção de Dados Sensíveis**:
- Criptografar cache local (Hive encryption)
- Proteger screenshots em telas sensíveis (Android)
- Limpar dados sensíveis de memória após uso
- Nunca armazenar dados sensíveis em logs

**Jailbreak/Root Detection**:
- Detectar dispositivos comprometidos
- Mostrar alerta ao usuário (modo aviso para MVP)
- Considerar bloqueio parcial de funcionalidades (pós-MVP)

**Sanitização de Inputs**:
- Validar e sanitizar todos os inputs client-side
- Remover tags HTML perigosas
- Validar formatos (email, URL, etc.)
- Limitar tamanhos de strings

**Rate Limiting Local**:
- Prevenir spam antes de atingir rate limit da API
- Limites: Posts (10/min), Eventos (5/min), Comentários (20/min)

### Rate Limiting

**Tratamento de 429**:
1. Ler header `Retry-After`
2. Aguardar tempo especificado
3. Retry automático (max 3 tentativas)
4. Mostrar mensagem ao usuário se todas falharem

**Rate Limiting Local** (ver acima):
- Prevenir spam localmente
- Mostrar mensagem antes de enviar requisição

---

## 🌐 INTERNACIONALIZAÇÃO (i18n)

**Idiomas Suportados**:
- pt-BR (português brasileiro) - padrão
- en-US (inglês americano)

**Estrutura**:
```
lib/l10n/
├── app_pt.arb
├── app_en.arb
└── generated/
    └── app_localizations.dart
```

**Uso**:
```dart
AppLocalizations.of(context)!.welcomeMessage
```

---

## 📊 ESTADO E PROVIDERS (Riverpod)

### Providers Globais

1. **authProvider** (StateNotifier): Estado de autenticação
2. **sessionProvider** (StateNotifier): Território ativo (X-Session-Id)
3. **userProvider** (FutureProvider): Dados do usuário logado (`GET /auth/me`)
4. **territoryProvider** (FutureProvider): Dados do território ativo
5. **preferencesProvider** (StateNotifier): Preferências do usuário (tema, idioma)

### Providers por Feature

**Feed**:
- `feedProvider(territoryId)`: Lista de posts do território
- `postProvider(postId)`: Detalhes de um post
- `createPostProvider`: Criar novo post

**Territórios**:
- `territoriesNearbyProvider(lat, lng)`: Territórios próximos
- `territoryProvider(territoryId)`: Detalhes de um território

**Eventos**:
- `eventsProvider(territoryId)`: Lista de eventos
- `eventProvider(eventId)`: Detalhes de um evento

**Membresia**:
- `membershipProvider(territoryId)`: Vínculo do usuário no território
- `joinRequestProvider(territoryId)`: Solicitação de residência

### Padrão de Provider

```dart
// Exemplo: Feed Provider
final feedProvider = FutureProvider.family<List<Post>, String>((ref, territoryId) async {
  final apiService = ref.watch(apiServiceProvider);
  final sessionId = ref.watch(sessionProvider).sessionId;
  
  return apiService.getFeed(territoryId: territoryId, sessionId: sessionId);
});

// Exemplo: Criar Post (StateNotifier)
final createPostProvider = StateNotifierProvider<CreatePostNotifier, CreatePostState>((ref) {
  return CreatePostNotifier(ref.watch(apiServiceProvider));
});
```

---

## 🗺️ NAVEGAÇÃO (go_router)

### Estrutura de Rotas

```dart
final appRouter = GoRouter(
  initialLocation: '/splash',
  routes: [
    // Splash e Onboarding
    GoRoute(path: '/splash', builder: (context, state) => SplashScreen()),
    GoRoute(path: '/onboarding', builder: (context, state) => OnboardingScreen()),
    
    // Autenticação
    GoRoute(path: '/login', builder: (context, state) => LoginScreen()),
    GoRoute(path: '/register', builder: (context, state) => RegisterScreen()),
    
    // App Principal (Shell Route)
    ShellRoute(
      builder: (context, state, child) => MainShell(child: child),
      routes: [
        GoRoute(path: '/feed', builder: (context, state) => FeedScreen()),
        GoRoute(path: '/map', builder: (context, state) => MapScreen()),
        GoRoute(path: '/events', builder: (context, state) => EventsListScreen()),
        GoRoute(path: '/notifications', builder: (context, state) => NotificationsScreen()),
        GoRoute(path: '/profile', builder: (context, state) => ProfileScreen()),
      ],
    ),
    
    // Rotas Detalhadas
    GoRoute(path: '/post/:id', builder: (context, state) {
      final postId = state.pathParameters['id']!;
      return PostDetailScreen(postId: postId);
    }),
    
    // Rotas Protegidas (com redirect)
    GoRoute(
      path: '/create-post',
      builder: (context, state) => CreatePostScreen(),
      redirect: (context, state) {
        final isAuthenticated = context.read(authProvider).isAuthenticated;
        if (!isAuthenticated) return '/login';
        return null;
      },
    ),
  ],
  redirect: (context, state) {
    // Redirect global (ex: verificar autenticação)
    return null;
  },
);
```

### Deep Linking

**Suportar**:
- `Arah://territory/{id}` → TerritoryDetailScreen
- `Arah://post/{id}` → PostDetailScreen
- `Arah://event/{id}` → EventDetailScreen

---

## 📡 INTEGRAÇÃO COM API

### Base URL

**Desenvolvimento**: `http://localhost:5000/api/v1`
**Produção**: `https://api.Arah.app/api/v1`

### Endpoints Principais (ver OpenAPI)

**Autenticação**:
- `POST /auth/social-login` - Login social
- `GET /auth/me` - Usuário logado
- `POST /auth/refresh` - Refresh token

**Territórios**:
- `GET /territories/nearby` - Territórios próximos
- `GET /territories/{id}` - Detalhes do território
- `POST /territories/selection` - Selecionar território ativo

**Feed**:
- `GET /feed` - Lista de posts (filtrado por território ativo)
- `POST /feed` - Criar post
- `GET /feed/{id}` - Detalhes do post
- `POST /feed/{id}/likes` - Curtir post
- `POST /feed/{id}/comments` - Comentar

**Eventos**:
- `GET /events` - Lista de eventos
- `POST /events` - Criar evento
- `GET /events/{id}` - Detalhes do evento
- `POST /events/{id}/register` - Inscrever-se

**Membresia**:
- `POST /territories/{id}/enter` - Entrar como VISITOR
- `POST /memberships/{territoryId}/become-resident` - Solicitar residência
- `GET /memberships/{territoryId}/me` - Meu vínculo

**Moderação**:
- `POST /reports` - Reportar conteúdo/usuário
- `GET /moderation/cases` - Casos de moderação (CURATOR/MODERATOR)

### Models (gerados do OpenAPI)

Todos os models devem ser gerados automaticamente a partir do `openapi.json` usando:
1. `openapi-generator` para gerar schemas Dart
2. `json_serializable` para fromJson/toJson

**Exemplos de Models**:
- `User`, `Territory`, `TerritoryMembership`
- `Post`, `Event`, `Comment`, `Like`
- `Report`, `ModerationCase`
- `Notification`, `JoinRequest`

---

## 🎯 FUNCIONALIDADES POR DOMÍNIO

### 1. Autenticação e Onboarding

**Onboarding**:
1. Splash screen (logo, 800ms fade-in)
2. Boas-vindas (se primeiro acesso)
3. Permissão de localização (com explicação de benefícios)
4. Autenticação social (Google, Apple)
5. Coleta de dados adicionais (nome, CPF/documento, telefone, endereço opcional)
6. Verificação 2FA (se habilitado)
7. Descoberta de territórios

**Autenticação**:
- Login social com OAuth2
- Armazenamento seguro de token
- Refresh automático de token
- Logout com limpeza de dados

### 2. Territórios

**Funcionalidades**:
- Buscar territórios próximos (raio 25km)
- Visualizar detalhes do território
- Selecionar território ativo
- Entrar como VISITOR ou solicitar RESIDENT
- Verificar status de vínculo

**Telas**:
- `TerritoryListScreen`: Lista de territórios próximos (com mapa miniaturizado)
- `TerritoryDetailScreen`: Detalhes + ações (entrar, solicitar residência)
- `TerritorySelectionScreen`: Seletor de território ativo (se múltiplos vínculos)

### 3. Feed Territorial

**Funcionalidades**:
- Visualizar feed do território (PUBLIC + RESIDENTS_ONLY se RESIDENT)
- Paginação infinita (20 posts por página)
- Pull-to-refresh
- Filtros (tipo, data, localização)
- Interações (like, comentário, compartilhamento)

**Visibilidade**:
- VISITOR: Apenas posts PUBLIC
- RESIDENT: Posts PUBLIC + RESIDENTS_ONLY

**Telas**:
- `FeedScreen`: Lista de posts (scroll infinito)
- `PostDetailScreen`: Detalhes completos + comentários

### 4. Criação de Posts

**Funcionalidades**:
- Criar post (título opcional, conteúdo obrigatório)
- Tipo: NOTICE, ALERT, ANNOUNCEMENT
- Visibilidade: PUBLIC, RESIDENTS_ONLY
- Adicionar mídias (imagens/vídeos, até 10)
- GeoAnchor automático (derivado de mídias com EXIF)
- Preview antes de publicar

**Validações**:
- Conteúdo mínimo 10 caracteres, máximo 4000
- Título máximo 200 caracteres
- Apenas RESIDENT pode criar posts RESIDENTS_ONLY
- Sanções de posting respeitadas

**Tela**:
- `CreatePostScreen`: Formulário multi-step com preview

### 5. Mapa Territorial

**Funcionalidades**:
- Visualizar mapa do território (polígono destacado)
- Pins de posts/eventos/assets georreferenciados
- Clustering de pins próximos
- Filtros por tipo (posts, eventos, alertas, assets)
- Bottom sheet ao tocar em pin
- Navegação para detalhes do item

**Pins Coloridos**:
- Post PUBLIC: Verde claro
- Evento PUBLIC: Azul
- Alerta PUBLIC: Laranja
- Asset PUBLIC: Terroso

**Tela**:
- `MapScreen`: Mapa Google Maps em tela cheia

### 6. Eventos

**Funcionalidades**:
- Listar eventos do território
- Criar evento (apenas RESIDENT ou EVENT_ORGANIZER)
- Visualizar detalhes do evento
- Inscrever-se em evento
- Check-in no evento (quando próximo)
- Adicionar ao calendário

**Telas**:
- `EventsListScreen`: Lista de eventos (filtros: todos, próximos, acontecendo agora, passados)
- `EventDetailScreen`: Detalhes completos + ações
- `CreateEventScreen`: Formulário multi-step (informações, data/hora, localização, mídia)

### 7. Membresia (Vínculos)

**Funcionalidades**:
- Entrar como VISITOR (imediato)
- Solicitar residência (PENDING até aprovação)
- Verificar status de solicitação
- Upload de documento (opcional)
- Verificação por geolocalização

**Telas**:
- `BecomeResidentScreen`: Formulário de solicitação (verificação geo, documento, mensagem)
- `MembershipStatusScreen`: Status da solicitação (PENDING, APPROVED, REJECTED)

### 8. Perfil

**Funcionalidades**:
- Visualizar perfil próprio
- Editar perfil (nome, bio, foto, preferências)
- Visualizar perfil de outros usuários
- Ver posts/eventos do usuário
- Seguir usuário (futuro)

**Telas**:
- `ProfileScreen`: Perfil próprio (editável)
- `OtherUserProfileScreen`: Perfil de outro usuário (somente leitura)

### 9. Moderação

**Funcionalidades**:
- Reportar post/evento/usuário
- Revisar reports (CURATOR/MODERATOR)
- Aplicar sanções (territoriais ou globais)
- Bloquear/desbloquear usuários
- Ocultar/restaurar conteúdo

**Telas**:
- `ModerationDashboardScreen`: Dashboard de moderação (casos pendentes, estatísticas)
- `ReportDetailScreen`: Detalhes do report + ações

### 10. Curadoria (CURATOR)

**Funcionalidades**:
- Aprovar/rejeitar solicitações de residência
- Validar assets territoriais
- Gerenciar work queue
- Criar votações comunitárias
- Configurar feature flags territoriais

**Telas**:
- `CuratorDashboardScreen`: Dashboard de curadoria
- `WorkQueueScreen`: Fila de itens para revisão

### 11. Notificações

**Funcionalidades**:
- Listar notificações (in-app)
- Marcar como lida
- Preferências de notificação (por tipo)
- **Push Notifications ⭐**: FCM/APNs com deep linking
- **Notificações Locais**: Para lembrete de eventos, etc.
- **Background Sync**: Sincronizar notificações em background

**Tipos de Notificações Push**:
- Novo post no território
- Comentário no seu post
- Evento próximo/começando
- Solicitação de residência aprovada/rejeitada
- Report resolvido
- Mensagem de chat

**Tela**:
- `NotificationsScreen`: Lista de notificações (pull-to-refresh, paginação)

**Configuração**:
- Solicitar permissão de notificações
- Configurar preferências por tipo
- Silenciar por horário (DND)

### 11.1 Modo Offline ⭐

**Funcionalidades**:
- Cache local inteligente (Hive)
- Sincronização automática ao voltar online
- Fila offline para ações pendentes
- Indicador visual de modo offline
- Resolução de conflitos

**Dados Cacheados**:
- Feed (últimos 50 posts)
- Eventos (próximos 30 eventos)
- Territórios (territórios recentes)
- Perfil (próprio e vínculos)

**Ações Offline**:
- Criar post (salvar localmente)
- Comentar (salvar localmente)
- Curtir (salvar localmente)
- Visualizar conteúdo cacheado

**Sincronização**:
- Detectar retorno de conexão (`connectivity_plus`)
- Processar fila offline automaticamente
- Notificar usuário sobre sincronização
- Resolver conflitos (last-write-wins ou diálogo)

### 11.2 Background Tasks ⭐

**Funcionalidades**:
- **Background Fetch**: Atualizar feed periodicamente
- **Background Sync**: Sincronizar dados em background
- **Background Upload**: Upload de mídias em background
- **Retry Automático**: Retry de operações falhadas

**Tarefas Periódicas**:
- Sincronizar feed: A cada 15 minutos (se online)
- Sincronizar notificações: A cada 30 minutos (se online)
- Sincronizar fila offline: Quando online

**Configuração**:
- `workmanager` para tarefas periódicas
- Constraints: NetworkType.connected, requiresBatteryNotLow
- Frequência mínima: 15 minutos (iOS)

### 11.3 Dynamic Links / Universal Links ⭐

**Funcionalidades**:
- Compartilhar posts/eventos/territórios com links dinâmicos
- Links funcionam mesmo se app não instalado (redirecionam para loja)
- Links funcionam na web (redirecionam para app se instalado)
- Deep linking automático ao tocar em link

**Implementação**:
- Firebase Dynamic Links
- Universal Links (iOS) via `apple-app-site-association`
- App Links (Android) via `assetlinks.json`

**Tipos de Links**:
- `https://Arah.page.link/post/{id}` → PostDetailScreen
- `https://Arah.page.link/event/{id}` → EventDetailScreen
- `https://Arah.page.link/territory/{id}` → TerritoryDetailScreen

### 11.4 Recuperação de Conta ⭐

**Funcionalidades**:
- Recuperar acesso via email/telefone
- Recuperar código 2FA
- Reset de método de autenticação
- Código de recuperação (6 dígitos, válido por 15 minutos)

**Fluxo**:
1. Usuário solicita recuperação
2. Recebe código por email/SMS
3. Insere código
4. Redefine acesso (novo método de autenticação, reset 2FA)

**Tela**:
- `AccountRecoveryScreen`: Solicitar recuperação
- `RecoveryCodeScreen`: Inserir código
- `ResetAccessScreen`: Redefinir acesso

### 11.5 Exclusão de Conta (LGPD/GDPR) ⭐

**Funcionalidades**:
- Excluir conta e dados pessoais
- Exportar dados antes de excluir
- Período de graça (7 dias) para cancelar exclusão
- Anonimização/deleção permanente após período de graça

**Fluxo**:
1. Usuário solicita exclusão nas configurações
2. Opção de exportar dados (JSON)
3. Confirmação dupla ("Tem certeza?", digitar "EXCLUIR")
4. Período de graça (7 dias)
5. Exclusão permanente após período

**Tela**:
- `DeleteAccountScreen`: Configurar exclusão
- `AccountDeletionPendingScreen`: Período de graça
- `ExportDataScreen`: Exportar dados (se solicitado)

### 12. Marketplace (se feature flag habilitada)

**Funcionalidades**:
- Listar items (ofertas/demandas)
- Criar item (apenas RESIDENT)
- Visualizar detalhes do item
- Contactar vendedor
- Gerenciar carrinho (se implementado)

### 13. Chat (se feature flag habilitada)

**Funcionalidades**:
- Canais territoriais
- Grupos
- Mensagens diretas (DM)
- Notificações de mensagens

### 14. Admin (SYSTEM_ADMIN)

**Funcionalidades**:
- Gerenciar territórios
- Gerenciar usuários globais
- Configurar sistema
- Monitorar work queue global
- Dashboard administrativo

---

## 📝 PADRÕES DE CÓDIGO

### Nomenclatura

- **Arquivos**: `snake_case.dart`
- **Classes**: `PascalCase`
- **Variáveis/Funções**: `camelCase`
- **Constantes**: `SCREAMING_SNAKE_CASE`
- **Private**: `_leadingUnderscore`

### Estrutura de Widget

```dart
class MyWidget extends ConsumerWidget {
  const MyWidget({super.key, required this.param});
  
  final String param;
  
  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final state = ref.watch(someProvider);
    
    return Scaffold(
      appBar: AppBar(title: const Text('Title')),
      body: _buildBody(context, ref, state),
    );
  }
  
  Widget _buildBody(BuildContext context, WidgetRef ref, SomeState state) {
    if (state.isLoading) {
      return const LoadingIndicator();
    }
    
    if (state.hasError) {
      return ErrorWidget(error: state.error);
    }
    
    return _buildContent(context, ref, state);
  }
  
  Widget _buildContent(BuildContext context, WidgetRef ref, SomeState state) {
    // Implementação
  }
}
```

### Error Handling

```dart
try {
  final result = await apiService.getData();
  // Processar resultado
} on DioException catch (e) {
  if (e.response?.statusCode == 401) {
    // Redirecionar para login
  } else if (e.response?.statusCode == 429) {
    // Rate limiting - aguardar e retry
  } else {
    // Outro erro - mostrar mensagem
  }
} catch (e, stackTrace) {
  // Capturar exceção não esperada
  ExceptionService.captureException(e, stackTrace: stackTrace);
}
```

### Logging

```dart
LoggingService.logApi(
  'GET /api/v1/feed completed',
  params: {'territory_id': territoryId, 'status_code': 200},
);

LoggingService.logError(
  'Failed to load feed',
  error: e,
  stackTrace: stackTrace,
);
```

### Métricas

```dart
MetricsService.trackScreenView('FeedScreen');
MetricsService.trackUserAction('post_created', {'territory_id': territoryId});
MetricsService.trackEngagement('post_created', territoryId: territoryId);
```

---

## ✅ CHECKLIST DE IMPLEMENTAÇÃO

### Setup Inicial
- [ ] Criar projeto Flutter
- [ ] Configurar dependências (pubspec.yaml) - **incluir todas as novas dependências de segurança e background**
- [ ] Configurar estrutura de pastas
- [ ] Configurar tema Material 3 (light/dark)
- [ ] Configurar go_router
- [ ] Configurar Riverpod
- [ ] Configurar Dio (interceptors) - **incluir SSL pinning em produção**
- [ ] Configurar Firebase (Analytics, Crashlytics, Performance, Messaging, Dynamic Links)
- [ ] Configurar Sentry
- [ ] Configurar i18n
- [ ] Gerar models do OpenAPI
- [ ] Configurar autenticação biométrica ⭐
- [ ] Configurar push notifications ⭐
- [ ] Configurar background tasks ⭐
- [ ] Configurar modo offline ⭐

### Funcionalidades Core
- [ ] Autenticação (login social, 2FA, biométrica ⭐)
- [ ] Onboarding
- [ ] Descoberta de territórios
- [ ] Seleção de território ativo
- [ ] Vínculo VISITOR/RESIDENT
- [ ] Feed territorial
- [ ] Criação de posts
- [ ] Mapa territorial
- [ ] Eventos
- [ ] Perfil
- [ ] Notificações (in-app + push ⭐)
- [ ] Modo offline ⭐
- [ ] Recuperação de conta ⭐
- [ ] Exclusão de conta (LGPD/GDPR) ⭐

### Funcionalidades Avançadas
- [ ] Moderação
- [ ] Curadoria (CURATOR)
- [ ] Marketplace (se flag habilitada)
- [ ] Chat (se flag habilitada)
- [ ] Admin (SYSTEM_ADMIN)

### Qualidade
- [ ] Testes unitários (críticos)
- [ ] Testes de widgets (principais)
- [ ] Testes de integração (fluxos principais)
- [ ] Testes de acessibilidade (WCAG AA) ⭐
- [ ] Testes de segurança (biometria, SSL pinning, jailbreak detection) ⭐
- [ ] Testes de modo offline ⭐
- [ ] Testes de push notifications ⭐
- [ ] Performance (FPS, memory, startup time)
- [ ] Observabilidade (métricas, logs, exceções)

---

## 🎯 PRIORIDADES DE IMPLEMENTAÇÃO

### Fase 1: MVP Core (P0)
1. Autenticação e onboarding
2. Descoberta e seleção de territórios
3. Feed territorial (visualização)
4. Mapa territorial (visualização)
5. Perfil básico

### Fase 2: Interações (P1)
1. Criação de posts
2. Interações (like, comentário)
3. Eventos (listar e criar)
4. Notificações

### Fase 3: Avançado (P2)
1. Moderação
2. Curadoria
3. Marketplace
4. Chat

---

## 📚 REFERÊNCIAS DA DOCUMENTAÇÃO

Toda a documentação detalhada está em:

### Documentação de Planejamento e Design
- **`docs/24_FLUTTER_FRONTEND_PLAN.md`**: Planejamento completo
- **`docs/25_FLUTTER_IMPLEMENTATION_ROADMAP.md`**: Roadmap por fases
- **`docs/26_FLUTTER_DESIGN_GUIDELINES.md`**: Design system completo
- **`docs/27_USER_JOURNEYS_MAP.md`**: Jornadas do usuário

### Documentação Técnica Complementar
- **`docs/28_FLUTTER_METRICS_LOGGING_EXCEPTIONS.md`**: Observabilidade completa
- **`docs/30_FLUTTER_TESTING_STRATEGY.md`**: Estratégia de testes (unit, widget, integration, golden)
- **`docs/31_FLUTTER_ACCESSIBILITY_GUIDE.md`**: Guia completo de acessibilidade (WCAG AA)
- **`docs/32_FLUTTER_I18N_GUIDE.md`**: Guia completo de internacionalização (pt-BR, en-US)

### Documentação de Backend e API
- **`docs/60_API_LÓGICA_NEGÓCIO.md`**: Lógica de negócio da API
- **`backend/Arah.Api/wwwroot/devportal/openapi.json`**: Especificação OpenAPI

---

## 🚀 INSTRUÇÕES FINAIS

**VOCÊ DEVE**:
1. Seguir TODA a estrutura de pastas especificada
2. Implementar TODAS as funcionalidades listadas
3. Usar EXATAMENTE as cores, tipografia e espaçamentos do design system
4. Implementar TODAS as animações e microinterações especificadas
5. Seguir TODOS os padrões de código e convenções
6. Implementar observabilidade completa (métricas, logs, exceções)
7. Garantir acessibilidade (WCAG AA)
8. Implementar dark mode completo
9. Implementar i18n (pt-BR, en-US)
10. Criar código limpo, testável e manutenível (Clean Architecture)

**VOCÊ NÃO DEVE**:
1. Desviar da estrutura de pastas especificada
2. Ignorar funcionalidades obrigatórias (incluindo as novas ⭐)
3. Usar cores/estilos diferentes do design system
4. Pular validações de segurança (incluindo sanitização e SSL pinning)
5. Ignorar tratamento de erros
6. Criar código não null-safe
7. Pular observabilidade (logs, métricas, exceções)
8. **Armazenar dados sensíveis em cache não criptografado** ⭐
9. **Ignorar proteção contra dispositivos comprometidos** ⭐
10. **Pular modo offline** ⭐ (crítico para experiência do usuário)
11. **Ignorar push notifications** ⭐ (crítico para engajamento)

**QUALIDADE ESPERADA**:
- Código corporate-level high-end
- Design pixel-perfect conforme diretrizes
- Performance otimizada (60 FPS, startup < 3s)
- Acessibilidade completa (WCAG AA)
- Testes automatizados (cobertura crítica)
- Documentação inline (doc comments)

---

**Este prompt contém TODAS as informações necessárias para criar o app Flutter Arah completo e profissional. Use este documento como referência única e completa para desenvolvimento.**
