using Common.Models.Review;
using FeedbackAPI.Repositories;

namespace FeedbackAPI.Data
{
    public class Query
    {
        public Task<IEnumerable<Review>> GetReviewsByProductId(Guid productId, [Service] IFeedbackRepository feedbackRepository) =>
            feedbackRepository.GetReviews(productId);
    }
}