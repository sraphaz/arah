namespace Araponga.Api.Contracts.Moderation;

public sealed record ReportRequest(string Reason, string? Details);
