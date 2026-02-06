namespace Araponga.Application.Interfaces;

/// <summary>
/// Define se a exigência de convergência geolocalização deve ser ignorada para um dado (território, usuário).
/// Quando true, o acesso ao território (feed, criar post, etc.) não exige que o dispositivo esteja no perímetro.
/// Política atual: bypass ativo para todos (visualização e conexão permitidas de qualquer local; status "no território" no perfil; regras por ação a definir).
/// </summary>
public interface IGeoConvergenceBypassService
{
    /// <summary>
    /// Retorna true se a validação geo deve ser ignorada. Atualmente sempre true (acesso permitido independente da localização).
    /// </summary>
    Task<bool> ShouldBypassGeoEnforcementAsync(
        Guid territoryId,
        Guid? userId,
        CancellationToken cancellationToken = default);
}
