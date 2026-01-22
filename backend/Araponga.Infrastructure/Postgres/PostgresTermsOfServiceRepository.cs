using Araponga.Application.Interfaces;
using Araponga.Domain.Policies;
using Araponga.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresTermsOfServiceRepository : ITermsOfServiceRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresTermsOfServiceRepository(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TermsOfService?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var record = await _dbContext.TermsOfServices
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        return record?.ToDomain();
    }

    public async Task<TermsOfService?> GetByVersionAsync(string version, CancellationToken cancellationToken)
    {
        var record = await _dbContext.TermsOfServices
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Version == version, cancellationToken);
        return record?.ToDomain();
    }

    public async Task<IReadOnlyList<TermsOfService>> GetActiveAsync(CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var records = await _dbContext.TermsOfServices
            .AsNoTracking()
            .Where(t => t.IsActive 
                && t.EffectiveDate <= now 
                && (t.ExpirationDate == null || t.ExpirationDate > now))
            .OrderByDescending(t => t.EffectiveDate)
            .ToListAsync(cancellationToken);
        return records.Select(r => r.ToDomain()).ToList();
    }

    public async Task<IReadOnlyList<TermsOfService>> ListAsync(CancellationToken cancellationToken)
    {
        var records = await _dbContext.TermsOfServices
            .AsNoTracking()
            .OrderByDescending(t => t.CreatedAtUtc)
            .ToListAsync(cancellationToken);
        return records.Select(r => r.ToDomain()).ToList();
    }

    public Task AddAsync(TermsOfService terms, CancellationToken cancellationToken)
    {
        _dbContext.TermsOfServices.Add(terms.ToRecord());
        return Task.CompletedTask;
    }

    public async Task UpdateAsync(TermsOfService terms, CancellationToken cancellationToken)
    {
        var record = await _dbContext.TermsOfServices
            .FirstOrDefaultAsync(t => t.Id == terms.Id, cancellationToken);

        if (record is null)
        {
            throw new InvalidOperationException($"Terms of Service with ID {terms.Id} not found.");
        }

        record.Version = terms.Version;
        record.Title = terms.Title;
        record.Content = terms.Content;
        record.EffectiveDate = terms.EffectiveDate;
        record.ExpirationDate = terms.ExpirationDate;
        record.IsActive = terms.IsActive;
        record.RequiredRoles = terms.RequiredRoles;
        record.RequiredCapabilities = terms.RequiredCapabilities;
        record.RequiredSystemPermissions = terms.RequiredSystemPermissions;
        record.UpdatedAtUtc = terms.UpdatedAtUtc;

        _dbContext.TermsOfServices.Update(record);
    }
}
