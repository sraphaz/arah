using Arah.Domain.Territories;



namespace Arah.Application.Interfaces.Geo;



public interface ITerritoryProposalService

{

    Task<TerritoryProposalResult?> ProposeAsync(

        Guid proposerUserId,

        string name,

        string city,

        string stateCode,

        double latitude,

        double longitude,

        double? radiusKm,

        IReadOnlyList<TerritoryBoundaryPoint>? boundaryPolygon,

        Guid? parentTerritoryId,

        CancellationToken cancellationToken = default);



    Task<IReadOnlyList<Territory>> ListPendingAsync(CancellationToken cancellationToken = default);



    Task<bool> ReviewAsync(

        Guid territoryId,

        bool approve,

        CancellationToken cancellationToken = default);

}



public sealed record TerritoryProposalResult(

    Guid TerritoryId,

    string Name,

    string City,

    string StateCode,

    string? Description,

    double Latitude,

    double Longitude,

    double DistanceKm,

    TerritoryStatus Status,

    bool Created,

    IReadOnlyList<TerritoryBoundaryPoint>? BoundaryPolygon)

{

    public bool IsPending => Status == TerritoryStatus.Pending;



    public static TerritoryProposalResult FromTerritory(

        Territory territory,

        double distanceKm,

        bool created) =>

        new(

            territory.Id,

            territory.Name,

            territory.City,

            territory.State,

            territory.Description,

            territory.Latitude,

            territory.Longitude,

            Math.Round(distanceKm, 2),

            territory.Status,

            created,

            territory.BoundaryPolygon);

}


