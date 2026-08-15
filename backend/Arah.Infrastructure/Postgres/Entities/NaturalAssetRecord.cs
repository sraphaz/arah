using Arah.Modules.Assets.Domain;

namespace Arah.Infrastructure.Postgres.Entities;

public sealed class NaturalAssetRecord
{
    public Guid Id { get; set; }
    public Guid TerritoryId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public NaturalAssetStatus Status { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? WaterType { get; set; }
    public string? PotabilityNotes { get; set; }
    public DateTime? LastTestedAtUtc { get; set; }
    public Guid CreatedByUserId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public Guid UpdatedByUserId { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
