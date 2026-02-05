using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Metrics;
using Araponga.Application.Models;
using Araponga.Domain.Territories;

namespace Araponga.Application.Services;

public sealed class TerritoryService
{
    private readonly ITerritoryRepository _territoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TerritoryCacheService? _cacheService;
    private readonly CacheInvalidationService? _cacheInvalidation;

    public TerritoryService(
        ITerritoryRepository territoryRepository,
        IUnitOfWork unitOfWork,
        TerritoryCacheService? cacheService = null,
        CacheInvalidationService? cacheInvalidation = null)
    {
        _territoryRepository = territoryRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
        _cacheInvalidation = cacheInvalidation;
    }

    public async Task<IReadOnlyList<Territory>> ListAvailableAsync(CancellationToken cancellationToken)
    {
        if (_cacheService is not null)
        {
            return await _cacheService.GetActiveTerritoriesAsync(cancellationToken);
        }

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
        CancellationToken cancellationToken,
        double? radiusKm = null)
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

        if (radiusKm.HasValue && radiusKm.Value <= 0)
        {
            return new TerritoryCreationResult(false, "RadiusKm must be positive when provided.", null);
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
            DateTime.UtcNow,
            radiusKm);

        await _territoryRepository.AddAsync(territory, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        // Invalidate cache when territory is created
        _cacheService?.InvalidateActiveTerritories();
        _cacheInvalidation?.InvalidateTerritoryCache(territory.Id);

        // Record business metric
        ArapongaMetrics.TerritoriesCreated.Add(1);

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

    public async Task<PagedResult<Territory>> ListAvailablePagedAsync(
        PaginationParameters pagination,
        CancellationToken cancellationToken)
    {
        var territories = await _territoryRepository.ListAsync(cancellationToken);
        var activeTerritories = territories
            .Where(t => t.Status == TerritoryStatus.Active)
            .OrderBy(t => t.Name)
            .ToList();
        return activeTerritories.ToPagedResult(pagination);
    }

    public async Task<PagedResult<Territory>> SearchPagedAsync(
        string? query,
        string? city,
        string? state,
        PaginationParameters pagination,
        CancellationToken cancellationToken)
    {
        var territories = await _territoryRepository.SearchAsync(query, city, state, cancellationToken);
        var activeTerritories = territories
            .Where(t => t.Status == TerritoryStatus.Active)
            .OrderBy(t => t.Name)
            .ToList();

        const int maxInt32 = int.MaxValue;
        var count = activeTerritories.Count;
        var totalCount = count > maxInt32 ? maxInt32 : count;
        var pagedItems = activeTerritories
            .Skip(pagination.Skip)
            .Take(pagination.Take)
            .ToList();

        return new PagedResult<Territory>(pagedItems, pagination.PageNumber, pagination.PageSize, totalCount);
    }

    public async Task<PagedResult<Territory>> NearbyPagedAsync(
        double latitude,
        double longitude,
        double radiusKm,
        PaginationParameters pagination,
        CancellationToken cancellationToken)
    {
        var limit = pagination.PageSize * pagination.PageNumber;
        var territories = await _territoryRepository.NearbyAsync(latitude, longitude, radiusKm, limit, cancellationToken);
        var activeTerritories = territories
            .Where(t => t.Status == TerritoryStatus.Active)
            .ToList();
        return activeTerritories.ToPagedResult(pagination);
    }

    private async Task<IReadOnlyList<Territory>> FilterActiveAsync(Task<IReadOnlyList<Territory>> task)
    {
        var territories = await task;
        return territories.Where(t => t.Status == TerritoryStatus.Active).ToList();
    }
}
