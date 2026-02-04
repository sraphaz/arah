using Araponga.Modules.Moderation.Application.Interfaces;
using Araponga.Modules.Moderation.Domain.Evidence;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryDocumentEvidenceRepository : IDocumentEvidenceRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryDocumentEvidenceRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<DocumentEvidence?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var found = _dataStore.DocumentEvidences.FirstOrDefault(e => e.Id == id);
        return Task.FromResult<DocumentEvidence?>(found);
    }

    public Task AddAsync(DocumentEvidence evidence, CancellationToken cancellationToken)
    {
        _dataStore.DocumentEvidences.Add(evidence);
        return Task.CompletedTask;
    }
}

