using Araponga.Application.Interfaces;
using Araponga.Domain.Users;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryUserTerritoryRepository : IUserTerritoryRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryUserTerritoryRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<UserTerritory?> GetByUserAndTerritoryAsync(Guid userId, Guid territoryId, CancellationToken cancellationToken)
    {
        var membership = _dataStore.UserTerritories
            .FirstOrDefault(entry => entry.UserId == userId && entry.TerritoryId == territoryId);
        return Task.FromResult(membership);
    }

    public Task AddAsync(UserTerritory membership, CancellationToken cancellationToken)
    {
        _dataStore.UserTerritories.Add(membership);
        return Task.CompletedTask;
    }

    public Task<bool> IsValidatedAsync(Guid userId, Guid territoryId, CancellationToken cancellationToken)
    {
        var isValidated = _dataStore.UserTerritories
            .Any(entry => entry.UserId == userId &&
                          entry.TerritoryId == territoryId &&
                          entry.Status == MembershipStatus.Validated);
        return Task.FromResult(isValidated);
    }
}
