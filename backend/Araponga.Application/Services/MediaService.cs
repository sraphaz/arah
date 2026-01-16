using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Interfaces.Media;
using Araponga.Application.Models;
using Araponga.Domain.Media;
using System.Security.Cryptography;
using System.Text;

namespace Araponga.Application.Services;

/// <summary>
/// Serviço de aplicação para gerenciamento de mídias.
/// </summary>
public sealed class MediaService
{
    private readonly IMediaAssetRepository _mediaAssetRepository;
    private readonly IMediaAttachmentRepository _mediaAttachmentRepository;
    private readonly IMediaStorageService _storageService;
    private readonly IMediaProcessingService _processingService;
    private readonly IMediaValidator _validator;
    private readonly IAuditLogger _auditLogger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAsyncMediaProcessor? _asyncProcessor;

    public MediaService(
        IMediaAssetRepository mediaAssetRepository,
        IMediaAttachmentRepository mediaAttachmentRepository,
        IMediaStorageService storageService,
        IMediaProcessingService processingService,
        IMediaValidator validator,
        IAuditLogger auditLogger,
        IUnitOfWork unitOfWork,
        IAsyncMediaProcessor? asyncProcessor = null)
    {
        _mediaAssetRepository = mediaAssetRepository;
        _mediaAttachmentRepository = mediaAttachmentRepository;
        _storageService = storageService;
        _processingService = processingService;
        _validator = validator;
        _auditLogger = auditLogger;
        _unitOfWork = unitOfWork;
        _asyncProcessor = asyncProcessor;
    }

    /// <summary>
    /// Faz upload de uma mídia (imagem, vídeo, etc.).
    /// </summary>
    public async Task<Result<MediaAsset>> UploadMediaAsync(
        Stream stream,
        string mimeType,
        string fileName,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        if (stream == null || !stream.CanRead)
        {
            return Result<MediaAsset>.Failure("Stream inválido.");
        }

        if (string.IsNullOrWhiteSpace(mimeType))
        {
            return Result<MediaAsset>.Failure("Tipo MIME é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(fileName))
        {
            return Result<MediaAsset>.Failure("Nome do arquivo é obrigatório.");
        }

        if (userId == Guid.Empty)
        {
            return Result<MediaAsset>.Failure("ID do usuário é obrigatório.");
        }

        // Validar tamanho do stream
        if (stream.CanSeek)
        {
            var sizeBytes = stream.Length;
            var validationResult = await _validator.ValidateAsync(stream, mimeType, sizeBytes, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<MediaAsset>.Failure(string.Join("; ", validationResult.Errors));
            }

            stream.Position = 0; // Resetar stream após validação
        }

        try
        {
            // Determinar tipo de mídia
            var mediaType = GetMediaTypeFromMimeType(mimeType);

            // Processar mídia (redimensionamento/otimização para imagens)
            Stream processedStream = stream;
            int? widthPx = null;
            int? heightPx = null;

            if (mediaType == MediaType.Image)
            {
                // Obter dimensões originais
                var dimensions = await _processingService.GetImageDimensionsAsync(stream, cancellationToken);
                if (dimensions.HasValue)
                {
                    widthPx = dimensions.Value.Width;
                    heightPx = dimensions.Value.Height;
                }

                stream.Position = 0;

                // Otimizar imagem
                processedStream = await _processingService.OptimizeImageAsync(stream, mimeType, cancellationToken);
            }

            // Calcular checksum do arquivo processado
            var checksum = await CalculateChecksumAsync(processedStream, cancellationToken);
            processedStream.Position = 0;

            // Upload para armazenamento
            var storageKey = await _storageService.UploadAsync(processedStream, mimeType, fileName, cancellationToken);

            // Criar MediaAsset
            var now = DateTime.UtcNow;
            var mediaAsset = new MediaAsset(
                Guid.NewGuid(),
                userId,
                mediaType,
                mimeType,
                storageKey,
                processedStream.CanSeek ? processedStream.Length : stream.Length,
                widthPx,
                heightPx,
                checksum,
                now,
                null,
                null);

            await _mediaAssetRepository.AddAsync(mediaAsset, cancellationToken);

            await _auditLogger.LogAsync(
                new AuditEntry("media.uploaded", userId, Guid.Empty, mediaAsset.Id, now),
                cancellationToken);

            await _unitOfWork.CommitAsync(cancellationToken);

            // Enfileirar processamento assíncrono adicional se o arquivo for muito grande
            if (_asyncProcessor != null && stream.CanSeek && stream.Length > 5 * 1024 * 1024) // 5MB threshold
            {
                _asyncProcessor.EnqueueProcessing(mediaAsset.Id, mediaAsset.StorageKey, mediaAsset.MimeType);
            }

            // Dispose do stream processado se for diferente do original
            if (processedStream != stream && processedStream is IDisposable disposable)
            {
                disposable.Dispose();
            }

            return Result<MediaAsset>.Success(mediaAsset);
        }
        catch (Exception ex)
        {
            return Result<MediaAsset>.Failure($"Erro ao fazer upload da mídia: {ex.Message}");
        }
    }

    /// <summary>
    /// Associa uma mídia a uma entidade (User, Post, Event, etc.).
    /// </summary>
    public async Task<Result<MediaAttachment>> AttachMediaToOwnerAsync(
        Guid mediaAssetId,
        MediaOwnerType ownerType,
        Guid ownerId,
        int? displayOrder,
        CancellationToken cancellationToken = default)
    {
        if (mediaAssetId == Guid.Empty)
        {
            return Result<MediaAttachment>.Failure("ID da mídia é obrigatório.");
        }

        if (ownerId == Guid.Empty)
        {
            return Result<MediaAttachment>.Failure("ID do proprietário é obrigatório.");
        }

        var mediaAsset = await _mediaAssetRepository.GetByIdAsync(mediaAssetId, cancellationToken);
        if (mediaAsset is null)
        {
            return Result<MediaAttachment>.Failure("Mídia não encontrada.");
        }

        if (mediaAsset.IsDeleted)
        {
            return Result<MediaAttachment>.Failure("Não é possível associar uma mídia deletada.");
        }

        // Se displayOrder não for fornecido, usar o próximo disponível
        if (!displayOrder.HasValue)
        {
            var existingAttachments = await _mediaAttachmentRepository.ListByOwnerAsync(ownerType, ownerId, cancellationToken);
            displayOrder = existingAttachments.Count > 0 ? existingAttachments.Max(a => a.DisplayOrder) + 1 : 0;
        }

        var attachment = new MediaAttachment(
            Guid.NewGuid(),
            mediaAssetId,
            ownerType,
            ownerId,
            displayOrder.Value,
            DateTime.UtcNow);

        await _mediaAttachmentRepository.AddAsync(attachment, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result<MediaAttachment>.Success(attachment);
    }

    /// <summary>
    /// Obtém um MediaAsset por ID.
    /// </summary>
    public async Task<MediaAsset?> GetMediaAssetAsync(
        Guid mediaAssetId,
        CancellationToken cancellationToken = default)
    {
        return await _mediaAssetRepository.GetByIdAsync(mediaAssetId, cancellationToken);
    }

    /// <summary>
    /// Obtém a URL de uma mídia (pública ou assinada).
    /// </summary>
    public async Task<Result<string>> GetMediaUrlAsync(
        Guid mediaAssetId,
        TimeSpan? expiresIn = null,
        CancellationToken cancellationToken = default)
    {
        var mediaAsset = await _mediaAssetRepository.GetByIdAsync(mediaAssetId, cancellationToken);
        if (mediaAsset is null)
        {
            return Result<string>.Failure("Mídia não encontrada.");
        }

        if (mediaAsset.IsDeleted)
        {
            return Result<string>.Failure("Mídia foi deletada.");
        }

        var url = await _storageService.GetUrlAsync(mediaAsset.StorageKey, expiresIn, cancellationToken);
        return Result<string>.Success(url);
    }

    /// <summary>
    /// Deleta uma mídia (soft delete).
    /// </summary>
    public async Task<OperationResult> DeleteMediaAsync(
        Guid mediaAssetId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var mediaAsset = await _mediaAssetRepository.GetByIdAsync(mediaAssetId, cancellationToken);
        if (mediaAsset is null)
        {
            return OperationResult.Failure("Mídia não encontrada.");
        }

        if (mediaAsset.IsDeleted)
        {
            return OperationResult.Failure("Mídia já foi deletada.");
        }

        if (mediaAsset.UploadedByUserId != userId)
        {
            return OperationResult.Failure("Apenas o criador da mídia pode deletá-la.");
        }

        var now = DateTime.UtcNow;
        mediaAsset.Delete(userId, now);
        await _mediaAssetRepository.UpdateAsync(mediaAsset, cancellationToken);

        // Deletar associações
        await _mediaAttachmentRepository.DeleteByMediaAssetIdAsync(mediaAssetId, cancellationToken);

        // Deletar arquivo do armazenamento (opcional - pode ser feito em background)
        try
        {
            await _storageService.DeleteAsync(mediaAsset.StorageKey, cancellationToken);
        }
        catch
        {
            // Log do erro mas não falha a operação (arquivo pode ser deletado em background)
        }

        await _auditLogger.LogAsync(
            new AuditEntry("media.deleted", userId, Guid.Empty, mediaAssetId, now),
            cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return OperationResult.Success();
    }

    /// <summary>
    /// Lista mídias associadas a uma entidade.
    /// </summary>
    public async Task<IReadOnlyList<MediaAsset>> ListMediaByOwnerAsync(
        MediaOwnerType ownerType,
        Guid ownerId,
        CancellationToken cancellationToken = default)
    {
        var attachments = await _mediaAttachmentRepository.ListByOwnerAsync(ownerType, ownerId, cancellationToken);
        if (attachments.Count == 0)
        {
            return Array.Empty<MediaAsset>();
        }

        var mediaAssetIds = attachments.Select(a => a.MediaAssetId).ToList();
        var mediaAssets = await _mediaAssetRepository.ListByIdsAsync(mediaAssetIds, cancellationToken);

        // Retornar ordenados por DisplayOrder
        var lookup = mediaAssets.ToDictionary(m => m.Id);
        return attachments
            .OrderBy(a => a.DisplayOrder)
            .Select(a => lookup.TryGetValue(a.MediaAssetId, out var asset) ? asset : null)
            .Where(asset => asset != null && !asset.IsDeleted)
            .Cast<MediaAsset>()
            .ToList();
    }

    private static MediaType GetMediaTypeFromMimeType(string mimeType)
    {
        return mimeType.ToLowerInvariant() switch
        {
            var m when m.StartsWith("image/") => MediaType.Image,
            var m when m.StartsWith("video/") => MediaType.Video,
            var m when m.StartsWith("audio/") => MediaType.Audio,
            _ => MediaType.Document
        };
    }

    private static async Task<string> CalculateChecksumAsync(Stream stream, CancellationToken cancellationToken)
    {
        using var sha256 = SHA256.Create();
        stream.Position = 0;
        var hashBytes = sha256.ComputeHash(stream);
        return BitConverter.ToString(hashBytes).Replace("-", "", StringComparison.OrdinalIgnoreCase).ToLowerInvariant();
    }
}