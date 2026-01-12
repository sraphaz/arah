namespace Araponga.Application.Events;

public sealed class NoOpEventBus : IEventBus
{
    public Task PublishAsync<TEvent>(TEvent appEvent, CancellationToken cancellationToken)
        where TEvent : IAppEvent
    {
        return Task.CompletedTask;
    }
}
