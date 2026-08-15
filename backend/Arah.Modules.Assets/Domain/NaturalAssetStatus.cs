namespace Arah.Modules.Assets.Domain;

/// <summary>
/// Máquina de estado canônica de NaturalAsset (FASE24.0). Distinta de <see cref="AssetStatus"/> da ponte TerritoryAsset.
/// </summary>
public enum NaturalAssetStatus
{
    Pending = 1,
    Published = 2,
    Hidden = 3,
    Review = 4
}
