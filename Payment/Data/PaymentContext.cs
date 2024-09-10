using Common.Configuration;
using Common.Models.Payment;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace PaymentService.Data
{
    internal class PaymentContext : IPaymentContext
    {
        public PaymentContext(IOptions<DatabaseSettings> databaseSettings)
        {
            var client = new MongoClient(databaseSettings.Value.ConnectionString);
            var database = client.GetDatabase(databaseSettings.Value.DatabaseName);

            Payments = database.GetCollection<Payment>(databaseSettings.Value.CollectionName);
        }

        public IMongoCollection<Payment> Payments { get; }
    }
}
