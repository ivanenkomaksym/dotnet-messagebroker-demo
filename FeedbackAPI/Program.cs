using Common.Extensions;
using FeedbackAPI.Data;
using FeedbackAPI.Repositories;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddScoped<IFeedbackContext, FeedbackContext>();
builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();
builder.Services.AddGraphQLServer().AddQueryType<Query>().AddMutationType<Mutation>().AddType<ReviewType>();

// Add services to the container.

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

// Configure the HTTP request pipeline.

app.MapControllers();
app.MapHealthChecks("/hc", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseRouting();

app.UseAuthorization();

app.MapGraphQL("/api/graphql");

app.Run();