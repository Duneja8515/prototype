using SharedKernel;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Infrastructure.EventBus;

/// <summary>
/// In-memory event bus implementation for prototype.
/// In production, this would be replaced with a message broker (RabbitMQ, Azure Service Bus, etc.)
/// </summary>
public class InMemoryEventBus : IEventBus
{
    private readonly ConcurrentDictionary<Type, List<Func<IDomainEvent, CancellationToken, Task>>> _handlers = new();
    private readonly ILogger<InMemoryEventBus>? _logger;

    public InMemoryEventBus(ILogger<InMemoryEventBus>? logger = null)
    {
        _logger = logger;
    }

    public void Subscribe<T>(Func<T, CancellationToken, Task> handler) where T : IDomainEvent
    {
        var eventType = typeof(T);
        var handlers = _handlers.GetOrAdd(eventType, _ => new List<Func<IDomainEvent, CancellationToken, Task>>());
        
        handlers.Add((domainEvent, ct) => handler((T)domainEvent, ct));
        
        _logger?.LogInformation("Subscribed handler for event type: {EventType}", eventType.Name);
    }

    public async Task PublishAsync<T>(T domainEvent, CancellationToken cancellationToken = default) where T : IDomainEvent
    {
        var eventType = typeof(T);
        
        if (!_handlers.TryGetValue(eventType, out var handlers) || handlers.Count == 0)
        {
            _logger?.LogWarning("No handlers registered for event type: {EventType}", eventType.Name);
            return;
        }

        _logger?.LogInformation("Publishing event: {EventType} with ID: {EventId}", eventType.Name, domainEvent.EventId);

        var tasks = handlers.Select(handler => handler(domainEvent, cancellationToken));
        await Task.WhenAll(tasks);
        
        _logger?.LogInformation("All handlers processed for event: {EventType}", eventType.Name);
    }
}

