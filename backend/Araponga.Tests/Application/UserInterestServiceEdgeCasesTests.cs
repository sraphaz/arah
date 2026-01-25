using Araponga.Application.Services;
using Araponga.Domain.Users;
using Araponga.Infrastructure.InMemory;
using Xunit;

namespace Araponga.Tests.Application;

/// <summary>
/// Edge case tests for UserInterestService (empty tag, duplicate, remove non-existent, list empty).
/// </summary>
public sealed class UserInterestServiceEdgeCasesTests
{
    [Fact]
    public async Task AddInterestAsync_WithEmptyTag_ReturnsFailure()
    {
        var ds = new InMemoryDataStore();
        var repo = new InMemoryUserInterestRepository(ds);
        var uow = new InMemoryUnitOfWork();
        var svc = new UserInterestService(repo, uow);
        var userId = Guid.NewGuid();

        var result = await svc.AddInterestAsync(userId, "  ", CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("required", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task AddInterestAsync_WhenDuplicate_ReturnsFailure()
    {
        var ds = new InMemoryDataStore();
        var repo = new InMemoryUserInterestRepository(ds);
        var uow = new InMemoryUnitOfWork();
        var svc = new UserInterestService(repo, uow);
        var userId = Guid.NewGuid();
        await svc.AddInterestAsync(userId, "eventos", CancellationToken.None);

        var result = await svc.AddInterestAsync(userId, "eventos", CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("already exists", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task RemoveInterestAsync_WithEmptyTag_ReturnsFailure()
    {
        var ds = new InMemoryDataStore();
        var repo = new InMemoryUserInterestRepository(ds);
        var uow = new InMemoryUnitOfWork();
        var svc = new UserInterestService(repo, uow);
        var userId = Guid.NewGuid();

        var result = await svc.RemoveInterestAsync(userId, "", CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("required", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task RemoveInterestAsync_WhenNotExists_ReturnsFailure()
    {
        var ds = new InMemoryDataStore();
        var repo = new InMemoryUserInterestRepository(ds);
        var uow = new InMemoryUnitOfWork();
        var svc = new UserInterestService(repo, uow);
        var userId = Guid.NewGuid();

        var result = await svc.RemoveInterestAsync(userId, "nonexistent", CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("does not exist", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ListInterestsAsync_WhenUserHasNone_ReturnsEmpty()
    {
        var ds = new InMemoryDataStore();
        var repo = new InMemoryUserInterestRepository(ds);
        var uow = new InMemoryUnitOfWork();
        var svc = new UserInterestService(repo, uow);
        var userId = Guid.NewGuid();

        var list = await svc.ListInterestsAsync(userId, CancellationToken.None);

        Assert.NotNull(list);
        Assert.Empty(list);
    }
}
