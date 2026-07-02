# API

## Endpoints Principais

### Autenticação

- `POST /api/v1/auth/register` - Registrar usuário
- `POST /api/v1/auth/login` - Login
- `POST /api/v1/auth/refresh-token` - Renovar token

### Feed

- `GET /api/v1/feed` - Listar posts
- `GET /api/v1/feed/filtered` - Feed filtrado por interesses
- `POST /api/v1/feed/posts` - Criar post
- `GET /api/v1/feed/posts/{id}` - Obter post
- `PUT /api/v1/feed/posts/{id}` - Editar post
- `DELETE /api/v1/feed/posts/{id}` - Deletar post

### Territórios

- `GET /api/v1/territories` - Listar territórios
- `GET /api/v1/territories/{id}` - Obter território
- `POST /api/v1/territories` - Criar território
- `PUT /api/v1/territories/{id}` - Atualizar território

### Governança

- `GET /api/v1/votings` - Listar votações
- `POST /api/v1/votings` - Criar votação
- `POST /api/v1/votings/{id}/vote` - Votar
- `POST /api/v1/votings/{id}/close` - Fechar votação

### Arah Core (FASE53)

- `POST /api/v1/core/instances` - Registrar instância (SystemAdmin)
- `GET /api/v1/core/instances` - Listar instâncias
- `POST /api/v1/core/instances/{id}/heartbeat` - Telemetria (header `X-Arah-Instance-Token`)
- `GET /api/v1/core/releases` - Catálogo de releases
- `POST /api/v1/core/directory/territories` - Publicar território no diretório
- `GET /api/v1/federation/identity/{globalUserId}` - Identidade federada

### Monetização / transações (FASE55 v0)

- `GET /api/v1/territories/{id}/plans` - Planos comerciais do território
- `POST /api/v1/transactions/{id}/quote` - Cotação com taxa e split (`id` = checkout)
- `GET /api/v1/transactions/{id}/receipt` - Comprovante (checkout `Paid`)
- `POST /api/v1/transactions/{id}/refund` - Estorno idempotente (reverte fee/split; AC-55-6)
- `GET /api/v1/territories/{id}/payouts/consolidated?from=&to=` - Payout consolidado por período (AC-55-5)

### Assinaturas (FASE15 — base FASE55)

- `GET /api/v1/subscription-plans` - Planos globais
- `GET /api/v1/territories/{id}/subscription-plans` - Planos do território
- `POST /api/v1/subscriptions` - Criar assinatura (mock Stripe sem secret em dev)

## Padrões de Resposta

### Sucesso (200-201)

```json
{
  "data": { /* objeto ou array */ },
  "message": "Operação realizada com sucesso"
}
```

### Erro (400-500)

```json
{
  "error": "Descrição do erro",
  "code": "ERROR_CODE",
  "details": { /* detalhes opcionais */ }
}
```

## Autenticação

Headers necessários:

```
Authorization: Bearer {token}
Content-Type: application/json
```

## Rate Limiting

- 100 requisições por minuto por usuário
- 1000 requisições por hora por IP

## Documentação Interativa

Swagger UI disponível em `/swagger/ui`
