using Araponga.Domain.Marketplace;

namespace Araponga.Application.Models;

public sealed record CartItemDetails(
    CartItem Item,
    StoreItem StoreItem,
    Store Store,
    bool IsPurchasable);

public sealed record CartDetails(
    Cart Cart,
    IReadOnlyList<CartItemDetails> Items);
