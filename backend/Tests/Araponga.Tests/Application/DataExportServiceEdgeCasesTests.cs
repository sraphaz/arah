using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Application.Services;
using Araponga.Domain.Events;
using Araponga.Domain.Feed;
using Araponga.Domain.Membership;
using Araponga.Domain.Policies;
using Araponga.Domain.Users;
using Moq;
using System.Text.Json;
using Xunit;

namespace Araponga.Tests.Application;

/// <summary>
/// Edge case tests for DataExportService,
/// focusing on empty/minimal data, SerializeToJson branches, and error paths.
/// </summary>
public sealed class DataExportServiceEdgeCasesTests
{
    private readonly Mock<IUserRepository> _userRepo;
    private readonly Mock<ITerritoryMembershipRepository> _membershipRepo;
    private readonly Mock<IFeedRepository> _feedRepo;
    private readonly Mock<ITerritoryEventRepository> _eventRepo;
    private readonly Mock<IEventParticipationRepository> _eventParticipationRepo;
    private readonly Mock<INotificationInboxRepository> _notificationRepo;
    private readonly Mock<IUserPreferencesRepository> _preferencesRepo;
    private readonly Mock<ITermsAcceptanceRepository> _termsRepo;
    private readonly Mock<IPrivacyPolicyAcceptanceRepository> _privacyRepo;
    private readonly DataExportService _service;

    public DataExportServiceEdgeCasesTests()
    {
        _userRepo = new Mock<IUserRepository>();
        _membershipRepo = new Mock<ITerritoryMembershipRepository>();
        _feedRepo = new Mock<IFeedRepository>();
        _eventRepo = new Mock<ITerritoryEventRepository>();
        _eventParticipationRepo = new Mock<IEventParticipationRepository>();
        _notificationRepo = new Mock<INotificationInboxRepository>();
        _preferencesRepo = new Mock<IUserPreferencesRepository>();
        _termsRepo = new Mock<ITermsAcceptanceRepository>();
        _privacyRepo = new Mock<IPrivacyPolicyAcceptanceRepository>();
        _service = new DataExportService(
            _userRepo.Object,
            _membershipRepo.Object,
            _feedRepo.Object,
            _eventRepo.Object,
            _eventParticipationRepo.Object,
            _notificationRepo.Object,
            _preferencesRepo.Object,
            _termsRepo.Object,
            _privacyRepo.Object);
    }

    [Fact]
    public async Task ExportUserDataAsync_WithEmptyGuid_ReturnsFailure()
    {
        _userRepo.Setup(r => r.GetByIdAsync(Guid.Empty, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var result = await _service.ExportUserDataAsync(Guid.Empty, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("not found", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ExportUserDataAsync_WithOnlyUser_ReturnsExportWithEmptyCollections()
    {
        var userId = Guid.NewGuid();
        var user = new User(
            userId,
            "Minimal",
            "m@x.com",
            "111.111.111-11",
            null,
            null,
            null,
            "google",
            "ext",
            DateTime.UtcNow);

        _userRepo.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _membershipRepo.Setup(r => r.ListByUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<TerritoryMembership>());
        _feedRepo.Setup(r => r.ListByAuthorAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<CommunityPost>());
        _eventRepo.Setup(r => r.ListByAuthorPagedAsync(userId, 0, int.MaxValue, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<TerritoryEvent>());
        _eventParticipationRepo.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<EventParticipation>());
        _notificationRepo.Setup(r => r.ListByUserAsync(userId, 0, 1000, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<UserNotification>());
        _preferencesRepo.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserPreferences?)null);
        _termsRepo.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<TermsAcceptance>());
        _privacyRepo.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<PrivacyPolicyAcceptance>());

        var result = await _service.ExportUserDataAsync(userId, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value.Memberships);
        Assert.Empty(result.Value.Posts);
        Assert.Empty(result.Value.Events);
        Assert.Empty(result.Value.EventParticipations);
        Assert.Empty(result.Value.Notifications);
        Assert.Null(result.Value.Preferences);
    }

    [Fact]
    public void SerializeToJson_WithUnicodeInDisplayName_SerializesAndRoundTrips()
    {
        var displayName = "Usuário café & naïve 文字";
        var export = new UserDataExport
        {
            User = new UserExportData
            {
                Id = Guid.NewGuid(),
                DisplayName = displayName,
                Email = "test@example.com",
                PhoneNumber = null,
                Address = null,
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

        var json = _service.SerializeToJson(export);

        Assert.NotNull(json);
        Assert.NotEmpty(json);
        var deserialized = JsonSerializer.Deserialize<UserDataExport>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        Assert.NotNull(deserialized);
        Assert.Equal(displayName, deserialized!.User.DisplayName);
    }
}
