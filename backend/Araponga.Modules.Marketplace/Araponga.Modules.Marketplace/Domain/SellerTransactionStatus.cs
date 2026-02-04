namespace Araponga.Modules.Marketplace.Domain;

/// <summary>
/// Status de uma transação de vendedor.
/// </summary>
public enum SellerTransactionStatus
{
    /// <summary>
    /// Pendente (aguardando período de retenção ou valor mínimo).
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Pronto para payout (atingiu condições de payout).
    /// </summary>
    ReadyForPayout = 2,

    /// <summary>
    /// Processando payout (payout em processamento).
    /// </summary>
    ProcessingPayout = 3,

    /// <summary>
    /// Pago (payout concluído).
    /// </summary>
    Paid = 4,

    /// <summary>
    /// Falhou (payout falhou).
    /// </summary>
    Failed = 5,

    /// <summary>
    /// Cancelada (cancelada antes de payout).
    /// </summary>
    Canceled = 6
}
