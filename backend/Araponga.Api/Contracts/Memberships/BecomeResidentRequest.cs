namespace Araponga.Api.Contracts.Memberships;

public sealed record BecomeResidentRequest(
    IReadOnlyList<Guid>? RecipientUserIds,
    string? Message);
