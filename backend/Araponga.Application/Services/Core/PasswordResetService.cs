using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Araponga.Application.Common;
using Araponga.Application.Configuration;
using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Araponga.Application.Services;

public sealed class PasswordResetService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IEmailSender _emailSender;
    private readonly IDistributedCache _cache;
    private readonly ILogger<PasswordResetService> _logger;
    private readonly PasswordResetOptions _options;

    public PasswordResetService(
        IUserRepository userRepository,
        ITokenService tokenService,
        IEmailSender emailSender,
        IDistributedCache cache,
        ILogger<PasswordResetService> logger,
        IOptions<PasswordResetOptions> options)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _emailSender = emailSender;
        _cache = cache;
        _logger = logger;
        _options = options.Value;
    }

    public async Task<OperationResult> RequestAsync(string email, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return OperationResult.Failure("Email é obrigatório.");
        }

        var normalizedEmail = email.Trim();
        var user = await _userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);
        if (user is null)
        {
            _logger.LogInformation("Password reset requested for unknown email.");
            return OperationResult.Success();
        }

        var token = GenerateToken();
        var tokenHash = HashToken(token);
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(_options.TokenTtlMinutes);

        var payload = new PasswordResetTokenPayload(user.Id, expiresAtUtc);
        var cacheKey = BuildCacheKey(tokenHash);

        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(payload, JsonOptions),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_options.TokenTtlMinutes)
            },
            cancellationToken);

        var resetLink = BuildResetLink(token);
        var body = string.IsNullOrWhiteSpace(resetLink)
            ? $"Use este token para recuperar o acesso: {token}"
            : $"Clique para recuperar o acesso: {resetLink}";

        await _emailSender.SendEmailAsync(
            user.Email ?? normalizedEmail,
            "Recuperação de acesso",
            body,
            isHtml: false,
            cancellationToken);

        return OperationResult.Success();
    }

    public async Task<Result<string>> ConfirmAsync(string token, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return Result<string>.Failure("Token inválido.");
        }

        var tokenHash = HashToken(token.Trim());
        var cacheKey = BuildCacheKey(tokenHash);
        var payloadJson = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (string.IsNullOrWhiteSpace(payloadJson))
        {
            return Result<string>.Failure("Token inválido ou expirado.");
        }

        var payload = JsonSerializer.Deserialize<PasswordResetTokenPayload>(payloadJson, JsonOptions);
        if (payload is null || payload.ExpiresAtUtc <= DateTime.UtcNow)
        {
            await _cache.RemoveAsync(cacheKey, cancellationToken);
            return Result<string>.Failure("Token inválido ou expirado.");
        }

        await _cache.RemoveAsync(cacheKey, cancellationToken);
        var jwt = _tokenService.IssueToken(payload.UserId);

        return Result<string>.Success(jwt);
    }

    private string BuildResetLink(string token)
    {
        if (string.IsNullOrWhiteSpace(_options.ResetUrlBase))
        {
            return string.Empty;
        }

        var separator = _options.ResetUrlBase.Contains('?') ? "&" : "?";
        return $"{_options.ResetUrlBase}{separator}token={Uri.EscapeDataString(token)}";
    }

    private static string GenerateToken()
    {
        var bytes = new byte[32];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
    }

    private static string HashToken(string token)
    {
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(hashBytes);
    }

    private static string BuildCacheKey(string tokenHash) => $"password_reset:{tokenHash}";
}
