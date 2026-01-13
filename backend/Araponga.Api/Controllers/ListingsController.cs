using Araponga.Api.Contracts.Marketplace;
using Araponga.Api.Security;
using Araponga.Application.Services;
using Araponga.Domain.Marketplace;
using Microsoft.AspNetCore.Mvc;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/listings")]
[Produces("application/json")]
[Tags("Listings")]
public sealed class ListingsController : ControllerBase
{
    private readonly ListingService _listingService;
    private readonly CurrentUserAccessor _currentUserAccessor;

    public ListingsController(ListingService listingService, CurrentUserAccessor currentUserAccessor)
    {
        _listingService = listingService;
        _currentUserAccessor = currentUserAccessor;
    }

    /// <summary>
    /// Cria um listing (produto ou serviço).
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ListingResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ListingResponse>> CreateListing(
        [FromBody] CreateListingRequest request,
        CancellationToken cancellationToken)
    {
        if (request.TerritoryId == Guid.Empty || request.StoreId == Guid.Empty)
        {
            return BadRequest(new { error = "territoryId and storeId are required." });
        }

        if (!TryParseListingType(request.Type, out var type))
        {
            return BadRequest(new { error = "Invalid type." });
        }

        if (!TryParsePricingType(request.PricingType, out var pricingType))
        {
            return BadRequest(new { error = "Invalid pricingType." });
        }

        var status = ListingStatus.Active;
        if (!string.IsNullOrWhiteSpace(request.Status) && !TryParseListingStatus(request.Status, out status))
        {
            return BadRequest(new { error = "Invalid status." });
        }

        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _listingService.CreateListingAsync(
            request.TerritoryId,
            userContext.User.Id,
            request.StoreId,
            type,
            request.Title,
            request.Description,
            request.Category,
            request.Tags,
            pricingType,
            request.PriceAmount,
            request.Currency,
            request.Unit,
            request.Latitude,
            request.Longitude,
            status,
            cancellationToken);

        if (!result.success || result.listing is null)
        {
            if (result.error?.Contains("not authorized", StringComparison.OrdinalIgnoreCase) == true)
            {
                return Forbid();
            }

            return BadRequest(new { error = result.error ?? "Unable to create listing." });
        }

        return CreatedAtAction(nameof(GetListingById), new { id = result.listing.Id }, ToResponse(result.listing));
    }

    /// <summary>
    /// Atualiza um listing.
    /// </summary>
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(typeof(ListingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ListingResponse>> UpdateListing(
        [FromRoute] Guid id,
        [FromBody] UpdateListingRequest request,
        CancellationToken cancellationToken)
    {
        ListingType? type = null;
        if (!string.IsNullOrWhiteSpace(request.Type))
        {
            if (!TryParseListingType(request.Type, out var parsedType))
            {
                return BadRequest(new { error = "Invalid type." });
            }

            type = parsedType;
        }

        ListingPricingType? pricingType = null;
        if (!string.IsNullOrWhiteSpace(request.PricingType))
        {
            if (!TryParsePricingType(request.PricingType, out var parsedPricing))
            {
                return BadRequest(new { error = "Invalid pricingType." });
            }

            pricingType = parsedPricing;
        }

        ListingStatus? status = null;
        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (!TryParseListingStatus(request.Status, out var parsedStatus))
            {
                return BadRequest(new { error = "Invalid status." });
            }

            status = parsedStatus;
        }

        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _listingService.UpdateListingAsync(
            id,
            userContext.User.Id,
            type,
            request.Title,
            request.Description,
            request.Category,
            request.Tags,
            pricingType,
            request.PriceAmount,
            request.Currency,
            request.Unit,
            request.Latitude,
            request.Longitude,
            status,
            cancellationToken);

        if (!result.success || result.listing is null)
        {
            if (result.error?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
            {
                return NotFound();
            }

            return result.error?.Contains("not authorized", StringComparison.OrdinalIgnoreCase) == true
                ? Forbid()
                : BadRequest(new { error = result.error ?? "Unable to update listing." });
        }

        return Ok(ToResponse(result.listing));
    }

    /// <summary>
    /// Arquiva um listing.
    /// </summary>
    [HttpPost("{id:guid}/archive")]
    [ProducesResponseType(typeof(ListingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ListingResponse>> ArchiveListing(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _listingService.ArchiveListingAsync(id, userContext.User.Id, cancellationToken);
        if (!result.success || result.listing is null)
        {
            if (result.error?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
            {
                return NotFound();
            }

            return result.error?.Contains("not authorized", StringComparison.OrdinalIgnoreCase) == true
                ? Forbid()
                : BadRequest(new { error = result.error ?? "Unable to archive listing." });
        }

        return Ok(ToResponse(result.listing));
    }

    /// <summary>
    /// Busca listings no território.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ListingResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<ListingResponse>>> SearchListings(
        [FromQuery] Guid territoryId,
        [FromQuery] string? type,
        [FromQuery] string? q,
        [FromQuery] string? category,
        [FromQuery] string? tags,
        [FromQuery] string? status,
        CancellationToken cancellationToken)
    {
        if (territoryId == Guid.Empty)
        {
            return BadRequest(new { error = "territoryId is required." });
        }

        ListingType? parsedType = null;
        if (!string.IsNullOrWhiteSpace(type))
        {
            if (!TryParseListingType(type, out var resolvedType))
            {
                return BadRequest(new { error = "Invalid type." });
            }

            parsedType = resolvedType;
        }

        ListingStatus? parsedStatus = ListingStatus.Active;
        if (!string.IsNullOrWhiteSpace(status))
        {
            if (!TryParseListingStatus(status, out var resolvedStatus))
            {
                return BadRequest(new { error = "Invalid status." });
            }

            parsedStatus = resolvedStatus;
        }

        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid)
        {
            return Unauthorized();
        }

        var listings = await _listingService.SearchListingsAsync(
            territoryId,
            parsedType,
            q,
            category,
            tags,
            parsedStatus,
            cancellationToken);

        var response = listings.Select(ToResponse).ToList();
        return Ok(response);
    }

    /// <summary>
    /// Detalhe de um listing.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ListingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ListingResponse>> GetListingById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid)
        {
            return Unauthorized();
        }

        var listing = await _listingService.GetByIdAsync(id, cancellationToken);
        if (listing is null)
        {
            return NotFound();
        }

        return Ok(ToResponse(listing));
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

    private static bool TryParseListingType(string? raw, out ListingType type)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            type = default;
            return false;
        }

        var normalized = raw.Replace("_", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase);

        return Enum.TryParse(normalized, true, out type);
    }

    private static bool TryParsePricingType(string? raw, out ListingPricingType pricingType)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            pricingType = default;
            return false;
        }

        var normalized = raw.Replace("_", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase);

        return Enum.TryParse(normalized, true, out pricingType);
    }

    private static bool TryParseListingStatus(string? raw, out ListingStatus status)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            status = default;
            return false;
        }

        var normalized = raw.Replace("_", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase);

        return Enum.TryParse(normalized, true, out status);
    }
}
