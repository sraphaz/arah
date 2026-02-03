using Araponga.Application.Interfaces;
using Araponga.Domain.Membership;

namespace Araponga.Infrastructure.Shared.InMemory;

/// <summary>Implementação InMemory de IMembershipSettingsRepository usando InMemorySharedStore.</summary>
public sealed class InMemoryMembershipSettingsRepository : IMembershipSettingsRepository
{
    private readonly InMemorySharedStore _store;

    public InMemoryMembershipSettingsRepository(InMemorySharedStore store) => _store = store;

    public Task<MembershipSettings?> GetByMembershipIdAsync(Guid membershipId, CancellationToken cancellationToken)
        => Task.FromResult(_store.MembershipSettings.FirstOrDefault(s => s.MembershipId == membershipId));

    public Task AddAsync(MembershipSettings settings, CancellationToken cancellationToken)
    {
        _store.MembershipSettings.Add(settings);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(MembershipSettings settings, CancellationToken cancellationToken)
    {
        var existing = _store.MembershipSettings.FirstOrDefault(s => s.MembershipId == settings.MembershipId);
        if (existing is null) _store.MembershipSettings.Add(settings);
        else
        {
            var i = _store.MembershipSettings.IndexOf(existing);
            if (i >= 0) _store.MembershipSettings[i] = settings;
        }
        return Task.CompletedTask;
    }
}
