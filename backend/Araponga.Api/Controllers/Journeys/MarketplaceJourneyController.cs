using Araponga.Api;
using Araponga.Api.Contracts.Journeys.Common;
using Araponga.Api.Contracts.Journeys.Marketplace;
using Araponga.Api.Security;
using Araponga.Application.Common;
using Araponga.Application.Models;
using Araponga.Application.Services;
using Araponga.Modules.Marketplace.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Araponga.Api.Controllers.Journeys;

/// <summary>
/// Jornadas do marketplace (BFF v2): busca, adicionar ao carrinho, checkout.
/// </summary>
[ApiController]
[Route("api/v2/journeys/marketplace")]
[Produces("application/json")]
[Tags("BFF - Marketplace")]
public sealed class MarketplaceJourneyController : ControllerBase
{
    private readonly MarketplaceSearchService _searchService;
    private readonly CartService _cartService;
    private readonly CurrentUserAccessor _currentUserAccessor;

    public MarketplaceJourneyController(
        MarketplaceSearchService searchService,
        CartService cartService,
        CurrentUserAccessor currentUserAccessor)
    {
        _searchService = searchService;
        _cartService = cartService;
        _currentUserAccessor = currentUserAccessor;
    }

    /// <summary>
    /// Busca itens no marketplace formatados para UI.
    /// </summary>
    [HttpGet("search")]
    [EnableRateLimiting("feed")]
    [ProducesResponseType(typeof(MarketplaceSearchJourneyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<MarketplaceSearchJourneyResponse>> Search(
        [FromQuery] Guid territoryId,
        [FromQuery] string? query,
        [FromQuery] string? category,
        [FromQuery] string? type,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid)
            return Unauthorized();

        if (territoryId == Guid.Empty)
            return BadRequest(new { error = "territoryId is required." });

        pageSize = Math.Clamp(pageSize, 1, 100);
        var filters = new SearchFilters
        {
            Query = query,
            Category = category,
            MinPrice = minPrice,
            MaxPrice = maxPrice
        };
        var pagination = new PaginationParameters(pageNumber, pageSize);
        var result = await _searchService.SearchAllAsync(territoryId, filters, pagination, cancellationToken);

        if (!result.IsSuccess || result.Value is null)
            return BadRequest(new { error = result.Error ?? "Search failed." });

        var combined = result.Value;
        var storeById = combined.Stores.ToDictionary(s => s.Store.Id, s => s.Store);

        var items = combined.Items.Select(r => ToItemJourney(r, storeById)).ToList();
        var stores = combined.Stores.Select(s => ToStoreSummary(s.Store)).ToList();

        const int maxInt32 = int.MaxValue;
        var totalCount = combined.TotalCount > maxInt32 ? maxInt32 : combined.TotalCount;
        var totalPages = pageSize > 0 ? (int)Math.Ceiling(totalCount / (double)pageSize) : 0;
        var paginationDto = new JourneyPaginationDto(
            pageNumber,
            pageSize,
            totalCount,
            totalPages,
            pageNumber > 1,
            pageNumber < totalPages);

        var response = new MarketplaceSearchJourneyResponse(
            items,
            stores,
            paginationDto,
            new MarketplaceFiltersDto(
                Array.Empty<string>(),
                new[] { "PRODUCT", "SERVICE" },
                new PriceRangeDto(minPrice, maxPrice)),
            null);

        return Ok(response);
    }

    /// <summary>
    /// Adiciona item ao carrinho e retorna carrinho atualizado.
    /// </summary>
    [HttpPost("add-to-cart")]
    [EnableRateLimiting("write")]
    [ProducesResponseType(typeof(AddToCartJourneyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AddToCartJourneyResponse>> AddToCart(
        [FromBody] AddToCartJourneyRequest request,
        CancellationToken cancellationToken = default)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
            return Unauthorized();

        if (request.TerritoryId == Guid.Empty || request.ItemId == Guid.Empty)
            return BadRequest(new { error = "territoryId and itemId are required." });

        request = request with { Quantity = Math.Clamp(request.Quantity, 1, 999) };

        var addResult = await _cartService.AddItemAsync(
            request.TerritoryId,
            userContext.User.Id,
            request.ItemId,
            request.Quantity,
            request.Notes,
            cancellationToken);

        if (!addResult.IsSuccess)
        {
            if (string.Equals(addResult.Error, "Marketplace is disabled for this territory.", StringComparison.Ordinal))
                return NotFound();
            return BadRequest(new { error = addResult.Error ?? "Unable to add item." });
        }

        var cartResult = await _cartService.GetCartAsync(request.TerritoryId, userContext.User.Id, cancellationToken);
        if (!cartResult.IsSuccess || cartResult.Value is null)
            return BadRequest(new { error = "Unable to load cart." });

        var cart = cartResult.Value;
        var cartDto = ToCartJourneyDto(cart);
        var addedDetail = cart.Items.FirstOrDefault(i => i.Item.ItemId == request.ItemId);
        var itemDto = addedDetail is null ? null : ToMarketplaceItemJourney(addedDetail);

        var response = new AddToCartJourneyResponse(true, cartDto, itemDto, cartDto.Total, null);
        return Ok(response);
    }

    /// <summary>
    /// Finaliza o checkout do carrinho.
    /// </summary>
    [HttpPost("checkout")]
    [EnableRateLimiting("write")]
    [ProducesResponseType(typeof(CheckoutJourneyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CheckoutJourneyResponse>> Checkout(
        [FromBody] CheckoutJourneyRequest request,
        CancellationToken cancellationToken = default)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
            return Unauthorized();

        if (request.TerritoryId == Guid.Empty)
            return BadRequest(new { error = "territoryId is required." });

        var result = await _cartService.CheckoutAsync(
            request.TerritoryId,
            userContext.User.Id,
            request.Message,
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            if (string.Equals(result.Error, "Marketplace is disabled for this territory.", StringComparison.Ordinal))
                return NotFound();
            return BadRequest(new { error = result.Error ?? "Unable to checkout." });
        }

        var value = result.Value;
        var orders = value.Checkouts.Select(ToOrderSummary).ToList();
        var summaries = value.Summaries.Select(s => new CheckoutSummaryDto(s.StoreId, s.CheckoutItemCount, s.InquiryCount)).ToList();
        var response = new CheckoutJourneyResponse(true, orders, summaries);
        return Ok(response);
    }

    private static MarketplaceItemJourneyDto ToItemJourney(ItemSearchResult r, Dictionary<Guid, Store> storeById)
    {
        var item = r.Item;
        storeById.TryGetValue(item.StoreId, out var store);
        var storeSummary = store is null
            ? new MarketplaceStoreSummaryDto(item.StoreId, item.TerritoryId, "", null, Guid.Empty, null, "ACTIVE")
            : ToStoreSummary(store);
        var itemSummary = new MarketplaceItemSummaryDto(
            item.Id,
            item.TerritoryId,
            item.StoreId,
            item.Type.ToString().ToUpperInvariant(),
            item.Title,
            item.Description,
            item.Category,
            item.PricingType.ToString().ToUpperInvariant(),
            item.PriceAmount,
            item.Currency,
            item.Unit,
            item.Status.ToString().ToUpperInvariant());
        var media = new MarketplaceItemMediaDto(null, Array.Empty<string>());
        var availability = new MarketplaceAvailabilityDto(true, null);
        return new MarketplaceItemJourneyDto(itemSummary, storeSummary, media, availability);
    }

    private static MarketplaceStoreSummaryDto ToStoreSummary(Store s)
    {
        return new MarketplaceStoreSummaryDto(
            s.Id,
            s.TerritoryId,
            s.DisplayName,
            s.Description,
            s.OwnerUserId,
            null,
            s.Status.ToString().ToUpperInvariant());
    }

    private static CartJourneyDto ToCartJourneyDto(CartDetails cart)
    {
        var items = cart.Items.Select(d => new CartItemJourneyDto(
            d.Item.Id,
            ToItemSummary(d.StoreItem),
            ToStoreSummary(d.Store),
            d.Item.Quantity,
            d.Item.Notes,
            d.IsPurchasable)).ToList();

        decimal subtotal = 0;
        foreach (var d in cart.Items)
        {
            if (d.StoreItem.PriceAmount.HasValue)
                subtotal += d.StoreItem.PriceAmount.Value * d.Item.Quantity;
        }
        var total = new CartTotalDto(subtotal, 0, subtotal, null);

        return new CartJourneyDto(cart.Cart.Id, cart.Cart.TerritoryId, items, total);
    }

    private static MarketplaceItemSummaryDto ToItemSummary(StoreItem item)
    {
        return new MarketplaceItemSummaryDto(
            item.Id,
            item.TerritoryId,
            item.StoreId,
            item.Type.ToString().ToUpperInvariant(),
            item.Title,
            item.Description,
            item.Category,
            item.PricingType.ToString().ToUpperInvariant(),
            item.PriceAmount,
            item.Currency,
            item.Unit,
            item.Status.ToString().ToUpperInvariant());
    }

    private static MarketplaceItemJourneyDto ToMarketplaceItemJourney(CartItemDetails d)
    {
        return new MarketplaceItemJourneyDto(
            ToItemSummary(d.StoreItem),
            ToStoreSummary(d.Store),
            null,
            new MarketplaceAvailabilityDto(true, null));
    }

    private static OrderSummaryDto ToOrderSummary(CheckoutBundle bundle)
    {
        var total = new CartTotalDto(
            bundle.Checkout.ItemsSubtotalAmount ?? 0,
            bundle.Checkout.PlatformFeeAmount ?? 0,
            bundle.Checkout.TotalAmount ?? 0,
            bundle.Checkout.Currency);
        var items = bundle.Items.Select(i => new CheckoutItemSummaryDto(
            i.Id,
            i.ItemId,
            i.TitleSnapshot,
            i.Quantity,
            i.UnitPrice,
            i.LineTotal)).ToList();
        return new OrderSummaryDto(
            bundle.Checkout.Id,
            bundle.Checkout.TerritoryId,
            bundle.Checkout.StoreId,
            bundle.Checkout.Status.ToString().ToUpperInvariant(),
            total,
            items,
            bundle.Checkout.CreatedAtUtc);
    }
}
