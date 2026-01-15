using Araponga.Domain.Financial;

namespace Araponga.Infrastructure.Postgres.Entities;

public sealed class ReconciliationRecordRecord
{
    public Guid Id { get; set; }
    public Guid TerritoryId { get; set; }
    public DateTime ReconciliationDate { get; set; }
    public long ExpectedAmountInCents { get; set; }
    public long ActualAmountInCents { get; set; }
    public long DifferenceInCents { get; set; }
    public string Currency { get; set; } = string.Empty;
    public int Status { get; set; } // ReconciliationStatus enum
    public string? Notes { get; set; }
    public Guid? ReconciledByUserId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
