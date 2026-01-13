namespace Araponga.Domain.Users;

public sealed record NotificationPreferences
{
    public NotificationPreferences(
        bool postsEnabled,
        bool commentsEnabled,
        bool eventsEnabled,
        bool alertsEnabled,
        bool marketplaceEnabled,
        bool moderationEnabled,
        bool membershipRequestsEnabled)
    {
        PostsEnabled = postsEnabled;
        CommentsEnabled = commentsEnabled;
        EventsEnabled = eventsEnabled;
        AlertsEnabled = alertsEnabled;
        MarketplaceEnabled = marketplaceEnabled;
        ModerationEnabled = moderationEnabled;
        MembershipRequestsEnabled = membershipRequestsEnabled;
    }

    public bool PostsEnabled { get; init; }
    public bool CommentsEnabled { get; init; }
    public bool EventsEnabled { get; init; }
    public bool AlertsEnabled { get; init; }
    public bool MarketplaceEnabled { get; init; }
    public bool ModerationEnabled { get; init; }
    public bool MembershipRequestsEnabled { get; init; }

    public static NotificationPreferences Default() => new(
        postsEnabled: true,
        commentsEnabled: true,
        eventsEnabled: true,
        alertsEnabled: true,
        marketplaceEnabled: true,
        moderationEnabled: true,
        membershipRequestsEnabled: true);
}
