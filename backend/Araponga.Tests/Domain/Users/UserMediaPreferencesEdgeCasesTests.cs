using Araponga.Domain.Users;
using Xunit;

namespace Araponga.Tests.Domain.Users;

public sealed class UserMediaPreferencesEdgeCasesTests
{
    [Fact]
    public void UserMediaPreferences_DefaultValues_ShowAllMedia()
    {
        var preferences = new UserMediaPreferences
        {
            UserId = Guid.NewGuid()
        };

        Assert.True(preferences.ShowImages);
        Assert.True(preferences.ShowVideos);
        Assert.True(preferences.ShowAudio);
        Assert.False(preferences.AutoPlayVideos);
        Assert.False(preferences.AutoPlayAudio);
    }

    [Fact]
    public void UserMediaPreferences_CanDisableImages()
    {
        var preferences = new UserMediaPreferences
        {
            UserId = Guid.NewGuid(),
            ShowImages = false
        };

        Assert.False(preferences.ShowImages);
    }

    [Fact]
    public void UserMediaPreferences_CanDisableVideos()
    {
        var preferences = new UserMediaPreferences
        {
            UserId = Guid.NewGuid(),
            ShowVideos = false
        };

        Assert.False(preferences.ShowVideos);
    }

    [Fact]
    public void UserMediaPreferences_CanDisableAudio()
    {
        var preferences = new UserMediaPreferences
        {
            UserId = Guid.NewGuid(),
            ShowAudio = false
        };

        Assert.False(preferences.ShowAudio);
    }

    [Fact]
    public void UserMediaPreferences_CanEnableAutoPlayVideos()
    {
        var preferences = new UserMediaPreferences
        {
            UserId = Guid.NewGuid(),
            AutoPlayVideos = true
        };

        Assert.True(preferences.AutoPlayVideos);
    }

    [Fact]
    public void UserMediaPreferences_CanEnableAutoPlayAudio()
    {
        var preferences = new UserMediaPreferences
        {
            UserId = Guid.NewGuid(),
            AutoPlayAudio = true
        };

        Assert.True(preferences.AutoPlayAudio);
    }

    [Fact]
    public void UserMediaPreferences_CanSetAllProperties()
    {
        var userId = Guid.NewGuid();
        var updatedAt = DateTime.UtcNow;

        var preferences = new UserMediaPreferences
        {
            UserId = userId,
            ShowImages = false,
            ShowVideos = true,
            ShowAudio = false,
            AutoPlayVideos = true,
            AutoPlayAudio = false,
            UpdatedAtUtc = updatedAt
        };

        Assert.Equal(userId, preferences.UserId);
        Assert.False(preferences.ShowImages);
        Assert.True(preferences.ShowVideos);
        Assert.False(preferences.ShowAudio);
        Assert.True(preferences.AutoPlayVideos);
        Assert.False(preferences.AutoPlayAudio);
        Assert.Equal(updatedAt, preferences.UpdatedAtUtc);
    }

    [Fact]
    public void UserMediaPreferences_CanDisableAllMedia()
    {
        var preferences = new UserMediaPreferences
        {
            UserId = Guid.NewGuid(),
            ShowImages = false,
            ShowVideos = false,
            ShowAudio = false
        };

        Assert.False(preferences.ShowImages);
        Assert.False(preferences.ShowVideos);
        Assert.False(preferences.ShowAudio);
    }
}
