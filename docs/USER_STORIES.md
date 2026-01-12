# User Stories (Consolidadas)

## Status atual (já desenvolvido)
Com base no estado atual documentado no projeto, já existem entregas relacionadas a:
- Backend inicial estruturado.
- Descoberta e seleção de territórios.
- Diferenciação entre morador (resident) e visitante (visitor).
- Feed e mapa orientados ao território.
- Testes automatizados e CI com builds reprodutíveis.

## MVP

### [MVP][P0] Cadastro e autenticação
**Como** visitante
**Quero** criar uma conta e autenticar
**Para** acessar consultas e ações no território.

**Critérios de aceite**
- Consultas ao feed e ao mapa exigem usuário autenticado.
- Usuário não autenticado é redirecionado para login/cadastro.

### [MVP][P0] Descobrir territórios próximos
**Como** visitante
**Quero** ver territórios próximos à minha localização
**Para** escolher o lugar correto para navegar.

**Critérios de aceite**
- A API retorna territórios ordenados por proximidade.
- A resposta contém apenas dados geográficos.
- Não existe associação remota no MVP.
- Sem localização disponível, a experiência comunica o motivo e sugere ativar permissões.

### [MVP][P1] Consultar território por ID
**Como** visitante
**Quero** consultar um território
**Para** validar detalhes geográficos.

**Critérios de aceite**
- A API retorna o território ou `404`.
- A resposta não inclui regras sociais.
- A consulta exige usuário autenticado.

### [MVP][P0] Declarar vínculo visitor/resident
**Como** usuário autenticado
**Quero** declarar meu papel em um território
**Para** acessar conteúdos adequados.

**Critérios de aceite**
- É possível registrar `VISITOR` ou `RESIDENT`.
- `VISITOR` é validado imediatamente.
- `RESIDENT` entra como `PENDING` até aprovação.
- Papel declarado impacta visibilidade conforme regras do feed (public/residents-only).
- Se a localização não estiver disponível, não é possível declarar vínculo.

### [MVP][P1] Consultar meu vínculo
**Como** usuário autenticado
**Quero** consultar meu vínculo com um território
**Para** entender meu status social.

**Critérios de aceite**
- Retorna `role` e `verificationStatus`.
- Se não houver vínculo, retorna `NONE`.

### [MVP][P0] Feed do território
**Como** visitor ou resident
**Quero** ver o feed do território
**Para** acompanhar atualizações locais.

**Critérios de aceite**
- Posts `PUBLIC` são visíveis para todos.
- Posts `RESIDENTS_ONLY` são visíveis apenas para residentes validados.
- Sem vínculo válido, o usuário recebe orientação para declarar vínculo.

### [MVP][P0] Post com GeoAnchor
**Como** usuário
**Quero** publicar um post associado a um ou mais GeoAnchors
**Para** contextualizar o conteúdo no território.

**Critérios de aceite**
- Post aceita 0..N GeoAnchors.
- GeoAnchors são derivados de mídias quando disponíveis; o client não define anchors manualmente.
- Sem geolocalização, o post pode ser publicado (não aparece no mapa).
- Mídias no post são pós-MVP.

### [MVP][P0] Eventos com aprovação de moradores
**Como** visitante ou morador
**Quero** publicar eventos no território
**Para** mobilizar ações locais com curadoria comunitária.

**Critérios de aceite**
- Moradores podem publicar eventos com status publicado imediatamente.
- Visitantes podem publicar eventos com status pendente até aprovação de moradores.
- Moradores podem aprovar ou rejeitar eventos pendentes.

### [MVP][P0] Entidades territoriais com confirmação
**Como** visitante ou morador
**Quero** sugerir entidades do território
**Para** registrar lugares relevantes no mapa.

**Critérios de aceite**
- Visitantes e moradores podem sugerir entidades.
- Moradores confirmam entidades sugeridas.
- Moradores podem se relacionar com entidades do território.
- Entidades podem ser usadas como filtro no feed/timeline.

### [POST-MVP] Sincronia feed ↔ mapa
**Como** usuário
**Quero** que a timeline do feed sincronize com a geolocalização dos posts
**Para** identificar o pin correspondente no mapa.

**Critérios de aceite**
- Ao navegar na timeline, o pin correspondente fica destacado.
- Ao tocar no pin, o post é destacado/aberto no feed.
- Se o post estiver oculto por moderação, ele não aparece nem no feed nem no mapa.

### [MVP][P1] Feed pessoal
**Como** usuário
**Quero** ver meus próprios posts em um feed pessoal
**Para** acompanhar meu histórico no território.

**Critérios de aceite**
- Feed pessoal lista posts do usuário.

### [MVP][P0] Reportar post
**Como** usuário
**Quero** reportar um post inadequado
**Para** ajudar na moderação do território.

**Critérios de aceite**
- Report exige motivo e detalhes opcionais.
- Report gera item de moderação.
- Reports repetidos do mesmo usuário na mesma janela são ignorados.

### [MVP][P0] Reportar usuário
**Como** usuário
**Quero** reportar um usuário inadequado
**Para** proteger a comunidade.

**Critérios de aceite**
- Report exige motivo e detalhes opcionais.
- Reports repetidos do mesmo usuário na mesma janela são ignorados.

### [MVP][P1] Notificações in-app
**Como** usuário
**Quero** receber notificações sobre eventos relevantes
**Para** acompanhar atualizações do território.

**Critérios de aceite**
- Eventos de post e report geram notificações in-app para os destinatários.
- Notificações são persistidas via outbox/inbox e listáveis pela API.
- Usuário consegue marcar notificações como lidas.

### [MVP][P0] Bloquear usuário
**Como** usuário
**Quero** bloquear outro usuário
**Para** não ver mais seu conteúdo.

**Critérios de aceite**
- Usuário bloqueado não aparece nos feeds do bloqueador.
- Conteúdo do usuário bloqueado não aparece no mapa do bloqueador.

### [MVP][P1] Moderação automática simples
**Como** moderador
**Quero** que o sistema aplique ações automáticas por threshold de reports
**Para** reduzir dano imediato.

**Critérios de aceite**
- Threshold de reports únicos por janela de tempo gera ação automática.
- Ação possível: ocultar conteúdo, restringir ou suspender territorialmente.
- Ação automática gera registro de auditoria.

### [MVP][P1] Sanções territoriais e globais
**Como** moderador
**Quero** aplicar sanções por território ou globais
**Para** ajustar a gravidade da resposta.

**Critérios de aceite**
- Sanção territorial afeta apenas um território.
- Sanção global bloqueia acesso a todos os territórios.

### [MVP][P1] Portal estático e documentação
**Como** visitante
**Quero** acessar o portal de autosserviço
**Para** entender rapidamente a visão do produto e seus domínios.

**Critérios de aceite**
- Portal estático publicado via GitHub Pages.
- Links para documentação e changelog disponíveis.

### [MVP][P1] Integração de mapas (base)
**Como** sistema
**Quero** integrar um provedor de mapas
**Para** renderizar os GeoAnchors no mapa do território.

**Critérios de aceite**
- Há configuração para trocar o provedor de mapa.
- O mapa suporta pin/cluster básico para posts.

### [MVP][P1] Observabilidade técnica mínima
**Como** time técnico
**Quero** métricas e logs mínimos
**Para** monitorar erros, latência e falhas de geolocalização.

**Critérios de aceite**
- Erros de geolocalização são logados com contexto mínimo.
- Há métricas de requisições e de falhas em report/moderação.

## Pós-MVP (registro)
- [POST-MVP] Buscar território por texto (nome/cidade/estado).
- [POST-MVP] Sugerir território canônico.
- [POST-MVP] Validar vínculos resident (curadoria dedicada).
- [POST-MVP] Interações no feed (curtir/comentar/compartilhar).
- [POST-MVP] Mapa de entidades além de posts (sugerir/validar/confirmar pontos).
- [POST-MVP] Friends (círculo interno) com solicitações e aceite.
- [POST-MVP] Stories visíveis apenas para friends.
- [POST-MVP] GeoAnchor avançado (memórias, galeria, pins visuais).
- [POST-MVP] Indicadores de saúde territorial e alertas ambientais.
- [POST-MVP] Admin/observabilidade com visão de territórios, erros e relatórios.
- [POST-MVP] Produtos/serviços territoriais.
- [POST-MVP] Integrações externas e indicadores comunitários/ambientais.
