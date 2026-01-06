using Shipping.Domain.Entities;
using Shipping.Domain.Repositories;

namespace Infrastructure.Repositories;

/// <summary>
/// In-memory implementation of shipment repository for prototype.
/// </summary>
public class InMemoryShipmentRepository : IShipmentRepository
{
    private readonly Dictionary<Guid, Shipment> _shipments = new();
    private readonly Dictionary<string, Shipment> _shipmentsByTracking = new();

    public Task<Shipment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _shipments.TryGetValue(id, out var shipment);
        return Task.FromResult<Shipment?>(shipment);
    }

    public Task<Shipment?> GetByTrackingNumberAsync(string trackingNumber, CancellationToken cancellationToken = default)
    {
        _shipmentsByTracking.TryGetValue(trackingNumber, out var shipment);
        return Task.FromResult<Shipment?>(shipment);
    }

    public Task AddAsync(Shipment shipment, CancellationToken cancellationToken = default)
    {
        _shipments[shipment.Id] = shipment;
        _shipmentsByTracking[shipment.TrackingNumber] = shipment;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Shipment shipment, CancellationToken cancellationToken = default)
    {
        _shipments[shipment.Id] = shipment;
        _shipmentsByTracking[shipment.TrackingNumber] = shipment;
        return Task.CompletedTask;
    }
}

