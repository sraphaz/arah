namespace Araponga.Api.Contracts.Users;

/// <summary>
/// Resposta pública do perfil de usuário, respeitando privacidade.
/// Contém apenas informações permitidas baseadas nas preferências de privacidade.
/// </summary>
public sealed record UserProfilePublicResponse(
    Guid Id,
    string DisplayName,
    DateTime CreatedAtUtc,
    IReadOnlyList<string> Interests,
    string? AvatarUrl = null,
    string? Bio = null);
