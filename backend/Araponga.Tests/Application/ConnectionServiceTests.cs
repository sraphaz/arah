using Araponga.Application.Events;
using Araponga.Application.Services.Connections;
using Araponga.Domain.Connections;
using Araponga.Domain.Moderation;
using Araponga.Infrastructure.InMemory;
using Araponga.Infrastructure.Shared.InMemory;
using Araponga.Tests;
using Xunit;

namespace Araponga.Tests.Application;

public sealed class ConnectionServiceTests
{
    /// <summary>Arquitetura modular: conexões no Infrastructure (dataStore); User/Membership no Shared (sharedStore).</summary>
    private static ConnectionService CreateService(InMemoryDataStore dataStore, InMemorySharedStore sharedStore)
    {
        var connectionRepo = new InMemoryUserConnectionRepository(dataStore);
        var privacyRepo = new InMemoryConnectionPrivacySettingsRepository(dataStore);
        var blockRepo = new InMemoryUserBlockRepository(dataStore);
        var userRepo = new InMemoryUserRepository(sharedStore);
        var membershipRepo = new InMemoryTerritoryMembershipRepository(sharedStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var eventBus = new NoOpEventBus();
        return new ConnectionService(connectionRepo, privacyRepo, blockRepo, userRepo, membershipRepo, unitOfWork, eventBus);
    }

    [Fact]
    public async Task RequestConnectionAsync_SameUser_ReturnsFailure()
    {
        var dataStore = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var service = CreateService(dataStore, sharedStore);
        var userId = TestIds.TestUserId1;

        var result = await service.RequestConnectionAsync(userId, userId, null, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("si mesmo", result.Error);
    }

    [Fact]
    public async Task RequestConnectionAsync_WhenNoExistingConnection_CreatesPending()
    {
        var dataStore = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var service = CreateService(dataStore, sharedStore);
        var requester = TestIds.TestUserId1;
        var target = TestIds.ResidentUser;

        var result = await service.RequestConnectionAsync(requester, target, TestIds.Territory1, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(ConnectionStatus.Pending, result.Value.Status);
        Assert.Equal(requester, result.Value.RequesterUserId);
        Assert.Equal(target, result.Value.TargetUserId);
        var list = await service.GetConnectionsAsync(requester, null, CancellationToken.None);
        Assert.Single(list);
    }

    [Fact]
    public async Task RequestConnectionAsync_WhenAlreadyExists_ReturnsFailure()
    {
        var dataStore = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var service = CreateService(dataStore, sharedStore);
        var requester = TestIds.TestUserId1;
        var target = TestIds.ResidentUser;
        await service.RequestConnectionAsync(requester, target, null, CancellationToken.None);

        var result = await service.RequestConnectionAsync(requester, target, null, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("existe", result.Error!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task AcceptConnectionAsync_WhenTargetUser_Accepts()
    {
        var dataStore = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var service = CreateService(dataStore, sharedStore);
        var requester = TestIds.TestUserId1;
        var target = TestIds.ResidentUser;
        var requestResult = await service.RequestConnectionAsync(requester, target, null, CancellationToken.None);
        var connectionId = requestResult.Value!.Id;

        var result = await service.AcceptConnectionAsync(connectionId, target, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(ConnectionStatus.Accepted, result.Value!.Status);
        var accepted = await service.GetConnectionsAsync(requester, ConnectionStatus.Accepted, CancellationToken.None);
        Assert.Single(accepted);
    }

    [Fact]
    public async Task AcceptConnectionAsync_WhenNotTarget_ReturnsFailure()
    {
        var dataStore = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var service = CreateService(dataStore, sharedStore);
        var requester = TestIds.TestUserId1;
        var target = TestIds.ResidentUser;
        var requestResult = await service.RequestConnectionAsync(requester, target, null, CancellationToken.None);
        var connectionId = requestResult.Value!.Id;

        var result = await service.AcceptConnectionAsync(connectionId, requester, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("destinatário", result.Error);
    }

    [Fact]
    public async Task RemoveConnectionAsync_WhenAccepted_Removes()
    {
        var dataStore = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var service = CreateService(dataStore, sharedStore);
        var requester = TestIds.TestUserId1;
        var target = TestIds.ResidentUser;
        await service.RequestConnectionAsync(requester, target, null, CancellationToken.None);
        var acceptResult = await service.AcceptConnectionAsync(
            (await service.GetPendingRequestsAsync(target, CancellationToken.None))[0].Id,
            target,
            CancellationToken.None);
        var connectionId = acceptResult.Value!.Id;

        var result = await service.RemoveConnectionAsync(connectionId, requester, CancellationToken.None);

        Assert.True(result.IsSuccess);
        var accepted = await service.GetConnectionsAsync(requester, ConnectionStatus.Accepted, CancellationToken.None);
        Assert.Empty(accepted);
    }
}
