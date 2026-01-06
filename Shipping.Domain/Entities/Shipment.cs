using SharedKernel;
using Shipping.Domain.Events;

namespace Shipping.Domain.Entities;

/// <summary>
/// Shipment aggregate root representing a shipment in the Shipping domain.
/// </summary>
public class Shipment
{
    public Guid Id { get; private set; }
    public string TrackingNumber { get; private set; } = string.Empty;
    public string RecipientAddress { get; private set; } = string.Empty;
    public ShipmentStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? DeliveredAt { get; private set; }

    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private Shipment() { } // For EF Core

    public Shipment(Guid id, string trackingNumber, string recipientAddress)
    {
        Id = id;
        TrackingNumber = trackingNumber ?? throw new ArgumentNullException(nameof(trackingNumber));
        RecipientAddress = recipientAddress ?? throw new ArgumentNullException(nameof(recipientAddress));
        Status = ShipmentStatus.Created;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the shipment as delivered and raises a domain event.
    /// </summary>
    public void MarkDelivered()
    {
        if (Status == ShipmentStatus.Delivered)
        {
            throw new InvalidOperationException("Shipment is already delivered.");
        }

        Status = ShipmentStatus.Delivered;
        DeliveredAt = DateTime.UtcNow;

        // Raise domain event - this is the ONLY way Shipping communicates with other domains
        _domainEvents.Add(new ShipmentDeliveredEvent(Id, TrackingNumber, DeliveredAt.Value));
    }

    /// <summary>
    /// Clears domain events after they have been published.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}

public enum ShipmentStatus
{
    Created,
    InTransit,
    Delivered
}

