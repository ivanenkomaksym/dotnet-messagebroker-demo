using Common.Models.Review;

namespace FeedbackAPI.Repositories
{
    public interface IFeedbackRepository
    {
        Task<IEnumerable<Review>> GetReviews(Guid productId);
        Task<Review> PostReview(Guid productId, Review review);
    }
}