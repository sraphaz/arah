using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Domain.Email;
using Araponga.Domain.Marketplace;
using Microsoft.Extensions.DependencyInjection;

namespace Araponga.Application.Services;

public sealed class CartService
{
    private readonly ICartRepository _cartRepository;
    private readonly ICartItemRepository _cartItemRepository;
    private readonly IStoreItemRepository _itemRepository;
    private readonly IStoreRepository _storeRepository;
    private readonly ICheckoutRepository _checkoutRepository;
    private readonly ICheckoutItemRepository _checkoutItemRepository;
    private readonly IInquiryRepository _inquiryRepository;
    private readonly IPlatformFeeConfigRepository _platformFeeConfigRepository;
    private readonly TerritoryFeatureFlagGuard _featureGuard;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IServiceProvider? _serviceProvider;

    public CartService(
        ICartRepository cartRepository,
        ICartItemRepository cartItemRepository,
        IStoreItemRepository itemRepository,
        IStoreRepository storeRepository,
        ICheckoutRepository checkoutRepository,
        ICheckoutItemRepository checkoutItemRepository,
        IInquiryRepository inquiryRepository,
        IPlatformFeeConfigRepository platformFeeConfigRepository,
        TerritoryFeatureFlagGuard featureGuard,
        IUnitOfWork unitOfWork,
        IServiceProvider? serviceProvider = null)
    {
        _cartRepository = cartRepository;
        _cartItemRepository = cartItemRepository;
        _itemRepository = itemRepository;
        _storeRepository = storeRepository;
        _checkoutRepository = checkoutRepository;
        _checkoutItemRepository = checkoutItemRepository;
        _inquiryRepository = inquiryRepository;
        _platformFeeConfigRepository = platformFeeConfigRepository;
        _featureGuard = featureGuard;
        _unitOfWork = unitOfWork;
        _serviceProvider = serviceProvider;
    }

    public async Task<Result<CartDetails>> GetCartAsync(Guid territoryId, Guid userId, CancellationToken cancellationToken)
    {
        var gate = _featureGuard.EnsureMarketplaceEnabled(territoryId);
        if (gate.IsFailure)
        {
            return Result<CartDetails>.Failure(gate.Error ?? "Marketplace is disabled for this territory.");
        }

        var cart = await GetOrCreateCartAsync(territoryId, userId, cancellationToken);
        var details = await BuildCartDetailsAsync(cart, cancellationToken);
        return Result<CartDetails>.Success(details);
    }

    public async Task<Result<CartDetails?>> GetCartByIdAsync(Guid cartId, Guid userId, CancellationToken cancellationToken)
    {
        var cart = await _cartRepository.GetByIdAsync(cartId, cancellationToken);
        if (cart is null || cart.UserId != userId)
        {
            return Result<CartDetails?>.Success(null);
        }

        var gate = _featureGuard.EnsureMarketplaceEnabled(cart.TerritoryId);
        if (gate.IsFailure)
        {
            return Result<CartDetails?>.Failure(gate.Error ?? "Marketplace is disabled for this territory.");
        }

        var details = await BuildCartDetailsAsync(cart, cancellationToken);
        return Result<CartDetails?>.Success(details);
    }

    private async Task<CartDetails> BuildCartDetailsAsync(Cart cart, CancellationToken cancellationToken)
    {
        var items = await _cartItemRepository.ListByCartIdAsync(cart.Id, cancellationToken);

        if (items.Count == 0)
        {
            return new CartDetails(cart, Array.Empty<CartItemDetails>());
        }

        var itemIds = items.Select(i => i.ItemId).Distinct().ToList();
        var storeItems = await _itemRepository.ListByIdsAsync(itemIds, cancellationToken);
        var itemMap = storeItems.ToDictionary(l => l.Id, l => l);

        var storeIds = storeItems.Select(l => l.StoreId).Distinct().ToList();
        var stores = await _storeRepository.ListByIdsAsync(storeIds, cancellationToken);
        var storeMap = stores.ToDictionary(s => s.Id, s => s);

        var itemDetails = new List<CartItemDetails>();

        foreach (var item in items)
        {
            if (!itemMap.TryGetValue(item.ItemId, out var storeItem))
            {
                continue;
            }

            if (!storeMap.TryGetValue(storeItem.StoreId, out var store))
            {
                continue;
            }

            var isPurchasable = IsPurchasable(storeItem, store);
            itemDetails.Add(new CartItemDetails(item, storeItem, store, isPurchasable));
        }

        return new CartDetails(cart, itemDetails);
    }

    public async Task<Result<CartItem>> AddItemAsync(
        Guid territoryId,
        Guid userId,
        Guid itemId,
        int quantity,
        string? notes,
        CancellationToken cancellationToken)
    {
        var gate = _featureGuard.EnsureMarketplaceEnabled(territoryId);
        if (gate.IsFailure)
        {
            return Result<CartItem>.Failure(gate.Error ?? "Marketplace is disabled for this territory.");
        }

        if (quantity < 1)
        {
            return Result<CartItem>.Failure("Quantity must be at least 1.");
        }

        var storeItem = await _itemRepository.GetByIdAsync(itemId, cancellationToken);
        if (storeItem is null || storeItem.TerritoryId != territoryId)
        {
            return Result<CartItem>.Failure("Item not found for territory.");
        }

        var cart = await GetOrCreateCartAsync(territoryId, userId, cancellationToken);
        var existing = await _cartItemRepository.GetByCartAndListingAsync(cart.Id, itemId, cancellationToken);
        var now = DateTime.UtcNow;

        if (existing is null)
        {
            var item = new CartItem(Guid.NewGuid(), cart.Id, itemId, quantity, notes, now, now);
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
        var gate = _featureGuard.EnsureMarketplaceEnabled(territoryId);
        if (gate.IsFailure)
        {
            return Result<CheckoutResult>.Failure(gate.Error ?? "Marketplace is disabled for this territory.");
        }

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

        var itemIds = cartItems.Select(i => i.ItemId).Distinct().ToList();
        var storeItems = await _itemRepository.ListByIdsAsync(itemIds, cancellationToken);
        var itemMap = storeItems.ToDictionary(l => l.Id, l => l);

        var storeIds = storeItems.Select(l => l.StoreId).Distinct().ToList();
        var stores = await _storeRepository.ListByIdsAsync(storeIds, cancellationToken);
        var storeMap = stores.ToDictionary(s => s.Id, s => s);

        var groupedItems = cartItems
            .Where(item => itemMap.ContainsKey(item.ItemId))
            .GroupBy(item => itemMap[item.ItemId].StoreId)
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

            var purchasable = new List<(CartItem Item, StoreItem StoreItem)>();
            var nonPurchasable = new List<(CartItem Item, StoreItem StoreItem)>();

            foreach (var item in group)
            {
                var storeItem = itemMap[item.ItemId];
                if (IsPurchasable(storeItem, store))
                {
                    purchasable.Add((item, storeItem));
                }
                else
                {
                    nonPurchasable.Add((item, storeItem));
                }
            }

            if (purchasable.Count > 0)
            {
                var checkoutId = Guid.NewGuid();
                var currency = purchasable
                    .Select(p => p.StoreItem.Currency)
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

                foreach (var (item, storeItem) in purchasable)
                {
                    var unitPrice = storeItem.PriceAmount ?? 0m;
                    var lineSubtotal = unitPrice * item.Quantity;
                    var platformFeeLine = await CalculatePlatformFeeAsync(territoryId, storeItem.Type, lineSubtotal, item.Quantity, cancellationToken);
                    var lineTotal = lineSubtotal + platformFeeLine;

                    checkoutItems.Add(new CheckoutItem(
                        Guid.NewGuid(),
                        checkoutId,
                        storeItem.Id,
                        storeItem.Type,
                        storeItem.Title,
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
                foreach (var (item, storeItem) in nonPurchasable)
                {
                    var inquiry = new ItemInquiry(
                        Guid.NewGuid(),
                        territoryId,
                        storeItem.Id,
                        store.Id,
                        userId,
                        message,
                        InquiryStatus.Open,
                        batchId,
                        DateTime.UtcNow);

                    await _inquiryRepository.AddAsync(inquiry, cancellationToken);
                    inquiries.Add(new InquiryBundle(inquiry.Id, store.Id, storeItem.Id, inquiry.BatchId));
                }
            }
        }

        await _cartItemRepository.RemoveByCartIdAsync(cart.Id, cancellationToken);
        cart.Touch(DateTime.UtcNow);
        await _cartRepository.UpdateAsync(cart, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        // Enfileirar emails de confirmação de pedido (opcional - não bloqueia checkout)
        _ = Task.Run(async () =>
        {
            try
            {
                var emailQueueService = _serviceProvider?.GetService<EmailQueueService>();
                var userRepository = _serviceProvider?.GetService<IUserRepository>();
                var storeRepository = _serviceProvider?.GetService<IStoreRepository>();
                var baseUrl = "https://araponga.com";

                if (emailQueueService != null && userRepository != null && storeRepository != null)
                {
                    var user = await userRepository.GetByIdAsync(userId, CancellationToken.None);
                    if (user != null && !string.IsNullOrWhiteSpace(user.Email))
                    {
                        foreach (var bundle in checkoutBundles)
                        {
                            var store = await storeRepository.GetByIdAsync(bundle.Checkout.StoreId, CancellationToken.None);
                            if (store != null)
                            {
                                var items = bundle.Items.Select(i => new OrderItem
                                {
                                    Name = i.TitleSnapshot,
                                    Quantity = i.Quantity,
                                    UnitPrice = i.UnitPrice ?? 0
                                }).ToList();

                                var templateData = new MarketplaceOrderEmailTemplateData
                                {
                                    UserName = user.DisplayName,
                                    BaseUrl = baseUrl,
                                    OrderId = bundle.Checkout.Id,
                                    Items = items,
                                    Total = bundle.Checkout.TotalAmount ?? 0,
                                    SellerName = store.DisplayName,
                                    OrderLink = $"{baseUrl}/orders/{bundle.Checkout.Id}"
                                };

                                var emailMessage = new EmailMessage
                                {
                                    To = user.Email,
                                    Subject = "Pedido Confirmado - Araponga",
                                    Body = string.Empty,
                                    TemplateName = "marketplace-order",
                                    TemplateData = templateData,
                                    IsHtml = true
                                };

                                await emailQueueService.EnqueueEmailAsync(
                                    emailMessage,
                                    EmailQueuePriority.High,
                                    null,
                                    CancellationToken.None);
                            }
                        }
                    }
                }
            }
            catch
            {
                // Silenciar erros de email - não deve bloquear o checkout
            }
        }, cancellationToken);

        var result = new CheckoutResult(checkoutBundles, inquiries, summaries);
        return Result<CheckoutResult>.Success(result);
    }

    private static bool IsPurchasable(StoreItem storeItem, Store store)
    {
        return storeItem.PricingType == ItemPricingType.Fixed &&
               storeItem.Status == ItemStatus.Active &&
               store.PaymentsEnabled;
    }

    private async Task<decimal> CalculatePlatformFeeAsync(
        Guid territoryId,
        ItemType itemType,
        decimal lineSubtotal,
        int quantity,
        CancellationToken cancellationToken)
    {
        var config = await _platformFeeConfigRepository.GetActiveAsync(territoryId, itemType, cancellationToken);
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
