using Araponga.Application.Common;
using Araponga.Application.Services;
using Araponga.Domain.Users;
using Araponga.Infrastructure.InMemory;
using Xunit;

namespace Araponga.Tests.Application;

public sealed class UserInterestServiceTests
{
    [Fact]
    public async Task AddInterestAsync_WhenValid_ReturnsSuccess()
    {
        // Arrange
        var sharedStore = new InMemorySharedStore();
        var repository = new InMemoryUserInterestRepository(sharedStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new UserInterestService(repository, unitOfWork);
        var userId = Guid.NewGuid();

        // Act
        var result = await service.AddInterestAsync(userId, "meio ambiente", CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("meio ambiente", result.Value.InterestTag);
    }

    [Fact]
    public async Task AddInterestAsync_WhenTagTooLong_ReturnsFailure()
    {
        // Arrange
        var sharedStore = new InMemorySharedStore();
        var repository = new InMemoryUserInterestRepository(sharedStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new UserInterestService(repository, unitOfWork);
        var userId = Guid.NewGuid();
        var longTag = new string('a', 51);

        // Act
        var result = await service.AddInterestAsync(userId, longTag, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task AddInterestAsync_WhenMaxInterestsReached_ReturnsFailure()
    {
        // Arrange
        var sharedStore = new InMemorySharedStore();
        var repository = new InMemoryUserInterestRepository(sharedStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new UserInterestService(repository, unitOfWork);
        var userId = Guid.NewGuid();

        // Adicionar 10 interesses
        for (int i = 0; i < 10; i++)
        {
            await service.AddInterestAsync(userId, $"interest{i}", CancellationToken.None);
        }

        // Act
        var result = await service.AddInterestAsync(userId, "extra", CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task RemoveInterestAsync_WhenExists_ReturnsSuccess()
    {
        // Arrange
        var sharedStore = new InMemorySharedStore();
        var repository = new InMemoryUserInterestRepository(sharedStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new UserInterestService(repository, unitOfWork);
        var userId = Guid.NewGuid();
        await service.AddInterestAsync(userId, "meio ambiente", CancellationToken.None);

        // Act
        var result = await service.RemoveInterestAsync(userId, "meio ambiente", CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task ListInterestsAsync_WhenUserHasInterests_ReturnsList()
    {
        // Arrange
        var sharedStore = new InMemorySharedStore();
        var repository = new InMemoryUserInterestRepository(sharedStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new UserInterestService(repository, unitOfWork);
        var userId = Guid.NewGuid();
        await service.AddInterestAsync(userId, "meio ambiente", CancellationToken.None);
        await service.AddInterestAsync(userId, "eventos", CancellationToken.None);

        // Act
        var interests = await service.ListInterestsAsync(userId, CancellationToken.None);

        // Assert
        Assert.Equal(2, interests.Count);
    }
}
