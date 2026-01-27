# Fase 15: Integrações Reais com Gateways de Pagamento

**Data**: 2026-01-25  
**Status**: ✅ **Implementado com Fallback para Mock**

---

## 📋 Visão Geral

As integrações com Stripe e Mercado Pago foram implementadas com suporte automático para:
- ✅ **Integração Real**: Quando credenciais estão configuradas
- ✅ **Mock Mode**: Quando credenciais não estão configuradas (desenvolvimento)

---

## 🔧 Configuração

### Stripe

**Variáveis de Ambiente**:
```bash
Stripe__SecretKey=sk_test_...  # ou sk_live_... para produção
Stripe__WebhookSecret=whsec_...
```

**Configuração no appsettings.json**:
```json
{
  "Stripe": {
    "SecretKey": "sk_test_...",
    "WebhookSecret": "whsec_...",
    "PublishableKey": "pk_test_..." // Opcional, para frontend
  }
}
```

**Como Obter**:
1. Acesse [Stripe Dashboard](https://dashboard.stripe.com/)
2. Vá em **Developers > API keys**
3. Copie a **Secret key** (test ou live)
4. Para webhook secret, vá em **Developers > Webhooks**
5. Crie um endpoint e copie o **Signing secret**

### Mercado Pago

**Variáveis de Ambiente**:
```bash
MercadoPago__AccessToken=TEST-...  # ou APP_USR-... para produção
MercadoPago__WebhookSecret=...
```

**Configuração no appsettings.json**:
```json
{
  "MercadoPago": {
    "AccessToken": "TEST-...",
    "WebhookSecret": "..."
  }
}
```

**Como Obter**:
1. Acesse [Mercado Pago Developers](https://www.mercadopago.com.br/developers)
2. Crie uma aplicação
3. Copie o **Access Token** (test ou production)
4. Configure webhook e copie o secret

---

## 🚀 Funcionalidades Implementadas

### Stripe

#### ✅ Criação de Assinaturas
- Busca ou cria cliente no Stripe automaticamente
- Cria preço dinamicamente se `StripePriceId` não estiver configurado
- Suporta cupons e trials
- Metadata com `userId` e `planId` para rastreamento

#### ✅ Atualização de Assinaturas
- Atualiza plano da assinatura
- Cria proratação automática
- Atualiza metadata

#### ✅ Cancelamento
- Cancelamento imediato ou ao fim do período
- Preserva acesso até fim do período quando `cancelAtPeriodEnd = true`

#### ✅ Reativação
- Remove cancelamento agendado
- Reativa assinatura imediatamente

#### ✅ Busca de Assinaturas
- Busca assinatura por `StripeSubscriptionId`
- Retorna informações completas (status, períodos, trial, etc.)

### Mercado Pago

**Status**: ⚠️ Mock implementado, aguardando SDK oficial ou implementação manual

**Nota**: A estrutura está pronta para integração real. Quando o SDK do Mercado Pago estiver disponível ou quando houver necessidade, a implementação seguirá o mesmo padrão do Stripe.

---

## 🔄 Fluxo de Funcionamento

### Modo Real (Com Credenciais)

1. **Verificação**: Sistema verifica se `Stripe:SecretKey` está configurado
2. **Inicialização**: Configura `StripeConfiguration.ApiKey` automaticamente
3. **Operações**: Todas as operações usam Stripe.net SDK
4. **Logging**: Logs detalhados de todas as operações

### Modo Mock (Sem Credenciais)

1. **Verificação**: Sistema detecta ausência de credenciais
2. **Fallback**: Usa implementação mock
3. **Logging**: Logs de warning indicando uso de mock
4. **Funcionalidade**: Sistema funciona normalmente para desenvolvimento/testes

---

## 📝 Exemplo de Uso

### Criar Assinatura

```csharp
var gateway = _gatewayFactory.GetGateway(); // Retorna Stripe ou Mercado Pago
var result = await gateway.CreateSubscriptionAsync(
    userId: userId,
    planId: planId,
    couponCode: "DESCONTO10",
    cancellationToken: cancellationToken);

if (result.IsSuccess)
{
    var subscriptionId = result.Value!.GatewaySubscriptionId;
    // Usar subscriptionId para atualizar assinatura local
}
```

### Atualizar Assinatura

```csharp
var result = await gateway.UpdateSubscriptionAsync(
    subscriptionId: subscriptionId,
    newPlanId: newPlanId,
    cancellationToken: cancellationToken);
```

### Cancelar Assinatura

```csharp
var result = await gateway.CancelSubscriptionAsync(
    subscriptionId: subscriptionId,
    cancelAtPeriodEnd: true, // Cancela ao fim do período
    cancellationToken: cancellationToken);
```

---

## ⚠️ Importante

### Em Desenvolvimento
- ✅ Mock funciona perfeitamente
- ✅ Não precisa de credenciais
- ✅ Logs indicam uso de mock

### Em Produção
- ⚠️ **OBRIGATÓRIO** configurar credenciais reais
- ⚠️ Sistema detecta ausência e usa mock (com warnings)
- ⚠️ Configure webhook secrets para validação

### Segurança
- ✅ Secrets nunca são logados
- ✅ Validação de webhook obrigatória em produção
- ✅ Metadata para rastreamento sem expor dados sensíveis

---

## 🧪 Testes

Os testes podem ser executados em dois modos:

1. **Mock Mode** (padrão): Sem credenciais, usa mocks
2. **Integration Mode**: Com credenciais de teste, testa integração real

Para testes de integração real, configure:
```bash
Stripe__SecretKey=sk_test_...
```

---

## 📊 Status de Implementação

| Funcionalidade | Stripe | Mercado Pago |
|----------------|--------|--------------|
| Criar Assinatura | ✅ Real | ⚠️ Mock |
| Atualizar Assinatura | ✅ Real | ⚠️ Mock |
| Cancelar Assinatura | ✅ Real | ⚠️ Mock |
| Reativar Assinatura | ✅ Real | ⚠️ Mock |
| Buscar Assinatura | ✅ Real | ⚠️ Mock |
| Validação de Webhook | ✅ Real | ✅ Real |

---

**Última Atualização**: 2026-01-25
