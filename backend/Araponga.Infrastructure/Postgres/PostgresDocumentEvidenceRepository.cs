using Araponga.Application.Interfaces;
using Araponga.Domain.Evidence;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresDocumentEvidenceRepository : IDocumentEvidenceRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresDocumentEvidenceRepository(ArapongaDbContext dbContext)
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

