C4Context
title Araponga — C4 Context (Nível 1)

Person(user, "Usuário", "Morador ou Visitante. Interage com feed, mapa, eventos, alertas e saúde do território.")
Person(moderator, "Curador/Moderador", "Morador verificado com permissões ampliadas para validação e governança.")

System(araponga, "Araponga", "Plataforma comunitária orientada a território: feed, mapa, entidades e governança mínima.")
System_Ext(identity, "Provedores de Identidade", "Login social (Google, Apple, Microsoft) / OIDC")
System_Ext(payments, "Meios de Pagamento", "Mercado Pago / PayPal (MVP monetização e doações)")
System_Ext(notifications, "Notificações", "Push (FCM/APNs), e-mail (futuro)")
System_Ext(maps, "Mapas e Geocoding", "Tiles, geocoding/reverse geocoding (futuro)")

Rel(user, araponga, "Usa para explorar e contribuir com o território", "HTTPS")
Rel(moderator, araponga, "Modera e valida entidades do território", "HTTPS")

Rel(araponga, identity, "Autentica usuários", "OIDC/OAuth2")
Rel(araponga, payments, "Processa pagamentos/doações", "HTTPS")
Rel(araponga, notifications, "Envia notificações", "HTTPS")
Rel(araponga, maps, "Consulta serviços de mapas", "HTTPS")
