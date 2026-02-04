using Araponga.Api.Contracts.Policies;
using Araponga.Api.Security;
using Araponga.Application.Services;
using Araponga.Domain.Policies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Text.Json;

namespace Araponga.Api.Controllers;

/// <summary>
/// Controller para gerenciar Termos de Uso.
/// </summary>
[ApiController]
[Route("api/v1/terms")]
[Produces("application/json")]
[Tags("Terms of Service")]
public sealed class TermsOfServiceController : ControllerBase
{
    private readonly TermsOfServiceService _termsService;
    private readonly TermsAcceptanceService _acceptanceService;
    private readonly PolicyRequirementService _policyRequirementService;
    private readonly CurrentUserAccessor _currentUserAccessor;
    private readonly AccessEvaluator _accessEvaluator;

    public TermsOfServiceController(
        TermsOfServiceService termsService,
        TermsAcceptanceService acceptanceService,
        PolicyRequirementService policyRequirementService,
        CurrentUserAccessor currentUserAccessor,
        AccessEvaluator accessEvaluator)
    {
        _termsService = termsService;
        _acceptanceService = acceptanceService;
        _policyRequirementService = policyRequirementService;
        _currentUserAccessor = currentUserAccessor;
        _accessEvaluator = accessEvaluator;
    }

    /// <summary>
    /// Obtém todos os termos ativos.
    /// </summary>
    [HttpGet("active")]
    [EnableRateLimiting("read")]
    [ProducesResponseType(typeof(IEnumerable<TermsOfServiceResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<IEnumerable<TermsOfServiceResponse>>> GetActiveTerms(
        CancellationToken cancellationToken)
    {
        var terms = await _termsService.GetActiveTermsAsync(cancellationToken);
        return Ok(terms.Select(ToResponse));
    }

    /// <summary>
    /// Obtém termos por ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [EnableRateLimiting("read")]
    [ProducesResponseType(typeof(TermsOfServiceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<TermsOfServiceResponse>> GetTermsById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var terms = await _termsService.GetTermsByIdAsync(id, cancellationToken);
        if (terms is null)
        {
            return NotFound();
        }

        return Ok(ToResponse(terms));
    }

    /// <summary>
    /// Obtém termos obrigatórios para o usuário autenticado.
    /// </summary>
    [HttpGet("required")]
    [EnableRateLimiting("read")]
    [ProducesResponseType(typeof(IEnumerable<TermsOfServiceResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<IEnumerable<TermsOfServiceResponse>>> GetRequiredTerms(
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

        return Ok(requirements.RequiredTerms.Select(ToResponse));
    }

    /// <summary>
    /// Aceita termos de serviço.
    /// </summary>
    [HttpPost("{id:guid}/accept")]
    [EnableRateLimiting("write")]
    [ProducesResponseType(typeof(TermsAcceptanceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<TermsAcceptanceResponse>> AcceptTerms(
        [FromRoute] Guid id,
        [FromBody] AcceptTermsRequest? request,
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
        if (request is not null && request.TermsId != Guid.Empty && request.TermsId != id)
        {
            return BadRequest(new { error = "Terms ID in route must match Terms ID in request body." });
        }

        var result = await _acceptanceService.AcceptTermsAsync(
            userContext.User.Id,
            id,
            ipAddress,
            userAgent,
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            return BadRequest(new { error = result.Error ?? "Unable to accept terms." });
        }

        return Ok(ToResponse(result.Value));
    }

    /// <summary>
    /// Obtém histórico de aceites do usuário autenticado.
    /// </summary>
    [HttpGet("acceptances")]
    [EnableRateLimiting("read")]
    [ProducesResponseType(typeof(IEnumerable<TermsAcceptanceResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<IEnumerable<TermsAcceptanceResponse>>> GetAcceptances(
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
    /// Revoga aceite de termos (opcional).
    /// </summary>
    [HttpDelete("{id:guid}/accept")]
    [EnableRateLimiting("write")]
    [ProducesResponseType(typeof(TermsAcceptanceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<TermsAcceptanceResponse>> RevokeAcceptance(
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

    private static TermsOfServiceResponse ToResponse(TermsOfService terms)
    {
        return new TermsOfServiceResponse
        {
            Id = terms.Id,
            Version = terms.Version,
            Title = terms.Title,
            Content = terms.Content,
            EffectiveDate = terms.EffectiveDate,
            ExpirationDate = terms.ExpirationDate,
            IsActive = terms.IsActive,
            RequiredRoles = ParseJsonArray<int>(terms.RequiredRoles),
            RequiredCapabilities = ParseJsonArray<int>(terms.RequiredCapabilities),
            RequiredSystemPermissions = ParseJsonArray<int>(terms.RequiredSystemPermissions),
            CreatedAtUtc = terms.CreatedAtUtc,
            UpdatedAtUtc = terms.UpdatedAtUtc
        };
    }

    private static TermsAcceptanceResponse ToResponse(TermsAcceptance acceptance)
    {
        return new TermsAcceptanceResponse
        {
            Id = acceptance.Id,
            UserId = acceptance.UserId,
            TermsOfServiceId = acceptance.TermsOfServiceId,
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
