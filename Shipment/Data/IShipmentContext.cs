using Common.Models.Shipment;
using MongoDB.Driver;

namespace Shipment.Data
{
    public interface IShipmentContext
    {
        IMongoCollection<Delivery> Deliveries { get; }
    }
}