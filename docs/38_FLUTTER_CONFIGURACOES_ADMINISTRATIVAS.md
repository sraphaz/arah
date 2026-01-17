# Configurações Administrativas no App Flutter - Araponga

**Versão**: 1.0  
**Data**: 2025-01-17  
**Status**: 📋 Planejamento Completo  
**Tipo**: Documentação de Funcionalidades Administrativas

---

## 📋 Índice

1. [Visão Geral](#visão-geral)
2. [Configurações por Papel](#configurações-por-papel)
3. [Funcionalidades por Fase](#funcionalidades-por-fase)
4. [Telas e Componentes](#telas-e-componentes)
5. [Integração com Backend](#integração-com-backend)
6. [Critérios de Implementação](#critérios-de-implementação)

---

## 🎯 Visão Geral

Este documento descreve as funcionalidades de **configurações administrativas** que serão implementadas no app Flutter, sincronizadas com as novas configurações flexíveis do backend identificadas na avaliação de flexibilização (`FASE10_CONFIG_FLEXIBILIZACAO_AVALIACAO.md`).

### Princípios

- ✅ **Configuração via Painel**: Todas as configurações via interface do app (não apenas `appsettings.json`)
- ✅ **Hierarquia de Configuração**: Global (SystemAdmin) → Territorial (Curator) → Usuário (preferências)
- ✅ **Flexibilidade**: Configurações ajustáveis sem deploy
- ✅ **Transparência**: Usuários veem configurações aplicadas
- ✅ **Segurança**: Apenas papéis autorizados podem configurar

---

## 👥 Configurações por Papel

### SYSTEM_ADMIN (Administrador do Sistema)

**Permissões**: Acesso a todas as configurações globais do sistema

**Configurações Disponíveis**:
1. **Media Storage Config** (Backend Fase 10 - concluída)
   - Provedor de blob storage (Local, S3, AzureBlob)
   - Configurações específicas por provedor
   - Ativação/desativação de configurações

2. **Rate Limiting Config** (Backend Fase 15 - item 15.X)
   - Limites globais por tipo de endpoint
   - Ajustes em tempo real

3. **JWT Config** (Backend Fase 15 - item 15.Y)
   - Expiração de tokens
   - Configuração de Issuer/Audience

4. **Observability Config** (Backend Fase 25 - item 25.X)
   - Providers de logging, métricas, tracing
   - Níveis de log configuráveis

5. **Data Retention Config** (Backend Fase 16 - Compliance)
   - Políticas de retenção por tipo de entidade
   - Configurações de arquivamento e anonimização

### CURATOR (Curador Territorial)

**Permissões**: Configurações específicas do território

**Configurações Disponíveis**:
1. **Territory Media Config** (Backend Fase 10 - item 10.9)
   - Limites de tamanho e tipos MIME por tipo de conteúdo
   - Override de limites globais

2. **Moderation Threshold Config** (Backend Fase 11 - item 11.X)
   - Thresholds de reports
   - Janela de tempo
   - Ações automáticas

3. **Platform Fee Limits Config** (Backend Fase 12 - item 12.X)
   - Limites de taxas e payouts
   - Métodos de cálculo de taxas

4. **Presence Policy Config** (Backend Fase 13 - item 13.X)
   - Políticas de presença (ResidentOnly, VerifiedOnly, Public)

5. **Notification Config** (Backend Fase 14 - item 14.X)
   - Tipos de notificação disponíveis
   - Canais e templates

6. **Map Config** (Backend Fase 4 - item 4.X)
   - Raio de busca
   - Provedor de mapas

7. **Feature Flags** (já existente)
   - Ativação/desativação de funcionalidades territoriais

### RESIDENT (Morador)

**Permissões**: Preferências pessoais de visualização

**Configurações Disponíveis**:
1. **User Media Preferences** (Backend Fase 10 - já implementado)
   - Tipos de mídia a visualizar
   - Filtros de conteúdo

2. **User Notification Preferences** (já existente)
   - Tipos de notificação
   - Canais preferidos

---

## 📅 Funcionalidades por Fase

### Frontend Fase 3: Mídias e Sincronização (Expandida)

**Sincronizada com**: Backend Fase 10

**Novo Item**: 3.6 Configuração Avançada de Mídias (Curador)

**Funcionalidades**:
- [ ] Tela de configuração de mídias por território
- [ ] Configuração de limites de tamanho (imagens, vídeos, áudios)
- [ ] Configuração de tipos MIME permitidos
- [ ] Visualização de limites globais vs territoriais
- [ ] Interface para override de limites globais

**Endpoints**:
- `GET /api/v1/territories/{territoryId}/media-config`
- `PUT /api/v1/territories/{territoryId}/media-config`

**Arquivos a Criar**:
- `lib/features/admin/media_config/presentation/screens/territory_media_config_screen.dart`
- `lib/features/admin/media_config/presentation/widgets/media_limits_form.dart`
- `lib/features/admin/media_config/data/models/territory_media_config_model.dart`
- `lib/features/admin/media_config/data/repositories/media_config_repository.dart`

---

### Frontend Fase 4: Funcionalidades Core (Expandida)

**Sincronizada com**: Backend Fase 11

**Novo Item**: 4.2 Configuração de Thresholds de Moderação (Curador)

**Funcionalidades**:
- [ ] Tela de configuração de thresholds de moderação
- [ ] Configuração de janela de tempo (dias)
- [ ] Configuração de número mínimo de reports
- [ ] Configuração de ações automáticas
- [ ] Visualização de políticas ativas

**Endpoints**:
- `GET /api/v1/territories/{territoryId}/moderation-threshold-config`
- `PUT /api/v1/territories/{territoryId}/moderation-threshold-config`

**Arquivos a Criar**:
- `lib/features/admin/moderation_config/presentation/screens/moderation_threshold_config_screen.dart`
- `lib/features/admin/moderation_config/presentation/widgets/threshold_form.dart`
- `lib/features/admin/moderation_config/data/models/moderation_threshold_config_model.dart`

---

### Frontend Fase 6: Marketplace (Expandida)

**Sincronizada com**: Backend Fase 12

**Novo Item**: 6.4 Configuração Avançada de Taxas (Curador)

**Funcionalidades**:
- [ ] Tela de configuração de taxas e limites
- [ ] Configuração de limites mínimo/máximo de taxas
- [ ] Configuração de métodos de cálculo
- [ ] Integração visual com configuração de payouts
- [ ] Alertas para configurações inconsistentes

**Endpoints**:
- `GET /api/v1/territories/{territoryId}/fee-limits-config`
- `PUT /api/v1/territories/{territoryId}/fee-limits-config`

**Arquivos a Criar**:
- `lib/features/admin/fees_config/presentation/screens/fee_limits_config_screen.dart`
- `lib/features/admin/fees_config/presentation/widgets/fee_limits_form.dart`
- `lib/features/admin/fees_config/data/models/fee_limits_config_model.dart`

---

### Frontend Fase 5: Territórios (Expandida)

**Sincronizada com**: Backend Fase 13

**Novo Item**: 5.3 Configuração de Políticas de Presença (Curador)

**Funcionalidades**:
- [ ] Tela de configuração de políticas de presença
- [ ] Seleção de política (ResidentOnly, VerifiedOnly, Public, Custom)
- [ ] Configuração de regras customizadas (quando Policy = Custom)
- [ ] Preview de impacto da política
- [ ] Histórico de mudanças de política

**Endpoints**:
- `GET /api/v1/territories/{territoryId}/presence-policy-config`
- `PUT /api/v1/territories/{territoryId}/presence-policy-config`

**Arquivos a Criar**:
- `lib/features/admin/presence_config/presentation/screens/presence_policy_config_screen.dart`
- `lib/features/admin/presence_config/presentation/widgets/policy_selector.dart`
- `lib/features/admin/presence_config/data/models/presence_policy_config_model.dart`

---

### Frontend Fase 5: Territórios (Expandida - Complementar)

**Sincronizada com**: Backend Fase 14

**Novo Item**: 5.4 Configuração Avançada de Notificações (Curador)

**Funcionalidades**:
- [ ] Tela de configuração de notificações
- [ ] Seleção de tipos de notificação disponíveis
- [ ] Configuração de canais por tipo (Email, Push, InApp, SMS)
- [ ] Editor de templates (opcional)
- [ ] Visualização de canais padrão

**Endpoints**:
- `GET /api/v1/territories/{territoryId}/notification-config`
- `PUT /api/v1/territories/{territoryId}/notification-config`

**Arquivos a Criar**:
- `lib/features/admin/notifications_config/presentation/screens/notification_config_screen.dart`
- `lib/features/admin/notifications_config/presentation/widgets/notification_type_selector.dart`
- `lib/features/admin/notifications_config/data/models/notification_config_model.dart`

---

### Frontend Fase 5: Territórios (Expandida - Complementar)

**Sincronizada com**: Backend Fase 4 (Observabilidade)

**Novo Item**: 5.5 Configuração de Mapas (Curador)

**Funcionalidades**:
- [ ] Tela de configuração de mapas e geo-localização
- [ ] Configuração de raio de busca (metros)
- [ ] Configuração de distância máxima para "territórios próximos"
- [ ] Seleção de provedor de mapas (Google, Mapbox, OpenStreetMap)
- [ ] Configurações específicas do provedor

**Endpoints**:
- `GET /api/v1/territories/{territoryId}/map-config`
- `PUT /api/v1/territories/{territoryId}/map-config`

**Arquivos a Criar**:
- `lib/features/admin/map_config/presentation/screens/map_config_screen.dart`
- `lib/features/admin/map_config/presentation/widgets/map_provider_selector.dart`
- `lib/features/admin/map_config/data/models/map_config_model.dart`

---

### Frontend Fase 12: Serviços Digitais (Expandida - Complementar)

**Sincronizada com**: Backend Fase 15

**Novo Item**: 12.3 Configuração de Rate Limiting (SystemAdmin)

**Funcionalidades**:
- [ ] Tela de configuração global de rate limiting
- [ ] Configuração por tipo de endpoint
- [ ] Ajustes em tempo real (sem restart)
- [ ] Visualização de limites atuais
- [ ] Alertas para limites muito baixos/altos

**Endpoints**:
- `GET /api/v1/admin/rate-limit-config`
- `PUT /api/v1/admin/rate-limit-config/{configId}`
- `POST /api/v1/admin/rate-limit-config/{configId}/activate`

**Arquivos a Criar**:
- `lib/features/admin/rate_limit_config/presentation/screens/rate_limit_config_screen.dart`
- `lib/features/admin/rate_limit_config/presentation/widgets/endpoint_type_selector.dart`
- `lib/features/admin/rate_limit_config/data/models/rate_limit_config_model.dart`

---

**Novo Item**: 12.4 Configuração de Autenticação JWT (SystemAdmin)

**Funcionalidades**:
- [ ] Tela de configuração de JWT
- [ ] Configuração de expiração de tokens
- [ ] Configuração de Issuer/Audience
- [ ] Suporte a múltiplas configurações (ativação seletiva)
- [ ] Alertas para expirações muito curtas/longas

**Endpoints**:
- `GET /api/v1/admin/jwt-config/active`
- `GET /api/v1/admin/jwt-config`
- `POST /api/v1/admin/jwt-config`
- `PUT /api/v1/admin/jwt-config/{configId}`
- `POST /api/v1/admin/jwt-config/{configId}/activate`

**Arquivos a Criar**:
- `lib/features/admin/jwt_config/presentation/screens/jwt_config_screen.dart`
- `lib/features/admin/jwt_config/presentation/widgets/jwt_expiration_form.dart`
- `lib/features/admin/jwt_config/data/models/jwt_config_model.dart`

---

**Novo Item**: 12.5 Configuração de Observabilidade (SystemAdmin)

**Funcionalidades**:
- [ ] Tela de configuração de observabilidade
- [ ] Seleção de providers (Logging, Metrics, Tracing)
- [ ] Configuração de níveis de log
- [ ] Configurações específicas de cada provider
- [ ] Visualização de configuração ativa

**Endpoints**:
- `GET /api/v1/admin/observability-config/active`
- `GET /api/v1/admin/observability-config`
- `POST /api/v1/admin/observability-config`
- `PUT /api/v1/admin/observability-config/{configId}`
- `POST /api/v1/admin/observability-config/{configId}/activate`

**Arquivos a Criar**:
- `lib/features/admin/observability_config/presentation/screens/observability_config_screen.dart`
- `lib/features/admin/observability_config/presentation/widgets/provider_selector.dart`
- `lib/features/admin/observability_config/data/models/observability_config_model.dart`

---

### Frontend Fase 10: Compra Coletiva (Expandida - Complementar)

**Sincronizada com**: Backend Fase 16 (Compliance)

**Novo Item**: 10.4 Configuração de Retenção de Dados (SystemAdmin/Curador)

**Funcionalidades**:
- [ ] Tela de configuração de retenção de dados
- [ ] Configuração por tipo de entidade (Posts, Reports, Media, Logs, etc.)
- [ ] Configuração de período de retenção (dias)
- [ ] Configuração de arquivamento e anonimização
- [ ] Visualização de políticas ativas
- [ ] Estatísticas de dados processados

**Endpoints**:
- `GET /api/v1/admin/retention-config` (SystemAdmin)
- `POST /api/v1/admin/retention-config` (SystemAdmin)
- `PUT /api/v1/admin/retention-config/{configId}` (SystemAdmin)
- `GET /api/v1/territories/{territoryId}/retention-config` (Curador, opcional)
- `PUT /api/v1/territories/{territoryId}/retention-config` (Curador, opcional)

**Arquivos a Criar**:
- `lib/features/admin/data_retention_config/presentation/screens/data_retention_config_screen.dart`
- `lib/features/admin/data_retention_config/presentation/widgets/entity_type_selector.dart`
- `lib/features/admin/data_retention_config/data/models/data_retention_config_model.dart`

---

### Frontend Fase 3: Mídias (Expandida - Complementar)

**Sincronizada com**: Backend Fase 10 (já implementado)

**Novo Item**: 3.7 Configuração de Media Storage (SystemAdmin)

**Funcionalidades**:
- [ ] Tela de configuração de media storage
- [ ] Seleção de provedor (Local, S3, AzureBlob)
- [ ] Configurações específicas por provedor
- [ ] Ativação/desativação de configurações
- [ ] Visualização de configuração ativa
- [ ] Masking de informações sensíveis (secrets)

**Endpoints**:
- `GET /api/v1/admin/media-storage-config/active`
- `GET /api/v1/admin/media-storage-config`
- `POST /api/v1/admin/media-storage-config`
- `PUT /api/v1/admin/media-storage-config/{configId}`
- `POST /api/v1/admin/media-storage-config/{configId}/activate`

**Arquivos a Criar**:
- `lib/features/admin/media_storage_config/presentation/screens/media_storage_config_screen.dart`
- `lib/features/admin/media_storage_config/presentation/widgets/storage_provider_selector.dart`
- `lib/features/admin/media_storage_config/data/models/media_storage_config_model.dart`

**Status**: ⏳ Pendente - Aguarda implementação completa do backend

---

## 🎨 Telas e Componentes

### Estrutura de Navegação

```
Admin/
├── Territory Settings/
│   ├── Media Configuration
│   ├── Moderation Thresholds
│   ├── Fee Limits
│   ├── Presence Policies
│   ├── Notification Settings
│   └── Map Configuration
└── System Settings/ (SystemAdmin only)
    ├── Media Storage Config
    ├── Rate Limiting
    ├── JWT Configuration
    ├── Observability
    └── Data Retention
```

### Componentes Compartilhados

1. **ConfigSection** - Seção de configuração reutilizável
2. **ConfigForm** - Formulário genérico de configuração
3. **ConfigProviderSelector** - Seletor de provedores (mapas, storage, observabilidade)
4. **ConfigLimitsInput** - Input para limites (tamanho, quantidade, tempo)
5. **ConfigPreview** - Preview de impacto de configurações
6. **ConfigHistory** - Histórico de mudanças de configuração

---

## 🔌 Integração com Backend

### Padrão de Integração

Todas as configurações seguem o padrão:

1. **Leitura**: `GET /api/v1/[admin/]territories/{territoryId}/[config-type]-config`
2. **Escrita**: `PUT /api/v1/[admin/]territories/{territoryId}/[config-type]-config`
3. **Validação**: Backend valida limites e consistência
4. **Fallback**: Frontend exibe configuração territorial ou global (quando territorial não existe)

### Tratamento de Erros

- **403 Forbidden**: Usuário sem permissão → Ocultar interface de configuração
- **404 Not Found**: Configuração não existe → Usar valores padrão
- **400 Bad Request**: Validação falhou → Exibir erros de validação
- **422 Unprocessable Entity**: Configuração inconsistente → Exibir alertas

### Cache e Sincronização

- **Cache Local**: Armazenar configurações em cache (TTL: 1 hora)
- **Invalidação**: Invalidar cache quando configuração é atualizada
- **Sincronização**: Background sync para garantir configuração atualizada

---

## ✅ Critérios de Implementação

### Por Feature

Cada tela de configuração deve incluir:

- ✅ **Formulário Validado**: Validação client-side + server-side
- ✅ **Feedback Visual**: Loading states, success/error messages
- ✅ **Preview de Impacto**: Mostrar efeitos da configuração antes de salvar
- ✅ **Histórico**: Visualizar mudanças recentes (quando aplicável)
- ✅ **Documentação Contextual**: Explicação de cada configuração
- ✅ **Acessibilidade**: Suporte completo a leitores de tela

### Segurança

- ✅ **Autorização**: Verificar permissões antes de exibir interface
- ✅ **Masking de Secrets**: Não exibir secrets em texto plano
- ✅ **Auditoria**: Logs de todas as mudanças de configuração
- ✅ **Validação**: Validar limites mínimos/máximos no frontend

### Testes

- ✅ **Unit Tests**: Lógica de validação e formatação
- ✅ **Widget Tests**: Componentes de formulário
- ✅ **Integration Tests**: Fluxo completo de configuração
- ✅ **E2E Tests**: Jornadas de curador e admin

---

## 📚 Referências

- **Backend**: `docs/backlog-api/FASE10_CONFIG_FLEXIBILIZACAO_AVALIACAO.md`
- **API**: `docs/60_API_LÓGICA_NEGÓCIO.md`
- **Roadmap**: `docs/25_FLUTTER_IMPLEMENTATION_ROADMAP.md`
- **Design**: `docs/26_FLUTTER_DESIGN_GUIDELINES.md`

---

**Status**: ⏳ Pendente  
**Última atualização**: 2025-01-17
