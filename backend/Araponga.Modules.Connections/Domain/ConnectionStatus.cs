namespace Araponga.Domain.Connections;

/// <summary>
/// Status da conexão entre dois usuários.
/// </summary>
public enum ConnectionStatus
{
    /// <summary>Solicitação enviada, aguardando resposta.</summary>
    Pending = 0,

    /// <summary>Conexão aceita, relação mútua estabelecida.</summary>
    Accepted = 1,

    /// <summary>Solicitação rejeitada.</summary>
    Rejected = 2,

    /// <summary>Conexão removida por uma das partes.</summary>
    Removed = 3
}
