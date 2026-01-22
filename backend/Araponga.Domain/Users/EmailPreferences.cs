namespace Araponga.Domain.Users;

/// <summary>
/// Preferências de email do usuário.
/// </summary>
public sealed record EmailPreferences
{
    public EmailPreferences(
        bool receiveEmails,
        EmailFrequency emailFrequency,
        EmailTypes emailTypes)
    {
        ReceiveEmails = receiveEmails;
        EmailFrequency = emailFrequency;
        EmailTypes = emailTypes;
    }

    /// <summary>
    /// Indica se o usuário deseja receber emails.
    /// </summary>
    public bool ReceiveEmails { get; init; }

    /// <summary>
    /// Frequência de envio de emails.
    /// </summary>
    public EmailFrequency EmailFrequency { get; init; }

    /// <summary>
    /// Tipos de email que o usuário deseja receber.
    /// </summary>
    public EmailTypes EmailTypes { get; init; }

    public static EmailPreferences Default() => new(
        receiveEmails: true,
        emailFrequency: EmailFrequency.Immediate,
        emailTypes: EmailTypes.All);
}

/// <summary>
/// Frequência de envio de emails.
/// </summary>
public enum EmailFrequency
{
    /// <summary>
    /// Envio imediato quando evento ocorre.
    /// </summary>
    Immediate = 0,

    /// <summary>
    /// Resumo diário (um email por dia com todas as notificações).
    /// </summary>
    Daily = 1,

    /// <summary>
    /// Resumo semanal (um email por semana com todas as notificações).
    /// </summary>
    Weekly = 2
}

/// <summary>
/// Tipos de email que o usuário deseja receber.
/// </summary>
[Flags]
public enum EmailTypes
{
    None = 0,
    Welcome = 1 << 0,
    PasswordReset = 1 << 1,
    Events = 1 << 2,
    Marketplace = 1 << 3,
    CriticalAlerts = 1 << 4,
    All = Welcome | PasswordReset | Events | Marketplace | CriticalAlerts
}
