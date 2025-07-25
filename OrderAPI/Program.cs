using System.Reflection;
using Common.Examples;
using Common.Extensions;
using MassTransit;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using OrderAPI.Data;
using OrderAPI.Messaging;
using OrderCommon.Data;
using OrderCommon.Repositories;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddScoped<IOrderContextSeed, OrderContextSeed>();

builder.Services.AddScoped<IOrderContext, OrderContext>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

builder.Services.AddScoped<IOrderPublisher, OrderPublisher>();

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq(Extensions.ConfigureRabbitMq);
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Order API",
        Version = "v1",
        Description = "An API to perform Order operations"
    });

    c.ExampleFilters();

    // Set the comments path for the Swagger JSON and UI.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

builder.Services.AddSwaggerExamplesFromAssemblyOf<OrderExample>();

builder.Services.AddHealthChecks()
                .AddMongoDb(
                    sp => new MongoDB.Driver.MongoClient(builder.Configuration.GetConnectionString()),
                    sp => builder.Configuration.GetConnectionString(),
                    "MongoDb Health",
                    HealthStatus.Degraded);
BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order API"));

app.UseAuthorization();

app.MapControllers();

app.Run();