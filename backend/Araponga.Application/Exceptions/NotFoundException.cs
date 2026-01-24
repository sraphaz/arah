namespace Araponga.Application.Exceptions;

public sealed class NotFoundException : DomainException
{
    public NotFoundException(string resourceName, object resourceId)
        : base($"{resourceName} with ID {resourceId} was not found.")
    {
    }

    public NotFoundException(string message)
        : base(message)
    {
    }
}
