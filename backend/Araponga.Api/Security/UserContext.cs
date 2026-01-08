using Araponga.Domain.Users;

namespace Araponga.Api.Security;

public sealed record UserContext(User? User, TokenStatus Status);
