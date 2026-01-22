using Araponga.Application.Interfaces;
using Araponga.Domain.Policies;
using Araponga.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresPrivacyPolicyAcceptanceRepository : IPrivacyPolicyAcceptanceRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresPrivacyPolicyAcceptanceRepository(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PrivacyPolicyAcceptance?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var record = await _dbContext.PrivacyPolicyAcceptances
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        return record?.ToDomain();
    }

    public async Task<PrivacyPolicyAcceptance?> GetByUserAndPolicyAsync(Guid userId, Guid policyId, CancellationToken cancellationToken)
    {
        var record = await _dbContext.PrivacyPolicyAcceptances
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.UserId == userId && a.PrivacyPolicyId == policyId, cancellationToken);
        return record?.ToDomain();
    }

    public async Task<IReadOnlyList<PrivacyPolicyAcceptance>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var records = await _dbContext.PrivacyPolicyAcceptances
            .AsNoTracking()
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.AcceptedAtUtc)
            .ToListAsync(cancellationToken);
        return records.Select(r => r.ToDomain()).ToList();
    }

    public async Task<IReadOnlyList<PrivacyPolicyAcceptance>> GetByPolicyIdAsync(Guid policyId, CancellationToken cancellationToken)
    {
        var records = await _dbContext.PrivacyPolicyAcceptances
            .AsNoTracking()
            .Where(a => a.PrivacyPolicyId == policyId)
            .OrderByDescending(a => a.AcceptedAtUtc)
            .ToListAsync(cancellationToken);
        return records.Select(r => r.ToDomain()).ToList();
    }

    public async Task<bool> HasAcceptedAsync(Guid userId, Guid policyId, CancellationToken cancellationToken)
    {
        var acceptance = await _dbContext.PrivacyPolicyAcceptances
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.UserId == userId 
                && a.PrivacyPolicyId == policyId 
                && !a.IsRevoked, cancellationToken);
        return acceptance is not null;
    }

    public Task AddAsync(PrivacyPolicyAcceptance acceptance, CancellationToken cancellationToken)
    {
        _dbContext.PrivacyPolicyAcceptances.Add(acceptance.ToRecord());
        return Task.CompletedTask;
    }

    public async Task UpdateAsync(PrivacyPolicyAcceptance acceptance, CancellationToken cancellationToken)
    {
        var record = await _dbContext.PrivacyPolicyAcceptances
            .FirstOrDefaultAsync(a => a.Id == acceptance.Id, cancellationToken);

        if (record is null)
        {
            throw new InvalidOperationException($"Privacy Policy Acceptance with ID {acceptance.Id} not found.");
        }

        record.IsRevoked = acceptance.IsRevoked;
        record.RevokedAtUtc = acceptance.RevokedAtUtc;

        _dbContext.PrivacyPolicyAcceptances.Update(record);
    }
}
