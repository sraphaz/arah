using Araponga.Api.Security;
using Araponga.Application.Models;
using Araponga.Application.Services;
using Araponga.Application.Services.Connections;
using Araponga.Domain.Connections;
using Araponga.Domain.Membership;
using Microsoft.AspNetCore.Mvc;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/connections")]
[Produces("application/json")]
[Tags("Connections")]
public sealed class ConnectionsController : ControllerBase
{
    private readonly ConnectionService _connectionService;
    private readonly ConnectionPrivacyService _privacyService;
    private readonly CurrentUserAccessor _currentUserAccessor;
    private readonly TerritoryFeatureFlagGuard _featureGuard;

    public ConnectionsController(
        ConnectionService connectionService,
        ConnectionPrivacyService privacyService,
        CurrentUserAccessor currentUserAccessor,
        TerritoryFeatureFlagGuard featureGuard)
    {
        _connectionService = connectionService;
        _privacyService = privacyService;
        _currentUserAccessor = currentUserAccessor;
        _featureGuard = featureGuard;
    }

    /// <summary>
    /// Envia solicitação de conexão para outro usuário.
    /// </summary>
    [HttpPost("request")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> RequestConnection(
        [FromBody] RequestConnectionRequest request,
        [FromQuery] Guid? territoryId,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
            return Unauthorized();

        if (territoryId.HasValue && !_featureGuard.IsEnabled(territoryId.Value, FeatureFlag.ConnectionsEnabled))
            return StatusCode(403, new { error = "Conexões não estão habilitadas neste território." });

        var result = await _connectionService.RequestConnectionAsync(
            userContext.User.Id,
            request.TargetUserId,
            territoryId,
            cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error?.Contains("já existe", StringComparison.OrdinalIgnoreCase) == true)
                return Conflict(new { error = result.Error });
            return BadRequest(new { error = result.Error });
        }

        var c = result.Value!;
        return CreatedAtAction(nameof(GetConnections), new { status = "Pending" }, new
        {
            c.Id,
            c.RequesterUserId,
            c.TargetUserId,
            Status = c.Status.ToString(),
            c.TerritoryId,
            c.RequestedAtUtc
        });
    }

    /// <summary>
    /// Aceita uma solicitação de conexão recebida.
    /// </summary>
    [HttpPost("{connectionId:guid}/accept")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> AcceptConnection(
        [FromRoute] Guid connectionId,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
            return Unauthorized();

        var result = await _connectionService.AcceptConnectionAsync(connectionId, userContext.User.Id, cancellationToken);
        if (!result.IsSuccess)
        {
            if (result.Error?.Contains("não encontrada", StringComparison.OrdinalIgnoreCase) == true)
                return NotFound(new { error = result.Error });
            return BadRequest(new { error = result.Error });
        }

        var c = result.Value!;
        return Ok(new { c.Id, Status = c.Status.ToString(), c.RespondedAtUtc });
    }

    /// <summary>
    /// Rejeita uma solicitação de conexão recebida.
    /// </summary>
    [HttpPost("{connectionId:guid}/reject")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> RejectConnection(
        [FromRoute] Guid connectionId,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
            return Unauthorized();

        var result = await _connectionService.RejectConnectionAsync(connectionId, userContext.User.Id, cancellationToken);
        if (!result.IsSuccess)
        {
            if (result.Error?.Contains("não encontrada", StringComparison.OrdinalIgnoreCase) == true)
                return NotFound(new { error = result.Error });
            return BadRequest(new { error = result.Error });
        }

        return Ok(new { connectionId, status = "Rejected" });
    }

    /// <summary>
    /// Remove uma conexão aceita (qualquer uma das partes pode remover).
    /// </summary>
    [HttpDelete("{connectionId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> RemoveConnection(
        [FromRoute] Guid connectionId,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
            return Unauthorized();

        var result = await _connectionService.RemoveConnectionAsync(connectionId, userContext.User.Id, cancellationToken);
        if (!result.IsSuccess)
        {
            if (result.Error?.Contains("não encontrada", StringComparison.OrdinalIgnoreCase) == true)
                return NotFound(new { error = result.Error });
            return BadRequest(new { error = result.Error });
        }

        return Ok(new { connectionId, status = "Removed" });
    }

    /// <summary>
    /// Lista conexões do usuário (aceitas, pendentes, etc.).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<ConnectionItemResponse>>> GetConnections(
        [FromQuery] string? status,
        [FromQuery] Guid? territoryId,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
            return Unauthorized();

        ConnectionStatus? filter = null;
        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<ConnectionStatus>(status, true, out var s))
            filter = s;

        var list = await _connectionService.GetConnectionsAsync(userContext.User.Id, filter, cancellationToken);

        var items = list.Select(c => new ConnectionItemResponse(
            c.Id,
            c.RequesterUserId,
            c.TargetUserId,
            c.Status.ToString(),
            c.TerritoryId,
            c.RequestedAtUtc,
            c.RespondedAtUtc,
            c.RemovedAtUtc,
            c.CreatedAtUtc,
            c.UpdatedAtUtc,
            userContext.User.Id == c.TargetUserId ? "Incoming" : "Outgoing")).ToList();

        if (territoryId.HasValue)
        {
            return Ok(items.Where(x => x.TerritoryId == territoryId).ToList());
        }

        return Ok(items);
    }

    /// <summary>
    /// Lista solicitações pendentes recebidas pelo usuário.
    /// </summary>
    [HttpGet("pending")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<ConnectionItemResponse>>> GetPending(
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
            return Unauthorized();

        var list = await _connectionService.GetPendingRequestsAsync(userContext.User.Id, cancellationToken);
        var items = list.Select(c => new ConnectionItemResponse(
            c.Id,
            c.RequesterUserId,
            c.TargetUserId,
            c.Status.ToString(),
            c.TerritoryId,
            c.RequestedAtUtc,
            c.RespondedAtUtc,
            c.RemovedAtUtc,
            c.CreatedAtUtc,
            c.UpdatedAtUtc,
            "Incoming")).ToList();

        return Ok(items);
    }

    /// <summary>
    /// Obtém as configurações de privacidade de conexões do usuário autenticado.
    /// </summary>
    [HttpGet("privacy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ConnectionPrivacyResponse>> GetPrivacy(CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
            return Unauthorized();

        var settings = await _privacyService.GetOrCreateDefaultAsync(userContext.User.Id, cancellationToken);
        return Ok(new ConnectionPrivacyResponse(
            settings.WhoCanAddMe.ToString(),
            settings.WhoCanSeeMyConnections.ToString(),
            settings.ShowConnectionsInProfile,
            settings.UpdatedAtUtc));
    }

    /// <summary>
    /// Atualiza as configurações de privacidade de conexões do usuário autenticado.
    /// </summary>
    [HttpPut("privacy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ConnectionPrivacyResponse>> UpdatePrivacy(
        [FromBody] UpdateConnectionPrivacyRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
            return Unauthorized();

        ConnectionRequestPolicy? whoCanAddMe = null;
        if (!string.IsNullOrWhiteSpace(request.WhoCanAddMe) && Enum.TryParse<ConnectionRequestPolicy>(request.WhoCanAddMe, true, out var wcam))
            whoCanAddMe = wcam;

        ConnectionVisibility? whoCanSeeMyConnections = null;
        if (!string.IsNullOrWhiteSpace(request.WhoCanSeeMyConnections) && Enum.TryParse<ConnectionVisibility>(request.WhoCanSeeMyConnections, true, out var wcsmc))
            whoCanSeeMyConnections = wcsmc;

        var result = await _privacyService.UpdateAsync(
            userContext.User.Id,
            whoCanAddMe,
            whoCanSeeMyConnections,
            request.ShowConnectionsInProfile,
            cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        var s = result.Value!;
        return Ok(new ConnectionPrivacyResponse(
            s.WhoCanAddMe.ToString(),
            s.WhoCanSeeMyConnections.ToString(),
            s.ShowConnectionsInProfile,
            s.UpdatedAtUtc));
    }

    /// <summary>
    /// Busca usuários por nome de exibição para adicionar como conexão.
    /// Opcional: filtrar por território e papel (Resident/Visitor).
    /// </summary>
    [HttpGet("users/search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<ConnectionUserSearchResponse>>> SearchUsers(
        [FromQuery] string? query,
        [FromQuery] Guid? territoryId,
        [FromQuery] string? role,
        [FromQuery] int limit = 20,
        CancellationToken cancellationToken = default)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
            return Unauthorized();

        MembershipRole? roleFilter = null;
        if (!string.IsNullOrWhiteSpace(role) && Enum.TryParse<MembershipRole>(role, true, out var r))
            roleFilter = r;

        var users = await _connectionService.SearchUsersAsync(
            userContext.User.Id,
            query,
            territoryId,
            roleFilter,
            limit,
            cancellationToken);

        var items = users.Select(u => new ConnectionUserSearchResponse(u.Id, u.DisplayName));
        return Ok(items);
    }

    /// <summary>
    /// Lista sugestões de conexão (pessoas que você pode adicionar).
    /// Quando territoryId é informado, prioriza membros do território.
    /// </summary>
    [HttpGet("suggestions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<ConnectionUserSearchResponse>>> GetSuggestions(
        [FromQuery] Guid? territoryId,
        [FromQuery] int limit = 10,
        CancellationToken cancellationToken = default)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
            return Unauthorized();

        var users = await _connectionService.GetSuggestionsAsync(
            userContext.User.Id,
            territoryId,
            limit,
            cancellationToken);

        var items = users.Select(u => new ConnectionUserSearchResponse(u.Id, u.DisplayName));
        return Ok(items);
    }
}

public sealed record RequestConnectionRequest(Guid TargetUserId);

public sealed record ConnectionUserSearchResponse(Guid Id, string DisplayName);

public sealed record UpdateConnectionPrivacyRequest(
    string? WhoCanAddMe,
    string? WhoCanSeeMyConnections,
    bool? ShowConnectionsInProfile);

public sealed record ConnectionPrivacyResponse(
    string WhoCanAddMe,
    string WhoCanSeeMyConnections,
    bool ShowConnectionsInProfile,
    DateTime UpdatedAtUtc);

public sealed record ConnectionItemResponse(
    Guid Id,
    Guid RequesterUserId,
    Guid TargetUserId,
    string Status,
    Guid? TerritoryId,
    DateTime RequestedAtUtc,
    DateTime? RespondedAtUtc,
    DateTime? RemovedAtUtc,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc,
    string Direction);
