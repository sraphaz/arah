using System.Text.Json;
using Araponga.Application.Events;
using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Application.Services;
using Araponga.Domain.Feed;
using Araponga.Infrastructure.Eventing;
using Araponga.Infrastructure.InMemory;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Araponga.Tests.Application;

public sealed class NotificationFlowTests
{
    private static readonly Guid ActiveTerritoryId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    [Fact]
    public async Task CreatePost_EnqueuesNotificationOutboxMessage()
    {
        var dataStore = new InMemoryDataStore();
        var serviceProvider = BuildServiceProvider(dataStore);
        var eventBus = new InMemoryEventBus(serviceProvider);

        var feedRepository = new InMemoryFeedRepository(dataStore);
        var accessEvaluator = new AccessEvaluator(new InMemoryTerritoryMembershipRepository(dataStore));
        var featureFlags = new InMemoryFeatureFlagService();
        var auditLogger = new InMemoryAuditLogger(dataStore);
        var blockRepository = new InMemoryUserBlockRepository(dataStore);
        var mapRepository = new InMemoryMapRepository(dataStore);
        var geoAnchorRepository = new InMemoryPostGeoAnchorRepository(dataStore);
        var postAssetRepository = new InMemoryPostAssetRepository(dataStore);
        var assetRepository = new InMemoryAssetRepository(dataStore);
        var sanctionRepository = new InMemorySanctionRepository(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();

        var service = new FeedService(
            feedRepository,
            accessEvaluator,
            featureFlags,
            auditLogger,
            blockRepository,
            mapRepository,
            geoAnchorRepository,
            postAssetRepository,
            assetRepository,
            sanctionRepository,
            eventBus,
            unitOfWork);

        var created = await service.CreatePostAsync(
            ActiveTerritoryId,
            Guid.NewGuid(),
            "Noticia",
            "Conteudo",
            PostType.General,
            PostVisibility.Public,
            PostStatus.Published,
            null,
            null,
            null,
            CancellationToken.None);

        Assert.True(created.success);
        Assert.Single(dataStore.OutboxMessages);

        var message = dataStore.OutboxMessages[0];
        Assert.Equal("notification.dispatch", message.Type);

        var payload = JsonSerializer.Deserialize<NotificationDispatchPayload>(
            message.PayloadJson,
            new JsonSerializerOptions(JsonSerializerDefaults.Web));
        Assert.NotNull(payload);
        Assert.Equal("post.created", payload!.Kind);
        Assert.Contains(dataStore.Users[0].Id, payload.Recipients);
    }

    [Fact]
    public async Task NotificationInbox_MarksAsRead()
    {
        var dataStore = new InMemoryDataStore();
        var inboxRepository = new InMemoryNotificationInboxRepository(dataStore);
        var notification = new UserNotification(
            Guid.NewGuid(),
            dataStore.Users[0].Id,
            "Aviso",
            "Conteudo",
            "post.created",
            null,
            DateTime.UtcNow,
            null,
            Guid.NewGuid());

        await inboxRepository.AddAsync(notification, CancellationToken.None);

        var updated = await inboxRepository.MarkAsReadAsync(
            notification.Id,
            notification.UserId,
            DateTime.UtcNow,
            CancellationToken.None);

        Assert.True(updated);

        var results = await inboxRepository.ListByUserAsync(notification.UserId, 0, 10, CancellationToken.None);
        Assert.Single(results);
        Assert.NotNull(results[0].ReadAtUtc);
    }

    private static ServiceProvider BuildServiceProvider(InMemoryDataStore dataStore)
    {
        var services = new ServiceCollection();
        services.AddSingleton(dataStore);
        services.AddSingleton<IOutbox, InMemoryOutbox>();
        services.AddSingleton<IUserRepository, InMemoryUserRepository>();
        services.AddSingleton<ITerritoryMembershipRepository, InMemoryTerritoryMembershipRepository>();
        services.AddSingleton<IEventHandler<PostCreatedEvent>, PostCreatedNotificationHandler>();
        services.AddSingleton<IEventHandler<ReportCreatedEvent>, ReportCreatedNotificationHandler>();
        return services.BuildServiceProvider();
    }
}
