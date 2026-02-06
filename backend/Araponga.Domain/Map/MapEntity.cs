namespace Araponga.Domain.Map;

public sealed class MapEntity
{
    public MapEntity(
        Guid id,
        Guid territoryId,
        Guid createdByUserId,
        string name,
        string category,
        double latitude,
        double longitude,
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

        if (!Geo.GeoCoordinate.IsValid(latitude, longitude))
        {
            throw new ArgumentOutOfRangeException(nameof(latitude), "Latitude/longitude is invalid.");
        }

        if (createdByUserId == Guid.Empty)
        {
            throw new ArgumentException("Created-by user ID is required.", nameof(createdByUserId));
        }

        Id = id;
        TerritoryId = territoryId;
        CreatedByUserId = createdByUserId;
        Name = name.Trim();
        Category = category.Trim();
        Latitude = latitude;
        Longitude = longitude;
        Status = status;
        Visibility = visibility;
        // Proteção contra valores que excedem int.MaxValue (pode vir do banco de dados)
        ConfirmationCount = confirmationCount > int.MaxValue ? int.MaxValue : confirmationCount;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid Id { get; }
    public Guid TerritoryId { get; }
    public Guid CreatedByUserId { get; }
    public string Name { get; }
    public string Category { get; }
    public double Latitude { get; }
    public double Longitude { get; }
    public MapEntityStatus Status { get; }
    public MapEntityVisibility Visibility { get; }
    public int ConfirmationCount { get; }
    public DateTime CreatedAtUtc { get; }
}
