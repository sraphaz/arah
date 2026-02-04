using Araponga.Api.Security;
using Araponga.Application.Models;
using Araponga.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Araponga.Api.Controllers;

/// <summary>
/// Controller para analytics e métricas de negócio.
/// </summary>
[ApiController]
[Route("api/v1/analytics")]
[Produces("application/json")]
[Tags("Analytics")]
public sealed class AnalyticsController : ControllerBase
{
    private readonly AnalyticsService _analyticsService;
    private readonly CurrentUserAccessor _currentUserAccessor;

    public AnalyticsController(
        AnalyticsService analyticsService,
        CurrentUserAccessor currentUserAccessor)
    {
        _analyticsService = analyticsService;
        _currentUserAccessor = currentUserAccessor;
    }

    /// <summary>
    /// Obtém estatísticas de um território específico.
    /// </summary>
    [HttpGet("territories/{id}/stats")]
    [EnableRateLimiting("read")]
    [ProducesResponseType(typeof(TerritoryStats), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> GetTerritoryStats(
        [FromRoute] Guid id,
        [FromQuery] DateTime? fromUtc = null,
        [FromQuery] DateTime? toUtc = null,
        CancellationToken cancellationToken = default)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var stats = await _analyticsService.GetTerritoryStatsAsync(
            id,
            fromUtc,
            toUtc,
            cancellationToken);

        if (stats is null)
        {
            return NotFound(new { error = "Territory not found." });
        }

        return Ok(stats);
    }

    /// <summary>
    /// Obtém estatísticas da plataforma inteira.
    /// </summary>
    [HttpGet("platform/stats")]
    [EnableRateLimiting("read")]
    [ProducesResponseType(typeof(PlatformStats), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> GetPlatformStats(
        [FromQuery] DateTime? fromUtc = null,
        [FromQuery] DateTime? toUtc = null,
        CancellationToken cancellationToken = default)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        // Nota: Em produção, adicionar verificação de permissão (apenas admins)
        var stats = await _analyticsService.GetPlatformStatsAsync(
            fromUtc,
            toUtc,
            cancellationToken);

        return Ok(stats);
    }

    /// <summary>
    /// Obtém estatísticas do marketplace.
    /// </summary>
    [HttpGet("marketplace/stats")]
    [EnableRateLimiting("read")]
    [ProducesResponseType(typeof(MarketplaceStats), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> GetMarketplaceStats(
        [FromQuery] Guid? territoryId = null,
        [FromQuery] DateTime? fromUtc = null,
        [FromQuery] DateTime? toUtc = null,
        CancellationToken cancellationToken = default)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var stats = await _analyticsService.GetMarketplaceStatsAsync(
            territoryId,
            fromUtc,
            toUtc,
            cancellationToken);

        return Ok(stats);
    }
}
