using Araponga.Application.Services.Notifications;
using Araponga.Domain.Notifications;
using Araponga.Infrastructure.InMemory;
using Xunit;

namespace Araponga.Tests.Application;

/// <summary>
/// Edge case tests for NotificationConfigService (GetConfig default, invalid channel, GetTemplate, GetAvailableTypes).
/// </summary>
public sealed class NotificationConfigServiceEdgeCasesTests
{
    [Fact]
    public async Task GetConfigAsync_WhenNoConfig_ReturnsDefault()
    {
        var ds = new InMemoryDataStore();
        var repo = new InMemoryNotificationConfigRepository(ds);
        var uow = new InMemoryUnitOfWork();
        var svc = new NotificationConfigService(repo, uow);

        var result = await svc.GetConfigAsync(null, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.NotNull(result.Value!.NotificationTypes);
        Assert.Contains("post.created", result.Value.NotificationTypes.Keys);
    }

    [Fact]
    public async Task GetConfigAsync_WithTerritoryIdWhenNone_ReturnsDefault()
    {
        var ds = new InMemoryDataStore();
        var repo = new InMemoryNotificationConfigRepository(ds);
        var uow = new InMemoryUnitOfWork();
        var svc = new NotificationConfigService(repo, uow);
        var territoryId = Guid.NewGuid();

        var result = await svc.GetConfigAsync(territoryId, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public async Task CreateOrUpdateConfigAsync_WithInvalidChannel_ReturnsFailure()
    {
        var ds = new InMemoryDataStore();
        var repo = new InMemoryNotificationConfigRepository(ds);
        var uow = new InMemoryUnitOfWork();
        var svc = new NotificationConfigService(repo, uow);
        var types = new Dictionary<string, NotificationTypeConfig>
        {
            ["post.created"] = new NotificationTypeConfig("post.created", true, new[] { "InApp" })
        };

        var result = await svc.CreateOrUpdateConfigAsync(
            null,
            types,
            new[] { "Email", "InvalidChannel" },
            new Dictionary<string, string>(),
            new Dictionary<string, IReadOnlyList<string>>(),
            true,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Invalid channel", result.Error ?? "");
    }

    [Fact]
    public async Task GetAvailableTypesAsync_WhenNoConfig_ReturnsDefaultEnabledTypes()
    {
        var ds = new InMemoryDataStore();
        var repo = new InMemoryNotificationConfigRepository(ds);
        var uow = new InMemoryUnitOfWork();
        var svc = new NotificationConfigService(repo, uow);

        var result = await svc.GetAvailableTypesAsync(null, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.True(result.Value!.Count > 0);
    }

    [Fact]
    public async Task GetTemplateAsync_WhenNoConfig_ReturnsNullTemplate()
    {
        var ds = new InMemoryDataStore();
        var repo = new InMemoryNotificationConfigRepository(ds);
        var uow = new InMemoryUnitOfWork();
        var svc = new NotificationConfigService(repo, uow);

        var result = await svc.GetTemplateAsync(null, "post.created", CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value);
    }
}
