using Common.Models.Review;
using MongoDB.Driver;

namespace FeedbackAPI.Data
{
    public class FeedbackContextSeed
    {
        public static void SeedData(IMongoCollection<ReviewedProduct> reviewedProductsCollection)
        {
            bool existReviews = reviewedProductsCollection.Find(p => true).Any();
            if (!existReviews)
            {
                reviewedProductsCollection.InsertManyAsync(GetPreconfiguredReviews());
            }
        }

        private static IEnumerable<ReviewedProduct> GetPreconfiguredReviews()
        {
            return new List<ReviewedProduct>()
            {
                new ReviewedProduct
                {
                    ProductId = Guid.NewGuid(),
                    Reviews = new List<Review>
                    {
                        new Review
                        {
                            Id = Guid.NewGuid(),
                            CustomerInfo = new Common.Models.CustomerInfo
                            {
                                CustomerId = Guid.NewGuid(),
                                FirstName = "Alice",
                                LastName = "Liddel",
                                Email = "alice@gmail.com"
                            },
                            Description = "Very good product.",
                            Rating = 5,
                            Anonymous = false,
                            ReviewDetails = new ReviewDetails
                            {
                                Id = Guid.NewGuid(),
                                Accuracy = 5,
                                Communication = 5,
                                DeliverySpeed = 5
                            },
                            CreationDateTime = DateTime.Now,
                        },
                        new Review
                        {
                            Id = Guid.NewGuid(),
                            CustomerInfo = new Common.Models.CustomerInfo
                            {
                                CustomerId = Guid.NewGuid(),
                                FirstName = "Bob",
                                LastName = "Liddel",
                                Email = "bob@gmail.com"
                            },
                            Description = "Slow delivery.",
                            Rating = 3,
                            Anonymous = false,
                            ReviewDetails = new ReviewDetails
                            {
                                Id = Guid.NewGuid(),
                                Accuracy = 5,
                                Communication = 5,
                                DeliverySpeed = 2
                            },
                            CreationDateTime = DateTime.Now,
                        }
                    }
                }
            };
        }
    }
}