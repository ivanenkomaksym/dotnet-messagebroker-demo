using Common.Models.Review;
using MongoDB.Driver;

namespace FeedbackAPI.Data
{
    public class FeedbackContext : IFeedbackContext
    {
        public FeedbackContext(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("DatabaseSettings:DatabaseName"));

            ReviewedProducts = database.GetCollection<ReviewedProduct>(configuration.GetValue<string>("DatabaseSettings:CollectionName"));
            FeedbackContextSeed.SeedData(ReviewedProducts);
        }

        public IMongoCollection<ReviewedProduct> ReviewedProducts { get; }
    }
}
