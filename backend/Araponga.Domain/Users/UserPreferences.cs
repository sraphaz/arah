namespace Araponga.Domain.Users;

public sealed class UserPreferences
{
    public UserPreferences(
        Guid userId,
        ProfileVisibility profileVisibility,
        ContactVisibility contactVisibility,
        bool shareLocation,
        bool showMemberships,
        NotificationPreferences notificationPreferences,
        DateTime createdAtUtc,
        DateTime updatedAtUtc)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User ID is required.", nameof(userId));
        }

        UserId = userId;
        ProfileVisibility = profileVisibility;
        ContactVisibility = contactVisibility;
        ShareLocation = shareLocation;
        ShowMemberships = showMemberships;
        NotificationPreferences = notificationPreferences;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
    }

    public Guid UserId { get; }
    public ProfileVisibility ProfileVisibility { get; private set; }
    public ContactVisibility ContactVisibility { get; private set; }
    public bool ShareLocation { get; private set; }
    public bool ShowMemberships { get; private set; }
    public NotificationPreferences NotificationPreferences { get; private set; }
    public DateTime CreatedAtUtc { get; }
    public DateTime UpdatedAtUtc { get; private set; }

    public void UpdatePrivacy(
        ProfileVisibility profileVisibility,
        ContactVisibility contactVisibility,
        bool shareLocation,
        bool showMemberships,
        DateTime updatedAtUtc)
    {
        ProfileVisibility = profileVisibility;
        ContactVisibility = contactVisibility;
        ShareLocation = shareLocation;
        ShowMemberships = showMemberships;
        UpdatedAtUtc = updatedAtUtc;
    }

    public void UpdateNotificationPreferences(
        NotificationPreferences preferences,
        DateTime updatedAtUtc)
    {
        NotificationPreferences = preferences;
        UpdatedAtUtc = updatedAtUtc;
    }

    public static UserPreferences CreateDefault(Guid userId, DateTime createdAtUtc)
    {
        return new UserPreferences(
            userId,
            ProfileVisibility.Public,
            ContactVisibility.ResidentsOnly,
            shareLocation: false,
            showMemberships: true,
            NotificationPreferences.Default(),
            createdAtUtc,
            createdAtUtc);
    }
}
