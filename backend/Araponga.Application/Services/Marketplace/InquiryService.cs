using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Modules.Marketplace.Application.Interfaces;
using Araponga.Modules.Marketplace.Domain;

namespace Araponga.Application.Services;

public sealed class InquiryService
{
    private readonly IInquiryRepository _inquiryRepository;
    private readonly IStoreItemRepository _itemRepository;
    private readonly IStoreRepository _storeRepository;
    private readonly TerritoryFeatureFlagGuard _featureGuard;
    private readonly IUnitOfWork _unitOfWork;

    public InquiryService(
        IInquiryRepository inquiryRepository,
        IStoreItemRepository itemRepository,
        IStoreRepository storeRepository,
        TerritoryFeatureFlagGuard featureGuard,
        IUnitOfWork unitOfWork)
    {
        _inquiryRepository = inquiryRepository;
        _itemRepository = itemRepository;
        _storeRepository = storeRepository;
        _featureGuard = featureGuard;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<InquiryCreationResult>> CreateInquiryAsync(
        Guid itemId,
        Guid fromUserId,
        string? message,
        Guid? batchId,
        CancellationToken cancellationToken)
    {
        var item = await _itemRepository.GetByIdAsync(itemId, cancellationToken);
        if (item is null)
        {
            return Result<InquiryCreationResult>.Failure("Item not found.");
        }

        var gate = _featureGuard.EnsureMarketplaceEnabled(item.TerritoryId);
        if (gate.IsFailure)
        {
            return Result<InquiryCreationResult>.Failure(gate.Error ?? "Marketplace is disabled for this territory.");
        }

        var store = await _storeRepository.GetByIdAsync(item.StoreId, cancellationToken);
        if (store is null)
        {
            return Result<InquiryCreationResult>.Failure("Store not found.");
        }

        var inquiry = new ItemInquiry(
            Guid.NewGuid(),
            item.TerritoryId,
            item.Id,
            item.StoreId,
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

    public Task<IReadOnlyList<ItemInquiry>> ListMyInquiriesAsync(Guid userId, CancellationToken cancellationToken)
    {
        return _inquiryRepository.ListByUserAsync(userId, cancellationToken);
    }

    public async Task<IReadOnlyList<ItemInquiry>> ListReceivedInquiriesAsync(Guid ownerUserId, CancellationToken cancellationToken)
    {
        var stores = await _storeRepository.ListByOwnerAsync(ownerUserId, cancellationToken);
        if (stores.Count == 0)
        {
            return Array.Empty<ItemInquiry>();
        }

        var storeIds = stores.Select(s => s.Id).ToList();
        return await _inquiryRepository.ListByStoreIdsAsync(storeIds, cancellationToken);
    }

    public async Task<Common.PagedResult<ItemInquiry>> ListMyInquiriesPagedAsync(
        Guid userId,
        Common.PaginationParameters pagination,
        CancellationToken cancellationToken)
    {
        var inquiries = await _inquiryRepository.ListByUserAsync(userId, cancellationToken);
        var ordered = inquiries.OrderByDescending(i => i.CreatedAtUtc).ToList();
        return ordered.ToPagedResult(pagination);
    }

    public async Task<Common.PagedResult<ItemInquiry>> ListReceivedInquiriesPagedAsync(
        Guid ownerUserId,
        Common.PaginationParameters pagination,
        CancellationToken cancellationToken)
    {
        var stores = await _storeRepository.ListByOwnerAsync(ownerUserId, cancellationToken);
        if (stores.Count == 0)
        {
            return new Common.PagedResult<ItemInquiry>(Array.Empty<ItemInquiry>(), pagination.PageNumber, pagination.PageSize, 0);
        }

        var storeIds = stores.Select(s => s.Id).ToList();
        var inquiries = await _inquiryRepository.ListByStoreIdsAsync(storeIds, cancellationToken);
        var ordered = inquiries.OrderByDescending(i => i.CreatedAtUtc).ToList();
        return ordered.ToPagedResult(pagination);
    }
}
