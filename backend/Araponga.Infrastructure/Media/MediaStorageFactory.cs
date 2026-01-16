using Araponga.Application.Interfaces.Media;
using Araponga.Infrastructure.Media;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Araponga.Infrastructure.Media;

/// <summary>
/// Factory para criar instâncias de IMediaStorageService baseado na configuração.
/// </summary>
public sealed class MediaStorageFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly MediaStorageOptions _options;
    private readonly ILoggerFactory? _loggerFactory;

    public MediaStorageFactory(
        IServiceProvider serviceProvider,
        IOptions<MediaStorageOptions> options,
        ILoggerFactory? loggerFactory = null)
    {
        _serviceProvider = serviceProvider;
        _options = options.Value;
        _loggerFactory = loggerFactory;
    }

    /// <summary>
    /// Cria uma instância de IMediaStorageService baseado no provider configurado.
    /// </summary>
    public IMediaStorageService CreateStorageService()
    {
        IMediaStorageService storageService = _options.Provider.ToLowerInvariant() switch
        {
            "s3" => CreateS3Service(),
            "azureblob" or "azure" => CreateAzureBlobService(),
            _ => new LocalMediaStorageService(Options.Create(_options))
        };

        // Adicionar cache se habilitado
        if (_options.EnableUrlCache)
        {
            var distributedCache = _serviceProvider.GetService<IDistributedCache>();
            if (distributedCache != null)
            {
                var logger = _loggerFactory?.CreateLogger<CachedMediaStorageService>();
                storageService = new CachedMediaStorageService(
                    storageService,
                    distributedCache,
                    logger,
                    _options.EnableUrlCache,
                    _options.UrlCacheExpiration);
            }
        }

        return storageService;
    }

    private IMediaStorageService CreateS3Service()
    {
        if (string.IsNullOrWhiteSpace(_options.S3BucketName))
        {
            throw new InvalidOperationException("S3BucketName é obrigatório quando Provider=S3.");
        }

        return new S3MediaStorageService(Options.Create(_options));
    }

    private IMediaStorageService CreateAzureBlobService()
    {
        if (string.IsNullOrWhiteSpace(_options.AzureBlobConnectionString))
        {
            throw new InvalidOperationException("AzureBlobConnectionString é obrigatório quando Provider=AzureBlob.");
        }

        if (string.IsNullOrWhiteSpace(_options.AzureBlobContainerName))
        {
            throw new InvalidOperationException("AzureBlobContainerName é obrigatório quando Provider=AzureBlob.");
        }

        return new AzureBlobMediaStorageService(Options.Create(_options));
    }
}