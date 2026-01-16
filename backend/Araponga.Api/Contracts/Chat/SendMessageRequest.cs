namespace Araponga.Api.Contracts.Chat;

public sealed record SendMessageRequest(string Text, Guid? MediaId = null);

