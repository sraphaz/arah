using Araponga.Domain.Marketplace;

namespace Araponga.Modules.Marketplace.Infrastructure.Postgres.Entities;

public sealed class PlatformFeeConfigRecord
{
    public Guid Id { get; set; }
    public Guid TerritoryId { get; set; }
    public ItemType ItemType { get; set; }
    public PlatformFeeMode FeeMode { get; set; }
    public decimal FeeValue { get; set; }
    public string? Currency { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
