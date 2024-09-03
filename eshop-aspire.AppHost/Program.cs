var builder = DistributedApplication.CreateBuilder(args);

var mongo = builder.AddMongoDB("mongo").PublishAsConnectionString();
var mongodb = mongo.AddDatabase("mongodb");

var messaging = builder.AddRabbitMQ("messaging");

var customers = builder.AddProject<Projects.CustomerAPI>("customerapi")
    .WithReference(mongodb)
    // Use Aspire's mongodb connection string in CustomerAPI's appsettings
    .WithEnvironment($"DatabaseSettings:ConnectionString", mongodb)
    .WithEnvironment($"ApplicationOptions:StartupEnvironment", "Aspire");

var catalog = builder.AddProject<Projects.CatalogAPI>("catalogapi")
    // Use Aspire's mongodb connection string in CatalogAPI's appsettings
    .WithEnvironment($"DatabaseSettings:ConnectionString", mongodb);

var shoppingCart = builder.AddProject<Projects.ShoppingCartAPI>("shoppingcartapi")
    // Use Aspire's mongodb connection string in ShoppingCartAPI's appsettings
    .WithEnvironment($"DatabaseSettings:ConnectionString", mongodb);

var warehouseapi = builder.AddProject<Projects.WarehouseAPI>("warehouseapi")
    // Use Aspire's mongodb connection string in WarehouseAPI's appsettings
    .WithEnvironment($"DatabaseSettings:ConnectionString", mongodb);

// Configure WebUIAggregator to startup in Aspire configuration, since it uses two services: CatalogAPI and WarehouseAPI to aggregate response
var webuiaggregator = builder.AddProject<Projects.WebUIAggregatorAPI>("webuiaggregatorapi")
    .WithReference(catalog)
    .WithReference(warehouseapi)
    .WithEnvironment($"ApplicationOptions:StartupEnvironment", "Aspire");

var order = builder.AddProject<Projects.OrderAPI>("orderapi")
    .WithReference(messaging)
    // Use Aspire's mongodb connection string in OrderAPI's appsettings
    .WithEnvironment($"DatabaseSettings:ConnectionString", mongodb)
    .WithEnvironment($"ApplicationOptions:StartupEnvironment", "Aspire");

var payment = builder.AddProject<Projects.PaymentService>("paymentservice")
    // Use Aspire's mongodb connection string in PaymentService's appsettings
    .WithEnvironment($"DatabaseSettings:ConnectionString", mongodb);

var shipment = builder.AddProject<Projects.Shipment>("shipment")
    // Use Aspire's mongodb connection string in Shipment's appsettings
    .WithEnvironment($"DatabaseSettings:ConnectionString", mongodb);

var warehouse = builder.AddProject<Projects.Warehouse>("warehouse")
    // Use Aspire's mongodb connection string in Warehouse's appsettings
    .WithEnvironment($"DatabaseSettings:ConnectionString", mongodb);

builder.AddProject<Projects.WebUI>("webui")
    .WithExternalHttpEndpoints()
    .WithReference(messaging)
    .WithReference(customers)
    .WithReference(catalog)
    .WithReference(shoppingCart)
    .WithReference(webuiaggregator)
    .WithReference(order)
    .WithReference(payment)
    .WithReference(shipment)
    .WithReference(warehouse)
    // Configure WebUI to use real CustomerAPI
    .WithEnvironment($"FeatureManagement:Customer", "true")
    // Configure WebUI to use real CatalogAPI
    .WithEnvironment($"FeatureManagement:Product", "true")
    // Configure WebUI to use real ShoppingCartAPI
    .WithEnvironment($"FeatureManagement:ShoppingCart", "true")
    .WithEnvironment($"ApplicationOptions:StartupEnvironment", "Aspire");

builder.Build().Run();
