using Billing.Application.Commands;
using Billing.Domain.Entities;
using Billing.Domain.Repositories;

namespace Billing.Application.Handlers;

/// <summary>
/// Handler for creating an invoice.
/// </summary>
public class CreateInvoiceCommandHandler
{
    private readonly IInvoiceRepository _invoiceRepository;

    public CreateInvoiceCommandHandler(IInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository ?? throw new ArgumentNullException(nameof(invoiceRepository));
    }

    public async Task<Guid> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoiceNumber = GenerateInvoiceNumber();
        var invoice = new Invoice(
            Guid.NewGuid(),
            invoiceNumber,
            request.ShipmentId,
            request.TrackingNumber,
            request.Amount
        );

        await _invoiceRepository.AddAsync(invoice, cancellationToken);

        return invoice.Id;
    }

    private static string GenerateInvoiceNumber()
    {
        return $"INV-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
    }
}

