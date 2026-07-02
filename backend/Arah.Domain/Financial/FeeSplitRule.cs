namespace Arah.Domain.Financial;

/// <summary>
/// Regra de split da taxa — imutável após vigência; nova versão via insert + supersede.
/// </summary>
public sealed class FeeSplitRule
{
    public FeeSplitRule(
        Guid id,
        Guid territoryId,
        string revenueType,
        decimal implementerSharePercent,
        decimal territoryFundSharePercent,
        decimal platformSharePercent,
        DateTimeOffset effectiveFromUtc)
    {
        if (territoryId == Guid.Empty)
        {
            throw new ArgumentException("Territory ID is required.", nameof(territoryId));
        }

        if (string.IsNullOrWhiteSpace(revenueType))
        {
            throw new ArgumentException("Revenue type is required.", nameof(revenueType));
        }

        ValidatePercentages(implementerSharePercent, territoryFundSharePercent, platformSharePercent);

        Id = id;
        TerritoryId = territoryId;
        RevenueType = revenueType.Trim();
        ImplementerSharePercent = implementerSharePercent;
        TerritoryFundSharePercent = territoryFundSharePercent;
        PlatformSharePercent = platformSharePercent;
        EffectiveFromUtc = effectiveFromUtc;
    }

    public Guid Id { get; }
    public Guid TerritoryId { get; }
    public string RevenueType { get; }
    public decimal ImplementerSharePercent { get; }
    public decimal TerritoryFundSharePercent { get; }
    public decimal PlatformSharePercent { get; }
    public DateTimeOffset EffectiveFromUtc { get; }
    public DateTimeOffset? SupersededAtUtc { get; private set; }

    public bool IsActiveAt(DateTimeOffset atUtc) =>
        EffectiveFromUtc <= atUtc && SupersededAtUtc is null;

    public void Supersede(DateTimeOffset supersededAtUtc)
    {
        if (SupersededAtUtc is not null)
        {
            throw new InvalidOperationException("Rule is already superseded.");
        }

        SupersededAtUtc = supersededAtUtc;
    }

    private static void ValidatePercentages(
        decimal implementer,
        decimal territoryFund,
        decimal platform)
    {
        if (implementer < 0 || territoryFund < 0 || platform < 0)
        {
            throw new ArgumentException("Split percentages must be non-negative.");
        }

        var total = implementer + territoryFund + platform;
        if (total != 100m)
        {
            throw new ArgumentException("Split percentages must sum to 100.");
        }
    }
}
