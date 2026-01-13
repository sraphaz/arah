# Matriz de rastreabilidade

| Feature/User Story | Documento de referência | Status (MVP/Posterior) | Evidência no código (arquivo + trecho) | Gap | Correção aplicada |
| --- | --- | --- | --- | --- | --- |
| Territórios próximos (nearby) | `docs/BACKLOG.md` | MVP | `backend/Araponga.Api/Controllers/TerritoriesController.cs` (`Nearby` com `radiusKm`/`limit`) | Endpoint sem raio/limite. | Adicionados parâmetros de raio/limite e filtro por distância. |
| Membership visitor/resident | `docs/USER_STORIES.md` | MVP | `backend/Araponga.Api/Controllers/MembershipsController.cs` (validação de headers geo para `RESIDENT`) | Solicitação resident sem presença mínima. | Header `X-Geo-Latitude/X-Geo-Longitude` obrigatório para resident. |
| Feed posts/eventos | `docs/USER_STORIES.md` | MVP | `backend/Araponga.Api/Controllers/EventsController.cs` + `FeedController.cs` (posts com referência a eventos) | Eventos estavam acoplados a posts com aprovação. | Eventos viraram entidade própria com integração no feed e no mapa. |
| MapEntities georreferenciadas | `docs/DOMAIN_MODEL_MER.md` | MVP | `backend/Araponga.Domain/Map/MapEntity.cs` (Latitude/Longitude) | MapEntity sem lat/lng. | Lat/lng adicionados com validação. |
| GeoAnchors + pins (feed ↔ mapa) | `docs/BACKLOG.md` | MVP | `backend/Araponga.Api/Controllers/MapController.cs` (`GET /api/v1/map/pins`) | Não havia endpoint de pins nem tabela de anchors. | Criado `PostGeoAnchor` + endpoint `/map/pins`. |
| Reports, blocks, sanctions | `docs/MODERATION_REPORTS.md` | MVP | `backend/Araponga.Application/Services/ReportService.cs` (threshold + sanção) | Sem automação, sanção, listagem e unblock. | Threshold 7d/3 reports, sanções, listagem de reports e DELETE unblock. |
| Notificações in-app | `docs/USER_STORIES.md` | MVP | `backend/Araponga.Api/Controllers/NotificationsController.cs` (`GET/POST /api/v1/notifications`) | Não havia inbox/outbox nem endpoints de leitura. | Outbox persistido, worker dispatcher e endpoints de listagem/leitura. |
| Sessão (X-Session-Id) | `docs/README.md` | MVP | `backend/Araponga.Api/ApiHeaders.cs` e `TerritoriesController` (selection) | Cabeçalho de sessão não documentado. | Documentação adicionada em `/docs/README.md`. |
| Feature flags | `docs/README.md` | MVP | `backend/Araponga.Api/Controllers/FeaturesController.cs` | Endpoints sem documentação clara. | Documentação adicionada em `/docs/README.md`. |
