# Autenticação e Cadastro - API Araponga

**Parte de**: [API Araponga - Lógica de Negócio e Usabilidade](./60_API_LÓGICA_NEGÓCIO_INDEX.md)  
**Versão**: 2.0  
**Data**: 2025-01-20

---

## 🔐 Autenticação e Cadastro

### Login Social (`POST /api/v1/auth/social`)

**Descrição**: Autentica ou cadastra um usuário via login social.

**Como usar**:
- Envie Provider (ex: "google", "facebook"), ExternalId, DisplayName
- Forneça CPF (formato: "123.456.789-00" ou "12345678900") OU ForeignDocument
- Campos opcionais: Email, PhoneNumber, Address

**Regras de negócio**:
- Se o usuário já existir (mesmo Provider + ExternalId), retorna token existente
- Se não existir, cria novo usuário e retorna token
- CPF e ForeignDocument são mutuamente exclusivos (não pode enviar ambos)
- CPF aceita formatação (pontos e hífen) ou apenas dígitos
- O token JWT retornado deve ser incluído em todas as requisições subsequentes no header `Authorization: Bearer {token}`

**Rate Limiting**:
- **Limite**: 5 requisições por minuto por IP/usuário
- **Resposta quando excedido**: `429 Too Many Requests` com header `Retry-After`

**Resposta**:
- **200 OK**: Token JWT e dados do usuário
- **400 Bad Request**: Validação falhou (campos obrigatórios ausentes, CPF inválido, etc.)
- **429 Too Many Requests**: Rate limit excedido

### Recuperacao de Acesso (`POST /api/v1/auth/password-reset`)

**Descricao**: solicita envio de token de recuperacao por email para permitir acesso seguro.

**Regras de negocio**:
- Sempre responde 200 mesmo quando o email nao existe (nao revela cadastro).
- Token expira conforme `Auth:PasswordReset:TokenTtlMinutes`.
- Quando `Auth:PasswordReset:ResetUrlBase` esta configurado, o email envia um link com o token.

**Resposta**:
- **200 OK**: mensagem informativa
- **400 Bad Request**: email invalido

---

### Confirmacao de Recuperacao (`POST /api/v1/auth/password-reset/confirm`)

**Descricao**: confirma token de recuperacao e retorna um JWT valido.

**Regras de negocio**:
- Token e de uso unico e expira automaticamente.
- Tokens invalidos ou expirados retornam erro.

**Resposta**:
- **200 OK**: token JWT
- **400 Bad Request**: token invalido ou expirado
---

## 📚 Documentação Relacionada

- **[Visão Geral](./60_00_API_VISAO_GERAL.md)** - Princípios fundamentais e segurança
- **[Territórios](./60_02_API_TERRITORIOS.md)** - Próximo passo após autenticação
- **DevPortal**: [Autenticação](../devportal/#auth) - Exemplos práticos de código

---

**Voltar para**: [Índice da Documentação da API](./60_API_LÓGICA_NEGÓCIO_INDEX.md)

