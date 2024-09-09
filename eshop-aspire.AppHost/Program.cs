using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

var featureFlagsOptions = builder.Configuration.GetSection("FeatureManagement");
var useCustomer = featureFlagsOptions.GetValue<bool>("Customer");
var useProduct = featureFlagsOptions.GetValue<bool>("Product");
var useShoppingCart = featureFlagsOptions.GetValue<bool>("ShoppingCart");

var mongo = builder.AddMongoDB("mongo").PublishAsConnectionString();
var mongodb = mongo.AddDatabase("mongodb");

var messaging = builder.AddRabbitMQ("AMQPConnectionString");

if (useCustomer)
{
    var customers = builder.AddProject<Projects.CustomerAPI>("customerapi")
        .WithReference(mongodb)
        .WithReference(messaging)
        // Use Aspire's mongodb connection string in CustomerAPI's appsettings
        .WithEnvironment($"DatabaseSettings:ConnectionString", mongodb)
        .WithEnvironment($"ApplicationOptions:StartupEnvironment", "Aspire");
}

if (useProduct)
{
    var catalog = builder.AddProject<Projects.CatalogAPI>("catalogapi")
        .WithReference(mongodb)
        // Use Aspire's mongodb connection string in CatalogAPI's appsettings
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
}

if (useShoppingCart)
{
    var shoppingCart = builder.AddProject<Projects.ShoppingCartAPI>("shoppingcartapi")
        .WithReference(mongodb)
        // Use Aspire's mongodb connection string in ShoppingCartAPI's appsettings
        .WithEnvironment($"DatabaseSettings:ConnectionString", mongodb);
}

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
    .WithReference(ordergrpc)
    // Use Aspire's mongodb connection string in OrderAPI's appsettings
    .WithEnvironment($"DatabaseSettings:ConnectionString", mongodb)
    .WithEnvironment($"GrpcSettings:OrderGrpcUrl", "http://ordergrpc/")
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
    .WithReference(order)
    // Configure WebUI to use real CustomerAPI
    .WithEnvironment($"FeatureManagement:Customer", useCustomer.ToString())
    // Configure WebUI to use real CatalogAPI
    .WithEnvironment($"FeatureManagement:Product", useProduct.ToString())
    // Configure WebUI to use real ShoppingCartAPI
    .WithEnvironment($"FeatureManagement:ShoppingCart", useShoppingCart.ToString())
    .WithEnvironment($"ApplicationOptions:StartupEnvironment", "Aspire");

builder.Build().Run();
