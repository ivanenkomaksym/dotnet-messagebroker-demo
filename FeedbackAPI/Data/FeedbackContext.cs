using Common.Configuration;
using Common.Models.Review;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace FeedbackAPI.Data
{
    public class FeedbackContext : IFeedbackContext
    {
        public FeedbackContext(IOptions<DatabaseSettings> databaseSettings)
        {
            var client = new MongoClient(databaseSettings.Value.ConnectionString);
            var database = client.GetDatabase(databaseSettings.Value.DatabaseName);

            ReviewedProducts = database.GetCollection<ReviewedProduct>(databaseSettings.Value.CollectionName);
            FeedbackContextSeed.SeedData(ReviewedProducts);
        }

        public IMongoCollection<ReviewedProduct> ReviewedProducts { get; }
    }
}
