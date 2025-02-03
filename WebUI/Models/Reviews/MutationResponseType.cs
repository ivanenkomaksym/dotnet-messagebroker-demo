using Common.Models.Review;

namespace WebUI.Models.Reviews
{
    public class MutationResponseType
    {
        /// <summary>
        /// Name of this property must match with the one returned from GraphQL endpoint
        /// </summary>
        public required Review Review { get; set; }
    }
}