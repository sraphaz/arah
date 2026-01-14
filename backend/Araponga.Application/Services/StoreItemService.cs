using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Domain.Marketplace;
using Araponga.Domain.Membership;

namespace Araponga.Application.Services;

public sealed class StoreItemService
{
    private readonly IStoreItemRepository _itemRepository;
    private readonly IStoreRepository _storeRepository;
    private readonly IUserRepository _userRepository;
    private readonly AccessEvaluator _accessEvaluator;
    private readonly MembershipAccessRules _accessRules;
    private readonly IUnitOfWork _unitOfWork;

    public StoreItemService(
        IStoreItemRepository itemRepository,
        IStoreRepository storeRepository,
        IUserRepository userRepository,
        AccessEvaluator accessEvaluator,
        MembershipAccessRules accessRules,
        IUnitOfWork unitOfWork)
    {
        _itemRepository = itemRepository;
        _storeRepository = storeRepository;
        _userRepository = userRepository;
        _accessEvaluator = accessEvaluator;
        _accessRules = accessRules;
        _unitOfWork = unitOfWork;
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
        CancellationToken cancellationToken)
    {
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
        await _unitOfWork.CommitAsync(cancellationToken);
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
        await _unitOfWork.CommitAsync(cancellationToken);
        return Result<StoreItem>.Success(item);
    }

    public Task<StoreItem?> GetByIdAsync(Guid itemId, CancellationToken cancellationToken)
    {
        return _itemRepository.GetByIdAsync(itemId, cancellationToken);
    }

    public Task<IReadOnlyList<StoreItem>> SearchItemsAsync(
        Guid territoryId,
        ItemType? type,
        string? query,
        string? category,
        string? tags,
        ItemStatus? status,
        CancellationToken cancellationToken)
    {
        return _itemRepository.SearchAsync(territoryId, type, query, category, tags, status, cancellationToken);
    }

    public async Task<PagedResult<StoreItem>> SearchItemsPagedAsync(
        Guid territoryId,
        ItemType? type,
        string? query,
        string? category,
        string? tags,
        ItemStatus? status,
        PaginationParameters pagination,
        CancellationToken cancellationToken)
    {
        var totalCount = await _itemRepository.CountSearchAsync(territoryId, type, query, category, tags, status, cancellationToken);
        var items = await _itemRepository.SearchPagedAsync(territoryId, type, query, category, tags, status, pagination.Skip, pagination.Take, cancellationToken);

        return new PagedResult<StoreItem>(items, pagination.PageNumber, pagination.PageSize, totalCount);
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
