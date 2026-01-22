# Conformidade LGPD - Araponga

**Última Atualização**: 2026-01-20  
**Status**: ✅ Implementado

---

## 📋 Resumo

Este documento descreve as medidas implementadas no Araponga para garantir conformidade com a **Lei Geral de Proteção de Dados (LGPD - Lei nº 13.709/2018)**.

---

## ✅ Funcionalidades Implementadas

### 1. Exportação de Dados Pessoais

**Endpoint**: `GET /api/v1/users/me/export`

**Descrição**: Permite que o usuário exporte todos os seus dados pessoais em formato JSON.

**Dados Exportados**:
- Perfil de usuário (nome, email, telefone, endereço)
- Memberships (vínculos territoriais)
- Posts criados
- Eventos criados
- Participações em eventos
- Notificações recebidas (últimas 1000)
- Preferências de privacidade e notificações
- Aceites de Termos de Uso
- Aceites de Políticas de Privacidade

**Formato**: JSON com indentação, propriedades em camelCase.

**Segurança**:
- Requer autenticação (usuário só pode exportar seus próprios dados)
- Rate limiting aplicado
- Download direto via `File` response

**Arquivo Gerado**: `user-data-export-{userId}-{timestamp}.json`

---

### 2. Exclusão de Conta com Anonimização

**Endpoint**: `DELETE /api/v1/users/me`

**Descrição**: Permite que o usuário exclua sua conta, anonimizando todos os dados pessoais identificáveis.

**Processo de Anonimização**:
1. **Dados Pessoais Anonimizados**:
   - `DisplayName`: Substituído por `User_{8 primeiros caracteres do ID}`
   - `Email`: Removido (null)
   - `CPF`: Substituído por valor fictício `000.000.000-00`
   - `ForeignDocument`: Removido (null)
   - `PhoneNumber`: Removido (null)
   - `Address`: Removido (null)
   - `ExternalId`: Substituído por `anon_{userId}`
   - `TwoFactorSecret`: Removido
   - `TwoFactorRecoveryCodesHash`: Removido
   - `IdentityVerificationStatus`: Resetado para `Unverified`
   - `IdentityVerifiedAtUtc`: Removido

2. **Dados Mantidos (para estatísticas agregadas)**:
   - `Id`: Mantido (necessário para integridade referencial)
   - `CreatedAtUtc`: Mantido (para estatísticas temporais)
   - Posts, eventos e outras entidades mantêm `AuthorUserId` (mas dados pessoais já foram anonimizados)

3. **Preferências**:
   - Resetadas para valores padrão (anonimizadas)

**Segurança**:
- Requer autenticação (usuário só pode excluir sua própria conta)
- Rate limiting aplicado
- Validação prévia (`CanDeleteUserAsync`) para verificar dependências

**Nota**: A anonimização é permanente e irreversível. O usuário deve exportar seus dados antes de excluir a conta.

---

## 🔒 Princípios LGPD Implementados

### 1. Finalidade ✅
- Dados coletados apenas para funcionalidades do sistema
- Não há coleta de dados desnecessários

### 2. Adequação ✅
- Dados coletados são adequados e necessários para as funcionalidades
- Não há coleta de dados sensíveis sem necessidade

### 3. Necessidade ✅
- Apenas dados necessários são coletados
- Dados agregados são mantidos apenas para estatísticas

### 4. Livre Acesso ✅
- **Implementado**: Endpoint de exportação permite acesso aos dados
- Usuário pode visualizar todos os seus dados em formato JSON

### 5. Qualidade dos Dados ✅
- Dados são mantidos atualizados
- Validações garantem qualidade dos dados

### 6. Transparência ✅
- Política de Privacidade implementada
- Termos de Uso implementados
- Usuário pode verificar quais dados são coletados via exportação

### 7. Segurança ✅
- Dados pessoais protegidos
- Autenticação obrigatória para acesso
- Rate limiting aplicado
- Anonimização garante que dados não podem ser reidentificados

### 8. Prevenção ✅
- Medidas de segurança implementadas
- Validações de entrada
- Proteção contra acesso não autorizado

### 9. Não Discriminação ✅
- Sistema não discrimina usuários
- Tratamento igualitário de dados

### 10. Responsabilização e Prestação de Contas ✅
- Sistema de auditoria implementado
- Logs de ações críticas
- Rastreabilidade de aceites de termos e políticas

---

## 📝 Direitos do Titular Implementados

### ✅ Direito de Acesso (Art. 9, I)
**Implementado**: Endpoint `GET /api/v1/users/me/export`
- Usuário pode acessar todos os seus dados pessoais
- Dados são fornecidos em formato estruturado (JSON)

### ✅ Direito de Correção (Art. 9, II)
**Implementado**: Endpoints de atualização de perfil
- `PUT /api/v1/users/me/profile` - Atualizar nome e informações de contato
- Dados podem ser corrigidos pelo usuário

### ✅ Direito de Anonimização, Bloqueio ou Eliminação (Art. 9, III)
**Implementado**: Endpoint `DELETE /api/v1/users/me`
- Usuário pode solicitar exclusão de conta
- Dados são anonimizados (não completamente deletados para manter integridade referencial)
- Dados pessoais identificáveis são removidos ou anonimizados

### ✅ Direito de Portabilidade (Art. 9, IV)
**Implementado**: Endpoint `GET /api/v1/users/me/export`
- Dados são exportados em formato JSON estruturado
- Formato permite importação em outros sistemas

### ✅ Direito de Eliminação (Art. 9, V)
**Implementado**: Via anonimização
- Dados pessoais são anonimizados quando conta é excluída
- Dados agregados são mantidos apenas para estatísticas

### ✅ Direito de Informação (Art. 9, VI)
**Implementado**: 
- Política de Privacidade disponível
- Termos de Uso disponíveis
- Usuário é informado sobre coleta e uso de dados

### ⚠️ Direito de Revogação de Consentimento (Art. 9, VII)
**Parcialmente Implementado**:
- Usuário pode revogar aceite de termos e políticas
- Endpoints: `DELETE /api/v1/terms/{id}/accept`, `DELETE /api/v1/privacy/{id}/accept`
- **Nota**: Revogação pode impedir acesso a funcionalidades que requerem aceite

---

## 🔐 Medidas de Segurança

### Autenticação e Autorização
- ✅ Autenticação obrigatória para todos os endpoints de dados pessoais
- ✅ Usuário só pode acessar seus próprios dados
- ✅ Validação de token JWT

### Proteção de Dados
- ✅ Dados pessoais não são expostos em respostas públicas
- ✅ Anonimização garante que dados não podem ser reidentificados
- ✅ Rate limiting previne abuso

### Auditoria
- ✅ Logs de ações críticas (aceites de termos, exclusão de conta)
- ✅ Rastreabilidade de mudanças
- ✅ Timestamps em todas as operações

---

## 📊 Dados Coletados

### Dados Pessoais Coletados
- Nome de exibição
- Email (opcional)
- CPF ou documento estrangeiro (opcional)
- Telefone (opcional)
- Endereço (opcional)
- Provedor de autenticação e ID externo

### Dados de Uso Coletados
- Posts criados
- Eventos criados/participados
- Memberships (vínculos territoriais)
- Notificações recebidas
- Preferências de privacidade e notificações
- Aceites de termos e políticas

### Dados Técnicos Coletados
- IP Address (ao aceitar termos/políticas)
- User Agent (ao aceitar termos/políticas)
- Timestamps de criação e atualização

---

## 🚫 Dados NÃO Coletados

- Dados de localização em tempo real (apenas quando usuário compartilha)
- Dados biométricos
- Dados de saúde (exceto alertas territoriais, que são públicos)
- Dados financeiros completos (apenas transações relacionadas ao marketplace)

---

## 📋 Procedimentos

### Como Exportar Dados

1. Usuário autenticado faz requisição: `GET /api/v1/users/me/export`
2. Sistema coleta todos os dados do usuário
3. Sistema serializa dados em JSON
4. Sistema retorna arquivo JSON para download

### Como Excluir Conta

1. Usuário autenticado faz requisição: `DELETE /api/v1/users/me`
2. Sistema valida se conta pode ser excluída
3. Sistema anonimiza dados pessoais
4. Sistema retorna confirmação de exclusão

**⚠️ Importante**: Usuário deve exportar dados antes de excluir conta, pois a anonimização é irreversível.

---

## 🔄 Atualizações Futuras

### Pendências (Opcionais)
- [ ] Período de graça para exclusão (7 dias para cancelar)
- [ ] Notificação antes da exclusão definitiva
- [ ] Dashboard administrativo para gerenciar solicitações de exclusão
- [ ] Relatório de conformidade LGPD

---

## 📚 Referências

- [Lei Geral de Proteção de Dados (LGPD)](http://www.planalto.gov.br/ccivil_03/_ato2015-2018/2018/lei/l13709.htm)
- [Guia de Implementação LGPD - ANPD](https://www.gov.br/anpd/pt-br)

---

**Última Atualização**: 2026-01-20  
**Responsável**: Equipe de Desenvolvimento Araponga
