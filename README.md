# Shipping & Billing Prototype

A .NET 8 prototype demonstrating Domain-Driven Design (DDD) with event-driven integration between Shipping and Billing domains.

## Architecture Overview

This solution follows Clean Architecture and DDD principles with clear domain boundaries:

```
┌─────────────────────────────────────────────────────────┐
│                         API Layer                        │
│              (Controllers, DTOs, HTTP Endpoints)          │
└─────────────────────────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────┐
│                    Application Layer                     │
│  ┌──────────────────┐      ┌──────────────────┐         │
│  │ Shipping.App     │      │ Billing.App      │         │
│  │ (Commands/       │      │ (Commands/       │         │
│  │  Handlers)       │      │  Handlers)       │         │
│  └──────────────────┘      └──────────────────┘         │
└─────────────────────────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────┐
│                      Domain Layer                         │
│  ┌──────────────────┐      ┌──────────────────┐         │
│  │ Shipping.Domain  │      │ Billing.Domain   │         │
│  │ • Shipment       │      │ • Invoice        │         │
│  │ • Events         │      │                  │         │
│  └──────────────────┘      └──────────────────┘         │
└─────────────────────────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────┐
│                   Infrastructure Layer                    │
│  • Event Bus (In-Memory)                                 │
│  • Repositories (In-Memory)                               │
│  • Event Handlers (Cross-Domain Integration)              │
└─────────────────────────────────────────────────────────┘
```

## Key Design Principles

### 1. Domain Boundaries
- **Shipping Domain**: Manages `Shipment` aggregates. Can mark shipments as delivered.
- **Billing Domain**: Manages `Invoice` aggregates. Issues invoices for delivered shipments.

### 2. Event-Driven Integration
- **Single Event**: `ShipmentDeliveredEvent` is the ONLY communication channel between domains
- **No Direct Coupling**: Domains cannot directly access each other's databases or APIs
- **Asynchronous**: Events are published and handled asynchronously via the event bus

### 3. Clean Architecture
- **Domain Layer**: Pure business logic, no dependencies on infrastructure
- **Application Layer**: Use cases and orchestration
- **Infrastructure Layer**: Technical implementations (repositories, event bus)
- **API Layer**: HTTP endpoints and DTOs

## Project Structure

```
ShippingBillingPrototype/
├── SharedKernel/              # Shared domain event infrastructure
├── Shipping.Domain/           # Shipping domain entities and events
├── Shipping.Application/       # Shipping use cases
├── Billing.Domain/            # Billing domain entities
├── Billing.Application/       # Billing use cases
├── Infrastructure/            # Event bus, repositories, event handlers
└── API/                       # Web API controllers and DTOs
```

## How It Works

### Flow: Shipment Delivery → Invoice Creation

1. **Shipment Created**: A shipment is created via the API
2. **Mark as Delivered**: The shipment is marked as delivered via `MarkDelivered()`
3. **Domain Event Raised**: `ShipmentDeliveredEvent` is raised by the `Shipment` aggregate
4. **Event Published**: The event is published to the event bus
5. **Event Handler**: `ShipmentDeliveredEventHandler` receives the event
6. **Invoice Created**: The handler creates an invoice in the Billing domain

### Event Flow Diagram

```
┌─────────────┐
│  Shipment   │
│  Aggregate  │
└──────┬──────┘
       │ MarkDelivered()
       │
       ▼
┌─────────────────────┐
│ ShipmentDelivered   │
│      Event          │
└──────┬──────────────┘
       │
       ▼
┌─────────────┐
│  Event Bus  │
└──────┬──────┘
       │
       ▼
┌──────────────────────────┐
│ ShipmentDeliveredEvent   │
│      Handler             │
└──────┬───────────────────┘
       │
       ▼
┌─────────────┐
│   Invoice   │
│  Aggregate  │
└─────────────┘
```

## API Endpoints

### Shipping

- `POST /api/shipments` - Create a new shipment
  ```json
  {
    "trackingNumber": "TRACK-001",
    "recipientAddress": "123 Main St, City, Country"
  }
  ```

- `POST /api/shipments/{shipmentId}/mark-delivered` - Mark shipment as delivered

### Billing

- `GET /api/invoices/{id}` - Get invoice by ID

## Running the Application

1. **Restore packages**:
   ```bash
   dotnet restore
   ```

2. **Build the solution**:
   ```bash
   dotnet build
   ```

3. **Run the API**:
   ```bash
   cd API
   dotnet run
   ```

4. **Access Swagger UI**:
   Navigate to `https://localhost:5001/swagger` (or the port shown in console)

## Example Usage

### 1. Create a Shipment
```bash
POST /api/shipments
{
  "trackingNumber": "TRACK-001",
  "recipientAddress": "123 Main St, City, Country"
}
```

### 2. Mark Shipment as Delivered
```bash
POST /api/shipments/{shipmentId}/mark-delivered
```

This will automatically trigger invoice creation in the Billing domain via the event.

### 3. Retrieve Invoice
```bash
GET /api/invoices/{invoiceId}
```

## Domain Events

### ShipmentDeliveredEvent
- **Source**: Shipping Domain
- **Trigger**: When `Shipment.MarkDelivered()` is called
- **Handler**: `ShipmentDeliveredEventHandler` in Infrastructure
- **Action**: Creates an invoice in the Billing domain

## Important Notes

1. **No Direct Integration**: Shipping and Billing domains are completely decoupled. They only communicate through events.

2. **Single Event**: Only one event (`ShipmentDeliveredEvent`) is used for integration, as per requirements.

3. **In-Memory Implementation**: This is a prototype using in-memory repositories and event bus. In production:
   - Use a real database (EF Core, Dapper, etc.)
   - Use a message broker (RabbitMQ, Azure Service Bus, etc.)
   - Implement proper error handling and retry policies

4. **Event Handler Location**: The event handler is in the Infrastructure layer, not in either domain, maintaining domain independence.

## Future Enhancements

- Add persistence layer (EF Core)
- Implement proper message broker (RabbitMQ/Azure Service Bus)
- Add event sourcing
- Implement CQRS with separate read models
- Add unit and integration tests
- Add validation and error handling middleware
- Implement authentication and authorization

## Technology Stack

- .NET 8
- ASP.NET Core Web API
- Domain-Driven Design (DDD)
- Event-Driven Architecture
- Clean Architecture

# prototype
