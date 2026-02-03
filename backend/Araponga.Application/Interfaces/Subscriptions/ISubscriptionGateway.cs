using Araponga.Application.Common;
using Araponga.Domain.Subscriptions;

namespace Araponga.Application.Interfaces;

/// <summary>
/// Interface genérica para gateways de assinatura (Stripe, Mercado Pago, etc.).
/// Permite trocar facilmente entre diferentes gateways.
/// </summary>
public interface ISubscriptionGateway
{
    /// <summary>
    /// Nome do gateway (ex: "Stripe", "MercadoPago").
    /// </summary>
    string GatewayName { get; }

    /// <summary>
    /// Cria uma assinatura no gateway.
    /// </summary>
    Task<OperationResult<SubscriptionGatewayResult>> CreateSubscriptionAsync(
        Guid userId,
        Guid planId,
        string? couponCode,
        CancellationToken cancellationToken);

    /// <summary>
    /// Atualiza uma assinatura (upgrade/downgrade).
    /// </summary>
    Task<OperationResult<SubscriptionGatewayResult>> UpdateSubscriptionAsync(
        Guid subscriptionId,
        Guid newPlanId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Cancela uma assinatura.
    /// </summary>
    Task<OperationResult> CancelSubscriptionAsync(
        Guid subscriptionId,
        bool cancelAtPeriodEnd,
        CancellationToken cancellationToken);

    /// <summary>
    /// Reativa uma assinatura cancelada.
    /// </summary>
    Task<OperationResult> ReactivateSubscriptionAsync(
        Guid subscriptionId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Obtém informações de uma assinatura do gateway.
    /// </summary>
    Task<SubscriptionGatewayInfo?> GetSubscriptionAsync(
        string gatewaySubscriptionId,
        CancellationToken cancellationToken);
}

/// <summary>
/// Resultado da criação/atualização de assinatura no gateway.
/// </summary>
public sealed class SubscriptionGatewayResult
{
    public string GatewaySubscriptionId { get; init; } = string.Empty;
    public string GatewayCustomerId { get; init; } = string.Empty;
    public DateTime CurrentPeriodStart { get; init; }
    public DateTime CurrentPeriodEnd { get; init; }
    public SubscriptionStatus Status { get; init; }
}

/// <summary>
/// Informações de uma assinatura no gateway.
/// </summary>
public sealed class SubscriptionGatewayInfo
{
    public string GatewaySubscriptionId { get; init; } = string.Empty;
    public string GatewayCustomerId { get; init; } = string.Empty;
    public SubscriptionStatus Status { get; init; }
    public DateTime CurrentPeriodStart { get; init; }
    public DateTime CurrentPeriodEnd { get; init; }
    public DateTime? TrialEnd { get; init; }
    public bool CancelAtPeriodEnd { get; init; }
}
