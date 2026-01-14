using Araponga.Api;
using Araponga.Api.Configuration;
using Araponga.Api.Contracts.Memberships;
using Araponga.Api.Security;
using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Domain.Social;
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
    private readonly CurrentUserAccessor _currentUserAccessor;
    private readonly TerritoryService _territoryService;
    private readonly AccessEvaluator _accessEvaluator;
    private readonly ITerritoryMembershipRepository _membershipRepository;
    private readonly PresencePolicyOptions _presencePolicy;

    public MembershipsController(
        MembershipService membershipService,
        CurrentUserAccessor currentUserAccessor,
        TerritoryService territoryService,
        AccessEvaluator accessEvaluator,
        ITerritoryMembershipRepository membershipRepository,
        IOptions<PresencePolicyOptions> presencePolicy)
    {
        _membershipService = membershipService;
        _currentUserAccessor = currentUserAccessor;
        _territoryService = territoryService;
        _accessEvaluator = accessEvaluator;
        _membershipRepository = membershipRepository;
        _presencePolicy = presencePolicy.Value;
    }

    /// <summary>
    /// Declara vínculo social com um território (visitor/resident).
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(MembershipResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MembershipResponse>> DeclareMembership(
        [FromRoute] Guid territoryId,
        [FromBody] DeclareMembershipRequest request,
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

        if (!Enum.TryParse<MembershipRole>(request.Role, true, out var role))
        {
            return BadRequest(new { error = "Invalid role." });
        }

        if (RequiresPresence(role) &&
            !GeoHeaderReader.TryGetCoordinates(Request.Headers, out _, out _))
        {
            var observabilityLogger = HttpContext.RequestServices.GetRequiredService<Araponga.Application.Interfaces.IObservabilityLogger>();
            observabilityLogger.LogGeolocationError(
                "DeclareMembership",
                "Missing geo headers for RESIDENT role",
                userContext.User?.Id,
                territoryId);

            return BadRequest(new { error = "X-Geo-Latitude and X-Geo-Longitude headers are required to declare membership." });
        }

        var membership = await _membershipService.DeclareMembershipAsync(
            userContext.User.Id,
            territoryId,
            role,
            cancellationToken);

        var response = new MembershipResponse(
            membership.Id,
            membership.UserId,
            membership.TerritoryId,
            membership.Role.ToString().ToUpperInvariant(),
            membership.VerificationStatus.ToString().ToUpperInvariant(),
            membership.CreatedAtUtc);

        return CreatedAtAction(nameof(DeclareMembership), new { territoryId }, response);
    }

    /// <summary>
    /// Retorna o status do vínculo do usuário com o território.
    /// </summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(MembershipStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<MembershipStatusResponse>> GetStatus(
        [FromRoute] Guid territoryId,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var status = await _membershipService.GetStatusAsync(
            userContext.User.Id,
            territoryId,
            cancellationToken);

        var role = await _accessEvaluator.GetRoleAsync(
            userContext.User.Id,
            territoryId,
            cancellationToken);

        var response = new MembershipStatusResponse(
            userContext.User.Id,
            territoryId,
            role?.ToString().ToUpperInvariant() ?? "NONE",
            status?.ToString().ToUpperInvariant() ?? "NONE");

        return Ok(response);
    }

    /// <summary>
    /// Valida um vínculo de morador (curadoria).
    /// </summary>
    [HttpPatch("{membershipId:guid}/validation")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Validate(
        [FromRoute] Guid territoryId,
        [FromRoute] Guid membershipId,
        [FromBody] ValidateMembershipRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var canValidate = await _accessEvaluator.IsResidentAsync(
            userContext.User.Id,
            territoryId,
            cancellationToken);

        if (!canValidate)
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        if (!Enum.TryParse<VerificationStatus>(request.Status, true, out var status))
        {
            return BadRequest(new { error = "Invalid status." });
        }

        var success = await _membershipService.ValidateAsync(
            membershipId,
            userContext.User.Id,
            territoryId,
            status,
            cancellationToken);
        return success ? NoContent() : BadRequest(new { error = "Membership not found." });
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
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        // Nota: Upload de comprovante será processado quando o sistema de assets/upload for implementado.
        // Por enquanto, a verificação documental é apenas registrada sem validação de arquivo.
        // Quando implementado, o endpoint deve:
        // 1. Receber arquivo via multipart/form-data ou referência a asset existente
        // 2. Validar tipo e tamanho do arquivo
        // 3. Armazenar referência ao asset no banco
        // 4. Criar registro de verificação pendente para aprovação manual
        var result = await _membershipService.VerifyResidencyByDocumentAsync(
            userContext.User.Id,
            territoryId,
            DateTime.UtcNow,
            cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(new { message = "Document verification submitted." });
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
