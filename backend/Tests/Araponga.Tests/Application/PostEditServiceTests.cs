using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Interfaces.Media;
using Araponga.Application.Services;
using Araponga.Application.Services.Media;
using Araponga.Domain.Feed;
using Araponga.Domain.Media;
using Araponga.Infrastructure.InMemory;
using Araponga.Tests.TestHelpers;
using Xunit;

namespace Araponga.Tests.Application;

/// <summary>
/// Testes TDD para PostEditService seguindo Red-Green-Refactor.
/// </summary>
public sealed class PostEditServiceTests
{
    private readonly InMemoryDataStore _dataStore;
    private readonly InMemoryFeedRepository _feedRepository;
    private readonly InMemoryMediaAttachmentRepository _mediaAttachmentRepository;
    private readonly InMemoryMediaAssetRepository _mediaAssetRepository;
    private readonly InMemoryPostGeoAnchorRepository _postGeoAnchorRepository;
    private readonly InMemoryUnitOfWork _unitOfWork;
    private readonly PostEditService _postEditService;
    private readonly IFeatureFlagService _featureFlags;
    private readonly TerritoryMediaConfigService _mediaConfigService;

    public PostEditServiceTests()
    {
        _dataStore = new InMemoryDataStore();
        _feedRepository = new InMemoryFeedRepository(_dataStore);
        _mediaAttachmentRepository = new InMemoryMediaAttachmentRepository(_dataStore);
        _mediaAssetRepository = new InMemoryMediaAssetRepository(_dataStore);
        _postGeoAnchorRepository = new InMemoryPostGeoAnchorRepository(_dataStore);
        _unitOfWork = new InMemoryUnitOfWork();
        _featureFlags = new InMemoryFeatureFlagService();
        _mediaConfigService = new TerritoryMediaConfigService(
            new InMemoryTerritoryMediaConfigRepository(_dataStore),
            _featureFlags,
            _unitOfWork,
            new InMemoryGlobalMediaLimits());

        _postEditService = new PostEditService(
            _feedRepository,
            _mediaAttachmentRepository,
            _mediaAssetRepository,
            _postGeoAnchorRepository,
            _featureFlags,
            _mediaConfigService,
            _unitOfWork);
    }

    [Fact]
    public async Task EditPostAsync_WhenPostExistsAndUserIsAuthor_ShouldUpdatePost()
    {
        // Arrange
        var territoryId = Guid.NewGuid();
        var authorId = Guid.NewGuid();
        var postId = Guid.NewGuid();

        var post = new CommunityPost(
            postId,
            territoryId,
            authorId,
            "Título Original",
            "Conteúdo Original",
            PostType.General,
            PostVisibility.Public,
            PostStatus.Published,
            null,
            DateTime.UtcNow);

        await _feedRepository.AddPostAsync(post, CancellationToken.None);

        // Act
        var result = await _postEditService.EditPostAsync(
            postId,
            authorId,
            "Novo Título",
            "Novo Conteúdo",
            null,
            null,
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Novo Título", result.Value.Title);
        Assert.Equal("Novo Conteúdo", result.Value.Content);
        Assert.NotNull(result.Value.EditedAtUtc);
        Assert.Equal(1, result.Value.EditCount);
    }

    [Fact]
    public async Task EditPostAsync_WhenUserIsNotAuthor_ShouldReturnUnauthorized()
    {
        // Arrange
        var territoryId = Guid.NewGuid();
        var authorId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var postId = Guid.NewGuid();

        var post = new CommunityPost(
            postId,
            territoryId,
            authorId,
            "Título Original",
            "Conteúdo Original",
            PostType.General,
            PostVisibility.Public,
            PostStatus.Published,
            null,
            DateTime.UtcNow);

        await _feedRepository.AddPostAsync(post, CancellationToken.None);

        // Act
        var result = await _postEditService.EditPostAsync(
            postId,
            otherUserId,
            "Novo Título",
            "Novo Conteúdo",
            null,
            null,
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("author", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task EditPostAsync_WhenPostNotFound_ShouldReturnNotFound()
    {
        // Arrange
        var postId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Act
        var result = await _postEditService.EditPostAsync(
            postId,
            userId,
            "Novo Título",
            "Novo Conteúdo",
            null,
            null,
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task EditPostAsync_WhenTitleIsEmpty_ShouldReturnValidationError()
    {
        // Arrange
        var territoryId = Guid.NewGuid();
        var authorId = Guid.NewGuid();
        var postId = Guid.NewGuid();

        var post = new CommunityPost(
            postId,
            territoryId,
            authorId,
            "Título Original",
            "Conteúdo Original",
            PostType.General,
            PostVisibility.Public,
            PostStatus.Published,
            null,
            DateTime.UtcNow);

        await _feedRepository.AddPostAsync(post, CancellationToken.None);

        // Act
        var result = await _postEditService.EditPostAsync(
            postId,
            authorId,
            "",
            "Novo Conteúdo",
            null,
            null,
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("title", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task EditPostAsync_WhenContentIsEmpty_ShouldReturnValidationError()
    {
        // Arrange
        var territoryId = Guid.NewGuid();
        var authorId = Guid.NewGuid();
        var postId = Guid.NewGuid();

        var post = new CommunityPost(
            postId,
            territoryId,
            authorId,
            "Título Original",
            "Conteúdo Original",
            PostType.General,
            PostVisibility.Public,
            PostStatus.Published,
            null,
            DateTime.UtcNow);

        await _feedRepository.AddPostAsync(post, CancellationToken.None);

        // Act
        var result = await _postEditService.EditPostAsync(
            postId,
            authorId,
            "Novo Título",
            "",
            null,
            null,
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("content", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task EditPostAsync_WhenAddingMedia_ShouldAssociateMediaToPost()
    {
        // Arrange
        var territoryId = Guid.NewGuid();
        var authorId = Guid.NewGuid();
        var postId = Guid.NewGuid();
        var mediaId1 = Guid.NewGuid();
        var mediaId2 = Guid.NewGuid();

        var post = new CommunityPost(
            postId,
            territoryId,
            authorId,
            "Título Original",
            "Conteúdo Original",
            PostType.General,
            PostVisibility.Public,
            PostStatus.Published,
            null,
            DateTime.UtcNow);

        await _feedRepository.AddPostAsync(post, CancellationToken.None);

        // Criar mídias
        var media1 = new MediaAsset(
            mediaId1,
            authorId,
            MediaType.Image,
            "image/jpeg",
            "images/2025/01/test1.jpg",
            1024,
            100,
            100,
            "checksum1",
            DateTime.UtcNow,
            null,
            null);

        var media2 = new MediaAsset(
            mediaId2,
            authorId,
            MediaType.Image,
            "image/jpeg",
            "images/2025/01/test2.jpg",
            2048,
            200,
            200,
            "checksum2",
            DateTime.UtcNow,
            null,
            null);

        await _mediaAssetRepository.AddAsync(media1, CancellationToken.None);
        await _mediaAssetRepository.AddAsync(media2, CancellationToken.None);

        // Act
        var result = await _postEditService.EditPostAsync(
            postId,
            authorId,
            "Novo Título",
            "Novo Conteúdo",
            new[] { mediaId1, mediaId2 },
            null,
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        var attachments = await _mediaAttachmentRepository.ListByOwnerAsync(
            MediaOwnerType.Post,
            postId,
            CancellationToken.None);

        Assert.Equal(2, attachments.Count);
        Assert.Contains(attachments, a => a.MediaAssetId == mediaId1);
        Assert.Contains(attachments, a => a.MediaAssetId == mediaId2);
    }

    [Fact]
    public async Task EditPostAsync_WhenRemovingMedia_ShouldRemoveMediaAttachments()
    {
        // Arrange
        var territoryId = Guid.NewGuid();
        var authorId = Guid.NewGuid();
        var postId = Guid.NewGuid();
        var mediaId1 = Guid.NewGuid();
        var mediaId2 = Guid.NewGuid();

        var post = new CommunityPost(
            postId,
            territoryId,
            authorId,
            "Título Original",
            "Conteúdo Original",
            PostType.General,
            PostVisibility.Public,
            PostStatus.Published,
            null,
            DateTime.UtcNow);

        await _feedRepository.AddPostAsync(post, CancellationToken.None);

        // Criar mídias e attachments existentes
        var media1 = new MediaAsset(
            mediaId1,
            authorId,
            MediaType.Image,
            "image/jpeg",
            "images/2025/01/test1.jpg",
            1024,
            100,
            100,
            "checksum1",
            DateTime.UtcNow,
            null,
            null);

        var media2 = new MediaAsset(
            mediaId2,
            authorId,
            MediaType.Image,
            "image/jpeg",
            "images/2025/01/test2.jpg",
            2048,
            200,
            200,
            "checksum2",
            DateTime.UtcNow,
            null,
            null);

        await _mediaAssetRepository.AddAsync(media1, CancellationToken.None);
        await _mediaAssetRepository.AddAsync(media2, CancellationToken.None);

        var attachment1 = new MediaAttachment(
            Guid.NewGuid(),
            mediaId1,
            MediaOwnerType.Post,
            postId,
            0,
            DateTime.UtcNow);

        var attachment2 = new MediaAttachment(
            Guid.NewGuid(),
            mediaId2,
            MediaOwnerType.Post,
            postId,
            1,
            DateTime.UtcNow);

        await _mediaAttachmentRepository.AddAsync(attachment1, CancellationToken.None);
        await _mediaAttachmentRepository.AddAsync(attachment2, CancellationToken.None);

        // Act - Remover todas as mídias
        var result = await _postEditService.EditPostAsync(
            postId,
            authorId,
            "Novo Título",
            "Novo Conteúdo",
            Array.Empty<Guid>(),
            null,
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        var attachments = await _mediaAttachmentRepository.ListByOwnerAsync(
            MediaOwnerType.Post,
            postId,
            CancellationToken.None);

        Assert.Empty(attachments);
    }

    [Fact]
    public async Task EditPostAsync_WhenReplacingMedia_ShouldUpdateMediaAttachments()
    {
        // Arrange
        var territoryId = Guid.NewGuid();
        var authorId = Guid.NewGuid();
        var postId = Guid.NewGuid();
        var oldMediaId = Guid.NewGuid();
        var newMediaId = Guid.NewGuid();

        var post = new CommunityPost(
            postId,
            territoryId,
            authorId,
            "Título Original",
            "Conteúdo Original",
            PostType.General,
            PostVisibility.Public,
            PostStatus.Published,
            null,
            DateTime.UtcNow);

        await _feedRepository.AddPostAsync(post, CancellationToken.None);

        // Criar mídias
        var oldMedia = new MediaAsset(
            oldMediaId,
            authorId,
            MediaType.Image,
            "image/jpeg",
            "images/2025/01/old.jpg",
            1024,
            100,
            100,
            "checksum_old",
            DateTime.UtcNow,
            null,
            null);

        var newMedia = new MediaAsset(
            newMediaId,
            authorId,
            MediaType.Image,
            "image/jpeg",
            "images/2025/01/new.jpg",
            2048,
            200,
            200,
            "checksum_new",
            DateTime.UtcNow,
            null,
            null);

        await _mediaAssetRepository.AddAsync(oldMedia, CancellationToken.None);
        await _mediaAssetRepository.AddAsync(newMedia, CancellationToken.None);

        // Attachment existente
        var oldAttachment = new MediaAttachment(
            Guid.NewGuid(),
            oldMediaId,
            MediaOwnerType.Post,
            postId,
            0,
            DateTime.UtcNow);

        await _mediaAttachmentRepository.AddAsync(oldAttachment, CancellationToken.None);

        // Act - Substituir mídia
        var result = await _postEditService.EditPostAsync(
            postId,
            authorId,
            "Novo Título",
            "Novo Conteúdo",
            new[] { newMediaId },
            null,
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        var attachments = await _mediaAttachmentRepository.ListByOwnerAsync(
            MediaOwnerType.Post,
            postId,
            CancellationToken.None);

        Assert.Single(attachments);
        Assert.Equal(newMediaId, attachments[0].MediaAssetId);
    }

    [Fact]
    public async Task EditPostAsync_WhenEditingMultipleTimes_ShouldIncrementEditCount()
    {
        // Arrange
        var territoryId = Guid.NewGuid();
        var authorId = Guid.NewGuid();
        var postId = Guid.NewGuid();

        var post = new CommunityPost(
            postId,
            territoryId,
            authorId,
            "Título Original",
            "Conteúdo Original",
            PostType.General,
            PostVisibility.Public,
            PostStatus.Published,
            null,
            DateTime.UtcNow);

        await _feedRepository.AddPostAsync(post, CancellationToken.None);

        // Act - Primeira edição
        var result1 = await _postEditService.EditPostAsync(
            postId,
            authorId,
            "Título 1",
            "Conteúdo 1",
            null,
            null,
            CancellationToken.None);

        // Segunda edição
        var result2 = await _postEditService.EditPostAsync(
            postId,
            authorId,
            "Título 2",
            "Conteúdo 2",
            null,
            null,
            CancellationToken.None);

        // Terceira edição
        var result3 = await _postEditService.EditPostAsync(
            postId,
            authorId,
            "Título 3",
            "Conteúdo 3",
            null,
            null,
            CancellationToken.None);

        // Assert
        Assert.True(result1.IsSuccess);
        Assert.True(result2.IsSuccess);
        Assert.True(result3.IsSuccess);

        var updatedPost = await _feedRepository.GetPostAsync(postId, CancellationToken.None);
        Assert.NotNull(updatedPost);
        Assert.Equal(3, updatedPost.EditCount);
        Assert.NotNull(updatedPost.EditedAtUtc);
    }
}
