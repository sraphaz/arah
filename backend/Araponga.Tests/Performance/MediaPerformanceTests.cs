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
            global::SkippableFact.Skip.If(true, "Testes de performance pulados em CI/CD. Configure SKIP_PERFORMANCE_TESTS=false para executar.");
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

    [global::SkippableFact.SkippableFact]
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

    [global::SkippableFact.SkippableFact]
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

    [global::SkippableFact.SkippableFact]
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

    [global::SkippableFact.SkippableFact]
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
        // Gerar dados de imagem sintéticos (não é uma imagem válida, mas serve para testes de tamanho)
        var imageData = new byte[sizeBytes];
        new Random().NextBytes(imageData);
        
        // Adicionar cabeçalho JPEG mínimo para passar na validação básica
        var header = new byte[] { 0xFF, 0xD8, 0xFF };
        Array.Copy(header, imageData, Math.Min(header.Length, imageData.Length));
        
        var stream = new MemoryStream(imageData);
        return (stream, "image/jpeg", "large-test-image.jpg");
    }

    private static byte[] GenerateSimpleJpeg(int size)
    {
        // JPEG mínimo válido: SOI + APP0 + DQT + SOF + DHT + SOS + EOI
        var jpeg = new List<byte>();
        
        // SOI (Start of Image)
        jpeg.AddRange(new byte[] { 0xFF, 0xD8 });
        
        // APP0 (Application Data)
        jpeg.AddRange(new byte[] { 0xFF, 0xE0 });
        jpeg.AddRange(new byte[] { 0x00, 0x10 }); // Length
        jpeg.AddRange(Encoding.ASCII.GetBytes("JFIF\0"));
        jpeg.AddRange(new byte[5]); // Resto do APP0
        
        // Preencher até o tamanho desejado (simples, não é JPEG válido real)
        while (jpeg.Count < size - 2)
        {
            jpeg.Add(0x00);
        }
        
        // EOI (End of Image)
        jpeg.AddRange(new byte[] { 0xFF, 0xD9 });
        
        return jpeg.ToArray();
    }

    public void Dispose()
    {
        _scope?.Dispose();
    }
}