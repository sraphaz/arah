using Araponga.Api;
using Araponga.Api.Configuration;
using Araponga.Api.Contracts.Memberships;
using Araponga.Api.Security;
using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Domain.Membership;
using Araponga.Domain.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/territories/{territoryId:guid}/membership")]
[Produces("application/json")]
[Tags("Memberships")]
public sealed class MembershipsController : ControllerBase
{
    private readonly MembershipService _membershipService;
    private readonly VerificationQueueService _verificationQueueService;
    private readonly DocumentEvidenceService _documentEvidenceService;
    private readonly CurrentUserAccessor _currentUserAccessor;
    private readonly TerritoryService _territoryService;
    private readonly AccessEvaluator _accessEvaluator;
    private readonly ITerritoryMembershipRepository _membershipRepository;
    private readonly PresencePolicyOptions _presencePolicy;

    public MembershipsController(
        MembershipService membershipService,
        VerificationQueueService verificationQueueService,
        DocumentEvidenceService documentEvidenceService,
        CurrentUserAccessor currentUserAccessor,
        TerritoryService territoryService,
        AccessEvaluator accessEvaluator,
        ITerritoryMembershipRepository membershipRepository,
        IOptions<PresencePolicyOptions> presencePolicy)
    {
        _membershipService = membershipService;
        _verificationQueueService = verificationQueueService;
        _documentEvidenceService = documentEvidenceService;
        _currentUserAccessor = currentUserAccessor;
        _territoryService = territoryService;
        _accessEvaluator = accessEvaluator;
        _membershipRepository = membershipRepository;
        _presencePolicy = presencePolicy.Value;
    }

    /// <summary>
    /// Entra em um território como Visitor.
    /// </summary>
    [HttpPost]
    [Route("/api/v1/territories/{territoryId:guid}/enter")]
    [ProducesResponseType(typeof(EnterTerritoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EnterTerritoryResponse>> EnterAsVisitor(
        [FromRoute] Guid territoryId,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var territory = await _territoryService.GetByIdAsync(territoryId, cancellationToken);
        if (territory is null)
        {
            return NotFound();
        }

        var membership = await _membershipService.EnterAsVisitorAsync(
            userContext.User.Id,
            territoryId,
            cancellationToken);

        var response = new EnterTerritoryResponse(
            membership.Id,
            membership.UserId,
            membership.TerritoryId,
            membership.Role.ToString().ToUpperInvariant(),
            membership.ResidencyVerification.ToString().ToUpperInvariant(),
            membership.LastGeoVerifiedAtUtc,
            membership.LastDocumentVerifiedAtUtc,
            membership.CreatedAtUtc);

        return Ok(response);
    }

    /// <summary>
    /// Solicita tornar-se Resident no território.
    /// </summary>
    [HttpPost]
    [Route("/api/v1/memberships/{territoryId:guid}/become-resident")]
    [ProducesResponseType(typeof(MembershipDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<MembershipDetailResponse>> BecomeResident(
        [FromRoute] Guid territoryId,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var territory = await _territoryService.GetByIdAsync(territoryId, cancellationToken);
        if (territory is null)
        {
            return NotFound();
        }

        var result = await _membershipService.BecomeResidentAsync(
            userContext.User.Id,
            territoryId,
            cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error?.Contains("already has a Resident") == true)
            {
                return Conflict(new { error = result.Error });
            }
            return BadRequest(new { error = result.Error });
        }

        var membership = result.Value!;
        var response = new MembershipDetailResponse(
            membership.Id,
            membership.UserId,
            membership.TerritoryId,
            membership.Role.ToString().ToUpperInvariant(),
            membership.ResidencyVerification.ToString().ToUpperInvariant(),
            membership.LastGeoVerifiedAtUtc,
            membership.LastDocumentVerifiedAtUtc,
            membership.CreatedAtUtc);

        return Ok(response);
    }

    /// <summary>
    /// Transfere residência para outro território.
    /// </summary>
    [HttpPost]
    [Route("/api/v1/memberships/transfer-residency")]
    [ProducesResponseType(typeof(MembershipDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<MembershipDetailResponse>> TransferResidency(
        [FromBody] TransferResidencyRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var territory = await _territoryService.GetByIdAsync(request.ToTerritoryId, cancellationToken);
        if (territory is null)
        {
            return NotFound();
        }

        var result = await _membershipService.TransferResidencyAsync(
            userContext.User.Id,
            request.ToTerritoryId,
            cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error?.Contains("does not have a Resident") == true)
            {
                return BadRequest(new { error = result.Error });
            }
            return Conflict(new { error = result.Error });
        }

        var membership = result.Value!;
        var response = new MembershipDetailResponse(
            membership.Id,
            membership.UserId,
            membership.TerritoryId,
            membership.Role.ToString().ToUpperInvariant(),
            membership.ResidencyVerification.ToString().ToUpperInvariant(),
            membership.LastGeoVerifiedAtUtc,
            membership.LastDocumentVerifiedAtUtc,
            membership.CreatedAtUtc);

        return Ok(response);
    }

    /// <summary>
    /// Verifica residência por geolocalização.
    /// </summary>
    [HttpPost]
    [Route("/api/v1/memberships/{territoryId:guid}/verify-residency/geo")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> VerifyResidencyGeo(
        [FromRoute] Guid territoryId,
        [FromBody] VerifyResidencyGeoRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        // Validação de geolocalização: verifica se as coordenadas estão dentro do raio permitido do território
        var result = await _membershipService.VerifyResidencyByGeoAsync(
            userContext.User.Id,
            territoryId,
            request.Lat,
            request.Lng,
            DateTime.UtcNow,
            cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(new { message = "Residency verified by geolocation." });
    }

    /// <summary>
    /// Verifica residência por comprovante documental.
    /// </summary>
    [HttpPost]
    [Route("/api/v1/memberships/{territoryId:guid}/verify-residency/document")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> VerifyResidencyDocument(
        [FromRoute] Guid territoryId,
        [FromBody] Araponga.Api.Contracts.Memberships.VerifyResidencyDocumentRequest? request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        if (request is null || request.EvidenceId == Guid.Empty)
        {
            return BadRequest(new { error = "evidenceId is required." });
        }

        // Enfileira verificação documental (fallback humano: Curator do território).
        var result = await _verificationQueueService.SubmitResidencyDocumentAsync(
            userContext.User.Id,
            territoryId,
            request.EvidenceId,
            cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(new { message = "Document verification submitted.", workItemId = result.Value!.Id });
    }

    /// <summary>
    /// Upload de comprovante para verificação documental de residência.
    /// </summary>
    [HttpPost]
    [Route("/api/v1/memberships/{territoryId:guid}/verify-residency/document/upload")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UploadResidencyDocument(
        [FromRoute] Guid territoryId,
        [FromForm] IFormFile file,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        if (file is null || file.Length <= 0)
        {
            return BadRequest(new { error = "file is required." });
        }

        await using var stream = file.OpenReadStream();
        var created = await _documentEvidenceService.CreateAsync(
            userContext.User.Id,
            territoryId,
            Araponga.Domain.Evidence.DocumentEvidenceKind.Residency,
            file.FileName,
            file.ContentType,
            stream,
            cancellationToken);

        if (!created.IsSuccess || created.Value is null)
        {
            return BadRequest(new { error = created.Error ?? "Unable to upload evidence." });
        }

        var work = await _verificationQueueService.SubmitResidencyDocumentAsync(
            userContext.User.Id,
            territoryId,
            created.Value.Id,
            cancellationToken);

        if (!work.IsSuccess || work.Value is null)
        {
            return BadRequest(new { error = work.Error ?? "Unable to submit residency verification." });
        }

        return Ok(new { message = "Document verification submitted.", evidenceId = created.Value.Id, workItemId = work.Value.Id });
    }

    /// <summary>
    /// Consulta meu estado de membership no território.
    /// </summary>
    [HttpGet]
    [Route("/api/v1/memberships/{territoryId:guid}/me")]
    [ProducesResponseType(typeof(MembershipDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MembershipDetailResponse>> GetMyMembership(
        [FromRoute] Guid territoryId,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var membership = await _membershipRepository.GetByUserAndTerritoryAsync(
            userContext.User.Id,
            territoryId,
            cancellationToken);

        if (membership is null)
        {
            return NotFound();
        }

        var response = new MembershipDetailResponse(
            membership.Id,
            membership.UserId,
            membership.TerritoryId,
            membership.Role.ToString().ToUpperInvariant(),
            membership.ResidencyVerification.ToString().ToUpperInvariant(),
            membership.LastGeoVerifiedAtUtc,
            membership.LastDocumentVerifiedAtUtc,
            membership.CreatedAtUtc);

        return Ok(response);
    }

    /// <summary>
    /// Lista todos os meus memberships.
    /// </summary>
    [HttpGet]
    [Route("/api/v1/memberships/me")]
    [ProducesResponseType(typeof(MyMembershipsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<MyMembershipsResponse>> ListMyMemberships(
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var memberships = await _membershipService.ListMyMembershipsAsync(
            userContext.User.Id,
            cancellationToken);

        var response = new MyMembershipsResponse(
            memberships.Select(m => new MembershipDetailResponse(
                m.Id,
                m.UserId,
                m.TerritoryId,
                m.Role.ToString().ToUpperInvariant(),
                m.ResidencyVerification.ToString().ToUpperInvariant(),
                m.LastGeoVerifiedAtUtc,
                m.LastDocumentVerifiedAtUtc,
                m.CreatedAtUtc)).ToList());

        return Ok(response);
    }

    private bool RequiresPresence(MembershipRole role)
    {
        return _presencePolicy.Policy switch
        {
            PresencePolicy.None => false,
            PresencePolicy.ResidentOnly => role == MembershipRole.Resident,
            PresencePolicy.VisitorAndResident => role == MembershipRole.Resident || role == MembershipRole.Visitor,
            _ => role == MembershipRole.Resident
        };
    }
}
