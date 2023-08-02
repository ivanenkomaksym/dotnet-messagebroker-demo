using Common.Models.Review;

namespace FeedbackAPI.Data
{
    public class ReviewType : ObjectType<Review>
    {
        protected override void Configure(IObjectTypeDescriptor<Review> descriptor)
        {
            descriptor.Field(_ => _.Id);
            descriptor.Field(_ => _.CustomerInfo);
            descriptor.Field(_ => _.Description);
            descriptor.Field(_ => _.Rating);
            descriptor.Field(_ => _.Anonymous);
            descriptor.Field(_ => _.ReviewDetails);
            descriptor.Field(_ => _.CreationDateTime);
        }
    }
}
