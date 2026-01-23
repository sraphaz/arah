using Araponga.Application.Interfaces;
using Araponga.Domain.Policies;
using Araponga.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresTermsAcceptanceRepository : ITermsAcceptanceRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresTermsAcceptanceRepository(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TermsAcceptance?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var record = await _dbContext.TermsAcceptances
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        return record?.ToDomain();
    }

    public async Task<TermsAcceptance?> GetByUserAndTermsAsync(Guid userId, Guid termsId, CancellationToken cancellationToken)
    {
        var record = await _dbContext.TermsAcceptances
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.UserId == userId && a.TermsOfServiceId == termsId, cancellationToken);
        return record?.ToDomain();
    }

    public async Task<IReadOnlyList<TermsAcceptance>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var records = await _dbContext.TermsAcceptances
            .AsNoTracking()
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.AcceptedAtUtc)
            .ToListAsync(cancellationToken);
        return records.Select(r => r.ToDomain()).ToList();
    }

    public async Task<IReadOnlyList<TermsAcceptance>> GetByTermsIdAsync(Guid termsId, CancellationToken cancellationToken)
    {
        var records = await _dbContext.TermsAcceptances
            .AsNoTracking()
            .Where(a => a.TermsOfServiceId == termsId)
            .OrderByDescending(a => a.AcceptedAtUtc)
            .ToListAsync(cancellationToken);
        return records.Select(r => r.ToDomain()).ToList();
    }

    public async Task<bool> HasAcceptedAsync(Guid userId, Guid termsId, CancellationToken cancellationToken)
    {
        var acceptance = await _dbContext.TermsAcceptances
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.UserId == userId 
                && a.TermsOfServiceId == termsId 
                && !a.IsRevoked, cancellationToken);
        return acceptance is not null;
    }

    public Task AddAsync(TermsAcceptance acceptance, CancellationToken cancellationToken)
    {
        _dbContext.TermsAcceptances.Add(acceptance.ToRecord());
        return Task.CompletedTask;
    }

    public async Task UpdateAsync(TermsAcceptance acceptance, CancellationToken cancellationToken)
    {
        var record = await _dbContext.TermsAcceptances
            .FirstOrDefaultAsync(a => a.Id == acceptance.Id, cancellationToken);

        if (record is null)
        {
            throw new InvalidOperationException($"Terms Acceptance with ID {acceptance.Id} not found.");
        }

        record.IsRevoked = acceptance.IsRevoked;
        record.RevokedAtUtc = acceptance.RevokedAtUtc;

        _dbContext.TermsAcceptances.Update(record);
    }
}
