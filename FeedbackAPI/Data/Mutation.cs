using Common.Models.Review;
using FeedbackAPI.Repositories;

namespace FeedbackAPI.Data
{
    public class Mutation
    {
        public Task<Review> PostReviewForProductId(Guid productId, Review review, [Service] IFeedbackRepository feedbackRepository) =>
            feedbackRepository.PostReview(productId, review);
    }
}