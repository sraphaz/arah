using Araponga.Domain.Users;
using Araponga.Infrastructure.InMemory;
using Xunit;

namespace Araponga.Tests.Infrastructure;

public sealed class UserRepositoryTests
{
    [Fact]
    public async Task GetByEmailAsync_ReturnsUser_WhenEmailExists()
    {
        // Arrange
        var sharedStore = new InMemorySharedStore();
        var repository = new InMemoryUserRepository(sharedStore);
        var user = new User(
            Guid.NewGuid(),
            "Test User",
            "test@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "google",
            "ext123",
            DateTime.UtcNow);
        await repository.AddAsync(user, CancellationToken.None);

        // Act
        var result = await repository.GetByEmailAsync("test@example.com", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result!.Id);
        Assert.Equal("test@example.com", result.Email);
    }

    [Fact]
    public async Task GetByEmailAsync_ReturnsNull_WhenEmailDoesNotExist()
    {
        // Arrange
        var sharedStore = new InMemorySharedStore();
        var repository = new InMemoryUserRepository(sharedStore);

        // Act
        var result = await repository.GetByEmailAsync("nonexistent@example.com", CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmailAsync_IsCaseInsensitive()
    {
        // Arrange
        var sharedStore = new InMemorySharedStore();
        var repository = new InMemoryUserRepository(sharedStore);
        var user = new User(
            Guid.NewGuid(),
            "Test User",
            "Test@Example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "google",
            "ext123",
            DateTime.UtcNow);
        await repository.AddAsync(user, CancellationToken.None);

        // Act
        var result1 = await repository.GetByEmailAsync("test@example.com", CancellationToken.None);
        var result2 = await repository.GetByEmailAsync("TEST@EXAMPLE.COM", CancellationToken.None);
        var result3 = await repository.GetByEmailAsync("TeSt@ExAmPlE.CoM", CancellationToken.None);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.NotNull(result3);
        Assert.Equal(user.Id, result1!.Id);
        Assert.Equal(user.Id, result2!.Id);
        Assert.Equal(user.Id, result3!.Id);
    }

    [Fact]
    public async Task GetByEmailAsync_ReturnsNull_WhenEmailIsNullOrEmpty()
    {
        // Arrange
        var sharedStore = new InMemorySharedStore();
        var repository = new InMemoryUserRepository(sharedStore);

        // Act
        var result1 = await repository.GetByEmailAsync("", CancellationToken.None);
        var result2 = await repository.GetByEmailAsync("   ", CancellationToken.None);
        var result3 = await repository.GetByEmailAsync(null!, CancellationToken.None);

        // Assert
        Assert.Null(result1);
        Assert.Null(result2);
        Assert.Null(result3);
    }
}
