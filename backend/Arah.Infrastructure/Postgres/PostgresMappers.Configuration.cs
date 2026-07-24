using Arah.Infrastructure.Postgres.Entities;

namespace Arah.Infrastructure.Postgres;

public static partial class PostgresMappers
{
    public static SystemConfigRecord ToRecord(this Arah.Domain.Configuration.SystemConfig config)
    {
        return new SystemConfigRecord
        {
            Id = config.Id,
            Key = config.Key,
            Value = config.Value,
            Category = config.Category,
            Description = config.Description,
            CreatedAtUtc = config.CreatedAtUtc,
            CreatedByUserId = config.CreatedByUserId,
            UpdatedAtUtc = config.UpdatedAtUtc,
            UpdatedByUserId = config.UpdatedByUserId
        };
    }

    public static Arah.Domain.Configuration.SystemConfig ToDomain(this SystemConfigRecord record)
    {
        return new Arah.Domain.Configuration.SystemConfig(
            record.Id,
            record.Key,
            record.Value,
            record.Category,
            record.Description,
            record.CreatedAtUtc,
            record.CreatedByUserId,
            record.UpdatedAtUtc,
            record.UpdatedByUserId);
    }
}
