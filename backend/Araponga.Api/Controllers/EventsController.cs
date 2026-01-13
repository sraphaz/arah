using Araponga.Api.Contracts.Events;
using Araponga.Api.Security;
using Araponga.Application.Services;
using Araponga.Domain.Events;
using Microsoft.AspNetCore.Mvc;

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
    [ProducesResponseType(typeof(EventResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

        if (!result.success || result.summary is null)
        {
            return BadRequest(new { error = result.error ?? "Unable to create event." });
        }

        return CreatedAtAction(nameof(GetEvents), new { }, ToResponse(result.summary));
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

        if (!result.success || result.summary is null)
        {
            return BadRequest(new { error = result.error ?? "Unable to update event." });
        }

        return Ok(ToResponse(result.summary));
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
        return result.success ? NoContent() : BadRequest(new { error = result.error });
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

        return result.success ? NoContent() : BadRequest(new { error = result.error });
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

        return result.success ? NoContent() : BadRequest(new { error = result.error });
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
