using Araponga.Application.Interfaces;
using Araponga.Application.Interfaces.Media;
using Araponga.Application.Services;
using Araponga.Domain.Media;
using Araponga.Infrastructure.InMemory;
using Araponga.Tests.TestHelpers;
using Moq;
using Xunit;

namespace Araponga.Tests.Application;

/// <summary>
/// Edge case tests for MediaService,
/// focusing on stream validation, Unicode filenames, large files, and error handling.
/// </summary>
public class MediaServiceEdgeCasesTests
{
    private readonly InMemoryDataStore _dataStore;
    private readonly Mock<IMediaStorageService> _storageServiceMock;
    private readonly Mock<IMediaProcessingService> _processingServiceMock;
    private readonly Mock<IMediaValidator> _validatorMock;
    private readonly IAuditLogger _auditLogger;
    private readonly IUnitOfWork _unitOfWork;

    public MediaServiceEdgeCasesTests()
    {
        _dataStore = new InMemoryDataStore();
        _storageServiceMock = new Mock<IMediaStorageService>();
        _processingServiceMock = new Mock<IMediaProcessingService>();
        _validatorMock = new Mock<IMediaValidator>();
        _auditLogger = new InMemoryAuditLogger(_dataStore);
        _unitOfWork = new InMemoryUnitOfWork();
    }

    [Fact]
    public async Task UploadMediaAsync_WithUnreadableStream_ReturnsFailure()
    {
        var mediaAssetRepository = new InMemoryMediaAssetRepository(_dataStore);
        var mediaAttachmentRepository = new InMemoryMediaAttachmentRepository(_dataStore);
        var mediaService = new MediaService(
            mediaAssetRepository,
            mediaAttachmentRepository,
            _storageServiceMock.Object,
            _processingServiceMock.Object,
            _validatorMock.Object,
            _auditLogger,
            _unitOfWork);

        var unreadableStream = new Mock<Stream>();
        unreadableStream.Setup(s => s.CanRead).Returns(false);

        var result = await mediaService.UploadMediaAsync(
            unreadableStream.Object,
            "image/jpeg",
            "test.jpg",
            Guid.NewGuid(),
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Stream inválido", result.Error ?? "");
    }

    [Fact]
    public async Task UploadMediaAsync_WithUnicodeFileName_HandlesCorrectly()
    {
        var mediaAssetRepository = new InMemoryMediaAssetRepository(_dataStore);
        var mediaAttachmentRepository = new InMemoryMediaAttachmentRepository(_dataStore);
        
        _storageServiceMock.Setup(s => s.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("images/café-naïve-文字.jpg");
        
        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(MediaValidationResult.Success());
        
        // Mock do processamento de imagem
        _processingServiceMock.Setup(p => p.GetImageDimensionsAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((1920, 1080));
        
        _processingServiceMock.Setup(p => p.OptimizeImageAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns<Stream, string, CancellationToken>((s, mime, ct) => Task.FromResult<Stream>(new MemoryStream(new byte[] { 1, 2, 3, 4 })));

        var mediaService = new MediaService(
            mediaAssetRepository,
            mediaAttachmentRepository,
            _storageServiceMock.Object,
            _processingServiceMock.Object,
            _validatorMock.Object,
            _auditLogger,
            _unitOfWork);

        var stream = new MemoryStream(new byte[] { 1, 2, 3, 4 });
        var result = await mediaService.UploadMediaAsync(
            stream,
            "image/jpeg",
            "café-naïve-文字.jpg",
            Guid.NewGuid(),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        _storageServiceMock.Verify(s => s.UploadAsync(
            It.IsAny<Stream>(),
            It.IsAny<string>(),
            "café-naïve-文字.jpg",
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UploadMediaAsync_WithEmptyUserId_ReturnsFailure()
    {
        var mediaAssetRepository = new InMemoryMediaAssetRepository(_dataStore);
        var mediaAttachmentRepository = new InMemoryMediaAttachmentRepository(_dataStore);
        var mediaService = new MediaService(
            mediaAssetRepository,
            mediaAttachmentRepository,
            _storageServiceMock.Object,
            _processingServiceMock.Object,
            _validatorMock.Object,
            _auditLogger,
            _unitOfWork);

        var stream = new MemoryStream(new byte[] { 1, 2, 3, 4 });
        var result = await mediaService.UploadMediaAsync(
            stream,
            "image/jpeg",
            "test.jpg",
            Guid.Empty,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("ID do usuário é obrigatório", result.Error ?? "");
    }

    [Fact]
    public async Task UploadMediaAsync_WithNullFileName_ReturnsFailure()
    {
        var mediaAssetRepository = new InMemoryMediaAssetRepository(_dataStore);
        var mediaAttachmentRepository = new InMemoryMediaAttachmentRepository(_dataStore);
        var mediaService = new MediaService(
            mediaAssetRepository,
            mediaAttachmentRepository,
            _storageServiceMock.Object,
            _processingServiceMock.Object,
            _validatorMock.Object,
            _auditLogger,
            _unitOfWork);

        var stream = new MemoryStream(new byte[] { 1, 2, 3, 4 });
        var result = await mediaService.UploadMediaAsync(
            stream,
            "image/jpeg",
            null!,
            Guid.NewGuid(),
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Nome do arquivo é obrigatório", result.Error ?? "");
    }

    [Fact]
    public async Task UploadMediaAsync_WithEmptyFileName_ReturnsFailure()
    {
        var mediaAssetRepository = new InMemoryMediaAssetRepository(_dataStore);
        var mediaAttachmentRepository = new InMemoryMediaAttachmentRepository(_dataStore);
        var mediaService = new MediaService(
            mediaAssetRepository,
            mediaAttachmentRepository,
            _storageServiceMock.Object,
            _processingServiceMock.Object,
            _validatorMock.Object,
            _auditLogger,
            _unitOfWork);

        var stream = new MemoryStream(new byte[] { 1, 2, 3, 4 });
        var result = await mediaService.UploadMediaAsync(
            stream,
            "image/jpeg",
            "   ",
            Guid.NewGuid(),
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Nome do arquivo é obrigatório", result.Error ?? "");
    }

    [Fact]
    public async Task AttachMediaToOwnerAsync_WithEmptyMediaAssetId_ReturnsFailure()
    {
        var mediaAssetRepository = new InMemoryMediaAssetRepository(_dataStore);
        var mediaAttachmentRepository = new InMemoryMediaAttachmentRepository(_dataStore);
        var mediaService = new MediaService(
            mediaAssetRepository,
            mediaAttachmentRepository,
            _storageServiceMock.Object,
            _processingServiceMock.Object,
            _validatorMock.Object,
            _auditLogger,
            _unitOfWork);

        var result = await mediaService.AttachMediaToOwnerAsync(
            Guid.Empty,
            MediaOwnerType.Post,
            Guid.NewGuid(),
            null,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("ID da mídia é obrigatório", result.Error ?? "");
    }

    [Fact]
    public async Task AttachMediaToOwnerAsync_WithEmptyOwnerId_ReturnsFailure()
    {
        var mediaAssetRepository = new InMemoryMediaAssetRepository(_dataStore);
        var mediaAttachmentRepository = new InMemoryMediaAttachmentRepository(_dataStore);
        var mediaService = new MediaService(
            mediaAssetRepository,
            mediaAttachmentRepository,
            _storageServiceMock.Object,
            _processingServiceMock.Object,
            _validatorMock.Object,
            _auditLogger,
            _unitOfWork);

        var result = await mediaService.AttachMediaToOwnerAsync(
            Guid.NewGuid(),
            MediaOwnerType.Post,
            Guid.Empty,
            null,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("ID do proprietário é obrigatório", result.Error ?? "");
    }

    [Fact]
    public async Task AttachMediaToOwnerAsync_WithNonExistentMedia_ReturnsFailure()
    {
        var mediaAssetRepository = new InMemoryMediaAssetRepository(_dataStore);
        var mediaAttachmentRepository = new InMemoryMediaAttachmentRepository(_dataStore);
        var mediaService = new MediaService(
            mediaAssetRepository,
            mediaAttachmentRepository,
            _storageServiceMock.Object,
            _processingServiceMock.Object,
            _validatorMock.Object,
            _auditLogger,
            _unitOfWork);

        var result = await mediaService.AttachMediaToOwnerAsync(
            Guid.NewGuid(),
            MediaOwnerType.Post,
            Guid.NewGuid(),
            null,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Mídia não encontrada", result.Error ?? "");
    }

    [Fact]
    public async Task AttachMediaToOwnerAsync_WithDeletedMedia_ReturnsFailure()
    {
        var mediaAssetRepository = new InMemoryMediaAssetRepository(_dataStore);
        var mediaAttachmentRepository = new InMemoryMediaAttachmentRepository(_dataStore);
        
        var userId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var mediaAsset = new MediaAsset(
            Guid.NewGuid(),
            userId,
            MediaType.Image,
            "image/jpeg",
            "images/test.jpg",
            1024,
            1920,
            1080,
            "abc123",
            now,
            null, // deletedByUserId - não deletado ainda
            null); // deletedAtUtc - não deletado ainda
        
        await mediaAssetRepository.AddAsync(mediaAsset, CancellationToken.None);
        
        // Deletar a mídia antes de testar
        mediaAsset.Delete(userId, now);
        await mediaAssetRepository.UpdateAsync(mediaAsset, CancellationToken.None);

        var mediaService = new MediaService(
            mediaAssetRepository,
            mediaAttachmentRepository,
            _storageServiceMock.Object,
            _processingServiceMock.Object,
            _validatorMock.Object,
            _auditLogger,
            _unitOfWork);

        var result = await mediaService.AttachMediaToOwnerAsync(
            mediaAsset.Id,
            MediaOwnerType.Post,
            Guid.NewGuid(),
            null,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        // O repositório in-memory filtra mídias deletadas, então pode retornar "Mídia não encontrada"
        Assert.True(
            (result.Error ?? "").Contains("Não é possível associar uma mídia deletada") ||
            (result.Error ?? "").Contains("Mídia não encontrada"));
    }

    [Fact]
    public async Task GetMediaUrlAsync_WithNonExistentMedia_ReturnsFailure()
    {
        var mediaAssetRepository = new InMemoryMediaAssetRepository(_dataStore);
        var mediaAttachmentRepository = new InMemoryMediaAttachmentRepository(_dataStore);
        var mediaService = new MediaService(
            mediaAssetRepository,
            mediaAttachmentRepository,
            _storageServiceMock.Object,
            _processingServiceMock.Object,
            _validatorMock.Object,
            _auditLogger,
            _unitOfWork);

        var result = await mediaService.GetMediaUrlAsync(
            Guid.NewGuid(),
            null,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Mídia não encontrada", result.Error ?? "");
    }

    [Fact]
    public async Task GetMediaUrlAsync_WithDeletedMedia_ReturnsFailure()
    {
        var mediaAssetRepository = new InMemoryMediaAssetRepository(_dataStore);
        var mediaAttachmentRepository = new InMemoryMediaAttachmentRepository(_dataStore);
        
        var userId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var mediaAsset = new MediaAsset(
            Guid.NewGuid(),
            userId,
            MediaType.Image,
            "image/jpeg",
            "images/test.jpg",
            1024,
            1920,
            1080,
            "abc123",
            now,
            null, // deletedByUserId - não deletado ainda
            null); // deletedAtUtc - não deletado ainda
        
        await mediaAssetRepository.AddAsync(mediaAsset, CancellationToken.None);
        
        // Deletar a mídia antes de testar
        mediaAsset.Delete(userId, now);
        await mediaAssetRepository.UpdateAsync(mediaAsset, CancellationToken.None);

        var mediaService = new MediaService(
            mediaAssetRepository,
            mediaAttachmentRepository,
            _storageServiceMock.Object,
            _processingServiceMock.Object,
            _validatorMock.Object,
            _auditLogger,
            _unitOfWork);

        var result = await mediaService.GetMediaUrlAsync(
            mediaAsset.Id,
            null,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        // O repositório in-memory filtra mídias deletadas, então retorna "Mídia não encontrada"
        Assert.True(
            (result.Error ?? "").Contains("Mídia foi deletada") ||
            (result.Error ?? "").Contains("Mídia não encontrada"));
    }

    [Fact]
    public async Task DeleteMediaAsync_WithNonExistentMedia_ReturnsFailure()
    {
        var mediaAssetRepository = new InMemoryMediaAssetRepository(_dataStore);
        var mediaAttachmentRepository = new InMemoryMediaAttachmentRepository(_dataStore);
        var mediaService = new MediaService(
            mediaAssetRepository,
            mediaAttachmentRepository,
            _storageServiceMock.Object,
            _processingServiceMock.Object,
            _validatorMock.Object,
            _auditLogger,
            _unitOfWork);

        var result = await mediaService.DeleteMediaAsync(
            Guid.NewGuid(),
            Guid.NewGuid(),
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Mídia não encontrada", result.Error ?? "");
    }

    [Fact]
    public async Task DeleteMediaAsync_WithAlreadyDeletedMedia_ReturnsFailure()
    {
        var mediaAssetRepository = new InMemoryMediaAssetRepository(_dataStore);
        var mediaAttachmentRepository = new InMemoryMediaAttachmentRepository(_dataStore);
        
        var userId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var mediaAsset = new MediaAsset(
            Guid.NewGuid(),
            userId,
            MediaType.Image,
            "image/jpeg",
            "images/test.jpg",
            1024,
            1920,
            1080,
            "abc123",
            now,
            null, // deletedByUserId - não deletado ainda
            null); // deletedAtUtc - não deletado ainda
        
        await mediaAssetRepository.AddAsync(mediaAsset, CancellationToken.None);

        var mediaService = new MediaService(
            mediaAssetRepository,
            mediaAttachmentRepository,
            _storageServiceMock.Object,
            _processingServiceMock.Object,
            _validatorMock.Object,
            _auditLogger,
            _unitOfWork);

        // Primeiro deletar a mídia através do serviço
        var deleteResult = await mediaService.DeleteMediaAsync(
            mediaAsset.Id,
            userId,
            CancellationToken.None);
        
        Assert.True(deleteResult.IsSuccess); // Primeira deleção deve ser bem-sucedida

        // Agora tentar deletar novamente
        var result = await mediaService.DeleteMediaAsync(
            mediaAsset.Id,
            userId,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        // Pode retornar "Mídia já foi deletada" ou "Mídia não encontrada" dependendo da implementação
        Assert.True(
            (result.Error ?? "").Contains("Mídia já foi deletada") ||
            (result.Error ?? "").Contains("Mídia não encontrada"));
    }
}
