namespace Billing.Application.Commands;

/// <summary>
/// Command to create an invoice for a delivered shipment.
/// </summary>
public record CreateInvoiceCommand(
    Guid ShipmentId,
    string TrackingNumber,
    decimal Amount
);

