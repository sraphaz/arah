using Araponga.Domain.Financial;

namespace Araponga.Modules.Marketplace.Infrastructure.Postgres.Entities;

public sealed class FinancialTransactionRecord
{
    public Guid Id { get; set; }
    public Guid TerritoryId { get; set; }
    public int Type { get; set; } // TransactionType enum
    public int Status { get; set; } // TransactionStatus enum
    public long AmountInCents { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid? RelatedEntityId { get; set; }
    public string? RelatedEntityType { get; set; }
    public string RelatedTransactionIdsJson { get; set; } = "[]"; // JSON array of Guids
    public string? MetadataJson { get; set; } // JSON object
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
