# Política de Segurança

Agradecemos o esforço para reportar vulnerabilidades de forma responsável.

## Versões suportadas

O projeto ainda está em evolução. Recomendamos sempre testar e reportar com base na versão mais recente da branch principal.

## Como reportar

Se você encontrou uma vulnerabilidade ou questão de segurança:

1. Envie um e-mail para **rapha.sos@gmail.com** com detalhes suficientes para reproduzir.
2. Inclua impacto esperado, ambiente e qualquer prova de conceito relevante.
3. Aguarde uma confirmação de recebimento. Faremos o possível para responder rapidamente.

## Divulgação responsável

Pedimos que não divulgue publicamente a vulnerabilidade até que haja uma análise e uma correção publicadas.

---

## 🔒 Medidas de Segurança Implementadas

### Autenticação e Autorização

- **JWT Tokens**: Autenticação baseada em JWT com validação de secret forte
- **Secret Management**: JWT secret configurado via variável de ambiente (nunca hardcoded)
- **Validação de Secret**: 
  - Obrigatório em todos os ambientes
  - Mínimo de 32 caracteres em produção
  - Validação que não é valor padrão em produção

### Rate Limiting

- **Proteção contra DDoS**: Rate limiting implementado em todos os endpoints
- **Limites por Tipo**:
  - **Auth endpoints**: 5 requisições/minuto
  - **Feed endpoints**: 100 requisições/minuto
  - **Write endpoints**: 30 requisições/minuto
  - **Global**: 60 requisições/minuto (configurável)
- **Rate Limiting por Usuário**: Usuários autenticados têm limites individuais
- **Headers de Resposta**: `Retry-After` retornado quando limite excedido
- **Status Code**: 429 Too Many Requests quando limite excedido

### HTTPS e Transport Security

- **HTTPS Obrigatório**: Redirecionamento automático para HTTPS em produção
- **HSTS (HTTP Strict Transport Security)**:
  - Max-Age: 365 dias
  - IncludeSubDomains: habilitado
  - Preload: habilitado

### Security Headers

Todos os endpoints retornam os seguintes headers de segurança:

- **X-Frame-Options**: `DENY` - Previne clickjacking
- **X-Content-Type-Options**: `nosniff` - Previne MIME type sniffing
- **X-XSS-Protection**: `1; mode=block` - Proteção XSS
- **Referrer-Policy**: `strict-origin-when-cross-origin` - Controla informações de referrer
- **Permissions-Policy**: Restringe geolocalização, microfone e câmera
- **Content-Security-Policy**: Política restritiva de conteúdo

### Validação de Input

- **FluentValidation**: Validação automática de todos os requests
- **Validators Completos**: 14 validators cobrindo endpoints críticos
- **Validações Implementadas**:
  - Campos obrigatórios
  - Tamanhos máximos
  - Enums e tipos
  - Geolocalização (latitude/longitude)
  - Emails e URLs
  - GUIDs

### CORS (Cross-Origin Resource Sharing)

- **Configuração por Ambiente**: Diferentes políticas para dev/prod
- **Validação em Produção**: Wildcard (*) não permitido em produção
- **Preflight Cache**: 24 horas
- **Credentials**: Permitidos quando necessário

---

## ⚙️ Configuração de Segurança

### Variáveis de Ambiente Obrigatórias

```bash
# JWT Secret (OBRIGATÓRIO - mínimo 32 caracteres em produção)
JWT__SIGNINGKEY=seu-secret-forte-aqui-minimo-32-caracteres

# CORS Origins (OBRIGATÓRIO em produção - sem wildcard)
Cors__AllowedOrigins__0=https://app.araponga.com
Cors__AllowedOrigins__1=https://www.araponga.com
```

### Configuração de Rate Limiting

Edite `appsettings.json` ou variáveis de ambiente:

```json
{
  "RateLimiting": {
    "PermitLimit": 60,        // Requisições por janela
    "WindowSeconds": 60,      // Janela em segundos
    "QueueLimit": 0           // Limite de fila (0 = sem fila)
  }
}
```

---

## 🧪 Testes de Segurança

Testes automatizados validam:
- ✅ Rate limiting funciona corretamente
- ✅ Security headers estão presentes
- ✅ Validação de input funciona
- ✅ CORS configurado corretamente

Execute os testes:
```bash
dotnet test backend/Araponga.Tests
```

---

## 📚 Referências

- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [ASP.NET Core Security](https://learn.microsoft.com/en-us/aspnet/core/security/)
- [Rate Limiting Best Practices](https://learn.microsoft.com/en-us/aspnet/core/performance/rate-limit)
