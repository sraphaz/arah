# Araponga — User Stories Revisadas

> **Regra de ouro:** `territory` é exclusivamente geográfico/canônico (lugar físico real).  
> `territory` **nunca** contém regras sociais (roles, membership, moderação, visibilidade, permissões).

## Epic: Territory (Geográfico)

### US-T01 — Descobrir territórios próximos
**Como** visitante  
**Quero** ver territórios próximos à minha localização  
**Para** escolher o lugar correto para navegar

**Critérios de aceite**
- Dado `lat` e `lng`, quando consultar `GET /territories/nearby`, então a API retorna territórios ordenados por proximidade.
- A resposta contém apenas dados geográficos (nome, cidade, estado, lat/lng, status canônico).

### US-T02 — Buscar território por texto
**Como** visitante  
**Quero** buscar territórios por nome/cidade/estado  
**Para** encontrar um território específico

**Critérios de aceite**
- Quando consultar `GET /territories/search?q&city&state`, então a API retorna territórios que correspondem aos filtros.
- A resposta não inclui qualquer dado social (roles, membership, visibilidade).

### US-T03 — Sugerir território canônico
**Como** usuário  
**Quero** sugerir um território geográfico  
**Para** habilitar a expansão da base canônica

**Critérios de aceite**
- Quando enviar `POST /territories/suggestions`, então o território é criado com status `PENDING`.
- A resposta contém apenas dados geográficos.

### US-T04 — Consultar território por ID
**Como** visitante  
**Quero** consultar um território  
**Para** validar detalhes geográficos

**Critérios de aceite**
- Quando consultar `GET /territories/{id}`, então a API retorna o território ou `404` se não existir.
- A resposta não inclui regras sociais.

---

## Epic: Camada Social (Membership/Visibilidade)

### US-S01 — Declarar membership
**Como** usuário autenticado  
**Quero** declarar meu papel (visitor/resident) em um território  
**Para** acessar conteúdos adequados

**Critérios de aceite**
- Quando enviar `POST /api/v1/territories/{id}/enter`, então a API cria (ou retorna) o vínculo como `VISITOR`.
- Para solicitar residência, o usuário chama `POST /api/v1/memberships/{id}/become-resident` (cria uma JoinRequest).
- `ResidencyVerification` do membership começa como `NONE` e só muda após verificação (geo/doc).

### US-S02 — Consultar meu membership
**Como** usuário autenticado  
**Quero** consultar meu vínculo com um território  
**Para** entender meu status social

**Critérios de aceite**
- Quando consultar `GET /api/v1/memberships/{id}/me`, então a API retorna `role` e `residencyVerification`.
- Se não houver vínculo, retorna `404`.

### US-S03 — Aprovar pedido de moradia (curadoria)
**Como** curador
**Quero** aprovar ou rejeitar pedidos de moradia (JoinRequests)
**Para** manter governança comunitária

**Critérios de aceite**
- Apenas usuários com capability `CURATOR` (ou destinatários elegíveis) podem aprovar/rejeitar.
- A ação registra auditoria.

### US-S04 — Solicitar aprovação para virar morador confirmado
**Como** visitante autenticado
**Quero** solicitar a confirmação como morador
**Para** ingressar na comunidade sem broadcast para todo o território

**Critérios de aceite**
- Quando enviar `POST /api/v1/memberships/{id}/become-resident`, a API cria um pedido `PENDING` (JoinRequest) com destinatários automáticos.
- Alternativamente (caso avançado), é possível usar `POST /api/v1/territories/{id}/join-requests` com `recipientUserIds[1..N]`.
- Apenas moradores já verificados (geo/doc) ou curadores podem ser destinatários.
- O pedido não gera post no feed nem broadcast.
- `GET /join-requests/incoming?status=pending` retorna apenas pedidos onde o usuário é destinatário.
- Destinatários (ou curadores) podem aprovar/rejeitar via `POST /join-requests/{id}/approve|reject`.
- Ao aprovar, o requester recebe membership `RESIDENT` com `ResidencyVerification=NONE` (não verificado).
- A verificação de residência ocorre depois via `POST /api/v1/memberships/{id}/verify-residency/geo|document`.

--- 

## Epic: Feed Comunitário

### US-F01 — Visualizar feed
**Como** visitante ou morador  
**Quero** ver o feed do território  
**Para** acompanhar atualizações locais

**Critérios de aceite**
- `PUBLIC` visível para todos.
- `RESIDENTS_ONLY` visível apenas para membros `RESIDENT` verificados (geo/doc).
- Sem autenticação ou sem membership ⇒ apenas `PUBLIC`.
- É possível filtrar o feed por entidade territorial quando disponível.
- É possível filtrar o feed por asset via `assetId`.

### US-F02 — Criar post
**Como** morador  
**Quero** criar posts no feed  
**Para** compartilhar informações com a comunidade

**Critérios de aceite**
- Apenas `RESIDENT` validado pode criar posts.
- Tipos de post seguem feature flags por território.
- Eventos podem ser criados por visitantes com status `PENDING` até aprovação de moradores.
- Moradores aprovam ou rejeitam eventos pendentes.
- Posts podem referenciar assets do território.

### US-F03 — Interações (curtir/comentar/compartilhar)
**Como** usuário  
**Quero** interagir com posts  
**Para** engajar com a comunidade

**Critérios de aceite**
- Curtidas em posts `PUBLIC` permitidas para visitantes autenticados ou por sessão.
- Comentários e compartilhamentos permitidos apenas para `RESIDENT` validado.

---

## Epic: Mapa

### US-M01 — Visualizar entidades do mapa
**Como** visitante ou morador  
**Quero** ver entidades do território  
**Para** explorar pontos relevantes

**Critérios de aceite**
- `PUBLIC` visível para todos.
- `RESIDENTS_ONLY` visível apenas para `RESIDENT` validado.

### US-M02 — Sugerir entidade
**Como** visitante ou morador  
**Quero** sugerir entidades no mapa  
**Para** enriquecer o território

**Critérios de aceite**
- Visitantes e moradores podem sugerir.
- Entidade sugerida nasce como `SUGGESTED`.

### US-M03 — Validar entidade (curadoria)
**Como** curador  
**Quero** validar entidades sugeridas  
**Para** garantir qualidade

**Critérios de aceite**
- Apenas `CURATOR` pode validar.
- Ação auditada.

### US-M04 — Confirmar entidade
**Como** morador  
**Quero** confirmar entidades existentes  
**Para** aumentar confiabilidade

**Critérios de aceite**
- Somente `RESIDENT` validado pode confirmar.

### US-M05 — Relacionar morador a entidade
**Como** morador  
**Quero** me relacionar com entidades do território  
**Para** declarar vínculo com lugares relevantes

**Critérios de aceite**
- Somente `RESIDENT` validado pode se relacionar.

---

## Epic: Alertas do Território

### US-A01 — Reportar alerta
**Como** morador  
**Quero** reportar alertas ambientais  
**Para** notificar problemas locais

**Critérios de aceite**
- Apenas `RESIDENT` validado pode reportar.
- Alerta inicia com status `PENDING`.

### US-A02 — Validar alerta
**Como** curador
**Quero** validar alertas
**Para** destacar problemas no feed

**Critérios de aceite**
- Apenas `CURATOR` pode validar.
- Ao validar, um post `ALERT` é criado no feed e ganha destaque visual.
- Alertas validados aparecem como pins no mapa.

## Epic: Assets do Território

### US-T01 — Cadastrar assets
**Como** morador validado  
**Quero** cadastrar assets do território  
**Para** registrar recursos geolocalizados no mapa e no feed

**Critérios de aceite**
- Assets devem ter ao menos um geo anchor.
- Apenas `RESIDENT` validado ou `CURATOR` pode criar/editar/arquivar.
- Assets aparecem como pins no mapa e podem ser filtrados por tipo.

### US-T02 — Validar assets
**Como** morador validado  
**Quero** validar assets  
**Para** confirmar coletivamente que eles existem/estão ativos

**Critérios de aceite**
- Cada morador validado pode validar um asset uma única vez.
- O sistema expõe contagem de validações e percentual sobre moradores elegíveis.

---

## Epic: Notificações

### US-N01 — Notificações in-app
**Como** usuário
**Quero** receber notificações sobre eventos relevantes
**Para** acompanhar atualizações do território

**Critérios de aceite**
- Eventos de post e report geram notificações in-app para os destinatários.
- Notificações são persistidas via outbox/inbox e listáveis pela API.
- Usuário consegue marcar notificações como lidas.

---

## Epic: Portal & Documentação

### US-P01 — Acessar portal estático
**Como** visitante  
**Quero** acessar o portal de autosserviço como site estático  
**Para** entender rapidamente a visão do produto e seus domínios

**Critérios de aceite**
- O portal estático é publicado via GitHub Pages.
- A página inicial explica visão, domínios, fluxos e quickstart.

### US-P02 — Navegar pela documentação do produto
**Como** colaborador  
**Quero** acessar documentação, user stories e changelog pelo portal  
**Para** me orientar sobre o estado atual do produto

**Critérios de aceite**
- O portal inclui links para user stories e changelog.
- A documentação fica disponível na mesma publicação estática.
