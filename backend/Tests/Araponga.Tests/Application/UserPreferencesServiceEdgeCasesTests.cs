using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Domain.Users;
using Araponga.Infrastructure.InMemory;
using Araponga.Infrastructure.Shared.InMemory;
using Xunit;

namespace Araponga.Tests.Application;

/// <summary>
/// Edge case tests for UserPreferencesService.
/// </summary>
public sealed class UserPreferencesServiceEdgeCasesTests
{
    private readonly InMemorySharedStore _sharedStore;
    private readonly InMemoryUserPreferencesRepository _repository;
    private readonly InMemoryUnitOfWork _unitOfWork;
    private readonly UserPreferencesService _service;

    public UserPreferencesServiceEdgeCasesTests()
    {
        _sharedStore = new InMemorySharedStore();
        _repository = new InMemoryUserPreferencesRepository(_sharedStore);
        _unitOfWork = new InMemoryUnitOfWork();
        _service = new UserPreferencesService(_repository, _unitOfWork);
    }

    [Fact]
    public async Task GetPreferencesAsync_WhenPreferencesNotExist_CreatesDefault()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var preferences = await _service.GetPreferencesAsync(userId, CancellationToken.None);

        // Assert
        Assert.NotNull(preferences);
        Assert.Equal(userId, preferences.UserId);
        Assert.Equal(ProfileVisibility.Public, preferences.ProfileVisibility);
        Assert.Equal(ContactVisibility.ResidentsOnly, preferences.ContactVisibility);
    }

    [Fact]
    public async Task UpdatePrivacyPreferencesAsync_WithValidData_UpdatesPreferences()
    {
        // Arrange
        var userId = Guid.NewGuid();
        await _service.GetPreferencesAsync(userId, CancellationToken.None); // Create default

        // Act
        var updated = await _service.UpdatePrivacyPreferencesAsync(
            userId,
            ProfileVisibility.Private,
            ContactVisibility.ResidentsOnly,
            shareLocation: false,
            showMemberships: false,
            CancellationToken.None);

        // Assert
        Assert.Equal(ProfileVisibility.Private, updated.ProfileVisibility);
        Assert.Equal(ContactVisibility.ResidentsOnly, updated.ContactVisibility);
        Assert.False(updated.ShareLocation);
        Assert.False(updated.ShowMemberships);
    }

    [Fact]
    public async Task UpdateNotificationPreferencesAsync_WithValidData_UpdatesPreferences()
    {
        // Arrange
        var userId = Guid.NewGuid();
        await _service.GetPreferencesAsync(userId, CancellationToken.None); // Create default

        var notificationPrefs = new NotificationPreferences(
            postsEnabled: true,
            commentsEnabled: true,
            eventsEnabled: false,
            alertsEnabled: true,
            marketplaceEnabled: true,
            moderationEnabled: false,
            membershipRequestsEnabled: true);

        // Act
        var updated = await _service.UpdateNotificationPreferencesAsync(
            userId,
            notificationPrefs,
            CancellationToken.None);

        // Assert
        Assert.True(updated.NotificationPreferences.PostsEnabled);
        Assert.True(updated.NotificationPreferences.CommentsEnabled);
        Assert.False(updated.NotificationPreferences.EventsEnabled);
        Assert.False(updated.NotificationPreferences.ModerationEnabled);
    }

    [Fact]
    public async Task UpdateEmailPreferencesAsync_WithValidData_UpdatesPreferences()
    {
        // Arrange
        var userId = Guid.NewGuid();
        await _service.GetPreferencesAsync(userId, CancellationToken.None); // Create default

        var emailPrefs = new EmailPreferences(
            receiveEmails: true,
            EmailFrequency.Immediate,
            EmailTypes.Welcome | EmailTypes.Events | EmailTypes.CriticalAlerts);

        // Act
        var updated = await _service.UpdateEmailPreferencesAsync(
            userId,
            emailPrefs,
            CancellationToken.None);

        // Assert
        Assert.True(updated.EmailPreferences.ReceiveEmails);
        Assert.Equal(EmailFrequency.Immediate, updated.EmailPreferences.EmailFrequency);
        Assert.True((updated.EmailPreferences.EmailTypes & EmailTypes.Welcome) != 0);
        Assert.True((updated.EmailPreferences.EmailTypes & EmailTypes.Events) != 0);
        Assert.True((updated.EmailPreferences.EmailTypes & EmailTypes.Marketplace) == 0);
    }

    [Fact]
    public async Task GetPreferencesAsync_WhenPreferencesExist_ReturnsExisting()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var firstCall = await _service.GetPreferencesAsync(userId, CancellationToken.None);
        await _service.UpdatePrivacyPreferencesAsync(
            userId,
            ProfileVisibility.Private,
            ContactVisibility.ResidentsOnly,
            shareLocation: false,
            showMemberships: false,
            CancellationToken.None);

        // Act
        var secondCall = await _service.GetPreferencesAsync(userId, CancellationToken.None);

        // Assert
        Assert.Equal(ProfileVisibility.Private, secondCall.ProfileVisibility);
        Assert.Equal(ContactVisibility.ResidentsOnly, secondCall.ContactVisibility);
    }

    [Fact]
    public async Task UpdatePrivacyPreferencesAsync_WhenPreferencesNotExist_CreatesAndUpdates()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var updated = await _service.UpdatePrivacyPreferencesAsync(
            userId,
            ProfileVisibility.Private,
            ContactVisibility.ResidentsOnly,
            shareLocation: true,
            showMemberships: true,
            CancellationToken.None);

        // Assert
        Assert.NotNull(updated);
        Assert.Equal(userId, updated.UserId);
        Assert.Equal(ProfileVisibility.Private, updated.ProfileVisibility);
        Assert.Equal(ContactVisibility.ResidentsOnly, updated.ContactVisibility);
        Assert.True(updated.ShareLocation);
        Assert.True(updated.ShowMemberships);
    }

    [Fact]
    public async Task UpdateNotificationPreferencesAsync_WhenPreferencesNotExist_CreatesAndUpdates()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var notificationPrefs = new NotificationPreferences(
            postsEnabled: true,
            commentsEnabled: false,
            eventsEnabled: true,
            alertsEnabled: false,
            marketplaceEnabled: true,
            moderationEnabled: false,
            membershipRequestsEnabled: true);

        // Act
        var updated = await _service.UpdateNotificationPreferencesAsync(
            userId,
            notificationPrefs,
            CancellationToken.None);

        // Assert
        Assert.NotNull(updated);
        Assert.Equal(userId, updated.UserId);
        Assert.True(updated.NotificationPreferences.PostsEnabled);
        Assert.False(updated.NotificationPreferences.CommentsEnabled);
        Assert.True(updated.NotificationPreferences.EventsEnabled);
    }

    [Fact]
    public async Task UpdateEmailPreferencesAsync_WhenPreferencesNotExist_CreatesAndUpdates()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var emailPrefs = new EmailPreferences(
            receiveEmails: true,
            EmailFrequency.Daily,
            EmailTypes.Events | EmailTypes.Marketplace);

        // Act
        var updated = await _service.UpdateEmailPreferencesAsync(
            userId,
            emailPrefs,
            CancellationToken.None);

        // Assert
        Assert.NotNull(updated);
        Assert.Equal(userId, updated.UserId);
        Assert.True(updated.EmailPreferences.ReceiveEmails);
        Assert.Equal(EmailFrequency.Daily, updated.EmailPreferences.EmailFrequency);
        Assert.True((updated.EmailPreferences.EmailTypes & EmailTypes.Events) != 0);
        Assert.True((updated.EmailPreferences.EmailTypes & EmailTypes.Marketplace) != 0);
        Assert.True((updated.EmailPreferences.EmailTypes & EmailTypes.Welcome) == 0);
    }
}
