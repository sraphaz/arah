using Araponga.Application.Interfaces.Users;
using Araponga.Domain.Users;
using Araponga.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresUserMediaPreferencesRepository : IUserMediaPreferencesRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresUserMediaPreferencesRepository(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserMediaPreferences?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var record = await _dbContext.UserMediaPreferences
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
        return record?.ToDomain();
    }

    public async Task<UserMediaPreferences> GetOrCreateDefaultAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var record = await _dbContext.UserMediaPreferences
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
        if (record is not null)
            return record.ToDomain();

        var defaultPrefs = new UserMediaPreferences
        {
            UserId = userId,
            ShowImages = true,
            ShowVideos = true,
            ShowAudio = true,
            AutoPlayVideos = false,
            AutoPlayAudio = false,
            UpdatedAtUtc = DateTime.UtcNow
        };
        _dbContext.UserMediaPreferences.Add(defaultPrefs.ToRecord());
        await _dbContext.SaveChangesAsync(cancellationToken);
        return defaultPrefs;
    }

    public async Task SaveAsync(UserMediaPreferences preferences, CancellationToken cancellationToken = default)
    {
        var record = await _dbContext.UserMediaPreferences
            .FirstOrDefaultAsync(p => p.UserId == preferences.UserId, cancellationToken);
        var newRecord = preferences.ToRecord();
        if (record is null)
        {
            _dbContext.UserMediaPreferences.Add(newRecord);
        }
        else
        {
            record.ShowImages = newRecord.ShowImages;
            record.ShowVideos = newRecord.ShowVideos;
            record.ShowAudio = newRecord.ShowAudio;
            record.AutoPlayVideos = newRecord.AutoPlayVideos;
            record.AutoPlayAudio = newRecord.AutoPlayAudio;
            record.UpdatedAtUtc = newRecord.UpdatedAtUtc;
        }
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
