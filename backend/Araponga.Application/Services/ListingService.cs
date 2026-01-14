using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Domain.Marketplace;

namespace Araponga.Application.Services;

public sealed class ListingService
{
    private readonly IListingRepository _listingRepository;
    private readonly IStoreRepository _storeRepository;
    private readonly IUserRepository _userRepository;
    private readonly AccessEvaluator _accessEvaluator;
    private readonly IUnitOfWork _unitOfWork;

    public ListingService(
        IListingRepository listingRepository,
        IStoreRepository storeRepository,
        IUserRepository userRepository,
        AccessEvaluator accessEvaluator,
        IUnitOfWork unitOfWork)
    {
        _listingRepository = listingRepository;
        _storeRepository = storeRepository;
        _userRepository = userRepository;
        _accessEvaluator = accessEvaluator;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<StoreItem>> CreateListingAsync(
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

        if (!await CanManageStoreAsync(store, userId, cancellationToken))
        {
            return Result<StoreItem>.Failure("Not authorized.");
        }

        var now = DateTime.UtcNow;
        var listing = new StoreItem(
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

        await _listingRepository.AddAsync(listing, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return Result<StoreItem>.Success(listing);
    }

    public async Task<Result<StoreItem>> UpdateListingAsync(
        Guid listingId,
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
        var listing = await _listingRepository.GetByIdAsync(listingId, cancellationToken);
        if (listing is null)
        {
            return Result<StoreItem>.Failure("Listing not found.");
        }

        if (title is not null && string.IsNullOrWhiteSpace(title))
        {
            return Result<StoreItem>.Failure("Title is required.");
        }

        var store = await _storeRepository.GetByIdAsync(listing.StoreId, cancellationToken);
        if (store is null)
        {
            return Result<StoreItem>.Failure("Store not found.");
        }

        if (!await CanManageStoreAsync(store, userId, cancellationToken))
        {
            return Result<StoreItem>.Failure("Not authorized.");
        }

        var now = DateTime.UtcNow;
        listing.UpdateDetails(
            type ?? listing.Type,
            title ?? listing.Title,
            description ?? listing.Description,
            category ?? listing.Category,
            tags ?? listing.Tags,
            pricingType ?? listing.PricingType,
            priceAmount ?? listing.PriceAmount,
            currency ?? listing.Currency,
            unit ?? listing.Unit,
            latitude ?? listing.Latitude,
            longitude ?? listing.Longitude,
            status ?? listing.Status,
            now);

        await _listingRepository.UpdateAsync(listing, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return Result<StoreItem>.Success(listing);
    }

    public async Task<Result<StoreItem>> ArchiveListingAsync(
        Guid listingId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var listing = await _listingRepository.GetByIdAsync(listingId, cancellationToken);
        if (listing is null)
        {
            return Result<StoreItem>.Failure("Listing not found.");
        }

        var store = await _storeRepository.GetByIdAsync(listing.StoreId, cancellationToken);
        if (store is null)
        {
            return Result<StoreItem>.Failure("Store not found.");
        }

        if (!await CanManageStoreAsync(store, userId, cancellationToken))
        {
            return Result<StoreItem>.Failure("Not authorized.");
        }

        listing.Archive(DateTime.UtcNow);
        await _listingRepository.UpdateAsync(listing, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return Result<StoreItem>.Success(listing);
    }

    public Task<StoreItem?> GetByIdAsync(Guid listingId, CancellationToken cancellationToken)
    {
        return _listingRepository.GetByIdAsync(listingId, cancellationToken);
    }

    public Task<IReadOnlyList<StoreItem>> SearchListingsAsync(
        Guid territoryId,
        ItemType? type,
        string? query,
        string? category,
        string? tags,
        ItemStatus? status,
        CancellationToken cancellationToken)
    {
        return _listingRepository.SearchAsync(territoryId, type, query, category, tags, status, cancellationToken);
    }

    public async Task<PagedResult<StoreItem>> SearchListingsPagedAsync(
        Guid territoryId,
        ItemType? type,
        string? query,
        string? category,
        string? tags,
        ItemStatus? status,
        PaginationParameters pagination,
        CancellationToken cancellationToken)
    {
        var totalCount = await _listingRepository.CountSearchAsync(territoryId, type, query, category, tags, status, cancellationToken);
        var listings = await _listingRepository.SearchPagedAsync(territoryId, type, query, category, tags, status, pagination.Skip, pagination.Take, cancellationToken);

        return new PagedResult<StoreItem>(listings, pagination.PageNumber, pagination.PageSize, totalCount);
    }

    private async Task<bool> CanManageStoreAsync(Store store, Guid userId, CancellationToken cancellationToken)
    {
        if (store.OwnerUserId == userId)
        {
            return true;
        }

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        return user is not null && _accessEvaluator.IsCurator(user);
    }
}
