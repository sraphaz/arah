using Arah.Domain.Territories;
using Arah.Infrastructure.Postgres.Entities;
using System.Text.Json;

namespace Arah.Infrastructure.Postgres;

public static partial class PostgresMappers
{
    public static TerritoryRecord ToRecord(this Territory territory)
    {
        return new TerritoryRecord
        {
            Id = territory.Id,
            ParentTerritoryId = territory.ParentTerritoryId,
            Name = territory.Name,
            Description = territory.Description,
            Status = territory.Status,
            City = territory.City,
            State = territory.State,
            Latitude = territory.Latitude,
            Longitude = territory.Longitude,
            CreatedAtUtc = territory.CreatedAtUtc,
            RadiusKm = territory.RadiusKm,
            BoundaryPolygonJson = territory.BoundaryPolygon is null or { Count: 0 }
                ? null
                : JsonSerializer.Serialize(territory.BoundaryPolygon),
        };
    }

    public static Territory ToDomain(this TerritoryRecord record)
    {
        IReadOnlyList<TerritoryBoundaryPoint>? boundaryPolygon = null;
        if (!string.IsNullOrWhiteSpace(record.BoundaryPolygonJson))
        {
            try
            {
                var list = JsonSerializer.Deserialize<List<TerritoryBoundaryPoint>>(record.BoundaryPolygonJson);
                if (list is { Count: >= 3 })
                    boundaryPolygon = list;
            }
            catch
            {
                // Ignore invalid JSON; treat as null polygon
            }
        }

        return new Territory(
            record.Id,
            record.ParentTerritoryId,
            record.Name,
            record.Description,
            record.Status,
            record.City,
            record.State,
            record.Latitude,
            record.Longitude,
            record.CreatedAtUtc,
            record.RadiusKm,
            boundaryPolygon);
    }
}
