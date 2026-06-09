using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using PortfolioShop.Contracts.Messages;

namespace PortfolioShop.InventoryService.Consumers
{
    public class OrderPlacedConsumer : IConsumer<OrderPlacedEvent>
    {
        private readonly ILogger<OrderPlacedConsumer> _logger;

        public OrderPlacedConsumer(ILogger<OrderPlacedConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OrderPlacedEvent> context)
        {
            var message = context.Message;

            _logger.LogInformation(">>> [RECEIVED] Order Placed Event processing...");
            _logger.LogInformation("Order ID: {OrderId}", message.OrderId);
            _logger.LogInformation("Product: {ProductId} | Quantity: {Quantity}", message.ProductId, message.Quantity);

            // Simulate database stock reduction/validation
            await Task.Delay(500);

            _logger.LogSuccess(">>> [SUCCESS] Stock successfully allocated for Order {OrderId}", message.OrderId);
        }
    }
}

// Simple extension method to print output logs in green
public static class LoggerExtensions
{
    public static void LogSuccess(this ILogger logger, string message, params object[] args)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        logger.LogInformation(message, args);
        Console.ResetColor();
    }
}