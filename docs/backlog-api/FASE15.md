# Fase 15: Subscriptions & Recurring Payments

**Duração**: 12 semanas (60 dias úteis)  
**Prioridade**: 🔴 CRÍTICA (Sustentabilidade financeira)  
**Depende de**: Fase 6 (Pagamentos), Fase 7 (Payout)  
**Estimativa Total**: 480 horas  
**Status**: ⏳ Pendente  
**Nota**: Esta fase está na Onda 1 (Fundação de Governança e Sustentabilidade), antes da Fase 16 (Finalização). Crítica para sustentabilidade financeira da plataforma.

---

## 🎯 Objetivo

Implementar sistema completo de **assinaturas e pagamentos recorrentes** que:
- **Garante acesso básico gratuito** para visitantes e residentes (alinhado com valores da plataforma)
- Permite criação de planos de assinatura (tiers: FREE, Básico, Intermediário, Premium)
- **Libera funcionalidades progressivamente** conforme o plano do usuário
- Gerencia pagamentos recorrentes automáticos
- Integra com gateway de pagamento (Stripe Subscriptions)
- Processa webhooks para renovações, cancelamentos e falhas
- Fornece dashboard de assinantes para administradores
- Suporta upgrades/downgrades de planos
- Implementa períodos de trial (opcional)
- Gerencia cupons e descontos

**Princípios**:
- ✅ **Acesso Básico Gratuito**: Funcionalidades essenciais sempre disponíveis (feed, posts básicos, eventos, marketplace básico)
- ✅ **Inclusão**: Ninguém é excluído por não poder pagar
- ✅ **Transparência**: Usuário sempre sabe o status da assinatura e funcionalidades disponíveis
- ✅ **Flexibilidade**: Múltiplos planos e opções de pagamento
- ✅ **Confiabilidade**: Processamento robusto de renovações
- ✅ **Sustentabilidade**: Base para receitas recorrentes sem excluir usuários

---

## 📋 Contexto e Requisitos

### Estado Atual
- ✅ Sistema de pagamentos básico (Fase 6)
- ✅ Sistema de payout (Fase 7)
- ✅ Integração com gateway de pagamento (Stripe)
- ❌ Não existe sistema de assinaturas
- ❌ Não existe pagamentos recorrentes
- ❌ Não existe gestão de planos

### Requisitos Funcionais

#### 1. Planos de Assinatura
- ✅ **Plano FREE (Gratuito)**: Padrão para visitantes e residentes
  - ✅ Funcionalidades básicas sempre disponíveis (feed, posts, eventos, marketplace básico)
  - ✅ Limites razoáveis para uso básico
  - ✅ Sem necessidade de pagamento
  - ✅ **Global por padrão**, mas pode ser customizado por território
- ✅ Criar planos pagos (tiers: Básico, Intermediário, Premium)
- ✅ **Planos Globais**: Aplicam a todos os territórios (SystemAdmin)
- ✅ **Planos Territoriais**: Específicos de um território (Curadores podem gerenciar)
- ✅ Definir preços e ciclos (mensal, trimestral, anual)
- ✅ **Sistema de liberação de funcionalidades** por plano
- ✅ Definir recursos/limites por plano (capacidade de funcionalidades)
- ✅ Ativar/desativar planos
- ✅ **Hierarquia**: Planos territoriais sobrescrevem planos globais quando existem

#### 2. Gestão de Assinaturas
- ✅ Criar assinatura para usuário
- ✅ Atualizar assinatura (upgrade/downgrade)
- ✅ Cancelar assinatura
- ✅ Reativar assinatura cancelada
- ✅ Histórico de assinaturas

#### 3. Pagamentos Recorrentes
- ✅ Processar renovações automáticas
- ✅ Lidar com falhas de pagamento
- ✅ Retry automático de pagamentos falhos
- ✅ Notificações de falhas
- ✅ Suspensão automática após múltiplas falhas

#### 4. Webhooks e Integrações
- ✅ Webhooks do gateway (Stripe)
- ✅ Processar eventos: subscription.created, subscription.updated, subscription.deleted
- ✅ Processar eventos: invoice.payment_succeeded, invoice.payment_failed
- ✅ Sincronização de status

#### 5. Períodos de Trial
- ✅ Trial gratuito (opcional)
- ✅ Duração configurável
- ✅ Conversão automática ao final do trial
- ✅ Notificações antes do fim do trial

#### 6. Cupons e Descontos
- ✅ Criar cupons de desconto
- ✅ Aplicar cupons a assinaturas
- ✅ Descontos percentuais ou fixos
- ✅ Validade e limites de uso

#### 7. Dashboard e Relatórios
- ✅ Dashboard de assinantes
- ✅ Métricas de receita recorrente (MRR)
- ✅ Taxa de churn
- ✅ Assinaturas ativas/canceladas
- ✅ Relatórios exportáveis

#### 8. Sistema de Liberação de Funcionalidades
- ✅ Verificar plano do usuário antes de acessar funcionalidades
- ✅ Bloquear funcionalidades premium para planos gratuitos
- ✅ Mostrar mensagens educativas sobre upgrade
- ✅ Feature flags por plano (integração com sistema existente)
- ✅ API para verificar permissões de funcionalidades

#### 9. Sistema Administrativo de Planos e Funcionalidades
- ✅ **Interface administrativa** para criar/editar planos
  - ✅ **SystemAdmin**: Pode criar/editar planos globais e de qualquer território
  - ✅ **Curadores**: Podem criar/editar planos do seu território
- ✅ **Planos configuráveis por território**:
  - ✅ Planos globais (aplicam a todos os territórios)
  - ✅ Planos territoriais (específicos de um território)
  - ✅ Hierarquia: Planos territoriais sobrescrevem globais
- ✅ **Seleção de funcionalidades** por plano (checkboxes, seleção múltipla)
- ✅ **Definição de valores** por plano (preço, ciclos de cobrança)
- ✅ **Ativar/desativar planos** sem deletar
- ✅ **Validação de integridade**: Garantir que funcionalidades básicas sempre estejam no FREE (global e territorial)
- ✅ **Gerenciamento de cupons** via interface administrativa (global e territorial)
- ✅ **Regras de negócio**: Validações automáticas para manter consistência
- ✅ **Histórico de mudanças** em planos (auditoria)

---

## 📋 Tarefas Detalhadas

### Semana 1-2: Modelo de Domínio e Planos

#### 15.1 Modelo de Domínio - Assinaturas
**Estimativa**: 32 horas (4 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar enum `SubscriptionPlanTier`:
  - [ ] `FREE = 0` (gratuito - padrão para visitantes e residentes)
  - [ ] `BASIC = 1` (básico pago)
  - [ ] `INTERMEDIATE = 2` (intermediário pago)
  - [ ] `PREMIUM = 3` (premium pago)
  - [ ] `ENTERPRISE = 4` (empresarial, custom)
- [ ] Criar enum `FeatureCapability` (capacidades de funcionalidades):
  - [ ] `FeedBasic` (feed básico - sempre no FREE)
  - [ ] `PostsBasic` (posts básicos - sempre no FREE)
  - [ ] `PostsUnlimited` (posts ilimitados)
  - [ ] `EventsBasic` (eventos básicos - sempre no FREE)
  - [ ] `EventsUnlimited` (eventos ilimitados)
  - [ ] `MarketplaceBasic` (marketplace básico - sempre no FREE)
  - [ ] `MarketplaceAdvanced` (marketplace avançado)
  - [ ] `ChatBasic` (chat básico - sempre no FREE)
  - [ ] `Analytics` (analytics e métricas)
  - [ ] `AIIntegration` (integração com IA)
  - [ ] `PrioritySupport` (suporte prioritário)
  - [ ] `CustomBranding` (branding customizado)
  - [ ] `APIAccess` (acesso à API)
  - [ ] `AdvancedGovernance` (governança avançada)
  - [ ] `TerritoryPremium` (recursos premium territoriais)
- [ ] Criar enum `FeatureCategory` (categorias de funcionalidades):
  - [ ] `Core` (funcionalidades core - sempre no FREE)
  - [ ] `Enhanced` (funcionalidades melhoradas)
  - [ ] `Premium` (funcionalidades premium)
  - [ ] `Enterprise` (funcionalidades empresariais)
- [ ] Criar enum `SubscriptionBillingCycle`:
  - [ ] `MONTHLY` (mensal)
  - [ ] `QUARTERLY` (trimestral)
  - [ ] `YEARLY` (anual)
- [ ] Criar enum `SubscriptionStatus`:
  - [ ] `ACTIVE` (ativa)
  - [ ] `CANCELED` (cancelada)
  - [ ] `PAST_DUE` (atrasada)
  - [ ] `UNPAID` (não paga)
  - [ ] `TRIALING` (em trial)
  - [ ] `EXPIRED` (expirada)
- [ ] Criar enum `PlanScope`:
  - [ ] `Global = 1` (plano global, aplica a todos os territórios)
  - [ ] `Territory = 2` (plano territorial, específico de um território)
- [ ] Criar modelo `SubscriptionPlan`:
  - [ ] `Id`, `Name`, `Description`
  - [ ] `Tier` (SubscriptionPlanTier)
  - [ ] `Scope` (PlanScope, Global ou Territory)
  - [ ] `TerritoryId?` (Guid?, nullable, se Scope = Territory, ID do território)
  - [ ] `PricePerCycle` (decimal, nullable para FREE, preço por ciclo)
  - [ ] `BillingCycle` (SubscriptionBillingCycle, nullable para FREE)
  - [ ] `Features` (JSON, recursos/limites do plano)
  - [ ] `Capabilities` (List<FeatureCapability>, funcionalidades liberadas)
  - [ ] `Limits` (JSON, limites específicos: maxPosts, maxEvents, maxStorage, etc.)
  - [ ] `IsDefault` (bool, se é o plano padrão - FREE sempre é default)
  - [ ] `TrialDays?` (int?, nullable, dias de trial - apenas para planos pagos)
  - [ ] `IsActive` (bool, pode ser desativado mas não deletado)
  - [ ] `CreatedByUserId` (Guid, quem criou o plano)
  - [ ] `StripePriceId?` (string?, nullable, ID do preço no Stripe - apenas para planos pagos)
  - [ ] `StripeProductId?` (string?, nullable, ID do produto no Stripe - apenas para planos pagos)
  - [ ] `CreatedAtUtc`, `UpdatedAtUtc`
- [ ] Criar modelo `SubscriptionPlanHistory` (auditoria):
  - [ ] `Id`, `PlanId`, `ChangedByUserId`
  - [ ] `ChangeType` (enum: Created, Updated, Activated, Deactivated, CapabilitiesChanged, PriceChanged)
  - [ ] `PreviousState` (JSON, estado anterior)
  - [ ] `NewState` (JSON, novo estado)
  - [ ] `ChangeReason?` (string?, nullable, motivo da mudança)
  - [ ] `ChangedAtUtc`
- [ ] Criar modelo `Subscription`:
  - [ ] `Id`, `UserId`, `TerritoryId?` (nullable, para assinaturas territoriais)
  - [ ] `PlanId` (SubscriptionPlan)
  - [ ] `Status` (SubscriptionStatus)
  - [ ] `CurrentPeriodStart` (DateTime)
  - [ ] `CurrentPeriodEnd` (DateTime)
  - [ ] `TrialStart?` (DateTime?, nullable)
  - [ ] `TrialEnd?` (DateTime?, nullable)
  - [ ] `CanceledAt?` (DateTime?, nullable)
  - [ ] `CancelAtPeriodEnd` (bool, cancelar ao fim do período)
  - [ ] `StripeSubscriptionId?` (string?, nullable, ID no Stripe)
  - [ ] `StripeCustomerId?` (string?, nullable, ID do cliente no Stripe)
  - [ ] `CreatedAtUtc`, `UpdatedAtUtc`
- [ ] Criar modelo `SubscriptionPayment`:
  - [ ] `Id`, `SubscriptionId`
  - [ ] `Amount` (decimal)
  - [ ] `Currency` (string, padrão: BRL)
  - [ ] `Status` (enum: Pending, Succeeded, Failed, Refunded)
  - [ ] `PaymentDate` (DateTime)
  - [ ] `PeriodStart` (DateTime)
  - [ ] `PeriodEnd` (DateTime)
  - [ ] `StripeInvoiceId?` (string?, nullable)
  - [ ] `StripePaymentIntentId?` (string?, nullable)
  - [ ] `FailureReason?` (string?, nullable)
  - [ ] `CreatedAtUtc`, `UpdatedAtUtc`
- [ ] Criar modelo `Coupon`:
  - [ ] `Id`, `Code` (string, único)
  - [ ] `Name`, `Description`
  - [ ] `DiscountType` (enum: Percentage, FixedAmount)
  - [ ] `DiscountValue` (decimal)
  - [ ] `ValidFrom` (DateTime)
  - [ ] `ValidUntil?` (DateTime?, nullable)
  - [ ] `MaxUses?` (int?, nullable, máximo de usos)
  - [ ] `UsedCount` (int, contador de usos)
  - [ ] `IsActive` (bool)
  - [ ] `StripeCouponId?` (string?, nullable)
  - [ ] `CreatedAtUtc`, `UpdatedAtUtc`
- [ ] Criar modelo `SubscriptionCoupon`:
  - [ ] `Id`, `SubscriptionId`, `CouponId`
  - [ ] `AppliedAtUtc` (DateTime)
- [ ] Criar repositórios
- [ ] Criar migrations

**Arquivos a Criar**:
- `backend/Araponga.Domain/Subscriptions/SubscriptionPlan.cs`
- `backend/Araponga.Domain/Subscriptions/SubscriptionPlanTier.cs`
- `backend/Araponga.Domain/Subscriptions/PlanScope.cs`
- `backend/Araponga.Domain/Subscriptions/SubscriptionBillingCycle.cs`
- `backend/Araponga.Domain/Subscriptions/Subscription.cs`
- `backend/Araponga.Domain/Subscriptions/SubscriptionStatus.cs`
- `backend/Araponga.Domain/Subscriptions/SubscriptionPayment.cs`
- `backend/Araponga.Domain/Subscriptions/Coupon.cs`
- `backend/Araponga.Domain/Subscriptions/SubscriptionCoupon.cs`
- `backend/Araponga.Application/Interfaces/ISubscriptionPlanRepository.cs`
- `backend/Araponga.Application/Interfaces/ISubscriptionPlanRepository.cs` (métodos: `GetGlobalPlansAsync`, `GetTerritoryPlansAsync`, `GetPlansForTerritoryAsync`)
- `backend/Araponga.Application/Interfaces/ISubscriptionRepository.cs`
- `backend/Araponga.Application/Interfaces/ISubscriptionPaymentRepository.cs`
- `backend/Araponga.Application/Interfaces/ICouponRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresSubscriptionPlanRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresSubscriptionRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresSubscriptionPaymentRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresCouponRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/Migrations/YYYYMMDDHHMMSS_AddSubscriptionsSystem.cs`

**Critérios de Sucesso**:
- ✅ Modelos criados
- ✅ Repositórios implementados
- ✅ Migrations aplicadas
- ✅ Testes de repositório passando

---

### Semana 2-3: Integração com Stripe

#### 15.2 Integração com Stripe Subscriptions
**Estimativa**: 40 horas (5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Instalar pacote `Stripe.net`
- [ ] Criar `StripeSubscriptionService`:
  - [ ] `CreateSubscriptionAsync(Guid userId, Guid planId, string? couponCode, CancellationToken)` → criar assinatura no Stripe
  - [ ] `UpdateSubscriptionAsync(Guid subscriptionId, Guid newPlanId, CancellationToken)` → atualizar plano
  - [ ] `CancelSubscriptionAsync(Guid subscriptionId, bool cancelAtPeriodEnd, CancellationToken)` → cancelar assinatura
  - [ ] `ReactivateSubscriptionAsync(Guid subscriptionId, CancellationToken)` → reativar assinatura
  - [ ] `GetSubscriptionAsync(string stripeSubscriptionId, CancellationToken)` → obter assinatura do Stripe
- [ ] Sincronização com Stripe:
  - [ ] Criar produtos e preços no Stripe ao criar plano
  - [ ] Sincronizar status de assinaturas
  - [ ] Sincronizar pagamentos
- [ ] Configuração:
  - [ ] `Stripe:SecretKey` (secret)
  - [ ] `Stripe:PublishableKey` (config)
  - [ ] `Stripe:WebhookSecret` (secret, para validação de webhooks)
- [ ] Tratamento de erros:
  - [ ] Rate limits
  - [ ] Timeouts
  - [ ] Retry policy
- [ ] Testes unitários

**Arquivos a Criar**:
- `backend/Araponga.Infrastructure/Payments/StripeSubscriptionService.cs`
- `backend/Araponga.Infrastructure/Payments/StripeConfiguration.cs`
- `backend/Araponga.Tests/Infrastructure/StripeSubscriptionServiceTests.cs`

**Arquivos a Modificar**:
- `backend/Araponga.Api/Program.cs` (registrar serviço)
- `backend/Araponga.Api/appsettings.json` (adicionar configuração Stripe)

**Critérios de Sucesso**:
- ✅ Integração Stripe funcionando
- ✅ Criação de assinaturas funcionando
- ✅ Atualização de assinaturas funcionando
- ✅ Cancelamento funcionando
- ✅ Sincronização funcionando
- ✅ Testes passando

---

#### 15.3 Webhooks do Stripe
**Estimativa**: 32 horas (4 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `StripeWebhookController`:
  - [ ] `POST /api/v1/webhooks/stripe` → receber webhooks
  - [ ] Validação de assinatura (usar `StripeSignature`)
  - [ ] Processar eventos assincronamente
- [ ] Processar eventos:
  - [ ] `customer.subscription.created` → criar assinatura local
  - [ ] `customer.subscription.updated` → atualizar assinatura
  - [ ] `customer.subscription.deleted` → cancelar assinatura
  - [ ] `invoice.payment_succeeded` → registrar pagamento
  - [ ] `invoice.payment_failed` → marcar pagamento como falho
  - [ ] `customer.subscription.trial_will_end` → notificar fim do trial
- [ ] Background jobs para processar eventos:
  - [ ] Usar Hangfire ou similar
  - [ ] Retry automático em caso de falha
- [ ] Logging e auditoria:
  - [ ] Registrar todos os eventos recebidos
  - [ ] Logging de erros
- [ ] Testes de integração

**Arquivos a Criar**:
- `backend/Araponga.Api/Controllers/StripeWebhookController.cs`
- `backend/Araponga.Application/Services/StripeWebhookService.cs`
- `backend/Araponga.Application/BackgroundJobs/StripeWebhookProcessingJob.cs`
- `backend/Araponga.Tests/Integration/StripeWebhookIntegrationTests.cs`

**Arquivos a Modificar**:
- `backend/Araponga.Api/Program.cs` (configurar webhook endpoint)

**Critérios de Sucesso**:
- ✅ Webhooks sendo recebidos
- ✅ Eventos sendo processados
- ✅ Assinaturas sendo sincronizadas
- ✅ Pagamentos sendo registrados
- ✅ Testes passando

---

### Semana 3-4: Serviços de Assinatura

#### 15.4 Serviço de Assinaturas
**Estimativa**: 40 horas (5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `SubscriptionService`:
  - [ ] `GetOrCreateUserSubscriptionAsync(Guid userId, Guid? territoryId, CancellationToken)` → obter ou criar assinatura FREE (padrão)
  - [ ] `GetAvailablePlansForTerritoryAsync(Guid territoryId, CancellationToken)` → obter planos disponíveis (territoriais + globais)
  - [ ] `CreateSubscriptionAsync(Guid userId, Guid territoryId, Guid planId, string? couponCode, CancellationToken)` → criar assinatura paga
  - [ ] `UpdateSubscriptionAsync(Guid subscriptionId, Guid newPlanId, CancellationToken)` → atualizar plano (upgrade/downgrade)
  - [ ] `CancelSubscriptionAsync(Guid subscriptionId, bool cancelAtPeriodEnd, CancellationToken)` → cancelar assinatura (volta para FREE)
  - [ ] `ReactivateSubscriptionAsync(Guid subscriptionId, CancellationToken)` → reativar assinatura
  - [ ] `GetUserSubscriptionAsync(Guid userId, Guid? territoryId, CancellationToken)` → obter assinatura do usuário (retorna FREE se não tiver pago)
  - [ ] `GetSubscriptionAsync(Guid subscriptionId, CancellationToken)` → obter assinatura
  - [ ] `ListSubscriptionsAsync(Guid? userId, Guid? territoryId, SubscriptionStatus? status, CancellationToken)` → listar assinaturas
- [ ] **Resolução de Planos por Território**:
  - [ ] Ao buscar planos, verificar primeiro planos territoriais
  - [ ] Se não houver plano territorial, usar plano global
  - [ ] FREE sempre disponível (global e pode ter versão territorial)
- [ ] **Atribuição automática de plano FREE**:
  - [ ] Ao criar novo usuário, atribuir automaticamente plano FREE global
  - [ ] Se território tem FREE customizado, usar ele quando usuário interagir com território
  - [ ] Se usuário não tem assinatura, considerar como FREE
  - [ ] FREE não precisa de registro no Stripe (é local apenas)
- [ ] Lógica de upgrade/downgrade:
  - [ ] Calcular proratação (crédito/débito)
  - [ ] Aplicar desconto proporcional
  - [ ] Atualizar período atual
- [ ] Lógica de cancelamento:
  - [ ] Cancelar imediatamente ou ao fim do período
  - [ ] Manter acesso até fim do período (se `cancelAtPeriodEnd = true`)
  - [ ] Notificar usuário
- [ ] Validações:
  - [ ] Usuário não pode ter múltiplas assinaturas ativas (exceto FREE que é implícito)
  - [ ] Plano deve estar ativo
  - [ ] Cupom deve ser válido (se fornecido)
  - [ ] FREE não pode ser cancelado (é o estado padrão)
  - [ ] Ao cancelar assinatura paga, voltar para FREE automaticamente
- [ ] Testes unitários

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/SubscriptionService.cs`
- `backend/Araponga.Tests/Application/SubscriptionServiceTests.cs`

**Arquivos a Modificar**:
- `backend/Araponga.Application/Services/StripeSubscriptionService.cs` (integrar)

**Critérios de Sucesso**:
- ✅ Criação de assinaturas funcionando
- ✅ Upgrade/downgrade funcionando
- ✅ Cancelamento funcionando
- ✅ Reativação funcionando
- ✅ Validações funcionando
- ✅ Testes passando

---

#### 15.5 Serviço de Cupons
**Estimativa**: 24 horas (3 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `CouponService`:
  - [ ] `CreateCouponAsync(Coupon coupon, CancellationToken)` → criar cupom
  - [ ] `ValidateCouponAsync(string code, CancellationToken)` → validar cupom
  - [ ] `ApplyCouponToSubscriptionAsync(Guid subscriptionId, string couponCode, CancellationToken)` → aplicar cupom
  - [ ] `RemoveCouponFromSubscriptionAsync(Guid subscriptionId, CancellationToken)` → remover cupom
  - [ ] `ListCouponsAsync(bool? isActive, CancellationToken)` → listar cupons
- [ ] Validação de cupons:
  - [ ] Verificar validade (data)
  - [ ] Verificar limite de usos
  - [ ] Verificar se está ativo
- [ ] Integração com Stripe:
  - [ ] Criar cupom no Stripe ao criar localmente
  - [ ] Aplicar cupom na assinatura do Stripe
- [ ] Testes

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/CouponService.cs`
- `backend/Araponga.Tests/Application/CouponServiceTests.cs`

**Critérios de Sucesso**:
- ✅ Criação de cupons funcionando
- ✅ Validação funcionando
- ✅ Aplicação de cupons funcionando
- ✅ Integração com Stripe funcionando
- ✅ Testes passando

---

### Semana 4-5: Processamento de Pagamentos e Renovações

#### 15.6 Processamento de Renovações
**Estimativa**: 32 horas (4 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Background job para processar renovações:
  - [ ] Verificar assinaturas próximas do fim do período
  - [ ] Processar renovação via Stripe
  - [ ] Atualizar período da assinatura
  - [ ] Registrar pagamento
- [ ] Lógica de falhas de pagamento:
  - [ ] Detectar falha de pagamento (via webhook)
  - [ ] Retry automático (configurável: 3 tentativas)
  - [ ] Notificar usuário após cada falha
  - [ ] Suspender assinatura após múltiplas falhas
- [ ] Notificações:
  - [ ] Notificar antes do fim do período (7 dias, 3 dias, 1 dia)
  - [ ] Notificar sobre falhas de pagamento
  - [ ] Notificar sobre suspensão
- [ ] Testes

**Arquivos a Criar**:
- `backend/Araponga.Application/BackgroundJobs/SubscriptionRenewalJob.cs`
- `backend/Araponga.Application/Services/SubscriptionRenewalService.cs`
- `backend/Araponga.Tests/Application/SubscriptionRenewalServiceTests.cs`

**Critérios de Sucesso**:
- ✅ Renovações automáticas funcionando
- ✅ Retry de falhas funcionando
- ✅ Notificações funcionando
- ✅ Suspensão automática funcionando
- ✅ Testes passando

---

#### 15.7 Gestão de Trials
**Estimativa**: 24 horas (3 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Lógica de trial:
  - [ ] Iniciar trial ao criar assinatura (se plano tem trial)
  - [ ] Calcular data de fim do trial
  - [ ] Converter automaticamente ao fim do trial
  - [ ] Notificar antes do fim do trial (3 dias, 1 dia)
- [ ] Integração com Stripe:
  - [ ] Criar assinatura com trial no Stripe
  - [ ] Processar conversão ao fim do trial
- [ ] Validações:
  - [ ] Usuário só pode ter um trial por plano
  - [ ] Trial não pode ser aplicado a assinatura existente
- [ ] Testes

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/SubscriptionTrialService.cs`
- `backend/Araponga.Tests/Application/SubscriptionTrialServiceTests.cs`

**Arquivos a Modificar**:
- `backend/Araponga.Application/Services/SubscriptionService.cs` (integrar trial)

**Critérios de Sucesso**:
- ✅ Trials funcionando
- ✅ Conversão automática funcionando
- ✅ Notificações funcionando
- ✅ Validações funcionando
- ✅ Testes passando

---

### Semana 5-6: Sistema Administrativo e Validações

#### 15.8 Sistema Administrativo de Planos e Funcionalidades
**Estimativa**: 40 horas (5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `SubscriptionPlanAdminService`:
  - [ ] `CreateGlobalPlanAsync(Guid adminUserId, CreatePlanRequest request, CancellationToken)` → criar plano global (SystemAdmin)
  - [ ] `CreateTerritoryPlanAsync(Guid territoryId, Guid curatorUserId, CreatePlanRequest request, CancellationToken)` → criar plano territorial (Curador)
  - [ ] `UpdatePlanAsync(Guid planId, Guid userId, UpdatePlanRequest request, CancellationToken)` → atualizar plano (valida permissões)
  - [ ] `UpdatePlanCapabilitiesAsync(Guid planId, Guid userId, List<FeatureCapability> capabilities, CancellationToken)` → atualizar funcionalidades
  - [ ] `UpdatePlanLimitsAsync(Guid planId, Guid userId, Dictionary<string, object> limits, CancellationToken)` → atualizar limites
  - [ ] `ActivatePlanAsync(Guid planId, Guid userId, CancellationToken)` → ativar plano
  - [ ] `DeactivatePlanAsync(Guid planId, Guid userId, string? reason, CancellationToken)` → desativar plano
  - [ ] `GetPlansForTerritoryAsync(Guid territoryId, CancellationToken)` → obter planos disponíveis para território (globais + territoriais)
  - [ ] `GetPlanHistoryAsync(Guid planId, CancellationToken)` → obter histórico de mudanças
- [ ] **Validação de Permissões**:
  - [ ] SystemAdmin pode criar/editar planos globais e de qualquer território
  - [ ] Curadores podem criar/editar apenas planos do seu território
  - [ ] Validar permissões antes de qualquer operação
- [ ] **Validações de Integridade**:
  - [ ] `ValidatePlanIntegrityAsync(SubscriptionPlan plan, CancellationToken)` → validar integridade do plano
  - [ ] Garantir que funcionalidades básicas (`FeedBasic`, `PostsBasic`, `EventsBasic`, `MarketplaceBasic`, `ChatBasic`) **sempre** estejam no plano FREE (global e territorial)
  - [ ] Impedir remoção de funcionalidades básicas do FREE
  - [ ] Validar que planos pagos não tenham preço zero
  - [ ] Validar que FREE sempre tenha preço zero
  - [ ] Validar que FREE global não pode ser desativado
  - [ ] Validar que FREE territorial pode ser desativado (mas não deletado)
  - [ ] Validar que FREE não pode ser deletado (global ou territorial)
  - [ ] Validar limites razoáveis (não permitir limites muito restritivos)
  - [ ] Validar que território existe (se plano territorial)
  - [ ] Validar que não há conflito de nomes (mesmo nome no mesmo território)
- [ ] **Regras de Negócio**:
  - [ ] Ao criar plano, validar integridade antes de salvar
  - [ ] Ao atualizar plano, verificar se quebra funcionalidades básicas
  - [ ] Ao desativar plano, verificar se há assinaturas ativas (aviso, não bloquear)
  - [ ] **Hierarquia de Planos**: Ao buscar planos para território, retornar planos territoriais primeiro, depois globais
  - [ ] **Resolução de Planos**: Se território tem plano customizado, usar ele; senão, usar plano global
  - [ ] Registrar todas as mudanças em `SubscriptionPlanHistory`
- [ ] **Sistema de Funcionalidades**:
  - [ ] `GetAvailableCapabilitiesAsync(CancellationToken)` → listar todas as funcionalidades disponíveis
  - [ ] `GetCapabilityInfoAsync(FeatureCapability capability, CancellationToken)` → informações sobre funcionalidade
  - [ ] `ValidateCapabilitySelectionAsync(List<FeatureCapability> capabilities, SubscriptionPlanTier tier, CancellationToken)` → validar seleção
- [ ] Testes unitários e de integração

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/SubscriptionPlanAdminService.cs`
- `backend/Araponga.Application/Services/SubscriptionPlanValidationService.cs`
- `backend/Araponga.Application/Models/SubscriptionPlanHistory.cs`
- `backend/Araponga.Application/Interfaces/ISubscriptionPlanHistoryRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresSubscriptionPlanHistoryRepository.cs`
- `backend/Araponga.Tests/Application/SubscriptionPlanAdminServiceTests.cs`
- `backend/Araponga.Tests/Application/SubscriptionPlanValidationServiceTests.cs`

**Critérios de Sucesso**:
- ✅ Criação de planos customizados funcionando
- ✅ Seleção de funcionalidades funcionando
- ✅ Validações de integridade funcionando
- ✅ Funcionalidades básicas sempre protegidas no FREE
- ✅ Histórico de mudanças sendo registrado
- ✅ Testes passando

---

#### 15.9 Sistema Administrativo de Cupons
**Estimativa**: 24 horas (3 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `CouponAdminService`:
  - [ ] `CreateCouponAsync(Guid adminUserId, CreateCouponRequest request, CancellationToken)` → criar cupom
  - [ ] `UpdateCouponAsync(Guid couponId, Guid adminUserId, UpdateCouponRequest request, CancellationToken)` → atualizar cupom
  - [ ] `ActivateCouponAsync(Guid couponId, Guid adminUserId, CancellationToken)` → ativar cupom
  - [ ] `DeactivateCouponAsync(Guid couponId, Guid adminUserId, CancellationToken)` → desativar cupom
  - [ ] `GetCouponUsageStatsAsync(Guid couponId, CancellationToken)` → estatísticas de uso
  - [ ] `ListCouponsAsync(bool? isActive, CancellationToken)` → listar cupons
- [ ] Validações:
  - [ ] Código único
  - [ ] Validade de datas
  - [ ] Limites de uso
  - [ ] Desconto válido (percentual 0-100%, valor fixo positivo)
- [ ] Integração com Stripe:
  - [ ] Criar cupom no Stripe ao criar localmente
  - [ ] Sincronizar status
- [ ] Testes

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/CouponAdminService.cs`
- `backend/Araponga.Tests/Application/CouponAdminServiceTests.cs`

**Critérios de Sucesso**:
- ✅ Criação de cupons funcionando
- ✅ Gerenciamento de cupons funcionando
- ✅ Validações funcionando
- ✅ Integração com Stripe funcionando
- ✅ Testes passando

---

### Semana 6-7: Controllers e API

#### 15.10 Sistema de Verificação de Funcionalidades
**Estimativa**: 24 horas (3 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `SubscriptionCapabilityService`:
  - [ ] `CheckCapabilityAsync(Guid userId, Guid? territoryId, FeatureCapability capability, CancellationToken)` → verificar se usuário tem capacidade (considera plano territorial ou global)
  - [ ] `GetUserCapabilitiesAsync(Guid userId, Guid? territoryId, CancellationToken)` → obter todas as capacidades do usuário
  - [ ] `CheckLimitAsync(Guid userId, Guid? territoryId, string limitType, int requestedAmount, CancellationToken)` → verificar limite
  - [ ] `GetUserLimitsAsync(Guid userId, Guid? territoryId, CancellationToken)` → obter limites do usuário
  - [ ] `ResolveUserPlanAsync(Guid userId, Guid? territoryId, CancellationToken)` → resolver plano do usuário (territorial ou global)
- [ ] Integrar com sistema de feature flags existente:
  - [ ] Verificar plano antes de liberar funcionalidades
  - [ ] Mensagens educativas sobre upgrade quando necessário
- [ ] Middleware para verificação automática:
  - [ ] Atributo `[RequiresCapability(FeatureCapability.X)]` para endpoints
  - [ ] Retornar 403 com mensagem educativa se não tiver capacidade
- [ ] Validações:
  - [ ] FREE sempre tem acesso às funcionalidades básicas
  - [ ] Não bloquear funcionalidades essenciais
- [ ] Testes unitários

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/SubscriptionCapabilityService.cs`
- `backend/Araponga.Application/Attributes/RequiresCapabilityAttribute.cs`
- `backend/Araponga.Api/Middleware/SubscriptionCapabilityMiddleware.cs`
- `backend/Araponga.Tests/Application/SubscriptionCapabilityServiceTests.cs`

**Critérios de Sucesso**:
- ✅ Verificação de capacidades funcionando
- ✅ Limites sendo respeitados
- ✅ Mensagens educativas funcionando
- ✅ Funcionalidades básicas sempre acessíveis
- ✅ Testes passando

---

#### 15.11 Controllers Administrativos
**Estimativa**: 32 horas (4 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `AdminSubscriptionPlansController` (SystemAdmin):
  - [ ] `GET /api/v1/admin/subscription-plans` → listar todos os planos (globais e territoriais)
  - [ ] `GET /api/v1/admin/subscription-plans/global` → listar apenas planos globais
  - [ ] `GET /api/v1/admin/subscription-plans/territory/{territoryId}` → listar planos de um território
  - [ ] `GET /api/v1/admin/subscription-plans/{id}` → obter plano detalhado
  - [ ] `POST /api/v1/admin/subscription-plans/global` → criar plano global
  - [ ] `POST /api/v1/admin/subscription-plans/territory/{territoryId}` → criar plano territorial (SystemAdmin pode criar para qualquer território)
  - [ ] `PUT /api/v1/admin/subscription-plans/{id}` → atualizar plano
  - [ ] `PATCH /api/v1/admin/subscription-plans/{id}/capabilities` → atualizar funcionalidades do plano
  - [ ] `PATCH /api/v1/admin/subscription-plans/{id}/limits` → atualizar limites do plano
  - [ ] `PATCH /api/v1/admin/subscription-plans/{id}/activate` → ativar plano
  - [ ] `PATCH /api/v1/admin/subscription-plans/{id}/deactivate` → desativar plano
  - [ ] `GET /api/v1/admin/subscription-plans/{id}/history` → histórico de mudanças
  - [ ] `GET /api/v1/admin/subscription-plans/capabilities` → listar funcionalidades disponíveis
- [ ] Criar `TerritorySubscriptionPlansController` (Curadores):
  - [ ] `GET /api/v1/territories/{territoryId}/subscription-plans` → listar planos do território (territoriais + globais)
  - [ ] `GET /api/v1/territories/{territoryId}/subscription-plans/{id}` → obter plano do território
  - [ ] `POST /api/v1/territories/{territoryId}/subscription-plans` → criar plano territorial (apenas curadores do território)
  - [ ] `PUT /api/v1/territories/{territoryId}/subscription-plans/{id}` → atualizar plano territorial
  - [ ] `PATCH /api/v1/territories/{territoryId}/subscription-plans/{id}/capabilities` → atualizar funcionalidades
  - [ ] `PATCH /api/v1/territories/{territoryId}/subscription-plans/{id}/activate` → ativar plano
  - [ ] `PATCH /api/v1/territories/{territoryId}/subscription-plans/{id}/deactivate` → desativar plano
- [ ] Criar `AdminCouponsController` (SystemAdmin apenas):
  - [ ] `GET /api/v1/admin/coupons` → listar cupons
  - [ ] `GET /api/v1/admin/coupons/{id}` → obter cupom
  - [ ] `POST /api/v1/admin/coupons` → criar cupom
  - [ ] `PUT /api/v1/admin/coupons/{id}` → atualizar cupom
  - [ ] `PATCH /api/v1/admin/coupons/{id}/activate` → ativar cupom
  - [ ] `PATCH /api/v1/admin/coupons/{id}/deactivate` → desativar cupom
  - [ ] `GET /api/v1/admin/coupons/{id}/usage-stats` → estatísticas de uso
- [ ] Validações (FluentValidation):
  - [ ] `CreatePlanRequestValidator` (validar integridade, escopo, território)
  - [ ] `UpdatePlanRequestValidator`
  - [ ] `UpdateCapabilitiesRequestValidator` (garantir funcionalidades básicas no FREE)
  - [ ] `CreateCouponRequestValidator`
- [ ] Autorização:
  - [ ] SystemAdmin pode criar/editar planos globais e de qualquer território
  - [ ] Curadores podem criar/editar apenas planos do seu território
  - [ ] SystemAdmin pode criar/editar cupons globais e territoriais
  - [ ] Curadores podem criar/editar cupons do seu território
- [ ] Testes de integração

**Arquivos a Criar**:
- `backend/Araponga.Api/Controllers/AdminSubscriptionPlansController.cs` (SystemAdmin)
- `backend/Araponga.Api/Controllers/TerritorySubscriptionPlansController.cs` (Curadores)
- `backend/Araponga.Api/Controllers/AdminCouponsController.cs` (SystemAdmin)
- `backend/Araponga.Api/Controllers/TerritoryCouponsController.cs` (Curadores)
- `backend/Araponga.Api/Contracts/Admin/CreatePlanRequest.cs`
- `backend/Araponga.Api/Contracts/Admin/UpdatePlanRequest.cs`
- `backend/Araponga.Api/Contracts/Admin/UpdateCapabilitiesRequest.cs`
- `backend/Araponga.Api/Contracts/Admin/PlanHistoryResponse.cs`
- `backend/Araponga.Api/Contracts/Admin/CapabilityInfoResponse.cs`
- `backend/Araponga.Api/Validators/CreatePlanRequestValidator.cs`
- `backend/Araponga.Api/Validators/UpdatePlanRequestValidator.cs`
- `backend/Araponga.Tests/Integration/AdminSubscriptionPlansIntegrationTests.cs`

**Critérios de Sucesso**:
- ✅ Endpoints administrativos funcionando
- ✅ Validações de integridade funcionando
- ✅ Autorização funcionando
- ✅ Funcionalidades básicas protegidas
- ✅ Testes passando

---

#### 15.12 Controllers Públicos e Endpoints
**Estimativa**: 32 horas (4 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `SubscriptionPlansController`:
  - [ ] `GET /api/v1/subscription-plans` → listar planos (inclui FREE)
  - [ ] `GET /api/v1/subscription-plans/{id}` → obter plano
  - [ ] `POST /api/v1/subscription-plans` → criar plano (Admin)
  - [ ] `PATCH /api/v1/subscription-plans/{id}` → atualizar plano (Admin)
  - [ ] `DELETE /api/v1/subscription-plans/{id}` → desativar plano (Admin, não pode desativar FREE)
- [ ] Criar `SubscriptionsController`:
  - [ ] `POST /api/v1/subscriptions` → criar assinatura (não necessário para FREE)
  - [ ] `GET /api/v1/subscriptions/me` → obter minha assinatura (retorna FREE se não tiver pago)
  - [ ] `GET /api/v1/subscriptions` → listar assinaturas (Admin)
  - [ ] `GET /api/v1/subscriptions/{id}` → obter assinatura
  - [ ] `PATCH /api/v1/subscriptions/{id}` → atualizar assinatura (upgrade/downgrade)
  - [ ] `POST /api/v1/subscriptions/{id}/cancel` → cancelar assinatura (volta para FREE)
  - [ ] `POST /api/v1/subscriptions/{id}/reactivate` → reativar assinatura
- [ ] Criar `SubscriptionCapabilitiesController`:
  - [ ] `GET /api/v1/subscriptions/me/capabilities` → minhas capacidades
  - [ ] `GET /api/v1/subscriptions/me/limits` → meus limites
  - [ ] `POST /api/v1/subscriptions/check-capability` → verificar capacidade específica
- [ ] Criar `CouponsController`:
  - [ ] `GET /api/v1/coupons` → listar cupons (Admin)
  - [ ] `GET /api/v1/coupons/{code}` → validar cupom
  - [ ] `POST /api/v1/coupons` → criar cupom (Admin)
  - [ ] `PATCH /api/v1/coupons/{id}` → atualizar cupom (Admin)
- [ ] Criar requests/responses
- [ ] Validação (FluentValidation)
- [ ] Testes de integração

**Arquivos a Criar**:
- `backend/Araponga.Api/Controllers/SubscriptionPlansController.cs`
- `backend/Araponga.Api/Controllers/SubscriptionsController.cs`
- `backend/Araponga.Api/Controllers/CouponsController.cs`
- `backend/Araponga.Api/Contracts/Subscriptions/CreateSubscriptionRequest.cs`
- `backend/Araponga.Api/Contracts/Subscriptions/SubscriptionResponse.cs`
- `backend/Araponga.Api/Contracts/Subscriptions/SubscriptionPlanResponse.cs`
- `backend/Araponga.Api/Contracts/Subscriptions/CouponResponse.cs`
- `backend/Araponga.Api/Validators/CreateSubscriptionRequestValidator.cs`

**Critérios de Sucesso**:
- ✅ Endpoints funcionando
- ✅ Validações funcionando
- ✅ Autorização funcionando
- ✅ Testes passando

---

### Semana 6-7: Dashboard e Relatórios

#### 15.13 Dashboard de Assinantes
**Estimativa**: 32 horas (4 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `SubscriptionAnalyticsService`:
  - [ ] `GetMRRAsync(DateTime? startDate, DateTime? endDate, CancellationToken)` → Monthly Recurring Revenue
  - [ ] `GetChurnRateAsync(DateTime? startDate, DateTime? endDate, CancellationToken)` → taxa de churn
  - [ ] `GetActiveSubscriptionsCountAsync(CancellationToken)` → número de assinaturas ativas
  - [ ] `GetNewSubscriptionsCountAsync(DateTime? startDate, DateTime? endDate, CancellationToken)` → novas assinaturas
  - [ ] `GetCanceledSubscriptionsCountAsync(DateTime? startDate, DateTime? endDate, CancellationToken)` → canceladas
  - [ ] `GetRevenueByPlanAsync(DateTime? startDate, DateTime? endDate, CancellationToken)` → receita por plano
- [ ] Criar `SubscriptionAnalyticsController`:
  - [ ] `GET /api/v1/admin/subscriptions/analytics` → métricas gerais
  - [ ] `GET /api/v1/admin/subscriptions/analytics/mrr` → MRR
  - [ ] `GET /api/v1/admin/subscriptions/analytics/churn` → churn rate
  - [ ] `GET /api/v1/admin/subscriptions/analytics/revenue` → receita por plano
- [ ] Exportação de relatórios:
  - [ ] Exportar CSV de assinaturas
  - [ ] Exportar relatório de receita
- [ ] Testes

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/SubscriptionAnalyticsService.cs`
- `backend/Araponga.Api/Controllers/SubscriptionAnalyticsController.cs`
- `backend/Araponga.Tests/Application/SubscriptionAnalyticsServiceTests.cs`

**Critérios de Sucesso**:
- ✅ Métricas sendo calculadas
- ✅ Dashboard funcionando
- ✅ Exportação funcionando
- ✅ Testes passando

---

### Semana 7-8: Frontend e Notificações

#### 15.14 Interface de Assinaturas (Frontend)
**Estimativa**: 40 horas (5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Página de planos:
  - [ ] Listar planos disponíveis
  - [ ] Comparação de recursos
  - [ ] Botão de assinar
- [ ] Página de minha assinatura:
  - [ ] Status atual
  - [ ] Plano atual
  - [ ] Próxima cobrança
  - [ ] Histórico de pagamentos
  - [ ] Opções de upgrade/downgrade
  - [ ] Cancelar assinatura
- [ ] Fluxo de checkout:
  - [ ] Seleção de plano
  - [ ] Aplicação de cupom (opcional)
  - [ ] Informações de pagamento
  - [ ] Confirmação
- [ ] Notificações:
  - [ ] Notificações de renovação
  - [ ] Notificações de falha de pagamento
  - [ ] Notificações de fim de trial
- [ ] Testes E2E

**Arquivos a Criar**:
- `frontend/portal/pages/SubscriptionPlans.tsx`
- `frontend/portal/pages/MySubscription.tsx`
- `frontend/portal/pages/Checkout.tsx`
- `frontend/portal/components/subscriptions/PlanCard.tsx`
- `frontend/portal/components/subscriptions/SubscriptionStatus.tsx`
- `frontend/portal/components/subscriptions/PaymentHistory.tsx`

**Critérios de Sucesso**:
- ✅ Interface funcionando
- ✅ Fluxo de checkout funcionando
- ✅ Notificações funcionando
- ✅ Testes E2E passando

---

### Semana 8-9: Testes e Documentação

#### 15.15 Interface Administrativa (Frontend)
**Estimativa**: 40 horas (5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Página de gerenciamento de planos globais (SystemAdmin):
  - [ ] Listar planos globais
  - [ ] Criar novo plano global
  - [ ] Editar plano global existente
  - [ ] **Seleção de funcionalidades** (checkboxes por categoria)
  - [ ] **Definição de limites** (inputs para maxPosts, maxEvents, maxStorage, etc.)
  - [ ] **Definição de preço** (valor, ciclo de cobrança)
  - [ ] Ativar/desativar planos
  - [ ] Visualizar histórico de mudanças
  - [ ] Validações em tempo real (mostrar erros de integridade)
- [ ] Página de gerenciamento de planos territoriais (Curadores):
  - [ ] Listar planos do território (territoriais + globais como referência)
  - [ ] Criar novo plano territorial
  - [ ] Editar plano territorial existente
  - [ ] **Seleção de funcionalidades** (checkboxes por categoria)
  - [ ] **Definição de limites** (inputs para maxPosts, maxEvents, maxStorage, etc.)
  - [ ] **Definição de preço** (valor, ciclo de cobrança) - pode ser diferente do global
  - [ ] Ativar/desativar planos
  - [ ] Visualizar histórico de mudanças
  - [ ] Validações em tempo real (mostrar erros de integridade)
  - [ ] **Indicador visual**: Mostrar quais planos são globais vs territoriais
- [ ] Página de gerenciamento de cupons (SystemAdmin):
  - [ ] Listar cupons
  - [ ] Criar cupom
  - [ ] Editar cupom
  - [ ] Ativar/desativar cupons
  - [ ] Visualizar estatísticas de uso
- [ ] Validações no frontend:
  - [ ] Impedir remover funcionalidades básicas do FREE
  - [ ] Validar preços (FREE = 0, outros > 0)
  - [ ] Validar limites razoáveis
  - [ ] Mostrar avisos antes de desativar plano com assinaturas ativas
- [ ] Testes E2E

**Arquivos a Criar**:
- `frontend/portal/pages/admin/SubscriptionPlans.tsx` (SystemAdmin - planos globais)
- `frontend/portal/pages/admin/CreatePlan.tsx` (SystemAdmin - criar plano global)
- `frontend/portal/pages/admin/EditPlan.tsx` (SystemAdmin - editar plano global)
- `frontend/portal/pages/territories/{territoryId}/subscription-plans.tsx` (Curadores - planos territoriais)
- `frontend/portal/pages/territories/{territoryId}/subscription-plans/create.tsx` (Curadores - criar plano territorial)
- `frontend/portal/pages/admin/Coupons.tsx` (SystemAdmin - cupons globais)
- `frontend/portal/pages/territories/{territoryId}/coupons.tsx` (Curadores - cupons territoriais)
- `frontend/portal/components/admin/PlanForm.tsx`
- `frontend/portal/components/admin/CapabilitySelector.tsx`
- `frontend/portal/components/admin/LimitsEditor.tsx`
- `frontend/portal/components/admin/PlanHistory.tsx`
- `frontend/portal/components/admin/PlanScopeSelector.tsx` (Global vs Territory)

**Critérios de Sucesso**:
- ✅ Interface administrativa funcionando
- ✅ Criação de planos customizados funcionando
- ✅ Seleção de funcionalidades funcionando
- ✅ Validações em tempo real funcionando
- ✅ Testes E2E passando

---

#### 15.16 Testes e Documentação
**Estimativa**: 40 horas (5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Testes de integração completos:
  - [ ] Criação de assinaturas
  - [ ] Renovações automáticas
  - [ ] Webhooks do Stripe
  - [ ] Upgrade/downgrade
  - [ ] Cancelamento
  - [ ] Cupons
  - [ ] Trials
- [ ] Testes de performance:
  - [ ] Processamento de webhooks em lote
  - [ ] Cálculo de métricas
- [ ] Testes de segurança:
  - [ ] Validação de webhooks
  - [ ] Autorização de endpoints
- [ ] Documentação técnica:
  - [ ] `docs/SUBSCRIPTIONS_SYSTEM.md`
  - [ ] Como funciona o sistema
  - [ ] Configuração do Stripe
  - [ ] Webhooks
- [ ] Atualizar `docs/CHANGELOG.md`
- [ ] Atualizar Swagger

**Arquivos a Criar**:
- `backend/Araponga.Tests/Integration/SubscriptionsCompleteIntegrationTests.cs`
- `docs/SUBSCRIPTIONS_SYSTEM.md`

**Critérios de Sucesso**:
- ✅ Testes passando
- ✅ Cobertura >85%
- ✅ Documentação completa

---

## 📊 Resumo da Fase 15

| Tarefa | Estimativa | Status | Prioridade |
|--------|------------|--------|------------|
| Modelo de Domínio | 32h | ❌ Pendente | 🔴 Crítica |
| Integração com Stripe | 40h | ❌ Pendente | 🔴 Crítica |
| Webhooks do Stripe | 32h | ❌ Pendente | 🔴 Crítica |
| Serviço de Assinaturas | 40h | ❌ Pendente | 🔴 Crítica |
| Serviço de Cupons | 24h | ❌ Pendente | 🟡 Importante |
| Processamento de Renovações | 32h | ❌ Pendente | 🔴 Crítica |
| Gestão de Trials | 24h | ❌ Pendente | 🟡 Importante |
| Sistema Administrativo de Planos | 48h | ❌ Pendente | 🔴 Crítica |
| Sistema Administrativo de Cupons | 24h | ❌ Pendente | 🟡 Importante |
| Verificação de Funcionalidades | 24h | ❌ Pendente | 🔴 Crítica |
| Controllers Administrativos | 32h | ❌ Pendente | 🔴 Crítica |
| Controllers Públicos | 32h | ❌ Pendente | 🔴 Crítica |
| Dashboard e Relatórios | 32h | ❌ Pendente | 🟡 Importante |
| Interface Frontend Pública | 40h | ❌ Pendente | 🟡 Importante |
| Interface Administrativa (Global + Territorial) | 48h | ❌ Pendente | 🔴 Crítica |
| Testes e Documentação | 40h | ❌ Pendente | 🟡 Importante |
| **Total** | **480h (60 dias)** | | |

---

## ✅ Critérios de Sucesso da Fase 15

### Funcionalidades
- ✅ **Plano FREE funcionando** (padrão para todos)
- ✅ **Funcionalidades básicas sempre acessíveis** (feed, posts, eventos, marketplace básico)
- ✅ Sistema completo de assinaturas funcionando
- ✅ **Sistema de verificação de funcionalidades** funcionando
- ✅ **Liberação progressiva de funcionalidades** por plano
- ✅ **Sistema administrativo completo** para criar/gerenciar planos
- ✅ **Seleção de funcionalidades** por plano via interface
- ✅ **Validações de integridade** garantindo funcionalidades básicas no FREE
- ✅ **Gerenciamento de cupons** via interface administrativa
- ✅ Pagamentos recorrentes automáticos funcionando
- ✅ Integração com Stripe funcionando
- ✅ Webhooks sendo processados
- ✅ Upgrade/downgrade funcionando
- ✅ Cancelamento funcionando (volta para FREE)
- ✅ Trials funcionando
- ✅ Cupons funcionando
- ✅ Dashboard de métricas funcionando

### Qualidade
- ✅ Cobertura de testes >85%
- ✅ Testes de integração passando
- ✅ Performance adequada
- ✅ Segurança validada
- Considerar **Testcontainers + PostgreSQL** para testes de integração (assinaturas, pagamentos, persistência) com banco real (estratégia na Fase 43; [TESTCONTAINERS_POSTGRES_IMPACTO](../../TESTCONTAINERS_POSTGRES_IMPACTO.md)).

### Integração
- ✅ Integração com Fase 6 (Pagamentos) funcionando
- ✅ Integração com Fase 7 (Payout) funcionando
- ✅ Sincronização com Stripe funcionando

### Documentação
- ✅ Documentação técnica completa
- ✅ Changelog atualizado
- ✅ Swagger atualizado

---

## 🔗 Dependências

- **Fase 6**: Sistema de Pagamentos (base para integração)
- **Fase 7**: Sistema de Payout (para pagamentos a vendedores)

---

## 📝 Notas de Implementação

### Planos Padrão

**FREE (Gratuito)** - **Padrão para todos**:
- Preço: **R$ 0,00** (sempre gratuito)
- **Funcionalidades Básicas (Sempre Disponíveis)**:
  - ✅ Feed comunitário (visualizar e criar posts básicos)
  - ✅ Eventos (criar e participar de eventos)
  - ✅ Marketplace básico (visualizar e criar itens simples)
  - ✅ Chat territorial (participar de conversas)
  - ✅ Visualização de territórios e mapas
  - ✅ Participação em votações
  - ✅ Perfil básico
- **Limites Razonáveis**:
  - Posts: 10/mês
  - Eventos: 3/mês
  - Itens no marketplace: 5 ativos
  - Armazenamento: 100MB
- **Princípio**: Ninguém é excluído. Funcionalidades essenciais sempre disponíveis.

**Básico** (R$ 29,90/mês):
- Tudo do FREE +
- Posts ilimitados
- Eventos ilimitados
- Marketplace completo
- Armazenamento: 1GB
- Analytics básico

**Intermediário** (R$ 59,90/mês):
- Tudo do Básico +
- Analytics avançado
- Integração com IA (limitada)
- Armazenamento: 5GB
- Suporte prioritário

**Premium** (R$ 99,90/mês):
- Tudo do Intermediário +
- Integração com IA completa
- Recursos premium territoriais
- Armazenamento: 20GB
- Suporte prioritário 24/7
- API access
- Custom branding (opcional)

### Ciclos de Cobrança

- Mensal: Cobrança a cada 30 dias
- Trimestral: Cobrança a cada 90 dias (desconto de 10%)
- Anual: Cobrança a cada 365 dias (desconto de 20%)

### Plano FREE (Gratuito)

**Princípios Fundamentais**:
- ✅ **Sempre disponível** para visitantes e residentes
- ✅ **Não pode ser desativado** ou removido
- ✅ **Funcionalidades básicas sempre acessíveis** (protegidas por validação)
- ✅ **Ninguém é excluído** por não poder pagar
- ✅ **Validação automática** impede remoção de funcionalidades básicas

**Funcionalidades Básicas Protegidas** (sempre no FREE):
- `FeedBasic` - Feed comunitário
- `PostsBasic` - Posts básicos (10/mês)
- `EventsBasic` - Eventos básicos (3/mês)
- `MarketplaceBasic` - Marketplace básico (5 itens ativos)
- `ChatBasic` - Chat territorial

**Sistema de Proteção**:
- Validação automática ao criar/editar planos
- Interface administrativa impede remover funcionalidades básicas do FREE
- API valida integridade antes de salvar
- Histórico de mudanças registra tentativas de violação

**Funcionalidades Básicas (Sempre Gratuitas)**:
- Feed comunitário (visualizar e criar posts básicos)
- Eventos (criar e participar)
- Marketplace básico (visualizar e criar itens simples)
- Chat territorial (participar de conversas)
- Visualização de territórios e mapas
- Participação em votações
- Perfil básico

**Limites Razonáveis**:
- Posts: 10/mês (suficiente para participação básica)
- Eventos: 3/mês (suficiente para organização básica)
- Itens no marketplace: 5 ativos (suficiente para trocas básicas)
- Armazenamento: 100MB (suficiente para conteúdo básico)

### Trials

- Duração padrão: 14 dias
- Configurável por plano
- Apenas uma vez por usuário por plano
- **Apenas para planos pagos** (FREE não precisa de trial)

### Webhooks do Stripe

**Eventos Importantes**:
- `customer.subscription.created` → Nova assinatura
- `customer.subscription.updated` → Assinatura atualizada
- `customer.subscription.deleted` → Assinatura cancelada
- `invoice.payment_succeeded` → Pagamento bem-sucedido
- `invoice.payment_failed` → Falha no pagamento
- `customer.subscription.trial_will_end` → Trial terminando em breve

### Métricas

**MRR (Monthly Recurring Revenue)**:
- Soma de todas as assinaturas ativas mensais
- + (Trimestrais / 3)
- + (Anuais / 12)

**Churn Rate**:
- (Cancelamentos no período / Assinaturas ativas no início do período) * 100

### Sistema Administrativo de Planos e Funcionalidades

**Visão Geral**:
O sistema permite que SystemAdmin crie e gerencie planos customizados com total flexibilidade, mas com validações automáticas que garantem a integridade das regras fundamentais da plataforma.

**Fluxo de Criação de Plano**:
1. **SystemAdmin ou Curador** acessa interface administrativa
2. **Escolhe escopo**: Global (SystemAdmin) ou Territorial (Curador do território)
3. Cria novo plano ou edita existente
4. Define nome, descrição, tier, preço, ciclo de cobrança
5. **Seleciona funcionalidades** (checkboxes organizadas por categoria)
6. **Define limites** (maxPosts, maxEvents, maxStorage, etc.)
7. Sistema valida integridade automaticamente:
   - Se é FREE: verifica se tem todas as funcionalidades básicas
   - Se é pago: verifica se preço > 0
   - Valida limites razoáveis
   - Se territorial: valida que território existe e usuário tem permissão
8. Se válido, salva e sincroniza com Stripe (se pago)
9. Registra mudança no histórico

**Hierarquia de Planos**:
- **Planos Globais**: Aplicam a todos os territórios por padrão
- **Planos Territoriais**: Sobrescrevem planos globais quando existem
- **Resolução**: Ao buscar planos para um território:
  1. Primeiro verifica se há planos territoriais
  2. Se não houver, usa planos globais
  3. FREE sempre disponível (global ou territorial)

**Validações de Integridade**:

**Para Plano FREE**:
- ✅ Deve ter preço = 0
- ✅ Deve ter todas as funcionalidades básicas: `FeedBasic`, `PostsBasic`, `EventsBasic`, `MarketplaceBasic`, `ChatBasic`
- ✅ FREE global não pode ser desativado
- ✅ FREE territorial pode ser desativado (mas não deletado)
- ✅ Não pode ser deletado (global ou territorial)
- ✅ Limites devem ser razoáveis (não muito restritivos)
- ✅ FREE global sempre existe (criado automaticamente)
- ✅ FREE territorial é opcional (curador pode criar versão customizada)

**Para Planos Pagos**:
- ✅ Deve ter preço > 0
- ✅ Deve ter ciclo de cobrança definido
- ✅ Pode ter qualquer combinação de funcionalidades
- ✅ Pode ser ativado/desativado
- ✅ Ao desativar, avisa se há assinaturas ativas
- ✅ **Planos Globais**: Aplicam a todos os territórios
- ✅ **Planos Territoriais**: Aplicam apenas ao território específico
- ✅ Território pode ter preços diferentes dos globais
- ✅ Território pode ter funcionalidades diferentes dos globais

**Sistema de Funcionalidades**:

**Categorias**:
- **Core** (sempre no FREE): Feed, Posts básicos, Eventos básicos, Marketplace básico, Chat
- **Enhanced**: Posts ilimitados, Eventos ilimitados, Marketplace avançado
- **Premium**: Analytics, IA, Suporte prioritário
- **Enterprise**: API access, Custom branding, Governança avançada

**Seleção de Funcionalidades**:
- Interface mostra todas as funcionalidades disponíveis
- Organizadas por categoria
- Checkboxes para seleção múltipla
- Validação em tempo real mostra erros
- Preview do plano antes de salvar

**Gerenciamento de Cupons**:
- Criar cupons com código, desconto, validade
- Ativar/desativar cupons
- Visualizar estatísticas de uso
- Integração automática com Stripe

**Histórico e Auditoria**:
- Todas as mudanças em planos são registradas
- Inclui: quem mudou, quando, o que mudou, motivo (opcional)
- Permite rastreabilidade completa
- Útil para debugging e compliance

### Planos Configuráveis por Território

**Conceito**:
Cada território pode ter seus próprios planos customizados, permitindo flexibilidade para diferentes contextos econômicos e necessidades locais, mantendo planos globais como padrão.

**Hierarquia de Planos**:

1. **Planos Globais** (SystemAdmin):
   - Aplicam a todos os territórios por padrão
   - Criados e gerenciados apenas por SystemAdmin
   - FREE global sempre existe e não pode ser removido
   - Exemplo: "Básico Global" (R$ 29,90/mês) aplica a todos os territórios

2. **Planos Territoriais** (Curadores):
   - Específicos de um território
   - Criados e gerenciados por curadores do território
   - Sobrescrevem planos globais quando existem
   - Exemplo: "Básico São Paulo" (R$ 25,00/mês) sobrescreve "Básico Global" apenas para São Paulo

**Resolução de Planos**:

Quando um usuário interage com um território:
1. Sistema verifica se há planos **territoriais** para aquele território
2. Se houver plano territorial, usa ele
3. Se não houver, usa plano **global**
4. FREE sempre disponível (global ou territorial)

**Exemplo Prático**:

```
Território: "Vila Madalena, São Paulo"

Planos Disponíveis:
├─ FREE (Global) - R$ 0,00
├─ Básico (Territorial) - R$ 25,00/mês ← Sobrescreve global
├─ Premium (Global) - R$ 99,90/mês ← Usa global (não tem territorial)
└─ Enterprise (Territorial) - R$ 150,00/mês ← Específico do território
```

**Permissões**:

- **SystemAdmin**:
  - Pode criar/editar planos globais
  - Pode criar/editar planos de qualquer território
  - Acesso completo

- **Curadores**:
  - Podem criar/editar apenas planos do seu território
  - Podem ver planos globais como referência
  - Não podem modificar planos globais

**Validações por Território**:

- FREE territorial deve ter todas as funcionalidades básicas (igual ao global)
- Planos territoriais podem ter preços diferentes dos globais
- Planos territoriais podem ter funcionalidades diferentes dos globais
- Não pode haver conflito de nomes (mesmo nome no mesmo território)
- Território deve existir antes de criar plano territorial

**Cupons por Território**:

- Cupons também podem ser globais ou territoriais
- Cupons territoriais aplicam apenas ao território específico
- Hierarquia similar: cupons territoriais têm prioridade sobre globais

---

**Status**: ⏳ **FASE 15 PENDENTE**  
**Depende de**: Fases 6, 7 (Pagamentos, Payout)  
**Crítico para**: Sustentabilidade Financeira
