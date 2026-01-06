namespace API.DTOs;

public record CreateShipmentRequest(
    string TrackingNumber,
    string RecipientAddress
);

