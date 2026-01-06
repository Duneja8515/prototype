namespace Shipping.Application.Commands;

public record CreateShipmentCommand(
    Guid ShipmentId,
    string TrackingNumber,
    string RecipientAddress
);

