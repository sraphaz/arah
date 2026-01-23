using Araponga.Application.Interfaces;
using Araponga.Domain.Policies;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryTermsOfServiceRepository : ITermsOfServiceRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryTermsOfServiceRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<TermsOfService?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var terms = _dataStore.TermsOfServices.FirstOrDefault(t => t.Id == id);
        return Task.FromResult(terms);
    }

    public Task<TermsOfService?> GetByVersionAsync(string version, CancellationToken cancellationToken)
    {
        var terms = _dataStore.TermsOfServices.FirstOrDefault(t => t.Version == version);
        return Task.FromResult(terms);
    }

    public Task<IReadOnlyList<TermsOfService>> GetActiveAsync(CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var terms = _dataStore.TermsOfServices
            .Where(t => t.IsActive 
                && t.EffectiveDate <= now 
                && (t.ExpirationDate == null || t.ExpirationDate > now))
            .OrderByDescending(t => t.EffectiveDate)
            .ToList();
        return Task.FromResult<IReadOnlyList<TermsOfService>>(terms);
    }

    public Task<IReadOnlyList<TermsOfService>> ListAsync(CancellationToken cancellationToken)
    {
        var terms = _dataStore.TermsOfServices
            .OrderByDescending(t => t.CreatedAtUtc)
            .ToList();
        return Task.FromResult<IReadOnlyList<TermsOfService>>(terms);
    }

    public Task AddAsync(TermsOfService terms, CancellationToken cancellationToken)
    {
        _dataStore.TermsOfServices.Add(terms);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(TermsOfService terms, CancellationToken cancellationToken)
    {
        var existing = _dataStore.TermsOfServices.FirstOrDefault(t => t.Id == terms.Id);
        if (existing is null)
        {
            throw new InvalidOperationException($"Terms of Service with ID {terms.Id} not found.");
        }

        _dataStore.TermsOfServices.Remove(existing);
        _dataStore.TermsOfServices.Add(terms);
        return Task.CompletedTask;
    }
}
