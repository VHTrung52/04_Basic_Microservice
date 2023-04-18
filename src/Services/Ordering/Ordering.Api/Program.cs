using Ordering.Api.Extensions;
using Ordering.Application;
using Ordering.Infrastructure;
using Ordering.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Application.Middleware;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add services to the container.
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureService(config);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// register middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthorization();

app.MapControllers();
// migrate database and seed data
app.MigrateDatabase<OrderContext>((context, services) =>
    {
        var logger = services.GetService<ILogger<OrderContextSeed>>();
        OrderContextSeed.SeedOrderData(context, logger).Wait();
    });

app.Run();
