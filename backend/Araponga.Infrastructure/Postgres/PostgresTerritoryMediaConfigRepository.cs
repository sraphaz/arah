using Araponga.Application.Interfaces.Media;
using Araponga.Domain.Media;
using Araponga.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresTerritoryMediaConfigRepository : ITerritoryMediaConfigRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresTerritoryMediaConfigRepository(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TerritoryMediaConfig?> GetByTerritoryIdAsync(Guid territoryId, CancellationToken cancellationToken = default)
    {
        var record = await _dbContext.TerritoryMediaConfigs
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.TerritoryId == territoryId, cancellationToken);
        return record?.ToDomain();
    }

    public async Task<TerritoryMediaConfig> GetOrCreateDefaultAsync(Guid territoryId, CancellationToken cancellationToken = default)
    {
        var record = await _dbContext.TerritoryMediaConfigs
            .FirstOrDefaultAsync(c => c.TerritoryId == territoryId, cancellationToken);
        if (record is not null)
            return record.ToDomain();

        var defaultConfig = CreateDefaultConfig(territoryId);
        _dbContext.TerritoryMediaConfigs.Add(defaultConfig.ToRecord());
        await _dbContext.SaveChangesAsync(cancellationToken);
        return defaultConfig;
    }

    public async Task SaveAsync(TerritoryMediaConfig config, CancellationToken cancellationToken = default)
    {
        var record = await _dbContext.TerritoryMediaConfigs
            .FirstOrDefaultAsync(c => c.TerritoryId == config.TerritoryId, cancellationToken);
        var newRecord = config.ToRecord();
        if (record is null)
        {
            _dbContext.TerritoryMediaConfigs.Add(newRecord);
        }
        else
        {
            record.PostsJson = newRecord.PostsJson;
            record.EventsJson = newRecord.EventsJson;
            record.MarketplaceJson = newRecord.MarketplaceJson;
            record.ChatJson = newRecord.ChatJson;
            record.UpdatedAtUtc = newRecord.UpdatedAtUtc;
            record.UpdatedByUserId = newRecord.UpdatedByUserId;
        }
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private static TerritoryMediaConfig CreateDefaultConfig(Guid territoryId)
    {
        return new TerritoryMediaConfig
        {
            TerritoryId = territoryId,
            Posts = new MediaContentConfig
            {
                ImagesEnabled = true,
                VideosEnabled = true,
                AudioEnabled = true,
                MaxMediaCount = 10,
                MaxVideoCount = 1,
                MaxAudioCount = 1,
                MaxImageSizeBytes = 10 * 1024 * 1024,
                MaxVideoSizeBytes = 50 * 1024 * 1024,
                MaxAudioSizeBytes = 10 * 1024 * 1024,
                MaxVideoDurationSeconds = null,
                MaxAudioDurationSeconds = null
            },
            Events = new MediaContentConfig
            {
                ImagesEnabled = true,
                VideosEnabled = true,
                AudioEnabled = true,
                MaxMediaCount = 6,
                MaxVideoCount = 1,
                MaxAudioCount = 1,
                MaxImageSizeBytes = 10 * 1024 * 1024,
                MaxVideoSizeBytes = 50 * 1024 * 1024,
                MaxAudioSizeBytes = 20 * 1024 * 1024,
                MaxVideoDurationSeconds = null,
                MaxAudioDurationSeconds = null
            },
            Marketplace = new MediaContentConfig
            {
                ImagesEnabled = true,
                VideosEnabled = true,
                AudioEnabled = true,
                MaxMediaCount = 10,
                MaxVideoCount = 1,
                MaxAudioCount = 1,
                MaxImageSizeBytes = 10 * 1024 * 1024,
                MaxVideoSizeBytes = 30 * 1024 * 1024,
                MaxAudioSizeBytes = 5 * 1024 * 1024,
                MaxVideoDurationSeconds = null,
                MaxAudioDurationSeconds = null
            },
            Chat = new MediaChatConfig
            {
                ImagesEnabled = true,
                AudioEnabled = true,
                VideosEnabled = false,
                MaxImageSizeBytes = 5 * 1024 * 1024,
                MaxAudioSizeBytes = 2 * 1024 * 1024,
                MaxAudioDurationSeconds = 60
            },
            UpdatedAtUtc = DateTime.UtcNow,
            UpdatedByUserId = null
        };
    }
}
