using Shipping.Application.Commands;
using Shipping.Domain.Entities;
using Shipping.Domain.Repositories;
using Infrastructure.EventBus;

namespace Shipping.Application.Handlers;

/// <summary>
/// Handler for marking a shipment as delivered.
/// </summary>
public class MarkShipmentDeliveredCommandHandler
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly IEventBus _eventBus;

    public MarkShipmentDeliveredCommandHandler(
        IShipmentRepository shipmentRepository,
        IEventBus eventBus)
    {
        _shipmentRepository = shipmentRepository ?? throw new ArgumentNullException(nameof(shipmentRepository));
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
    }

    public async Task Handle(MarkShipmentDeliveredCommand request, CancellationToken cancellationToken)
    {
        var shipment = await _shipmentRepository.GetByIdAsync(request.ShipmentId, cancellationToken);
        
        if (shipment == null)
        {
            throw new InvalidOperationException($"Shipment with ID {request.ShipmentId} not found.");
        }

        shipment.MarkDelivered();
        await _shipmentRepository.UpdateAsync(shipment, cancellationToken);

        // Publish domain events after saving
        foreach (var domainEvent in shipment.DomainEvents)
        {
            await _eventBus.PublishAsync(domainEvent, cancellationToken);
        }

        shipment.ClearDomainEvents();
    }
}

