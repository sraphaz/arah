namespace Araponga.Application.Interfaces;

/// <summary>
/// Armazena e valida refresh tokens para renovação de access tokens (padrão OAuth: expiração + rotação).
/// </summary>
public interface IRefreshTokenStore
{
    /// <summary>
    /// Emite um refresh token para o usuário. Expira conforme config (ex.: 7 dias).
    /// </summary>
    (string Token, DateTime ExpiresAt) Issue(Guid userId);

    /// <summary>
    /// Valida o refresh token e retorna o userId. Se oneTimeUse, invalida o token após uso.
    /// </summary>
    Task<Guid?> ConsumeAsync(string token, CancellationToken cancellationToken = default);
}
