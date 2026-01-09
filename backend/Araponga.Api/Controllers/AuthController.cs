using Araponga.Api.Contracts.Auth;
using Araponga.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
[Produces("application/json")]
[Tags("Auth")]
public sealed class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Autentica o usuário via login social (MVP).
    /// </summary>
    /// <remarks>
    /// Este endpoint retorna um token simples para o MVP.
    /// </remarks>
    [HttpPost("social")]
    [ProducesResponseType(typeof(SocialLoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SocialLoginResponse>> SocialLogin(
        [FromBody] SocialLoginRequest request,
        CancellationToken cancellationToken)
    {
        var hasCpf = !string.IsNullOrWhiteSpace(request.Cpf);
        var hasForeignDocument = !string.IsNullOrWhiteSpace(request.ForeignDocument);

        if (string.IsNullOrWhiteSpace(request.Provider) ||
            string.IsNullOrWhiteSpace(request.ExternalId) ||
            string.IsNullOrWhiteSpace(request.DisplayName) ||
            (!hasCpf && !hasForeignDocument))
        {
            return BadRequest(new { error = "Provider, ExternalId, DisplayName, and CPF or foreign document are required." });
        }

        if (hasCpf && hasForeignDocument)
        {
            return BadRequest(new { error = "Provide either CPF or foreign document, not both." });
        }

        var (user, token) = await _authService.LoginSocialAsync(
            request.Provider,
            request.ExternalId,
            request.DisplayName,
            request.Email,
            request.Cpf,
            request.ForeignDocument,
            request.PhoneNumber,
            request.Address,
            cancellationToken);

        var response = new SocialLoginResponse(
            new UserResponse(user.Id, user.DisplayName, user.Email),
            token);

        return Ok(response);
    }
}
