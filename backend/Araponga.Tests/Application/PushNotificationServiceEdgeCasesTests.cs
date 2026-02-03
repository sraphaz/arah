using Araponga.Application.Services;
using Araponga.Domain.Users;
using Araponga.Infrastructure.InMemory;
using Araponga.Infrastructure.Shared.InMemory;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Araponga.Tests.Application;

/// <summary>
/// Edge case tests for PushNotificationService (GetDevice not found, ListDevices empty, Unregister, SendToUser no provider).
/// </summary>
public sealed class PushNotificationServiceEdgeCasesTests
{
    [Fact]
    public async Task GetDeviceAsync_WhenNotFound_ReturnsFailure()
    {
        var sharedStore = new InMemorySharedStore();
        var repo = new InMemoryUserDeviceRepository(sharedStore);
        var logger = NullLogger<PushNotificationService>.Instance;
        var svc = new PushNotificationService(repo, null, logger);
        var userId = Guid.NewGuid();

        var result = await svc.GetDeviceAsync(userId, Guid.NewGuid(), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("not found", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ListDevicesAsync_WhenUserHasNone_ReturnsEmptyList()
    {
        var sharedStore = new InMemorySharedStore();
        var repo = new InMemoryUserDeviceRepository(sharedStore);
        var logger = NullLogger<PushNotificationService>.Instance;
        var svc = new PushNotificationService(repo, null, logger);
        var userId = Guid.NewGuid();

        var result = await svc.ListDevicesAsync(userId, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task UnregisterDeviceAsync_WhenNotFound_ReturnsFailure()
    {
        var sharedStore = new InMemorySharedStore();
        var repo = new InMemoryUserDeviceRepository(sharedStore);
        var logger = NullLogger<PushNotificationService>.Instance;
        var svc = new PushNotificationService(repo, null, logger);
        var userId = Guid.NewGuid();

        var result = await svc.UnregisterDeviceAsync(userId, Guid.NewGuid(), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("not found", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task SendToUserAsync_WhenNoProvider_ReturnsZeroSent()
    {
        var sharedStore = new InMemorySharedStore();
        var repo = new InMemoryUserDeviceRepository(sharedStore);
        var logger = NullLogger<PushNotificationService>.Instance;
        var svc = new PushNotificationService(repo, null, logger);
        var userId = Guid.NewGuid();

        var result = await svc.SendToUserAsync(userId, "Title", "Body", null, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.Value);
    }

    [Fact]
    public async Task SendToUsersAsync_WhenNoProvider_ReturnsZeroSent()
    {
        var sharedStore = new InMemorySharedStore();
        var repo = new InMemoryUserDeviceRepository(sharedStore);
        var logger = NullLogger<PushNotificationService>.Instance;
        var svc = new PushNotificationService(repo, null, logger);

        var result = await svc.SendToUsersAsync(new[] { Guid.NewGuid() }, "Title", "Body", null, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.Value);
    }
}
