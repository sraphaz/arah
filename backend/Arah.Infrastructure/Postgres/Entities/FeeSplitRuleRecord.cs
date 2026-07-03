namespace Arah.Infrastructure.Postgres.Entities;

public sealed class FeeSplitRuleRecord
{
    public Guid Id { get; set; }
    public Guid TerritoryId { get; set; }
    public string RevenueType { get; set; } = string.Empty;
    public decimal ImplementerSharePercent { get; set; }
    public decimal TerritoryFundSharePercent { get; set; }
    public decimal PlatformSharePercent { get; set; }
    public DateTimeOffset EffectiveFromUtc { get; set; }
    public DateTimeOffset? SupersededAtUtc { get; set; }
}
