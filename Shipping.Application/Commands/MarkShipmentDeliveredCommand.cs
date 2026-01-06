namespace Shipping.Application.Commands;

/// <summary>
/// Command to mark a shipment as delivered.
/// </summary>
public record MarkShipmentDeliveredCommand(Guid ShipmentId);

