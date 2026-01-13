using Araponga.Domain.Users;
using Xunit;

namespace Araponga.Tests.Domain;

public sealed class UserPreferencesTests
{
    [Fact]
    public void UserPreferences_RequiresUserId()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new UserPreferences(
                Guid.Empty,
                ProfileVisibility.Public,
                ContactVisibility.ResidentsOnly,
                false,
                true,
                NotificationPreferences.Default(),
                DateTime.UtcNow,
                DateTime.UtcNow));

        Assert.Contains("User ID", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void UserPreferences_CreateDefault_CreatesWithDefaults()
    {
        var userId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var preferences = UserPreferences.CreateDefault(userId, now);

        Assert.Equal(userId, preferences.UserId);
        Assert.Equal(ProfileVisibility.Public, preferences.ProfileVisibility);
        Assert.Equal(ContactVisibility.ResidentsOnly, preferences.ContactVisibility);
        Assert.False(preferences.ShareLocation);
        Assert.True(preferences.ShowMemberships);
        Assert.True(preferences.NotificationPreferences.PostsEnabled);
        Assert.True(preferences.NotificationPreferences.CommentsEnabled);
    }

    [Fact]
    public void UserPreferences_UpdatePrivacy_UpdatesCorrectly()
    {
        var userId = Guid.NewGuid();
        var preferences = UserPreferences.CreateDefault(userId, DateTime.UtcNow);
        var updatedAt = DateTime.UtcNow.AddMinutes(1);

        preferences.UpdatePrivacy(
            ProfileVisibility.Private,
            ContactVisibility.Private,
            true,
            false,
            updatedAt);

        Assert.Equal(ProfileVisibility.Private, preferences.ProfileVisibility);
        Assert.Equal(ContactVisibility.Private, preferences.ContactVisibility);
        Assert.True(preferences.ShareLocation);
        Assert.False(preferences.ShowMemberships);
        Assert.Equal(updatedAt, preferences.UpdatedAtUtc);
    }

    [Fact]
    public void UserPreferences_UpdateNotificationPreferences_UpdatesCorrectly()
    {
        var userId = Guid.NewGuid();
        var preferences = UserPreferences.CreateDefault(userId, DateTime.UtcNow);
        var updatedAt = DateTime.UtcNow.AddMinutes(1);
        var newNotificationPrefs = new NotificationPreferences(
            postsEnabled: false,
            commentsEnabled: false,
            eventsEnabled: true,
            alertsEnabled: true,
            marketplaceEnabled: false,
            moderationEnabled: true,
            membershipRequestsEnabled: false);

        preferences.UpdateNotificationPreferences(newNotificationPrefs, updatedAt);

        Assert.False(preferences.NotificationPreferences.PostsEnabled);
        Assert.False(preferences.NotificationPreferences.CommentsEnabled);
        Assert.True(preferences.NotificationPreferences.EventsEnabled);
        Assert.Equal(updatedAt, preferences.UpdatedAtUtc);
    }
}
