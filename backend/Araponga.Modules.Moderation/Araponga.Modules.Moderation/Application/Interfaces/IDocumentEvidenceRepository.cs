using Araponga.Modules.Moderation.Domain.Evidence;

namespace Araponga.Modules.Moderation.Application.Interfaces;

public interface IDocumentEvidenceRepository
{
    Task<DocumentEvidence?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(DocumentEvidence evidence, CancellationToken cancellationToken);
}
