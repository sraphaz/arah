using Arah.Domain.Financial;

namespace Arah.Application.Services;

/// <summary>
/// Fonte única da matemática de split (FASE55). Usa banker's rounding e garante
/// reconciliação: a soma das linhas é exatamente igual à taxa (plataforma recebe o resto).
/// </summary>
public static class FeeSplitCalculator
{
    public const string Implementer = "implementer";
    public const string TerritoryFund = "territory_fund";
    public const string Platform = "platform";

    public static IReadOnlyList<FeeSplitLine> Build(decimal feeAmount, FeeSplitRule rule)
    {
        var implementer = RoundBankers(feeAmount * rule.ImplementerSharePercent / 100m);
        var territoryFund = RoundBankers(feeAmount * rule.TerritoryFundSharePercent / 100m);
        var platform = RoundBankers(feeAmount - implementer - territoryFund);

        return new[]
        {
            new FeeSplitLine(Implementer, implementer),
            new FeeSplitLine(TerritoryFund, territoryFund),
            new FeeSplitLine(Platform, platform),
        };
    }

    /// <summary>Nega cada linha do split (usado no estorno — AC-55-6).</summary>
    public static IReadOnlyList<FeeSplitLine> Reverse(IReadOnlyList<FeeSplitLine> split) =>
        split.Select(l => new FeeSplitLine(l.Recipient, -l.Amount)).ToList();

    public static decimal RoundBankers(decimal value) =>
        Math.Round(value, 2, MidpointRounding.ToEven);
}
