using System.Security.Cryptography;
using System.Text;
using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Domain.Users;
using OtpNet;

namespace Araponga.Application.Services;

public sealed class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;
    private static readonly TimeSpan TwoFactorChallengeExpiration = TimeSpan.FromMinutes(5);

    // Cache temporário para challenges 2FA (em produção, usar Redis ou similar)
    private static readonly Dictionary<string, (Guid userId, DateTime expiresAt)> _twoFactorChallenges = new();

    public AuthService(IUserRepository userRepository, ITokenService tokenService, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Realiza login/cadastro via provedor de autenticação social.
    /// </summary>
    /// <param name="authProvider">Provedor de autenticação (ex: "google", "apple", "facebook").</param>
    /// <param name="externalId">ID único do usuário no provedor de autenticação.</param>
    /// <param name="displayName">Nome de exibição do usuário.</param>
    /// <param name="email">Endereço de e-mail (opcional).</param>
    /// <param name="cpf">CPF brasileiro (opcional, mutuamente exclusivo com foreignDocument).</param>
    /// <param name="foreignDocument">Documento de identificação estrangeiro (opcional, mutuamente exclusivo com cpf).</param>
    /// <param name="phoneNumber">Número de telefone (opcional).</param>
    /// <param name="address">Endereço físico (opcional).</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Resultado contendo o usuário e token JWT, ou erro se 2FA for necessário.</returns>
    public async Task<Result<(User user, string token)>> LoginSocialAsync(
        string authProvider,
        string externalId,
        string displayName,
        string? email,
        string? cpf,
        string? foreignDocument,
        string? phoneNumber,
        string? address,
        CancellationToken cancellationToken)
    {
        var existing = await _userRepository.GetByAuthProviderAsync(authProvider, externalId, cancellationToken);
        if (existing is not null)
        {
            // Se 2FA está habilitado, retornar challenge
            if (existing.TwoFactorEnabled)
            {
                var challengeId = Guid.NewGuid().ToString("N");
                _twoFactorChallenges[challengeId] = (existing.Id, DateTime.UtcNow.Add(TwoFactorChallengeExpiration));
                
                // Limpar challenges expirados
                CleanupExpiredChallenges();
                
                return Result<(User user, string token)>.Failure($"2FA_REQUIRED:{challengeId}");
            }

            return Result<(User user, string token)>.Success((existing, _tokenService.IssueToken(existing.Id)));
        }

        var user = new User(
            Guid.NewGuid(),
            displayName,
            email,
            cpf,
            foreignDocument,
            phoneNumber,
            address,
            authProvider,
            externalId,
            DateTime.UtcNow);

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result<(User user, string token)>.Success((user, _tokenService.IssueToken(user.Id)));
    }

    /// <summary>
    /// Inicia o setup de 2FA, gerando secret e QR code.
    /// </summary>
    public async Task<Result<TwoFactorSetupResult>> Setup2FAAsync(
        Guid userId,
        string email,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return Result<TwoFactorSetupResult>.Failure("User not found.");
        }

        if (user.TwoFactorEnabled)
        {
            return Result<TwoFactorSetupResult>.Failure("2FA is already enabled.");
        }

        // Gerar secret
        var secretBytes = KeyGeneration.GenerateRandomKey(20);
        var secret = Base32Encoding.ToString(secretBytes);

        // Gerar QR code URI
        var issuer = "Araponga";
        var accountName = email ?? user.DisplayName;
        var qrCodeUri = $"otpauth://totp/{Uri.EscapeDataString(issuer)}:{Uri.EscapeDataString(accountName)}?secret={secret}&issuer={Uri.EscapeDataString(issuer)}";

        return Result<TwoFactorSetupResult>.Success(new TwoFactorSetupResult(secret, qrCodeUri, secret));
    }

    /// <summary>
    /// Confirma e habilita 2FA após validar código TOTP.
    /// </summary>
    public async Task<Result<TwoFactorConfirmResult>> Confirm2FAAsync(
        Guid userId,
        string secret,
        string code,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return Result<TwoFactorConfirmResult>.Failure("User not found.");
        }

        if (user.TwoFactorEnabled)
        {
            return Result<TwoFactorConfirmResult>.Failure("2FA is already enabled.");
        }

        // Validar código TOTP
        var secretBytes = Base32Encoding.ToBytes(secret);
        var totp = new Totp(secretBytes);
        if (!totp.VerifyTotp(code, out _, new VerificationWindow(1, 1)))
        {
            return Result<TwoFactorConfirmResult>.Failure("Invalid TOTP code.");
        }

        // Gerar recovery codes
        var recoveryCodes = GenerateRecoveryCodes(10);
        var recoveryCodesHash = HashRecoveryCodes(recoveryCodes);

        // Habilitar 2FA no usuário
        user.EnableTwoFactor(secret, recoveryCodesHash, DateTime.UtcNow);

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result<TwoFactorConfirmResult>.Success(new TwoFactorConfirmResult(recoveryCodes));
    }

    /// <summary>
    /// Verifica código 2FA e retorna JWT.
    /// </summary>
    public async Task<Result<string>> Verify2FAAsync(
        string challengeId,
        string code,
        CancellationToken cancellationToken)
    {
        CleanupExpiredChallenges();

        if (!_twoFactorChallenges.TryGetValue(challengeId, out var challenge))
        {
            return Result<string>.Failure("Invalid or expired challenge.");
        }

        if (DateTime.UtcNow > challenge.expiresAt)
        {
            _twoFactorChallenges.Remove(challengeId);
            return Result<string>.Failure("Challenge expired.");
        }

        var user = await _userRepository.GetByIdAsync(challenge.userId, cancellationToken);
        if (user is null || !user.TwoFactorEnabled || string.IsNullOrEmpty(user.TwoFactorSecret))
        {
            return Result<string>.Failure("2FA not enabled for user.");
        }

        // Validar código TOTP
        var secretBytes = Base32Encoding.ToBytes(user.TwoFactorSecret);
        var totp = new Totp(secretBytes);
        if (!totp.VerifyTotp(code, out _, new VerificationWindow(1, 1)))
        {
            return Result<string>.Failure("Invalid TOTP code.");
        }

        // Remover challenge usado
        _twoFactorChallenges.Remove(challengeId);

        // Retornar JWT
        return Result<string>.Success(_tokenService.IssueToken(user.Id));
    }

    /// <summary>
    /// Usa recovery code para autenticação.
    /// </summary>
    public async Task<Result<string>> Recover2FAAsync(
        string challengeId,
        string recoveryCode,
        CancellationToken cancellationToken)
    {
        CleanupExpiredChallenges();

        if (!_twoFactorChallenges.TryGetValue(challengeId, out var challenge))
        {
            return Result<string>.Failure("Invalid or expired challenge.");
        }

        if (DateTime.UtcNow > challenge.expiresAt)
        {
            _twoFactorChallenges.Remove(challengeId);
            return Result<string>.Failure("Challenge expired.");
        }

        var user = await _userRepository.GetByIdAsync(challenge.userId, cancellationToken);
        if (user is null || !user.TwoFactorEnabled || string.IsNullOrEmpty(user.TwoFactorRecoveryCodesHash))
        {
            return Result<string>.Failure("2FA not enabled for user.");
        }

        // Validar recovery code
        var recoveryCodeHash = HashRecoveryCode(recoveryCode);
        if (!VerifyRecoveryCode(recoveryCodeHash, user.TwoFactorRecoveryCodesHash))
        {
            return Result<string>.Failure("Invalid recovery code.");
        }

        // Remover challenge usado
        _twoFactorChallenges.Remove(challengeId);

        // TODO: Invalidar recovery code usado (requer armazenar códigos individuais hasheados)
        // Por enquanto, apenas retornar JWT

        return Result<string>.Success(_tokenService.IssueToken(user.Id));
    }

    /// <summary>
    /// Desabilita 2FA para o usuário.
    /// </summary>
    public async Task<OperationResult> Disable2FAAsync(
        Guid userId,
        string? passwordOrCode,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return OperationResult.Failure("User not found.");
        }

        if (!user.TwoFactorEnabled)
        {
            return OperationResult.Failure("2FA is not enabled.");
        }

        // TODO: Validar senha ou código 2FA se fornecido
        // Por enquanto, apenas desabilitar

        user.DisableTwoFactor();

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return OperationResult.Success();
    }

    private static List<string> GenerateRecoveryCodes(int count)
    {
        var codes = new List<string>();
        for (int i = 0; i < count; i++)
        {
            // Gerar bytes suficientes para garantir 12 caracteres após remover caracteres especiais
            // 9 bytes = 12 caracteres Base64, garantindo pelo menos 12 após limpeza
            var bytes = new byte[9];
            RandomNumberGenerator.Fill(bytes);
            var base64 = Convert.ToBase64String(bytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
            
            // Pegar os primeiros 12 caracteres (garantido ter pelo menos 12)
            var code = base64.Length >= 12 ? base64.Substring(0, 12) : base64;
            codes.Add(code);
        }
        return codes;
    }

    private static string HashRecoveryCodes(IReadOnlyList<string> codes)
    {
        // Hash todos os códigos juntos (simplificado - em produção, armazenar individualmente)
        var combined = string.Join("|", codes);
        return HashRecoveryCode(combined);
    }

    private static string HashRecoveryCode(string code)
    {
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(code));
        return Convert.ToBase64String(hashBytes);
    }

    private static bool VerifyRecoveryCode(string codeHash, string storedHash)
    {
        // Comparação segura de hashes
        return string.Equals(codeHash, storedHash, StringComparison.Ordinal);
    }

    private static void CleanupExpiredChallenges()
    {
        var now = DateTime.UtcNow;
        var expired = _twoFactorChallenges
            .Where(kvp => now > kvp.Value.expiresAt)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in expired)
        {
            _twoFactorChallenges.Remove(key);
        }
    }
}
