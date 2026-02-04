using Araponga.Application.Interfaces;
using Araponga.Application.Interfaces.Connections;
using Araponga.Domain.Connections;

namespace Araponga.Application.Services.Connections;

/// <summary>
/// Implementação de <see cref="IAcceptedConnectionsProvider"/> que usa o repositório de conexões.
/// Registrado apenas quando o módulo de Conexões está ativo (Postgres).
/// </summary>
public sealed class AcceptedConnectionsProvider : IAcceptedConnectionsProvider
{
    private readonly IUserConnectionRepository _repository;

    public AcceptedConnectionsProvider(IUserConnectionRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<Guid>> GetAcceptedConnectionUserIdsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var connections = await _repository.GetAcceptedConnectionsAsync(userId, cancellationToken);
        return connections
            .Select(c => c.GetOtherUserId(userId))
            .ToList();
    }
}
