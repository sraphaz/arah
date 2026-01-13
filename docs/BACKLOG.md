# Backlog

## Observações de status
- **Já desenvolvido**: baseado no estado atual documentado do projeto.
- Itens MVP abaixo mantêm critérios de aceite curtos.

## Epic 1 — Território e vínculo
### Feature: Cadastro e autenticação
- [MVP] **Cadastro e login obrigatórios para consulta** *(já desenvolvido)*
  - Critérios de aceite:
    - Consultas de feed e mapa exigem usuário autenticado.
    - Usuário não autenticado é direcionado ao cadastro/login.

### Feature: Descoberta de território por presença
- [MVP] **Busca de territórios próximos via localização** *(já desenvolvido)*
  - Critérios de aceite:
    - Usuário vê territórios próximos a partir de sua posição atual.
    - Resultados respeitam raio (`radiusKm`) e limite (`limit`).
    - Não existe opção de associação remota no MVP.
    - Sem permissão de localização, o fluxo orienta o usuário a habilitar acesso.
- [MVP] **Entrada como visitor** *(já desenvolvido)*
  - Critérios de aceite:
    - Usuário entra no território com papel visitor.
    - O vínculo fica registrado como visitor.

### Feature: Vínculo visitor/resident
- [MVP] **Solicitação de vínculo resident** *(já desenvolvido)*
  - Critérios de aceite:
    - Usuário pode solicitar mudança para resident.
    - Status de vínculo fica como pending/approved.
    - Sem localização válida (lat/lng), a solicitação é bloqueada.
- [MVP] **Status de vínculo visível no perfil**
  - Critérios de aceite:
    - Perfil do usuário exibe visitor ou resident.

## Epic 2 — Feed e mapa integrados
### Feature: Feed do território e feed pessoal
- [MVP] **Feed do território com posts georreferenciados** *(já desenvolvido)*
  - Critérios de aceite:
    - Feed do território lista posts associados ao território.
    - Posts podem ter 0..N GeoAnchors; sem geo, o post só aparece no feed.
    - Sem vínculo válido, o usuário recebe orientação para declarar vínculo.
- [MVP] **Eventos do território (abertos)**
  - Critérios de aceite:
    - Visitantes e moradores podem criar eventos com data/hora e local obrigatório.
    - Evento registra se o criador era visitante ou morador no momento da criação.
    - Participações Interested/Confirmed são registradas.
    - Eventos aparecem no feed e no mapa via filtros.
- [MVP] **Feed pessoal do usuário**
  - Critérios de aceite:
    - Usuário vê seus próprios posts em um feed pessoal.

### Feature: Mapa + feed integrados
- [MVP] **Pins no mapa para posts** *(já desenvolvido)*
  - Critérios de aceite:
    - Post aparece no mapa quando tiver GeoAnchor(s).
    - Pin expõe dados mínimos (id, type, lat/lng, title, status).
- [POST-MVP] **Sincronia timeline ↔ mapa (pin)**
  - Critérios de aceite:
    - Ao navegar na timeline, o pin correspondente é destacado (UI).
    - Ao tocar no pin, o post é destacado/aberto no feed (UI).
    - Conteúdo oculto por moderação não aparece no mapa.
- [MVP] **Filtro de feed por entidade territorial**
  - Critérios de aceite:
    - Feed pode filtrar posts associados a uma entidade.
    - Entidades são sugeridas por visitantes ou moradores e confirmadas por moradores.
    - Moradores podem se relacionar com entidades do território.

## Epic 3 — Postagem e GeoAnchor
### Feature: Criação de post
- [POST-MVP] **Post com múltiplas mídias**
  - Critérios de aceite:
    - Post pode conter mais de uma mídia.
- [MVP] **Post com múltiplos GeoAnchors**
  - Critérios de aceite:
    - Post aceita 0..N GeoAnchors.
    - GeoAnchors são derivados de mídia quando disponíveis; request do client não define anchors.
    - Sem geolocalização, o post ainda pode ser publicado (mas não aparece no mapa).

### Feature: GeoAnchor avançado
- [POST-MVP] **Memórias, galeria e pins visuais avançados**

## Epic 4 — Visibilidade e círculo interno
### Feature: Visibilidade por papel
- [MVP] **Visitor/resident** *(já desenvolvido)*
  - Critérios de aceite:
    - Conteúdo respeita visibilidade para visitor e resident.
    - Termos de aceite de papel: `VISITOR` tem acesso apenas a conteúdo público; `RESIDENT` validado acessa conteúdo restrito do território.

### Feature: Friends (círculo interno)
- [POST-MVP] **Solicitação e aceite de amizade**
- [POST-MVP] **Stories visíveis apenas para friends**

## Epic 5 — Moderação e segurança
### Feature: Reports e bloqueio
- [MVP] **Reportar post**
  - Critérios de aceite:
    - Usuário reporta um post com motivo e detalhes.
    - Reports repetidos na mesma janela são ignorados.
- [MVP] **Reportar usuário**
  - Critérios de aceite:
    - Usuário reporta um usuário com motivo e detalhes.
    - Reports repetidos na mesma janela são ignorados.
- [MVP] **Bloquear usuário**
  - Critérios de aceite:
    - Usuário bloqueado não aparece no feed pessoal/território do bloqueador.
    - Usuário bloqueado não aparece no mapa do bloqueador.

### Feature: Moderação automática (básica)
- [MVP] **Threshold de reports únicos por janela de tempo**
  - Critérios de aceite:
    - Acionamento automático para ocultar conteúdo ou restringir usuário.
    - Ação automática gera registro de auditoria.
- [POST-MVP] **Score de risco e escalonamento avançado**

### Feature: Sanções territoriais e globais
- [MVP] **Sanções por território**
  - Critérios de aceite:
    - Restrição/suspensão pode ser aplicada apenas em um território.
- [MVP] **Sanções globais**
  - Critérios de aceite:
    - Banimento global bloqueia acesso a todos os territórios.

## Epic 6 — Notificações
### Feature: Notificações in-app
- [MVP] **Inbox de notificações com outbox**
  - Critérios de aceite:
    - Eventos relevantes (post/report) geram mensagens em outbox na mesma transação.
    - Worker converte mensagens em notificações persistidas por usuário.
    - Usuário pode listar notificações e marcá-las como lidas.

## Epic 7 — Admin e observabilidade
### Feature: Visão administrativa
- [POST-MVP] **Painel admin de territórios, erros e relatórios**
- [POST-MVP] **Saúde do sistema e indicadores**

## Epic 8 — Integrações e observabilidade técnica
### Feature: Provedor de mapas
- [MVP] **Integração base com provedor de mapas**
  - Critérios de aceite:
    - Configuração permite trocar o provedor de mapas.
    - Suporte a pins/cluster básico para posts.

### Feature: Observabilidade mínima
- [MVP] **Logs e métricas essenciais**
  - Critérios de aceite:
    - Erros de geolocalização são registrados.
    - Há métricas para requisições e falhas de moderação.

## Epic 9 — Gaps e boas práticas para o modelo de rede
### Feature: Presença e privacidade
- [MVP] **Consentimento explícito de localização**
  - Critérios de aceite:
    - Usuário aprova uso de localização e entende por que é necessário.
- [MVP] **Heurística de presença física**
  - Critérios de aceite:
    - Vínculo territorial depende de sinais locais (ex.: GPS e tempo mínimo no território).

### Feature: Segurança e governança mínima
- [MVP] **Auditoria de decisões de moderação**
  - Critérios de aceite:
    - Toda ação de moderação gera registro com autor, data e justificativa.
- [MVP] **Proteção contra abuso de reports**
  - Critérios de aceite:
    - Reports duplicados são ignorados por janela de tempo.
    - Threshold considera apenas reports únicos.
