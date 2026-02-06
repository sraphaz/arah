using Araponga.Application.Interfaces;

namespace Araponga.Application.Services;

/// <summary>
/// Centraliza a decisão de ignorar a exigência de convergência geo.
/// Política atual: permitir visualização e conexão ao território independentemente da localização.
/// Visitantes e moradores podem ver o território mesmo fora do perímetro; o perfil exibirá
/// status "no território" ou "fora do território". Quais ações exigirão estar no território
/// será definido depois; por ora o bypass está ativo para todas as operações (ler feed, criar post, etc.).
/// </summary>
public sealed class GeoConvergenceBypassService : IGeoConvergenceBypassService
{
    /// <inheritdoc />
    public Task<bool> ShouldBypassGeoEnforcementAsync(
        Guid territoryId,
        Guid? userId,
        CancellationToken cancellationToken = default)
    {
        // Allow connection and viewing from anywhere; in-territory status will be shown on profile.
        // Per-action rules (which require being in territory) to be defined later.
        return Task.FromResult(true);
    }
}
