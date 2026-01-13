using Araponga.Api.Contracts.Marketplace;
using Araponga.Api.Security;
using Araponga.Application.Services;
using Araponga.Domain.Marketplace;
using Microsoft.AspNetCore.Mvc;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/platform-fees")]
[Produces("application/json")]
[Tags("PlatformFees")]
public sealed class PlatformFeesController : ControllerBase
{
    private readonly PlatformFeeService _platformFeeService;
    private readonly CurrentUserAccessor _currentUserAccessor;
    private readonly AccessEvaluator _accessEvaluator;

    public PlatformFeesController(
        PlatformFeeService platformFeeService,
        CurrentUserAccessor currentUserAccessor,
        AccessEvaluator accessEvaluator)
    {
        _platformFeeService = platformFeeService;
        _currentUserAccessor = currentUserAccessor;
        _accessEvaluator = accessEvaluator;
    }

    /// <summary>
    /// Lista configurações ativas de fee por território.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PlatformFeeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<PlatformFeeResponse>>> ListFees(
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

        if (!_accessEvaluator.IsCurator(userContext.User))
        {
             return StatusCode(StatusCodes.Status403Forbidden);
;
        }

        var configs = await _platformFeeService.ListActiveAsync(territoryId, cancellationToken);
        var response = configs.Select(ToResponse).ToList();
        return Ok(response);
    }

    /// <summary>
    /// Atualiza ou cria uma configuração de fee.
    /// </summary>
    [HttpPut]
    [ProducesResponseType(typeof(PlatformFeeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PlatformFeeResponse>> UpsertFee(
        [FromBody] PlatformFeeRequest request,
        CancellationToken cancellationToken)
    {
        if (request.TerritoryId == Guid.Empty)
        {
            return BadRequest(new { error = "territoryId is required." });
        }

        if (!TryParseListingType(request.ListingType, out var listingType))
        {
            return BadRequest(new { error = "Invalid listingType." });
        }

        if (!TryParseFeeMode(request.FeeMode, out var feeMode))
        {
            return BadRequest(new { error = "Invalid feeMode." });
        }

        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        if (!_accessEvaluator.IsCurator(userContext.User))
        {
            return Forbid();
        }

        var config = await _platformFeeService.UpsertFeeConfigAsync(
            request.TerritoryId,
            listingType,
            feeMode,
            request.FeeValue,
            request.Currency,
            request.IsActive,
            cancellationToken);

        return Ok(ToResponse(config));
    }

    private static PlatformFeeResponse ToResponse(PlatformFeeConfig config)
    {
        return new PlatformFeeResponse(
            config.Id,
            config.TerritoryId,
            config.ListingType.ToString().ToUpperInvariant(),
            config.FeeMode.ToString().ToUpperInvariant(),
            config.FeeValue,
            config.Currency,
            config.IsActive,
            config.CreatedAtUtc,
            config.UpdatedAtUtc);
    }

    private static bool TryParseListingType(string? raw, out ListingType listingType)
    {
        return Enum.TryParse(raw, true, out listingType);
    }

    private static bool TryParseFeeMode(string? raw, out PlatformFeeMode feeMode)
    {
        return Enum.TryParse(raw, true, out feeMode);
    }
}
