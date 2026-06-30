using Arah.Api;
using Arah.Api.Contracts.Journeys.Onboarding;
using Arah.Api.Security;
using Arah.Api.Services.Journeys;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Arah.Api.Controllers.Journeys;

/// <summary>
/// Jornadas de onboarding e primeiro acesso (BFF v2).
/// </summary>
[ApiController]
[Route("api/v2/journeys/onboarding")]
[Produces("application/json")]
[Tags("BFF - Onboarding")]
public sealed class OnboardingJourneyController : ControllerBase
{
    private readonly IOnboardingJourneyService _onboardingService;
    private readonly CurrentUserAccessor _currentUserAccessor;

    public OnboardingJourneyController(
        IOnboardingJourneyService onboardingService,
        CurrentUserAccessor currentUserAccessor)
    {
        _onboardingService = onboardingService;
        _currentUserAccessor = currentUserAccessor;
    }

    /// <summary>
    /// Completa o onboarding do usuário: seleciona território, entra como visitante e retorna contexto inicial.
    /// </summary>
    [HttpPost("complete")]
    [EnableRateLimiting("write")]
    [ProducesResponseType(typeof(CompleteOnboardingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<CompleteOnboardingResponse>> CompleteOnboarding(
        [FromBody] CompleteOnboardingRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        if (!TryGetSessionId(out var sessionId))
        {
            return BadRequest(new { error = "Header X-Session-Id is required for onboarding." });
        }

        var response = await _onboardingService.CompleteOnboardingAsync(
            userContext.User.Id,
            sessionId!,
            request.SelectedTerritoryId,
            cancellationToken);

        if (response is null)
        {
            return BadRequest(new { error = "Territory not found or could not complete onboarding." });
        }

        return Ok(response);
    }

    /// <summary>
    /// Obtém territórios sugeridos com base na localização do usuário.
    /// </summary>
    [HttpGet("suggested-territories")]
    [EnableRateLimiting("read")]
    [ProducesResponseType(typeof(SuggestedTerritoriesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SuggestedTerritoriesResponse>> GetSuggestedTerritories(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] double radiusKm = 10,
        CancellationToken cancellationToken = default)
    {
        if (latitude < -90 || latitude > 90 || longitude < -180 || longitude > 180)
        {
            return BadRequest(new { error = "Invalid latitude or longitude." });
        }

        radiusKm = Math.Clamp(radiusKm, 1, 100);
        var response = await _onboardingService.GetSuggestedTerritoriesAsync(latitude, longitude, radiusKm, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Cadastra ou reutiliza território do município (contorno IBGE) com base na localização do usuário.
    /// </summary>
    [HttpPost("suggest-municipality")]
    [EnableRateLimiting("write")]
    [ProducesResponseType(typeof(SuggestMunicipalityResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<SuggestMunicipalityResponse>> SuggestMunicipality(
        [FromBody] SuggestMunicipalityRequest request,
        CancellationToken cancellationToken = default)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        if (request.Latitude < -90 || request.Latitude > 90 ||
            request.Longitude < -180 || request.Longitude > 180)
        {
            return BadRequest(new { error = "Invalid latitude or longitude." });
        }

        var response = await _onboardingService.SuggestMunicipalityAsync(
            request.Latitude,
            request.Longitude,
            request.City,
            request.State,
            cancellationToken);

        if (response is null)
        {
            return BadRequest(new { error = "Could not resolve municipality or IBGE boundary for this location." });
        }

        return Ok(response);
    }

    /// <summary>
    /// Propõe território desenhado manualmente (status Pending até validação de curador).
    /// </summary>
    [HttpPost("propose-territory")]
    [EnableRateLimiting("write")]
    [ProducesResponseType(typeof(ProposeTerritoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ProposeTerritoryResponse>> ProposeTerritory(
        [FromBody] ProposeTerritoryRequest request,
        CancellationToken cancellationToken = default)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        if (request.Latitude < -90 || request.Latitude > 90 ||
            request.Longitude < -180 || request.Longitude > 180 ||
            string.IsNullOrWhiteSpace(request.City) ||
            string.IsNullOrWhiteSpace(request.State))
        {
            return BadRequest(new { error = "Invalid location or city/state." });
        }

        var response = await _onboardingService.ProposeTerritoryAsync(
            userContext.User.Id,
            request,
            cancellationToken);

        if (response is null)
        {
            return BadRequest(new { error = "Could not create territory proposal." });
        }

        return Ok(response);
    }

    private bool TryGetSessionId(out string? sessionId)
    {
        if (Request.Headers.TryGetValue(ApiHeaders.SessionId, out var header) && !string.IsNullOrWhiteSpace(header))
        {
            sessionId = header.ToString();
            return true;
        }
        sessionId = null;
        return false;
    }
}
