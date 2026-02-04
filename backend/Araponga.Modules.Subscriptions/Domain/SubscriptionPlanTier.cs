namespace Araponga.Domain.Subscriptions;

/// <summary>
/// Tier (nível) do plano de assinatura.
/// </summary>
public enum SubscriptionPlanTier
{
    /// <summary>
    /// Plano gratuito - padrão para visitantes e residentes.
    /// Funcionalidades básicas sempre disponíveis.
    /// </summary>
    FREE = 0,

    /// <summary>
    /// Plano básico pago.
    /// </summary>
    BASIC = 1,

    /// <summary>
    /// Plano intermediário pago.
    /// </summary>
    INTERMEDIATE = 2,

    /// <summary>
    /// Plano premium pago.
    /// </summary>
    PREMIUM = 3,

    /// <summary>
    /// Plano empresarial (custom).
    /// </summary>
    ENTERPRISE = 4
}
