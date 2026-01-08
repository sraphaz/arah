namespace Araponga.Domain.Map;

public sealed class MapEntity
{
    public MapEntity(
        Guid id,
        Guid territoryId,
        string name,
        string category,
        MapEntityVisibility visibility,
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

        if (string.IsNullOrWhiteSpace(category))
        {
            throw new ArgumentException("Category is required.", nameof(category));
        }

        Id = id;
        TerritoryId = territoryId;
        Name = name.Trim();
        Category = category.Trim();
        Visibility = visibility;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid Id { get; }
    public Guid TerritoryId { get; }
    public string Name { get; }
    public string Category { get; }
    public MapEntityVisibility Visibility { get; }
    public DateTime CreatedAtUtc { get; }
}
