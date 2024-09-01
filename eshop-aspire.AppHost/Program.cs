var builder = DistributedApplication.CreateBuilder(args);

var mongo = builder.AddMongoDB("mongo").PublishAsConnectionString();
var mongodb = mongo.AddDatabase("mongodb");

var customers = builder.AddProject<Projects.CustomerAPI>("customerapi")
    .WithReference(mongodb)
    // Use Aspire's mongodb connection string in CustomerAPI's appsettings
    .WithEnvironment($"DatabaseSettings:ConnectionString", mongodb);

var catalog = builder.AddProject<Projects.CatalogAPI>("catalogapi")
    // Use Aspire's mongodb connection string in CatalogAPI's appsettings
    .WithEnvironment($"DatabaseSettings:ConnectionString", mongodb);

var shoppingCart = builder.AddProject<Projects.ShoppingCartAPI>("shoppingcartapi")
    // Use Aspire's mongodb connection string in ShoppingCartAPI's appsettings
    .WithEnvironment($"DatabaseSettings:ConnectionString", mongodb);

builder.AddProject<Projects.WebUI>("webui")
    .WithExternalHttpEndpoints()
    .WithReference(customers)
    .WithReference(catalog)
    .WithReference(shoppingCart)
    // Configure WebUI to use real CustomerAPI
    .WithEnvironment($"FeatureManagement:Customer", "true")
    // Configure WebUI to use real CatalogAPI
    .WithEnvironment($"FeatureManagement:Product", "true")
    // Configure WebUI to use real ShoppingCartAPI
    .WithEnvironment($"FeatureManagement:ShoppingCart", "true")
    .WithEnvironment($"ApplicationOptions:StartupEnvironment", "Aspire");

builder.Build().Run();
