using Araponga.Domain.Map;

namespace Araponga.Infrastructure.Postgres.Entities;

public sealed class MapEntityRecord
{
    public Guid Id { get; set; }
    public Guid TerritoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public MapEntityStatus Status { get; set; }
    public MapEntityVisibility Visibility { get; set; }
    public int ConfirmationCount { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
