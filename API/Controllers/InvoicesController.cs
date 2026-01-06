using Microsoft.AspNetCore.Mvc;
using Billing.Domain.Repositories;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ILogger<InvoicesController> _logger;

    public InvoicesController(
        IInvoiceRepository invoiceRepository,
        ILogger<InvoicesController> logger)
    {
        _invoiceRepository = invoiceRepository ?? throw new ArgumentNullException(nameof(invoiceRepository));
        _logger = logger;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(id, cancellationToken);
        
        if (invoice == null)
        {
            return NotFound(new { error = "Invoice not found" });
        }

        return Ok(new
        {
            id = invoice.Id,
            invoiceNumber = invoice.InvoiceNumber,
            shipmentId = invoice.ShipmentId,
            trackingNumber = invoice.TrackingNumber,
            amount = invoice.Amount,
            status = invoice.Status.ToString(),
            issuedAt = invoice.IssuedAt
        });
    }
}

