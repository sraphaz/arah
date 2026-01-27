namespace Araponga.Domain.Subscriptions;

/// <summary>
/// Ciclo de cobrança da assinatura.
/// </summary>
public enum SubscriptionBillingCycle
{
    /// <summary>
    /// Mensal - cobrança a cada 30 dias.
    /// </summary>
    MONTHLY = 0,

    /// <summary>
    /// Trimestral - cobrança a cada 90 dias (desconto de 10%).
    /// </summary>
    QUARTERLY = 1,

    /// <summary>
    /// Anual - cobrança a cada 365 dias (desconto de 20%).
    /// </summary>
    YEARLY = 2
}
