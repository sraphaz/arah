using Araponga.Modules.Assets.Domain;

namespace Araponga.Application.Models;

public sealed record TerritoryAssetDetails(
    TerritoryAsset Asset,
    IReadOnlyList<AssetGeoAnchor> Anchors,
    int ValidationsCount,
    int EligibleResidentsCount)
{
    public double ValidationPct => EligibleResidentsCount == 0
        ? 0
        : (double)ValidationsCount / EligibleResidentsCount;
}
