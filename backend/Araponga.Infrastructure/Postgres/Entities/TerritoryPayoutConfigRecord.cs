using Araponga.Domain.Marketplace;

namespace Araponga.Infrastructure.Postgres.Entities;

public sealed class TerritoryPayoutConfigRecord
{
    public Guid Id { get; set; }
    public Guid TerritoryId { get; set; }
    public int RetentionPeriodDays { get; set; }
    public long MinimumPayoutAmountInCents { get; set; }
    public long? MaximumPayoutAmountInCents { get; set; }
    public int Frequency { get; set; } // PayoutFrequency enum
    public bool AutoPayoutEnabled { get; set; }
    public bool RequiresApproval { get; set; }
    public string Currency { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
