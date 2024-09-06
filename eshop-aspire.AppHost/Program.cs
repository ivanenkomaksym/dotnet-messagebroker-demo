var builder = DistributedApplication.CreateBuilder(args);

var mongo = builder.AddMongoDB("mongo").PublishAsConnectionString();
var mongodb = mongo.AddDatabase("mongodb");

var messaging = builder.AddRabbitMQ("AMQPConnectionString");

var customers = builder.AddProject<Projects.CustomerAPI>("customerapi")
    .WithReference(mongodb)
    .WithReference(messaging)
    // Use Aspire's mongodb connection string in CustomerAPI's appsettings
    .WithEnvironment($"DatabaseSettings:ConnectionString", mongodb)
    .WithEnvironment($"ApplicationOptions:StartupEnvironment", "Aspire");

var catalog = builder.AddProject<Projects.CatalogAPI>("catalogapi")
    .WithReference(mongodb)
    // Use Aspire's mongodb connection string in CatalogAPI's appsettings
    .WithEnvironment($"DatabaseSettings:ConnectionString", mongodb);

var shoppingCart = builder.AddProject<Projects.ShoppingCartAPI>("shoppingcartapi")
    .WithReference(mongodb)
    // Use Aspire's mongodb connection string in ShoppingCartAPI's appsettings
    .WithEnvironment($"DatabaseSettings:ConnectionString", mongodb);

var warehouseapi = builder.AddProject<Projects.WarehouseAPI>("warehouseapi")
    .WithReference(mongodb)
    // Use Aspire's mongodb connection string in WarehouseAPI's appsettings
    .WithEnvironment($"DatabaseSettings:ConnectionString", mongodb);

// Configure WebUIAggregator to startup in Aspire configuration, since it uses two services: CatalogAPI and WarehouseAPI to aggregate response
var webuiaggregator = builder.AddProject<Projects.WebUIAggregatorAPI>("webuiaggregatorapi")
    .WithReference(catalog)
    .WithReference(warehouseapi)
    .WithEnvironment($"ApplicationOptions:StartupEnvironment", "Aspire");

var order = builder.AddProject<Projects.OrderAPI>("orderapi")
    .WithReference(mongodb)
    .WithReference(messaging)
    // Use Aspire's mongodb connection string in OrderAPI's appsettings
    .WithEnvironment($"DatabaseSettings:ConnectionString", mongodb)
    .WithEnvironment($"ApplicationOptions:StartupEnvironment", "Aspire");

var ordergrpc = builder.AddProject<Projects.OrderGrpc>("ordergrpc")
    .WithReference(mongodb)
    // Use Aspire's mongodb connection string in OrderAPI's appsettings
    .WithEnvironment($"DatabaseSettings:ConnectionString", mongodb)
    .WithEnvironment($"ApplicationOptions:StartupEnvironment", "Aspire");

var orderprocessor = builder.AddProject<Projects.OrderProcessor>("orderprocessor")
    .WithReference(mongodb)
    .WithReference(messaging)
    // Use Aspire's mongodb connection string in OrderAPI's appsettings
    .WithEnvironment($"DatabaseSettings:ConnectionString", mongodb)
    .WithEnvironment($"ApplicationOptions:StartupEnvironment", "Aspire");

var payment = builder.AddProject<Projects.PaymentService>("paymentservice")
    .WithReference(mongodb)
    .WithReference(messaging)
    // Use Aspire's mongodb connection string in PaymentService's appsettings
    .WithEnvironment($"DatabaseSettings:ConnectionString", mongodb)
    .WithEnvironment($"ApplicationOptions:StartupEnvironment", "Aspire");

var shipment = builder.AddProject<Projects.Shipment>("shipment")
    .WithReference(mongodb)
    .WithReference(messaging)
    // Use Aspire's mongodb connection string in Shipment's appsettings
    .WithEnvironment($"DatabaseSettings:ConnectionString", mongodb)
    .WithEnvironment($"ApplicationOptions:StartupEnvironment", "Aspire");

var warehouse = builder.AddProject<Projects.Warehouse>("warehouse")
    .WithReference(mongodb)
    .WithReference(messaging)
    // Use Aspire's mongodb connection string in Warehouse's appsettings
    .WithEnvironment($"DatabaseSettings:ConnectionString", mongodb)
    .WithEnvironment($"ApplicationOptions:StartupEnvironment", "Aspire");

builder.AddProject<Projects.WebUI>("webui")
    .WithExternalHttpEndpoints()
    .WithReference(messaging)
    .WithReference(customers)
    .WithReference(catalog)
    .WithReference(shoppingCart)
    .WithReference(webuiaggregator)
    .WithReference(order)
    // Configure WebUI to use real CustomerAPI
    .WithEnvironment($"FeatureManagement:Customer", "true")
    // Configure WebUI to use real CatalogAPI
    .WithEnvironment($"FeatureManagement:Product", "true")
    // Configure WebUI to use real ShoppingCartAPI
    .WithEnvironment($"FeatureManagement:ShoppingCart", "true")
    .WithEnvironment($"ApplicationOptions:StartupEnvironment", "Aspire");

builder.Build().Run();
