using Araponga.Application.Common;
using Araponga.Application.Models;
using Araponga.Application.Services;
using Araponga.Domain.Feed;
using Araponga.Domain.Events;
using Araponga.Domain.Membership;
using Araponga.Infrastructure.InMemory;
using Xunit;

namespace Araponga.Tests.Application;

public sealed class UserActivityServiceTests
{
    [Fact]
    public async Task GetUserPostsAsync_WhenUserHasPosts_ReturnsPagedResults()
    {
        // Arrange
        var dataStore = new InMemoryDataStore();
        var feedRepository = new InMemoryFeedRepository(dataStore);
        var eventRepository = new InMemoryTerritoryEventRepository(dataStore);
        var checkoutRepository = new InMemoryCheckoutRepository(dataStore);
        var participationRepository = new InMemoryEventParticipationRepository(dataStore);
        var storeRepository = new InMemoryStoreRepository(dataStore);
        var service = new UserActivityService(
            feedRepository,
            eventRepository,
            checkoutRepository,
            participationRepository,
            storeRepository);

        var userId = Guid.NewGuid();
        var territoryId = Guid.NewGuid();

        // Criar alguns posts
        for (int i = 0; i < 5; i++)
        {
            var post = new CommunityPost(
                Guid.NewGuid(),
                territoryId,
                userId,
                $"Post {i}",
                $"Content {i}",
                PostType.General,
                PostVisibility.Public,
                PostStatus.Published,
                null,
                DateTime.UtcNow);
            await feedRepository.AddPostAsync(post, CancellationToken.None);
        }

        var pagination = new PaginationParameters(1, 10);

        // Act
        var result = await service.GetUserPostsAsync(userId, pagination, CancellationToken.None);

        // Assert
        Assert.True(result.Items.Count >= 5);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(10, result.PageSize);
        Assert.True(result.TotalCount >= 5);
    }

    [Fact]
    public async Task GetUserEventsAsync_WhenUserHasEvents_ReturnsPagedResults()
    {
        // Arrange
        var dataStore = new InMemoryDataStore();
        var feedRepository = new InMemoryFeedRepository(dataStore);
        var eventRepository = new InMemoryTerritoryEventRepository(dataStore);
        var checkoutRepository = new InMemoryCheckoutRepository(dataStore);
        var participationRepository = new InMemoryEventParticipationRepository(dataStore);
        var storeRepository = new InMemoryStoreRepository(dataStore);
        var service = new UserActivityService(
            feedRepository,
            eventRepository,
            checkoutRepository,
            participationRepository,
            storeRepository);

        var userId = Guid.NewGuid();
        var territoryId = Guid.NewGuid();

        // Criar alguns eventos
        for (int i = 0; i < 3; i++)
        {
            var evt = new TerritoryEvent(
                Guid.NewGuid(),
                territoryId,
                $"Event {i}",
                $"Description {i}",
                DateTime.UtcNow.AddDays(i + 1),
                null,
                -23.2,
                -45.2,
                null,
                userId,
                MembershipRole.Resident,
                EventStatus.Scheduled,
                DateTime.UtcNow,
                DateTime.UtcNow);
            await eventRepository.AddAsync(evt, CancellationToken.None);
        }

        var pagination = new PaginationParameters(1, 10);

        // Act
        var result = await service.GetUserEventsAsync(userId, pagination, CancellationToken.None);

        // Assert
        Assert.True(result.Items.Count >= 3);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(10, result.PageSize);
        Assert.True(result.TotalCount >= 3);
    }

    [Fact]
    public async Task GetUserActivityHistoryAsync_WhenUserHasActivity_ReturnsHistory()
    {
        // Arrange
        var dataStore = new InMemoryDataStore();
        var feedRepository = new InMemoryFeedRepository(dataStore);
        var eventRepository = new InMemoryTerritoryEventRepository(dataStore);
        var checkoutRepository = new InMemoryCheckoutRepository(dataStore);
        var participationRepository = new InMemoryEventParticipationRepository(dataStore);
        var storeRepository = new InMemoryStoreRepository(dataStore);
        var service = new UserActivityService(
            feedRepository,
            eventRepository,
            checkoutRepository,
            participationRepository,
            storeRepository);

        var userId = Guid.NewGuid();
        var territoryId = Guid.NewGuid();

        // Criar post
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
        await feedRepository.AddPostAsync(post, CancellationToken.None);

        // Criar evento
        var evt = new TerritoryEvent(
            Guid.NewGuid(),
            territoryId,
            "Test Event",
            "Description",
            DateTime.UtcNow.AddDays(1),
            null,
            -23.2,
            -45.2,
            null,
            userId,
            MembershipRole.Resident,
            EventStatus.Scheduled,
            DateTime.UtcNow,
            DateTime.UtcNow);
        await eventRepository.AddAsync(evt, CancellationToken.None);

        var pagination = new PaginationParameters(1, 10);

        // Act
        var result = await service.GetUserActivityHistoryAsync(userId, pagination, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Posts.Items.Count >= 1);
        Assert.True(result.Events.Items.Count >= 1);
    }
}
