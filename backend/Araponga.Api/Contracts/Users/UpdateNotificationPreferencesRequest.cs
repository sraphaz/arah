namespace Araponga.Api.Contracts.Users;

public sealed record UpdateNotificationPreferencesRequest(
    bool PostsEnabled,
    bool CommentsEnabled,
    bool EventsEnabled,
    bool AlertsEnabled,
    bool MarketplaceEnabled,
    bool ModerationEnabled,
    bool MembershipRequestsEnabled);
