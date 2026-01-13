using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Domain.Marketplace;

namespace Araponga.Application.Services;

public sealed class InquiryService
{
    private readonly IInquiryRepository _inquiryRepository;
    private readonly IListingRepository _listingRepository;
    private readonly IStoreRepository _storeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public InquiryService(
        IInquiryRepository inquiryRepository,
        IListingRepository listingRepository,
        IStoreRepository storeRepository,
        IUnitOfWork unitOfWork)
    {
        _inquiryRepository = inquiryRepository;
        _listingRepository = listingRepository;
        _storeRepository = storeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<InquiryCreationResult>> CreateInquiryAsync(
        Guid listingId,
        Guid fromUserId,
        string? message,
        Guid? batchId,
        CancellationToken cancellationToken)
    {
        var listing = await _listingRepository.GetByIdAsync(listingId, cancellationToken);
        if (listing is null)
        {
            return Result<InquiryCreationResult>.Failure("Listing not found.");
        }

        var store = await _storeRepository.GetByIdAsync(listing.StoreId, cancellationToken);
        if (store is null)
        {
            return Result<InquiryCreationResult>.Failure("Store not found.");
        }

        var inquiry = new ListingInquiry(
            Guid.NewGuid(),
            listing.TerritoryId,
            listing.Id,
            listing.StoreId,
            fromUserId,
            message,
            InquiryStatus.Open,
            batchId,
            DateTime.UtcNow);

        await _inquiryRepository.AddAsync(inquiry, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        var contact = new StoreContactInfo(
            store.ContactVisibility,
            store.Phone,
            store.Whatsapp,
            store.Email,
            store.Instagram,
            store.Website,
            store.PreferredContactMethod);

        return Result<InquiryCreationResult>.Success(new InquiryCreationResult(inquiry, contact));
    }

    public Task<IReadOnlyList<ListingInquiry>> ListMyInquiriesAsync(Guid userId, CancellationToken cancellationToken)
    {
        return _inquiryRepository.ListByUserAsync(userId, cancellationToken);
    }

    public async Task<IReadOnlyList<ListingInquiry>> ListReceivedInquiriesAsync(Guid ownerUserId, CancellationToken cancellationToken)
    {
        var stores = await _storeRepository.ListByOwnerAsync(ownerUserId, cancellationToken);
        if (stores.Count == 0)
        {
            return Array.Empty<ListingInquiry>();
        }

        var storeIds = stores.Select(s => s.Id).ToList();
        return await _inquiryRepository.ListByStoreIdsAsync(storeIds, cancellationToken);
    }

    public async Task<Common.PagedResult<ListingInquiry>> ListMyInquiriesPagedAsync(
        Guid userId,
        Common.PaginationParameters pagination,
        CancellationToken cancellationToken)
    {
        var inquiries = await _inquiryRepository.ListByUserAsync(userId, cancellationToken);
        var totalCount = inquiries.Count;
        var pagedItems = inquiries
            .OrderByDescending(i => i.CreatedAtUtc)
            .Skip(pagination.Skip)
            .Take(pagination.Take)
            .ToList();

        return new Common.PagedResult<ListingInquiry>(pagedItems, pagination.PageNumber, pagination.PageSize, totalCount);
    }

    public async Task<Common.PagedResult<ListingInquiry>> ListReceivedInquiriesPagedAsync(
        Guid ownerUserId,
        Common.PaginationParameters pagination,
        CancellationToken cancellationToken)
    {
        var stores = await _storeRepository.ListByOwnerAsync(ownerUserId, cancellationToken);
        if (stores.Count == 0)
        {
            return new Common.PagedResult<ListingInquiry>(Array.Empty<ListingInquiry>(), pagination.PageNumber, pagination.PageSize, 0);
        }

        var storeIds = stores.Select(s => s.Id).ToList();
        var inquiries = await _inquiryRepository.ListByStoreIdsAsync(storeIds, cancellationToken);
        var totalCount = inquiries.Count;
        var pagedItems = inquiries
            .OrderByDescending(i => i.CreatedAtUtc)
            .Skip(pagination.Skip)
            .Take(pagination.Take)
            .ToList();

        return new Common.PagedResult<ListingInquiry>(pagedItems, pagination.PageNumber, pagination.PageSize, totalCount);
    }
}
