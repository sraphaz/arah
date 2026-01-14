using Araponga.Api.Contracts.Common;
using Araponga.Api.Contracts.Marketplace;
using Araponga.Api.Security;
using Araponga.Application.Common;
using Araponga.Application.Services;
using Araponga.Domain.Marketplace;
using Araponga.Domain.Membership;
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

        var isCurator = await _accessEvaluator.HasCapabilityAsync(userContext.User.Id, territoryId, MembershipCapabilityType.Curator, cancellationToken);
        if (!isCurator)
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        var configs = await _platformFeeService.ListActiveAsync(territoryId, cancellationToken);
        var response = configs.Select(ToResponse).ToList();
        return Ok(response);
    }

    /// <summary>
    /// Lista configurações ativas de fee por território (paginado).
    /// </summary>
    /// <param name="territoryId">ID do território</param>
    /// <param name="pageNumber">Número da página (padrão: 1)</param>
    /// <param name="pageSize">Tamanho da página (padrão: 20, máximo recomendado: 100)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista paginada de configurações de fee</returns>
    /// <remarks>
    /// Requer autenticação e permissões de curador.
    /// A paginação é feita no nível do repositório para melhor performance.
    /// </remarks>
    [HttpGet("paged")]
    [ProducesResponseType(typeof(PagedResponse<PlatformFeeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PagedResponse<PlatformFeeResponse>>> ListFeesPaged(
        [FromQuery] Guid territoryId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
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

        var isCurator = await _accessEvaluator.HasCapabilityAsync(userContext.User.Id, territoryId, MembershipCapabilityType.Curator, cancellationToken);
        if (!isCurator)
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        var pagination = new PaginationParameters(pageNumber, pageSize);
        var pagedResult = await _platformFeeService.ListActivePagedAsync(territoryId, pagination, cancellationToken);
        var response = new PagedResponse<PlatformFeeResponse>(
            pagedResult.Items.Select(ToResponse).ToList(),
            pagedResult.PageNumber,
            pagedResult.PageSize,
            pagedResult.TotalCount,
            pagedResult.TotalPages,
            pagedResult.HasPreviousPage,
            pagedResult.HasNextPage);
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

        if (!TryParseItemType(request.ItemType, out var itemType))
        {
            return BadRequest(new { error = "Invalid itemType." });
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

        var isCurator = await _accessEvaluator.HasCapabilityAsync(userContext.User.Id, request.TerritoryId, MembershipCapabilityType.Curator, cancellationToken);
        if (!isCurator)
        {
            return Forbid();
        }

        var config = await _platformFeeService.UpsertFeeConfigAsync(
            request.TerritoryId,
            itemType,
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
            config.ItemType.ToString().ToUpperInvariant(),
            config.FeeMode.ToString().ToUpperInvariant(),
            config.FeeValue,
            config.Currency,
            config.IsActive,
            config.CreatedAtUtc,
            config.UpdatedAtUtc);
    }

    private static bool TryParseItemType(string? raw, out ItemType itemType)
    {
        return Enum.TryParse(raw, true, out itemType);
    }

    private static bool TryParseFeeMode(string? raw, out PlatformFeeMode feeMode)
    {
        return Enum.TryParse(raw, true, out feeMode);
    }
}
