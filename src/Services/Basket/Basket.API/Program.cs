using Basket.API.GrpcServices;
using Basket.API.Repositories;
using Discount.Grpc.Protos;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Redis Configuration
builder.Services.AddStackExchangeRedisCache(options =>
{
    var x = builder.Configuration.GetValue<string>("CacheSettings:ConnectionString");
    options.Configuration = x;
});

// General Configuration
builder.Services.AddScoped<IBasketRepository, BasketRepository>();

// Discount.Grpc Configuration 
builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(options => options.Address = new Uri(builder.Configuration["GrpcSettings:DiscountUrl"]));
builder.Services.AddScoped<DiscountGrpcService>();

// AutoMapper Configuration
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// MassTransit-RabbitMq Configuration
builder.Services.AddMassTransit(config =>
{
    config.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);
    });
});
// Depricated - https://code-maze.com/masstransit-rabbitmq-aspnetcore/
//builder.Services.AddMassTransitHostedService();

builder.Services.AddControllers();
// Swagger/OpenAPI Configuration https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Basket.API v1"));
}

app.UseAuthorization();

app.MapControllers();

app.Run();
