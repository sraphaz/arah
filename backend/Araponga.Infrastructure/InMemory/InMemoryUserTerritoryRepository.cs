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

    public Task<UserTerritory?> GetByUserAndTerritoryAsync(
        Guid userId,
        Guid territoryId,
        CancellationToken cancellationToken)
    {
        var membership = _dataStore.Memberships.FirstOrDefault(m =>
            m.UserId == userId && m.TerritoryId == territoryId);

        return Task.FromResult(membership);
    }

    public Task AddAsync(UserTerritory membership, CancellationToken cancellationToken)
    {
        _dataStore.Memberships.Add(membership);
        return Task.CompletedTask;
    }

    public Task<bool> IsValidatedAsync(Guid userId, Guid territoryId, CancellationToken cancellationToken)
    {
        var isResident = _dataStore.Memberships.Any(m =>
            m.UserId == userId &&
            m.TerritoryId == territoryId &&
            m.Status == MembershipStatus.Validated);

        return Task.FromResult(isResident);
    }
}
