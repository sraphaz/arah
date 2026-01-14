using Araponga.Application.Interfaces;
using Araponga.Domain.Membership;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryMembershipCapabilityRepository : IMembershipCapabilityRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryMembershipCapabilityRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<MembershipCapability?> GetByIdAsync(Guid capabilityId, CancellationToken cancellationToken)
    {
        var capability = _dataStore.MembershipCapabilities.FirstOrDefault(c => c.Id == capabilityId);
        return Task.FromResult(capability);
    }

    public Task<IReadOnlyList<MembershipCapability>> GetByMembershipIdAsync(Guid membershipId, CancellationToken cancellationToken)
    {
        var capabilities = _dataStore.MembershipCapabilities
            .Where(c => c.MembershipId == membershipId)
            .ToList();
        return Task.FromResult<IReadOnlyList<MembershipCapability>>(capabilities);
    }

    public Task<IReadOnlyList<MembershipCapability>> GetActiveByMembershipIdAsync(Guid membershipId, CancellationToken cancellationToken)
    {
        var capabilities = _dataStore.MembershipCapabilities
            .Where(c => c.MembershipId == membershipId && c.IsActive())
            .ToList();
        return Task.FromResult<IReadOnlyList<MembershipCapability>>(capabilities);
    }

    public Task<bool> HasCapabilityAsync(Guid membershipId, MembershipCapabilityType capabilityType, CancellationToken cancellationToken)
    {
        var hasCapability = _dataStore.MembershipCapabilities
            .Any(c => c.MembershipId == membershipId &&
                      c.CapabilityType == capabilityType &&
                      c.IsActive());
        return Task.FromResult(hasCapability);
    }

    public Task AddAsync(MembershipCapability capability, CancellationToken cancellationToken)
    {
        _dataStore.MembershipCapabilities.Add(capability);
        return Task.CompletedTask;
    }

    public async Task UpdateAsync(MembershipCapability capability, CancellationToken cancellationToken)
    {
        var existing = await GetByIdAsync(capability.Id, cancellationToken);
        if (existing is null)
        {
            await AddAsync(capability, cancellationToken);
        }
        else
        {
            var index = _dataStore.MembershipCapabilities.IndexOf(existing);
            if (index >= 0)
            {
                _dataStore.MembershipCapabilities[index] = capability;
            }
        }
    }

    public Task<IReadOnlyList<Guid>> ListMembershipIdsWithCapabilityAsync(
        MembershipCapabilityType capabilityType,
        Guid territoryId,
        CancellationToken cancellationToken)
    {
        // Nota: Esta implementação requer acesso a Memberships para filtrar por territoryId
        // Por simplicidade, retorna todos os membershipIds com a capability
        // Em produção, isso seria otimizado com join
        var membershipIds = _dataStore.MembershipCapabilities
            .Where(c => c.CapabilityType == capabilityType && c.IsActive())
            .Select(c => c.MembershipId)
            .Distinct()
            .ToList();

        // Filtrar por territoryId se necessário
        var filteredIds = _dataStore.Memberships
            .Where(m => m.TerritoryId == territoryId && membershipIds.Contains(m.Id))
            .Select(m => m.Id)
            .ToList();

        return Task.FromResult<IReadOnlyList<Guid>>(filteredIds);
    }
}
