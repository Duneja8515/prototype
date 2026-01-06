using Shipping.Domain.Events;
using Billing.Application.Commands;
using Billing.Application.Handlers;
using Microsoft.Extensions.Logging;

namespace Infrastructure.EventHandlers;

/// <summary>
/// Event handler that connects Shipping domain to Billing domain.
/// This is the ONLY integration point between the two domains - via the ShipmentDeliveredEvent.
/// </summary>
public class ShipmentDeliveredEventHandler
{
    private readonly CreateInvoiceCommandHandler _createInvoiceHandler;
    private readonly ILogger<ShipmentDeliveredEventHandler>? _logger;

    public ShipmentDeliveredEventHandler(
        CreateInvoiceCommandHandler createInvoiceHandler,
        ILogger<ShipmentDeliveredEventHandler>? logger = null)
    {
        _createInvoiceHandler = createInvoiceHandler ?? throw new ArgumentNullException(nameof(createInvoiceHandler));
        _logger = logger;
    }

    public async Task Handle(ShipmentDeliveredEvent domainEvent, CancellationToken cancellationToken)
    {
        _logger?.LogInformation(
            "ShipmentDeliveredEvent received for ShipmentId: {ShipmentId}, TrackingNumber: {TrackingNumber}",
            domainEvent.ShipmentId,
            domainEvent.TrackingNumber);

        // Calculate invoice amount (in a real system, this would come from shipment details)
        // For prototype, using a simple fixed amount
        var invoiceAmount = 100.00m;

        var command = new CreateInvoiceCommand(
            domainEvent.ShipmentId,
            domainEvent.TrackingNumber,
            invoiceAmount
        );

        try
        {
            var invoiceId = await _createInvoiceHandler.Handle(command, cancellationToken);
            _logger?.LogInformation(
                "Invoice created successfully. InvoiceId: {InvoiceId} for ShipmentId: {ShipmentId}",
                invoiceId,
                domainEvent.ShipmentId);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex,
                "Failed to create invoice for ShipmentId: {ShipmentId}",
                domainEvent.ShipmentId);
            throw;
        }
    }
}

