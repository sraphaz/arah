# Análise: Diagramas de Sequência Faltantes

**Data**: 2025-01-17  
**Autor**: Análise de Cenários Complexos

---

## 📊 Diagramas Existentes (6)

1. ✅ **auth** - Autenticação social → JWT
2. ✅ **territory-discovery** - Descoberta de territórios próximos
3. ✅ **feed-listing** - Listagem de feed territorial
4. ✅ **post-creation** - Criação de post com mídias e GeoAnchors
5. ✅ **membership-resident** - Visitor → Resident (solicitação e aprovação)
6. ✅ **moderation** - Reports, triagem e bloqueios

---

## 🎯 Cenários Complexos que FALTAM Diagramas

### 1. **Marketplace - Fluxo Completo (CRÍTICO)** ⭐⭐⭐

**Complexidade**: MUITO ALTA  
**Etapas**: 6-7 interações  
**Participantes**: Cliente, CartController, CartService, ItemRepository, CheckoutRepository, CheckoutService, PaymentGateway, PayoutService

**Fluxo**:
1. Criar loja (`POST /api/v1/stores`)
2. Criar item com imagens (`POST /api/v1/items` + `mediaIds`)
3. Adicionar ao carrinho (`POST /api/v1/cart/items`)
4. Checkout (`POST /api/v1/cart/checkout`)
   - Agrupa por loja
   - Calcula fees da plataforma
   - Cria Checkout e CheckoutItems
   - Converte não-compravíveis em Inquiries
5. Pagamento (externo)
6. Marcar checkout como Paid (cria SellerTransaction)
7. Payout automático (background worker)

**Por que é importante**:
- Fluxo multi-etapa complexo
- Múltiplas validações e regras de negócio
- Processamento assíncrono (payout)
- Integrações externas (payment gateway)

---

### 2. **Eventos - Criação e Confirmação** ⭐⭐

**Complexidade**: MÉDIA-ALTA  
**Etapas**: 4-5 interações  
**Participantes**: Cliente, EventsController, EventsService, FeedRepository, EventRepository, PostCreationService

**Fluxo**:
1. Criar evento (`POST /api/v1/events`)
   - Valida geo obrigatória
   - Determina membership (VISITOR vs RESIDENT)
2. Criação automática de post no feed
   - Post referenciando o evento
   - GeoAnchor derivado do evento
3. Marcar interesse (`POST /api/v1/events/{id}/interest`)
4. Confirmar presença (`POST /api/v1/events/{id}/confirm`)
5. Aparece no feed E no mapa (pins)

**Por que é importante**:
- Criação automática de post
- Sincronização feed + mapa
- Múltiplos estados (SCHEDULED, INTEREST, CONFIRMED)

---

### 3. **Verificação de Residência (Geo/Document)** ⭐⭐

**Complexidade**: MÉDIA  
**Etapas**: 3-4 interações  
**Participantes**: Cliente, MembershipsController, MembershipService, TerritoryRepository, EvidenceRepository (document)

**Fluxo Geo**:
1. `POST /api/v1/memberships/{territoryId}/verify-residency/geo`
   - Valida coordenadas dentro do raio do território
   - Calcula distância do centro
   - Atualiza `LastGeoVerifiedAtUtc`
2. Atualiza `ResidencyVerification` para `GEO_VERIFIED`
3. Auditoria e cache invalidation

**Fluxo Document**:
1. `POST /api/v1/evidences` (upload)
2. `POST /api/v1/memberships/{territoryId}/verify-residency/document`
   - Associa evidência ao membership
   - Atualiza `LastDocumentVerifiedAtUtc`
3. Validação por curador (work queue)
4. Atualiza `ResidencyVerification` para `DOCUMENT_VERIFIED`

**Por que é importante**:
- Dois caminhos diferentes (geo vs document)
- Validações geográficas (raio)
- Upload e processamento de evidências

---

### 4. **Chat - Envio de Mensagem com Mídia** ⭐

**Complexidade**: BAIXA-MÉDIA  
**Etapas**: 3 interações  
**Participantes**: Cliente, MediaController, ChatController, ChatService, MediaRepository

**Fluxo**:
1. Upload de mídia (`POST /api/v1/media/upload`)
   - Valida tipo e tamanho (5MB, apenas imagens)
   - Retorna `mediaId`
2. Enviar mensagem com `mediaId` (`POST /api/v1/chat/conversations/{id}/messages`)
   - Valida que mídia pertence ao usuário
   - Cria mensagem com `mediaId`
3. Resposta inclui `mediaUrl` e `hasMedia`

**Por que é importante**:
- Fluxo de upload + associação
- Validações de propriedade

---

### 5. **Sistema de Notificações (Outbox Pattern)** ⭐⭐⭐

**Complexidade**: ALTA  
**Etapas**: 4-5 interações assíncronas  
**Participantes**: EventPublisher, OutboxRepository, BackgroundWorker, NotificationService, NotificationRepository

**Fluxo**:
1. Evento de domínio ocorre (ex: `PostCreatedEvent`)
2. `EventPublisher` salva mensagem em `Outbox`
3. Background worker processa `Outbox` periodicamente
4. Para cada mensagem:
   - Cria `Notification` para usuários relevantes
   - Marca como processada
5. Usuário consulta (`GET /api/v1/notifications`)
6. Marca como lida (`POST /api/v1/notifications/{id}/read`)

**Por que é importante**:
- Padrão outbox (garantia de entrega)
- Processamento assíncrono
- Escalabilidade

---

### 6. **Assets - Criação e Validação por Curador** ⭐⭐

**Complexidade**: MÉDIA-ALTA  
**Etapas**: 4-5 interações  
**Participantes**: Cliente, AssetsController, TerritoryAssetService, AssetRepository, WorkQueue (Curator), ValidationService

**Fluxo**:
1. Criar asset (`POST /api/v1/assets`)
   - Valida geo obrigatória (pelo menos 1 GeoAnchor)
   - Status inicial: `SUGGESTED`
2. Criação de `WorkItem` para fila de curadoria
3. Curador lista work queue (`GET /api/v1/admin/work-queue`)
4. Validação (`POST /api/v1/map/entities/{id}/validation`)
   - Status: `SUGGESTED` → `VALIDATED`
5. Asset aparece no mapa (pins)

**Por que é importante**:
- Work queue pattern
- Validação por curadores
- Mudança de status (SUGGESTED → VALIDATED)

---

### 7. **Mapa - Entidades (Sugestão → Confirmação → Validação)** ⭐

**Complexidade**: MÉDIA  
**Etapas**: 3-4 interações  
**Participantes**: Cliente, MapController, MapEntityService, MapEntityRepository, WorkQueue

**Fluxo**:
1. Sugerir entidade (`POST /api/v1/map/entities`)
   - Categorias: estabelecimento, órgão do governo, espaço público, espaço natural
   - Status: `SUGGESTED`
2. Confirmação por moradores (`POST /api/v1/map/entities/{id}/confirmations`)
   - Múltiplos moradores podem confirmar
3. Validação por curador (`POST /api/v1/map/entities/{id}/validation`)
   - Status: `SUGGESTED` → `VALIDATED`
4. Aparece no mapa e pode filtrar feed

**Por que é importante**:
- Fluxo colaborativo (sugestão → confirmação → validação)
- Work queue para curadores

---

## 📋 Priorização

### 🔴 **Alta Prioridade** (Recomendado criar agora)
1. **Marketplace - Fluxo Completo** ⭐⭐⭐
   - MUITO complexo, múltiplas integrações
   - Central para economia local
   
2. **Sistema de Notificações (Outbox)** ⭐⭐⭐
   - Padrão arquitetural importante
   - Fluxo assíncrono crítico

### 🟡 **Média Prioridade** (Considerar depois)
3. **Eventos - Criação e Confirmação** ⭐⭐
4. **Assets - Criação e Validação** ⭐⭐
5. **Verificação de Residência** ⭐⭐

### 🟢 **Baixa Prioridade** (Se houver tempo)
6. **Chat com Mídia** ⭐
7. **Mapa - Entidades** ⭐

---

## 💡 Recomendação

**Criar imediatamente**: **Marketplace** e **Notificações (Outbox)**

Esses dois cenários são:
- Os mais complexos tecnicamente
- Fundamentais para o funcionamento do sistema
- Melhor documentados ajudam novos desenvolvedores

Os outros podem ser adicionados progressivamente conforme necessidade.
