namespace Araponga.Domain.Subscriptions;

/// <summary>
/// Status do pagamento da assinatura.
/// </summary>
public enum SubscriptionPaymentStatus
{
    /// <summary>
    /// Pagamento pendente.
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Pagamento bem-sucedido.
    /// </summary>
    Succeeded = 1,

    /// <summary>
    /// Pagamento falhou.
    /// </summary>
    Failed = 2,

    /// <summary>
    /// Pagamento reembolsado.
    /// </summary>
    Refunded = 3
}
