namespace Araponga.Api.Contracts.Users;

public sealed record UserPreferencesResponse(
    Guid UserId,
    string ProfileVisibility,
    string ContactVisibility,
    bool ShareLocation,
    bool ShowMemberships,
    NotificationPreferencesResponse Notifications,
    EmailPreferencesResponse Email,
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

public sealed record EmailPreferencesResponse(
    bool ReceiveEmails,
    string EmailFrequency,
    int EmailTypes);
