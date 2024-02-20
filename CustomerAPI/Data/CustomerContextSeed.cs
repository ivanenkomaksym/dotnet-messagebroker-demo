using Common.Models;
using Common.SeedData;
using MongoDB.Driver;

namespace CustomerAPI.Data
{
    public class CustomerContextSeed
    {
        public static void SeedData(IMongoCollection<Customer> customerCollection)
        {
            bool existProduct = customerCollection.Find(p => true).Any();
            if (!existProduct)
            {
                customerCollection.InsertManyAsync(CustomerSeed.GetPreconfiguredCustomer());
            }
        }
    }
}
