using Araponga.Domain.Territories;

namespace Araponga.Infrastructure.Postgres.Entities;

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
    /// <summary>Raio do território em km; null = usar padrão do sistema.</summary>
    public double? RadiusKm { get; set; }
}
