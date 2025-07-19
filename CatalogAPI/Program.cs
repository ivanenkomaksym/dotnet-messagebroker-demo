using Catalog.API.Repositories;
using Catalog.API.Repositories.Interfaces;
using CatalogAPI.Data;
using CatalogAPI.Extensions;
using CatalogAPI.Services;
using Common.Extensions;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddApplicationServices();

// Register MCP server and discover tools from the current assembly
builder.Services.AddMcpServer().WithHttpTransport().WithToolsFromAssembly();

// Add services to the container.
builder.Services.AddScoped<ICatalogAI, CatalogAI>();
builder.Services.AddScoped<ICatalogContext, CatalogContext>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHealthChecks()
                .AddMongoDb(
                    sp => new MongoDB.Driver.MongoClient(builder.Configuration.GetConnectionString()),
                    sp => builder.Configuration.GetConnectionString(),
                    "MongoDb Health",
                    HealthStatus.Degraded);
BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{    
    var catalogContext = scope.ServiceProvider.GetRequiredService<ICatalogContext>();
    var catalogAI = scope.ServiceProvider.GetRequiredService<ICatalogAI>();
    await CatalogContextSeed.SeedDataAsync(catalogAI, catalogContext.Products, true);
}

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/hc", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

// Add MCP middleware
app.MapMcp();

app.Run();