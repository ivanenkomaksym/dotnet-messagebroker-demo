using Common.Models.Review;
using MongoDB.Driver;

namespace FeedbackAPI.Data
{
    public interface IFeedbackContext
    {
        IMongoCollection<ReviewedProduct> ReviewedProducts { get; }
    }
}
