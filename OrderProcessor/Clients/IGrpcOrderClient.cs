using Common.Models;
using Common.Models.Payment;
using Common.Models.Shipment;

namespace OrderProcessor.Clients
{
    public interface IGrpcOrderClient
    {
        public Task<Order> GetOrder(Guid orderId);

        public Task<bool> UpdateOrder(Guid orderId,
                                      OrderStatus? orderStatus = null,
                                      PaymentStatus? paymentStatus = null,
                                      DeliveryStatus? deliveryStatus = null);
    }
}