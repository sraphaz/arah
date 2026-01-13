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

        var service = FeedServiceTestHelper.CreateFeedService(dataStore, eventBus);

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

        Assert.True(created.IsSuccess);
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

    [Fact]
    public async Task NotificationInbox_PaginationWorks()
    {
        var dataStore = new InMemoryDataStore();
        var inboxRepository = new InMemoryNotificationInboxRepository(dataStore);
        var userId = dataStore.Users[0].Id;

        // Criar múltiplas notificações
        for (var i = 0; i < 15; i++)
        {
            var notification = new UserNotification(
                Guid.NewGuid(),
                userId,
                $"Aviso {i}",
                $"Conteúdo {i}",
                "post.created",
                null,
                DateTime.UtcNow.AddMinutes(-i),
                null,
                Guid.NewGuid());

            await inboxRepository.AddAsync(notification, CancellationToken.None);
        }

        // Primeira página
        var page1 = await inboxRepository.ListByUserAsync(userId, 0, 10, CancellationToken.None);
        Assert.Equal(10, page1.Count);

        // Segunda página
        var page2 = await inboxRepository.ListByUserAsync(userId, 10, 10, CancellationToken.None);
        Assert.Equal(5, page2.Count);

        // Verificar ordenação (mais recentes primeiro)
        Assert.True(page1[0].CreatedAtUtc >= page1[1].CreatedAtUtc);
    }

    [Fact]
    public async Task NotificationInbox_MarkAsReadIsIdempotent()
    {
        var dataStore = new InMemoryDataStore();
        var inboxRepository = new InMemoryNotificationInboxRepository(dataStore);
        var userId = dataStore.Users[0].Id;

        var notification = new UserNotification(
            Guid.NewGuid(),
            userId,
            "Aviso",
            "Conteúdo",
            "post.created",
            null,
            DateTime.UtcNow,
            null,
            Guid.NewGuid());

        await inboxRepository.AddAsync(notification, CancellationToken.None);

        // Marcar como lida primeira vez
        var firstMark = await inboxRepository.MarkAsReadAsync(
            notification.Id,
            userId,
            DateTime.UtcNow,
            CancellationToken.None);
        Assert.True(firstMark);

        // Marcar como lida segunda vez (deve ser idempotente)
        var secondMark = await inboxRepository.MarkAsReadAsync(
            notification.Id,
            userId,
            DateTime.UtcNow.AddMinutes(1),
            CancellationToken.None);
        Assert.True(secondMark);

        var results = await inboxRepository.ListByUserAsync(userId, 0, 10, CancellationToken.None);
        Assert.Single(results);
        Assert.NotNull(results[0].ReadAtUtc);
    }

    [Fact]
    public async Task NotificationInbox_OnlyOwnerCanMarkAsRead()
    {
        var dataStore = new InMemoryDataStore();
        var inboxRepository = new InMemoryNotificationInboxRepository(dataStore);
        var ownerId = dataStore.Users[0].Id;
        var otherUserId = Guid.NewGuid();

        var notification = new UserNotification(
            Guid.NewGuid(),
            ownerId,
            "Aviso",
            "Conteúdo",
            "post.created",
            null,
            DateTime.UtcNow,
            null,
            Guid.NewGuid());

        await inboxRepository.AddAsync(notification, CancellationToken.None);

        // Tentar marcar como lida com outro usuário (deve falhar)
        var unauthorizedMark = await inboxRepository.MarkAsReadAsync(
            notification.Id,
            otherUserId,
            DateTime.UtcNow,
            CancellationToken.None);
        Assert.False(unauthorizedMark);

        // Marcar como lida com o dono (deve funcionar)
        var authorizedMark = await inboxRepository.MarkAsReadAsync(
            notification.Id,
            ownerId,
            DateTime.UtcNow,
            CancellationToken.None);
        Assert.True(authorizedMark);
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
