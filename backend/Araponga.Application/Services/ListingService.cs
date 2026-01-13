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

    public async Task<(bool success, string? error, StoreListing? listing)> CreateListingAsync(
        Guid territoryId,
        Guid userId,
        Guid storeId,
        ListingType type,
        string title,
        string? description,
        string? category,
        string? tags,
        ListingPricingType pricingType,
        decimal? priceAmount,
        string? currency,
        string? unit,
        double? latitude,
        double? longitude,
        ListingStatus status,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return (false, "Title is required.", null);
        }

        var store = await _storeRepository.GetByIdAsync(storeId, cancellationToken);
        if (store is null || store.TerritoryId != territoryId)
        {
            return (false, "Store not found.", null);
        }

        if (!await CanManageStoreAsync(store, userId, cancellationToken))
        {
            return (false, "Not authorized.", null);
        }

        var now = DateTime.UtcNow;
        var listing = new StoreListing(
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
        return (true, null, listing);
    }

    public async Task<(bool success, string? error, StoreListing? listing)> UpdateListingAsync(
        Guid listingId,
        Guid userId,
        ListingType? type,
        string? title,
        string? description,
        string? category,
        string? tags,
        ListingPricingType? pricingType,
        decimal? priceAmount,
        string? currency,
        string? unit,
        double? latitude,
        double? longitude,
        ListingStatus? status,
        CancellationToken cancellationToken)
    {
        var listing = await _listingRepository.GetByIdAsync(listingId, cancellationToken);
        if (listing is null)
        {
            return (false, "Listing not found.", null);
        }

        if (title is not null && string.IsNullOrWhiteSpace(title))
        {
            return (false, "Title is required.", null);
        }

        var store = await _storeRepository.GetByIdAsync(listing.StoreId, cancellationToken);
        if (store is null)
        {
            return (false, "Store not found.", null);
        }

        if (!await CanManageStoreAsync(store, userId, cancellationToken))
        {
            return (false, "Not authorized.", null);
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
        return (true, null, listing);
    }

    public async Task<(bool success, string? error, StoreListing? listing)> ArchiveListingAsync(
        Guid listingId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var listing = await _listingRepository.GetByIdAsync(listingId, cancellationToken);
        if (listing is null)
        {
            return (false, "Listing not found.", null);
        }

        var store = await _storeRepository.GetByIdAsync(listing.StoreId, cancellationToken);
        if (store is null)
        {
            return (false, "Store not found.", null);
        }

        if (!await CanManageStoreAsync(store, userId, cancellationToken))
        {
            return (false, "Not authorized.", null);
        }

        listing.Archive(DateTime.UtcNow);
        await _listingRepository.UpdateAsync(listing, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return (true, null, listing);
    }

    public Task<StoreListing?> GetByIdAsync(Guid listingId, CancellationToken cancellationToken)
    {
        return _listingRepository.GetByIdAsync(listingId, cancellationToken);
    }

    public Task<IReadOnlyList<StoreListing>> SearchListingsAsync(
        Guid territoryId,
        ListingType? type,
        string? query,
        string? category,
        string? tags,
        ListingStatus? status,
        CancellationToken cancellationToken)
    {
        return _listingRepository.SearchAsync(territoryId, type, query, category, tags, status, cancellationToken);
    }

    private async Task<bool> CanManageStoreAsync(TerritoryStore store, Guid userId, CancellationToken cancellationToken)
    {
        if (store.OwnerUserId == userId)
        {
            return true;
        }

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        return user is not null && _accessEvaluator.IsCurator(user);
    }
}
