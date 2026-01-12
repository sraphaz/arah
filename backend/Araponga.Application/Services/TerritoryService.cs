using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Domain.Territories;

namespace Araponga.Application.Services;

public sealed class TerritoryService
{
    private readonly ITerritoryRepository _territoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TerritoryService(ITerritoryRepository territoryRepository, IUnitOfWork unitOfWork)
    {
        _territoryRepository = territoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<Territory>> ListAvailableAsync(CancellationToken cancellationToken)
    {
        var territories = await _territoryRepository.ListAsync(cancellationToken);
        return territories
            .Where(t => t.Status == TerritoryStatus.Active)
            .OrderBy(t => t.Name)
            .ToList();
    }

    public Task<Territory?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _territoryRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<TerritoryCreationResult> CreateAsync(
        string name,
        string? description,
        string city,
        string state,
        double latitude,
        double longitude,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return new TerritoryCreationResult(false, "Name is required.", null);
        }

        if (string.IsNullOrWhiteSpace(city))
        {
            return new TerritoryCreationResult(false, "City is required.", null);
        }

        if (string.IsNullOrWhiteSpace(state))
        {
            return new TerritoryCreationResult(false, "State is required.", null);
        }

        var territory = new Territory(
            Guid.NewGuid(),
            null,
            name,
            description,
            TerritoryStatus.Pending,
            city,
            state,
            latitude,
            longitude,
            DateTime.UtcNow);

        await _territoryRepository.AddAsync(territory, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return new TerritoryCreationResult(true, null, territory);
    }

    public Task<IReadOnlyList<Territory>> SearchAsync(
        string? query,
        string? city,
        string? state,
        CancellationToken cancellationToken)
    {
        return FilterActiveAsync(_territoryRepository.SearchAsync(query, city, state, cancellationToken));
    }

    public Task<IReadOnlyList<Territory>> NearbyAsync(
        double latitude,
        double longitude,
        double radiusKm,
        int limit,
        CancellationToken cancellationToken)
    {
        return FilterActiveAsync(_territoryRepository.NearbyAsync(latitude, longitude, radiusKm, limit, cancellationToken));
    }

    private async Task<IReadOnlyList<Territory>> FilterActiveAsync(Task<IReadOnlyList<Territory>> task)
    {
        var territories = await task;
        return territories.Where(t => t.Status == TerritoryStatus.Active).ToList();
    }
}
