# Moderação e Reports

## Escopo
A moderação do Araponga é **território-first**, com capacidade de sanções **territoriais** e **globais**.
O MVP implementa fluxos essenciais e automações simples baseadas em thresholds.

## Fluxos de report

### Reportar post [MVP]
1. Usuário escolhe um post e aciona “Reportar”.
2. Informa motivo e detalhes (opcional).
3. Sistema registra o report e envia para fila de moderação.

**Endpoints**
- `POST /api/v1/reports/posts/{postId}`

### Reportar usuário [MVP]
1. Usuário acessa perfil e aciona “Reportar usuário”.
2. Informa motivo e detalhes (opcional).
3. Sistema registra o report para análise.

**Endpoints**
- `POST /api/v1/reports/users/{userId}`

### Bloquear usuário [MVP]
- Usuário bloqueado deixa de aparecer nos feeds do bloqueador.
- Bloqueio não remove conteúdo globalmente; atua na experiência do bloqueador.

**Endpoints**
- `POST /api/v1/blocks/users/{userId}`

## Automações do MVP (threshold simples)
- **Regra**: número mínimo de reports únicos por janela de tempo aciona resposta automática.
- **Ações automáticas possíveis**:
  - Ocultar conteúdo do feed do território (pending review).
  - Restringir usuário no território (não pode postar).
  - Suspensão curta territorial.

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
