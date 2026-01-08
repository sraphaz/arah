using Araponga.Domain.Feed;
using Araponga.Domain.Map;
using Araponga.Domain.Territories;
using Araponga.Domain.Users;
using Xunit;

namespace Araponga.Tests.Domain;

public sealed class DomainValidationTests
{
    [Fact]
    public void Territory_RequiresName()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new Territory(Guid.NewGuid(), "", null, SensitivityLevel.Low, TerritoryStatus.Active, DateTime.UtcNow));

        Assert.Contains("name", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void User_RequiresDisplayName()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new User(Guid.NewGuid(), "", "user@araponga.com", "google", "ext", DateTime.UtcNow));

        Assert.Contains("display", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void User_RequiresEmail()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new User(Guid.NewGuid(), "User", "", "google", "ext", DateTime.UtcNow));

        Assert.Contains("email", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void User_RequiresProvider()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new User(Guid.NewGuid(), "User", "user@araponga.com", "", "ext", DateTime.UtcNow));

        Assert.Contains("provider", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void User_RequiresExternalId()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new User(Guid.NewGuid(), "User", "user@araponga.com", "google", "", DateTime.UtcNow));

        Assert.Contains("external", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void UserTerritory_RequiresUserId()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new UserTerritory(Guid.NewGuid(), Guid.Empty, Guid.NewGuid(), MembershipStatus.Pending, DateTime.UtcNow));

        Assert.Contains("user", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void UserTerritory_RequiresTerritoryId()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new UserTerritory(Guid.NewGuid(), Guid.NewGuid(), Guid.Empty, MembershipStatus.Pending, DateTime.UtcNow));

        Assert.Contains("territory", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CommunityPost_RequiresTerritoryId()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new CommunityPost(Guid.NewGuid(), Guid.Empty, "Title", "Content", PostVisibility.Public, DateTime.UtcNow));

        Assert.Contains("territory", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CommunityPost_RequiresTitle()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new CommunityPost(Guid.NewGuid(), Guid.NewGuid(), "", "Content", PostVisibility.Public, DateTime.UtcNow));

        Assert.Contains("title", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CommunityPost_RequiresContent()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new CommunityPost(Guid.NewGuid(), Guid.NewGuid(), "Title", "", PostVisibility.Public, DateTime.UtcNow));

        Assert.Contains("content", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void MapEntity_RequiresTerritoryId()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new MapEntity(Guid.NewGuid(), Guid.Empty, "Ponto", "Categoria", MapEntityVisibility.Public, DateTime.UtcNow));

        Assert.Contains("territory", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void MapEntity_RequiresName()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new MapEntity(Guid.NewGuid(), Guid.NewGuid(), "", "Categoria", MapEntityVisibility.Public, DateTime.UtcNow));

        Assert.Contains("name", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void MapEntity_RequiresCategory()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new MapEntity(Guid.NewGuid(), Guid.NewGuid(), "Ponto", "", MapEntityVisibility.Public, DateTime.UtcNow));

        Assert.Contains("category", exception.Message, StringComparison.OrdinalIgnoreCase);
    }
}
