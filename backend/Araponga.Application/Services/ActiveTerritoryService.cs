using Araponga.Application.Interfaces;

namespace Araponga.Application.Services;

public sealed class ActiveTerritoryService
{
    private readonly IActiveTerritoryStore _store;
    private readonly ITerritoryRepository _territoryRepository;

    public ActiveTerritoryService(IActiveTerritoryStore store, ITerritoryRepository territoryRepository)
    {
        _store = store;
        _territoryRepository = territoryRepository;
    }

    public async Task<bool> SetActiveAsync(string sessionId, Guid territoryId, CancellationToken cancellationToken)
    {
        var territory = await _territoryRepository.GetByIdAsync(territoryId, cancellationToken);
        if (territory is null)
        {
            return false;
        }

        await _store.SetAsync(sessionId, territoryId, cancellationToken);
        return true;
    }

    public Task<Guid?> GetActiveAsync(string sessionId, CancellationToken cancellationToken)
    {
        return _store.GetAsync(sessionId, cancellationToken);
    }
}
