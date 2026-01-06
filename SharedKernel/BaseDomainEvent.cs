namespace SharedKernel;

/// <summary>
/// Base class for domain events providing common properties.
/// </summary>
public abstract class BaseDomainEvent : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public Guid EventId { get; } = Guid.NewGuid();
}

