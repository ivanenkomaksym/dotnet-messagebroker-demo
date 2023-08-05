using GraphQL;
using GraphQL.Client.Abstractions;
using Common.Models.Review;
using WebUI.Models.Reviews;

namespace WebUI.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IGraphQLClient _client;
        public FeedbackService(IGraphQLClient client)
        {
            _client = client;
        }

        public async Task<IEnumerable<Review>> GetReviews(Guid productId)
        {
            var query = new GraphQLRequest
            {
                Query = @"
                query reviewsByProductId($productId: UUID!) {
                  reviewsByProductId(productId: $productId) {
                   id,
                    customerInfo{
                      customerId,
                      firstName,
                      lastName,
                      email
                    },
                    description
                    rating,
                    anonymous,
                    reviewDetails {
                      id,
                      accuracy,
                      communication,
                      deliverySpeed
                    },
                    creationDateTime
                  }
                }",
                Variables = new { productId = productId }
            };
            var response = await _client.SendQueryAsync<ReviewCollectionType>(query);
            return response.Data.ReviewsByProductId;
        }

        public Task<Review> PostReview(Guid productId, Review review)
        {
            throw new NotImplementedException();
        }
    }
}
