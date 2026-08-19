using Arah.Api;
using Arah.Api.Contracts.Fiscal;
using Arah.Api.Security;
using Arah.Application.Services;
using Arah.Application.Services.Fiscal;
using Arah.Domain.Users;
using Microsoft.AspNetCore.Mvc;

namespace Arah.Api.Controllers.Fiscal;

[ApiController]
[Route("api/v1/territories/{territoryId:guid}/payment-methods")]
[Produces("application/json")]
[Tags("Fiscal Packs")]
public sealed class TerritoryPaymentMethodsController : ControllerBase
{
    private readonly TerritoryPaymentMethodsService _service;
    private readonly CurrentUserAccessor _currentUserAccessor;
    private readonly AccessEvaluator _accessEvaluator;

    public TerritoryPaymentMethodsController(
        TerritoryPaymentMethodsService service,
        CurrentUserAccessor currentUserAccessor,
        AccessEvaluator accessEvaluator)
    {
        _service = service;
        _currentUserAccessor = currentUserAccessor;
        _accessEvaluator = accessEvaluator;
    }

    /// <summary>
    /// Lista meios de pagamento ativos do território (checkout). Sem config: lista vazia.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(TerritoryPaymentMethodsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TerritoryPaymentMethodsResponse>> Get(
        [FromRoute] Guid territoryId,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetAsync(territoryId, cancellationToken);
        if (result.IsFailure || result.Value is null)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(ToResponse(result.Value));
    }

    /// <summary>
    /// Configura meios de pagamento do território (SystemAdmin / implementador).
    /// </summary>
    [HttpPut]
    [ProducesResponseType(typeof(TerritoryPaymentMethodsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<TerritoryPaymentMethodsResponse>> Upsert(
        [FromRoute] Guid territoryId,
        [FromBody] UpsertTerritoryPaymentMethodsRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var isAdmin = await _accessEvaluator.HasSystemPermissionAsync(
            userContext.User.Id,
            SystemPermissionType.SystemAdmin,
            cancellationToken);
        if (!isAdmin)
        {
            return Forbid();
        }

        var result = await _service.UpsertAsync(
            territoryId,
            request.Methods ?? Array.Empty<string>(),
            request.PspProvider,
            userContext.User.Id,
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(ToResponse(result.Value));
    }

    private static TerritoryPaymentMethodsResponse ToResponse(TerritoryPaymentMethodsView view) =>
        new(view.TerritoryId, view.Methods, view.PspProvider, view.UpdatedAtUtc, view.IsDefaultEmpty);
}
