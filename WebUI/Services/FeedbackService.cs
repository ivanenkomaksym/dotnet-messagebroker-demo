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

        public async Task<Review> PostReview(Guid productId, Review review)
        {
            var query = new GraphQLRequest
            {
                Query = @"
                mutation reviewForProductId($productId: UUID!, $review: ReviewInput!){
                  postReviewForProductId(productId: $productId, review: $review){
                    id,
                    description,
                    rating,
                    creationDateTime
                  }
                }",
                Variables = new { productId = productId, review = review }
            };
            var response = await _client.SendMutationAsync<MutationResponseType>(query);
            return response.Data.Review;
        }
    }
}
