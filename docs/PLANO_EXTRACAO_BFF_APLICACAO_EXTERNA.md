# Plano de Extração do BFF para Aplicação Externa com Autenticação Própria

**Data**: 2026-01-28  
**Status**: 📋 Plano Detalhado  
**Objetivo**: Extrair BFF de módulo interno para aplicação externa com autenticação própria e suporte a múltiplos apps consumidores (OAuth2 Client Credentials)

---

## 🎯 Objetivo

Extrair o BFF (Backend for Frontend) de módulo interno para uma **aplicação ASP.NET Core separada** que:

1. ✅ **Tem autenticação própria** (OAuth2 Client Credentials Flow)
2. ✅ **Registra múltiplos apps consumidores** (BFF, Mobile App, Web App, etc.)
3. ✅ **Consome a API principal** via HTTP com autenticação
4. ✅ **Escala independentemente** da API principal
5. ✅ **Mantém compatibilidade** com API v1 existente

---

## 📊 Arquitetura Proposta

### Visão Geral

```
┌─────────────────────────────────────────────────────────┐
│              Aplicações Cliente (Frontend)               │
│  - Flutter App (Mobile)                                 │
│  - Web App (React/Vue)                                  │
│  - Admin Dashboard                                       │
└────────────────────┬────────────────────────────────────┘
                     │
                     │ HTTP/REST + OAuth2 Bearer Token
                     │ (Client Credentials Flow)
                     │
┌────────────────────▼────────────────────────────────────┐
│         Araponga.Api.Bff (Aplicação Externa)            │
│  ┌────────────────────────────────────────────────────┐  │
│  │  OAuth2 Authorization Server                        │  │
│  │  - Client Registration                              │  │
│  │  - Token Endpoint (/oauth/token)                   │  │
│  │  - Client Credentials Flow                          │  │
│  └────────────────────────────────────────────────────┘  │
│  ┌────────────────────────────────────────────────────┐  │
│  │  Journey Controllers                               │  │
│  │  - FeedJourneyController                          │  │
│  │  - EventJourneyController                         │  │
│  │  - MarketplaceJourneyController                    │  │
│  └────────────────────────────────────────────────────┘  │
│  ┌────────────────────────────────────────────────────┐  │
│  │  Journey Services (orquestração)                    │  │
│  │  - FeedJourneyService                             │  │
│  │  - EventJourneyService                            │  │
│  └────────────────────────────────────────────────────┘  │
│  ┌────────────────────────────────────────────────────┐  │
│  │  API Client (consome API principal)                │  │
│  │  - ApiHttpClient (com retry, circuit breaker)      │  │
│  │  - Token forwarding (BFF token → API token)       │  │
│  └────────────────────────────────────────────────────┘  │
└────────────────────┬────────────────────────────────────┘
                     │
                     │ HTTP/REST + JWT Token
                     │ (Token do usuário repassado)
                     │
┌────────────────────▼────────────────────────────────────┐
│         Araponga.Api (API Principal)                    │
│  ┌────────────────────────────────────────────────────┐  │
│  │  Controllers (por recurso)                        │  │
│  │  - FeedController                                  │  │
│  │  - EventsController                                │  │
│  │  - MarketplaceController                           │  │
│  └────────────────────────────────────────────────────┘  │
│  ┌────────────────────────────────────────────────────┐  │
│  │  Application Services (lógica de negócio)          │  │
│  │  - FeedService                                     │  │
│  │  - EventsService                                  │  │
│  └────────────────────────────────────────────────────┘  │
└───────────────────────────────────────────────────────────┘
```

### Fluxo de Autenticação

#### 1. Registro de App Consumidor

```
Admin → POST /api/v1/admin/clients
{
  "name": "Flutter Mobile App",
  "description": "Aplicativo mobile Flutter",
  "scopes": ["journeys:read", "journeys:write"],
  "redirectUris": ["araponga://callback"]
}

Response:
{
  "clientId": "550e8400-e29b-41d4-a716-446655440000",
  "clientSecret": "super-secret-key-here",
  "createdAt": "2026-01-28T10:00:00Z"
}
```

#### 2. Obtenção de Token (Client Credentials)

```
App → POST /oauth/token
Content-Type: application/x-www-form-urlencoded

grant_type=client_credentials
&client_id=550e8400-e29b-41d4-a716-446655440000
&client_secret=super-secret-key-here
&scope=journeys:read journeys:write

Response:
{
  "access_token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "token_type": "Bearer",
  "expires_in": 3600,
  "scope": "journeys:read journeys:write"
}
```

#### 3. Uso do Token no BFF

```
App → GET /api/v2/journeys/feed/territory-feed?territoryId=...
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...

BFF → GET http://api-principal/api/v1/feed?territoryId=...
Authorization: Bearer <token-do-usuario-repassado>
X-BFF-Client-Id: 550e8400-e29b-41d4-a716-446655440000
```

---

## 📋 Componentes Necessários

### 1. Sistema de Registro de Clientes OAuth2

#### 1.1 Modelo de Domínio

```csharp
// Araponga.Domain.OAuth/ClientApplication.cs
public sealed class ClientApplication
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string ClientId { get; private set; }
    public string ClientSecretHash { get; private set; }
    public IReadOnlyList<string> Scopes { get; private set; }
    public IReadOnlyList<string> RedirectUris { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? LastUsedAtUtc { get; private set; }
    public Guid CreatedByUserId { get; private set; }
}
```

#### 1.2 Repositório

```csharp
// Araponga.Application.Interfaces.OAuth/IClientApplicationRepository.cs
public interface IClientApplicationRepository
{
    Task<ClientApplication?> GetByClientIdAsync(string clientId, CancellationToken cancellationToken);
    Task<ClientApplication?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<ClientApplication>> ListAsync(CancellationToken cancellationToken);
    Task AddAsync(ClientApplication client, CancellationToken cancellationToken);
    Task UpdateAsync(ClientApplication client, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
```

#### 1.3 Serviço de Registro

```csharp
// Araponga.Application.Services.OAuth/ClientRegistrationService.cs
public sealed class ClientRegistrationService
{
    public async Task<Result<(string clientId, string clientSecret)>> RegisterClientAsync(
        string name,
        string description,
        IReadOnlyList<string> scopes,
        IReadOnlyList<string> redirectUris,
        Guid createdByUserId,
        CancellationToken cancellationToken)
    {
        // Gerar clientId e clientSecret
        var clientId = Guid.NewGuid().ToString("N");
        var clientSecret = GenerateSecureSecret();
        var secretHash = HashSecret(clientSecret);
        
        var client = new ClientApplication(
            Guid.NewGuid(),
            name,
            description,
            clientId,
            secretHash,
            scopes,
            redirectUris,
            true,
            DateTime.UtcNow,
            null,
            createdByUserId);
        
        await _repository.AddAsync(client, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        
        return Result<(string, string)>.Success((clientId, clientSecret));
    }
}
```

### 2. OAuth2 Authorization Server

#### 2.1 Token Service para BFF

```csharp
// Araponga.Api.Bff/Security/IBffTokenService.cs
public interface IBffTokenService
{
    string IssueClientToken(string clientId, IReadOnlyList<string> scopes);
    (string? clientId, IReadOnlyList<string>? scopes) ParseClientToken(string token);
}
```

#### 2.2 OAuth2 Token Endpoint

```csharp
// Araponga.Api.Bff/Controllers/OAuthController.cs
[ApiController]
[Route("oauth")]
public sealed class OAuthController : ControllerBase
{
    [HttpPost("token")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<IActionResult> Token([FromForm] TokenRequest request)
    {
        if (request.GrantType != "client_credentials")
        {
            return BadRequest(new { error = "unsupported_grant_type" });
        }
        
        var client = await _clientRepository.GetByClientIdAsync(
            request.ClientId, 
            cancellationToken);
        
        if (client is null || !client.IsActive)
        {
            return Unauthorized(new { error = "invalid_client" });
        }
        
        if (!VerifyClientSecret(request.ClientSecret, client.ClientSecretHash))
        {
            return Unauthorized(new { error = "invalid_client" });
        }
        
        var token = _tokenService.IssueClientToken(client.ClientId, client.Scopes);
        
        return Ok(new
        {
            access_token = token,
            token_type = "Bearer",
            expires_in = 3600,
            scope = string.Join(" ", client.Scopes)
        });
    }
}
```

### 3. Middleware de Autenticação no BFF

```csharp
// Araponga.Api.Bff/Middleware/BffAuthenticationMiddleware.cs
public sealed class BffAuthenticationMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (!context.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new { error = "missing_authorization" });
            return;
        }
        
        var token = ExtractBearerToken(authHeader);
        var (clientId, scopes) = _tokenService.ParseClientToken(token);
        
        if (clientId is null)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new { error = "invalid_token" });
            return;
        }
        
        // Adicionar clientId ao contexto
        context.Items["BffClientId"] = clientId;
        context.Items["BffScopes"] = scopes;
        
        await next(context);
    }
}
```

### 4. API Client para Consumir API Principal

```csharp
// Araponga.Api.Bff/Clients/ApiHttpClient.cs
public sealed class ApiHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiBaseUrl;
    private readonly ILogger<ApiHttpClient> _logger;
    
    public async Task<TResponse> GetAsync<TResponse>(
        string endpoint,
        string? userToken,
        CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_apiBaseUrl}{endpoint}");
        
        // Repassar token do usuário se fornecido
        if (!string.IsNullOrWhiteSpace(userToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userToken);
        }
        
        // Adicionar identificação do BFF
        request.Headers.Add("X-BFF-Client-Id", _currentClientId);
        
        var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken);
    }
}
```

### 5. Repasse de Token do Usuário

O BFF precisa repassar o token do usuário para a API principal. Duas abordagens:

#### Opção A: Token do Usuário no Header Customizado

```
App → BFF: Authorization: Bearer <bff-client-token>
         X-User-Token: <user-token>

BFF → API: Authorization: Bearer <user-token>
         X-BFF-Client-Id: <client-id>
```

#### Opção B: Token do Usuário no Body/Query (para endpoints específicos)

```
App → BFF: Authorization: Bearer <bff-client-token>
         Body: { "userToken": "<user-token>", ... }

BFF → API: Authorization: Bearer <user-token>
         X-BFF-Client-Id: <client-id>
```

**Recomendação**: **Opção A** (header customizado) - mais simples e padrão.

---

## 🗄️ Estrutura de Banco de Dados

### Tabela: `oauth_clients`

```sql
CREATE TABLE oauth_clients (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(200) NOT NULL,
    description TEXT,
    client_id VARCHAR(100) NOT NULL UNIQUE,
    client_secret_hash VARCHAR(255) NOT NULL,
    scopes TEXT[] NOT NULL,
    redirect_uris TEXT[] NOT NULL,
    is_active BOOLEAN NOT NULL DEFAULT true,
    created_at_utc TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    last_used_at_utc TIMESTAMP WITH TIME ZONE,
    created_by_user_id UUID NOT NULL REFERENCES users(id),
    
    CONSTRAINT oauth_clients_name_not_empty CHECK (LENGTH(TRIM(name)) > 0),
    CONSTRAINT oauth_clients_client_id_not_empty CHECK (LENGTH(TRIM(client_id)) > 0)
);

CREATE INDEX idx_oauth_clients_client_id ON oauth_clients(client_id);
CREATE INDEX idx_oauth_clients_created_by_user_id ON oauth_clients(created_by_user_id);
CREATE INDEX idx_oauth_clients_is_active ON oauth_clients(is_active);
```

### Migração

```csharp
// Araponga.Infrastructure.Postgres/Migrations/XXXXXX_AddOAuthClients.cs
public partial class AddOAuthClients : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "oauth_clients",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                Description = table.Column<string>(type: "text", nullable: true),
                ClientId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                ClientSecretHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                Scopes = table.Column<string[]>(type: "text[]", nullable: false),
                RedirectUris = table.Column<string[]>(type: "text[]", nullable: false),
                IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                LastUsedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_oauth_clients", x => x.Id);
                table.ForeignKey(
                    name: "FK_oauth_clients_users_CreatedByUserId",
                    column: x => x.CreatedByUserId,
                    principalTable: "users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.UniqueConstraint("AK_oauth_clients_ClientId", x => x.ClientId);
            });

        migrationBuilder.CreateIndex(
            name: "IX_oauth_clients_ClientId",
            table: "oauth_clients",
            column: "ClientId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_oauth_clients_CreatedByUserId",
            table: "oauth_clients",
            column: "CreatedByUserId");

        migrationBuilder.CreateIndex(
            name: "IX_oauth_clients_IsActive",
            table: "oauth_clients",
            column: "IsActive");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "oauth_clients");
    }
}
```

---

## 📁 Estrutura de Projetos

```
backend/
├── Araponga.Api.Bff/                    # Nova aplicação BFF
│   ├── Controllers/
│   │   ├── OAuthController.cs           # OAuth2 token endpoint
│   │   ├── Journeys/
│   │   │   ├── FeedJourneyController.cs
│   │   │   └── EventJourneyController.cs
│   │   └── Admin/
│   │       └── ClientRegistrationController.cs
│   ├── Services/
│   │   ├── Journeys/
│   │   │   ├── FeedJourneyService.cs
│   │   │   └── EventJourneyService.cs
│   │   └── OAuth/
│   │       └── ClientRegistrationService.cs
│   ├── Clients/
│   │   └── ApiHttpClient.cs             # HTTP client para API principal
│   ├── Security/
│   │   ├── IBffTokenService.cs
│   │   ├── BffTokenService.cs
│   │   └── BffAuthenticationMiddleware.cs
│   ├── Middleware/
│   │   └── BffAuthenticationMiddleware.cs
│   ├── Program.cs
│   └── Araponga.Api.Bff.csproj
│
├── Araponga.Domain.OAuth/               # Novo domínio OAuth
│   ├── ClientApplication.cs
│   └── OAuthScopes.cs
│
├── Araponga.Application/                 # Atualizar
│   └── Services/
│       └── OAuth/
│           └── ClientRegistrationService.cs
│
└── Araponga.Infrastructure/              # Atualizar
    └── Postgres/
        ├── Entities/
        │   └── OAuthClientRecord.cs
        ├── Repositories/
        │   └── PostgresOAuthClientRepository.cs
        └── Migrations/
            └── XXXXXX_AddOAuthClients.cs
```

---

## 🔧 Implementação Passo a Passo

### Fase 1: Preparação (1 semana)

#### 1.1 Criar Domínio OAuth (2 dias)

- [ ] Criar projeto `Araponga.Domain.OAuth`
- [ ] Criar `ClientApplication` entity
- [ ] Criar `OAuthScopes` (enum/constants)
- [ ] Criar interfaces de repositório

#### 1.2 Criar Infraestrutura OAuth (2 dias)

- [ ] Criar `OAuthClientRecord` (Postgres entity)
- [ ] Criar `PostgresOAuthClientRepository`
- [ ] Criar migration `AddOAuthClients`
- [ ] Aplicar migration

#### 1.3 Criar Serviços OAuth (1 dia)

- [ ] Criar `ClientRegistrationService`
- [ ] Implementar geração de `clientId` e `clientSecret`
- [ ] Implementar hash de `clientSecret` (BCrypt/Argon2)
- [ ] Testes unitários

### Fase 2: OAuth2 Authorization Server (1 semana)

#### 2.1 Token Service BFF (2 dias)

- [ ] Criar `IBffTokenService`
- [ ] Implementar `BffTokenService` (JWT para clientes)
- [ ] Configurar JWT options para BFF
- [ ] Testes unitários

#### 2.2 OAuth2 Token Endpoint (2 dias)

- [ ] Criar `OAuthController`
- [ ] Implementar `POST /oauth/token` (client credentials)
- [ ] Validação de clientId/clientSecret
- [ ] Validação de scopes
- [ ] Testes de integração

#### 2.3 Middleware de Autenticação (1 dia)

- [ ] Criar `BffAuthenticationMiddleware`
- [ ] Validar token do cliente
- [ ] Adicionar clientId ao contexto
- [ ] Testes de integração

### Fase 3: API Client e Integração (1 semana)

#### 3.1 API HTTP Client (2 dias)

- [ ] Criar `ApiHttpClient`
- [ ] Implementar retry policy (Polly)
- [ ] Implementar circuit breaker
- [ ] Repasse de token do usuário
- [ ] Header `X-BFF-Client-Id`
- [ ] Testes unitários

#### 3.2 Journey Services (2 dias)

- [ ] Mover `FeedJourneyService` para BFF
- [ ] Mover `EventJourneyService` para BFF
- [ ] Atualizar para usar `ApiHttpClient`
- [ ] Testes de integração

#### 3.3 Journey Controllers (1 dia)

- [ ] Mover controllers de `Araponga.Api` para `Araponga.Api.Bff`
- [ ] Atualizar rotas para `/api/v2/journeys/*`
- [ ] Aplicar middleware de autenticação
- [ ] Testes de integração

### Fase 4: Admin e Registro de Clientes (1 semana)

#### 4.1 Admin Controller (2 dias)

- [ ] Criar `ClientRegistrationController`
- [ ] `POST /api/v1/admin/clients` (registrar)
- [ ] `GET /api/v1/admin/clients` (listar)
- [ ] `GET /api/v1/admin/clients/{id}` (obter)
- [ ] `PUT /api/v1/admin/clients/{id}` (atualizar)
- [ ] `DELETE /api/v1/admin/clients/{id}` (desativar)
- [ ] Autorização (apenas SystemAdmin)

#### 4.2 Documentação (1 dia)

- [ ] Documentar OAuth2 flow
- [ ] Documentar endpoints de registro
- [ ] Exemplos de uso
- [ ] Atualizar Swagger/OpenAPI

### Fase 5: Deploy e Configuração (1 semana)

#### 5.1 Configuração (2 dias)

- [ ] Configurar `appsettings.json` para BFF
- [ ] Configurar connection string (compartilhado ou separado)
- [ ] Configurar JWT options
- [ ] Configurar API principal URL
- [ ] Variáveis de ambiente
- [ ] Configurar logging (Serilog, Seq)
- [ ] Configurar métricas (Prometheus, OpenTelemetry)
- [ ] Configurar health checks

#### 5.2 Deploy (2 dias)

- [ ] Dockerfile para BFF
- [ ] docker-compose atualizado
- [ ] Health checks
- [ ] Logging e monitoring
- [ ] Configuração de ambiente (dev, staging, prod)

#### 5.3 Testes End-to-End (1 dia)

- [ ] Testar fluxo completo
- [ ] Testar múltiplos clientes
- [ ] Testar revogação de cliente
- [ ] Testar rate limiting

### Fase 6: Documentação e Observabilidade (1 semana)

#### 6.1 Atualização de Documentação (3 dias)

- [ ] Documentar OAuth2 flow completo
- [ ] Documentar endpoints de registro de clientes
- [ ] Atualizar Swagger/OpenAPI
- [ ] Documentar configuração e deploy
- [ ] Guias de integração para desenvolvedores
- [ ] Exemplos de código
- [ ] Troubleshooting guide

#### 6.2 Configuração de Logs e Monitoramento (2 dias)

- [ ] Configurar Serilog no BFF
- [ ] Configurar Seq (se aplicável)
- [ ] Configurar Prometheus metrics
- [ ] Configurar OpenTelemetry tracing
- [ ] Dashboards e alertas
- [ ] Logging estruturado para auditoria

---

## ⏱️ Estimativa de Esforço

### Resumo por Fase

| Fase | Descrição | Duração | Esforço (horas) |
|------|-----------|---------|-----------------|
| **Fase 1** | Preparação (Domínio, Infra, Serviços) | 1 semana | 40h |
| **Fase 2** | OAuth2 Authorization Server | 1 semana | 40h |
| **Fase 3** | API Client e Integração | 1 semana | 40h |
| **Fase 4** | Admin e Registro de Clientes | 1 semana | 40h |
| **Fase 5** | Deploy e Configuração | 1 semana | 40h |
| **Fase 6** | Documentação e Observabilidade | 1 semana | 40h |
| **TOTAL** | | **6 semanas** | **240h** |

### Detalhamento por Tarefa

#### Fase 1: Preparação (40h)

- Domínio OAuth: 8h
- Infraestrutura OAuth: 16h
- Serviços OAuth: 8h
- Testes: 8h

#### Fase 2: OAuth2 Authorization Server (40h)

- Token Service BFF: 8h
- OAuth2 Token Endpoint: 16h
- Middleware de Autenticação: 8h
- Testes: 8h

#### Fase 3: API Client e Integração (40h)

- API HTTP Client: 16h
- Journey Services: 16h
- Journey Controllers: 8h

#### Fase 4: Admin e Registro de Clientes (40h)

- Admin Controller: 24h
- Documentação: 8h
- Testes: 8h

#### Fase 5: Deploy e Configuração (40h)

- Configuração: 16h
- Deploy: 16h
- Testes End-to-End: 8h

#### Fase 6: Documentação e Observabilidade (40h)

- Atualização de Documentação: 24h
- Configuração de Logs e Monitoramento: 16h

---

## 🔐 Segurança

### 1. Armazenamento de Client Secret

- ✅ **Hash com BCrypt/Argon2** (nunca armazenar em texto plano)
- ✅ **Rotação de secrets** (permitir regenerar secret)
- ✅ **Validação de força** (mínimo 32 caracteres)

### 2. Validação de Token

- ✅ **Assinatura JWT** (HS256 com secret forte)
- ✅ **Expiração** (tokens expiram em 1 hora)
- ✅ **Validação de issuer/audience**
- ✅ **Validação de scopes**

### 3. Rate Limiting

- ✅ **Rate limit por clientId** (prevenir abuso)
- ✅ **Rate limit por IP** (prevenir ataques)
- ✅ **Throttling** (circuit breaker)

### 4. Logging e Auditoria

- ✅ **Log de registros de clientes**
- ✅ **Log de uso de tokens**
- ✅ **Log de falhas de autenticação**
- ✅ **Auditoria de mudanças**

---

## 📊 Considerações de Performance

### 1. Cache

- ✅ **Cache de clientes** (Redis/MemoryCache)
- ✅ **Cache de tokens** (TTL curto)
- ✅ **Invalidação de cache** (quando cliente desativado)

### 2. Connection Pooling

- ✅ **HTTP Client pooling** (reutilizar conexões)
- ✅ **Connection timeout** (configurável)
- ✅ **Retry policy** (Polly)

### 3. Monitoring

- ✅ **Métricas de latência** (BFF → API)
- ✅ **Métricas de erro** (taxa de falha)
- ✅ **Métricas de uso** (tokens emitidos, clientes ativos)

---

## ✅ Checklist de Implementação

### Fase 1: Preparação
- [ ] Criar projeto `Araponga.Domain.OAuth`
- [ ] Criar `ClientApplication` entity
- [ ] Criar `OAuthClientRecord` (Postgres)
- [ ] Criar `PostgresOAuthClientRepository`
- [ ] Criar migration `AddOAuthClients`
- [ ] Criar `ClientRegistrationService`
- [ ] Testes unitários

### Fase 2: OAuth2 Authorization Server
- [ ] Criar `IBffTokenService`
- [ ] Implementar `BffTokenService`
- [ ] Criar `OAuthController` (`POST /oauth/token`)
- [ ] Criar `BffAuthenticationMiddleware`
- [ ] Testes de integração

### Fase 3: API Client e Integração
- [ ] Criar `ApiHttpClient`
- [ ] Implementar retry/circuit breaker
- [ ] Mover `FeedJourneyService` para BFF
- [ ] Mover `EventJourneyController` para BFF
- [ ] Testes de integração

### Fase 4: Admin e Registro
- [ ] Criar `ClientRegistrationController`
- [ ] Implementar CRUD de clientes
- [ ] Documentação OAuth2
- [ ] Atualizar Swagger

### Fase 5: Deploy
- [ ] Configurar `appsettings.json`
- [ ] Configurar logging (Serilog, Seq)
- [ ] Configurar métricas (Prometheus)
- [ ] Configurar OpenTelemetry
- [ ] Criar Dockerfile
- [ ] Atualizar docker-compose
- [ ] Health checks
- [ ] Testes End-to-End

### Fase 6: Documentação e Observabilidade
- [ ] Criar `BFF_OAUTH2_GUIDE.md`
- [ ] Criar `BFF_DEVELOPER_INTEGRATION_GUIDE.md`
- [ ] Criar `BFF_API_REFERENCE.md`
- [ ] Atualizar `BFF_API_CONTRACT.yaml`
- [ ] Criar `BFF_DEPLOYMENT_GUIDE.md`
- [ ] Atualizar documentação principal
- [ ] Configurar Serilog no BFF
- [ ] Implementar logging estruturado
- [ ] Configurar métricas Prometheus
- [ ] Configurar OpenTelemetry tracing
- [ ] Criar dashboards e alertas

---

## 📚 Documentação Adicional

### Endpoints OAuth2

#### `POST /oauth/token`

**Request**:
```
Content-Type: application/x-www-form-urlencoded

grant_type=client_credentials
&client_id=<client-id>
&client_secret=<client-secret>
&scope=journeys:read journeys:write
```

**Response**:
```json
{
  "access_token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "token_type": "Bearer",
  "expires_in": 3600,
  "scope": "journeys:read journeys:write"
}
```

### Endpoints Admin

#### `POST /api/v1/admin/clients`

**Request**:
```json
{
  "name": "Flutter Mobile App",
  "description": "Aplicativo mobile Flutter",
  "scopes": ["journeys:read", "journeys:write"],
  "redirectUris": ["araponga://callback"]
}
```

**Response**:
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "clientId": "550e8400e29b41d4a716446655440000",
  "clientSecret": "super-secret-key-here",
  "name": "Flutter Mobile App",
  "description": "Aplicativo mobile Flutter",
  "scopes": ["journeys:read", "journeys:write"],
  "redirectUris": ["araponga://callback"],
  "isActive": true,
  "createdAtUtc": "2026-01-28T10:00:00Z"
}
```

**⚠️ IMPORTANTE**: O `clientSecret` só é retornado uma vez no momento do registro. Guarde-o com segurança!

---

## 📚 Atualização de Documentação

### Documentos a Criar/Atualizar

#### 1. Documentação OAuth2

**Arquivo**: `docs/BFF_OAUTH2_GUIDE.md`

**Conteúdo**:
- Visão geral do OAuth2 Client Credentials Flow
- Como registrar um novo cliente
- Como obter token de acesso
- Como usar token no BFF
- Exemplos de código (C#, JavaScript, Flutter)
- Troubleshooting comum

#### 2. Guia de Integração para Desenvolvedores

**Arquivo**: `docs/BFF_DEVELOPER_INTEGRATION_GUIDE.md`

**Conteúdo**:
- Passo a passo para integrar app com BFF
- Fluxo de autenticação completo
- Exemplos de requisições
- Tratamento de erros
- Best practices
- Rate limiting e quotas

#### 3. Documentação de API BFF

**Arquivo**: `docs/BFF_API_REFERENCE.md`

**Conteúdo**:
- Endpoints OAuth2 (`/oauth/token`)
- Endpoints de Journeys (`/api/v2/journeys/*`)
- Endpoints Admin (`/api/v1/admin/clients/*`)
- Schemas de request/response
- Códigos de status
- Exemplos de uso

#### 4. Atualizar Swagger/OpenAPI

**Arquivo**: `docs/BFF_API_CONTRACT.yaml` (atualizar)

**Conteúdo**:
- Adicionar seção OAuth2
- Adicionar security schemes
- Documentar endpoints de registro
- Exemplos de autenticação

#### 5. Guia de Configuração e Deploy

**Arquivo**: `docs/BFF_DEPLOYMENT_GUIDE.md`

**Conteúdo**:
- Requisitos de infraestrutura
- Configuração de variáveis de ambiente
- Docker e docker-compose
- Health checks
- Logging e monitoramento
- Troubleshooting de deploy

#### 6. Atualizar Documentação Principal

**Arquivos a atualizar**:
- `docs/AVALIACAO_BFF_BACKEND_FOR_FRONTEND.md` - Adicionar referência ao OAuth2
- `docs/REAVALIACAO_BFF_MODULO_VS_APLICACAO_EXTERNA.md` - Atualizar com detalhes de autenticação
- `docs/00_INDEX.md` - Adicionar links para nova documentação
- `README.md` - Atualizar seção de arquitetura

### Checklist de Documentação

- [ ] Criar `BFF_OAUTH2_GUIDE.md`
- [ ] Criar `BFF_DEVELOPER_INTEGRATION_GUIDE.md`
- [ ] Criar `BFF_API_REFERENCE.md`
- [ ] Atualizar `BFF_API_CONTRACT.yaml`
- [ ] Criar `BFF_DEPLOYMENT_GUIDE.md`
- [ ] Atualizar `AVALIACAO_BFF_BACKEND_FOR_FRONTEND.md`
- [ ] Atualizar `REAVALIACAO_BFF_MODULO_VS_APLICACAO_EXTERNA.md`
- [ ] Atualizar `00_INDEX.md`
- [ ] Atualizar `README.md`
- [ ] Adicionar exemplos de código em múltiplas linguagens
- [ ] Criar diagramas de sequência
- [ ] Criar diagramas de arquitetura

---

## 📊 Configuração de Logs e Observabilidade

### 1. Configuração de Logging (Serilog)

#### 1.1 appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Araponga.Api.Bff": "Information"
    },
    "Seq": {
      "ServerUrl": "http://localhost:5341",
      "ApiKey": ""
    }
  },
  "Serilog": {
    "Using": [ "Serilog.Sinks.Console", "Serilog.Sinks.File", "Serilog.Sinks.Seq" ],
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning",
        "Araponga.Api.Bff.Security": "Debug"
      }
    },
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{CorrelationId}] [{BffClientId}] {Message:lj}{NewLine}{Exception}"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/bff-.log",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 30,
          "outputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{CorrelationId}] [{BffClientId}] [{MachineName}] [{ThreadId}] {Message:lj}{NewLine}{Exception}"
        }
      }
    ],
    "Enrich": [ "FromLogContext", "WithMachineName", "WithThreadId", "WithEnvironmentName" ],
    "Properties": {
      "Application": "Araponga.Bff",
      "Version": "1.0.0"
    }
  }
}
```

#### 1.2 Program.cs - Configuração Serilog

```csharp
// Araponga.Api.Bff/Program.cs
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Serilog Configuration
builder.Host.UseSerilog((context, configuration) =>
{
    var seqUrl = context.Configuration["Logging:Seq:ServerUrl"];
    var logLevel = context.Configuration["Logging:LogLevel:Default"] ?? "Information";
    var minLevel = Enum.TryParse<Serilog.Events.LogEventLevel>(logLevel, true, out var level) 
        ? level 
        : Serilog.Events.LogEventLevel.Information;

    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithThreadId()
        .Enrich.WithEnvironmentName()
        .Enrich.WithProperty("Application", "Araponga.Bff")
        .Enrich.WithProperty("Version", Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown")
        .MinimumLevel.Is(minLevel)
        .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
        .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
        .WriteTo.Console(
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{CorrelationId}] [{BffClientId}] {Message:lj}{NewLine}{Exception}")
        .WriteTo.File(
            "logs/bff-.log",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 30,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{CorrelationId}] [{BffClientId}] [{MachineName}] [{ThreadId}] {Message:lj}{NewLine}{Exception}");

    // Add Seq sink if configured
    if (!string.IsNullOrWhiteSpace(seqUrl))
    {
        configuration.WriteTo.Seq(
            serverUrl: seqUrl,
            apiKey: context.Configuration["Logging:Seq:ApiKey"],
            restrictedToMinimumLevel: minLevel);
    }
});
```

#### 1.3 Middleware de Logging com Enriquecimento

```csharp
// Araponga.Api.Bff/Middleware/BffRequestLoggingMiddleware.cs
using Serilog.Context;
using System.Diagnostics;

namespace Araponga.Api.Bff.Middleware;

public sealed class BffRequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<BffRequestLoggingMiddleware> _logger;

    public BffRequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<BffRequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var method = context.Request.Method;
        var path = context.Request.Path.Value ?? "/";
        var correlationId = context.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString();
        var clientId = context.Items["BffClientId"]?.ToString() ?? "anonymous";

        // Enriquecer contexto de log
        using (LogContext.PushProperty("CorrelationId", correlationId))
        using (LogContext.PushProperty("BffClientId", clientId))
        using (LogContext.PushProperty("RequestMethod", method))
        using (LogContext.PushProperty("RequestPath", path))
        {
            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();
                var statusCode = context.Response.StatusCode;

                _logger.LogInformation(
                    "Request: {Method} {Path} {StatusCode} {DurationMs}ms CorrelationId: {CorrelationId} ClientId: {BffClientId}",
                    method, path, statusCode, stopwatch.ElapsedMilliseconds, correlationId, clientId);
            }
        }
    }
}
```

### 2. Logging Estruturado para Auditoria

#### 2.1 Eventos de Auditoria OAuth2

```csharp
// Araponga.Api.Bff/Services/OAuth/AuditLogService.cs
public sealed class AuditLogService
{
    private readonly ILogger<AuditLogService> _logger;

    public void LogClientRegistration(
        string clientId,
        string clientName,
        Guid createdByUserId,
        IReadOnlyList<string> scopes)
    {
        _logger.LogInformation(
            "OAuth2 Client Registered: ClientId={ClientId}, Name={ClientName}, CreatedBy={CreatedByUserId}, Scopes={Scopes}",
            clientId, clientName, createdByUserId, string.Join(",", scopes));
    }

    public void LogTokenIssued(
        string clientId,
        IReadOnlyList<string> scopes,
        DateTime expiresAt)
    {
        _logger.LogInformation(
            "OAuth2 Token Issued: ClientId={ClientId}, Scopes={Scopes}, ExpiresAt={ExpiresAt}",
            clientId, string.Join(",", scopes), expiresAt);
    }

    public void LogTokenValidationFailed(string clientId, string reason)
    {
        _logger.LogWarning(
            "OAuth2 Token Validation Failed: ClientId={ClientId}, Reason={Reason}",
            clientId, reason);
    }

    public void LogClientDeactivated(string clientId, Guid deactivatedByUserId)
    {
        _logger.LogWarning(
            "OAuth2 Client Deactivated: ClientId={ClientId}, DeactivatedBy={DeactivatedByUserId}",
            clientId, deactivatedByUserId);
    }
}
```

### 3. Métricas (Prometheus)

#### 3.1 Configuração Prometheus

```csharp
// Araponga.Api.Bff/Program.cs
using Prometheus;

// Métricas customizadas
var bffTokenIssuedCounter = Metrics.CreateCounter(
    "bff_oauth_tokens_issued_total",
    "Total number of OAuth2 tokens issued",
    new[] { "client_id", "scope" });

var bffRequestDuration = Metrics.CreateHistogram(
    "bff_request_duration_seconds",
    "Duration of BFF requests",
    new[] { "method", "endpoint", "status_code" });

var bffApiClientDuration = Metrics.CreateHistogram(
    "bff_api_client_duration_seconds",
    "Duration of API client requests to main API",
    new[] { "endpoint", "status_code" });

var bffApiClientErrors = Metrics.CreateCounter(
    "bff_api_client_errors_total",
    "Total number of API client errors",
    new[] { "endpoint", "error_type" });

// Registrar métricas no middleware
app.UseMetricServer(); // Endpoint /metrics
app.UseHttpMetrics(); // Métricas HTTP automáticas
```

#### 3.2 Instrumentação de Código

```csharp
// Araponga.Api.Bff/Services/OAuth/BffTokenService.cs
public sealed class BffTokenService : IBffTokenService
{
    private readonly ILogger<BffTokenService> _logger;
    private readonly ICounter _tokenIssuedCounter;

    public string IssueClientToken(string clientId, IReadOnlyList<string> scopes)
    {
        var token = /* ... gerar token ... */;
        
        // Registrar métrica
        foreach (var scope in scopes)
        {
            _tokenIssuedCounter.WithLabels(clientId, scope).Inc();
        }
        
        _logger.LogDebug("Token issued for client {ClientId} with scopes {Scopes}", clientId, string.Join(",", scopes));
        
        return token;
    }
}
```

### 4. OpenTelemetry Tracing

#### 4.1 Configuração OpenTelemetry

```csharp
// Araponga.Api.Bff/Program.cs
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using OpenTelemetry.Metrics;

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService(
            serviceName: "Araponga.Bff",
            serviceVersion: "1.0.0"))
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddSource("Araponga.Bff")
        .AddOtlpExporter(options =>
        {
            options.Endpoint = new Uri(builder.Configuration["OpenTelemetry:Otlp:Endpoint"] ?? "");
        }))
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddPrometheusExporter());
```

### 5. Health Checks

#### 5.1 Health Checks Customizados

```csharp
// Araponga.Api.Bff/HealthChecks/BffHealthCheck.cs
public sealed class BffHealthCheck : IHealthCheck
{
    private readonly IClientApplicationRepository _clientRepository;
    private readonly ApiHttpClient _apiClient;
    private readonly ILogger<BffHealthCheck> _logger;

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Verificar acesso ao banco de dados
            var clients = await _clientRepository.ListAsync(cancellationToken);
            
            // Verificar conectividade com API principal
            var apiHealth = await _apiClient.GetHealthAsync(cancellationToken);
            
            if (!apiHealth.IsHealthy)
            {
                return HealthCheckResult.Degraded(
                    "BFF is running but API principal is unavailable",
                    data: new Dictionary<string, object>
                    {
                        ["api_principal_status"] = apiHealth.Status
                    });
            }
            
            return HealthCheckResult.Healthy(
                "BFF is healthy",
                data: new Dictionary<string, object>
                {
                    ["registered_clients"] = clients.Count,
                    ["api_principal_status"] = apiHealth.Status
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            return HealthCheckResult.Unhealthy("BFF health check failed", ex);
        }
    }
}

// Registrar health checks
builder.Services.AddHealthChecks()
    .AddCheck<BffHealthCheck>("bff")
    .AddCheck<DatabaseHealthCheck>("database")
    .AddUrlGroup(new Uri(builder.Configuration["ApiPrincipal:BaseUrl"] + "/health"), "api_principal");
```

### 6. Configuração de Ambiente

#### 6.1 appsettings.Development.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Araponga.Api.Bff": "Debug"
    },
    "Seq": {
      "ServerUrl": "http://localhost:5341"
    }
  },
  "ApiPrincipal": {
    "BaseUrl": "http://localhost:5000",
    "TimeoutSeconds": 30
  },
  "Bff": {
    "Jwt": {
      "Issuer": "Araponga.Bff",
      "Audience": "Araponga.Bff",
      "SigningKey": "dev-only-change-me-in-production-min-32-chars",
      "ExpirationMinutes": 60
    }
  }
}
```

#### 6.2 appsettings.Production.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Araponga.Api.Bff": "Information"
    },
    "Seq": {
      "ServerUrl": "${SEQ_SERVER_URL}",
      "ApiKey": "${SEQ_API_KEY}"
    }
  },
  "ApiPrincipal": {
    "BaseUrl": "${API_PRINCIPAL_BASE_URL}",
    "TimeoutSeconds": 30
  },
  "Bff": {
    "Jwt": {
      "Issuer": "Araponga.Bff",
      "Audience": "Araponga.Bff",
      "SigningKey": "${BFF_JWT_SIGNING_KEY}",
      "ExpirationMinutes": 60
    }
  },
  "OpenTelemetry": {
    "Otlp": {
      "Endpoint": "${OTEL_EXPORTER_OTLP_ENDPOINT}"
    }
  }
}
```

### 7. Dashboards e Alertas

#### 7.1 Métricas Principais

- **Taxa de tokens emitidos** (`bff_oauth_tokens_issued_total`)
- **Latência de requisições BFF** (`bff_request_duration_seconds`)
- **Latência de chamadas à API principal** (`bff_api_client_duration_seconds`)
- **Taxa de erros** (`bff_api_client_errors_total`)
- **Clientes ativos** (query no banco)

#### 7.2 Alertas Recomendados

- **Alta taxa de erros**: > 5% de requisições falhando
- **Latência alta**: P95 > 1s
- **API principal indisponível**: Health check falhando
- **Muitos tokens inválidos**: > 10% de tokens rejeitados
- **Cliente desativado tentando usar**: Tentativas de uso após desativação

---

## 🎯 Conclusão

Este plano detalha a extração do BFF para uma aplicação externa com:

1. ✅ **Autenticação própria** (OAuth2 Client Credentials)
2. ✅ **Registro de múltiplos apps** (sistema de clientes OAuth2)
3. ✅ **Consumo da API principal** (HTTP com repasse de token)
4. ✅ **Escalabilidade independente** (aplicação separada)
5. ✅ **Segurança** (hash de secrets, validação de tokens, rate limiting)
6. ✅ **Observabilidade completa** (logs estruturados, métricas, tracing)
7. ✅ **Documentação abrangente** (guias, referências, exemplos)

**Esforço Total**: **6 semanas (240 horas)**

**Próximos Passos**:
1. Aprovar plano
2. Iniciar Fase 1 (Preparação)
3. Implementar em sprints de 1 semana
4. Testar incrementalmente
5. Documentar durante implementação

---

**Última Atualização**: 2026-01-28  
**Status**: 📋 Plano Completo - Pronto para Implementação
