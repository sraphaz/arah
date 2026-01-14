namespace Araponga.Api.Contracts.Marketplace;

/// <summary>
/// Request para adicionar um item ao carrinho.
/// </summary>
/// <param name="TerritoryId">Identificador do território</param>
/// <param name="ItemId">Identificador do item (produto ou serviço)</param>
/// <param name="Quantity">Quantidade do item</param>
/// <param name="Notes">Notas adicionais sobre o item</param>
public sealed record AddCartItemRequest(
    Guid TerritoryId,
    Guid ItemId,
    int Quantity,
    string? Notes);

/// <summary>
/// Request para atualizar um item do carrinho.
/// </summary>
/// <param name="Quantity">Nova quantidade</param>
/// <param name="Notes">Novas notas</param>
public sealed record UpdateCartItemRequest(
    int Quantity,
    string? Notes);

/// <summary>
/// Response com os dados de um item do carrinho.
/// </summary>
/// <param name="Id">Identificador do item do carrinho</param>
/// <param name="ItemId">Identificador do item (produto ou serviço)</param>
/// <param name="Quantity">Quantidade</param>
/// <param name="Notes">Notas</param>
/// <param name="CreatedAtUtc">Data de criação</param>
/// <param name="UpdatedAtUtc">Data da última atualização</param>
/// <param name="Item">Dados do item</param>
/// <param name="Store">Dados da loja</param>
/// <param name="IsPurchasable">Indica se o item pode ser comprado</param>
public sealed record CartItemResponse(
    Guid Id,
    Guid ItemId,
    int Quantity,
    string? Notes,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc,
    ItemResponse Item,
    StoreResponse Store,
    bool IsPurchasable);

public sealed record CartResponse(
    Guid CartId,
    Guid TerritoryId,
    Guid UserId,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc,
    IReadOnlyList<CartItemResponse> Items);

public sealed record CartCheckoutRequest(
    Guid TerritoryId,
    string? Message);

/// <summary>
/// Response com os dados de um item no checkout.
/// </summary>
/// <param name="Id">Identificador do item do checkout</param>
/// <param name="ItemId">Identificador do item (produto ou serviço)</param>
/// <param name="ItemType">Tipo do item (PRODUCT ou SERVICE)</param>
/// <param name="TitleSnapshot">Título do item no momento do checkout</param>
/// <param name="Quantity">Quantidade</param>
/// <param name="UnitPrice">Preço unitário</param>
/// <param name="LineSubtotal">Subtotal da linha</param>
/// <param name="PlatformFeeLine">Taxa da plataforma na linha</param>
/// <param name="LineTotal">Total da linha</param>
/// <param name="CreatedAtUtc">Data de criação</param>
public sealed record CheckoutItemResponse(
    Guid Id,
    Guid ItemId,
    string ItemType,
    string TitleSnapshot,
    int Quantity,
    decimal? UnitPrice,
    decimal? LineSubtotal,
    decimal? PlatformFeeLine,
    decimal? LineTotal,
    DateTime CreatedAtUtc);

public sealed record CheckoutResponse(
    Guid Id,
    Guid TerritoryId,
    Guid BuyerUserId,
    Guid StoreId,
    string Status,
    string Currency,
    decimal? ItemsSubtotalAmount,
    decimal? PlatformFeeAmount,
    decimal? TotalAmount,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc,
    IReadOnlyList<CheckoutItemResponse> Items);

/// <summary>
/// Response com os dados de uma inquiry criada durante o checkout.
/// </summary>
/// <param name="Id">Identificador da inquiry</param>
/// <param name="StoreId">Identificador da loja</param>
/// <param name="ItemId">Identificador do item</param>
/// <param name="BatchId">Identificador do lote (se aplicável)</param>
public sealed record InquiryBundleResponse(
    Guid Id,
    Guid StoreId,
    Guid ItemId,
    Guid? BatchId);

public sealed record CheckoutSummaryResponse(
    Guid StoreId,
    int CheckoutItemCount,
    int InquiryCount);

public sealed record CartCheckoutResponse(
    IReadOnlyList<CheckoutResponse> Checkouts,
    IReadOnlyList<InquiryBundleResponse> Inquiries,
    IReadOnlyList<CheckoutSummaryResponse> Summaries);
