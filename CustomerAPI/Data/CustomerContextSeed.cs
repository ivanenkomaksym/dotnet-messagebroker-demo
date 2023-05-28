using Common.Models;
using Common.Models.Payment;
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
                    Password = "alice",
                    PaymentInfo = new PaymentInfo
                    {
                        CardName = "Alice Liddell",
                        CardNumber = "1234 1234 1234 1234",
                        CVV = "123",
                        Expiration = "01/30",
                        PaymentMethod = PaymentMethod.CreditCard_AlwaysExpire
                    },
                    ShippingAddress = new Address
                    {
                        FirstName = "Alice",
                        LastName = "Liddell",
                        Email = "alice@gmail.com",
                        Country = "England",
                        AddressLine = "London",
                        ZipCode = "12345"
                    }
                },
                new Customer
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Bob",
                    LastName = "Liddell",
                    Email = "bob@gmail.com",
                    Password = "bob",
                    PaymentInfo = new PaymentInfo
                    {
                        CardName = "Bob Liddell",
                        CardNumber = "9876 9876 9876 9876",
                        CVV = "987",
                        Expiration = "12/29",
                        PaymentMethod = PaymentMethod.PayPal_AlwaysFail
                    },
                    ShippingAddress = new Address
                    {
                        FirstName = "Bob",
                        LastName = "Liddell",
                        Email = "alice@gmail.com",
                        Country = "England",
                        AddressLine = "London",
                        ZipCode = "12345"
                    }
                }
            };
        }
    }
}
