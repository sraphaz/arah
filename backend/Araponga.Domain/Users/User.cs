using Araponga.Domain.Users;

namespace Araponga.Domain.Users;

/// <summary>
/// Representa um usuário do sistema. Contém informações de identidade, autenticação e verificação.
/// </summary>
public sealed class User
{
    /// <summary>
    /// Cria uma nova instância de User com valores padrão para 2FA e verificação de identidade.
    /// </summary>
    public User(
        Guid id,
        string displayName,
        string? email,
        string? cpf,
        string? foreignDocument,
        string? phoneNumber,
        string? address,
        string authProvider,
        string externalId,
        DateTime createdAtUtc)
        : this(id, displayName, email, cpf, foreignDocument, phoneNumber, address, authProvider, externalId, false, null, null, null, UserIdentityVerificationStatus.Unverified, null, null, null, null, createdAtUtc)
    {
    }

    /// <summary>
    /// Cria uma nova instância de User com todos os parâmetros.
    /// </summary>
    /// <param name="id">Identificador único do usuário.</param>
    /// <param name="displayName">Nome de exibição do usuário.</param>
    /// <param name="email">Endereço de e-mail (opcional).</param>
    /// <param name="cpf">CPF brasileiro (opcional, mutuamente exclusivo com foreignDocument).</param>
    /// <param name="foreignDocument">Documento de identificação estrangeiro (opcional, mutuamente exclusivo com cpf).</param>
    /// <param name="phoneNumber">Número de telefone (opcional).</param>
    /// <param name="address">Endereço físico (opcional).</param>
    /// <param name="authProvider">Provedor de autenticação social (ex: "google", "apple", "facebook").</param>
    /// <param name="externalId">ID único do usuário no provedor de autenticação.</param>
    /// <param name="twoFactorEnabled">Indica se autenticação de dois fatores está habilitada.</param>
    /// <param name="twoFactorSecret">Secret para geração de códigos 2FA (opcional).</param>
    /// <param name="twoFactorRecoveryCodesHash">Hash dos códigos de recuperação 2FA (opcional).</param>
    /// <param name="twoFactorVerifiedAtUtc">Data/hora UTC em que o 2FA foi verificado (opcional).</param>
    /// <param name="identityVerificationStatus">Status da verificação de identidade global do usuário.</param>
    /// <param name="identityVerifiedAtUtc">Data/hora UTC em que a identidade foi verificada (opcional).</param>
    /// <param name="avatarMediaAssetId">ID do MediaAsset usado como avatar (opcional).</param>
    /// <param name="bio">Biografia/descrição pessoal do usuário (opcional, máx. 500 caracteres).</param>
    /// <param name="passwordHash">Hash da senha (opcional; usado para login por e-mail/senha).</param>
    /// <param name="createdAtUtc">Data/hora UTC de criação do usuário.</param>
    public User(
        Guid id,
        string displayName,
        string? email,
        string? cpf,
        string? foreignDocument,
        string? phoneNumber,
        string? address,
        string authProvider,
        string externalId,
        bool twoFactorEnabled,
        string? twoFactorSecret,
        string? twoFactorRecoveryCodesHash,
        DateTime? twoFactorVerifiedAtUtc,
        UserIdentityVerificationStatus identityVerificationStatus,
        DateTime? identityVerifiedAtUtc,
        Guid? avatarMediaAssetId,
        string? bio,
        string? passwordHash,
        DateTime createdAtUtc)
    {
        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new ArgumentException("Display name is required.", nameof(displayName));
        }

        if (string.IsNullOrWhiteSpace(authProvider))
        {
            throw new ArgumentException("Auth provider is required.", nameof(authProvider));
        }

        if (string.IsNullOrWhiteSpace(externalId))
        {
            throw new ArgumentException("External ID is required.", nameof(externalId));
        }

        var normalizedCpf = NormalizeOptional(cpf);
        var normalizedForeignDocument = NormalizeOptional(foreignDocument);

        if (string.IsNullOrWhiteSpace(normalizedCpf) && string.IsNullOrWhiteSpace(normalizedForeignDocument))
        {
            throw new ArgumentException("CPF or foreign document is required.", nameof(cpf));
        }

        if (!string.IsNullOrWhiteSpace(normalizedCpf) && !string.IsNullOrWhiteSpace(normalizedForeignDocument))
        {
            throw new ArgumentException("Provide either CPF or foreign document, not both.", nameof(cpf));
        }

        Id = id;
        DisplayName = displayName.Trim();
        Email = NormalizeOptional(email);
        Cpf = normalizedCpf;
        ForeignDocument = normalizedForeignDocument;
        PhoneNumber = NormalizeOptional(phoneNumber);
        Address = NormalizeOptional(address);
        AuthProvider = authProvider.Trim();
        ExternalId = externalId.Trim();
        TwoFactorEnabled = twoFactorEnabled;
        TwoFactorSecret = twoFactorSecret;
        TwoFactorRecoveryCodesHash = twoFactorRecoveryCodesHash;
        TwoFactorEnabled = twoFactorEnabled;
        TwoFactorSecret = twoFactorSecret;
        TwoFactorRecoveryCodesHash = twoFactorRecoveryCodesHash;
        TwoFactorVerifiedAtUtc = twoFactorVerifiedAtUtc;
        IdentityVerificationStatus = identityVerificationStatus;
        IdentityVerifiedAtUtc = identityVerifiedAtUtc;
        AvatarMediaAssetId = avatarMediaAssetId;
        Bio = NormalizeBio(bio);
        PasswordHash = passwordHash;
        CreatedAtUtc = createdAtUtc;
    }

    /// <summary>
    /// Identificador único do usuário.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Nome de exibição do usuário.
    /// </summary>
    public string DisplayName { get; }

    /// <summary>
    /// Endereço de e-mail do usuário (opcional).
    /// </summary>
    public string? Email { get; }

    /// <summary>
    /// CPF brasileiro do usuário (opcional, mutuamente exclusivo com ForeignDocument).
    /// </summary>
    public string? Cpf { get; }

    /// <summary>
    /// Documento de identificação estrangeiro (opcional, mutuamente exclusivo com Cpf).
    /// </summary>
    public string? ForeignDocument { get; }

    /// <summary>
    /// Número de telefone do usuário (opcional).
    /// </summary>
    public string? PhoneNumber { get; }

    /// <summary>
    /// Endereço físico do usuário (opcional).
    /// </summary>
    public string? Address { get; }

    /// <summary>
    /// Provedor de autenticação social usado pelo usuário (ex: "google", "apple", "facebook").
    /// Combinado com ExternalId, forma uma chave única para identificar o usuário.
    /// </summary>
    public string AuthProvider { get; }

    /// <summary>
    /// ID único do usuário no provedor de autenticação (ex: OIDC "sub", Facebook ID).
    /// Combinado com AuthProvider, forma uma chave única para identificar o usuário.
    /// </summary>
    public string ExternalId { get; }

    /// <summary>
    /// Indica se autenticação de dois fatores está habilitada para este usuário.
    /// </summary>
    public bool TwoFactorEnabled { get; private set; }

    /// <summary>
    /// Secret usado para geração de códigos 2FA (opcional, presente apenas se TwoFactorEnabled for true).
    /// </summary>
    public string? TwoFactorSecret { get; private set; }

    /// <summary>
    /// Hash dos códigos de recuperação 2FA (opcional, presente apenas se TwoFactorEnabled for true).
    /// </summary>
    public string? TwoFactorRecoveryCodesHash { get; private set; }

    /// <summary>
    /// Data/hora UTC em que o 2FA foi verificado pela primeira vez (opcional).
    /// </summary>
    public DateTime? TwoFactorVerifiedAtUtc { get; private set; }

    /// <summary>
    /// Status da verificação de identidade global do usuário.
    /// Esta verificação é independente das verificações territoriais (ResidencyVerification).
    /// </summary>
    public UserIdentityVerificationStatus IdentityVerificationStatus { get; private set; }

    /// <summary>
    /// Data/hora UTC em que a identidade do usuário foi verificada (opcional).
    /// </summary>
    public DateTime? IdentityVerifiedAtUtc { get; private set; }

    /// <summary>
    /// ID do MediaAsset usado como avatar do usuário (opcional).
    /// </summary>
    public Guid? AvatarMediaAssetId { get; private set; }

    /// <summary>
    /// Biografia/descrição pessoal do usuário (opcional, máx. 500 caracteres).
    /// </summary>
    public string? Bio { get; private set; }

    /// <summary>
    /// Hash da senha (opcional; presente para contas criadas com e-mail/senha).
    /// </summary>
    public string? PasswordHash { get; }

    /// <summary>
    /// Data/hora UTC de criação do usuário no sistema.
    /// </summary>
    public DateTime CreatedAtUtc { get; }

    /// <summary>
    /// Habilita autenticação de dois fatores para o usuário.
    /// </summary>
    /// <param name="secret">Secret para geração de códigos TOTP.</param>
    /// <param name="recoveryCodesHash">Hash dos códigos de recuperação.</param>
    /// <param name="verifiedAtUtc">Data/hora UTC em que o 2FA foi verificado.</param>
    public void EnableTwoFactor(string secret, string recoveryCodesHash, DateTime verifiedAtUtc)
    {
        TwoFactorEnabled = true;
        TwoFactorSecret = secret;
        TwoFactorRecoveryCodesHash = recoveryCodesHash;
        TwoFactorVerifiedAtUtc = verifiedAtUtc;
    }

    /// <summary>
    /// Desabilita autenticação de dois fatores para o usuário, removendo todos os dados relacionados.
    /// </summary>
    public void DisableTwoFactor()
    {
        TwoFactorEnabled = false;
        TwoFactorSecret = null;
        TwoFactorRecoveryCodesHash = null;
        TwoFactorVerifiedAtUtc = null;
    }

    /// <summary>
    /// Atualiza o status da verificação de identidade global do usuário.
    /// </summary>
    /// <param name="status">Novo status de verificação.</param>
    /// <param name="verifiedAtUtc">Data/hora UTC em que a verificação ocorreu (opcional).</param>
    public void UpdateIdentityVerification(UserIdentityVerificationStatus status, DateTime? verifiedAtUtc = null)
    {
        IdentityVerificationStatus = status;
        IdentityVerifiedAtUtc = verifiedAtUtc;
    }

    /// <summary>
    /// Atualiza o avatar do usuário.
    /// </summary>
    /// <param name="avatarMediaAssetId">ID do MediaAsset a ser usado como avatar (null para remover).</param>
    public void UpdateAvatar(Guid? avatarMediaAssetId)
    {
        AvatarMediaAssetId = avatarMediaAssetId;
    }

    /// <summary>
    /// Atualiza a biografia do usuário.
    /// </summary>
    /// <param name="bio">Nova biografia (null ou vazio para remover, máx. 500 caracteres).</param>
    /// <exception cref="ArgumentException">Se a biografia exceder 500 caracteres.</exception>
    public void UpdateBio(string? bio)
    {
        var normalized = NormalizeBio(bio);
        if (normalized is not null && normalized.Length > 500)
        {
            throw new ArgumentException("Bio must not exceed 500 characters.", nameof(bio));
        }
        Bio = normalized;
    }

    private static string? NormalizeOptional(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string? NormalizeBio(string? bio)
    {
        if (string.IsNullOrWhiteSpace(bio))
        {
            return null;
        }
        var trimmed = bio.Trim();
        return trimmed.Length == 0 ? null : trimmed;
    }
}
