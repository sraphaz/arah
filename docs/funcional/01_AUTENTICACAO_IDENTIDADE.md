# Autenticação e Identidade - Documentação Funcional

**Versão**: 1.0  
**Data**: 2026-01-28  
**Status**: Funcionalidade Implementada  
**Parte de**: [Documentação Funcional da Plataforma](./00_PLATAFORMA_ARAPONGA.md)

---

## 📋 Índice

1. [Visão Geral](#visão-geral)
2. [Função de Negócio](#função-de-negócio)
3. [Elementos da Arquitetura](#elementos-da-arquitetura)
4. [Fluxos Funcionais](#fluxos-funcionais)
5. [Casos de Uso](#casos-de-uso)
6. [Regras de Negócio](#regras-de-negócio)

---

## 🎯 Visão Geral

O domínio de **Autenticação e Identidade** é responsável por garantir que cada pessoa tenha uma identidade única, verificada e segura na plataforma Araponga. Este é o primeiro passo para qualquer interação na plataforma.

### Objetivo

Permitir que usuários:
- **Cadastrem-se** na plataforma de forma segura
- **Autentiquem-se** para acessar funcionalidades
- **Verifiquem sua identidade** para acesso a funcionalidades sensíveis
- **Recuperem acesso** caso esqueçam credenciais

---

## 💼 Função de Negócio

### Para o Usuário

**Como Visitante Novo**:
- Cadastrar-se via login social (Google, Facebook, Apple)
- Fornecer informações básicas (nome, CPF/documento)
- Receber token de acesso para usar a plataforma
- Recuperar acesso caso perca credenciais

**Como Usuário Existente**:
- Fazer login rapidamente via login social
- Acessar todas as funcionalidades baseadas em seu perfil
- Gerenciar segurança da conta (2FA)

### Para a Plataforma

- **Segurança**: Garantir que apenas usuários autenticados acessem funcionalidades
- **Verificação**: Validar identidade para operações sensíveis (marketplace, votações)
- **Rastreabilidade**: Associar todas as ações a uma identidade verificada
- **Compliance**: Atender requisitos de LGPD/GDPR com verificação de identidade

---

## 🏗️ Elementos da Arquitetura

### Entidades Principais

#### User (Usuário)
- **Propósito**: Representa uma pessoa única e global na plataforma
- **Atributos**:
  - `Id`: Identificador único
  - `DisplayName`: Nome de exibição
  - `Email`: Email (opcional)
  - `CPF` ou `ForeignDocument`: Documento de identidade
  - `PhoneNumber`: Telefone (opcional)
  - `Address`: Endereço (opcional)
  - `AuthProvider`: Provedor de autenticação (google, facebook, apple)
  - `ExternalId`: ID no provedor externo
  - `UserIdentityVerificationStatus`: Status de verificação (Unverified, Pending, Verified, Rejected)

#### SystemPermission (Permissão do Sistema)
- **Propósito**: Permissões globais do sistema (não territoriais)
- **Tipos**: Admin, SystemOperator
- **Características**: Concedidas/revogadas com auditoria

#### UserPreferences (Preferências do Usuário)
- **Propósito**: Configurações de privacidade e notificações
- **Atributos**: Preferências de notificação, privacidade, etc.

### Serviços e Interfaces

#### IAuthService
- `LoginSocialAsync()`: Autentica ou cadastra via login social
- `PasswordResetAsync()`: Solicita recuperação de senha
- `PasswordResetConfirmAsync()`: Confirma recuperação e retorna token

#### IUserIdentityVerificationService
- `RequestVerificationAsync()`: Solicita verificação de identidade
- `GetVerificationStatusAsync()`: Obtém status da verificação

---

## 🔄 Fluxos Funcionais

### Fluxo 1: Cadastro e Primeiro Acesso

```
┌─────────────┐
│   Usuário   │
└──────┬──────┘
       │
       │ 1. Acessa app/web
       ▼
┌─────────────────────┐
│  Tela de Login      │
│  (Google/Facebook)  │
└──────┬──────────────┘
       │
       │ 2. Seleciona provedor
       ▼
┌─────────────────────┐
│  Autenticação Social│
│  (OAuth)            │
└──────┬──────────────┘
       │
       │ 3. Retorna ExternalId
       ▼
┌─────────────────────┐
│  POST /auth/social  │
│  - Provider         │
│  - ExternalId       │
│  - DisplayName      │
│  - CPF/Documento    │
└──────┬──────────────┘
       │
       │ 4. Sistema verifica se existe
       ▼
    ┌──┴──┐
    │ SIM │  ──► 5a. Retorna token existente
    └──┬──┘
       │ NÃO
       ▼
┌─────────────────────┐
│  Cria novo User     │
│  - Gera Id único    │
│  - Salva dados      │
│  - Status: Unverified│
└──────┬──────────────┘
       │
       │ 5b. Retorna token JWT
       ▼
┌─────────────────────┐
│  Token JWT          │
│  (usado em todas    │
│   requisições)      │
└─────────────────────┘
```

**Resultado**: Usuário autenticado com token JWT válido

### Fluxo 2: Login de Usuário Existente

```
┌─────────────┐
│   Usuário   │
└──────┬──────┘
       │
       │ 1. Acessa app/web
       ▼
┌─────────────────────┐
│  Tela de Login      │
└──────┬──────────────┘
       │
       │ 2. Seleciona provedor
       ▼
┌─────────────────────┐
│  Autenticação Social│
└──────┬──────────────┘
       │
       │ 3. Retorna ExternalId
       ▼
┌─────────────────────┐
│  POST /auth/social  │
│  - Provider         │
│  - ExternalId       │
└──────┬──────────────┘
       │
       │ 4. Sistema encontra User existente
       ▼
┌─────────────────────┐
│  Retorna token JWT  │
│  (mesmo token ou    │
│   novo se expirou)  │
└─────────────────────┘
```

**Resultado**: Usuário autenticado rapidamente

### Fluxo 3: Recuperação de Acesso

```
┌─────────────┐
│   Usuário   │
└──────┬──────┘
       │
       │ 1. Esqueceu acesso
       ▼
┌─────────────────────┐
│  Tela "Esqueci      │
│   minha senha"      │
└──────┬──────────────┘
       │
       │ 2. Informa email
       ▼
┌─────────────────────┐
│  POST /auth/        │
│  password-reset     │
│  - Email            │
└──────┬──────────────┘
       │
       │ 3. Sistema gera token único
       │    (sempre retorna 200, mesmo
       │     se email não existe)
       ▼
┌─────────────────────┐
│  Envia email com    │
│  link de recuperação│
│  (se configurado)   │
└──────┬──────────────┘
       │
       │ 4. Usuário clica no link
       ▼
┌─────────────────────┐
│  POST /auth/        │
│  password-reset/    │
│  confirm            │
│  - Token            │
└──────┬──────────────┘
       │
       │ 5. Sistema valida token
       ▼
┌─────────────────────┐
│  Retorna novo       │
│  token JWT          │
└─────────────────────┘
```

**Resultado**: Usuário recupera acesso com novo token

### Fluxo 4: Verificação de Identidade

```
┌─────────────┐
│   Usuário   │
└──────┬──────┘
       │
       │ 1. Tenta acessar funcionalidade
       │    que requer verificação
       │    (ex: criar loja no marketplace)
       ▼
┌─────────────────────┐
│  Sistema verifica   │
│  UserIdentity       │
│  VerificationStatus │
└──────┬──────────────┘
       │
    ┌──┴──┐
    │Verified│  ──► 2a. Permite acesso
    └──┬──┘
       │ Unverified/Pending/Rejected
       ▼
┌─────────────────────┐
│  Solicita           │
│  verificação        │
└──────┬──────────────┘
       │
       │ 3. Usuário envia documentos
       ▼
┌─────────────────────┐
│  Upload de          │
│  documentos         │
│  (CPF, comprovante) │
└──────┬──────────────┘
       │
       │ 4. Sistema processa
       │    (manual ou automático)
       ▼
┌─────────────────────┐
│  Status atualizado: │
│  - Pending           │
│  - Verified          │
│  - Rejected         │
└─────────────────────┘
```

**Resultado**: Usuário com identidade verificada pode acessar funcionalidades sensíveis

---

## 📖 Casos de Uso

### Caso de Uso 1: Novo Usuário se Cadastra

**Ator**: Usuário novo  
**Pré-condições**: Nenhuma  
**Fluxo Principal**:
1. Usuário acessa app/web
2. Seleciona "Cadastrar com Google/Facebook"
3. Autentica no provedor social
4. Informa CPF ou documento estrangeiro
5. (Opcional) Informa email, telefone, endereço
6. Sistema cria conta e retorna token
7. Usuário é redirecionado para descoberta de territórios

**Fluxo Alternativo**: Se usuário já existe, retorna token existente

**Pós-condições**: Usuário autenticado com status Unverified

### Caso de Uso 2: Usuário Faz Login

**Ator**: Usuário existente  
**Pré-condições**: Conta já existe  
**Fluxo Principal**:
1. Usuário acessa app/web
2. Seleciona "Entrar com Google/Facebook"
3. Autentica no provedor social
4. Sistema encontra conta existente
5. Retorna token JWT
6. Usuário acessa plataforma

**Pós-condições**: Usuário autenticado

### Caso de Uso 3: Usuário Recupera Acesso

**Ator**: Usuário que perdeu acesso  
**Pré-condições**: Conta existe com email cadastrado  
**Fluxo Principal**:
1. Usuário acessa "Esqueci minha senha"
2. Informa email
3. Sistema envia email com link de recuperação (se configurado)
4. Usuário clica no link
5. Sistema valida token
6. Retorna novo token JWT
7. Usuário acessa plataforma

**Fluxo Alternativo**: Se email não existe, sistema retorna 200 mas não envia email (segurança)

**Pós-condições**: Usuário com novo token válido

### Caso de Uso 4: Usuário Verifica Identidade

**Ator**: Usuário que precisa de verificação  
**Pré-condições**: Conta existe, status Unverified  
**Fluxo Principal**:
1. Usuário tenta acessar funcionalidade que requer verificação
2. Sistema bloqueia e solicita verificação
3. Usuário faz upload de documentos (CPF, comprovante de residência)
4. Sistema processa verificação (manual ou automático)
5. Status atualizado para Pending → Verified ou Rejected
6. Se Verified, usuário pode acessar funcionalidade

**Pós-condições**: Usuário com identidade verificada (ou rejeitada)

---

## ⚙️ Regras de Negócio

### Autenticação

1. **Login Social**:
   - Provider e ExternalId formam chave única
   - Se usuário já existe, retorna token existente
   - Se não existe, cria novo usuário

2. **CPF/Documento**:
   - CPF e ForeignDocument são mutuamente exclusivos
   - CPF aceita formatação (123.456.789-00) ou apenas dígitos
   - Validação de formato obrigatória

3. **Token JWT**:
   - Incluído em todas as requisições: `Authorization: Bearer {token}`
   - Expira após período configurado
   - Renovação automática quando possível

### Verificação de Identidade

1. **Status de Verificação**:
   - `Unverified`: Padrão para novos usuários
   - `Pending`: Verificação em andamento
   - `Verified`: Identidade confirmada
   - `Rejected`: Verificação negada

2. **Requisitos para Verificação**:
   - Upload de documentos válidos
   - Validação manual ou automática
   - Auditoria completa do processo

3. **Impacto da Verificação**:
   - Marketplace: Requer Verified para criar loja
   - Votações: Algumas requerem Verified
   - Operações financeiras: Sempre requerem Verified

### Segurança

1. **Rate Limiting**:
   - Autenticação: 5 requisições/minuto por IP/usuário
   - Proteção contra brute force

2. **2FA (Two-Factor Authentication)**:
   - Opcional para usuários
   - Obrigatório para operações sensíveis (futuro)

3. **Recuperação de Acesso**:
   - Token de uso único
   - Expira após período configurado
   - Sempre retorna 200 (não revela se email existe)

### Privacidade

1. **Dados Sensíveis**:
   - CPF/documento criptografado em repouso
   - Nunca logado em texto plano
   - Acesso apenas para verificação

2. **LGPD/GDPR**:
   - Consentimento explícito para coleta
   - Direito ao esquecimento
   - Exportação de dados

---

## 🔗 Integrações

### Com Outros Domínios

- **Territórios**: Após autenticação, usuário descobre territórios
- **Memberships**: Autenticação necessária para criar vínculos
- **Marketplace**: Verificação de identidade para operações financeiras
- **Governança**: Verificação para votações importantes

### Com Sistemas Externos

- **OAuth Providers**: Google, Facebook, Apple
- **Email Service**: Envio de recuperação de acesso
- **Document Verification**: Serviços de verificação de documentos (futuro)

---

## 📊 Métricas e Observabilidade

### Métricas Importantes

- Taxa de cadastro (novos usuários/dia)
- Taxa de login (logins/dia)
- Taxa de recuperação de acesso (solicitações/dia)
- Taxa de verificação de identidade (aprovadas/rejeitadas)
- Tempo médio de verificação

### Logs Estruturados

- Tentativas de login (sucesso/falha)
- Criação de contas
- Solicitações de recuperação
- Mudanças de status de verificação

---

## 📚 Documentação Relacionada

- **[Plataforma Araponga](./00_PLATAFORMA_ARAPONGA.md)** - Visão geral
- **[Territórios e Memberships](./02_TERRITORIOS_MEMBERSHIPS.md)** - Próximo passo após autenticação
- **[API - Autenticação](../api/60_01_API_AUTENTICACAO.md)** - Documentação técnica da API

---

**Última Atualização**: 2026-01-28  
**Versão**: 1.0  
**Status**: Funcionalidade Implementada
