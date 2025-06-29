using Catalog.API.Repositories;
using CatalogAPI.Data;
using Common.Configuration;
using Microsoft.Extensions.Options;
using Testcontainers.MongoDb;

namespace CatalogAPI.Tests;

public class CatalogFixture : IAsyncLifetime
{
    public MongoDbContainer MongoDbContainer { get; }
    public ProductRepository ProductRepository { get; private set; }

    public CatalogFixture()
    {
        MongoDbContainer = new MongoDbBuilder()
            .WithImage("mongo:latest")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await MongoDbContainer.StartAsync();

        var settings = Options.Create(new DatabaseSettings
        {
            ConnectionString = MongoDbContainer.GetConnectionString(),
            DatabaseName = "CatalogDb",
            CollectionName = "Products"
        });

        var context = new CatalogContext(settings);
        await CatalogContextSeed.SeedDataAsync(context.Products);

        ProductRepository = new ProductRepository(context);
    }

    public async Task DisposeAsync()
    {
        await MongoDbContainer.DisposeAsync();
    }
}
