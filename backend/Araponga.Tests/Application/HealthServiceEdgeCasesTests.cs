using Araponga.Application.Common;
using Araponga.Application.Models;
using Araponga.Application.Services;
using Araponga.Domain.Feed;
using Araponga.Domain.Health;
using Araponga.Infrastructure.InMemory;
using Araponga.Tests.TestHelpers;
using Xunit;

namespace Araponga.Tests.Application;

/// <summary>
/// Edge case tests for HealthService (ReportAlert validation, ValidateAlert not found/wrong territory, ListAlerts).
/// </summary>
public sealed class HealthServiceEdgeCasesTests
{
    private static HealthService CreateService(
        InMemoryDataStore ds,
        bool alertPostsEnabled = true,
        AlertCacheService? alertCache = null)
    {
        var alertRepo = new InMemoryHealthAlertRepository(ds);
        var feedRepo = new InMemoryFeedRepository(ds);
        var auditLogger = new InMemoryAuditLogger(ds);
        var unitOfWork = new InMemoryUnitOfWork();
        var cache = CacheTestHelper.CreateDistributedCacheService();
        var featureFlags = new InMemoryFeatureFlagService();
        featureFlags.SetEnabledFlags(ds.Territories[0].Id, alertPostsEnabled
            ? new List<FeatureFlag> { FeatureFlag.AlertPosts }
            : Array.Empty<FeatureFlag>());
        var featureFlagCache = new FeatureFlagCacheService(featureFlags, cache);
        var guard = new TerritoryFeatureFlagGuard(featureFlagCache);
        return new HealthService(alertRepo, feedRepo, auditLogger, unitOfWork, guard, alertCache);
    }

    [Fact]
    public async Task ReportAlertAsync_WithEmptyTitle_ReturnsFailure()
    {
        var ds = new InMemoryDataStore();
        var svc = CreateService(ds);
        var territoryId = ds.Territories[0].Id;
        var userId = Guid.NewGuid();

        var result = await svc.ReportAlertAsync(territoryId, userId, "", "Desc", CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Title", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ReportAlertAsync_WithEmptyDescription_ReturnsFailure()
    {
        var ds = new InMemoryDataStore();
        var svc = CreateService(ds);
        var territoryId = ds.Territories[0].Id;
        var userId = Guid.NewGuid();

        var result = await svc.ReportAlertAsync(territoryId, userId, "Title", "  ", CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("description", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ValidateAlertAsync_WhenAlertNotFound_ReturnsFalse()
    {
        var ds = new InMemoryDataStore();
        var svc = CreateService(ds);
        var territoryId = ds.Territories[0].Id;

        var ok = await svc.ValidateAlertAsync(
            territoryId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            HealthAlertStatus.Validated,
            CancellationToken.None);

        Assert.False(ok);
    }

    [Fact]
    public async Task ValidateAlertAsync_WhenAlertInDifferentTerritory_ReturnsFalse()
    {
        var ds = new InMemoryDataStore();
        var svc = CreateService(ds);
        var territoryId = ds.Territories[0].Id;
        var otherTerritoryId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var alert = new HealthAlert(
            Guid.NewGuid(),
            otherTerritoryId,
            userId,
            "A",
            "B",
            HealthAlertStatus.Pending,
            DateTime.UtcNow);
        var alertRepo = new InMemoryHealthAlertRepository(ds);
        await alertRepo.AddAsync(alert, CancellationToken.None);

        var ok = await svc.ValidateAlertAsync(
            territoryId,
            alert.Id,
            Guid.NewGuid(),
            HealthAlertStatus.Validated,
            CancellationToken.None);

        Assert.False(ok);
    }

    [Fact]
    public async Task ListAlertsPagedAsync_ReturnsPagedResult()
    {
        var ds = new InMemoryDataStore();
        var svc = CreateService(ds);
        var territoryId = ds.Territories[0].Id;
        var pagination = new PaginationParameters(1, 10);

        var result = await svc.ListAlertsPagedAsync(territoryId, pagination, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotNull(result.Items);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(10, result.PageSize);
        Assert.True(result.TotalCount >= 0);
    }

    [Fact]
    public async Task ListAlertsAsync_WhenNoCache_UsesRepository()
    {
        var ds = new InMemoryDataStore();
        var svc = CreateService(ds, alertPostsEnabled: true, alertCache: null);
        var territoryId = ds.Territories[0].Id;

        var list = await svc.ListAlertsAsync(territoryId, CancellationToken.None);

        Assert.NotNull(list);
    }
}
