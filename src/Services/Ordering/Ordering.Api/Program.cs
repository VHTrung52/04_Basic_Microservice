using EventBus.Messages.Common;
using MassTransit;
using Ordering.Api.EventBusConsumer;
using Ordering.Api.Extensions;
using Ordering.Application;
using Ordering.Application.Middleware;
using Ordering.Infrastructure;
using Ordering.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// MassTransit-RabbitMq Configuration
builder.Services.AddMassTransit(config =>
{
    config.AddConsumer<BasketCheckoutConsumer>();
    config.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);
        cfg.ReceiveEndpoint(EventBusConstants.BasketCheckoutQueue, c =>
        {
            c.ConfigureConsumer<BasketCheckoutConsumer>(ctx);
        });
    });
});
// Depricated - https://code-maze.com/masstransit-rabbitmq-aspnetcore/
//builder.Services.AddMassTransitHostedService();

// Add services to the container.
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureService(config);

builder.Services.AddControllers();

// Swagger/OpenAPI Configuration https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<BasketCheckoutConsumer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Register middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthorization();

app.MapControllers();
// Migrate database and seed data
app.MigrateDatabase<OrderContext>((context, services) =>
    {
        var logger = services.GetService<ILogger<OrderContextSeed>>();
        OrderContextSeed.SeedOrderData(context, logger).Wait();
    });

app.Run();
