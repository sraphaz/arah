namespace Araponga.Modules.Assets.Infrastructure.Postgres.Entities;

public sealed class AssetGeoAnchorRecord
{
    public Guid Id { get; set; }
    public Guid AssetId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
