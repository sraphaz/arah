using Araponga.Api.Contracts.Marketplace;
using Araponga.Api.Security;
using Araponga.Application.Services;
using Araponga.Domain.Marketplace;
using Microsoft.AspNetCore.Mvc;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/cart")]
[Produces("application/json")]
[Tags("Cart")]
public sealed class CartController : ControllerBase
{
    private readonly CartService _cartService;
    private readonly CurrentUserAccessor _currentUserAccessor;

    public CartController(CartService cartService, CurrentUserAccessor currentUserAccessor)
    {
        _cartService = cartService;
        _currentUserAccessor = currentUserAccessor;
    }

    /// <summary>
    /// Retorna o carrinho do usuário no território.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CartResponse>> GetCart(
        [FromQuery] Guid territoryId,
        CancellationToken cancellationToken)
    {
        if (territoryId == Guid.Empty)
        {
            return BadRequest(new { error = "territoryId is required." });
        }

        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var cart = await _cartService.GetCartAsync(territoryId, userContext.User.Id, cancellationToken);
        var response = new CartResponse(
            cart.Cart.Id,
            cart.Cart.TerritoryId,
            cart.Cart.UserId,
            cart.Cart.CreatedAtUtc,
            cart.Cart.UpdatedAtUtc,
            cart.Items.Select(ToResponse).ToList());

        return Ok(response);
    }

    /// <summary>
    /// Adiciona um item ao carrinho.
    /// </summary>
    [HttpPost("items")]
    [ProducesResponseType(typeof(CartItemResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CartItemResponse>> AddItem(
        [FromBody] AddCartItemRequest request,
        CancellationToken cancellationToken)
    {
        if (request.TerritoryId == Guid.Empty)
        {
            return BadRequest(new { error = "territoryId is required." });
        }

        if (request.ListingId == Guid.Empty)
        {
            return BadRequest(new { error = "listingId is required." });
        }

        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _cartService.AddItemAsync(
            request.TerritoryId,
            userContext.User.Id,
            request.ListingId,
            request.Quantity,
            request.Notes,
            cancellationToken);

        if (!result.success || result.item is null)
        {
            return BadRequest(new { error = result.error ?? "Unable to add item." });
        }

        var cart = await _cartService.GetCartAsync(request.TerritoryId, userContext.User.Id, cancellationToken);
        var detail = cart.Items.FirstOrDefault(i => i.Item.Id == result.item.Id);
        if (detail is null)
        {
            return BadRequest(new { error = "Unable to retrieve cart item." });
        }

        return CreatedAtAction(nameof(GetCart), new { territoryId = request.TerritoryId }, ToResponse(detail));
    }

    /// <summary>
    /// Atualiza um item do carrinho.
    /// </summary>
    [HttpPatch("items/{id:guid}")]
    [ProducesResponseType(typeof(CartItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CartItemResponse>> UpdateItem(
        [FromRoute] Guid id,
        [FromBody] UpdateCartItemRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _cartService.UpdateItemAsync(id, userContext.User.Id, request.Quantity, request.Notes, cancellationToken);
        if (!result.success || result.item is null)
        {
            return BadRequest(new { error = result.error ?? "Unable to update item." });
        }

        var cart = await _cartService.GetCartByIdAsync(result.item.CartId, userContext.User.Id, cancellationToken);
        if (cart is null)
        {
            return BadRequest(new { error = "Unable to retrieve cart item." });
        }

        var detail = cart.Items.FirstOrDefault(i => i.Item.Id == result.item.Id);
        if (detail is null)
        {
            return BadRequest(new { error = "Unable to retrieve cart item." });
        }

        return Ok(ToResponse(detail));
    }

    /// <summary>
    /// Remove um item do carrinho.
    /// </summary>
    [HttpDelete("items/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RemoveItem(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var removed = await _cartService.RemoveItemAsync(id, userContext.User.Id, cancellationToken);
        if (!removed)
        {
            return BadRequest(new { error = "Cart item not found." });
        }

        return NoContent();
    }

    /// <summary>
    /// Finaliza o checkout do carrinho.
    /// </summary>
    [HttpPost("checkout")]
    [ProducesResponseType(typeof(CartCheckoutResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CartCheckoutResponse>> Checkout(
        [FromBody] CartCheckoutRequest request,
        CancellationToken cancellationToken)
    {
        if (request.TerritoryId == Guid.Empty)
        {
            return BadRequest(new { error = "territoryId is required." });
        }

        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _cartService.CheckoutAsync(request.TerritoryId, userContext.User.Id, request.Message, cancellationToken);
        if (!result.success || result.result is null)
        {
            return BadRequest(new { error = result.error ?? "Unable to checkout." });
        }

        var response = new CartCheckoutResponse(
            result.result.Checkouts.Select(ToResponse).ToList(),
            result.result.Inquiries.Select(inquiry => new InquiryBundleResponse(
                inquiry.InquiryId,
                inquiry.StoreId,
                inquiry.ListingId,
                inquiry.BatchId)).ToList(),
            result.result.Summaries.Select(summary => new CheckoutSummaryResponse(
                summary.StoreId,
                summary.CheckoutItemCount,
                summary.InquiryCount)).ToList());

        return Ok(response);
    }

    private static CartItemResponse ToResponse(Araponga.Application.Models.CartItemDetails detail)
    {
        return new CartItemResponse(
            detail.Item.Id,
            detail.Item.ListingId,
            detail.Item.Quantity,
            detail.Item.Notes,
            detail.Item.CreatedAtUtc,
            detail.Item.UpdatedAtUtc,
            ToResponse(detail.Listing),
            ToResponse(detail.Store),
            detail.IsPurchasable);
    }

    private static ListingResponse ToResponse(StoreListing listing)
    {
        return new ListingResponse(
            listing.Id,
            listing.TerritoryId,
            listing.StoreId,
            listing.Type.ToString().ToUpperInvariant(),
            listing.Title,
            listing.Description,
            listing.Category,
            listing.Tags,
            listing.PricingType.ToString().ToUpperInvariant(),
            listing.PriceAmount,
            listing.Currency,
            listing.Unit,
            listing.Latitude,
            listing.Longitude,
            listing.Status.ToString().ToUpperInvariant(),
            listing.CreatedAtUtc,
            listing.UpdatedAtUtc);
    }

    private static StoreResponse ToResponse(TerritoryStore store)
    {
        var contact = new StoreContactPayload(
            store.Phone,
            store.Whatsapp,
            store.Email,
            store.Instagram,
            store.Website,
            store.PreferredContactMethod);

        return new StoreResponse(
            store.Id,
            store.TerritoryId,
            store.OwnerUserId,
            store.DisplayName,
            store.Description,
            store.Status.ToString().ToUpperInvariant(),
            store.PaymentsEnabled,
            store.ContactVisibility.ToString().ToUpperInvariant(),
            contact,
            store.CreatedAtUtc,
            store.UpdatedAtUtc);
    }

    private static CheckoutResponse ToResponse(Araponga.Application.Models.CheckoutBundle bundle)
    {
        return new CheckoutResponse(
            bundle.Checkout.Id,
            bundle.Checkout.TerritoryId,
            bundle.Checkout.BuyerUserId,
            bundle.Checkout.StoreId,
            bundle.Checkout.Status.ToString().ToUpperInvariant(),
            bundle.Checkout.Currency,
            bundle.Checkout.ItemsSubtotalAmount,
            bundle.Checkout.PlatformFeeAmount,
            bundle.Checkout.TotalAmount,
            bundle.Checkout.CreatedAtUtc,
            bundle.Checkout.UpdatedAtUtc,
            bundle.Items.Select(ToResponse).ToList());
    }

    private static CheckoutItemResponse ToResponse(CheckoutItem item)
    {
        return new CheckoutItemResponse(
            item.Id,
            item.ListingId,
            item.ListingType.ToString().ToUpperInvariant(),
            item.TitleSnapshot,
            item.Quantity,
            item.UnitPrice,
            item.LineSubtotal,
            item.PlatformFeeLine,
            item.LineTotal,
            item.CreatedAtUtc);
    }
}
