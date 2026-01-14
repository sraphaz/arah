using Araponga.Domain.Evidence;

namespace Araponga.Application.Interfaces;

public interface IDocumentEvidenceRepository
{
    Task<DocumentEvidence?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(DocumentEvidence evidence, CancellationToken cancellationToken);
}

