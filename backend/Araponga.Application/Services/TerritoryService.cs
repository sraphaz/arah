using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Domain.Territories;

namespace Araponga.Application.Services;

public sealed class TerritoryService
{
    private static readonly TerritoryStatus[] AllowedStatuses =
        { TerritoryStatus.Active, TerritoryStatus.Pilot };

    private readonly ITerritoryRepository _territoryRepository;

    public TerritoryService(ITerritoryRepository territoryRepository)
    {
        _territoryRepository = territoryRepository;
    }

    public async Task<IReadOnlyList<Territory>> ListAvailableAsync(CancellationToken cancellationToken)
    {
        var territories = await _territoryRepository.ListAsync(cancellationToken);
        return territories
            .Where(t => AllowedStatuses.Contains(t.Status))
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
        SensitivityLevel sensitivity,
        bool isPilot,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return new TerritoryCreationResult(false, "Name is required.", null);
        }

        var territory = new Territory(
            Guid.NewGuid(),
            name,
            description,
            sensitivity,
            isPilot ? TerritoryStatus.Pilot : TerritoryStatus.Active,
            DateTime.UtcNow);

        await _territoryRepository.AddAsync(territory, cancellationToken);

        return new TerritoryCreationResult(true, null, territory);
    }
}
