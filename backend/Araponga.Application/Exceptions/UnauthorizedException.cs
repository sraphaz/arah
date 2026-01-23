namespace Araponga.Application.Exceptions;

public sealed class UnauthorizedException : DomainException
{
    public UnauthorizedException(string message)
        : base(message)
    {
    }
}
