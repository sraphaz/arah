# Modelo de Domínio (MER conceitual)

> **Nota**: Este documento descreve o modelo atual. Para detalhes da refatoração User-Centric, ver [Refatoração User-Centric Membership](./refactoring/REFACTOR_USER_CENTRIC_MEMBERSHIP.md).

## Entidades principais

### Identidade e Autenticação
- **User** (identidade pessoal global, autenticação via AuthProvider, verificação de identidade global via UserIdentityVerificationStatus)
- **SystemPermission** (permissões globais do sistema: Admin, SystemOperator - não territoriais)
- **UserPreferences** (preferências de privacidade e notificações do usuário)

### Território e Vínculos
- **Territory** (território geográfico)
- **TerritoryMembership** (vínculo User ↔ Territory: MembershipRole Visitor/Resident, ResidencyVerification Flags, MembershipStatus)
- **MembershipSettings** (configurações e opt-ins do membro no território, ex: marketplace_opt_in)
- **MembershipCapability** (capacidades operacionais territoriais: Curator, Moderator, EventOrganizer - empilháveis)
- **FeatureFlag** (flags de funcionalidades por território)

### Conteúdo
- **Post** (territoryId, authorId, visibility, status)
- **MapEntity** (territoryId, createdByUserId, name, category, lat/lng, status, visibility)
- **MapEntityRelation** (userId, entityId, createdAt)
- **PostGeoAnchor** (id, postId, lat/lng, type, createdAt)
- **Media** (postId, type, url, metadata) *(pós-MVP)*

### Social
- **FriendRelation** (requester, target, status pending/accepted/blocked) *(pós-MVP)*

### Chat
- **ChatConversation** (territoryId opcional; kind: TerritoryPublic, TerritoryResidents, Group, Direct; status; auditoria mínima de estado)
- **ConversationParticipant** (conversationId, userId, role Owner/Member; mute; lastRead)
- **ChatMessage** (conversationId, senderUserId, contentType, texto/payload; createdAt; soft delete)
- **ChatConversationStats** (lastMessageAt, lastPreview, messageCount) — read model para performance

### Moderação
- **Report** (territoryId, reporterId, targetType post|user, reason, details, status)
- **Sanction** (scope territory|global, target user|post, type, reason, status, startAt, endAt)
- **WorkItem** (fila genérica para revisão humana: verificação, curadoria e moderação; status/outcome; requirements de permissão/capability)

### Admin, Configuração e Evidências
- **SystemConfig** (configurações globais calibráveis: providers, segurança, moderação, validação)
- **DocumentEvidence** (metadados de evidências documentais para verificação; conteúdo em storage externo)

### Notificações
- **OutboxMessage** (type, payloadJson, occurredAt, processedAt, attempts)
- **UserNotification** (userId, title, body, kind, dataJson, createdAt, readAt, sourceOutboxId)

### Marketplace
- **Store** (territoryId, ownerUserId, displayName, status, paymentsEnabled)
- **StoreItem** (territoryId, storeId, type, title, pricingType, status)
- **ItemInquiry** (territoryId, itemId, storeId, fromUserId, message, status)
- **Cart** (userId, territoryId, items)
- **Checkout** (cartId, status, paymentInfo)

## Relacionamentos (alto nível)

### Identidade
- **User 1..1 UserPreferences** → preferências de privacidade e notificações.

### Território e Vínculos
- **User 1..N TerritoryMembership** → vínculo com territórios (Visitor/Resident).
- **Territory 1..N TerritoryMembership** → vínculos de presença.
- **TerritoryMembership 1..1 MembershipSettings** → configurações do membro (opt-ins, preferências).
- **TerritoryMembership 1..N MembershipCapability** → capacidades operacionais (Curator, Moderator).
- **Territory 1..N FeatureFlag** → flags de funcionalidades habilitadas.

### Conteúdo
- **User 1..N Post** → autor das postagens.
- **Territory 1..N Post** → posts no território.
- **Post 0..N PostGeoAnchor** → localização(ões) da postagem.
- **Post 0..N Media** → mídias anexadas (pós-MVP).
- **Post 0..1 MapEntity** → postagem pode referenciar entidade territorial.
- **User 0..N MapEntityRelation** → moradores vinculam-se a entidades.
- **PostGeoAnchor 1..1 Post** → GeoAnchor referencia um post específico.

### Social
- **User N..N FriendRelation** → relações friends (pós-MVP).

### Chat
- **Territory 1..N ChatConversation** → canais e grupos do território (DM tem territoryId nulo).
- **ChatConversation 1..N ConversationParticipant** → participantes (grupos/DM são explícitos; canais podem ter “participantes implícitos” e também estado por usuário).
- **ChatConversation 1..N ChatMessage** → mensagens ordenadas por tempo.
- **ChatConversation 1..1 ChatConversationStats** → resumo para performance (inbox/listagens).

### Moderação
- **User 1..N Report** → reports feitos por usuários.
- **Report 0..1 Sanction** → sanções derivadas.
- **User 1..N Sanction** → sanções aplicadas.
- **Territory 0..N Sanction** → sanções territoriais (territoryId preenchido) ou globais (territoryId nulo).
- **Report 0..1 WorkItem (ModerationCase)** → casos de moderação enfileirados a partir de reports (fallback humano).

### Admin, Configuração e Evidências
- **SystemConfig** é global (não territorial) e auditável (created/updated por usuários com permissão).
- **DocumentEvidence** referencia:
  - **User** (quem submeteu)
  - **Territory (opcional)**: obrigatório para evidências de residência; nulo para identidade
- **WorkItem** pode referenciar:
  - **USER** (ex.: IdentityVerification)
  - **MEMBERSHIP** (ex.: ResidencyVerification)
  - **REPORT** (ex.: ModerationCase)
  - **ASSET** (ex.: AssetCuration)

### Notificações
- **OutboxMessage 1..N UserNotification** → mensagens geram notificações para destinatários.
- **User 1..N UserNotification** → inbox de notificações do usuário.

### Marketplace
- **Territory 1..N Store** → lojas no território.
- **User 1..N Store** → lojas do usuário (owner).
- **Store 1..N StoreItem** → itens da loja.
- **StoreItem 0..N ItemInquiry** → consultas sobre o item.
- **User 0..N Cart** → carrinho de compras do usuário.
- **Cart 0..1 Checkout** → checkout do carrinho.

## Princípios do Modelo

### User-Centric
- **User** é a pessoa única e global, focada em identidade e autenticação:
  - DisplayName, Email, CPF/ForeignDocument, PhoneNumber, Address
  - AuthProvider (ex: "google", "apple") + ExternalId (chave única)
  - TwoFactor (2FA) settings
  - UserIdentityVerificationStatus (verificação global de identidade: Unverified, Pending, Verified, Rejected)
- **SystemPermission** representa permissões globais do sistema (Admin, SystemOperator):
  - Não são territoriais, são globais
  - Concedidas/revogadas com auditoria (GrantedByUserId, RevokedByUserId)
- Verificação de identidade (`UserIdentityVerificationStatus`) pertence ao User, não ao Membership.

### Membership como Vínculo Territorial
- **TerritoryMembership** representa apenas o vínculo User ↔ Territory:
  - MembershipRole: Visitor ou Resident
  - ResidencyVerification (Flags): None, GeoVerified, DocumentVerified (permite acumulação)
  - MembershipStatus: Pending, Active, Suspended, Revoked
- Regra: 1 Resident por User (máximo) em todo o sistema.
- UserTerritory foi removido (legado, substituído por TerritoryMembership).

### Settings como Escolhas
- **MembershipSettings** concentra configurações e opt-ins do membro.
- Criado automaticamente com o Membership.
- MarketplaceOptIn é exemplo de setting.

### Capacidades como Poderes Operacionais
- **MembershipCapability** representa capacidades territoriais (Curator, Moderator, EventOrganizer, etc.).
- Não são papéis sociais, são poderes operacionais.
- Empilháveis (um Membership pode ter múltiplas).
- Atuam apenas no território do Membership.
- Podem ser concedidas/revogadas com auditoria (GrantedByUserId, GrantedByMembershipId, Reason).

### Hierarquia de Permissões
- **SystemAdmin** tem implicitamente todas as `MembershipCapabilities` em todos os territórios.
- Não requer configuração explícita de capabilities para SystemAdmin.
- Funciona mesmo sem membership no território (acesso global).
- Verificação de SystemAdmin é cacheada (15min) para performance.

### Marketplace como Regra Composta
- Marketplace não é papel, identidade ou verificação.
- Depende de:
  - Territory.FeatureFlags.MarketplaceEnabled
  - MembershipSettings.MarketplaceOptIn
  - Membership.Role == Resident
  - Membership.ResidencyVerificationStatus != Unverified
  - User.IdentityVerificationStatus == Verified (para operações plenas)

## Observações de MVP vs Pós-MVP
- [MVP] TerritoryMembership, MembershipSettings, MembershipCapability, FeatureFlag, Post, PostGeoAnchor, MapEntity, MapEntityRelation, Report, Sanction, OutboxMessage, UserNotification, UserPreferences, Store, StoreItem.
- [MVP] ChatConversation, ConversationParticipant, ChatMessage, ChatConversationStats (canais do território + grupos com aprovação).
- [MVP] SystemConfig, WorkItem (Work Queue), DocumentEvidence (metadados) e storage (Local/S3 via proxy).
- [POST-MVP] Media e FriendRelation.
- [POST-MVP] FriendRelation e comportamentos de círculo interno.
