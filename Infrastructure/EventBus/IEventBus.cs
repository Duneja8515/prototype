using SharedKernel;

namespace Infrastructure.EventBus;

/// <summary>
/// Event bus interface for publishing domain events.
/// </summary>
public interface IEventBus
{
    Task PublishAsync<T>(T domainEvent, CancellationToken cancellationToken = default) where T : IDomainEvent;
    void Subscribe<T>(Func<T, CancellationToken, Task> handler) where T : IDomainEvent;
}

