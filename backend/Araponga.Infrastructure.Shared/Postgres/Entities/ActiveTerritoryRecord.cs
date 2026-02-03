namespace Araponga.Infrastructure.Shared.Postgres.Entities;

public sealed class ActiveTerritoryRecord
{
    public string SessionId { get; set; } = string.Empty;
    public Guid TerritoryId { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
