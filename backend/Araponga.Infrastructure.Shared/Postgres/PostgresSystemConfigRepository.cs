using Araponga.Application.Interfaces;
using Araponga.Domain.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Shared.Postgres;

/// <summary>Implementação Postgres de ISystemConfigRepository usando SharedDbContext.</summary>
public sealed class PostgresSystemConfigRepository : ISystemConfigRepository
{
    private readonly SharedDbContext _dbContext;

    public PostgresSystemConfigRepository(SharedDbContext dbContext) => _dbContext = dbContext;

    public async Task<SystemConfig?> GetByKeyAsync(string key, CancellationToken cancellationToken)
    {
        var normalizedKey = NormalizeKey(key);
        if (string.IsNullOrWhiteSpace(normalizedKey)) return null;
        var record = await _dbContext.SystemConfigs
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Key == normalizedKey, cancellationToken);
        return record?.ToDomain();
    }

    public async Task<IReadOnlyList<SystemConfig>> ListAsync(SystemConfigCategory? category, CancellationToken cancellationToken)
    {
        var query = _dbContext.SystemConfigs.AsNoTracking();
        if (category.HasValue) query = query.Where(c => c.Category == category.Value);
        var records = await query.OrderBy(c => c.Key).ToListAsync(cancellationToken);
        return records.Select(r => r.ToDomain()).ToList();
    }

    public async Task UpsertAsync(SystemConfig config, CancellationToken cancellationToken)
    {
        var existing = await _dbContext.SystemConfigs
            .FirstOrDefaultAsync(c => c.Key == config.Key, cancellationToken);
        var record = config.ToRecord();
        if (existing is null)
            _dbContext.SystemConfigs.Add(record);
        else
            _dbContext.Entry(existing).CurrentValues.SetValues(record);
    }

    private static string NormalizeKey(string key)
        => string.IsNullOrWhiteSpace(key) ? string.Empty : key.Trim().ToLowerInvariant();
}
