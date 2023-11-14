using Common.Models.Review;

namespace WebUI.Services
{
    public class EmptyFeedbackService : IFeedbackService
    {
        public Task<IEnumerable<Review>> GetReviews(Guid productId) => Task.FromResult<IEnumerable<Review>>(new List<Review>());

        public Task<Review> PostReview(Guid productId, Review review) => Task.FromResult(new Review());
    }
}
