using Common.Extensions;
using Common.Routing;
using WebUIAggregatorAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var gatewayAddress = builder.Configuration.GetGatewayAddress();

// Add services to the container.
builder.Services.AddSingleton<IEnvironmentRouter, EnvironmentRouter>();

builder.Services.AddHttpClient<ICatalogApiService, CatalogApiService>(options =>
{
    options.BaseAddress = new Uri(gatewayAddress);
});
builder.Services.AddHttpClient<IWarehouseApiService, WarehouseApiService>(options =>
{
    options.BaseAddress = new Uri(gatewayAddress);
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();