# Fase 48: Banco de Sementes e Mudas Territorial

**Duração**: 3 semanas (21 dias úteis)  
**Prioridade**: 🟢 MÉDIA (Recursos compartilhados)  
**Depende de**: Fase 6 (Marketplace), Fase 14 (Governança)  
**Estimativa Total**: 120 horas  
**Status**: ⏳ Pendente  
**Nota**: Renumerada de Fase 29 para Fase 48, reposicionada para Onda 10 (Diferenciação Avançada). Fase 30 agora é Mobile Avançado.

---

## 🎯 Objetivo

Implementar suporte completo para funcionalidades mobile avançadas que melhoram significativamente a experiência do usuário no app Flutter, incluindo:
- Analytics mobile (app version, platform, device info)
- Deep linking avançado (universal links, app links)
- Background tasks otimizados (endpoints leves)
- Push notifications refinados (badges, ações customizadas)

**Princípios**:
- ✅ **Performance**: Endpoints otimizados para background fetch
- ✅ **Experiência**: Deep linking nativo e fluido
- ✅ **Observabilidade**: Rastreamento completo de uso mobile
- ✅ **Notificações**: Push notifications refinados e contextualizados

---

## 📋 Contexto e Requisitos

### Estado Atual
- ✅ Push notifications básicos implementados (Fase 9)
- ✅ Deep linking básico funcionando
- ✅ Analytics básicos (Firebase Analytics)
- ❌ Analytics mobile específicos não existem
- ❌ Deep linking avançado não implementado
- ❌ Background tasks não otimizados
- ❌ Push notifications não refinados

### Requisitos Funcionais

#### 1. Analytics Mobile
- ✅ Rastreamento de app version
- ✅ Detecção de platform (iOS/Android)
- ✅ Device info (modelo, OS version)
- ✅ Session tracking mobile
- ✅ Screen view tracking mobile

#### 2. Deep Linking Avançado
- ✅ Universal Links (iOS) com validação
- ✅ App Links (Android) com validação
- ✅ Dynamic links pelo backend (opcional)
- ✅ Fallback para web quando app não instalado
- ✅ Metadados customizados nos links

#### 3. Background Tasks Otimizados
- ✅ Endpoints leves para background fetch
- ✅ Sumários de feed/notificações
- ✅ Status de sincronização
- ✅ Cache headers otimizados
- ✅ Compressão de respostas

#### 4. Push Notifications Refinados
- ✅ Badges atualizados (contagem de notificações)
- ✅ Ações customizadas em notificações
- ✅ Agrupamento de notificações
- ✅ Categorização de notificações
- ✅ Priorização de notificações

---

## 📋 Tarefas Detalhadas

### Semana 1: Analytics e Deep Linking

#### 29.1 Analytics Mobile Avançado
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar endpoint `POST /api/v1/mobile/analytics/event`:
  - [ ] Aceitar eventos mobile customizados
  - [ ] Incluir: `appVersion`, `platform`, `deviceInfo`, `sessionId`
  - [ ] Integrar com sistema de analytics existente
- [ ] Criar endpoint `GET /api/v1/mobile/analytics/session`:
  - [ ] Retornar informações de sessão mobile
  - [ ] Incluir: `sessionId`, `startTime`, `screenViews`
- [ ] Atualizar `AuthController` para incluir device info no login:
  - [ ] Capturar `User-Agent` header
  - [ ] Extrair app version, platform
  - [ ] Armazenar em `USER_DEVICE`
- [ ] Criar modelo `MobileAnalyticsEvent`:
  - [ ] `EventType` (ScreenView, UserAction, Error, Performance)
  - [ ] `AppVersion`, `Platform`, `DeviceInfo`
  - [ ] `Timestamp`, `SessionId`, `UserId`
- [ ] Testes de integração

**Arquivos a Criar**:
- `backend/Araponga.Api/Controllers/MobileAnalyticsController.cs`
- `backend/Araponga.Api/Contracts/Mobile/MobileAnalyticsEventRequest.cs`
- `backend/Araponga.Application/Models/MobileAnalyticsEvent.cs`
- `backend/Araponga.Application/Services/MobileAnalyticsService.cs`

**Critérios de Sucesso**:
- ✅ Analytics mobile funcionando
- ✅ Device info rastreado
- ✅ Sessões mobile registradas
- ✅ Testes passando

---

#### 29.2 Deep Linking Avançado
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar endpoint `POST /api/v1/links/generate`:
  - [ ] Aceitar: `type` (post, event, territory), `targetId`, `metadata`
  - [ ] Gerar short link e long link
  - [ ] Armazenar metadados do link
  - [ ] Retornar QR code (opcional)
- [ ] Criar endpoint `GET /api/v1/links/{shortCode}`:
  - [ ] Validar link
  - [ ] Retornar metadados e redirecionamento
  - [ ] Rastrear acesso (analytics)
- [ ] Criar validação de Universal Links:
  - [ ] Endpoint para `.well-known/apple-app-site-association`
  - [ ] Retornar JSON com app IDs e paths
- [ ] Criar validação de App Links:
  - [ ] Endpoint para `.well-known/assetlinks.json`
  - [ ] Retornar JSON com package names e fingerprints
- [ ] Integrar com Firebase Dynamic Links (opcional):
  - [ ] Encurtar links via Firebase
  - [ ] Fallback para web quando app não instalado
- [ ] Criar modelo `DynamicLink`:
  - [ ] `ShortCode`, `LongLink`, `TargetType`, `TargetId`
  - [ ] `Metadata`, `CreatedAt`, `ExpiresAt`, `AccessCount`
- [ ] Testes de integração

**Arquivos a Criar**:
- `backend/Araponga.Api/Controllers/LinksController.cs`
- `backend/Araponga.Api/Contracts/Links/GenerateLinkRequest.cs`
- `backend/Araponga.Api/Contracts/Links/LinkResponse.cs`
- `backend/Araponga.Application/Models/DynamicLink.cs`
- `backend/Araponga.Application/Services/LinkGenerationService.cs`
- `backend/Araponga.Api/Controllers/WellKnownController.cs` (para .well-known)

**Critérios de Sucesso**:
- ✅ Deep linking funcionando
- ✅ Universal Links validados (iOS)
- ✅ App Links validados (Android)
- ✅ Links rastreados (analytics)
- ✅ Testes passando

---

### Semana 2: Background Tasks e Push Refinados

#### 29.3 Background Tasks Otimizados
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar endpoint `GET /api/v1/feed/summary?territoryId={id}`:
  - [ ] Retornar apenas: `lastUpdated`, `unreadCount`, `newPostsCount`
  - [ ] Não incluir posts completos (economia de banda)
  - [ ] Cache headers otimizados (5 minutos)
  - [ ] Compressão de resposta (gzip)
- [ ] Criar endpoint `GET /api/v1/notifications/summary`:
  - [ ] Retornar apenas: `unreadCount`, `lastNotificationAt`
  - [ ] Não incluir notificações completas
  - [ ] Cache headers otimizados (2 minutos)
- [ ] Criar endpoint `GET /api/v1/sync/status`:
  - [ ] Retornar: `lastSyncAt`, `pendingActionsCount`, `conflictsCount`
  - [ ] Status de sincronização do usuário
  - [ ] Indicar se precisa sincronizar
- [ ] Otimizar headers de cache em endpoints existentes:
  - [ ] Feed: `Cache-Control: public, max-age=300` (5 minutos)
  - [ ] Notificações: `Cache-Control: public, max-age=120` (2 minutos)
  - [ ] Territórios: `Cache-Control: public, max-age=1800` (30 minutos)
- [ ] Adicionar compressão gzip automática:
  - [ ] Configurar middleware de compressão
  - [ ] Comprimir respostas > 1KB
- [ ] Testes de performance

**Arquivos a Modificar**:
- `backend/Araponga.Api/Controllers/FeedController.cs`
- `backend/Araponga.Api/Controllers/NotificationsController.cs`
- `backend/Araponga.Api/Controllers/SyncController.cs` (novo)
- `backend/Araponga.Api/Program.cs` (compressão)

**Arquivos a Criar**:
- `backend/Araponga.Api/Contracts/Feed/FeedSummaryResponse.cs`
- `backend/Araponga.Api/Contracts/Notifications/NotificationSummaryResponse.cs`
- `backend/Araponga.Api/Contracts/Sync/SyncStatusResponse.cs`

**Critérios de Sucesso**:
- ✅ Endpoints leves funcionando
- ✅ Tempo de resposta < 100ms
- ✅ Tamanho de resposta reduzido (> 50%)
- ✅ Cache headers funcionando
- ✅ Testes de performance passando

---

#### 29.4 Push Notifications Refinados
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar endpoint `GET /api/v1/users/devices/{deviceId}/badge`:
  - [ ] Retornar contagem de notificações não lidas
  - [ ] Atualizar badge do dispositivo
- [ ] Criar endpoint `POST /api/v1/notifications/{id}/actions`:
  - [ ] Aceitar ações customizadas em notificações
  - [ ] Processar ação (ex: "Ver Post", "Participar Evento")
  - [ ] Retornar resultado da ação
- [ ] Implementar agrupamento de notificações:
  - [ ] Agrupar notificações do mesmo tipo (ex: múltiplos comentários)
  - [ ] Criar notificação resumo quando > 3 notificações do mesmo tipo
  - [ ] Endpoint `GET /api/v1/notifications/grouped`
- [ ] Implementar categorização de notificações:
  - [ ] Adicionar campo `Category` ao modelo de notificação
  - [ ] Categorias: `POST`, `EVENT`, `MEMBERSHIP`, `MARKETPLACE`, `MODERATION`
  - [ ] Permitir filtro por categoria
- [ ] Implementar priorização de notificações:
  - [ ] Adicionar campo `Priority` ao modelo
  - [ ] Prioridades: `LOW`, `NORMAL`, `HIGH`, `URGENT`
  - [ ] Notificações urgentes sempre enviadas (ignorar preferências temporariamente)
- [ ] Atualizar `NotificationDispatchService`:
  - [ ] Incluir ações customizadas no payload
  - [ ] Incluir categoria e prioridade
  - [ ] Suportar agrupamento
- [ ] Testes de integração

**Arquivos a Modificar**:
- `backend/Araponga.Domain/Notifications/Notification.cs` (adicionar Category, Priority)
- `backend/Araponga.Application/Services/NotificationDispatchService.cs`
- `backend/Araponga.Api/Controllers/NotificationsController.cs`
- `backend/Araponga.Api/Controllers/DevicesController.cs` (novo)

**Arquivos a Criar**:
- `backend/Araponga.Api/Contracts/Notifications/BadgeResponse.cs`
- `backend/Araponga.Api/Contracts/Notifications/NotificationActionRequest.cs`
- `backend/Araponga.Api/Contracts/Notifications/GroupedNotificationsResponse.cs`

**Critérios de Sucesso**:
- ✅ Badges atualizados
- ✅ Ações customizadas funcionando
- ✅ Agrupamento de notificações funcionando
- ✅ Categorização funcionando
- ✅ Priorização funcionando
- ✅ Testes passando

---

## ✅ Critérios de Sucesso da Fase

### Funcionalidades
- ✅ Analytics mobile funcionando completamente
- ✅ Deep linking avançado funcionando (iOS e Android)
- ✅ Background tasks otimizados (< 100ms)
- ✅ Push notifications refinados (badges, ações, agrupamento)

### Performance
- ✅ Endpoints leves < 100ms de resposta
- ✅ Tamanho de resposta reduzido > 50%
- ✅ Cache headers funcionando corretamente
- ✅ Compressão gzip funcionando

### Qualidade
- ✅ Testes unitários: > 80% cobertura
- ✅ Testes de integração: todos passando
- ✅ Testes de performance: dentro dos limites
- ✅ Documentação atualizada
- Considerar **Testcontainers + PostgreSQL** para testes de integração (analytics mobile, push, deep linking) com banco real (estratégia na Fase 19; [TESTCONTAINERS_POSTGRES_IMPACTO](../../TESTCONTAINERS_POSTGRES_IMPACTO.md)).

---

## 🔗 Integrações e Dependências

### Dependências Externas
- **Firebase Dynamic Links** (opcional): Para encurtar links
- **Apple App Store Connect**: Para configurar Universal Links
- **Google Play Console**: Para configurar App Links

### Integrações Internas
- **Fase 9**: Push tokens (devices), segurança
- **Fase 10**: Sincronização offline (sync status)
- **Sistema de Notificações**: Push notifications refinados
- **Sistema de Analytics**: Analytics mobile

---

## 📊 Métricas de Sucesso

### Performance
- **Tempo de resposta de endpoints leves**: < 100ms ✅
- **Redução de tamanho de resposta**: > 50% ✅
- **Taxa de cache hit**: > 70% ✅

### Funcionalidade
- **Taxa de sucesso de deep linking**: > 95% ✅
- **Taxa de entrega de push notifications**: > 98% ✅
- **Precisão de badges**: 100% ✅

### Experiência do Usuário
- **Tempo de atualização em background**: < 30s ✅
- **Taxa de cliques em ações customizadas**: > 20% ✅
- **Satisfação com notificações**: > 4.5/5 ✅

---

## 🚀 Próximos Passos (Pós-Fase 29)

1. **Analytics Avançado**:
   - Funnels de conversão
   - Segmentação de usuários
   - A/B testing

2. **Deep Linking Avançado**:
   - Links temporários com expiração
   - Links com autenticação
   - Links com metadados complexos

3. **Background Tasks**:
   - Pre-fetch inteligente
   - Sync incremental
   - Priorização de dados

4. **Push Notifications**:
   - Notificações ricas (imagens, vídeos)
   - Notificações interativas
   - Notificações agendadas

---

**Versão**: 1.0  
**Criado em**: 2025-01-20  
**Status**: ⏳ Pendente  
**Ver**: [Priorização Estratégica](../35_PRIORIZACAO_ESTRATEGICA_API_FRONTEND.md) para ordem otimizada
