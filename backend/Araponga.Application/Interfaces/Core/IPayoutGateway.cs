using Araponga.Application.Common;

namespace Araponga.Application.Interfaces;

/// <summary>
/// Interface para abstrair gateways de payout (transferência para vendedores).
/// Permite trocar facilmente entre diferentes gateways (Stripe Connect, MercadoPago, etc.).
/// </summary>
public interface IPayoutGateway
{
    /// <summary>
    /// Cria um payout (transferência) para um vendedor.
    /// </summary>
    /// <param name="amountInCents">Valor em centavos</param>
    /// <param name="currency">Moeda (ex: "BRL", "USD")</param>
    /// <param name="sellerAccountId">ID da conta do vendedor no gateway</param>
    /// <param name="description">Descrição do payout</param>
    /// <param name="metadata">Metadados adicionais</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado com ID do payout e status</returns>
    Task<OperationResult<PayoutResult>> CreatePayoutAsync(
        long amountInCents,
        string currency,
        string sellerAccountId,
        string description,
        Dictionary<string, string>? metadata,
        CancellationToken cancellationToken);

    /// <summary>
    /// Obtém o status de um payout.
    /// </summary>
    /// <param name="payoutId">ID do payout no gateway</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Status do payout</returns>
    Task<OperationResult<PayoutStatusResult>> GetPayoutStatusAsync(
        string payoutId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Cancela um payout pendente.
    /// </summary>
    /// <param name="payoutId">ID do payout no gateway</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação</returns>
    Task<OperationResult> CancelPayoutAsync(
        string payoutId,
        CancellationToken cancellationToken);
}

/// <summary>
/// Resultado da criação de um payout.
/// </summary>
public record PayoutResult(
    string PayoutId,
    PayoutStatus Status,
    DateTime CreatedAtUtc);

/// <summary>
/// Status de um payout.
/// </summary>
public enum PayoutStatus
{
    /// <summary>
    /// Pendente (aguardando processamento).
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Processando (em processamento).
    /// </summary>
    Processing = 2,

    /// <summary>
    /// Concluído (transferência realizada).
    /// </summary>
    Completed = 3,

    /// <summary>
    /// Falhou (transferência falhou).
    /// </summary>
    Failed = 4,

    /// <summary>
    /// Cancelado (cancelado antes de processar).
    /// </summary>
    Canceled = 5
}

/// <summary>
/// Resultado da consulta de status de um payout.
/// </summary>
public record PayoutStatusResult(
    string PayoutId,
    PayoutStatus Status,
    string? FailureReason,
    DateTime? CompletedAtUtc);
