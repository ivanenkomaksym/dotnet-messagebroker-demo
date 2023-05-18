using Common.Models;

namespace WebUI.Services
{
    public interface IOrderService
    {
        Task<bool> CreateOrder(Order order);
    }
}
