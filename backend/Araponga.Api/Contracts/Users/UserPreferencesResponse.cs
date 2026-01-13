namespace Araponga.Api.Contracts.Users;

public sealed record UserPreferencesResponse(
    Guid UserId,
    string ProfileVisibility,
    string ContactVisibility,
    bool ShareLocation,
    bool ShowMemberships,
    NotificationPreferencesResponse Notifications,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);

public sealed record NotificationPreferencesResponse(
    bool PostsEnabled,
    bool CommentsEnabled,
    bool EventsEnabled,
    bool AlertsEnabled,
    bool MarketplaceEnabled,
    bool ModerationEnabled,
    bool MembershipRequestsEnabled);
