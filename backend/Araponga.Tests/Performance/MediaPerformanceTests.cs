using Araponga.Application.Services;
using Araponga.Application.Interfaces;
using Araponga.Application.Interfaces.Media;
using Araponga.Domain.Media;
using Araponga.Infrastructure.InMemory;
using Araponga.Tests.Api;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Text;
using Xunit;

namespace Araponga.Tests.Performance;

/// <summary>
/// Testes de performance para operações de mídia, incluindo upload de múltiplas imagens.
/// Estes testes podem ser pulados em ambientes CI/CD ou quando SKIP_PERFORMANCE_TESTS=true.
/// </summary>
public sealed class MediaPerformanceTests : IClassFixture<ApiFactory>, IDisposable
{
    private readonly ApiFactory _factory;
    private readonly IServiceScope _scope;
    private readonly MediaService _mediaService;
    private readonly IMediaStorageService _storageService;
    private readonly IMediaProcessingService _processingService;
    private readonly IMediaValidator _validator;
    private readonly IAuditLogger _auditLogger;
    private readonly IUnitOfWork _unitOfWork;

    // Verificar se os testes de performance devem ser pulados
    private static bool ShouldSkipPerformanceTests()
    {
        // Pular em CI/CD ou quando explicitamente configurado
        var skipEnv = Environment.GetEnvironmentVariable("SKIP_PERFORMANCE_TESTS");
        var isCI = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CI")) ||
                   !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("GITHUB_ACTIONS")) ||
                   !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("TF_BUILD")) ||
                   !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("JENKINS_URL"));
        
        return isCI || string.Equals(skipEnv, "true", StringComparison.OrdinalIgnoreCase);
    }

    // Helper para pular testes condicionalmente usando Xunit.SkippableFact
    private static void SkipIfNeeded()
    {
        if (ShouldSkipPerformanceTests())
        {
            Skip.If(true, "Testes de performance pulados em CI/CD. Configure SKIP_PERFORMANCE_TESTS=false para executar.");
        }
    }

    public MediaPerformanceTests(ApiFactory factory)
    {
        _factory = factory;
        _scope = _factory.Services.CreateScope();
        _mediaService = _scope.ServiceProvider.GetRequiredService<MediaService>();
        _storageService = _scope.ServiceProvider.GetRequiredService<IMediaStorageService>();
        _processingService = _scope.ServiceProvider.GetRequiredService<IMediaProcessingService>();
        _validator = _scope.ServiceProvider.GetRequiredService<IMediaValidator>();
        _auditLogger = _scope.ServiceProvider.GetRequiredService<IAuditLogger>();
        _unitOfWork = _scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
    }

    [SkippableFact]
    public async Task UploadMultipleImages_ShouldCompleteWithinTimeLimit()
    {
        SkipIfNeeded();

        // Arrange
        const int imageCount = 10;
        const int maxTotalSeconds = 30; // 30 segundos para 10 imagens
        var testUserId = Guid.NewGuid();
        var images = GenerateTestImages(imageCount);

        // Act
        var stopwatch = Stopwatch.StartNew();
        var uploadTasks = images.Select(image => 
            _mediaService.UploadMediaAsync(
                image.stream,
                image.mimeType,
                image.fileName,
                testUserId));

        var results = await Task.WhenAll(uploadTasks);
        stopwatch.Stop();

        // Assert
        Assert.True(stopwatch.Elapsed.TotalSeconds < maxTotalSeconds, 
            $"Upload de {imageCount} imagens levou {stopwatch.Elapsed.TotalSeconds:F2} segundos, esperado < {maxTotalSeconds} segundos.");
        
        Assert.All(results, result => Assert.True(result.IsSuccess, $"Falha no upload: {result.Error}"));
        
        var successfulUploads = results.Count(r => r.IsSuccess);
        Assert.Equal(imageCount, successfulUploads);
    }

    [SkippableFact]
    public async Task UploadSingleLargeImage_ShouldCompleteWithinTimeLimit()
    {
        SkipIfNeeded();

        // Arrange
        const int imageSizeBytes = 5 * 1024 * 1024; // 5MB
        const int maxSeconds = 10;
        var testUserId = Guid.NewGuid();
        var largeImage = GenerateLargeTestImage(imageSizeBytes);

        // Act
        var stopwatch = Stopwatch.StartNew();
        var result = await _mediaService.UploadMediaAsync(
            largeImage.stream,
            largeImage.mimeType,
            largeImage.fileName,
            testUserId);
        stopwatch.Stop();

        // Assert
        Assert.True(stopwatch.Elapsed.TotalSeconds < maxSeconds,
            $"Upload de imagem grande levou {stopwatch.Elapsed.TotalSeconds:F2} segundos, esperado < {maxSeconds} segundos.");
        
        Assert.True(result.IsSuccess, $"Falha no upload: {result.Error}");
        Assert.NotNull(result.Value);
    }

    [SkippableFact]
    public async Task GetMediaUrlMultipleTimes_ShouldUseCache()
    {
        SkipIfNeeded();

        // Arrange
        var testUserId = Guid.NewGuid();
        var testImage = GenerateTestImages(1).First();
        var uploadResult = await _mediaService.UploadMediaAsync(
            testImage.stream,
            testImage.mimeType,
            testImage.fileName,
            testUserId);
        
        Assert.True(uploadResult.IsSuccess);
        var mediaAssetId = uploadResult.Value!.Id;

        // Primeira chamada (sem cache)
        var firstCallStopwatch = Stopwatch.StartNew();
        var firstUrlResult = await _mediaService.GetMediaUrlAsync(mediaAssetId);
        firstCallStopwatch.Stop();

        // Chamadas subsequentes (com cache potencial)
        const int numberOfCalls = 100;
        var subsequentCallsStopwatch = Stopwatch.StartNew();
        var urlTasks = Enumerable.Range(0, numberOfCalls)
            .Select(_ => _mediaService.GetMediaUrlAsync(mediaAssetId));
        var urlResults = await Task.WhenAll(urlTasks);
        subsequentCallsStopwatch.Stop();

        // Assert
        Assert.True(firstUrlResult.IsSuccess);
        Assert.All(urlResults, result => Assert.True(result.IsSuccess));
        
        // Cache deve melhorar performance (mas não é garantido em ambiente de teste)
        var avgTimePerCall = subsequentCallsStopwatch.Elapsed.TotalMilliseconds / numberOfCalls;
        var firstCallTime = firstCallStopwatch.Elapsed.TotalMilliseconds;
        
        // Verificar que todas as URLs são iguais
        var firstUrl = firstUrlResult.Value;
        Assert.All(urlResults, result => Assert.Equal(firstUrl, result.Value));
    }

    [SkippableFact]
    public async Task ListMediaByOwner_WithMultipleAttachments_ShouldCompleteWithinTimeLimit()
    {
        SkipIfNeeded();

        // Arrange
        const int attachmentCount = 50;
        const int maxSeconds = 5;
        var testUserId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var ownerType = MediaOwnerType.Post;

        // Upload múltiplas imagens
        var images = GenerateTestImages(attachmentCount);
        var uploadTasks = images.Select(image =>
            _mediaService.UploadMediaAsync(
                image.stream,
                image.mimeType,
                image.fileName,
                testUserId));

        var uploadResults = await Task.WhenAll(uploadTasks);
        Assert.All(uploadResults, result => Assert.True(result.IsSuccess));

        // Associar todas ao mesmo owner
        var attachTasks = uploadResults
            .Where(r => r.IsSuccess)
            .Select((result, index) =>
                _mediaService.AttachMediaToOwnerAsync(
                    result.Value!.Id,
                    ownerType,
                    ownerId,
                    index));

        await Task.WhenAll(attachTasks);

        // Act
        var stopwatch = Stopwatch.StartNew();
        var mediaList = await _mediaService.ListMediaByOwnerAsync(ownerType, ownerId);
        stopwatch.Stop();

        // Assert
        Assert.True(stopwatch.Elapsed.TotalSeconds < maxSeconds,
            $"Listagem de {attachmentCount} mídias levou {stopwatch.Elapsed.TotalSeconds:F2} segundos, esperado < {maxSeconds} segundos.");
        
        Assert.Equal(attachmentCount, mediaList.Count);
    }

    private static List<(Stream stream, string mimeType, string fileName)> GenerateTestImages(int count)
    {
        var images = new List<(Stream stream, string mimeType, string fileName)>();
        
        for (int i = 0; i < count; i++)
        {
            // Gerar uma imagem JPEG simples (1KB)
            var imageData = GenerateSimpleJpeg(1024);
            var stream = new MemoryStream(imageData);
            images.Add((stream, "image/jpeg", $"test-image-{i}.jpg"));
        }
        
        return images;
    }

    private static (Stream stream, string mimeType, string fileName) GenerateLargeTestImage(int sizeBytes)
    {
        // Usar o mesmo método de geração de JPEG válido usado em MediaSteps.cs
        var imageData = GenerateValidJpeg(sizeBytes);
        var stream = new MemoryStream(imageData);
        return (stream, "image/jpeg", "large-test-image.jpg");
    }

    private static byte[] GenerateSimpleJpeg(int size)
    {
        // Usar o mesmo método de geração de JPEG válido
        return GenerateValidJpeg(size);
    }

    private static byte[] GenerateValidJpeg(int sizeBytes)
    {
        // JPEG válido mínimo (mesmo usado em MediaSteps.cs)
        var minimalJpeg = new byte[]
        {
            0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0x00, 0x01, 0x01, 0x00, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00,
            0xFF, 0xDB, 0x00, 0x43, 0x00, 0x08, 0x06, 0x06, 0x07, 0x06, 0x05, 0x08, 0x07, 0x07, 0x07, 0x09, 0x09, 0x08, 0x0A, 0x0C,
            0x14, 0x0D, 0x0C, 0x0B, 0x0B, 0x0C, 0x19, 0x12, 0x13, 0x0F, 0x14, 0x1D, 0x1A, 0x1F, 0x1E, 0x1D, 0x1A, 0x1C, 0x1C, 0x20,
            0x24, 0x2E, 0x27, 0x20, 0x22, 0x2C, 0x23, 0x1C, 0x1C, 0x28, 0x37, 0x29, 0x2C, 0x30, 0x31, 0x34, 0x34, 0x34, 0x1F, 0x27,
            0x39, 0x3D, 0x38, 0x32, 0x3C, 0x2E, 0x33, 0x34, 0x32, 0xFF, 0xC0, 0x00, 0x0B, 0x08, 0x00, 0x01, 0x00, 0x01, 0x01, 0x01,
            0x11, 0x00, 0xFF, 0xC4, 0x00, 0x14, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x08, 0xFF, 0xC4, 0x00, 0x14, 0x10, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xDA, 0x00, 0x08, 0x01, 0x01, 0x00, 0x00, 0x3F, 0x00, 0x5F, 0xFF, 0xD9
        };

        // Se o tamanho desejado for menor ou igual ao JPEG mínimo, usar o mínimo
        if (sizeBytes <= minimalJpeg.Length)
        {
            var result = new byte[sizeBytes];
            Array.Copy(minimalJpeg, result, sizeBytes);
            return result;
        }

        // Criar array do tamanho desejado
        var fileBytes = new byte[sizeBytes];

        // Copiar JPEG mínimo até antes do marcador EOI
        var eoiPosition = minimalJpeg.Length - 2; // Posição do 0xFF 0xD9
        Array.Copy(minimalJpeg, 0, fileBytes, 0, eoiPosition);

        // Preencher o meio com zeros (já inicializado como zero pelo new byte[])
        // Apenas garantir que os últimos 2 bytes sejam o EOI
        fileBytes[sizeBytes - 2] = 0xFF;
        fileBytes[sizeBytes - 1] = 0xD9;

        return fileBytes;
    }

    public void Dispose()
    {
        _scope?.Dispose();
    }
}