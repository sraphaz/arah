using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Domain.Marketplace;

namespace Araponga.Application.Services;

public sealed class CartService
{
    private readonly ICartRepository _cartRepository;
    private readonly ICartItemRepository _cartItemRepository;
    private readonly IListingRepository _listingRepository;
    private readonly IStoreRepository _storeRepository;
    private readonly ICheckoutRepository _checkoutRepository;
    private readonly ICheckoutItemRepository _checkoutItemRepository;
    private readonly IInquiryRepository _inquiryRepository;
    private readonly IPlatformFeeConfigRepository _platformFeeConfigRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CartService(
        ICartRepository cartRepository,
        ICartItemRepository cartItemRepository,
        IListingRepository listingRepository,
        IStoreRepository storeRepository,
        ICheckoutRepository checkoutRepository,
        ICheckoutItemRepository checkoutItemRepository,
        IInquiryRepository inquiryRepository,
        IPlatformFeeConfigRepository platformFeeConfigRepository,
        IUnitOfWork unitOfWork)
    {
        _cartRepository = cartRepository;
        _cartItemRepository = cartItemRepository;
        _listingRepository = listingRepository;
        _storeRepository = storeRepository;
        _checkoutRepository = checkoutRepository;
        _checkoutItemRepository = checkoutItemRepository;
        _inquiryRepository = inquiryRepository;
        _platformFeeConfigRepository = platformFeeConfigRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CartDetails> GetCartAsync(Guid territoryId, Guid userId, CancellationToken cancellationToken)
    {
        var cart = await GetOrCreateCartAsync(territoryId, userId, cancellationToken);
        return await BuildCartDetailsAsync(cart, cancellationToken);
    }

    public async Task<CartDetails?> GetCartByIdAsync(Guid cartId, Guid userId, CancellationToken cancellationToken)
    {
        var cart = await _cartRepository.GetByIdAsync(cartId, cancellationToken);
        if (cart is null || cart.UserId != userId)
        {
            return null;
        }

        return await BuildCartDetailsAsync(cart, cancellationToken);
    }

    private async Task<CartDetails> BuildCartDetailsAsync(Cart cart, CancellationToken cancellationToken)
    {
        var items = await _cartItemRepository.ListByCartIdAsync(cart.Id, cancellationToken);

        if (items.Count == 0)
        {
            return new CartDetails(cart, Array.Empty<CartItemDetails>());
        }

        var listingIds = items.Select(i => i.ListingId).Distinct().ToList();
        var listings = await _listingRepository.ListByIdsAsync(listingIds, cancellationToken);
        var listingMap = listings.ToDictionary(l => l.Id, l => l);

        var storeIds = listings.Select(l => l.StoreId).Distinct().ToList();
        var stores = await _storeRepository.ListByIdsAsync(storeIds, cancellationToken);
        var storeMap = stores.ToDictionary(s => s.Id, s => s);

        var itemDetails = new List<CartItemDetails>();

        foreach (var item in items)
        {
            if (!listingMap.TryGetValue(item.ListingId, out var listing))
            {
                continue;
            }

            if (!storeMap.TryGetValue(listing.StoreId, out var store))
            {
                continue;
            }

            var isPurchasable = IsPurchasable(listing, store);
            itemDetails.Add(new CartItemDetails(item, listing, store, isPurchasable));
        }

        return new CartDetails(cart, itemDetails);
    }

    public async Task<Result<CartItem>> AddItemAsync(
        Guid territoryId,
        Guid userId,
        Guid listingId,
        int quantity,
        string? notes,
        CancellationToken cancellationToken)
    {
        if (quantity < 1)
        {
            return Result<CartItem>.Failure("Quantity must be at least 1.");
        }

        var listing = await _listingRepository.GetByIdAsync(listingId, cancellationToken);
        if (listing is null || listing.TerritoryId != territoryId)
        {
            return Result<CartItem>.Failure("Listing not found for territory.");
        }

        var cart = await GetOrCreateCartAsync(territoryId, userId, cancellationToken);
        var existing = await _cartItemRepository.GetByCartAndListingAsync(cart.Id, listingId, cancellationToken);
        var now = DateTime.UtcNow;

        if (existing is null)
        {
            var item = new CartItem(Guid.NewGuid(), cart.Id, listingId, quantity, notes, now, now);
            await _cartItemRepository.AddAsync(item, cancellationToken);
            cart.Touch(now);
            await _cartRepository.UpdateAsync(cart, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return Result<CartItem>.Success(item);
        }

        existing.Update(quantity, notes ?? existing.Notes, now);
        await _cartItemRepository.UpdateAsync(existing, cancellationToken);
        cart.Touch(now);
        await _cartRepository.UpdateAsync(cart, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return Result<CartItem>.Success(existing);
    }

    public async Task<Result<CartItem>> UpdateItemAsync(
        Guid cartItemId,
        Guid userId,
        int quantity,
        string? notes,
        CancellationToken cancellationToken)
    {
        if (quantity < 1)
        {
            return Result<CartItem>.Failure("Quantity must be at least 1.");
        }

        var item = await _cartItemRepository.GetByIdAsync(cartItemId, cancellationToken);
        if (item is null)
        {
            return Result<CartItem>.Failure("Cart item not found.");
        }

        var cart = await _cartRepository.GetByIdAsync(item.CartId, cancellationToken);
        if (cart is null || cart.UserId != userId)
        {
            return Result<CartItem>.Failure("Cart item not found.");
        }

        var now = DateTime.UtcNow;
        item.Update(quantity, notes, now);
        await _cartItemRepository.UpdateAsync(item, cancellationToken);
        cart.Touch(now);
        await _cartRepository.UpdateAsync(cart, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return Result<CartItem>.Success(item);
    }

    public async Task<bool> RemoveItemAsync(Guid cartItemId, Guid userId, CancellationToken cancellationToken)
    {
        var item = await _cartItemRepository.GetByIdAsync(cartItemId, cancellationToken);
        if (item is null)
        {
            return false;
        }

        var cart = await _cartRepository.GetByIdAsync(item.CartId, cancellationToken);
        if (cart is null || cart.UserId != userId)
        {
            return false;
        }

        await _cartItemRepository.RemoveAsync(cartItemId, cancellationToken);
        cart.Touch(DateTime.UtcNow);
        await _cartRepository.UpdateAsync(cart, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return true;
    }

    public async Task<Result<CheckoutResult>> CheckoutAsync(
        Guid territoryId,
        Guid userId,
        string? message,
        CancellationToken cancellationToken)
    {
        var cart = await _cartRepository.GetByUserAsync(territoryId, userId, cancellationToken);
        if (cart is null)
        {
            return Result<CheckoutResult>.Success(new CheckoutResult(Array.Empty<CheckoutBundle>(), Array.Empty<InquiryBundle>(), Array.Empty<CheckoutSummary>()));
        }

        var cartItems = await _cartItemRepository.ListByCartIdAsync(cart.Id, cancellationToken);
        if (cartItems.Count == 0)
        {
            return Result<CheckoutResult>.Success(new CheckoutResult(Array.Empty<CheckoutBundle>(), Array.Empty<InquiryBundle>(), Array.Empty<CheckoutSummary>()));
        }

        var listingIds = cartItems.Select(i => i.ListingId).Distinct().ToList();
        var listings = await _listingRepository.ListByIdsAsync(listingIds, cancellationToken);
        var listingMap = listings.ToDictionary(l => l.Id, l => l);

        var storeIds = listings.Select(l => l.StoreId).Distinct().ToList();
        var stores = await _storeRepository.ListByIdsAsync(storeIds, cancellationToken);
        var storeMap = stores.ToDictionary(s => s.Id, s => s);

        var groupedItems = cartItems
            .Where(item => listingMap.ContainsKey(item.ListingId))
            .GroupBy(item => listingMap[item.ListingId].StoreId)
            .ToList();

        var checkoutBundles = new List<CheckoutBundle>();
        var inquiries = new List<InquiryBundle>();
        var summaries = new List<CheckoutSummary>();

        foreach (var group in groupedItems)
        {
            if (!storeMap.TryGetValue(group.Key, out var store))
            {
                continue;
            }

            var purchasable = new List<(CartItem Item, StoreListing Listing)>();
            var nonPurchasable = new List<(CartItem Item, StoreListing Listing)>();

            foreach (var item in group)
            {
                var listing = listingMap[item.ListingId];
                if (IsPurchasable(listing, store))
                {
                    purchasable.Add((item, listing));
                }
                else
                {
                    nonPurchasable.Add((item, listing));
                }
            }

            if (purchasable.Count > 0)
            {
                var checkoutId = Guid.NewGuid();
                var currency = purchasable
                    .Select(p => p.Listing.Currency)
                    .FirstOrDefault(value => !string.IsNullOrWhiteSpace(value)) ?? "BRL";

                var checkout = new Checkout(
                    checkoutId,
                    territoryId,
                    userId,
                    store.Id,
                    CheckoutStatus.Created,
                    currency,
                    null,
                    null,
                    null,
                    DateTime.UtcNow,
                    DateTime.UtcNow);

                var checkoutItems = new List<CheckoutItem>();
                decimal subtotal = 0;
                decimal platformFeeTotal = 0;

                foreach (var (item, listing) in purchasable)
                {
                    var unitPrice = listing.PriceAmount ?? 0m;
                    var lineSubtotal = unitPrice * item.Quantity;
                    var platformFeeLine = await CalculatePlatformFeeAsync(territoryId, listing.Type, lineSubtotal, item.Quantity, cancellationToken);
                    var lineTotal = lineSubtotal + platformFeeLine;

                    checkoutItems.Add(new CheckoutItem(
                        Guid.NewGuid(),
                        checkoutId,
                        listing.Id,
                        listing.Type,
                        listing.Title,
                        item.Quantity,
                        unitPrice,
                        lineSubtotal,
                        platformFeeLine,
                        lineTotal,
                        DateTime.UtcNow));

                    subtotal += lineSubtotal;
                    platformFeeTotal += platformFeeLine;
                }

                var total = subtotal + platformFeeTotal;
                checkout.SetTotals(subtotal, platformFeeTotal, total, DateTime.UtcNow);

                await _checkoutRepository.AddAsync(checkout, cancellationToken);
                await _checkoutItemRepository.AddRangeAsync(checkoutItems, cancellationToken);

                checkoutBundles.Add(new CheckoutBundle(checkout, checkoutItems));
                summaries.Add(new CheckoutSummary(store.Id, checkoutItems.Count, nonPurchasable.Count));
            }
            else
            {
                summaries.Add(new CheckoutSummary(store.Id, 0, nonPurchasable.Count));
            }

            if (nonPurchasable.Count > 0)
            {
                var batchId = Guid.NewGuid();
                foreach (var (item, listing) in nonPurchasable)
                {
                    var inquiry = new ListingInquiry(
                        Guid.NewGuid(),
                        territoryId,
                        listing.Id,
                        store.Id,
                        userId,
                        message,
                        InquiryStatus.Open,
                        batchId,
                        DateTime.UtcNow);

                    await _inquiryRepository.AddAsync(inquiry, cancellationToken);
                    inquiries.Add(new InquiryBundle(inquiry.Id, store.Id, listing.Id, inquiry.BatchId));
                }
            }
        }

        await _cartItemRepository.RemoveByCartIdAsync(cart.Id, cancellationToken);
        cart.Touch(DateTime.UtcNow);
        await _cartRepository.UpdateAsync(cart, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        var result = new CheckoutResult(checkoutBundles, inquiries, summaries);
        return Result<CheckoutResult>.Success(result);
    }

    private static bool IsPurchasable(StoreListing listing, TerritoryStore store)
    {
        return listing.PricingType == ListingPricingType.Fixed &&
               listing.Status == ListingStatus.Active &&
               store.PaymentsEnabled;
    }

    private async Task<decimal> CalculatePlatformFeeAsync(
        Guid territoryId,
        ListingType listingType,
        decimal lineSubtotal,
        int quantity,
        CancellationToken cancellationToken)
    {
        var config = await _platformFeeConfigRepository.GetActiveAsync(territoryId, listingType, cancellationToken);
        if (config is null)
        {
            return 0m;
        }

        return config.FeeMode switch
        {
            PlatformFeeMode.Fixed => config.FeeValue * quantity,
            PlatformFeeMode.Percentage => lineSubtotal * config.FeeValue,
            _ => 0m
        };
    }

    private async Task<Cart> GetOrCreateCartAsync(Guid territoryId, Guid userId, CancellationToken cancellationToken)
    {
        var existing = await _cartRepository.GetByUserAsync(territoryId, userId, cancellationToken);
        if (existing is not null)
        {
            return existing;
        }

        var now = DateTime.UtcNow;
        var cart = new Cart(Guid.NewGuid(), territoryId, userId, now, now);
        await _cartRepository.AddAsync(cart, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return cart;
    }
}
