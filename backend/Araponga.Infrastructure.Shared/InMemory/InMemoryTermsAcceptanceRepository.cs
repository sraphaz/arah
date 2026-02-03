using Araponga.Application.Interfaces;
using Araponga.Domain.Policies;

namespace Araponga.Infrastructure.Shared.InMemory;

/// <summary>Implementação InMemory de ITermsAcceptanceRepository usando InMemorySharedStore.</summary>
public sealed class InMemoryTermsAcceptanceRepository : ITermsAcceptanceRepository
{
    private readonly InMemorySharedStore _store;

    public InMemoryTermsAcceptanceRepository(InMemorySharedStore store) => _store = store;

    public Task<TermsAcceptance?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var acceptance = _store.TermsAcceptances.FirstOrDefault(a => a.Id == id);
        return Task.FromResult(acceptance);
    }

    public Task<TermsAcceptance?> GetByUserAndTermsAsync(Guid userId, Guid termsId, CancellationToken cancellationToken)
    {
        var acceptance = _store.TermsAcceptances
            .FirstOrDefault(a => a.UserId == userId && a.TermsOfServiceId == termsId);
        return Task.FromResult(acceptance);
    }

    public Task<IReadOnlyList<TermsAcceptance>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var acceptances = _store.TermsAcceptances
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.AcceptedAtUtc)
            .ToList();
        return Task.FromResult<IReadOnlyList<TermsAcceptance>>(acceptances);
    }

    public Task<IReadOnlyList<TermsAcceptance>> GetByTermsIdAsync(Guid termsId, CancellationToken cancellationToken)
    {
        var acceptances = _store.TermsAcceptances
            .Where(a => a.TermsOfServiceId == termsId)
            .OrderByDescending(a => a.AcceptedAtUtc)
            .ToList();
        return Task.FromResult<IReadOnlyList<TermsAcceptance>>(acceptances);
    }

    public Task<bool> HasAcceptedAsync(Guid userId, Guid termsId, CancellationToken cancellationToken)
    {
        var acceptance = _store.TermsAcceptances
            .FirstOrDefault(a => a.UserId == userId
                && a.TermsOfServiceId == termsId
                && !a.IsRevoked);
        return Task.FromResult(acceptance is not null);
    }

    public Task AddAsync(TermsAcceptance acceptance, CancellationToken cancellationToken)
    {
        _store.TermsAcceptances.Add(acceptance);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(TermsAcceptance acceptance, CancellationToken cancellationToken)
    {
        var existing = _store.TermsAcceptances.FirstOrDefault(a => a.Id == acceptance.Id);
        if (existing is null)
            throw new InvalidOperationException($"Terms Acceptance with ID {acceptance.Id} not found.");
        _store.TermsAcceptances.Remove(existing);
        _store.TermsAcceptances.Add(acceptance);
        return Task.CompletedTask;
    }
}
