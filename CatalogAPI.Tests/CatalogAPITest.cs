namespace CatalogAPI.Tests; using Xunit;

[CollectionDefinition("MongoDb collection")]
public class MongoDbCollection : ICollectionFixture<CatalogFixture> { }


[Collection("MongoDb collection")]
public class CatalogAPITest
{
    private readonly CatalogFixture _fixture;

    public CatalogAPITest(CatalogFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ShouldReturnProducts()
    {
        var products = await _fixture.ProductRepository.GetProducts();
        Assert.NotNull(products);
        Assert.NotEmpty(products);
    }

    [Fact]
    public async Task ShouldReturnProductById()
    {
        var products = await _fixture.ProductRepository.GetProducts();
        var productId = products.First().Id;
        var product = await _fixture.ProductRepository.GetProduct(productId);
        Assert.NotNull(product);
        Assert.Equal(productId, product.Id);
    }
}