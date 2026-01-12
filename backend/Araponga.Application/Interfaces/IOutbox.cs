namespace Araponga.Application.Interfaces;

public interface IOutbox
{
    Task EnqueueAsync(Models.OutboxMessage message, CancellationToken cancellationToken);
}
