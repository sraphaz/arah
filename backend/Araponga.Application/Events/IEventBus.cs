namespace Araponga.Application.Events;

public interface IEventBus
{
    Task PublishAsync<TEvent>(TEvent appEvent, CancellationToken cancellationToken)
        where TEvent : IAppEvent;
}
