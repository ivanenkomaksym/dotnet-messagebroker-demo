using WebUI.Models;

namespace WebUI.Services
{
    public interface IOrderService
    {
        Task<bool> CreateOrder(OrderModel order);
    }
}
