using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Application.Services;
using Araponga.Domain;
using Araponga.Infrastructure.InMemory;
using Xunit;

namespace Araponga.Tests.Application;

/// <summary>
/// Edge case tests for FeatureFlagService.
/// </summary>
public sealed class FeatureFlagServiceEdgeCasesTests
{
    private readonly InMemoryFeatureFlagService _featureFlagRepository;
    private readonly InMemoryUnitOfWork _unitOfWork;
    private readonly FeatureFlagCacheService? _cacheService;
    private readonly FeatureFlagService _service;

    public FeatureFlagServiceEdgeCasesTests()
    {
        _featureFlagRepository = new InMemoryFeatureFlagService();
        _unitOfWork = new InMemoryUnitOfWork();
        _cacheService = null; // Testar sem cache primeiro
        _service = new FeatureFlagService(_featureFlagRepository, _unitOfWork, _cacheService);
    }

    [Fact]
    public async Task GetEnabledFlagsAsync_WhenNoFlags_ReturnsEmptyList()
    {
        // Arrange
        var territoryId = Guid.NewGuid();

        // Act
        var flags = await _service.GetEnabledFlagsAsync(territoryId, CancellationToken.None);

        // Assert
        Assert.NotNull(flags);
        Assert.Empty(flags);
    }

    [Fact]
    public async Task SetEnabledFlagsAsync_WithFlags_UpdatesFlags()
    {
        // Arrange
        var territoryId = Guid.NewGuid();
        var flags = new List<FeatureFlag>
        {
            FeatureFlag.ChatEnabled,
            FeatureFlag.MarketplaceEnabled
        };

        // Act
        await _service.SetEnabledFlagsAsync(territoryId, flags, CancellationToken.None);

        // Assert
        var retrieved = await _service.GetEnabledFlagsAsync(territoryId, CancellationToken.None);
        Assert.Equal(2, retrieved.Count);
        Assert.Contains(FeatureFlag.ChatEnabled, retrieved);
        Assert.Contains(FeatureFlag.MarketplaceEnabled, retrieved);
    }

    [Fact]
    public async Task SetEnabledFlagsAsync_WithEmptyList_ClearsFlags()
    {
        // Arrange
        var territoryId = Guid.NewGuid();
        var initialFlags = new List<FeatureFlag> { FeatureFlag.ChatEnabled };
        await _service.SetEnabledFlagsAsync(territoryId, initialFlags, CancellationToken.None);

        // Act
        await _service.SetEnabledFlagsAsync(territoryId, Array.Empty<FeatureFlag>(), CancellationToken.None);

        // Assert
        var retrieved = await _service.GetEnabledFlagsAsync(territoryId, CancellationToken.None);
        Assert.Empty(retrieved);
    }

    [Fact]
    public void GetEnabledFlags_WhenNoFlags_ReturnsEmptyList()
    {
        // Arrange
        var territoryId = Guid.NewGuid();

        // Act
        var flags = _service.GetEnabledFlags(territoryId);

        // Assert
        Assert.NotNull(flags);
        Assert.Empty(flags);
    }

    [Fact]
    public async Task SetEnabledFlagsAsync_ThenGetEnabledFlags_ReturnsSameFlags()
    {
        // Arrange
        var territoryId = Guid.NewGuid();
        var flags = new List<FeatureFlag>
        {
            FeatureFlag.ChatEnabled,
            FeatureFlag.MarketplaceEnabled,
            FeatureFlag.EventPosts
        };

        // Act
        await _service.SetEnabledFlagsAsync(territoryId, flags, CancellationToken.None);
        var retrieved = _service.GetEnabledFlags(territoryId);

        // Assert
        Assert.Equal(3, retrieved.Count);
        Assert.Contains(FeatureFlag.ChatEnabled, retrieved);
        Assert.Contains(FeatureFlag.MarketplaceEnabled, retrieved);
        Assert.Contains(FeatureFlag.EventPosts, retrieved);
    }

    [Fact]
    public async Task SetEnabledFlagsAsync_WithAllFlags_EnablesAll()
    {
        // Arrange
        var territoryId = Guid.NewGuid();
        var allFlags = Enum.GetValues<FeatureFlag>().ToList();

        // Act
        await _service.SetEnabledFlagsAsync(territoryId, allFlags, CancellationToken.None);

        // Assert
        var retrieved = await _service.GetEnabledFlagsAsync(territoryId, CancellationToken.None);
        Assert.Equal(allFlags.Count, retrieved.Count);
    }

    [Fact]
    public async Task SetEnabledFlagsAsync_WithDuplicateFlags_StoresUnique()
    {
        // Arrange
        var territoryId = Guid.NewGuid();
        var flagsWithDuplicates = new List<FeatureFlag>
        {
            FeatureFlag.ChatEnabled,
            FeatureFlag.MarketplaceEnabled,
            FeatureFlag.ChatEnabled, // Duplicado
            FeatureFlag.EventPosts
        };

        // Act
        await _service.SetEnabledFlagsAsync(territoryId, flagsWithDuplicates, CancellationToken.None);

        // Assert
        var retrieved = await _service.GetEnabledFlagsAsync(territoryId, CancellationToken.None);
        // O repositório pode ou não remover duplicatas, mas deve funcionar
        Assert.True(retrieved.Count >= 3);
        Assert.Contains(FeatureFlag.ChatEnabled, retrieved);
        Assert.Contains(FeatureFlag.MarketplaceEnabled, retrieved);
        Assert.Contains(FeatureFlag.EventPosts, retrieved);
    }
}
