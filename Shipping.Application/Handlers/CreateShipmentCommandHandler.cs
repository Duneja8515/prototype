using Shipping.Application.Commands;
using Shipping.Domain.Entities;
using Shipping.Domain.Repositories;

namespace Shipping.Application.Handlers;

public class CreateShipmentCommandHandler
{
    private readonly IShipmentRepository _shipmentRepository;

    public CreateShipmentCommandHandler(IShipmentRepository shipmentRepository)
    {
        _shipmentRepository = shipmentRepository ?? throw new ArgumentNullException(nameof(shipmentRepository));
    }

    public async Task<Guid> Handle(CreateShipmentCommand command, CancellationToken cancellationToken)
    {
        var shipment = new Shipment(
            command.ShipmentId,
            command.TrackingNumber,
            command.RecipientAddress
        );

        await _shipmentRepository.AddAsync(shipment, cancellationToken);

        return shipment.Id;
    }
}

