# Fase 23: Inteligência Artificial

**Duração**: 4 semanas (28 dias úteis)  
**Prioridade**: 🟡 ALTA (Conformidade e inteligência)  
**Depende de**: Fase 12 (Otimizações), Fase 14 (Governança)  
**Estimativa Total**: 160 horas  
**Status**: ⏳ Pendente  
**Nota**: Este arquivo precisa ser atualizado - Fase 22 agora é Moeda Territorial, Fase 23 é IA, Fase 44 é Integrações Externas

---

## 🎯 Objetivo

Implementar **integrações com aplicativos externos** que:
- Permite postagem cross-platform (Instagram, Facebook, WhatsApp)
- Conecta com WhatsApp para notificações e comunicação
- Integra Apple Pay e Google Pay para pagamentos rápidos
- Implementa gestão de assinaturas digitais
- Facilita transição de outras plataformas
- Mantém valores de soberania territorial (opt-in, não invasivo)

**Princípios**:
- ✅ **Opt-in**: Usuário escolhe se quer integrar
- ✅ **Não Invasivo**: Não substitui funcionalidades principais
- ✅ **Transparência**: Usuário sabe o que está sendo compartilhado
- ✅ **Privacidade**: Respeita preferências de privacidade
- ✅ **Soberania**: Feed cronológico do Araponga permanece principal

---

## 📋 Contexto e Requisitos

### Estado Atual
- ✅ Sistema de pagamentos básico (Fase 6)
- ✅ Sistema de termos e políticas (Fase 12)
- ✅ Sistema de emails (Fase 13)
- ❌ Não existe integração com redes sociais
- ❌ Não existe integração com WhatsApp
- ❌ Não existe integração com Apple Pay/Google Pay
- ❌ Não existe sistema de assinaturas digitais

### Requisitos Funcionais

#### 1. Postagem Cross-Platform
- ✅ Publicar post do Araponga no Instagram
- ✅ Publicar post do Araponga no Facebook
- ✅ Compartilhar via WhatsApp (link + preview)
- ✅ Sincronização opcional (usuário escolhe)
- ✅ Respeitar privacidade territorial (não compartilhar RESIDENTS_ONLY)

#### 2. Conexão com WhatsApp
- ✅ Notificações via WhatsApp (eventos, alertas, mensagens)
- ✅ Alertas de saúde territorial via WhatsApp
- ✅ Confirmação de participação em eventos
- ✅ Suporte básico via WhatsApp

#### 3. Apple Pay e Google Pay
- ✅ Pagamento rápido no marketplace
- ✅ Doações territoriais
- ✅ Assinaturas (se implementadas)
- ✅ Integração com sistema de payout (Fase 7)

#### 4. Gestão de Assinaturas Digitais
- ✅ Assinaturas de documentos (termos, políticas)
- ✅ Assinaturas de projetos comunitários
- ✅ Assinaturas de petições territoriais
- ✅ Histórico de assinaturas no perfil

---

## 📋 Tarefas Detalhadas

### Semana 1-2: Postagem Cross-Platform

#### 22.1 Sistema de Postagem Cross-Platform
**Estimativa**: 80 horas (10 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `ISocialMediaPublisher` interface:
  - [ ] `PublishPostAsync(Guid postId, SocialMediaPlatform platform, ...)` → publicar post
  - [ ] `DeletePostAsync(string externalPostId, SocialMediaPlatform platform)` → deletar post
  - [ ] `UpdatePostAsync(string externalPostId, SocialMediaPlatform platform, ...)` → atualizar post
- [ ] Implementar `InstagramPublisher`:
  - [ ] Integração com Instagram Graph API
  - [ ] OAuth 2.0 para autenticação
  - [ ] Publicação de posts com imagens
  - [ ] Publicação de stories (opcional)
- [ ] Implementar `FacebookPublisher`:
  - [ ] Integração com Facebook Graph API
  - [ ] OAuth 2.0 para autenticação
  - [ ] Publicação de posts
  - [ ] Publicação em grupos (opcional)
- [ ] Implementar `WhatsAppPublisher`:
  - [ ] Integração com WhatsApp Business API
  - [ ] Compartilhamento de link com preview
  - [ ] Mensagens de texto
- [ ] Criar modelo `SocialMediaConnection`:
  - [ ] `Id`, `UserId`, `Platform` (INSTAGRAM, FACEBOOK, WHATSAPP)
  - [ ] `AccessToken` (criptografado)
  - [ ] `RefreshToken?` (nullable, criptografado)
  - [ ] `ExternalUserId` (ID do usuário na plataforma)
  - [ ] `IsActive` (bool)
  - [ ] `ExpiresAt?` (nullable)
  - [ ] `CreatedAtUtc`, `UpdatedAtUtc`
- [ ] Criar modelo `SocialMediaPost`:
  - [ ] `Id`, `PostId` (post do Araponga)
  - [ ] `Platform` (SocialMediaPlatform)
  - [ ] `ExternalPostId` (ID na plataforma externa)
  - [ ] `Status` (PENDING, PUBLISHED, FAILED, DELETED)
  - [ ] `PublishedAtUtc?` (nullable)
  - [ ] `CreatedAtUtc`, `UpdatedAtUtc`
- [ ] Criar `SocialMediaService`:
  - [ ] `ConnectAccountAsync(Guid userId, SocialMediaPlatform platform, string authCode)` → conectar conta
  - [ ] `DisconnectAccountAsync(Guid userId, SocialMediaPlatform platform)` → desconectar conta
  - [ ] `PublishPostAsync(Guid postId, SocialMediaPlatform[] platforms, ...)` → publicar post
  - [ ] `ListConnectionsAsync(Guid userId)` → listar conexões
- [ ] Criar `SocialMediaController`:
  - [ ] `POST /api/v1/social/connect` → conectar conta
  - [ ] `GET /api/v1/social/connections` → listar conexões
  - [ ] `DELETE /api/v1/social/connections/{id}` → desconectar conta
  - [ ] `POST /api/v1/posts/{postId}/publish-social` → publicar post
- [ ] Feature flags: `SocialMediaPublishingEnabled`, `InstagramEnabled`, `FacebookEnabled`, `WhatsAppEnabled`
- [ ] Validações e segurança
- [ ] Testes

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/SocialMediaService.cs`
- `backend/Araponga.Infrastructure/SocialMedia/ISocialMediaPublisher.cs`
- `backend/Araponga.Infrastructure/SocialMedia/InstagramPublisher.cs`
- `backend/Araponga.Infrastructure/SocialMedia/FacebookPublisher.cs`
- `backend/Araponga.Infrastructure/SocialMedia/WhatsAppPublisher.cs`
- `backend/Araponga.Domain/SocialMedia/SocialMediaConnection.cs`
- `backend/Araponga.Domain/SocialMedia/SocialMediaPlatform.cs`
- `backend/Araponga.Domain/SocialMedia/SocialMediaPost.cs`
- `backend/Araponga.Application/Interfaces/ISocialMediaConnectionRepository.cs`
- `backend/Araponga.Application/Interfaces/ISocialMediaPostRepository.cs`
- `backend/Araponga.Api/Controllers/SocialMediaController.cs`
- `backend/Araponga.Api/Contracts/SocialMedia/ConnectAccountRequest.cs`
- `backend/Araponga.Api/Contracts/SocialMedia/SocialMediaConnectionResponse.cs`

**Critérios de Sucesso**:
- ✅ Integração com Instagram funcionando
- ✅ Integração com Facebook funcionando
- ✅ Integração com WhatsApp funcionando
- ✅ Postagem cross-platform funcionando
- ✅ Testes passando

---

### Semana 2-3: Conexão com WhatsApp

#### 22.2 Sistema de Conexão com WhatsApp
**Estimativa**: 60 horas (7.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Configurar WhatsApp Business API:
  - [ ] Criar conta de negócio
  - [ ] Configurar webhook
  - [ ] Obter tokens de acesso
- [ ] Criar `IWhatsAppService` interface:
  - [ ] `SendMessageAsync(string phoneNumber, string message, ...)` → enviar mensagem
  - [ ] `SendTemplateMessageAsync(string phoneNumber, string templateName, ...)` → enviar template
  - [ ] `ReceiveMessageAsync(...)` → receber mensagem (webhook)
- [ ] Implementar `WhatsAppService`:
  - [ ] Integração com WhatsApp Business API
  - [ ] Envio de mensagens
  - [ ] Envio de templates
  - [ ] Recebimento de mensagens (webhook)
- [ ] Integrar com sistema de notificações (Fase 4):
  - [ ] Notificações de eventos via WhatsApp
  - [ ] Notificações de alertas de saúde via WhatsApp
  - [ ] Notificações de mensagens via WhatsApp
- [ ] Criar templates de mensagens:
  - [ ] Template de evento criado
  - [ ] Template de alerta de saúde
  - [ ] Template de confirmação de participação
  - [ ] Template de lembretes
- [ ] Criar `WhatsAppWebhookController`:
  - [ ] `POST /api/v1/whatsapp/webhook` → receber mensagens
- [ ] Criar `WhatsAppController`:
  - [ ] `POST /api/v1/whatsapp/send` → enviar mensagem (admin)
  - [ ] `GET /api/v1/whatsapp/templates` → listar templates
- [ ] Feature flags: `WhatsAppNotificationsEnabled`, `WhatsAppAlertsEnabled`
- [ ] Validações e segurança
- [ ] Testes

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/WhatsAppService.cs`
- `backend/Araponga.Infrastructure/WhatsApp/IWhatsAppClient.cs`
- `backend/Araponga.Infrastructure/WhatsApp/WhatsAppClient.cs`
- `backend/Araponga.Api/Controllers/WhatsAppController.cs`
- `backend/Araponga.Api/Controllers/WhatsAppWebhookController.cs`
- `backend/Araponga.Api/Contracts/WhatsApp/SendMessageRequest.cs`

**Critérios de Sucesso**:
- ✅ Integração com WhatsApp funcionando
- ✅ Notificações via WhatsApp funcionando
- ✅ Webhook funcionando
- ✅ Testes passando

---

### Semana 3-4: Apple Pay e Google Pay

#### 22.3 Sistema de Apple Pay e Google Pay
**Estimativa**: 40 horas (5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Expandir `IPaymentGateway` para suportar Apple Pay/Google Pay:
  - [ ] `ProcessApplePayAsync(...)` → processar Apple Pay
  - [ ] `ProcessGooglePayAsync(...)` → processar Google Pay
- [ ] Implementar `ApplePayGateway`:
  - [ ] Integração via Stripe/PagSeguro
  - [ ] Processamento de pagamentos
  - [ ] Validação de tokens
- [ ] Implementar `GooglePayGateway`:
  - [ ] Integração via Stripe/PagSeguro
  - [ ] Processamento de pagamentos
  - [ ] Validação de tokens
- [ ] Integrar com Fase 6 (Pagamentos):
  - [ ] Adicionar Apple Pay/Google Pay como métodos de pagamento
- [ ] Integrar com Fase 7 (Payout):
  - [ ] Suporte para payout via Apple Pay/Google Pay (se aplicável)
- [ ] Criar endpoints:
  - [ ] `POST /api/v1/payments/apple-pay` → processar Apple Pay
  - [ ] `POST /api/v1/payments/google-pay` → processar Google Pay
- [ ] Testes em dispositivos reais (iOS e Android)
- [ ] Feature flags: `ApplePayEnabled`, `GooglePayEnabled`
- [ ] Validações e segurança
- [ ] Testes

**Arquivos a Criar**:
- `backend/Araponga.Infrastructure/Payments/ApplePayGateway.cs`
- `backend/Araponga.Infrastructure/Payments/GooglePayGateway.cs`
- `backend/Araponga.Api/Controllers/PaymentController.cs` (expandir)
- `backend/Araponga.Api/Contracts/Payments/ApplePayRequest.cs`
- `backend/Araponga.Api/Contracts/Payments/GooglePayRequest.cs`

**Critérios de Sucesso**:
- ✅ Integração com Apple Pay funcionando
- ✅ Integração com Google Pay funcionando
- ✅ Testes em dispositivos reais passando
- ✅ Integração com pagamentos funcionando
- ✅ Testes passando

---

### Semana 4-5: Gestão de Assinaturas Digitais

#### 22.4 Sistema de Assinaturas Digitais
**Estimativa**: 40 horas (5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Escolher serviço de assinatura digital (DocuSign, ClickSign, ou implementação própria):
  - [ ] Avaliar DocuSign
  - [ ] Avaliar ClickSign
  - [ ] Avaliar implementação própria (simples, para documentos internos)
- [ ] Criar `IDigitalSignatureService` interface:
  - [ ] `SignDocumentAsync(Guid documentId, Guid userId, ...)` → assinar documento
  - [ ] `GetSignatureStatusAsync(Guid signatureId)` → obter status
  - [ ] `ListSignaturesAsync(Guid userId, ...)` → listar assinaturas
  - [ ] `GetSignatureHistoryAsync(Guid documentId)` → histórico de assinaturas
- [ ] Implementar `DigitalSignatureService`:
  - [ ] Integração com serviço escolhido
  - [ ] Processamento de assinaturas
  - [ ] Validação de assinaturas
- [ ] Criar modelo `DocumentSignature`:
  - [ ] `Id`, `DocumentId`, `UserId`
  - [ ] `DocumentType` (TERMS, PRIVACY, PROJECT, PETITION, OTHER)
  - [ ] `Status` (PENDING, SIGNED, REJECTED, EXPIRED)
  - [ ] `SignedAtUtc?` (nullable)
  - [ ] `SignatureHash` (hash da assinatura)
  - [ ] `CreatedAtUtc`, `UpdatedAtUtc`
- [ ] Integrar com Fase 12 (Termos):
  - [ ] Assinatura de termos de serviço
  - [ ] Assinatura de políticas de privacidade
- [ ] Criar `DigitalSignatureController`:
  - [ ] `POST /api/v1/signatures/sign` → assinar documento
  - [ ] `GET /api/v1/signatures` → listar assinaturas
  - [ ] `GET /api/v1/signatures/{id}` → obter assinatura
  - [ ] `GET /api/v1/signatures/documents/{documentId}/history` → histórico
- [ ] Feature flags: `DigitalSignaturesEnabled`
- [ ] Validações e segurança
- [ ] Testes

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/DigitalSignatureService.cs`
- `backend/Araponga.Infrastructure/Signatures/IDigitalSignatureClient.cs`
- `backend/Araponga.Infrastructure/Signatures/DocuSignClient.cs` (ou ClickSignClient)
- `backend/Araponga.Domain/Signatures/DocumentSignature.cs`
- `backend/Araponga.Domain/Signatures/DocumentType.cs`
- `backend/Araponga.Domain/Signatures/SignatureStatus.cs`
- `backend/Araponga.Application/Interfaces/IDocumentSignatureRepository.cs`
- `backend/Araponga.Api/Controllers/DigitalSignatureController.cs`
- `backend/Araponga.Api/Contracts/Signatures/SignDocumentRequest.cs`
- `backend/Araponga.Api/Contracts/Signatures/DocumentSignatureResponse.cs`

**Critérios de Sucesso**:
- ✅ Sistema de assinaturas funcionando
- ✅ Integração com termos funcionando
- ✅ Histórico de assinaturas funcionando
- ✅ Testes passando

---

## 📊 Resumo da Fase 22

| Tarefa | Estimativa | Status | Prioridade |
|--------|------------|--------|------------|
| Postagem Cross-Platform | 80h | ❌ Pendente | 🟡 Alta |
| Conexão WhatsApp | 60h | ❌ Pendente | 🟡 Alta |
| Apple Pay / Google Pay | 40h | ❌ Pendente | 🟡 Alta |
| Assinaturas Digitais | 40h | ❌ Pendente | 🟢 Média |
| **Total** | **200h (35 dias)** | | |

---

## ✅ Critérios de Sucesso da Fase 22

### Funcionalidades
- ✅ Postagem cross-platform funcionando (Instagram, Facebook, WhatsApp)
- ✅ Conexão com WhatsApp funcionando
- ✅ Notificações via WhatsApp funcionando
- ✅ Apple Pay funcionando
- ✅ Google Pay funcionando
- ✅ Sistema de assinaturas digitais funcionando

### Qualidade
- ✅ Testes com cobertura adequada
- ✅ Documentação completa
- ✅ Feature flags implementados
- ✅ Validações e segurança implementadas
- Considerar **Testcontainers + PostgreSQL** para testes de integração (postagem cross-platform, assinaturas, persistência) com banco real (estratégia na Fase 19; [TESTCONTAINERS_POSTGRES_IMPACTO](../../TESTCONTAINERS_POSTGRES_IMPACTO.md)).

### Integração
- ✅ Integração com Fase 6 (Pagamentos) funcionando
- ✅ Integração com Fase 7 (Payout) funcionando
- ✅ Integração com Fase 12 (Termos) funcionando
- ✅ Integração com Fase 13 (Emails) funcionando

---

## 🔗 Dependências

- **Fase 6**: Pagamentos (base para Apple Pay/Google Pay)
- **Fase 12**: Termos (integração com assinaturas)
- **Fase 13**: Emails (notificações)

---

## 📝 Notas de Implementação

### Privacidade e Soberania

**Opt-in**:
- Todas as integrações são opt-in
- Usuário escolhe se quer conectar contas
- Usuário escolhe se quer publicar em redes sociais

**Privacidade Territorial**:
- Não compartilhar conteúdo RESIDENTS_ONLY
- Respeitar preferências de privacidade do usuário
- Notificar usuário sobre o que está sendo compartilhado

**Feed Cronológico**:
- Feed do Araponga permanece principal
- Postagem cross-platform é opcional
- Não substitui funcionalidades principais

### Manutenção

**APIs Externas**:
- Documentar todas as APIs externas
- Versionamento de integrações
- Fallbacks quando APIs externas falharem
- Monitoramento de saúde das integrações

**Tokens e Segurança**:
- Tokens armazenados criptografados
- Refresh tokens quando disponível
- Expiração de tokens tratada
- Revogação de tokens quando usuário desconecta

### Conformidade

**LGPD**:
- Consentimento explícito para compartilhamento
- Usuário pode revogar consentimento
- Auditoria de compartilhamento de dados

**Termos de Uso**:
- Conformidade com termos das plataformas externas
- Respeitar limites de API
- Rate limiting para evitar abuso

---

**Status**: ⏳ **FASE 22 PENDENTE**  
**Depende de**: Fases 6, 12, 13  
**Crítico para**: Transição e Adoção
