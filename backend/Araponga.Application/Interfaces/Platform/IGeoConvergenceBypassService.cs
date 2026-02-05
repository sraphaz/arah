namespace Araponga.Application.Interfaces;

/// <summary>
/// Define se a exigência de convergência geolocalização deve ser ignorada para um dado (território, usuário).
/// Quando true, o acesso ao território (feed, criar post, etc.) não exige que o dispositivo esteja no perímetro.
/// </summary>
public interface IGeoConvergenceBypassService
{
    /// <summary>
    /// Retorna true se a validação geo deve ser ignorada: território com RemoteAccessToTerritoryEnabled
    /// ou usuário com SystemAdmin ou RemoteAccessToTerritory (sys admin tem bypass por padrão).
    /// </summary>
    Task<bool> ShouldBypassGeoEnforcementAsync(
        Guid territoryId,
        Guid? userId,
        CancellationToken cancellationToken = default);
}
