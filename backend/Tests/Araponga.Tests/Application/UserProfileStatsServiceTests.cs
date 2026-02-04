using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Application.Services;
using Araponga.Domain.Events;
using Araponga.Domain.Membership;
using Moq;
using Xunit;

namespace Araponga.Tests.Application;

/// <summary>
/// Testes para UserProfileStatsService (próximos passos Fase 12 - cobertura >90%).
/// </summary>
public sealed class UserProfileStatsServiceTests
{
    [Fact]
    public async Task GetStatsAsync_WhenAllReposNull_ReturnsZeroCounts()
    {
        var service = new UserProfileStatsService(
            feedRepository: null,
            eventRepository: null,
            participationRepository: null,
            membershipRepository: null);

        var userId = Guid.NewGuid();
        var result = await service.GetStatsAsync(userId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(0, result.PostsCreated);
        Assert.Equal(0, result.EventsCreated);
        Assert.Equal(0, result.EventsParticipated);
        Assert.Equal(0, result.TerritoriesMember);
        Assert.Equal(0, result.EntitiesConfirmed);
    }

    [Fact]
    public async Task GetStatsAsync_WithFeedRepo_ReturnsPostsCount()
    {
        var feedMock = new Mock<IFeedRepository>();
        var userId = Guid.NewGuid();
        feedMock.Setup(r => r.CountByAuthorAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(5);

        var service = new UserProfileStatsService(
            feedRepository: feedMock.Object,
            eventRepository: null,
            participationRepository: null,
            membershipRepository: null);

        var result = await service.GetStatsAsync(userId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(5, result.PostsCreated);
        Assert.Equal(0, result.EventsCreated);
        Assert.Equal(0, result.EventsParticipated);
        Assert.Equal(0, result.TerritoriesMember);
    }

    [Fact]
    public async Task GetStatsAsync_WithEventRepo_ReturnsEventsCreated()
    {
        var eventMock = new Mock<ITerritoryEventRepository>();
        var userId = Guid.NewGuid();
        eventMock.Setup(r => r.CountByAuthorAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(3);

        var service = new UserProfileStatsService(
            feedRepository: null,
            eventRepository: eventMock.Object,
            participationRepository: null,
            membershipRepository: null);

        var result = await service.GetStatsAsync(userId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(0, result.PostsCreated);
        Assert.Equal(3, result.EventsCreated);
        Assert.Equal(0, result.EventsParticipated);
        Assert.Equal(0, result.TerritoriesMember);
    }

    [Fact]
    public async Task GetStatsAsync_WithParticipationRepo_CountsOnlyConfirmedOrInterested()
    {
        var partMock = new Mock<IEventParticipationRepository>();
        var userId = Guid.NewGuid();
        var eventId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var participations = new List<EventParticipation>
        {
            new(eventId, userId, EventParticipationStatus.Confirmed, now, now),
            new(Guid.NewGuid(), userId, EventParticipationStatus.Interested, now, now),
        };
        partMock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(participations);

        var service = new UserProfileStatsService(
            feedRepository: null,
            eventRepository: null,
            participationRepository: partMock.Object,
            membershipRepository: null);

        var result = await service.GetStatsAsync(userId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(2, result.EventsParticipated);
    }

    [Fact]
    public async Task GetStatsAsync_WithMembershipRepo_CountsResidentOrVerifiedVisitor()
    {
        var memberMock = new Mock<ITerritoryMembershipRepository>();
        var userId = Guid.NewGuid();
        var territoryId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var memberships = new List<TerritoryMembership>
        {
            new(Guid.NewGuid(), userId, territoryId, MembershipRole.Resident, ResidencyVerification.None, null, null, now),
            new(Guid.NewGuid(), userId, Guid.NewGuid(), MembershipRole.Visitor, ResidencyVerification.GeoVerified, now, null, now),
        };
        memberMock.Setup(r => r.ListByUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(memberships);

        var service = new UserProfileStatsService(
            feedRepository: null,
            eventRepository: null,
            participationRepository: null,
            membershipRepository: memberMock.Object);

        var result = await service.GetStatsAsync(userId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(2, result.TerritoriesMember);
    }

    [Fact]
    public async Task GetStatsAsync_WithMembershipRepo_ExcludesVisitorWithNoVerification()
    {
        var memberMock = new Mock<ITerritoryMembershipRepository>();
        var userId = Guid.NewGuid();
        var territoryId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var memberships = new List<TerritoryMembership>
        {
            new(Guid.NewGuid(), userId, territoryId, MembershipRole.Visitor, ResidencyVerification.None, null, null, now),
        };
        memberMock.Setup(r => r.ListByUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(memberships);

        var service = new UserProfileStatsService(
            feedRepository: null,
            eventRepository: null,
            participationRepository: null,
            membershipRepository: memberMock.Object);

        var result = await service.GetStatsAsync(userId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(0, result.TerritoriesMember);
    }

    [Fact]
    public async Task GetStatsAsync_WithAllRepos_AggregatesCorrectly()
    {
        var feedMock = new Mock<IFeedRepository>();
        var eventMock = new Mock<ITerritoryEventRepository>();
        var partMock = new Mock<IEventParticipationRepository>();
        var memberMock = new Mock<ITerritoryMembershipRepository>();
        var userId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        feedMock.Setup(r => r.CountByAuthorAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(2);
        eventMock.Setup(r => r.CountByAuthorAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(1);
        partMock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<EventParticipation>
            {
                new(Guid.NewGuid(), userId, EventParticipationStatus.Confirmed, now, now),
            });
        memberMock.Setup(r => r.ListByUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TerritoryMembership>
            {
                new(Guid.NewGuid(), userId, Guid.NewGuid(), MembershipRole.Resident, ResidencyVerification.None, null, null, now),
            });

        var service = new UserProfileStatsService(
            feedMock.Object,
            eventMock.Object,
            partMock.Object,
            memberMock.Object);

        var result = await service.GetStatsAsync(userId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(2, result.PostsCreated);
        Assert.Equal(1, result.EventsCreated);
        Assert.Equal(1, result.EventsParticipated);
        Assert.Equal(1, result.TerritoriesMember);
        Assert.Equal(0, result.EntitiesConfirmed);
    }
}
