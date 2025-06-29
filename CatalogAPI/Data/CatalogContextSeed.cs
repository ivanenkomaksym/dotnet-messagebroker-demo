using Common.Models;
using Common.SeedData;
using MongoDB.Driver;

namespace CatalogAPI.Data
{
    public class CatalogContextSeed
    {
        public static async Task SeedDataAsync(IMongoCollection<Product> productCollection)
        {
            bool existProduct = productCollection.Find(p => true).Any();
            if (!existProduct)
            {
                await productCollection.InsertManyAsync(CatalogSeed.GetPreconfiguredProducts());
            }
        }
    }
}