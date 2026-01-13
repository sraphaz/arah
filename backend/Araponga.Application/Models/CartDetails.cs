using Araponga.Domain.Marketplace;

namespace Araponga.Application.Models;

public sealed record CartItemDetails(
    CartItem Item,
    StoreListing Listing,
    TerritoryStore Store,
    bool IsPurchasable);

public sealed record CartDetails(
    Cart Cart,
    IReadOnlyList<CartItemDetails> Items);
