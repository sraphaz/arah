using Araponga.Api.Contracts.Common;
using Araponga.Api.Contracts.Events;
using Araponga.Api.Security;
using Araponga.Application.Common;
using Araponga.Application.Services;
using Araponga.Domain.Events;
using Araponga.Domain.Media;
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
    private readonly MediaService _mediaService;
    private readonly CurrentUserAccessor _currentUserAccessor;

    public EventsController(
        EventsService eventsService,
        MediaService mediaService,
        CurrentUserAccessor currentUserAccessor)
    {
        _eventsService = eventsService;
        _mediaService = mediaService;
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
            request.CoverMediaId,
            request.AdditionalMediaIds,
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            return BadRequest(new { error = result.Error ?? "Unable to create event." });
        }

        var mediaUrls = await LoadMediaUrlsForEventAsync(result.Value.Event.Id, cancellationToken);
        return CreatedAtAction(nameof(GetEvents), new { }, ToResponse(result.Value, mediaUrls.CoverImageUrl, mediaUrls.AdditionalImageUrls));
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
            request.CoverMediaId,
            request.AdditionalMediaIds,
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            return BadRequest(new { error = result.Error ?? "Unable to update event." });
        }

        var mediaUrls = await LoadMediaUrlsForEventAsync(result.Value.Event.Id, cancellationToken);
        return Ok(ToResponse(result.Value, mediaUrls.CoverImageUrl, mediaUrls.AdditionalImageUrls));
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
        var responses = new List<EventResponse>();
        foreach (var evt in events)
        {
            var mediaUrls = await LoadMediaUrlsForEventAsync(evt.Event.Id, cancellationToken);
            responses.Add(ToResponse(evt, mediaUrls.CoverImageUrl, mediaUrls.AdditionalImageUrls));
        }
        return Ok(responses);
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
        var items = new List<EventResponse>();
        foreach (var evt in pagedResult.Items)
        {
            var mediaUrls = await LoadMediaUrlsForEventAsync(evt.Event.Id, cancellationToken);
            items.Add(ToResponse(evt, mediaUrls.CoverImageUrl, mediaUrls.AdditionalImageUrls));
        }
        const int maxInt32 = int.MaxValue;
        var safeTotalCount = pagedResult.TotalCount > maxInt32 ? maxInt32 : pagedResult.TotalCount;
        var safeTotalPages = pagedResult.TotalPages > maxInt32 ? maxInt32 : pagedResult.TotalPages;
        var response = new PagedResponse<EventResponse>(
            items,
            pagedResult.PageNumber,
            pagedResult.PageSize,
            safeTotalCount,
            safeTotalPages,
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

        var responses = new List<EventResponse>();
        foreach (var evt in events)
        {
            var mediaUrls = await LoadMediaUrlsForEventAsync(evt.Event.Id, cancellationToken);
            responses.Add(ToResponse(evt, mediaUrls.CoverImageUrl, mediaUrls.AdditionalImageUrls));
        }
        return Ok(responses);
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

        var items = new List<EventResponse>();
        foreach (var evt in pagedResult.Items)
        {
            var mediaUrls = await LoadMediaUrlsForEventAsync(evt.Event.Id, cancellationToken);
            items.Add(ToResponse(evt, mediaUrls.CoverImageUrl, mediaUrls.AdditionalImageUrls));
        }
        const int maxInt32 = int.MaxValue;
        var safeTotalCount = pagedResult.TotalCount > maxInt32 ? maxInt32 : pagedResult.TotalCount;
        var safeTotalPages = pagedResult.TotalPages > maxInt32 ? maxInt32 : pagedResult.TotalPages;
        var response = new PagedResponse<EventResponse>(
            items,
            pagedResult.PageNumber,
            pagedResult.PageSize,
            safeTotalCount,
            safeTotalPages,
            pagedResult.HasPreviousPage,
            pagedResult.HasNextPage);

        return Ok(response);
    }

    private static EventResponse ToResponse(
        Application.Models.EventSummary summary,
        string? coverImageUrl = null,
        IReadOnlyCollection<string>? additionalImageUrls = null)
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
            summary.ConfirmedCount,
            coverImageUrl,
            additionalImageUrls);
    }

    private async Task<(string? CoverImageUrl, IReadOnlyCollection<string> AdditionalImageUrls)> LoadMediaUrlsForEventAsync(
        Guid eventId,
        CancellationToken cancellationToken)
    {
        var mediaAssets = await _mediaService.ListMediaByOwnerAsync(MediaOwnerType.Event, eventId, cancellationToken);
        if (mediaAssets.Count == 0)
        {
            return (null, Array.Empty<string>());
        }

        // MediaService.ListMediaByOwnerAsync retorna mídias ordenadas por DisplayOrder
        // DisplayOrder = 0 é sempre a capa (se existir), DisplayOrder >= 1 são adicionais
        string? coverImageUrl = null;
        var additionalImageUrls = new List<string>();
        bool isFirst = true;

        foreach (var mediaAsset in mediaAssets)
        {
            var urlResult = await _mediaService.GetMediaUrlAsync(mediaAsset.Id, null, cancellationToken);
            if (urlResult.IsSuccess && urlResult.Value is not null)
            {
                // Primeira mídia (DisplayOrder = 0) é sempre a capa se existir
                // Caso contrário, todas são adicionais (DisplayOrder >= 1)
                if (isFirst)
                {
                    coverImageUrl = urlResult.Value;
                    isFirst = false;
                }
                else
                {
                    additionalImageUrls.Add(urlResult.Value);
                }
            }
        }

        return (coverImageUrl, additionalImageUrls);
    }

    /// <summary>
    /// Lista participantes de um evento.
    /// </summary>
    /// <remarks>
    /// Retorna lista de participantes confirmados ou interessados. Pode filtrar por status.
    /// </remarks>
    [HttpGet("{eventId:guid}/participants")]
    [ProducesResponseType(typeof(IEnumerable<EventParticipantResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<EventParticipantResponse>>> GetEventParticipants(
        [FromRoute] Guid eventId,
        [FromQuery] string? status,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid)
        {
            return Unauthorized();
        }

        EventParticipationStatus? parsedStatus = null;
        if (!string.IsNullOrWhiteSpace(status))
        {
            if (!Enum.TryParse<EventParticipationStatus>(status, true, out var parsed))
            {
                return BadRequest(new { error = "Invalid status. Valid values: Interested, Confirmed." });
            }
            parsedStatus = parsed;
        }

        var result = await _eventsService.GetEventParticipantsAsync(eventId, parsedStatus, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
            {
                return NotFound(new { error = result.Error });
            }
            return BadRequest(new { error = result.Error });
        }

        var responses = result.Value?.Select(p => new EventParticipantResponse(
            p.UserId,
            p.DisplayName,
            p.Status.ToString().ToUpperInvariant(),
            p.CreatedAtUtc,
            p.UpdatedAtUtc)) ?? Array.Empty<EventParticipantResponse>();

        return Ok(responses);
    }
}
