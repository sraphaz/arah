using Arah.Api;
using Arah.Api.Contracts.Fiscal;
using Arah.Api.Security;
using Arah.Application.Services;
using Arah.Application.Services.Fiscal;
using Arah.Domain.Fiscal;
using Arah.Domain.Users;
using Microsoft.AspNetCore.Mvc;

namespace Arah.Api.Controllers.Fiscal;

[ApiController]
[Route("api/v1/territories/{territoryId:guid}/fiscal-pack")]
[Produces("application/json")]
[Tags("Fiscal Packs")]
public sealed class TerritoryFiscalPackController : ControllerBase
{
    private readonly TerritoryFiscalPackService _service;
    private readonly CurrentUserAccessor _currentUserAccessor;
    private readonly AccessEvaluator _accessEvaluator;

    public TerritoryFiscalPackController(
        TerritoryFiscalPackService service,
        CurrentUserAccessor currentUserAccessor,
        AccessEvaluator accessEvaluator)
    {
        _service = service;
        _currentUserAccessor = currentUserAccessor;
        _accessEvaluator = accessEvaluator;
    }

    /// <summary>
    /// Obtém o binding fiscal do território. Sem binding: Off legado (IsLegacyDefault=true).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(TerritoryFiscalPackBindingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TerritoryFiscalPackBindingResponse>> Get(
        [FromRoute] Guid territoryId,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _service.GetBindingAsync(territoryId, cancellationToken);
        if (result.IsFailure || result.Value is null)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(ToResponse(result.Value));
    }

    /// <summary>
    /// Ativa ou desliga o pacote fiscal no território (implementador / SystemAdmin).
    /// </summary>
    [HttpPut]
    [ProducesResponseType(typeof(TerritoryFiscalPackBindingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<TerritoryFiscalPackBindingResponse>> Upsert(
        [FromRoute] Guid territoryId,
        [FromBody] UpsertTerritoryFiscalPackBindingRequest request,
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

        if (!Enum.TryParse<FiscalPackBindingStatus>(request.Status, ignoreCase: true, out var status)
            || !Enum.IsDefined(status))
        {
            return BadRequest(new { error = "Status must be Off or Active." });
        }

        var result = await _service.UpsertBindingAsync(
            territoryId,
            request.PackId,
            status,
            request.MunicipalityIbge,
            userContext.User.Id,
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(ToResponse(result.Value));
    }

    private static TerritoryFiscalPackBindingResponse ToResponse(TerritoryFiscalPackBindingView view) =>
        new(
            view.TerritoryId,
            view.PackId,
            view.Status,
            view.ActivatedByUserId,
            view.ActivatedAtUtc,
            view.MunicipalityIbge,
            view.IsLegacyDefault);
}
