using Araponga.Application.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Araponga.Infrastructure.Eventing;

/// <summary>
/// Processador de eventos em background com retry logic e dead letter queue.
/// </summary>
public sealed class BackgroundEventProcessor : BackgroundService, IEventBus
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BackgroundEventProcessor> _logger;
    private readonly ConcurrentQueue<EventMessage> _eventQueue = new();
    private readonly ConcurrentDictionary<Guid, EventMessage> _deadLetterQueue = new();
    private readonly SemaphoreSlim _processingSemaphore;
    private const int MaxRetries = 3;
    private const int MaxConcurrentProcessing = 5;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
        MaxDepth = 64,
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    };

    public BackgroundEventProcessor(
        IServiceProvider serviceProvider,
        ILogger<BackgroundEventProcessor> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _processingSemaphore = new SemaphoreSlim(MaxConcurrentProcessing, MaxConcurrentProcessing);
    }

    /// <summary>
    /// Publica um evento para processamento assíncrono.
    /// </summary>
    public Task PublishAsync<TEvent>(TEvent appEvent, CancellationToken cancellationToken)
        where TEvent : IAppEvent
    {
        var message = new EventMessage
        {
            Id = Guid.NewGuid(),
            EventType = typeof(TEvent).FullName ?? typeof(TEvent).Name,
            EventData = JsonSerializer.Serialize(appEvent, JsonOptions),
            CreatedAtUtc = DateTime.UtcNow,
            Attempts = 0
        };

        _eventQueue.Enqueue(message);
        _logger.LogDebug("Event {EventId} of type {EventType} queued for processing", message.Id, message.EventType);

        return Task.CompletedTask;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("BackgroundEventProcessor started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (_eventQueue.TryDequeue(out var message))
                {
                    // Processar em background sem bloquear a fila
                    _ = Task.Run(() => ProcessEventAsync(message, stoppingToken), stoppingToken);
                }
                else
                {
                    // Aguardar um pouco antes de verificar novamente
                    await Task.Delay(TimeSpan.FromMilliseconds(100), stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in BackgroundEventProcessor main loop");
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }

        _logger.LogInformation("BackgroundEventProcessor stopped");
    }

    private async Task ProcessEventAsync(EventMessage message, CancellationToken cancellationToken)
    {
        await _processingSemaphore.WaitAsync(cancellationToken);
        try
        {
            message.Attempts++;
            message.LastAttemptAtUtc = DateTime.UtcNow;

            _logger.LogDebug(
                "Processing event {EventId} of type {EventType} (attempt {Attempt})",
                message.Id,
                message.EventType,
                message.Attempts);

            var success = await TryProcessEventAsync(message, cancellationToken);

            if (!success)
            {
                if (message.Attempts >= MaxRetries)
                {
                    _deadLetterQueue.TryAdd(message.Id, message);
                    _logger.LogWarning(
                        "Event {EventId} of type {EventType} moved to dead letter queue after {Attempts} attempts",
                        message.Id,
                        message.EventType,
                        message.Attempts);
                }
                else
                {
                    // Re-enfileirar para retry com backoff exponencial
                    var delay = TimeSpan.FromMilliseconds(100 * Math.Pow(2, message.Attempts - 1));
                    _logger.LogDebug(
                        "Event {EventId} will be retried after {Delay}ms",
                        message.Id,
                        delay.TotalMilliseconds);

                    await Task.Delay(delay, cancellationToken);
                    _eventQueue.Enqueue(message);
                }
            }
        }
        finally
        {
            _processingSemaphore.Release();
        }
    }

    private async Task<bool> TryProcessEventAsync(EventMessage message, CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var eventType = Type.GetType(message.EventType);
            if (eventType is null)
            {
                _logger.LogWarning("Event type {EventType} not found", message.EventType);
                return false;
            }

            var appEvent = JsonSerializer.Deserialize(message.EventData, eventType, JsonOptions);
            if (appEvent is not IAppEvent typedEvent)
            {
                _logger.LogWarning("Failed to deserialize event {EventId}", message.Id);
                return false;
            }

            // Obter handlers dinamicamente
            var handlerType = typeof(IEventHandler<>).MakeGenericType(eventType);
            var handlers = scope.ServiceProvider.GetServices(handlerType);

            foreach (var handler in handlers)
            {
                var handleMethod = handlerType.GetMethod("HandleAsync");
                if (handleMethod is not null)
                {
                    var parameters = new object?[] { typedEvent, cancellationToken };
                    var task = (Task?)handleMethod.Invoke(handler, parameters);
                    if (task is not null)
                    {
                        await task;
                    }
                }
            }

            _logger.LogDebug("Event {EventId} processed successfully", message.Id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error processing event {EventId} of type {EventType}",
                message.Id,
                message.EventType);
            return false;
        }
    }

    /// <summary>
    /// Obtém eventos na dead letter queue.
    /// </summary>
    public IReadOnlyList<EventMessage> GetDeadLetterQueue()
    {
        return _deadLetterQueue.Values.ToList();
    }

    /// <summary>
    /// Remove um evento da dead letter queue.
    /// </summary>
    public bool RemoveFromDeadLetterQueue(Guid eventId)
    {
        return _deadLetterQueue.TryRemove(eventId, out _);
    }

    public override void Dispose()
    {
        _processingSemaphore?.Dispose();
        base.Dispose();
    }
}

/// <summary>
/// Mensagem de evento na fila.
/// </summary>
public sealed class EventMessage
{
    public Guid Id { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string EventData { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? LastAttemptAtUtc { get; set; }
    public int Attempts { get; set; }
}
