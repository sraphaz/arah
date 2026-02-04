namespace Araponga.Modules.Map.Domain;

public sealed class MapEntityRelation
{
    public MapEntityRelation(Guid userId, Guid entityId, DateTime createdAtUtc)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User ID is required.", nameof(userId));
        }

        if (entityId == Guid.Empty)
        {
            throw new ArgumentException("Entity ID is required.", nameof(entityId));
        }

        UserId = userId;
        EntityId = entityId;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid UserId { get; }
    public Guid EntityId { get; }
    public DateTime CreatedAtUtc { get; }
}
