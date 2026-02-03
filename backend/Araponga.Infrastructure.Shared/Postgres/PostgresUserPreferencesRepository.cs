using Araponga.Application.Interfaces;
using Araponga.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Shared.Postgres;

/// <summary>Implementação Postgres de IUserPreferencesRepository usando SharedDbContext.</summary>
public sealed class PostgresUserPreferencesRepository : IUserPreferencesRepository
{
    private readonly SharedDbContext _dbContext;

    public PostgresUserPreferencesRepository(SharedDbContext dbContext) => _dbContext = dbContext;

    public async Task<UserPreferences?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var record = await _dbContext.UserPreferences
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
        return record?.ToDomain();
    }

    public async Task<UserPreferences> GetOrCreateDefaultAsync(Guid userId, CancellationToken cancellationToken)
    {
        var existing = await GetByUserIdAsync(userId, cancellationToken);
        if (existing is not null) return existing;
        var defaultPreferences = UserPreferences.CreateDefault(userId, DateTime.UtcNow);
        await AddAsync(defaultPreferences, cancellationToken);
        return defaultPreferences;
    }

    public Task AddAsync(UserPreferences preferences, CancellationToken cancellationToken)
    {
        _dbContext.UserPreferences.Add(preferences.ToRecord());
        return Task.CompletedTask;
    }

    public async Task UpdateAsync(UserPreferences preferences, CancellationToken cancellationToken)
    {
        var record = await _dbContext.UserPreferences
            .FirstOrDefaultAsync(p => p.UserId == preferences.UserId, cancellationToken);
        if (record is null)
            _dbContext.UserPreferences.Add(preferences.ToRecord());
        else
            _dbContext.Entry(record).CurrentValues.SetValues(preferences.ToRecord());
    }
}
