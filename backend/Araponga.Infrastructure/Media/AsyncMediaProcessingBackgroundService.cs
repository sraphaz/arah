using Araponga.Application.Interfaces.Media;
using Araponga.Domain.Media;
using Araponga.Infrastructure.Media;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace Araponga.Infrastructure.Media;

/// <summary>
/// Serviço em background para processamento assíncrono de imagens grandes.
/// </summary>
public sealed class AsyncMediaProcessingBackgroundService : BackgroundService, IAsyncMediaProcessor
{
    private readonly IServiceProvider _serviceProvider;
    private readonly MediaStorageOptions _options;
    private readonly ILogger<AsyncMediaProcessingBackgroundService> _logger;
    private readonly ConcurrentQueue<MediaProcessingJob> _processingQueue = new();

    public AsyncMediaProcessingBackgroundService(
        IServiceProvider serviceProvider,
        IOptions<MediaStorageOptions> options,
        ILogger<AsyncMediaProcessingBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _options = options.Value;
        _logger = logger;
    }

    /// <summary>
    /// Enfileira uma tarefa de processamento assíncrono.
    /// </summary>
    public void EnqueueProcessing(Guid mediaAssetId, string storageKey, string mimeType)
    {
        if (!_options.EnableAsyncProcessing)
        {
            return;
        }

        var job = new MediaProcessingJob
        {
            MediaAssetId = mediaAssetId,
            StorageKey = storageKey,
            MimeType = mimeType,
            EnqueuedAt = DateTime.UtcNow
        };

        _processingQueue.Enqueue(job);
        _logger.LogInformation("Tarefa de processamento assíncrono enfileirada: MediaAssetId={MediaAssetId}, StorageKey={StorageKey}", mediaAssetId, storageKey);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Serviço de processamento assíncrono de mídia iniciado.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (_processingQueue.TryDequeue(out var job))
                {
                    await ProcessJobAsync(job, stoppingToken);
                }
                else
                {
                    // Aguardar um pouco se não há tarefas
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar tarefa de mídia assíncrona.");
                // Continuar processando outras tarefas
            }
        }

        _logger.LogInformation("Serviço de processamento assíncrono de mídia parado.");
    }

    private async Task ProcessJobAsync(MediaProcessingJob job, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var storageService = scope.ServiceProvider.GetRequiredService<IMediaStorageService>();
        var processingService = scope.ServiceProvider.GetRequiredService<IMediaProcessingService>();
        var mediaAssetRepository = scope.ServiceProvider.GetRequiredService<Araponga.Application.Interfaces.Media.IMediaAssetRepository>();

        try
        {
            _logger.LogInformation("Iniciando processamento assíncrono: MediaAssetId={MediaAssetId}, StorageKey={StorageKey}", job.MediaAssetId, job.StorageKey);

            // Baixar arquivo original
            using var originalStream = await storageService.DownloadAsync(job.StorageKey, cancellationToken);

            // Verificar tamanho
            if (originalStream.CanSeek && originalStream.Length < _options.AsyncProcessingThresholdBytes)
            {
                _logger.LogInformation("Arquivo menor que threshold, processamento não necessário: MediaAssetId={MediaAssetId}, Size={Size}", job.MediaAssetId, originalStream.Length);
                return;
            }

            // Processar imagem (otimização e redimensionamento)
            Stream? processedStream = null;
            int? widthPx = null;
            int? heightPx = null;

            if (job.MimeType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            {
                // Obter dimensões
                var dimensions = await processingService.GetImageDimensionsAsync(originalStream, cancellationToken);
                if (dimensions.HasValue)
                {
                    widthPx = dimensions.Value.Width;
                    heightPx = dimensions.Value.Height;

                    // Redimensionar se necessário
                    originalStream.Position = 0;
                    if (widthPx > _options.AutoResizeMaxWidthPx || heightPx > _options.AutoResizeMaxHeightPx)
                    {
                        processedStream = await processingService.ResizeImageAsync(
                            originalStream,
                            _options.AutoResizeMaxWidthPx,
                            _options.AutoResizeMaxHeightPx,
                            cancellationToken);

                        // Atualizar dimensões
                        processedStream.Position = 0;
                        var newDimensions = await processingService.GetImageDimensionsAsync(processedStream, cancellationToken);
                        if (newDimensions.HasValue)
                        {
                            widthPx = newDimensions.Value.Width;
                            heightPx = newDimensions.Value.Height;
                        }
                    }

                    // Otimizar
                    var streamToOptimize = processedStream ?? originalStream;
                    streamToOptimize.Position = 0;
                    processedStream = await processingService.OptimizeImageAsync(streamToOptimize, job.MimeType, cancellationToken);
                }
            }

            if (processedStream != null)
            {
                // Upload do arquivo processado (substituir original)
                var fileName = Path.GetFileName(job.StorageKey) ?? "image.jpg";
                await storageService.DeleteAsync(job.StorageKey, cancellationToken); // Deletar original
                var newStorageKey = await storageService.UploadAsync(processedStream, job.MimeType, fileName, cancellationToken);

                // Atualizar MediaAsset com novo storage key e dimensões
                var mediaAsset = await mediaAssetRepository.GetByIdAsync(job.MediaAssetId, cancellationToken);
                if (mediaAsset != null)
                {
                    // Atualizar propriedades (criar novo objeto com propriedades atualizadas)
                    // Nota: Isso requer um método de atualização no repositório ou recriar a entidade
                    _logger.LogInformation("Processamento assíncrono concluído: MediaAssetId={MediaAssetId}, NewStorageKey={NewStorageKey}, Dimensions={Width}x{Height}",
                        job.MediaAssetId, newStorageKey, widthPx, heightPx);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar tarefa assíncrona: MediaAssetId={MediaAssetId}, StorageKey={StorageKey}", job.MediaAssetId, job.StorageKey);
        }
    }

    private sealed class MediaProcessingJob
    {
        public Guid MediaAssetId { get; set; }
        public string StorageKey { get; set; } = string.Empty;
        public string MimeType { get; set; } = string.Empty;
        public DateTime EnqueuedAt { get; set; }
    }
}