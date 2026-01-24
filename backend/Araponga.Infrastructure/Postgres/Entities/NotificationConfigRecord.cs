namespace Araponga.Infrastructure.Postgres.Entities;

public sealed class NotificationConfigRecord
{
    public Guid Id { get; set; }
    public Guid? TerritoryId { get; set; } // null = config global
    public string NotificationTypesJson { get; set; } = "{}"; // JSON: Dictionary<string, NotificationTypeConfig>
    public string AvailableChannelsJson { get; set; } = "[]"; // JSON: List<string>
    public string TemplatesJson { get; set; } = "{}"; // JSON: Dictionary<string, string>
    public string DefaultChannelsJson { get; set; } = "{}"; // JSON: Dictionary<string, List<string>>
    public bool Enabled { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
