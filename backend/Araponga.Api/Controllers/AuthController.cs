using Araponga.Api.Contracts.Auth;
using Araponga.Api.Security;
using Araponga.Application.Interfaces;
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
    private readonly ITokenService _tokenService;
    private readonly CurrentUserAccessor _currentUserAccessor;

    public AuthController(
        AuthService authService,
        ITokenService tokenService,
        CurrentUserAccessor currentUserAccessor)
    {
        _authService = authService;
        _tokenService = tokenService;
        _currentUserAccessor = currentUserAccessor;
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

        if (string.IsNullOrWhiteSpace(request.AuthProvider) ||
            string.IsNullOrWhiteSpace(request.ExternalId) ||
            string.IsNullOrWhiteSpace(request.DisplayName) ||
            (!hasCpf && !hasForeignDocument))
        {
            return BadRequest(new { error = "AuthProvider, ExternalId, DisplayName, and CPF or foreign document are required." });
        }

        if (hasCpf && hasForeignDocument)
        {
            return BadRequest(new { error = "Provide either CPF or foreign document, not both." });
        }

        var result = await _authService.LoginSocialAsync(
            request.AuthProvider,
            request.ExternalId,
            request.DisplayName,
            request.Email,
            request.Cpf,
            request.ForeignDocument,
            request.PhoneNumber,
            request.Address,
            cancellationToken);

        if (result.IsFailure)
        {
            // Verificar se é 2FA_REQUIRED
            if (result.Error?.StartsWith("2FA_REQUIRED:") == true)
            {
                var challengeId = result.Error.Substring("2FA_REQUIRED:".Length);
                return BadRequest(new { error = "2FA_REQUIRED", challengeId });
            }

            return BadRequest(new { error = result.Error });
        }

        var (user, token) = result.Value!;
        var response = new SocialLoginResponse(
            new UserResponse(user.Id, user.DisplayName, user.Email),
            token);

        return Ok(response);
    }

    /// <summary>
    /// Inicia o setup de 2FA, gerando secret e QR code.
    /// </summary>
    [HttpPost("2fa/setup")]
    [ProducesResponseType(typeof(TwoFactorSetupResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TwoFactorSetupResponse>> Setup2FA(
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _authService.Setup2FAAsync(
            userContext.User.Id,
            userContext.User.Email ?? userContext.User.DisplayName,
            cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error });
        }

        var setup = result.Value!;
        var response = new TwoFactorSetupResponse(
            setup.Secret,
            setup.QrCodeUri,
            setup.ManualEntryKey);

        return Ok(response);
    }

    /// <summary>
    /// Confirma e habilita 2FA após validar código TOTP.
    /// </summary>
    [HttpPost("2fa/confirm")]
    [ProducesResponseType(typeof(TwoFactorConfirmResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TwoFactorConfirmResponse>> Confirm2FA(
        [FromBody] TwoFactorConfirmRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _authService.Confirm2FAAsync(
            userContext.User.Id,
            request.Secret,
            request.Code,
            cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error });
        }

        var confirm = result.Value!;
        var response = new TwoFactorConfirmResponse(confirm.RecoveryCodes);

        return Ok(response);
    }

    /// <summary>
    /// Verifica código 2FA e retorna JWT.
    /// </summary>
    [HttpPost("2fa/verify")]
    [ProducesResponseType(typeof(SocialLoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SocialLoginResponse>> Verify2FA(
        [FromBody] TwoFactorVerifyRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.Verify2FAAsync(
            request.ChallengeId,
            request.Code,
            cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error });
        }

        var token = result.Value!;
        var userId = _tokenService.ParseToken(token);
        if (userId is null)
        {
            return BadRequest(new { error = "Invalid token." });
        }

        // Buscar user para response
        var userRepository = HttpContext.RequestServices.GetRequiredService<IUserRepository>();
        var user = await userRepository.GetByIdAsync(userId.Value, cancellationToken);
        if (user is null)
        {
            return BadRequest(new { error = "User not found." });
        }

        var response = new SocialLoginResponse(
            new UserResponse(user.Id, user.DisplayName, user.Email),
            token);

        return Ok(response);
    }

    /// <summary>
    /// Usa recovery code para autenticação.
    /// </summary>
    [HttpPost("2fa/recover")]
    [ProducesResponseType(typeof(SocialLoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SocialLoginResponse>> Recover2FA(
        [FromBody] TwoFactorRecoverRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.Recover2FAAsync(
            request.ChallengeId,
            request.RecoveryCode,
            cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error });
        }

        var token = result.Value!;
        var userId = _tokenService.ParseToken(token);
        if (userId is null)
        {
            return BadRequest(new { error = "Invalid token." });
        }

        var userRepository = HttpContext.RequestServices.GetRequiredService<IUserRepository>();
        var user = await userRepository.GetByIdAsync(userId.Value, cancellationToken);
        if (user is null)
        {
            return BadRequest(new { error = "User not found." });
        }

        var response = new SocialLoginResponse(
            new UserResponse(user.Id, user.DisplayName, user.Email),
            token);

        return Ok(response);
    }

    /// <summary>
    /// Desabilita 2FA para o usuário.
    /// </summary>
    [HttpPost("2fa/disable")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Disable2FA(
        [FromBody] TwoFactorDisableRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        // TODO: Validar senha ou código 2FA se fornecido
        var result = await _authService.Disable2FAAsync(
            userContext.User.Id,
            request.Password ?? request.Code,
            cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(new { message = "2FA disabled successfully." });
    }
}
