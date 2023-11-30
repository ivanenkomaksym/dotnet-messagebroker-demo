using Common.Extensions;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using WarehouseAPI.Data;
using WarehouseCommon.Data;
using WarehouseCommon.Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ConfigureOpenTelemetry();

// Add services to the container.
builder.Services.AddHttpClient<IWarehouseContextSeed, WarehouseContextSeed>(options =>
{
    options.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]);
});

builder.Services.AddSingleton<IWarehouseContext, WarehouseContext>();
builder.Services.AddSingleton<IWarehouseRepository, WarehouseRepository>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHealthChecks()
                .AddMongoDb(builder.Configuration["DatabaseSettings:ConnectionString"], "MongoDb Health", HealthStatus.Degraded);

BsonDefaults.GuidRepresentationMode = GuidRepresentationMode.V3;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/hc", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();
