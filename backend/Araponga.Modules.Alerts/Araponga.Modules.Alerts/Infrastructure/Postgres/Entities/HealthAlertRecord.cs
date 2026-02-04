using Araponga.Domain.Health;

namespace Araponga.Modules.Alerts.Infrastructure.Postgres.Entities;

public sealed class HealthAlertRecord
{
    public Guid Id { get; set; }
    public Guid TerritoryId { get; set; }
    public Guid ReporterUserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public HealthAlertStatus Status { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
