namespace Araponga.Domain.Map;

public sealed class MapEntity
{
    public MapEntity(
        Guid id,
        Guid territoryId,
        Guid createdByUserId,
        string name,
        string category,
        MapEntityStatus status,
        MapEntityVisibility visibility,
        int confirmationCount,
        DateTime createdAtUtc)
    {
        if (territoryId == Guid.Empty)
        {
            throw new ArgumentException("Territory ID is required.", nameof(territoryId));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name is required.", nameof(name));
        }

        if (!MapEntityCategory.TryNormalize(category, out var normalizedCategory))
        {
            throw new ArgumentException(
                $"Category must be one of: {MapEntityCategory.AllowedList}.",
                nameof(category));
        }

        if (createdByUserId == Guid.Empty)
        {
            throw new ArgumentException("Created-by user ID is required.", nameof(createdByUserId));
        }

        Id = id;
        TerritoryId = territoryId;
        CreatedByUserId = createdByUserId;
        Name = name.Trim();
        Category = normalizedCategory;
        Status = status;
        Visibility = visibility;
        ConfirmationCount = confirmationCount;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid Id { get; }
    public Guid TerritoryId { get; }
    public Guid CreatedByUserId { get; }
    public string Name { get; }
    public string Category { get; }
    public MapEntityStatus Status { get; }
    public MapEntityVisibility Visibility { get; }
    public int ConfirmationCount { get; }
    public DateTime CreatedAtUtc { get; }
}
