using Basket.API.GrpcServices;
using Basket.API.Repositories;
using Discount.Grpc.Protos;

var builder = WebApplication.CreateBuilder(args);

var Configuration = builder.Configuration;
// Add services to the container.
builder.Services.AddStackExchangeRedisCache(options =>
{
    var x = Configuration.GetValue<string>("CacheSettings:ConnectionString");
    options.Configuration = x;
});

builder.Services.AddScoped<IBasketRepository, BasketRepository>();
var discountGrpcUrl = Configuration["GrpcSettings:DiscountUrl"];
builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(options => options.Address = new Uri(Configuration["GrpcSettings:DiscountUrl"]));
builder.Services.AddScoped<DiscountGrpcService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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
