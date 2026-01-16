using Araponga.Application.Interfaces;
using Araponga.Application.Interfaces.Media;
using Araponga.Application.Services;
using Araponga.Domain.Media;
using Araponga.Infrastructure.InMemory;
using Araponga.Tests.TestHelpers;
using Moq;
using Xunit;

namespace Araponga.Tests.Application;

public sealed class MediaServiceTests
{
    private readonly InMemoryDataStore _dataStore;
    private readonly Mock<IMediaStorageService> _storageServiceMock;
    private readonly Mock<IMediaProcessingService> _processingServiceMock;
    private readonly Mock<IMediaValidator> _validatorMock;
    private readonly IAuditLogger _auditLogger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly MediaService _mediaService;

    public MediaServiceTests()
    {
        _dataStore = new InMemoryDataStore();
        _storageServiceMock = new Mock<IMediaStorageService>();
        _processingServiceMock = new Mock<IMediaProcessingService>();
        _validatorMock = new Mock<IMediaValidator>();

        // Criar repositórios InMemory (precisa ser implementado, mas por enquanto usaremos mocks)
        // Por enquanto, vamos criar um mock do repositório também
        var mediaAssetRepositoryMock = new Mock<IMediaAssetRepository>();
        var mediaAttachmentRepositoryMock = new Mock<IMediaAttachmentRepository>();
        
        _auditLogger = new InMemoryAuditLogger(_dataStore);
        _unitOfWork = new InMemoryUnitOfWork();

        // Configurar validator para aceitar arquivos válidos
        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(MediaValidationResult.Success());

        _mediaService = new MediaService(
            mediaAssetRepositoryMock.Object,
            mediaAttachmentRepositoryMock.Object,
            _storageServiceMock.Object,
            _processingServiceMock.Object,
            _validatorMock.Object,
            _auditLogger,
            _unitOfWork);
    }

    [Fact]
    public async Task UploadMedia_WithValidImage_Success()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var fileStream = new MemoryStream(new byte[] { 1, 2, 3, 4 });
        var mimeType = "image/jpeg";
        var fileName = "test.jpg";
        var storageKey = "images/2025/01/test.jpg";

        _storageServiceMock.Setup(s => s.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(storageKey);

        // Configurar dimensões para imagem
        _processingServiceMock.Setup(p => p.GetImageDimensionsAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((100, 200));

        // Configurar otimização de imagem
        _processingServiceMock.Setup(p => p.OptimizeImageAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Stream stream, string mimeType, CancellationToken ct) =>
            {
                // Retornar um novo stream com os mesmos dados para simular otimização
                var optimizedStream = new MemoryStream();
                stream.Position = 0;
                stream.CopyTo(optimizedStream);
                optimizedStream.Position = 0;
                return optimizedStream;
            });

        // Configurar o mock do repositório para aceitar o MediaAsset
        var mediaAssetRepositoryMock = new Mock<IMediaAssetRepository>();
        mediaAssetRepositoryMock.Setup(r => r.AddAsync(It.IsAny<MediaAsset>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Recriar o MediaService com o mock do repositório configurado
        var mediaService = new MediaService(
            mediaAssetRepositoryMock.Object,
            new Mock<IMediaAttachmentRepository>().Object,
            _storageServiceMock.Object,
            _processingServiceMock.Object,
            _validatorMock.Object,
            _auditLogger,
            _unitOfWork);

        // Act
        var result = await mediaService.UploadMediaAsync(fileStream, mimeType, fileName, userId, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        _storageServiceMock.Verify(s => s.UploadAsync(It.IsAny<Stream>(), mimeType, fileName, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UploadMedia_WithInvalidStream_Fails()
    {
        // Arrange
        var userId = Guid.NewGuid();
        Stream? nullStream = null;

        // Act
        var result = await _mediaService.UploadMediaAsync(nullStream!, "image/jpeg", "test.jpg", userId, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("Stream inválido", result.Error!);
    }

    [Fact]
    public async Task UploadMedia_WithInvalidMimeType_Fails()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var fileStream = new MemoryStream(new byte[] { 1, 2, 3, 4 });

        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(MediaValidationResult.Failure("Tipo MIME não é permitido."));

        // Act
        var result = await _mediaService.UploadMediaAsync(fileStream, "", "test.jpg", userId, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task DeleteMedia_WithValidMediaAndOwner_Success()
    {
        // Arrange
        var mediaAssetId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var mediaAsset = new MediaAsset(
            mediaAssetId,
            userId,
            MediaType.Image,
            "image/jpeg",
            "images/test.jpg",
            1024,
            1920,
            1080,
            "abc123",
            DateTime.UtcNow,
            null,
            null);

        var mediaAssetRepositoryMock = new Mock<IMediaAssetRepository>();
        mediaAssetRepositoryMock.Setup(r => r.GetByIdAsync(mediaAssetId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mediaAsset);

        var mediaService = new MediaService(
            mediaAssetRepositoryMock.Object,
            new Mock<IMediaAttachmentRepository>().Object,
            _storageServiceMock.Object,
            _processingServiceMock.Object,
            _validatorMock.Object,
            _auditLogger,
            _unitOfWork);

        // Act
        var result = await mediaService.DeleteMediaAsync(mediaAssetId, userId, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        mediaAssetRepositoryMock.Verify(r => r.UpdateAsync(It.Is<MediaAsset>(m => m.IsDeleted), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteMedia_WithNonOwner_Fails()
    {
        // Arrange
        var mediaAssetId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var mediaAsset = new MediaAsset(
            mediaAssetId,
            ownerId,
            MediaType.Image,
            "image/jpeg",
            "images/test.jpg",
            1024,
            1920,
            1080,
            "abc123",
            DateTime.UtcNow,
            null,
            null);

        var mediaAssetRepositoryMock = new Mock<IMediaAssetRepository>();
        mediaAssetRepositoryMock.Setup(r => r.GetByIdAsync(mediaAssetId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mediaAsset);

        var mediaService = new MediaService(
            mediaAssetRepositoryMock.Object,
            new Mock<IMediaAttachmentRepository>().Object,
            _storageServiceMock.Object,
            _processingServiceMock.Object,
            _validatorMock.Object,
            _auditLogger,
            _unitOfWork);

        // Act
        var result = await mediaService.DeleteMediaAsync(mediaAssetId, otherUserId, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("apenas o criador", result.Error!, StringComparison.OrdinalIgnoreCase);
    }
}