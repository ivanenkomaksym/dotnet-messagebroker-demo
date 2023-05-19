using Common.Models;
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
                customerCollection.InsertManyAsync(GetPreconfiguredCustomer());
            }
        }

        private static IEnumerable<Customer> GetPreconfiguredCustomer()
        {
            return new List<Customer>()
            {
                new Customer
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Alice",
                    LastName = "Liddell",
                    Email = "alice@gmail.com",
                    Password = "alice"
                },
                new Customer
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Bob",
                    LastName = "Liddell",
                    Email = "bob@gmail.com",
                    Password = "bob"
                }
            };
        }
    }
}
