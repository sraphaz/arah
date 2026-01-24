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
