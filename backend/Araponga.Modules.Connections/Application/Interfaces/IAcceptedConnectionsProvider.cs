namespace Araponga.Application.Interfaces.Connections;

/// <summary>
/// Fornece os IDs dos usuários que são conexões aceitas de um dado usuário.
/// Usado pelo Feed para priorizar posts de amigos quando o módulo de Conexões está ativo.
/// Quando o módulo não está registrado, esta interface não é injetada (opcional).
/// </summary>
public interface IAcceptedConnectionsProvider
{
    /// <summary>
    /// Retorna os IDs dos usuários que têm conexão aceita com <paramref name="userId"/>.
    /// </summary>
    Task<IReadOnlyList<Guid>> GetAcceptedConnectionUserIdsAsync(Guid userId, CancellationToken cancellationToken);
}
