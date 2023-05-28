using Common.Models.Shipment;
using MongoDB.Driver;

namespace Shipment.Data
{
    internal class ShipmentContext : IShipmentContext
    {
        public ShipmentContext(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("DatabaseSettings:DatabaseName"));

            Deliveries = database.GetCollection<Delivery>(configuration.GetValue<string>("DatabaseSettings:CollectionName"));
        }

        public IMongoCollection<Delivery> Deliveries { get; }
    }
}
