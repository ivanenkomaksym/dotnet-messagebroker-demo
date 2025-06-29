using CatalogAPI.Data;
using Common.Configuration;
using Microsoft.Extensions.Options;
using Testcontainers.MongoDb;

namespace CatalogAPI.Tests;

public class CatalogAPITest : IAsyncLifetime
{
    private readonly MongoDbContainer _mongoDbContainer = new MongoDbBuilder()
        .WithImage("mongo:latest")
        .Build();

    public Task InitializeAsync()
    {
        return _mongoDbContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _mongoDbContainer.DisposeAsync().AsTask();
    }

    [Fact]
    public void ShouldReturnProducts()
    {
        // Arrange
        var settings = Options.Create(new DatabaseSettings
        {
            ConnectionString = _mongoDbContainer.GetConnectionString(),
            DatabaseName = "CatalogDb",
            CollectionName = "Products"
        });
        var catalogContext = new CatalogContext(settings);

        // Act
        var products = catalogContext.Products;

        // Assert
        Assert.NotNull(products);
    }
}
