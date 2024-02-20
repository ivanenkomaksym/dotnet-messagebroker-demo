using Common.Models;
using Common.SeedData;
using MongoDB.Driver;

namespace CatalogAPI.Data
{
    public class CatalogContextSeed
    {
        public static void SeedData(IMongoCollection<Product> productCollection)
        {
            bool existProduct = productCollection.Find(p => true).Any();
            if (!existProduct)
            {
                productCollection.InsertManyAsync(CatalogSeed.GetPreconfiguredProducts());
            }
        }
    }
}
