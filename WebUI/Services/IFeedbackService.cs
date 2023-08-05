using Common.Models.Review;

namespace WebUI.Services
{
    public interface IFeedbackService
    {
        Task<IEnumerable<Review>> GetReviews(Guid productId);
        Task<Review> PostReview(Guid productId, Review review);
    }
}
