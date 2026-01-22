using Araponga.Application.Interfaces;
using Araponga.Domain.Policies;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryPrivacyPolicyRepository : IPrivacyPolicyRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryPrivacyPolicyRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<PrivacyPolicy?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var policy = _dataStore.PrivacyPolicies.FirstOrDefault(p => p.Id == id);
        return Task.FromResult(policy);
    }

    public Task<PrivacyPolicy?> GetByVersionAsync(string version, CancellationToken cancellationToken)
    {
        var policy = _dataStore.PrivacyPolicies.FirstOrDefault(p => p.Version == version);
        return Task.FromResult(policy);
    }

    public Task<IReadOnlyList<PrivacyPolicy>> GetActiveAsync(CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var policies = _dataStore.PrivacyPolicies
            .Where(p => p.IsActive 
                && p.EffectiveDate <= now 
                && (p.ExpirationDate == null || p.ExpirationDate > now))
            .OrderByDescending(p => p.EffectiveDate)
            .ToList();
        return Task.FromResult<IReadOnlyList<PrivacyPolicy>>(policies);
    }

    public Task<IReadOnlyList<PrivacyPolicy>> ListAsync(CancellationToken cancellationToken)
    {
        var policies = _dataStore.PrivacyPolicies
            .OrderByDescending(p => p.CreatedAtUtc)
            .ToList();
        return Task.FromResult<IReadOnlyList<PrivacyPolicy>>(policies);
    }

    public Task AddAsync(PrivacyPolicy policy, CancellationToken cancellationToken)
    {
        _dataStore.PrivacyPolicies.Add(policy);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(PrivacyPolicy policy, CancellationToken cancellationToken)
    {
        var existing = _dataStore.PrivacyPolicies.FirstOrDefault(p => p.Id == policy.Id);
        if (existing is null)
        {
            throw new InvalidOperationException($"Privacy Policy with ID {policy.Id} not found.");
        }

        _dataStore.PrivacyPolicies.Remove(existing);
        _dataStore.PrivacyPolicies.Add(policy);
        return Task.CompletedTask;
    }
}
