using Araponga.Api.Contracts.Marketplace;
using Araponga.Api.Security;
using Araponga.Application.Services;
using Araponga.Modules.Marketplace.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/stores")]
[Produces("application/json")]
[Tags("Stores")]
public sealed class StoresController : ControllerBase
{
    private readonly StoreService _storeService;
    private readonly CurrentUserAccessor _currentUserAccessor;
    private readonly ActiveTerritoryService _activeTerritoryService;

    public StoresController(
        StoreService storeService,
        CurrentUserAccessor currentUserAccessor,
        ActiveTerritoryService activeTerritoryService)
    {
        _storeService = storeService;
        _currentUserAccessor = currentUserAccessor;
        _activeTerritoryService = activeTerritoryService;
    }

    /// <summary>
    /// Cria ou atualiza a loja do usuário no território.
    /// </summary>
    [HttpPost]
    [EnableRateLimiting("write")]
    [ProducesResponseType(typeof(StoreResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<StoreResponse>> UpsertMyStore(
        [FromBody] UpsertStoreRequest request,
        CancellationToken cancellationToken)
    {
        if (request.TerritoryId == Guid.Empty)
        {
            return BadRequest(new { error = "territoryId is required." });
        }

        if (!TryParseContactVisibility(request.ContactVisibility, out var visibility))
        {
            return BadRequest(new { error = "Invalid contactVisibility." });
        }

        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var contact = request.Contact;
        var result = await _storeService.UpsertMyStoreAsync(
            request.TerritoryId,
            userContext.User.Id,
            request.DisplayName,
            request.Description,
            visibility,
            contact?.Phone,
            contact?.Whatsapp,
            contact?.Email,
            contact?.Instagram,
            contact?.Website,
            contact?.PreferredContactMethod,
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            return result.Error?.Contains("Only confirmed residents", StringComparison.OrdinalIgnoreCase) == true
                ? StatusCode(StatusCodes.Status403Forbidden)
                : BadRequest(new { error = result.Error ?? "Unable to manage store." });
        }

        return Ok(ToResponse(result.Value));
    }

    /// <summary>
    /// Retorna a loja do usuário no território.
    /// </summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(StoreResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<StoreResponse>> GetMyStore(
        [FromQuery] Guid? territoryId,
        CancellationToken cancellationToken)
    {
        var resolvedTerritoryId = await ResolveTerritoryIdAsync(territoryId, cancellationToken);
        if (resolvedTerritoryId is null)
        {
            return BadRequest(new { error = "territoryId (query) or X-Session-Id header is required." });
        }

        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var store = await _storeService.GetMyStoreAsync(resolvedTerritoryId.Value, userContext.User.Id, cancellationToken);
        if (store is null)
        {
            return NotFound();
        }

        return Ok(ToResponse(store));
    }

    /// <summary>
    /// Atualiza detalhes da loja.
    /// </summary>
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(typeof(StoreResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<StoreResponse>> UpdateStore(
        [FromRoute] Guid id,
        [FromBody] UpdateStoreRequest request,
        CancellationToken cancellationToken)
    {
        StoreContactVisibility? visibility = null;
        if (!string.IsNullOrWhiteSpace(request.ContactVisibility))
        {
            if (!TryParseContactVisibility(request.ContactVisibility, out var parsedVisibility))
            {
                return BadRequest(new { error = "Invalid contactVisibility." });
            }

            visibility = parsedVisibility;
        }

        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var contact = request.Contact;
        var result = await _storeService.UpdateStoreAsync(
            id,
            userContext.User.Id,
            request.DisplayName,
            request.Description,
            visibility,
            contact?.Phone,
            contact?.Whatsapp,
            contact?.Email,
            contact?.Instagram,
            contact?.Website,
            contact?.PreferredContactMethod,
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            if (result.Error?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
            {
                return NotFound();
            }

            return result.Error?.Contains("Not authorized", StringComparison.OrdinalIgnoreCase) == true
                ? StatusCode(StatusCodes.Status403Forbidden)
                : BadRequest(new { error = result.Error ?? "Unable to update store." });
        }

        return Ok(ToResponse(result.Value));
    }

    /// <summary>
    /// Pausa uma loja.
    /// </summary>
    [HttpPost("{id:guid}/pause")]
    [ProducesResponseType(typeof(StoreResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<StoreResponse>> PauseStore(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        return await SetStoreStatusAsync(id, StoreStatus.Paused, cancellationToken);
    }

    /// <summary>
    /// Ativa uma loja.
    /// </summary>
    [HttpPost("{id:guid}/activate")]
    [ProducesResponseType(typeof(StoreResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<StoreResponse>> ActivateStore(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        return await SetStoreStatusAsync(id, StoreStatus.Active, cancellationToken);
    }

    /// <summary>
    /// Arquiva uma loja.
    /// </summary>
    [HttpPost("{id:guid}/archive")]
    [ProducesResponseType(typeof(StoreResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<StoreResponse>> ArchiveStore(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        return await SetStoreStatusAsync(id, StoreStatus.Archived, cancellationToken);
    }

    /// <summary>
    /// Define se a loja aceita pagamentos.
    /// </summary>
    [HttpPost("{id:guid}/payments/enable")]
    [ProducesResponseType(typeof(StoreResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<StoreResponse>> SetPaymentsEnabled(
        [FromRoute] Guid id,
        [FromBody] StorePaymentRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _storeService.SetPaymentsEnabledAsync(id, userContext.User.Id, request.Enabled, cancellationToken);
        if (!result.IsSuccess || result.Value is null)
        {
            if (result.Error?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
            {
                return NotFound();
            }

            return result.Error?.Contains("Not authorized", StringComparison.OrdinalIgnoreCase) == true
                ? StatusCode(StatusCodes.Status403Forbidden)
                : BadRequest(new { error = result.Error ?? "Unable to update payments." });
        }

        return Ok(ToResponse(result.Value));
    }

    private async Task<ActionResult<StoreResponse>> SetStoreStatusAsync(
        Guid id,
        StoreStatus status,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _storeService.SetStoreStatusAsync(id, userContext.User.Id, status, cancellationToken);
        if (!result.IsSuccess || result.Value is null)
        {
            if (result.Error?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
            {
                return NotFound();
            }

            return result.Error?.Contains("Not authorized", StringComparison.OrdinalIgnoreCase) == true
                ? StatusCode(StatusCodes.Status403Forbidden)
                : BadRequest(new { error = result.Error ?? "Unable to update store status." });
        }

        return Ok(ToResponse(result.Value));
    }

    private static StoreResponse ToResponse(Store store)
    {
        var contact = new StoreContactPayload(
            store.Phone,
            store.Whatsapp,
            store.Email,
            store.Instagram,
            store.Website,
            store.PreferredContactMethod);

        return new StoreResponse(
            store.Id,
            store.TerritoryId,
            store.OwnerUserId,
            store.DisplayName,
            store.Description,
            store.Status.ToString().ToUpperInvariant(),
            store.PaymentsEnabled,
            store.ContactVisibility.ToString().ToUpperInvariant(),
            contact,
            store.CreatedAtUtc,
            store.UpdatedAtUtc);
    }

    private static bool TryParseContactVisibility(string? raw, out StoreContactVisibility visibility)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            visibility = default;
            return false;
        }

        var normalized = raw.Replace("_", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase);

        return Enum.TryParse(normalized, true, out visibility);
    }

    private string? GetSessionId()
    {
        return Request.Headers.TryGetValue(ApiHeaders.SessionId, out var header) &&
               !string.IsNullOrWhiteSpace(header)
            ? header.ToString()
            : null;
    }

    private async Task<Guid?> ResolveTerritoryIdAsync(Guid? territoryId, CancellationToken cancellationToken)
    {
        if (territoryId is not null)
        {
            return territoryId;
        }

        var sessionId = GetSessionId();
        if (sessionId is null)
        {
            return null;
        }

        return await _activeTerritoryService.GetActiveAsync(sessionId, cancellationToken);
    }
}
