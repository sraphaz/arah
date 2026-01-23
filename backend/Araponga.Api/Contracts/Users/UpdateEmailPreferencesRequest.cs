namespace Araponga.Api.Contracts.Users;

/// <summary>
/// Request para atualizar preferências de email do usuário.
/// </summary>
public sealed record UpdateEmailPreferencesRequest(
    bool? ReceiveEmails,
    string? EmailFrequency, // "Immediate", "Daily", "Weekly"
    int? EmailTypes); // Bit flags: Welcome=1, PasswordReset=2, Events=4, Marketplace=8, CriticalAlerts=16, All=31
