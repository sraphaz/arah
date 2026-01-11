# Modelo de Domínio (MER conceitual)

## Entidades principais
- **User**
- **Territory**
- **UserTerritoryLink** (vínculo visitor/resident, status pending/approved)
- **Post** (territoryId, authorId, visibility, status)
- **MapEntity** (territoryId, createdByUserId, name, category, lat/lng, status, visibility)
- **MapEntityRelation** (userId, entityId, createdAt)
- **PostGeoAnchor** (id, postId, lat/lng, type, createdAt)
- **Media** (postId, type, url, metadata) *(pós-MVP)*
- **FriendRelation** (requester, target, status pending/accepted/blocked)
- **Report** (territoryId, reporterId, targetType post|user, reason, details, status)
- **Sanction** (scope territory|global, target user|post, type, reason, status, startAt, endAt)

## Relacionamentos (alto nível)
- **User 1..N UserTerritoryLink** → vínculo com territórios.
- **Territory 1..N UserTerritoryLink** → vínculos de presença.
- **User 1..N Post** → autor das postagens.
- **Territory 1..N Post** → posts no território.
- **Post 1..N PostGeoAnchor** → localização(ões) da postagem.
- **Post 0..N Media** → mídias anexadas (pós-MVP).
- **Post 0..1 MapEntity** → postagem pode referenciar entidade territorial.
- **User 0..N MapEntityRelation** → moradores vinculam-se a entidades.
- **PostGeoAnchor 1..1 Post** → GeoAnchor referencia um post específico.
- **User N..N FriendRelation** → relações friends (pós-MVP).
- **User 1..N Report** → reports feitos por usuários.
- **Report 0..1 Sanction** → sanções derivadas.
- **User 1..N Sanction** → sanções aplicadas.
- **Territory 0..N Sanction** → sanções territoriais (territoryId preenchido) ou globais (territoryId nulo).

## Observações de MVP vs Pós-MVP
- [MVP] UserTerritoryLink, Post, PostGeoAnchor, MapEntity, MapEntityRelation, Report, Sanction.
- [POST-MVP] Media e FriendRelation.
- [POST-MVP] FriendRelation e comportamentos de círculo interno.
