using Araponga.Application.Interfaces;
using Araponga.Domain.Membership;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryMembershipSettingsRepository : IMembershipSettingsRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryMembershipSettingsRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<MembershipSettings?> GetByMembershipIdAsync(Guid membershipId, CancellationToken cancellationToken)
    {
        var settings = _dataStore.MembershipSettings.FirstOrDefault(s => s.MembershipId == membershipId);
        return Task.FromResult(settings);
    }

    public Task AddAsync(MembershipSettings settings, CancellationToken cancellationToken)
    {
        _dataStore.MembershipSettings.Add(settings);
        return Task.CompletedTask;
    }

    public async Task UpdateAsync(MembershipSettings settings, CancellationToken cancellationToken)
    {
        var existing = await GetByMembershipIdAsync(settings.MembershipId, cancellationToken);
        if (existing is null)
        {
            await AddAsync(settings, cancellationToken);
        }
        else
        {
            var index = _dataStore.MembershipSettings.IndexOf(existing);
            if (index >= 0)
            {
                _dataStore.MembershipSettings[index] = settings;
            }
        }
    }
}
