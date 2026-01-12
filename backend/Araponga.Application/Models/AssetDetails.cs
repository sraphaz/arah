using Araponga.Domain.Assets;

namespace Araponga.Application.Models;

public sealed record AssetDetails(
    TerritoryAsset Asset,
    IReadOnlyList<AssetGeoAnchor> Anchors,
    int ValidationsCount,
    int EligibleResidentsCount)
{
    public double ValidationPct => EligibleResidentsCount == 0
        ? 0
        : (double)ValidationsCount / EligibleResidentsCount;
}
