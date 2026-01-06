C4Container
title Araponga — C4 Container (Nível 2)

Person(user, "Usuário", "Morador ou Visitante")
Person(moderator, "Curador/Moderador", "Morador verificado")

System_Boundary(s1, "Araponga") {
  Container(mobile, "App Mobile", "Flutter/React Native (futuro)", "Cliente principal para feed, mapa, eventos, alertas e verificação de morador.")
  Container(web, "Web App", "Next.js/React (futuro)", "Cliente web para exploração e administração leve.")
  Container(api, "Araponga API (BFF)", ".NET 8 ASP.NET Core", "Exposição de endpoints REST: territórios, feed, mapa, entidades, interações e auth.")
  ContainerDb(db, "Banco de Dados", "PostgreSQL + PostGIS", "Persistência relacional e geoespacial: territórios, memberships, entidades, feed e saúde do território.")
  Container(storage, "Object Storage", "S3 compatível (futuro)", "Armazenamento de mídia: fotos de usuários, posts, lugares, eventos, alertas.")
  Container(queue, "Event Bus", "RabbitMQ/Kafka (futuro)", "Eventos de domínio/integração: auditoria, notificação, indexação, moderação.")
}

System_Ext(identity, "Provedores de Identidade", "Google/Apple/Microsoft (OIDC)")
System_Ext(payments, "Meios de Pagamento", "Mercado Pago/PayPal")
System_Ext(push, "Push Notifications", "FCM/APNs")
System_Ext(maps, "Mapas/Geocoding", "Tiles/Geocode")

Rel(user, mobile, "Interage", "HTTPS")
Rel(user, web, "Interage", "HTTPS")
Rel(moderator, web, "Modera/Valida", "HTTPS")

Rel(mobile, api, "Consome API", "HTTPS/JSON")
Rel(web, api, "Consome API", "HTTPS/JSON")

Rel(api, db, "Lê/Escreve", "SQL")
Rel(api, storage, "Upload/Download mídia", "HTTPS")
Rel(api, queue, "Publica eventos", "AMQP/HTTP")

Rel(api, identity, "Auth login social", "OIDC/OAuth2")
Rel(api, payments, "Cobranças/doações", "HTTPS")
Rel(api, push, "Dispara push", "HTTPS")
Rel(api, maps, "Geocoding/tiles", "HTTPS")
