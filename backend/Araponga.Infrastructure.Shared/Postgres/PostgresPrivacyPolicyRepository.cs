using Araponga.Application.Interfaces;
using Araponga.Domain.Policies;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Shared.Postgres;

/// <summary>Implementação Postgres de IPrivacyPolicyRepository usando SharedDbContext.</summary>
public sealed class PostgresPrivacyPolicyRepository : IPrivacyPolicyRepository
{
    private readonly SharedDbContext _dbContext;

    public PostgresPrivacyPolicyRepository(SharedDbContext dbContext) => _dbContext = dbContext;

    public async Task<PrivacyPolicy?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var record = await _dbContext.PrivacyPolicies
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        return record?.ToDomain();
    }

    public async Task<PrivacyPolicy?> GetByVersionAsync(string version, CancellationToken cancellationToken)
    {
        var record = await _dbContext.PrivacyPolicies
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Version == version, cancellationToken);
        return record?.ToDomain();
    }

    public async Task<IReadOnlyList<PrivacyPolicy>> GetActiveAsync(CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var records = await _dbContext.PrivacyPolicies
            .AsNoTracking()
            .Where(p => p.IsActive && p.EffectiveDate <= now && (p.ExpirationDate == null || p.ExpirationDate > now))
            .OrderByDescending(p => p.EffectiveDate)
            .ToListAsync(cancellationToken);
        return records.Select(r => r.ToDomain()).ToList();
    }

    public async Task<IReadOnlyList<PrivacyPolicy>> ListAsync(CancellationToken cancellationToken)
    {
        var records = await _dbContext.PrivacyPolicies
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedAtUtc)
            .ToListAsync(cancellationToken);
        return records.Select(r => r.ToDomain()).ToList();
    }

    public Task AddAsync(PrivacyPolicy policy, CancellationToken cancellationToken)
    {
        _dbContext.PrivacyPolicies.Add(policy.ToRecord());
        return Task.CompletedTask;
    }

    public async Task UpdateAsync(PrivacyPolicy policy, CancellationToken cancellationToken)
    {
        var record = await _dbContext.PrivacyPolicies.FirstOrDefaultAsync(p => p.Id == policy.Id, cancellationToken);
        if (record is null)
            throw new InvalidOperationException($"Privacy Policy with ID {policy.Id} not found.");
        record.Version = policy.Version;
        record.Title = policy.Title;
        record.Content = policy.Content;
        record.EffectiveDate = policy.EffectiveDate;
        record.ExpirationDate = policy.ExpirationDate;
        record.IsActive = policy.IsActive;
        record.RequiredRoles = policy.RequiredRoles;
        record.RequiredCapabilities = policy.RequiredCapabilities;
        record.RequiredSystemPermissions = policy.RequiredSystemPermissions;
        record.UpdatedAtUtc = policy.UpdatedAtUtc;
        _dbContext.PrivacyPolicies.Update(record);
    }
}
