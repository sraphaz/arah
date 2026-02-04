using Araponga.Api.Contracts.Journeys.Onboarding;
using Araponga.Api.Services.Journeys;
using Araponga.Api.Services.Journeys.Backend;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Araponga.Tests.Bff;

public sealed class OnboardingJourneyServiceTests
{
    private static readonly Guid UserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid TerritoryId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private const string SessionId = "session-1";

    private static OnboardingJourneyService CreateService(Mock<IOnboardingJourneyBackend>? backendMock = null)
    {
        var mock = backendMock ?? new Mock<IOnboardingJourneyBackend>();
        var logger = new Mock<ILogger<OnboardingJourneyService>>();
        return new OnboardingJourneyService(mock.Object, logger.Object);
    }

    [Fact]
    public async Task CompleteOnboardingAsync_ReturnsNull_WhenTerritoryNotFound()
    {
        var backend = new Mock<IOnboardingJourneyBackend>();
        backend.Setup(x => x.GetTerritoryByIdAsync(TerritoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((BackendTerritoryInfo?)null);
        var service = CreateService(backend);

        var result = await service.CompleteOnboardingAsync(UserId, SessionId, TerritoryId);

        Assert.Null(result);
    }

    [Fact]
    public async Task CompleteOnboardingAsync_ReturnsNull_WhenSetActiveTerritoryFails()
    {
        var backend = new Mock<IOnboardingJourneyBackend>();
        backend.Setup(x => x.GetTerritoryByIdAsync(TerritoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BackendTerritoryInfo(TerritoryId, "T", "D"));
        backend.Setup(x => x.SetActiveTerritoryAsync(SessionId, TerritoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        var service = CreateService(backend);

        var result = await service.CompleteOnboardingAsync(UserId, SessionId, TerritoryId);

        Assert.Null(result);
    }

    [Fact]
    public async Task CompleteOnboardingAsync_ReturnsNull_WhenEnterAsVisitorReturnsNull()
    {
        var backend = new Mock<IOnboardingJourneyBackend>();
        backend.Setup(x => x.GetTerritoryByIdAsync(TerritoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BackendTerritoryInfo(TerritoryId, "T", "D"));
        backend.Setup(x => x.SetActiveTerritoryAsync(SessionId, TerritoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        backend.Setup(x => x.EnterAsVisitorAsync(UserId, TerritoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((BackendMembershipInfo?)null);
        var service = CreateService(backend);

        var result = await service.CompleteOnboardingAsync(UserId, SessionId, TerritoryId);

        Assert.Null(result);
    }

    [Fact]
    public async Task CompleteOnboardingAsync_ReturnsResponse_WhenBackendSucceeds()
    {
        var backend = new Mock<IOnboardingJourneyBackend>();
        backend.Setup(x => x.GetTerritoryByIdAsync(TerritoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BackendTerritoryInfo(TerritoryId, "Territory", "Desc"));
        backend.Setup(x => x.SetActiveTerritoryAsync(SessionId, TerritoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        backend.Setup(x => x.EnterAsVisitorAsync(UserId, TerritoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BackendMembershipInfo("VISITOR"));
        backend.Setup(x => x.GetUserByIdAsync(UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BackendUserInfo(UserId, "User", null));
        backend.Setup(x => x.ListFeedPagedAsync(TerritoryId, UserId, 1, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BackendPagedResult<BackendFeedPost>(Array.Empty<BackendFeedPost>(), 1, 20, 0, 0, false, false));
        backend.Setup(x => x.GetCountsByPostIdsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<Guid, BackendPostCounts>());
        var service = CreateService(backend);

        var result = await service.CompleteOnboardingAsync(UserId, SessionId, TerritoryId);

        Assert.NotNull(result);
        Assert.Equal(UserId, result.User.Id);
        Assert.Equal("User", result.User.DisplayName);
        Assert.Equal("VISITOR", result.User.Membership);
        Assert.Equal(TerritoryId, result.Territory.Id);
        Assert.Equal("Territory", result.Territory.Name);
        Assert.Equal(2, result.SuggestedActions.Count);
    }

    [Fact]
    public async Task CompleteOnboardingAsync_UsesDefaultDisplayName_WhenUserNotFound()
    {
        var backend = new Mock<IOnboardingJourneyBackend>();
        backend.Setup(x => x.GetTerritoryByIdAsync(TerritoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BackendTerritoryInfo(TerritoryId, "T", null));
        backend.Setup(x => x.SetActiveTerritoryAsync(SessionId, TerritoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        backend.Setup(x => x.EnterAsVisitorAsync(UserId, TerritoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BackendMembershipInfo("VISITOR"));
        backend.Setup(x => x.GetUserByIdAsync(UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((BackendUserInfo?)null);
        backend.Setup(x => x.ListFeedPagedAsync(TerritoryId, UserId, 1, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BackendPagedResult<BackendFeedPost>(Array.Empty<BackendFeedPost>(), 1, 20, 0, 0, false, false));
        backend.Setup(x => x.GetCountsByPostIdsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<Guid, BackendPostCounts>());
        var service = CreateService(backend);

        var result = await service.CompleteOnboardingAsync(UserId, SessionId, TerritoryId);

        Assert.NotNull(result);
        Assert.Equal("Usuário", result.User.DisplayName);
    }

    [Fact]
    public async Task GetSuggestedTerritoriesAsync_ReturnsResponse_FromBackend()
    {
        var backend = new Mock<IOnboardingJourneyBackend>();
        var list = new List<BackendTerritorySuggestion>
        {
            new(TerritoryId, "T1", "D1", -23.5, -46.6)
        };
        backend.Setup(x => x.GetTerritoriesNearbyAsync(-23.5, -46.6, 10, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(list);
        var service = CreateService(backend);

        var result = await service.GetSuggestedTerritoriesAsync(-23.5, -46.6, 10);

        Assert.NotNull(result);
        Assert.Single(result.Territories);
        Assert.Equal(TerritoryId, result.Territories[0].Id);
    }
}
