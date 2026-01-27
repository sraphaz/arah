# Fase 16: Cenários de Teste Necessários - Fase 15

**Data**: 2026-01-26  
**Status**: ⏳ **PENDENTE**  
**Prioridade**: 🔴 **CRÍTICA**

---

## 📊 Resumo

**Cobertura Atual Fase 15**: ~5%  
**Cobertura Alvo**: >85%  
**Cenários Necessários**: 81 (61 críticos + 20 importantes)  
**Estimativa**: 52 horas (6.5 dias)

---

## 🔴 Cenários Críticos (61)

### 1. SubscriptionAnalyticsServiceTests (12 cenários)

**Arquivo**: `backend/Araponga.Tests/Application/SubscriptionAnalyticsServiceTests.cs`

```csharp
[Fact]
public async Task GetMRRAsync_ReturnsCorrectMRR_WhenSubscriptionsExist()
{
    // Arrange: Criar assinaturas ativas com diferentes valores
    // Act: Chamar GetMRRAsync
    // Assert: Verificar que MRR está correto
}

[Fact]
public async Task GetMRRAsync_ReturnsZero_WhenNoSubscriptions()
{
    // Arrange: Nenhuma assinatura
    // Act: Chamar GetMRRAsync
    // Assert: MRR = 0
}

[Fact]
public async Task GetMRRAsync_FiltersByDateRange_Correctly()
{
    // Arrange: Assinaturas em diferentes períodos
    // Act: Chamar GetMRRAsync com startDate e endDate
    // Assert: Apenas assinaturas no período são consideradas
}

[Fact]
public async Task GetChurnRateAsync_ReturnsCorrectRate_WhenCancellationsExist()
{
    // Arrange: Criar assinaturas canceladas e ativas
    // Act: Chamar GetChurnRateAsync
    // Assert: Taxa de churn calculada corretamente
}

[Fact]
public async Task GetChurnRateAsync_ReturnsZero_WhenNoCancellations()
{
    // Arrange: Apenas assinaturas ativas
    // Act: Chamar GetChurnRateAsync
    // Assert: Churn rate = 0
}

[Fact]
public async Task GetChurnRateAsync_FiltersByDateRange_Correctly()
{
    // Arrange: Cancelamentos em diferentes períodos
    // Act: Chamar GetChurnRateAsync com startDate e endDate
    // Assert: Apenas cancelamentos no período são considerados
}

[Fact]
public async Task GetActiveSubscriptionsCountAsync_ReturnsCorrectCount()
{
    // Arrange: Criar múltiplas assinaturas (ativas e canceladas)
    // Act: Chamar GetActiveSubscriptionsCountAsync
    // Assert: Contagem apenas de assinaturas ativas
}

[Fact]
public async Task GetNewSubscriptionsCountAsync_ReturnsCorrectCount_ForDateRange()
{
    // Arrange: Criar assinaturas em diferentes datas
    // Act: Chamar GetNewSubscriptionsCountAsync com período
    // Assert: Contagem apenas de assinaturas criadas no período
}

[Fact]
public async Task GetCanceledSubscriptionsCountAsync_ReturnsCorrectCount_ForDateRange()
{
    // Arrange: Criar cancelamentos em diferentes datas
    // Act: Chamar GetCanceledSubscriptionsCountAsync com período
    // Assert: Contagem apenas de cancelamentos no período
}

[Fact]
public async Task GetRevenueByPlanAsync_ReturnsCorrectRevenue_GroupedByPlan()
{
    // Arrange: Criar assinaturas de diferentes planos
    // Act: Chamar GetRevenueByPlanAsync
    // Assert: Receita agrupada corretamente por plano
}

[Fact]
public async Task GetRevenueByPlanAsync_ReturnsEmpty_WhenNoSubscriptions()
{
    // Arrange: Nenhuma assinatura
    // Act: Chamar GetRevenueByPlanAsync
    // Assert: Dicionário vazio retornado
}

[Fact]
public async Task GetRevenueByPlanAsync_FiltersByDateRange_Correctly()
{
    // Arrange: Assinaturas em diferentes períodos
    // Act: Chamar GetRevenueByPlanAsync com período
    // Assert: Apenas receita do período é considerada
}
```

---

### 2. SubscriptionPlanAdminServiceTests (10 cenários)

**Arquivo**: `backend/Araponga.Tests/Application/SubscriptionPlanAdminServiceTests.cs`

```csharp
[Fact]
public async Task CreatePlanAsync_CreatesPlan_WhenValidData()
{
    // Arrange: Dados válidos de plano
    // Act: Chamar CreatePlanAsync
    // Assert: Plano criado com sucesso
}

[Fact]
public async Task CreatePlanAsync_ReturnsFailure_WhenInvalidData()
{
    // Arrange: Dados inválidos (ex: preço negativo)
    // Act: Chamar CreatePlanAsync
    // Assert: Retorna falha com mensagem apropriada
}

[Fact]
public async Task CreatePlanAsync_ValidatesRequiredCapabilities_ForFreePlan()
{
    // Arrange: Tentar criar plano FREE sem funcionalidades básicas
    // Act: Chamar CreatePlanAsync
    // Assert: Retorna falha indicando funcionalidades básicas obrigatórias
}

[Fact]
public async Task UpdatePlanAsync_UpdatesPlan_WhenValidData()
{
    // Arrange: Plano existente e dados de atualização válidos
    // Act: Chamar UpdatePlanAsync
    // Assert: Plano atualizado e histórico criado
}

[Fact]
public async Task UpdatePlanAsync_ReturnsFailure_WhenPlanNotFound()
{
    // Arrange: ID de plano inexistente
    // Act: Chamar UpdatePlanAsync
    // Assert: Retorna falha
}

[Fact]
public async Task UpdatePlanAsync_PreventsRemovingBasicCapabilities()
{
    // Arrange: Tentar remover funcionalidades básicas do plano FREE
    // Act: Chamar UpdatePlanAsync
    // Assert: Retorna falha indicando que não pode remover
}

[Fact]
public async Task DeactivatePlanAsync_DeactivatesPlan_WhenNoActiveSubscriptions()
{
    // Arrange: Plano sem assinaturas ativas
    // Act: Chamar DeactivatePlanAsync
    // Assert: Plano desativado
}

[Fact]
public async Task DeactivatePlanAsync_ReturnsFailure_WhenActiveSubscriptionsExist()
{
    // Arrange: Plano com assinaturas ativas
    // Act: Chamar DeactivatePlanAsync
    // Assert: Retorna falha indicando assinaturas ativas
}

[Fact]
public async Task GetPlanHistoryAsync_ReturnsHistory_WhenChangesExist()
{
    // Arrange: Plano com histórico de mudanças
    // Act: Chamar GetPlanHistoryAsync
    // Assert: Histórico retornado ordenado por data
}

[Fact]
public async Task GetPlanHistoryAsync_ReturnsEmpty_WhenNoHistory()
{
    // Arrange: Plano sem histórico
    // Act: Chamar GetPlanHistoryAsync
    // Assert: Lista vazia retornada
}
```

---

### 3. CouponServiceTests (10 cenários)

**Arquivo**: `backend/Araponga.Tests/Application/CouponServiceTests.cs`

```csharp
[Fact]
public async Task CreateCouponAsync_CreatesCoupon_WhenValidData()
{
    // Arrange: Dados válidos de cupom
    // Act: Chamar CreateCouponAsync
    // Assert: Cupom criado com sucesso
}

[Fact]
public async Task CreateCouponAsync_ReturnsFailure_WhenInvalidData()
{
    // Arrange: Dados inválidos (ex: desconto > 100%)
    // Act: Chamar CreateCouponAsync
    // Assert: Retorna falha
}

[Fact]
public async Task CreateCouponAsync_ValidatesExpirationDate()
{
    // Arrange: Cupom com data de expiração no passado
    // Act: Chamar CreateCouponAsync
    // Assert: Retorna falha
}

[Fact]
public async Task ApplyCouponAsync_AppliesCoupon_WhenValid()
{
    // Arrange: Cupom válido e não expirado
    // Act: Chamar ApplyCouponAsync
    // Assert: Cupom aplicado e desconto calculado corretamente
}

[Fact]
public async Task ApplyCouponAsync_ReturnsFailure_WhenCouponExpired()
{
    // Arrange: Cupom expirado
    // Act: Chamar ApplyCouponAsync
    // Assert: Retorna falha
}

[Fact]
public async Task ApplyCouponAsync_ReturnsFailure_WhenCouponNotFound()
{
    // Arrange: ID de cupom inexistente
    // Act: Chamar ApplyCouponAsync
    // Assert: Retorna falha
}

[Fact]
public async Task ApplyCouponAsync_ValidatesUsageLimit()
{
    // Arrange: Cupom com limite de uso atingido
    // Act: Chamar ApplyCouponAsync
    // Assert: Retorna falha
}

[Fact]
public async Task ValidateCouponAsync_ReturnsTrue_WhenValid()
{
    // Arrange: Cupom válido, não expirado, com uso disponível
    // Act: Chamar ValidateCouponAsync
    // Assert: Retorna true
}

[Fact]
public async Task ValidateCouponAsync_ReturnsFalse_WhenExpired()
{
    // Arrange: Cupom expirado
    // Act: Chamar ValidateCouponAsync
    // Assert: Retorna false
}

[Fact]
public async Task ValidateCouponAsync_ReturnsFalse_WhenUsageLimitExceeded()
{
    // Arrange: Cupom com limite de uso atingido
    // Act: Chamar ValidateCouponAsync
    // Assert: Retorna false
}
```

---

### 4. StripeWebhookServiceTests (10 cenários)

**Arquivo**: `backend/Araponga.Tests/Application/StripeWebhookServiceTests.cs`

```csharp
[Fact]
public async Task ProcessWebhookAsync_ProcessesSubscriptionCreated_WhenValidEvent()
{
    // Arrange: Evento customer.subscription.created válido
    // Act: Chamar ProcessWebhookAsync
    // Assert: Assinatura criada no sistema
}

[Fact]
public async Task ProcessWebhookAsync_ProcessesSubscriptionUpdated_WhenValidEvent()
{
    // Arrange: Evento customer.subscription.updated válido
    // Act: Chamar ProcessWebhookAsync
    // Assert: Assinatura atualizada no sistema
}

[Fact]
public async Task ProcessWebhookAsync_ProcessesSubscriptionDeleted_WhenValidEvent()
{
    // Arrange: Evento customer.subscription.deleted válido
    // Act: Chamar ProcessWebhookAsync
    // Assert: Assinatura cancelada no sistema
}

[Fact]
public async Task ProcessWebhookAsync_ProcessesInvoicePaymentSucceeded_WhenValidEvent()
{
    // Arrange: Evento invoice.payment_succeeded válido
    // Act: Chamar ProcessWebhookAsync
    // Assert: Pagamento registrado e assinatura renovada
}

[Fact]
public async Task ProcessWebhookAsync_ProcessesInvoicePaymentFailed_WhenValidEvent()
{
    // Arrange: Evento invoice.payment_failed válido
    // Act: Chamar ProcessWebhookAsync
    // Assert: Falha de pagamento registrada
}

[Fact]
public async Task ProcessWebhookAsync_ProcessesTrialWillEnd_WhenValidEvent()
{
    // Arrange: Evento customer.subscription.trial_will_end válido
    // Act: Chamar ProcessWebhookAsync
    // Assert: Notificação de fim de trial criada
}

[Fact]
public async Task ProcessWebhookAsync_ReturnsFailure_WhenInvalidEvent()
{
    // Arrange: Evento desconhecido ou inválido
    // Act: Chamar ProcessWebhookAsync
    // Assert: Retorna falha
}

[Fact]
public async Task ProcessWebhookAsync_HandlesIdempotency_Correctly()
{
    // Arrange: Processar mesmo evento duas vezes
    // Act: Chamar ProcessWebhookAsync duas vezes
    // Assert: Evento processado apenas uma vez
}

[Fact]
public async Task ProcessWebhookAsync_UpdatesSubscriptionStatus_Correctly()
{
    // Arrange: Evento de atualização de assinatura
    // Act: Chamar ProcessWebhookAsync
    // Assert: Status da assinatura atualizado corretamente
}

[Fact]
public async Task ProcessWebhookAsync_CreatesPaymentRecord_WhenPaymentSucceeded()
{
    // Arrange: Evento invoice.payment_succeeded
    // Act: Chamar ProcessWebhookAsync
    // Assert: SubscriptionPayment criado com dados corretos
}
```

---

### 5. MercadoPagoWebhookServiceTests (6 cenários)

**Arquivo**: `backend/Araponga.Tests/Application/MercadoPagoWebhookServiceTests.cs`

```csharp
[Fact]
public async Task ProcessWebhookAsync_ProcessesSubscriptionCreated_WhenValidEvent()
{
    // Arrange: Evento de criação de assinatura válido
    // Act: Chamar ProcessWebhookAsync
    // Assert: Assinatura criada no sistema
}

[Fact]
public async Task ProcessWebhookAsync_ProcessesSubscriptionUpdated_WhenValidEvent()
{
    // Arrange: Evento de atualização válido
    // Act: Chamar ProcessWebhookAsync
    // Assert: Assinatura atualizada
}

[Fact]
public async Task ProcessWebhookAsync_ProcessesPaymentApproved_WhenValidEvent()
{
    // Arrange: Evento de pagamento aprovado
    // Act: Chamar ProcessWebhookAsync
    // Assert: Pagamento registrado
}

[Fact]
public async Task ProcessWebhookAsync_ProcessesPaymentRejected_WhenValidEvent()
{
    // Arrange: Evento de pagamento rejeitado
    // Act: Chamar ProcessWebhookAsync
    // Assert: Falha de pagamento registrada
}

[Fact]
public async Task ProcessWebhookAsync_ReturnsFailure_WhenInvalidEvent()
{
    // Arrange: Evento inválido
    // Act: Chamar ProcessWebhookAsync
    // Assert: Retorna falha
}

[Fact]
public async Task ProcessWebhookAsync_HandlesIdempotency_Correctly()
{
    // Arrange: Processar mesmo evento duas vezes
    // Act: Chamar ProcessWebhookAsync duas vezes
    // Assert: Evento processado apenas uma vez
}
```

---

### 6. SubscriptionRenewalServiceTests (6 cenários)

**Arquivo**: `backend/Araponga.Tests/Application/SubscriptionRenewalServiceTests.cs`

```csharp
[Fact]
public async Task ProcessRenewalsAsync_ProcessesRenewals_WhenDue()
{
    // Arrange: Assinaturas com data de renovação vencida
    // Act: Chamar ProcessRenewalsAsync
    // Assert: Renovações processadas
}

[Fact]
public async Task ProcessRenewalsAsync_SkipsRenewals_WhenNotDue()
{
    // Arrange: Assinaturas com data de renovação futura
    // Act: Chamar ProcessRenewalsAsync
    // Assert: Nenhuma renovação processada
}

[Fact]
public async Task ProcessRenewalsAsync_HandlesPaymentFailure_Correctly()
{
    // Arrange: Assinatura com falha de pagamento
    // Act: Chamar ProcessRenewalsAsync
    // Assert: Falha registrada e notificação enviada
}

[Fact]
public async Task ProcessRenewalsAsync_UpdatesNextBillingDate_Correctly()
{
    // Arrange: Renovação bem-sucedida
    // Act: Chamar ProcessRenewalsAsync
    // Assert: Próxima data de cobrança atualizada
}

[Fact]
public async Task ProcessRenewalsAsync_CreatesPaymentRecord_WhenSuccessful()
{
    // Arrange: Renovação bem-sucedida
    // Act: Chamar ProcessRenewalsAsync
    // Assert: SubscriptionPayment criado
}

[Fact]
public async Task ProcessRenewalsAsync_CancelsSubscription_WhenPaymentFailsMultipleTimes()
{
    // Arrange: Assinatura com múltiplas falhas de pagamento
    // Act: Chamar ProcessRenewalsAsync
    // Assert: Assinatura cancelada após limite de tentativas
}
```

---

### 7. SubscriptionTrialServiceTests (7 cenários)

**Arquivo**: `backend/Araponga.Tests/Application/SubscriptionTrialServiceTests.cs`

```csharp
[Fact]
public async Task StartTrialAsync_StartsTrial_WhenPlanHasTrialDays()
{
    // Arrange: Plano com trialDays > 0
    // Act: Chamar StartTrialAsync
    // Assert: Trial iniciado e data de término calculada
}

[Fact]
public async Task StartTrialAsync_ReturnsFailure_WhenNoTrialDays()
{
    // Arrange: Plano sem trialDays
    // Act: Chamar StartTrialAsync
    // Assert: Retorna falha
}

[Fact]
public async Task StartTrialAsync_ReturnsFailure_WhenUserAlreadyHadTrial()
{
    // Arrange: Usuário que já teve trial
    // Act: Chamar StartTrialAsync
    // Assert: Retorna falha
}

[Fact]
public async Task EndTrialAsync_EndsTrial_WhenExpired()
{
    // Arrange: Trial expirado
    // Act: Chamar EndTrialAsync
    // Assert: Trial finalizado
}

[Fact]
public async Task EndTrialAsync_ActivatesSubscription_WhenTrialEnds()
{
    // Arrange: Trial expirado
    // Act: Chamar EndTrialAsync
    // Assert: Assinatura ativada automaticamente
}

[Fact]
public async Task EndTrialAsync_SendsNotification_WhenTrialEnding()
{
    // Arrange: Trial terminando em breve
    // Act: Chamar EndTrialAsync
    // Assert: Notificação enviada
}

[Fact]
public async Task EndTrialAsync_SendsNotification_WhenTrialEnded()
{
    // Arrange: Trial terminado
    // Act: Chamar EndTrialAsync
    // Assert: Notificação de término enviada
}
```

---

## 🟡 Cenários Importantes (20)

### 8. SubscriptionServiceTests - Adicionais (10 cenários)

**Arquivo**: `backend/Araponga.Tests/Application/SubscriptionServiceTests.cs` (atualizar)

```csharp
[Fact]
public async Task UpgradeSubscriptionAsync_UpgradesSubscription_WhenValidPlan()
{
    // Arrange: Assinatura ativa e plano superior
    // Act: Chamar UpgradeSubscriptionAsync
    // Assert: Assinatura atualizada para plano superior
}

[Fact]
public async Task UpgradeSubscriptionAsync_CalculatesProrata_Correctly()
{
    // Arrange: Upgrade no meio do período
    // Act: Chamar UpgradeSubscriptionAsync
    // Assert: Proratação calculada corretamente
}

[Fact]
public async Task DowngradeSubscriptionAsync_DowngradesSubscription_WhenValidPlan()
{
    // Arrange: Assinatura ativa e plano inferior
    // Act: Chamar DowngradeSubscriptionAsync
    // Assert: Assinatura atualizada para plano inferior
}

[Fact]
public async Task DowngradeSubscriptionAsync_CalculatesProrata_Correctly()
{
    // Arrange: Downgrade no meio do período
    // Act: Chamar DowngradeSubscriptionAsync
    // Assert: Proratação calculada corretamente
}

[Fact]
public async Task ReactivateSubscriptionAsync_ReactivatesSubscription_WhenCanceled()
{
    // Arrange: Assinatura cancelada
    // Act: Chamar ReactivateSubscriptionAsync
    // Assert: Assinatura reativada
}

[Fact]
public async Task ReactivateSubscriptionAsync_ReturnsFailure_WhenNotCanceled()
{
    // Arrange: Assinatura ativa
    // Act: Chamar ReactivateSubscriptionAsync
    // Assert: Retorna falha
}

[Fact]
public async Task GetAvailablePlansForTerritoryAsync_ReturnsPlans_Correctly()
{
    // Arrange: Planos globais e territoriais
    // Act: Chamar GetAvailablePlansForTerritoryAsync
    // Assert: Planos retornados corretamente
}

[Fact]
public async Task GetAvailablePlansForTerritoryAsync_RespectsTerritorialHierarchy()
{
    // Arrange: Plano territorial e global para mesmo território
    // Act: Chamar GetAvailablePlansForTerritoryAsync
    // Assert: Plano territorial tem prioridade
}

[Fact]
public async Task ApplyCouponToSubscriptionAsync_AppliesCoupon_WhenValid()
{
    // Arrange: Cupom válido e assinatura
    // Act: Chamar ApplyCouponToSubscriptionAsync
    // Assert: Cupom aplicado e desconto calculado
}

[Fact]
public async Task ApplyCouponToSubscriptionAsync_ReturnsFailure_WhenInvalidCoupon()
{
    // Arrange: Cupom inválido ou expirado
    // Act: Chamar ApplyCouponToSubscriptionAsync
    // Assert: Retorna falha
}
```

---

### 9. SubscriptionPlanSeedServiceTests (4 cenários)

**Arquivo**: `backend/Araponga.Tests/Application/SubscriptionPlanSeedServiceTests.cs`

```csharp
[Fact]
public async Task SeedFreePlanAsync_CreatesPlan_WhenNotExists()
{
    // Arrange: Nenhum plano FREE existente
    // Act: Chamar SeedFreePlanAsync
    // Assert: Plano FREE criado
}

[Fact]
public async Task SeedFreePlanAsync_ReturnsSuccess_WhenAlreadyExists()
{
    // Arrange: Plano FREE já existe
    // Act: Chamar SeedFreePlanAsync
    // Assert: Retorna sucesso sem criar duplicado
}

[Fact]
public async Task SeedFreePlanAsync_ValidatesBasicCapabilities()
{
    // Arrange: Seed de plano FREE
    // Act: Chamar SeedFreePlanAsync
    // Assert: Funcionalidades básicas validadas
}

[Fact]
public async Task SeedFreePlanAsync_SetsCorrectLimits()
{
    // Arrange: Seed de plano FREE
    // Act: Chamar SeedFreePlanAsync
    // Assert: Limites padrão configurados corretamente
}
```

---

### 10. SubscriptionIntegrationTests (6 cenários)

**Arquivo**: `backend/Araponga.Tests/Api/SubscriptionIntegrationTests.cs`

```csharp
[Fact]
public async Task POST_Subscriptions_CreatesSubscription_WhenValidData()
{
    // Arrange: Dados válidos de assinatura
    // Act: POST /api/v1/subscriptions
    // Assert: Assinatura criada e retornada
}

[Fact]
public async Task GET_Subscriptions_Me_ReturnsSubscription_WhenExists()
{
    // Arrange: Usuário com assinatura
    // Act: GET /api/v1/subscriptions/me
    // Assert: Assinatura retornada
}

[Fact]
public async Task PATCH_Subscriptions_UpdatesSubscription_WhenValidData()
{
    // Arrange: Assinatura existente e dados de atualização
    // Act: PATCH /api/v1/subscriptions/{id}
    // Assert: Assinatura atualizada
}

[Fact]
public async Task POST_Subscriptions_Cancel_CancelsSubscription_WhenValid()
{
    // Arrange: Assinatura ativa
    // Act: POST /api/v1/subscriptions/{id}/cancel
    // Assert: Assinatura cancelada
}

[Fact]
public async Task GET_SubscriptionPlans_ReturnsPlans_ForTerritory()
{
    // Arrange: Planos disponíveis para território
    // Act: GET /api/v1/subscription-plans?territoryId={id}
    // Assert: Planos retornados corretamente
}

[Fact]
public async Task GET_Admin_Subscriptions_Analytics_ReturnsAnalytics_WhenAuthorized()
{
    // Arrange: Usuário SystemAdmin e dados de analytics
    // Act: GET /api/v1/admin/subscriptions/analytics
    // Assert: Analytics retornados
}
```

---

## 📊 Resumo

| Categoria | Cenários | Prioridade | Estimativa |
|-----------|----------|------------|------------|
| SubscriptionAnalyticsService | 12 | 🔴 Crítica | 8h |
| SubscriptionPlanAdminService | 10 | 🔴 Crítica | 6h |
| CouponService | 10 | 🔴 Crítica | 6h |
| StripeWebhookService | 10 | 🔴 Crítica | 6h |
| MercadoPagoWebhookService | 6 | 🔴 Crítica | 4h |
| SubscriptionRenewalService | 6 | 🔴 Crítica | 4h |
| SubscriptionTrialService | 7 | 🔴 Crítica | 4h |
| SubscriptionService (adicionais) | 10 | 🟡 Importante | 6h |
| SubscriptionPlanSeedService | 4 | 🟡 Importante | 2h |
| SubscriptionIntegrationTests | 6 | 🟡 Importante | 6h |
| **Total** | **81** | | **52h** |

---

**Última Atualização**: 2026-01-26
