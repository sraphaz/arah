using Araponga.Domain.Connections;
using Araponga.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresConnectionPrivacySettingsRepository : IConnectionPrivacySettingsRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresConnectionPrivacySettingsRepository(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ConnectionPrivacySettings?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var record = await _dbContext.ConnectionPrivacySettings
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken);
        return record is null ? null : MapToDomain(record);
    }

    public Task<ConnectionPrivacySettings> AddAsync(ConnectionPrivacySettings settings, CancellationToken cancellationToken)
    {
        var record = MapToRecord(settings);
        _dbContext.ConnectionPrivacySettings.Add(record);
        return Task.FromResult(settings);
    }

    public async Task UpdateAsync(ConnectionPrivacySettings settings, CancellationToken cancellationToken)
    {
        var record = await _dbContext.ConnectionPrivacySettings
            .FirstOrDefaultAsync(s => s.UserId == settings.UserId, cancellationToken);
        if (record is null) return;

        record.WhoCanAddMe = (int)settings.WhoCanAddMe;
        record.WhoCanSeeMyConnections = (int)settings.WhoCanSeeMyConnections;
        record.ShowConnectionsInProfile = settings.ShowConnectionsInProfile;
        record.UpdatedAtUtc = settings.UpdatedAtUtc;
    }

    private static ConnectionPrivacySettings MapToDomain(ConnectionPrivacySettingsRecord r)
    {
        var s = ConnectionPrivacySettings.CreateDefault(r.UserId, r.CreatedAtUtc);
        s.Update(
            (ConnectionRequestPolicy)r.WhoCanAddMe,
            (ConnectionVisibility)r.WhoCanSeeMyConnections,
            r.ShowConnectionsInProfile,
            r.UpdatedAtUtc);
        return s;
    }

    private static ConnectionPrivacySettingsRecord MapToRecord(ConnectionPrivacySettings s)
    {
        return new ConnectionPrivacySettingsRecord
        {
            UserId = s.UserId,
            WhoCanAddMe = (int)s.WhoCanAddMe,
            WhoCanSeeMyConnections = (int)s.WhoCanSeeMyConnections,
            ShowConnectionsInProfile = s.ShowConnectionsInProfile,
            CreatedAtUtc = s.CreatedAtUtc,
            UpdatedAtUtc = s.UpdatedAtUtc
        };
    }
}
