# Modelo de Domínio (MER conceitual)

## Entidades principais
- **User**
- **Territory**
- **UserTerritoryLink** (vínculo visitor/resident, status pending/approved)
- **Post** (territoryId, authorId, visibility, status)
- **GeoAnchor** (territoryId, lat/lng, type, linkedPostId opcional)
- **Media** (postId, type, url, metadata)
- **FriendRelation** (requester, target, status pending/accepted/blocked)
- **Report** (reporterId, targetType post|user, reason, details, status)
- **Sanction** (userId, territoryId null=global, type warning|restriction|suspension|ban, expiresAt, sourceReportId)

## Relacionamentos (alto nível)
- **User 1..N UserTerritoryLink** → vínculo com territórios.
- **Territory 1..N UserTerritoryLink** → vínculos de presença.
- **User 1..N Post** → autor das postagens.
- **Territory 1..N Post** → posts no território.
- **Post 1..N GeoAnchor** → localização(ões) da postagem.
- **Post 0..N Media** → mídias anexadas.
- **GeoAnchor 0..1 Post** → GeoAnchor pode referenciar post específico.
- **User N..N FriendRelation** → relações friends (pós-MVP).
- **User 1..N Report** → reports feitos por usuários.
- **Report 0..1 Sanction** → sanções derivadas.
- **User 1..N Sanction** → sanções aplicadas.
- **Territory 0..N Sanction** → sanções territoriais (territoryId preenchido) ou globais (territoryId nulo).

## Observações de MVP vs Pós-MVP
- [MVP] UserTerritoryLink, Post, GeoAnchor, Media, Report, Sanction.
- [POST-MVP] FriendRelation e comportamentos de círculo interno.
