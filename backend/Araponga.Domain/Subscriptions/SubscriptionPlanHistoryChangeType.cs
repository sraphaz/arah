namespace Araponga.Domain.Subscriptions;

/// <summary>
/// Tipo de mudança no histórico do plano.
/// </summary>
public enum SubscriptionPlanHistoryChangeType
{
    /// <summary>
    /// Plano criado.
    /// </summary>
    Created = 0,

    /// <summary>
    /// Plano atualizado.
    /// </summary>
    Updated = 1,

    /// <summary>
    /// Plano ativado.
    /// </summary>
    Activated = 2,

    /// <summary>
    /// Plano desativado.
    /// </summary>
    Deactivated = 3,

    /// <summary>
    /// Capacidades do plano alteradas.
    /// </summary>
    CapabilitiesChanged = 4,

    /// <summary>
    /// Preço do plano alterado.
    /// </summary>
    PriceChanged = 5
}
