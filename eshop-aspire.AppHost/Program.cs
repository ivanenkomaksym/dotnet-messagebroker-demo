var builder = DistributedApplication.CreateBuilder(args);

var mongo = builder.AddMongoDB("mongo").PublishAsConnectionString();
var mongodb = mongo.AddDatabase("mongodb");

var customers = builder.AddProject<Projects.CustomerAPI>("customerapi")
    .WithReference(mongodb)
    // Use Aspire's mongodb connection string in CustomerAPI's appsettings
    .WithEnvironment($"DatabaseSettings:ConnectionString", mongodb);

builder.AddProject<Projects.WebUI>("webui")
    .WithExternalHttpEndpoints()
    .WithReference(customers)
    // Configure WebUI to use real CustomerAPI
    .WithEnvironment($"FeatureManagement:Customer", "true");

builder.Build().Run();
