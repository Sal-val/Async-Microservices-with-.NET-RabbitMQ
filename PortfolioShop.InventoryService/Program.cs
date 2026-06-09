using MassTransit;
using MassTransit.Futures.Contracts;
using PortfolioShop.InventoryService.Consumers;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMassTransit(x =>
{
    // Automatically discover and register all consumer classes in this project's assembly
    x.AddConsumers(typeof(Program).Assembly);

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        // Configures default endpoints for your consumers (creates the queues automatically)
        cfg.ConfigureEndpoints(context);
    });
});

var host = builder.Build();
host.Run();