# Resumo de Endpoints Principais - API Araponga

**Parte de**: [API Araponga - Lógica de Negócio e Usabilidade](./60_API_LÓGICA_NEGÓCIO_INDEX.md)  
**Versão**: 2.0  
**Data**: 2025-01-20

---

## 📚 Resumo de Endpoints Principais

### Autenticação
- `POST /api/v1/auth/social` - Login/Cadastro social

### Territórios
- `GET /api/v1/territories` - Listar territórios
- `GET /api/v1/territories/nearby` - Territórios próximos
- `GET /api/v1/territories/search` - Buscar territórios
- `GET /api/v1/territories/{id}` - Consultar território
- `POST /api/v1/territories/suggestions` - Sugerir território
- `POST /api/v1/territories/selection` - Selecionar território ativo
- `GET /api/v1/territories/selection` - Consultar território ativo

### Memberships
- `POST /api/v1/territories/{id}/enter` - Entrar como VISITOR
- `POST /api/v1/memberships/{territoryId}/become-resident` - Solicitar residência
- `GET /api/v1/memberships/{territoryId}/me` - Consultar vínculo
- `POST /api/v1/memberships/{territoryId}/verify-residency/geo` - Verificar por geolocalização
- `POST /api/v1/memberships/{territoryId}/verify-residency/document` - Verificar por documento

### Feed
- `POST /api/v1/feed` - Criar post
- `GET /api/v1/feed` - Listar feed
- `GET /api/v1/feed/me` - Feed pessoal
- `POST /api/v1/feed/{id}/likes` - Curtir post
- `POST /api/v1/feed/{id}/comments` - Comentar post
- `POST /api/v1/feed/{id}/shares` - Compartilhar post

### Eventos
- `POST /api/v1/events` - Criar evento
- `GET /api/v1/events` - Listar eventos
- `GET /api/v1/events/nearby` - Eventos próximos
- `POST /api/v1/events/{id}/interest` - Marcar interesse
- `POST /api/v1/events/{id}/confirm` - Confirmar participação
- `POST /api/v1/events/{id}/cancel` - Cancelar evento

### Mapa
- `GET /api/v1/map/entities` - Listar entidades
- `POST /api/v1/map/entities` - Sugerir entidade
- `PATCH /api/v1/map/entities/{id}/validation` - Validar entidade
- `POST /api/v1/map/entities/{id}/confirmations` - Confirmar entidade
- `POST /api/v1/map/entities/{id}/relations` - Relacionar-se com entidade
- `GET /api/v1/map/pins` - Obter pins do mapa

### Alertas
- `POST /api/v1/alerts` - Reportar alerta
- `GET /api/v1/alerts` - Listar alertas
- `PATCH /api/v1/alerts/{id}/validation` - Validar alerta

### Assets
- `POST /api/v1/assets` - Criar asset
- `GET /api/v1/assets` - Listar assets
- `POST /api/v1/assets/{id}/validate` - Validar asset

### Marketplace
- `POST /api/v1/stores` - Criar store
- `GET /api/v1/stores` - Listar stores
- `POST /api/v1/items` - Criar item
- `GET /api/v1/items` - Buscar items
- `POST /api/v1/items/{id}/inquiries` - Criar inquiry
- `POST /api/v1/cart` - Adicionar ao carrinho
- `GET /api/v1/cart` - Obter carrinho
- `POST /api/v1/cart/checkout` - Finalizar compra
- Observação: o módulo de marketplace é controlado por flag territorial `MARKETPLACEENABLED`. Quando desabilitado no território, endpoints de consulta/ação retornam `404` para evitar exposição do marketplace.

### Chat
- `GET /api/v1/territories/{territoryId}/chat/channels` - Listar canais do território (Público/Moradores)
- `GET /api/v1/territories/{territoryId}/chat/groups` - Listar grupos do território (apenas ativos)
- `POST /api/v1/territories/{territoryId}/chat/groups` - Criar grupo (pendente de aprovação)
- `POST /api/v1/territories/{territoryId}/chat/groups/{groupId}/approve` - Aprovar/habilitar grupo (curadoria)
- `POST /api/v1/territories/{territoryId}/chat/groups/{groupId}/disable` - Desabilitar grupo (moderação)
- `POST /api/v1/territories/{territoryId}/chat/groups/{groupId}/lock` - Trancar grupo (moderação)
- `GET /api/v1/chat/conversations/{conversationId}` - Detalhes da conversa
- `GET /api/v1/chat/conversations/{conversationId}/messages` - Listar mensagens (cursor-based)
- `POST /api/v1/chat/conversations/{conversationId}/messages` - Enviar mensagem
- `GET /api/v1/chat/conversations/{conversationId}/participants` - Listar participantes
- `POST /api/v1/chat/conversations/{conversationId}/participants` - Adicionar participante (owner/admin)
- `DELETE /api/v1/chat/conversations/{conversationId}/participants/{userId}` - Remover participante
- `POST /api/v1/chat/conversations/{conversationId}/read` - Marcar conversa como lida

### Notificações
- `GET /api/v1/notifications` - Listar notificações
- `POST /api/v1/notifications/{id}/read` - Marcar como lida

### Moderação
- `POST /api/v1/reports/posts/{id}` - Reportar post
- `POST /api/v1/reports/users/{id}` - Reportar usuário
- `GET /api/v1/reports` - Listar reports (curadoria)
- `POST /api/v1/users/{id}/block` - Bloquear usuário
- `DELETE /api/v1/users/{id}/block` - Desbloquear usuário

### Governança comunitária
- `GET /api/v1/users/me/interests` - Listar interesses
- `POST /api/v1/users/me/interests` - Adicionar interesse
- `DELETE /api/v1/users/me/interests/{tag}` - Remover interesse
- `GET /api/v1/users/me/profile/governance` - Histórico de participação (votações, moderação)
- `POST /api/v1/territories/{territoryId}/votings` - Criar votação
- `GET /api/v1/territories/{territoryId}/votings` - Listar votações
- `GET /api/v1/territories/{territoryId}/votings/{votingId}` - Obter votação
- `POST /api/v1/territories/{territoryId}/votings/{votingId}/vote` - Votar
- `POST /api/v1/territories/{territoryId}/votings/{votingId}/close` - Fechar votação
- `GET /api/v1/territories/{territoryId}/votings/{votingId}/results` - Resultados

### Join Requests
- `POST /api/v1/territories/{id}/join-requests` - Criar solicitação
- `GET /api/v1/join-requests/incoming` - Listar recebidas
- `POST /api/v1/join-requests/{id}/approve` - Aprovar
- `POST /api/v1/join-requests/{id}/reject` - Rejeitar
- `POST /api/v1/join-requests/{id}/cancel` - Cancelar

### Mídias
- `POST /api/v1/media/upload` - Upload de mídia
- `GET /api/v1/territories/{territoryId}/media-config` - Obter configuração de mídias
- `PUT /api/v1/territories/{territoryId}/media-config` - Atualizar configuração (Curator)
- `GET /api/v1/user/media-preferences` - Obter preferências de mídia
- `PUT /api/v1/user/media-preferences` - Atualizar preferências de mídia

### Feature Flags
- `GET /api/v1/territories/{id}/features` - Listar flags
- `PUT /api/v1/territories/{id}/features` - Atualizar flags (curadoria)

### Preferências de Usuário
- `GET /api/v1/users/me/preferences` - Obter preferências
- `PUT /api/v1/users/me/preferences/privacy` - Atualizar privacidade
- `PUT /api/v1/users/me/preferences/notifications` - Atualizar notificações

### Admin
- `GET /api/v1/admin/system-config` - Obter configurações globais
- `PUT /api/v1/admin/system-config` - Atualizar configurações (SystemAdmin)
- `GET /api/v1/admin/work-items` - Listar work items globais
- `POST /api/v1/admin/work-items/{workItemId}/complete` - Completar work item
- `GET /api/v1/territories/{territoryId}/work-items` - Listar work items territoriais
- `POST /api/v1/territories/{territoryId}/work-items/{workItemId}/complete` - Completar work item territorial

### Verificações e Evidências
- `POST /api/v1/verification/identity/document/upload` - Upload de documento de identidade
- `POST /api/v1/memberships/{territoryId}/verify-residency/document/upload` - Upload de documento de residência
- `POST /api/v1/admin/verifications/identity/{workItemId}/decide` - Decidir verificação de identidade (SystemAdmin)
- `POST /api/v1/territories/{territoryId}/verification/residency/{workItemId}/decide` - Decidir verificação de residência (Curator)
- `GET /api/v1/admin/evidences/{evidenceId}/download` - Download de evidência (SystemAdmin)
- `GET /api/v1/territories/{territoryId}/evidences/{evidenceId}/download` - Download de evidência (Curator/Moderator)

---

## 📚 Documentação Detalhada

Para detalhes completos de cada endpoint, consulte os subdocumentos específicos:

- **[Autenticação](./60_01_API_AUTENTICACAO.md)**
- **[Territórios](./60_02_API_TERRITORIOS.md)**
- **[Vínculos e Membros](./60_03_API_MEMBERSHIPS.md)**
- **[Feed](./60_04_API_FEED.md)**
- **[Eventos](./60_05_API_EVENTOS.md)**
- **[Mapa](./60_06_API_MAPA.md)**
- **[Alertas](./60_07_API_ALERTAS.md)**
- **[Assets](./60_08_API_ASSETS.md)**
- **[Marketplace](./60_09_API_MARKETPLACE.md)**
- **[Chat](./60_10_API_CHAT.md)**
- **[Notificações](./60_11_API_NOTIFICACOES.md)**
- **[Moderação](./60_12_API_MODERACAO.md)**
- **[Join Requests](./60_13_API_JOIN_REQUESTS.md)**
- **[Admin](./60_14_API_ADMIN.md)**
- **[Mídias](./60_15_API_MIDIAS.md)**
- **[Feature Flags](./60_16_API_FEATURE_FLAGS.md)**
- **[Visibilidade](./60_17_API_VISIBILIDADE.md)**
- **[Preferências](./60_18_API_PREFERENCIAS.md)**

---

## 🔗 Links Úteis

- **DevPortal**: `devportal.araponga.app/` - Portal de desenvolvedor com exemplos práticos
- **OpenAPI Explorer**: `devportal.araponga.app/#openapi` - Contratos completos da API
- **Diagramas de Sequência**: `devportal.araponga.app/#fluxos` - Fluxos principais documentados

---

**Voltar para**: [Índice da Documentação da API](./60_API_LÓGICA_NEGÓCIO_INDEX.md)

