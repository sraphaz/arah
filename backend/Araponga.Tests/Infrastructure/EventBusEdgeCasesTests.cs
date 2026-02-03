using Araponga.Application.Events;
using Araponga.Infrastructure.Eventing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Araponga.Tests.Infrastructure;

/// <summary>
/// Edge case tests for InMemoryEventBus,
/// focusing on null events, missing handlers, and error handling.
/// </summary>
public class EventBusEdgeCasesTests
{
    [Fact]
    public async Task InMemoryEventBus_PublishAsync_WithNullEvent_HandlesCorrectly()
    {
        var serviceCollection = new ServiceCollection();
        // Registrar um handler para garantir que o null será detectado
        var handler = new Mock<IEventHandler<TestEvent>>();
        serviceCollection.AddSingleton<IEventHandler<TestEvent>>(handler.Object);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventBus = new InMemoryEventBus(serviceProvider);

        // InMemoryEventBus não valida null, então quando há handlers,
        // pode lançar NullReferenceException ao tentar chamar HandleAsync com null
        // Se não houver handlers, completa sem erro
        try
        {
            await eventBus.PublishAsync<TestEvent>(null!, CancellationToken.None);
            // Se chegou aqui sem exceção, o handler foi chamado com null (comportamento atual)
            // Verificar que o handler foi chamado (mesmo com null)
            handler.Verify(h => h.HandleAsync(It.IsAny<TestEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }
        catch (Exception ex)
        {
            // Se lançou exceção, deve ser relacionada a null
            Assert.True(
                ex is ArgumentNullException || 
                ex is NullReferenceException ||
                ex.InnerException is ArgumentNullException ||
                ex.InnerException is NullReferenceException);
        }
    }

    [Fact]
    public async Task InMemoryEventBus_PublishAsync_WithNoHandlers_CompletesSuccessfully()
    {
        var serviceCollection = new ServiceCollection();
        // Não registrar handlers
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventBus = new InMemoryEventBus(serviceProvider);

        var testEvent = new TestEvent { Message = "Test" };

        // Deve completar sem erros mesmo sem handlers
        await eventBus.PublishAsync(testEvent, CancellationToken.None);
    }

    [Fact]
    public async Task InMemoryEventBus_PublishAsync_WithMultipleHandlers_InvokesAll()
    {
        var serviceCollection = new ServiceCollection();
        var handler1 = new Mock<IEventHandler<TestEvent>>();
        var handler2 = new Mock<IEventHandler<TestEvent>>();

        // Registrar como IEventHandler<TestEvent> explicitamente
        serviceCollection.AddSingleton<IEventHandler<TestEvent>>(handler1.Object);
        serviceCollection.AddSingleton<IEventHandler<TestEvent>>(handler2.Object);

        var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventBus = new InMemoryEventBus(serviceProvider);

        var testEvent = new TestEvent { Message = "Test" };

        await eventBus.PublishAsync(testEvent, CancellationToken.None);

        handler1.Verify(h => h.HandleAsync(testEvent, It.IsAny<CancellationToken>()), Times.Once);
        handler2.Verify(h => h.HandleAsync(testEvent, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task InMemoryEventBus_PublishAsync_WithHandlerThrowingException_PropagatesException()
    {
        var serviceCollection = new ServiceCollection();
        var handler = new Mock<IEventHandler<TestEvent>>();
        handler.Setup(h => h.HandleAsync(It.IsAny<TestEvent>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Handler error"));

        serviceCollection.AddSingleton<IEventHandler<TestEvent>>(handler.Object);

        var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventBus = new InMemoryEventBus(serviceProvider);

        var testEvent = new TestEvent { Message = "Test" };

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await eventBus.PublishAsync(testEvent, CancellationToken.None);
        });
    }

    [Fact]
    public async Task InMemoryEventBus_PublishAsync_WithCancelledToken_RespectsCancellation()
    {
        var serviceCollection = new ServiceCollection();
        var handler = new Mock<IEventHandler<TestEvent>>();
        handler.Setup(h => h.HandleAsync(It.IsAny<TestEvent>(), It.IsAny<CancellationToken>()))
            .Returns(async (TestEvent e, CancellationToken ct) =>
            {
                await Task.Delay(1000, ct); // Simular trabalho longo; lança se ct já estiver cancelado
            });

        serviceCollection.AddSingleton<IEventHandler<TestEvent>>(handler.Object);

        var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventBus = new InMemoryEventBus(serviceProvider);

        var testEvent = new TestEvent { Message = "Test" };
        var cts = new CancellationTokenSource();
        cts.Cancel(); // Token já cancelado antes de PublishAsync para teste determinístico

        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
        {
            await eventBus.PublishAsync(testEvent, cts.Token);
        });
    }

    // Classe de teste para eventos
    public class TestEvent : IAppEvent
    {
        public string Message { get; set; } = "";
        public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;
    }
}
