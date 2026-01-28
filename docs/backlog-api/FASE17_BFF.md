# Fase 17: Backend for Frontend (BFF) - Aplicação Externa com OAuth2

**Duração**: 6 semanas (30 dias úteis)  
**Prioridade**: 🟡 ALTA (Melhoria de UX e Performance)  
**Depende de**: Fase 1 (Segurança), Fase 4 (Observabilidade), Fase 6 (Marketplace), Fase 8 (Mídia)  
**Integra com**: Todas as fases anteriores (Feed, Events, Marketplace, etc.)  
**Estimativa Total**: 240 horas  
**Status**: ⏳ Pendente  

---

## 🎯 Objetivo

Implementar um **Backend for Frontend (BFF)** como aplicação externa que:

1. ✅ **Reduz chamadas de rede** (de 5-10 para 1 por jornada)
2. ✅ **Simplifica lógica no frontend** (dados já formatados para UI)
3. ✅ **Melhora experiência do usuário** (sugestões contextuais, dados agregados)
4. ✅ **Tem autenticação própria** (OAuth2 Client Credentials Flow)
5. ✅ **Registra múltiplos apps consumidores** (BFF, Mobile App, Web App, etc.)
6. ✅ **Escala independentemente** da API principal
7. ✅ **Mantém compatibilidade** com API v1 existente

---

## 📋 Contexto e Requisitos

### Estado Atual

- ✅ API principal (`Araponga.Api`) com endpoints RESTful organizados por recursos
- ✅ Arquitetura modular (módulos por funcionalidade)
- ✅ Autenticação JWT implementada
- ✅ Sistema de observabilidade (logs, métricas, tracing)
- ⚠️ Frontend precisa fazer múltiplas chamadas para completar uma jornada
- ⚠️ Lógica de agregação/composição fica no frontend
- ⚠️ Transformações de dados para UI ficam no frontend

### Problemas Identificados

#### 1. Múltiplas Chamadas para Jornadas Simples

**Exemplo: Criar Post com Mídia**
```
Frontend precisa:
1. POST /api/v1/media/upload (upload de mídia)
2. POST /api/v1/feed (criar post com mediaIds)
3. GET /api/v1/feed/{id} (buscar post criado para exibir)
```

**Com BFF**:
```
1. POST /api/v2/journeys/feed/create-post (faz tudo internamente)
```

#### 2. Lógica de Agregação no Frontend

**Exemplo: Feed do Território**
```
Frontend precisa:
1. GET /api/v1/feed (posts)
2. GET /api/v1/feed/{id}/counts (contadores para cada post)
3. GET /api/v1/media/{id} (mídias para cada post)
4. GET /api/v1/events/{id} (eventos relacionados)
5. Agregar tudo no frontend
```

**Com BFF**:
```
1. GET /api/v2/journeys/feed/territory-feed (retorna tudo agregado)
```

---

## 🏗️ Arquitetura

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

### Estratégia de Implementação

**Fase 1 (Atual)**: BFF como Módulo Interno
- ✅ Implementar BFF como módulo dentro de `Araponga.Api`
- ✅ Rotas em `/api/v2/journeys/*`
- ✅ Coexiste com API v1 existente
- ✅ Simplicidade e zero custo

**Fase 2 (Esta Fase)**: Migrar BFF para Aplicação Externa
- ✅ Extrair BFF para `Araponga.Api.Bff` (aplicação separada)
- ✅ Implementar OAuth2 Client Credentials Flow
- ✅ Sistema de registro de múltiplos apps consumidores
- ✅ BFF consome API principal via HTTP
- ✅ Escalabilidade independente

---

## 📋 Requisitos Funcionais

### 1. Sistema OAuth2

#### 1.1 Registro de Clientes

- ✅ Admin pode registrar novos clientes OAuth2
- ✅ Campos obrigatórios:
  - Nome do cliente
  - Descrição
  - Scopes (permissões)
  - Redirect URIs (opcional)
- ✅ Geração automática de `clientId` e `clientSecret`
- ✅ Hash de `clientSecret` (BCrypt/Argon2)
- ✅ Ativação/desativação de clientes

#### 1.2 Token Endpoint

- ✅ `POST /oauth/token` (Client Credentials Flow)
- ✅ Validação de `clientId` e `clientSecret`
- ✅ Validação de scopes
- ✅ Emissão de token JWT
- ✅ Expiração de tokens (1 hora)
- ✅ Rate limiting por clientId

#### 1.3 Autenticação no BFF

- ✅ Middleware de autenticação OAuth2
- ✅ Validação de tokens JWT
- ✅ Extração de clientId e scopes do token
- ✅ Verificação de permissões (scopes)

### 2. Jornadas do Usuário

#### 2.1 Onboarding

- ✅ `POST /api/v2/journeys/onboarding/complete`
- ✅ `GET /api/v2/journeys/onboarding/suggested-territories`

#### 2.2 Feed

- ✅ `GET /api/v2/journeys/feed/territory-feed`
- ✅ `POST /api/v2/journeys/feed/create-post`
- ✅ `POST /api/v2/journeys/feed/interact`

#### 2.3 Eventos

- ✅ `GET /api/v2/journeys/events/territory-events`
- ✅ `POST /api/v2/journeys/events/create-event`
- ✅ `POST /api/v2/journeys/events/participate`

#### 2.4 Marketplace

- ✅ `GET /api/v2/journeys/marketplace/search`
- ✅ `POST /api/v2/journeys/marketplace/add-to-cart`
- ✅ `POST /api/v2/journeys/marketplace/checkout`

### 3. Integração com API Principal

#### 3.1 API HTTP Client

- ✅ HTTP client para consumir API principal
- ✅ Retry policy (Polly)
- ✅ Circuit breaker
- ✅ Repasse de token do usuário
- ✅ Header `X-BFF-Client-Id` para identificação

#### 3.2 Transformação de Dados

- ✅ Response transformers (formatação para UI)
- ✅ Agregação de dados de múltiplos endpoints
- ✅ Sugestões contextuais
- ✅ Metadados para filtros

### 4. Admin e Gestão

#### 4.1 CRUD de Clientes OAuth2

- ✅ `POST /api/v1/admin/clients` (registrar)
- ✅ `GET /api/v1/admin/clients` (listar)
- ✅ `GET /api/v1/admin/clients/{id}` (obter)
- ✅ `PUT /api/v1/admin/clients/{id}` (atualizar)
- ✅ `DELETE /api/v1/admin/clients/{id}` (desativar)
- ✅ Autorização (apenas SystemAdmin)

---

## 📋 Requisitos Não Funcionais

### 1. Performance

- ✅ Latência < 500ms para jornadas principais
- ✅ Throughput: Suportar 1000+ req/s
- ✅ Cache de clientes OAuth2
- ✅ Connection pooling HTTP

### 2. Segurança

- ✅ Hash de client secrets (BCrypt/Argon2)
- ✅ Validação de tokens JWT
- ✅ Rate limiting por clientId
- ✅ Logging e auditoria
- ✅ Rotação de secrets

### 3. Observabilidade

- ✅ Logging estruturado (Serilog)
- ✅ Métricas Prometheus
- ✅ OpenTelemetry tracing
- ✅ Health checks
- ✅ Dashboards e alertas

### 4. Escalabilidade

- ✅ Escalabilidade horizontal independente
- ✅ Stateless (sem estado compartilhado)
- ✅ Load balancing ready

---

## 📋 Tarefas Detalhadas

### Semana 1: Preparação (40h)

#### 1.1 Domínio OAuth (8h)
- [ ] Criar projeto `Araponga.Domain.OAuth`
- [ ] Criar `ClientApplication` entity
- [ ] Criar `OAuthScopes` (enum/constants)
- [ ] Criar interfaces de repositório

#### 1.2 Infraestrutura OAuth (16h)
- [ ] Criar `OAuthClientRecord` (Postgres entity)
- [ ] Criar `PostgresOAuthClientRepository`
- [ ] Criar migration `AddOAuthClients`
- [ ] Aplicar migration

#### 1.3 Serviços OAuth (8h)
- [ ] Criar `ClientRegistrationService`
- [ ] Implementar geração de `clientId` e `clientSecret`
- [ ] Implementar hash de `clientSecret` (BCrypt/Argon2)
- [ ] Testes unitários (8h)

### Semana 2: OAuth2 Authorization Server (40h)

#### 2.1 Token Service BFF (8h)
- [ ] Criar `IBffTokenService`
- [ ] Implementar `BffTokenService` (JWT para clientes)
- [ ] Configurar JWT options para BFF
- [ ] Testes unitários

#### 2.2 OAuth2 Token Endpoint (16h)
- [ ] Criar `OAuthController`
- [ ] Implementar `POST /oauth/token` (client credentials)
- [ ] Validação de clientId/clientSecret
- [ ] Validação de scopes
- [ ] Testes de integração

#### 2.3 Middleware de Autenticação (8h)
- [ ] Criar `BffAuthenticationMiddleware`
- [ ] Validar token do cliente
- [ ] Adicionar clientId ao contexto
- [ ] Testes de integração

### Semana 3: API Client e Integração (40h)

#### 3.1 API HTTP Client (16h)
- [ ] Criar `ApiHttpClient`
- [ ] Implementar retry policy (Polly)
- [ ] Implementar circuit breaker
- [ ] Repasse de token do usuário
- [ ] Header `X-BFF-Client-Id`
- [ ] Testes unitários

#### 3.2 Journey Services (16h)
- [ ] Mover `FeedJourneyService` para BFF
- [ ] Mover `EventJourneyService` para BFF
- [ ] Atualizar para usar `ApiHttpClient`
- [ ] Testes de integração

#### 3.3 Journey Controllers (8h)
- [ ] Mover controllers de `Araponga.Api` para `Araponga.Api.Bff`
- [ ] Atualizar rotas para `/api/v2/journeys/*`
- [ ] Aplicar middleware de autenticação
- [ ] Testes de integração

### Semana 4: Admin e Registro de Clientes (40h)

#### 4.1 Admin Controller (24h)
- [ ] Criar `ClientRegistrationController`
- [ ] `POST /api/v1/admin/clients` (registrar)
- [ ] `GET /api/v1/admin/clients` (listar)
- [ ] `GET /api/v1/admin/clients/{id}` (obter)
- [ ] `PUT /api/v1/admin/clients/{id}` (atualizar)
- [ ] `DELETE /api/v1/admin/clients/{id}` (desativar)
- [ ] Autorização (apenas SystemAdmin)

#### 4.2 Documentação (8h)
- [ ] Documentar OAuth2 flow
- [ ] Documentar endpoints de registro
- [ ] Exemplos de uso
- [ ] Atualizar Swagger/OpenAPI

#### 4.3 Testes (8h)
- [ ] Testes de integração Admin
- [ ] Testes de segurança
- [ ] Testes de performance

### Semana 5: Deploy e Configuração (40h)

#### 5.1 Configuração (16h)
- [ ] Configurar `appsettings.json` para BFF
- [ ] Configurar connection string
- [ ] Configurar JWT options
- [ ] Configurar API principal URL
- [ ] Variáveis de ambiente
- [ ] Configurar logging (Serilog, Seq)
- [ ] Configurar métricas (Prometheus)
- [ ] Configurar OpenTelemetry

#### 5.2 Deploy (16h)
- [ ] Dockerfile para BFF
- [ ] docker-compose atualizado
- [ ] Health checks
- [ ] Configuração de ambiente (dev, staging, prod)

#### 5.3 Testes End-to-End (8h)
- [ ] Testar fluxo completo
- [ ] Testar múltiplos clientes
- [ ] Testar revogação de cliente
- [ ] Testar rate limiting

### Semana 6: Documentação e Observabilidade (40h)

#### 6.1 Atualização de Documentação (24h)
- [ ] Criar `BFF_OAUTH2_GUIDE.md`
- [ ] Criar `BFF_DEVELOPER_INTEGRATION_GUIDE.md`
- [ ] Criar `BFF_API_REFERENCE.md`
- [ ] Atualizar `BFF_API_CONTRACT.yaml`
- [ ] Criar `BFF_DEPLOYMENT_GUIDE.md`
- [ ] Atualizar documentação principal
- [ ] Adicionar exemplos de código
- [ ] Criar diagramas

#### 6.2 Configuração de Logs e Monitoramento (16h)
- [ ] Configurar Serilog no BFF
- [ ] Configurar Seq (se aplicável)
- [ ] Configurar Prometheus metrics
- [ ] Configurar OpenTelemetry tracing
- [ ] Dashboards e alertas
- [ ] Logging estruturado para auditoria

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
│   │   └── BffRequestLoggingMiddleware.cs
│   ├── HealthChecks/
│   │   └── BffHealthCheck.cs
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

## 🔐 Segurança

### 1. Armazenamento de Client Secret

- ✅ Hash com BCrypt/Argon2 (nunca armazenar em texto plano)
- ✅ Rotação de secrets (permitir regenerar secret)
- ✅ Validação de força (mínimo 32 caracteres)

### 2. Validação de Token

- ✅ Assinatura JWT (HS256 com secret forte)
- ✅ Expiração (tokens expiram em 1 hora)
- ✅ Validação de issuer/audience
- ✅ Validação de scopes

### 3. Rate Limiting

- ✅ Rate limit por clientId (prevenir abuso)
- ✅ Rate limit por IP (prevenir ataques)
- ✅ Throttling (circuit breaker)

### 4. Logging e Auditoria

- ✅ Log de registros de clientes
- ✅ Log de uso de tokens
- ✅ Log de falhas de autenticação
- ✅ Auditoria de mudanças

---

## 📊 Métricas de Sucesso

### Métricas Técnicas

- **Redução de chamadas**: 70%+ de redução
- **Latência**: < 500ms para jornadas principais
- **Throughput**: Suportar 1000+ req/s
- **Disponibilidade**: 99.9%+

### Métricas de UX

- **Tempo de carregamento**: Redução de 50%+
- **Interatividade**: Melhoria de 30%+
- **Taxa de erro**: < 1%

---

## 📚 Documentação

### Documentos a Criar/Atualizar

1. **`BFF_OAUTH2_GUIDE.md`** - Guia OAuth2 completo
2. **`BFF_DEVELOPER_INTEGRATION_GUIDE.md`** - Guia de integração
3. **`BFF_API_REFERENCE.md`** - Referência da API
4. **`BFF_DEPLOYMENT_GUIDE.md`** - Guia de deploy
5. **`BFF_API_CONTRACT.yaml`** - Contrato OpenAPI atualizado
6. **`AVALIACAO_BFF_BACKEND_FOR_FRONTEND.md`** - Atualizar com OAuth2
7. **`REAVALIACAO_BFF_MODULO_VS_APLICACAO_EXTERNA.md`** - Atualizar
8. **`PLANO_EXTRACAO_BFF_APLICACAO_EXTERNA.md`** - Referência

---

## ✅ Checklist de Implementação

### Semana 1: Preparação
- [ ] Criar projeto `Araponga.Domain.OAuth`
- [ ] Criar `ClientApplication` entity
- [ ] Criar `OAuthClientRecord` (Postgres)
- [ ] Criar `PostgresOAuthClientRepository`
- [ ] Criar migration `AddOAuthClients`
- [ ] Criar `ClientRegistrationService`
- [ ] Testes unitários

### Semana 2: OAuth2 Authorization Server
- [ ] Criar `IBffTokenService`
- [ ] Implementar `BffTokenService`
- [ ] Criar `OAuthController` (`POST /oauth/token`)
- [ ] Criar `BffAuthenticationMiddleware`
- [ ] Testes de integração

### Semana 3: API Client e Integração
- [ ] Criar `ApiHttpClient`
- [ ] Implementar retry/circuit breaker
- [ ] Mover `FeedJourneyService` para BFF
- [ ] Mover `EventJourneyController` para BFF
- [ ] Testes de integração

### Semana 4: Admin e Registro
- [ ] Criar `ClientRegistrationController`
- [ ] Implementar CRUD de clientes
- [ ] Documentação OAuth2
- [ ] Atualizar Swagger

### Semana 5: Deploy
- [ ] Configurar `appsettings.json`
- [ ] Configurar logging (Serilog, Seq)
- [ ] Configurar métricas (Prometheus)
- [ ] Configurar OpenTelemetry
- [ ] Criar Dockerfile
- [ ] Atualizar docker-compose
- [ ] Health checks
- [ ] Testes End-to-End

### Semana 6: Documentação e Observabilidade
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

## 🔄 Estratégia de Migração

### Opção 1: Coexistência (Recomendada)

- ✅ BFF e API principal coexistem
- ✅ Frontend migra gradualmente
- ✅ API principal continua funcionando
- ✅ Rollback fácil se necessário

### Opção 2: Feature Flag

- ✅ BFF é feature flag
- ✅ Pode ser desabilitado
- ✅ Teste A/B possível

---

## 📊 Referências

- **Avaliação BFF**: [`AVALIACAO_BFF_BACKEND_FOR_FRONTEND.md`](../AVALIACAO_BFF_BACKEND_FOR_FRONTEND.md)
- **Reavaliação Arquitetural**: [`REAVALIACAO_BFF_MODULO_VS_APLICACAO_EXTERNA.md`](../REAVALIACAO_BFF_MODULO_VS_APLICACAO_EXTERNA.md)
- **Plano de Extração**: [`PLANO_EXTRACAO_BFF_APLICACAO_EXTERNA.md`](../PLANO_EXTRACAO_BFF_APLICACAO_EXTERNA.md)
- **Guia de Implementação**: [`BFF_FRONTEND_IMPLEMENTATION_GUIDE.md`](../BFF_FRONTEND_IMPLEMENTATION_GUIDE.md)
- **Contrato API**: [`BFF_API_CONTRACT.yaml`](../BFF_API_CONTRACT.yaml)

---

**Última Atualização**: 2026-01-28  
**Status**: ⏳ Pendente - Pronto para Implementação
