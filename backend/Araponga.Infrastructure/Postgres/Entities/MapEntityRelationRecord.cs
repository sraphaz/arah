namespace Araponga.Infrastructure.Postgres.Entities;

public sealed class MapEntityRelationRecord
{
    public Guid UserId { get; set; }
    public Guid EntityId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
