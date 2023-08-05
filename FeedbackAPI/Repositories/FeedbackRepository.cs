using Common.Models.Review;
using FeedbackAPI.Data;
using MongoDB.Driver;

namespace FeedbackAPI.Repositories
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly IFeedbackContext _context;

        public FeedbackRepository(IFeedbackContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Review>> GetReviews(Guid productId)
        {
            FilterDefinition<ReviewedProduct> filter = Builders<ReviewedProduct>.Filter.Eq(p => p.ProductId, productId);

            var foundReviewedProduct = await _context
                            .ReviewedProducts
                            .Find(filter)
                            .FirstOrDefaultAsync();

            if (foundReviewedProduct == null)
                return new List<Review> { };

            return foundReviewedProduct.Reviews;
        }

        public async Task<Review> PostReview(Guid productId, Review review)
        {
            FilterDefinition<ReviewedProduct> filter = Builders<ReviewedProduct>.Filter.Eq(p => p.ProductId, productId);

            var foundReviewedProduct = await _context
                            .ReviewedProducts
                            .Find(filter)
                            .FirstOrDefaultAsync();

            if (foundReviewedProduct != null)
            {
                foundReviewedProduct.Reviews.Add(review);

                await _context.ReviewedProducts.ReplaceOneAsync(filter: p => p.ProductId == foundReviewedProduct.ProductId, replacement: foundReviewedProduct);
            }
            else
            {
                var newReviewedProduct = new ReviewedProduct
                {
                    ProductId = productId,
                    Reviews = { review }
                };
                await _context.ReviewedProducts.InsertOneAsync(newReviewedProduct);
            }

            return review;
        }
    }
}
