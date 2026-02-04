namespace Araponga.Application.Interfaces;

public interface IActiveTerritoryStore
{
    Task<Guid?> GetAsync(string sessionId, CancellationToken cancellationToken);
    Task SetAsync(string sessionId, Guid territoryId, CancellationToken cancellationToken);
}
