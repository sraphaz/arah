# Moderação e Reports

## Escopo
A moderação do Araponga é **território-first**, com capacidade de sanções **territoriais** e **globais**.
O MVP implementa fluxos essenciais e automações simples baseadas em thresholds.

## Fluxos de report

### Reportar post [MVP]
1. Usuário escolhe um post e aciona “Reportar”.
2. Informa motivo e detalhes (opcional).
3. Sistema registra o report e envia para fila de moderação.

**Fila (P2 inicial)**:
- Ao criar um report, o sistema também cria um **Work Item** do tipo `MODERATIONCASE` (fallback humano).
- Curadores podem listar via `GET /api/v1/territories/{territoryId}/work-items?type=MODERATIONCASE`.
- Decisão humana (Curator/Moderator): `POST /api/v1/territories/{territoryId}/moderation/cases/{workItemId}/decide`

**Endpoints**
- `POST /api/v1/reports/posts/{postId}`

### Reportar usuário [MVP]
1. Usuário acessa perfil e aciona “Reportar usuário”.
2. Informa motivo e detalhes (opcional).
3. Sistema registra o report para análise.

**Fila (P2 inicial)**:
- Também cria Work Item `MODERATIONCASE` para o território.

**Endpoints**
- `POST /api/v1/reports/users/{userId}?territoryId=`

### Bloquear usuário [MVP]
- Usuário bloqueado deixa de aparecer nos feeds do bloqueador.
- Bloqueio não remove conteúdo globalmente; atua na experiência do bloqueador.

**Endpoints**
- `POST /api/v1/blocks/users/{userId}`
- `DELETE /api/v1/blocks/users/{userId}` (idempotente)

### Listar reports (curadoria) [MVP]
**Endpoints**
- `GET /api/v1/reports?territoryId=&targetType=&status=&from=&to=`

**Retorna**
- `id`, `territoryId`, `targetType`, `targetId`, `reason`, `status`, `createdAt`.

## Automações do MVP (threshold simples)
- **Regra**: número mínimo de reports **únicos** por janela de tempo aciona resposta automática.
  - Janela: **7 dias**
  - Threshold: **3 reports únicos**
- **Ações automáticas**:
  - **Post**: status alterado para `HIDDEN` (oculto do feed/pins).
  - **Usuário**: sanção territorial `POSTING_RESTRICTION` por 7 dias.
- **Auditoria**: toda ação automática gera registro (`moderation.threshold.*`).

## Sanções: territorial vs global
- **Territorial**: aplica-se a um território específico (territoryId preenchido).
  - Ex.: restrição temporária para postar naquele território.
- **Global**: aplica-se a todos os territórios (territoryId nulo).
  - Ex.: banimento global do usuário.

## Painel mínimo de moderação (MVP)
- Lista de reports com status.
- Visão de usuário/post reportado.
- Ações: ocultar post, restringir usuário, suspender territorialmente.
- Histórico mínimo de decisões.

## Evolução pós-MVP
- [POST-MVP] Score de risco por usuário/post.
- [POST-MVP] Escalonamento avançado e revisão humana contextual.

## Infra de fila (P0)
Foi adicionada uma **Work Queue** genérica para suportar fluxos com **fallback humano** (curadoria/moderação/verificações).
Ainda não está totalmente integrada aos reports, mas já existe para operacionalizar revisões.

- Detalhes e endpoints: `docs/33_ADMIN_SYSTEM_CONFIG_WORKQUEUE.md`
