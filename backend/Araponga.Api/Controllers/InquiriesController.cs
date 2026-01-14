using Araponga.Api.Contracts.Common;
using Araponga.Api.Contracts.Marketplace;
using Araponga.Api.Security;
using Araponga.Application.Common;
using Araponga.Application.Services;
using Araponga.Domain.Marketplace;
using Microsoft.AspNetCore.Mvc;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1")]
[Produces("application/json")]
[Tags("Inquiries")]
public sealed class InquiriesController : ControllerBase
{
    private readonly InquiryService _inquiryService;
    private readonly CurrentUserAccessor _currentUserAccessor;

    public InquiriesController(InquiryService inquiryService, CurrentUserAccessor currentUserAccessor)
    {
        _inquiryService = inquiryService;
        _currentUserAccessor = currentUserAccessor;
    }

    /// <summary>
    /// Cria uma inquiry para um listing.
    /// </summary>
    [HttpPost("listings/{id:guid}/inquiries")]
    [ProducesResponseType(typeof(InquiryResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<InquiryResponse>> CreateInquiry(
        [FromRoute] Guid id,
        [FromBody] CreateInquiryRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _inquiryService.CreateInquiryAsync(id, userContext.User.Id, request.Message, null, cancellationToken);
        if (!result.IsSuccess || result.Value is null)
        {
            return BadRequest(new { error = result.Error ?? "Unable to create inquiry." });
        }

        return CreatedAtAction(nameof(ListMyInquiries), new { }, ToResponse(result.Value.Inquiry, result.Value.Contact));
    }

    /// <summary>
    /// Lista inquiries enviadas pelo usuário.
    /// </summary>
    [HttpGet("inquiries/me")]
    [ProducesResponseType(typeof(IEnumerable<InquiryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<InquiryResponse>>> ListMyInquiries(CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var inquiries = await _inquiryService.ListMyInquiriesAsync(userContext.User.Id, cancellationToken);
        var response = inquiries.Select(inquiry => ToResponse(inquiry, null)).ToList();
        return Ok(response);
    }

    /// <summary>
    /// Lista inquiries enviadas pelo usuário (paginado).
    /// </summary>
    [HttpGet("inquiries/me/paged")]
    [ProducesResponseType(typeof(PagedResponse<InquiryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResponse<InquiryResponse>>> ListMyInquiriesPaged(
        CancellationToken cancellationToken,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var pagination = new PaginationParameters(pageNumber, pageSize);
        var pagedResult = await _inquiryService.ListMyInquiriesPagedAsync(userContext.User.Id, pagination, cancellationToken);
        var response = new PagedResponse<InquiryResponse>(
            pagedResult.Items.Select(inquiry => ToResponse(inquiry, null)).ToList(),
            pagedResult.PageNumber,
            pagedResult.PageSize,
            pagedResult.TotalCount,
            pagedResult.TotalPages,
            pagedResult.HasPreviousPage,
            pagedResult.HasNextPage);
        return Ok(response);
    }

    /// <summary>
    /// Lista inquiries recebidas pelo dono das lojas.
    /// </summary>
    [HttpGet("inquiries/received")]
    [ProducesResponseType(typeof(IEnumerable<InquiryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<InquiryResponse>>> ListReceivedInquiries(CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var inquiries = await _inquiryService.ListReceivedInquiriesAsync(userContext.User.Id, cancellationToken);
        var response = inquiries.Select(inquiry => ToResponse(inquiry, null)).ToList();
        return Ok(response);
    }

    /// <summary>
    /// Lista inquiries recebidas pelo dono das lojas (paginado).
    /// </summary>
    [HttpGet("inquiries/received/paged")]
    [ProducesResponseType(typeof(PagedResponse<InquiryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResponse<InquiryResponse>>> ListReceivedInquiriesPaged(
        CancellationToken cancellationToken,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var pagination = new PaginationParameters(pageNumber, pageSize);
        var pagedResult = await _inquiryService.ListReceivedInquiriesPagedAsync(userContext.User.Id, pagination, cancellationToken);
        var response = new PagedResponse<InquiryResponse>(
            pagedResult.Items.Select(inquiry => ToResponse(inquiry, null)).ToList(),
            pagedResult.PageNumber,
            pagedResult.PageSize,
            pagedResult.TotalCount,
            pagedResult.TotalPages,
            pagedResult.HasPreviousPage,
            pagedResult.HasNextPage);
        return Ok(response);
    }

    private static InquiryResponse ToResponse(ItemInquiry inquiry, Araponga.Application.Models.StoreContactInfo? contact)
    {
        StoreContactResponse? contactResponse = null;
        if (contact is not null)
        {
            contactResponse = new StoreContactResponse(
                contact.ContactVisibility.ToString().ToUpperInvariant(),
                contact.Phone,
                contact.Whatsapp,
                contact.Email,
                contact.Instagram,
                contact.Website,
                contact.PreferredContactMethod);
        }

        return new InquiryResponse(
            inquiry.Id,
            inquiry.TerritoryId,
            inquiry.ItemId,
            inquiry.StoreId,
            inquiry.FromUserId,
            inquiry.Message,
            inquiry.Status.ToString().ToUpperInvariant(),
            inquiry.BatchId,
            inquiry.CreatedAtUtc,
            contactResponse);
    }
}
