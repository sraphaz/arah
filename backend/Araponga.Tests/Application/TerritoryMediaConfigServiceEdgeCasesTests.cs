using Araponga.Application.Services.Media;
using Araponga.Domain.Media;
using Araponga.Infrastructure.InMemory;
using Xunit;

namespace Araponga.Tests.Application;

/// <summary>
/// Edge case tests for TerritoryMediaConfigService (próximos passos Fase 12 - cobertura >90%).
/// </summary>
public sealed class TerritoryMediaConfigServiceEdgeCasesTests
{
    private readonly InMemoryDataStore _dataStore;
    private readonly InMemoryTerritoryMediaConfigRepository _configRepository;
    private readonly InMemoryFeatureFlagService _featureFlags;
    private readonly InMemoryUnitOfWork _unitOfWork;
    private readonly InMemoryGlobalMediaLimits _globalLimits;
    private readonly TerritoryMediaConfigService _service;

    public TerritoryMediaConfigServiceEdgeCasesTests()
    {
        _dataStore = new InMemoryDataStore();
        _configRepository = new InMemoryTerritoryMediaConfigRepository(_dataStore);
        _featureFlags = new InMemoryFeatureFlagService();
        _unitOfWork = new InMemoryUnitOfWork();
        _globalLimits = new InMemoryGlobalMediaLimits();
        _service = new TerritoryMediaConfigService(
            _configRepository,
            _featureFlags,
            _unitOfWork,
            _globalLimits);
    }

    [Fact]
    public async Task GetConfigAsync_WhenNoConfig_CreatesDefault()
    {
        var territoryId = Guid.NewGuid();

        var config = await _service.GetConfigAsync(territoryId, CancellationToken.None);

        Assert.NotNull(config);
        Assert.Equal(territoryId, config.TerritoryId);
        Assert.NotNull(config.Posts);
        Assert.NotNull(config.Events);
        Assert.NotNull(config.Marketplace);
        Assert.NotNull(config.Chat);
        Assert.True(config.Posts.MaxMediaCount >= 1);
        Assert.False(config.Chat.VideosEnabled);
    }

    [Fact]
    public async Task GetConfigAsync_WhenConfigExists_ReturnsExisting()
    {
        var territoryId = Guid.NewGuid();
        var first = await _service.GetConfigAsync(territoryId, CancellationToken.None);
        first.Posts.MaxMediaCount = 5;

        var second = await _service.GetConfigAsync(territoryId, CancellationToken.None);

        Assert.Same(first, second);
        Assert.Equal(5, second.Posts.MaxMediaCount);
    }

    [Fact]
    public async Task UpdateConfigAsync_WhenTerritoryIdMismatch_ThrowsArgumentException()
    {
        var territoryA = Guid.NewGuid();
        var territoryB = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var config = await _service.GetConfigAsync(territoryA, CancellationToken.None);

        var ex = await Assert.ThrowsAsync<ArgumentException>(
            () => _service.UpdateConfigAsync(territoryB, config, userId, CancellationToken.None));

        Assert.Contains("Territory ID mismatch", ex.Message);
    }

    [Fact]
    public async Task UpdateConfigAsync_WithValidConfig_SavesSuccessfully()
    {
        var territoryId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var config = await _service.GetConfigAsync(territoryId, CancellationToken.None);
        config.Posts.MaxMediaCount = 8;

        var updated = await _service.UpdateConfigAsync(territoryId, config, userId, CancellationToken.None);

        Assert.NotNull(updated);
        Assert.Equal(territoryId, updated.TerritoryId);
        Assert.Equal(8, updated.Posts.MaxMediaCount);
        Assert.Equal(userId, updated.UpdatedByUserId);

        var retrieved = await _service.GetConfigAsync(territoryId, CancellationToken.None);
        Assert.Equal(8, retrieved.Posts.MaxMediaCount);
    }

    [Fact]
    public async Task GetEffectiveContentLimitsAsync_ReturnsConfigWithinGlobalLimits()
    {
        var territoryId = Guid.NewGuid();
        _ = await _service.GetConfigAsync(territoryId, CancellationToken.None);

        var effective = await _service.GetEffectiveContentLimitsAsync(
            territoryId,
            MediaContentType.Posts,
            CancellationToken.None);

        Assert.NotNull(effective);
        Assert.True(effective.MaxImageSizeBytes <= _globalLimits.MaxImageSizeBytes);
        Assert.True(effective.MaxVideoSizeBytes <= _globalLimits.MaxVideoSizeBytes);
        Assert.True(effective.MaxAudioSizeBytes <= _globalLimits.MaxAudioSizeBytes);
        Assert.True(effective.MaxMediaCount >= 1);
    }

    [Fact]
    public async Task GetEffectiveChatLimitsAsync_ReturnsConfigWithVideosDisabled()
    {
        var territoryId = Guid.NewGuid();
        _ = await _service.GetConfigAsync(territoryId, CancellationToken.None);

        var effective = await _service.GetEffectiveChatLimitsAsync(territoryId, CancellationToken.None);

        Assert.NotNull(effective);
        Assert.False(effective.VideosEnabled);
        Assert.True(effective.MaxImageSizeBytes <= _globalLimits.MaxImageSizeBytes);
        Assert.True(effective.MaxAudioSizeBytes <= _globalLimits.MaxAudioSizeBytes);
    }
}
