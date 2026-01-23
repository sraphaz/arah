namespace Araponga.Application.Exceptions;

public sealed class ForbiddenException : DomainException
{
    public ForbiddenException(string message)
        : base(message)
    {
    }
}
