using System.Reflection;
using Common.Examples;
using CustomerAPI;
using CustomerAPI.Data;
using CustomerAPI.Repositories;
using MassTransit;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
builder.Configuration.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false);
builder.Configuration.AddJsonFile("appsettings.k8s.json", optional: true, reloadOnChange: false);

// Add services to the container.
builder.Services.AddScoped<ICustomerContext, CustomerContext>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

builder.Services.AddScoped<ICustomerService, CustomerService>();

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq();
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Customer API",
        Version = "v1",
        Description = "An API to perform Customer operations"
    });

    c.ExampleFilters();

    // Set the comments path for the Swagger JSON and UI.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

builder.Services.AddSwaggerExamplesFromAssemblyOf<CustomerExample>();

builder.Services.AddHealthChecks()
                .AddMongoDb(builder.Configuration["DatabaseSettings:ConnectionString"], "MongoDb Health", HealthStatus.Degraded);

BsonDefaults.GuidRepresentationMode = GuidRepresentationMode.V3;

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();