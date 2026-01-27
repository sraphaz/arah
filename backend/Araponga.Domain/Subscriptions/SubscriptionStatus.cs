namespace Araponga.Domain.Subscriptions;

/// <summary>
/// Status da assinatura.
/// </summary>
public enum SubscriptionStatus
{
    /// <summary>
    /// Assinatura ativa.
    /// </summary>
    ACTIVE = 0,

    /// <summary>
    /// Assinatura cancelada.
    /// </summary>
    CANCELED = 1,

    /// <summary>
    /// Assinatura atrasada (pagamento pendente).
    /// </summary>
    PAST_DUE = 2,

    /// <summary>
    /// Assinatura não paga.
    /// </summary>
    UNPAID = 3,

    /// <summary>
    /// Assinatura em período de trial.
    /// </summary>
    TRIALING = 4,

    /// <summary>
    /// Assinatura expirada.
    /// </summary>
    EXPIRED = 5
}
