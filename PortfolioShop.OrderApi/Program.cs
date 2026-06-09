using MassTransit;
using Swashbuckle.AspNetCore;
using PortfolioShop.Contracts.Messages;

var builder = WebApplication.CreateBuilder(args);

// Grab configuration values
var rabbitMqHost = builder.Configuration["RabbitMQ:Host"] ?? "localhost";
var username = builder.Configuration["RabbitMQ:Username"] ?? "guest";
var password = builder.Configuration["RabbitMQ:Password"] ?? "guest";

// 1. Register MassTransit
builder.Services.AddMassTransit(x =>
{
    // Configure RabbitMQ Transport
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbitMqHost, "/", h =>
        {
            h.Username(username);
            h.Password(password);
        });
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 2. Minimal API Endpoint to Checkout/Place Order
app.MapPost("/api/orders", async (OrderRequest request, IPublishEndpoint publishEndpoint) =>
{
    var orderEvent = new OrderPlacedEvent
    {
        OrderId = Guid.NewGuid(),
        ProductId = request.ProductId,
        Quantity = request.Quantity,
        Price = request.Price,
        Timestamp = DateTime.UtcNow
    };

    // Publish the message to the exchange asynchronously
    await publishEndpoint.Publish(orderEvent);

    return Results.Accepted($"/api/orders/{orderEvent.OrderId}", new { Message = "Order accepted, processing asynchronously.", OrderId = orderEvent.OrderId });
});

app.Run();

// DTO wrapper for HTTP request
public record OrderRequest(string ProductId, int Quantity, decimal Price);