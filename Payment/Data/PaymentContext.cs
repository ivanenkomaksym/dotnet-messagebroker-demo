using Common.Models.Payment;
using MongoDB.Driver;

namespace PaymentService.Data
{
    internal class PaymentContext : IPaymentContext
    {
        public PaymentContext(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("DatabaseSettings:DatabaseName"));

            Payments = database.GetCollection<Payment>(configuration.GetValue<string>("DatabaseSettings:CollectionName"));
        }

        public IMongoCollection<Payment> Payments { get; }
    }
}
