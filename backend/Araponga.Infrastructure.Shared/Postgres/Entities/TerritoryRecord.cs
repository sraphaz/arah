using Araponga.Domain.Territories;

namespace Araponga.Infrastructure.Shared.Postgres.Entities;

public sealed class TerritoryRecord
{
    public Guid Id { get; set; }
    public Guid? ParentTerritoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TerritoryStatus Status { get; set; }
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public double? RadiusKm { get; set; }
    /// <summary>Polígono do perímetro (JSON array de { Latitude, Longitude }). jsonb no Postgres.</summary>
    public string? BoundaryPolygonJson { get; set; }
}
