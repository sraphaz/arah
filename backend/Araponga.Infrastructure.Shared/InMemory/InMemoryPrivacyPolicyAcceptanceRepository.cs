using Araponga.Application.Interfaces;
using Araponga.Domain.Policies;

namespace Araponga.Infrastructure.Shared.InMemory;

/// <summary>Implementação InMemory de IPrivacyPolicyAcceptanceRepository usando InMemorySharedStore.</summary>
public sealed class InMemoryPrivacyPolicyAcceptanceRepository : IPrivacyPolicyAcceptanceRepository
{
    private readonly InMemorySharedStore _store;

    public InMemoryPrivacyPolicyAcceptanceRepository(InMemorySharedStore store) => _store = store;

    public Task<PrivacyPolicyAcceptance?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var acceptance = _store.PrivacyPolicyAcceptances.FirstOrDefault(a => a.Id == id);
        return Task.FromResult(acceptance);
    }

    public Task<PrivacyPolicyAcceptance?> GetByUserAndPolicyAsync(Guid userId, Guid policyId, CancellationToken cancellationToken)
    {
        var acceptance = _store.PrivacyPolicyAcceptances
            .FirstOrDefault(a => a.UserId == userId && a.PrivacyPolicyId == policyId);
        return Task.FromResult(acceptance);
    }

    public Task<IReadOnlyList<PrivacyPolicyAcceptance>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var acceptances = _store.PrivacyPolicyAcceptances
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.AcceptedAtUtc)
            .ToList();
        return Task.FromResult<IReadOnlyList<PrivacyPolicyAcceptance>>(acceptances);
    }

    public Task<IReadOnlyList<PrivacyPolicyAcceptance>> GetByPolicyIdAsync(Guid policyId, CancellationToken cancellationToken)
    {
        var acceptances = _store.PrivacyPolicyAcceptances
            .Where(a => a.PrivacyPolicyId == policyId)
            .OrderByDescending(a => a.AcceptedAtUtc)
            .ToList();
        return Task.FromResult<IReadOnlyList<PrivacyPolicyAcceptance>>(acceptances);
    }

    public Task<bool> HasAcceptedAsync(Guid userId, Guid policyId, CancellationToken cancellationToken)
    {
        var acceptance = _store.PrivacyPolicyAcceptances
            .FirstOrDefault(a => a.UserId == userId
                && a.PrivacyPolicyId == policyId
                && !a.IsRevoked);
        return Task.FromResult(acceptance is not null);
    }

    public Task AddAsync(PrivacyPolicyAcceptance acceptance, CancellationToken cancellationToken)
    {
        _store.PrivacyPolicyAcceptances.Add(acceptance);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(PrivacyPolicyAcceptance acceptance, CancellationToken cancellationToken)
    {
        var existing = _store.PrivacyPolicyAcceptances.FirstOrDefault(a => a.Id == acceptance.Id);
        if (existing is null)
            throw new InvalidOperationException($"Privacy Policy Acceptance with ID {acceptance.Id} not found.");
        _store.PrivacyPolicyAcceptances.Remove(existing);
        _store.PrivacyPolicyAcceptances.Add(acceptance);
        return Task.CompletedTask;
    }
}
