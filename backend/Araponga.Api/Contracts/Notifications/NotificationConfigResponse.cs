namespace Araponga.Api.Contracts.Notifications;

public sealed record NotificationConfigResponse(
    Guid Id,
    Guid? TerritoryId,
    IReadOnlyDictionary<string, NotificationTypeConfigResponse> NotificationTypes,
    IReadOnlyList<string> AvailableChannels,
    IReadOnlyDictionary<string, string> Templates,
    IReadOnlyDictionary<string, IReadOnlyList<string>> DefaultChannels,
    bool Enabled,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc
);

public sealed record NotificationTypeConfigResponse(
    string Type,
    bool Enabled,
    IReadOnlyList<string> AllowedChannels
);

public sealed record CreateOrUpdateNotificationConfigRequest(
    IReadOnlyDictionary<string, NotificationTypeConfigRequest> NotificationTypes,
    IReadOnlyList<string> AvailableChannels,
    IReadOnlyDictionary<string, string> Templates,
    IReadOnlyDictionary<string, IReadOnlyList<string>> DefaultChannels,
    bool Enabled
);

public sealed record NotificationTypeConfigRequest(
    string Type,
    bool Enabled,
    IReadOnlyList<string> AllowedChannels
);
