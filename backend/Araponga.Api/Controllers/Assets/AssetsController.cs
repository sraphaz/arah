using Araponga.Api.Contracts.Assets;
using Araponga.Api.Contracts.Common;
using Araponga.Api.Security;
using Araponga.Application.Common;
using Araponga.Application.Models;
using Araponga.Application.Services;
using Araponga.Domain.Assets;
using Araponga.Domain.Membership;
using Araponga.Domain.Work;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/assets")]
[Produces("application/json")]
[Tags("Assets")]
public sealed class AssetsController : ControllerBase
{
    private readonly ActiveTerritoryService _activeTerritoryService;
    private readonly CurrentUserAccessor _currentUserAccessor;
    private readonly AccessEvaluator _accessEvaluator;
    private readonly TerritoryAssetService _assetService;

    public AssetsController(
        ActiveTerritoryService activeTerritoryService,
        CurrentUserAccessor currentUserAccessor,
        AccessEvaluator accessEvaluator,
        TerritoryAssetService assetService)
    {
        _activeTerritoryService = activeTerritoryService;
        _currentUserAccessor = currentUserAccessor;
        _accessEvaluator = accessEvaluator;
        _assetService = assetService;
    }

    /// <summary>
    /// Lista assets do território.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AssetResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<AssetResponse>>> GetAssets(
        [FromQuery] Guid? territoryId,
        [FromQuery] Guid? assetId,
        [FromQuery(Name = "types")] string? types,
        [FromQuery] string? status,
        [FromQuery] string? search,
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

        if (!await IsResidentOrCuratorAsync(userContext.User.Id, resolvedTerritoryId.Value, cancellationToken, userContext.User))
        {
            return Unauthorized();
        }

        if (assetId is not null)
        {
            var assetDetails = await _assetService.GetByIdAsync(assetId.Value, cancellationToken);
            if (assetDetails is null || assetDetails.Asset.TerritoryId != resolvedTerritoryId.Value)
            {
                return BadRequest(new { error = "Asset not found for territory." });
            }

            return Ok(new[] { ToResponse(assetDetails) });
        }

        var statusFilter = ParseStatus(status);
        if (status is not null && statusFilter is null)
        {
            return BadRequest(new { error = "Invalid status." });
        }
        var typeList = ParseCsv(types);

        var assets = await _assetService.ListAsync(
            resolvedTerritoryId.Value,
            typeList,
            statusFilter,
            search,
            cancellationToken);

        var response = assets.Select(ToResponse).ToList();
        return Ok(response);
    }

    /// <summary>
    /// Lista assets do território (paginado).
    /// </summary>
    [HttpGet("paged")]
    [ProducesResponseType(typeof(PagedResponse<AssetResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResponse<AssetResponse>>> GetAssetsPaged(
        [FromQuery] Guid? territoryId,
        [FromQuery(Name = "types")] string? types,
        [FromQuery] string? status,
        [FromQuery] string? search,
        CancellationToken cancellationToken,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
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

        var isResident = await _accessEvaluator.IsResidentAsync(userContext.User.Id, resolvedTerritoryId.Value, cancellationToken);
        var isCurator = await _accessEvaluator.HasCapabilityAsync(userContext.User.Id, resolvedTerritoryId.Value, MembershipCapabilityType.Curator, cancellationToken);
        if (!isResident && !isCurator)
        {
            return Unauthorized();
        }

        var statusFilter = ParseStatus(status);
        if (status is not null && statusFilter is null)
        {
            return BadRequest(new { error = "Invalid status." });
        }
        var typeList = ParseCsv(types);

        var pagination = new PaginationParameters(pageNumber, pageSize);
        var pagedResult = await _assetService.ListPagedAsync(
            resolvedTerritoryId.Value,
            typeList,
            statusFilter,
            search,
            pagination,
            cancellationToken);

        const int maxInt32 = int.MaxValue;
        var safeTotalCount = pagedResult.TotalCount > maxInt32 ? maxInt32 : pagedResult.TotalCount;
        var safeTotalPages = pagedResult.TotalPages > maxInt32 ? maxInt32 : pagedResult.TotalPages;
        var response = new PagedResponse<AssetResponse>(
            pagedResult.Items.Select(ToResponse).ToList(),
            pagedResult.PageNumber,
            pagedResult.PageSize,
            safeTotalCount,
            safeTotalPages,
            pagedResult.HasPreviousPage,
            pagedResult.HasNextPage);

        return Ok(response);
    }

    /// <summary>
    /// Detalhe de um asset.
    /// </summary>
    [HttpGet("{assetId:guid}")]
    [ProducesResponseType(typeof(AssetResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AssetResponse>> GetAssetById(
        [FromRoute] Guid assetId,
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

        if (!await IsResidentOrCuratorAsync(userContext.User.Id, resolvedTerritoryId.Value, cancellationToken, userContext.User))
        {
            return Unauthorized();
        }

        var assetDetails = await _assetService.GetByIdAsync(assetId, cancellationToken);
        if (assetDetails is null || assetDetails.Asset.TerritoryId != resolvedTerritoryId.Value)
        {
            return BadRequest(new { error = "Asset not found for territory." });
        }

        return Ok(ToResponse(assetDetails));
    }

    /// <summary>
    /// Cria um asset territorial.
    /// </summary>
    [HttpPost]
    [EnableRateLimiting("write")]
    [ProducesResponseType(typeof(AssetResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AssetResponse>> CreateAsset(
        [FromBody] CreateAssetRequest request,
        CancellationToken cancellationToken)
    {
        if (request.TerritoryId == Guid.Empty)
        {
            return BadRequest(new { error = "territoryId is required." });
        }

        if (request.GeoAnchors is null || request.GeoAnchors.Count == 0)
        {
            return BadRequest(new { error = "At least one geoAnchor is required." });
        }

        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        if (!await IsResidentOrCuratorAsync(userContext.User.Id, request.TerritoryId, cancellationToken, userContext.User))
        {
            return Unauthorized();
        }

        var result = await _assetService.CreateAsync(
            request.TerritoryId,
            userContext.User.Id,
            request.Type,
            request.Name,
            request.Description,
            request.GeoAnchors.Select(anchor => new TerritoryAssetGeoAnchorInput(anchor.Latitude, anchor.Longitude)).ToList(),
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            return BadRequest(new { error = result.Error ?? "Unable to create asset." });
        }

        return CreatedAtAction(nameof(GetAssetById), new { assetId = result.Value.Asset.Id }, ToResponse(result.Value));
    }

    /// <summary>
    /// Atualiza um asset territorial.
    /// </summary>
    [HttpPatch("{assetId:guid}")]
    [ProducesResponseType(typeof(AssetResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AssetResponse>> UpdateAsset(
        [FromRoute] Guid assetId,
        [FromQuery] Guid? territoryId,
        [FromBody] UpdateAssetRequest request,
        CancellationToken cancellationToken)
    {
        var resolvedTerritoryId = await ResolveTerritoryIdAsync(territoryId, cancellationToken);
        if (resolvedTerritoryId is null)
        {
            return BadRequest(new { error = "territoryId (query) or X-Session-Id header is required." });
        }

        if (request.GeoAnchors is null || request.GeoAnchors.Count == 0)
        {
            return BadRequest(new { error = "At least one geoAnchor is required." });
        }

        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        if (!await IsResidentOrCuratorAsync(userContext.User.Id, resolvedTerritoryId.Value, cancellationToken, userContext.User))
        {
            return Unauthorized();
        }

        var result = await _assetService.UpdateAsync(
            assetId,
            resolvedTerritoryId.Value,
            userContext.User.Id,
            request.Type,
            request.Name,
            request.Description,
            request.GeoAnchors.Select(anchor => new TerritoryAssetGeoAnchorInput(anchor.Latitude, anchor.Longitude)).ToList(),
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            return BadRequest(new { error = result.Error ?? "Unable to update asset." });
        }

        return Ok(ToResponse(result.Value));
    }

    /// <summary>
    /// Arquiva um asset territorial.
    /// </summary>
    [HttpPost("{assetId:guid}/archive")]
    [ProducesResponseType(typeof(AssetResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AssetResponse>> ArchiveAsset(
        [FromRoute] Guid assetId,
        [FromQuery] Guid? territoryId,
        [FromBody] ArchiveAssetRequest? request,
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

        if (!await IsResidentOrCuratorAsync(userContext.User.Id, resolvedTerritoryId.Value, cancellationToken, userContext.User))
        {
            return Unauthorized();
        }

        var result = await _assetService.ArchiveAsync(
            assetId,
            resolvedTerritoryId.Value,
            userContext.User.Id,
            request?.Reason,
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            return BadRequest(new { error = result.Error ?? "Unable to archive asset." });
        }

        return Ok(ToResponse(result.Value));
    }

    /// <summary>
    /// Valida um asset territorial.
    /// </summary>
    [HttpPost("{assetId:guid}/validate")]
    [ProducesResponseType(typeof(AssetValidationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AssetValidationResponse>> ValidateAsset(
        [FromRoute] Guid assetId,
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

        if (!await IsResidentOrCuratorAsync(userContext.User.Id, resolvedTerritoryId.Value, cancellationToken, userContext.User))
        {
            return Unauthorized();
        }

        var result = await _assetService.ValidateAsync(
            assetId,
            resolvedTerritoryId.Value,
            userContext.User.Id,
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            return BadRequest(new { error = result.Error ?? "Unable to validate asset." });
        }

        return Ok(new AssetValidationResponse(
            result.Value.Asset.Asset.Id,
            result.Value.Asset.ValidationsCount,
            result.Value.Asset.EligibleResidentsCount,
            result.Value.Asset.ValidationPct));
    }

    /// <summary>
    /// Curadoria de asset territorial (aprovar/rejeitar) - Curator.
    /// </summary>
    [HttpPost("{assetId:guid}/curate")]
    [ProducesResponseType(typeof(AssetResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AssetResponse>> CurateAsset(
        [FromRoute] Guid assetId,
        [FromQuery] Guid? territoryId,
        [FromBody] CurateAssetRequest request,
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

        var isCurator = await _accessEvaluator.HasCapabilityAsync(
            userContext.User.Id,
            resolvedTerritoryId.Value,
            MembershipCapabilityType.Curator,
            cancellationToken);
        if (!isCurator)
        {
            return Unauthorized();
        }

        if (!Enum.TryParse<WorkItemOutcome>(request.Outcome, true, out var outcome) ||
            (outcome != WorkItemOutcome.Approved && outcome != WorkItemOutcome.Rejected))
        {
            return BadRequest(new { error = $"Invalid outcome: {request.Outcome}" });
        }

        var result = await _assetService.CurateAsync(
            assetId,
            resolvedTerritoryId.Value,
            userContext.User.Id,
            outcome,
            request.Notes,
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            return BadRequest(new { error = result.Error ?? "Unable to curate asset." });
        }

        return Ok(ToResponse(result.Value));
    }

    private AssetResponse ToResponse(TerritoryAssetDetails details)
    {
        var asset = details.Asset;
        return new AssetResponse(
            asset.Id,
            asset.TerritoryId,
            asset.Type,
            asset.Name,
            asset.Description,
            asset.Status.ToString().ToUpperInvariant(),
            details.Anchors.Select(anchor => new AssetGeoAnchorResponse(
                anchor.Id,
                anchor.Latitude,
                anchor.Longitude)).ToList(),
            details.ValidationsCount,
            details.EligibleResidentsCount,
            details.ValidationPct,
            asset.CreatedAtUtc,
            asset.UpdatedAtUtc,
            asset.ArchivedAtUtc,
            asset.ArchiveReason);
    }

    private static AssetStatus? ParseStatus(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            return AssetStatus.Active;
        }

        return Enum.TryParse<AssetStatus>(status, true, out var parsed) ? parsed : null;
    }

    private static IReadOnlyCollection<string>? ParseCsv(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return null;
        }

        return raw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();
    }

    private async Task<bool> IsResidentOrCuratorAsync(
        Guid userId,
        Guid territoryId,
        CancellationToken cancellationToken,
        Araponga.Domain.Users.User user)
    {
        if (await _accessEvaluator.HasCapabilityAsync(userId, territoryId, MembershipCapabilityType.Curator, cancellationToken))
        {
            return true;
        }

        return await _accessEvaluator.IsResidentAsync(userId, territoryId, cancellationToken);
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
