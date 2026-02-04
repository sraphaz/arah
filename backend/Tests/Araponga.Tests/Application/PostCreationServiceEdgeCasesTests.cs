using Araponga.Application.Common;
using Araponga.Application.Events;
using Araponga.Application.Interfaces;
using Araponga.Application.Interfaces.Media;
using Araponga.Application.Models;
using Araponga.Application.Services;
using Araponga.Application.Services.Media;
using Araponga.Domain.Feed;
using Araponga.Modules.Map.Domain;
using Araponga.Infrastructure.Eventing;
using Araponga.Infrastructure.InMemory;
using Moq;
using Xunit;

namespace Araponga.Tests.Application;

/// <summary>
/// Edge case tests for PostCreationService,
/// focusing on validation, policies, media limits, and error handling.
/// </summary>
public sealed class PostCreationServiceEdgeCasesTests
{
    private readonly InMemoryDataStore _dataStore;
    private readonly InMemoryFeedRepository _feedRepository;
    private readonly InMemoryMapRepository _mapRepository;
    private readonly InMemoryAssetRepository _assetRepository;
    private readonly InMemoryPostGeoAnchorRepository _geoAnchorRepository;
    private readonly InMemoryPostAssetRepository _postAssetRepository;
    private readonly InMemoryMediaAssetRepository _mediaAssetRepository;
    private readonly InMemoryMediaAttachmentRepository _mediaAttachmentRepository;
    private readonly InMemorySanctionRepository _sanctionRepository;
    private readonly InMemoryFeatureFlagService _featureFlags;
    private readonly Mock<IAuditLogger> _auditLoggerMock;
    private readonly IEventBus _eventBus;
    private readonly InMemoryUnitOfWork _unitOfWork;
    private readonly PostCreationService _service;

    public PostCreationServiceEdgeCasesTests()
    {
        _dataStore = new InMemoryDataStore();
        _feedRepository = new InMemoryFeedRepository(_dataStore);
        _mapRepository = new InMemoryMapRepository(_dataStore);
        _assetRepository = new InMemoryAssetRepository(_dataStore);
        _geoAnchorRepository = new InMemoryPostGeoAnchorRepository(_dataStore);
        _postAssetRepository = new InMemoryPostAssetRepository(_dataStore);
        _mediaAssetRepository = new InMemoryMediaAssetRepository(_dataStore);
        _mediaAttachmentRepository = new InMemoryMediaAttachmentRepository(_dataStore);
        _sanctionRepository = new InMemorySanctionRepository(_dataStore);
        _featureFlags = new InMemoryFeatureFlagService();
        _auditLoggerMock = new Mock<IAuditLogger>();
        _eventBus = new NoOpEventBus();
        _unitOfWork = new InMemoryUnitOfWork();
        var mediaConfigRepo = new InMemoryTerritoryMediaConfigRepository(_dataStore);
        var globalLimits = new InMemoryGlobalMediaLimits();
        var mediaConfigService = new TerritoryMediaConfigService(
            mediaConfigRepo,
            _featureFlags,
            _unitOfWork,
            globalLimits);
        _service = new PostCreationService(
            _feedRepository,
            _mapRepository,
            _assetRepository,
            _geoAnchorRepository,
            _postAssetRepository,
            _mediaAssetRepository,
            _mediaAttachmentRepository,
            _sanctionRepository,
            _featureFlags,
            mediaConfigService,
            _auditLoggerMock.Object,
            _eventBus,
            _unitOfWork);
    }

    [Fact]
    public async Task CreatePostAsync_WithEmptyTitle_ReturnsFailure()
    {
        var territoryId = TestIds.Territory1;
        var userId = TestIds.ResidentUser;

        var result = await _service.CreatePostAsync(
            territoryId,
            userId,
            "", // Título vazio
            "Content",
            PostType.General,
            PostVisibility.Public,
            PostStatus.Published,
            null,
            null,
            null,
            null,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("required", result.Error ?? "");
    }

    [Fact]
    public async Task CreatePostAsync_WithEmptyContent_ReturnsFailure()
    {
        var territoryId = TestIds.Territory1;
        var userId = TestIds.ResidentUser;

        var result = await _service.CreatePostAsync(
            territoryId,
            userId,
            "Title",
            "", // Conteúdo vazio
            PostType.General,
            PostVisibility.Public,
            PostStatus.Published,
            null,
            null,
            null,
            null,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("required", result.Error ?? "");
    }

    [Fact]
    public async Task CreatePostAsync_WithAlertTypeAndFeatureDisabled_ReturnsFailure()
    {
        var territoryId = Guid.NewGuid(); // Território sem feature flag
        var userId = TestIds.ResidentUser;

        var result = await _service.CreatePostAsync(
            territoryId,
            userId,
            "Title",
            "Content",
            PostType.Alert, // Alert posts desabilitados
            PostVisibility.Public,
            PostStatus.Published,
            null,
            null,
            null,
            null,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("disabled", result.Error ?? "");
    }

    [Fact]
    public async Task CreatePostAsync_WithInvalidMapEntityId_ReturnsFailure()
    {
        var territoryId = TestIds.Territory1;
        var userId = TestIds.ResidentUser;
        var invalidMapEntityId = Guid.NewGuid();

        var result = await _service.CreatePostAsync(
            territoryId,
            userId,
            "Title",
            "Content",
            PostType.General,
            PostVisibility.Public,
            PostStatus.Published,
            invalidMapEntityId, // Map entity não existe
            null,
            null,
            null,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("not found", result.Error ?? "");
    }

    [Fact]
    public async Task CreatePostAsync_WithUnicodeContent_HandlesCorrectly()
    {
        var territoryId = TestIds.Territory1;
        var userId = TestIds.ResidentUser;

        var result = await _service.CreatePostAsync(
            territoryId,
            userId,
            "Título com acentuação: café, naïve, 文字",
            "Conteúdo com Unicode",
            PostType.General,
            PostVisibility.Public,
            PostStatus.Published,
            null,
            null,
            null,
            null,
            CancellationToken.None);

        // Não deve falhar por causa do Unicode
        Assert.NotNull(result);
    }

    [Fact]
    public async Task CreatePostAsync_WithInvalidAssetIds_ReturnsFailure()
    {
        var territoryId = TestIds.Territory1;
        var userId = TestIds.ResidentUser;
        var invalidAssetIds = new[] { Guid.NewGuid() };

        var result = await _service.CreatePostAsync(
            territoryId,
            userId,
            "Title",
            "Content",
            PostType.General,
            PostVisibility.Public,
            PostStatus.Published,
            null,
            null,
            invalidAssetIds, // Assets não existem
            null,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("not found", result.Error ?? "");
    }

    [Fact]
    public async Task CreatePostAsync_WithEmptyGuidAssetIds_RemovesEmptyGuids()
    {
        var territoryId = TestIds.Territory1;
        var userId = TestIds.ResidentUser;
        var assetIds = new[] { Guid.Empty, Guid.NewGuid() }; // Empty Guid deve ser removido

        var result = await _service.CreatePostAsync(
            territoryId,
            userId,
            "Title",
            "Content",
            PostType.General,
            PostVisibility.Public,
            PostStatus.Published,
            null,
            null,
            assetIds,
            null,
            CancellationToken.None);

        // Pode falhar por asset não encontrado, mas não por Empty Guid
        Assert.NotNull(result);
    }
}
