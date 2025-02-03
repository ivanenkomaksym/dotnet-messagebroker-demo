using Common.Configuration;
using Common.Models.Shipment;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Shipment.Data
{
    internal class ShipmentContext : IShipmentContext
    {
        public ShipmentContext(IOptions<DatabaseSettings> databaseSettings)
        {
            var client = new MongoClient(databaseSettings.Value.ConnectionString);
            var database = client.GetDatabase(databaseSettings.Value.DatabaseName);

            Deliveries = database.GetCollection<Delivery>(databaseSettings.Value.CollectionName);
        }

        public IMongoCollection<Delivery> Deliveries { get; }
    }
}