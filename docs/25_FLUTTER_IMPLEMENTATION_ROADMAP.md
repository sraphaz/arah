# Roadmap de Implementação do Frontend Flutter - Sincronizado com API até Fase 28

**Versão**: 1.0  
**Data**: 2025-01-20  
**Status**: 📋 Planejamento Extensivo  
**Tipo**: Roadmap de Implementação Detalhado

---

## 📋 Índice

1. [Visão Geral](#visão-geral)
2. [Metodologia e Padrões](#metodologia-e-padrões)
3. [Fases Sincronizadas com Backend](#fases-sincronizadas-com-backend)
4. [Jornadas de Usuário por Papel](#jornadas-de-usuário-por-papel)
5. [Fases Detalhadas](#fases-detalhadas)
6. [Critérios de Qualidade e Entrega](#critérios-de-qualidade-e-entrega)
7. [Plano de Testes](#plano-de-testes)
8. [Deploy e Lançamento](#deploy-e-lançamento)

---

## 🎯 Visão Geral

Este documento define o **roadmap completo de implementação do app mobile Flutter** sincronizado com o desenvolvimento da API até a **Fase 28**, considerando:

- ✅ **Sincronização com Backend**: Cada fase do frontend está alinhada com as fases da API
- ✅ **Jornadas Completas**: Funcionalidades implementadas por jornadas de usuário completas
- ✅ **Todos os Papéis**: VISITOR, RESIDENT, Curator, Moderator, EventOrganizer, SystemAdmin
- ✅ **Padrões Elevados**: Design system, arquitetura, testes, acessibilidade, performance
- ✅ **Incremental e Entregável**: Cada fase entrega valor tangível aos usuários

### Princípios de Implementação

1. **Mobile-First**: Experiência otimizada para dispositivos móveis
2. **Offline-First**: Funcionalidades básicas funcionam offline
3. **Performance**: Tempos de resposta < 200ms, animações 60fps
4. **Acessibilidade**: WCAG AA, suporte completo a leitores de tela
5. **Testes**: Cobertura mínima de 80% (unitários) e 60% (widget)
6. **Design System**: Material 3 com tema claro/escuro automático
7. **Internacionalização**: pt-BR e en-US desde o início

---

## 🏗️ Metodologia e Padrões

### Arquitetura

- **Clean Architecture**: Separação clara de responsabilidades (Data, Domain, Presentation)
- **Feature-First**: Organização por features verticais
- **Riverpod**: Gerenciamento de estado declarativo e testável
- **Go Router**: Navegação declarativa com deep linking

### Padrões de Design

- **Material Design 3**: Design system moderno e acessível
- **Design Tokens**: Cores, tipografia, espaçamento centralizados
- **Component Library**: Biblioteca de componentes reutilizáveis
- **Design Patterns**: MVVM com providers, Repository pattern, Factory pattern

### Padrões de Desenvolvimento

- **TDD**: Test-Driven Development quando possível
- **Code Review**: Todas as PRs revisadas por pelo menos 1 desenvolvedor
- **Documentação**: Código documentado, READMEs por feature
- **CI/CD**: Pipeline automatizado (build, testes, lint, deploy)

### Padrões de Implantação

- **Feature Flags**: Liberação gradual de funcionalidades
- **A/B Testing**: Testes de experiência quando aplicável
- **Analytics**: Rastreamento de eventos e erros (Firebase Analytics, Sentry)
- **Monitoring**: Performance monitoring (Firebase Performance, Sentry)

---

## 🔄 Fases Sincronizadas com Backend

### Mapeamento de Fases

| Fase Backend | Fase Frontend | Duração | Prioridade | Dependências |
|--------------|---------------|---------|------------|--------------|
| **Fase 1-7** | **Frontend Fase 0** | 4 semanas | 🔴 Crítica | Setup inicial |
| **Fase 8** | **Frontend Fase 1** | 3 semanas | 🔴 Crítica | Fase 0 |
| **Fase 9** | **Frontend Fase 2** | 3 semanas | 🔴 Crítica | Fase 1 |
| **Fase 10** | **Frontend Fase 3** | 4 semanas | 🔴 Crítica | Fase 2 |
| **Fase 11** | **Frontend Fase 4** | 3 semanas | 🟡 Importante | Fase 3 |
| **Fase 13** | **Frontend Fase 5** | 2 semanas | 🔴 Crítica | Fase 2 |
| **Fase 14** | **Frontend Fase 6** | 4 semanas | 🔴 Crítica | Fase 5 |
| **Fase 17** | **Frontend Fase 7** | 4 semanas | 🟡 Importante | Fase 2, 6 |
| **Fase 18** | **Frontend Fase 8** | 5 semanas | 🔴 Alta | Fase 7 |
| **Fase 20** | **Frontend Fase 9** | 5 semanas | 🟡 Alta | Fase 6 |
| **Fase 23** | **Frontend Fase 10** | 4 semanas | 🔴 Alta | Fase 9 |
| **Fase 24** | **Frontend Fase 11** | 3 semanas | 🟡 Alta | Fase 9 |
| **Fase 25** | **Frontend Fase 12** | 4 semanas | 🔴 Alta | Fase 2 |
| **Fase 26** | **Frontend Fase 13** | 3 semanas | 🔴 Alta | Fase 12 |
| **Fase 27** | **Frontend Fase 14** | 4 semanas | 🔴 Alta | Fase 12, 9, 6 |
| **Fase 28** | **Frontend Fase 15** | 4 semanas | 🟡 Média-Alta | Fase 7, 9 |

**Total Estimado**: ~60 semanas (~15 meses) com paralelização controlada

---

## 👥 Jornadas de Usuário por Papel

### VISITOR (Visitante)

#### Jornada Principal
1. **Descoberta**
   - Baixar app
   - Onboarding (localização, permissões)
   - Descobrir territórios próximos
   - Visualizar feed público

2. **Engajamento Inicial**
   - Ler posts públicos
   - Visualizar eventos públicos
   - Explorar mapa territorial
   - Ver lojas do marketplace (sem comprar)

3. **Transição para RESIDENT**
   - Solicitar residência
   - Upload de documento (opcional)
   - Acompanhar aprovação
   - Acesso a conteúdo exclusivo após aprovação

#### Funcionalidades Visíveis
- ✅ Feed público do território
- ✅ Eventos públicos
- ✅ Mapa territorial (entidades públicas)
- ✅ Lojas do marketplace (visualização)
- ✅ Chat público do território
- ❌ Criar posts RESIDENT_ONLY
- ❌ Criar eventos RESIDENT_ONLY
- ❌ Marketplace (compras)
- ❌ Votação em decisões comunitárias

---

### RESIDENT (Morador)

#### Jornada Principal
1. **Onboarding de Residente**
   - Verificação de residência aprovada
   - Tutorial de funcionalidades de morador
   - Configurar preferências

2. **Participação Comunitária**
   - Criar posts (públicos e privados)
   - Criar eventos
   - Participar de eventos
   - Criar lojas no marketplace
   - Comprar/vender no marketplace

3. **Engajamento Profundo**
   - Participar de votações
   - Contribuir para saúde territorial
   - Ganhar pontos de gamificação
   - Usar moeda territorial

#### Funcionalidades Visíveis
- ✅ Todas as funcionalidades de VISITOR
- ✅ Criar posts RESIDENT_ONLY
- ✅ Criar eventos RESIDENT_ONLY
- ✅ Marketplace completo (compras e vendas)
- ✅ Participar de votações
- ✅ Sistema de gamificação
- ✅ Moeda territorial
- ✅ Saúde territorial (observações, ações)
- ✅ Compra coletiva
- ✅ Trocas comunitárias
- ✅ Banco de sementes

---

### CURATOR (Curador)

#### Jornada Principal
1. **Curadoria de Conteúdo**
   - Aprovar/rejeitar posts
   - Aprovar/rejeitar vínculos
   - Revisar reports
   - Gerenciar work items

2. **Governança Territorial**
   - Criar votações
   - Gerenciar feature flags
   - Monitorar saúde do território
   - Aprovar solicitações de residência

#### Funcionalidades Visíveis
- ✅ Todas as funcionalidades de RESIDENT
- ✅ Dashboard de moderação
- ✅ Painel de curadoria
- ✅ Work queue territorial
- ✅ Gerenciar feature flags
- ✅ Criar e gerenciar votações
- ✅ Aprovar/rejeitar vínculos
- ✅ Configurar interesses do território

---

### MODERATOR (Moderador)

#### Jornada Principal
1. **Moderação Ativa**
   - Resolver reports
   - Aplicar sanções
   - Revisar conteúdo reportado
   - Bloquear usuários problemáticos

#### Funcionalidades Visíveis
- ✅ Todas as funcionalidades de RESIDENT
- ✅ Dashboard de moderação avançado
- ✅ Aplicar sanções territoriais
- ✅ Bloquear usuários
- ✅ Revisar casos de moderação

---

### EVENT_ORGANIZER (Organizador de Eventos)

#### Jornada Principal
1. **Organização de Eventos**
   - Criar eventos territoriais
   - Gerenciar participantes
   - Credenciamento (check-in)
   - Relatórios de eventos

#### Funcionalidades Visíveis
- ✅ Todas as funcionalidades de RESIDENT
- ✅ Criar eventos sem limitações
- ✅ Gerenciar participantes
- ✅ Credenciamento (check-in)
- ✅ Relatórios de eventos

---

### SYSTEM_ADMIN (Administrador do Sistema)

#### Jornada Principal
1. **Administração Global**
   - Gerenciar configurações do sistema
   - ⭐ **Novo**: Media Storage Config, Rate Limiting, JWT Config, Observability Config, Data Retention Config
   - ⭐ Consulte [38_FLUTTER_CONFIGURACOES_ADMINISTRATIVAS.md](../38_FLUTTER_CONFIGURACOES_ADMINISTRATIVAS.md) para detalhes completos
   - Revisar work items globais
   - Monitorar saúde do sistema
   - Gerenciar usuários

#### Funcionalidades Visíveis
- ✅ Dashboard administrativo completo
- ✅ System config
- ✅ Work queue global
- ✅ Verificações de identidade
- ✅ Monitoramento de sistema
- ✅ Acesso a todos os territórios

---

## 📦 Fases Detalhadas

---

## **FRONTEND FASE 0: Fundação e Infraestrutura Base** (4 semanas)

**Sincronizada com**: Backend Fases 1-7 (já implementadas)  
**Prioridade**: 🔴 Crítica  
**Duração**: 4 semanas (20 dias úteis)  
**Estimativa**: 160 horas

### Objetivo

Estabelecer a fundação técnica do app Flutter: arquitetura, infraestrutura, design system, autenticação básica e navegação.

### Entregas

#### Semana 1: Setup do Projeto e Arquitetura Base

**Tarefas**:
- [ ] Criar projeto Flutter 3.19+
- [ ] Configurar estrutura de pastas (features/)
- [ ] Configurar dependências (pubspec.yaml)
- [ ] Configurar build_runner para code generation
- [ ] Configurar linting (flutter_lints)
- [ ] Configurar CI/CD básico (GitHub Actions)
- [ ] Configurar internacionalização (pt-BR, en-US)

**Arquivos Criados**:
- `lib/main.dart`
- `lib/app.dart`
- `lib/core/config/app_config.dart`
- `lib/core/config/constants.dart`
- `pubspec.yaml`
- `.github/workflows/flutter.yml`

#### Semana 2: Design System e Tema

**Tarefas**:
- [ ] Implementar Material 3 Theme (claro/escuro)
- [ ] Definir paleta de cores (verde, azul, terrosos)
- [ ] Configurar tipografia (sem serifa)
- [ ] Criar design tokens (espaçamento, border radius)
- [ ] Implementar ThemeProvider (Riverpod)
- [ ] Criar widgets base (Button, Card, Input, etc.)
- [ ] Testes de componentes de UI

**Arquivos Criados**:
- `lib/shared/theme/app_theme.dart`
- `lib/shared/theme/app_colors.dart`
- `lib/shared/theme/app_text_styles.dart`
- `lib/shared/theme/theme_provider.dart`
- `lib/shared/widgets/button.dart`
- `lib/shared/widgets/card.dart`
- `lib/shared/widgets/input.dart`

#### Semana 3: Network Layer e Autenticação Base

**Tarefas**:
- [ ] Configurar Dio com interceptors
- [ ] Implementar AuthInterceptor (JWT)
- [ ] Implementar SessionInterceptor (X-Session-Id)
- [ ] Implementar GeoInterceptor (X-Latitude/X-Longitude)
- [ ] Implementar RetryInterceptor
- [ ] Implementar RateLimitInterceptor (429)
- [ ] Implementar LoggingInterceptor (dev)
- [ ] Configurar SecureStorage para tokens
- [ ] Implementar AuthProvider básico
- [ ] Tela de login simples (sem social login ainda)

**Arquivos Criados**:
- `lib/core/network/dio_client.dart`
- `lib/core/network/interceptors/auth_interceptor.dart`
- `lib/core/network/interceptors/session_interceptor.dart`
- `lib/core/network/interceptors/geo_interceptor.dart`
- `lib/core/network/interceptors/retry_interceptor.dart`
- `lib/core/network/interceptors/rate_limit_interceptor.dart`
- `lib/core/storage/secure_storage_service.dart`
- `lib/features/auth/presentation/providers/auth_provider.dart`
- `lib/features/auth/presentation/screens/login_screen.dart`

#### Semana 4: Navegação e Roteamento

**Tarefas**:
- [ ] Configurar go_router
- [ ] Implementar rotas básicas (login, home)
- [ ] Implementar route guards (autenticação)
- [ ] Configurar deep linking
- [ ] Implementar navegação com estado preservado
- [ ] Testes de navegação

**Arquivos Criados**:
- `lib/shared/router/app_router.dart`
- `lib/shared/router/route_guards.dart`
- `lib/shared/router/route_names.dart`

### Critérios de Sucesso

- ✅ Projeto criado e configurado
- ✅ Design system funcionando (tema claro/escuro)
- ✅ Network layer configurado (interceptors funcionando)
- ✅ Autenticação básica funcionando
- ✅ Navegação configurada
- ✅ Testes passando (>80% cobertura)
- ✅ CI/CD funcionando

---

## **FRONTEND FASE 1: Mídia e Armazenamento** (3 semanas)

**Sincronizada com**: Backend Fase 8  
**Prioridade**: 🔴 Crítica  
**Duração**: 3 semanas (15 dias úteis)  
**Estimativa**: 120 horas

### Objetivo

Implementar suporte completo a mídia: upload, download, preview, galeria e integração com storage.

### Entregas

#### Semana 1: Upload e Download de Mídia

**Tarefas**:
- [ ] Implementar ImagePicker para seleção
- [ ] Implementar FilePicker para documentos
- [ ] Implementar upload de imagens (multipart/form-data)
- [ ] Implementar upload de vídeos
- [ ] Implementar progress tracking
- [ ] Implementar download de mídias
- [ ] Implementar cache de imagens (cached_network_image)
- [ ] Tela de upload de mídia

**Arquivos Criados**:
- `lib/features/media/data/models/media_model.dart`
- `lib/features/media/data/repositories/media_repository.dart`
- `lib/features/media/domain/entities/media_entity.dart`
- `lib/features/media/presentation/providers/media_provider.dart`
- `lib/features/media/presentation/widgets/image_picker_widget.dart`
- `lib/features/media/presentation/widgets/media_preview.dart`
- `lib/features/media/presentation/screens/upload_media_screen.dart`

#### Semana 2: Galeria e Preview

**Tarefas**:
- [ ] Implementar galeria de mídias
- [ ] Implementar viewer de imagens (zoom, pinch)
- [ ] Implementar viewer de vídeos (player)
- [ ] Implementar grid de mídias
- [ ] Implementar seleção múltipla
- [ ] Implementar compressão de imagens antes do upload
- [ ] Implementar thumbnail generation

**Arquivos Criados**:
- `lib/features/media/presentation/screens/media_gallery_screen.dart`
- `lib/features/media/presentation/screens/image_viewer_screen.dart`
- `lib/features/media/presentation/screens/video_player_screen.dart`
- `lib/features/media/presentation/widgets/media_grid.dart`
- `lib/shared/utils/image_utils.dart`

#### Semana 3: Integração e Cache

**Tarefas**:
- [ ] Integrar mídia com posts (preview)
- [ ] Integrar mídia com eventos (preview)
- [ ] Implementar cache local de mídias (Hive)
- [ ] Implementar sincronização offline
- [ ] Implementar fallback para imagens quebradas
- [ ] Otimizar performance de carregamento
- [ ] Testes de upload/download

**Arquivos Criados**:
- `lib/features/media/data/datasources/media_local_datasource.dart`
- `lib/core/storage/cache_service.dart`
- `lib/shared/widgets/cached_image.dart`

### Jornadas Implementadas

- **VISITOR/RESIDENT**: Upload de foto de perfil
- **VISITOR/RESIDENT**: Visualizar mídias em posts/eventos
- **VISITOR/RESIDENT**: Galeria de mídias

### Critérios de Sucesso

- ✅ Upload de imagens funcionando
- ✅ Upload de vídeos funcionando
- ✅ Preview de mídias funcionando
- ✅ Galeria de mídias funcionando
- ✅ Cache local funcionando
- ✅ Performance otimizada (< 2s para upload)
- ✅ Testes passando

---

## **FRONTEND FASE 2: Perfil de Usuário Completo** (3 semanas)

**Sincronizada com**: Backend Fase 9  
**Prioridade**: 🔴 Crítica  
**Duração**: 3 semanas (15 dias úteis)  
**Estimativa**: 120 horas

### Objetivo

Implementar perfil completo de usuário: avatar, bio, visualização de perfil, estatísticas e preferências.

### Entregas

#### Semana 1: Avatar e Bio

**Tarefas**:
- [ ] Tela de edição de perfil
- [ ] Upload de avatar (integração com Fase 1)
- [ ] Editor de bio (máx. 500 caracteres)
- [ ] Preview de avatar em tempo real
- [ ] Validação de bio
- [ ] Atualização de perfil via API
- [ ] Persistência local de perfil

**Arquivos Criados**:
- `lib/features/profile/presentation/screens/edit_profile_screen.dart`
- `lib/features/profile/presentation/widgets/avatar_picker.dart`
- `lib/features/profile/presentation/widgets/bio_editor.dart`
- `lib/features/profile/data/models/user_profile_model.dart`
- `lib/features/profile/data/repositories/user_profile_repository.dart`
- `lib/features/profile/presentation/providers/user_profile_provider.dart`

#### Semana 2: Visualização de Perfil

**Tarefas**:
- [ ] Tela de perfil próprio
- [ ] Tela de perfil de outros usuários
- [ ] Header de perfil (avatar, nome, bio)
- [ ] Estatísticas de contribuição (posts, eventos, territórios)
- [ ] Lista de posts do usuário
- [ ] Lista de eventos do usuário
- [ ] Badges e conquistas (preparação para Fase 7)
- [ ] Privacidade (respeitar configurações)

**Arquivos Criados**:
- `lib/features/profile/presentation/screens/profile_screen.dart`
- `lib/features/profile/presentation/screens/other_user_profile_screen.dart`
- `lib/features/profile/presentation/widgets/profile_header.dart`
- `lib/features/profile/presentation/widgets/profile_stats.dart`
- `lib/features/profile/presentation/widgets/profile_posts_list.dart`
- `lib/features/profile/presentation/widgets/profile_events_list.dart`

#### Semana 3: Preferências e Configurações

**Tarefas**:
- [ ] Tela de configurações
- [ ] Preferências de privacidade
- [ ] Preferências de notificações
- [ ] Configurações de tema (claro/escuro)
- [ ] Configurações de idioma (pt-BR/en-US)
- [ ] Exclusão de conta
- [ ] Exportação de dados pessoais
- [ ] Testes de perfil

**Arquivos Criados**:
- `lib/features/profile/presentation/screens/settings_screen.dart`
- `lib/features/profile/presentation/screens/privacy_settings_screen.dart`
- `lib/features/profile/presentation/screens/notification_settings_screen.dart`
- `lib/features/profile/data/models/user_preferences_model.dart`
- `lib/features/profile/presentation/providers/user_preferences_provider.dart`

### Jornadas Implementadas

- **VISITOR/RESIDENT**: Editar perfil próprio
- **VISITOR/RESIDENT**: Visualizar perfil de outros
- **VISITOR/RESIDENT**: Configurar preferências

### Critérios de Sucesso

- ✅ Avatar e bio editáveis
- ✅ Visualização de perfil funcionando
- ✅ Estatísticas exibidas corretamente
- ✅ Preferências persistidas
- ✅ Privacidade respeitada
- ✅ Testes passando

---

## **FRONTEND FASE 3: Mídias em Conteúdo** (4 semanas)

**Sincronizada com**: Backend Fase 10  
**Prioridade**: 🔴 Crítica  
**Duração**: 4 semanas (20 dias úteis)  
**Estimativa**: 160 horas

### Objetivo

Implementar suporte completo a mídias em posts e eventos: upload, preview, galeria integrada e GeoAnchors derivados.

### Entregas

#### Semana 1: Posts com Mídias

**Tarefas**:
- [ ] Integrar upload de mídias no criador de posts
- [ ] Preview de mídias durante criação
- [ ] Suporte a múltiplas mídias por post
- [ ] Ordenação de mídias
- [ ] Galeria de mídias no feed
- [ ] Viewer de mídias em posts
- [ ] Lazy loading de mídias

**Arquivos Criados**:
- `lib/features/feed/presentation/widgets/post_media_gallery.dart`
- `lib/features/feed/presentation/widgets/post_media_viewer.dart`
- `lib/features/feed/presentation/widgets/media_upload_preview.dart`

#### Semana 2: Eventos com Mídias

**Tarefas**:
- [ ] Integrar upload de mídias no criador de eventos
- [ ] Preview de mídias em eventos
- [ ] Galeria de mídias em eventos
- [ ] Mídias de capa para eventos
- [ ] Mídias de participantes em eventos
- [ ] Compartilhamento de mídias de eventos

**Arquivos Criados**:
- `lib/features/events/presentation/widgets/event_media_gallery.dart`
- `lib/features/events/presentation/widgets/event_cover_selector.dart`

#### Semana 3: GeoAnchors e Mapa

**Tarefas**:
- [ ] Exibir GeoAnchors derivados de mídias
- [ ] Pins no mapa baseados em GeoAnchors
- [ ] Integração feed ↔ mapa (seleção sincronizada)
- [ ] Visualização de posts no mapa
- [ ] Filtro de posts por localização

**Arquivos Criados**:
- `lib/features/map/presentation/widgets/geo_anchor_pin.dart`
- `lib/features/map/presentation/widgets/post_map_integration.dart`

#### Semana 4: Otimização e Performance

**Tarefas**:
- [ ] Compressão automática de imagens
- [ ] Thumbnails para preview rápido
- [ ] Cache inteligente de mídias
- [ ] Lazy loading otimizado
- [ ] Animações suaves (60fps)
- [ ] Testes de mídias em conteúdo

**Arquivos Criados**:
- `lib/shared/utils/image_compression.dart`
- `lib/shared/utils/thumbnail_generator.dart`

### Jornadas Implementadas

- **VISITOR/RESIDENT**: Criar posts com mídias
- **VISITOR/RESIDENT**: Criar eventos com mídias
- **VISITOR/RESIDENT**: Visualizar mídias no feed
- **VISITOR/RESIDENT**: Visualizar posts no mapa via GeoAnchors

### Critérios de Sucesso

- ✅ Posts com mídias funcionando
- ✅ Eventos com mídias funcionando
- ✅ GeoAnchors exibidos corretamente
- ✅ Integração feed ↔ mapa funcionando
- ✅ Performance otimizada (< 1s para carregar galeria)
- ✅ Testes passando

---

## **FRONTEND FASE 4: Edição e Gestão** (3 semanas)

**Sincronizada com**: Backend Fase 11  
**Prioridade**: 🟡 Importante  
**Duração**: 3 semanas (15 dias úteis)  
**Estimativa**: 120 horas

### Objetivo

Implementar edição e gestão de conteúdo: editar posts, editar eventos, deletar conteúdo, histórico de edições.

### Entregas

#### Semana 1: Edição de Posts

**Tarefas**:
- [ ] Tela de edição de posts
- [ ] Editar conteúdo (texto, markdown)
- [ ] Adicionar/remover mídias
- [ ] Editar visibilidade
- [ ] Editar tipo (NOTICE, ALERT, ANNOUNCEMENT)
- [ ] Histórico de edições (se disponível)
- [ ] Validações de edição

**Arquivos Criados**:
- `lib/features/feed/presentation/screens/edit_post_screen.dart`
- `lib/features/feed/presentation/widgets/post_editor.dart`

#### Semana 2: Edição de Eventos

**Tarefas**:
- [ ] Tela de edição de eventos
- [ ] Editar título e descrição
- [ ] Editar data/hora
- [ ] Editar localização
- [ ] Adicionar/remover mídias
- [ ] Editar tipo e visibilidade
- [ ] Cancelar evento

**Arquivos Criados**:
- `lib/features/events/presentation/screens/edit_event_screen.dart`
- `lib/features/events/presentation/widgets/event_editor.dart`

#### Semana 3: Gestão e Deleção

**Tarefas**:
- [ ] Confirmar deleção (dialog)
- [ ] Deletar posts
- [ ] Deletar eventos
- [ ] Soft delete (remover do feed)
- [ ] Lista de conteúdo criado pelo usuário
- [ ] Filtros e busca
- [ ] Testes de edição e deleção

**Arquivos Criados**:
- `lib/features/feed/presentation/screens/my_posts_screen.dart`
- `lib/features/events/presentation/screens/my_events_screen.dart`
- `lib/shared/widgets/delete_confirmation_dialog.dart`

### Jornadas Implementadas

- **RESIDENT**: Editar posts próprios
- **RESIDENT**: Editar eventos próprios
- **RESIDENT**: Deletar conteúdo próprio
- **RESIDENT**: Gerenciar conteúdo criado

### Critérios de Sucesso

- ✅ Edição de posts funcionando
- ✅ Edição de eventos funcionando
- ✅ Deleção funcionando
- ✅ Validações implementadas
- ✅ Testes passando

---

## **FRONTEND FASE 5: Comunicação por Email** (2 semanas)

**Sincronizada com**: Backend Fase 13  
**Prioridade**: 🔴 Crítica  
**Duração**: 2 semanas (10 dias úteis)  
**Estimativa**: 80 horas

### Objetivo

Implementar integração com sistema de emails: notificações por email, preferências de email, templates de email.

### Entregas

#### Semana 1: Preferências de Email

**Tarefas**:
- [ ] Configurar preferências de email
- [ ] Ativar/desativar notificações por email
- [ ] Selecionar tipos de notificações
- [ ] Frequência de emails (imediato, resumo diário, semanal)
- [ ] Integração com UserPreferences

**Arquivos Criados**:
- `lib/features/profile/presentation/screens/email_preferences_screen.dart`
- `lib/features/profile/presentation/widgets/email_notification_toggle.dart`

#### Semana 2: Templates e Previews

**Tarefas**:
- [ ] Preview de templates de email (se disponível)
- [ ] Teste de envio de email (dev)
- [ ] Histórico de emails enviados (se disponível)
- [ ] Verificar status de email (se disponível)
- [ ] Testes de preferências de email

**Arquivos Criados**:
- `lib/features/notifications/presentation/widgets/email_preview.dart`

### Jornadas Implementadas

- **VISITOR/RESIDENT**: Configurar notificações por email
- **VISITOR/RESIDENT**: Gerenciar preferências de email

### Critérios de Sucesso

- ✅ Preferências de email funcionando
- ✅ Notificações por email ativadas/desativadas corretamente
- ✅ Testes passando

---

## **FRONTEND FASE 6: Governança Comunitária e Votação** (4 semanas)

**Sincronizada com**: Backend Fase 14  
**Prioridade**: 🔴 Crítica  
**Duração**: 4 semanas (20 dias úteis)  
**Estimativa**: 160 horas

### Objetivo

Implementar sistema completo de governança: votações, interesses do território, dashboard de governança e participação cidadã.

### Entregas

#### Semana 1: Sistema de Votação

**Tarefas**:
- [ ] Listar votações ativas
- [ ] Visualizar detalhes de votação
- [ ] Votar em proposições
- [ ] Visualizar resultados em tempo real
- [ ] Histórico de votações
- [ ] Notificações de novas votações

**Arquivos Criados**:
- `lib/features/governance/data/models/vote_model.dart`
- `lib/features/governance/data/models/vote_option_model.dart`
- `lib/features/governance/data/repositories/vote_repository.dart`
- `lib/features/governance/presentation/providers/votes_provider.dart`
- `lib/features/governance/presentation/screens/votes_list_screen.dart`
- `lib/features/governance/presentation/screens/vote_detail_screen.dart`
- `lib/features/governance/presentation/widgets/vote_card.dart`
- `lib/features/governance/presentation/widgets/vote_results_chart.dart`

#### Semana 2: Criar Votações (CURATOR)

**Tarefas**:
- [ ] Tela de criação de votação
- [ ] Definir título e descrição
- [ ] Adicionar opções de voto
- [ ] Configurar duração (início/fim)
- [ ] Configurar regras (quorum, maioria)
- [ ] Publicar votação
- [ ] Gerenciar votações ativas

**Arquivos Criados**:
- `lib/features/governance/presentation/screens/create_vote_screen.dart`
- `lib/features/governance/presentation/widgets/vote_option_editor.dart`
- `lib/features/governance/presentation/widgets/vote_rules_config.dart`

#### Semana 3: Interesses do Território

**Tarefas**:
- [ ] Visualizar interesses do território
- [ ] Sugerir novos interesses (RESIDENT)
- [ ] Aprovar interesses (CURATOR)
- [ ] Editar interesses (CURATOR)
- [ ] Tags de interesses em posts/eventos
- [ ] Filtro por interesses

**Arquivos Criados**:
- `lib/features/governance/data/models/territory_interest_model.dart`
- `lib/features/governance/presentation/screens/territory_interests_screen.dart`
- `lib/features/governance/presentation/widgets/interest_tag.dart`
- `lib/features/governance/presentation/widgets/interest_suggestions_list.dart`

#### Semana 4: Dashboard de Governança (CURATOR)

**Tarefas**:
- [ ] Dashboard de governança territorial
- [ ] Estatísticas de participação
- [ ] Votações por status (ativas, finalizadas, pendentes)
- [ ] Interesses mais populares
- [ ] Relatórios de governança
- [ ] Testes de governança

**Arquivos Criados**:
- `lib/features/governance/presentation/screens/governance_dashboard_screen.dart`
- `lib/features/governance/presentation/widgets/governance_stats.dart`
- `lib/features/governance/presentation/widgets/participation_chart.dart`

### Jornadas Implementadas

- **RESIDENT**: Participar de votações
- **RESIDENT**: Visualizar resultados de votações
- **RESIDENT**: Sugerir interesses do território
- **RESIDENT**: Visualizar interesses do território
- **CURATOR**: Criar votações
- **CURATOR**: Gerenciar interesses do território
- **CURATOR**: Dashboard de governança

### Critérios de Sucesso

- ✅ Sistema de votação funcionando
- ✅ Criação de votações funcionando
- ✅ Interesses do território funcionando
- ✅ Dashboard de governança funcionando
- ✅ Testes passando

---

## **FRONTEND FASE 7: Sistema de Gamificação Harmoniosa** (4 semanas)

**Sincronizada com**: Backend Fase 17  
**Prioridade**: 🟡 Importante  
**Duração**: 4 semanas (20 dias úteis)  
**Estimativa**: 160 horas

### Objetivo

Implementar sistema de gamificação: pontos, badges, níveis, reconhecimento comunitário e dashboard de contribuições.

### Entregas

#### Semana 1: Sistema de Pontos e Contribuições

**Tarefas**:
- [ ] Visualizar pontos totais
- [ ] Visualizar contribuições recentes
- [ ] Histórico de contribuições
- [ ] Tipos de contribuições (posts, eventos, marketplace, etc.)
- [ ] Cálculo de pontos baseado em valor agregado
- [ ] Multiplicadores por interesses do território

**Arquivos Criados**:
- `lib/features/gamification/data/models/contribution_model.dart`
- `lib/features/gamification/data/models/contribution_type.dart`
- `lib/features/gamification/data/repositories/contribution_repository.dart`
- `lib/features/gamification/presentation/providers/contributions_provider.dart`
- `lib/features/gamification/presentation/widgets/points_display.dart`
- `lib/features/gamification/presentation/widgets/contributions_history.dart`

#### Semana 2: Badges e Conquistas

**Tarefas**:
- [ ] Visualizar badges conquistadas
- [ ] Tipos de badges (Criador, Organizador, Vendedor, etc.)
- [ ] Progresso para próximo badge
- [ ] Notificações de novas conquistas
- [ ] Badges no perfil
- [ ] Galeria de badges disponíveis

**Arquivos Criados**:
- `lib/features/gamification/data/models/badge_model.dart`
- `lib/features/gamification/presentation/widgets/badge_card.dart`
- `lib/features/gamification/presentation/widgets/badge_gallery.dart`
- `lib/features/gamification/presentation/widgets/badge_progress.dart`

#### Semana 3: Níveis e Reconhecimento

**Tarefas**:
- [ ] Sistema de níveis (baseado em pontos)
- [ ] Visualizar nível atual
- [ ] Progresso para próximo nível
- [ ] Reconhecimento comunitário (discreto)
- [ ] Ranking opcional (se habilitado pelo território)
- [ ] Dashboard de contribuições

**Arquivos Criados**:
- `lib/features/gamification/data/models/level_model.dart`
- `lib/features/gamification/presentation/widgets/level_progress.dart`
- `lib/features/gamification/presentation/screens/contributions_dashboard_screen.dart`

#### Semana 4: Integração e Visualização Suave

**Tarefas**:
- [ ] Integrar gamificação em perfil
- [ ] Badges discretos em posts/eventos
- [ ] Notificações ocasionais de conquistas
- [ ] Feed não manipulado (mantém ordem cronológica)
- [ ] Ética e transparência (usuário vê como ganha pontos)
- [ ] Testes de gamificação

**Arquivos Criados**:
- `lib/features/gamification/presentation/widgets/gamification_badge.dart`
- `lib/features/gamification/presentation/widgets/contribution_tooltip.dart`

### Jornadas Implementadas

- **RESIDENT**: Visualizar pontos e contribuições
- **RESIDENT**: Ver badges conquistadas
- **RESIDENT**: Acompanhar progresso de níveis
- **RESIDENT**: Dashboard de contribuições

### Critérios de Sucesso

- ✅ Sistema de pontos funcionando
- ✅ Badges funcionando
- ✅ Níveis funcionando
- ✅ Integração com perfil funcionando
- ✅ Visualização suave e não invasiva
- ✅ Testes passando

---

## **FRONTEND FASE 8: Saúde Territorial e Monitoramento** (5 semanas)

**Sincronizada com**: Backend Fase 18  
**Prioridade**: 🔴 Alta  
**Duração**: 5 semanas (25 dias úteis)  
**Estimativa**: 200 horas

### Objetivo

Implementar sistema completo de saúde territorial: observações, sensores, indicadores, ações territoriais e atividades específicas.

### Entregas

#### Semana 1: Observações de Saúde

**Tarefas**:
- [ ] Criar observação de saúde (água, ar, solo, biodiversidade, resíduos, segurança, mobilidade, bem-estar)
- [ ] Selecionar domínio de saúde
- [ ] Selecionar severidade (INFO, WARNING, URGENT)
- [ ] Adicionar descrição e localização
- [ ] Listar observações do território
- [ ] Confirmar observações de outros usuários
- [ ] Filtros por domínio, severidade, status

**Arquivos Criados**:
- `lib/features/health/data/models/health_observation_model.dart`
- `lib/features/health/data/models/health_domain.dart`
- `lib/features/health/data/models/health_severity.dart`
- `lib/features/health/data/repositories/health_repository.dart`
- `lib/features/health/presentation/providers/health_observations_provider.dart`
- `lib/features/health/presentation/screens/health_observations_list_screen.dart`
- `lib/features/health/presentation/screens/create_health_observation_screen.dart`
- `lib/features/health/presentation/widgets/health_observation_card.dart`
- `lib/features/health/presentation/widgets/health_domain_selector.dart`

#### Semana 2: Sensores e Indicadores

**Tarefas**:
- [ ] Visualizar sensores do território
- [ ] Visualizar leituras de sensores (gráficos)
- [ ] Registrar sensor (CURATOR)
- [ ] Dashboard de indicadores de saúde
- [ ] Gráficos de tendências
- [ ] Alertas automáticos quando indicadores pioram

**Arquivos Criados**:
- `lib/features/health/data/models/sensor_device_model.dart`
- `lib/features/health/data/models/sensor_reading_model.dart`
- `lib/features/health/data/models/health_indicator_model.dart`
- `lib/features/health/presentation/screens/sensors_list_screen.dart`
- `lib/features/health/presentation/screens/health_dashboard_screen.dart`
- `lib/features/health/presentation/widgets/sensor_chart.dart`
- `lib/features/health/presentation/widgets/indicator_trend_chart.dart`

#### Semana 3: Ações Territoriais

**Tarefas**:
- [ ] Criar ação territorial (mutirão, manutenção, educação, restauração, monitoramento)
- [ ] Organizar ação (data, hora, localização)
- [ ] Participar de ação
- [ ] Listar ações do território
- [ ] Visualizar participantes
- [ ] Confirmar participação
- [ ] Status de ações (PLANNED, IN_PROGRESS, DONE, CANCELLED)

**Arquivos Criados**:
- `lib/features/health/data/models/territory_action_model.dart`
- `lib/features/health/data/models/territory_action_type.dart`
- `lib/features/health/presentation/screens/territory_actions_list_screen.dart`
- `lib/features/health/presentation/screens/create_territory_action_screen.dart`
- `lib/features/health/presentation/widgets/territory_action_card.dart`
- `lib/features/health/presentation/widgets/action_participants_list.dart`

#### Semana 4: Atividades Específicas

**Tarefas**:
- [ ] Reportar coleta de resíduos
- [ ] Reportar plantio
- [ ] Reportar manutenção de recursos naturais
- [ ] Listar atividades do território
- [ ] Estatísticas de atividades
- [ ] Integração com gamificação (pontos por atividade)

**Arquivos Criados**:
- `lib/features/health/data/models/waste_collection_model.dart`
- `lib/features/health/data/models/tree_planting_model.dart`
- `lib/features/health/presentation/screens/report_waste_collection_screen.dart`
- `lib/features/health/presentation/screens/report_tree_planting_screen.dart`
- `lib/features/health/presentation/widgets/activity_stats.dart`

#### Semana 5: Integração e Dashboard Completo

**Tarefas**:
- [ ] Dashboard completo de saúde territorial
- [ ] Integração com mapa (observações como pins)
- [ ] Integração com feed (observações como posts)
- [ ] Integração com gamificação
- [ ] Notificações de alertas de saúde
- [ ] Testes de saúde territorial

**Arquivos Criados**:
- `lib/features/health/presentation/screens/health_dashboard_screen.dart`
- `lib/features/map/presentation/widgets/health_observation_pin.dart`

### Jornadas Implementadas

- **RESIDENT**: Criar observação de saúde
- **RESIDENT**: Confirmar observações
- **RESIDENT**: Participar de ações territoriais
- **RESIDENT**: Reportar atividades (coleta, plantio)
- **RESIDENT**: Visualizar dashboard de saúde
- **CURATOR**: Registrar sensores
- **CURATOR**: Monitorar indicadores

### Critérios de Sucesso

- ✅ Observações de saúde funcionando
- ✅ Sensores e indicadores funcionando
- ✅ Ações territoriais funcionando
- ✅ Atividades específicas funcionando
- ✅ Dashboard completo funcionando
- ✅ Integrações funcionando
- ✅ Testes passando

---

## **FRONTEND FASE 9: Sistema de Moeda Territorial** (5 semanas)

**Sincronizada com**: Backend Fase 20  
**Prioridade**: 🟡 Alta  
**Duração**: 5 semanas (25 dias úteis)  
**Estimativa**: 200 horas

### Objetivo

Implementar sistema completo de moeda territorial: wallet, transações, mint por atividades, transferências e dashboard financeiro.

### Entregas

#### Semana 1: Wallet e Balanço

**Tarefas**:
- [ ] Visualizar wallet (saldo em moeda territorial)
- [ ] Histórico de transações
- [ ] Tipos de transações (mint, transfer, purchase, etc.)
- [ ] Filtros por tipo, data, valor
- [ ] Detalhes de transação
- [ ] QR code para recebimento

**Arquivos Criados**:
- `lib/features/currency/data/models/currency_wallet_model.dart`
- `lib/features/currency/data/models/currency_transaction_model.dart`
- `lib/features/currency/data/repositories/currency_repository.dart`
- `lib/features/currency/presentation/providers/currency_wallet_provider.dart`
- `lib/features/currency/presentation/screens/wallet_screen.dart`
- `lib/features/currency/presentation/widgets/wallet_balance_card.dart`
- `lib/features/currency/presentation/widgets/transaction_history_list.dart`
- `lib/features/currency/presentation/widgets/qr_code_receiver.dart`

#### Semana 2: Mint por Atividades

**Tarefas**:
- [ ] Visualizar atividades que geram moeda
- [ ] Mint automático após atividades (notificação)
- [ ] Histórico de mint por atividade
- [ ] Dashboard de ganhos por atividade
- [ ] Integração com gamificação

**Arquivos Criados**:
- `lib/features/currency/presentation/widgets/mint_activities_list.dart`
- `lib/features/currency/presentation/widgets/mint_notification.dart`

#### Semana 3: Transferências

**Tarefas**:
- [ ] Transferir moeda para outro usuário
- [ ] Escanear QR code para transferência
- [ ] Selecionar destinatário
- [ ] Inserir valor
- [ ] Confirmar transferência
- [ ] Receber transferência (notificação)

**Arquivos Criados**:
- `lib/features/currency/presentation/screens/transfer_currency_screen.dart`
- `lib/features/currency/presentation/widgets/qr_code_scanner.dart`
- `lib/features/currency/presentation/widgets/transfer_confirmation_dialog.dart`

#### Semana 4: Integração com Marketplace

**Tarefas**:
- [ ] Pagar com moeda territorial no checkout
- [ ] Receber moeda territorial em vendas
- [ ] Conversão opcional (moeda territorial ↔ real)
- [ ] Histórico de pagamentos/recebimentos
- [ ] Integração com carrinho

**Arquivos Criados**:
- `lib/features/market/presentation/widgets/currency_payment_method.dart`
- `lib/features/currency/presentation/widgets/currency_converter.dart`

#### Semana 5: Dashboard e Relatórios

**Tarefas**:
- [ ] Dashboard financeiro territorial (CURATOR)
- [ ] Estatísticas de circulação de moeda
- [ ] Gráficos de transações
- [ ] Relatórios de mint por atividade
- [ ] Testes de moeda territorial

**Arquivos Criados**:
- `lib/features/currency/presentation/screens/currency_dashboard_screen.dart`
- `lib/features/currency/presentation/widgets/currency_stats_chart.dart`

### Jornadas Implementadas

- **RESIDENT**: Visualizar wallet
- **RESIDENT**: Receber moeda por atividades
- **RESIDENT**: Transferir moeda
- **RESIDENT**: Pagar com moeda no marketplace
- **RESIDENT**: Receber moeda em vendas
- **CURATOR**: Dashboard financeiro territorial

### Critérios de Sucesso

- ✅ Wallet funcionando
- ✅ Mint por atividades funcionando
- ✅ Transferências funcionando
- ✅ Integração com marketplace funcionando
- ✅ Dashboard funcionando
- ✅ Testes passando

---

## **FRONTEND FASE 10: Sistema de Compra Coletiva** (4 semanas)

**Sincronizada com**: Backend Fase 23  
**Prioridade**: 🔴 Alta  
**Duração**: 4 semanas (20 dias úteis)  
**Estimativa**: 160 horas

### Objetivo

Implementar sistema de compra coletiva: criar grupos de compra, aderir a grupos, checkout coletivo e gestão de grupos.

### Entregas

#### Semana 1: Grupos de Compra

**Tarefas**:
- [ ] Criar grupo de compra (RESIDENT)
- [ ] Definir produto e meta (quantidade mínima)
- [ ] Definir prazo
- [ ] Visualizar grupos ativos
- [ ] Filtrar por status (aberto, em andamento, fechado)
- [ ] Visualizar progresso do grupo (quantidade atingida)

**Arquivos Criados**:
- `lib/features/collective_purchase/data/models/collective_purchase_group_model.dart`
- `lib/features/collective_purchase/data/repositories/collective_purchase_repository.dart`
- `lib/features/collective_purchase/presentation/providers/collective_purchase_provider.dart`
- `lib/features/collective_purchase/presentation/screens/collective_purchase_groups_list_screen.dart`
- `lib/features/collective_purchase/presentation/screens/create_collective_purchase_group_screen.dart`
- `lib/features/collective_purchase/presentation/widgets/collective_purchase_group_card.dart`
- `lib/features/collective_purchase/presentation/widgets/group_progress_bar.dart`

#### Semana 2: Aderir a Grupos

**Tarefas**:
- [ ] Visualizar detalhes do grupo
- [ ] Adicionar quantidade desejada
- [ ] Confirmar adesão
- [ ] Visualizar participantes do grupo
- [ ] Cancelar adesão (antes do fechamento)
- [ ] Notificações de progresso do grupo

**Arquivos Criados**:
- `lib/features/collective_purchase/presentation/screens/collective_purchase_group_detail_screen.dart`
- `lib/features/collective_purchase/presentation/widgets/group_participants_list.dart`
- `lib/features/collective_purchase/presentation/widgets/join_group_dialog.dart`

#### Semana 3: Checkout Coletivo

**Tarefas**:
- [ ] Notificação quando meta atingida
- [ ] Processar checkout coletivo
- [ ] Pagamento individual (cada participante paga sua parte)
- [ ] Confirmação de pagamento
- [ ] Organização de entrega
- [ ] Notificações de status da compra

**Arquivos Criados**:
- `lib/features/collective_purchase/presentation/screens/collective_checkout_screen.dart`
- `lib/features/collective_purchase/presentation/widgets/collective_payment_summary.dart`
- `lib/features/collective_purchase/presentation/widgets/delivery_organization_widget.dart`

#### Semana 4: Gestão e Relatórios

**Tarefas**:
- [ ] Gerenciar grupos criados (organizador)
- [ ] Cancelar grupo (antes de fechar)
- [ ] Estender prazo (se não atingiu meta)
- [ ] Relatórios de compras coletivas
- [ ] Histórico de participações
- [ ] Testes de compra coletiva

**Arquivos Criados**:
- `lib/features/collective_purchase/presentation/screens/my_collective_purchase_groups_screen.dart`
- `lib/features/collective_purchase/presentation/widgets/group_management_actions.dart`

### Jornadas Implementadas

- **RESIDENT**: Criar grupo de compra
- **RESIDENT**: Aderir a grupos
- **RESIDENT**: Participar de checkout coletivo
- **RESIDENT**: Gerenciar grupos criados
- **RESIDENT**: Visualizar histórico de compras coletivas

### Critérios de Sucesso

- ✅ Criação de grupos funcionando
- ✅ Adesão a grupos funcionando
- ✅ Checkout coletivo funcionando
- ✅ Gestão de grupos funcionando
- ✅ Testes passando

---

## **FRONTEND FASE 11: Sistema de Trocas Comunitárias** (3 semanas)

**Sincronizada com**: Backend Fase 24  
**Prioridade**: 🟡 Alta  
**Duração**: 3 semanas (15 dias úteis)  
**Estimativa**: 120 horas

### Objetivo

Implementar sistema de trocas comunitárias: criar ofertas, buscar trocas, negociar e finalizar trocas.

### Entregas

#### Semana 1: Criar e Listar Ofertas

**Tarefas**:
- [ ] Criar oferta de troca (item oferecido + item desejado)
- [ ] Adicionar descrição e fotos
- [ ] Listar ofertas ativas
- [ ] Filtrar por categoria, proximidade
- [ ] Visualizar detalhes da oferta

**Arquivos Criados**:
- `lib/features/trades/data/models/trade_offer_model.dart`
- `lib/features/trades/data/repositories/trade_repository.dart`
- `lib/features/trades/presentation/providers/trades_provider.dart`
- `lib/features/trades/presentation/screens/trade_offers_list_screen.dart`
- `lib/features/trades/presentation/screens/create_trade_offer_screen.dart`
- `lib/features/trades/presentation/widgets/trade_offer_card.dart`

#### Semana 2: Negociação

**Tarefas**:
- [ ] Propor troca alternativa
- [ ] Aceitar/rejeitar proposta
- [ ] Chat de negociação
- [ ] Visualizar histórico de propostas
- [ ] Notificações de novas propostas

**Arquivos Criados**:
- `lib/features/trades/data/models/trade_proposal_model.dart`
- `lib/features/trades/presentation/screens/trade_negotiation_screen.dart`
- `lib/features/trades/presentation/widgets/trade_proposal_card.dart`

#### Semana 3: Finalização e Gestão

**Tarefas**:
- [ ] Confirmar troca (ambas as partes)
- [ ] Marcar como concluída
- [ ] Avaliar troca realizada
- [ ] Histórico de trocas
- [ ] Gerenciar ofertas criadas
- [ ] Testes de trocas

**Arquivos Criados**:
- `lib/features/trades/presentation/screens/trade_confirmation_screen.dart`
- `lib/features/trades/presentation/widgets/trade_rating_widget.dart`
- `lib/features/trades/presentation/screens/my_trades_screen.dart`

### Jornadas Implementadas

- **RESIDENT**: Criar oferta de troca
- **RESIDENT**: Buscar ofertas
- **RESIDENT**: Negociar troca
- **RESIDENT**: Finalizar troca
- **RESIDENT**: Avaliar troca

### Critérios de Sucesso

- ✅ Criação de ofertas funcionando
- ✅ Negociação funcionando
- ✅ Finalização funcionando
- ✅ Testes passando

---

## **FRONTEND FASE 12: Hub de Serviços Digitais Base** (4 semanas)

**Sincronizada com**: Backend Fase 25  
**Prioridade**: 🔴 Alta  
**Duração**: 4 semanas (20 dias úteis)  
**Estimativa**: 160 horas

### Objetivo

Implementar infraestrutura base para serviços digitais: conectar contas de serviços, rastreamento de consumo e extrato de consumo.

### Entregas

#### Semana 1: Conexão de Serviços

**Tarefas**:
- [ ] Listar serviços digitais disponíveis (IA, Storage, Translation, etc.)
- [ ] Conectar conta de serviço (credenciais)
- [ ] Validar conexão
- [ ] Desconectar serviço
- [ ] Gerenciar serviços conectados

**Arquivos Criados**:
- `lib/features/digital_services/data/models/digital_service_model.dart`
- `lib/features/digital_services/data/models/service_provider_model.dart`
- `lib/features/digital_services/data/repositories/digital_service_repository.dart`
- `lib/features/digital_services/presentation/providers/digital_services_provider.dart`
- `lib/features/digital_services/presentation/screens/digital_services_list_screen.dart`
- `lib/features/digital_services/presentation/screens/connect_service_screen.dart`
- `lib/features/digital_services/presentation/widgets/service_card.dart`

#### Semana 2: Rastreamento de Consumo

**Tarefas**:
- [ ] Visualizar consumo por serviço
- [ ] Visualizar consumo por período (diário, semanal, mensal)
- [ ] Gráficos de consumo
- [ ] Alertas de quota próxima ao limite
- [ ] Histórico detalhado de uso

**Arquivos Criados**:
- `lib/features/digital_services/data/models/consumption_log_model.dart`
- `lib/features/digital_services/presentation/screens/consumption_dashboard_screen.dart`
- `lib/features/digital_services/presentation/widgets/consumption_chart.dart`
- `lib/features/digital_services/presentation/widgets/quota_alert_banner.dart`

#### Semana 3: Extrato de Consumo

**Tarefas**:
- [ ] Extrato detalhado de consumo
- [ ] Custos estimados (quando disponível)
- [ ] Filtros por serviço, período
- [ ] Exportar extrato (PDF/CSV)
- [ ] Comparar períodos

**Arquivos Criados**:
- `lib/features/digital_services/presentation/screens/consumption_statement_screen.dart`
- `lib/features/digital_services/presentation/widgets/consumption_summary_card.dart`

#### Semana 4: Feature Flags e Configurações

**Tarefas**:
- [ ] Feature flags territoriais (`DigitalServicesEnabled`)
- [ ] Feature flags por categoria
- [ ] Preferências do usuário
- [ ] Quotas configuráveis
- [ ] Testes de serviços digitais

**Arquivos Criados**:
- `lib/features/digital_services/presentation/screens/digital_services_settings_screen.dart`

### Jornadas Implementadas

- **RESIDENT**: Conectar serviços digitais
- **RESIDENT**: Visualizar consumo
- **RESIDENT**: Gerenciar quotas
- **RESIDENT**: Exportar extrato

### Critérios de Sucesso

- ✅ Conexão de serviços funcionando
- ✅ Rastreamento de consumo funcionando
- ✅ Extrato funcionando
- ✅ Feature flags funcionando
- ✅ Testes passando

---

## **FRONTEND FASE 13: Chat com IA e Consumo Consciente** (3 semanas)

**Sincronizada com**: Backend Fase 26  
**Prioridade**: 🔴 Alta  
**Duração**: 3 semanas (15 dias úteis)  
**Estimativa**: 120 horas

### Objetivo

Implementar IA integrada ao chat: seleção de provedor, uso em conversas, rastreamento de consumo e quotas.

### Entregas

#### Semana 1: Integração de IA no Chat

**Tarefas**:
- [ ] Botão de IA em conversas
- [ ] Selecionar provedor de IA (OpenAI, Claude, Gemini, etc.)
- [ ] Enviar mensagem com IA
- [ ] Receber resposta de IA na conversa
- [ ] Indicador visual de mensagens com IA
- [ ] Histórico de mensagens com IA

**Arquivos Criados**:
- `lib/features/chat/presentation/widgets/ai_chat_button.dart`
- `lib/features/chat/presentation/widgets/ai_provider_selector.dart`
- `lib/features/chat/presentation/widgets/ai_message_bubble.dart`
- `lib/features/chat/data/models/ai_message_model.dart`

#### Semana 2: Rastreamento de Consumo por Conversa

**Tarefas**:
- [ ] Exibir consumo de tokens por mensagem
- [ ] Exibir custo estimado por mensagem
- [ ] Consumo total da conversa
- [ ] Gráfico de consumo da conversa
- [ ] Integração com extrato de consumo

**Arquivos Criados**:
- `lib/features/chat/presentation/widgets/ai_consumption_indicator.dart`
- `lib/features/chat/presentation/widgets/conversation_consumption_chart.dart`

#### Semana 3: Quotas e Limites

**Tarefas**:
- [ ] Alertas quando próximo ao limite (80%, 90%, 100%)
- [ ] Bloqueio quando quota esgotada
- [ ] Mensagem informativa sobre quota
- [ ] Opção de aumentar quota (se disponível)
- [ ] Testes de chat com IA

**Arquivos Criados**:
- `lib/features/chat/presentation/widgets/quota_alert_dialog.dart`
- `lib/features/chat/presentation/widgets/quota_exhausted_message.dart`

### Jornadas Implementadas

- **RESIDENT**: Usar IA em conversas
- **RESIDENT**: Selecionar provedor de IA
- **RESIDENT**: Acompanhar consumo de IA
- **RESIDENT**: Gerenciar quotas de IA

### Critérios de Sucesso

- ✅ Integração de IA funcionando
- ✅ Rastreamento de consumo funcionando
- ✅ Quotas funcionando
- ✅ Testes passando

---

## **FRONTEND FASE 14: Negociação Territorial e Assinatura Coletiva** (4 semanas)

**Sincronizada com**: Backend Fase 27  
**Prioridade**: 🔴 Alta  
**Duração**: 4 semanas (20 dias úteis)  
**Estimativa**: 160 horas

### Objetivo

Implementar sistema de negociação territorial: acordos de serviço, pool de quotas, alocação para membros e subsídios.

### Entregas

#### Semana 1: Acordos de Serviço (CURATOR)

**Tarefas**:
- [ ] Criar proposta de acordo de serviço
- [ ] Definir quotas negociadas
- [ ] Definir período de validade
- [ ] Criar votação para aprovar acordo (integração com Fase 6)
- [ ] Visualizar acordos ativos
- [ ] Gerenciar acordos

**Arquivos Criados**:
- `lib/features/territory_services/data/models/territory_service_agreement_model.dart`
- `lib/features/territory_services/data/repositories/territory_service_repository.dart`
- `lib/features/territory_services/presentation/providers/territory_services_provider.dart`
- `lib/features/territory_services/presentation/screens/create_service_agreement_screen.dart`
- `lib/features/territory_services/presentation/screens/service_agreements_list_screen.dart`

#### Semana 2: Pool de Quotas e Distribuição

**Tarefas**:
- [ ] Visualizar pool de quotas territoriais
- [ ] Visualizar política de distribuição (EQUAL, NEED_BASED, etc.)
- [ ] Solicitar quota territorial (RESIDENT)
- [ ] Aprovar solicitação de quota (CURATOR)
- [ ] Visualizar alocações de quota

**Arquivos Criados**:
- `lib/features/territory_services/presentation/screens/territory_quota_pool_screen.dart`
- `lib/features/territory_services/presentation/screens/request_quota_screen.dart`
- `lib/features/territory_services/presentation/widgets/quota_allocation_card.dart`

#### Semana 3: Subsídios e Inclusão

**Tarefas**:
- [ ] Identificar membros sem quota pessoal
- [ ] Alocação automática de quota territorial (política NEED_BASED)
- [ ] Visualizar subsídios concedidos
- [ ] Dashboard de inclusão (CURATOR)

**Arquivos Criados**:
- `lib/features/territory_services/presentation/screens/subsidies_dashboard_screen.dart`
- `lib/features/territory_services/presentation/widgets/subsidy_card.dart`

#### Semana 4: Dashboard Territorial

**Tarefas**:
- [ ] Dashboard territorial de serviços (CURATOR)
- [ ] Serviços negociados pelo território
- [ ] Quota disponível por serviço
- [ ] Uso e consumo por membro
- [ ] Custos e subsídios
- [ ] Testes de serviços territoriais

**Arquivos Criados**:
- `lib/features/territory_services/presentation/screens/territory_services_dashboard_screen.dart`
- `lib/features/territory_services/presentation/widgets/territory_services_stats.dart`

### Jornadas Implementadas

- **CURATOR**: Criar acordos de serviço
- **CURATOR**: Gerenciar pool de quotas
- **CURATOR**: Aprovar solicitações de quota
- **CURATOR**: Dashboard territorial de serviços
- **RESIDENT**: Solicitar quota territorial
- **RESIDENT**: Visualizar quota alocada

### Critérios de Sucesso

- ✅ Acordos de serviço funcionando
- ✅ Pool de quotas funcionando
- ✅ Alocação funcionando
- ✅ Subsídios funcionando
- ✅ Dashboard funcionando
- ✅ Testes passando

---

## **FRONTEND FASE 15: Banco de Sementes e Mudas Territorial** (4 semanas)

**Sincronizada com**: Backend Fase 28  
**Prioridade**: 🟡 Média-Alta  
**Duração**: 4 semanas (20 dias úteis)  
**Estimativa**: 160 horas

### Objetivo

Implementar sistema de banco de sementes: catálogo, doações, solicitações, rastreabilidade e eventos de troca.

### Entregas

#### Semana 1: Catálogo de Sementes

**Tarefas**:
- [ ] Visualizar catálogo de sementes do território
- [ ] Filtrar por espécie, variedade, estação
- [ ] Visualizar detalhes da semente (origem, características, qualidade)
- [ ] Visualizar estoque e disponibilidade
- [ ] Rastreabilidade (quem doou, quando, multiplicação)

**Arquivos Criados**:
- `lib/features/seed_bank/data/models/seed_catalog_model.dart`
- `lib/features/seed_bank/data/models/seed_model.dart`
- `lib/features/seed_bank/data/repositories/seed_bank_repository.dart`
- `lib/features/seed_bank/presentation/providers/seed_bank_provider.dart`
- `lib/features/seed_bank/presentation/screens/seed_catalog_screen.dart`
- `lib/features/seed_bank/presentation/widgets/seed_card.dart`
- `lib/features/seed_bank/presentation/widgets/seed_details_dialog.dart`

#### Semana 2: Doações de Sementes

**Tarefas**:
- [ ] Doar sementes para o banco (RESIDENT)
- [ ] Preencher informações (espécie, variedade, quantidade, origem)
- [ ] Upload de fotos da semente
- [ ] Enviar para revisão (WorkQueue)
- [ ] Acompanhar status da doação
- [ ] Aprovar/rejeitar doação (CURATOR)

**Arquivos Criados**:
- `lib/features/seed_bank/presentation/screens/donate_seeds_screen.dart`
- `lib/features/seed_bank/presentation/widgets/seed_donation_form.dart`
- `lib/features/seed_bank/presentation/screens/seed_donations_queue_screen.dart`

#### Semana 3: Solicitações e Retiradas

**Tarefas**:
- [ ] Solicitar sementes do banco (RESIDENT)
- [ ] Selecionar sementes desejadas
- [ ] Definir quantidade
- [ ] Aprovação automática ou por votação
- [ ] Confirmar retirada
- [ ] Compromisso de devolução (opcional)

**Arquivos Criados**:
- `lib/features/seed_bank/presentation/screens/request_seeds_screen.dart`
- `lib/features/seed_bank/presentation/widgets/seed_request_form.dart`
- `lib/features/seed_bank/presentation/screens/my_seed_requests_screen.dart`

#### Semana 4: Eventos de Troca e Integrações

**Tarefas**:
- [ ] Criar evento de troca de sementes
- [ ] Participar de evento de troca
- [ ] Integração com marketplace (sementes como ItemType.SEED)
- [ ] Integração com gamificação (pontos por doação)
- [ ] Integração com notificações
- [ ] Testes de banco de sementes

**Arquivos Criados**:
- `lib/features/seed_bank/presentation/screens/seed_exchange_event_screen.dart`
- `lib/features/seed_bank/presentation/widgets/seed_exchange_event_card.dart`

### Jornadas Implementadas

- **RESIDENT**: Visualizar catálogo de sementes
- **RESIDENT**: Doar sementes
- **RESIDENT**: Solicitar sementes
- **RESIDENT**: Participar de eventos de troca
- **CURATOR**: Revisar doações
- **CURATOR**: Gerenciar catálogo

### Critérios de Sucesso

- ✅ Catálogo funcionando
- ✅ Doações funcionando
- ✅ Solicitações funcionando
- ✅ Eventos de troca funcionando
- ✅ Integrações funcionando
- ✅ Testes passando

---

## ✅ Critérios de Qualidade e Entrega

### Critérios por Fase

Cada fase deve atender aos seguintes critérios antes de ser considerada completa:

#### Funcionalidade
- ✅ Todas as funcionalidades especificadas implementadas
- ✅ Jornadas de usuário testadas e validadas
- ✅ Integrações com outras features funcionando
- ✅ Edge cases tratados

#### Design e UX
- ✅ Design system respeitado
- ✅ Animações suaves (60fps)
- ✅ Tempos de resposta < 200ms
- ✅ Acessibilidade (WCAG AA)
- ✅ Responsividade (mobile e tablet)

#### Testes
- ✅ Cobertura mínima de 80% (unitários)
- ✅ Cobertura mínima de 60% (widget)
- ✅ Testes de integração para fluxos críticos
- ✅ Testes E2E para jornadas principais

#### Performance
- ✅ Tempo de build < 2 minutos
- ✅ Tamanho do APK < 50MB (release)
- ✅ Tempo de startup < 3 segundos
- ✅ Uso de memória < 200MB (média)

#### Segurança
- ✅ Tokens armazenados com segurança
- ✅ Validação de inputs
- ✅ Sanitização de dados
- ✅ Tratamento seguro de erros

#### Documentação
- ✅ README por feature
- ✅ Código documentado
- ✅ Changelog atualizado
- ✅ Guia de testes atualizado

---

## 🧪 Plano de Testes

### Estrutura de Testes

```
test/
├── unit/
│   ├── features/
│   │   ├── auth/
│   │   ├── feed/
│   │   └── ...
│   └── core/
│       ├── network/
│       ├── storage/
│       └── utils/
├── widget/
│   ├── features/
│   └── shared/
└── integration/
    ├── journeys/
    └── e2e/
```

### Tipos de Testes

1. **Unit Tests**: Lógica de negócio, validadores, formatters
2. **Widget Tests**: Componentes de UI isolados
3. **Integration Tests**: Fluxos completos (login → feed → criar post)
4. **E2E Tests**: Jornadas críticas de usuário

### Cobertura Mínima

- **Unit**: 80%
- **Widget**: 60%
- **Integration**: Fluxos críticos
- **E2E**: Jornadas principais por papel

---

## 🚀 Deploy e Lançamento

### Estratégia de Lançamento

#### Fase Beta (Fase 0-4)
- Testes internos
- Beta fechado (100 usuários)
- Coleta de feedback
- Correções críticas

#### Fase MVP (Fase 5-8)
- Beta aberto (1000 usuários)
- Liberação gradual
- Monitoramento intensivo
- Correções rápidas

#### Fase Produção (Fase 9+)
- Lançamento público
- Feature flags para liberação gradual
- Monitoramento contínuo
- Atualizações incrementais

### Canal de Distribuição

- **Android**: Google Play Store
- **iOS**: Apple App Store
- **Atualizações**: OTA (Over-The-Air) quando possível

### Monitoramento

- **Analytics**: Firebase Analytics
- **Crash Reporting**: Sentry
- **Performance**: Firebase Performance Monitoring
- **Feedback**: In-app feedback + App Store reviews

---

## 📊 Métricas de Sucesso

### Métricas Técnicas

- **Crash Rate**: < 0.1%
- **ANR Rate**: < 0.05%
- **Startup Time**: < 3s
- **API Response Time**: < 500ms (p95)
- **App Size**: < 50MB

### Métricas de Negócio

- **User Retention**: D1 > 40%, D7 > 20%, D30 > 10%
- **Session Duration**: > 5 minutos
- **Feature Adoption**: > 50% dos usuários usam feature principal
- **User Satisfaction**: > 4.0/5.0 (App Store rating)

---

**Status**: 📋 Roadmap Completo  
**Próximos Passos**: Iniciar Frontend Fase 0 (Fundação e Infraestrutura Base)

---

**Documentação relacionada**:
- [Planejamento do Frontend Flutter](./24_FLUTTER_FRONTEND_PLAN.md)
- [API - Lógica de Negócio e Usabilidade](./60_API_LÓGICA_NEGÓCIO.md)
- [Backlog API](../backlog-api/README.md)
