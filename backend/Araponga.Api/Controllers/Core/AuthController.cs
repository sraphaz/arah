using Araponga.Api.Configuration;
using Araponga.Api.Contracts.Auth;
using Araponga.Api.Security;
using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Application.Services;
using Araponga.Domain.Email;
using Araponga.Domain.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
[Produces("application/json")]
[Tags("Auth")]
public sealed class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenStore _refreshTokenStore;
    private readonly IUserRepository _userRepository;
    private readonly CurrentUserAccessor _currentUserAccessor;
    private readonly ClientCredentialsOptions _clientCredentialsOptions;

    public AuthController(
        AuthService authService,
        ITokenService tokenService,
        IRefreshTokenStore refreshTokenStore,
        IUserRepository userRepository,
        CurrentUserAccessor currentUserAccessor,
        IOptions<ClientCredentialsOptions> clientCredentialsOptions)
    {
        _authService = authService;
        _tokenService = tokenService;
        _refreshTokenStore = refreshTokenStore;
        _userRepository = userRepository;
        _currentUserAccessor = currentUserAccessor;
        _clientCredentialsOptions = clientCredentialsOptions.Value;
    }

    /// <summary>
    /// Autentica o usuário via login social (MVP).
    /// </summary>
    /// <remarks>
    /// Este endpoint retorna um token simples para o MVP.
    /// </remarks>
    [HttpPost("social")]
    [EnableRateLimiting("auth")]
    [ProducesResponseType(typeof(SocialLoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
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

        var (user, accessToken, refreshToken) = result.Value!;
        var expiresInSeconds = (int)TimeSpan.FromMinutes(_tokenService.GetAccessTokenExpirationMinutes()).TotalSeconds;
        var response = new SocialLoginResponse(
            new UserResponse(user.Id, user.DisplayName, user.Email),
            accessToken,
            refreshToken,
            expiresInSeconds);

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

        var (user, accessToken, refreshToken) = result.Value!;
        var expiresInSeconds = (int)TimeSpan.FromMinutes(_tokenService.GetAccessTokenExpirationMinutes()).TotalSeconds;
        var response = new SocialLoginResponse(
            new UserResponse(user.Id, user.DisplayName, user.Email),
            accessToken,
            refreshToken,
            expiresInSeconds);

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

        var (user, accessToken, refreshToken) = result.Value!;
        var expiresInSeconds = (int)TimeSpan.FromMinutes(_tokenService.GetAccessTokenExpirationMinutes()).TotalSeconds;
        var response = new SocialLoginResponse(
            new UserResponse(user.Id, user.DisplayName, user.Email),
            accessToken,
            refreshToken,
            expiresInSeconds);

        return Ok(response);
    }

    /// <summary>
    /// Solicita recuperação de senha via email.
    /// </summary>
    [HttpPost("forgot-password")]
    [EnableRateLimiting("auth")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public Task<ActionResult> ForgotPassword(
        [FromBody] ForgotPasswordRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return Task.FromResult<ActionResult>(BadRequest(new { error = "Email is required." }));
        }

        // Sempre retornar sucesso (security best practice - não revelar se email existe)
        _ = Task.Run(async () =>
        {
            try
            {
                using var scope = HttpContext.RequestServices.CreateScope();
                var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                var emailQueueService = scope.ServiceProvider.GetRequiredService<EmailQueueService>();
                var configuration = scope.ServiceProvider.GetService<Microsoft.Extensions.Configuration.IConfiguration>();
                var baseUrl = configuration?["BaseUrl"] ?? "https://araponga.com";

                var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
                if (user is null || string.IsNullOrWhiteSpace(user.Email))
                {
                    // Não revelar se email existe ou não
                    return;
                }

                // Gerar token de reset (simplificado - em produção, usar um sistema mais robusto)
                var resetToken = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32))
                    .Replace("+", "-")
                    .Replace("/", "_")
                    .Replace("=", "");
                var resetLink = $"{baseUrl}/reset-password?token={resetToken}&email={Uri.EscapeDataString(user.Email)}";
                var expirationMinutes = 60;

                var templateData = new PasswordResetEmailTemplateData
                {
                    UserName = user.DisplayName,
                    BaseUrl = baseUrl,
                    ResetLink = resetLink,
                    ExpirationMinutes = expirationMinutes
                };

                var emailMessage = new EmailMessage
                {
                    To = user.Email,
                    Subject = "Recuperação de Senha - Araponga",
                    TemplateName = "password-reset",
                    TemplateData = templateData,
                    Body = string.Empty
                };

                await emailQueueService.EnqueueEmailAsync(
                    emailMessage,
                    EmailQueuePriority.High,
                    cancellationToken: cancellationToken);
            }
            catch (Exception)
            {
                // Logar erro silenciosamente - não revelar falha ao usuário
            }
        }, cancellationToken);

        return Task.FromResult<ActionResult>(Ok(new { message = "If the email exists, a password reset link has been sent." }));
    }

    /// <summary>
    /// Emite token para workers/sistemas (client credentials). O BFF não usa; workers que precisam chamar a API usam client_id + client_secret configurados.
    /// </summary>
    [HttpPost("token")]
    [EnableRateLimiting("auth")]
    [ProducesResponseType(typeof(ClientCredentialsTokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public ActionResult<ClientCredentialsTokenResponse> ClientCredentialsToken(
        [FromBody] ClientCredentialsTokenRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.GrantType) ||
            !string.Equals(request.GrantType, "client_credentials", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new { error = "grant_type must be client_credentials." });
        }
        if (string.IsNullOrWhiteSpace(request.ClientId) || string.IsNullOrWhiteSpace(request.ClientSecret))
        {
            return BadRequest(new { error = "client_id and client_secret are required." });
        }
        if (string.IsNullOrWhiteSpace(_clientCredentialsOptions.ClientId) ||
            string.IsNullOrWhiteSpace(_clientCredentialsOptions.ClientSecret))
        {
            return Unauthorized();
        }
        if (!string.Equals(request.ClientId, _clientCredentialsOptions.ClientId, StringComparison.Ordinal) ||
            !string.Equals(request.ClientSecret, _clientCredentialsOptions.ClientSecret, StringComparison.Ordinal))
        {
            return Unauthorized();
        }

        var accessToken = _tokenService.IssueSystemToken(request.ClientId);
        return Ok(new ClientCredentialsTokenResponse(accessToken, "Bearer", 15 * 60)); // 15 min
    }

    /// <summary>
    /// Renova o access token usando o refresh token (padrão OAuth: expiração + rotação).
    /// O BFF/frontend chama este endpoint quando o access token expira; o refresh token é consumido uma vez e novos tokens são retornados.
    /// </summary>
    [HttpPost("refresh")]
    [EnableRateLimiting("auth")]
    [ProducesResponseType(typeof(SocialLoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<SocialLoginResponse>> Refresh(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return BadRequest(new { error = "RefreshToken is required." });
        }

        var userId = await _refreshTokenStore.ConsumeAsync(request.RefreshToken, cancellationToken);
        if (userId is null)
        {
            return BadRequest(new { error = "Invalid or expired refresh token." });
        }

        var user = await _userRepository.GetByIdAsync(userId.Value, cancellationToken);
        if (user is null)
        {
            return BadRequest(new { error = "User not found." });
        }

        var (newRefreshToken, _) = _refreshTokenStore.Issue(user.Id);
        var accessToken = _tokenService.IssueToken(user.Id);
        var expiresInSeconds = (int)TimeSpan.FromMinutes(_tokenService.GetAccessTokenExpirationMinutes()).TotalSeconds;

        var response = new SocialLoginResponse(
            new UserResponse(user.Id, user.DisplayName, user.Email),
            accessToken,
            newRefreshToken,
            expiresInSeconds);

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
