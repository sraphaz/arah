using Araponga.Application.Services;
using Araponga.Infrastructure.InMemory;
using Xunit;

namespace Araponga.Tests.Application;

/// <summary>
/// Edge case tests for AuditService,
/// focusing on LogAsync and ListAsync (no repository vs with repository).
/// </summary>
public sealed class AuditServiceEdgeCasesTests
{
    [Fact]
    public async Task LogAsync_WithValidArgs_DoesNotThrow()
    {
        var dataStore = new InMemoryDataStore();
        var logger = new InMemoryAuditLogger(dataStore);
        var service = new AuditService(logger, auditRepository: null);

        await service.LogAsync(
            "test.action",
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            CancellationToken.None);
    }

    [Fact]
    public async Task ListAsync_WhenNoAuditRepository_ReturnsEmptyPagedResult()
    {
        var dataStore = new InMemoryDataStore();
        var logger = new InMemoryAuditLogger(dataStore);
        var service = new AuditService(logger, auditRepository: null);

        var result = await service.ListAsync(
            territoryId: null,
            actorUserId: null,
            action: null,
            pageNumber: 1,
            pageSize: 50,
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotNull(result.Items);
        Assert.Empty(result.Items);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(50, result.PageSize);
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public async Task ListAsync_WhenNoAuditRepository_WithFilters_StillReturnsEmpty()
    {
        var dataStore = new InMemoryDataStore();
        var logger = new InMemoryAuditLogger(dataStore);
        var service = new AuditService(logger, auditRepository: null);

        var result = await service.ListAsync(
            territoryId: Guid.NewGuid(),
            actorUserId: Guid.NewGuid(),
            action: "user.blocked",
            pageNumber: 2,
            pageSize: 10,
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(2, result.PageNumber);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(0, result.TotalCount);
    }
}
