using Common.Models.Review;

namespace WebUI.Models.Reviews
{
    public class ReviewCollectionType
    {
        /// <summary>
        /// Name of this property must match with the one returned from GraphQL endpoint
        /// </summary>
        public required IEnumerable<Review> ReviewsByProductId { get; set; }
    }
}
