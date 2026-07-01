# Planejamento do Frontend Flutter - Arah Mobile App

**Versão**: 1.0  
**Data**: 2025-01-20  
**Status**: 📋 Planejamento  
**Tipo**: Documentação de Arquitetura e Especificação Técnica

---

## 📋 Índice

1. [Contexto do Projeto](#contexto-do-projeto)
2. [Stack Tecnológica](#stack-tecnológica)
3. [Estrutura do Projeto](#estrutura-do-projeto)
4. [Funcionalidades por Domínio](#funcionalidades-por-domínio)
5. [Design System e UX](#design-system-e-ux)
6. [Segurança e Autenticação](#segurança-e-autenticação)
7. [Navegação e Roteamento](#navegação-e-roteamento)
8. [Internacionalização](#internacionalização)
9. [Gerenciamento de Estado](#gerenciamento-de-estado)
10. [Testes](#testes)
11. [Dependências](#dependências)
12. [Instruções de Implementação](#instruções-de-implementação)

---

## 🎯 Contexto do Projeto

O **Arah** é uma plataforma **território-first** e **comunidade-first** para organização comunitária local, onde:

- O **território físico é a unidade central** de organização
- A **presença física é critério de vínculo** (geolocalização obrigatória para residentes)
- Existe diferenciação clara entre **VISITOR** (visitante) e **RESIDENT** (morador)
- A governança é baseada em **Capabilities** (Curator, Moderator, EventOrganizer)
- **Feature flags territoriais** ativam/desativam funcionalidades
- Tudo é **georreferenciado** e conectado ao mapa territorial

Este documento especifica o planejamento completo para desenvolvimento do **app mobile Flutter** que consome a API Arah (`/api/v1/*`), cobrindo todos os domínios funcionais do projeto.

### Princípios Fundamentais da Aplicação

- **Território é geográfico e neutro**: Representa apenas um lugar físico real
- **Consulta exige cadastro**: Feed, mapa e operações sociais exigem usuário autenticado
- **Presença física é critério de vínculo**: No MVP, não é possível associar território remotamente
- **Visibilidade diferenciada**: Conteúdo pode ser Público (todos) ou Apenas Moradores (RESIDENTS_ONLY)

---

## 🧱 Stack Tecnológica

### Core

- **Flutter**: 3.19.0 ou superior (stable channel)
- **Dart**: 3.3.0 ou superior
- **Material Design 3**: Suporte completo a tema claro/escuro automático
- **Null Safety**: Todo código deve ser null-safe

### Navegação e Roteamento

- **go_router**: ^14.0.0
  - Roteamento declarativo com suporte a deep linking
  - Transições customizadas entre telas
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
  - Timeout configurável por tipo de endpoint

- **dio_cache_interceptor**: Cache HTTP com TTL
- **connectivity_plus**: Verificação de conectividade

### Serialização JSON

- **json_serializable**: ^6.7.0 + **json_annotation**: ^4.8.0
- **build_runner**: Geração automática de fromJson/toJson
- Todos os models devem ser gerados automaticamente a partir do OpenAPI

### Persistência Local

- **shared_preferences**: Preferências do usuário (tema, idioma, território selecionado)
- **hive**: ^2.2.3 + **hive_flutter**: Cache local de dados (posts, territórios, perfil)
- **flutter_secure_storage**: Armazenamento seguro de tokens JWT

### Geolocalização

- **geolocator**: ^10.1.0 (obter localização do usuário)
- **geocoding**: ^2.1.1 (geocodificação reversa para endereços)
- **google_maps_flutter**: ^2.5.0 (mapa territorial integrado)
- Permissões de localização com explicação contextual
- Fallback quando permissão negada (mapa sem pins do usuário)

### UI/UX e Componentes

- **flutter_slidable**: Cards deslizáveis para ações rápidas
- **pull_to_refresh**: Refresh manual de feeds
- **infinite_scroll_pagination**: Paginação infinita em listas
- **cached_network_image**: Cache de imagens de rede
- **flutter_markdown**: Renderização de markdown em posts
- **url_launcher**: Abrir links externos
- **share_plus**: Compartilhamento de conteúdo
- **image_picker**: Seleção de fotos para upload
- **file_picker**: Seleção de documentos para evidências
- **flutter_map**: Alternativa leve para mapas (opcional)

### Notificações

- **firebase_messaging**: Push notifications (se configurado no backend)
- **flutter_local_notifications**: Notificações locais
- Badge de contador de notificações não lidas

### Validação e Formulários

- **reactive_forms**: ^16.1.1 (formulários reativos com validação)
- Validação de CPF brasileiro
- Validação de coordenadas geográficas
- Validação de formatos de data/hora

### Internacionalização

- **flutter_localizations**: Suporte a pt-BR e en-US
- **intl**: Formatação de datas, números, moedas
- Arquivos de tradução organizados por feature

### Acessibilidade

- **flutter/semantics**: Semântica completa para TalkBack/VoiceOver
- Contraste adequado (WCAG AA mínimo)
- Tamanhos de fonte escaláveis
- Suporte a modo de alto contraste

### Utilitários

- **equatable**: Comparação de objetos
- **freezed**: Immutable data classes (opcional, mas recomendado)
- **uuid**: Geração de UUIDs
- **timeago**: Formatação de "há X minutos"

---

## 📁 Estrutura do Projeto

Organização por **features** (arquitetura vertical), seguindo Clean Architecture com separação de responsabilidades.

```
lib/
├── main.dart                          # Entry point + ProviderScope
├── app.dart                           # MaterialApp com tema, rotas, localização
│
├── core/                              # Infraestrutura compartilhada
│   ├── config/
│   │   ├── app_config.dart           # Configurações da app (API base URL, env)
│   │   └── constants.dart            # Constantes (cores, dimensões, strings)
│   ├── network/
│   │   ├── dio_client.dart           # Cliente Dio configurado com interceptors
│   │   ├── interceptors/
│   │   │   ├── auth_interceptor.dart # JWT Bearer token
│   │   │   ├── geo_interceptor.dart  # X-Latitude/X-Longitude
│   │   │   ├── session_interceptor.dart # X-Session-Id
│   │   │   ├── retry_interceptor.dart
│   │   │   ├── rate_limit_interceptor.dart # Trata 429
│   │   │   └── logging_interceptor.dart
│   │   ├── api_client.dart           # Cliente base abstrato
│   │   └── api_exception.dart        # Exceções customizadas
│   ├── storage/
│   │   ├── secure_storage_service.dart # Tokens JWT
│   │   ├── local_storage_service.dart  # Hive/SharedPreferences
│   │   └── cache_service.dart         # Cache de dados
│   ├── utils/
│   │   ├── validators.dart           # Validações (CPF, email, geo)
│   │   ├── formatters.dart           # Formatação de dados
│   │   ├── date_utils.dart
│   │   └── geo_utils.dart
│   └── widgets/
│       ├── error_widget.dart         # Widget de erro genérico
│       ├── loading_widget.dart
│       ├── empty_state_widget.dart
│       └── retry_button.dart
│
├── features/                          # Features organizadas verticalmente
│   │
│   ├── auth/                          # Autenticação e sessão
│   │   ├── data/
│   │   │   ├── models/
│   │   │   │   ├── user_model.dart
│   │   │   │   ├── auth_request.dart
│   │   │   │   └── auth_response.dart
│   │   │   ├── repositories/
│   │   │   │   └── auth_repository.dart
│   │   │   └── datasources/
│   │   │       ├── auth_remote_datasource.dart
│   │   │       └── auth_local_datasource.dart
│   │   ├── domain/
│   │   │   ├── entities/
│   │   │   │   └── user_entity.dart
│   │   │   └── repositories/
│   │   │       └── auth_repository_interface.dart
│   │   ├── presentation/
│   │   │   ├── providers/
│   │   │   │   ├── auth_provider.dart
│   │   │   │   └── current_user_provider.dart
│   │   │   ├── screens/
│   │   │   │   ├── login_screen.dart
│   │   │   │   ├── register_screen.dart
│   │   │   │   ├── two_factor_setup_screen.dart
│   │   │   │   └── two_factor_verify_screen.dart
│   │   │   └── widgets/
│   │   │       ├── social_login_button.dart
│   │   │       └── cpf_input_field.dart
│   │   └── routes.dart
│   │
│   ├── onboarding/                    # Onboarding e descoberta
│   │   ├── presentation/
│   │   │   ├── screens/
│   │   │   │   ├── welcome_screen.dart
│   │   │   │   ├── location_permission_screen.dart
│   │   │   │   ├── territory_discovery_screen.dart
│   │   │   │   └── territory_selection_screen.dart
│   │   │   └── widgets/
│   │   │       └── territory_card.dart
│   │   └── routes.dart
│   │
│   ├── territories/                   # Territórios e vínculos
│   │   ├── data/
│   │   │   ├── models/
│   │   │   │   ├── territory_model.dart
│   │   │   │   ├── membership_model.dart
│   │   │   │   ├── membership_capability_model.dart
│   │   │   │   ├── membership_settings_model.dart
│   │   │   │   └── feature_flag_model.dart
│   │   │   ├── repositories/
│   │   │   │   └── territory_repository.dart
│   │   │   └── datasources/
│   │   │       ├── territory_remote_datasource.dart
│   │   │       └── territory_local_datasource.dart
│   │   ├── domain/
│   │   │   ├── entities/
│   │   │   │   ├── territory_entity.dart
│   │   │   │   └── membership_entity.dart
│   │   │   └── repositories/
│   │   │       └── territory_repository_interface.dart
│   │   ├── presentation/
│   │   │   ├── providers/
│   │   │   │   ├── territories_provider.dart
│   │   │   │   ├── current_territory_provider.dart
│   │   │   │   ├── memberships_provider.dart
│   │   │   │   └── nearby_territories_provider.dart
│   │   │   ├── screens/
│   │   │   │   ├── territories_list_screen.dart
│   │   │   │   ├── territory_detail_screen.dart
│   │   │   │   ├── territory_map_screen.dart
│   │   │   │   ├── membership_status_screen.dart
│   │   │   │   └── become_resident_screen.dart
│   │   │   └── widgets/
│   │   │       ├── territory_card.dart
│   │   │       ├── membership_badge.dart
│   │   │       ├── capability_badge.dart
│   │   │       └── feature_flag_indicator.dart
│   │   └── routes.dart
│   │
│   ├── feed/                          # Feed comunitário
│   │   ├── data/
│   │   │   ├── models/
│   │   │   │   ├── post_model.dart
│   │   │   │   ├── post_type.dart
│   │   │   │   ├── post_visibility.dart
│   │   │   │   ├── post_status.dart
│   │   │   │   └── post_geo_anchor.dart
│   │   │   ├── repositories/
│   │   │   │   └── feed_repository.dart
│   │   │   └── datasources/
│   │   │       ├── feed_remote_datasource.dart
│   │   │       └── feed_local_datasource.dart
│   │   ├── domain/
│   │   │   ├── entities/
│   │   │   │   └── post_entity.dart
│   │   │   └── repositories/
│   │   │       └── feed_repository_interface.dart
│   │   ├── presentation/
│   │   │   ├── providers/
│   │   │   │   ├── feed_provider.dart
│   │   │   │   ├── personal_feed_provider.dart
│   │   │   │   ├── territory_feed_provider.dart
│   │   │   │   └── post_detail_provider.dart
│   │   │   ├── screens/
│   │   │   │   ├── feed_screen.dart
│   │   │   │   ├── post_detail_screen.dart
│   │   │   │   └── create_post_screen.dart
│   │   │   └── widgets/
│   │   │       ├── post_card.dart
│   │   │       ├── post_header.dart
│   │   │       ├── post_content.dart
│   │   │       ├── post_actions.dart
│   │   │       └── post_type_selector.dart
│   │   └── routes.dart
│   │
│   ├── events/                        # Eventos territoriais
│   │   ├── data/
│   │   │   ├── models/
│   │   │   │   ├── event_model.dart
│   │   │   │   ├── event_type.dart
│   │   │   │   ├── event_status.dart
│   │   │   │   └── event_participant.dart
│   │   │   └── ...
│   │   ├── domain/
│   │   │   └── ...
│   │   ├── presentation/
│   │   │   ├── providers/
│   │   │   │   ├── events_provider.dart
│   │   │   │   ├── nearby_events_provider.dart
│   │   │   │   └── event_detail_provider.dart
│   │   │   ├── screens/
│   │   │   │   ├── events_list_screen.dart
│   │   │   │   ├── event_detail_screen.dart
│   │   │   │   ├── create_event_screen.dart
│   │   │   │   └── event_map_screen.dart
│   │   │   └── widgets/
│   │   │       ├── event_card.dart
│   │   │       ├── event_participants_list.dart
│   │   │       └── event_date_picker.dart
│   │   └── routes.dart
│   │
│   ├── map/                           # Mapa territorial integrado
│   │   ├── data/
│   │   │   ├── models/
│   │   │   │   ├── map_entity_model.dart
│   │   │   │   ├── map_pin_model.dart
│   │   │   │   └── map_entity_type.dart
│   │   │   └── ...
│   │   ├── domain/
│   │   │   └── ...
│   │   ├── presentation/
│   │   │   ├── providers/
│   │   │   │   ├── map_entities_provider.dart
│   │   │   │   ├── map_pins_provider.dart
│   │   │   │   └── user_location_provider.dart
│   │   │   ├── screens/
│   │   │   │   ├── map_screen.dart
│   │   │   │   └── map_entity_detail_screen.dart
│   │   │   └── widgets/
│   │   │       ├── map_widget.dart
│   │   │       ├── map_pin.dart
│   │   │       └── map_cluster.dart
│   │   └── routes.dart
│   │
│   ├── market/                        # Marketplace territorial
│   │   ├── data/
│   │   │   ├── models/
│   │   │   │   ├── store_model.dart
│   │   │   │   ├── store_item_model.dart
│   │   │   │   ├── item_inquiry_model.dart
│   │   │   │   ├── cart_model.dart
│   │   │   │   └── checkout_model.dart
│   │   │   └── ...
│   │   ├── domain/
│   │   │   └── ...
│   │   ├── presentation/
│   │   │   ├── providers/
│   │   │   │   ├── stores_provider.dart
│   │   │   │   ├── store_items_provider.dart
│   │   │   │   ├── cart_provider.dart
│   │   │   │   └── inquiries_provider.dart
│   │   │   ├── screens/
│   │   │   │   ├── stores_list_screen.dart
│   │   │   │   ├── store_detail_screen.dart
│   │   │   │   ├── item_detail_screen.dart
│   │   │   │   ├── cart_screen.dart
│   │   │   │   └── checkout_screen.dart
│   │   │   └── widgets/
│   │   │       ├── store_card.dart
│   │   │       ├── item_card.dart
│   │   │       └── cart_item_widget.dart
│   │   └── routes.dart
│   │
│   ├── alerts/                        # Alertas de saúde territorial
│   │   ├── data/
│   │   │   ├── models/
│   │   │   │   ├── health_alert_model.dart
│   │   │   │   └── alert_type.dart
│   │   │   └── ...
│   │   ├── domain/
│   │   │   └── ...
│   │   ├── presentation/
│   │   │   ├── providers/
│   │   │   │   └── alerts_provider.dart
│   │   │   ├── screens/
│   │   │   │   ├── alerts_list_screen.dart
│   │   │   │   └── alert_detail_screen.dart
│   │   │   └── widgets/
│   │   │       └── alert_card.dart
│   │   └── routes.dart
│   │
│   ├── chat/                          # Chat (canais, grupos, DM)
│   │   ├── data/
│   │   │   ├── models/
│   │   │   │   ├── chat_conversation_model.dart
│   │   │   │   ├── chat_message_model.dart
│   │   │   │   ├── conversation_participant_model.dart
│   │   │   │   └── conversation_kind.dart
│   │   │   └── ...
│   │   ├── domain/
│   │   │   └── ...
│   │   ├── presentation/
│   │   │   ├── providers/
│   │   │   │   ├── conversations_provider.dart
│   │   │   │   └── messages_provider.dart
│   │   │   ├── screens/
│   │   │   │   ├── conversations_list_screen.dart
│   │   │   │   ├── chat_screen.dart
│   │   │   │   └── create_group_screen.dart
│   │   │   └── widgets/
│   │   │       ├── conversation_item.dart
│   │   │       ├── message_bubble.dart
│   │   │       └── message_input.dart
│   │   └── routes.dart
│   │
│   ├── moderation/                    # Moderação e curadoria
│   │   ├── data/
│   │   │   ├── models/
│   │   │   │   ├── report_model.dart
│   │   │   │   ├── sanction_model.dart
│   │   │   │   ├── work_item_model.dart
│   │   │   │   └── moderation_case_model.dart
│   │   │   └── ...
│   │   ├── domain/
│   │   │   └── ...
│   │   ├── presentation/
│   │   │   ├── providers/
│   │   │   │   ├── reports_provider.dart
│   │   │   │   ├── work_items_provider.dart
│   │   │   │   └── moderation_queue_provider.dart
│   │   │   ├── screens/
│   │   │   │   ├── moderation_dashboard_screen.dart
│   │   │   │   ├── reports_list_screen.dart
│   │   │   │   ├── work_items_queue_screen.dart
│   │   │   │   └── moderation_detail_screen.dart
│   │   │   └── widgets/
│   │   │       ├── report_card.dart
│   │   │       └── work_item_card.dart
│   │   └── routes.dart
│   │
│   ├── profile/                       # Perfil do usuário
│   │   ├── data/
│   │   │   ├── models/
│   │   │   │   ├── user_preferences_model.dart
│   │   │   │   └── user_profile_model.dart
│   │   │   └── ...
│   │   ├── domain/
│   │   │   └── ...
│   │   ├── presentation/
│   │   │   ├── providers/
│   │   │   │   ├── user_profile_provider.dart
│   │   │   │   └── user_preferences_provider.dart
│   │   │   ├── screens/
│   │   │   │   ├── profile_screen.dart
│   │   │   │   ├── edit_profile_screen.dart
│   │   │   │   ├── settings_screen.dart
│   │   │   │   └── verification_screen.dart
│   │   │   └── widgets/
│   │   │       ├── profile_header.dart
│   │   │       └── verification_badge.dart
│   │   └── routes.dart
│   │
│   ├── notifications/                 # Notificações in-app
│   │   ├── data/
│   │   │   ├── models/
│   │   │   │   └── notification_model.dart
│   │   │   └── ...
│   │   ├── domain/
│   │   │   └── ...
│   │   ├── presentation/
│   │   │   ├── providers/
│   │   │   │   ├── notifications_provider.dart
│   │   │   │   └── unread_count_provider.dart
│   │   │   ├── screens/
│   │   │   │   └── notifications_screen.dart
│   │   │   └── widgets/
│   │   │       └── notification_item.dart
│   │   └── routes.dart
│   │
│   ├── verification/                  # Verificações e evidências
│   │   ├── data/
│   │   │   ├── models/
│   │   │   │   ├── document_evidence_model.dart
│   │   │   │   └── verification_status.dart
│   │   │   └── ...
│   │   ├── domain/
│   │   │   └── ...
│   │   ├── presentation/
│   │   │   ├── providers/
│   │   │   │   └── verification_provider.dart
│   │   │   ├── screens/
│   │   │   │   ├── identity_verification_screen.dart
│   │   │   │   ├── residency_verification_screen.dart
│   │   │   │   └── document_upload_screen.dart
│   │   │   └── widgets/
│   │   │       └── document_picker.dart
│   │   └── routes.dart
│   │
│   ├── admin/                         # Administração (SystemAdmin)
│   │   ├── data/
│   │   │   ├── models/
│   │   │   │   ├── system_config_model.dart
│   │   │   │   └── work_item_model.dart
│   │   │   └── ...
│   │   ├── domain/
│   │   │   └── ...
│   │   ├── presentation/
│   │   │   ├── providers/
│   │   │   │   ├── system_config_provider.dart
│   │   │   │   └── admin_work_queue_provider.dart
│   │   │   ├── screens/
│   │   │   │   ├── admin_dashboard_screen.dart
│   │   │   │   ├── system_config_screen.dart
│   │   │   │   └── admin_work_queue_screen.dart
│   │   │   └── widgets/
│   │   │       └── config_editor.dart
│   │   └── routes.dart
│   │
│   └── shared/                        # Componentes e utilitários compartilhados
│       ├── theme/
│       │   ├── app_theme.dart        # Material 3 ThemeData
│       │   ├── app_colors.dart       # Paleta de cores
│       │   ├── app_text_styles.dart  # Tipografia
│       │   └── theme_provider.dart   # Provider de tema
│       ├── widgets/
│       │   ├── loading_overlay.dart
│       │   ├── error_snackbar.dart
│       │   ├── confirmation_dialog.dart
│       │   ├── image_viewer.dart
│       │   └── paginated_list_view.dart
│       ├── utils/
│       │   ├── permissions_helper.dart
│       │   ├── image_utils.dart
│       │   └── date_formatter.dart
│       └── router/
│           ├── app_router.dart       # Configuração go_router
│           ├── route_guards.dart     # Guards de autenticação
│           └── route_names.dart      # Nomes de rotas
│
└── generated/                         # Código gerado (build_runner)
    └── *.g.dart
```

---

## 🔌 Funcionalidades por Domínio

### 1. Autenticação e Sessão (`/api/v1/auth`)

#### Login Social
- **Endpoint**: `POST /api/v1/auth/social`
- **Fluxo**:
  1. Tela de login com botões sociais (Google, Apple, Facebook)
  2. Após autenticação social, coleta: `authProvider`, `externalId`, `displayName`, `email`
  3. Solicita **CPF** (formato aceito: "123.456.789-00" ou "12345678900") **OU** `foreignDocument`
  4. Valida CPF no frontend (dígito verificador)
  5. Envia request e armazena JWT no secure storage
  6. Redireciona para onboarding se primeiro acesso, ou feed se já cadastrado

- **2FA**: Se usuário tem 2FA habilitado, retorna `2FA_REQUIRED` com `challengeId`. Mostrar tela de código 2FA.

#### Setup 2FA
- **Endpoints**:
  - `POST /api/v1/auth/2fa/setup` → Gera secret e QR code
  - `POST /api/v1/auth/2fa/confirm` → Confirma e salva recovery codes
  - `POST /api/v1/auth/2fa/verify` → Verifica código e retorna JWT

#### Perfil do Usuário Logado
- **Endpoint**: `GET /api/v1/auth/me` (ou usar dados do token JWT decodificado)
- Provider global (`currentUserProvider`) expõe usuário atual
- Atualizar automaticamente após mutations

#### Logout
- Limpar tokens do secure storage
- Limpar cache local (Hive)
- Redirecionar para login

#### Rate Limiting de Auth
- Tratar `429 Too Many Requests` com `Retry-After`
- Mostrar snackbar informativo
- Desabilitar botão de login temporariamente

---

### 2. Territórios e Vínculos (`/api/v1/territories`, `/api/v1/memberships`)

#### Descoberta de Territórios
- **Endpoints**:
  - `GET /api/v1/territories` → Lista todos (público)
  - `GET /api/v1/territories/nearby?lat={lat}&lng={lng}&radiusKm=25` → Próximos (público)
  - `GET /api/v1/territories/search?q={query}` → Busca por texto (público)
- **Tela**: Lista com mapa, busca por texto, filtro por proximidade
- **Geolocalização**: Solicitar permissão e usar localização atual para "nearby"

#### Detalhes do Território
- **Endpoint**: `GET /api/v1/territories/{id}`
- Mostrar: nome, descrição, polígono no mapa, feature flags ativas, estatísticas

#### Entrar como VISITOR
- **Endpoint**: `POST /api/v1/territories/{territoryId}/enter`
- Cria vínculo leve (sem verificação)
- Atualiza contexto de sessão (X-Session-Id)

#### Solicitar Residência (Become Resident)
- **Endpoint**: `POST /api/v1/memberships/{territoryId}/become-resident`
- **Fluxo**:
  1. Explicar necessidade de verificação (geolocalização + documento)
  2. Solicitar permissão de localização
  3. Validar presença física (lat/lng dentro do polígono ou próximo)
  4. Opcional: Upload de documento (evidência de residência)
  5. Criar JoinRequest (destinatários automáticos: moradores verificados/curadores)
  6. Mostrar status: `PENDING` → aguardando aprovação
- **Provider**: `membershipStatusProvider` monitora mudanças de status

#### Status de Vínculo
- **Endpoint**: `GET /api/v1/memberships/{territoryId}`
- Exibir:
  - **Role**: VISITOR ou RESIDENT
  - **ResidencyVerification**: NONE, GeoVerified, DocumentVerified, BothVerified
  - **MembershipStatus**: Pending, Active, Suspended, Revoked
  - **Capabilities**: Array de capabilities (Curator, Moderator, EventOrganizer)
- **Badges visuais**: Ícones distintos por role/verificação

#### Seleção de Território Ativo
- **Endpoints**:
  - `POST /api/v1/territories/selection` → Define território ativo
  - `GET /api/v1/territories/selection` → Obtém território ativo
- **Header obrigatório**: `X-Session-Id` (gerar UUID único por instalação da app)
- **Provider**: `currentTerritoryProvider` gerencia território ativo
- **Interceptor**: Injetar `X-Session-Id` em todas as requisições autenticadas

#### Feature Flags Territoriais
- **Endpoint**: `GET /api/v1/territories/{id}/feature-flags`
- Verificar flags antes de habilitar funcionalidades:
  - `MarketplaceEnabled`
  - `EventsEnabled`
  - `ChatEnabled`
  - `AlertsEnabled`
- Esconder/desabilitar UI se flag desativada

#### Multivínculo
- Um usuário pode ser VISITOR em múltiplos territórios
- Apenas **1 RESIDENT** por usuário (global)
- Seletor de território na app bar permite trocar contexto

---

### 3. Feed Comunitário (`/api/v1/feed`)

#### Feed do Território
- **Endpoint**: `GET /api/v1/feed/paged?territoryId={id}&pageNumber=1&pageSize=20`
- **Filtros opcionais**: `postType` (NOTICE, ALERT, ANNOUNCEMENT), `visibility` (PUBLIC, RESIDENT_ONLY)
- **Paginação infinita**: Carregar mais ao rolar
- **Pull to refresh**: Atualizar feed manualmente
- **Cache**: Cachear últimas 50 posts localmente (Hive)

#### Feed Pessoal
- **Endpoint**: `GET /api/v1/feed/me/paged?pageNumber=1&pageSize=20`
- Apenas posts do usuário logado

#### Detalhes do Post
- **Endpoint**: `GET /api/v1/feed/{postId}`
- Mostrar: autor, conteúdo (markdown), data, GeoAnchors (pins no mapa), tipo, visibilidade, status

#### Criar Post
- **Endpoint**: `POST /api/v1/feed`
- **Campos**:
  - `territoryId` (obrigatório)
  - `type` (NOTICE, ALERT, ANNOUNCEMENT)
  - `visibility` (PUBLIC, RESIDENT_ONLY)
  - `content` (texto, suporta markdown)
  - `geoAnchors` (opcional, array de {latitude, longitude, type})
- **Validações**:
  - Território deve estar selecionado (X-Session-Id)
  - Conteúdo não pode estar vazio
  - Visibilidade: VISITOR só pode PUBLIC, RESIDENT pode ambos
- **UX**: Editor markdown com preview, seletor de tipo, toggle de visibilidade

#### Tipos de Post
- **NOTICE**: Comunicado geral (padrão)
- **ALERT**: Alerta importante (destaque visual)
- **ANNOUNCEMENT**: Anúncio oficial (badge especial)

#### Curadoria de Posts
- **Endpoint**: `PATCH /api/v1/moderation/posts/{postId}/curate`
- **Permissão**: Apenas Curators
- Ações: Aprovar (status → PUBLISHED), Rejeitar (status → REJECTED), Restringir visibilidade

#### Denunciar Post
- **Endpoint**: `POST /api/v1/reports/posts/{postId}`
- Body: `reason`, `details` (opcional)
- Modal de denúncia com lista de motivos

#### Visibilidade Restrita
- Posts `RESIDENT_ONLY` só aparecem para moradores (RESIDENT) verificados
- VISITOR vê apenas posts `PUBLIC`

---

### 4. Eventos (`/api/v1/events`)

#### Listar Eventos
- **Endpoints**:
  - `GET /api/v1/events/paged?territoryId={id}&pageNumber=1&pageSize=20`
  - `GET /api/v1/events/nearby/paged?lat={lat}&lng={lng}&radiusKm=10&pageNumber=1&pageSize=20`
- **Filtros**: `startDate`, `endDate`, `type`, `status`
- **Ordenação**: Por data de início (próximos primeiro)

#### Detalhes do Evento
- **Endpoint**: `GET /api/v1/events/{id}`
- Mostrar: título, descrição, data/hora início/fim, localização (lat/lng), organizador, participantes, tipo, status

#### Criar Evento
- **Endpoint**: `POST /api/v1/events`
- **Campos**:
  - `territoryId`, `title`, `description`
  - `startDateTime` (ISO 8601), `endDateTime` (opcional)
  - `locationLat`, `locationLng` (obrigatório)
  - `type`, `visibility` (PUBLIC, RESIDENT_ONLY)
- **Permissão**: Apenas EventOrganizer capability ou RESIDENT
- **UX**: Date/time picker, mapa para selecionar localização

#### Inscrever-se no Evento
- **Endpoint**: `POST /api/v1/events/{id}/register`
- Adiciona usuário à lista de participantes

#### Cancelar Inscrição
- **Endpoint**: `DELETE /api/v1/events/{id}/register`

#### Credenciamento (Check-in)
- **Endpoint**: `POST /api/v1/events/{id}/checkin`
- **Requisição**: Enviar `X-Latitude` e `X-Longitude` (geolocalização obrigatória)
- **Validação**: Verificar proximidade ao evento (raio configurável)

---

### 5. Marketplace (`/api/v1/stores`, `/api/v1/items`, `/api/v1/cart`)

#### Pré-requisitos
- Verificar `FeatureFlag.MarketplaceEnabled` do território
- Verificar `MembershipSettings.MarketplaceOptIn` do usuário
- Verificar `Membership.Role == RESIDENT` e verificação de residência

#### Listar Lojas
- **Endpoint**: `GET /api/v1/stores/paged?territoryId={id}&pageNumber=1&pageSize=20`
- Cards com nome, descrição, status

#### Detalhes da Loja
- **Endpoint**: `GET /api/v1/stores/{id}`
- Mostrar itens da loja, informações de contato

#### Criar Loja
- **Endpoint**: `POST /api/v1/stores`
- Campos: `territoryId`, `displayName`, `description`, `status`

#### Listar Itens
- **Endpoint**: `GET /api/v1/items/paged?territoryId={id}&storeId={id}&pageNumber=1&pageSize=20`
- Filtros: `category`, `type` (PRODUCT, SERVICE), `pricingType`

#### Detalhes do Item
- **Endpoint**: `GET /api/v1/items/{id}`
- Mostrar: título, descrição, preço, categoria, tipo, loja, contato

#### Criar Item
- **Endpoint**: `POST /api/v1/items`
- Campos: `territoryId`, `storeId`, `title`, `description`, `category`, `type`, `pricingType`, `price` (opcional)

#### Carrinho de Compras
- **Endpoints**:
  - `GET /api/v1/cart` → Obtém carrinho atual
  - `POST /api/v1/cart` → Adiciona item
  - `PUT /api/v1/cart/{itemId}` → Atualiza quantidade
  - `DELETE /api/v1/cart/{itemId}` → Remove item
  - `POST /api/v1/cart/checkout` → Finaliza compra (calcula taxas)
- **Provider**: `cartProvider` gerencia estado do carrinho

#### Inquiries (Consultas)
- **Endpoint**: `POST /api/v1/items/{id}/inquiries`
- Body: `message`
- Notifica o vendedor

#### Listar Inquiries Recebidos
- **Endpoint**: `GET /api/v1/inquiries/received/paged?pageNumber=1&pageSize=20`

---

### 6. Alertas de Saúde (`/api/v1/alerts`)

#### Listar Alertas
- **Endpoint**: `GET /api/v1/alerts/paged?territoryId={id}&pageNumber=1&pageSize=20`
- Filtros: `type`, `severity` (INFO, WARNING, URGENT), `status`

#### Detalhes do Alerta
- **Endpoint**: `GET /api/v1/alerts/{id}`
- Mostrar: tipo, severidade, descrição, localização, tempo de expiração

#### Criar Alerta
- **Endpoint**: `POST /api/v1/alerts`
- Campos: `territoryId`, `type`, `severity`, `description`, `locationLat`, `locationLng`, `expiresAt` (opcional)
- **Permissão**: Apenas RESIDENT ou Curator

#### Tipos de Alerta
- Saúde, Segurança, Mobilidade, Serviços, etc.

---

### 7. Mapa Territorial (`/api/v1/map`)

#### Entidades do Mapa
- **Endpoint**: `GET /api/v1/map/entities/paged?territoryId={id}&pageNumber=1&pageSize=50`
- Retorna: pontos de interesse, recursos naturais, infraestrutura

#### Pins do Mapa
- **Endpoint**: `GET /api/v1/map/pins/paged?territoryId={id}&bounds={swLat},{swLng},{neLat},{neLng}&pageNumber=1&pageSize=100`
- Retorna pins agregados para visualização no mapa
- **Filtros por bounds**: Carregar apenas pins visíveis na viewport

#### Criar Entidade
- **Endpoint**: `POST /api/v1/map/entities`
- Campos: `territoryId`, `name`, `category`, `description`, `latitude`, `longitude`
- **Permissão**: Apenas RESIDENT ou Curator
- **Geolocalização obrigatória**: Enviar `X-Latitude` e `X-Longitude`

#### Integração com Feed
- Posts com `geoAnchors` aparecem como pins no mapa
- Clicar no pin abre detalhes do post

---

### 8. Chat (`/api/v1/chat`)

#### Listar Conversas
- **Endpoint**: `GET /api/v1/chat/conversations?territoryId={id}&skip=0&take=50`
- **Tipos**: `TerritoryPublic` (canal público), `TerritoryResidents` (apenas moradores), `Group` (grupo privado), `Direct` (DM)
- **Ordenação**: Por última mensagem (mais recente primeiro)

#### Mensagens da Conversa
- **Endpoint**: `GET /api/v1/chat/conversations/{id}/messages?beforeCreatedAtUtc={timestamp}&beforeMessageId={id}&take=50`
- **Paginação cursor-based**: Carregar mensagens anteriores ao rolar para cima

#### Enviar Mensagem
- **Endpoint**: `POST /api/v1/chat/conversations/{id}/messages`
- Body: `content`, `contentType` (TEXT, IMAGE, FILE)
- **WebSocket** (se suportado pelo backend): Atualizar mensagens em tempo real
- **Polling fallback**: Atualizar a cada 5 segundos se WebSocket não disponível

#### Criar Grupo
- **Endpoint**: `POST /api/v1/chat/conversations`
- Body: `kind=Group`, `territoryId`, `name`, `participantUserIds`

#### Participar de Conversa
- **Endpoint**: `POST /api/v1/chat/conversations/{id}/participants`

---

### 9. Notificações (`/api/v1/notifications`)

#### Listar Notificações
- **Endpoint**: `GET /api/v1/notifications/paged?pageNumber=1&pageSize=50`
- **Ordenação**: Mais recentes primeiro
- **Tipos**: Post criado, report criado, inquiry recebido, join request aprovado, etc.

#### Marcar como Lida
- **Endpoint**: `POST /api/v1/notifications/{id}/read`

#### Badge de Contador
- Provider: `unreadNotificationsCountProvider`
- Atualizar automaticamente via polling ou push notification

#### Preferências de Notificação
- Gerenciar preferências via `UserPreferences` (canal: push, email)

---

### 10. Moderação e Curadoria (`/api/v1/moderation`, `/api/v1/reports`)

#### Dashboard de Moderação
- **Permissão**: Apenas Curator ou Moderator capability
- Mostrar: Work items pendentes, reports abertos, estatísticas

#### Listar Reports
- **Endpoint**: `GET /api/v1/reports/paged?territoryId={id}&targetType={POST|USER}&status={OPEN|RESOLVED}&pageNumber=1&pageSize=20`
- **Filtros**: Por tipo de alvo, status

#### Resolver Report
- **Endpoint**: `POST /api/v1/moderation/reports/{id}/resolve`
- Ações: Aprovar report (aplicar sanção), Rejeitar report, Sem ação

#### Work Items (Fila de Revisão)
- **Endpoint**: `GET /api/v1/territories/{id}/work-items?type={MODERATION_CASE|ASSET_CURATION}&status={OPEN|REQUIRES_HUMAN_REVIEW}`
- **Tipos**: `MODERATION_CASE`, `ASSET_CURATION`, `RESIDENCY_VERIFICATION`
- **Completar Work Item**: `POST /api/v1/territories/{id}/work-items/{id}/complete`
- Body: `outcome` (APPROVED, REJECTED, NOACTION), `completionNotes`

#### Bloquear Usuário
- **Endpoint**: `POST /api/v1/users/{userId}/block`
- Efeito: Posts e conteúdo do usuário bloqueado não aparecem mais

---

### 11. Verificações e Evidências (`/api/v1/verification`, `/api/v1/memberships/{id}/verify-residency`)

#### Upload de Documento de Identidade
- **Endpoint**: `POST /api/v1/verification/identity/document/upload`
- **Content-Type**: `multipart/form-data`
- **Campo**: `file` (PDF, JPG, PNG)
- **Tela**: File picker → preview → upload → status PENDING

#### Upload de Documento de Residência
- **Endpoint**: `POST /api/v1/memberships/{territoryId}/verify-residency/document/upload`
- Similar ao de identidade, mas territorial

#### Status de Verificação
- Monitorar via Work Items ou endpoint específico
- Badge visual: Unverified, Pending, Verified, Rejected

---

### 12. Administração (`/api/v1/admin`)

#### Pré-requisito
- **Permissão**: SystemAdmin (global, não territorial)

#### System Config
- **Endpoints**:
  - `GET /api/v1/admin/system-configs?category={category}`
  - `GET /api/v1/admin/system-configs/{key}`
  - `PUT /api/v1/admin/system-configs/{key}`
- **Tela**: Lista de configurações editáveis, organizadas por categoria

#### Work Queue Global
- **Endpoint**: `GET /api/v1/admin/work-items?type={IDENTITY_VERIFICATION}&status={OPEN}`
- **Completar**: `POST /api/v1/admin/work-items/{id}/complete`

#### Download de Evidências
- **Endpoint**: `GET /api/v1/admin/evidences/{id}/download`
- Stream de arquivo via API (proxy)

---

## 🎨 Design System e UX

### Material Design 3
- Usar `MaterialApp` com `theme` e `darkTheme`
- Cores primárias: Verde (natureza/comunidade), Azul (confiança), Tons terrosos (território)
- Tipografia: Fonte sem serifa (ex: `Inter`, `Roboto`, `Poppins`)
- Espaçamento: Grid de 4px (4, 8, 12, 16, 24, 32, 48, 64)
- Border radius: 8px (cards), 12px (dialogs), 4px (inputs)

### Modo Claro/Escuro
- Provider: `themeProvider` (Riverpod)
- Persistir preferência em SharedPreferences
- Detectar preferência do sistema como padrão
- Toggle na tela de configurações

### Responsividade
- **Mobile-first**: Layout otimizado para celular
- **Tablet**: Breakpoint em 600px (largura)
  - Layout em 2 colunas (feed + mapa)
  - Drawer lateral permanente
- **Adaptativo**: Usar `LayoutBuilder` para ajustar layout

### Acessibilidade
- Semântica completa: `Semantics` widget onde necessário
- Labels descritivos: `label`, `hint`
- Contraste: WCAG AA (4.5:1 para texto normal, 3:1 para texto grande)
- Tamanhos de fonte escaláveis: Respeitar preferências do sistema
- Foco visível: Indicadores de foco em navegação por teclado

### Estados de UI
- **Loading**: Skeleton loaders ou circular progress
- **Erro**: Snackbar com ação de retry
- **Vazio**: Empty state com ilustração e CTA
- **Offline**: Banner informativo + cache local

---

## 🔐 Segurança e Autenticação

### JWT Token
- Armazenar em `flutter_secure_storage`
- Decodificar para obter `userId`, `exp` (expiração)
- Validar expiração antes de usar
- Refresh automático (se endpoint disponível)

### Headers Obrigatórios
- `Authorization: Bearer {token}` → Interceptor automático
- `X-Session-Id: {uuid}` → Território ativo (gerar UUID único por instalação)
- `X-Latitude: {lat}`, `X-Longitude: {lng}` → Geolocalização (quando necessário)

### Rate Limiting
- Interceptor captura `429 Too Many Requests`
- Ler header `Retry-After` (segundos)
- Mostrar snackbar informativo
- Retry automático após delay

### Permissões
- **Localização**: Solicitar `whenInUse` ou `always` (explicar uso)
- **Câmera/Galeria**: Para upload de imagens
- **Armazenamento**: Para download de documentos

---

## 📱 Navegação e Roteamento

### Estrutura de Rotas (go_router)

```dart
GoRouter(
  routes: [
    // Auth
    GoRoute(path: '/login', ...),
    GoRoute(path: '/register', ...),
    
    // Onboarding
    GoRoute(path: '/onboarding', ...),
    
    // Main (Shell Route)
    ShellRoute(
      builder: (context, state, child) => MainScaffold(child: child),
      routes: [
        GoRoute(path: '/feed', ...),
        GoRoute(path: '/map', ...),
        GoRoute(path: '/events', ...),
        GoRoute(path: '/market', ...),
        GoRoute(path: '/chat', ...),
        GoRoute(path: '/profile', ...),
      ],
    ),
    
    // Nested routes
    GoRoute(path: '/territories/:id', ...),
    GoRoute(path: '/posts/:id', ...),
    GoRoute(path: '/events/:id', ...),
  ],
)
```

### Route Guards
- Verificar autenticação antes de rotas protegidas
- Verificar capability (Curator, Moderator) antes de telas de moderação
- Verificar feature flag antes de habilitar funcionalidade

### Deep Linking
- Suportar: `Arah://territory/{id}`, `Arah://post/{id}`, `Arah://event/{id}`
- Configurar no `AndroidManifest.xml` e `Info.plist`

---

## 🌍 Internacionalização

### Idiomas Suportados
- **pt-BR** (padrão)
- **en-US**

### Arquivos de Tradução
- Organizar por feature: `auth_pt.dart`, `territories_pt.dart`, etc.
- Usar `intl` para formatação de datas/números

### Troca de Idioma
- Persistir em SharedPreferences
- Reiniciar app ou recarregar recursos

---

## 📊 Gerenciamento de Estado (Riverpod)

### Providers Globais
- `authProvider` → Estado de autenticação (logged in/out, user)
- `currentTerritoryProvider` → Território ativo
- `themeProvider` → Tema (claro/escuro)
- `userLocationProvider` → Localização atual do usuário

### Providers por Feature
- `territoriesProvider` → Lista de territórios
- `feedProvider` → Feed do território (com paginação)
- `eventsProvider` → Eventos do território
- `notificationsProvider` → Notificações do usuário

### Auto-refresh
- Providers de lista podem auto-refresh a cada X segundos
- Usar `ref.watch` para reagir a mudanças

---

## 🧪 Testes

### Testes Unitários
- Validadores (CPF, email, geo)
- Formatters
- Utils

### Testes de Widget
- Componentes compartilhados
- Cards de post, evento, território

### Testes de Integração
- Fluxos completos (login → feed → criar post)

---

## 📦 Dependências

```yaml
dependencies:
  flutter:
    sdk: flutter
  flutter_localizations:
    sdk: flutter

  # State Management
  flutter_riverpod: ^2.5.0
  hooks_riverpod: ^2.5.0

  # Navigation
  go_router: ^14.0.0

  # HTTP
  dio: ^5.4.0
  dio_cache_interceptor: ^3.4.0
  connectivity_plus: ^5.0.0

  # Serialization
  json_serializable: ^6.7.0
  json_annotation: ^4.8.0

  # Storage
  shared_preferences: ^2.2.0
  flutter_secure_storage: ^9.0.0
  hive: ^2.2.3
  hive_flutter: ^1.1.0

  # Location
  geolocator: ^10.1.0
  geocoding: ^2.1.1
  google_maps_flutter: ^2.5.0

  # UI
  flutter_slidable: ^3.0.0
  pull_to_refresh: ^2.0.0
  infinite_scroll_pagination: ^4.0.0
  cached_network_image: ^3.3.0
  flutter_markdown: ^0.6.18
  url_launcher: ^6.2.0
  share_plus: ^7.0.0
  image_picker: ^1.0.0
  file_picker: ^6.0.0

  # Notifications
  firebase_messaging: ^14.7.0
  flutter_local_notifications: ^16.0.0

  # Forms
  reactive_forms: ^16.1.1

  # Utils
  equatable: ^2.0.5
  uuid: ^4.0.0
  intl: ^0.19.0
  timeago: ^3.6.0

dev_dependencies:
  flutter_test:
    sdk: flutter
  build_runner: ^2.4.0
  json_serializable: ^6.7.0
  hive_generator: ^2.0.0
```

---

## 🚀 Instruções de Implementação

### Fase 1: Setup Inicial
1. Criar projeto Flutter
2. Configurar `pubspec.yaml` com todas as dependências
3. Configurar estrutura de pastas
4. Configurar `go_router` básico
5. Configurar tema Material 3
6. Configurar internacionalização

### Fase 2: Core e Infraestrutura
1. Implementar interceptors Dio (auth, geo, session)
2. Implementar storage (secure, local, cache)
3. Implementar providers globais (auth, territory, theme)
4. Implementar utilitários (validators, formatters)
5. Implementar widgets compartilhados (loading, error, empty)

### Fase 3: Autenticação e Onboarding
1. Implementar tela de login social
2. Implementar fluxo de cadastro
3. Implementar 2FA (setup e verify)
4. Implementar onboarding (localização, descoberta de territórios)
5. Implementar seleção de território

### Fase 4: Territórios e Vínculos
1. Implementar listagem de territórios
2. Implementar busca e filtros
3. Implementar detalhes do território
4. Implementar entrar como VISITOR
5. Implementar solicitar residência
6. Implementar status de vínculo

### Fase 5: Feed e Posts
1. Implementar feed do território (com paginação)
2. Implementar feed pessoal
3. Implementar criar post
4. Implementar detalhes do post
5. Implementar curadoria (para curators)
6. Implementar denúncia de post

### Fase 6: Eventos
1. Implementar listagem de eventos
2. Implementar eventos próximos
3. Implementar criar evento
4. Implementar inscrição/check-in
5. Implementar mapa de eventos

### Fase 7: Marketplace
1. Implementar listagem de lojas
2. Implementar criação de loja
3. Implementar listagem de itens
4. Implementar criação de item
5. Implementar carrinho de compras
6. Implementar checkout
7. Implementar inquiries

### Fase 8: Mapa Territorial
1. Implementar tela de mapa
2. Implementar pins do mapa
3. Implementar entidades do mapa
4. Implementar criar entidade
5. Integrar com feed (posts georreferenciados)

### Fase 9: Chat
1. Implementar listagem de conversas
2. Implementar tela de chat
3. Implementar envio de mensagens
4. Implementar criação de grupos
5. Implementar polling/WebSocket para atualizações em tempo real

### Fase 10: Notificações
1. Implementar listagem de notificações
2. Implementar badge de contador
3. Implementar push notifications (se configurado)
4. Implementar preferências de notificação

### Fase 11: Moderação e Curadoria
1. Implementar dashboard de moderação
2. Implementar listagem de reports
3. Implementar resolver report
4. Implementar work items queue
5. Implementar bloquear usuário

### Fase 12: Verificações
1. Implementar upload de documento de identidade
2. Implementar upload de documento de residência
3. Implementar status de verificação
4. Implementar download de evidências (para admin/curator)

### Fase 13: Admin
1. Implementar dashboard admin
2. Implementar system config
3. Implementar work queue global
4. Implementar download de evidências

### Fase 14: Perfil e Configurações
1. Implementar tela de perfil
2. Implementar edição de perfil
3. Implementar configurações (tema, idioma, notificações)
4. Implementar exclusão de conta
5. Implementar exportação de dados

### Fase 15: Polimento e Otimização
1. Implementar cache inteligente
2. Implementar offline mode básico
3. Otimizar performance (lazy loading, debounce)
4. Implementar testes unitários
5. Implementar testes de widget
6. Implementar testes de integração
7. Revisar acessibilidade
8. Revisar tratamento de erros

---

## 📝 Notas Importantes

### Geração de Código a partir do OpenAPI
1. Use ferramentas como `openapi-generator` ou `swagger-dart-code-generator` para gerar models e API clients automaticamente a partir do `openapi.json`.
2. Os models gerados devem ser colocados em `features/{feature}/data/models/`.
3. Adaptar providers e repositories para usar os models gerados.

### Testes em Dispositivos Reais
- Geolocalização e permissões devem ser testadas em dispositivos físicos.
- Testar em diferentes tamanhos de tela (celular, tablet).
- Testar em diferentes versões do Android/iOS.

### Cache Local Inteligente
- Use Hive para cache de dados que não mudam frequentemente (territórios, perfil do usuário).
- Invalidar cache quando necessário (após mutations).
- Implementar TTL apropriado para cada tipo de dado.

### Tratamento de Erros Robusto
- Sempre tratar `401 Unauthorized` (logout forçado)
- Sempre tratar `403 Forbidden` (sem permissão - mostrar mensagem adequada)
- Sempre tratar `429 Too Many Requests` (rate limit - retry após delay)
- Sempre tratar `500 Internal Server Error` (retry com backoff exponencial)
- Sempre tratar erros de rede (offline, timeout)

### Performance
- Lazy loading de imagens
- Paginação infinita para listas grandes
- Debounce em buscas
- Cache de imagens (cached_network_image)
- Compressão de imagens antes de upload

### Acessibilidade desde o Início
- Não deixe para depois, implemente semântica e contraste adequados desde o início.
- Teste com TalkBack (Android) e VoiceOver (iOS).
- Certifique-se de que todos os botões e ações são acessíveis via leitor de tela.

---

## 🔗 Referências

- [API - Lógica de Negócio e Usabilidade](./60_API_LÓGICA_NEGÓCIO.md)
- [Modelo de Domínio](./12_DOMAIN_MODEL.md)
- [Decisões Arquiteturais](./10_ARCHITECTURE_DECISIONS.md)
- [OpenAPI Specification](../backend/Arah.Api/wwwroot/devportal/openapi.json)

---

**Status**: 📋 Planejamento Completo  
**Próximos Passos**: Iniciar implementação seguindo a ordem das fases descritas acima.
