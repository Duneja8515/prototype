using SharedKernel;

namespace Shipping.Domain.Events;

/// <summary>
/// Domain event raised when a shipment is marked as delivered.
/// This is the single event that triggers invoice creation in the Billing domain.
/// </summary>
public class ShipmentDeliveredEvent : BaseDomainEvent
{
    public Guid ShipmentId { get; }
    public string TrackingNumber { get; }
    public DateTime DeliveredAt { get; }

    public ShipmentDeliveredEvent(Guid shipmentId, string trackingNumber, DateTime deliveredAt)
    {
        ShipmentId = shipmentId;
        TrackingNumber = trackingNumber ?? throw new ArgumentNullException(nameof(trackingNumber));
        DeliveredAt = deliveredAt;
    }
}

