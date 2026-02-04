using Araponga.Domain.Financial;

namespace Araponga.Modules.Marketplace.Infrastructure.Postgres.Entities;

public sealed class TransactionStatusHistoryRecord
{
    public Guid Id { get; set; }
    public Guid FinancialTransactionId { get; set; }
    public int PreviousStatus { get; set; } // TransactionStatus enum
    public int NewStatus { get; set; } // TransactionStatus enum
    public Guid? ChangedByUserId { get; set; }
    public string? Reason { get; set; }
    public DateTime ChangedAtUtc { get; set; }
}
