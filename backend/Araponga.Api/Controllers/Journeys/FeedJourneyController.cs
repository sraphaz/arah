using Araponga.Api;
using Araponga.Api.Contracts.Journeys.Feed;
using Araponga.Api.Security;
using Araponga.Api.Services.Journeys;
using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Araponga.Api.Controllers.Journeys;

/// <summary>
/// Jornadas do feed (BFF v2): feed agregado, criar post, interações.
/// </summary>
[ApiController]
[Route("api/v2/journeys/feed")]
[Produces("application/json")]
[Tags("BFF - Feed")]
public sealed class FeedJourneyController : ControllerBase
{
    private readonly IFeedJourneyService _feedJourneyService;
    private readonly CurrentUserAccessor _currentUserAccessor;
    private readonly TerritoryService _territoryService;
    private readonly IGeoConvergenceBypassService _geoBypass;

    public FeedJourneyController(
        IFeedJourneyService feedJourneyService,
        CurrentUserAccessor currentUserAccessor,
        TerritoryService territoryService,
        IGeoConvergenceBypassService geoBypass)
    {
        _feedJourneyService = feedJourneyService;
        _currentUserAccessor = currentUserAccessor;
        _territoryService = territoryService;
        _geoBypass = geoBypass;
    }

    /// <summary>
    /// Obtém o feed do território formatado para UI (posts, contadores, mídias, autores).
    /// </summary>
    [HttpGet("territory-feed")]
    [EnableRateLimiting("feed")]
    [ProducesResponseType(typeof(TerritoryFeedJourneyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TerritoryFeedJourneyResponse>> GetTerritoryFeed(
        [FromQuery] Guid territoryId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool filterByInterests = false,
        [FromQuery] Guid? mapEntityId = null,
        [FromQuery] Guid? assetId = null,
        CancellationToken cancellationToken = default)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid)
        {
            return Unauthorized();
        }

        if (territoryId == Guid.Empty)
        {
            return BadRequest(new { error = "territoryId is required." });
        }

        // Convergência geo/território: se o cliente enviar X-Geo-Latitude/X-Geo-Longitude, exige que a posição esteja no raio do território (salvo bypass por flag ou permissão).
        if (GeoHeaderReader.TryGetCoordinates(Request.Headers, out var userLat, out var userLon))
        {
            var bypass = await _geoBypass.ShouldBypassGeoEnforcementAsync(territoryId, userContext.User?.Id, cancellationToken);
            if (!bypass)
            {
                var territory = await _territoryService.GetByIdAsync(territoryId, cancellationToken);
                if (territory is null)
                {
                    return NotFound(new { error = "Territory not found." });
                }

                var maxRadiusKm = territory.RadiusKm ?? Constants.Geo.VerificationRadiusKm;
                var distanceKm = GeographyHelper.DistanceKm(userLat, userLon, territory.Latitude, territory.Longitude);
                if (distanceKm > maxRadiusKm)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new
                    {
                        error = "Geolocation does not converge with the observed territory.",
                        detail = $"User position is {distanceKm:F1} km from territory center. Maximum allowed: {maxRadiusKm:F1} km.",
                    });
                }
            }
        }

        pageSize = Math.Clamp(pageSize, 1, 100);
        var response = await _feedJourneyService.GetTerritoryFeedAsync(
            territoryId,
            userContext.User?.Id,
            pageNumber,
            pageSize,
            filterByInterests,
            mapEntityId,
            assetId,
            cancellationToken);

        if (response is null)
        {
            return BadRequest(new { error = "Could not load feed." });
        }

        return Ok(response);
    }

    /// <summary>
    /// Cria um post no território (com mídias opcionais já vinculadas).
    /// </summary>
    [HttpPost("create-post")]
    [EnableRateLimiting("write")]
    [ProducesResponseType(typeof(CreatePostJourneyResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<CreatePostJourneyResponse>> CreatePost(
        [FromQuery] Guid territoryId,
        [FromBody] CreatePostJourneyRequest request,
        CancellationToken cancellationToken = default)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        var user = userContext.User;
        if (userContext.Status != TokenStatus.Valid || user is null)
        {
            return Unauthorized();
        }

        if (territoryId == Guid.Empty)
        {
            return BadRequest(new { error = "territoryId is required." });
        }

        // Convergência geo/território ao criar post (salvo bypass por flag ou permissão)
        if (GeoHeaderReader.TryGetCoordinates(Request.Headers, out var userLat, out var userLon))
        {
            var bypass = await _geoBypass.ShouldBypassGeoEnforcementAsync(territoryId, user.Id, cancellationToken);
            if (!bypass)
            {
                var territory = await _territoryService.GetByIdAsync(territoryId, cancellationToken);
                if (territory is null)
                {
                    return NotFound(new { error = "Territory not found." });
                }

                var maxRadiusKm = territory.RadiusKm ?? Constants.Geo.VerificationRadiusKm;
                var distanceKm = GeographyHelper.DistanceKm(userLat, userLon, territory.Latitude, territory.Longitude);
                if (distanceKm > maxRadiusKm)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new
                    {
                        error = "Geolocation does not converge with the observed territory.",
                        detail = $"User position is {distanceKm:F1} km from territory center. Maximum allowed: {maxRadiusKm:F1} km.",
                    });
                }
            }
        }

        // MediaIds podem vir no body se o cliente fez upload prévio; senão envie null
        var response = await _feedJourneyService.CreatePostAsync(
            territoryId,
            user.Id,
            request with { TerritoryId = territoryId },
            request.MediaIds,
            cancellationToken);

        if (response is null)
        {
            return BadRequest(new { error = "Invalid request or unable to create post." });
        }

        return CreatedAtAction(nameof(GetTerritoryFeed), new { territoryId }, response);
    }

    /// <summary>
    /// Interage com um post (like, comment ou share) e retorna o estado atualizado.
    /// </summary>
    [HttpPost("interact")]
    [EnableRateLimiting("write")]
    [ProducesResponseType(typeof(PostInteractionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PostInteractionResponse>> Interact(
        [FromBody] PostInteractionRequestWithComment request,
        CancellationToken cancellationToken = default)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status == TokenStatus.Invalid)
        {
            return Unauthorized();
        }

        var sessionId = Request.Headers.TryGetValue(ApiHeaders.SessionId, out var h) && !string.IsNullOrWhiteSpace(h)
            ? h.ToString()
            : userContext.User?.Id.ToString() ?? string.Empty;

        var interactionRequest = new PostInteractionRequest(
            request.PostId,
            request.TerritoryId,
            request.Action);

        var response = await _feedJourneyService.InteractAsync(
            request.TerritoryId,
            userContext.User?.Id,
            sessionId,
            interactionRequest,
            request.CommentContent,
            cancellationToken);

        if (response is null)
        {
            return BadRequest(new { error = "Invalid action or unable to perform interaction." });
        }

        return Ok(response);
    }
}
