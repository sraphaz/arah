using Araponga.Api.Contracts.Policies;
using Araponga.Api.Security;
using Araponga.Application.Services;
using Araponga.Domain.Policies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Text.Json;

namespace Araponga.Api.Controllers;

/// <summary>
/// Controller para gerenciar Políticas de Privacidade.
/// </summary>
[ApiController]
[Route("api/v1/privacy")]
[Produces("application/json")]
[Tags("Privacy Policy")]
public sealed class PrivacyPolicyController : ControllerBase
{
    private readonly PrivacyPolicyService _policyService;
    private readonly PrivacyPolicyAcceptanceService _acceptanceService;
    private readonly PolicyRequirementService _policyRequirementService;
    private readonly CurrentUserAccessor _currentUserAccessor;

    public PrivacyPolicyController(
        PrivacyPolicyService policyService,
        PrivacyPolicyAcceptanceService acceptanceService,
        PolicyRequirementService policyRequirementService,
        CurrentUserAccessor currentUserAccessor)
    {
        _policyService = policyService;
        _acceptanceService = acceptanceService;
        _policyRequirementService = policyRequirementService;
        _currentUserAccessor = currentUserAccessor;
    }

    /// <summary>
    /// Obtém todas as políticas ativas.
    /// </summary>
    [HttpGet("active")]
    [EnableRateLimiting("read")]
    [ProducesResponseType(typeof(IEnumerable<PrivacyPolicyResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<IEnumerable<PrivacyPolicyResponse>>> GetActivePolicies(
        CancellationToken cancellationToken)
    {
        var policies = await _policyService.GetActivePoliciesAsync(cancellationToken);
        return Ok(policies.Select(ToResponse));
    }

    /// <summary>
    /// Obtém política por ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [EnableRateLimiting("read")]
    [ProducesResponseType(typeof(PrivacyPolicyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<PrivacyPolicyResponse>> GetPolicyById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var policy = await _policyService.GetPolicyByIdAsync(id, cancellationToken);
        if (policy is null)
        {
            return NotFound();
        }

        return Ok(ToResponse(policy));
    }

    /// <summary>
    /// Obtém políticas obrigatórias para o usuário autenticado.
    /// </summary>
    [HttpGet("required")]
    [EnableRateLimiting("read")]
    [ProducesResponseType(typeof(IEnumerable<PrivacyPolicyResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<IEnumerable<PrivacyPolicyResponse>>> GetRequiredPolicies(
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var requirements = await _policyRequirementService.GetRequiredPoliciesForUserAsync(
            userContext.User.Id,
            cancellationToken);

        return Ok(requirements.RequiredPrivacyPolicies.Select(ToResponse));
    }

    /// <summary>
    /// Aceita política de privacidade.
    /// </summary>
    [HttpPost("{id:guid}/accept")]
    [EnableRateLimiting("write")]
    [ProducesResponseType(typeof(PrivacyPolicyAcceptanceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<PrivacyPolicyAcceptanceResponse>> AcceptPolicy(
        [FromRoute] Guid id,
        [FromBody] AcceptPrivacyPolicyRequest? request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var ipAddress = request?.IpAddress ?? HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = request?.UserAgent ?? Request.Headers["User-Agent"].ToString();

        // Garantir que o ID da rota corresponde ao ID do request body (se fornecido)
        if (request is not null && request.PolicyId != Guid.Empty && request.PolicyId != id)
        {
            return BadRequest(new { error = "Policy ID in route must match Policy ID in request body." });
        }

        var result = await _acceptanceService.AcceptPolicyAsync(
            userContext.User.Id,
            id,
            ipAddress,
            userAgent,
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            return BadRequest(new { error = result.Error ?? "Unable to accept policy." });
        }

        return Ok(ToResponse(result.Value));
    }

    /// <summary>
    /// Obtém histórico de aceites do usuário autenticado.
    /// </summary>
    [HttpGet("acceptances")]
    [EnableRateLimiting("read")]
    [ProducesResponseType(typeof(IEnumerable<PrivacyPolicyAcceptanceResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<IEnumerable<PrivacyPolicyAcceptanceResponse>>> GetAcceptances(
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var acceptances = await _acceptanceService.GetAcceptanceHistoryAsync(
            userContext.User.Id,
            cancellationToken);

        return Ok(acceptances.Select(ToResponse));
    }

    /// <summary>
    /// Revoga aceite de política (opcional).
    /// </summary>
    [HttpDelete("{id:guid}/accept")]
    [EnableRateLimiting("write")]
    [ProducesResponseType(typeof(PrivacyPolicyAcceptanceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<PrivacyPolicyAcceptanceResponse>> RevokeAcceptance(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _acceptanceService.RevokeAcceptanceAsync(
            userContext.User.Id,
            id,
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            return BadRequest(new { error = result.Error ?? "Unable to revoke acceptance." });
        }

        return Ok(ToResponse(result.Value));
    }

    private static PrivacyPolicyResponse ToResponse(PrivacyPolicy policy)
    {
        return new PrivacyPolicyResponse
        {
            Id = policy.Id,
            Version = policy.Version,
            Title = policy.Title,
            Content = policy.Content,
            EffectiveDate = policy.EffectiveDate,
            ExpirationDate = policy.ExpirationDate,
            IsActive = policy.IsActive,
            RequiredRoles = ParseJsonArray<int>(policy.RequiredRoles),
            RequiredCapabilities = ParseJsonArray<int>(policy.RequiredCapabilities),
            RequiredSystemPermissions = ParseJsonArray<int>(policy.RequiredSystemPermissions),
            CreatedAtUtc = policy.CreatedAtUtc,
            UpdatedAtUtc = policy.UpdatedAtUtc
        };
    }

    private static PrivacyPolicyAcceptanceResponse ToResponse(PrivacyPolicyAcceptance acceptance)
    {
        return new PrivacyPolicyAcceptanceResponse
        {
            Id = acceptance.Id,
            UserId = acceptance.UserId,
            PrivacyPolicyId = acceptance.PrivacyPolicyId,
            AcceptedAtUtc = acceptance.AcceptedAtUtc,
            IpAddress = acceptance.IpAddress,
            UserAgent = acceptance.UserAgent,
            AcceptedVersion = acceptance.AcceptedVersion,
            IsRevoked = acceptance.IsRevoked,
            RevokedAtUtc = acceptance.RevokedAtUtc
        };
    }

    private static List<T>? ParseJsonArray<T>(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<List<T>>(json);
        }
        catch
        {
            return null;
        }
    }
}
