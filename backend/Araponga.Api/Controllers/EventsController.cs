using Araponga.Api.Contracts.Common;
using Araponga.Api.Contracts.Events;
using Araponga.Api.Security;
using Araponga.Application.Common;
using Araponga.Application.Services;
using Araponga.Domain.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/events")]
[Produces("application/json")]
[Tags("Events")]
public sealed class EventsController : ControllerBase
{
    private readonly EventsService _eventsService;
    private readonly CurrentUserAccessor _currentUserAccessor;

    public EventsController(EventsService eventsService, CurrentUserAccessor currentUserAccessor)
    {
        _eventsService = eventsService;
        _currentUserAccessor = currentUserAccessor;
    }

    /// <summary>
    /// Cria um evento no território.
    /// </summary>
    [HttpPost]
    [EnableRateLimiting("write")]
    [ProducesResponseType(typeof(EventResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<EventResponse>> CreateEvent(
        [FromBody] CreateEventRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _eventsService.CreateEventAsync(
            request.TerritoryId,
            userContext.User.Id,
            request.Title,
            request.Description,
            request.StartsAtUtc,
            request.EndsAtUtc,
            request.Latitude,
            request.Longitude,
            request.LocationLabel,
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            return BadRequest(new { error = result.Error ?? "Unable to create event." });
        }

        return CreatedAtAction(nameof(GetEvents), new { }, ToResponse(result.Value));
    }

    /// <summary>
    /// Atualiza um evento existente.
    /// </summary>
    [HttpPatch("{eventId:guid}")]
    [ProducesResponseType(typeof(EventResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<EventResponse>> UpdateEvent(
        [FromRoute] Guid eventId,
        [FromBody] UpdateEventRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _eventsService.UpdateEventAsync(
            eventId,
            userContext.User.Id,
            request.Title,
            request.Description,
            request.StartsAtUtc,
            request.EndsAtUtc,
            request.Latitude,
            request.Longitude,
            request.LocationLabel,
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            return BadRequest(new { error = result.Error ?? "Unable to update event." });
        }

        return Ok(ToResponse(result.Value));
    }

    /// <summary>
    /// Cancela um evento.
    /// </summary>
    [HttpPost("{eventId:guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CancelEvent(
        [FromRoute] Guid eventId,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _eventsService.CancelEventAsync(eventId, userContext.User.Id, cancellationToken);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Marca interesse em um evento.
    /// </summary>
    [HttpPost("{eventId:guid}/interest")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SetInterest(
        [FromRoute] Guid eventId,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _eventsService.SetParticipationAsync(
            eventId,
            userContext.User.Id,
            EventParticipationStatus.Interested,
            cancellationToken);

        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Confirma participação em um evento.
    /// </summary>
    [HttpPost("{eventId:guid}/confirm")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SetConfirmed(
        [FromRoute] Guid eventId,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _eventsService.SetParticipationAsync(
            eventId,
            userContext.User.Id,
            EventParticipationStatus.Confirmed,
            cancellationToken);

        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Lista eventos por território e intervalo.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EventResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<EventResponse>>> GetEvents(
        [FromQuery] Guid territoryId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] string? status,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid)
        {
            return Unauthorized();
        }

        EventStatus? parsedStatus = null;
        if (!string.IsNullOrWhiteSpace(status))
        {
            if (!Enum.TryParse<EventStatus>(status, true, out var parsed))
            {
                return BadRequest(new { error = "Invalid status." });
            }

            parsedStatus = parsed;
        }

        var events = await _eventsService.ListEventsAsync(territoryId, from, to, parsedStatus, cancellationToken);
        return Ok(events.Select(ToResponse));
    }

    /// <summary>
    /// Lista eventos do território (paginado).
    /// </summary>
    [HttpGet("paged")]
    [ProducesResponseType(typeof(PagedResponse<EventResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResponse<EventResponse>>> ListEventsPaged(
        [FromQuery] Guid territoryId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] string? status,
        CancellationToken cancellationToken,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid)
        {
            return Unauthorized();
        }

        EventStatus? parsedStatus = null;
        if (!string.IsNullOrWhiteSpace(status))
        {
            if (!Enum.TryParse<EventStatus>(status, true, out var parsed))
            {
                return BadRequest(new { error = "Invalid status." });
            }

            parsedStatus = parsed;
        }

        var pagination = new PaginationParameters(pageNumber, pageSize);
        var pagedResult = await _eventsService.ListEventsPagedAsync(territoryId, from, to, parsedStatus, pagination, cancellationToken);
        var response = new PagedResponse<EventResponse>(
            pagedResult.Items.Select(ToResponse).ToList(),
            pagedResult.PageNumber,
            pagedResult.PageSize,
            pagedResult.TotalCount,
            pagedResult.TotalPages,
            pagedResult.HasPreviousPage,
            pagedResult.HasNextPage);
        return Ok(response);
    }

    /// <summary>
    /// Lista eventos próximos de uma coordenada.
    /// </summary>
    [HttpGet("nearby")]
    [ProducesResponseType(typeof(IEnumerable<EventResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<EventResponse>>> GetNearbyEvents(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] double radiusKm,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] Guid? territoryId,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid)
        {
            return Unauthorized();
        }

        var events = await _eventsService.GetEventsNearbyAsync(
            latitude,
            longitude,
            radiusKm,
            from,
            to,
            territoryId,
            cancellationToken);

        return Ok(events.Select(ToResponse));
    }

    /// <summary>
    /// Lista eventos próximos de uma coordenada (paginado).
    /// </summary>
    [HttpGet("nearby/paged")]
    [ProducesResponseType(typeof(PagedResponse<EventResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResponse<EventResponse>>> GetNearbyEventsPaged(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] double radiusKm,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] Guid? territoryId,
        CancellationToken cancellationToken,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid)
        {
            return Unauthorized();
        }

        var pagination = new PaginationParameters(pageNumber, pageSize);
        var pagedResult = await _eventsService.GetEventsNearbyPagedAsync(
            latitude,
            longitude,
            radiusKm,
            from,
            to,
            territoryId,
            pagination,
            cancellationToken);

        var response = new PagedResponse<EventResponse>(
            pagedResult.Items.Select(ToResponse).ToList(),
            pagedResult.PageNumber,
            pagedResult.PageSize,
            pagedResult.TotalCount,
            pagedResult.TotalPages,
            pagedResult.HasPreviousPage,
            pagedResult.HasNextPage);

        return Ok(response);
    }

    private static EventResponse ToResponse(Application.Models.EventSummary summary)
    {
        var evt = summary.Event;
        return new EventResponse(
            evt.Id,
            evt.TerritoryId,
            evt.Title,
            evt.Description,
            evt.StartsAtUtc,
            evt.EndsAtUtc,
            evt.Latitude,
            evt.Longitude,
            evt.LocationLabel,
            evt.CreatedByUserId,
            summary.CreatedByDisplayName,
            evt.CreatedByMembership.ToString().ToUpperInvariant(),
            evt.Status.ToString().ToUpperInvariant(),
            summary.InterestedCount,
            summary.ConfirmedCount);
    }
}
