# Modelo de Domínio (MER conceitual)

> **Nota**: Este documento descreve o modelo atual. Para detalhes da refatoração User-Centric, ver [REFACTOR_USER_CENTRIC_MEMBERSHIP.md](./REFACTOR_USER_CENTRIC_MEMBERSHIP.md).

## Entidades principais

### Identidade e Autenticação
- **User** (identidade pessoal global, autenticação, verificação de identidade global)
- **UserPreferences** (preferências de privacidade e notificações do usuário)

### Território e Vínculos
- **Territory** (território geográfico)
- **TerritoryMembership** (vínculo User ↔ Territory: Visitor/Resident, verificação de residência)
- **MembershipSettings** (configurações e opt-ins do membro no território) *[novo na refatoração]*
- **MembershipCapability** (capacidades operacionais: Curator, Moderator) *[novo na refatoração]*
- **FeatureFlag** (flags de funcionalidades por território)

### Conteúdo
- **Post** (territoryId, authorId, visibility, status)
- **MapEntity** (territoryId, createdByUserId, name, category, lat/lng, status, visibility)
- **MapEntityRelation** (userId, entityId, createdAt)
- **PostGeoAnchor** (id, postId, lat/lng, type, createdAt)
- **Media** (postId, type, url, metadata) *(pós-MVP)*

### Social
- **FriendRelation** (requester, target, status pending/accepted/blocked) *(pós-MVP)*

### Moderação
- **Report** (territoryId, reporterId, targetType post|user, reason, details, status)
- **Sanction** (scope territory|global, target user|post, type, reason, status, startAt, endAt)

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

### Moderação
- **User 1..N Report** → reports feitos por usuários.
- **Report 0..1 Sanction** → sanções derivadas.
- **User 1..N Sanction** → sanções aplicadas.
- **Territory 0..N Sanction** → sanções territoriais (territoryId preenchido) ou globais (territoryId nulo).

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
- **User** é a pessoa única e global.
- Verificação de identidade (`UserIdentityVerificationStatus`) pertence ao User, não ao Membership.
- Papel técnico global (Admin) pertence ao User.

### Membership como Vínculo Territorial
- **TerritoryMembership** representa apenas o vínculo User ↔ Territory.
- Papel territorial: Visitor ou Resident.
- Verificação de residência: Unverified, GeoVerified, DocumentVerified.
- Regra: 1 Resident por User (máximo) em todo o sistema.

### Settings como Escolhas
- **MembershipSettings** concentra configurações e opt-ins do membro.
- Criado automaticamente com o Membership.
- MarketplaceOptIn é exemplo de setting.

### Capacidades como Poderes Operacionais
- **MembershipCapability** representa capacidades territoriais (Curator, Moderator).
- Não são papéis sociais, são poderes operacionais.
- Empilháveis (um Membership pode ter múltiplas).
- Atuam apenas no território do Membership.

### Marketplace como Regra Composta
- Marketplace não é papel, identidade ou verificação.
- Depende de:
  - Territory.FeatureFlags.MarketplaceEnabled
  - MembershipSettings.MarketplaceOptIn
  - Membership.Role == Resident
  - Membership.ResidencyVerificationStatus != Unverified
  - User.IdentityVerificationStatus == Verified (para operações plenas)

## Observações de MVP vs Pós-MVP
- [MVP] TerritoryMembership, MembershipSettings, MembershipCapability, Post, PostGeoAnchor, MapEntity, MapEntityRelation, Report, Sanction, OutboxMessage, UserNotification, UserPreferences, Store, StoreItem.
- [POST-MVP] Media e FriendRelation.
- [POST-MVP] FriendRelation e comportamentos de círculo interno.
