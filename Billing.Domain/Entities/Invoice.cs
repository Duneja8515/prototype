using SharedKernel;

namespace Billing.Domain.Entities;

/// <summary>
/// Invoice aggregate root representing an invoice in the Billing domain.
/// </summary>
public class Invoice
{
    public Guid Id { get; private set; }
    public string InvoiceNumber { get; private set; } = string.Empty;
    public Guid ShipmentId { get; private set; }
    public string TrackingNumber { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }
    public InvoiceStatus Status { get; private set; }
    public DateTime IssuedAt { get; private set; }

    private Invoice() { } // For EF Core

    public Invoice(Guid id, string invoiceNumber, Guid shipmentId, string trackingNumber, decimal amount)
    {
        Id = id;
        InvoiceNumber = invoiceNumber ?? throw new ArgumentNullException(nameof(invoiceNumber));
        ShipmentId = shipmentId;
        TrackingNumber = trackingNumber ?? throw new ArgumentNullException(nameof(trackingNumber));
        Amount = amount;
        Status = InvoiceStatus.Issued;
        IssuedAt = DateTime.UtcNow;
    }

    public void MarkAsPaid()
    {
        if (Status == InvoiceStatus.Paid)
        {
            throw new InvalidOperationException("Invoice is already paid.");
        }

        Status = InvoiceStatus.Paid;
    }
}

public enum InvoiceStatus
{
    Issued,
    Paid,
    Cancelled
}

