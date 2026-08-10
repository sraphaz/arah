using Arah.Api.Contracts.Assets;
using Arah.Api.Contracts.Common;
using Arah.Api.Security;
using Arah.Application.Common;
using Arah.Application.Services;
using Arah.Domain.Membership;
using Arah.Modules.Assets.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Arah.Api.Controllers;

[ApiController]
[Route("api/v1/territories/{territoryId:guid}/natural-assets")]
[Produces("application/json")]
[Tags("Natural Assets")]
public sealed class NaturalAssetsController : ControllerBase
{
    private readonly CurrentUserAccessor _currentUserAccessor;
    private readonly AccessEvaluator _accessEvaluator;
    private readonly NaturalAssetService _naturalAssetService;

    public NaturalAssetsController(
        CurrentUserAccessor currentUserAccessor,
        AccessEvaluator accessEvaluator,
        NaturalAssetService naturalAssetService)
    {
        _currentUserAccessor = currentUserAccessor;
        _accessEvaluator = accessEvaluator;
        _naturalAssetService = naturalAssetService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<NaturalAssetResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResponse<NaturalAssetResponse>>> List(
        [FromRoute] Guid territoryId,
        [FromQuery] string? status,
        [FromQuery(Name = "types")] string? types,
        CancellationToken cancellationToken,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var auth = await AuthorizeResidentOrCuratorAsync(territoryId, cancellationToken);
        if (auth is not null)
        {
            return auth;
        }

        var statusFilter = ParseStatus(status);
        if (status is not null && statusFilter is null)
        {
            return BadRequest(new { error = "Invalid status." });
        }

        var pagination = new PaginationParameters(pageNumber, pageSize);
        var paged = await _naturalAssetService.ListPagedAsync(
            territoryId,
            statusFilter,
            ParseCsv(types),
            pagination,
            cancellationToken);

        return Ok(new PagedResponse<NaturalAssetResponse>(
            paged.Items.Select(ToResponse).ToList(),
            paged.PageNumber,
            paged.PageSize,
            paged.TotalCount,
            paged.TotalPages,
            paged.HasPreviousPage,
            paged.HasNextPage));
    }

    [HttpGet("{assetId:guid}")]
    [ProducesResponseType(typeof(NaturalAssetResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<NaturalAssetResponse>> Get(
        [FromRoute] Guid territoryId,
        [FromRoute] Guid assetId,
        CancellationToken cancellationToken)
    {
        var auth = await AuthorizeResidentOrCuratorAsync(territoryId, cancellationToken);
        if (auth is not null)
        {
            return auth;
        }

        var asset = await _naturalAssetService.GetByIdAsync(assetId, cancellationToken);
        if (asset is null || asset.TerritoryId != territoryId)
        {
            return NotFound(new { error = "Natural asset not found for territory." });
        }

        return Ok(ToResponse(asset));
    }

    [HttpPost]
    [ProducesResponseType(typeof(NaturalAssetResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<NaturalAssetResponse>> Create(
        [FromRoute] Guid territoryId,
        [FromBody] CreateNaturalAssetRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        if (!await IsResidentOrCuratorAsync(userContext.User.Id, territoryId, cancellationToken))
        {
            return Unauthorized();
        }

        var result = await _naturalAssetService.CreateAsync(
            territoryId,
            userContext.User.Id,
            request.Type,
            request.Name,
            request.Description,
            request.Latitude,
            request.Longitude,
            request.WaterType,
            request.PotabilityNotes,
            request.LastTestedAtUtc,
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            return BadRequest(new { error = result.Error ?? "Unable to create natural asset." });
        }

        return Ok(ToResponse(result.Value));
    }

    [HttpPost("{assetId:guid}/publish")]
    [ProducesResponseType(typeof(NaturalAssetResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<NaturalAssetResponse>> Publish(
        [FromRoute] Guid territoryId,
        [FromRoute] Guid assetId,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var isCurator = await _accessEvaluator.HasCapabilityAsync(
            userContext.User.Id,
            territoryId,
            MembershipCapabilityType.Curator,
            cancellationToken);
        if (!isCurator)
        {
            return Unauthorized();
        }

        var result = await _naturalAssetService.PublishAsync(
            assetId,
            territoryId,
            userContext.User.Id,
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            return BadRequest(new { error = result.Error ?? "Unable to publish natural asset." });
        }

        return Ok(ToResponse(result.Value));
    }

    private async Task<ActionResult?> AuthorizeResidentOrCuratorAsync(
        Guid territoryId,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        if (!await IsResidentOrCuratorAsync(userContext.User.Id, territoryId, cancellationToken))
        {
            return Unauthorized();
        }

        return null;
    }

    private async Task<bool> IsResidentOrCuratorAsync(
        Guid userId,
        Guid territoryId,
        CancellationToken cancellationToken)
    {
        if (await _accessEvaluator.HasCapabilityAsync(
                userId,
                territoryId,
                MembershipCapabilityType.Curator,
                cancellationToken))
        {
            return true;
        }

        return await _accessEvaluator.IsResidentAsync(userId, territoryId, cancellationToken);
    }

    private static NaturalAssetResponse ToResponse(NaturalAsset asset) => new(
        asset.Id,
        asset.TerritoryId,
        asset.Type,
        asset.Name,
        asset.Description,
        asset.Status.ToString().ToUpperInvariant(),
        asset.WaterPoint.Latitude,
        asset.WaterPoint.Longitude,
        asset.WaterPoint.WaterType,
        asset.WaterPoint.PotabilityNotes,
        asset.WaterPoint.LastTestedAtUtc,
        asset.CreatedAtUtc,
        asset.UpdatedAtUtc);

    private static NaturalAssetStatus? ParseStatus(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            return null;
        }

        return Enum.TryParse<NaturalAssetStatus>(status, true, out var parsed) ? parsed : null;
    }

    private static IReadOnlyCollection<string>? ParseCsv(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return null;
        }

        return raw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }
}
