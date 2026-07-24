using Arah.Domain.Health;
using Arah.Infrastructure.Postgres.Entities;

namespace Arah.Infrastructure.Postgres;

public static partial class PostgresMappers
{
    public static HealthAlertRecord ToRecord(this HealthAlert alert)
    {
        return new HealthAlertRecord
        {
            Id = alert.Id,
            TerritoryId = alert.TerritoryId,
            ReporterUserId = alert.ReporterUserId,
            Title = alert.Title,
            Description = alert.Description,
            Status = alert.Status,
            CreatedAtUtc = alert.CreatedAtUtc
        };
    }

    public static HealthAlert ToDomain(this HealthAlertRecord record)
    {
        return new HealthAlert(
            record.Id,
            record.TerritoryId,
            record.ReporterUserId,
            record.Title,
            record.Description,
            record.Status,
            record.CreatedAtUtc);
    }
}
