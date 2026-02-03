using Araponga.Application.Interfaces;
using Araponga.Domain.Policies;

namespace Araponga.Infrastructure.Shared.InMemory;

/// <summary>Implementação InMemory de ITermsOfServiceRepository usando InMemorySharedStore.</summary>
public sealed class InMemoryTermsOfServiceRepository : ITermsOfServiceRepository
{
    private readonly InMemorySharedStore _store;

    public InMemoryTermsOfServiceRepository(InMemorySharedStore store) => _store = store;

    public Task<TermsOfService?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var terms = _store.TermsOfServices.FirstOrDefault(t => t.Id == id);
        return Task.FromResult(terms);
    }

    public Task<TermsOfService?> GetByVersionAsync(string version, CancellationToken cancellationToken)
    {
        var terms = _store.TermsOfServices.FirstOrDefault(t => t.Version == version);
        return Task.FromResult(terms);
    }

    public Task<IReadOnlyList<TermsOfService>> GetActiveAsync(CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var terms = _store.TermsOfServices
            .Where(t => t.IsActive
                && t.EffectiveDate <= now
                && (t.ExpirationDate == null || t.ExpirationDate > now))
            .OrderByDescending(t => t.EffectiveDate)
            .ToList();
        return Task.FromResult<IReadOnlyList<TermsOfService>>(terms);
    }

    public Task<IReadOnlyList<TermsOfService>> ListAsync(CancellationToken cancellationToken)
    {
        var terms = _store.TermsOfServices
            .OrderByDescending(t => t.CreatedAtUtc)
            .ToList();
        return Task.FromResult<IReadOnlyList<TermsOfService>>(terms);
    }

    public Task AddAsync(TermsOfService terms, CancellationToken cancellationToken)
    {
        _store.TermsOfServices.Add(terms);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(TermsOfService terms, CancellationToken cancellationToken)
    {
        var existing = _store.TermsOfServices.FirstOrDefault(t => t.Id == terms.Id);
        if (existing is null)
            throw new InvalidOperationException($"Terms of Service with ID {terms.Id} not found.");
        _store.TermsOfServices.Remove(existing);
        _store.TermsOfServices.Add(terms);
        return Task.CompletedTask;
    }
}
