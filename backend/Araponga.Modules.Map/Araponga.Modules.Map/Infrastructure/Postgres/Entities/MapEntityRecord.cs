using Araponga.Modules.Map.Domain;

namespace Araponga.Modules.Map.Infrastructure.Postgres.Entities;

public sealed class MapEntityRecord
{
    public Guid Id { get; set; }
    public Guid TerritoryId { get; set; }
    public Guid CreatedByUserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public MapEntityStatus Status { get; set; }
    public MapEntityVisibility Visibility { get; set; }
    public int ConfirmationCount { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
