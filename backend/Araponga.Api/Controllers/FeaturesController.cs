using Araponga.Api.Contracts.Features;
using Araponga.Api.Security;
using Araponga.Application.Models;
using Araponga.Application.Services;
using Araponga.Domain.Membership;
using Microsoft.AspNetCore.Mvc;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/territories/{territoryId:guid}/features")]
[Produces("application/json")]
[Tags("Features")]
public sealed class FeaturesController : ControllerBase
{
    private readonly FeatureFlagService _featureFlags;
    private readonly CurrentUserAccessor _currentUserAccessor;
    private readonly AccessEvaluator _accessEvaluator;

    public FeaturesController(
        FeatureFlagService featureFlags,
        CurrentUserAccessor currentUserAccessor,
        AccessEvaluator accessEvaluator)
    {
        _featureFlags = featureFlags;
        _currentUserAccessor = currentUserAccessor;
        _accessEvaluator = accessEvaluator;
    }

    /// <summary>
    /// Lista flags habilitadas no território.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(FeatureFlagResponse), StatusCodes.Status200OK)]
    public ActionResult<FeatureFlagResponse> Get([FromRoute] Guid territoryId)
    {
        var flags = _featureFlags.GetEnabledFlags(territoryId)
            .Select(flag => flag.ToString().ToUpperInvariant())
            .ToList();

        return Ok(new FeatureFlagResponse(territoryId, flags));
    }

    /// <summary>
    /// Atualiza flags habilitadas (curadoria).
    /// </summary>
    [HttpPut]
    [ProducesResponseType(typeof(FeatureFlagResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<FeatureFlagResponse>> Update(
        [FromRoute] Guid territoryId,
        [FromBody] UpdateFeatureFlagsRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var isCurator = await _accessEvaluator.HasCapabilityAsync(userContext.User.Id, territoryId, MembershipCapabilityType.Curator, cancellationToken);
        if (!isCurator)
        {
            return Unauthorized();
        }

        var parsed = new List<FeatureFlag>();
        foreach (var flag in request.EnabledFlags)
        {
            if (!Enum.TryParse<FeatureFlag>(flag, true, out var parsedFlag))
            {
                return BadRequest(new { error = $"Invalid flag: {flag}" });
            }

            parsed.Add(parsedFlag);
        }

        await _featureFlags.SetEnabledFlagsAsync(territoryId, parsed, cancellationToken);

        var response = new FeatureFlagResponse(
            territoryId,
            _featureFlags.GetEnabledFlags(territoryId).Select(f => f.ToString().ToUpperInvariant()).ToList());

        return Ok(response);
    }
}
