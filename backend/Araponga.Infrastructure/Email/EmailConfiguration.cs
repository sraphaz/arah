namespace Araponga.Infrastructure.Email;

/// <summary>
/// Configuração para envio de emails via SMTP.
/// </summary>
public sealed class EmailConfiguration
{
    /// <summary>
    /// Host do servidor SMTP.
    /// </summary>
    public required string Host { get; init; }

    /// <summary>
    /// Porta do servidor SMTP.
    /// </summary>
    public int Port { get; init; } = 587;

    /// <summary>
    /// Nome de usuário para autenticação SMTP.
    /// </summary>
    public string? Username { get; init; }

    /// <summary>
    /// Senha para autenticação SMTP.
    /// </summary>
    public string? Password { get; init; }

    /// <summary>
    /// Indica se SSL/TLS deve ser habilitado.
    /// </summary>
    public bool EnableSsl { get; init; } = true;

    /// <summary>
    /// Endereço de email do remetente padrão.
    /// </summary>
    public required string FromAddress { get; init; }

    /// <summary>
    /// Nome do remetente padrão.
    /// </summary>
    public required string FromName { get; init; }

    /// <summary>
    /// Valida se a configuração está completa e válida.
    /// </summary>
    /// <returns>True se válida, false caso contrário.</returns>
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Host) &&
               Port > 0 &&
               Port <= 65535 &&
               !string.IsNullOrWhiteSpace(FromAddress) &&
               !string.IsNullOrWhiteSpace(FromName);
    }
}
