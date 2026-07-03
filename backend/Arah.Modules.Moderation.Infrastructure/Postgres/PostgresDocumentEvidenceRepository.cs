using Arah.Modules.Moderation.Application.Interfaces;
using Arah.Modules.Moderation.Domain.Evidence;
using Arah.Modules.Moderation.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Arah.Modules.Moderation.Infrastructure.Postgres;

public sealed class PostgresDocumentEvidenceRepository : IDocumentEvidenceRepository
{
    private readonly ModerationDbContext _dbContext;

    public PostgresDocumentEvidenceRepository(ModerationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DocumentEvidence?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var record = await _dbContext.DocumentEvidences
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
        return record?.ToDomain();
    }

    public Task AddAsync(DocumentEvidence evidence, CancellationToken cancellationToken)
    {
        _dbContext.DocumentEvidences.Add(evidence.ToRecord());
        return Task.CompletedTask;
    }
}
