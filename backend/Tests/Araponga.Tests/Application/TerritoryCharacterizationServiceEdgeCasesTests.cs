using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Domain.Territories;
using Araponga.Infrastructure.InMemory;
using Araponga.Infrastructure.Shared.InMemory;
using Xunit;

namespace Araponga.Tests.Application;

/// <summary>
/// Edge case tests for TerritoryCharacterizationService,
/// focusing on null/empty inputs, empty tags, Unicode handling, and duplicate tags.
/// </summary>
public sealed class TerritoryCharacterizationServiceEdgeCasesTests
{
    private readonly InMemorySharedStore _sharedStore;
    private readonly InMemoryTerritoryCharacterizationRepository _repository;
    private readonly InMemoryUnitOfWork _unitOfWork;
    private readonly TerritoryCharacterizationService _service;

    public TerritoryCharacterizationServiceEdgeCasesTests()
    {
        _sharedStore = new InMemorySharedStore();
        _repository = new InMemoryTerritoryCharacterizationRepository(_sharedStore);
        _unitOfWork = new InMemoryUnitOfWork();
        _service = new TerritoryCharacterizationService(_repository, _unitOfWork);
    }

    [Fact]
    public async Task UpdateCharacterizationAsync_WithEmptyGuid_HandlesGracefully()
    {
        var result = await _service.UpdateCharacterizationAsync(
            Guid.Empty,
            new List<string> { "tag1" },
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(Guid.Empty, result.Value.TerritoryId);
    }

    [Fact]
    public async Task UpdateCharacterizationAsync_WithEmptyTags_CreatesEmptyCharacterization()
    {
        var territoryId = Guid.NewGuid();
        var result = await _service.UpdateCharacterizationAsync(
            territoryId,
            Array.Empty<string>(),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value.Tags);
    }

    [Fact]
    public async Task UpdateCharacterizationAsync_WithNullTags_ThrowsArgumentNullException()
    {
        var territoryId = Guid.NewGuid();
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _service.UpdateCharacterizationAsync(
                territoryId,
                null!,
                CancellationToken.None));
    }

    [Fact]
    public async Task UpdateCharacterizationAsync_WithWhitespaceTags_RemovesWhitespace()
    {
        var territoryId = Guid.NewGuid();
        var result = await _service.UpdateCharacterizationAsync(
            territoryId,
            new List<string> { "  tag1  ", "  ", "", "tag2" },
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Tags.Count); // tag1 e tag2 (whitespace removido)
        Assert.Contains("tag1", result.Value.Tags);
        Assert.Contains("tag2", result.Value.Tags);
        Assert.DoesNotContain("", result.Value.Tags);
    }

    [Fact]
    public async Task UpdateCharacterizationAsync_WithDuplicateTags_RemovesDuplicates()
    {
        var territoryId = Guid.NewGuid();
        var result = await _service.UpdateCharacterizationAsync(
            territoryId,
            new List<string> { "tag1", "TAG1", "tag1", "Tag1" },
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!.Tags);
        Assert.Equal("tag1", result.Value.Tags[0]);
    }

    [Fact]
    public async Task UpdateCharacterizationAsync_WithUnicodeTags_HandlesCorrectly()
    {
        var territoryId = Guid.NewGuid();
        var result = await _service.UpdateCharacterizationAsync(
            territoryId,
            new List<string> { "café", "naïve", "文字", "São Paulo" },
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(4, result.Value!.Tags.Count);
        Assert.Contains("café", result.Value.Tags);
        Assert.Contains("naïve", result.Value.Tags);
        Assert.Contains("文字", result.Value.Tags);
        Assert.Contains("são paulo", result.Value.Tags); // Normalizado para lowercase
    }

    [Fact]
    public async Task UpdateCharacterizationAsync_WithMixedCaseTags_NormalizesToLowercase()
    {
        var territoryId = Guid.NewGuid();
        var result = await _service.UpdateCharacterizationAsync(
            territoryId,
            new List<string> { "TAG1", "Tag2", "tag3", "TAG4" },
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(4, result.Value!.Tags.Count);
        Assert.All(result.Value.Tags, tag => Assert.Equal(tag, tag.ToLowerInvariant()));
    }

    [Fact]
    public async Task UpdateCharacterizationAsync_WithExistingCharacterization_UpdatesTags()
    {
        var territoryId = Guid.NewGuid();
        
        // Criar caracterização inicial
        var result1 = await _service.UpdateCharacterizationAsync(
            territoryId,
            new List<string> { "tag1", "tag2" },
            CancellationToken.None);

        Assert.True(result1.IsSuccess);
        Assert.Equal(2, result1.Value!.Tags.Count);

        // Atualizar caracterização
        var result2 = await _service.UpdateCharacterizationAsync(
            territoryId,
            new List<string> { "tag3", "tag4" },
            CancellationToken.None);

        Assert.True(result2.IsSuccess);
        Assert.Equal(2, result2.Value!.Tags.Count);
        Assert.Contains("tag3", result2.Value.Tags);
        Assert.Contains("tag4", result2.Value.Tags);
        Assert.DoesNotContain("tag1", result2.Value.Tags);
        Assert.DoesNotContain("tag2", result2.Value.Tags);
    }

    [Fact]
    public async Task GetCharacterizationAsync_WithNonExistentTerritory_ReturnsNull()
    {
        var result = await _service.GetCharacterizationAsync(
            Guid.NewGuid(),
            CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetCharacterizationAsync_WithExistingCharacterization_ReturnsCharacterization()
    {
        var territoryId = Guid.NewGuid();
        
        await _service.UpdateCharacterizationAsync(
            territoryId,
            new List<string> { "tag1", "tag2" },
            CancellationToken.None);

        var result = await _service.GetCharacterizationAsync(
            territoryId,
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(territoryId, result!.TerritoryId);
        Assert.Equal(2, result.Tags.Count);
    }

    [Fact]
    public async Task UpdateCharacterizationAsync_WithVeryLongTags_HandlesCorrectly()
    {
        var territoryId = Guid.NewGuid();
        var longTag = new string('a', 1000);
        var result = await _service.UpdateCharacterizationAsync(
            territoryId,
            new List<string> { longTag },
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!.Tags);
        Assert.Equal(longTag, result.Value.Tags[0]);
    }

    [Fact]
    public async Task UpdateCharacterizationAsync_WithManyTags_HandlesCorrectly()
    {
        var territoryId = Guid.NewGuid();
        var manyTags = Enumerable.Range(1, 100).Select(i => $"tag{i}").ToList();
        var result = await _service.UpdateCharacterizationAsync(
            territoryId,
            manyTags,
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(100, result.Value!.Tags.Count);
    }

    [Fact]
    public async Task UpdateCharacterizationAsync_WithSpecialCharacters_HandlesCorrectly()
    {
        var territoryId = Guid.NewGuid();
        var result = await _service.UpdateCharacterizationAsync(
            territoryId,
            new List<string> { "tag-with-dashes", "tag_with_underscores", "tag.with.dots", "tag with spaces" },
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(4, result.Value!.Tags.Count);
        Assert.Contains("tag-with-dashes", result.Value.Tags);
        Assert.Contains("tag_with_underscores", result.Value.Tags);
        Assert.Contains("tag.with.dots", result.Value.Tags);
        Assert.Contains("tag with spaces", result.Value.Tags);
    }
}
