namespace Araponga.Domain.Subscriptions;

/// <summary>
/// Escopo do plano (Global ou Territorial).
/// </summary>
public enum PlanScope
{
    /// <summary>
    /// Plano global - aplica a todos os territórios.
    /// </summary>
    Global = 1,

    /// <summary>
    /// Plano territorial - específico de um território.
    /// </summary>
    Territory = 2
}
