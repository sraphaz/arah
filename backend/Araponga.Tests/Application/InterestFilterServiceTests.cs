using Araponga.Application.Services;
using Araponga.Domain.Feed;
using Araponga.Domain.Users;
using Araponga.Infrastructure.InMemory;
using Xunit;

namespace Araponga.Tests.Application;

public sealed class InterestFilterServiceTests
{
    [Fact]
    public async Task FilterFeedByInterestsAsync_WhenUserHasInterests_ReturnsFilteredPosts()
    {
        // Arrange
        var dataStore = new InMemoryDataStore();
        var interestRepository = new InMemoryUserInterestRepository(dataStore);
        var service = new InterestFilterService(interestRepository);

        var userId = Guid.NewGuid();
        var territoryId = Guid.NewGuid();

        // Adicionar interesses ao usuário
        await interestRepository.AddAsync(new UserInterest(
            Guid.NewGuid(), userId, "eventos", DateTime.UtcNow), CancellationToken.None);
        await interestRepository.AddAsync(new UserInterest(
            Guid.NewGuid(), userId, "meio ambiente", DateTime.UtcNow), CancellationToken.None);

        // Criar posts
        var posts = new List<CommunityPost>
        {
            new CommunityPost(
                Guid.NewGuid(),
                territoryId,
                Guid.NewGuid(),
                "Eventos na praça",
                "Conteúdo sobre eventos",
                PostType.General,
                PostVisibility.Public,
                PostStatus.Published,
                null,
                DateTime.UtcNow),
            new CommunityPost(
                Guid.NewGuid(),
                territoryId,
                Guid.NewGuid(),
                "Outro assunto",
                "Conteúdo sem match",
                PostType.General,
                PostVisibility.Public,
                PostStatus.Published,
                null,
                DateTime.UtcNow),
            new CommunityPost(
                Guid.NewGuid(),
                territoryId,
                Guid.NewGuid(),
                "Meio ambiente",
                "Sobre sustentabilidade",
                PostType.General,
                PostVisibility.Public,
                PostStatus.Published,
                null,
                DateTime.UtcNow)
        };

        // Act
        var filtered = await service.FilterFeedByInterestsAsync(
            posts, userId, territoryId, CancellationToken.None);

        // Assert
        Assert.True(filtered.Count >= 2); // Pelo menos 2 posts devem passar (eventos e meio ambiente)
        Assert.All(filtered, post =>
        {
            var titleLower = post.Title?.ToLowerInvariant() ?? "";
            var contentLower = post.Content?.ToLowerInvariant() ?? "";
            Assert.True(
                titleLower.Contains("eventos") || contentLower.Contains("eventos") ||
                titleLower.Contains("meio ambiente") || contentLower.Contains("meio ambiente"),
                $"Post '{post.Title}' não deveria estar no feed filtrado");
        });
    }

    [Fact]
    public async Task FilterFeedByInterestsAsync_WhenUserHasNoInterests_ReturnsAllPosts()
    {
        // Arrange
        var dataStore = new InMemoryDataStore();
        var interestRepository = new InMemoryUserInterestRepository(dataStore);
        var service = new InterestFilterService(interestRepository);

        var userId = Guid.NewGuid();
        var territoryId = Guid.NewGuid();

        // Criar posts
        var posts = new List<CommunityPost>
        {
            new CommunityPost(
                Guid.NewGuid(),
                territoryId,
                Guid.NewGuid(),
                "Post 1",
                "Content 1",
                PostType.General,
                PostVisibility.Public,
                PostStatus.Published,
                null,
                DateTime.UtcNow),
            new CommunityPost(
                Guid.NewGuid(),
                territoryId,
                Guid.NewGuid(),
                "Post 2",
                "Content 2",
                PostType.General,
                PostVisibility.Public,
                PostStatus.Published,
                null,
                DateTime.UtcNow)
        };

        // Act
        var filtered = await service.FilterFeedByInterestsAsync(
            posts, userId, territoryId, CancellationToken.None);

        // Assert
        Assert.Equal(posts.Count, filtered.Count); // Todos os posts devem ser retornados
    }

    [Fact]
    public async Task FilterFeedByInterestsAsync_WhenEmptyPosts_ReturnsEmpty()
    {
        // Arrange
        var dataStore = new InMemoryDataStore();
        var interestRepository = new InMemoryUserInterestRepository(dataStore);
        var service = new InterestFilterService(interestRepository);

        var userId = Guid.NewGuid();
        var territoryId = Guid.NewGuid();

        // Act
        var filtered = await service.FilterFeedByInterestsAsync(
            Array.Empty<CommunityPost>(), userId, territoryId, CancellationToken.None);

        // Assert
        Assert.Empty(filtered);
    }

    [Fact]
    public async Task FilterFeedByInterestsAsync_WhenCaseInsensitive_MatchesCorrectly()
    {
        // Arrange
        var dataStore = new InMemoryDataStore();
        var interestRepository = new InMemoryUserInterestRepository(dataStore);
        var service = new InterestFilterService(interestRepository);

        var userId = Guid.NewGuid();
        var territoryId = Guid.NewGuid();

        // Adicionar interesse em minúsculas
        await interestRepository.AddAsync(new UserInterest(
            Guid.NewGuid(), userId, "eventos", DateTime.UtcNow), CancellationToken.None);

        // Criar post com título em maiúsculas
        var posts = new List<CommunityPost>
        {
            new CommunityPost(
                Guid.NewGuid(),
                territoryId,
                Guid.NewGuid(),
                "EVENTOS NA PRAÇA",
                "Conteúdo",
                PostType.General,
                PostVisibility.Public,
                PostStatus.Published,
                null,
                DateTime.UtcNow)
        };

        // Act
        var filtered = await service.FilterFeedByInterestsAsync(
            posts, userId, territoryId, CancellationToken.None);

        // Assert
        Assert.Single(filtered); // Deve encontrar match mesmo com case diferente
    }
}
