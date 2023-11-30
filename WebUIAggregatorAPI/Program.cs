using Common.Extensions;
using WebUIAggregatorAPI.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ConfigureOpenTelemetry();

// Add services to the container.
builder.Services.AddHttpClient<ICatalogApiService, CatalogApiService>(options =>
{
    options.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]);
});
builder.Services.AddHttpClient<IWarehouseApiService, WarehouseApiService>(options =>
{
    options.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]);
});

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

app.UseAuthorization();

app.MapControllers();

app.Run();
