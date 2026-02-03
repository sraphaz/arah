using Araponga.Domain.Users;

namespace Araponga.Infrastructure.Postgres.Entities;

public sealed class UserPreferencesRecord
{
    public Guid UserId { get; set; }
    public ProfileVisibility ProfileVisibility { get; set; }
    public ContactVisibility ContactVisibility { get; set; }
    public bool ShareLocation { get; set; }
    public bool ShowMemberships { get; set; }
    public bool NotificationsPostsEnabled { get; set; }
    public bool NotificationsCommentsEnabled { get; set; }
    public bool NotificationsEventsEnabled { get; set; }
    public bool NotificationsAlertsEnabled { get; set; }
    public bool NotificationsMarketplaceEnabled { get; set; }
    public bool NotificationsModerationEnabled { get; set; }
    public bool NotificationsMembershipRequestsEnabled { get; set; }

    // Email Preferences
    public bool EmailReceiveEmails { get; set; } = true;
    public int EmailFrequency { get; set; } = 0; // Immediate
    public int EmailTypes { get; set; } = 31; // All (bit flags)

    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
