using CatalogAPI.Services;
using Common.Models;
using Common.SeedData;
using MongoDB.Driver;

namespace CatalogAPI.Data
{
    public class CatalogContextSeed
    {
        public static async Task SeedDataAsync(ICatalogAI catalogAI, IMongoCollection<Product> productCollection)
        {
            bool existProduct = productCollection.Find(p => true).Any();
            if (!existProduct)
            {
                var products = CatalogSeed.GetPreconfiguredProducts().ToArray();

                if (catalogAI.IsEnabled)
                {
                    var embeddings = await catalogAI.GetEmbeddingsAsync(products);
                    for (int i = 0; i < products.Count(); i++)
                    {
                        products[i].Embedding = embeddings[i];
                    }
                }

                await productCollection.InsertManyAsync(products);
            }
        }
    }
}