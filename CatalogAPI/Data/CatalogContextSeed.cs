using CatalogAPI.Services;
using Common.Models;
using Common.SeedData;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CatalogAPI.Data
{
    public class CatalogContextSeed
    {
        public static async Task SeedDataAsync(ICatalogAI catalogAI,
                                               IMongoCollection<Product> productCollection,
                                               bool seedIndexes = false)
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

            if (!seedIndexes)
                return;

            var cursor = await productCollection.SearchIndexes.ListAsync("vector_index");
            if (await cursor.MoveNextAsync() && cursor.Current.Any())
            {
                return; // Index already exists, no need to create it again
            }

            // Equivalent to the MongoDB shell command:
            //db.Products.createSearchIndex({
            //  "name": "vector_index",
            //  "definition": {
            //    "mappings": {
            //      "dynamic": false,
            //      "fields": {
            //        "Embedding": {
            //          "type": "knnVector",
            //          "dimensions": 384,
            //          "similarity": "cosine" // Or "euclidean", "dotProduct"
            //        }
            //      }
            //    }
            //  }
            //});
            var indexDefinitionBson = new BsonDocument
            {
                 { "mappings", new BsonDocument
                     {
                         { "dynamic", false },
                         { "fields", new BsonDocument
                                {
                                    { "Embedding", new BsonDocument
                                        {
                                            { "type", "knnVector" },
                                            { "dimensions", 384 },
                                            { "similarity", "cosine" }
                                        }
                                    }
                                }
                         }
                     }
                 }
            };

            var indexModel = new CreateSearchIndexModel(
                "vector_index",
                SearchIndexType.Search,
                indexDefinitionBson
            );

            await productCollection.SearchIndexes.CreateOneAsync(indexModel);
        }
    }
}