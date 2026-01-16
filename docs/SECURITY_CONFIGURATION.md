# Configuração de Segurança - Araponga

**Última atualização**: 2025-01-15  
**Versão**: 1.1

---

## 📋 Índice

1. [Variáveis de Ambiente Obrigatórias](#variáveis-de-ambiente-obrigatórias)
2. [Configuração de JWT](#configuração-de-jwt)
3. [Configuração de Rate Limiting](#configuração-de-rate-limiting)
4. [Configuração de CORS](#configuração-de-cors)
5. [Configuração de HTTPS](#configuração-de-https)
6. [Checklist de Deploy](#checklist-de-deploy)

---

## 🔐 Variáveis de Ambiente Obrigatórias

### Produção (OBRIGATÓRIAS)

```bash
# JWT Secret - OBRIGATÓRIO
# Mínimo: 32 caracteres
# Gere um secret forte usando: openssl rand -base64 32
JWT__SIGNINGKEY=seu-secret-forte-aqui-minimo-32-caracteres-aleatorios

# CORS Origins - OBRIGATÓRIO
# Não pode ser wildcard (*) em produção
Cors__AllowedOrigins__0=https://app.araponga.com
Cors__AllowedOrigins__1=https://www.araponga.com
```

### Desenvolvimento

```bash
# JWT Secret (pode usar valor padrão em dev)
JWT__SIGNINGKEY=dev-only-change-me

# CORS (pode usar wildcard em dev)
# Configurado em appsettings.json
```

---

## 🔑 Configuração de JWT

### Gerar Secret Forte

**Linux/Mac:**
```bash
openssl rand -base64 32
```

**PowerShell (Windows):**
```powershell
[Convert]::ToBase64String((1..32 | ForEach-Object { Get-Random -Minimum 0 -Maximum 256 }))
```

**Online (não recomendado para produção):**
- Use apenas para desenvolvimento
- Nunca compartilhe o secret gerado

### Validações Implementadas

A aplicação valida automaticamente:

1. **Secret não pode estar vazio**
   - Erro: `JWT SigningKey must be configured via environment variable JWT__SIGNINGKEY`

2. **Secret não pode ser valor padrão em produção**
   - Erro: `JWT SigningKey must be configured via environment variable JWT__SIGNINGKEY in production`

3. **Secret mínimo de 32 caracteres em produção**
   - Erro: `JWT SigningKey must be at least 32 characters long in production`

### Rotação de Secrets

Para rotacionar o secret JWT:

1. **Gerar novo secret** (usando método acima)
2. **Atualizar variável de ambiente** `JWT__SIGNINGKEY`
3. **Reiniciar aplicação**
4. **Nota**: Tokens antigos serão invalidados

---

## ⚡ Configuração de Rate Limiting

### Limites Padrão

| Tipo de Endpoint | Limite | Janela |
|------------------|--------|--------|
| Auth (login) | 5 req/min | 1 minuto |
| Feed (leitura) | 100 req/min | 1 minuto |
| Write (escrita) | 30 req/min | 1 minuto |
| Global | 60 req/min | 1 minuto |

### Configuração via appsettings.json

```json
{
  "RateLimiting": {
    "PermitLimit": 60,        // Requisições por janela
    "WindowSeconds": 60,      // Janela em segundos (60 = 1 minuto)
    "QueueLimit": 0           // Limite de fila (0 = sem fila, rejeita imediatamente)
  }
}
```

### Configuração via Variáveis de Ambiente

```bash
RateLimiting__PermitLimit=60
RateLimiting__WindowSeconds=60
RateLimiting__QueueLimit=0
```

### Resposta quando Limite Excedido

**Status Code**: `429 Too Many Requests`

**Headers**:
```
Retry-After: 60
```

**Body**:
```json
{
  "title": "Too Many Requests",
  "status": 429,
  "detail": "Rate limit exceeded. Please try again later.",
  "instance": "/api/v1/auth/social"
}
```

### Ajustando Limites

Para ajustar limites específicos, edite `Program.cs`:

```csharp
// Auth: 5 req/min
options.AddFixedWindowLimiter("auth", limiterOptions =>
{
    limiterOptions.PermitLimit = 5;  // Ajuste aqui
    limiterOptions.Window = TimeSpan.FromMinutes(1);
});
```

---

## 🌐 Configuração de CORS

### Produção (OBRIGATÓRIO)

**Não pode usar wildcard (*) em produção!**

```json
{
  "Cors": {
    "AllowedOrigins": [
      "https://app.araponga.com",
      "https://www.araponga.com"
    ]
  }
}
```

Ou via variáveis de ambiente:
```bash
Cors__AllowedOrigins__0=https://app.araponga.com
Cors__AllowedOrigins__1=https://www.araponga.com
```

### Desenvolvimento

```json
{
  "Cors": {
    "AllowedOrigins": [ "*" ]
  }
}
```

### Validação Automática

A aplicação valida automaticamente em produção:
- ✅ Origins não podem ser vazios
- ✅ Wildcard (*) não permitido
- ✅ Erro claro se configuração inválida

---

## 🔒 Configuração de HTTPS

### Produção

HTTPS é **obrigatório** em produção. A aplicação:

1. **Redireciona HTTP para HTTPS** automaticamente
2. **Configura HSTS** (HTTP Strict Transport Security):
   - Max-Age: 365 dias
   - IncludeSubDomains: habilitado
   - Preload: habilitado

### Certificados SSL/TLS

Configure certificados no servidor web (Nginx, IIS, etc.) ou use serviços como:
- Let's Encrypt (gratuito)
- Cloudflare (gratuito com proxy)
- AWS Certificate Manager
- Azure App Service (gerenciado)

### Desenvolvimento

HTTPS é **desabilitado** em desenvolvimento para facilitar testes locais.

---

## 🛡️ Security Headers

Os seguintes headers são adicionados automaticamente em todas as respostas:

| Header | Valor | Descrição |
|--------|-------|-----------|
| `X-Frame-Options` | `DENY` | Previne clickjacking |
| `X-Content-Type-Options` | `nosniff` | Previne MIME type sniffing |
| `X-XSS-Protection` | `1; mode=block` | Proteção XSS (legacy) |
| `Referrer-Policy` | `strict-origin-when-cross-origin` | Controla informações de referrer |
| `Permissions-Policy` | `geolocation=(), microphone=(), camera=()` | Restringe features do navegador |
| `Content-Security-Policy` | (configurado dinamicamente) | Política de segurança de conteúdo |
| `Strict-Transport-Security` | `max-age=31536000; includeSubDomains; preload` | Força HTTPS (apenas em HTTPS) |

### Content-Security-Policy (CSP)

O CSP é configurado dinamicamente:
- **API endpoints**: CSP mais restritivo (sem `unsafe-inline` ou `unsafe-eval`)
- **DevPortal/Swagger**: CSP mais permissivo (necessário para funcionamento)

**Não é necessário configurar manualmente** - são aplicados automaticamente.

---

## ✅ Checklist de Deploy

### Antes do Deploy

- [ ] **JWT Secret configurado** via `JWT__SIGNINGKEY`
- [ ] **JWT Secret tem mínimo 32 caracteres**
- [ ] **JWT Secret não é valor padrão**
- [ ] **CORS Origins configurados** (sem wildcard)
- [ ] **HTTPS configurado** no servidor web
- [ ] **Certificados SSL válidos**
- [ ] **Rate limiting configurado** (se necessário ajustar limites)
- [ ] **Variáveis de ambiente** configuradas no servidor

### Após o Deploy

- [ ] **Testar autenticação** (login funciona)
- [ ] **Testar rate limiting** (429 retornado quando excedido)
- [ ] **Verificar security headers** (presentes em todas as respostas)
- [ ] **Testar CORS** (frontend consegue fazer requisições)
- [ ] **Verificar HTTPS** (redirecionamento funciona)
- [ ] **Verificar HSTS** (header presente)

### Comandos de Verificação

```bash
# Verificar security headers
curl -I https://api.araponga.com/api/v1/territories

# Verificar rate limiting
for i in {1..6}; do
  curl -X POST https://api.araponga.com/api/v1/auth/social
done

# Verificar CORS
curl -H "Origin: https://app.araponga.com" \
     -H "Access-Control-Request-Method: POST" \
     -X OPTIONS https://api.araponga.com/api/v1/feed
```

---

## 🚨 Troubleshooting

### Erro: "JWT SigningKey must be configured"

**Causa**: Variável de ambiente `JWT__SIGNINGKEY` não configurada.

**Solução**:
```bash
export JWT__SIGNINGKEY="seu-secret-aqui"
# Ou configure no servidor/container
```

### Erro: "Cors:AllowedOrigins must be configured"

**Causa**: CORS não configurado ou usando wildcard em produção.

**Solução**:
```json
{
  "Cors": {
    "AllowedOrigins": ["https://app.araponga.com"]
  }
}
```

### Rate Limiting muito restritivo

**Causa**: Limites padrão podem ser muito baixos para seu caso.

**Solução**: Ajuste em `appsettings.json` ou `Program.cs`:
```json
{
  "RateLimiting": {
    "PermitLimit": 120,  // Aumentar limite
    "WindowSeconds": 60
  }
}
```

### Security Headers não aparecem

**Causa**: Middleware não está sendo executado.

**Solução**: Verificar ordem dos middlewares em `Program.cs`. `SecurityHeadersMiddleware` deve ser um dos primeiros.

---

## 📚 Referências

- [ASP.NET Core Security](https://learn.microsoft.com/en-us/aspnet/core/security/)
- [Rate Limiting](https://learn.microsoft.com/en-us/aspnet/core/performance/rate-limit)
- [CORS](https://learn.microsoft.com/en-us/aspnet/core/security/cors)
- [HSTS](https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Strict-Transport-Security)
- [OWASP Security Headers](https://owasp.org/www-project-secure-headers/)

---

## 🧪 Testes de Segurança

A aplicação inclui uma suíte completa de testes de segurança (14 testes) que validam:

- ✅ Autenticação (JWT válido/inválido/expirado)
- ✅ Autorização (Visitor vs Resident vs Curator)
- ✅ Rate limiting
- ✅ Validação de input (SQL injection, XSS, NoSQL injection)
- ✅ Path traversal
- ✅ CSRF
- ✅ Command injection
- ✅ Resource ownership
- ✅ HTTPS enforcement
- ✅ CORS
- ✅ Security headers

**Arquivo**: `backend/Araponga.Tests/Api/SecurityTests.cs`

---

## 🔐 Autenticação de Dois Fatores (2FA)

A API suporta autenticação de dois fatores usando TOTP (Time-based One-Time Password).

### Endpoints de 2FA

- **Setup**: `POST /api/v1/auth/2fa/setup` - Gera secret e QR code
- **Confirm**: `POST /api/v1/auth/2fa/confirm` - Habilita 2FA após validar código TOTP
- **Verify**: `POST /api/v1/auth/2fa/verify` - Verifica código TOTP e retorna JWT
- **Recover**: `POST /api/v1/auth/2fa/recover` - Usa recovery code para autenticação
- **Disable**: `POST /api/v1/auth/2fa/disable` - Desabilita 2FA (requer código TOTP ou recovery code válido)

### Fluxo de 2FA

1. Usuário faz login social → Se 2FA habilitado, recebe `2FA_REQUIRED:{challengeId}`
2. Usuário envia código TOTP via `POST /api/v1/auth/2fa/verify` → Recebe JWT
3. Alternativamente, usuário pode usar recovery code via `POST /api/v1/auth/2fa/recover`

### Recovery Codes

Durante a configuração de 2FA, o usuário recebe recovery codes que podem ser usados caso perca acesso ao dispositivo TOTP. Guarde esses códigos em local seguro.

---

## 🧹 Sanitização de Inputs

A API implementa sanitização avançada de inputs através do `InputSanitizationService`:

- **HTML**: Remove tags e escapa caracteres especiais
- **Paths**: Remove caracteres perigosos e normaliza separadores
- **URLs**: Valida formato e bloqueia javascript:, data:, etc
- **SQL**: Proteção adicional (EF Core já protege contra SQL injection)
- **Texto**: Remove caracteres de controle e normaliza espaços

**Uso**: O serviço está disponível via DI e pode ser injetado em controllers/services conforme necessário.

---

## 🛡️ Proteção CSRF

A API implementa proteção CSRF usando anti-forgery tokens:

- **Header esperado**: `X-CSRF-Token`
- **Cookie**: `__Host-CSRF` (HttpOnly, Secure, SameSite=Strict)
- **Configuração**: Automática via `AddAntiforgery()` no `Program.cs`

**Nota**: Validação explícita em endpoints específicos pode ser adicionada usando `[ValidateAntiForgeryToken]` quando necessário.

---

## 🔑 Secrets Management

A API suporta gerenciamento de secrets através da interface `ISecretsService`:

- **Implementação padrão**: `EnvironmentSecretsService` (usa variáveis de ambiente)
- **Extensível**: Interface pronta para Azure Key Vault ou AWS Secrets Manager
- **Formato de variável**: `SECRET_NAME` (substitui `:` por `__`)
- **Fallback**: Se não encontrado em variável de ambiente, tenta `appsettings.json`

**Exemplo**:
```bash
# Variável de ambiente
JWT__SIGNINGKEY=seu-secret-aqui

# Ou em appsettings.json
{
  "Jwt": {
    "SigningKey": "seu-secret-aqui"
  }
}
```

---

## 📋 Auditoria

A API implementa auditoria de ações críticas:

- **Serviço**: `AuditService` disponível via DI
- **Repositório**: Interface `IAuditRepository` criada (implementação pode ser adicionada)
- **Ações auditadas**: Reports, bloqueios, validações, mudanças de dados críticas

**Uso**:
```csharp
await _auditService.LogAsync("user.blocked", userId, territoryId, targetId);
```

---

## 📚 Documentação Adicional

- **[Security Audit](./SECURITY_AUDIT.md)** - Checklist completo de segurança e guia de penetration testing
- **[Fase 5: Segurança Avançada](./backlog-api/implementacoes/FASE5_IMPLEMENTACAO_RESUMO.md)** - Resumo completo das implementações de segurança avançada

---

**Última atualização**: 2026-01-15
