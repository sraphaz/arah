namespace Araponga.Domain.Territories;

/// <summary>
/// Representa um território geográfico. Territory não possui Owner individual.
/// A governança é exercida por papéis territoriais atribuídos a memberships via TerritoryMembership.
/// </summary>
public sealed class Territory
{
    public Territory(
        Guid id,
        Guid? parentTerritoryId,
        string name,
        string? description,
        TerritoryStatus status,
        string city,
        string state,
        double latitude,
        double longitude,
        DateTime createdAtUtc,
        double? radiusKm = null,
        IReadOnlyList<TerritoryBoundaryPoint>? boundaryPolygon = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Territory name is required.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(city))
        {
            throw new ArgumentException("City is required.", nameof(city));
        }

        if (string.IsNullOrWhiteSpace(state))
        {
            throw new ArgumentException("State is required.", nameof(state));
        }

        if (radiusKm.HasValue && radiusKm.Value <= 0)
        {
            throw new ArgumentException("RadiusKm must be positive when provided.", nameof(radiusKm));
        }

        if (boundaryPolygon is { Count: < 3 })
        {
            throw new ArgumentException("Boundary polygon must have at least 3 points when provided.", nameof(boundaryPolygon));
        }

        Id = id;
        ParentTerritoryId = parentTerritoryId;
        Name = name.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        Status = status;
        City = city.Trim();
        State = state.Trim();
        Latitude = latitude;
        Longitude = longitude;
        CreatedAtUtc = createdAtUtc;
        RadiusKm = radiusKm;
        BoundaryPolygon = boundaryPolygon;
    }

    public Guid Id { get; }
    public Guid? ParentTerritoryId { get; }
    public string Name { get; }
    public string? Description { get; }
    public TerritoryStatus Status { get; }
    public string City { get; }
    public string State { get; }
    public double Latitude { get; }
    public double Longitude { get; }
    public DateTime CreatedAtUtc { get; }

    /// <summary>
    /// Raio do perímetro do território em km. Quando null, usa o valor padrão do sistema (ex.: 5 km) para convergência geo e verificação de residência.
    /// Ignorado para desenho no mapa quando <see cref="BoundaryPolygon"/> estiver preenchido.
    /// </summary>
    public double? RadiusKm { get; }

    /// <summary>
    /// Polígono do perímetro do território (lista ordenada de pontos no mapa).
    /// Quando preenchido, o perímetro é exibido como contorno no mapa; caso contrário usa-se centro (Latitude, Longitude) + RadiusKm.
    /// Permite cadastro de território a partir do desenho no mapa no futuro.
    /// </summary>
    public IReadOnlyList<TerritoryBoundaryPoint>? BoundaryPolygon { get; }
}
