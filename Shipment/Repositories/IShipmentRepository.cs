using Common.Models.Shipment;

namespace Shipment.Repositories
{
    public interface IShipmentRepository
    {
        Task<IEnumerable<Delivery>> GetDeliveries();
        Task<Delivery> GetDeliveryByOrderId(Guid orderId);
        Task<Delivery> GetDeliveryByCustomerId(Guid customerId);

        Task<Delivery> CreateDelivery(Delivery delivery);
        Task<bool> UpdateDelivery(Delivery delivery);
    }
}
