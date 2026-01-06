using Billing.Domain.Entities;
using Billing.Domain.Repositories;

namespace Infrastructure.Repositories;

/// <summary>
/// In-memory implementation of invoice repository for prototype.
/// </summary>
public class InMemoryInvoiceRepository : IInvoiceRepository
{
    private readonly Dictionary<Guid, Invoice> _invoices = new();
    private readonly Dictionary<string, Invoice> _invoicesByNumber = new();

    public Task<Invoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _invoices.TryGetValue(id, out var invoice);
        return Task.FromResult<Invoice?>(invoice);
    }

    public Task<Invoice?> GetByInvoiceNumberAsync(string invoiceNumber, CancellationToken cancellationToken = default)
    {
        _invoicesByNumber.TryGetValue(invoiceNumber, out var invoice);
        return Task.FromResult<Invoice?>(invoice);
    }

    public Task AddAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        _invoices[invoice.Id] = invoice;
        _invoicesByNumber[invoice.InvoiceNumber] = invoice;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        _invoices[invoice.Id] = invoice;
        _invoicesByNumber[invoice.InvoiceNumber] = invoice;
        return Task.CompletedTask;
    }
}

