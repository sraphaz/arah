namespace Araponga.Modules.Feed.Infrastructure.Postgres.Entities;

public sealed class PostGeoAnchorRecord
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Type { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
}
