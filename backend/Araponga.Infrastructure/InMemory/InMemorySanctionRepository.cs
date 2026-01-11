using Araponga.Application.Interfaces;
using Araponga.Domain.Moderation;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemorySanctionRepository : ISanctionRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemorySanctionRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task AddAsync(Sanction sanction, CancellationToken cancellationToken)
    {
        _dataStore.Sanctions.Add(sanction);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Sanction>> ListActiveForTargetAsync(
        Guid targetId,
        DateTime referenceUtc,
        CancellationToken cancellationToken)
    {
        var sanctions = _dataStore.Sanctions
            .Where(sanction => sanction.TargetId == targetId && IsActive(sanction, referenceUtc))
            .ToList();

        return Task.FromResult<IReadOnlyList<Sanction>>(sanctions);
    }

    public Task<bool> HasActiveSanctionAsync(
        Guid targetId,
        Guid territoryId,
        SanctionType type,
        DateTime referenceUtc,
        CancellationToken cancellationToken)
    {
        var has = _dataStore.Sanctions.Any(sanction =>
            sanction.TargetId == targetId &&
            sanction.Type == type &&
            IsActive(sanction, referenceUtc) &&
            (sanction.Scope == SanctionScope.Global ||
             (sanction.Scope == SanctionScope.Territory && sanction.TerritoryId == territoryId)));

        return Task.FromResult(has);
    }

    private static bool IsActive(Sanction sanction, DateTime referenceUtc)
    {
        if (sanction.Status != SanctionStatus.Active)
        {
            return false;
        }

        if (sanction.StartAtUtc > referenceUtc)
        {
            return false;
        }

        if (sanction.EndAtUtc is not null && sanction.EndAtUtc.Value < referenceUtc)
        {
            return false;
        }

        return true;
    }
}
