using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Application.Services;
using Araponga.Domain.Events;
using Araponga.Domain.Feed;
using Araponga.Domain.Membership;
using Araponga.Domain.Policies;
using Araponga.Domain.Users;
using Araponga.Infrastructure.InMemory;
using Moq;
using System.Text.Json;
using Xunit;

namespace Araponga.Tests.Application;

public sealed class DataExportServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ITerritoryMembershipRepository> _membershipRepositoryMock;
    private readonly Mock<IFeedRepository> _feedRepositoryMock;
    private readonly Mock<ITerritoryEventRepository> _eventRepositoryMock;
    private readonly Mock<IEventParticipationRepository> _eventParticipationRepositoryMock;
    private readonly Mock<INotificationInboxRepository> _notificationRepositoryMock;
    private readonly Mock<IUserPreferencesRepository> _preferencesRepositoryMock;
    private readonly Mock<ITermsAcceptanceRepository> _termsAcceptanceRepositoryMock;
    private readonly Mock<IPrivacyPolicyAcceptanceRepository> _privacyAcceptanceRepositoryMock;
    private readonly DataExportService _service;

    public DataExportServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _membershipRepositoryMock = new Mock<ITerritoryMembershipRepository>();
        _feedRepositoryMock = new Mock<IFeedRepository>();
        _eventRepositoryMock = new Mock<ITerritoryEventRepository>();
        _eventParticipationRepositoryMock = new Mock<IEventParticipationRepository>();
        _notificationRepositoryMock = new Mock<INotificationInboxRepository>();
        _preferencesRepositoryMock = new Mock<IUserPreferencesRepository>();
        _termsAcceptanceRepositoryMock = new Mock<ITermsAcceptanceRepository>();
        _privacyAcceptanceRepositoryMock = new Mock<IPrivacyPolicyAcceptanceRepository>();

        _service = new DataExportService(
            _userRepositoryMock.Object,
            _membershipRepositoryMock.Object,
            _feedRepositoryMock.Object,
            _eventRepositoryMock.Object,
            _eventParticipationRepositoryMock.Object,
            _notificationRepositoryMock.Object,
            _preferencesRepositoryMock.Object,
            _termsAcceptanceRepositoryMock.Object,
            _privacyAcceptanceRepositoryMock.Object);
    }

    [Fact]
    public async Task ExportUserDataAsync_WhenUserNotFound_ReturnsFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _service.ExportUserDataAsync(userId, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("not found", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ExportUserDataAsync_WhenUserExists_ReturnsCompleteData()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User(
            userId,
            "Test User",
            "test@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "google",
            "external-id",
            false,
            null,
            null,
            null,
            UserIdentityVerificationStatus.Unverified,
            null,
            null,
            null,
            null,
            DateTime.UtcNow);

        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _membershipRepositoryMock.Setup(r => r.ListByUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<TerritoryMembership>());
        _feedRepositoryMock.Setup(r => r.ListByAuthorAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<CommunityPost>());
        _eventRepositoryMock.Setup(r => r.ListByAuthorPagedAsync(userId, 0, int.MaxValue, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<TerritoryEvent>());
        _eventParticipationRepositoryMock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<EventParticipation>());
        _notificationRepositoryMock.Setup(r => r.ListByUserAsync(userId, 0, 1000, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<UserNotification>());
        _preferencesRepositoryMock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserPreferences?)null);
        _termsAcceptanceRepositoryMock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<TermsAcceptance>());
        _privacyAcceptanceRepositoryMock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<PrivacyPolicyAcceptance>());

        // Act
        var result = await _service.ExportUserDataAsync(userId, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(userId, result.Value.User.Id);
        Assert.Equal("Test User", result.Value.User.DisplayName);
        Assert.Equal("test@example.com", result.Value.User.Email);
        Assert.NotEqual(default(DateTime), result.Value.ExportedAtUtc);
    }

    [Fact]
    public async Task ExportUserDataAsync_IncludesAllUserData()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var territoryId = Guid.NewGuid();
        var user = new User(
            userId,
            "Test User",
            "test@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "google",
            "external-id",
            false,
            null,
            null,
            null,
            UserIdentityVerificationStatus.Unverified,
            null,
            null,
            null,
            null,
            DateTime.UtcNow);

        var membership = new TerritoryMembership(
            Guid.NewGuid(),
            userId,
            territoryId,
            MembershipRole.Resident,
            ResidencyVerification.None,
            null,
            null,
            DateTime.UtcNow);

        var post = new CommunityPost(
            Guid.NewGuid(),
            territoryId,
            userId,
            "Test Post",
            "Content",
            PostType.General,
            PostVisibility.Public,
            PostStatus.Published,
            null,
            DateTime.UtcNow);

        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _membershipRepositoryMock.Setup(r => r.ListByUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { membership });
        _feedRepositoryMock.Setup(r => r.ListByAuthorAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { post });
        _eventRepositoryMock.Setup(r => r.ListByAuthorPagedAsync(userId, 0, int.MaxValue, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<TerritoryEvent>());
        _eventParticipationRepositoryMock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<EventParticipation>());
        _notificationRepositoryMock.Setup(r => r.ListByUserAsync(userId, 0, 1000, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<UserNotification>());
        _preferencesRepositoryMock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserPreferences?)null);
        _termsAcceptanceRepositoryMock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<TermsAcceptance>());
        _privacyAcceptanceRepositoryMock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<PrivacyPolicyAcceptance>());

        // Act
        var result = await _service.ExportUserDataAsync(userId, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value.Memberships);
        Assert.Single(result.Value.Posts);
        Assert.Equal(membership.Id, result.Value.Memberships[0].Id);
        Assert.Equal(post.Id, result.Value.Posts[0].Id);
    }

    [Fact]
    public void SerializeToJson_ReturnsValidJson()
    {
        // Arrange
        var export = new UserDataExport
        {
            User = new UserExportData
            {
                Id = Guid.NewGuid(),
                DisplayName = "Test",
                Email = "test@example.com",
                CreatedAtUtc = DateTime.UtcNow
            },
            Memberships = Array.Empty<MembershipExportData>(),
            Posts = Array.Empty<PostExportData>(),
            Events = Array.Empty<EventExportData>(),
            EventParticipations = Array.Empty<EventParticipationExportData>(),
            Notifications = Array.Empty<NotificationExportData>(),
            Preferences = null,
            TermsAcceptances = Array.Empty<TermsAcceptanceExportData>(),
            PrivacyPolicyAcceptances = Array.Empty<PrivacyPolicyAcceptanceExportData>(),
            ExportedAtUtc = DateTime.UtcNow
        };

        // Act
        var json = _service.SerializeToJson(export);

        // Assert
        Assert.NotNull(json);
        Assert.NotEmpty(json);
        Assert.Contains("test", json, StringComparison.OrdinalIgnoreCase);
        
        // Verificar que é JSON válido
        var deserialized = JsonSerializer.Deserialize<UserDataExport>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        Assert.NotNull(deserialized);
        Assert.Equal(export.User.Id, deserialized!.User.Id);
    }
}
