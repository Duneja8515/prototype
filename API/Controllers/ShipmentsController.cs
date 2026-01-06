using Microsoft.AspNetCore.Mvc;
using Shipping.Application.Commands;
using Shipping.Application.Handlers;
using API.DTOs;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShipmentsController : ControllerBase
{
    private readonly MarkShipmentDeliveredCommandHandler _markDeliveredHandler;
    private readonly CreateShipmentCommandHandler _createHandler;
    private readonly ILogger<ShipmentsController> _logger;

    public ShipmentsController(
        MarkShipmentDeliveredCommandHandler markDeliveredHandler,
        CreateShipmentCommandHandler createHandler,
        ILogger<ShipmentsController> logger)
    {
        _markDeliveredHandler = markDeliveredHandler ?? throw new ArgumentNullException(nameof(markDeliveredHandler));
        _createHandler = createHandler ?? throw new ArgumentNullException(nameof(createHandler));
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateShipmentRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var shipmentId = Guid.NewGuid();
            var command = new CreateShipmentCommand(
                shipmentId,
                request.TrackingNumber,
                request.RecipientAddress
            );

            await _createHandler.Handle(command, cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = shipmentId },
                new { id = shipmentId, trackingNumber = request.TrackingNumber });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating shipment");
            return StatusCode(500, new { error = "An error occurred while creating the shipment" });
        }
    }

    [HttpGet("{id}")]
    public IActionResult GetById(Guid id)
    {
        // This would require a query handler - simplified for prototype
        return Ok(new { message = "Use repository directly in production" });
    }

    [HttpPost("{shipmentId}/mark-delivered")]
    public async Task<IActionResult> MarkDelivered(Guid shipmentId, CancellationToken cancellationToken)
    {
        try
        {
            var command = new MarkShipmentDeliveredCommand(shipmentId);
            await _markDeliveredHandler.Handle(command, cancellationToken);
            
            return Ok(new { message = "Shipment marked as delivered successfully" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to mark shipment as delivered");
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking shipment as delivered");
            return StatusCode(500, new { error = "An error occurred while processing the request" });
        }
    }
}

