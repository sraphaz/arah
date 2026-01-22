using Araponga.Application.Interfaces;
using Araponga.Domain.Policies;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryPrivacyPolicyAcceptanceRepository : IPrivacyPolicyAcceptanceRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryPrivacyPolicyAcceptanceRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<PrivacyPolicyAcceptance?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var acceptance = _dataStore.PrivacyPolicyAcceptances.FirstOrDefault(a => a.Id == id);
        return Task.FromResult(acceptance);
    }

    public Task<PrivacyPolicyAcceptance?> GetByUserAndPolicyAsync(Guid userId, Guid policyId, CancellationToken cancellationToken)
    {
        var acceptance = _dataStore.PrivacyPolicyAcceptances
            .FirstOrDefault(a => a.UserId == userId && a.PrivacyPolicyId == policyId);
        return Task.FromResult(acceptance);
    }

    public Task<IReadOnlyList<PrivacyPolicyAcceptance>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var acceptances = _dataStore.PrivacyPolicyAcceptances
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.AcceptedAtUtc)
            .ToList();
        return Task.FromResult<IReadOnlyList<PrivacyPolicyAcceptance>>(acceptances);
    }

    public Task<IReadOnlyList<PrivacyPolicyAcceptance>> GetByPolicyIdAsync(Guid policyId, CancellationToken cancellationToken)
    {
        var acceptances = _dataStore.PrivacyPolicyAcceptances
            .Where(a => a.PrivacyPolicyId == policyId)
            .OrderByDescending(a => a.AcceptedAtUtc)
            .ToList();
        return Task.FromResult<IReadOnlyList<PrivacyPolicyAcceptance>>(acceptances);
    }

    public Task<bool> HasAcceptedAsync(Guid userId, Guid policyId, CancellationToken cancellationToken)
    {
        var acceptance = _dataStore.PrivacyPolicyAcceptances
            .FirstOrDefault(a => a.UserId == userId 
                && a.PrivacyPolicyId == policyId 
                && !a.IsRevoked);
        return Task.FromResult(acceptance is not null);
    }

    public Task AddAsync(PrivacyPolicyAcceptance acceptance, CancellationToken cancellationToken)
    {
        _dataStore.PrivacyPolicyAcceptances.Add(acceptance);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(PrivacyPolicyAcceptance acceptance, CancellationToken cancellationToken)
    {
        var existing = _dataStore.PrivacyPolicyAcceptances.FirstOrDefault(a => a.Id == acceptance.Id);
        if (existing is null)
        {
            throw new InvalidOperationException($"Privacy Policy Acceptance with ID {acceptance.Id} not found.");
        }

        _dataStore.PrivacyPolicyAcceptances.Remove(existing);
        _dataStore.PrivacyPolicyAcceptances.Add(acceptance);
        return Task.CompletedTask;
    }
}
