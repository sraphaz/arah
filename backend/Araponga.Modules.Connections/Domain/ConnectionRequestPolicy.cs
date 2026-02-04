namespace Araponga.Domain.Connections;

/// <summary>
/// Política de quem pode enviar solicitação de conexão ao usuário.
/// </summary>
public enum ConnectionRequestPolicy
{
    /// <summary>Qualquer pessoa pode me adicionar.</summary>
    Anyone = 0,

    /// <summary>Apenas moradores podem me adicionar.</summary>
    ResidentsOnly = 1,

    /// <summary>Apenas pessoas que eu já adicionei podem me adicionar.</summary>
    ConnectionsOnly = 2,

    /// <summary>Ninguém pode me adicionar.</summary>
    Disabled = 3
}
