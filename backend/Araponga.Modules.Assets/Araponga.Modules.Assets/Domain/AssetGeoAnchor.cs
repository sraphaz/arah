namespace Araponga.Modules.Assets.Domain;

public sealed class AssetGeoAnchor
{
    public AssetGeoAnchor(
        Guid id,
        Guid assetId,
        double latitude,
        double longitude,
        DateTime createdAtUtc)
    {
        Id = id;
        AssetId = assetId;
        Latitude = latitude;
        Longitude = longitude;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid Id { get; }
    public Guid AssetId { get; }
    public double Latitude { get; }
    public double Longitude { get; }
    public DateTime CreatedAtUtc { get; }
}
