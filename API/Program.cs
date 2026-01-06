using Shipping.Domain.Repositories;
using Billing.Domain.Repositories;
using Infrastructure.Repositories;
using Infrastructure.EventBus;
using Infrastructure.EventHandlers;
using Shipping.Application.Handlers;
using Billing.Application.Handlers;
using Shipping.Domain.Events;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register repositories
builder.Services.AddSingleton<IShipmentRepository, InMemoryShipmentRepository>();
builder.Services.AddSingleton<IInvoiceRepository, InMemoryInvoiceRepository>();

// Register event bus
builder.Services.AddSingleton<IEventBus, InMemoryEventBus>();

// Register application handlers
builder.Services.AddScoped<MarkShipmentDeliveredCommandHandler>();
builder.Services.AddScoped<CreateShipmentCommandHandler>();
builder.Services.AddScoped<CreateInvoiceCommandHandler>();

// Register event handlers
builder.Services.AddScoped<ShipmentDeliveredEventHandler>();

var app = builder.Build();

// Subscribe event handlers (this is the ONLY integration point between Shipping and Billing)
// Use a service provider to resolve scoped services when events are handled
var eventBus = app.Services.GetRequiredService<IEventBus>();
var serviceProvider = app.Services;

eventBus.Subscribe<ShipmentDeliveredEvent>(async (domainEvent, ct) =>
{
    // Create a scope to resolve scoped services
    using var scope = serviceProvider.CreateScope();
    var eventHandler = scope.ServiceProvider.GetRequiredService<ShipmentDeliveredEventHandler>();
    await eventHandler.Handle(domainEvent, ct);
});

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
