namespace Araponga.Api.Configuration;

/// <summary>
/// Configuração para autenticação de workers/sistemas (client credentials).
/// </summary>
public sealed class ClientCredentialsOptions
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
}
