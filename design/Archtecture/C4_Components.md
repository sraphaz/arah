C4Component
title Araponga API — C4 Component (Nível 3)

Container_Boundary(api, "Araponga API (.NET 8)") {

  Component(auth, "Auth Module", "ASP.NET Core Auth", "Login social (OIDC), emissão/validação de token, sessão e claims.")
  Component(access, "Access Control", "Policy/Rules", "Regras de visibilidade: morador vs visitante; território confirmado; permissões de curadoria.")
  Component(featureFlags, "Feature Flags", "Toggle Service", "Ativa/desativa features por território e por perfil.")

  Component(territories, "Territories Controller", "REST Controller", "Endpoints de territórios (listar, obter, criar).")
  Component(feed, "Feed Controller", "REST Controller", "Endpoints do feed (posts, filtros por território e perfil).")
  Component(map, "Map Controller", "REST Controller", "Entidades do território (lugares, trilhas, cachoeiras, nascentes, mirantes).")
  Component(healthTerritory, "Territory Health Controller", "REST Controller", "Saúde do território: água potável, nascentes, árvores nativas, santuários, mirantes (MVP evolutivo).")
  Component(interactions, "Interactions Controller", "REST Controller", "Curtidas, comentários, compartilhamentos.")
  Component(moderation, "Moderation Controller", "REST Controller", "Fluxos de validação comunitária/curadoria (confirmar entidade, sinalizar abuso).")
  Component(admin, "Admin Controllers", "REST Controller", "SystemConfig, WorkItems, decisões de verificação e downloads por proxy (SystemAdmin/Curator/Moderator).")
  Component(workQueue, "Work Queue", "Application Service", "Fila genérica (WorkItem) para revisões humanas: verificação, curadoria e moderação.")
  Component(evidence, "Evidence Service", "Application Service", "Criação de DocumentEvidence e integração com storage (upload) e download por proxy.")
  Component(storage, "File Storage", "Abstraction", "IFileStorage com providers Local e S3/MinIO.")

  Component(appServices, "Application Services", "Use Cases", "Orquestram casos de uso e regras de negócio (Clean Architecture).")
  Component(domain, "Domain Model", "Entities/Value Objects", "User, Territory, Membership, Place, Event, Post, Alert, etc.")
  Component(repo, "Repositories", "Interfaces", "Contratos de persistência e consultas.")
  Component(ef, "EF Core Persistence", "EF Core + PostGIS", "DbContext, mapeamentos, migrations, consultas geoespaciais.")
  Component(audit, "Audit/Telemetry", "Logging", "Logs de auditoria e rastreabilidade (eventos e trilhas).")
}

Rel(auth, appServices, "Autentica e emite claims", "in-process")
Rel(access, appServices, "Aplica políticas de acesso", "in-process")
Rel(featureFlags, appServices, "Resolve toggles", "in-process")

Rel(territories, appServices, "Chama", "in-process")
Rel(feed, appServices, "Chama", "in-process")
Rel(map, appServices, "Chama", "in-process")
Rel(healthTerritory, appServices, "Chama", "in-process")
Rel(interactions, appServices, "Chama", "in-process")
Rel(moderation, appServices, "Chama", "in-process")
Rel(admin, appServices, "Chama", "in-process")

Rel(appServices, domain, "Usa", "in-process")
Rel(appServices, repo, "Usa", "in-process")
Rel(repo, ef, "Implementa com", "in-process")
Rel(ef, audit, "Emite logs/telemetria", "in-process")
