namespace Araponga.Domain.Notifications;

/// <summary>
/// Configuração avançada de notificações por território ou global.
/// </summary>
public sealed class NotificationConfig
{
    public NotificationConfig(
        Guid id,
        Guid? territoryId,
        IReadOnlyDictionary<string, NotificationTypeConfig> notificationTypes,
        IReadOnlyList<string> availableChannels,
        IReadOnlyDictionary<string, string> templates,
        IReadOnlyDictionary<string, IReadOnlyList<string>> defaultChannels,
        bool enabled,
        DateTime createdAtUtc,
        DateTime updatedAtUtc)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("ID is required.", nameof(id));
        }

        Id = id;
        TerritoryId = territoryId;
        NotificationTypes = notificationTypes ?? new Dictionary<string, NotificationTypeConfig>();
        AvailableChannels = availableChannels ?? new List<string>();
        Templates = templates ?? new Dictionary<string, string>();
        DefaultChannels = defaultChannels ?? new Dictionary<string, IReadOnlyList<string>>();
        Enabled = enabled;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
    }

    public Guid Id { get; }
    public Guid? TerritoryId { get; } // null = config global
    public IReadOnlyDictionary<string, NotificationTypeConfig> NotificationTypes { get; private set; }
    public IReadOnlyList<string> AvailableChannels { get; private set; } // Email, Push, InApp, SMS
    public IReadOnlyDictionary<string, string> Templates { get; private set; } // template por tipo
    public IReadOnlyDictionary<string, IReadOnlyList<string>> DefaultChannels { get; private set; } // canais padrão por tipo
    public bool Enabled { get; private set; }
    public DateTime CreatedAtUtc { get; }
    public DateTime UpdatedAtUtc { get; private set; }

    public void Update(
        IReadOnlyDictionary<string, NotificationTypeConfig> notificationTypes,
        IReadOnlyList<string> availableChannels,
        IReadOnlyDictionary<string, string> templates,
        IReadOnlyDictionary<string, IReadOnlyList<string>> defaultChannels,
        bool enabled,
        DateTime updatedAtUtc)
    {
        NotificationTypes = notificationTypes ?? new Dictionary<string, NotificationTypeConfig>();
        AvailableChannels = availableChannels ?? new List<string>();
        Templates = templates ?? new Dictionary<string, string>();
        DefaultChannels = defaultChannels ?? new Dictionary<string, IReadOnlyList<string>>();
        Enabled = enabled;
        UpdatedAtUtc = updatedAtUtc;
    }
}

/// <summary>
/// Configuração de um tipo específico de notificação.
/// </summary>
public sealed class NotificationTypeConfig
{
    public NotificationTypeConfig(
        string type,
        bool enabled,
        IReadOnlyList<string> allowedChannels)
    {
        if (string.IsNullOrWhiteSpace(type))
        {
            throw new ArgumentException("Type is required.", nameof(type));
        }

        Type = type;
        Enabled = enabled;
        AllowedChannels = allowedChannels ?? new List<string>();
    }

    public string Type { get; }
    public bool Enabled { get; }
    public IReadOnlyList<string> AllowedChannels { get; }
}
