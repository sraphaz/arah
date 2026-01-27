namespace Araponga.Api.Contracts.Admin;

public sealed class PlanHistoryResponse
{
    public Guid Id { get; set; }
    public Guid PlanId { get; set; }
    public Guid ChangedByUserId { get; set; }
    public string ChangeType { get; set; } = string.Empty;
    public Dictionary<string, object>? PreviousState { get; set; }
    public Dictionary<string, object>? NewState { get; set; }
    public string? ChangeReason { get; set; }
    public DateTime ChangedAtUtc { get; set; }
}
