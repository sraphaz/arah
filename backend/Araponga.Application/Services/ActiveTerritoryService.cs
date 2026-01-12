using Araponga.Application.Interfaces;

namespace Araponga.Application.Services;

public sealed class ActiveTerritoryService
{
    private readonly IActiveTerritoryStore _store;
    private readonly ITerritoryRepository _territoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActiveTerritoryService(
        IActiveTerritoryStore store,
        ITerritoryRepository territoryRepository,
        IUnitOfWork unitOfWork)
    {
        _store = store;
        _territoryRepository = territoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> SetActiveAsync(string sessionId, Guid territoryId, CancellationToken cancellationToken)
    {
        var territory = await _territoryRepository.GetByIdAsync(territoryId, cancellationToken);
        if (territory is null)
        {
            return false;
        }

        await _store.SetAsync(sessionId, territoryId, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return true;
    }

    public Task<Guid?> GetActiveAsync(string sessionId, CancellationToken cancellationToken)
    {
        return _store.GetAsync(sessionId, cancellationToken);
    }
}
