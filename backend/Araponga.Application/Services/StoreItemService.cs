using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Interfaces.Media;
using Araponga.Application.Services;
using Araponga.Application.Services.Media;
using Araponga.Domain.Marketplace;
using Araponga.Domain.Media;
using Araponga.Domain.Membership;

namespace Araponga.Application.Services;

public sealed class StoreItemService
{
    private readonly IStoreItemRepository _itemRepository;
    private readonly IStoreRepository _storeRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMediaAssetRepository _mediaAssetRepository;
    private readonly IMediaAttachmentRepository _mediaAttachmentRepository;
    private readonly TerritoryMediaConfigService _mediaConfigService;
    private readonly AccessEvaluator _accessEvaluator;
    private readonly MembershipAccessRules _accessRules;
    private readonly TerritoryFeatureFlagGuard _featureGuard;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CacheInvalidationService? _cacheInvalidation;
    private readonly TerritoryModerationService? _moderationService;

    public StoreItemService(
        IStoreItemRepository itemRepository,
        IStoreRepository storeRepository,
        IUserRepository userRepository,
        IMediaAssetRepository mediaAssetRepository,
        IMediaAttachmentRepository mediaAttachmentRepository,
        TerritoryMediaConfigService mediaConfigService,
        AccessEvaluator accessEvaluator,
        MembershipAccessRules accessRules,
        TerritoryFeatureFlagGuard featureGuard,
        IUnitOfWork unitOfWork,
        CacheInvalidationService? cacheInvalidation = null,
        TerritoryModerationService? moderationService = null)
    {
        _itemRepository = itemRepository;
        _storeRepository = storeRepository;
        _userRepository = userRepository;
        _mediaAssetRepository = mediaAssetRepository;
        _mediaAttachmentRepository = mediaAttachmentRepository;
        _mediaConfigService = mediaConfigService;
        _accessEvaluator = accessEvaluator;
        _accessRules = accessRules;
        _featureGuard = featureGuard;
        _unitOfWork = unitOfWork;
        _cacheInvalidation = cacheInvalidation;
        _moderationService = moderationService;
    }

    public async Task<Result<StoreItem>> CreateItemAsync(
        Guid territoryId,
        Guid userId,
        Guid storeId,
        ItemType type,
        string title,
        string? description,
        string? category,
        string? tags,
        ItemPricingType pricingType,
        decimal? priceAmount,
        string? currency,
        string? unit,
        double? latitude,
        double? longitude,
        ItemStatus status,
        IReadOnlyCollection<Guid>? mediaIds,
        CancellationToken cancellationToken)
    {
        // Verificar aceite de políticas obrigatórias
        var policiesResult = await _accessEvaluator.HasAcceptedRequiredPoliciesAsync(userId, cancellationToken);
        if (policiesResult.IsFailure || !policiesResult.Value)
        {
            var pendingPolicies = await _accessEvaluator.GetPendingPoliciesAsync(userId, cancellationToken);
            var errorMessage = "You must accept the required terms of service and privacy policies before creating items.";
            if (pendingPolicies is not null && !pendingPolicies.IsEmpty)
            {
                var pendingTermsCount = pendingPolicies.RequiredTerms.Count;
                var pendingPoliciesCount = pendingPolicies.RequiredPrivacyPolicies.Count;
                if (pendingTermsCount > 0 || pendingPoliciesCount > 0)
                {
                    errorMessage = $"You must accept {pendingTermsCount + pendingPoliciesCount} required policy(ies) before creating items.";
                }
            }
            return Result<StoreItem>.Failure(errorMessage);
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            return Result<StoreItem>.Failure("Title is required.");
        }

        var store = await _storeRepository.GetByIdAsync(storeId, cancellationToken);
        if (store is null || store.TerritoryId != territoryId)
        {
            return Result<StoreItem>.Failure("Store not found.");
        }

        // Verificar regras de marketplace
        if (!await _accessRules.CanCreateStoreOrItemInMarketplaceAsync(userId, territoryId, cancellationToken))
        {
            // Verificar se pode gerenciar a store (owner ou curator)
            if (!await CanManageStoreAsync(store, userId, cancellationToken))
            {
                return Result<StoreItem>.Failure("Not authorized. Marketplace rules not met or not store owner/curator.");
            }
        }

        // Validar e normalizar mediaIds
        var normalizedMediaIds = mediaIds?
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToList();

        if (normalizedMediaIds is not null && normalizedMediaIds.Count > 0)
        {
            // Obter limites efetivos da configuração territorial (com fallback para global)
            var limits = await _mediaConfigService.GetEffectiveContentLimitsAsync(
                territoryId,
                MediaContentType.Marketplace,
                cancellationToken);

            // Validar quantidade máxima de mídias
            if (normalizedMediaIds.Count > limits.MaxMediaCount)
            {
                return Result<StoreItem>.Failure($"Maximum {limits.MaxMediaCount} media items allowed per item.");
            }

            var mediaAssets = await _mediaAssetRepository.ListByIdsAsync(normalizedMediaIds, cancellationToken);
            if (mediaAssets.Count != normalizedMediaIds.Count)
            {
                return Result<StoreItem>.Failure("One or more media assets not found.");
            }

            // Validar que todas as mídias pertencem ao usuário
            if (mediaAssets.Any(media => media.UploadedByUserId != userId || media.IsDeleted))
            {
                return Result<StoreItem>.Failure("One or more media assets are invalid or do not belong to the user.");
            }

            var images = mediaAssets.Where(media => media.MediaType == Domain.Media.MediaType.Image).ToList();
            var videos = mediaAssets.Where(media => media.MediaType == Domain.Media.MediaType.Video).ToList();
            var audios = mediaAssets.Where(media => media.MediaType == Domain.Media.MediaType.Audio).ToList();

            // Validar imagens
            if (images.Count > 0 && !limits.ImagesEnabled)
            {
                return Result<StoreItem>.Failure("Images are not enabled for marketplace items in this territory.");
            }

            // Validar vídeos
            if (videos.Count > 0 && !limits.VideosEnabled)
            {
                return Result<StoreItem>.Failure("Videos are not enabled for marketplace items in this territory.");
            }
            if (videos.Count > limits.MaxVideoCount)
            {
                return Result<StoreItem>.Failure($"Maximum {limits.MaxVideoCount} video(s) allowed per item.");
            }
            foreach (var video in videos)
            {
                if (video.SizeBytes > limits.MaxVideoSizeBytes)
                {
                    var maxSizeMB = limits.MaxVideoSizeBytes / (1024.0 * 1024.0);
                    return Result<StoreItem>.Failure($"Video size exceeds {maxSizeMB:F1}MB limit for marketplace items.");
                }
                // Validar tipo MIME se configurado
                if (limits.AllowedVideoMimeTypes != null && limits.AllowedVideoMimeTypes.Count > 0)
                {
                    if (!limits.AllowedVideoMimeTypes.Contains(video.MimeType, StringComparer.OrdinalIgnoreCase))
                    {
                        return Result<StoreItem>.Failure($"Video MIME type '{video.MimeType}' is not allowed for marketplace items.");
                    }
                }
            }

            // Validar áudios
            if (audios.Count > 0 && !limits.AudioEnabled)
            {
                return Result<StoreItem>.Failure("Audio is not enabled for marketplace items in this territory.");
            }
            if (audios.Count > limits.MaxAudioCount)
            {
                return Result<StoreItem>.Failure($"Maximum {limits.MaxAudioCount} audio(s) allowed per item.");
            }
            foreach (var audio in audios)
            {
                if (audio.SizeBytes > limits.MaxAudioSizeBytes)
                {
                    var maxSizeMB = limits.MaxAudioSizeBytes / (1024.0 * 1024.0);
                    return Result<StoreItem>.Failure($"Audio size exceeds {maxSizeMB:F1}MB limit for marketplace items.");
                }
                // Validar tipo MIME se configurado
                if (limits.AllowedAudioMimeTypes != null && limits.AllowedAudioMimeTypes.Count > 0)
                {
                    if (!limits.AllowedAudioMimeTypes.Contains(audio.MimeType, StringComparer.OrdinalIgnoreCase))
                    {
                        return Result<StoreItem>.Failure($"Audio MIME type '{audio.MimeType}' is not allowed for marketplace items.");
                    }
                }
            }
        }

        // Verificar regras de moderação comunitária
        if (_moderationService is not null)
        {
            // Criar item temporário para validação
            var tempItem = new StoreItem(
                Guid.NewGuid(),
                territoryId,
                storeId,
                type,
                title,
                description,
                category,
                tags,
                pricingType,
                priceAmount,
                currency,
                unit,
                latitude,
                longitude,
                status,
                DateTime.UtcNow,
                DateTime.UtcNow);

            var moderationResult = await _moderationService.ApplyRulesAsync(tempItem, cancellationToken);
            if (moderationResult.IsFailure)
            {
                return Result<StoreItem>.Failure(moderationResult.Error ?? "Item violates territory moderation rules.");
            }
        }

        var now = DateTime.UtcNow;
        var item = new StoreItem(
            Guid.NewGuid(),
            territoryId,
            storeId,
            type,
            title,
            description,
            category,
            tags,
            pricingType,
            priceAmount,
            currency,
            unit,
            latitude,
            longitude,
            status,
            now,
            now);

        await _itemRepository.AddAsync(item, cancellationToken);

        // Criar MediaAttachments para as mídias associadas ao item
        if (normalizedMediaIds is not null && normalizedMediaIds.Count > 0)
        {
            foreach (var (mediaId, index) in normalizedMediaIds.Select((id, idx) => (id, idx)))
            {
                var attachment = new MediaAttachment(
                    Guid.NewGuid(),
                    mediaId,
                    MediaOwnerType.StoreItem,
                    item.Id,
                    index,
                    now);

                await _mediaAttachmentRepository.AddAsync(attachment, cancellationToken);
            }
        }

        await _unitOfWork.CommitAsync(cancellationToken);
        
        // Invalidar cache de items da store
        _cacheInvalidation?.InvalidateItemCache(storeId, item.Id);
        
        return Result<StoreItem>.Success(item);
    }

    public async Task<Result<StoreItem>> UpdateItemAsync(
        Guid itemId,
        Guid userId,
        ItemType? type,
        string? title,
        string? description,
        string? category,
        string? tags,
        ItemPricingType? pricingType,
        decimal? priceAmount,
        string? currency,
        string? unit,
        double? latitude,
        double? longitude,
        ItemStatus? status,
        CancellationToken cancellationToken)
    {
        var item = await _itemRepository.GetByIdAsync(itemId, cancellationToken);
        if (item is null)
        {
            return Result<StoreItem>.Failure("Item not found.");
        }

        if (title is not null && string.IsNullOrWhiteSpace(title))
        {
            return Result<StoreItem>.Failure("Title is required.");
        }

        var store = await _storeRepository.GetByIdAsync(item.StoreId, cancellationToken);
        if (store is null)
        {
            return Result<StoreItem>.Failure("Store not found.");
        }

        if (!await CanManageStoreAsync(store, userId, cancellationToken))
        {
            return Result<StoreItem>.Failure("Not authorized.");
        }

        var now = DateTime.UtcNow;
        item.UpdateDetails(
            type ?? item.Type,
            title ?? item.Title,
            description ?? item.Description,
            category ?? item.Category,
            tags ?? item.Tags,
            pricingType ?? item.PricingType,
            priceAmount ?? item.PriceAmount,
            currency ?? item.Currency,
            unit ?? item.Unit,
            latitude ?? item.Latitude,
            longitude ?? item.Longitude,
            status ?? item.Status,
            now);

        await _itemRepository.UpdateAsync(item, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        
        // Invalidar cache de items da store
        _cacheInvalidation?.InvalidateItemCache(item.StoreId, item.Id);
        
        return Result<StoreItem>.Success(item);
    }

    public async Task<Result<StoreItem>> ArchiveItemAsync(
        Guid itemId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var item = await _itemRepository.GetByIdAsync(itemId, cancellationToken);
        if (item is null)
        {
            return Result<StoreItem>.Failure("Item not found.");
        }

        var store = await _storeRepository.GetByIdAsync(item.StoreId, cancellationToken);
        if (store is null)
        {
            return Result<StoreItem>.Failure("Store not found.");
        }

        if (!await CanManageStoreAsync(store, userId, cancellationToken))
        {
            return Result<StoreItem>.Failure("Not authorized.");
        }

        item.Archive(DateTime.UtcNow);
        await _itemRepository.UpdateAsync(item, cancellationToken);

        // Deletar mídias associadas ao item quando arquivado
        await _mediaAttachmentRepository.DeleteByOwnerAsync(MediaOwnerType.StoreItem, item.Id, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);
        
        // Invalidar cache de items da store
        _cacheInvalidation?.InvalidateItemCache(item.StoreId, item.Id);
        
        return Result<StoreItem>.Success(item);
    }

    public async Task<StoreItem?> GetByIdAsync(Guid itemId, CancellationToken cancellationToken)
    {
        var item = await _itemRepository.GetByIdAsync(itemId, cancellationToken);
        if (item is null)
        {
            return null;
        }

        // Evita expor detalhes do marketplace quando desabilitado no território.
        var gate = _featureGuard.EnsureMarketplaceEnabled(item.TerritoryId);
        return gate.IsSuccess ? item : null;
    }

    public async Task<Result<IReadOnlyList<StoreItem>>> SearchItemsAsync(
        Guid territoryId,
        ItemType? type,
        string? query,
        string? category,
        string? tags,
        ItemStatus? status,
        CancellationToken cancellationToken)
    {
        var gate = _featureGuard.EnsureMarketplaceEnabled(territoryId);
        if (gate.IsFailure)
        {
            return Result<IReadOnlyList<StoreItem>>.Failure(gate.Error ?? "Marketplace is disabled for this territory.");
        }

        var items = await _itemRepository.SearchAsync(territoryId, type, query, category, tags, status, cancellationToken);
        return Result<IReadOnlyList<StoreItem>>.Success(items);
    }

    public async Task<Result<PagedResult<StoreItem>>> SearchItemsPagedAsync(
        Guid territoryId,
        ItemType? type,
        string? query,
        string? category,
        string? tags,
        ItemStatus? status,
        PaginationParameters pagination,
        CancellationToken cancellationToken)
    {
        var gate = _featureGuard.EnsureMarketplaceEnabled(territoryId);
        if (gate.IsFailure)
        {
            return Result<PagedResult<StoreItem>>.Failure(gate.Error ?? "Marketplace is disabled for this territory.");
        }

        var totalCount = await _itemRepository.CountSearchAsync(territoryId, type, query, category, tags, status, cancellationToken);
        var items = await _itemRepository.SearchPagedAsync(territoryId, type, query, category, tags, status, pagination.Skip, pagination.Take, cancellationToken);

        return Result<PagedResult<StoreItem>>.Success(new PagedResult<StoreItem>(items, pagination.PageNumber, pagination.PageSize, totalCount));
    }

    private async Task<bool> CanManageStoreAsync(Store store, Guid userId, CancellationToken cancellationToken)
    {
        if (store.OwnerUserId == userId)
        {
            return true;
        }

        // Verificar se tem capacidade de Curator no território da Store
        return await _accessEvaluator.HasCapabilityAsync(
            userId,
            store.TerritoryId,
            MembershipCapabilityType.Curator,
            cancellationToken);
    }
}
